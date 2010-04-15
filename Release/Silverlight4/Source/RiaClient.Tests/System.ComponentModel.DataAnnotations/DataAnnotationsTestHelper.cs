// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.DataAnnotations.Test {
    internal class DataAnnotationsTestHelper {
        public static TAttribute GetAttribute<TAttribute, T>() where TAttribute : Attribute {
            return GetAttribute<TAttribute>(typeof(T));
        }

        public static TAttribute GetAttribute<TAttribute>(Type t) where TAttribute : Attribute {
            return t.GetCustomAttributes(typeof(TAttribute), false).OfType<TAttribute>().Single();
        }

        public static TAttribute[] GetAttributes<TAttribute, T>() where TAttribute : Attribute {
            return typeof(T).GetCustomAttributes(typeof(TAttribute), false).OfType<TAttribute>().ToArray();
        }

        public static void TestAttributeUsage<T>(AttributeTargets targets, bool allowMultiple) where T : Attribute {
            TestAttributeUsage(typeof(T), targets, allowMultiple);
        }

        public static void TestAttributeUsage(Type t, AttributeTargets targets, bool allowMultiple) {
            var attribute = GetAttribute<AttributeUsageAttribute>(t);

            Assert.AreEqual<AttributeTargets>(targets, attribute.ValidOn, t.Name);
            Assert.AreEqual<bool>(allowMultiple, attribute.AllowMultiple, t.Name);
        }
    }
}
