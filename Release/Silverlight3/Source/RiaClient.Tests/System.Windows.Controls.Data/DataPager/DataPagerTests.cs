//-----------------------------------------------------------------------
// <copyright file="DataPagerTests.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Controls.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Media;
    using Microsoft.Silverlight.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class runs the unit tests for DataPager.
    /// </summary>
    [TestClass]
    public class DataPagerTests : SilverlightTest
    {
        #region Constants

        private const int DefaultWaitTimeout = 500;
        private const int MinimumWaitTimeout = 250;
        private const int LegitimateWaitTimeout = 250;
        private const int VisualDelayInMilliseconds = 100;

        #endregion

        #region Fields

        private DataPagerTestSource<int> _dataPagerSource;
        private DataPager _pager;
        private PagedCollectionView _pagedCollectionView;
        private List<int> _enumerableSource;
        private List<string> _propertiesChangedList;
        private int _pageIndexChangedEventCount;
        private int _pageIndexChangingEventCount;
        private bool _pagerLoaded;
        private bool _cancelPageIndexChanging;
        private DateTime _startedWaiting;

        #endregion Fields

        #region Initialization Methods

        /// <summary>
        /// Initializes the DataPager to be used in testing.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this._cancelPageIndexChanging = false;
            this._pagerLoaded = false;
            this._pageIndexChangedEventCount = 0;
            this._pageIndexChangingEventCount = 0;
            this._pager = new DataPager();
            this._propertiesChangedList = new List<string>();
        }

        #endregion Initialization Methods

        #region Test Methods

        #region Event Tests

        /// <summary>
        /// Validate the PageIndexChanging and PageIndexChanged events on the DataPager class in typical synchronous scenario.
        /// </summary>
        [TestMethod]
        [Description("Validate the PageIndexChanging and PageIndexChanged events on the DataPager class in typical synchronous scenario.")]
        public void PageIndexChanges()
        {
            this.SetUpFuturePageIndexChangesHandling();

            this.LoadCollectionView();

            Assert.AreEqual(0, this._pager.PageIndex);
            this.AssertNoPageIndexChangeEvent(() => this._pager.PageIndex = 0);
            this.AssertBothPageIndexChangeEvents(() => this._pager.PageIndex = 1);
        }

        /// <summary>
        /// Validate the PageIndexChanging and PageIndexChanged events on the DataPager class in typical asynchronous scenario.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the PageIndexChanging and PageIndexChanged events on the DataPager class in typical asynchronous scenario.")]
        public void PageIndexChangesAsync()
        {
            this.LoadCollectionView(true, true);
            this.LoadControl();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this._pager.PageIndex);
                this.SetUpFuturePageIndexChangesHandling();
                this._pager.PageIndex = 0;
            });

            this.WaitForNoPageIndexChanges(LegitimateWaitTimeout);

            EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this._pager.PageIndex);
                this.SetUpFuturePageIndexChangesHandling();
                this._pager.PageIndex = 1;
            });

            this.WaitForPageIndexChanges();

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the PageIndexChanging and PageIndexChanged events on the DataPager class when a synchronous page move is cancelled.
        /// </summary>
        [TestMethod]
        [Description("Validate the PageIndexChanging and PageIndexChanged events on the DataPager class when page move is cancelled.")]
        public void CancelPageIndexChanges()
        {
            this.LoadCollectionView();

            this.SetUpFuturePageIndexChangesHandling();

            Assert.AreEqual(0, this._pager.PageIndex);
            this.AssertNoPageIndexChangeEvent(() => this._pager.PageIndex = 0);
            this.AssertBothPageIndexChangeEvents(() => this._pager.PageIndex = 1);

            this._cancelPageIndexChanging = true;
            // Move to PageIndex 2 should fail
            this.AssertPageIndexChangeEvents(() => this._pager.PageIndex = 2, 1, 0);
            // Move to PageIndex 2 should fail again
            this.AssertPageIndexChangeEvents(() => this._pager.PageIndex = 2, 1, 0);

            this._cancelPageIndexChanging = false;
            this.AssertBothPageIndexChangeEvents(() => this._pager.PageIndex = 2);
        }

        /// <summary>
        /// Validate the PageIndexChanging and PageIndexChanged events on the DataPager class when an asynchronous page move is cancelled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the PageIndexChanging and PageIndexChanged events on the DataPager class when page move is cancelled.")]
        public void CancelPageIndexChangesAsync()
        {
            this.LoadCollectionView(true, true);
            this.LoadControl();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this._pager.PageIndex);
                this.SetUpFuturePageIndexChangesHandling();
                this._cancelPageIndexChanging = true;
                // Try to move to PageIndex 1 a first time...
                this._pager.PageIndex = 1;
            });

            this.WaitForPageIndexChangingAlone(LegitimateWaitTimeout);

            EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this._pager.PageIndex);
                this.SetUpFuturePageIndexChangesHandling();
                this._cancelPageIndexChanging = true;
                // ...and a second time.
                this._pager.PageIndex = 1;
            });

            this.WaitForPageIndexChangingAlone(LegitimateWaitTimeout);

            EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this._pager.PageIndex);
                this.SetUpFuturePageIndexChangesHandling();
                this._cancelPageIndexChanging = false;
                // This attempt should succeed this time.
                this._pager.PageIndex = 1;
            });

            this.WaitForPageIndexChanges();

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the PageIndexChanging and PageIndexChanged events get fired when we click on a button to page
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the PageIndexChanging and PageIndexChanged events get fired when we click on a button to page.")]
        public void ButtonClickPageEvents()
        {
            this.LoadControl();

            IInvokeProvider nextPageButton = null;
            IInvokeProvider previousPageButton = null;
            IInvokeProvider lastPageButton = null;
            IInvokeProvider firstPageButton = null;

            bool makeSourceCollectionPaged = false;

            do
            {
                makeSourceCollectionPaged = !makeSourceCollectionPaged;
                this.LoadCollectionView(makeSourceCollectionPaged, true /*canPage*/);

                EnqueueCallback(() =>
                {
                    Assert.IsFalse(this._pager.IsTotalItemCountFixed);

                    nextPageButton = new ButtonAutomationPeer(this._pager.FindChildByName("NextPageButton") as Button).GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    previousPageButton = new ButtonAutomationPeer(this._pager.FindChildByName("PreviousPageButton") as Button).GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    lastPageButton = new ButtonAutomationPeer(this._pager.FindChildByName("LastPageButton") as Button).GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    firstPageButton = new ButtonAutomationPeer(this._pager.FindChildByName("FirstPageButton") as Button).GetPattern(PatternInterface.Invoke) as IInvokeProvider;

                    Assert.IsNotNull(nextPageButton);
                    Assert.IsNotNull(previousPageButton);
                    Assert.IsNotNull(lastPageButton);
                    Assert.IsNotNull(firstPageButton);
                });

                EnqueueCallback(() =>
                {
                    Assert.AreEqual(0, this._pager.PageIndex);
                    this.SetUpFuturePageIndexChangesHandling();
                    nextPageButton.Invoke();
                });

                this.WaitForPageIndexChanges();

                EnqueueCallback(() =>
                {
                    Assert.AreEqual(1, this._pager.PageIndex);
                    this.SetUpFuturePageIndexChangesHandling();
                    previousPageButton.Invoke();
                });

                this.WaitForPageIndexChanges();

                EnqueueCallback(() =>
                {
                    Assert.AreEqual(0, this._pager.PageIndex);
                    this.SetUpFuturePageIndexChangesHandling();
                    lastPageButton.Invoke();
                });

                this.WaitForPageIndexChanges();

                EnqueueCallback(() =>
                {
                    Assert.AreEqual(4, this._pager.PageIndex);
                    this.SetUpFuturePageIndexChangesHandling();
                    nextPageButton.Invoke();
                });

                this.WaitForPageIndexChanges();

                EnqueueCallback(() =>
                {
                    Assert.AreEqual(4, this._pager.PageIndex);
                    firstPageButton.Invoke();
                });

                this.WaitForPageIndexChanges();

                EnqueueCallback(() =>
                {
                    Assert.AreEqual(0, this._pager.PageIndex);
                    this._cancelPageIndexChanging = true;
                    this.SetUpFuturePageIndexChangesHandling();
                    // Attempt a first move to the next page, expecting a cancellation
                    nextPageButton.Invoke();
                });

                this.WaitForPageIndexChangingAlone(LegitimateWaitTimeout);

                EnqueueCallback(() =>
                {
                    Assert.AreEqual(0, this._pager.PageIndex);
                    this.SetUpFuturePageIndexChangesHandling();
                    // Attempt a second move to the next page, expecting a cancellation
                    nextPageButton.Invoke();
                });

                this.WaitForPageIndexChangingAlone(LegitimateWaitTimeout);

                EnqueueCallback(() =>
                {
                    Assert.AreEqual(0, this._pager.PageIndex);
                    this._cancelPageIndexChanging = false;
                    this.SetUpFuturePageIndexChangesHandling();
                    // Attempt a move to the next page, expecting a success
                    nextPageButton.Invoke();
                });

                this.WaitForPageIndexChanges();

                EnqueueCallback(() =>
                {
                    Assert.AreEqual(1, this._pager.PageIndex);
                    this.SetUpFuturePageIndexChangesHandling();
                    // Attempt a move to the previous page, expecting a success
                    previousPageButton.Invoke();
                });

                this.WaitForPageIndexChanges();
            }
            while (makeSourceCollectionPaged);

            EnqueueTestComplete();
        }

        #endregion Event Tests

        #region Public Property Tests

        /// <summary>
        /// Validate the AutoEllipsis property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the AutoEllipsis property on the DataPager class.")]
        public void AutoEllipsis()
        {
            Assert.IsFalse(this._pager.AutoEllipsis);
            this._pager.DisplayMode = PagerDisplayMode.Numeric;
            this._pager.AutoEllipsis = true;
            this._pager.NumericButtonCount = 3;
            this.LoadCollectionView();

            this.CreateAsyncTask(
                this._pager,
                delegate
                {
                    Assert.AreEqual(3, this._pager.NumericButtonCount);

                    StackPanel sp = this.GetButtonPanel();
                    Assert.AreEqual(1, (sp.Children[0] as ToggleButton).Content);
                    Assert.AreEqual(2, (sp.Children[1] as ToggleButton).Content);
                    Assert.AreEqual("...", (sp.Children[2] as ToggleButton).Content);
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the CanMoveToFirstPage property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CanMoveToFirstPage property on the DataPager class.")]
        public void CanMoveToFirstPage()
        {
            Assert.IsFalse(this._pager.CanMoveToFirstPage);
            this.LoadCollectionView();
            Assert.IsFalse(this._pager.CanMoveToFirstPage);

            this._pagedCollectionView.MoveToNextPage();
            Assert.IsTrue(this._pager.CanMoveToFirstPage);

            this._pagedCollectionView.MoveToPreviousPage();
            Assert.IsFalse(this._pager.CanMoveToFirstPage);
        }

        /// <summary>
        /// Validate the CanMoveToPreviousPage property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CanMoveToPreviousPage property on the DataPager class.")]
        public void CanMoveToPreviousPage()
        {
            Assert.IsFalse(this._pager.CanMoveToPreviousPage);
            this.LoadCollectionView();
            Assert.IsFalse(this._pager.CanMoveToPreviousPage);
            
            this._pagedCollectionView.MoveToNextPage();
            Assert.IsTrue(this._pager.CanMoveToPreviousPage);
            
            this._pagedCollectionView.MoveToPreviousPage();
            Assert.IsFalse(this._pager.CanMoveToPreviousPage);
        }

        /// <summary>
        /// Validate the CanMoveToNextPage property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the CanMoveToNextPage property on the DataPager class.")]
        public void CanMoveToNextPage()
        {
            this._pager.IsTotalItemCountFixed = true;
            Assert.IsFalse(this._pager.CanMoveToNextPage);

            this.LoadCollectionView(true, true);
            Assert.IsTrue(this._pager.CanMoveToNextPage);

            EnqueueCallback(() =>
            {
                this.SetUpFuturePageIndexChangesHandling();
                this._dataPagerSource.MoveToLastPage();
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.IsFalse(this._pager.CanMoveToNextPage);
                this.SetUpFuturePageIndexChangesHandling();
                this._dataPagerSource.MoveToPreviousPage();
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.IsTrue(this._pager.CanMoveToNextPage);
                this.SetUpFuturePageIndexChangesHandling();
                this._dataPagerSource.MoveToNextPage();
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.IsFalse(this._pager.CanMoveToNextPage);

                this._dataPagerSource.TotalItemCount = -1;
                Assert.IsTrue(this._pager.CanMoveToNextPage);

                this._pager.IsTotalItemCountFixed = false;
                Assert.IsTrue(this._pager.CanMoveToNextPage);

                this._dataPagerSource.TotalItemCount = 100;
                Assert.IsTrue(this._pager.CanMoveToNextPage);
            });

            EnqueueTestComplete();
        }


        /// <summary>
        /// Validate the CanMoveToLastPage property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the CanMoveToLastPage property on the DataPager class.")]
        public void CanMoveToLastPage()
        {
            this._pager.IsTotalItemCountFixed = true;
            Assert.IsFalse(this._pager.CanMoveToLastPage);

            this.LoadCollectionView(true, true);
            Assert.IsTrue(this._pager.CanMoveToLastPage);

            EnqueueCallback(() =>
            {
                this.SetUpFuturePageIndexChangesHandling();
                this._dataPagerSource.MoveToLastPage();
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.IsFalse(this._pager.CanMoveToLastPage);
                this.SetUpFuturePageIndexChangesHandling();
                this._dataPagerSource.MoveToPreviousPage();
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.IsTrue(this._pager.CanMoveToLastPage);
                this._dataPagerSource.TotalItemCount = -1;
                Assert.IsFalse(this._pager.CanMoveToLastPage);

                this._pager.IsTotalItemCountFixed = false;
                Assert.IsFalse(this._pager.CanMoveToLastPage);

                this._dataPagerSource.TotalItemCount = 100;
                Assert.IsTrue(this._pager.CanMoveToLastPage);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the CanPage property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Description("Validate the CanPage property on the DataPager class.")]
        public void CanPage()
        {
            this.LoadCollectionView(true, false);
            Assert.IsFalse(this._pager.CanChangePage);

            this.LoadCollectionView(true, true);
            Assert.IsTrue(this._pager.CanChangePage);
        }

        /// <summary>
        /// Validate the ItemCount property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Description("Validate the ItemCount property on the DataPager class.")]
        public void ItemCount()
        {
            Assert.AreEqual(0, this._pager.ItemCount);
            
            this.LoadCollectionView();
            Assert.AreEqual(25, this._pager.ItemCount);
            
            this._pagedCollectionView.RemoveAt(0);
            Assert.AreEqual(24, this._pager.ItemCount);
        }

        /// <summary>
        /// Validate the Source property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Description("Validate the Source property on the DataPager class.")]
        public void Source()
        {
            Assert.IsNull(this._pager.Source);
            
            this.LoadCollectionView();
            this._pagedCollectionView.MoveToPage(1);
            Assert.AreEqual(this._pagedCollectionView, this._pager.Source);
            Assert.AreEqual(this._pagedCollectionView.ItemCount, this._pager.ItemCount);
            Assert.AreEqual(this._pagedCollectionView.PageIndex, this._pager.PageIndex);
            Assert.AreEqual(this._pagedCollectionView.PageSize, this._pager.PageSize);
        }

        /// <summary>
        /// Ensure that setting the Source property does not prevent the DataPager from being garbage-collected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that setting the Source property does not prevent the DataPager from being garbage-collected.")]
        public void DataPagerCanBeCollected()
        {
            WeakReference dataPagerReference = new WeakReference(this._pager);
            Assert.IsTrue(dataPagerReference.IsAlive);

            this.LoadCollectionView();
            this.LoadControl();

            this.EnqueueCallback(() =>
            {
                this.TestPanel.Children.Remove(this._pager);
                this._pager = null;
            });

            // 


            this.EnqueueCallback(() =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            });

            this.EnqueueCallback(() =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Assert.IsFalse(dataPagerReference.IsAlive);
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the NumericButtonCount property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the NumericButtonCount property on the DataPager class.")]
        public void NumericButtonCount()
        {
            this.CreateAsyncTask(
                this._pager,
                delegate
                {
                    this.LoadCollectionView();
                    this._pager.DisplayMode = PagerDisplayMode.Numeric;
                    Assert.AreEqual(5, this._pager.NumericButtonCount);

                    this._pager.NumericButtonCount = 3;
                    Assert.AreEqual(3, this._pager.NumericButtonCount);

                    StackPanel sp = this.GetButtonPanel();
                    Assert.AreEqual(3, sp.Children.Count);

                    this._pager.NumericButtonCount = 0;
                    Assert.AreEqual(0, this._pager.NumericButtonCount);

                    AssertExpectedException(
                        new ArgumentOutOfRangeException(
                            "value",
                            string.Format(
                                CultureInfo.InvariantCulture,
                                PagerResources.ValueMustBeGreaterThanOrEqualTo,
                                "NumericButtonCount",
                                0)),
                        () =>
                        {
                            this._pager.NumericButtonCount = -1;
                        });
                    Assert.AreEqual(0, this._pager.NumericButtonCount);
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the PageCount property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Description("Validate the PageCount property on the DataPager class.")]
        public void PageCount()
        {
            Assert.AreEqual(0, this._pager.PageCount);
            
            this.LoadCollectionView();
            Assert.AreEqual(5, this._pager.PageCount);
            
            this._pager.PageSize = 7;
            Assert.AreEqual(4, this._pager.PageCount);

            this._pager.Source = new PagedCollectionView(new List<int>());
            Assert.AreEqual(1, this._pager.PageCount);

            this._pager.Source = null;
            Assert.AreEqual(0, this._pager.PageCount);
        }

        /// <summary>
        /// Validate the PageIndex property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Description("Validate the PageIndex property on the DataPager class.")]
        public void PageIndex()
        {
            Assert.AreEqual(-1, this._pager.PageIndex);
            AssertExpectedException(
                new ArgumentOutOfRangeException(
                    "value",
                    PagerResources.PageIndexMustBeNegativeOne),
                () =>
                {
                    this._pager.PageIndex = 10;
                });
            this.LoadCollectionView();
            Assert.AreEqual(0, this._pager.PageIndex);
            Assert.AreEqual(0, this._pagedCollectionView[0]);
            Assert.AreEqual(1, this._pagedCollectionView[1]);
            Assert.AreEqual(2, this._pagedCollectionView[2]);
            Assert.AreEqual(3, this._pagedCollectionView[3]);
            Assert.AreEqual(4, this._pagedCollectionView[4]);

            this.SetUpFuturePageIndexChangesHandling();

            this._cancelPageIndexChanging = true;
            this.AssertPageIndexChangeEvents(() => this._pager.PageIndex = 1, 1, 0);
            Assert.AreEqual(0, this._pager.PageIndex);
            Assert.AreEqual(0, this._pagedCollectionView[0]);
            Assert.AreEqual(1, this._pagedCollectionView[1]);
            Assert.AreEqual(2, this._pagedCollectionView[2]);
            Assert.AreEqual(3, this._pagedCollectionView[3]);
            Assert.AreEqual(4, this._pagedCollectionView[4]);
            this._cancelPageIndexChanging = false;

            this.AssertBothPageIndexChangeEvents(() => this._pager.PageIndex = 1);
            Assert.AreEqual(1, this._pager.PageIndex);
            Assert.AreEqual(5, this._pagedCollectionView[0]);
            Assert.AreEqual(6, this._pagedCollectionView[1]);
            Assert.AreEqual(7, this._pagedCollectionView[2]);
            Assert.AreEqual(8, this._pagedCollectionView[3]);
            Assert.AreEqual(9, this._pagedCollectionView[4]);

            this.AssertBothPageIndexChangeEvents(() => this._pager.PageIndex = 2);
            Assert.AreEqual(2, this._pager.PageIndex);
            Assert.AreEqual(10, this._pagedCollectionView[0]);
            Assert.AreEqual(11, this._pagedCollectionView[1]);
            Assert.AreEqual(12, this._pagedCollectionView[2]);
            Assert.AreEqual(13, this._pagedCollectionView[3]);
            Assert.AreEqual(14, this._pagedCollectionView[4]);

            this.AssertBothPageIndexChangeEvents(() => this._pager.PageIndex = 3);
            Assert.AreEqual(3, this._pager.PageIndex);
            Assert.AreEqual(15, this._pagedCollectionView[0]);
            Assert.AreEqual(16, this._pagedCollectionView[1]);
            Assert.AreEqual(17, this._pagedCollectionView[2]);
            Assert.AreEqual(18, this._pagedCollectionView[3]);
            Assert.AreEqual(19, this._pagedCollectionView[4]);

            // Expecting both a PageIndexChanging and PageIndexChanged event for with move attempt
            this.AssertBothPageIndexChangeEvents(() => this._pager.PageIndex = 100);
        }

        /// <summary>
        /// Validate the PageIndex property on the DataPager class when we hook it up to a data source that is not an IPagedCollectionView.
        /// </summary>
        [TestMethod]
        [Description("Validate the PageIndex property on the DataPager class when we hook it up to a data source that is not an IPagedCollectionView.")]
        public void PageIndexWithInvalidDataSource()
        {
            this.LoadCollectionView();
            Assert.AreEqual(0, this._pager.PageIndex);
            this._pager.PageIndex = 3;
            Assert.AreEqual(25, this._pager.ItemCount);
            Assert.AreEqual(5, this._pager.PageCount);
            Assert.AreEqual(5, this._pager.PageSize);
            Assert.AreEqual(3, this._pager.PageIndex);

            this._pager.Source = null;
            Assert.AreEqual(0, this._pager.ItemCount);
            Assert.AreEqual(0, this._pager.PageCount);
            Assert.AreEqual(5, this._pager.PageSize);
            Assert.AreEqual(-1, this._pager.PageIndex);
        }

        /// <summary>
        /// Validate the PageIndex property on the DataPager class when we hook up multiple DataPagers to the same source.
        /// </summary>
        [TestMethod]
        [Description("Validate the PageIndex property on the DataPager class when we hook up multiple DataPagers to the same source.")]
        public void PageIndexWithMultipleDataPagers()
        {
            this.LoadCollectionView();
            Assert.AreEqual(0, this._pager.PageIndex);
            this._pager.PageIndex = 3;
            Assert.AreEqual(3, this._pager.PageIndex);

            // There was previously a bug where once we hooked up a
            // second DataPager to a source, the PageIndex would
            // not get updated right away. We want to test to make 
            // sure that this is no longer the case.
            DataPager secondDataPager = new DataPager();
            Assert.AreEqual(-1, secondDataPager.PageIndex);
            
            secondDataPager.Source = this._pagedCollectionView;
            Assert.AreEqual(3, secondDataPager.PageIndex);
        }

        /// <summary>
        /// Validate the DisplayMode property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the DisplayMode property on the DataPager class.")]
        public void DisplayMode()
        {
            this.CreateAsyncTask(
                this._pager,
                delegate
                {
                    // By default starts out in FirstLastPreviousNext
                    Assert.AreEqual(PagerDisplayMode.FirstLastPreviousNext, this._pager.DisplayMode);

                    StackPanel sp = this.GetButtonPanel();
                    Assert.AreEqual(Visibility.Collapsed, sp.Visibility);

                    // Numeric Mode
                    this._pager.DisplayMode = PagerDisplayMode.Numeric;
                    Assert.AreEqual(Visibility.Visible, sp.Visibility);

                    // FirstLastPreviousNextNumeric
                    this._pager.DisplayMode = PagerDisplayMode.FirstLastPreviousNextNumeric;
                    Assert.AreEqual(Visibility.Visible, sp.Visibility);

                    // PreviousNext
                    this._pager.DisplayMode = PagerDisplayMode.PreviousNext;
                    Assert.AreEqual(Visibility.Collapsed, sp.Visibility);

                    // FirstLastNumeric Mode
                    this._pager.DisplayMode = PagerDisplayMode.FirstLastNumeric;
                    Assert.AreEqual(Visibility.Visible, sp.Visibility);

                    // PreviousNextNumeric Mode
                    this._pager.DisplayMode = PagerDisplayMode.PreviousNextNumeric;
                    Assert.AreEqual(Visibility.Visible, sp.Visibility);

                    AssertExpectedException(
                        new ArgumentException(
                            string.Format(CultureInfo.InvariantCulture,
                                Resource.InvalidEnumArgumentException_InvalidEnumArgument,
                                "value",
                                "100",
                                typeof(PagerDisplayMode).Name)),
                        () =>
                        {
                            this._pager.DisplayMode = (PagerDisplayMode)100;
                        });
                });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the PageSize property on the DataPager class.
        /// </summary>
        [TestMethod]
        [Description("Validate the PageSize property on the DataPager class.")]
        public void PageSize()
        {
            Assert.AreEqual(0, this._pager.PageSize);
            this.LoadCollectionView();
            Assert.AreEqual(5, this._pager.PageSize);
            Assert.AreEqual(5, this._pagedCollectionView.Count);
            this._pager.PageSize = 6;
            Assert.AreEqual(6, this._pager.PageSize);
            Assert.AreEqual(6, this._pagedCollectionView.Count);
            this._pager.PageIndex = 4;
            Assert.AreEqual(1, this._pagedCollectionView.Count);

            this._pager.Source = null;
            this._pager.PageSize = 0;
            PagedCollectionView pagedCollectionView = new PagedCollectionView(new List<int>());
            pagedCollectionView.PageSize = 10;
            this._pager.Source = pagedCollectionView;
            Assert.AreEqual(0, pagedCollectionView.PageIndex);
            Assert.AreEqual(10, pagedCollectionView.PageSize);
            this._pager.PageSize = 0;
            Assert.AreEqual(-1, pagedCollectionView.PageIndex);
            Assert.AreEqual(0, pagedCollectionView.PageSize);
        }

        /// <summary>
        /// Test a Source of type IEnumerable.
        /// </summary>
        [TestMethod]
        [Description("Test a Source of type IEnumerable.")]
        public void EnumerableSource()
        {
            Assert.IsNull(this._pager.Source);

            this.LoadEnumerable();

            // DataPager.ItemCount == IEnumerable's count
            Assert.AreEqual(this._enumerableSource.Count, this._pager.ItemCount);

            // DataPager.PageIndex is -1 when PageSize is 0
            Assert.AreEqual(0, this._pager.PageSize);
            Assert.AreEqual(-1, this._pager.PageIndex);

            // DataPager.CanChangePage is false when PageSize is 0. All CanMove* too
            Assert.AreEqual(false, this._pager.CanChangePage);
            Assert.AreEqual(false, this._pager.CanMoveToFirstPage);
            Assert.AreEqual(false, this._pager.CanMoveToLastPage);
            Assert.AreEqual(false, this._pager.CanMoveToPreviousPage);
            Assert.AreEqual(false, this._pager.CanMoveToNextPage);

            // Changing DataPager.IsTotalItemCountFixed is still allowed
            Assert.AreEqual(false, this._pager.IsTotalItemCountFixed);
            this._pager.IsTotalItemCountFixed = true;
            Assert.AreEqual(true, this._pager.IsTotalItemCountFixed);

            // Changing PageIndex to something other than -1 is not allowed
            AssertExpectedException(
                new ArgumentOutOfRangeException(
                    "value",
                    PagerResources.PageIndexMustBeNegativeOne),
                () =>
                {
                    this._pager.PageIndex = 0;
                });

            // Setting a positive PageSize turns CanChangePage to true
            this._pager.PageSize = 2;
            Assert.AreEqual(2, this._pager.PageSize);
            Assert.AreEqual(0, this._pager.PageIndex);
            Assert.AreEqual(true, this._pager.CanChangePage);

            // All CanMove* are still false
            Assert.AreEqual(false, this._pager.CanMoveToFirstPage);
            Assert.AreEqual(false, this._pager.CanMoveToLastPage);
            Assert.AreEqual(false, this._pager.CanMoveToPreviousPage);
            Assert.AreEqual(false, this._pager.CanMoveToNextPage);

            // Setting PageIndex > 0 fails silently (same as when Source is an IPagedCollectionView)
            this._pager.PageIndex = 1;
            Assert.AreEqual(0, this._pager.PageIndex);

            // Changing to a numeric display is allowed
            this._pager.DisplayMode = PagerDisplayMode.FirstLastNumeric;

            // Going back to PageSize = 0 is allowed
            this._pager.PageSize = 0;
            Assert.AreEqual(0, this._pager.PageSize);
            Assert.AreEqual(-1, this._pager.PageIndex);
        }

        /// <summary>
        /// Changes the Source type between IEnumerable and IPagedCollectionView.
        /// </summary>
        [TestMethod]
        [Description("Changes the Source type between IEnumerable and IPagedCollectionView.")]
        public void SwitchSourceType()
        {
            Assert.IsNull(this._pager.Source);

            this._pager.PageSize = 1;
            this.LoadEnumerable();
            this.LoadCollectionView();

            // DataPager.ItemCount == IPagedCollectionView's count
            Assert.AreEqual(this._pagedCollectionView.ItemCount, this._pager.ItemCount);

            // DataPager.PageIndex is 0 when PageSize > 0
            Assert.AreEqual(0, this._pager.PageIndex);

            this._pager.PageIndex = 10;
            this.LoadEnumerable();

            // DataPager.PageIndex went back to 0
            Assert.AreEqual(0, this._pager.PageIndex);

            this._pager.Source = null;

            // DataPager.PageIndex went back to -1
            Assert.AreEqual(-1, this._pager.PageIndex);

            this.LoadCollectionView();

            this._pager.PageIndex = 10;

            // Load an IPagedCollectionView again and move from PageIndex 10 back to 0
            this.LoadCollectionView();

            this.LoadEnumerable();

            // Switch source type when PageSize is 0
            this._pager.PageSize = 0;
            this.LoadCollectionView();
            this.LoadEnumerable();
        }

        #endregion Public Property Tests

        #region UI Element Tests

        /// <summary>
        /// Validate the first page button on the DataPager class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the first page button method on the DataPager class.")]
        public void MoveToFirstPage()
        {
            ButtonBase firstPageButton;
            ButtonBaseAutomationPeer firstPageButtonBaseAutomationPeer;
            IInvokeProvider firstPageProvider = null;

            this.LoadCollectionView();
            this.LoadControl();

            EnqueueCallback(() =>
            {
                this._pagedCollectionView.MoveToPage(2);
                this.SetUpFuturePageIndexChangesHandling();
                firstPageButton = this._pager.FindChildByName("FirstPageButton") as ButtonBase;
                firstPageButtonBaseAutomationPeer = ButtonBaseAutomationPeer.CreatePeerForElement(firstPageButton) as ButtonBaseAutomationPeer;
                firstPageProvider = firstPageButtonBaseAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                firstPageProvider.Invoke();
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this._pager.PageIndex);
                AssertExpectedException(
                    new ElementNotEnabledException(),
                    () =>
                    {
                        firstPageProvider.Invoke();
                    });
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the previous page button on the DataPager class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the previous page method on the DataPager class.")]
        public void MoveToPreviousPage()
        {
            ButtonBase previousPageButton;
            ButtonBaseAutomationPeer previousPageButtonBaseAutomationPeer;
            IInvokeProvider previousPageProvider = null;

            this.LoadCollectionView();
            this.LoadControl();

            EnqueueCallback(() =>
            {
                this._pagedCollectionView.MoveToPage(2);
                this.SetUpFuturePageIndexChangesHandling();
                previousPageButton = this._pager.FindChildByName("PreviousPageButton") as ButtonBase;
                previousPageButtonBaseAutomationPeer = ButtonBaseAutomationPeer.CreatePeerForElement(previousPageButton) as ButtonBaseAutomationPeer;
                previousPageProvider = previousPageButtonBaseAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                previousPageProvider.Invoke();
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(1, this._pager.PageIndex);
                this.SetUpFuturePageIndexChangesHandling();
                previousPageProvider.Invoke();
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this._pager.PageIndex);
                AssertExpectedException(
                    new ElementNotEnabledException(),
                    () =>
                    {
                        previousPageProvider.Invoke();
                    });
            });

            EnqueueTestComplete();
        }

        //// 


        /// <summary>
        /// Validate the current page text box on the DataPager class.
        /// </summary>
        [TestMethod]
        [Ignore]
        [Asynchronous]
        [Description("Validate the current page text box on the DataPager class.")]
        public void ChangeCurrentPage()
        {
            TextBox currentPageTextBox = null;
            TextBoxAutomationPeer currentPageTextBoxAutomationPeer;
            IValueProvider currentPageValueProvider = null;

            this.LoadCollectionView();
            this.LoadControl();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this._pager.PageIndex);
                Assert.AreEqual(5, this._pager.PageCount);
                this.SetUpFuturePageIndexChangedHandling();
                currentPageTextBox = _pager.FindChildByName("CurrentPageTextBox") as TextBox;
                currentPageTextBoxAutomationPeer = TextBoxAutomationPeer.CreatePeerForElement(currentPageTextBox) as TextBoxAutomationPeer;
                currentPageValueProvider = currentPageTextBoxAutomationPeer.GetPattern(PatternInterface.Value) as IValueProvider;
                this.SetUpFuturePageIndexChangedHandling();
                //Changes to page 1
                currentPageValueProvider.SetValue((2).ToString(CultureInfo.CurrentCulture));
                Assert.AreEqual("2", currentPageValueProvider.Value, "TextBox Text should be 2");
            });

            this.WaitForNoPageIndexChanges(LegitimateWaitTimeout);

            EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this._pager.PageIndex);
                this.SetUpFuturePageIndexChangesHandling();
                //Changes to page 2.
                currentPageValueProvider.SetValue((3).ToString(CultureInfo.CurrentCulture));
                Assert.AreEqual("3", currentPageValueProvider.Value);
            });

            this.WaitForPageIndexChanges(1000);

            EnqueueCallback(() =>
            {
                Assert.AreEqual(2, this._pager.PageIndex, "DataPager PageIndex should be 2");
                this.SetUpFuturePageIndexChangesHandling();
                //Changes to page 4.
                currentPageValueProvider.SetValue((5).ToString(CultureInfo.CurrentCulture));
                Assert.AreEqual("5", currentPageValueProvider.Value, "TextBox Text should be 5");
                Assert.AreEqual(2, this._pager.PageIndex, "DataPager PageIndex should still be 2");
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(4, this._pager.PageIndex);
                //Changes current page to page 2.
                currentPageValueProvider.SetValue((3).ToString(CultureInfo.CurrentCulture));
                ButtonBase previousPageButton = this._pager.FindChildByName("PreviousPageButton") as ButtonBase;
                Assert.AreEqual(4, this._pager.PageIndex);
                bool focusSuccessful = previousPageButton.Focus();
                Assert.IsTrue(focusSuccessful);
                Assert.AreEqual(4, this._pager.PageIndex);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the next page button on the DataPager class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the next page method on the DataPager class.")]
        public void MoveToNextPage()
        {
            ButtonBase nextPageButton;
            ButtonBaseAutomationPeer nextPageButtonBaseAutomationPeer;
            IInvokeProvider nextPageProvider = null;

            this.LoadCollectionView();
            this.LoadControl();
            this._pager.IsTotalItemCountFixed = true;

            EnqueueCallback(() =>
            {
                this.SetUpFuturePageIndexChangesHandling();
                nextPageButton = this._pager.FindChildByName("NextPageButton") as ButtonBase;
                nextPageButtonBaseAutomationPeer = ButtonBaseAutomationPeer.CreatePeerForElement(nextPageButton) as ButtonBaseAutomationPeer;
                nextPageProvider = nextPageButtonBaseAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                nextPageProvider.Invoke();
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(1, this._pager.PageIndex);
                this._pagedCollectionView.MoveToPage(this._pager.PageCount - 2);
                this.SetUpFuturePageIndexChangesHandling();
                nextPageProvider.Invoke();
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(this._pager.PageCount - 1, this._pager.PageIndex);
                AssertExpectedException(
                    new ElementNotEnabledException(),
                    () =>
                    {
                        nextPageProvider.Invoke();
                    });
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Validate the last page button on the DataPager class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Validate the last page method on the DataPager class.")]
        public void MoveToLastPage()
        {
            ButtonBase lastPageButton;
            ButtonBaseAutomationPeer lastPageButtonBaseAutomationPeer;
            IInvokeProvider lastPageProvider = null;

            this.LoadCollectionView();
            this.LoadControl();
            this._pager.IsTotalItemCountFixed = true;

            EnqueueCallback(() =>
            {
                this.SetUpFuturePageIndexChangesHandling();
                lastPageButton = this._pager.FindChildByName("LastPageButton") as ButtonBase;
                lastPageButtonBaseAutomationPeer = ButtonBaseAutomationPeer.CreatePeerForElement(lastPageButton) as ButtonBaseAutomationPeer;
                lastPageProvider = lastPageButtonBaseAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                lastPageProvider.Invoke();
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(this._pager.PageCount - 1, this._pager.PageIndex);
                AssertExpectedException(
                    new ElementNotEnabledException(),
                    () =>
                    {
                        lastPageProvider.Invoke();
                    });
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Make sure the LastPage button on the DataPager is enabled even when IsTotalItemCountFixed is false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Make sure the LastPage button on the DataPager is enabled even when IsTotalItemCountFixed is false.")]
        public void CanMoveToLastPageWithVariableTotalItemCount()
        {
            ButtonBase lastPageButton;
            ButtonBaseAutomationPeer lastPageButtonBaseAutomationPeer;
            IInvokeProvider lastPageProvider = null;

            this.LoadCollectionView();
            this.LoadControl();
            Assert.IsFalse(this._pager.IsTotalItemCountFixed);

            EnqueueCallback(() =>
            {
                lastPageButton = this._pager.FindChildByName("LastPageButton") as ButtonBase;
                lastPageButtonBaseAutomationPeer = ButtonBaseAutomationPeer.CreatePeerForElement(lastPageButton) as ButtonBaseAutomationPeer;
                lastPageProvider = lastPageButtonBaseAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                Assert.AreNotEqual(this._pager.PageCount - 1, this._pager.PageIndex);
                lastPageProvider.Invoke();
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Set the DataPager's Background and Foreground brushes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Set the DataPager's Background and Foreground brushes.")]
        public void SetDataPagerBackAndForegroundProperties()
        {
            this.LoadCollectionView();
            this.LoadControl();

            EnqueueCallback(() =>
            {
                this._pager.DisplayMode = PagerDisplayMode.FirstLastPreviousNextNumeric;

                // Use custom colors
                this._pager.Foreground = new SolidColorBrush(Color.FromArgb(255, 100, 132, 34));
                this._pager.Background = new SolidColorBrush(Color.FromArgb(255, 222, 215, 136));

                DataPagerAutomationPeer dataPagerAutomationPeer = DataPagerAutomationPeer.CreatePeerForElement(this._pager) as DataPagerAutomationPeer;
                Assert.IsNotNull(dataPagerAutomationPeer);

                List<AutomationPeer> children = dataPagerAutomationPeer.GetChildren();
                Assert.IsInstanceOfType(children[2], typeof(ToggleButtonAutomationPeer));

                // Check that the numeric toggle button uses the same foreground brush
                Assert.AreEqual(this._pager.Foreground, ((ToggleButton)((FrameworkElementAutomationPeer)children[2]).Owner).Foreground);

                // Reset to default colors
                this._pager.SetValue(DataPager.ForegroundProperty, DependencyProperty.UnsetValue);
                this._pager.SetValue(DataPager.BackgroundProperty, DependencyProperty.UnsetValue);

                // Check that the numeric toggle button uses the same default foreground brush
                Assert.AreEqual(this._pager.Foreground, ((ToggleButton)((FrameworkElementAutomationPeer)children[2]).Owner).Foreground);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Switch the DataPager's IsEnabled property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Switches the DataPager's IsEnabled property.")]
        public void SetDataPagerIsEnabledProperty()
        {
            this.LoadCollectionView();
            this.LoadControl();

            EnqueueCallback(() =>
            {
                // Use custom colors
                this._pager.IsEnabled = false;
                Assert.IsFalse(this._pager.IsEnabled);
                this._pager.IsEnabled = true;
                Assert.IsTrue(this._pager.IsEnabled);
            });

            EnqueueTestComplete();
        }

        #endregion UI Element Tests

        #region Automation Peer Tests

        /// <summary>
        /// Make sure the DataPager exposes a DataPagerAutomationPeer with a IRangeValueProvider pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Make sure the DataPager exposes a DataPagerAutomationPeer with a IRangeValueProvider pattern.")]
        public void AccessDataPagerAutomationPeer()
        {
            TextBlock tbLabel = new TextBlock();
            tbLabel.Text = "Data Paging Control";

            this.LoadControl();
            this.TestPanel.Children.Add(tbLabel);

            EnqueueCallback(() =>
            {
                DataPagerAutomationPeer dataPagerAutomationPeer = DataPagerAutomationPeer.CreatePeerForElement(this._pager) as DataPagerAutomationPeer;
                Assert.IsNotNull(dataPagerAutomationPeer);

                IRangeValueProvider rangeValueProvider = dataPagerAutomationPeer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
                Assert.IsNotNull(rangeValueProvider);

                Assert.AreEqual(AutomationControlType.Spinner, dataPagerAutomationPeer.GetAutomationControlType());
                Assert.AreEqual("DataPager", dataPagerAutomationPeer.GetClassName());
                Assert.AreEqual("DataPager", dataPagerAutomationPeer.GetName());
                AutomationProperties.SetLabeledBy(this._pager, tbLabel);
                Assert.AreEqual("Data Paging Control", dataPagerAutomationPeer.GetName());
                AutomationProperties.SetLabeledBy(this._pager, null);
                this._pager.Name = "pager";
                Assert.AreEqual("pager", dataPagerAutomationPeer.GetName());

                Assert.IsTrue(dataPagerAutomationPeer.IsContentElement());
                Assert.IsTrue(dataPagerAutomationPeer.IsControlElement());

                Assert.IsTrue(rangeValueProvider.IsReadOnly);

                Assert.AreEqual(1, rangeValueProvider.LargeChange);
                Assert.AreEqual(1, rangeValueProvider.SmallChange);
                Assert.AreEqual(-1, rangeValueProvider.Minimum);
                Assert.AreEqual(-1, rangeValueProvider.Maximum);
                Assert.AreEqual(-1, rangeValueProvider.Value);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the DataPagerAutomationPeer with a IEnumerable Source.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the DataPagerAutomationPeer with a IEnumerable Source.")]
        public void TestDataPagerAutomationPeerWithEnumerableSource()
        {
            this.LoadControl();
            this.LoadEnumerable();
            this._pager.PageSize = 1;

            EnqueueCallback(() =>
            {
                DataPagerAutomationPeer dataPagerAutomationPeer = DataPagerAutomationPeer.CreatePeerForElement(this._pager) as DataPagerAutomationPeer;
                Assert.IsNotNull(dataPagerAutomationPeer);

                IRangeValueProvider rangeValueProvider = dataPagerAutomationPeer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
                Assert.IsNotNull(rangeValueProvider);

                Assert.IsFalse(rangeValueProvider.IsReadOnly);

                Assert.AreEqual(1, rangeValueProvider.LargeChange);
                Assert.AreEqual(1, rangeValueProvider.SmallChange);
                Assert.AreEqual(1, rangeValueProvider.Minimum);
                Assert.AreEqual(1, rangeValueProvider.Maximum);
                Assert.AreEqual(1, rangeValueProvider.Value);

                // Name is "Page 1 of 1"
                Assert.AreEqual(string.Format(
                    CultureInfo.InvariantCulture,
                    PagerResources.AutomationPeerName_TotalPageCountKnown,
                    "1", "1"),
                    dataPagerAutomationPeer.GetName());

                // Can't move to page #2 when source is IEnumerable
                rangeValueProvider.SetValue(2);
                Assert.AreEqual(1, rangeValueProvider.Value);

                this._pager.DisplayMode = PagerDisplayMode.Numeric;
                List<AutomationPeer> children = dataPagerAutomationPeer.GetChildren();
                Assert.AreEqual(8, children.Count);

                // Verify type of children
                Assert.IsInstanceOfType(children[0], typeof(ButtonAutomationPeer));
                Assert.IsInstanceOfType(children[1], typeof(ButtonAutomationPeer));
                Assert.IsInstanceOfType(children[2], typeof(ToggleButtonAutomationPeer));
                Assert.IsInstanceOfType(children[3], typeof(TextBlockAutomationPeer));
                Assert.IsInstanceOfType(children[4], typeof(TextBoxAutomationPeer));
                Assert.IsInstanceOfType(children[5], typeof(TextBlockAutomationPeer));
                Assert.IsInstanceOfType(children[6], typeof(ButtonAutomationPeer));
                Assert.IsInstanceOfType(children[7], typeof(ButtonAutomationPeer));

                // Verify AutomationId of children
                Assert.AreEqual("LargeDecrement", children[0].GetAutomationId());
                Assert.AreEqual("SmallDecrement", children[1].GetAutomationId());
                Assert.AreEqual("MoveToPage1", children[2].GetAutomationId());
                Assert.AreEqual("CurrentPage", children[4].GetAutomationId());
                Assert.AreEqual("SmallIncrement", children[6].GetAutomationId());
                Assert.AreEqual("LargeIncrement", children[7].GetAutomationId());
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the DataPagerAutomationPeer with a PagedCollectionView Source.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the DataPagerAutomationPeer with a PagedCollectionView Source.")]
        public void TestDataPagerAutomationPeerWithPagedCollectionView()
        {
            this.LoadControl();
            this.LoadCollectionView();

            EnqueueCallback(() =>
            {
                DataPagerAutomationPeer dataPagerAutomationPeer = DataPagerAutomationPeer.CreatePeerForElement(this._pager) as DataPagerAutomationPeer;
                Assert.IsNotNull(dataPagerAutomationPeer);

                IRangeValueProvider rangeValueProvider = dataPagerAutomationPeer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
                Assert.IsNotNull(rangeValueProvider);

                Assert.IsFalse(rangeValueProvider.IsReadOnly);

                Assert.AreEqual(this._pager.PageCount, rangeValueProvider.LargeChange);
                Assert.AreEqual(1, rangeValueProvider.SmallChange);
                Assert.AreEqual(1, rangeValueProvider.Minimum);
                Assert.AreEqual(this._pager.PageCount + 1, rangeValueProvider.Maximum);
                Assert.AreEqual(1, rangeValueProvider.Value);

                // Name is "Page 1 of {PageCount}"
                Assert.AreEqual(string.Format(
                    CultureInfo.InvariantCulture,
                    PagerResources.AutomationPeerName_TotalPageCountKnown,
                    "1", this._pager.PageCount.ToString(CultureInfo.CurrentCulture)),
                    dataPagerAutomationPeer.GetName());

                // Can move to page #2 when source is IPagedCollectionView
                rangeValueProvider.SetValue(2);
                Assert.AreEqual(2, rangeValueProvider.Value);

                this._pager.NumericButtonCount = 2;
                List<AutomationPeer> children = dataPagerAutomationPeer.GetChildren();
                Assert.AreEqual(9, children.Count);

                // Verify type of children
                Assert.IsInstanceOfType(children[0], typeof(ButtonAutomationPeer));
                Assert.IsInstanceOfType(children[1], typeof(ButtonAutomationPeer));
                Assert.IsInstanceOfType(children[2], typeof(ToggleButtonAutomationPeer));
                Assert.IsInstanceOfType(children[3], typeof(ToggleButtonAutomationPeer));
                Assert.IsInstanceOfType(children[4], typeof(TextBlockAutomationPeer));
                Assert.IsInstanceOfType(children[5], typeof(TextBoxAutomationPeer));
                Assert.IsInstanceOfType(children[6], typeof(TextBlockAutomationPeer));
                Assert.IsInstanceOfType(children[7], typeof(ButtonAutomationPeer));
                Assert.IsInstanceOfType(children[8], typeof(ButtonAutomationPeer));

                // Verify AutomationId of children
                Assert.AreEqual("LargeDecrement", children[0].GetAutomationId());
                Assert.AreEqual("SmallDecrement", children[1].GetAutomationId());
                Assert.AreEqual("MoveToPage1", children[2].GetAutomationId());
                Assert.AreEqual("MoveToPage2", children[3].GetAutomationId());
                Assert.AreEqual("CurrentPage", children[5].GetAutomationId());
                Assert.AreEqual("SmallIncrement", children[7].GetAutomationId());
                Assert.AreEqual("LargeIncrement", children[8].GetAutomationId());

                this._pager.DisplayMode = PagerDisplayMode.Numeric;
                // Access automation peer of the first toggle button
                ToggleButtonAutomationPeer tbtnAutomationPeer = dataPagerAutomationPeer.GetChildren()[2] as ToggleButtonAutomationPeer;
                Assert.IsNotNull(tbtnAutomationPeer);
                Assert.AreEqual("MoveToPage1", tbtnAutomationPeer.GetAutomationId());

                // Push that button to move to first page
                IToggleProvider toggleProvider = tbtnAutomationPeer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
                Assert.IsNotNull(toggleProvider);
                // First toggle button is not expected to be pressed
                Assert.AreEqual(ToggleState.Off, toggleProvider.ToggleState);
                toggleProvider.Toggle();
                // Toggling a button when the Source is a PagedCollectionView has immediate effect because the
                // PagedCollectionView does local paging.
                Assert.AreEqual(ToggleState.On, toggleProvider.ToggleState);
                // Page move is expected to succeed
                Assert.AreEqual(1, rangeValueProvider.Value);

                // Toggling a pressed button should have no effect.
                toggleProvider.Toggle();
                Assert.AreEqual(ToggleState.On, toggleProvider.ToggleState);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the DataPagerAutomationPeer with a DataPagerTestSource Source.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the DataPagerAutomationPeer with a DataPagerTestSource Source.")]
        public void TestDataPagerAutomationPeerWithDataPagerTestSource()
        {
            this.LoadControl();
            this.LoadCollectionView(true /*makeSourceCollectionPaged*/, true /*canPage*/);

            DataPagerAutomationPeer dataPagerAutomationPeer = DataPagerAutomationPeer.CreatePeerForElement(this._pager) as DataPagerAutomationPeer;
            Assert.IsNotNull(dataPagerAutomationPeer);

            IRangeValueProvider rangeValueProvider = dataPagerAutomationPeer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider;
            Assert.IsNotNull(rangeValueProvider);

            EnqueueCallback(() =>
            {
                Assert.IsFalse(rangeValueProvider.IsReadOnly);

                Assert.AreEqual(this._pager.PageCount, rangeValueProvider.LargeChange);
                Assert.AreEqual(1, rangeValueProvider.SmallChange);
                Assert.AreEqual(1, rangeValueProvider.Minimum);
                Assert.AreEqual(this._pager.PageCount + 1, rangeValueProvider.Maximum);
                Assert.AreEqual(1, rangeValueProvider.Value);

                // Name is "Page 1 of {PageCount}"
                Assert.AreEqual(string.Format(
                    CultureInfo.InvariantCulture,
                    PagerResources.AutomationPeerName_TotalPageCountKnown,
                    "1", this._pager.PageCount.ToString(CultureInfo.CurrentCulture)),
                    dataPagerAutomationPeer.GetName());

                // Can move to page #2 when source is a DataPagerTestSource
                this.SetUpFuturePageIndexChangesHandling();
                rangeValueProvider.SetValue(2);
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(2, rangeValueProvider.Value);

                this._pager.NumericButtonCount = 2;
                List<AutomationPeer> children = dataPagerAutomationPeer.GetChildren();
                Assert.AreEqual(9, children.Count);

                // Verify type of children
                Assert.IsInstanceOfType(children[0], typeof(ButtonAutomationPeer));
                Assert.IsInstanceOfType(children[1], typeof(ButtonAutomationPeer));
                Assert.IsInstanceOfType(children[2], typeof(ToggleButtonAutomationPeer));
                Assert.IsInstanceOfType(children[3], typeof(ToggleButtonAutomationPeer));
                Assert.IsInstanceOfType(children[4], typeof(TextBlockAutomationPeer));
                Assert.IsInstanceOfType(children[5], typeof(TextBoxAutomationPeer));
                Assert.IsInstanceOfType(children[6], typeof(TextBlockAutomationPeer));
                Assert.IsInstanceOfType(children[7], typeof(ButtonAutomationPeer));
                Assert.IsInstanceOfType(children[8], typeof(ButtonAutomationPeer));

                // Verify AutomationId of children
                Assert.AreEqual("LargeDecrement", children[0].GetAutomationId());
                Assert.AreEqual("SmallDecrement", children[1].GetAutomationId());
                Assert.AreEqual("MoveToPage1", children[2].GetAutomationId());
                Assert.AreEqual("MoveToPage2", children[3].GetAutomationId());
                Assert.AreEqual("CurrentPage", children[5].GetAutomationId());
                Assert.AreEqual("SmallIncrement", children[7].GetAutomationId());
                Assert.AreEqual("LargeIncrement", children[8].GetAutomationId());

                this._pager.DisplayMode = PagerDisplayMode.Numeric;
                // Access automation peer of the first toggle button
                ToggleButtonAutomationPeer tbtnAutomationPeer = dataPagerAutomationPeer.GetChildren()[2] as ToggleButtonAutomationPeer;
                Assert.IsNotNull(tbtnAutomationPeer);
                Assert.AreEqual("MoveToPage1", tbtnAutomationPeer.GetAutomationId());

                // Push that button to move to first page
                this.SetUpFuturePageIndexChangesHandling();
                IToggleProvider toggleProvider = tbtnAutomationPeer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
                Assert.IsNotNull(toggleProvider);
                // First toggle button is not expected to be pressed
                Assert.AreEqual(ToggleState.Off, toggleProvider.ToggleState, "un");
                toggleProvider.Toggle();
                // Even though it was pressed, the toggle button goes back to unpressed while the asynchronous
                // move operation occurs.
                Assert.AreEqual(ToggleState.Off, toggleProvider.ToggleState, "deux");
            });

            this.WaitForPageIndexChanges();

            EnqueueCallback(() =>
            {
                // Page move is expected to succeed
                Assert.AreEqual(1, rangeValueProvider.Value);

                ToggleButtonAutomationPeer tbtnAutomationPeer = dataPagerAutomationPeer.GetChildren()[2] as ToggleButtonAutomationPeer;
                Assert.IsNotNull(tbtnAutomationPeer);
                Assert.AreEqual("MoveToPage1", tbtnAutomationPeer.GetAutomationId());

                IToggleProvider toggleProvider = tbtnAutomationPeer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
                Assert.IsNotNull(toggleProvider);
                Assert.AreEqual(ToggleState.On, toggleProvider.ToggleState);

                // Toggling a pressed button should have no effect.
                toggleProvider.Toggle();
                Assert.AreEqual(ToggleState.On, toggleProvider.ToggleState);
            });

            EnqueueTestComplete();
        }

        #endregion Automation Peer Tests

        #endregion Test Methods

        #region Helper Methods

        /// <summary>
        /// Helper method that verifies that the test () => raises the specified exception.
        /// </summary>
        /// <typeparam name="TException">Type of exception</typeparam>
        /// <param name="exceptionPrototype">Exception prototype, with the expected exception message populated.</param>
        /// <param name="test">Action () => to expect exception from.</param>
        private static void AssertExpectedException<TException>(TException exceptionPrototype, Action test)
            where TException : Exception
        {
            TException exception = null;

            try
            {
                test();
            }
            catch (TException e)
            {
                // Looking for exact matches
                if (e.GetType() == typeof(TException))
                {
                    exception = e;
                }
            }

            if (exception == null)
            {
                Assert.Fail("Expected {0} with message \"{1}\". \nActual: none.", typeof(TException).FullName, exceptionPrototype.Message);
            }
            else if (exception.Message != exceptionPrototype.Message)
            {
                Assert.Fail("Expected {0} with message \"{1}\". \nActual: {2} => \"{3}\".", typeof(TException).FullName, exceptionPrototype.Message, exception.GetType().FullName, exception.Message);
            }
        }

        /// <summary>
        /// Helper method that loads a colleciton view into the pager.
        /// </summary>
        private void LoadCollectionView()
        {
            this.LoadCollectionView(false, true);
        }

        /// <summary>
        /// Helper method that loads a colleciton view into the pager.
        /// </summary>
        /// <param name="makeSourceCollectionPaged">Whether or not to set IsSourceCollectionPaged = true.</param>
        /// <param name="canPage">Determines the target value for IPagedCollection.CanChangePage</param>
        private void LoadCollectionView(bool makeSourceCollectionPaged, bool canPage)
        {
            if (makeSourceCollectionPaged)
            {
                this._dataPagerSource = new DataPagerTestSource<int>(25);
                this._dataPagerSource.ItemCount = this._dataPagerSource.Count;
                this._dataPagerSource.TotalItemCount = this._dataPagerSource.ItemCount;
                this._dataPagerSource.CanChangePage = canPage;
                this._dataPagerSource.PageSize = 5;
                this._pager.Source = this._dataPagerSource;
            }
            else
            {
                List<int> intList = new List<int>();
                for (int i = 0; i < 25; i++)
                {
                    intList.Add(i);
                }

                IEnumerable sourceCollection = intList;
                this._pagedCollectionView = new PagedCollectionView(sourceCollection);
                this._pager.PageSize = 5;
                this._pager.Source = this._pagedCollectionView;
            }
        }

        /// <summary>
        /// Helper method that loads the pager onto the test panel.
        /// </summary>
        private void LoadControl()
        {
            EnqueueCallback(() =>
            {
                this._pager.Loaded += (object sender, System.Windows.RoutedEventArgs e) => this._pagerLoaded = true;
                this.TestPanel.Children.Add(this._pager);
            });

            EnqueueConditional(() => this._pagerLoaded);
        }

        /// <summary>
        /// Helper method that loads an IEnumerable into the pager.
        /// </summary>
        private void LoadEnumerable()
        {
            this._enumerableSource = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                this._enumerableSource.Add(i);
            }
            this._pager.Source = this._enumerableSource;
        }

        /// <summary>
        /// Helper method that hooks up the PageIndexChanged event for future monitoring.
        /// </summary>
        private void SetUpFuturePageIndexChangedHandling()
        {
            this._pageIndexChangedEventCount = 0;
            this._pager.PageIndexChanged += new EventHandler<EventArgs>(this.OnPageIndexChanged);
        }

        /// <summary>
        /// Helper method that hooks up the PageIndexChanging event for future monitoring.
        /// </summary>
        private void SetUpFuturePageIndexChangingHandling()
        {
            this._pageIndexChangingEventCount = 0;
            this._pager.PageIndexChanging += new EventHandler<System.ComponentModel.CancelEventArgs>(this.OnPageIndexChanging);
        }

        /// <summary>
        /// Helper method that hooks up the PageIndexChanging/PageIndexChanged events for future monitoring.
        /// </summary>
        private void SetUpFuturePageIndexChangesHandling()
        {
            this.SetUpFuturePageIndexChangingHandling();
            this.SetUpFuturePageIndexChangedHandling();
        }

        /// <summary>
        /// Helper method that waits for the PageIndexChanging/PageIndexChanged events to be raised within a default timeframe.
        /// </summary>
        private void WaitForPageIndexChanges()
        {
            this.WaitForPageIndexChanges(DefaultWaitTimeout);
        }

        /// <summary>
        /// Helper method that waits for the PageIndexChanging/PageIndexChanged events to be raised within the specified timeframe.
        /// </summary>
        /// <param name="millisecondsTimeout">The amount of time in milliseconds to wait before concluding the events were not raised.</param>
        private void WaitForPageIndexChanges(int millisecondsTimeout)
        {
            EnqueueCallback(() => this._startedWaiting = DateTime.Now);
            EnqueueConditional(() => ((this._pageIndexChangingEventCount > 0 && this._pageIndexChangedEventCount > 0) || (DateTime.Now - this._startedWaiting).TotalMilliseconds > millisecondsTimeout) &&
                (DateTime.Now - this._startedWaiting).TotalMilliseconds > MinimumWaitTimeout);

            EnqueueCallback(() =>
            {
                this._pager.PageIndexChanging -= new EventHandler<System.ComponentModel.CancelEventArgs>(this.OnPageIndexChanging);
                this._pager.PageIndexChanged -= new EventHandler<EventArgs>(this.OnPageIndexChanged);

                if (this._pageIndexChangingEventCount == 0)
                {
                    Assert.Fail("Timed out while waiting for PageIndexChanging to be raised by the pager during " + millisecondsTimeout.ToString() + " milliseconds.");
                }
                else if (this._pageIndexChangingEventCount > 1)
                {
                    Assert.Fail("PageIndexChanging was raised more than once.");
                }

                if (this._pageIndexChangedEventCount == 0)
                {
                    Assert.Fail("Timed out while waiting for PageIndexChanged to be raised by the pager during " + millisecondsTimeout.ToString() + " milliseconds.");
                }
                else if (this._pageIndexChangedEventCount > 1)
                {
                    Assert.Fail("PageIndexChanged was raised more than once.");
                }
            });
        }

        /// <summary>
        /// Helper method that verifies that the PageIndexChanging event is raised, while PageIndexChanged is not.
        /// </summary>
        /// <param name="millisecondsTimeout">The amount of time in milliseconds to wait for the events to be raised.</param>
        private void WaitForPageIndexChangingAlone(int millisecondsTimeout)
        {
            EnqueueCallback(() => this._startedWaiting = DateTime.Now);
            EnqueueConditional(() => this._pageIndexChangedEventCount > 0 || (DateTime.Now - this._startedWaiting).TotalMilliseconds > millisecondsTimeout);

            EnqueueCallback(() =>
            {
                this._pager.PageIndexChanging -= new EventHandler<System.ComponentModel.CancelEventArgs>(this.OnPageIndexChanging);
                this._pager.PageIndexChanged -= new EventHandler<EventArgs>(this.OnPageIndexChanged);

                if (this._pageIndexChangingEventCount != 1)
                {
                    Assert.Fail("PageIndexChanging event was not raised exactly once, but " + this._pageIndexChangingEventCount.ToString() + " times.");
                }

                if (this._pageIndexChangedEventCount > 0)
                {
                    Assert.Fail("PageIndexChanged event was raised " + this._pageIndexChangedEventCount.ToString() + " times, instead of 0.");
                }
            });
        }

        /// <summary>
        /// Helper method that verifies that the PageIndexChanging/PageIndexChanged events do not get raised.
        /// </summary>
        /// <param name="millisecondsTimeout">The amount of time in milliseconds to wait for the events to be raised.</param>
        private void WaitForNoPageIndexChanges(int millisecondsTimeout)
        {
            EnqueueCallback(() => this._startedWaiting = DateTime.Now);
            EnqueueConditional(() => this._pageIndexChangingEventCount > 0 || this._pageIndexChangedEventCount > 0 || (DateTime.Now - this._startedWaiting).TotalMilliseconds > millisecondsTimeout);

            EnqueueCallback(() =>
            {
                this._pager.PageIndexChanging -= new EventHandler<System.ComponentModel.CancelEventArgs>(this.OnPageIndexChanging);
                this._pager.PageIndexChanged -= new EventHandler<EventArgs>(this.OnPageIndexChanged);

                if (this._pageIndexChangingEventCount > 0)
                {
                    Assert.Fail("Unexpected PageIndexChanging event.");
                }

                if (this._pageIndexChangedEventCount > 0)
                {
                    Assert.Fail("Unexpected PageIndexChanged event.");
                }
            });
        }

        /// <summary>
        /// Helper method that verifies that the Action raises both the PageIndexChanging 
        /// and PageIndexChanged events exactly once.
        /// </summary>
        /// <param name="test">The Action which should raise the events.</param>
        private void AssertBothPageIndexChangeEvents(Action test)
        {
            this.AssertPageIndexChangeEvents(test, 1 /*pageIndexChangingCount*/, 1 /*pageIndexChangedCount*/);
        }

        /// <summary>
        /// Helper method that verifies that the Action does not raise a PageIndexChanging 
        /// or PageIndexChanged event.
        /// </summary>
        /// <param name="test">The Action which should not raise an event.</param>
        private void AssertNoPageIndexChangeEvent(Action test)
        {
            this._pageIndexChangingEventCount = 0;
            this._pageIndexChangedEventCount = 0;
            test();
            Assert.AreEqual(0, this._pageIndexChangingEventCount, "Got unexpected PageIndexChanging event");
            Assert.AreEqual(0, this._pageIndexChangedEventCount, "Got unexpected PageIndexChanged event");
        }

        /// <summary>
        /// Helper method that verifies that the Action raises the correct number of PageIndexChanging 
        /// and PageIndexChanged events.
        /// </summary>
        /// <param name="test">The Action which should raise the events.</param>
        /// <param name="pageIndexChangingCount">Expected number of PageIndexChanging notifications.</param>
        /// <param name="pageIndexChangedCount">Expected number of PageIndexChanged notifications.</param>
        private void AssertPageIndexChangeEvents(Action test, int pageIndexChangingCount, int pageIndexChangedCount)
        {
            this._pageIndexChangingEventCount = 0;
            this._pageIndexChangedEventCount = 0;
            test();
            Assert.AreEqual(pageIndexChangingCount, this._pageIndexChangingEventCount, "Got incorrect number of PageIndexChanging events");
            Assert.AreEqual(pageIndexChangedCount, this._pageIndexChangedEventCount, "Got incorrect number of PageIndexChanged events");
        }

        /// <summary>
        /// Helper method that will return the NumericButtonPanel of the DataPager control. We
        /// are using the default template for the unit tests, so we are assuming that
        /// all the template parts are there.
        /// </summary>
        /// <returns>The NumericButtonPanel of the DataPager control</returns>
        private StackPanel GetButtonPanel()
        {
            return this._pager.FindChildByName("NumericButtonPanel") as StackPanel;
        }

        /// <summary>
        /// Helper method that handles a PageIndexChanged event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void OnPageIndexChanged(object sender, EventArgs e)
        {
            this._pageIndexChangedEventCount++;
        }

        /// <summary>
        /// Helper method that handles a PageIndexChanging event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void OnPageIndexChanging(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = this._cancelPageIndexChanging;
            this._pageIndexChangingEventCount++;
        }

        /// <summary>
        /// Add an element to the test surface and perform a series of test
        /// actions with a pause in between each allowing the test surface to be
        /// updated.  This task does not complete the async test and a call to
        /// EnqueueTestCompleted is still required.
        /// </summary>
        /// <param name="element">Element that we are creating the task for.</param>
        /// <param name="actions">Test actions.</param>
        /// <remarks>
        /// CreateAsyncTask should only be invoked via test methods with the
        /// AsynchronousAttribute applied.
        /// </remarks>
        protected void CreateAsyncTask(FrameworkElement element, params Action[] actions)
        {
            Assert.IsNotNull(element);
            actions = actions ?? new Action[] { };

            // Add a handler to determine when the element is loaded
            bool isLoaded = false;
            element.Loaded += delegate { isLoaded = true; };

            // Add the element to the test surface and wait until it's loaded
            TestPanel.Children.Add(element);
            EnqueueConditional(() => isLoaded);

            // Perform the test actions
            foreach (Action action in actions)
            {
                Action capturedAction = action;
                EnqueueCallback(() => capturedAction());
                EnqueueDelay(VisualDelayInMilliseconds);
            }

            // Remove the element from the test surface and finish the test
            EnqueueCallback(() => TestPanel.Children.Remove(element));
        }

        /// <summary>
        /// Add an element to the test surface and perform a series of test
        /// actions with a pause in between each allowing the test surface to be
        /// updated.
        /// </summary>
        /// <param name="element">Element that we are creating the task for.</param>
        /// <param name="actions">Test actions.</param>
        /// <remarks>
        /// CreateAsyncTest should only be invoked via test methods with the
        /// AsynchronousAttribute applied.
        /// </remarks>
        protected void CreateAsyncTest(FrameworkElement element, params Action[] actions)
        {
            this.CreateAsyncTask(element, actions);
            EnqueueTestComplete();
        }

        #endregion
    }
}
