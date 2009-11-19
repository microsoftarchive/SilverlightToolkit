// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class RequiredAttributeTest {
        [TestMethod]
        public void IsValid() {
            RequiredAttribute required = new RequiredAttribute();

            Assert.AreEqual(true, required.IsValid("abcd"));
            Assert.AreEqual(true, required.IsValid("  ab  "));
            Assert.AreEqual(true, required.IsValid(10));

            Assert.AreEqual(false, required.IsValid(""));
            Assert.AreEqual(false, required.IsValid("   "));
            Assert.AreEqual(false, required.IsValid("\t"));
            Assert.AreEqual(false, required.IsValid(null));
        }

        [TestMethod]
        public void DefaultErrorMessage() {
            RequiredAttribute required = new RequiredAttribute();
            Assert.AreEqual(String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.RequiredAttribute_ValidationError, "SOME_NAME"), required.FormatErrorMessage("SOME_NAME"));
        }
    }
}
