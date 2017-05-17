// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.DataVisualization;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// TreeMapItemDefinitionTest unit tests.
    /// </summary>
    [TestClass]
    [Tag("TreeMap")]
    public class TreeMapItemDefinitionTest : TestBase
    {
        /// <summary>
        /// Delta used for double precisions checks.
        /// </summary>
        private const double Delta = 0.000001;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeMapItemDefinitionTest"/> class. 
        /// </summary>
        public TreeMapItemDefinitionTest()
        {
        }

        /// <summary>
        /// Verifies that ValuePath works.
        /// </summary>
        [TestMethod]
        [Description("Verifies that ValuePath works.")]
        [Bug("706249: Implement TreeMap API review changes", Fixed = true)]
        public virtual void ValuePathTest()
        {
            const string BindingName = "Foo";
            TreeMapItemDefinition testItemDef = new TreeMapItemDefinition();

            Assert.IsNull(testItemDef.ValueBinding);
            Assert.IsNull(testItemDef.ValuePath);

            testItemDef.ValuePath = BindingName;
            
            Assert.IsNotNull(testItemDef.ValueBinding);
            Assert.AreEqual(BindingName, testItemDef.ValueBinding.Path.Path);

            testItemDef.ValuePath = null;

            Assert.IsNull(testItemDef.ValueBinding);
            Assert.IsNull(testItemDef.ValuePath);
        }

        /// <summary>
        /// Verifies default ChildItemPadding.
        /// </summary>
        [TestMethod]
        [Description("Verifies that ValuePath works.")]
        [Bug("706249: Implement TreeMap API review changes", Fixed = true)]
        public virtual void ValueChildItemPadding()
        {
            TreeMapItemDefinition testItemDef = new TreeMapItemDefinition();

            Assert.IsNotNull(testItemDef.ChildItemPadding);
            Assert.AreEqual(0, testItemDef.ChildItemPadding.Left);
            Assert.AreEqual(0, testItemDef.ChildItemPadding.Top);
            Assert.AreEqual(0, testItemDef.ChildItemPadding.Right);
            Assert.AreEqual(0, testItemDef.ChildItemPadding.Bottom);
        }
    }
}
