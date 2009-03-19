using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Resources;
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
            Assert.AreEqual(3, output.Count);
            UnitTestHelper.AssertListContains(output, "!ValTestClass");    // custom formatter
            UnitTestHelper.AssertListContains(output, "The RequiredProperty field is required.");
            UnitTestHelper.AssertListContains(output, "The NullableDoubleProperty field is required.");
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
            UnitTestHelper.AssertListContains(output, "The RequiredProperty field is required.");
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
            UnitTestHelper.AssertListContains(output, "The field StringPropertyDisplayName must be a string with a maximum length of 10.");
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
            UnitTestHelper.AssertListContains(output, "The NullableDoubleProperty field is required.");
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
            Assert.AreEqual<string>("The field StringPropertyWithEmptyDisplayName must be a string with a maximum length of 2.", results[0].ErrorMessage);
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
            }, string.Format(DataAnnotationsResources.RequiredAttribute_ValidationError, "RequiredProperty"), typeof(RequiredAttribute), null);
        }

        [TestMethod]
        [Description("Validating an invalid property throws")]
        public void Validator_Property_Invalid_Throws() {
            ValTestClass vtc = new ValTestClass();
            ValidationContext context = new ValidationContext(vtc, null, null);
            context.MemberName = "RequiredProperty";

            ExceptionHelper.ExpectValidationException(delegate() {
                Validator.ValidateProperty(null, context);
            }, "The RequiredProperty field is required.", typeof(RequiredAttribute), null);
        }

        [TestMethod]
        [Description("Validating an null value against a value type throws ArgumentException")]
        public void Validator_Fail_Property_Null_ValueType_Throws() {
            ValTestClass vtc = new ValTestClass();
            ValidationContext context = new ValidationContext(vtc, null, null);
            context.MemberName = "DoubleProperty";

            ExceptionHelper.ExpectArgumentException(delegate() {
                Validator.ValidateProperty(null, context);   // DoubleProperty is a double and should fail
            }, "The value for property 'DoubleProperty' must be of type 'System.Double'.\r\nParameter name: value");
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
            }, "The value for property 'RequiredProperty' must be of type 'System.String'.\r\nParameter name: value");

            // Validate entry point
            ExceptionHelper.ExpectArgumentException(delegate() {
                Validator.ValidateProperty(2.0, context); // 2.0 s/b string
            }, "The value for property 'RequiredProperty' must be of type 'System.String'.\r\nParameter name: value");
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
        [Description("Verifies that field-level validation is performed before type-level validation.")]
        public void Validator_PropertiesValidatedBeforeType() {
            var errors = new Collection<ValidationResult>();
            var testObj = new ValTestClass();

            // Validate the test class without [Required] properties set, this should fail.
            ValidationContext context = new ValidationContext(testObj, null, null);
            Assert.IsFalse(Validator.TryValidateObject(testObj, context, errors));

            Assert.AreEqual<int>(3, errors.Count);
            Assert.AreEqual<string>(string.Format(DataAnnotationsResources.RequiredAttribute_ValidationError, "RequiredProperty"), errors[0].ErrorMessage);
            Assert.AreEqual<string>(string.Format(DataAnnotationsResources.RequiredAttribute_ValidationError, "NullableDoubleProperty"), errors[1].ErrorMessage);
            Assert.AreEqual<string>("!ValTestClass", errors[2].ErrorMessage);
        }

        #endregion Validation Order

        #region Validation Context Usage

        [TestMethod]
        [Description("Instance provided must match ValidationContext instance for Object validation")]
        public void Validator_Instance_Must_Match_ValidationContext_Instance_For_Object_Validation() {
            ValidationContext context = new ValidationContext(this, null, null);
            ValTestClass instance = new ValTestClass();

            // Without specifying validate all properties
            ExceptionHelper.ExpectArgumentException(() => Validator.TryValidateObject(instance, context, null), DataAnnotationsResources.Validator_InstanceMustMatchValidationContextInstance + "\r\nParameter name: instance");
            ExceptionHelper.ExpectArgumentException(() => Validator.ValidateObject(instance, context), DataAnnotationsResources.Validator_InstanceMustMatchValidationContextInstance + "\r\nParameter name: instance");

            // With specifying validate all properties
            ExceptionHelper.ExpectArgumentException(() => Validator.TryValidateObject(instance, context, null, true), DataAnnotationsResources.Validator_InstanceMustMatchValidationContextInstance + "\r\nParameter name: instance");
            ExceptionHelper.ExpectArgumentException(() => Validator.ValidateObject(instance, context, true), DataAnnotationsResources.Validator_InstanceMustMatchValidationContextInstance + "\r\nParameter name: instance");
        }

        #endregion Validation Context Usage
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

    public class ValTestClass_PropertyLevel_Validation {
        [CustomValidation(typeof(ValTestValidator), "IsValTest_IsValidationContextValid")]
        public object ObjectProperty { get; set; }
    }

    public static class ValTestValidator {
        // Cross-field validation -- 2 properties must be non-null and equal
        public static bool IsValTestValid(object vtcObject, ValidationContext context, out ValidationResult validationResult) {
            validationResult = null;
            ValTestClass vtc = vtcObject as ValTestClass;
            bool result = vtc != null &&
                            vtc.RequiredProperty != null &&
                            vtc.StringLengthProperty != null &&
                            vtc.StringLengthProperty.Equals(vtc.RequiredProperty, StringComparison.Ordinal);

            if (!result)
                validationResult = new ValidationResult("!" + context.DisplayName);
            return result;
        }

        // Method level validator
        public static bool IsValTestMethodValid(object vtcObject, ValidationContext context, out ValidationResult validationResult) {
            validationResult = null;
            ValTestClass vtc = vtcObject as ValTestClass;
            bool result = vtc != null && vtc._failMethod == false;
            if (!result)
                validationResult = new ValidationResult("-" + context.DisplayName);
            return result;
        }

        // Validation method always fails if value is null.
        public static bool IsValTest_InvalidIfNull(object value, ValidationContext context, out ValidationResult validationResult) {
            validationResult = null;
            bool valid = (value != null);
            if (!valid) {
                validationResult = new ValidationResult("Invalid!  Value cannot be null.");
            }
            return valid;
        }

        // Validation method verifies expected ValidationContext is received during property-level validation.
        public static bool IsValTest_IsValidationContextValid(object value, ValidationContext context, out ValidationResult validationResult) {
            ValTestClass_PropertyLevel_Validation originalObjectReference = context.ObjectInstance as ValTestClass_PropertyLevel_Validation;
            validationResult = null;
            if ((string)context.Items[typeof(int)] != "int expected")
                return false;
            if ((string)context.Items[typeof(string)] != "string expected")
                return false;

            return true;
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

    #endregion Test classes
}
