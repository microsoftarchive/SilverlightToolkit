#if !SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class MetadataTypeAttributeTest {
        [TestMethod]
        public void Constructor_NullValue() {
            MetadataTypeAttribute attribute = new MetadataTypeAttribute(null);
            ExceptionHelper.ExpectInvalidOperationException(() => {
                Type classType = attribute.MetadataClassType;
            }, Resources.DataAnnotationsResources.MetadataTypeAttribute_TypeCannotBeNull);
        }
    }
}
#endif
