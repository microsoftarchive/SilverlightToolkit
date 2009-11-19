//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Navigation
{
    /// <summary>
    /// Event data used to qualify navigation events.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public sealed class NavigationEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="content">A reference to the content being navigated to (if available).</param>
        /// <param name="uri">A URI representing the navigation destination.</param>
        internal NavigationEventArgs(object content, Uri uri)
        {
            this.Content = content;
            this.Uri = uri;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a reference to the content being navigated to (if available).
        /// </summary>
        public object Content { get; private set; }

        /// <summary>
        /// Gets a URI representing the navigation destination.
        /// </summary>
        public Uri Uri { get; private set; }

        #endregion Properties
    }
}
