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
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);

            ExceptionHelper.ExpectArgumentException(delegate() {
                cva.TryValidate("Fred", context, out validationResult);    // valid call, but this method throws
            }, "o");
        }

        [TestMethod]
        [Description("Setting non-existing method returns error message")]
        public void CustomValidation_Fail_Method_Doesnt_Exist() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "DoesntExist");
            AssertFailure(cva, "The CustomValidationAttribute method 'DoesntExist' does not exist in type 'MockValidator' or is not public and static (or Shared in Visual Basic).");
        }

        [TestMethod]
        [Description("Setting non-public method returns error message")]
        public void CustomValidation_Fail_Method_Not_Public() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "NotPublic");
            AssertFailure(cva, "The CustomValidationAttribute method 'NotPublic' does not exist in type 'MockValidator' or is not public and static (or Shared in Visual Basic).");
        }

        [TestMethod]
        [Description("Setting non-static method returns error message")]
        public void CustomValidation_Fail_Method_Not_Static() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "NotStatic");
            AssertFailure(cva, "The CustomValidationAttribute method 'NotStatic' does not exist in type 'MockValidator' or is not public and static (or Shared in Visual Basic).");
        }

        [TestMethod]
        [Description("Setting void returning method returns error message")]
        public void CustomValidation_Fail_Method_Void_Return() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "VoidReturn");
            AssertFailure(cva, "The CustomValidationAttribute method 'VoidReturn' in type 'MockValidator' must return a Boolean.");
        }

        [TestMethod]
        [Description("Setting parameterless method returns error message")]
        public void CustomValidation_Fail_Method_No_Params() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "NoParams");
            AssertFailure(cva, "The CustomValidationAttribute method 'NoParams' in type 'MockValidator' must declare an input parameter for the value.");
        }

        [TestMethod]
        [Description("Setting method without 'out' param returns error message")]
        public void CustomValidation_Fail_Method_Not_Out_Params() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "NotOut");
            AssertFailure(cva, "The CustomValidationAttribute method 'NotOut' in type 'MockValidator' does not have the correct signature.  It should declare 3 parameters: the object to be validated, a ValidationContext, and a parameter to receive a ValidationResult.   (In C#, this last parameter must be an \"out\" parameter. In Visual Basic, this last parameter must be passed using \"ByRef\".)");
        }

        [TestMethod]
        [Description("Setting method with 'out' first param returns error message")]
        public void CustomValidation_Fail_Method_Param1_Out() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "NotIn");
            AssertFailure(cva, "The CustomValidationAttribute method 'NotIn' in type 'MockValidator' must declare an input parameter for the value.");
        }


        [TestMethod]
        [Description("Setting method with too many parameters returns error message")]
        public void CustomValidation_Fail_Method_Too_Many_Params() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "TooManyParams");
            AssertFailure(cva, "The CustomValidationAttribute method 'TooManyParams' in type 'MockValidator' does not have the correct signature.  It should declare 3 parameters: the object to be validated, a ValidationContext, and a parameter to receive a ValidationResult.   (In C#, this last parameter must be an \"out\" parameter. In Visual Basic, this last parameter must be passed using \"ByRef\".)");
        }
        #endregion Method

        #region Reference Types

        [TestMethod]
        [Description("Setting valid value succeeds, 1 param form")]
        public void CustomValidation_Valid_Value_1_Param() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNoMessage");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);

            Assert.IsTrue(cva.TryValidate("Sue", context, out validationResult));
        }

        [TestMethod]
        [Description("Setting valid value succeeds, 2 param form")]
        public void CustomValidation_Valid_Value_2_Param() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValid");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsTrue(cva.TryValidate("Fred", context, out validationResult));
        }

        [TestMethod]
        [Description("Setting valid value fails using short form of API against long form of custom method")]
        public void CustomValidation_Fail_Short_Form_Call() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValid");
            //Assert.IsTrue(cva.IsValid("Fred"));
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                cva.IsValid("Fred");
            }, "The CustomValidationAttribute method 'IsValid' in type 'MockValidator' requires 3 parameters: the object, a ValidationContext and a ValidationResult parameter that will be updated with the validation result.  (In C#, this last parameter must be an \"out\" parameter. In Visual Basic, this last parameter must be passed using \"ByRef\".)");
        }

        [TestMethod]
        [Description("Setting invalid value returns false, 3 parameter method")]
        public void CustomValidation_Invalid_Value_3_Param() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValid");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            context.MemberName = "member";
            Assert.IsFalse(cva.TryValidate("Joe", context, out validationResult));
            Assert.AreEqual("$member", cva.FormatErrorMessage(""));
        }

        [TestMethod]
        [Description("Setting illegal value returns false, 1 parameter method")]
        public void CustomValidation_Invalid_Value_1_Param() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNoMessage");
            cva.ErrorMessage = "The value '{0}' is bogus";  // to show we get message from ErrorMessage instead
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);

            Assert.IsFalse(cva.TryValidate("Joe", context, out validationResult));
            Assert.AreEqual("The value 'joe' is bogus", cva.FormatErrorMessage("joe"));
        }

        [TestMethod]
        [Description("Setting inconvertible reference type fails")]
        public void CustomValidation_Invalid_Type_Reference_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValid");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsFalse(cva.TryValidate(cva, context, out validationResult));   // cva is wrong type -- must be string
            Assert.AreEqual("wrong is not valid.", cva.FormatErrorMessage("wrong"));
        }

        [TestMethod]
        [Description("Setting legal custom reference type succeeds")]
        public void CustomValidation_Legal_Custom_Reference_Type() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidRefType");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsTrue(cva.TryValidate(new BaseRefType(), context, out validationResult));
        }

        [TestMethod]
        [Description("Setting legal derived reference type succeeds")]
        public void CustomValidation_Legal_Derived_Reference_Type() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidRefType");
            ValidationResult validationResult = null;
            DerivedRefType instance = new DerivedRefType();
            ValidationContext context = new ValidationContext(instance, null, null);
            Assert.IsTrue(cva.TryValidate(instance, context, out validationResult));
        }

        [TestMethod]
        [Description("Setting illegal supertype reference type fails")]
        public void CustomValidation_Illegal_SuperType_Reference_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidRefType");
            ValidationResult validationResult = null;
            BaseBaseRefType instance = new BaseBaseRefType();
            ValidationContext context = new ValidationContext(instance, null, null);
            Assert.IsFalse(cva.TryValidate(instance, context, out validationResult));
        }

        #endregion ReferenceTypes

        #region ValueTypes

        [TestMethod]
        [Description("Setting legal value type succeeds")]
        public void CustomValidation_Legal_Value_Type() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidValueType");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsTrue(cva.TryValidate(5, context, out validationResult));
        }

        [TestMethod]
        [Description("Setting legal value type succeeds")]
        public void CustomValidation_Legal_Convertible_Value_Type() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidValueType");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsTrue(cva.TryValidate(5, context, out validationResult));
            Assert.IsTrue(cva.TryValidate((byte)1, context, out validationResult));
            Assert.IsTrue(cva.TryValidate((short)2, context, out validationResult));
            Assert.IsTrue(cva.TryValidate((long)2, context, out validationResult));
            Assert.IsTrue(cva.TryValidate((uint)3, context, out validationResult));
            Assert.IsTrue(cva.TryValidate((ulong)3, context, out validationResult));
            Assert.IsTrue(cva.TryValidate(4.1f, context, out validationResult));
            Assert.IsTrue(cva.TryValidate(5.2d, context, out validationResult));
            Assert.IsTrue(cva.TryValidate("6", context, out validationResult));
            Assert.IsTrue(cva.TryValidate((decimal)7.33, context, out validationResult));
        }


        [TestMethod]
        [Description("Setting out of range value type fails")]
        public void CustomValidation_Out_of_Range_Value_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidValueType");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            context.MemberName = "range";
            Assert.IsFalse(cva.TryValidate(-1, context, out validationResult));
            Assert.AreEqual("wrong range", cva.FormatErrorMessage(""));
        }

        [TestMethod]
        [Description("Setting null value type fails")]
        public void CustomValidation_Null_Value_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidValueType");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsFalse(cva.TryValidate(null, context, out validationResult));
            Assert.AreEqual("null is not valid.", cva.FormatErrorMessage("null"));
        }

        [TestMethod]
        [Description("Setting unconvertable type for value type fails")]
        public void CustomValidation_NonConvertable_Value_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidValueType");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsFalse(cva.TryValidate("fred", context, out validationResult));
            Assert.AreEqual("type is not valid.", cva.FormatErrorMessage("type"));
        }

        #endregion ValueTypes

        #region Nullables
        [TestMethod]
        [Description("Setting legal nullable literal value type succeeds")]
        public void CustomValidation_Legal_Nullable_Value_Type_Literal() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNullableValueType");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsTrue(cva.TryValidate(5, context, out validationResult));
        }

        [TestMethod]
        [Description("Setting legal nullable value type succeeds")]
        public void CustomValidation_Legal_Nullable_Value_Type() {
            int? nullableInt = 5;
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNullableValueType");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsTrue(cva.TryValidate(nullableInt, context, out validationResult));
        }

        [TestMethod]
        [Description("Setting explicit null to nullable value type succeeds")]
        public void CustomValidation_Explicit_Null_Nullable_Value_Type() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNullableValueType");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsTrue(cva.TryValidate(null, context, out validationResult));
        }

        [TestMethod]
        [Description("Setting null nullable value type succeeds")]
        public void CustomValidation_Null_Nullable_Value_Type() {
            int? nullableInt = null;
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNullableValueType");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsTrue(cva.TryValidate(nullableInt, context, out validationResult));
        }

        [TestMethod]
        [Description("Setting unconvertable type for nullable value type fails")]
        public void CustomValidation_NonConvertable_Nullable_Value_Type_Fails() {
            CustomValidationAttribute cva = new CustomValidationAttribute(typeof(MockValidator), "IsValidNullableValueType");
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);
            Assert.IsFalse(cva.TryValidate("fred", context, out validationResult));
            Assert.AreEqual("type is not valid.", cva.FormatErrorMessage("type"));
        }
        #endregion Nullables

        // Helper to assert the attribute fails with the expected error
        private static void AssertFailure(CustomValidationAttribute cva, string expectedError) {
            ValidationResult validationResult = null;
            ValidationContext context = new ValidationContext(new object(), null, null);

            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                cva.TryValidate(null, context, out validationResult);
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
        public static bool ValidatesBob(object valueObject, ValidationContext context, out ValidationResult validationResult) {
            validationResult = null;
            string value = valueObject as string;
            bool result = (value != null && value.Equals("Bob"));
            if (!result)
                validationResult = new ValidationResult("$" + context.MemberName);
            return result;
        }

        // Full form of method -- 2 parameters -- only matches "Fred" and will accept only strings
        public static bool IsValid(string value, ValidationContext context, out ValidationResult validationResult) {
            validationResult = null;
            bool result = (value != null && value.Equals("Fred"));
            if (!result)
                validationResult = new ValidationResult("$" + context.MemberName);
            return result;
        }

        // Short form of method, no errorMessage.  Validates only "Sue" as true
        public static bool IsValidNoMessage(string value) {
            bool result = (value != null && value.Equals("Sue"));
            return result;
        }

        // Matches any BaseRefType
        public static bool IsValidRefType(BaseRefType value, ValidationContext context, out ValidationResult validationResult) {
            validationResult = null;
            bool result = (value != null);
            if (!result)
                validationResult = new ValidationResult("$" + context.MemberName);
            return result;
        }

        // Value type
        public static bool IsValidValueType(int value, ValidationContext context, out ValidationResult validationResult) {
            validationResult = null;
            if (value < 0 || value >= 10) {
                validationResult = new ValidationResult("wrong " + context.MemberName);
            }
            return validationResult == null;
        }

        // Nullable value type
        public static bool IsValidNullableValueType(int? value, ValidationContext context, out ValidationResult validationResult) {
            validationResult = null;
            // Null is acceptable, else from 0 - 9
            if (value.HasValue && (value < 0 || value >= 10)) {
                validationResult = new ValidationResult("wrong " + context.MemberName);
            }
            return validationResult == null;
        }

        public static bool NotOut(object valueObject, ValidationContext context, ValidationResult validationResult) { return false; }
        public static bool NotIn(out object valueObject, ValidationContext context, out ValidationResult validationResult) { valueObject = null; validationResult = null; return false; }
        internal static bool NotPublic(object value, ValidationContext context, out ValidationResult validationResult) { validationResult = null; return false; }
        public bool NotStatic(object value, ValidationContext context, out ValidationResult validationResult) { validationResult = null; return false; }
        public static void VoidReturn(object value, ValidationContext context, out ValidationResult validationResult) { validationResult = null; }
        public static bool NoParams() { return false; }
        public static bool TooManyParams(object o, ValidationContext context, out ValidationResult validationResult, object o1) { validationResult = null; return false; }
        public static bool Throws(object o, ValidationContext context, out ValidationResult validationResult) { throw new ArgumentException("o"); }
    }



}
