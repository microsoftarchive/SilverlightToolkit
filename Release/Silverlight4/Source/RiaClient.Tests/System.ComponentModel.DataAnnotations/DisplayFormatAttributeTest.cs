// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.ComponentModel.DataAnnotations.Test {
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DisplayFormatAttributeTest {
        [TestMethod]
        public void DefaultAttributeValues() {
            // Setup
            DisplayFormatAttribute attribute = new DisplayFormatAttribute();

            // Verify
#if !SILVERLIGHT
            Assert.IsTrue(attribute.HtmlEncode);
#endif
            Assert.IsTrue(attribute.ConvertEmptyStringToNull);
            Assert.IsFalse(attribute.ApplyFormatInEditMode);
            Assert.IsNull(attribute.DataFormatString);
            Assert.IsNull(attribute.NullDisplayText);
        }
    }
}
