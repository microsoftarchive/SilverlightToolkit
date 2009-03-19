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
            }, "The pattern must be set to a valid regular expression.");
        }

        [TestMethod]
        public void Constructor_InvalidPattern() {
            var attr = new RegularExpressionAttribute("(");
            ExceptionHelper.ExpectArgumentException(delegate() {
                attr.IsValid("");
            }, "parsing \"(\" - Not enough )'s.");
        }

        [TestMethod]
        public void IsValid_NullValue() {
            Assert.IsTrue(attribute.IsValid(null));
        }

        [TestMethod]
        public void IsValid_ValidValue_SingleLine() {
            Assert.IsTrue(attribute.IsValid("abc"));
        }

        // This test is no longer valid since we now use MultiLine mode and check for match length, to match the
        // behavior of RegularExpressionValidator.
#if OLD
        [TestMethod]
        public void IsValid_ValidValue_MultiLine() {
            Assert.IsTrue(attribute.IsValid("That is a nice\r\nmultiline陌陏陋陷\r\nstring."));
        }
#endif

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
            Assert.AreEqual(@"The field SOME_NAME must match the regular expression '\d\d'.", attrib.FormatErrorMessage("SOME_NAME"));
        }
    }
}
