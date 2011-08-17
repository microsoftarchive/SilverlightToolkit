// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Phone.Controls.Primitives
{
    /// <summary>
    /// Implements a wrapper for DateTime that provides formatted strings for DatePicker.
    /// </summary>
    public class DateTimeWrapper
    {
        /// <summary>
        /// Gets the DateTime being wrapped.
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Gets the 4-digit year as a string.
        /// </summary>
        public string YearNumber { get { return DateTime.ToString("yyyy", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the 2-digit month as a string.
        /// </summary>
        public string MonthNumber { get { return DateTime.ToString("MM", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the month name as a string.
        /// </summary>
        public string MonthName { get { return DateTime.ToString("MMMM", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the 2-digit day as a string.
        /// </summary>
        public string DayNumber { get { return DateTime.ToString("dd", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the day name as a string.
        /// </summary>
        public string DayName { get { return DateTime.ToString("dddd", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the hour as a string.
        /// </summary>
        public string HourNumber { get { return DateTime.ToString(CurrentCultureUsesTwentyFourHourClock() ? "%H" : "%h", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the 2-digit minute as a string.
        /// </summary>
        public string MinuteNumber { get { return DateTime.ToString("mm", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Gets the AM/PM designator as a string.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Pm", Justification = "Clearest way of expressing the concept.")]
        public string AmPmString { get { return DateTime.ToString("tt", CultureInfo.CurrentCulture); } }

        /// <summary>
        /// Initializes a new instance of the DateTimeWrapper class.
        /// </summary>
        /// <param name="dateTime">DateTime to wrap.</param>
        public DateTimeWrapper(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        /// <summary>
        /// Returns a value indicating whether the current culture uses a 24-hour clock.
        /// </summary>
        /// <returns>True if it uses a 24-hour clock; false otherwise.</returns>
        public static bool CurrentCultureUsesTwentyFourHourClock()
        {
            return !CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern.Contains("t");
        }
    }
}
