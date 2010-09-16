// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Represents a control that allows the user to choose a time (hour/minute/am/pm).
    /// </summary>
    public class TimePicker : DateTimePickerBase
    {
        /// <summary>
        /// Initializes a new instance of the TimePicker control.
        /// </summary>
        public TimePicker()
        {
            DefaultStyleKey = typeof(TimePicker);
            Value = DateTime.Now;
        }
    }
}
