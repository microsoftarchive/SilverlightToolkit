//-----------------------------------------------------------------------
// <copyright file="TestEditablePagedCollection.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Windows.Threading;

    /// <summary>
    /// IEditableCollection/IPagedCollection implementation used for unit testing
    /// </summary>
    /// <typeparam name="T">Newable type</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface", Justification = "Test Class")]
    public class TestEditablePagedCollection<T> : TestEditableCollection<T>, IPagedCollection where T : new()
    {
        /// <summary>
        /// Gets a value indicating whether the PageIndex value is allowed to change or not.
        /// </summary>
        private bool _canChangePage = true;

        /// <summary>
        /// Timer used to simulate asynchronous page moves.
        /// </summary>
        private DispatcherTimer _dispatchTimer;

        /// <summary>
        /// Page index that is exposed in the PageChanged event.
        /// </summary>
        private int _finalPageIndex;

        /// <summary>
        /// Gets a value indicating whether a page move is in process.
        /// </summary>
        private bool _isPageChanging;

        /// <summary>
        /// Gets the minimum number of items known to be in the collection.
        /// </summary>
        private int _itemCount;

        /// <summary>
        /// Gets or sets the number of items to display on a page.
        /// </summary>
        private int _pageSize;

        /// <summary>
        /// Gets the index of the first page of items provided by the collection.
        /// </summary>
        private int _startPageIndex;

        /// <summary>
        /// Gets the total number of items in the collection, or -1 if that value is unknown.
        /// </summary>
        private int _totalItemCount;

        /// <summary>
        /// Initializes a new instance of the TestEditablePagedCollection class.
        /// </summary>
        public TestEditablePagedCollection()
        {
            this._dispatchTimer = new DispatcherTimer();
            this._dispatchTimer.Interval = TimeSpan.FromMilliseconds(150);
            this._dispatchTimer.Tick += new EventHandler(this.DispatchTimer_Tick);
        }

        #region IPagedCollection Members

        /// <summary>
        /// Raised when a page move completed.
        /// </summary>
        public event EventHandler<PageChangedEventArgs> PageChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the PageIndex value is allowed to change or not.
        /// </summary>
        public bool CanChangePage
        {
            get
            {
                return this._canChangePage;
            }

            set
            {
                if (this._canChangePage != value)
                {
                    this._canChangePage = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("CanChangePage"));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a page move is in the process.
        /// </summary>
        public bool IsPageChanging
        {
            get
            {
                return this._isPageChanging;
            }

            set
            {
                if (this._isPageChanging != value)
                {
                    this._isPageChanging = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsPageChanging"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum number of items known to be in the collection.
        /// </summary>
        public int ItemCount
        {
            get
            {
                return this._itemCount;
            }

            set
            {
                if (this._itemCount != value)
                {
                    this._itemCount = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ItemCount"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of items to display on a page.
        /// </summary>
        public int PageSize
        {
            get
            {
                return this._pageSize;
            }

            set
            {
                if (this._pageSize != value)
                {
                    this._pageSize = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("PageSize"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the first page of items provided by the collection.
        /// </summary>
        public int StartPageIndex
        {
            get
            {
                return this._startPageIndex;
            }

            set
            {
                if (this._startPageIndex != value)
                {
                    this._startPageIndex = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("StartPageIndex"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the total number of items in the collection, or -1 if that value is unknown.
        /// </summary>
        public int TotalItemCount
        {
            get
            {
                return this._totalItemCount;
            }

            set
            {
                if (this._totalItemCount != value)
                {
                    this._totalItemCount = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("TotalItemCount"));
                }
            }
        }

        /// <summary>
        /// Requests a move to page <paramref name="pageIndex"/>.
        /// </summary>
        /// <param name="pageIndex">Index of the target page.</param>
        /// <returns>True when an asynchronous page move was initiated</returns>
        public bool MoveToPage(int pageIndex)
        {
            if (!this.CanChangePage)
            {
                return false;
            }

            this._finalPageIndex = pageIndex;
            this._dispatchTimer.Start();
            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Raises the PageChanged event, completing the page move operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatchTimer_Tick(object sender, EventArgs e)
        {
            this._dispatchTimer.Stop();
            this.RaisePageChanged();
        }

        /// <summary>
        /// Raises the PageChanged event
        /// </summary>
        private void RaisePageChanged()
        {
            EventHandler<PageChangedEventArgs> handler = this.PageChanged;
            if (handler != null)
            {
                PageChangedEventArgs e = new PageChangedEventArgs(this._finalPageIndex);
                handler(this, e);
            }
        }

        #endregion Private Methods
    }
}