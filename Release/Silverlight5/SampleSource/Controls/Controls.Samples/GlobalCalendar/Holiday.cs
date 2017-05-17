// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents a holiday to be marked on the GlobalCalendar.
    /// </summary>
    public abstract partial class Holiday
    {
        /// <summary>
        /// Gets or sets the title of the holiday.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Initializes a new instance of the Holiday class.
        /// </summary>
        protected Holiday()
        {
        }

        /// <summary>
        /// Determine if this holiday falls on a specific date.
        /// </summary>
        /// <param name="day">The date to check.</param>
        /// <returns>
        /// A value indicating whether this holiday falls on a specific date.
        /// </returns>
        public abstract bool FallsOn(DateTime day);
    }
}