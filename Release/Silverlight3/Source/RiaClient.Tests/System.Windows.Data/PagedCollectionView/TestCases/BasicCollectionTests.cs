//-----------------------------------------------------------------------
// <copyright file="BasicCollectionTests.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Collections;
    using System.Reflection;
    using System.Windows.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class runs basic tests for methods and properties that are found on IList
    /// and ICollection such as Contains, and IndexOf.
    /// </summary>
    public class BasicCollectionTests : PagedCollectionViewTest
    {
        /// <summary>
        /// Verify that if we cancel a CurrentChanging operation during a PageSize update that takes place during an Add/Edit we will not be able to continue and throw.
        /// </summary>
        [TestMethod]
        [Description("Verify that if we cancel a CurrentChanging operation during a PageSize update that takes place during an Add/Edit we will not be able to continue and throw.")]
        public void CancelCurrentChangingDuringPageSizeUpdate()
        {
            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                CollectionView.CurrentChanging += new CurrentChangingEventHandler(CancelCurrentChanging);

                // add a new item
                TestClass newItem = CollectionView.AddNew() as TestClass;
                Assert.IsTrue(CollectionView.IsAddingNew);

                // verify that because the currency change is cancelled, we do not continue and throw
                PagedCollectionViewTest.AssertExpectedException(
                    new InvalidOperationException(PagedCollectionViewResources.ChangingPageSizeNotAllowedDuringAddOrEdit),
                    delegate
                    {
                        CollectionView.PageSize = 10;
                    });


                // now edit an item
                CollectionView.EditItem(CollectionView[0]);
                Assert.IsTrue(CollectionView.IsEditingItem);

                // verify that because the currency change is cancelled, we do not continue and throw
                PagedCollectionViewTest.AssertExpectedException(
                    new InvalidOperationException(PagedCollectionViewResources.ChangingPageSizeNotAllowedDuringAddOrEdit),
                    delegate
                    {
                        CollectionView.PageSize = 10;
                    });

                CollectionView.CurrentChanging -= new CurrentChangingEventHandler(CancelCurrentChanging);
            }
        }

        /// <summary>
        /// Check that the Contains method returns true only when the specified item is in the current view.
        /// </summary>
        [TestMethod]
        [Description("Check that the Contains method returns true only when the specified item is in the current view.")]
        public void ContainsTest()
        {
            TestClass item = CollectionView[0] as TestClass;

            // show that items in the Collection will return true.
            Assert.IsTrue(CollectionView.Contains(item));

            // show that items not added to the Collection will return false
            item = new TestClass();
            Assert.IsFalse(CollectionView.Contains(item));

            // show that once we add it in, it will return true.
            MethodInfo mi = SourceCollection.GetType().GetMethod("Insert");
            mi.Invoke(SourceCollection, new object[] { 25, item });
            Assert.IsTrue(CollectionView.Contains(item));

            // now test that with paging, if the item is not on the current page,
            // it will return false
            CollectionView.PageSize = 5;
            Assert.IsFalse(CollectionView.Contains(item));

            // now move to the last page where the item is, and verify
            // that it will now return true
            CollectionView.MoveToLastPage();
            Assert.IsTrue(CollectionView.Contains(item));
        }

        /// <summary>
        /// Check that the Count property correctly returns the current number of items in the view.
        /// </summary>
        [TestMethod]
        [Description("Check that the Count property correctly returns the current number of items in the view.")]
        public void CountTest()
        {
            // we should have 25 items initially
            Assert.AreEqual(25, CollectionView.Count);

            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // our count should increase when we add items
                TestClass newItem = CollectionView.AddNew() as TestClass;
                CollectionView.CommitNew();
                Assert.AreEqual(26, CollectionView.Count);

                // and it should decrease when we remove items
                CollectionView.Remove(newItem);
                CollectionView.RemoveAt(0);
                Assert.AreEqual(24, CollectionView.Count);

                // when we add paging, it should display just the items on
                // the current page
                CollectionView.PageSize = 5;
                Assert.AreEqual(5, CollectionView.Count);

                // if we move to the last page, it will display the number
                // of items on that page (which is less than the page size)
                CollectionView.MoveToLastPage();
                Assert.AreEqual(4, CollectionView.Count);

                // add grouping and test
                CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
                CollectionView.MoveToFirstPage();
                Assert.AreEqual(5, CollectionView.Count);

                // get the count of items in the group
                int count = 0;
                for (int i = 0; i < CollectionView.Groups.Count; i++)
                {
                    count += (CollectionView.Groups[i] as CollectionViewGroup).ItemCount;
                }
                Assert.AreEqual(CollectionView.Count, count);

                // remove paging and test again
                CollectionView.PageSize = 0;
                Assert.AreEqual(24, CollectionView.Count);

                // get the count of items in the group
                count = 0;
                for (int i = 0; i < CollectionView.Groups.Count; i++)
                {
                    count += (CollectionView.Groups[i] as CollectionViewGroup).ItemCount;
                }
                Assert.AreEqual(CollectionView.Count, count);
            }
        }

        /// <summary>
        /// Verify that the DeferRefresh method will prevent updates to the collection.
        /// </summary>
        [TestMethod]
        [Description("Verify that the DeferRefresh method will prevent updates to the collection.")]
        public void DeferRefreshTest()
        {
            // verify that we do not need a refresh
            Assert.IsFalse(CollectionView.NeedsRefresh);

            // also verify that we don't have grouping
            Assert.IsNull(CollectionView.Groups);

            // once we set the DeferRefresh, sorts, groups, filters, and paging
            // should not be applied until the refresh is committed.
            using (CollectionView.DeferRefresh())
            {
                // we still don't need a refresh until we update states
                Assert.IsFalse(CollectionView.NeedsRefresh);

                // apply sorts, groups, filters, paging
                CollectionView.SortDescriptions.Add(new SortDescription("IntProperty", ListSortDirection.Descending));
                CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));
                CollectionView.Filter = this.FilterNegativeNumbers;
                CollectionView.PageSize = 10;
                CollectionView.MoveToPage(2);

                // now that we have deferred, we need a refresh
                Assert.IsTrue(CollectionView.NeedsRefresh);

                // verify that we haven't set up the groups yet
                Assert.IsNull(CollectionView.Groups);
            }

            // now verify that the CollectionView is updated
            Assert.AreEqual(10, CollectionView.PageSize);
            Assert.AreEqual(2, CollectionView.PageIndex);
            Assert.IsNotNull(CollectionView.Groups);
        }

        /// <summary>
        /// Check that the GetEnumerator method returns all the items in the correct order in the collection.
        /// </summary>
        [TestMethod]
        [Description("Check that the GetEnumerator method returns all the items in the correct order in the collection.")]
        public void GetEnumeratorTest()
        {
            // check that we get the correct item for each index in the source collection
            IEnumerator enumerator = CollectionView.GetEnumerator();
            foreach (TestClass item in SourceCollection)
            {
                enumerator.MoveNext();
                Assert.AreEqual(item, enumerator.Current);
            }

            // check that when we have paging, the enumerator only returns the items on
            // the current page
            CollectionView.PageSize = 5;
            int count = 0;
            enumerator = CollectionView.GetEnumerator();
            while (enumerator.MoveNext())
            {
                count++;
            }
            Assert.AreEqual(5, count);

            // verify that when we have grouping, we will still fetch the correct items
            // from the group to enumerate through
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IntProperty"));

            count = 0;
            enumerator = CollectionView.GetEnumerator();
            while (enumerator.MoveNext())
            {
                // show that we are getting the items from the correct group
                Assert.AreEqual(1, (enumerator.Current as TestClass).IntProperty);
                count++;
            }
            Assert.AreEqual(5, count);
        }

        /// <summary>
        /// Check that the GetItemAt method returns the correct item at the specified index in the collection.
        /// </summary>
        [TestMethod]
        [Description("Check that the GetItemAt method returns the correct item at the specified index in the collection.")]
        public void GetItemAtTest()
        {
            // store off the first item in the list for comparing later in the test
            TestClass firstItem = CollectionView[0] as TestClass;

            // check that we get the correct item for each index in the source collection
            int index = 0;
            foreach (TestClass item in SourceCollection)
            {
                Assert.AreEqual(item, CollectionView.GetItemAt(index));
                index++;
            }

            // check that for an index not in the collection, we throw an argument
            // out of range exception
            AssertExpectedException(
                new ArgumentOutOfRangeException("index"),
                delegate
                {
                    CollectionView.GetItemAt(-1);
                });
            AssertExpectedException(
                new ArgumentOutOfRangeException("index"),
                delegate
                {
                    CollectionView.GetItemAt(25);
                });

            // now check that with paging, we return the item relative
            // to the index of the page its on.
            CollectionView.PageSize = 5;

            index = 0;
            foreach (TestClass item in SourceCollection)
            {
                Assert.AreEqual(item, CollectionView.GetItemAt(index % 5));
                index++;

                // move to the next page after we go through all the items 
                // on the current page
                if (index % 5 == 0)
                {
                    CollectionView.MoveToNextPage();
                }
            }
        }

        /// <summary>
        /// Check that the Indexer '[]' returns the correct item at the specified index in the collection.
        /// </summary>
        [TestMethod]
        [Description("Check that the Indexer '[]' returns the correct item at the specified index in the collection.")]
        public void IndexerTest()
        {
            // store off the first item in the list for comparing later in the test
            TestClass firstItem = CollectionView[0] as TestClass;

            // check that we get the correct item for each index in the source collection
            int index = 0;
            foreach (TestClass item in SourceCollection)
            {
                Assert.AreEqual(item, CollectionView[index]);
                index++;
            }

            // check that for an index not in the collection, we throw an argument
            // out of range exception
            AssertExpectedException(
                new ArgumentOutOfRangeException("index"),
                delegate
                {
                    object obj = CollectionView[-1];
                });
            AssertExpectedException(
                new ArgumentOutOfRangeException("index"),
                delegate
                {
                    object obj = CollectionView[25];
                });

            // now check that with paging, we return the item relative
            // to the index of the page its on.
            CollectionView.PageSize = 5;

            index = 0;
            foreach (TestClass item in SourceCollection)
            {
                Assert.AreEqual(item, CollectionView[index % 5]);
                index++;

                // move to the next page after we go through all the items 
                // on the current page
                if (index % 5 == 0)
                {
                    CollectionView.MoveToNextPage();
                }
            }
        }

        /// <summary>
        /// Check that the IndexOf method returns the correct index of items in the collection view.
        /// </summary>
        [TestMethod]
        [Description("Check that the IndexOf method returns the correct index of items in the collection view.")]
        public void IndexOfTest()
        {
            // store off the first item in the list for comparing later in the test
            TestClass firstItem = CollectionView[0] as TestClass;

            // check that we get the correct index for each of the items in the source collection
            int index = 0;
            foreach (TestClass item in SourceCollection)
            {
                Assert.AreEqual(index, CollectionView.IndexOf(item));
                index++;
            }

            // check that for an item not in the collection, we return -1
            Assert.AreEqual(-1, CollectionView.IndexOf(new TestClass()));

            // now check that with paging, we return the index of the item relative
            // to the page its on.
            CollectionView.PageSize = 5;

            index = 0;
            foreach (TestClass item in SourceCollection)
            {
                Assert.AreEqual(index % 5, CollectionView.IndexOf(item));
                index++;
                
                // move to the next page after we go through all the items 
                // on the current page
                if (index % 5 == 0)
                {
                    CollectionView.MoveToNextPage();
                }
            }

            // check that items that are not on the current page will
            // return an index of -1.
            Assert.AreEqual(-1, CollectionView.IndexOf(firstItem));
        }

        /// <summary>
        /// Verify that the IsEmpty property tells us when the Count is zero.
        /// </summary>
        [TestMethod]
        [Description("Verify that the IsEmpty property tells us when the Count is zero.")]
        public void IsEmptyTest()
        {
            // we have 25 items so it should not be empty
            Assert.IsFalse(CollectionView.IsEmpty);

            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {

                // remove all items and verify that it is empty
                for (int i = 0; i < 25; i++)
                {
                    CollectionView.RemoveAt(0);
                }
                Assert.IsTrue(CollectionView.IsEmpty);
            }
        }

        /// <summary>
        /// Check that the ItemCount property correctly returns the current number of items in the view.
        /// </summary>
        [TestMethod]
        [Description("Check that the ItemCount property correctly returns the current number of items in the view.")]
        public void ItemCountTest()
        {
            // we should have 25 items initially
            Assert.AreEqual(25, CollectionView.ItemCount);

            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // our count should increase when we add items
                TestClass newItem = CollectionView.AddNew() as TestClass;
                CollectionView.CommitNew();
                Assert.AreEqual(26, CollectionView.ItemCount);

                // and it should decrease when we remove items
                CollectionView.Remove(newItem);
                CollectionView.RemoveAt(0);
                Assert.AreEqual(24, CollectionView.ItemCount);

                // item count shows the total items we hold onto in our
                // collection, so even if we page, it is unaffected
                CollectionView.PageSize = 5;
                Assert.AreEqual(24, CollectionView.ItemCount);
            }
        }

        /// <summary>
        /// Verify that PageSize changes are allowed during an add/edit operation, if the operation is committed in the CurrentChanged event handler.
        /// </summary>
        [TestMethod]
        [Description("Verify that PageSize changes are allowed during an add/edit operation, if the operation is committed in the CurrentChanged event handler.")]
        public void PageSizeWithAddEditTest()
        {
            // if we don't implement IList, we cannot run the rest of
            // the test as we cannot add/remove items
            if (this.ImplementsIList)
            {
                // add a new item
                TestClass newItem = CollectionView.AddNew() as TestClass;
                Assert.IsTrue(CollectionView.IsAddingNew);

                CollectionView.CurrentChanged +=new EventHandler(CommitAddOrEditOperation);

                // verify that we can update the PageSize and that the add operation is committed
                CollectionView.PageSize = 10;
                Assert.AreEqual(10, CollectionView.PageSize);
                Assert.IsFalse(CollectionView.IsAddingNew);

                CollectionView.CurrentChanged -= new EventHandler(CommitAddOrEditOperation);

                // now edit an item
                CollectionView.EditItem(CollectionView[0]);
                Assert.IsTrue(CollectionView.IsEditingItem);

                CollectionView.CurrentChanged += new EventHandler(CommitAddOrEditOperation);

                // verify that we can update the PageSize and that the edit operation is committed
                CollectionView.PageSize = 5;
                Assert.AreEqual(5, CollectionView.PageSize);
                Assert.IsFalse(CollectionView.IsEditingItem);

                CollectionView.CurrentChanged -= new EventHandler(CommitAddOrEditOperation);
            }
        }

        /// <summary>
        /// Check that the SourceCollection property returns the collection we passed in.
        /// </summary>
        [TestMethod]
        [Description("Check that the SourceCollection property returns the collection we passed in.")]
        public void SourceCollectionTest()
        {
            Assert.AreEqual(this.SourceCollection, CollectionView.SourceCollection);
        }


        /// <summary>
        /// This method will listen to the CurrentChanged event and will commit any add or edit
        /// operations taking place in the PagedCollectionView.
        /// </summary>
        /// <param name="sender">PagedCollectionView firing the event</param>
        /// <param name="e">CurrentChanged event args</param>
        private void CommitAddOrEditOperation(object sender, EventArgs e)
        {
            PagedCollectionView pcv = sender as PagedCollectionView;
            if (pcv != null)
            {
                if (pcv.IsAddingNew)
                {
                    pcv.CommitNew();
                }
                else if (pcv.IsEditingItem)
                {
                    pcv.CommitEdit();
                }
            }
        }

        /// <summary>
        /// This method will listen to the CurrentChanging event and cancel it.
        /// </summary>
        /// <param name="sender">PagedCollectionView firing the event</param>
        /// <param name="e">CurrentChanging event args</param>
        private void CancelCurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
