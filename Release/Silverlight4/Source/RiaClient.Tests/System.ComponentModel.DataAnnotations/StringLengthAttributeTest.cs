// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class StringLengthAttributeTest {
        private StringLengthAttribute attribute = new StringLengthAttribute(3);

        [TestMethod]
        [Description("Ctor(maxLength) succeeds")]
        public void StringLengthAttribute_Ctor() {
            StringLengthAttribute attr = new StringLengthAttribute(10);
            Assert.AreEqual(10, attr.MaximumLength);
            Assert.AreEqual(0, attr.MinimumLength);
        }


        [TestMethod]
        [Description("Ctor(minLength, maxLength) succeeds")]
        public void StringLengthAttribute_Ctor_And_MinLength() {
            StringLengthAttribute attr = new StringLengthAttribute(10);
            attr.MinimumLength = 5;
            Assert.AreEqual(10, attr.MaximumLength);
            Assert.AreEqual(5, attr.MinimumLength);
        }

        [TestMethod]
        [Description("Ctor accept invalid values without throwing exceptions")]
        public void StringLengthAttribute_Invalid_Ctor_And_MinLength_No_Exceptions() {
            StringLengthAttribute attr = new StringLengthAttribute(-10);
            attr.MinimumLength = -5;
            Assert.AreEqual(-5, attr.MinimumLength);
            Assert.AreEqual(-10, attr.MaximumLength);
        }

        [TestMethod]
        [Description("Negative maximum length fails in use")]
        public void StringLengthAttribute_Fail_Negative_Max() {
            StringLengthAttribute attr = new StringLengthAttribute(-10);
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid("");
            }, Resources.DataAnnotationsResources.StringLengthAttribute_InvalidMaxLength);
        }

        [TestMethod]
        [Description("Min exceeding max length fails in use")]
        public void StringLengthAttribute_Fail_Min_Exceed_Max() {
            StringLengthAttribute attr = new StringLengthAttribute(5);
            attr.MinimumLength = 10;
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid("");
            },
            String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RangeAttribute_MinGreaterThanMax, 5, 10));
        }

        [TestMethod]
        public void StringLengthAttribute_Constructor_ZeroMaxLength() {
            new StringLengthAttribute(0);
        }

        [TestMethod]
        public void IsValid_ValidString() {
            Assert.AreEqual(true, attribute.IsValid(String.Empty));
            Assert.AreEqual(true, attribute.IsValid("abc"));
        }

        [TestMethod]
        public void StringLengthAttribute_IsValid_Min_And_Max() {
            StringLengthAttribute attr = new StringLengthAttribute(4);
            attr.MinimumLength = 2;
            Assert.IsFalse(attr.IsValid(""));
            Assert.IsFalse(attr.IsValid("a"));
            Assert.IsTrue(attr.IsValid("ab"));
            Assert.IsTrue(attr.IsValid("abc"));
            Assert.IsTrue(attr.IsValid("abcd"));
            Assert.IsFalse(attr.IsValid("abcde"));
        }

        [TestMethod]
        public void IsValid_InvalidString() {
            Assert.AreEqual(false, new StringLengthAttribute(3).IsValid("abcd"));
        }


        [TestMethod]
        public void IsValid_NullValue() {
            Assert.AreEqual(true, attribute.IsValid(null));
        }

        [TestMethod]
        public void IsValid_NonStringValue() {
            ExceptionHelper.ExpectException<InvalidCastException>(delegate() {
                attribute.IsValid(1);
            });
        }

        [TestMethod]
        public void FormatErrorMessage() {
            var attrib = new StringLengthAttribute(3);
            Assert.AreEqual(String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.StringLengthAttribute_ValidationError, "SOME_NAME", 3), attrib.FormatErrorMessage("SOME_NAME"));
        }

        [TestMethod]
        public void FormatErrorMessage_CustomError() {
            var attrib = new StringLengthAttribute(3) { ErrorMessage = "Foo {0} Bar {1} Baz {2}" };
            Assert.AreEqual("Foo SOME_NAME Bar 3 Baz 0", attrib.FormatErrorMessage("SOME_NAME"));
        }

        [TestMethod]
        public void FormatErrorMessage_MinimumLength() {
            var attrib = new StringLengthAttribute(3) { MinimumLength = 1 };
            Assert.AreEqual(String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.StringLengthAttribute_ValidationErrorIncludingMinimum, "SOME_NAME", 3, 1), attrib.FormatErrorMessage("SOME_NAME"));
        }

        [TestMethod]
        public void FormatErrorMessage_MinimumLength_CustomError() {
            var attrib = new StringLengthAttribute(3) { MinimumLength = 1, ErrorMessage = "Foo {0} Bar {1} Baz {2}" };
            Assert.AreEqual("Foo SOME_NAME Bar 3 Baz 1", attrib.FormatErrorMessage("SOME_NAME"));
        }
    }
}
