// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// An enumeration defining the supported ListPicker modes.
    /// </summary>
    public enum ListPickerMode
    {
        /// <summary>
        /// Normal mode; only the selected item is visible on the original page.
        /// </summary>
        Normal,

        /// <summary>
        /// Expanded mode; all items are visible on the original page.
        /// </summary>
        Expanded,

        /// <summary>
        /// Full mode; all items are visible in a separate Popup.
        /// </summary>
        Full,
    };
}
