// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Represents a control that allows the user to choose a date (day/month/year).
    /// </summary>
    public class DatePicker : DateTimePickerBase
    {
        /// <summary>
        /// Initializes a new instance of the DatePicker control.
        /// </summary>
        public DatePicker()
        {
            DefaultStyleKey = typeof(DatePicker);
            Value = DateTime.Now.Date;
        }
    }
}
