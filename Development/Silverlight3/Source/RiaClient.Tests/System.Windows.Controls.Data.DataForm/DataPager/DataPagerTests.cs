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
    using System.ComponentModel.UnitTests;
    using System.Globalization;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
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
        private const int VisualDelayInMilliseconds = 100;

        #endregion

        #region Fields
        
        private DataPager _pager;
        private PagedCollectionView _pagedCollectionView;
        private List<string> _propertiesChangedList;
        private bool _eventFired;
        private bool _pageIndexChanged;
        private bool _pagerLoaded;
        private DateTime _startedWaiting;

        #endregion Fields

        #region Initialization Methods

        /// <summary>
        /// Initializes the DataPager to be used in testing.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this._pager = new DataPager();
            this._propertiesChangedList = new List<string>();
        }

        #endregion Initialization Methods

        #region Test Methods

        #region Event Tests

        /// <summary>
        /// Validate the PageIndexChanging event on the DataPager class.
        /// </summary>
        [TestMethod]
        [Description("Validate the PageIndexChanging event on the DataPager class.")]
        public void PageIndexChanging()
        {
            this.LoadCollectionView();
            this.AssertNoEvent(() => this._pager.PageIndex = 1);
            this._pager.PageIndexChanging += (object sender, System.ComponentModel.CancelEventArgs e) => this._eventFired = true;
            this.AssertExpectedEvent(() => this._pager.PageIndex = 2);
        }

        /// <summary>
        /// Validate the PageIndexChanging event on the DataPager class.
        /// </summary>
        [TestMethod]
        [Description("Validate the PageIndexChanging event on the DataPager class.")]
        public void PageIndexChanged()
        {
            this.LoadCollectionView();
            this.AssertNoEvent(() => this._pager.PageIndex = 1);
            this._pager.PageIndexChanged += (object sender, EventArgs e) => this._eventFired = true;
            this.AssertExpectedEvent(() => this._pager.PageIndex = 2);
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
                this.SetUpFuturePageIndexChangedHandling();
                this._pagedCollectionView.MoveToLastPage();
            });

            this.WaitForPageIndexChange();

            EnqueueCallback(() =>
            {
                Assert.IsFalse(this._pager.CanMoveToNextPage);
                this.SetUpFuturePageIndexChangedHandling();
                this._pagedCollectionView.MoveToPreviousPage();
            });

            this.WaitForPageIndexChange();

            EnqueueCallback(() =>
            {
                Assert.IsTrue(this._pager.CanMoveToNextPage);
                this.SetUpFuturePageIndexChangedHandling();
                this._pagedCollectionView.MoveToNextPage();
            });

            this.WaitForPageIndexChange();

            EnqueueCallback(() =>
            {
                Assert.IsFalse(this._pager.CanMoveToNextPage);

                ((TestEditablePagedCollection<int>)this._pagedCollectionView.SourceCollection).TotalItemCount = -1;
                Assert.IsTrue(this._pager.CanMoveToNextPage);

                this._pager.IsTotalItemCountFixed = false;
                Assert.IsTrue(this._pager.CanMoveToNextPage);

                ((TestEditablePagedCollection<int>)this._pagedCollectionView.SourceCollection).TotalItemCount = 100;
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
                this.SetUpFuturePageIndexChangedHandling();
                this._pagedCollectionView.MoveToLastPage();
            });

            this.WaitForPageIndexChange();

            EnqueueCallback(() =>
            {
                Assert.IsFalse(this._pager.CanMoveToLastPage);
                this.SetUpFuturePageIndexChangedHandling();
                this._pagedCollectionView.MoveToPreviousPage();
            });

            this.WaitForPageIndexChange();

            EnqueueCallback(() =>
            {
                Assert.IsTrue(this._pager.CanMoveToLastPage);
                ((TestEditablePagedCollection<int>)this._pagedCollectionView.SourceCollection).TotalItemCount = -1;
                Assert.IsFalse(this._pager.CanMoveToLastPage);

                this._pager.IsTotalItemCountFixed = false;
                Assert.IsFalse(this._pager.CanMoveToLastPage);

                ((TestEditablePagedCollection<int>)this._pagedCollectionView.SourceCollection).TotalItemCount = 100;
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

            this._pager.PageIndexChanging += new EventHandler<System.ComponentModel.CancelEventArgs>(this.PageIndexChangingCancel);
            this.AssertExpectedEvent(() => this._pager.PageIndex = 1);
            Assert.AreEqual(0, this._pager.PageIndex);
            Assert.AreEqual(0, this._pagedCollectionView[0]);
            Assert.AreEqual(1, this._pagedCollectionView[1]);
            Assert.AreEqual(2, this._pagedCollectionView[2]);
            Assert.AreEqual(3, this._pagedCollectionView[3]);
            Assert.AreEqual(4, this._pagedCollectionView[4]);

            this._pager.PageIndexChanging -= new EventHandler<System.ComponentModel.CancelEventArgs>(this.PageIndexChangingCancel);
            this._pager.PageIndexChanging += new EventHandler<System.ComponentModel.CancelEventArgs>(this.PageIndexChangingNoCancel);
            this.AssertExpectedEvent(() => this._pager.PageIndex = 1);
            Assert.AreEqual(1, this._pager.PageIndex);
            Assert.AreEqual(5, this._pagedCollectionView[0]);
            Assert.AreEqual(6, this._pagedCollectionView[1]);
            Assert.AreEqual(7, this._pagedCollectionView[2]);
            Assert.AreEqual(8, this._pagedCollectionView[3]);
            Assert.AreEqual(9, this._pagedCollectionView[4]);

            this._pager.PageIndexChanging -= new EventHandler<System.ComponentModel.CancelEventArgs>(this.PageIndexChangingNoCancel);
            this._pager.PageIndexChanged += new EventHandler<EventArgs>(this.PageIndexChangedEventFired);
            this.AssertExpectedEvent(() => this._pager.PageIndex = 2);
            Assert.AreEqual(2, this._pager.PageIndex);
            Assert.AreEqual(10, this._pagedCollectionView[0]);
            Assert.AreEqual(11, this._pagedCollectionView[1]);
            Assert.AreEqual(12, this._pagedCollectionView[2]);
            Assert.AreEqual(13, this._pagedCollectionView[3]);
            Assert.AreEqual(14, this._pagedCollectionView[4]);

            this._pager.PageIndexChanged -= new EventHandler<EventArgs>(this.PageIndexChangedEventFired);
            this._pager.PageIndex = 3;
            Assert.AreEqual(3, this._pager.PageIndex);
            Assert.AreEqual(15, this._pagedCollectionView[0]);
            Assert.AreEqual(16, this._pagedCollectionView[1]);
            Assert.AreEqual(17, this._pagedCollectionView[2]);
            Assert.AreEqual(18, this._pagedCollectionView[3]);
            Assert.AreEqual(19, this._pagedCollectionView[4]);
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
                this.SetUpFuturePageIndexChangedHandling();
                firstPageButton = this._pager.FindChildByName("FirstPageButton") as ButtonBase;
                firstPageButtonBaseAutomationPeer = ButtonBaseAutomationPeer.CreatePeerForElement(firstPageButton) as ButtonBaseAutomationPeer;
                firstPageProvider = firstPageButtonBaseAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                firstPageProvider.Invoke();
            });

            this.WaitForPageIndexChange();

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
                this.SetUpFuturePageIndexChangedHandling();
                previousPageButton = this._pager.FindChildByName("PreviousPageButton") as ButtonBase;
                previousPageButtonBaseAutomationPeer = ButtonBaseAutomationPeer.CreatePeerForElement(previousPageButton) as ButtonBaseAutomationPeer;
                previousPageProvider = previousPageButtonBaseAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                previousPageProvider.Invoke();
            });

            this.WaitForPageIndexChange();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(1, this._pager.PageIndex);
                this.SetUpFuturePageIndexChangedHandling();
                previousPageProvider.Invoke();
            });

            this.WaitForPageIndexChange();

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

            this.WaitForNoPageIndexChange(250);

            EnqueueCallback(() =>
            {
                Assert.AreEqual(0, this._pager.PageIndex);
                this.SetUpFuturePageIndexChangedHandling();
                //Changes to page 2.
                currentPageValueProvider.SetValue((3).ToString(CultureInfo.CurrentCulture));
                Assert.AreEqual("3", currentPageValueProvider.Value);
            });

            this.WaitForPageIndexChange(1000);

            EnqueueCallback(() =>
            {
                Assert.AreEqual(2, this._pager.PageIndex, "DataPager PageIndex should be 2");
                this.SetUpFuturePageIndexChangedHandling();
                //Changes to page 4.
                currentPageValueProvider.SetValue((5).ToString(CultureInfo.CurrentCulture));
                Assert.AreEqual("5", currentPageValueProvider.Value, "TextBox Text should be 5");
                Assert.AreEqual(2, this._pager.PageIndex, "DataPager PageIndex should still be 2");
            });

            this.WaitForPageIndexChange(250);

            EnqueueCallback(() =>
            {
                Assert.AreEqual(4, this._pager.PageIndex);
                this.SetUpFuturePageIndexChangedHandling();
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
                this.SetUpFuturePageIndexChangedHandling();
                nextPageButton = this._pager.FindChildByName("NextPageButton") as ButtonBase;
                nextPageButtonBaseAutomationPeer = ButtonBaseAutomationPeer.CreatePeerForElement(nextPageButton) as ButtonBaseAutomationPeer;
                nextPageProvider = nextPageButtonBaseAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                nextPageProvider.Invoke();
            });

            this.WaitForPageIndexChange();

            EnqueueCallback(() =>
            {
                Assert.AreEqual(1, this._pager.PageIndex);
                this._pagedCollectionView.MoveToPage(this._pager.PageCount - 2);
                this.SetUpFuturePageIndexChangedHandling();
                nextPageProvider.Invoke();
            });

            this.WaitForPageIndexChange();

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
                this.SetUpFuturePageIndexChangedHandling();
                lastPageButton = this._pager.FindChildByName("LastPageButton") as ButtonBase;
                lastPageButtonBaseAutomationPeer = ButtonBaseAutomationPeer.CreatePeerForElement(lastPageButton) as ButtonBaseAutomationPeer;
                lastPageProvider = lastPageButtonBaseAutomationPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                lastPageProvider.Invoke();
            });

            this.WaitForPageIndexChange();

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

        #endregion UI Element Tests

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
            IEnumerable sourceCollection = null;

            if (makeSourceCollectionPaged)
            {
                TestEditablePagedCollection<int> intCollection = new TestEditablePagedCollection<int>();
                for (int i = 0; i < 25; i++)
                {
                    intCollection.Add(i);
                }

                intCollection.ItemCount = intCollection.Count;
                intCollection.TotalItemCount = intCollection.ItemCount;
                intCollection.CanChangePage = canPage;
                sourceCollection = intCollection;
            }
            else
            {
                List<int> intList = new List<int>();
                for (int i = 0; i < 25; i++)
                {
                    intList.Add(i);
                }

                sourceCollection = intList;
            }

            this._pagedCollectionView = new PagedCollectionView(sourceCollection);

            this._pager.PageSize = 5;
            this._pager.Source = this._pagedCollectionView;
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
        /// Helper method that verifies that the Action fires an event.
        /// </summary>
        private void SetUpFuturePageIndexChangedHandling()
        {
            this._pageIndexChanged = false;
            this._pager.PageIndexChanged += new EventHandler<EventArgs>(delegate { this._pageIndexChanged = true; });
        }

        /// <summary>
        /// Helper method that verifies that the Action fires an event.
        /// </summary>
        private void WaitForPageIndexChange()
        {
            this.WaitForPageIndexChange(DefaultWaitTimeout);
        }

        /// <summary>
        /// Helper method that verifies that the Action fires an event.
        /// </summary>
        /// <param name="millisecondsTimeout">The amount of time in milliseconds to wait before concluding the property did not change.</param>
        private void WaitForPageIndexChange(int millisecondsTimeout)
        {
            EnqueueCallback(() => this._startedWaiting = DateTime.Now);
            EnqueueConditional(() => this._pageIndexChanged || (DateTime.Now - _startedWaiting).TotalMilliseconds > millisecondsTimeout);

            EnqueueCallback(() =>
            {
                this._pager.PageIndexChanged -= new EventHandler<EventArgs>(OnPageIndexChanged);

                if (!this._pageIndexChanged)
                {
                    Assert.Fail("Timed out while waiting for PageIndex to change on the pager during " + millisecondsTimeout.ToString() + " milliseconds.");
                }
            });
        }

        /// <summary>
        /// Helper method that verifies that the Action fires an event.
        /// </summary>
        /// <param name="millisecondsTimeout">The amount of time in milliseconds to wait before concluding the property did not change.</param>
        private void WaitForNoPageIndexChange(int millisecondsTimeout)
        {
            EnqueueCallback(() => this._startedWaiting = DateTime.Now);
            EnqueueConditional(() => this._pageIndexChanged || (DateTime.Now - _startedWaiting).TotalMilliseconds > millisecondsTimeout);

            EnqueueCallback(() =>
            {
                this._pager.PageIndexChanged -= new EventHandler<EventArgs>(OnPageIndexChanged);

                if (this._pageIndexChanged)
                {
                    Assert.Fail("Unexpected change in PageIndex on the pager.");
                }
            });
        }

        /// <summary>
        /// Helper method that verifies that the Action fires an event.
        /// </summary>
        /// <param name="test">The Action which should fire an event.</param>
        private void AssertExpectedEvent(Action test)
        {
            this._eventFired = false;
            test();
            Assert.IsTrue(this._eventFired);
        }

        /// <summary>
        /// Helper method that verifies that the Action does not fire an event.
        /// </summary>
        /// <param name="test">The Action which should not fire an event.</param>
        private void AssertNoEvent(Action test)
        {
            this._eventFired = false;
            test();
            Assert.IsFalse(this._eventFired);
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
        /// Helper method that handles and cancels a PageIndexChanging event.
        /// </summary>
        /// <param name="sender">The object the fired the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void PageIndexChangingCancel(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this._eventFired = true;
            e.Cancel = true;
        }

        /// <summary>
        /// Helper method that handles and does not cancel a PageIndexChanging event.
        /// </summary>
        /// <param name="sender">The object the fired the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void PageIndexChangingNoCancel(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this._eventFired = true;
        }

        /// <summary>
        /// Helper method that handles a PageIndexChanged event.
        /// </summary>
        /// <param name="sender">The object the fired the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void PageIndexChangedEventFired(object sender, EventArgs e)
        {
            this._eventFired = true;
        }

        /// <summary>
        /// Helper method that handles a PropertyChanged event.
        /// </summary>
        /// <param name="sender">The object the fired the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void OnPageIndexChanged(object sender, EventArgs e)
        {
            this._pageIndexChanged = true;
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
