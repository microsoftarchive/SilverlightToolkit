// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if !SILVERLIGHT
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

        // Dev10 Bug 722118: Inheritance is broken with the AssociatedMetadataTypeTypeDescription provider
        [TestMethod]
        public void AddProviderToBaseType_GetPropertiesOfDerivedType() {
            // Before adding TDP
            var props = TypeDescriptor.GetProperties(typeof(Base)).Cast<PropertyDescriptor>().
                Select(p => p.Name).ToArray();
            CollectionAssert.AreEquivalent(new[] { "A", "B" }, props);

            props = TypeDescriptor.GetProperties(typeof(Derived)).Cast<PropertyDescriptor>().
                Select(p => p.Name).ToArray();
            CollectionAssert.AreEquivalent(new[] { "A", "B", "C" }, props);

            TypeDescriptionProvider provider = null;
            try {
                provider = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(Base));
                TypeDescriptor.AddProvider(provider, typeof(Base));

                // After adding TDP
                props = TypeDescriptor.GetProperties(typeof(Base)).Cast<PropertyDescriptor>().
                    Select(p => p.Name).ToArray();
                CollectionAssert.AreEquivalent(new[] { "A", "B" }, props);

                props = TypeDescriptor.GetProperties(typeof(Derived)).Cast<PropertyDescriptor>().
                    Select(p => p.Name).ToArray();
                CollectionAssert.AreEquivalent(new[] { "A", "B", "C" }, props);
            }
            finally {
                // Ensure we remove the provider we attached.
                TypeDescriptor.RemoveProvider(provider, typeof(Base));
            }
        }

        [TestMethod]
        public void AddProviderToTypeAndInstance() {
            var instance = new Base();

            // Before adding TDPs
            var props = TypeDescriptor.GetProperties(instance).Cast<PropertyDescriptor>().
                Select(p => p.Name).ToArray();
            CollectionAssert.AreEquivalent(new[] { "A", "B" }, props);

            TypeDescriptionProvider typeProvider = null;
            TypeDescriptionProvider instanceProvider = null;
            try {
                typeProvider = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(Base));
                TypeDescriptor.AddProvider(typeProvider, typeof(Base));

                instanceProvider = new TestTypeDescriptionProvider(instance);
                TypeDescriptor.AddProvider(instanceProvider, instance);

                // After adding TDPs
                props = TypeDescriptor.GetProperties(instance).Cast<PropertyDescriptor>().
                    Select(p => p.Name).ToArray();
                CollectionAssert.AreEquivalent(new[] { "A", "B", "CustomProperty1", "CustomProperty2" }, props);
            }
            finally {
                // Ensure we remove the providers we attached.
                TypeDescriptor.RemoveProvider(typeProvider, typeof(Base));
                TypeDescriptor.RemoveProvider(instanceProvider, instance);
            }
        }

        [TestMethod]
        public void AddMultipleProvidersToType() {
            // Before adding TDPs
            var props = TypeDescriptor.GetProperties(typeof(Base)).Cast<PropertyDescriptor>().
                Select(p => p.Name).ToArray();
            CollectionAssert.AreEquivalent(new[] { "A", "B" }, props);

            TypeDescriptionProvider associatedMetadataProvider = null;
            TypeDescriptionProvider testProvider = null;
            
            try {
                associatedMetadataProvider = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(Base));
                TypeDescriptor.AddProvider(associatedMetadataProvider, typeof(Base));
                testProvider = new TestTypeDescriptionProvider(typeof(Base));
                TypeDescriptor.AddProvider(testProvider, typeof(Base));

                // After adding TDPs
                props = TypeDescriptor.GetProperties(typeof(Base)).Cast<PropertyDescriptor>().
                    Select(p => p.Name).ToArray();
                CollectionAssert.AreEquivalent(new[] { "A", "B", "CustomProperty1", "CustomProperty2" }, props);
            }
            finally {
                // Ensure we remove the providers we attached.
                TypeDescriptor.RemoveProvider(associatedMetadataProvider, typeof(Base));
                TypeDescriptor.RemoveProvider(testProvider, typeof(Base));
            }

            // Add providers in reverse order
            try {
                testProvider = new TestTypeDescriptionProvider(typeof(Base));
                TypeDescriptor.AddProvider(testProvider, typeof(Base));
                associatedMetadataProvider = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(Base));
                TypeDescriptor.AddProvider(associatedMetadataProvider, typeof(Base));

                props = TypeDescriptor.GetProperties(typeof(Base)).Cast<PropertyDescriptor>().
                    Select(p => p.Name).ToArray();
                CollectionAssert.AreEquivalent(new[] { "A", "B", "CustomProperty1", "CustomProperty2" }, props);
            }
            finally {
                // Ensure we remove the providers we attached.
                TypeDescriptor.RemoveProvider(associatedMetadataProvider, typeof(Base));
                TypeDescriptor.RemoveProvider(testProvider, typeof(Base));
            }
        }

        [TestMethod]
        public void MetadataInheritance() {
            TypeDescriptionProvider baseProvider = null;
            TypeDescriptionProvider derivedProvider = null;
            TypeDescriptionProvider derivedNoMetadataProvider = null;
            try {
                baseProvider = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(Base));
                TypeDescriptor.AddProvider(baseProvider, typeof(Base));
                derivedProvider = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(Derived));
                TypeDescriptor.AddProvider(derivedProvider, typeof(Derived));
                derivedNoMetadataProvider = new AssociatedMetadataTypeTypeDescriptionProvider(typeof(DerivedNoMetadata));
                TypeDescriptor.AddProvider(derivedNoMetadataProvider, typeof(DerivedNoMetadata));

                var attrs = TypeDescriptor.GetProperties(typeof(Base))["A"].Attributes.Cast<Attribute>().
                    Select(a => a.GetType().Name).ToArray();
                CollectionAssert.IsSubsetOf(new[] { "DescriptionAttribute" }, attrs);

                attrs = TypeDescriptor.GetProperties(typeof(Derived))["A"].Attributes.Cast<Attribute>().
                    Select(a => a.GetType().Name).ToArray();
                CollectionAssert.IsSubsetOf(new[] { "DescriptionAttribute", "CategoryAttribute" }, attrs);

                attrs = TypeDescriptor.GetProperties(typeof(DerivedNoMetadata))["A"].Attributes.Cast<Attribute>().
                    Select(a => a.GetType().Name).ToArray();
                CollectionAssert.IsSubsetOf(new[] { "DescriptionAttribute" }, attrs);
            }
            finally {
                // Ensure we remove the providers we attached.
                TypeDescriptor.RemoveProvider(baseProvider, typeof(Base));
                TypeDescriptor.RemoveProvider(derivedProvider, typeof(Derived));
                TypeDescriptor.RemoveProvider(derivedNoMetadataProvider, typeof(DerivedNoMetadata));
            }
        }

        [MetadataType(typeof(BaseMetadata))]
        private class Base {
            public int A { get; set; }
            public int B { get; set; }
        }

        [MetadataType(typeof(DerivedMetadata))]
        private class Derived : Base {
            public int C { get; set; }
        }

        private class DerivedNoMetadata : Base {
            public int D { get; set; }
        }

        private class BaseMetadata {
            [Description("TestDescription")]
            public virtual int A { get; set; }
        }

        private class DerivedMetadata : BaseMetadata {
            [Category("TestCategory")]
            public override int A { get; set; }
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

        private class TestTypeDescriptionProvider : TypeDescriptionProvider {
            private TypeDescriptionProvider _baseProvider;

            public TestTypeDescriptionProvider(object instance) {
                _baseProvider = TypeDescriptor.GetProvider(instance);
            }

            public TestTypeDescriptionProvider(Type type) {
                _baseProvider = TypeDescriptor.GetProvider(type);
            }

            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
                return new TestTypeDescriptor(_baseProvider.GetTypeDescriptor(objectType, instance));
            }

            private class TestTypeDescriptor : CustomTypeDescriptor {
                public TestTypeDescriptor(ICustomTypeDescriptor parent)
                    : base(parent) {
                }

                public override PropertyDescriptorCollection GetProperties() {
                    var baseProperties = base.GetProperties().Cast<PropertyDescriptor>();
                    var customProperties = new[] {
                        (new Mock<PropertyDescriptor>("CustomProperty1", null) { CallBase = true }).Object,
                        (new Mock<PropertyDescriptor>("CustomProperty2", null) { CallBase = true }).Object };
                    var allProperties = baseProperties.Concat(customProperties).ToArray();

                    return new PropertyDescriptorCollection(allProperties, true);
                }
            }
        }
    }
}
#endif
