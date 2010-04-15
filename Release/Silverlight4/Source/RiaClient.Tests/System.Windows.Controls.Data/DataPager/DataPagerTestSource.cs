//-----------------------------------------------------------------------
// <copyright file="DataPagerTestSource.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;

namespace System.Windows.Controls.UnitTests
{
    public class DataPagerTestSource<T> : IPagedCollectionView, IEnumerable<T>, INotifyPropertyChanged where T : new()
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
        /// Gets the internal representation of the items
        /// </summary>
        private List<T> _items;

        /// <summary>
        /// Gets the current page index.
        /// </summary>
        private int _pageIndex;

        /// <summary>
        /// Gets the items in the current page
        /// </summary>
        private List<T> _pageItems;

        /// <summary>
        /// Gets or sets the number of items to display on a page.
        /// </summary>
        private int _pageSize;

        /// <summary>
        /// Gets the total number of items in the collection, or -1 if that value is unknown.
        /// </summary>
        private int _totalItemCount;

        #region Constructors

        /// <summary>
        /// DataPagerSource constructor that takes the size of the enumerable to create.
        /// </summary>
        /// <param name="initialCount"></param>
        public DataPagerTestSource(int initialCount)
        {
            this._dispatchTimer = new DispatcherTimer();
            this._dispatchTimer.Interval = TimeSpan.FromMilliseconds(150);
            this._dispatchTimer.Tick += new EventHandler(this.DispatchTimer_Tick);

            this._items = new List<T>();
            for (int i = 0; i < initialCount; i++)
            {
                this._items.Add(new T());
            }
            this.ItemCount = initialCount;
            this.TotalItemCount = initialCount;
        }

        #endregion

        #region IPagedCollectionView Members

        public event EventHandler<EventArgs> PageChanged;

        public event EventHandler<PageChangingEventArgs> PageChanging;

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
                    this.RaisePropertyChanged("CanChangePage");
                }
            }
        }

        /// <summary>
        /// Gets a value showing the number of items we have
        /// </summary>
        public int Count
        {
            get
            {
                return this._items.Count;
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
                    this.RaisePropertyChanged("IsPageChanging");
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
                    this.RaisePropertyChanged("ItemCount");
                }
            }
        }

        /// <summary>
        /// Gets the number of pages we currently have
        /// </summary>
        private int PageCount
        {
            get
            {
                return (this._pageSize > 0) ? (int)Math.Ceiling((double)this.ItemCount / this._pageSize) : 0;
            }
        }

        /// <summary>
        /// Gets the current page index
        /// </summary>
        public int PageIndex
        {
            get
            {
                return this._pageIndex;
            }

            private set
            {
                if (this._pageIndex != value)
                {
                    this._pageIndex = value;
                    this.RaisePropertyChanged("PageIndex");
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
                    if (this._pageSize > 0)
                    {
                        this.UpdatePageItems(this._pageIndex);
                    }
                    this.RaisePropertyChanged("PageSize");
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
                    this.RaisePropertyChanged("TotalItemCount");
                }
            }
        }

        public bool MoveToFirstPage()
        {
            return this.MoveToPage(0);
        }

        public bool MoveToLastPage()
        {
            if (this.TotalItemCount != -1 && this.PageSize > 0)
            {
                return this.MoveToPage(this.PageCount - 1);
            }
            else
            {
                return false;
            }
        }

        public bool MoveToNextPage()
        {
            return this.MoveToPage(this._pageIndex + 1);
        }

        public bool MoveToPage(int pageIndex)
        {
            // May want to add check far valid pageIndex upper boundary
            if (!this.CanChangePage)
            {
                return false;
            }

            // Boundary checks for negative pageIndex
            if (pageIndex < -1 || (pageIndex == -1 && this.PageSize > 0))
            {
                return false;
            }

            // Check if the target page is out of bound, or equal to the current page
            if ((pageIndex > 0 && pageIndex >= this.PageCount) || this._pageIndex == pageIndex)
            {
                return false;
            }

            this._finalPageIndex = pageIndex;
            this._dispatchTimer.Start();
            this.IsPageChanging = true;
            this.RaisePageChanging(new PageChangingEventArgs(this._finalPageIndex));
            return true;
        }

        public bool MoveToPreviousPage()
        {
            return this.MoveToPage(this._pageIndex - 1);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (this.PageSize > 0)
            {
                return this._pageItems.GetEnumerator();
            }
            return this._items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
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
            this.UpdatePageItems(this._finalPageIndex);
            this.PageIndex = this._finalPageIndex;
            this.IsPageChanging = false;
            this.RaisePageChanged();
        }

        /// <summary>
        /// Raises the PageChanged event
        /// </summary>
        private void RaisePageChanged()
        {
            EventHandler<EventArgs> handler = this.PageChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the PageChanging event
        /// </summary>
        /// <param name="e">PageChangingEventArgs instance used for the event</param>
        private void RaisePageChanging(PageChangingEventArgs e)
        {
            EventHandler<PageChangingEventArgs> handler = this.PageChanging;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UpdatePageItems(int pageIndex)
        {
            this._pageItems = new List<T>();
            for (int i = 0; i < this._pageSize; i++)
            {
                this._pageItems.Add(this._items[pageIndex + i]);
            }
        }

        #endregion
    }
}
