// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Provides a way to apply different styles for weekdays and weekends.
    /// </summary>
    public class WeekendDayButtonStyleSelector : CalendarDayButtonStyleSelector
    {
        /// <summary>
        /// Gets or sets the style for weekdays.
        /// </summary>
        public Style WeekdayStyle { get; set; }

        /// <summary>
        /// Gets or sets the style for weekend days.
        /// </summary>
        public Style WeekendStyle { get; set; }

        /// <summary>
        /// Initializes a new instance of the WeekendDayButtonStyleSelector
        /// class.
        /// </summary>
        public WeekendDayButtonStyleSelector()
        {
        }

        /// <summary>
        /// Returns a GlobalCalendarDayButton Style based on whether the day is
        /// a weekday or a weekend.
        /// </summary>
        /// <param name="day">The day to select a Style for.</param>
        /// <param name="container">The GlobalCalendarDayButton.</param>
        /// <returns>A Style for the GlobalCalendarDayButton.</returns>
        public override Style SelectStyle(DateTime day, GlobalCalendarDayButton container)
        {
            DayOfWeek d = day.DayOfWeek;
            return (d == DayOfWeek.Saturday || d == DayOfWeek.Sunday) ?
                WeekendStyle :
                WeekdayStyle;
        }
    }
}