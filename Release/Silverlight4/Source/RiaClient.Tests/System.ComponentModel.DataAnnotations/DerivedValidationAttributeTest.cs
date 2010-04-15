// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class DerivedValidationAttributeTest {
        [TestMethod]
        [Description("Calling short form of IsValid invokes the derived attributes short form")]
        public void Derived_ValidationAttribute_Short_Form_Called() {
            MyValAttr m = new MyValAttr();
            Assert.IsTrue(m.IsValid("stuff"));
            Assert.IsTrue(m._longIsValidCalled);
            Assert.IsNull(m._validationContext);
            Assert.AreEqual("stuff", m._object);
        }

        [TestMethod]
        [Description("Calling TryValidate calls the long form of IsValid")]
        public void Derived_ValidationAttribute_Long_Form_Called() {
            MyValAttr m = new MyValAttr();
            ValidationContext context = new ValidationContext(new object(), null, null);
            ValidationResult result = m.GetValidationResult("stuff", context);

            Assert.IsTrue(m._longIsValidCalled);
            Assert.IsNotNull(m._validationContext);
            Assert.IsNull(result);
            Assert.AreEqual("stuff", m._object);
        }

        [TestMethod]
        [Description("Calling short form of IsValid with an error invokes the derived attributes short form")]
        public void Derived_ValidationAttribute_Short_Form_Called_With_Error() {
            MyValAttr m = new MyValAttr("error message");
            Assert.IsFalse(m.IsValid("stuff"));
            Assert.IsTrue(m._longIsValidCalled);
            Assert.IsNull(m._validationContext);
            Assert.AreEqual("stuff", m._object);
        }

        [TestMethod]
        [Description("Calling TryValidate with an error calls the long form of IsValid")]
        public void Derived_ValidationAttribute_Long_Form_Called_With_Error() {
            MyValAttr m = new MyValAttr("error message");
            ValidationContext context = new ValidationContext(new object(), null, null);
            context.MemberName = "member";
            ValidationResult result = m.GetValidationResult("stuff", context);

            Assert.IsTrue(m._longIsValidCalled);
            Assert.IsNotNull(m._validationContext);
            Assert.IsNotNull(result);
            Assert.AreEqual("stuff", m._object);
            Assert.AreEqual("error message", result.ErrorMessage);
            IEnumerable<string> memberNames = result.MemberNames;
            Assert.IsNotNull(memberNames);
            Assert.AreEqual(1, memberNames.Count());
            Assert.AreEqual("member", memberNames.First());
        }

        [TestMethod]
        [Description("Calling TryValidate with an error and member names calls the long form of IsValid")]
        public void Derived_ValidationAttribute_Long_Form_Called_With_Error_And_MemberNames() {
            MyValAttr m = new MyValAttr("error message", new string[] { "m1", "m2" });
            ValidationContext context = new ValidationContext(new object(), null, null);
            context.MemberName = "member";
            ValidationResult result = m.GetValidationResult("stuff", context);

            Assert.IsTrue(m._longIsValidCalled);
            Assert.IsNotNull(m._validationContext);
            Assert.IsNotNull(result);
            Assert.AreEqual("stuff", m._object);
            Assert.AreEqual("error message", result.ErrorMessage);
            IEnumerable<string> memberNames = result.MemberNames;
            Assert.IsNotNull(memberNames);
            Assert.AreEqual(2, memberNames.Count());
            Assert.IsTrue(memberNames.Contains("m1"));
            Assert.IsTrue(memberNames.Contains("m2"));

        }

        [TestMethod]
        [Description("Calling TryValidate with an error default to stock error message")]
        public void Derived_ValidationAttribute_Long_Form_Defaults_Error_Message() {
            MyValAttr m = new MyValAttr(null);
            m.ErrorMessage = "stock error message for {0}";
            ValidationContext context = new ValidationContext(new MyObject(), null, null);
            context.MemberName = "Member";
            ValidationResult result = m.GetValidationResult("stuff", context);

            Assert.IsNotNull(result);
            Assert.AreEqual("stock error message for Member", result.ErrorMessage);
        }
    }

    public class MyValAttr : ValidationAttribute {
        internal bool _longIsValidCalled;
        internal object _object;
        internal ValidationContext _validationContext;
        internal bool _valueToReturn;
        internal string _errorMessage;
        internal IEnumerable<string> _memberNames;

        public MyValAttr() {
            this._valueToReturn = true;
        }

        public MyValAttr(string errorMessage) {
            this._valueToReturn = false;
            this._errorMessage = errorMessage;
        }

        public MyValAttr(string errorMessage, IEnumerable<string> memberNames) {
            this._valueToReturn = false;
            this._errorMessage = errorMessage;
            this._memberNames = memberNames;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context) {
            this._object = value;
            this._longIsValidCalled = true;
            this._validationContext = context;
            ValidationResult result = null;

            if (!this._valueToReturn) {
                if (this._memberNames == null && context != null) {
                    this._memberNames = new string[] { context.MemberName };
                }
                result = new ValidationResult(this._errorMessage, this._memberNames);
            }
            return result;

        }
    }

    public class MyObject {
        public string Member { get { return null; } set { } }
    }
}
