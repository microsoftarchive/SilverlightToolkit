//-----------------------------------------------------------------------
// <copyright file="PagingTests.cs" company="Microsoft">
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
    /// This class runs the unit tests for paging items in a PagedCollectionView
    /// </summary>
    public class PagingTests : PagedCollectionViewTest
    {
        /// <summary>
        /// Verify currency updates when we change the PageSize.
        /// </summary>
        [TestMethod]
        [Description("Verify currency updates when we change the PageSize.")]
        public void CurrencyWithChangedPageSize()
        {
            // add a pagesize and set currency
            CollectionView.PageSize = 5;
            CollectionView.MoveCurrentToLast();
            Assert.IsTrue(CollectionView.Contains(CollectionView.CurrentItem));
            Assert.AreEqual(4, CollectionView.CurrentPosition);
            TestClass item = CollectionView.CurrentItem as TestClass;

            // change the pagesize and verify that currency has changed to the
            // first item since the item that had currency is no longer in the view
            CollectionView.PageSize = 4;
            Assert.IsTrue(CollectionView.Contains(CollectionView.CurrentItem));
            Assert.AreEqual(0, CollectionView.CurrentPosition);
            Assert.IsFalse(CollectionView.Contains(item));

            // now move currency to the last item again and set the pagesize to
            // a larger value
            CollectionView.MoveCurrentToLast();
            Assert.AreEqual(3, CollectionView.CurrentPosition);
            item = CollectionView.CurrentItem as TestClass;
            CollectionView.PageSize = 10;
            
            // verify that the currency stayed on that item, since it was still
            // in the view
            Assert.AreEqual(3, CollectionView.CurrentPosition);
            Assert.IsTrue(CollectionView.Contains(item));

            // set currency to after last and change page size to verify that it
            // will stay after last
            CollectionView.MoveCurrentToPosition(CollectionView.Count);
            Assert.IsTrue(CollectionView.IsCurrentAfterLast);
            Assert.AreEqual(10, CollectionView.CurrentPosition);
            CollectionView.PageSize = 5;

            // verify that it is still after last
            Assert.IsTrue(CollectionView.IsCurrentAfterLast);
            Assert.AreEqual(5, CollectionView.CurrentPosition);
        }

        /// <summary>
        /// Validate conditions in which we are not allowed to move to a page because an invalid index is specified.
        /// </summary>
        [TestMethod]
        [Description("Validate conditions in which we are not allowed to move to a page because an invalid index is specified.")]
        public void InvalidPageIndexTest()
        {
            // set a pagesize
            CollectionView.PageSize = 5;
            Assert.AreEqual(0, CollectionView.PageIndex);

            // try moving to a negative page index
            Assert.IsFalse(CollectionView.MoveToPage(-1));
            Assert.IsFalse(CollectionView.MoveToPage(-2));

            // verify that if we try to move to the current page,
            // then we will just return false
            Assert.IsFalse(CollectionView.MoveToPage(CollectionView.PageIndex));

            // verify that if we specify a PageIndex >= PageCount 
            // (5 in this case), then we will also return false
            Assert.IsFalse(CollectionView.MoveToPage(5));
            Assert.IsFalse(CollectionView.MoveToPage(6));
        }

        /// <summary>
        /// Verify that the methods used to change the PageIndex work correctly.
        /// </summary>
        [TestMethod]
        [Description("Verify that the methods used to change the PageIndex work correctly.")]
        public void PageMoveTest()
        {
            // set a pagesize
            CollectionView.PageSize = 5;
            Assert.AreEqual(0, CollectionView.PageIndex);

            // call MoveToNextPage
            CollectionView.MoveToNextPage();
            Assert.AreEqual(1, CollectionView.PageIndex);

            // call MoveToPreviousPage
            CollectionView.MoveToPreviousPage();
            Assert.AreEqual(0, CollectionView.PageIndex);

            // verify that if we call MoveToPreviousPage
            // when we're at the first page, it returns false
            Assert.IsFalse(CollectionView.MoveToPreviousPage());

            // call MoveToLastPage
            CollectionView.MoveToLastPage();
            Assert.AreEqual(4, CollectionView.PageIndex);

            // verify that if we call MoveToNextPage
            // when we're at the last page, it returns false
            Assert.IsFalse(CollectionView.MoveToNextPage());

            // call MoveToFirstPage
            CollectionView.MoveToFirstPage();
            Assert.AreEqual(0, CollectionView.PageIndex);

            // call MoveToPage
            CollectionView.MoveToPage(3);
            Assert.AreEqual(3, CollectionView.PageIndex);

            // call MoveToPage with a different index
            CollectionView.MoveToPage(2);
            Assert.AreEqual(2, CollectionView.PageIndex);
        }

        /// <summary>
        /// Verify that when we remove and insert a sort description, that we update the data correctly.
        /// </summary>
        [TestMethod]
        [Description("Verify that when we remove and insert a sort description, that we update the data correctly.")]
        public void PageSizeTest()
        {
            // verify that we start out with no paging => PageSize == 0
            Assert.AreEqual(0, CollectionView.PageSize);
            Assert.AreEqual(25, CollectionView.Count);

            // add a pagesize and verify
            CollectionView.PageSize = 5;
            Assert.AreEqual(5, CollectionView.PageSize);
            Assert.AreEqual(5, CollectionView.Count);

            // change the pagesize and verify
            CollectionView.PageSize = 3;
            Assert.AreEqual(3, CollectionView.PageSize);
            Assert.AreEqual(3, CollectionView.Count);

            // turn off paging and verify
            CollectionView.PageSize = 0;
            Assert.AreEqual(0, CollectionView.PageSize);
            Assert.AreEqual(25, CollectionView.Count);
        }
    }
}
