//-----------------------------------------------------------------------
// <copyright file="PageChangedEventArgs.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel
{
    /// <summary>
    /// Event argument used for page index change notifications. 
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public sealed class PageChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor that takes the final page index. That index
        /// may be different from the requested page index.
        /// </summary>
        /// <param name="pageIndex">Final page index</param>
        public PageChangedEventArgs(int pageIndex)
        {
            this.PageIndex = pageIndex;
        }

        /// <summary>
        /// Gets the final page index resulting from the page move
        /// </summary>
        public int PageIndex
        {
            get;
            private set;
        }
    }
}
