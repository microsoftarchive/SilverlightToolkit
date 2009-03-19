//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace System.Windows.Controls
{
    /// <summary>
    /// Event args for the ContentLoaded event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormContentLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance of DataFormContentLoadedEventArgs.
        /// </summary>
        /// <param name="content">The content that was loaded.</param>
        public DataFormContentLoadedEventArgs(FrameworkElement content)
        {
            this.Content = content;
        }

        /// <summary>
        /// Gets or sets the content that was loaded.
        /// </summary>
        public FrameworkElement Content
        {
            get;
            private set;
        }
    }
}
