//-----------------------------------------------------------------------
// <copyright file="GroupingTests.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class runs the unit tests for grouping items in a PagedCollectionView
    /// </summary>
    public class GroupingTests : PagedCollectionViewTest
    {
        /// <summary>
        /// Set a group descriptor and verify that items are grouped by that property.
        /// </summary>
        [TestMethod]
        [Description("Set a group descriptor and verify that items are grouped by that property.")]
        public void BasicGroupingTest()
        {
            Assert.IsNull(CollectionView.Groups);
            
            // check the order of items before we group
            for (int i = 0; i < 25; i++)
            {
                string value = (i % 2 == 0) ? "A" : "B";
                Assert.AreEqual(value, (CollectionView[i] as TestClass).StringProperty);
            }

            // set group description and verify that the items get grouped in this order
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

            Assert.IsNotNull(CollectionView.Groups);
            Assert.AreEqual(2, CollectionView.Groups.Count);
            Assert.AreEqual("A", (CollectionView.Groups[0] as CollectionViewGroup).Name);
            Assert.AreEqual(13, (CollectionView.Groups[0] as CollectionViewGroup).ItemCount);
            Assert.AreEqual("B", (CollectionView.Groups[1] as CollectionViewGroup).Name);
            Assert.AreEqual(12, (CollectionView.Groups[1] as CollectionViewGroup).ItemCount);

            for (int i = 0; i < 25; i++)
            {
                string value = (i < 13) ? "A" : "B";
                Assert.AreEqual(value, (CollectionView[i] as TestClass).StringProperty);
            }
        }

        /// <summary>
        /// Verify that our CanGroup property indicates that we support grouping.
        /// </summary>
        [TestMethod]
        [Description("Verify that our CanGroup property indicates that we support grouping.")]
        public void CanGroupTest()
        {
            Assert.IsTrue(CollectionView.CanGroup);
        }

        /// <summary>
        /// Validate the Groups property when no GroupDescriptions are specified.
        /// </summary>
        [TestMethod]
        [Description("Validate the Groups property when no GroupDescriptions are specified.")]
        public void EmptyGroupsTest()
        {
            Assert.AreEqual(0, CollectionView.GroupDescriptions.Count);
            Assert.IsNull(CollectionView.Groups);
        }

        /// <summary>
        /// Validate the Enumerator from the Groups property on the PagedCollectionView class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Enumerator from the Groups property on the PagedCollectionView class.")]
        public void GroupsEnumeratorTest()
        {
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

            // Test the Enumerator on the Groups
            IEnumerator groupEnumerator = CollectionView.GetEnumerator();

            int count = 0;
            while (groupEnumerator.MoveNext())
            {
                Assert.AreEqual(CollectionView[count], groupEnumerator.Current);
                count++;
            }
        }

        /// <summary>
        /// Verify that when we add items, they are placed into the correct groups.
        /// </summary>
        [TestMethod]
        [Description("Verify that when we add items, they are placed into the correct groups.")]
        public void GroupingWithAddTest()
        {
            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // set group description and verify that the items get grouped in this order
                CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

                // currently there should be groups "A" and "B"
                Assert.AreEqual(2, CollectionView.Groups.Count);

                // insert a new item and verify that it gets placed into the right group
                TestClass newItem = CollectionView.AddNew() as TestClass;
                newItem.StringProperty = "A";
                CollectionView.CommitNew();


                Assert.AreEqual(2, CollectionView.Groups.Count);
                Assert.AreEqual(13, CollectionView.IndexOf(newItem));
                Assert.IsTrue((CollectionView.Groups[0] as CollectionViewGroup).Items.Contains(newItem));
            }
        }

        /// <summary>
        /// Set a group descriptor and paging and verify that items are grouped correctly and then paged.
        /// </summary>
        [TestMethod]
        [Description("Set a group descriptor and paging and verify that items are grouped correctly and then paged.")]
        public void GroupingWithPagingTest()
        {
            // set group description and verify that the items get grouped in this order
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
            CollectionView.PageSize = 5;

            // for each page
            for (int i = 0; i < 5; i++)
            {
                CollectionView.MoveToPage(i);

                for (int j = 0; j < CollectionView.Count; j++)
                {
                    Assert.AreEqual(i+1, (CollectionView[j] as TestClass).IntProperty);
                }
            }
        }

        /// <summary>
        /// Set a group descriptor and sort descriptor and verify that items are sorted before we group.
        /// </summary>
        [TestMethod]
        [Description("Set a group descriptor and sort descriptor and verify that items are sorted before we group.")]
        public void GroupingWithSortingTest()
        {
            // set sort and group description and verify that the items get grouped in this order
            CollectionView.SortDescriptions.Add(new SortDescription("StringProperty", ListSortDirection.Descending));
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

            // because we set a descending order on the property, "B" should come before "A"
            Assert.AreEqual(2, CollectionView.Groups.Count);
            Assert.AreEqual("B", (CollectionView.Groups[0] as CollectionViewGroup).Name);
            Assert.AreEqual(12, (CollectionView.Groups[0] as CollectionViewGroup).ItemCount);
            Assert.AreEqual("A", (CollectionView.Groups[1] as CollectionViewGroup).Name);
            Assert.AreEqual(13, (CollectionView.Groups[1] as CollectionViewGroup).ItemCount);

            for (int i = 0; i < 25; i++)
            {
                string value = (i < 12) ? "B" : "A";
                Assert.AreEqual(value, (CollectionView[i] as TestClass).StringProperty);
            }
        }

        /// <summary>
        /// Verify that when we add items that don't have an associated group, one is created, and that when all items are removed from a group, it is removed.
        /// </summary>
        [TestMethod]
        [Description("Verify that when we add items that don't have an associated group, one is created, and that when all items are removed from a group, it is removed.")]
        public void InsertAndRemoveGroupsTest()
        {
            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // set group description and verify that the items get grouped in this order
                CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));

                // currently there should be groups "A" and "B"
                Assert.AreEqual(2, CollectionView.Groups.Count);

                // insert a new item and verify that a new group is created
                TestClass newItem = CollectionView.AddNew() as TestClass;
                newItem.StringProperty = "C";
                CollectionView.CommitNew();

                // verify that we now have an additional group and that it appears at the end
                Assert.AreEqual(3, CollectionView.Groups.Count);
                Assert.AreEqual(25, CollectionView.IndexOf(newItem));

                // now remove that item (since it is the only one in its group) and verify that
                // the group is removed
                CollectionView.Remove(newItem);
                Assert.AreEqual(2, CollectionView.Groups.Count);
                Assert.IsFalse(CollectionView.Contains(newItem));
            }
        }

        /// <summary>
        /// Set multiple group descriptors and verify that items are grouped correctly.
        /// </summary>
        [TestMethod]
        [Description("Set multiple group descriptors and verify that items are grouped correctly.")]
        public void MultipleGroupsTest()
        {
            Assert.IsNull(CollectionView.Groups);

            // check the order of items before we group
            for (int i = 0; i < 25; i++)
            {
                string value = (i % 2 == 0) ? "A" : "B";
                Assert.AreEqual(value, (CollectionView[i] as TestClass).StringProperty);
            }

            // set group description and verify that the items get grouped in this order
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));

            Assert.IsNotNull(CollectionView.Groups);
            Assert.AreEqual(2, CollectionView.Groups.Count);
            Assert.AreEqual("A", (CollectionView.Groups[0] as CollectionViewGroup).Name);
            Assert.AreEqual(13, (CollectionView.Groups[0] as CollectionViewGroup).ItemCount);
            Assert.AreEqual("B", (CollectionView.Groups[1] as CollectionViewGroup).Name);
            Assert.AreEqual(12, (CollectionView.Groups[1] as CollectionViewGroup).ItemCount);

            // first verify that within the first level of grouping, that we are in order
            for (int i = 0; i < 25; i++)
            {
                string value = (i < 13) ? "A" : "B";
                Assert.AreEqual(value, (CollectionView[i] as TestClass).StringProperty);
            }

            // this list will keep track of the group names that already appeared
            List<int> intList = new List<int>();
            
            // first we will check the subgroups under "A"
            for (int i = 0; i < 13; i++)
            {
                int value = (CollectionView[i] as TestClass).IntProperty;
                if (intList.Count == 0 || !intList.Contains(value))
                {
                    intList.Add(value);
                }

                // we check to see that the current items value will be in grouped order by ensuring that 
                // it belongs to the last group in this list. if not, it shows that multiple groups exist for
                // the same value.
                Assert.AreEqual(intList.Count - 1, intList.IndexOf(value), "Items are not grouped in the correct order");
            }

            // next we will check the subgroups under "B"
            intList.Clear();
            for (int i = 0; i < 13; i++)
            {
                int value = (CollectionView[i] as TestClass).IntProperty;
                if (intList.Count == 0 || !intList.Contains(value))
                {
                    intList.Add(value);
                }

                // we check to see that the current items value will be in grouped order by ensuring that 
                // it belongs to the last group in this list. if not, it shows that multiple groups exist for
                // the same value.
                Assert.AreEqual(intList.Count - 1, intList.IndexOf(value), "Items are not grouped in the correct order");
            }
        }

        /// <summary>
        /// Verify that we support grouping on nested properties.
        /// </summary>
        [TestMethod]
        [Description("Verify that we support grouping on nested properties.")]
        public void NestedPropertyPathGroupingTest()
        {
            // this list will keep track of the group names that already appeared
            List<int> intList = new List<int>();

            // set nested group description and verify that the items get grouped in this order
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("InnerClassProperty.InnerIntProperty"));
            for (int i = 0; i < CollectionView.Count; i++)
            {
                int value = (CollectionView[i] as TestClass).InnerClassProperty.InnerIntProperty;
                if (intList.Count == 0 || !intList.Contains(value))
                {
                    intList.Add(value);
                }

                // we check to see that the current items value will be in grouped order by ensuring that 
                // it belongs to the last group in this list. if not, it shows that multiple groups exist for
                // the same value.
                Assert.AreEqual(intList.Count-1, intList.IndexOf(value), "Items are not grouped in the correct order");
            }
        }
    }
}
