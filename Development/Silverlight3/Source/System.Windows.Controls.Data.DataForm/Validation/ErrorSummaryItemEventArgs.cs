//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides data for the ErrorClicked event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class ErrorSummaryItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ErrorSummaryItemEventArgs class.
        /// </summary>
        /// <param name="error">The ErrorSummaryItem reference</param>
        public ErrorSummaryItemEventArgs(ErrorSummaryItem error)
        {
            this.ErrorSummaryItem = error;
        }

        /// <summary>
        /// Gets the error message string
        /// </summary>
        public ErrorSummaryItem ErrorSummaryItem
        {
            get;
            private set;
        }
    }
}
