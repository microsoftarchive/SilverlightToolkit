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
            Assert.IsTrue(m._shortIsValidCalled);
            Assert.IsFalse(m._longIsValidCalled);
            Assert.IsNull(m._validationContext);
            Assert.AreEqual("stuff", m._object);
        }

        [TestMethod]
        [Description("Calling TryValidate calls the long form of IsValid")]
        public void Derived_ValidationAttribute_Long_Form_Called() {
            MyValAttr m = new MyValAttr();
            ValidationContext context = new ValidationContext(new object(), null, null);
            ValidationResult result = null;

            Assert.IsTrue(m.TryValidate("stuff", context, out result));
            Assert.IsFalse(m._shortIsValidCalled);
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
            Assert.IsTrue(m._shortIsValidCalled);
            Assert.IsFalse(m._longIsValidCalled);
            Assert.IsNull(m._validationContext);
            Assert.AreEqual("stuff", m._object);
        }

        [TestMethod]
        [Description("Calling TryValidate with an error calls the long form of IsValid")]
        public void Derived_ValidationAttribute_Long_Form_Called_With_Error() {
            MyValAttr m = new MyValAttr("error message");
            ValidationContext context = new ValidationContext(new object(), null, null);
            context.MemberName = "member";
            ValidationResult result = null;

            Assert.IsFalse(m.TryValidate("stuff", context, out result));
            Assert.IsFalse(m._shortIsValidCalled);
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
            ValidationResult result = null;

            Assert.IsFalse(m.TryValidate("stuff", context, out result));
            Assert.IsFalse(m._shortIsValidCalled);
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
            ValidationResult result = null;

            Assert.IsFalse(m.TryValidate("stuff", context, out result));
            Assert.IsNotNull(result);
            Assert.AreEqual("stock error message for Member", result.ErrorMessage);
        }

        [TestMethod]
        [Description("Calling TryValidate with an error defaults to non-null ValidationResult")]
        public void Derived_ValidationAttribute_Long_Form_Defaults_ValidationResult() {
            MyValAttr m = new MyValAttr(null);
            m.ErrorMessage = "stock error message for {0}";
            m._returnNullValidationResult = true;   // forces it to return null ValidationResult

            ValidationContext context = new ValidationContext(new MyObject(), null, null);
            context.MemberName = "Member";
            ValidationResult result = null;

            Assert.IsFalse(m.TryValidate("stuff", context, out result));
            Assert.IsNotNull(result);
            Assert.AreEqual("stock error message for Member", result.ErrorMessage);
        }
    }

    public class MyValAttr : ValidationAttribute {
        internal bool _shortIsValidCalled;
        internal bool _longIsValidCalled;
        internal object _object;
        internal ValidationContext _validationContext;
        internal bool _valueToReturn;
        internal string _errorMessage;
        internal IEnumerable<string> _memberNames;
        internal bool _returnNullValidationResult;

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

        public override bool IsValid(object value) {
            this._object = value;
            this._shortIsValidCalled = true;
            return this._valueToReturn;
        }

        protected override bool IsValid(object value, ValidationContext context, out ValidationResult result) {
            this._object = value;
            this._longIsValidCalled = true;
            this._validationContext = context;
            result = null;
            if (!this._valueToReturn) {
                result = (this._returnNullValidationResult)
                            ? null
                            : (this._memberNames == null)
                                ? new ValidationResult(this._errorMessage)
                                : new ValidationResult(this._errorMessage, this._memberNames);
            }
            return this._valueToReturn;

        }
    }

    public class MyObject {
        public string Member { get { return null; } set { } }
    }
}
