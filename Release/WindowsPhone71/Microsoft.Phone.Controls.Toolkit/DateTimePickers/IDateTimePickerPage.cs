// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Phone.Controls.Primitives
{
    /// <summary>
    /// Represents an interface for DatePicker/TimePicker to use for communicating with a picker page.
    /// </summary>
    public interface IDateTimePickerPage
    {
        /// <summary>
        /// Gets or sets the DateTime to show in the picker page and to set when the user makes a selection.
        /// </summary>
        DateTime? Value { get; set; }
    }
}
