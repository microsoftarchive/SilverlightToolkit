// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Provides data for the CustomMessageBox's Dismissed event.
    /// </summary>
    public class DismissedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DismissedEventArgs class.
        /// </summary>
        /// <param name="result">The result value.</param>
        public DismissedEventArgs(CustomMessageBoxResult result)
        {
            Result = result;
        }

        /// <summary>
        /// Gets or sets the result value.
        /// </summary>
        public CustomMessageBoxResult Result { get; private set; }
    }
}
