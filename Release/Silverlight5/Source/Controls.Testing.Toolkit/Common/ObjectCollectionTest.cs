// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// ObjectCollection unit tests.
    /// </summary>
    [TestClass]
    public partial class ObjectCollectionTest : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the ObjectCollectionTest class.
        /// </summary>
        public ObjectCollectionTest()
            : base()
        {
        }

        /// <summary>
        /// Create a new ObjectCollection.
        /// </summary>
        [TestMethod]
        [Description("Create a new ObjectCollection.")]
        public virtual void CreateDefault()
        {
            ObjectCollection collection = new ObjectCollection();
            Assert.AreEqual(0, collection.Count, "Expected empty collection!");
        }

        /// <summary>
        /// Create a new ObjectCollection using an empty sequence.
        /// </summary>
        [TestMethod]
        [Description("Create a new ObjectCollection using an empty sequence.")]
        public virtual void CreateWithEmptySequence()
        {
            ObjectCollection collection = new ObjectCollection(new object[] { });
            Assert.AreEqual(0, collection.Count, "Expected empty collection!");
        }

        /// <summary>
        /// Create a new ObjectCollection using a sequence of value types.
        /// </summary>
        [TestMethod]
        [Description("Create a new ObjectCollection using a sequence of value types.")]
        public virtual void CreateWithValueTypes()
        {
            int[] sequence = new int[] { 1, 2, 3 };
            ObjectCollection collection = new ObjectCollection(sequence);
            Assert.AreEqual(sequence.Length, collection.Count, "Collection should be the same size as the sequence!");
            for (int i = 0; i < sequence.Length; i++)
            {
                Assert.AreEqual(sequence[i], collection[i], "collection[{0}] does not match sequence[{0}]!", i);
            }
        }

        /// <summary>
        /// Create a new ObjectCollection using a sequence of reference types.
        /// </summary>
        [TestMethod]
        [Description("Create a new ObjectCollection using a sequence of reference types.")]
        public virtual void CreateWithReferenceTypes()
        {
            string[] sequence = new string[] { "Hello", "World" };
            ObjectCollection collection = new ObjectCollection(sequence);
            Assert.AreEqual(sequence.Length, collection.Count, "Collection should be the same size as the sequence!");
            for (int i = 0; i < sequence.Length; i++)
            {
                Assert.AreEqual(sequence[i], collection[i], "collection[{0}] does not match sequence[{0}]!", i);
            }
        }
    }
}