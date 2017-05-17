// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Provides a way to apply different styles for holidays and normal days.
    /// </summary>
    public class HolidayDayButtonStyleSelector : CalendarDayButtonStyleSelector
    {
        /// <summary>
        /// Gets or sets the style for normal days.
        /// </summary>
        public Style NormalStyle { get; set; }

        /// <summary>
        /// Gets or sets the style for holidays.
        /// </summary>
        public Style HolidayStyle { get; set; }

        /// <summary>
        /// Gets a collection of Holidays.
        /// </summary>
        public Collection<Holiday> Holidays { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HolidayDayButtonStyleSelector
        /// class.
        /// </summary>
        public HolidayDayButtonStyleSelector()
        {
            Holidays = new Collection<Holiday>();
        }

        /// <summary>
        /// Returns a GlobalCalendarDayButton Style based on whether the day is
        /// a holiday.
        /// </summary>
        /// <param name="day">The day to select a Style for.</param>
        /// <param name="container">The GlobalCalendarDayButton.</param>
        /// <returns>A Style for the GlobalCalendarDayButton.</returns>
        public override Style SelectStyle(DateTime day, GlobalCalendarDayButton container)
        {
            Holiday holiday = Holidays.Where(h => h.FallsOn(day)).FirstOrDefault();

            // Use the Holiday.Title as the Tooltip
            string title = holiday != null ? holiday.Title : null;
            ToolTipService.SetToolTip(container, title);

            return (holiday != null) ?
                HolidayStyle :
                NormalStyle;
        }
    }
}