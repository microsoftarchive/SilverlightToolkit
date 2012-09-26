// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Provides data for the CustomMessageBox's Dismissing event.
    /// </summary>
    public class DismissingEventArgs : CancelEventArgs
    {
         /// <summary>
        /// Initializes a new instance of the DismissingEventArgs class.
        /// </summary>
        /// <param name="result">The result value.</param>
        public DismissingEventArgs(CustomMessageBoxResult result)
        {
            Result = result;
        }

        /// <summary>
        /// Gets or sets the result value.
        /// </summary>
        public CustomMessageBoxResult Result { get; private set; }
    }
}
