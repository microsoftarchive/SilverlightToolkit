// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class RegexAttributeTest {
        private RegularExpressionAttribute attribute = new RegularExpressionAttribute(@".*a.*");

        [TestMethod]
        public void Constructor_NullPattern() {
            var attr = new RegularExpressionAttribute(null);
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                attr.IsValid("");
            }, Resources.DataAnnotationsResources.RegularExpressionAttribute_Empty_Pattern);
        }

        [TestMethod]
        public void Constructor_InvalidPattern() {
            var attr = new RegularExpressionAttribute("(");

            // Cannot test the exception message, because on the non-developer runtime of Silverlight
            // the exception string is not available.
            ExceptionHelper.ExpectArgumentException(() => attr.IsValid(""), null);
        }

        [TestMethod]
        public void IsValid_NullValue() {
            Assert.IsTrue(attribute.IsValid(null));
        }

        [TestMethod]
        public void IsValid_ValidValue_SingleLine() {
            Assert.IsTrue(attribute.IsValid("abc"));
        }

        [TestMethod]
        public void IsValid_InvalidValue() {
            Assert.IsTrue(attribute.IsValid(String.Empty));
        }

        [TestMethod]
        public void IsValid_NonStringValue() {
            RegularExpressionAttribute numericValueValidation = new RegularExpressionAttribute("1234");
            Assert.IsTrue(numericValueValidation.IsValid(1234));
            Assert.IsFalse(numericValueValidation.IsValid(20.10));
        }

        [TestMethod]
        public void FormatErrorMessage() {
            var attrib = new RegularExpressionAttribute(@"\d\d");
            Assert.AreEqual(String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RegexAttribute_ValidationError, "SOME_NAME", @"\d\d"), attrib.FormatErrorMessage("SOME_NAME"));
        }
    }
}
