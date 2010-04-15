// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Resources;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class ValidatorTest {
        #region NoAttributes
        [TestMethod]
        [Description("Validating an object with no attributes succeeds")]
        public void Validator_Object_No_Attributes() {
            ValTestNoAttributesClass instance = new ValTestNoAttributesClass();
            ValidationContext context = new ValidationContext(instance, null, null);
            bool isValid = Validator.TryValidateObject(instance, context, null);
            Assert.IsTrue(isValid);

            List<ValidationResult> output = new List<ValidationResult>();
            instance = new ValTestNoAttributesClass();
            context = new ValidationContext(instance, context, context.Items);
            isValid = Validator.TryValidateObject(instance, context, output);
            Assert.IsTrue(isValid);
            Assert.AreEqual(0, output.Count);
        }

        [TestMethod]
        [Description("Validating a property with no attributes succeeds")]
        public void Validator_Property_No_Attributes() {
            ValidationContext context = new ValidationContext(new ValTestNoAttributesClass(), null, null);
            context.MemberName = "PropertyWithoutAttributes";
            bool isValid = Validator.TryValidateProperty(null, context, null);
            Assert.IsTrue(isValid);

            List<ValidationResult> output = new List<ValidationResult>();
            isValid = Validator.TryValidateProperty(null, context, output);
            Assert.IsTrue(isValid);
            Assert.AreEqual(0, output.Count);
        }

        #endregion NoAttributes

        #region Null Parameters

        [TestMethod]
        [Description("A null object throws ArgumentNullException")]
        public void Validator_Fail_Null_Context() {
            ExceptionHelper.ExpectArgumentNullExceptionStandard(delegate() {
                Validator.TryValidateObject(new ValTestClass(), null, null);
            }, "validationContext");
        }

        [TestMethod]
        [Description("A null object throws ArgumentNullException")]
        public void Validator_Fail_Null_Object() {
            ValidationContext context = new ValidationContext(new ValTestNoAttributesClass(), null, null);

            ExceptionHelper.ExpectArgumentNullExceptionStandard(delegate() {
                Validator.TryValidateObject(null, context, null);
            }, "instance");
        }

        #endregion Null Parameters

        #region Simple Valid Patterns
        [TestMethod]
        [Description("Validating an object succeeds")]
        public void Validator_Object_Valid() {
            ValTestClass vtc = new ValTestClass();
            vtc.RequiredProperty = "xxx";
            vtc.StringLengthProperty = "xxx";
            vtc.NullableDoubleProperty = 2.0;

            ValidationContext context = new ValidationContext(vtc, null, null);
            bool isValid = Validator.TryValidateObject(vtc, context, null);
            Assert.IsTrue(isValid);

            List<ValidationResult> output = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(vtc, context, output);
            Assert.IsTrue(isValid);
            Assert.AreEqual(0, output.Count);

            // do not set the required DoubleProperty, verify object is not valid when validateAllProperties is true
            isValid = Validator.TryValidateObject(vtc, context, null, true /*validateAllProperties*/);
            Assert.IsFalse(isValid);

            // now set the DoubleProperty, verify object is now valid
            vtc.DoubleProperty = 3.0;
            isValid = Validator.TryValidateObject(vtc, context, null, true /*validateAllProperties*/);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        [Description("Validating a property succeeds")]
        public void Validator_Property_Valid() {
            ValTestClass vtc = new ValTestClass();
            ValidationContext context = new ValidationContext(vtc, null, null);
            context.MemberName = "RequiredProperty";

            bool isValid = Validator.TryValidateProperty("xxx", context, null);
            Assert.IsTrue(isValid);

            context = new ValidationContext(vtc, null, null);
            context.MemberName = "StringLengthProperty";
            isValid = Validator.TryValidateProperty("yyy", context, null);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        [Description("Validating that property-level validation receives type-level ValidationContext items.")]
        public void Validator_IsValidationContextValid() {
            ValTestClass_PropertyLevel_Validation obj = new ValTestClass_PropertyLevel_Validation() { ObjectProperty = new object() };
            ValidationContext context = new ValidationContext(obj, null, null);

            context.Items.Add(typeof(int), "int expected");
            context.Items.Add(typeof(string), "string expected");

            Assert.IsTrue(Validator.TryValidateObject(obj, context, null, true));
        }

        #endregion Simple Valid Patterns

        #region Simple Invalid Patterns
        [TestMethod]
        [Description("Validating an invalid object returns false")]
        public void Validator_Object_Invalid() {
            ValTestClass vtc = new ValTestClass();
            ValidationContext context = new ValidationContext(vtc, null, null);

            //vtc.RequiredProperty = null causes validation error for [Required]
            //vtc.StringLengthProperty = null causes validation error for object;
            vtc.NullableDoubleProperty = null;  // this is [Required] and should fail

            bool isValid = Validator.TryValidateObject(vtc, context, null);
            Assert.IsFalse(isValid);

            List<ValidationResult> output = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(vtc, context, output);
            Assert.IsFalse(isValid);
            Assert.AreEqual(2, output.Count);
            UnitTestHelper.AssertListContains(output, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RequiredAttribute_ValidationError, "RequiredProperty"));
            UnitTestHelper.AssertListContains(output, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RequiredAttribute_ValidationError, "NullableDoubleProperty"));
        }

        [TestMethod]
        [Description("Validating an invalid property returns false")]
        public void Validator_Property_Invalid() {
            ValTestClass vtc = new ValTestClass();
            ValidationContext context = new ValidationContext(vtc, null, null);
            context.MemberName = "RequiredProperty";

            bool isValid = Validator.TryValidateProperty(null, context, null);
            Assert.IsFalse(isValid);

            List<ValidationResult> output = new List<ValidationResult>();
            isValid = Validator.TryValidateProperty(null, context, output);
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, output.Count);
            UnitTestHelper.AssertListContains(output, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RequiredAttribute_ValidationError, "RequiredProperty"));
        }

        [TestMethod]
        [Description("Validating an invalid property returns false and inherits DisplayName")]
        public void Validator_Property_Invalid_Inherits_DisplayName() {
            ValTestClass vtc = new ValTestClass();
            ValidationContext context = new ValidationContext(vtc, null, null);
            context.MemberName = "StringLengthProperty";

            List<ValidationResult> output = new List<ValidationResult>();
            bool isValid = Validator.TryValidateProperty("LongerThan10CharactersFails", context, output);
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, output.Count);
            UnitTestHelper.AssertListContains(output, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.StringLengthAttribute_ValidationError, "StringPropertyDisplayName", 10));
        }

        [TestMethod]
        [Description("Validating an invalid property returns false and multiple reasons")]
        public void Validator_Property_Invalid_Multiple_Reasons() {
            ValTestClass vtc = new ValTestClass();
            ValidationContext context = new ValidationContext(vtc, null, null);
            context.MemberName = "NullableDoubleProperty";

            bool isValid = Validator.TryValidateProperty(null, context, null);
            Assert.IsFalse(isValid);

            List<ValidationResult> output = new List<ValidationResult>();
            isValid = Validator.TryValidateProperty(null, context, output);
            Assert.IsFalse(isValid);

            // The [Required] is the only expected failure -- the Range will signal success for null
            Assert.AreEqual(1, output.Count);
            UnitTestHelper.AssertListContains(output, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RequiredAttribute_ValidationError, "NullableDoubleProperty"));
        }

        [TestMethod]
        [Description("Validating that error messages use MemberName instead of DisplayName when the DisplayName value is null or an empty string.")]
        public void Validator_DisplayNameEmpty_UseMemberName() {
            ValTestClass vtc = new ValTestClass();
            vtc.RequiredProperty = vtc.StringLengthProperty = "required";
            vtc.NullableDoubleProperty = vtc.DoubleProperty = 1.0d;
            vtc.StringPropertyWithEmptyDisplayName = "invalid string length";
            List<ValidationResult> results = new List<ValidationResult>();

            ValidationContext context = new ValidationContext(vtc, null, null);
            Assert.IsFalse(Validator.TryValidateObject(vtc, context, results, true));
            Assert.AreEqual<int>(1, results.Count);
            Assert.AreEqual<string>(String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.StringLengthAttribute_ValidationError, "StringPropertyWithEmptyDisplayName", 2), results[0].ErrorMessage);
        }

        #endregion Simple Invalid Patterns

        #region Simple Invalid Patterns Throw
        [TestMethod]
        [Description("Validating an invalid object throws")]
        public void Validator_Object_Invalid_Throws() {
            ValTestClass vtc = new ValTestClass();
            ValidationContext context = new ValidationContext(vtc, null, null);
            //vtc.RequiredProperty = null causes validation error for [Required]
            //vtc.StringLengthProperty = null causes validation error for object;
            vtc.NullableDoubleProperty = null;

            ExceptionHelper.ExpectValidationException(delegate() {
                Validator.ValidateObject(vtc, context);
            }, String.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.RequiredAttribute_ValidationError, "RequiredProperty"), typeof(RequiredAttribute), null);
        }

        [TestMethod]
        [Description("Validating an invalid property throws")]
        public void Validator_Property_Invalid_Throws() {
            ValTestClass vtc = new ValTestClass();
            ValidationContext context = new ValidationContext(vtc, null, null);
            context.MemberName = "RequiredProperty";

            ExceptionHelper.ExpectValidationException(delegate() {
                Validator.ValidateProperty(null, context);
            }, String.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.RequiredAttribute_ValidationError, "RequiredProperty"), typeof(RequiredAttribute), null);
        }

        [TestMethod]
        [Description("Validating an null value against a value type throws ArgumentException")]
        public void Validator_Fail_Property_Null_ValueType_Throws() {
            ValTestClass vtc = new ValTestClass();
            ValidationContext context = new ValidationContext(vtc, null, null);
            context.MemberName = "DoubleProperty";

            ExceptionHelper.ExpectArgumentException(delegate() {
                Validator.ValidateProperty(null, context);   // DoubleProperty is a double and should fail
            }, String.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.Validator_Property_Value_Wrong_Type, "DoubleProperty", typeof(double).FullName));
        }

        [TestMethod]
        [Description("Validating an invalid object with only entity-level failures yields an empty MemberNames property")]
        public void Validator_Object_Invalid_Yields_Empty_MemberNames() {
            ValTestClass_TypeLevel_Invalid fail = new ValTestClass_TypeLevel_Invalid();
            ValidationContext context = new ValidationContext(fail, null, null);
            ICollection<ValidationResult> results = new Collection<ValidationResult>();
            bool isValid = Validator.TryValidateObject(fail, context, results);

            Assert.IsFalse(isValid, "TryValidateObject should return false");
            Assert.AreEqual(1, results.Count, "There should be 1 ValidationResult");
            Assert.AreEqual(0, results.First().MemberNames.Count(), "MemberNames should be empty");
        }

        #endregion Simple Invalid Patterns Throw

        #region Type Mismatch

        [TestMethod]
        [Description("Validating a property with the wrong type throws ArgumentException")]
        public void Validator_Fail_Property_Type_Mismatch() {
            ValTestClass vtc = new ValTestClass();
            ValidationContext context = new ValidationContext(vtc, null, null);
            context.MemberName = "RequiredProperty";

            // IsValid entry point
            ExceptionHelper.ExpectArgumentException(delegate() {
                Validator.TryValidateProperty(2.0, context, null); // 2.0 s/b string
            }, String.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.Validator_Property_Value_Wrong_Type, "RequiredProperty", typeof(string).FullName));

            // Validate entry point
            ExceptionHelper.ExpectArgumentException(delegate() {
                Validator.ValidateProperty(2.0, context); // 2.0 s/b string
            }, String.Format(CultureInfo.CurrentCulture, DataAnnotationsResources.Validator_Property_Value_Wrong_Type, "RequiredProperty", typeof(string).FullName));
        }

        #endregion Type Mismatch

        #region Validation attribute discovery behavior

        /// <summary>
        /// Verifies that validation attributes (other than [Required]) on a type are not invoked on a null property of the same type.
        /// </summary>
        [TestMethod]
        [Description("Verifies that validation (other than [Required]) is not invoked on a null property.")]
        public void Validator_TypeLevelValidator_DoesNotCascadeToNullProperty() {
            var testObj = new ValTestClass_TypeLevel_CustomValidation() { CanBeNullProperty = null };
            ValidationContext context = new ValidationContext(testObj, null, null);
            Assert.IsTrue(Validator.TryValidateObject(testObj, context, null, true));
        }

        /// <summary>
        /// Verifies that validation attributes on a type are invoked on a non-null property of the same type.
        /// </summary>
        [TestMethod]
        [Description("Verifies that validation (other than [Required]) is invoked on a non-null property.")]
        public void Validator_TypeLevelValidator_DoesNotCascadeToProperty() {
            var testObj = new ValTestClass_TypeLevel_CustomValidation() { CanBeNullProperty = new ValTestClass_TypeLevel_CustomValidation() };
            ValidationContext context = new ValidationContext(testObj, null, null);
            Assert.IsTrue(Validator.TryValidateObject(testObj, context, null, true));
        }

        /// <summary>
        /// Verifies that validation attributes (of the same type and value) declared at the type and property level are correctly preserved.
        /// </summary>
        [TestMethod]
        [Description("Verifies that validation attributes (of the same type and value) declared at the type and property level are correctly preserved.")]
        public void Validator_TypeAndPropertyLevel_DoesNotIntersect() {
            var errors = new Collection<ValidationResult>();
            var testObj = new ValTestClass_TypeAndPropertyLevel_CustomValidation();
            ValidationContext context = new ValidationContext(testObj, null, null);
            Assert.IsFalse(Validator.TryValidateObject(testObj, context, errors, true));

            Assert.AreEqual<int>(1, errors.Count);
            Assert.AreEqual<string>("Invalid!  Value cannot be null.", errors[0].ErrorMessage);
        }

        [TestMethod]
        [Description("Verifies that the Validator skips over indexed properties.")]
        public void Validator_IndexedProperties_Are_Ignored() {
            var errors = new Collection<ValidationResult>();
            var testObj = new ValTestClass_With_IndexedProperties();
            testObj["test"] = 1;
            ValidationContext context = new ValidationContext(testObj, null, null);
            Assert.IsTrue(Validator.TryValidateObject(testObj, context, errors));
            Assert.AreEqual<int>(0, errors.Count);
        }

        #endregion Validation attribute discovery behavior

        #region Validation Order

        /// <summary>
        /// Verifies that field-level validation is performed before type-level validation.
        /// </summary>
        [TestMethod]
        [Description("Verifies that field-level validation failures preclude type-level validation.")]
        public void Validator_Property_Errors_Preclude_Type_Validation() {
            var errors = new Collection<ValidationResult>();
            var testObj = new ValTestClass();

            // Validate the test class without [Required] properties set, this should fail.
            ValidationContext context = new ValidationContext(testObj, null, null);
            Assert.IsFalse(Validator.TryValidateObject(testObj, context, errors));

            Assert.AreEqual<int>(2, errors.Count);
            Assert.AreEqual<string>(string.Format(DataAnnotationsResources.RequiredAttribute_ValidationError, "RequiredProperty"), errors[0].ErrorMessage);
            Assert.AreEqual<string>(string.Format(DataAnnotationsResources.RequiredAttribute_ValidationError, "NullableDoubleProperty"), errors[1].ErrorMessage);
        }

        #endregion Validation Order

        #region Validation Context Usage

        [TestMethod]
        [Description("Instance provided must match ValidationContext instance for Object validation")]
        public void Validator_Instance_Must_Match_ValidationContext_Instance_For_Object_Validation() {
            ValidationContext context = new ValidationContext(this, null, null);
            ValTestClass instance = new ValTestClass();

            // Without specifying validate all properties
            ExceptionHelper.ExpectArgumentException(() => Validator.TryValidateObject(instance, context, null), DataAnnotationsResources.Validator_InstanceMustMatchValidationContextInstance);
            ExceptionHelper.ExpectArgumentException(() => Validator.ValidateObject(instance, context), DataAnnotationsResources.Validator_InstanceMustMatchValidationContextInstance);

            // With specifying validate all properties
            ExceptionHelper.ExpectArgumentException(() => Validator.TryValidateObject(instance, context, null, true), DataAnnotationsResources.Validator_InstanceMustMatchValidationContextInstance);
            ExceptionHelper.ExpectArgumentException(() => Validator.ValidateObject(instance, context, true), DataAnnotationsResources.Validator_InstanceMustMatchValidationContextInstance);
        }

        #endregion Validation Context Usage

#if !SILVERLIGHT
        #region IValidatableObject

        [TestMethod]
        [Description("Validator.TryValidateObject executes IValidatableObject.Validate to get errors")]
        public void Validator_Returns_Errors_From_IValidatableObject() {
            ICollection<ValidationResult> injectedResults = new Collection<ValidationResult>
            {
                new ValidationResult("Error")
            };

            ValTestClass_With_IValidatableObject entity = new ValTestClass_With_IValidatableObject(injectedResults);
            ValidationContext context = new ValidationContext(entity, null, null);

            ICollection<ValidationResult> results = new Collection<ValidationResult>();
            bool isValid = Validator.TryValidateObject(entity, context, results);

            Assert.IsFalse(isValid, "isValid should be false since the object returns validation results");
            Assert.AreEqual(1, results.Count, "There should be 1 result returned");
        }

        [TestMethod]
        [Description("Validator.TryValidateObject filters out Success results from IValidatorObject results")]
        public void Validator_Filters_Out_Success_Results_From_IValidatableObject() {
            ICollection<ValidationResult> injectedResults = new Collection<ValidationResult>
            {
                new ValidationResult("Error"),
                ValidationResult.Success
            };

            ValTestClass_With_IValidatableObject entity = new ValTestClass_With_IValidatableObject(injectedResults);
            ValidationContext context = new ValidationContext(entity, null, null);

            ICollection<ValidationResult> results = new Collection<ValidationResult>();
            bool isValid = Validator.TryValidateObject(entity, context, results);

            Assert.IsFalse(isValid, "isValid should be false since the object returns validation results");
            Assert.AreEqual(1, results.Count, "There should be 1 result returned since the ValidationResult.Success item should be filtered out");
        }

        [TestMethod]
        [Description("Validator.ValidateObject throws an exception containing the first error result")]
        public void Validator_Fails_With_IValidatableObject_Errors() {
            ICollection<ValidationResult> injectedResults = new Collection<ValidationResult>
            {
                new ValidationResult("Error"),
                new ValidationResult("Another Error")
            };

            ValTestClass_With_IValidatableObject entity = new ValTestClass_With_IValidatableObject(injectedResults);
            ValidationContext context = new ValidationContext(entity, null, null);

            ExceptionHelper.ExpectValidationException(() => { Validator.ValidateObject(entity, context); }, "Error", null, entity);
        }

        [TestMethod]
        [Description("Validator.TryValidateObject doesn't invoke IValidatableObject when an entity attribute failure occurs")]
        public void Validator_Does_Not_Invoke_IValidatableObject_With_Entity_Attribute_Failure() {
            ValTestClass_With_IValidatableObject entity = new ValTestClass_With_IValidatableObject("IValidatableObject should not be called when an entity attribute validation fails");

            Dictionary<object, object> items = new Dictionary<object, object>();
            items.Add(ValTestClass_With_IValidatableObject.AttributeFailure, true);
            items.Add(ValTestClass_With_IValidatableObject.AttributeFailureMessage, "Attribute failure");

            ValidationContext context = new ValidationContext(entity, null, items);

            ICollection<ValidationResult> results = new Collection<ValidationResult>();
            bool isValid = Validator.TryValidateObject(entity, context, results);

            Assert.IsFalse(isValid, "isValid should be false since the object returns validation results");
            Assert.AreEqual(1, results.Count, "There should be 1 result returned");
            Assert.AreEqual("Attribute failure", results.First().ErrorMessage);
        }

        #endregion
#endif
    }

    #region Test classes

    public class ValTestNoAttributesClass {
        public string PropertyWithoutAttributes { get; set; }
        public void MethodWithoutAttributes(string param1, double param2) { }
    }

    internal class ValTestInternalClass {

    }

    [CustomValidation(typeof(ValTestValidator), "IsValTestValid")]
    public class ValTestClass {
        internal bool _failMethod = false;

        [Required]
        public string RequiredProperty { get; set; }

        [StringLength(10)]
        [Display(Name = "StringPropertyDisplayName")]
        public string StringLengthProperty { get; set; }

        [Range(1.0, 5.0)]
        public double DoubleProperty { get; set; }

        // Deliberately omit error message formatter to force it to choose default
        [CustomValidation(typeof(ValTestValidator), "IsValTestMethodValid")]
        [Display(Name = "MethodDisplayName")]
        public void MethodWithParameters(
            [Required] [StringLength(5)] [Display(Name = "FirstParameterDisplayName")] string param1,
            [Required] [Range(1.0, 10.0)] double param2) { }

        [CustomValidation(typeof(ValTestValidator), "IsValTestMethodValid")]
        public void MethodWithOptionalParameter(
            [Required] [StringLength(5)] string param1,
            [Range(1.0, 10.0)] double param2) { }

        public void MethodWithRequiredNullableParameter(
            [Required] double? doubleParam
        ) { }

        public void MethodWithOptionalNullableParameter(double? doubleParam) { }

        [CustomValidation(typeof(ValTestValidator), "IsValTestMethodValid")]
        public void MethodWithNoParameters() { }

        [Required]
        [Range(1.0, 5.0)]
        public double? NullableDoubleProperty { get; set; }

        [StringLength(2)]
        [Display(Name = "")]
        public string StringPropertyWithEmptyDisplayName { get; set; }

        public string PropertyThatShouldNotBeTouched {
            get {
                throw new InvalidOperationException("This property should not be evaluated");
            }
            set {
                throw new InvalidOperationException("This property should not be set");
            }
        }
    }

    [CustomValidation(typeof(ValTestValidator), "IsValTest_InvalidIfNull")]
    public class ValTestClass_TypeLevel_CustomValidation {
        public ValTestClass_TypeLevel_CustomValidation CanBeNullProperty { get; set; }
    }

    [CustomValidation(typeof(ValTestValidator), "IsValTest_InvalidIfNull")]
    public class ValTestClass_TypeAndPropertyLevel_CustomValidation {
        [CustomValidation(typeof(ValTestValidator), "IsValTest_InvalidIfNull")]
        public ValTestClass_TypeAndPropertyLevel_CustomValidation AlwaysNullProperty { get; private set; }
    }

    [Invalid]
    public class ValTestClass_TypeLevel_Invalid {
    }

    public class ValTestClass_PropertyLevel_Validation {
        [CustomValidation(typeof(ValTestValidator), "IsValTest_IsValidationContextValid")]
        public object ObjectProperty { get; set; }
    }

    public class InvalidAttribute : ValidationAttribute {
        public override string FormatErrorMessage(string name) {
            return null;
        }

#if !SILVERLIGHT
        public
#else
        internal
#endif
        override bool IsValid(object value) {
            return false;
        }
    }

    public static class ValTestValidator {
        // Cross-field validation -- 2 properties must be non-null and equal
        public static ValidationResult IsValTestValid(object vtcObject, ValidationContext context) {
            ValTestClass vtc = vtcObject as ValTestClass;
            bool result = vtc != null &&
                            vtc.RequiredProperty != null &&
                            vtc.StringLengthProperty != null &&
                            vtc.StringLengthProperty.Equals(vtc.RequiredProperty, StringComparison.Ordinal);

            if (!result)
                return new ValidationResult("!" + context.DisplayName);
            return null;
        }

        // Method level validator
        public static ValidationResult IsValTestMethodValid(object vtcObject, ValidationContext context) {
            ValTestClass vtc = vtcObject as ValTestClass;
            bool result = vtc != null && vtc._failMethod == false;
            if (!result)
                return new ValidationResult("-" + context.DisplayName);
            return null;
        }

        // Validation method always fails if value is null.
        public static ValidationResult IsValTest_InvalidIfNull(object value, ValidationContext context) {
            bool valid = (value != null);
            if (!valid) {
                return new ValidationResult("Invalid!  Value cannot be null.");
            }
            return null;
        }

        // Validation method verifies expected ValidationContext is received during property-level validation.
        public static ValidationResult IsValTest_IsValidationContextValid(object value, ValidationContext context) {
            ValTestClass_PropertyLevel_Validation originalObjectReference = context.ObjectInstance as ValTestClass_PropertyLevel_Validation;
            if ((string)context.Items[typeof(int)] != "int expected")
                return new ValidationResult(null);
            if ((string)context.Items[typeof(string)] != "string expected")
                return new ValidationResult(null);

            return null;
        }
    }

    public class ValTestClass_With_IndexedProperties {
        private IDictionary<string, int> keyValues = new Dictionary<string, int>();

        public int this[string key] {
            get {
                return keyValues[key];
            }
            set {
                keyValues[key] = value;
            }
        }
    }

#if !SILVERLIGHT
    [CustomValidation(typeof(ValTestClass_With_IValidatableObject), "CustomValidationMethod")]
    public class ValTestClass_With_IValidatableObject : IValidatableObject {
        private ICollection<ValidationResult> _injectedResults;
        private string _failureMessage;

        public ValTestClass_With_IValidatableObject(ICollection<ValidationResult> injectedResults) {
            this._injectedResults = injectedResults;
        }

        public ValTestClass_With_IValidatableObject(string failureMessageWhenInvoked) {
            this._failureMessage = failureMessageWhenInvoked;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this._failureMessage)) {
                Assert.Fail(this._failureMessage);
            }

            return this._injectedResults;
        }

        public const string AttributeFailure = "FailFromAttribute";
        public const string AttributeFailureMessage = "AttributeFailureMessage";

        public static ValidationResult CustomValidationMethod(ValTestClass_With_IValidatableObject instance, ValidationContext context) {
            bool fail = context.Items.ContainsKey(AttributeFailure) ? (bool)context.Items[AttributeFailure] : false;
            string message = context.Items.ContainsKey(AttributeFailureMessage) ? (string)context.Items[AttributeFailureMessage] : "CustomValidationMethod Failed";

            return fail ? new ValidationResult(message) : ValidationResult.Success;
        }
    }
#endif

    #endregion Test classes
}
