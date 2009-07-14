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
