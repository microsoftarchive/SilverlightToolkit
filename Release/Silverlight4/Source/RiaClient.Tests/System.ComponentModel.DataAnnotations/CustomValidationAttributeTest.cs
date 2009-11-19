// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class CustomValidationAttributeTest {
        #region Ctor

        [TestMethod]
        [Description("Normal ctor with valid type and method")]
        public void CustomValidation_Ctor_3_Params() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValid");
            Assert.AreEqual(typeof(MockValidator), cva.ValidatorType);
            Assert.AreEqual("IsValid", cva.Method);
        }

        [TestMethod]
        [Description("Normal ctor with valid type and short form of method")]
        public void CustomValidation_Ctor_1_Param() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNoMessage");
            Assert.AreEqual(typeof(MockValidator), cva.ValidatorType);
            Assert.AreEqual("IsValidNoMessage", cva.Method);
        }

        [TestMethod]
        [Description("Normal ctor with null type returns error message")]
        public void CustomValidation_Fail_Ctor_Null_Type() {
            CustomValidationAttribute cva = new CustomValidationAttribute(null, "IsValid");
            AssertFailure(cva, "The CustomValidationAttribute.ValidatorType was not specified.");
        }

        [TestMethod]
        [Description("Normal ctor with null method returns error message")]
        public void CustomValidation_Fail_Ctor_Null_Method() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), null);
            AssertFailure(cva, "The CustomValidationAttribute.Method was not specified.");
        }

        [TestMethod]
        [Description("Normal ctor with empty method returns error message")]
        public void CustomValidation_Fail_Ctor_Empty_Method() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), string.Empty);
            AssertFailure(cva, "The CustomValidationAttribute.Method was not specified.");
        }

        [TestMethod]
        [Description("Normal ctor with non-public type returns error message")]
        public void CustomValidation_Fail_Ctor_Non_Public() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(PrivateMockValidator), "IsValid");
            AssertFailure(cva, "The custom validation type 'PrivateMockValidator' must be public.");
        }

        #endregion Ctor

        #region Method

        [TestMethod]
        [Description("A validation method that throws receives that exception")]
        public void CustomValidation_Method_Throws() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "Throws");
            Assert.AreEqual("Throws", cva.Method);
            ValidationContext context = new ValidationContext(new object(), null, null);

            ExceptionHelper.ExpectArgumentException(delegate() {
                cva.GetValidationResult("Fred", context);    // valid call, but this method throws
            }, "o");
        }

        [TestMethod]
        [Description("Setting non-existing method returns error message")]
        public void CustomValidation_Fail_Method_Doesnt_Exist() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "DoesntExist");
            AssertFailure(cva, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.CustomValidationAttribute_Method_Not_Found, cva.Method, cva.ValidatorType.Name));
        }

        [TestMethod]
        [Description("Setting non-public method returns error message")]
        public void CustomValidation_Fail_Method_Not_Public() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "NotPublic");
            AssertFailure(cva, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.CustomValidationAttribute_Method_Not_Found, cva.Method, cva.ValidatorType.Name));
        }

        [TestMethod]
        [Description("Setting non-static method returns error message")]
        public void CustomValidation_Fail_Method_Not_Static() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "NotStatic");
            AssertFailure(cva, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.CustomValidationAttribute_Method_Not_Found, cva.Method, cva.ValidatorType.Name));
        }

        [TestMethod]
        [Description("Setting void returning method returns error message")]
        public void CustomValidation_Fail_Method_Void_Return() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "VoidReturn");
            AssertFailure(cva, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.CustomValidationAttribute_Method_Must_Return_ValidationResult, cva.Method, cva.ValidatorType.Name));
        }

        [TestMethod]
        [Description("Setting bool returning method (short signature) returns error message")]
        public void CustomValidation_Fail_Method_Bool_Return_Short() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "BoolReturnShort");
            AssertFailure(cva, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.CustomValidationAttribute_Method_Must_Return_ValidationResult, cva.Method, cva.ValidatorType.Name));
        }

        [TestMethod]
        [Description("Setting bool returning method (long signature) returns error message")]
        public void CustomValidation_Fail_Method_Bool_Return_Long() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "BoolReturnLong");
            AssertFailure(cva, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.CustomValidationAttribute_Method_Must_Return_ValidationResult, cva.Method, cva.ValidatorType.Name));
        }

        [TestMethod]
        [Description("Setting parameterless method returns error message")]
        public void CustomValidation_Fail_Method_No_Params() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "NoParams");
            AssertFailure(cva, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.CustomValidationAttribute_Method_Signature, cva.Method, cva.ValidatorType.Name));
        }

        [TestMethod]
        [Description("Setting method with 'out' first param returns error message")]
        public void CustomValidation_Fail_Method_Param1_Out() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "NotInObject");
            AssertFailure(cva, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.CustomValidationAttribute_Method_Signature, cva.Method, cva.ValidatorType.Name));
        }

        [TestMethod]
        [Description("Setting method with 'out' second param returns error message")]
        public void CustomValidation_Fail_Method_Param2_Out() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "NotInContext");
            AssertFailure(cva, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.CustomValidationAttribute_Method_Signature, cva.Method, cva.ValidatorType.Name));
        }

        [TestMethod]
        [Description("Setting method with too many parameters returns error message")]
        public void CustomValidation_Fail_Method_Too_Many_Params() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "TooManyParams");
            AssertFailure(cva, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.CustomValidationAttribute_Method_Signature, cva.Method, cva.ValidatorType.Name));
        }
        #endregion Method

        #region Reference Types

        [TestMethod]
        [Description("Setting valid value succeeds, 1 param form")]
        public void CustomValidation_Valid_Value_1_Param() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNoMessage");
            ValidationContext context = new ValidationContext(new object(), null, null);

            Assert.IsNull(cva.GetValidationResult("Sue", context));
        }

        [TestMethod]
        [Description("Setting valid value succeeds, 2 param form")]
        public void CustomValidation_Valid_Value_2_Param() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValid");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNull(cva.GetValidationResult("Fred", context));
        }

        [TestMethod]
        [Description("Setting invalid value returns false, 3 parameter method")]
        public void CustomValidation_Invalid_Value_3_Param() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValid");
            ValidationContext context = new ValidationContext(new object(), null, null);
            context.MemberName = "member";
            Assert.IsNotNull(cva.GetValidationResult("Joe", context));
            Assert.AreEqual("$member", cva.FormatErrorMessage(""));
        }

        [TestMethod]
        [Description("Setting illegal value returns false, 1 parameter method")]
        public void CustomValidation_Invalid_Value_1_Param() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNoMessage");
            cva.ErrorMessage = "The value '{0}' is bogus";  // to show we get message from ErrorMessage instead
            ValidationContext context = new ValidationContext(new object(), null, null);

            Assert.IsNotNull(cva.GetValidationResult("Joe", context));
            Assert.AreEqual("The value 'joe' is bogus", cva.FormatErrorMessage("joe"));
        }

        [TestMethod]
        [Description("Setting inconvertible reference type fails")]
        public void CustomValidation_Invalid_Type_Reference_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValid");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNotNull(cva.GetValidationResult(cva, context));   // cva is wrong type -- must be string
            Assert.AreEqual("wrong is not valid.", cva.FormatErrorMessage("wrong"));
        }

        [TestMethod]
        [Description("Setting legal custom reference type succeeds")]
        public void CustomValidation_Legal_Custom_Reference_Type() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidRefType");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNull(cva.GetValidationResult(new BaseRefType(), context));
        }

        [TestMethod]
        [Description("Setting legal derived reference type succeeds")]
        public void CustomValidation_Legal_Derived_Reference_Type() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidRefType");
            DerivedRefType instance = new DerivedRefType();
            ValidationContext context = new ValidationContext(instance, null, null);
            Assert.IsNull(cva.GetValidationResult(instance, context));
        }

        [TestMethod]
        [Description("Setting illegal supertype reference type fails")]
        public void CustomValidation_Illegal_SuperType_Reference_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidRefType");
            BaseBaseRefType instance = new BaseBaseRefType();
            ValidationContext context = new ValidationContext(instance, null, null);
            Assert.IsNotNull(cva.GetValidationResult(instance, context));
        }

        #endregion ReferenceTypes

        #region ValueTypes

        [TestMethod]
        [Description("Setting legal value type succeeds")]
        public void CustomValidation_Legal_Value_Type() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidValueType");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNull(cva.GetValidationResult(5, context));
        }

        [TestMethod]
        [Description("Setting legal value type succeeds")]
        public void CustomValidation_Legal_Convertible_Value_Type() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidValueType");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNull(cva.GetValidationResult(5, context));
            Assert.IsNull(cva.GetValidationResult((byte)1, context));
            Assert.IsNull(cva.GetValidationResult((short)2, context));
            Assert.IsNull(cva.GetValidationResult((long)2, context));
            Assert.IsNull(cva.GetValidationResult((uint)3, context));
            Assert.IsNull(cva.GetValidationResult((ulong)3, context));
            Assert.IsNull(cva.GetValidationResult(4.1f, context));
            Assert.IsNull(cva.GetValidationResult(5.2d, context));
            Assert.IsNull(cva.GetValidationResult("6", context));
            Assert.IsNull(cva.GetValidationResult((decimal)7.33, context));
        }


        [TestMethod]
        [Description("Setting out of range value type fails")]
        public void CustomValidation_Out_of_Range_Value_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidValueType");
            ValidationContext context = new ValidationContext(new object(), null, null);
            context.MemberName = "range";
            Assert.IsNotNull(cva.GetValidationResult(-1, context));
            Assert.AreEqual("wrong range", cva.FormatErrorMessage(""));
        }

        [TestMethod]
        [Description("Setting null value type fails")]
        public void CustomValidation_Null_Value_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidValueType");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNotNull(cva.GetValidationResult(null, context));
            Assert.AreEqual("null is not valid.", cva.FormatErrorMessage("null"));
        }

        [TestMethod]
        [Description("Setting unconvertable type for value type fails")]
        public void CustomValidation_NonConvertable_Value_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidValueType");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNotNull(cva.GetValidationResult("fred", context));
            Assert.AreEqual("type is not valid.", cva.FormatErrorMessage("type"));
        }

        [TestMethod]
        [Description("Setting custom type for value type succeeds")]
        public void CustomValidation_CustomType_Succeeds() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidCustomType");
            Customer cust = new Customer { IsValid = true };
            ValidationContext context = new ValidationContext(cust, null, null);
            Assert.IsNull(cva.GetValidationResult(cust, context));
        }

        #endregion ValueTypes

        #region Nullables
        [TestMethod]
        [Description("Setting legal nullable literal value type succeeds")]
        public void CustomValidation_Legal_Nullable_Value_Type_Literal() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNullableValueType");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNull(cva.GetValidationResult(5, context));
        }

        [TestMethod]
        [Description("Setting legal nullable value type succeeds")]
        public void CustomValidation_Legal_Nullable_Value_Type() {
            int? nullableInt = 5;
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNullableValueType");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNull(cva.GetValidationResult(nullableInt, context));
        }

        [TestMethod]
        [Description("Setting explicit null to nullable value type succeeds")]
        public void CustomValidation_Explicit_Null_Nullable_Value_Type() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNullableValueType");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNull(cva.GetValidationResult(null, context));
        }

        [TestMethod]
        [Description("Setting null nullable value type succeeds")]
        public void CustomValidation_Null_Nullable_Value_Type() {
            int? nullableInt = null;
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNullableValueType");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNull(cva.GetValidationResult(nullableInt, context));
        }

        [TestMethod]
        [Description("Setting unconvertable type for nullable value type fails")]
        public void CustomValidation_NonConvertable_Nullable_Value_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNullableValueType");
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsNotNull(cva.GetValidationResult("fred", context));
            Assert.AreEqual("type is not valid.", cva.FormatErrorMessage("type"));
        }
        #endregion Nullables

        // Helper to assert the attribute fails with the expected error
        private static void AssertFailure(CustomValidationAttribute cva, string expectedError) {
            ValidationContext context = new ValidationContext(new object(), null, null);

            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                cva.GetValidationResult(null, context);
            }, expectedError);

            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                cva.FormatErrorMessage(string.Empty);
            }, expectedError);
        }

        private class PrivateMockValidator {

        }
    }

    public class BaseBaseRefType { }                // ultimate base -- not acceptable
    public class BaseRefType : BaseBaseRefType { }  // acceptable base type     
    public class DerivedRefType : BaseRefType { }   // acceptable derived type


    public class MockValidator {
        // Custom validation method that matches only "Bob"
        public static ValidationResult ValidatesBob(object valueObject, ValidationContext context) {
            string value = valueObject as string;
            if (!(value != null && value.Equals("Bob")))
                return new ValidationResult("$" + context.MemberName);
            return null;
        }

        // Full form of method -- 2 parameters -- only matches "Fred" and will accept only strings
        public static ValidationResult IsValid(string value, ValidationContext context) {
            if (!(value != null && value.Equals("Fred")))
                return new ValidationResult("$" + context.MemberName);
            return null;
        }

        // Short form of method, no errorMessage.  Validates only "Sue" as true
        public static ValidationResult IsValidNoMessage(string value) {
            return (value != null && value.Equals("Sue")) ? null : new ValidationResult(null);
        }

        // Matches any BaseRefType
        public static ValidationResult IsValidRefType(BaseRefType value, ValidationContext context) {
            if (!(value != null))
                return new ValidationResult("$" + context.MemberName);
            return null;
        }

        // Value type
        public static ValidationResult IsValidValueType(int value, ValidationContext context) {
            if (value < 0 || value >= 10) {
                return new ValidationResult("wrong " + context.MemberName);
            }
            return null;
        }

        // Nullable value type
        public static ValidationResult IsValidNullableValueType(int? value, ValidationContext context) {
            // Null is acceptable, else from 0 - 9
            if (value.HasValue && (value < 0 || value >= 10)) {
                return new ValidationResult("wrong " + context.MemberName);
            }
            return null;
        }

        // Custom type
        public static ValidationResult IsValidCustomType(Customer customer, ValidationContext context) {
            if (!customer.IsValid) {
                return new ValidationResult("Customer is invalid", new[] { "IsValid" });
            }

            return null;
        }

        public static ValidationResult NotInObject(out object valueObject, ValidationContext context) { valueObject = null; return null; }
        public static ValidationResult NotInContext(object valueObject, out ValidationContext context) { context = null; return null; }
        internal static ValidationResult NotPublic(object value, ValidationContext context) { return null; }
        public ValidationResult NotStatic(object value, ValidationContext context) { return null; }
        public static void VoidReturn(object value, ValidationContext context) { }
        public static bool BoolReturnShort(object value) { return false; }
        public static bool BoolReturnLong(object value, ValidationContext context) { return false; }
        public static ValidationResult NoParams() { return null; }
        public static ValidationResult TooManyParams(object o, ValidationContext context, object o1) { return null; }
        public static ValidationResult Throws(object o, ValidationContext context) { throw new ArgumentException("o"); }
    }

    public class Customer {
        public bool IsValid { get; set; }
    }

}
