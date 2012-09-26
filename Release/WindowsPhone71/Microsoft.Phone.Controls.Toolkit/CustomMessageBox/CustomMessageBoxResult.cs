// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Describes the different possible results 
    /// for the dismissal of a CustomMessageBox.
    /// </summary>
    public enum CustomMessageBoxResult
    {
        /// <summary>
        /// The left button contains the result value
        /// of the custom message box.
        /// </summary>
        LeftButton,

        /// <summary>
        /// The right button contains the result value
        /// of custom the message box.
        /// </summary>
        RightButton,

        /// <summary>
        /// The custom message box has no result.
        /// </summary>
        None
    }
}
