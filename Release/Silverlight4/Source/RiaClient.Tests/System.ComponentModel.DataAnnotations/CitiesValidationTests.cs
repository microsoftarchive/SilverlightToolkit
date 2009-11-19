// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Cities;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {

    [TestClass]
    public class CitiesValidationTests : SilverlightTest {
        #region Property tests

        [TestMethod]
        [Description("Normal ctor for State validates legal values correctly")]
        public void Validation_Cities_Ctor_State() {
            State state = new State() { Name = "WA", FullName = "Washington" };
            Assert.AreEqual("WA", state.Name);
            Assert.AreEqual("Washington", state.FullName);
        }

        [TestMethod]
        [Description("Setting null to Required property passes if already null")]
        public void Validation_Cities_Set_Null_Required_Passes_If_Already_Null() {
            State state = new State() { Name = null };
        }

        [TestMethod]
        [Description("Setting null to Required property throws ValidationException")]
        public void Validation_Cities_Fail_Set_Null_Required() {
            // Must first set non-null since setter is NOP if incoming value == existing value
            State state = new State() { Name = "WA", FullName = "Washington" };

            ExceptionHelper.ExpectValidationException(delegate() {
                state.Name = null;
            }, "The Name field is required.", typeof(RequiredAttribute), null);
        }

        [TestMethod]
        [Description("Setting invalid RegExp throws ValidationException")]
        public void Validation_Cities_Fail_Set_Bad_RegExp() {
            // Must first set non-null since setter is NOP if incoming value == existing value
            State state = new State() { Name = "WA", FullName = "Washington" };

            ExceptionHelper.ExpectValidationException(delegate() {
                state.Name = "wa";
            }, "The field Name must match the regular expression '^[A-Z]*'.", typeof(RegularExpressionAttribute), "wa");
        }

        [TestMethod]
        [Description("Setting too short a string throws ValidationException")]
        public void Validation_Cities_Fail_Set_Short_StringLength() {
            // Must first set non-null since setter is NOP if incoming value == existing value
            State state = new State() { Name = "WA", FullName = "Washington" };

            ExceptionHelper.ExpectValidationException(delegate() {
                state.Name = "W";   // must be 2 letters -- custom validation exception checks that
            }, "The value for Name must have exactly 2 letters", typeof(CustomValidationAttribute), "W");
        }

        [TestMethod]
        [Description("Setting too long a string throws ValidationException")]
        public void Validation_Cities_Fail_Set_Long_StringLength() {
            // Must first set non-null since setter is NOP if incoming value == existing value
            State state = new State() { Name = "WA", FullName = "Washington" };

            ExceptionHelper.ExpectValidationException(delegate() {
                state.Name = "WASH";   // must be 2 letters -- [StringLength] validates that
            }, "The field Name must be a string with a maximum length of 2.", typeof(StringLengthAttribute), "WASH");
        }

        [TestMethod]
        [Description("Setting invalid [Range] throws ValidationException")]
        public void Validation_Cities_Fail_Set_Bad_Range() {
            ExceptionHelper.ExpectValidationException(delegate() {
                Zip zip = new Zip() { Code = 999990 };
            }, "The field Code must be between 0 and 99999.", typeof(RangeAttribute), 999990);
        }

        [TestMethod]
        [Description("Setting invalid [MustStartWith] throws ValidationException")]
        public void Validation_Cities_Fail_Set_Bad_MustStartWith() {
            ExceptionHelper.ExpectValidationException(delegate() {
                Zip zip = new Zip() { Code = 8 };
            }, "Code must start with the prefix 9", typeof(MustStartWithAttribute), 8);
        }

        [TestMethod]
        [Description("Setting [ReadOnly] property throws InvalidOperationException")]
        public void Validation_Cities_Fail_Set_ReadOnly_Property() {
            ExceptionHelper.ExpectInvalidOperationException(delegate() {
                City city = new City { CalculatedCounty = "King County" };
            }, "The CalculatedCounty property is read only.");
        }

        [TestMethod]
        [Description("Setting invalid value during deserialization succeeds silently")]
        public void Validation_Cities_Set_Invalid_Value_During_Deserialization() {
            State state = new State();
            state.OnDeserializing(new System.Runtime.Serialization.StreamingContext());
            state.Name = "bogus";   // would fail RegExp, StrLen, etc
            Assert.AreEqual("bogus", state.Name); // setter should have worked
            state.OnDeserialized(new System.Runtime.Serialization.StreamingContext());
        }

        [TestMethod]
        [Description("Setting invalid value after deserialization throws InvalidOperationException")]
        public void Validation_Cities_Fail_Invalid_Value_After_Deserialization() {
            State state = new State();
            state.OnDeserializing(new System.Runtime.Serialization.StreamingContext());
            state.Name = "bogus";   // would fail RegExp, StrLen, etc
            Assert.AreEqual("bogus", state.Name);         // setter should still have worked
            state.OnDeserialized(new System.Runtime.Serialization.StreamingContext());

            // Now, outside of serialization, the same property should throw
            ExceptionHelper.ExpectValidationException(delegate() {
                state.Name = "WASH";   // must be 2 letters -- [StringLength] validates that
            }, "The field Name must be a string with a maximum length of 2.", typeof(StringLengthAttribute), "WASH");

        }

        #endregion Property tests

        #region Object tests
        [TestMethod]
        [Description("Validating object with null Required property throws ValidationException")]
        public void Validation_Cities_Fail_Object_Null_Required() {
            // construct object with null Name -- this is invalid
            State state = new State() { FullName = "Washington" };

            ExceptionHelper.ExpectValidationException(delegate() {
                ValidationContext context = new ValidationContext(state, null, null);
                Validator.ValidateObject(state, context);
                state.Name = null;
            }, "The Name field is required.", typeof(RequiredAttribute), null);
        }

        [TestMethod]
        [Description("Cross-field validation throws ValidationException")]
        public void Validation_Cities_Fail_Object_Cross_Field_Validation() {
            // First verify the validation test passes for a legal construction
            Zip zip = new Zip() { Code = 90563, CityName = "Redmond", StateName = "WA" };
            ValidationContext context = new ValidationContext(zip, null, null);

            Validator.ValidateObject(zip, context);

            // now make a cross-field validation error
            zip.CityName = zip.StateName;

            ExceptionHelper.ExpectValidationException(delegate() {
                Validator.ValidateObject(zip, context);
            }, "Zip codes cannot have matching city and state names", typeof(CustomValidationAttribute), zip);
        }

        #endregion Object tests

    }
}
