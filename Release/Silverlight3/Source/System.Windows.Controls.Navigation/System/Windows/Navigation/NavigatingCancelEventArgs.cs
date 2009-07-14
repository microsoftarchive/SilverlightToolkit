//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace System.Windows.Navigation
{
    /// <summary>
    /// Event data used to qualify navigating events.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public sealed class NavigatingCancelEventArgs : CancelEventArgs
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="uri">A URI representing the navigation destination.</param>
        /// <param name="mode">What type of navigation this is (New, Forward or Back)</param>
        internal NavigatingCancelEventArgs(Uri uri, NavigationMode mode)
        {
            this.Uri = uri;
            this.NavigationMode = mode;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the Uri that is being navigated to
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the type of navigation that has been initiated (New, Forward or Back)
        /// </summary>
        public NavigationMode NavigationMode { get; private set; }

        #endregion Properties
    }
}
