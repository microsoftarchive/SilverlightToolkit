//-----------------------------------------------------------------------
// <copyright file="CurrencyTests.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Collections.Generic;
    using System.Windows.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Silverlight.Testing;

    /// <summary>
    /// This class runs the unit tests for checking currency in a PagedCollectionView
    /// </summary>
    public class CurrencyTests : PagedCollectionViewTest
    {
        /// <summary>
        /// Check when the IsCurrentBeforeFirst and the IsCurrentAfterLast properties get set.
        /// </summary>
        [TestMethod]
        [Description("Check when the IsCurrentBeforeFirst and the IsCurrentAfterLast properties get set.")]
        public void CurrencyFlags()
        {
            // when we have currency set to a value in view, both values should show false
            Assert.IsFalse(CollectionView.IsCurrentBeforeFirst);
            Assert.IsFalse(CollectionView.IsCurrentAfterLast);

            // when we move currency to the item at -1 when we have a non-empty list,
            // we show that the current value is before the first item
            CollectionView.MoveCurrentToPosition(-1);
            Assert.IsTrue(CollectionView.IsCurrentBeforeFirst);
            Assert.IsFalse(CollectionView.IsCurrentAfterLast);

            // when we move currency to the item at Count when we have a non-empty list,
            // we show that the current value is after the first item
            CollectionView.MoveCurrentToPosition(CollectionView.Count);
            Assert.IsFalse(CollectionView.IsCurrentBeforeFirst);
            Assert.IsTrue(CollectionView.IsCurrentAfterLast);

            // when we have no items in the collection, both values are set
            CollectionView = new PagedCollectionView(new List<object>());
            Assert.IsTrue(CollectionView.IsCurrentBeforeFirst);
            Assert.IsTrue(CollectionView.IsCurrentAfterLast);
        }

        /// <summary>
        /// Verify that currency is set to the new item when AddNew is called.
        /// </summary>
        [TestMethod]
        [Description("Verify that currency is set to the new item when AddNew is called.")]
        public void CurrencyWithAddNew()
        {
            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // whenever we add a new item through the AddNew() method
                // currency gets set to the new item
                TestClass item = CollectionView.AddNew() as TestClass;
                Assert.AreEqual(item, CollectionView.CurrentItem);
            }
        }

        /// <summary>
        /// Verify that currency is updated with add and remove operations.
        /// </summary>
        [TestMethod]
        [Description("Verify that currency is updated with add and remove operations.")]
        public void CurrencyWithAddRemove()
        {
            // move currency to the last item
            CollectionView.MoveCurrentToLast();
            TestClass item = CollectionView[24] as TestClass;
            Assert.AreEqual(item, CollectionView.CurrentItem);
            Assert.AreEqual(24, CollectionView.CurrentPosition);

            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // verify that when we remove an item, the current position gets updated
                // but the current item remains the same
                CollectionView.RemoveAt(0);
                Assert.AreEqual(item, CollectionView.CurrentItem);
                Assert.AreEqual(23, CollectionView.CurrentPosition);

                // now set currency to the first item
                CollectionView.MoveCurrentToFirst();
                item = CollectionView[0] as TestClass;
                Assert.AreEqual(item, CollectionView.CurrentItem);
                Assert.AreEqual(0, CollectionView.CurrentPosition);

                // verify that when we remove an item after the current position,
                // currency remains untouched
                CollectionView.RemoveAt(1);
                Assert.AreEqual(item, CollectionView.CurrentItem);
                Assert.AreEqual(0, CollectionView.CurrentPosition);

                // now insert a new TestClass into the beginning of the collection
                Assert.AreEqual(23, CollectionView.Count);
                SourceCollection.GetType().GetMethod("Insert").Invoke(SourceCollection, new object[] { 0, new TestClass() });
                Assert.AreEqual(24, CollectionView.Count);

                // verify that the current position got shifted up one, but currency stayed on the same item
                Assert.AreEqual(item, CollectionView.CurrentItem);
                Assert.AreEqual(1, CollectionView.CurrentPosition);

                // now insert at an index beyond the current item and verify
                // that currency remains untouched
                SourceCollection.GetType().GetMethod("Insert").Invoke(SourceCollection, new object[] { 3, new TestClass() });
                Assert.AreEqual(25, CollectionView.Count);
                Assert.AreEqual(item, CollectionView.CurrentItem);
                Assert.AreEqual(1, CollectionView.CurrentPosition);
            }
        }

        /// <summary>
        /// Verify that currency is updated correctly when we change pages.
        /// </summary>
        [TestMethod]
        [Description("Verify that currency is updated correctly when we change pages.")]
        public void CurrencyWithPaging()
        {
            // add paging and move the currency to the first item
            CollectionView.PageSize = 5;
            CollectionView.MoveCurrentToFirst();
            TestClass item = CollectionView[0] as TestClass;
            Assert.AreEqual(item, CollectionView.CurrentItem);
            Assert.AreEqual(0, CollectionView.CurrentPosition);

            // change the page and verify that the currency events get fired
            // currency should be set to null and then to the first item on the next page
            _expectedEventQueue.Clear();
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CollectionChanged", Parameter = "Reset" });
            _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
            CollectionView.MoveToNextPage();
            Assert.AreEqual(0, _expectedEventQueue.Count);
            Assert.AreNotEqual(item, CollectionView.CurrentItem);
            Assert.AreEqual(0, CollectionView.CurrentPosition);

            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // if we have an add/edit operation currently taking place,
                // verify that it will get committed and that we will update
                // the currency when moving pages. we will change currency
                // twice, as we first set currency to null and then because
                // we did not commit the new operation, we will cancel the
                // page move and stay on the current page.
                item = CollectionView.AddNew() as TestClass;
                Assert.AreEqual(item, CollectionView.CurrentItem);
                _expectedEventQueue.Clear();

                _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanging" });
                _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
                _expectedEventQueue.Add(new EventNotification() { EventType = "CurrentChanged" });
                Assert.AreEqual(1, CollectionView.PageIndex);
                CollectionView.MoveToNextPage();

                // verify that we stay on the same page and that we are still adding the new item
                Assert.IsTrue(CollectionView.IsAddingNew);
                Assert.AreEqual(1, CollectionView.PageIndex);
                Assert.AreEqual(0, _expectedEventQueue.Count);
                Assert.AreEqual(item, CollectionView.CurrentItem);
                Assert.AreEqual(4, CollectionView.CurrentPosition);
            }
        }

        /// <summary>
        /// Check when the IsCurrentBeforeFirst and the IsCurrentAfterLast properties get set.
        /// </summary>
        [TestMethod]
        [Description("Check when the IsCurrentBeforeFirst and the IsCurrentAfterLast properties get set.")]
        public void IsCurrentInViewTest()
        {
            // MoveCurrentToPosition() returns the value of IsCurrentInView after the operation.
            Assert.IsTrue(CollectionView.MoveCurrentToPosition(0));
            Assert.IsFalse(CollectionView.MoveCurrentToPosition(-1));
        }

        /// <summary>
        /// Check a couple of scenarios in which we move currency.
        /// </summary>
        [TestMethod]
        [Description("Check a couple of scenarios in which we move currency.")]
        public void MoveCurrencyTests()
        {
            // when we call this method, currency should be set to the first item
            CollectionView.MoveCurrentToFirst();
            Assert.AreEqual(CollectionView[0], CollectionView.CurrentItem);
            Assert.AreEqual(0, CollectionView.CurrentPosition);

            // when we call this method, currency should be set to the last item
            CollectionView.MoveCurrentToLast();
            Assert.AreEqual(CollectionView[24], CollectionView.CurrentItem);
            Assert.AreEqual(24, CollectionView.CurrentPosition);

            // when we call this method, currency should be set to the specified item
            CollectionView.MoveCurrentToPosition(10);
            Assert.AreEqual(CollectionView[10], CollectionView.CurrentItem);
            Assert.AreEqual(10, CollectionView.CurrentPosition);

            // when we call this method, currency should be set to the previous item
            CollectionView.MoveCurrentToPrevious();
            Assert.AreEqual(CollectionView[9], CollectionView.CurrentItem);
            Assert.AreEqual(9, CollectionView.CurrentPosition);

            // when we call this method, currency should be set to the next item
            CollectionView.MoveCurrentToNext();
            Assert.AreEqual(CollectionView[10], CollectionView.CurrentItem);
            Assert.AreEqual(10, CollectionView.CurrentPosition);
        }

        /// <summary>
        /// Verify that we throw when we try to move currency to an invalid position.
        /// </summary>
        [TestMethod]
        [Description("Verify that we throw when we try to move currency to an invalid position.")]
        public void MoveCurrencyToInvalidPosition()
        {
            // when setting to a value < -1, we should throw
            PagedCollectionViewTest.AssertExpectedException(
                new ArgumentOutOfRangeException("position"),
                delegate
                {
                    CollectionView.MoveCurrentToPosition(-2);
                });

            // when setting to a value > count, we should throw
            PagedCollectionViewTest.AssertExpectedException(
                new ArgumentOutOfRangeException("position"),
                delegate
                {
                    CollectionView.MoveCurrentToPosition(26);
                });
        }

        /// <summary>
        /// Verify that currency can be set to null.
        /// </summary>
        [TestMethod]
        [Description("Verify that currency can be set to null.")]
        public void NullCurrency()
        {
            // show that we can set currency to null
            CollectionView.MoveCurrentTo(null);
            Assert.AreEqual(-1, CollectionView.CurrentPosition);
            Assert.IsNull(CollectionView.CurrentItem);

            // we can also set this value through moving position to -1
            CollectionView.MoveCurrentToPosition(-1);
            Assert.AreEqual(-1, CollectionView.CurrentPosition);
            Assert.IsNull(CollectionView.CurrentItem);

            // or when moving position to Count
            CollectionView.MoveCurrentToPosition(CollectionView.Count);
            Assert.AreEqual(25, CollectionView.CurrentPosition);
            Assert.IsNull(CollectionView.CurrentItem);
        }

        /// <summary>
        /// Check a couple of basic scenarios in which currency is set.
        /// </summary>
        [TestMethod]
        [Description("Check a couple of basic scenarios in which currency is set.")]
        public void SetCurrencyTests()
        {
            // when we create a collection view with a source collection
            // that is not empty, currency should be set to the first item
            TestClass item = CollectionView[0] as TestClass;
            Assert.AreEqual(item, CollectionView.CurrentItem);
            Assert.AreEqual(0, CollectionView.CurrentPosition);

            // when we explicitly set currency to an item, it should 
            // update the CurrentItem and CurrentPosition properties
            item = CollectionView[1] as TestClass;
            Assert.AreNotEqual(item, CollectionView.CurrentItem);
            Assert.AreNotEqual(1, CollectionView.CurrentPosition);
            CollectionView.MoveCurrentTo(item);
            Assert.AreEqual(item, CollectionView.CurrentItem);
            Assert.AreEqual(1, CollectionView.CurrentPosition);
        }
    }
}
