// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    [TestClass]
    public class AttributeUsageTest {
        [TestMethod]
        public void AttributeUsage() {
            Dictionary<Type, AttributeUsageAttribute> expectedUsage = new Dictionary<Type, AttributeUsageAttribute>();

#if !SILVERLIGHT
            expectedUsage.Add<MetadataTypeAttribute>(AttributeTargets.Class, false);

            expectedUsage.Add<ScaffoldColumnAttribute>(AttributeTargets.Field | AttributeTargets.Property, false);
            expectedUsage.Add<ScaffoldTableAttribute>(AttributeTargets.Class, false);
#endif

            expectedUsage.Add<UIHintAttribute>(AttributeTargets.Field | AttributeTargets.Property, true);
            expectedUsage.Add<FilterUIHintAttribute>(AttributeTargets.Field | AttributeTargets.Property, false);
            expectedUsage.Add<DataTypeAttribute>(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Parameter, false);
            expectedUsage.Add<EnumDataTypeAttribute>(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter, false);

            expectedUsage.Add<DisplayFormatAttribute>(AttributeTargets.Field | AttributeTargets.Property, false);
            expectedUsage.Add<DisplayColumnAttribute>(AttributeTargets.Class, false);

            expectedUsage.Add<RangeAttribute>(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, false);
            expectedUsage.Add<RegularExpressionAttribute>(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, false);
            expectedUsage.Add<RequiredAttribute>(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, false);
            expectedUsage.Add<StringLengthAttribute>(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, false);

            expectedUsage.Add<AssociationAttribute>(AttributeTargets.Field | AttributeTargets.Property, false);
            expectedUsage.Add<ConcurrencyCheckAttribute>(AttributeTargets.Field | AttributeTargets.Property, false);
            expectedUsage.Add<TimestampAttribute>(AttributeTargets.Field | AttributeTargets.Property, false);
            expectedUsage.Add<CustomValidationAttribute>(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter, true);
            expectedUsage.Add<DisplayAttribute>(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Parameter, false);
            expectedUsage.Add<KeyAttribute>(AttributeTargets.Property | AttributeTargets.Field, false);
            expectedUsage.Add<EditableAttribute>(AttributeTargets.Property | AttributeTargets.Field, false);

            // check if we have expectations for all instantiable attribute types in the assembly
            var untestedList = DerivedInAssembly<Attribute>(typeof(KeyAttribute).Assembly).Where(t => !t.IsAbstract && !expectedUsage.ContainsKey(t)).ToList();
            if (untestedList.Count > 0) {
                Assert.Fail("The test is not validating the following attributes:\r\n\t" + String.Join("\r\n\t", untestedList.Select(t => t.FullName).ToArray()));
            }

            // validate expectations
            foreach (var pair in expectedUsage) {
                DataAnnotationsTestHelper.TestAttributeUsage(pair.Key, pair.Value.ValidOn, pair.Value.AllowMultiple);
            }
        }

        private IEnumerable<Type> DerivedInAssembly<T>(Assembly assembly) where T : class {
            Type baseType = typeof(T);
            foreach (Type t in assembly.GetTypes()) {
                if (t.IsSubclassOf(baseType)) {
                    yield return t;
                }
            }
        }
    }

    internal static class AttributeUsageTestExtensions {
        public static void Add<T>(this Dictionary<Type, AttributeUsageAttribute> d, AttributeTargets targets, bool allowMultiple) where T : Attribute {
            Type t = typeof(T);
            if (d.ContainsKey(t)) {
                throw new InvalidOperationException(String.Format("The expectations dictionary already contains the type {0}", t.FullName));
            }
            d[t] = new AttributeUsageAttribute(targets) { AllowMultiple = allowMultiple };
        }
    }
}
