//-----------------------------------------------------------------------
// <copyright file="IPagedCollection.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel
{
    /// <summary>
    /// Interface implemented by pageable collections.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public interface IPagedCollection
    {
        /// <summary>
        /// Raised when a page move completed.
        /// </summary>
        event EventHandler<PageChangedEventArgs> PageChanged;

        /// <summary>
        /// Gets a value indicating whether the PageIndex value is allowed to change or not.
        /// </summary>
        bool CanChangePage
        {
            get;
        }

        /// <summary>
        /// Gets the minimum number of items known to be in the collection.
        /// </summary>
        int ItemCount
        {
            get;
        }

        /// <summary>
        /// Gets or sets the number of items to display on a page.
        /// </summary>
        int PageSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the index of the first page of items provided by the collection.
        /// </summary>
        int StartPageIndex
        {
            get;
        }

        /// <summary>
        /// Gets the total number of items in the collection, or -1 if that value is unknown.
        /// </summary>
        int TotalItemCount
        {
            get;
        }

        /// <summary>
        /// Requests a move to page <paramref name="pageIndex"/>.
        /// </summary>
        /// <param name="pageIndex">Index of the target page.</param>
        /// <returns>Whether or not a move was successfully initiated.</returns>
        bool MoveToPage(int pageIndex);
    }
}
