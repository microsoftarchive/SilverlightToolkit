// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if !SILVERLIGHT
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class AssociatedMetadataTypeTypeDescriptionProviderTest {

        [TestMethod]
        public void ConstructorAndGetTypeDescriptor() {
            AssociatedMetadataTypeTypeDescriptionProvider provider1 = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(TestTable));
            Assert.IsTrue(provider1.GetTypeDescriptor(typeof(TestTable)).GetProperties().Find("Number", true).Attributes.ContainsEquivalent(new RangeAttribute(0, 10)));

            AssociatedMetadataTypeTypeDescriptionProvider provider2 = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(TestTable), typeof(TestTable_MetadataAlternative));
            Assert.IsTrue(provider2.GetTypeDescriptor(typeof(TestTable)).GetProperties().Find("Number", true).Attributes.ContainsEquivalent(new RequiredAttribute()));
        }

        [MetadataType(typeof(TestTable_Metadata))]
        class TestTable {
            public int Number { get; set; }
        }

        class TestTable_Metadata {
            [Range(0, 10)]
            public object Number { get; set; }
        }

        class TestTable_MetadataAlternative {
            [Required]
            public object Number { get; set; }
        }
    }
}
#endif
