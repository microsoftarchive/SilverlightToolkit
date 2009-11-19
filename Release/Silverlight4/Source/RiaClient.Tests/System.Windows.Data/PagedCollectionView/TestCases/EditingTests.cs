//-----------------------------------------------------------------------
// <copyright file="EditingTests.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Microsoft.Silverlight.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class runs the unit tests for editing items in a PagedCollectionView
    /// </summary>
    public class EditingTests : PagedCollectionViewTest
    {
        #region Fields

        private bool? _cancelCurrencyMove;

        #endregion Fields

        /// <summary>
        /// Validate the CanCancelEdit property.
        /// </summary>
        [TestMethod]
        [Description("Validate the CanCancelEdit property.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception is just being used to test against an exception being generated elsewhere; it is not itself being thrown.")]
        public void CanCancelEditTest()
        {
            object editItem = CollectionView[0];
            CollectionView.EditItem(CollectionView[0]);

            if (editItem is IEditableObject)
            {
                Assert.IsTrue(CollectionView.CanCancelEdit);

                // if we don't implement IList, we cannot run the rest of
                // the test as we cannot add/remove items
                if (this.ImplementsIList)
                {
                    // verify that we cannot call CommitEdit during an AddNew operation
                    CollectionView.AddNew();
                    AssertExpectedException(
                        new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CancelEdit", "AddNew")),
                        delegate
                        {
                            CollectionView.CancelEdit();
                        });
                    CollectionView.CommitNew();
                }

                // if we are not editing trying to CancelEdit will throw
                if (!CollectionView.CanCancelEdit)
                {
                    AssertExpectedException(
                        new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.CancelEditNotSupported)),
                        delegate
                        {
                            CollectionView.CancelEdit();
                        });
                }
            }
            else
            {
                // if our collection does not implement IEditableCollection
                // and our data does not impelement IEditableObject, then we
                // cannot cancel an edit, as we do not have the old data cached
                Assert.IsFalse(CollectionView.CanCancelEdit);
            }
        }

        /// <summary>
        /// Validate the CancelEdit method.
        /// </summary>
        [TestMethod]
        [Description("Validate the CancelEdit method.")]
        public void CancelEditTest()
        {
            // check initial values before we edit
            TestClass editItem = CollectionView[0] as TestClass;
            Assert.AreEqual(1, editItem.IntProperty);
            Assert.AreEqual("A", editItem.StringProperty);

            // edit the item
            CollectionView.EditItem(CollectionView[0]);

            // set new values during the edit
            editItem.IntProperty = 0;
            editItem.StringProperty = "Edit";

            // some collections or items cannot cancel edits.
            // we have a separate test for this
            if (CollectionView.CanCancelEdit)
            {
                // check that after we cancel, the old values are reverted
                CollectionView.CancelEdit();
                Assert.AreEqual(1, editItem.IntProperty);
                Assert.AreEqual("A", editItem.StringProperty);
            }
        }

        /// <summary>
        /// Validate the CommitEdit method.
        /// </summary>
        [TestMethod]
        [Description("Validate the CommitEdit method.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception is just being used to test against an exception being generated elsewhere; it is not itself being thrown.")]
        public void CommitEditTest()
        {
            // verify that once we commit the data after an edit, the CollectionView
            // contains the updated values
            TestClass editItem = CollectionView[0] as TestClass;
            CollectionView.EditItem(editItem);
            editItem.IntProperty = 0;
            editItem.StringProperty = "Edit";
            CollectionView.CommitEdit();
            Assert.AreEqual(0, (CollectionView[0] as TestClass).IntProperty);
            Assert.AreEqual("Edit", (CollectionView[0] as TestClass).StringProperty);

            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // verify that we cannot call CommitEdit during an AddNew operation
                CollectionView.AddNew();
                AssertExpectedException(
                    new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringTransaction, "CommitEdit", "AddNew")),
                    delegate
                    {
                        CollectionView.CommitEdit();
                    });
            }
        }

        /// <summary>
        /// Validate the CurrentEditItem property.
        /// </summary>
        [TestMethod]
        [Description("Validate the CurrentItem and CurrentPosition property.")]
        public void CurrencyTest()
        {
            // set currency on an item and edit
            TestClass editItem = CollectionView[0] as TestClass;
            TestClass testItem = null;
            CollectionView.MoveCurrentTo(editItem);
            Assert.AreEqual(editItem, CollectionView.CurrentItem);

            CollectionView.EditItem(editItem);
            Assert.AreEqual(editItem, CollectionView.CurrentItem);
            
            // verify that after we commit, the editItem
            // still has currency
            CollectionView.CommitEdit();
            Assert.AreEqual(editItem, CollectionView.CurrentItem);

            // add sorting and verify that after an edit, even though
            // the item moves, it is still set as the current item
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            CollectionView.EditItem(editItem);
            editItem.IntProperty = 100;
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.AreEqual(editItem, CollectionView.CurrentItem);

            // now commit and verify that the position changed
            // but the item is still current
            CollectionView.CommitEdit();
            Assert.AreEqual(24, CollectionView.CurrentPosition);
            Assert.AreEqual(editItem, CollectionView.CurrentItem);

            // now add paging
            CollectionView.PageSize = 5;
            editItem = CollectionView[0] as TestClass;
            testItem = CollectionView[1] as TestClass;
            CollectionView.MoveCurrentTo(editItem);
            Assert.AreEqual(editItem, CollectionView.CurrentItem);

            // edit the item so that it will be moved to a
            // different page. the next item on the current
            // page should get the currency
            CollectionView.EditItem(editItem);
            editItem.IntProperty = 100;
            CollectionView.CommitEdit();
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.AreEqual(testItem, CollectionView.CurrentItem);
            Assert.AreNotEqual(editItem, CollectionView.CurrentItem);

            // now set currency to null and edit an item
            CollectionView.MoveCurrentTo(null);
            Assert.AreEqual(null, CollectionView.CurrentItem);
            editItem = CollectionView[0] as TestClass;
            CollectionView.EditItem(editItem);
            editItem.IntProperty = 100;
            
            // check that after we commit, the currency
            // has not changed
            CollectionView.CommitEdit();
            Assert.AreEqual(null, CollectionView.CurrentItem);
        }

        /// <summary>
        /// Validate the CurrentEditItem property.
        /// </summary>
        [TestMethod]
        [Description("Validate the CurrentEditItem property.")]
        public void CurrentEditItemTest()
        {
            // verify that we are not editing anything
            Assert.IsNull(CollectionView.CurrentEditItem);

            // verify that the property gets set to the item being edited
            object editItem = CollectionView[0];
            CollectionView.EditItem(editItem);
            Assert.AreEqual(editItem, CollectionView.CurrentEditItem);
        }

        /// <summary>
        /// Calls EditItem/CommitEdit on items not in the current view.
        /// </summary>
        [TestMethod]
        [Description("Calls EditItem/CommitEdit on items not in the current view.")]
        public void EditItemNotInViewTest()
        {
            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // test that we can edit an item not in the collection
                // and that it will get added to the collection upon
                // calling CommitEdit
                TestClass item = new TestClass();
                Assert.IsFalse(CollectionView.Contains(item));
                CollectionView.EditItem(item);
                CollectionView.CommitEdit();
                Assert.IsTrue(CollectionView.Contains(item));

                // also, show that when we have paging and call edit item
                // on an item for a different page, we will still be able
                // to edit it and move it to the correct location
                CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
                item = CollectionView[0] as TestClass;
                CollectionView.PageSize = 5;
                CollectionView.MoveToLastPage();

                Assert.IsFalse(CollectionView.Contains(item));
                CollectionView.EditItem(item);

                // because of sorting, this item should be moved to the last page
                // upon commit
                item.IntProperty = 100; 
                CollectionView.CommitEdit();
                Assert.IsTrue(CollectionView.Contains(item));
            }
        }

        /// <summary>
        /// Validate the EditItem method.
        /// </summary>
        [TestMethod]
        [Description("Validate the EditItem method.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception is just being used to test against an exception being generated elsewhere; it is not itself being thrown.")]
        public void EditItemTest()
        {
            // initially we should not be editing anything
            Assert.IsFalse(CollectionView.IsEditingItem);

            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // test that we cannot edit a new item that has not been
                // committed yet
                object item = CollectionView.AddNew();
                CollectionView.EditItem(item);
                Assert.IsFalse(CollectionView.IsEditingItem);
                Assert.IsTrue(CollectionView.IsAddingNew);
                CollectionView.CommitNew();

                // however, we can edit other items other than the new item.
                // this will force a commit on the new item and start editing
                // the other item
                CollectionView.AddNew();
                object editItem = CollectionView[0];
                CollectionView.EditItem(editItem);
                Assert.AreEqual(editItem, CollectionView.CurrentEditItem);
                Assert.IsTrue(CollectionView.IsEditingItem);
                Assert.IsFalse(CollectionView.IsAddingNew);
                CollectionView.CommitEdit();
            }
        }

        /// <summary>
        /// Check that the correct events are fired when we edit an item.
        /// </summary>
        [TestMethod]
        [Description("Check that the correct events are fired when we edit an item.")]
        public void EventsTest()
        {
            // begin editing an item
            TestClass editItem = CollectionView[0] as TestClass;
            _propertyChangedTracked = true;
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "IsEditingItem" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "CurrentEditItem" });
            if (editItem is IEditableObject)
            {
                _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "CanCancelEdit" });
            }
            CollectionView.EditItem(editItem);
            Assert.AreEqual(0, _expectedEventQueue.Count);
            editItem.IntProperty = 100;
            Assert.AreEqual(0, CollectionView.IndexOf(editItem));

            // because we have no sorting, filtering, grouping, or paging, we should not get a
            // CollectionChanged event or modify currency after we CommitEdit
            this.AssertNoEvent(delegate { CollectionView.CommitEdit(); });
            Assert.AreEqual(0, CollectionView.IndexOf(editItem));

            // we will add sorting now. this will remove and add the item back
            // in when we edit and commit
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));

            CollectionView.MoveCurrentTo(editItem);
            CollectionView.EditItem(editItem);
            editItem.IntProperty = 0;
            Assert.AreEqual(24, CollectionView.IndexOf(editItem));
            Assert.AreEqual(24, CollectionView.CurrentPosition);
            Assert.AreEqual(editItem, CollectionView.CurrentItem);

            // make sure all these events are fired in the correct order
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "IsEditingItem" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "CurrentEditItem" });
            if (editItem is IEditableObject)
            {
                _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "CanCancelEdit" });
            }
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "Count" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "PropertyChanged", Parameter = "Count" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            _propertyChangedTracked = false;
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.AreEqual(editItem, CollectionView.CurrentItem);

            // now add grouping
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("StringProperty"));
            CollectionView.EditItem(editItem);
            editItem.IntProperty = 100;
            Assert.AreEqual(0, CollectionView.IndexOf(editItem));
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.AreEqual(editItem, CollectionView.CurrentItem);

            // make sure all these events are fired in the correct order
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(12, CollectionView.CurrentPosition);
            Assert.AreEqual(editItem, CollectionView.CurrentItem);

            // now remove grouping and add paging. once we edit,
            // the item should be moved to an earlier page, shifting
            // items up.
            CollectionView.GroupDescriptions.Clear();
            CollectionView.PageSize = 5;
            CollectionView.MoveToPage(4);
            CollectionView.EditItem(editItem);
            editItem.IntProperty = 0;
            Assert.AreEqual(4, CollectionView.IndexOf(editItem));

            // since we have changed pages, the current item should
            // not be the edit item
            object currentItem = CollectionView.CurrentItem; 
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.AreNotEqual(editItem, CollectionView.CurrentItem);

            // make sure all these events are fired in the correct order
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(-1, CollectionView.IndexOf(editItem));

            // verify that the currency still points to the same item, but
            // with a shifted index
            Assert.AreEqual(1, CollectionView.CurrentPosition);
            Assert.AreEqual(currentItem, CollectionView.CurrentItem);

            // now edit an item at an index greater than the CurrentPosition to
            // verify that with a CommitEdit, there will be no currency change
            // however, we will still fire the current changed event for subscribers
            editItem = CollectionView[4] as TestClass;
            CollectionView.EditItem(editItem);

            // make sure all these events are fired in the correct order
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(1, CollectionView.CurrentPosition);
            Assert.AreEqual(currentItem, CollectionView.CurrentItem);
        }

        /// <summary>
        /// Check that if the edited item does not pass the filter, it will not show up after commiting.
        /// </summary>
        [TestMethod]
        [Description("Check that if the edited item does not pass the filter, it will not show up after commiting.")]
        public void FilteredEditItemTest()
        {
            // apply a filter to the CollectionView
            CollectionView.Filter = FilterNegativeNumbers;
            Assert.AreEqual(25, CollectionView.Count);

            // edit the item and set the IntProperty to a negative
            // number. before we commit, it should stay in the view
            TestClass editItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentTo(editItem);
            CollectionView.EditItem(editItem);
            editItem.IntProperty = -1;
            Assert.AreEqual(25, CollectionView.Count);
            Assert.IsTrue(CollectionView.Contains(editItem));

            // now commit to verify that it gets filtered out of the view
            // also verify that the correct events get fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(24, CollectionView.Count);
            Assert.IsFalse(CollectionView.Contains(editItem));
            
            // verify that the currency has moved to a different item
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.AreNotEqual(editItem, CollectionView.CurrentItem);

            // try adding paging and run through the same test
            // the only differnce should be that we add in a new item
            // to replace the one that got filtered out of the current page
            CollectionView.PageSize = 5;

            editItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentTo(editItem);
            CollectionView.EditItem(editItem);
            editItem.IntProperty = -1;
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsTrue(CollectionView.Contains(editItem));

            // now commit to verify that it gets filtered out of the view
            // also verify that the correct events get fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsFalse(CollectionView.Contains(editItem));

            // also, try this on the last page to verify that a new item is not brought in
            CollectionView.MoveToLastPage();

            editItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentTo(editItem);
            CollectionView.EditItem(editItem);
            editItem.IntProperty = -1;
            Assert.AreEqual(3, CollectionView.Count);
            Assert.IsTrue(CollectionView.Contains(editItem));

            // now commit to verify that it gets filtered out of the view
            // also verify that the correct events get fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(2, CollectionView.Count);
            Assert.IsFalse(CollectionView.Contains(editItem));

            // now add grouping, move to the first page, and run
            // through the scenario again
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
            CollectionView.MoveToFirstPage();
            editItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentTo(editItem);
            CollectionView.EditItem(editItem);
            editItem.IntProperty = -1;
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsTrue(CollectionView.Contains(editItem));

            // now commit to verify that it gets filtered out of the view
            // also verify that the correct events get fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsFalse(CollectionView.Contains(editItem));

            // now try the same thing on the last page. since it
            // only has one item, once it gets filtered out, we
            // should move to the previous page
            CollectionView.MoveToLastPage();
            Assert.AreEqual(4, CollectionView.PageIndex);
            editItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentTo(editItem);
            CollectionView.EditItem(editItem);
            editItem.IntProperty = -1;
            Assert.AreEqual(1, CollectionView.Count);
            Assert.IsTrue(CollectionView.Contains(editItem));

            // now commit to verify that it gets filtered out of the view
            // also verify that the correct events get fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Reset" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsFalse(CollectionView.Contains(editItem));

            // now try the same thing on the last page. we should get
            // a remove event, but not an add
            editItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentTo(editItem);
            CollectionView.EditItem(editItem);
            editItem.IntProperty = -1;
            Assert.AreEqual(5, CollectionView.Count);
            Assert.IsTrue(CollectionView.Contains(editItem));

            // now commit to verify that it gets filtered out of the view
            // also verify that the correct events get fired
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(4, CollectionView.Count);
            Assert.IsFalse(CollectionView.Contains(editItem));

            // now verify that even if the item is filtered out, we can
            // still edit it, and that if we change the value so that
            // we pass the filter, we would display it again
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            CollectionView.MoveToLastPage();
            Assert.IsFalse(CollectionView.Contains(editItem));
            CollectionView.EditItem(editItem);
            editItem.IntProperty = 10;
            CollectionView.CommitEdit();
            Assert.IsTrue(CollectionView.Contains(editItem));
        }

        /// <summary>
        /// Validate behavior for when PageSize=1 and the item is filtered out.
        /// </summary>
        [TestMethod]
        [Description("Validate behavior for when PageSize=1 and the item is filtered out.")]
        public void FilterWithOneItemPerPage()
        {
            // apply a filter and pagesize to the CollectionView
            CollectionView.Filter = FilterNegativeNumbers;
            CollectionView.PageSize = 1;
            CollectionView.MoveToPage(5);

            // edit item and filter it out
            TestClass editItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentTo(editItem);
            CollectionView.EditItem(editItem);
            editItem.IntProperty = -1;
            Assert.IsTrue(CollectionView.IndexOf(editItem) >= 0);

            // now commit to verify that it gets filtered out of the view
            // but we stay on the same page as a new item is brought in
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(5, CollectionView.PageIndex);
            Assert.IsFalse(CollectionView.IndexOf(editItem) >= 0);

            // now move to the last page and run the same test. this
            // time, we can't bring in another item and we will have no
            // more items on the page, so we move to the previous page
            CollectionView.MoveToLastPage();
            editItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentTo(editItem);
            CollectionView.EditItem(editItem);
            editItem.IntProperty = -1;
            Assert.AreEqual(23, CollectionView.PageIndex);

            // now commit and verify that we move to the previous page
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Reset" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(22, CollectionView.PageIndex);

            // now run the same test from the last page but make sure
            // that the item does not get filtered out this time.
            // we want to make sure that we will stay on the same page.
            CollectionView.MoveToLastPage();
            editItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentTo(editItem);
            CollectionView.EditItem(editItem);
            Assert.AreEqual(22, CollectionView.PageIndex);

            // now commit and verify that we stayed on the current page
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.CommitEdit();
            Assert.AreEqual(22, CollectionView.PageIndex);
            Assert.AreEqual(editItem, CollectionView.CurrentItem);
        }

        /// <summary>
        /// Check that when we apply grouping and edit items, it gets moved to the correct spot.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Check that when we apply grouping and edit items, it gets moved to the correct spot.")]
        public void GroupingTest()
        {
            // add grouping
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
            TestClass editItem = CollectionView[0] as TestClass;
            TestClass testItem = null;

            ListBox listBox = new ListBox();
            listBox.ItemsSource = CollectionView;

            this.CreateAsyncTask(
                listBox,
                delegate
                {
                    // check initial state
                    Assert.AreEqual(editItem, listBox.Items[0]);
                    Assert.AreEqual(25, listBox.Items.Count);

                    // edit item
                    CollectionView.EditItem(editItem);
                    Assert.AreEqual(editItem, listBox.Items[0]);
                    
                    // commit to verify that it does not move position,
                    // as it did not change groups
                    CollectionView.CommitEdit();
                    Assert.AreEqual(editItem, listBox.Items[0]);

                    // add paging and edit
                    CollectionView.PageSize = 5;
                    CollectionView.MoveToFirstPage();
                    editItem = CollectionView[0] as TestClass;
                    CollectionView.EditItem(editItem);
                    editItem.IntProperty = 5;
                    testItem = CollectionView[4] as TestClass;
                    Assert.AreEqual(4, listBox.Items.IndexOf(testItem));
                    Assert.AreEqual(0, listBox.Items.IndexOf(editItem));

                    // verify that once we commit, the edit item is
                    // no longer in the view, and the index of the 
                    // other items on the page get shifted down
                    CollectionView.CommitEdit();
                    Assert.AreEqual(3, listBox.Items.IndexOf(testItem));
                    Assert.AreEqual(-1, listBox.Items.IndexOf(editItem));

                    // move to next page, and edit an item so that now it
                    // appears on the first page, shifting items down.
                    CollectionView.MoveToNextPage();
                    editItem = CollectionView[4] as TestClass;
                    CollectionView.EditItem(editItem);
                    editItem.IntProperty = 1;
                    testItem = CollectionView[3] as TestClass;
                    Assert.AreEqual(3, listBox.Items.IndexOf(testItem));
                    Assert.AreEqual(4, listBox.Items.IndexOf(editItem));

                    // verify that once we commit, the edit item is
                    // no longer in the view, and the index of the 
                    // other items on the page get shifted up
                    CollectionView.CommitEdit();
                    Assert.AreEqual(4, listBox.Items.IndexOf(testItem));
                    Assert.AreEqual(-1, listBox.Items.IndexOf(editItem));
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Test Editing with an IEditableObject.
        /// </summary>
        /// <remarks>
        /// Our class that implements IEditableObject is set up so that it will
        /// update a debug string property when the IEditableObject interface
        /// is invoked. We use this to verify our editing scenarios.
        /// </remarks>
        [TestMethod]
        [Description("Test Editing with an IEditableObject.")]
        public void IEditableObject()
        {
            // we only want to run this test for items that implement IEditableObject
            // and where the source collection won't handle the editing first
            EditableTestClass editItem = CollectionView[0] as EditableTestClass;
            if (editItem == null)
            {
                return;
            }

            // check that the debug string has not been set
            Assert.IsNull(editItem.DebugString);

            // verify that BeginEdit was called on the IEditable interface
            CollectionView.EditItem(editItem);
            Assert.AreEqual("BeginEdit", editItem.DebugString);
            
            // verify that CancelEdit was called on the IEditable interface
            CollectionView.CancelEdit();
            Assert.AreEqual("CancelEdit", editItem.DebugString);

            // verify that EndEdit was called on the IEditable interface
            CollectionView.EditItem(editItem);
            CollectionView.CommitEdit();
            Assert.AreEqual("EndEdit", editItem.DebugString);

            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // verify that when adding a new item, it will call BeginEdit on it
                editItem = CollectionView.AddNew() as EditableTestClass;
                Assert.AreEqual("BeginEdit", editItem.DebugString);

                // verify that when canceling the new item, it will call CancelEdit on it.
                CollectionView.CancelNew();
                Assert.AreEqual("CancelEdit", editItem.DebugString);

                // verify that when committing a new item, it will call EndEdit on it.
                editItem = CollectionView.AddNew() as EditableTestClass;
                CollectionView.CommitNew();
                Assert.AreEqual("EndEdit", editItem.DebugString);
            }
        }

        /// <summary>
        /// Check editing scenarios when we have paging.
        /// </summary>
        [TestMethod]
        [Description("Check editing scenarios when we have paging.")]
        public void PagingTest()
        {
            // set up sorting and paging
            CollectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("IntProperty", System.ComponentModel.ListSortDirection.Ascending));
            CollectionView.PageSize = 5;

            TestClass editItem = CollectionView[0] as TestClass;
            CollectionView.EditItem(editItem);

            CollectionView.CurrentChanging += new CurrentChangingEventHandler(CollectionView_PagingTestCurrentChanging);

            this._cancelCurrencyMove = true;

            // verify that we cannot change pages because the collection view cancels the move 
            // in the CurrentChanging event handler.
            Assert.IsFalse(CollectionView.MoveToNextPage());
            Assert.IsFalse(CollectionView.MoveToPreviousPage());

            this._cancelCurrencyMove = false;

            // verify that we can change pages even though the current item is edited.
            Assert.IsTrue(CollectionView.MoveToNextPage());

            // Editing was committed before the page move.
            Assert.IsFalse(CollectionView.IsEditingItem);

            this._cancelCurrencyMove = null;

            // Move back to the first page.
            Assert.IsTrue(CollectionView.MoveToFirstPage());

            // Edit the first item again.
            editItem = CollectionView[0] as TestClass;
            CollectionView.EditItem(editItem);

            // verify that the item is still on the same page after we update
            // the values, as we have not committed the change yet
            editItem.IntProperty = 100;
            Assert.AreEqual(0, CollectionView.IndexOf(editItem));

            // verify that after we commit, the item is moved off the page
            // and that we can change pages.
            CollectionView.CommitEdit();
            Assert.AreEqual(-1, CollectionView.IndexOf(editItem));
            Assert.IsTrue(CollectionView.MoveToNextPage());
            Assert.IsTrue(CollectionView.MoveToPreviousPage());

            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // Begin an addition transaction.
                CollectionView.AddNew();

                // Do not allow page moves.
                this._cancelCurrencyMove = true;
                Assert.IsFalse(CollectionView.MoveToNextPage());

                // Addition is still in progress.
                Assert.IsTrue(CollectionView.IsAddingNew);

                // verify that we can change pages even though an addition is in progress.
                this._cancelCurrencyMove = false;
                Assert.IsTrue(CollectionView.MoveToNextPage());

                // Addition was committed before the page move.
                Assert.IsFalse(CollectionView.IsAddingNew);
            }

            CollectionView.CurrentChanging -= new CurrentChangingEventHandler(CollectionView_PagingTestCurrentChanging);
        }

        /// <summary>
        /// Check that when we apply sorting and edit items, it gets moved to the correct spot.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Check that when we apply sorting and edit items, it gets moved to the correct spot.")]
        public void SortingTest()
        {
            // add sorting
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            TestClass editItem = CollectionView[0] as TestClass;
            TestClass testItem = null;

            ListBox listBox = new ListBox();
            listBox.ItemsSource = CollectionView;

            this.CreateAsyncTask(
                listBox,
                delegate
                {
                    // check initial state
                    Assert.AreEqual(editItem, listBox.Items[0]);
                    Assert.AreEqual(25, listBox.Items.Count);

                    // edit item
                    CollectionView.EditItem(editItem);
                    editItem.IntProperty = 5;
                    Assert.AreEqual(editItem, listBox.Items[0]);

                    // commit to verify that it gets moved
                    CollectionView.CommitEdit();
                    Assert.AreEqual(editItem, listBox.Items[20]);

                    // add paging and edit
                    CollectionView.PageSize = 5;
                    CollectionView.MoveToFirstPage();
                    editItem = CollectionView[0] as TestClass;
                    CollectionView.EditItem(editItem);
                    editItem.IntProperty = 5;
                    testItem = CollectionView[4] as TestClass;
                    Assert.AreEqual(4, listBox.Items.IndexOf(testItem));
                    Assert.AreEqual(0, listBox.Items.IndexOf(editItem));

                    // verify that once we commit, the edit item is
                    // no longer in the view, and the index of the 
                    // other items on the page get shifted down
                    CollectionView.CommitEdit();
                    Assert.AreEqual(3, listBox.Items.IndexOf(testItem));
                    Assert.AreEqual(-1, listBox.Items.IndexOf(editItem));

                    // move to next page, and edit an item so that now it
                    // appears on the first page, shifting items down.
                    CollectionView.MoveToNextPage();
                    editItem = CollectionView[4] as TestClass;
                    CollectionView.EditItem(editItem);
                    editItem.IntProperty = 1;
                    testItem = CollectionView[3] as TestClass;
                    Assert.AreEqual(3, listBox.Items.IndexOf(testItem));
                    Assert.AreEqual(4, listBox.Items.IndexOf(editItem));

                    // verify that once we commit, the edit item is
                    // no longer in the view, and the index of the 
                    // other items on the page get shifted up
                    CollectionView.CommitEdit();
                    Assert.AreEqual(4, listBox.Items.IndexOf(testItem));
                    Assert.AreEqual(-1, listBox.Items.IndexOf(editItem));

                    // move to the first page and check out an item for
                    // editing without updating any values
                    CollectionView.MoveToFirstPage();
                    editItem = CollectionView[2] as TestClass;
                    CollectionView.EditItem(editItem);
                    Assert.AreEqual(editItem, listBox.Items[2]);

                    // commit with out making any changes and verify
                    // that the index remained the same
                    CollectionView.CommitEdit();
                    Assert.AreEqual(editItem, CollectionView[2]);
                    Assert.AreEqual(editItem, listBox.Items[2]);

                    // add grouping as well and try the same thing
                    CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
                    editItem = CollectionView[2] as TestClass;
                    CollectionView.EditItem(editItem);
                    Assert.AreEqual(editItem, listBox.Items[2]);

                    // verify that after we commit, the index stays the same
                    CollectionView.CommitEdit();
                    Assert.AreEqual(editItem, CollectionView[2]);
                    Assert.AreEqual(editItem, listBox.Items[2]);
                });

            EnqueueTestComplete();
        }
        
        #region Helper Functions

        private void CollectionView_PagingTestCurrentChanging(object source, CurrentChangingEventArgs e)
        {
            if (this._cancelCurrencyMove == true)
            {
                e.Cancel = true;
            }
            else if (this._cancelCurrencyMove == false)
            {
                // Commit the edited/addition item before the page move occurs.
                IEditableCollectionView editableCollectionView = (IEditableCollectionView)source;
                if (editableCollectionView.IsAddingNew)
                {
                    editableCollectionView.CommitNew();
                }
                else
                {
                    editableCollectionView.CommitEdit();
                }
            }
        }

        #endregion
    }
}
