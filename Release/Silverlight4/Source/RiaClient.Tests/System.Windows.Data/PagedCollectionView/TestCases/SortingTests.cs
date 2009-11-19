//-----------------------------------------------------------------------
// <copyright file="SortingTests.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Text;
    using System.Windows.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class runs the unit tests for sorting items in a PagedCollectionView
    /// </summary>
    public class SortingTests : PagedCollectionViewTest
    {
        /// <summary>
        /// Set SortDescriptions and verify that items get correctly sorted.
        /// </summary>
        [TestMethod]
        [Description("Set SortDescriptions and verify that items get correctly sorted.")]
        public void BasicSortingTest()
        {
            // assert that the sort descriptions are not null, but empty at startup
            Assert.IsNotNull(CollectionView.SortDescriptions);
            Assert.IsTrue(CollectionView.SortDescriptions.Count == 0);

            // set sort description and verify that the items get sorted in this order
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            for (int i = 1; i < CollectionView.Count; i++)
            {
                Assert.IsTrue((CollectionView[i - 1] as TestClass).IntProperty <= 
                    (CollectionView[i] as TestClass).IntProperty);
            }

            // now set the same sort description in the opposite direction 
            // and verify that the items are now sorted in reverse order
            CollectionView.SortDescriptions.Clear();
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Descending));
            for (int i = 1; i < CollectionView.Count; i++)
            {
                Assert.IsTrue((CollectionView[i - 1] as TestClass).IntProperty >=
                    (CollectionView[i] as TestClass).IntProperty);
            }
        }

        /// <summary>
        /// Verify that we support sorting.
        /// </summary>
        [TestMethod]
        [Description("Verify that we support sorting.")]
        public void CanSortTest()
        {
            Assert.IsTrue(CollectionView.CanSort);
        }

        /// <summary>
        /// Verify that when IsDataSorted is set to true, we do not apply sorting.
        /// </summary>
        [TestMethod]
        [Description("Verify that when IsDataSorted is set to true, we do not apply sorting.")]
        public void IsDataSortedTest()
        {
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < CollectionView.Count; i++)
            {
                sb.Append((CollectionView[i] as TestClass).StringProperty);
            }

            string beforeSort = sb.ToString();

            // first verify that without this flag, that we will get sorting
            CollectionView.SortDescriptions.Add(new SortDescription("StringProperty", ListSortDirection.Ascending));

            sb = new StringBuilder();
            for (int i = 0; i < CollectionView.Count; i++)
            {
                sb.Append((CollectionView[i] as TestClass).StringProperty);
            }

            // sorting should get applied so the strings should be different
            Assert.AreNotEqual(beforeSort, sb.ToString());

            // re-create the CollectionView with the IsDataSorted flag set
            CollectionView = new PagedCollectionView(this.SourceCollection, true /*isDataSorted*/, false);

            // now sort again and verify that the sort did not get applied due to the flag being set
            CollectionView.SortDescriptions.Add(new SortDescription("StringProperty", ListSortDirection.Ascending));

            sb = new StringBuilder();
            for (int i = 0; i < CollectionView.Count; i++)
            {
                sb.Append((CollectionView[i] as TestClass).StringProperty);
            }

            // sorting should not get applied so the strings should be the same
            Assert.AreEqual(beforeSort, sb.ToString());
        }

        /// <summary>
        /// Verify that items respect the filter when sorting.
        /// </summary>
        [TestMethod]
        [Description("Verify that items respect the filter when sorting.")]
        public void FilterWithSortingTest()
        {
            Assert.AreEqual(25, CollectionView.Count);

            // set sort description and verify that the items get sorted in this order
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            for (int i = 1; i < CollectionView.Count; i++)
            {
                Assert.IsTrue((CollectionView[i - 1] as TestClass).IntProperty <=
                    (CollectionView[i] as TestClass).IntProperty);
            }
            Assert.AreEqual(25, CollectionView.Count);

            // now add a filter and verify that items have been filtered out
            // but are still in sort order
            CollectionView.Filter = this.FilterOutOnes;
            for (int i = 1; i < CollectionView.Count; i++)
            {
                Assert.IsTrue((CollectionView[i - 1] as TestClass).IntProperty <=
                    (CollectionView[i] as TestClass).IntProperty);
            }
            Assert.AreEqual(20, CollectionView.Count);
        }

        /// <summary>
        /// Verify that items within groups are sorted.
        /// </summary>
        [TestMethod]
        [Description("Verify that items within groups are sorted.")]
        public void GroupWithSortingTest()
        {
            // add grouping and sorting
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));

            // check that we have 2 different groups ("A" : "B")
            Assert.AreEqual(2, CollectionView.Groups.Count);

            for (int i = 0; i < CollectionView.Groups.Count; i++)
            {
                for (int j = 1; j < (CollectionView.Groups[i] as CollectionViewGroup).Items.Count; j++)
                {
                    Assert.IsTrue(((CollectionView.Groups[i] as CollectionViewGroup).Items[j - 1] as TestClass).IntProperty <=
                        ((CollectionView.Groups[i] as CollectionViewGroup).Items[j] as TestClass).IntProperty);
                }
            }
        }

        /// <summary>
        /// Verify that we support sorting on nested properties.
        /// </summary>
        [TestMethod]
        [Description("Verify that we support sorting on nested properties.")]
        public void NestedSortingTest()
        {
            // set nested sort description and verify that the items get sorted in this order
            CollectionView.SortDescriptions.Add(new SortDescription("InnerClassProperty.InnerIntProperty", ListSortDirection.Ascending));
            for (int i = 1; i < CollectionView.Count; i++)
            {
                Assert.IsTrue((CollectionView[i - 1] as TestClass).InnerClassProperty.InnerIntProperty <=
                    (CollectionView[i] as TestClass).InnerClassProperty.InnerIntProperty);
            }

            // now set the same sort description in the opposite direction 
            // and verify that the items are now sorted in reverse order
            CollectionView.SortDescriptions.Clear();
            CollectionView.SortDescriptions.Add(new SortDescription("InnerClassProperty.InnerIntProperty", ListSortDirection.Descending));
            for (int i = 1; i < CollectionView.Count; i++)
            {
                Assert.IsTrue((CollectionView[i - 1] as TestClass).InnerClassProperty.InnerIntProperty >=
                    (CollectionView[i] as TestClass).InnerClassProperty.InnerIntProperty);
            }
        }

        /// <summary>
        /// Verify that when we remove and insert a sort description, that we update the data correctly.
        /// </summary>
        [TestMethod]
        [Description("Verify that when we remove and insert a sort description, that we update the data correctly.")]
        public void RemoveAndInsertSortDescriptionsTest()
        {
            // add groups and sorts
            using (CollectionView.DeferRefresh())
            {
                CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));
                CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
                CollectionView.SortDescriptions.Add(new SortDescription("StringProperty", ListSortDirection.Ascending));
                CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            }

            // verify that we also get the correct number of groups
            Assert.AreEqual(2, CollectionView.Groups.Count);

            // change the direction of the first sort
            CollectionView.SortDescriptions.RemoveAt(0);
            CollectionView.SortDescriptions.Add(new SortDescription("StringProperty", ListSortDirection.Descending));

            // also verify that we still get the correct number of groups
            Assert.AreEqual(2, CollectionView.Groups.Count);
        }
    }
}
