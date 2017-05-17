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
    /// the same day of the same week every year (i.e., Thanksgiving in the
    /// United States falls on the fourth Thursday in November).
    /// </summary>
    public partial class RelativeHoliday : Holiday
    {
        /// <summary>
        /// Gets or sets the month of the holiday.
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Gets or sets the day of the holiday.
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the nth day of week in the
        /// month.  Negative values start from the end of the month (which is
        /// used to specify relative holidays like the last Monday in May).
        /// </summary>
        public int DayOfWeekNumber { get; set; }

        /// <summary>
        /// Initializes a new instance of the RelativeHoliday class.
        /// </summary>
        public RelativeHoliday()
        {
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
            // Short circuit on the month or day
            if (day.Month != Month || day.DayOfWeek != DayOfWeek)
            {
                return false;
            }

            // Trim off anything but the date
            day = new DateTime(day.Year, day.Month, day.Day);

            // Loop through all of the dates in the month to count how many days
            // are occurences of DayOfWeek.
            int occurences = 0;
            int dateOccurenceNumber = 0;
            for (DateTime d = new DateTime(day.Year, day.Month, 1); d.Month == day.Month; d = d.AddDays(1))
            {
                if (d.DayOfWeek == DayOfWeek)
                {
                    occurences++;

                    if (d <= day)
                    {
                        dateOccurenceNumber++;
                    }
                }
            }

            return DayOfWeekNumber >= 0 ?
                (DayOfWeekNumber == dateOccurenceNumber) :
                (occurences + DayOfWeekNumber + 1 == dateOccurenceNumber);
        }
    }
}