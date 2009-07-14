//-----------------------------------------------------------------------
// <copyright file="CollectionChangedTests.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Reflection;
    using System.Windows.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class runs the unit tests for processing the CollectionChanged event 
    /// from the source collection in a PagedCollectionView. These tests only
    /// apply for sources that implement INotifyCollectionChanged.
    /// </summary>
    public class CollectionChangedTests : PagedCollectionViewTest
    {
        /// <summary>
        /// Verify that adding an item in the source collection will correctly insert it into the CollectionView.
        /// </summary>
        [TestMethod]
        [Description("Verify that adding an item in the source collection will correctly insert it into the CollectionView.")]
        public void AddItemTest()
        {
            // verify that when we do not have sorting or grouping, when an item
            // is inserted, we will respect the index specified by the event.
            Assert.AreEqual(25, CollectionView.Count);
            MethodInfo mi = SourceCollection.GetType().GetMethod("Insert");

            TestClass newItem = new TestClass() {IntProperty = 6 };
            mi.Invoke(SourceCollection, new object[] { 5, newItem });
            Assert.AreEqual(5, CollectionView.IndexOf(newItem));
            Assert.AreEqual(26, CollectionView.Count);

            newItem = new TestClass() {IntProperty = 6 };
            mi.Invoke(SourceCollection, new object[] { 10, newItem });
            Assert.AreEqual(10, CollectionView.IndexOf(newItem));
            Assert.AreEqual(27, CollectionView.Count);

            // verify that once we add sorting, the items will be inserted into the correct index
            // instead of looking at the index from the event handler.
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));

            newItem = new TestClass() {IntProperty = 0 };
            mi.Invoke(SourceCollection, new object[] { 15, newItem });
            Assert.AreEqual(0, CollectionView.IndexOf(newItem));
            Assert.AreEqual(28, CollectionView.Count);

            newItem = new TestClass() { IntProperty = 1 };
            mi.Invoke(SourceCollection, new object[] { 20, newItem });
            Assert.AreEqual(2, CollectionView.IndexOf(newItem));
            Assert.AreEqual(29, CollectionView.Count);

            // verify that when we add grouping, the items will be inserted into
            // the correct groups
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));

            newItem = new TestClass() { IntProperty = 1 };
            mi.Invoke(SourceCollection, new object[] { 0, newItem });
            Assert.AreEqual(6, CollectionView.IndexOf(newItem));
            Assert.IsTrue((CollectionView.Groups[1] as CollectionViewGroup).Items.Contains(newItem));
            Assert.AreEqual(30, CollectionView.Count);

            newItem = new TestClass() { IntProperty = 2 };
            mi.Invoke(SourceCollection, new object[] { 0, newItem });
            Assert.AreEqual(10, CollectionView.IndexOf(newItem));
            Assert.IsTrue((CollectionView.Groups[2] as CollectionViewGroup).Items.Contains(newItem));
            Assert.AreEqual(31, CollectionView.Count);
        }

        /// <summary>
        /// Verify that if you add an item that doesn't pass the filter, it will not show in the CollectionView.
        /// </summary>
        [TestMethod]
        [Description("Verify that if you add an item that doesn't pass the filter, it will not show in the CollectionView.")]
        public void AddItemWithFilterTest()
        {
            CollectionView.Filter = this.FilterNegativeNumbers;
            Assert.AreEqual(25, CollectionView.Count);

            // verify that because of the filter, after adding the item,
            // the count will not change and the item won't be in the view.
            MethodInfo mi = SourceCollection.GetType().GetMethod("Insert");
            TestClass newItem = new TestClass() { IntProperty = -1 };
            mi.Invoke(SourceCollection, new object[] { 0, newItem });

            Assert.AreEqual(25, CollectionView.Count);
            Assert.IsFalse(CollectionView.Contains(newItem));

            // verify that when we have a pagesize set, we will have the same result
            // since the items should not be inserted into the collection
            CollectionView.PageSize = 30;
            newItem = new TestClass() { IntProperty = -1 };
            mi.Invoke(SourceCollection, new object[] { 0, newItem });
            Assert.AreEqual(25, CollectionView.Count);
            Assert.IsFalse(CollectionView.Contains(newItem));
        }

        /// <summary>
        /// Test adding an item in the source collection with paging in the CollectionView.
        /// </summary>
        [TestMethod]
        [Description("Test adding an item in the source collection with paging in the CollectionView.")]
        public void AddItemWithPagingTest()
        {
            // verify that when we do not have sorting or grouping, when an item
            // is inserted, we will respect the index specified by the event.
            CollectionView.PageSize = 5;
            Assert.AreEqual(5, CollectionView.Count);
            MethodInfo mi = SourceCollection.GetType().GetMethod("Insert");

            TestClass newItem = new TestClass() { IntProperty = 6 };
            mi.Invoke(SourceCollection, new object[] { 1, newItem });
            Assert.AreEqual(1, CollectionView.IndexOf(newItem));
            Assert.AreEqual(5, CollectionView.Count);

            // however, if that index is beyond what the current page is showing,
            // it will not be present in the view
            newItem = new TestClass() { IntProperty = 6 };
            mi.Invoke(SourceCollection, new object[] { 10, newItem });
            Assert.IsFalse(CollectionView.Contains(newItem));

            // now add sorting
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));

            // verify that if we insert into the current page, the last item will be
            // pushed off
            TestClass lastItem = CollectionView[4] as TestClass;
            Assert.IsTrue(CollectionView.Contains(lastItem));
            newItem = new TestClass() { IntProperty = 0 };
            mi.Invoke(SourceCollection, new object[] { 0, newItem });
            Assert.AreEqual(0, CollectionView.IndexOf(newItem));
            Assert.IsFalse(CollectionView.Contains(lastItem));

            // also in this case, if the sorting pushes the item to a different
            // page, it will not be displayed
            newItem = new TestClass() { IntProperty = 2 };
            mi.Invoke(SourceCollection, new object[] { 20, newItem });
            Assert.IsFalse(CollectionView.Contains(newItem));

            // now add grouping - because of paging, only two of the groups will 
            // be currently displayed (0 and 1).
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
            Assert.AreEqual(2, CollectionView.Groups.Count);

            // add an item in the first group and verify that the last item in the
            // second group gets pushed off
            newItem = new TestClass() { IntProperty = 0 };
            Assert.AreEqual(1, (CollectionView.Groups[0] as CollectionViewGroup).Items.Count);
            Assert.AreEqual(4, (CollectionView.Groups[1] as CollectionViewGroup).Items.Count);
            mi.Invoke(SourceCollection, new object[] { 0, newItem });
            Assert.AreEqual(2, (CollectionView.Groups[0] as CollectionViewGroup).Items.Count);
            Assert.AreEqual(3, (CollectionView.Groups[1] as CollectionViewGroup).Items.Count);

            // again add an item into another page beyond the current one and verify that
            // the items remain unchanged
            newItem = new TestClass() { IntProperty = 6 };
            Assert.AreEqual(2, (CollectionView.Groups[0] as CollectionViewGroup).Items.Count);
            Assert.AreEqual(3, (CollectionView.Groups[1] as CollectionViewGroup).Items.Count);
            mi.Invoke(SourceCollection, new object[] { 0, newItem });
            Assert.AreEqual(2, (CollectionView.Groups[0] as CollectionViewGroup).Items.Count);
            Assert.AreEqual(3, (CollectionView.Groups[1] as CollectionViewGroup).Items.Count);            

            // move to the last page and add an item in an earlier group to 
            // verify that an item gets pushed up into this page
            CollectionView.MoveToLastPage();
            Assert.AreEqual(1, (CollectionView.Groups[0] as CollectionViewGroup).Items.Count);
            newItem = new TestClass() { IntProperty = 2 };
            mi.Invoke(SourceCollection, new object[] { 0, newItem });
            Assert.AreEqual(2, (CollectionView.Groups[0] as CollectionViewGroup).Items.Count);
        }

        /// <summary>
        /// Verify that currency is correctly updated when adding items into the collection.
        /// </summary>
        [TestMethod]
        [Description("Verify that currency is correctly updated when adding items into the collection.")]
        public void CurrencyWithAddItemTest()
        {
            // set the current item to the last item in the collection
            TestClass item = CollectionView[24] as TestClass;
            CollectionView.MoveCurrentToLast();
            Assert.AreEqual(24, CollectionView.CurrentPosition);
            Assert.AreEqual(item, CollectionView.CurrentItem);

            // verify that when an item is inserted in an index after the 
            // current item, the current item does not change
            MethodInfo mi = SourceCollection.GetType().GetMethod("Insert");
            TestClass newItem = new TestClass() { IntProperty = 6 };
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            mi.Invoke(SourceCollection, new object[] { 25, newItem });
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(24, CollectionView.CurrentPosition);
            Assert.AreEqual(item, CollectionView.CurrentItem);

            // now verify that if we insert into an index before the current item
            // then because the current item shifts position, the currency events
            // are fired.
            newItem = new TestClass() { IntProperty = 6 };
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            mi.Invoke(SourceCollection, new object[] { 10, newItem });
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(25, CollectionView.CurrentPosition);
            Assert.AreEqual(item, CollectionView.CurrentItem);

            // now add grouping and verify that the items will be inserted into
            // the correct groups
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
            Assert.AreEqual(24, CollectionView.CurrentPosition);
            Assert.AreEqual(item, CollectionView.CurrentItem);

            // verify that adding an item in an earlier group would push the item and update
            // the current item
            newItem = new TestClass() { IntProperty = 1 }; 
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            mi.Invoke(SourceCollection, new object[] { 10, newItem });
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(25, CollectionView.CurrentPosition);
            Assert.AreEqual(item, CollectionView.CurrentItem);

            // now add paging and sorting. we should have updated the current item 
            // to the first item in the collection because we switched pages
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            CollectionView.PageSize = 5;
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.AreNotEqual(item, CollectionView.CurrentItem);

            // move currency to the last item on the page
            CollectionView.MoveCurrentToLast();

            // update our reference to "item" so that we point to the current item
            item = CollectionView[4] as TestClass;
            Assert.AreEqual(item, CollectionView.CurrentItem);

            // now add an item that would push the current item off the page
            newItem = new TestClass() { IntProperty = 0 };
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            mi.Invoke(SourceCollection, new object[] { 10, newItem });
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(4, CollectionView.CurrentPosition);
            Assert.AreNotEqual(item, CollectionView.CurrentItem);

            // now set currency to the first item and add an item that 
            // will not change currency
            CollectionView.PageSize = 10;
            CollectionView.MoveCurrentToFirst();
            newItem = new TestClass() { IntProperty = 1 };
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            mi.Invoke(SourceCollection, new object[] { 10, newItem });
            Assert.AreEqual(0, _expectedEventQueue.Count);
        }

        /// <summary>
        /// Verify that currency is correctly updated when removing items from the collection.
        /// </summary>
        [TestMethod]
        [Description("Verify that currency is correctly updated when removing items from the collection.")]
        public void CurrencyWithRemoveItemTest()
        {
            // set the current item to item #10 in the collection
            TestClass item = CollectionView[10] as TestClass;
            CollectionView.MoveCurrentToPosition(10);
            Assert.AreEqual(10, CollectionView.CurrentPosition);
            Assert.AreEqual(item, CollectionView.CurrentItem);

            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // verify that when an item is removed from an index after the 
                // current item, the current item does not change
                _expectedEventQueue.Clear();
                _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
                CollectionView.RemoveAt(24);
                Assert.AreEqual(0, _expectedEventQueue.Count);
                Assert.AreEqual(10, CollectionView.CurrentPosition);
                Assert.AreEqual(item, CollectionView.CurrentItem);

                // now verify that if we remove from an index before the current item
                // then because the current item shifts position, the currency events
                // are fired.
                _expectedEventQueue.Clear();
                _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
                _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
                _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
                CollectionView.RemoveAt(0);
                Assert.AreEqual(0, _expectedEventQueue.Count);
                Assert.AreEqual(9, CollectionView.CurrentPosition);
                Assert.AreEqual(item, CollectionView.CurrentItem);

                // now add grouping and verify that the items will be inserted into
                // the correct groups
                CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
                Assert.AreEqual(20, CollectionView.CurrentPosition);
                Assert.AreEqual(item, CollectionView.CurrentItem);

                // verify that removing an item from an earlier group would move down 
                // the item and update the currency
                _expectedEventQueue.Clear();
                _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
                _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
                _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
                CollectionView.RemoveAt(0);
                Assert.AreEqual(0, _expectedEventQueue.Count);
                Assert.AreEqual(19, CollectionView.CurrentPosition);
                Assert.AreEqual(item, CollectionView.CurrentItem);

                // now add paging and sorting. we should have updated the current position
                // because the current item stayed in the view, even through the page size changed
                CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
                CollectionView.PageSize = 5;
                Assert.AreEqual(1, CollectionView.CurrentPosition);
                Assert.AreEqual(item, CollectionView.CurrentItem);

                // now try removing the current item and verify that the 
                // next item after it would get currency
                item = CollectionView[1] as TestClass;
                _expectedEventQueue.Clear();
                _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
                _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
                _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
                CollectionView.RemoveAt(0);
                Assert.AreEqual(0, _expectedEventQueue.Count);
                Assert.AreEqual(0, CollectionView.CurrentPosition);
                Assert.AreEqual(item, CollectionView.CurrentItem);
            }
        }

        /// <summary>
        /// Regression test after issue we ran into where a Refresh after adding items would make the added items disappear.
        /// </summary>
        [TestMethod]
        [Description("Regression test after issue we ran into where a Refresh after adding items would make the added items disappear.")]
        public void RefreshAfterAddTest()
        {
            // we initialize the list with 25 items
            Assert.AreEqual(25, CollectionView.Count);
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));

            // add in 25 more to the source directly, which should call into our
            // ProcessCollectionChanged method
            MethodInfo mi = SourceCollection.GetType().GetMethod("Insert");
            for (int i = 0; i < 25; i++)
            {
                mi.Invoke(SourceCollection, new object[] { 0, new TestClass() });
            }

            // check that we now show 50 items
            Assert.AreEqual(50, CollectionView.Count);

            // refresh and validate that the added items don't disappear
            CollectionView.Refresh();
            Assert.AreEqual(50, CollectionView.Count);
        }

        /// <summary>
        /// Make sure we handle a Reset event from the source appropriately.
        /// </summary>
        [TestMethod]
        [Description("Make sure we handle a Reset event from the source appropriately.")]
        public void ResetTest()
        {
            // when we get a reset event through a "Clear", we should refresh
            // and show that we now have a count of zero.
            SourceCollection.GetType().GetMethod("Clear").Invoke(SourceCollection, new object[]{});
            Assert.AreEqual(0, CollectionView.Count);
        }
    }
}
