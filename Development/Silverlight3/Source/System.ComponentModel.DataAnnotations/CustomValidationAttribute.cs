using System.ComponentModel.DataAnnotations.Resources;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations {
    /// <summary>
    /// Validation attribute that executes a user-supplied method at runtime.
    /// </summary>
    /// <remarks>
    /// This validation attribute is used to invoke custom logic to perform validation at runtime.
    /// Like any other <see cref="ValidationAttribute"/>, its <see cref="IsValid(object)"/> method is invoked to
    /// perform validation.  This implementation simply redirects that call to the method identified
    /// by <see cref="Method"/> on a type identified by <see cref="ValidatorType"/>
    /// <para>
    /// The supplied <see cref="ValidatorType"/> cannot be null, and it must be a public type.
    /// </para>
    /// <para>
    /// The named <see cref="Method"/> must be public, static, return a bool and take at least one input
    /// parameter for the value to be validated.  This value parameter may be strongly typed.  Type conversion will
    /// be attempted if clients pass in a value of a different type.
    /// </para>
    /// <para>
    /// The <see cref="Method"/> may also declare 2 additional parameters; a <see cref="ValidationContext"/>
    /// and an (out) <see cref="ValidationResult"/>.   The input <see cref="ValidationContext"/> parameter provides
    /// additional context the method may use to determine the context in which it is being used.  The
    /// <see cref="ValidationResult"/> output parameter allows the method to return a custom error message in
    /// the case the value is invalid.
    /// </para>
    /// <para>
    /// If the method returns a <c>null</c> for <see cref="ValidationResult"/> or an empty <see cref="ValidationResult.ErrorMessage"/>
    /// then the normal <see cref="ValidationAttribute.FormatErrorMessage"/> method will be called to compose the error message.
    /// </para>
    /// <para>
    /// The bool value returned by <see cref="Method"/> indicates whether a given value is acceptable.  A <c>true</c> return means
    /// the given value is acceptable, and a <c>false</c> return indicates it is not.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true)]
    public sealed class CustomValidationAttribute : ValidationAttribute {
        private Type _validatorType;
        private string _method;
        private MethodInfo _methodInfo;
        private bool _isSingleArgumentMethod;
        private string _lastMessage;
        private Type _valuesType;
        private bool _validatedAttribute;
        private string _cachedErrorMessage;

        /// <summary>
        /// Instantiates a custom validation attribute that will invoke a method in the
        /// specified type.
        /// </summary>
        /// <remarks>An invalid <paramref name="ValidatorType"/> or <paramref name="Method"/> will be cause <see cref="ValidationAttribute.IsValid(object)"/>
        /// to return <c>false</c> and <see cref="ValidationAttribute.FormatErrorMessage"/> to return a summary error message
        /// </remarks>
        /// <param name="validatorType">The type that will contain the method to invoke.  It cannot be null.  See <see cref="Method"/></param>
        /// <param name="method">The name of the method to invoke in <paramref name="validatorType"/></param>
        public CustomValidationAttribute(Type validatorType, string method)
            : base(() => DataAnnotationsResources.CustomValidationAttribute_ValidationError) {
            this._validatorType = validatorType;
            this._method = method;
        }

        /// <summary>
        /// Gets the type that contains the validation method identified by <see cref="Method"/>
        /// </summary>
        public Type ValidatorType {
            get {
                return this._validatorType;
            }
        }

        /// <summary>
        /// Gets the name of the method in <see cref="ValidatorType"/> to invoke to perform validation.
        /// </summary>
        public string Method {
            get {
                return this._method;
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets a unique identifier for this attribute.
        /// </summary>
        public override object TypeId {
            get {
                return this;
            }
        }
#endif

        /// <summary>
        /// Override of <see cref="ValidationAttribute.IsValid(object)"/>
        /// </summary>
        /// <remarks>This method is forwarded to the method declared in <see cref="Method"/>.</remarks>
        /// <param name="value">The value to be tested</param>
        /// <returns>Whatever the <see cref="Method"/> in <see cref="ValidatorType"/> returns.</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public override bool IsValid(object value) {
            ValidationResult result = null;
            return IsValidInternal(value, null, out result);
        }
        /// <summary>
        /// Override of long form of validation method.  See <see cref="ValidationAttribute"/>.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context in which the validation is happening.</param>
        /// <param name="validationResult">The output parameter to receive the detailed results of the validation.</param>
        /// <returns>Whatever the <see cref="Method"/> in <see cref="ValidatorType"/> returns.</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        protected override bool IsValid(object value, ValidationContext validationContext, out ValidationResult validationResult) {
            return this.IsValidInternal(value, validationContext, out validationResult);
        }

        /// <summary>
        /// Private helper method to handle both forms of IsValid.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="validationContext">The context, which may be null in the short-form of IsValid.</param>
        /// <param name="validationResult">The output parameter for the results.</param>
        /// <returns>Whatever the <see cref="Method"/> in <see cref="ValidatorType"/> returns.</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        private bool IsValidInternal(object value, ValidationContext validationContext, out ValidationResult validationResult) {
            validationResult = null;

            // If attribute is not valid, throw an exeption right away to inform the developer
            this.ThrowIfAttributeNotValid();

            MethodInfo mInfo = this._methodInfo;

            // If user did not specify method and/or we couldn't locate one, return FALSE.
            // The error message formatting method is aware to include this in its message.
            if (mInfo == null) {
                return false;
            }

            // If the value is not of the correct type and cannot be converted, simply return 'false'
            // to indicate it is not acceptable.  The convention is that IsValid is merely a probe,
            // and clients are not expecting exceptions.
            object convertedValue;
            if (!this.TryConvertValue(value, out convertedValue)) {
                return false;
            }

            // Invoke the method.  Catch TargetInvocationException merely to unwrap it.
            // Callers don't know Reflection is being used and will not typically see
            // the real exception
            try {
                // It is an error for the caller to have used the short form of CustomValidationAttribute.IsValid when the
                // custom method takes 3 parameters.  We cannot synthesize a ValidationContext at this late point.
                if (!this._isSingleArgumentMethod && (validationContext == null)) {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Short_Form_Not_Supported, this._method, this._validatorType.Name));
                }

                // 3-parameter form is Method(object value, ValidationContext context, out ValidationResult result),
                // 1-parameter form is Method(object value)
                object[] methodParams = this._isSingleArgumentMethod
                                            ? new object[] { convertedValue }
                                            : new object[] { convertedValue, validationContext, validationResult };

                bool result = (bool)mInfo.Invoke(null, methodParams);

                this._lastMessage = null;

                // Out params are placed back in the input array -- pull it back out into our own out param
                if (!this._isSingleArgumentMethod)
                    validationResult = methodParams[2] as ValidationResult;

                // We capture the message they provide us only in the event of failure,
                // otherwise we use the normal message supplied via the ctor
                if (!result && validationResult != null) {
                    this._lastMessage = validationResult.ErrorMessage;
                }

                return result;
            } catch (TargetInvocationException tie) {
                if (tie.InnerException != null) {
                    throw tie.InnerException;
                }
                throw;
            }
        }

        /// <summary>
        /// Override of <see cref="ValidationAttribute.FormatErrorMessage"/>
        /// </summary>
        /// <param name="name">The name to include in the formatted string</param>
        /// <returns>A localized string to describe the problem.</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public override string FormatErrorMessage(string name) {
            // If attribute is not valid, throw an exeption right away to inform the developer
            this.ThrowIfAttributeNotValid();

            if (!string.IsNullOrEmpty(this._lastMessage)) {
                return String.Format(CultureInfo.CurrentCulture, this._lastMessage, name);
            }

            // If success or they supplied no custom message, use normal base class behavior
            return base.FormatErrorMessage(name);
        }

        /// <summary>
        /// Determines whether the current attribute instance itself is valid for use.
        /// </summary>
        /// <param name="errorMessage">The returned error message if it is not valid, null if it is valid</param>
        /// <returns><c>true</c> if it is valid</returns>
        private bool IsAttributeValid(out string errorMessage) {
            // We cache this test and message since it will be called on every call to IsValid()
            if (!this._validatedAttribute) {
                this._validatedAttribute = true;
                this._cachedErrorMessage = this.ValidateValidatorTypeParameter();
                if (this._cachedErrorMessage == null) {
                    this._cachedErrorMessage = this.ValidateMethodParameter();
                }
            }
            errorMessage = this._cachedErrorMessage;
            return (errorMessage == null);
        }

        /// <summary>
        /// Internal helper to determine whether <see cref="ValidatorType"/> is legal for use.
        /// </summary>
        /// <returns>Null or the appropriate error message</returns>
        private string ValidateValidatorTypeParameter() {
            if (this._validatorType == null) {
                return DataAnnotationsResources.CustomValidationAttribute_ValidatorType_Required;
            }
            if (!this._validatorType.IsVisible) {
                return String.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Type_Must_Be_Public, this._validatorType.Name);
            }
            return null;
        }

        /// <summary>
        /// Internal helper to determine whether <see cref="Method"/> is legal for use.
        /// </summary>
        /// <returns>Null or the appropriate error message</returns>
        private string ValidateMethodParameter() {
            // MethodName cannot be empty
            if (String.IsNullOrEmpty(this._method)) {
                return DataAnnotationsResources.CustomValidationAttribute_Method_Required;
            }

            // Named method must be public and static
            MethodInfo mInfo = this._validatorType.GetMethod(this._method, BindingFlags.Public | BindingFlags.Static);
            if (mInfo == null) {
                return string.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Method_Not_Found, this._method, this._validatorType.Name);
            }

            // Method must return a bool
            if (mInfo.ReturnType != typeof(bool)) {
                return string.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Method_Must_Return_Bool, this._method, this._validatorType.Name);
            }

            ParameterInfo[] pInfos = mInfo.GetParameters();

            // Must declare at least one input parameter for the value
            if (pInfos.Length == 0 || pInfos[0].ParameterType.IsByRef) {
                return string.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Must_Accept_At_Least_One_Param, this._method, this._validatorType.Name);
            }

            // We accept 2 forms:
            //  1) public static bool IsValid(object value)
            //  2) public static bool IsValid(object value, ValidationContext context, out ValidationResult result)
            this._isSingleArgumentMethod = (pInfos.Length == 1);

            if (!this._isSingleArgumentMethod) {
                if ((pInfos.Length != 3) ||
                    (pInfos[1].ParameterType != typeof(ValidationContext)) ||
                    ((pInfos[2].ParameterType != typeof(ValidationResult).MakeByRefType()) || !pInfos[2].ParameterType.IsByRef)) {
                    return string.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.CustomValidationAttribute_Wrong_Parameters, this._method, this._validatorType.Name);
                }
            }

            this._methodInfo = mInfo;
            this._valuesType = pInfos[0].ParameterType;
            return null;
        }

        /// <summary>
        /// Throws InvalidOperationException if the attribute is not valid
        /// </summary>
        private void ThrowIfAttributeNotValid() {
            string errorMessage;
            if (!this.IsAttributeValid(out errorMessage)) {
                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Attempts to convert the given value to the type needed to invoke the method for the current
        /// CustomValidationAttribute
        /// </summary>
        /// <param name="value">The value to check/convert</param>
        /// <param name="convertedValue">If successful, the converted (or copied) value</param>
        /// <returns><c>true</c> If type value was already correct or was successfully converted</returns>
        private bool TryConvertValue(object value, out object convertedValue) {
            convertedValue = null;
            Type t = this._valuesType;

            // Null is permitted for reference types or for Nullable<>'s only
            if (value == null) {
                if (t.IsValueType && (!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(Nullable<>))) {
                    return false;
                }
                return true;    // convertedValue already null, which is correct for this case
            }

            // If the type is already legally assignable, we're good
            if (t.IsAssignableFrom(value.GetType())) {
                convertedValue = value;
                return true;
            }

            // Value is not the right type -- attempt a convert.
            // Any expected exception returns a false
            try {
                convertedValue = Convert.ChangeType(value, t, CultureInfo.CurrentCulture);
                return true;
            } catch (FormatException) {
                return false;
            } catch (InvalidCastException) {
                return false;
            } catch (NotSupportedException) {
                return false;
            }
        }
    }
}
