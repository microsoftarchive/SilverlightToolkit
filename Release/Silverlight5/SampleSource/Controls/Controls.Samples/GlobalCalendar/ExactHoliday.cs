// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents a holiday to be marked on the GlobalCalendar that occurs on
    /// the exact date every year.
    /// </summary>
    public partial class ExactHoliday : Holiday
    {
        /// <summary>
        /// Gets or sets the date of the holiday.
        /// </summary>
        [TypeConverter(typeof(DateTimeTypeConverter))]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the holiday occurs annually
        /// on the same date.
        /// </summary>
        public bool IsAnnual { get; set; }

        /// <summary>
        /// Initializes a new instance of the ExactHoliday class.
        /// </summary>
        public ExactHoliday()
        {
            IsAnnual = true;
        }

        /// <summary>
        /// Determine if this holiday falls on a specific date.
        /// </summary>
        /// <param name="day">The date to check.</param>
        /// <returns>
        /// A value indicating whether this holiday falls on a specific date.
        /// </returns>
        public override bool FallsOn(DateTime day)
        {
            return IsAnnual ?
                (day.Day == Date.Day && day.Month == Date.Month) :
                day == Date;
        }
    }
}