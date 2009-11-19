//-----------------------------------------------------------------------
// <copyright file="AddItemTests.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Collections;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Microsoft.Silverlight.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class runs the unit tests for adding items in a PagedCollectionView
    /// </summary>
    public class AddItemTests : PagedCollectionViewTest
    {
        /// <summary>
        /// Verify that when we call AddNew on a filtered collection, we will still add the new item as the last item in the view.
        /// </summary>
        [TestMethod]
        [Description("Verify that when we call AddNew on a filtered collection, we will still add the new item as the last item in the view.")]
        public void AddNewOnFilteredCollection()
        {
            // add a filter to the collection
            CollectionView.Filter = FilterOutOnes;

            // now add an item and verify that it is added as the last item
            object addItem = CollectionView.AddNew();
            Assert.AreEqual(CollectionView.Count-1, CollectionView.IndexOf(addItem));
            CollectionView.CancelNew();

            // verify that this holds for paging as well
            CollectionView.PageSize = 10;
            addItem = CollectionView.AddNew();
            Assert.AreEqual(CollectionView.Count - 1, CollectionView.IndexOf(addItem));
        }

        /// <summary>
        /// Validate various scenarios where we add new items to the collection.
        /// </summary>
        [TestMethod]
        [Description("Validate various scenarios where we add new items to the collection.")]
        public void AddNewTest()
        {
            // verify the the count gets updated when we add items
            Assert.AreEqual(25, CollectionView.Count);
            CollectionView.AddNew();
            Assert.AreEqual(26, CollectionView.Count);
            CollectionView.CommitNew();
            Assert.AreEqual(26, CollectionView.Count);

            // verify that we can call an AddNew, when there is already
            // an add in progress (it will commit the previous add and
            // start a new add).
            object addItem = CollectionView.AddNew();
            CollectionView.AddNew();
            Assert.AreNotEqual(addItem, CollectionView.CurrentAddItem);
            CollectionView.CommitNew();
            Assert.AreEqual(28, CollectionView.Count);
            
            // also the same applies for when we edit items. by calling
            // AddNew, we will commit the edit and start a new add operation
            CollectionView.EditItem(CollectionView[0]);
            Assert.IsTrue(CollectionView.IsEditingItem);
            addItem = CollectionView.AddNew();
            Assert.IsFalse(CollectionView.IsEditingItem);
            Assert.AreEqual(addItem, CollectionView.CurrentAddItem);
            CollectionView.CancelNew();

            // verify that AddNew succeeds even on empty collections that
            // implement IEnumerable<T>
            Assert.IsTrue(CollectionView.CanRemove);
            while (CollectionView.Count > 0)
            {
                CollectionView.RemoveAt(0);
            }
            addItem = CollectionView.AddNew();
            Assert.IsNotNull(addItem);
            CollectionView.CancelNew();
        }

        /// <summary>
        /// Verify that we will correctly handle the situation when we call AddNew while we are not in sync with the source collection.
        /// </summary>
        [TestMethod]
        [Description("Verify that we will correctly handle the situation when we call AddNew while we are not in sync with the source collection.")]
        public void AddNewWithUnsyncedCollection()
        {
            // this issue should only affect source collections that do not 
            // implement INotifyCollectionChanged, but we can still run the
            // test for all collections.
            IList sourceCollection = CollectionView.SourceCollection as IList;

            // first add items to the source collection, which should get the
            // collection out of sync with the internal list if it doesn't
            // implement INotifyCollectionChanged.
            if (sourceCollection != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    sourceCollection.Add(new EditableTestClass());
                }
            }

            // now try to call AddNew and verify that we will get it added at the correct index
            // 25 (original items) + 10 (loop) + 1 (add new) = 36 (count) - 1 (for zero based index) = 35
            object newItem = CollectionView.AddNew();
            Assert.AreEqual(35, CollectionView.IndexOf(newItem)); 
        }

        /// <summary>
        /// Validate when we can add new items and when the property is set to false.
        /// </summary>
        [TestMethod]
        [Description("Validate when we can add new items and when the property is set to false.")]
        public void CanAddNewTest()
        {
            // verify that we can add items into our collection view
            Assert.IsTrue(CollectionView.CanAddNew);

            // verify that we are not allowed to add a new item while we are still editing
            CollectionView.EditItem(CollectionView[0]);
            Assert.IsFalse(CollectionView.CanAddNew);
            CollectionView.CommitEdit();
            Assert.IsTrue(CollectionView.CanAddNew);
            
            // verify that when we are adding a new item, we can add another
            CollectionView.AddNew();
            Assert.IsTrue(CollectionView.CanAddNew);
        }

        /// <summary>
        /// Verify that when we call CancelNew, the new item is removed from the collection.
        /// </summary>
        [TestMethod]
        [Description("Verify that when we call CancelNew, the new item is removed from the collection.")]
        public void CancelNewTest()
        {
            // assert that the count is updated when we add a new item
            // and that the item is within the view.
            Assert.AreEqual(25, CollectionView.Count);
            TestClass addItem = CollectionView.AddNew() as TestClass;
            Assert.AreEqual(26, CollectionView.Count);
            Assert.IsTrue(CollectionView.IndexOf(addItem) >= 0);

            // now verify that when we cancel teh add, the item no longer
            // appears in the view and the count is decremented.
            CollectionView.CancelNew();
            Assert.AreEqual(25, CollectionView.Count);
            Assert.IsFalse(CollectionView.IndexOf(addItem) >= 0);
        }

        /// <summary>
        /// Validate that we throw an error if we try to CommitNew or CancelNew during an edit.
        /// </summary>
        [TestMethod]
        [Description("Validate that we throw an error if we try to CommitNew or CancelNew during an edit.")]
        public void CannotCommitOrCancelNewDuringEdit()
        {
            // verify that we cannot commit new during an edit
            CollectionView.EditItem(CollectionView[0]);

            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CommitNew", "EditItem")),
                delegate
                {
                    CollectionView.CommitNew();
                });

            // verify that we cannot cancel new during an edit
            CollectionView.EditItem(CollectionView[0]);

            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CancelNew", "EditItem")),
                delegate
                {
                    CollectionView.CancelNew();
                });
        }

        /// <summary>
        /// Validate that currecy is correctly set when adding and commiting new items.
        /// </summary>
        [TestMethod]
        [Description("Validate that currecy is correctly set when adding and commiting new items.")]
        public void CurrencyTest()
        {
            // verify that when we call AddNew, the new item gets currency
            TestClass newItem = CollectionView.AddNew() as TestClass;
            Assert.AreEqual(newItem, CollectionView.CurrentItem);
            Assert.AreEqual(CollectionView.IndexOf(newItem), CollectionView.CurrentPosition);
            CollectionView.CommitNew();
            Assert.AreEqual(newItem, CollectionView.CurrentItem);
            Assert.AreEqual(CollectionView.IndexOf(newItem), CollectionView.CurrentPosition);

            // verify that we can set currency to another item in between the AddNew/CommitNew
            newItem = CollectionView.AddNew() as TestClass;
            Assert.AreEqual(newItem, CollectionView.CurrentItem);
            Assert.AreEqual(CollectionView.IndexOf(newItem), CollectionView.CurrentPosition);
            CollectionView.MoveCurrentToFirst();
            CollectionView.CommitNew();
            Assert.AreNotEqual(newItem, CollectionView.CurrentItem);
            Assert.AreNotEqual(CollectionView.IndexOf(newItem), CollectionView.CurrentPosition);

            // add sorting and paging
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            CollectionView.PageSize = 5;
            CollectionView.MoveToLastPage();

            // add a new item that would get moved to a different page
            newItem = CollectionView.AddNew() as TestClass;
            newItem.IntProperty = 2;
            CollectionView.CommitNew();

            // verify that the last item on the page gets currency
            Assert.AreEqual(CollectionView[CollectionView.Count - 1], CollectionView.CurrentItem);
            Assert.AreEqual(CollectionView.Count - 1, CollectionView.CurrentPosition);

            // try setting the currency to null after a AddNew and verify that on commit
            // the currency will stay at null.
            CollectionView.AddNew();
            CollectionView.MoveCurrentTo(null);
            CollectionView.CommitNew();
            Assert.IsNull(CollectionView.CurrentItem);

        }

        /// <summary>
        /// Validate that currecy is correctly set when adding and commiting new items with paging.
        /// </summary>
        [TestMethod]
        [Description("Validate that currecy is correctly set when adding and commiting new items with paging.")]
        public void CurrencyWithPagingTest()
        {
            // set a page size 
            CollectionView.PageSize = 5;

            // verify that when we call AddNew, the new item gets currency
            TestClass newItem = CollectionView.AddNew() as TestClass;
            Assert.AreEqual(newItem, CollectionView.CurrentItem);
            
            // verify that after we commit, we still have currency
            CollectionView.CommitNew();
            Assert.AreEqual(newItem, CollectionView.CurrentItem);
            Assert.AreEqual(CollectionView.IndexOf(newItem), CollectionView.CurrentPosition);
        }

        /// <summary>
        /// Validate that when we are adding a new item, the CurrentAddItem property is correctly set.
        /// </summary>
        [TestMethod]
        [Description("Validate that when we are adding a new item, the CurrentAddItem property is correctly set.")]
        public void CurrentAddItemTest()
        {
            // verify that the property is null when we are not adding
            Assert.IsNull(CollectionView.CurrentAddItem);

            // add a new item and verify that the property is set
            object newItem = CollectionView.AddNew();
            Assert.AreEqual(newItem, CollectionView.CurrentAddItem);

            // verify that once we are done adding, the property will revert to null
            CollectionView.CancelNew();
            Assert.IsNull(CollectionView.CurrentAddItem);
        }

        /// <summary>
        /// Check that the correct events are fired when we add an item.
        /// </summary>
        [TestMethod]
        [Description("Check that the correct events are fired when we add an item.")]
        public void EventsTest()
        {
            // begin adding an item - when we call AddNew, it sets
            // currency to the new item.
            _propertyChangedTracked = true;
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "ItemCount" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "Count" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "IsAddingNew" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "CurrentAddItem" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            TestClass addItem = CollectionView.AddNew() as TestClass;
            _propertyChangedTracked = false;
            Assert.AreEqual(0, _expectedEventQueue.Count);
            addItem.IntProperty = 5;
            addItem.StringProperty = "A";

            // because we have no sorting, filtering, grouping, or paging, we should 
            // not have to remove any items  from the view during an add. currency 
            // should also be unchanged for this first test.
            this.AssertNoEvent(delegate { CollectionView.CommitNew(); });
            
            // we will add sorting now. this will remove and add the item back
            // in when we commit
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            addItem = CollectionView.AddNew() as TestClass;
            addItem.IntProperty = 5;
            addItem.StringProperty = "B";
            
            // make sure that we fire the remove/add, and also the currency events
            _propertyChangedTracked = true;
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "IsAddingNew" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "CurrentAddItem" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "Count" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "Count" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitNew();
            _propertyChangedTracked = false;
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(26, CollectionView.CurrentPosition);

            // add an item that will move with sorting to test the currency changes.
            addItem = CollectionView.AddNew() as TestClass;
            addItem.IntProperty = 4;
            addItem.StringProperty = "A";

            // make sure that we fire the remove/add, once we commit and that the currency
            // events are again fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitNew();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(16, CollectionView.CurrentPosition);

            // now add in paging and edit the item so that it will be moved
            // to a different page upon committing.
            CollectionView.PageSize = 5;
            addItem = CollectionView.AddNew() as TestClass;
            addItem.IntProperty = 5;
            addItem.StringProperty = "C";

            // again, make sure that we fire the remove/add, and update the currency 
            // when the new item moves out.
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitNew();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(4, CollectionView.CurrentPosition);
            Assert.AreNotEqual(addItem, CollectionView.CurrentItem);

            // now add and edit the values so that it should stay on the first page.
            addItem = CollectionView.AddNew() as TestClass;
            addItem.IntProperty = 0;
            addItem.StringProperty = "A";

            // make sure that we fire the remove/add, and update the currency
            // as the new item has moved within the page.
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitNew();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.AreEqual(addItem, CollectionView.CurrentItem);

            // now add grouping and test editing so that it should remain on the same page.
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
            addItem = CollectionView.AddNew() as TestClass;
            addItem.IntProperty = 0;
            addItem.StringProperty = "B";

            // make sure that we fire the remove/add, and update the currency
            // as the new item has moved within the page.
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitNew();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.AreEqual(addItem, CollectionView.CurrentItem);

            // now add an item that will move off the current page
            addItem = CollectionView.AddNew() as TestClass;
            addItem.IntProperty = 5;
            addItem.StringProperty = "D";

            // make sure that we fire the remove/add, and update the currency
            // as the new item has moved within the page.
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitNew();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(4, CollectionView.CurrentPosition);
            Assert.AreNotEqual(addItem, CollectionView.CurrentItem);
        }

        /// <summary>
        /// Check that if the added item does not pass the filter, it will not show up after commiting.
        /// </summary>
        [TestMethod]
        [Description("Check that if the added item does not pass the filter, it will not show up after commiting.")]
        public void FilteredAddItemTest()
        {
            // apply a filter to the CollectionView
            CollectionView.Filter = FilterNegativeNumbers;
            Assert.AreEqual(25, CollectionView.Count);

            // add the item and set the IntProperty to a negative
            // number. before we commit, it should stay in the view
            TestClass addItem = CollectionView.AddNew() as TestClass;
            addItem.IntProperty = -1;
            Assert.AreEqual(26, CollectionView.Count);
            Assert.IsTrue(CollectionView.IndexOf(addItem) >= 0);

            // now commit to verify that it gets filtered out of the view
            // also verify that the correct events get fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitNew();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(25, CollectionView.Count);
            Assert.IsFalse(CollectionView.IndexOf(addItem) >= 0);

            // verify that the currency has moved to a different item
            Assert.AreEqual(24, CollectionView.CurrentPosition);
            Assert.AreNotEqual(addItem, CollectionView.CurrentItem);

            // try adding paging and run through the same test
            // the only differnce should be that we add in a new item
            // to replace the one that got filtered out of the current page
            CollectionView.PageSize = 5;

            addItem = CollectionView.AddNew() as TestClass;
            addItem.IntProperty = -1;
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsTrue(CollectionView.IndexOf(addItem) >= 0);

            // now commit to verify that it gets filtered out of the view
            // also verify that the correct events get fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitNew();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsFalse(CollectionView.IndexOf(addItem) >= 0);

            // also, try this on the last page to verify that a new item is not brought in
            CollectionView.RemoveAt(CollectionView.Count - 1);
            CollectionView.MoveToLastPage();
            Assert.AreEqual(4, CollectionView.Count);

            addItem = CollectionView.AddNew() as TestClass;
            addItem.IntProperty = -1;
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsTrue(CollectionView.IndexOf(addItem) >= 0);

            // now commit to verify that it gets filtered out of the view
            // also verify that the correct events get fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitNew();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(4, CollectionView.Count);
            Assert.IsFalse(CollectionView.IndexOf(addItem) >= 0);

            // now add grouping, move to the first page, and run
            // through the scenario again
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
            CollectionView.MoveToFirstPage();
            addItem = CollectionView.AddNew() as TestClass;
            addItem.IntProperty = -1;
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsTrue(CollectionView.IndexOf(addItem) >= 0);

            // now commit to verify that it gets filtered out of the view
            // also verify that the correct events get fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitNew();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsFalse(CollectionView.IndexOf(addItem) >= 0);

            // now try the same thing on the last page. we should get
            // a remove event, but not an add
            CollectionView.MoveToLastPage();
            addItem = CollectionView.AddNew() as TestClass;
            addItem.IntProperty = -1;
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsTrue(CollectionView.IndexOf(addItem) >= 0);

            // now commit to verify that it gets filtered out of the view
            // also verify that the correct events get fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitNew();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(4, CollectionView.Count);
            Assert.IsFalse(CollectionView.IndexOf(addItem) >= 0);
        }

        /// <summary>
        /// Validate adding a new item when we have grouping enabled.
        /// </summary>
        [TestMethod]
        [Description("Validate adding a new item when we have grouping enabled.")]
        public void GroupingTest()
        {
            // add a group description to the collection view
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));

            // add a new item and verify that it does not get moved until we commit
            TestClass newItem = CollectionView.AddNew() as TestClass;
            Assert.AreEqual(25, CollectionView.IndexOf(newItem));
            newItem.IntProperty = 1;
            newItem.StringProperty = "C";
            Assert.AreEqual(25, CollectionView.IndexOf(newItem));

            // it should get moved to the end of the first group once we commit
            CollectionView.CommitNew();
            Assert.AreEqual(5, CollectionView.IndexOf(newItem));

            // now add paging and add a new item. verify that it will stay
            // on the current page until we commit
            CollectionView.PageSize = 5;
            newItem = CollectionView.AddNew() as TestClass;
            newItem.IntProperty = 5;
            newItem.StringProperty = "C";
            Assert.AreEqual(4, CollectionView.IndexOf(newItem));

            // now commit, and the item will move out of this page
            CollectionView.CommitNew();
            Assert.IsFalse(CollectionView.IndexOf(newItem) >= 0);

            // now add sorting and try adding a new item
            CollectionView.SortDescriptions.Add(new SortDescription("StringProperty", ListSortDirection.Ascending));
            newItem = CollectionView.AddNew() as TestClass;
            newItem.IntProperty = 1;
            newItem.StringProperty = "";
            Assert.AreEqual(4, CollectionView.IndexOf(newItem));

            // when we commit, this time the sorting should move the item into the top
            // of the first group
            CollectionView.CommitNew();
            Assert.AreEqual(0, CollectionView.IndexOf(newItem));
        }

        /// <summary>
        /// Validate that when we are adding a new item, the IsAddingNew property is correctly set.
        /// </summary>
        [TestMethod]
        [Description("Validate that when we are adding a new item, the IsAddingNew property is correctly set.")]
        public void IsAddingNewTest()
        {
            // verify that the property is false when we are not adding
            Assert.IsFalse(CollectionView.IsAddingNew);

            // add a new item and verify that the property is set
            CollectionView.AddNew();
            Assert.IsTrue(CollectionView.IsAddingNew);

            // verify that once we are cancel adding, the property will revert to false
            CollectionView.CancelNew();
            Assert.IsFalse(CollectionView.IsAddingNew);

            // add an item again
            CollectionView.AddNew();
            Assert.IsTrue(CollectionView.IsAddingNew);

            // verify that once we are commit the add, the property will revert to false
            CollectionView.CommitNew();
            Assert.IsFalse(CollectionView.IsAddingNew);
        }

        /// <summary>
        /// Validate the AddNew functionality when hooked up to a ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the AddNew functionality when hooked up to a ListBox.")]
        public void NewItemWithListBoxTest()
        {
            CollectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));
            CollectionView.PageSize = 6;

            ListBox lb = new ListBox();
            lb.ItemsSource = CollectionView;

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    // ------------------------------------
                    // |Page | 0  | 1  | 2  | 3  | 4  | 5  |
                    // ------------------------------------
                    // | 0   | 1  | 2  | 3  | 4  | 5  | 6  |
                    // | 1   | 7  | 8  | 9  | 10 | 11 | 12 |
                    // | 2   | 13 | 14 | 15 | 16 | 17 | 18 |
                    // | 3   | 19 | 20 | 21 | 22 | 23 | 24 |
                    // | 4   | 25 |    |    |    |    |    |
                    // ------------------------------------
                    Assert.AreEqual(6, lb.Items.Count);
                    CollectionView.MoveToLastPage();
                    Assert.AreEqual(1, lb.Items.Count);

                    // ------------------------------------
                    // |Page | 0  | 1  | 2  | 3  | 4  | 5  |
                    // ------------------------------------
                    // | 0   | 1  | 2  | 3  | 4  | 5  | 6  |
                    // | 1   | 7  | 8  | 9  | 10 | 11 | 12 |
                    // | 2   | 13 | 14 | 15 | 16 | 17 | 18 |
                    // | 3   | 19 | 20 | 21 | 22 | 23 | 24 |
                    // | 4   | 25 |-26-|    |    |    |    |
                    // ------------------------------------
                    TestClass item = CollectionView.AddNew() as TestClass;
                    item.IntProperty = 6;
                    item.StringProperty = "F";
                    Assert.AreEqual(2, lb.Items.Count);
                    CollectionView.CommitNew();
                    Assert.AreEqual(item, lb.Items[1]);

                    // ------------------------------------
                    // |Page | 0  | 1  | 2  | 3  | 4  | 5  |
                    // ------------------------------------
                    // | 0   | 1  | 2  | 3  | 4  | 5  | 6  |
                    // | 1   | 7  | 8  | 9  | 10 | 11 | 12 | <-- move to this page
                    // | 2   | 13 | 14 | 15 | 16 | 17 | 18 |
                    // | 3   | 19 | 20 | 21 | 22 | 23 | 24 |
                    // | 4   | 25 | 26 |    |    |    |    |
                    // ------------------------------------
                    CollectionView.MoveToPage(1);
                    item = CollectionView.AddNew() as TestClass;
                    item.IntProperty = 6;
                    item.StringProperty = "F";
                    Assert.AreEqual(6, lb.Items.Count);
                    Assert.IsTrue(lb.Items.Contains(item));

                    // after we commit, due to sorting, the item
                    // will move to the last page
                    CollectionView.CommitNew();
                    Assert.AreEqual(6, lb.Items.Count);
                    Assert.IsFalse(lb.Items.Contains(item));
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate that adding and committing a new item when we have paging enabled keeps the item on the same page.
        /// </summary>
        [TestMethod]
        [Description("Validate that adding and committing a new item when we have paging enabled keeps the item on the same page.")]
        public void PagingWithAddNewTest()
        {
            // add paging
            CollectionView.PageSize = 5;

            // add a new item and check that it is the last item on this page
            object addItem = CollectionView.AddNew();
            Assert.AreEqual(CollectionView.PageSize - 1, CollectionView.IndexOf(addItem));

            // now commit and verify that it stayed in the same position
            CollectionView.CommitNew();
            Assert.AreEqual(CollectionView.PageSize - 1, CollectionView.IndexOf(addItem));

            // move to the next page and test the same thing
            CollectionView.MoveToNextPage();
            addItem = CollectionView.AddNew();
            Assert.AreEqual(CollectionView.PageSize - 1, CollectionView.IndexOf(addItem));

            // now commit and verify that it stayed in the same position
            CollectionView.CommitNew();
            Assert.AreEqual(CollectionView.PageSize - 1, CollectionView.IndexOf(addItem));
        }

        /// <summary>
        /// Validate adding a new item when we have sorting enabled.
        /// </summary>
        [TestMethod]
        [Description("Validate adding a new item when we have sorting enabled.")]
        public void SortingTest()
        {
            // add a sort description to the collection view
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));

            // add a new item and verify that it does not get moved until we commit
            TestClass newItem = CollectionView.AddNew() as TestClass;
            Assert.AreEqual(25, CollectionView.IndexOf(newItem));
            newItem.IntProperty = 0;
            Assert.AreEqual(25, CollectionView.IndexOf(newItem));

            // now commit - the item should move to the top due to sorting
            CollectionView.CommitNew();
            Assert.AreEqual(0, CollectionView.IndexOf(newItem));

            // now combine with paging
            CollectionView.PageSize = 5;
            CollectionView.MoveToPage(1);

            // verify that adding a new item will still preserve the
            // count, as we have a page size constraint
            Assert.AreEqual(5, CollectionView.Count);
            newItem = CollectionView.AddNew() as TestClass;
            Assert.AreEqual(5, CollectionView.Count);

            // now set a value that will make the item move out of the
            // page once committed.
            newItem.IntProperty = 0;
            Assert.IsTrue(CollectionView.IndexOf(newItem) >= 0);
            CollectionView.CommitNew();
            Assert.IsFalse(CollectionView.IndexOf(newItem) >= 0);

            // now try editing an item and setting a value where it will stay
            // on the current page
            CollectionView.MoveToFirstPage();
            newItem = CollectionView.AddNew() as TestClass;
            Assert.IsTrue(CollectionView.IndexOf(newItem) >= 0);
            CollectionView.CommitNew();
            Assert.IsTrue(CollectionView.IndexOf(newItem) >= 0);
        }
    }
}
