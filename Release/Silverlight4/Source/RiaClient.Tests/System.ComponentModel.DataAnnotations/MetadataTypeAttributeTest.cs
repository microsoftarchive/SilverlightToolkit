// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

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
