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

#if !SILVERLIGHT
            ValidationResult original = new ValidationResult("error", new string[] { "a", "b" });
            result = new DerivedValidationResult(original);
            Assert.AreEqual("error", result.ErrorMessage);
            CollectionAssert.AreEquivalent(new string[] { "a", "b" }, result.MemberNames.ToList());

            ExceptionHelper.ExpectArgumentNullException(delegate() {
                new DerivedValidationResult(null);
            }, "validationResult");
#endif
        }

#if SILVERLIGHT
        [TestMethod]
        [Description("ValidationResult.ToString() returns the ErrorMessage when specified")]
        public void ValidationResult_ToString_ErrorMessage_Specified() {
            string message = "This is the error message";
            ValidationResult result = new ValidationResult(message);
            Assert.AreEqual(message, result.ToString());
        }

        [TestMethod]
        [Description("ValidationResult.ToString() returns the base ToString() result when ErrorMessage is null")]
        public void ValidationResult_ToString_ErrorMessage_Null() {
            ValidationResult result = new ValidationResult(null);
            Assert.AreEqual(typeof(ValidationResult).FullName, result.ToString());
        }

        [TestMethod]
        [Description("ValidationResult.ToString() returns the ErrorMessage when it is empty but not null")]
        public void ValidationResult_ToString_ErrorMessage_Empty() {
            ValidationResult result = new ValidationResult(string.Empty);
            Assert.AreEqual(string.Empty, result.ToString());
        }
#endif

#if !SILVERLIGHT
        public class DerivedValidationResult : ValidationResult {
            public DerivedValidationResult(ValidationResult validationResult)
                : base(validationResult) {
            }
        }
#endif
    }
}
