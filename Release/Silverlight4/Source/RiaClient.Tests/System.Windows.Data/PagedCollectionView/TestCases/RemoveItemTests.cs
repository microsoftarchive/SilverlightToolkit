//-----------------------------------------------------------------------
// <copyright file="RemoveItemTests.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Microsoft.Silverlight.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class runs the unit tests for removing items in a PagedCollectionView
    /// </summary>
    public class RemoveItemTests : PagedCollectionViewTest
    {
        /// <summary>
        /// Validate when we can remove items from the PagedCollectionView.
        /// </summary>
        [TestMethod]
        [Description("Validate when we can remove items from the PagedCollectionView.")]
        public void CanRemoveTest()
        {
            // verify that we can add items into our collection view
            Assert.IsTrue(CollectionView.CanRemove);

            // verify that when we are adding a new item, we cannot remove items
            CollectionView.AddNew();
            Assert.IsFalse(CollectionView.CanRemove);
            CollectionView.CommitNew();
            Assert.IsTrue(CollectionView.CanRemove);

            // verify that while we are still editing, we cannot remove items
            CollectionView.EditItem(CollectionView[0]);
            Assert.IsFalse(CollectionView.CanRemove);
            CollectionView.CommitEdit();
            Assert.IsTrue(CollectionView.CanRemove);
        }

        /// <summary>
        /// Check that the correct events are fired when we remove an item.
        /// </summary>
        [TestMethod]
        [Description("Check that the correct events are fired when we remove an item.")]
        public void EventsTest()
        {
            TestClass removeItem = CollectionView[0] as TestClass;
            Assert.AreEqual(removeItem, CollectionView.CurrentItem);

            // because we have no paging, the remove will not trigger an add. 
            // however, it should trigger the currency changes.
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.Remove(removeItem);
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreNotEqual(removeItem, CollectionView.CurrentItem);
            
            // we will add sorting now. this time, since we are not removing an
            // item at an index before or equal to the current item, currency
            // should not be changed
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            removeItem = CollectionView[1] as TestClass;
            CollectionView.MoveCurrentToFirst();
            
            // make sure that we fire the remove/add, and that this time the currency
            // events are not fired, since the current item was unchanged and unmoved
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            CollectionView.Remove(removeItem);
            Assert.AreEqual(0, _expectedEventQueue.Count);

            // this time, we will set currency to an item that will move 
            // after we remove an item, so we will get the currency events.
            removeItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentToFirst();

            // make sure that we fire the remove/add, once we commit and that the currency
            // events are not fired, since the current item was unchanged and unmoved
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.Remove(removeItem);
            Assert.AreEqual(0, _expectedEventQueue.Count);

            // now add in paging and remove the item so that we will
            // bring in a new item from the next page
            CollectionView.PageSize = 5;
            removeItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentToFirst();

            // make sure that we fire the remove/add, and update the currency 
            // when the new item moves out.
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            CollectionView.Remove(removeItem);
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.AreNotEqual(removeItem, CollectionView.CurrentItem);

            // now move to the last page and remove. we should 
            // not bring in a new item
            CollectionView.MoveToLastPage();
            removeItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentToFirst();

            // make sure that we fire the remove, and update the currency
            // as the new item has moved within the page.
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.Remove(removeItem);
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.AreNotEqual(removeItem, CollectionView.CurrentItem);

            // now add grouping and test removing. this time, I will set currency on an item
            // other than the first item on the page, so that we can verify that the currency
            // will stay on the same item, but shifted down a position.
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
            CollectionView.MoveToFirstPage();
            removeItem = CollectionView[0] as TestClass;
            CollectionView.MoveCurrentTo(CollectionView[2]);
            Assert.AreEqual(2, CollectionView.CurrentPosition);

            // make sure that we fire the remove, and update the currency
            // as the new item has moved within the page.
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Remove" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Add" });
            CollectionView.Remove(removeItem);
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreEqual(1, CollectionView.CurrentPosition);
        }

        /// <summary>
        /// Validate removing an item when we have grouping enabled.
        /// </summary>
        [TestMethod]
        [Description("Validate removing an item when we have grouping enabled.")]
        public void GroupingTest()
        {
            // add a group description to the collection view and add paging
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
            CollectionView.PageSize = 5;

            // the first page should just have one group currently. verify
            // that once we remove an item, we will bring in an item from the
            // next group and have two groups.
            Assert.AreEqual(1, CollectionView.Groups.Count);
            CollectionView.RemoveAt(0);
            Assert.AreEqual(2, CollectionView.Groups.Count);
            Assert.AreEqual(1, (CollectionView.Groups[0] as CollectionViewGroup).Name);
            Assert.AreEqual(2, (CollectionView.Groups[1] as CollectionViewGroup).Name);
            
            // if we remove the first four items (the remaining items
            // from the first group), then the new items from the second
            // group will move in, and we should just have one group again
            CollectionView.RemoveAt(0);
            CollectionView.RemoveAt(0);
            CollectionView.RemoveAt(0);
            CollectionView.RemoveAt(0);
            Assert.AreEqual(1, CollectionView.Groups.Count);
            Assert.AreEqual(2, (CollectionView.Groups[0] as CollectionViewGroup).Name);
        }

        /// <summary>
        /// Check that we throw exceptions when the user tries to remove items from an invalid index.
        /// </summary>
        [TestMethod]
        [Description("Check that we throw exceptions when the user tries to remove items from an invalid index.")]
        public void RemoveAtInvalidIndexTest()
        {
            // remove at an index < 0
            AssertExpectedException(
                new ArgumentOutOfRangeException("index", PagedCollectionViewResources.IndexOutOfRange),
                delegate
                {
                    CollectionView.RemoveAt(-1);
                });

            // remove at an index >= count
            AssertExpectedException(
                new ArgumentOutOfRangeException("index", PagedCollectionViewResources.IndexOutOfRange),
                delegate
                {
                    CollectionView.RemoveAt(100);
                });
        }

        /// <summary>
        /// Validate that removing an item on the last page while it's full will correctly update the count.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate that removing an item on the last page while it's full will correctly update the count.")]
        public void RemoveItemOnFullLastPage()
        {
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            CollectionView.PageSize = 5;

            ListBox lb = new ListBox();
            lb.ItemsSource = CollectionView;

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    // move to the last page
                    CollectionView.MoveToLastPage();
                    Assert.AreEqual(5, lb.Items.Count);

                    // remove an item and verify that the count is correctly updated
                    CollectionView.RemoveAt(0);
                    Assert.AreEqual(4, lb.Items.Count);

                    // add grouping and change the page size to 4 so that we have
                    // full pages again (24 items / 4)
                    CollectionView.PageSize = 4;
                    CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
                    CollectionView.MoveToLastPage();
                    Assert.AreEqual(4, lb.Items.Count);

                    // now remove an item again and verify that the count is correctly updated
                    CollectionView.RemoveAt(0);
                    Assert.AreEqual(3, lb.Items.Count);
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the Remove functionality when hooked up to a ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the Remove functionality when hooked up to a ListBox.")]
        public void RemoveItemWithListBoxTest()
        {
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            CollectionView.PageSize = 5;

            ListBox lb = new ListBox();
            lb.ItemsSource = CollectionView;

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    CollectionView.MoveToLastPage();

                    // --------------------------------
                    // |Page | 0  | 1  | 2  | 3  | 4  |
                    // --------------------------------
                    // | 0   | 1A | 1B | 1A | 1B | 1A |
                    // | 1   | 2B | 2A | 2B | 2A | 2B |
                    // | 2   | 3A | 3B | 3A | 3B | 3A |
                    // | 3   | 4B | 4A | 4B | 4A | 4B |
                    // | 4   | 5A | 5B | 5A | 5B | 5A | <- current page

                    Assert.AreEqual(5, lb.Items.Count);
                    Assert.AreEqual(5, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual("A", (lb.Items[0] as TestClass).StringProperty);

                    // remove the item on the first page and verify that 
                    // the first item in the listbox points to the new item.
                    TestClass removeItem = CollectionView[0] as TestClass;
                    CollectionView.Remove(removeItem);

                    // --------------------------------
                    // |Page | 0  | 1  | 2  | 3  | 4  |
                    // --------------------------------
                    // | 0   | 1A | 1B | 1A | 1B | 1A |
                    // | 1   | 2B | 2A | 2B | 2A | 2B |
                    // | 2   | 3A | 3B | 3A | 3B | 3A |
                    // | 3   | 4B | 4A | 4B | 4A | 4B |
                    // | 4   | 5B | 5A | 5B | 5A |    | <- current page

                    Assert.AreEqual(4, lb.Items.Count);
                    Assert.IsFalse(lb.Items.Contains(removeItem));
                    Assert.AreEqual(5, (lb.Items[0] as TestClass).IntProperty);
                    Assert.AreEqual("B", (lb.Items[0] as TestClass).StringProperty);

                    // now move to the previous page and remove the first item
                    // verify that everything got shifted down and that we brought
                    // in an item from the next page
                    CollectionView.MoveToPreviousPage();
                    Assert.AreEqual(4, (lb.Items[4] as TestClass).IntProperty);
                    Assert.AreEqual("B", (lb.Items[4] as TestClass).StringProperty);
                    CollectionView.RemoveAt(0);

                    // --------------------------------
                    // |Page | 0  | 1  | 2  | 3  | 4  |
                    // --------------------------------
                    // | 0   | 1A | 1B | 1A | 1B | 1A |
                    // | 1   | 2B | 2A | 2B | 2A | 2B |
                    // | 2   | 3A | 3B | 3A | 3B | 3A |
                    // | 3   | 4A | 4B | 4A | 4B | 5B | <- current page
                    // | 4   | 5A | 5B | 5A |    |    |

                    Assert.AreEqual(4, (lb.Items[3] as TestClass).IntProperty);
                    Assert.AreEqual("B", (lb.Items[3] as TestClass).StringProperty);
                    Assert.AreEqual(5, (lb.Items[4] as TestClass).IntProperty);
                    Assert.AreEqual("B", (lb.Items[4] as TestClass).StringProperty);
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate various scenarios where we remove items from the collection.
        /// </summary>
        [TestMethod]
        [Description("Validate various scenarios where we remove items from the collection.")]
        public void RemoveTest()
        {
            // verify the the count gets updated when we remove items
            TestClass removeItem = CollectionView[0] as TestClass;
            Assert.AreEqual(25, CollectionView.Count);
            Assert.IsTrue(CollectionView.Contains(removeItem));
            CollectionView.Remove(removeItem);
            Assert.AreEqual(24, CollectionView.Count);
            Assert.IsFalse(CollectionView.Contains(removeItem));
            
            // verify that we cannot call Remove/RemoveAt during an Edit
            CollectionView.EditItem(CollectionView[0]);
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "RemoveAt")),
                delegate
                {
                    CollectionView.RemoveAt(0);
                });

            // verify that we also cannot call Remove/RemoveAt during an Add
            CollectionView.AddNew();
            AssertExpectedException(
                new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PagedCollectionViewResources.OperationNotAllowedDuringAddOrEdit, "RemoveAt")),
                delegate
                {
                    CollectionView.RemoveAt(0);
                });
        }

        /// <summary>
        /// Validate that when removing an item on the second to last page would cause the add event to fire for the last item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate that when removing an item on the second to last page would cause the add event to fire for the last item.")]
        public void RemoveOnSecondToLastPageWithOneItemLeft()
        {
            CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Ascending));
            CollectionView.PageSize = 6;

            ListBox lb = new ListBox();
            lb.ItemsSource = CollectionView;

            this.CreateAsyncTask(
                lb,
                delegate
                {
                    CollectionView.MoveToPage(3);

                    // -------------------------------------
                    // |Page | 0  | 1  | 2  | 3  | 4  | 5  |
                    // -------------------------------------
                    // | 0   | 1A | 1B | 1A | 1B | 1A | 2B |
                    // | 1   | 2A | 2B | 2A | 2B | 3A | 3B |
                    // | 2   | 3A | 3B | 3A | 4B | 4A | 4B |
                    // | 3   | 4A | 4B | 5A | 5B | 5A | 5B | <- current page
                    // | 4   | 5A |

                    Assert.AreEqual(6, lb.Items.Count);
                    Assert.AreEqual(5, (lb.Items[5] as TestClass).IntProperty);
                    Assert.AreEqual("B", (lb.Items[5] as TestClass).StringProperty);

                    // remove the item on the page and verify that 
                    // the item from the last page moves onto the current page.
                    TestClass removeItem = CollectionView[0] as TestClass;
                    CollectionView.Remove(removeItem);

                    // -------------------------------------
                    // |Page | 0  | 1  | 2  | 3  | 4  | 5  |
                    // -------------------------------------
                    // | 0   | 1A | 1B | 1A | 1B | 1A | 2B |
                    // | 1   | 2A | 2B | 2A | 2B | 3A | 3B |
                    // | 2   | 3A | 3B | 3A | 4B | 4A | 4B |
                    // | 3   | 4B | 5A | 5B | 5A | 5B | 5A | <- current page

                    Assert.IsFalse(lb.Items.Contains(removeItem));
                    Assert.AreEqual(6, lb.Items.Count);
                    Assert.AreEqual(5, (lb.Items[5] as TestClass).IntProperty);
                    Assert.AreEqual("A", (lb.Items[5] as TestClass).StringProperty);

                    // now change the page size and add grouping
                    Assert.AreEqual(24, CollectionView.ItemCount);
                    CollectionView.PageSize = 11;
                    CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));

                    // now there should be 3 pages, so move to the second to last page
                    CollectionView.MoveToPage(1);

                    // remove two items and verify that we get the two items from the next page
                    CollectionView.RemoveAt(0);
                    CollectionView.RemoveAt(0);
                    Assert.AreEqual(22, CollectionView.ItemCount);
                    Assert.AreEqual(11, lb.Items.Count);
                });

            EnqueueTestComplete();
        }
    }
}
