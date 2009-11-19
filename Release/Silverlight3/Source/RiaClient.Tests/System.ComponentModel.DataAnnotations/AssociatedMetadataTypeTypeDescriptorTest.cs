// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if !SILVERLIGHT
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class AssociatedMetadataTypeTypeDescriptorTest {
        private Dictionary<Type, ICustomTypeDescriptor> typeDescriptionProviders = new Dictionary<Type, ICustomTypeDescriptor>();

        [TestInitialize]
        public void Setup() {
            Type[] types = new Type[] {
                typeof(TestTable),
                typeof(TestTableWithExtraMetadata),
                typeof(TestTableWithMetadata),
                typeof(TestTableWithExtraMetadataConflict),
                typeof(FieldTestTable),
            };

            foreach (Type type in types) {
                typeDescriptionProviders[type] = CreateAssociatedTypeDescriptor(type);
            }
        }

        [TestCleanup]
        public void TearDown() {
            typeDescriptionProviders.Clear();
        }

        #region HELPER METHODS

        private ICustomTypeDescriptor GetTypeDescriptorHelper(Type type) {
            return typeDescriptionProviders[type];
        }

        private AssociatedMetadataTypeTypeDescriptor CreateAssociatedTypeDescriptor(Type type) {
            return new AssociatedMetadataTypeTypeDescriptor(TypeDescriptor.GetProvider(type).GetTypeDescriptor(type), type);
        }

        #endregion

        [TestMethod]
        public void RetrievePropertyAttributesNoExtraMetadata() {
            var normalPropAttributes = GetTypeDescriptorHelper(typeof(TestTable)).GetProperties().Find("PropertyNormal", true).Attributes;
            Assert.AreEqual<int>(3, normalPropAttributes.Count);
        }

        [TestMethod]
        public void RetrievePropertyAttributesWithExtraMetadata() {
            var extraPropAttributes = GetTypeDescriptorHelper(typeof(TestTable)).GetProperties().Find("PropertyWithExtraMetadata", true).Attributes;
            Assert.AreEqual<int>(5, extraPropAttributes.Count);
            Assert.IsTrue(extraPropAttributes.ContainsEquivalent(new RangeAttribute(0, 10)));
            Assert.IsTrue(extraPropAttributes.ContainsEquivalent(new DescriptionAttribute()));
        }

        [TestMethod]
        public void RetrieveEntityAttributesNoExtraMetadata() {
            var attributes = GetTypeDescriptorHelper(typeof(TestTable)).GetAttributes();
            Assert.IsTrue(attributes.ContainsEquivalent(new MetadataTypeAttribute(typeof(TestTable_Metadata))));
            Assert.AreEqual<int>(1, attributes.Count);

            attributes = GetTypeDescriptorHelper(typeof(TestTableWithMetadata)).GetAttributes();
            Assert.IsTrue(attributes.ContainsEquivalent(new DisplayColumnAttribute("")));
            Assert.AreEqual<int>(1, attributes.Count);
        }

        [TestMethod]
        public void RetrieveEntityAttributesWithExtraMetadata() {
            var tableAttributes = GetTypeDescriptorHelper(typeof(TestTableWithExtraMetadata)).GetAttributes();
            Assert.IsTrue(tableAttributes.ContainsEquivalent(new DisplayColumnAttribute("")));
        }

        [TestMethod]
        public void InvalidAssociatedType() {
            ExceptionHelper.ExpectException<InvalidOperationException>(delegate() {
                new AssociatedMetadataTypeTypeDescriptor(null, typeof(TestTableInvalid));
            }, String.Format(CultureInfo.CurrentCulture, Resources.DataAnnotationsResources.AssociatedMetadataTypeTypeDescriptor_MetadataTypeContainsUnknownProperties, typeof(TestTableInvalid).FullName, "WrongPropertyName, AnotherWrongProperty, WrongFieldName, AnotherWrongFieldName"));
        }

        [TestMethod]
        public void FieldMetadataComingFromFieldsAndProperties() {
            var fieldMetadata = GetTypeDescriptorHelper(typeof(FieldTestTable)).GetProperties();

            var integerFieldMetadata = fieldMetadata.Find("Integer", false).Attributes;
            Assert.IsTrue(integerFieldMetadata.ContainsEquivalent(new RangeAttribute(1, 10)), "Attributes not copied from property");
            Assert.IsTrue(integerFieldMetadata.ContainsEquivalent(new ScaffoldColumnAttribute(true)), "Attributes not copied from property");

            var stringFieldMetadata = fieldMetadata.Find("String", false).Attributes;
            Assert.IsTrue(stringFieldMetadata.ContainsEquivalent(new RequiredAttribute()), "Attributes not copied from field");
            Assert.IsTrue(stringFieldMetadata.ContainsEquivalent(new DisplayFormatAttribute()), "Attributes not copied from field");

            var doubleFieldMetadata = fieldMetadata.Find("Double", false).Attributes;
            Assert.IsFalse(doubleFieldMetadata.ContainsEquivalent(new TestAttribute("")), "Attributes are being copied from method");
        }

        [MetadataType(typeof(TestTable_Metadata))]
        class TestTable {
            public int PropertyNormal { get; set; }
            public int PropertyWithExtraMetadata { get; set; }
        }

        #region DATA STRUCTURE MOCKS

        class TestTable_Metadata {
            [Range(0, 10)]
            [Description]
            public object PropertyWithExtraMetadata { get; set; }
        }

        [DisplayColumn("")]
        class TestTableWithMetadata { }

        [MetadataType(typeof(TestTableWithExtraMetadata_Metadata))]
        class TestTableWithExtraMetadata { }

        [DisplayColumn("")]
        class TestTableWithExtraMetadata_Metadata { }

        [MetadataType(typeof(TestTableWithExtraMetadataConflict_Metadata))]
        [DisplayColumn("conflict")]
        class TestTableWithExtraMetadataConflict { }

        [DisplayColumn("conflict")]
        class TestTableWithExtraMetadataConflict_Metadata { }

        [MetadataType(typeof(TestTableInvalid_Metadata))]
        class TestTableInvalid {
            public int ValidProperty { get; set; }
        }

        class TestTableInvalid_Metadata {
            public object WrongPropertyName { get; set; }
            public object AnotherWrongProperty { get; set; }
            public object WrongFieldName = null;
            public object AnotherWrongFieldName = null;
            public object OkMethodName() { return null; }
        }

        [MetadataType(typeof(FieldTestTable_Metadata))]
        class FieldTestTable {
            public int Integer { get; set; }
            public string String { get; set; }
            public double Double { get; set; }
        }

        class FieldTestTable_Metadata {
            [Range(1, 10)]
            [ScaffoldColumn(true)]
            public int Integer { get; set; }
            [Required]
            [DisplayFormat]
            public string String = null;

            [Test("")]
            public double Double() { return 0.0; }
        }

        class TestAttribute : Attribute {
            public string Text { get; private set; }
            public TestAttribute(string s) {
                Text = s;
            }
        }

        #endregion
    }

    public static class AttributeCollectionHelper {
        public static bool ContainsEquivalent(this AttributeCollection ac, Attribute a) {
            return ContainsEquivalentHelper(ac, a);
        }

        public static bool ContainsEquivalent<A>(this IEnumerable<A> ac, Attribute a) where A : Attribute {
            return ContainsEquivalentHelper(ac, a);
        }

        private static bool ContainsEquivalentHelper(IEnumerable ac, Attribute a) {
            foreach (Attribute attribute in ac) {
                // this works because the Equals method for attributes does equality comparisson by comparing all the properties of the attributes
                if (attribute.Equals(a))
                    return true;
            }
            return false;
        }
    }
}
#endif
