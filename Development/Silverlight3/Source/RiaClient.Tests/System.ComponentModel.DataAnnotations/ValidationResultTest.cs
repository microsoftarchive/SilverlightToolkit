using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class ValidationResultTest {

        [TestMethod]
        [Description("Tests all legal ctors for ValidationResult")]
        public void ValidationResult_Ctors() {
            ValidationResult result = new ValidationResult(null);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsTrue(!result.MemberNames.Any());

            result = new ValidationResult(string.Empty);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
            Assert.IsTrue(!result.MemberNames.Any());

            result = new ValidationResult("stuff");
            Assert.AreEqual("stuff", result.ErrorMessage);
            Assert.IsTrue(!result.MemberNames.Any());

            result = new ValidationResult("stuff", new string[] { "m1", "m2" });
            Assert.AreEqual("stuff", result.ErrorMessage);
            IEnumerable<string> memberNames = result.MemberNames;
            Assert.AreEqual(2, memberNames.Count());
            Assert.IsTrue(memberNames.Contains("m1"));
            Assert.IsTrue(memberNames.Contains("m2"));
        }
    }
}
