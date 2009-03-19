using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Resources;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations {
    /// <summary>
    /// Base class for all validation attributes.
    /// </summary>
    /// <remarks>
    /// The properties <see cref="ErrorMessageResourceType"/> and <see cref="ErrorMessageResourceName"/> are used to provide
    /// a localized error message, but they cannot be set if <see cref="ErrorMessage"/> is also used to provide a non-localized
    /// error message.
    /// </remarks>
    public abstract class ValidationAttribute : Attribute {
        private string _errorMessage;
        private string _errorMessageResourceName;
        private Type _errorMessageResourceType;

        /// <summary>
        /// Default constructor for any validation attribute.
        /// </summary>
        /// <remarks>This constructor chooses a very generic validation error message.
        /// Developers subclassing ValidationAttribute should use other constructors
        /// or supply a better message.
        /// </remarks>
        protected ValidationAttribute()
            : this(() => DataAnnotationsResources.ValidationAttribute_ValidationError) {
        }

        /// <summary>
        /// Constructor that accepts a fixed validation error message.
        /// </summary>
        /// <param name="errorMessage">A non-localized error message to use in <see cref="ErrorMessageString"/></param>
        protected ValidationAttribute(string errorMessage)
            : this(() => errorMessage) {
        }

        /// <summary>
        /// Allows for providing a resource accessor function that will be used by the <see cref="ErrorMessageString"/> property to retrieve the error message.
        /// An example would be to have something like CustomAttribute() : base( () =&gt; MyResources.MyErrorMessage ) {}
        /// </summary>
        /// <param name="errorMessageAccessor"></param>
        protected ValidationAttribute(Func<string> errorMessageAccessor) {
            // If null, will later be exposed as lack of error message to be able to construct accessor
            this.ResourceAccessor = errorMessageAccessor;
        }

        private Func<string> ResourceAccessor { get; set; }

        /// <summary>
        /// Gets the localized error message string, coming either from <see cref="ErrorMessage"/>, or from evaluating the 
        /// <see cref="ErrorMessageResourceType"/> and <see cref="ErrorMessageResourceName"/> pair.
        /// </summary>
        protected string ErrorMessageString {
            get {
                this.SetupResourceAccessor();
                return this.ResourceAccessor();
            }
        }

        /// <summary>
        /// Gets or sets the resource type to use for error message lookups.
        /// </summary>
        /// <value>
        /// Use this property only in conjunction with <see cref="ErrorMessageResourceName"/>.  They are
        /// used together to retrieve localized error messages at runtime.
        /// <para>Use <see cref="ErrorMessage"/> instead of this pair if error messages are not localized.
        /// </para>
        /// </value>
        public Type ErrorMessageResourceType {
            get {
                return this._errorMessageResourceType;
            }
            set {
                this._errorMessageResourceType = value;
                this.ResourceAccessor = null;
            }
        }

        /// <summary>
        /// Gets or sets the resource name (property name) to use as the key for lookups on the resource type.
        /// </summary>
        /// <value>
        /// Use this property to set the name of the property within <see cref="ErrorMessageResourceType"/>
        /// that will provide a localized error message.  Use <see cref="ErrorMessage"/> for non-localized error messages.
        /// </value>
        public string ErrorMessageResourceName {
            get {
                return this._errorMessageResourceName;
            }
            set {
                this._errorMessageResourceName = value;
                this.ResourceAccessor = null;
            }
        }

        /// <summary>
        /// Validates the configuration of this attribute and sets up the appropriate error string accessor.
        /// This method bypasses all verification once the ResourceAccessor has been set.
        /// </summary>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        private void SetupResourceAccessor() {
            if (this.ResourceAccessor == null) {
                string localErrorMessage = this._errorMessage;
                bool resourceNameSet = !string.IsNullOrEmpty(this._errorMessageResourceName);
                bool errorMessageSet = !string.IsNullOrEmpty(localErrorMessage);
                bool resourceTypeSet = this._errorMessageResourceType != null;

                // Either ErrorMessageResourceName or ErrorMessage may be set, but not both.
                // The following test checks both being set as well as both being not set.
                if (resourceNameSet == errorMessageSet) {
                    throw new InvalidOperationException(DataAnnotationsResources.ValidationAttribute_Cannot_Set_ErrorMessage_And_Resource);
                }

                // Must set both or neither of ErrorMessageResourceType and ErrorMessageResourceName
                if (resourceTypeSet != resourceNameSet) {
                    throw new InvalidOperationException(DataAnnotationsResources.ValidationAttribute_NeedBothResourceTypeAndResourceName);
                }

                // If set resource type (and we know resource name too), then go setup the accessor
                if (resourceNameSet) {
                    this.SetResourceAccessorByPropertyLookup();
                } else {
                    // Here if not using resource type/name -- the accessor is just the error message string,
                    // which we know is not empty to have gotten this far.
                    this.ResourceAccessor = delegate() {
                        // We captured error message to local in case it changes before accessor runs
                        return localErrorMessage;
                    };
                }
            }
        }

        private void SetResourceAccessorByPropertyLookup() {
            if (this._errorMessageResourceType != null && !string.IsNullOrEmpty(this._errorMessageResourceName)) {
                var property = this._errorMessageResourceType.GetProperty(this._errorMessageResourceName, BindingFlags.Public | BindingFlags.Static);
                if (property == null) {
                    throw new InvalidOperationException(
                        String.Format(
                        CultureInfo.CurrentCulture,
                        DataAnnotationsResources.ValidationAttribute_ResourceTypeDoesNotHaveProperty,
                        this._errorMessageResourceType.FullName,
                        this._errorMessageResourceName));
                }
                if (property.PropertyType != typeof(string)) {
                    throw new InvalidOperationException(
                        String.Format(
                        CultureInfo.CurrentCulture,
                        DataAnnotationsResources.ValidationAttribute_ResourcePropertyNotStringType,
                        property.Name,
                        this._errorMessageResourceType.FullName));
                }

                this.ResourceAccessor = delegate() {
                    return (string)property.GetValue(null, null);
                };
            } else {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.ValidationAttribute_NeedBothResourceTypeAndResourceName));
            }
        }

        /// <summary>
        /// Gets or sets and explicit error message string.
        /// </summary>
        /// <value>
        /// This property is intended to be used for non-localizable error messages.
        /// Use <see cref="ErrorMessageResourceType"/> and <see cref="ErrorMessageResourceName"/> for localizable error messages.
        /// </value>
        public string ErrorMessage {
            get {
                return this._errorMessage;
            }
            set {
                this._errorMessage = value;
                this.ResourceAccessor = null;
            }
        }

        /// <summary>
        /// Formats the error message to present to the user.
        /// </summary>
        /// <remarks>The error message will be re-evaluated every time this function is called. 
        /// It applies the <paramref name="name"/> (for example, the name of a field) to the formated error message, resulting 
        /// in something like "The field 'name' has an incorrect value".
        /// <para>
        /// Derived classes can override this method to customize how errors are generated.
        /// </para>
        /// <para>
        /// The base class implementation will use <see cref="ErrorMessageString"/> to obtain a localized
        /// error message from properties within the current attribute.  If those have not been set, a generic
        /// error message will be provided.
        /// </para>
        /// </remarks>
        /// <param name="name">The user-visible name to include in the formatted message.</param>
        /// <returns>The localized string describing the validation error</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public virtual string FormatErrorMessage(string name) {
            return String.Format(CultureInfo.CurrentCulture, this.ErrorMessageString, name);
        }

        /// <summary>
        /// Gets the value indicating whether or not the specified <paramref name="value"/> is valid
        /// with respect to the current validation attribute.
        /// </summary>
        /// <remarks>
        /// Derived classes should implement their validation logic here.
        /// <para>
        /// The preferred public entry point for clients requesting validation is the <see cref="TryValidate"/> method.
        /// </para>
        /// </remarks>
        /// <param name="value">The value to validate</param>
        /// <returns><c>true</c> if the <paramref name="value"/> is acceptable, <c>false</c> if it is not acceptable</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public abstract bool IsValid(object value);

        /// <summary>
        /// Tests whether the given <paramref name="value"/> is valid with respect to the current
        /// validation attribute without throwing a <see cref="ValidationException"/>
        /// </summary>
        /// <remarks>
        /// If this method returns <c>false</c>, the output <paramref name="validationResult"/> will be guaranteed
        /// to be non-null.  Additionally, <see cref="ValidationResult.ErrorMessage"/> and <see cref="ValidationResult.MemberNames"/>
        /// will be guaranteed to be non-empty.
        /// </remarks>
        /// <param name="value">The value to validate</param>
        /// <param name="validationContext">Additional context in which this test is being made.  It cannot be null.</param>
        /// <param name="validationResult">The output information describing the problem when this method returns <c>false</c>.</param>
        /// <returns><c>true</c> if the value is valid, otherwise <c>false</c></returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="validationContext"/> is null.</exception>
        public bool TryValidate(object value, ValidationContext validationContext, out ValidationResult validationResult) {
            if (validationContext == null) {
                throw new ArgumentNullException("validationContext");
            }

            validationResult = null;
            bool success = this.IsValid(value, validationContext, out validationResult);

            // If validation fails, we want to ensure we have a ValidationResult that guarantees
            // it has an ErrorMessage and non-empty MemberNames
            if (!success) {
                bool hasMemberNames = (validationResult != null) ? validationResult.MemberNames.Any() : false;
                bool hasErrorMessage = (validationResult != null) ? !string.IsNullOrEmpty(validationResult.ErrorMessage) : false;
                if (!hasMemberNames || !hasErrorMessage) {
                    string errorMessage = (hasErrorMessage) ? validationResult.ErrorMessage : this.FormatErrorMessage(validationContext.DisplayName);
                    IEnumerable<string> memberNames = (hasMemberNames) ? validationResult.MemberNames : new string[] { validationContext.MemberName };
                    validationResult = new ValidationResult(errorMessage, memberNames);
                }
            }
            return success;
        }

        /// <summary>
        /// Protected virtual method which can be overridden to perform contextual validation.
        /// </summary>
        /// <remarks>
        /// This implementation in the base class does not use the <paramref name="validationContext"/> but merely
        /// calls the current object's <see cref="IsValid(object)"/>.  If that method returns <c>false</c>,
        /// this method constructs a <paramref name="validationResult"/> using <see cref="ValidationContext.MemberName"/>
        /// and the current attributes <see cref="FormatErrorMessage"/>.  Subclasses that want this behavior should
        /// invoke this base class's method.  Subclasses that want to set <see cref="ValidationResult"/> explicitly should
        /// not invoke this base class's method.
        /// </remarks>
        /// <param name="value">The value to validate</param>
        /// <param name="validationContext">Additional context in which the test is being made</param>
        /// <param name="validationResult">Output information describing the problem if this method returns <c>false</c></param>
        /// <returns><c>true</c> if the value is valid, otherwise <c>false</c></returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "This follows the standard TryGet pattern")]
        protected virtual bool IsValid(object value, ValidationContext validationContext, out ValidationResult validationResult) {
            validationResult = null;
            bool success = this.IsValid(value);
            if (!success) {
                validationResult = new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName), new string[] { validationContext.MemberName });
            }
            return success;
        }

        /// <summary>
        /// Validates the specified <paramref name="value"/> and throws <see cref="ValidationException"/> if it is not.
        /// </summary>
        /// <remarks>This base method invokes the <see cref="IsValid(object)"/> method to determine whether or not the
        /// <paramref name="value"/> is acceptable.  If <see cref="IsValid(object)"/> returns <c>false</c>, this base
        /// method will invoke the <see cref="FormatErrorMessage"/> to obtain a localized message describing
        /// the problem, and it will throw a <see cref="ValidationException"/>
        /// </remarks>
        /// <param name="value">The value to validate</param>
        /// <param name="name">The string to be included in the validation error message if <paramref name="value"/> is not valid</param>
        /// <exception cref="ValidationException"> is thrown if <see cref="IsValid(object)"/> returns <c>false</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public void Validate(object value, string name) {
            if (!this.IsValid(value)) {
                throw new ValidationException(this.FormatErrorMessage(name), this, value);
            }
        }

        /// <summary>
        /// Validates the specified <paramref name="value"/> and throws <see cref="ValidationException"/> if it is not.
        /// </summary>
        /// <remarks>This base method invokes the <see cref="IsValid(object, ValidationContext, out ValidationResult)"/> method 
        /// to determine whether or not the <paramref name="value"/> is acceptable.  
        /// If that method returns <c>false</c>, this base method will throw a <see cref="ValidationException"/> containing
        /// the <see cref="ValidationResult"/> describing the problem.
        /// </remarks>
        /// <param name="value">The value to validate</param>
        /// <param name="validationContext">Additional context that may be used for validation.  It cannot be null.</param>
        /// <exception cref="ValidationException"> is thrown if <see cref="IsValid(object, ValidationContext, out ValidationResult)"/> 
        /// returns <c>false</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public void Validate(object value, ValidationContext validationContext) {
            if (validationContext == null)
                throw new ArgumentNullException("validationContext");

            ValidationResult validationResult;
            if (!this.TryValidate(value, validationContext, out validationResult)) {
                // Convenience -- if implementation did not fill in an error message,
                throw new ValidationException(validationResult, this, value);
            }
        }
    }
}
