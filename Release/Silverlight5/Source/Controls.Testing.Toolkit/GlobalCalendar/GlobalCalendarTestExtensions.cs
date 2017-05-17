// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Extension methods providing access to parts of the GlobalCalendar.
    /// </summary>
    internal static class GlobalCalendarTestExtensions
    {
        /// <summary>
        /// Get a template part.
        /// </summary>
        /// <typeparam name="T">Type of the template part.</typeparam>
        /// <param name="element">The element.</param>
        /// <param name="name">Name of the template part.</param>
        /// <returns>The template part.</returns>
        public static T GetTemplatePart<T>(this FrameworkElement element, string name)
            where T : FrameworkElement
        {
            Debug.Assert(element != null, "element should not be null!");
            Debug.Assert(!string.IsNullOrEmpty(name), "name should not be null!");
            return element
                .GetVisualDescendants()
                .OfType<T>()
                .Where(e => e.Name == name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Click the Button via its AutomationPeer.
        /// </summary>
        /// <param name="button">The Button.</param>
        public static void ClickViaPeer(this Button button)
        {
            Debug.Assert(button != null, "button should not be null!");
            IInvokeProvider peer =
                FrameworkElementAutomationPeer.FromElement(button) as IInvokeProvider ??
                FrameworkElementAutomationPeer.CreatePeerForElement(button) as IInvokeProvider;
            if (peer != null)
            {
                peer.Invoke();
            }
        }

        #region GlobalCalendar
        /// <summary>
        /// Get the CalendarItem of a GlobalCalendar.
        /// </summary>
        /// <param name="calendar">The GlobalCalendar.</param>
        /// <returns>The GlobalCalendar's CalendarItem.</returns>
        public static GlobalCalendarItem GetCalendarItem(this GlobalCalendar calendar)
        {
            Debug.Assert(calendar != null, "calendar should not be null!");
            return calendar
                .GetVisualDescendants()
                .OfType<GlobalCalendarItem>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Find a button given a date.
        /// </summary>
        /// <param name="calendar">The Calendar.</param>
        /// <param name="day">The date to find.</param>
        /// <returns>The corresponding button.</returns>
        public static GlobalCalendarDayButton FindDayButtonFromDay(this GlobalCalendar calendar, DateTime day)
        {
            GlobalCalendarItem monthControl = calendar.GetCalendarItem();
            if (monthControl != null)
            {
                Grid monthView = monthControl.GetMonthView();
                if (monthView != null)
                {
                    return monthView
                        .Children
                        .OfType<GlobalCalendarDayButton>()
                        .Where(b =>
                        {
                            DateTime? d = b.DataContext as DateTime?;
                            return d != null && CompareDates(d.Value, day);
                        })
                        .FirstOrDefault();
                }
            }

            return null;
        }
        #endregion GlobalCalendar

        #region GlobalCalendarItem
        /// <summary>
        /// Get the MonthView of a CalendarItem.
        /// </summary>
        /// <param name="item">The CalendarItem.</param>
        /// <returns>The MonthView of a CalendarItem.</returns>
        public static Grid GetMonthView(this GlobalCalendarItem item)
        {
            return item.GetTemplatePart<Grid>("MonthView");
        }

        /// <summary>
        /// Get the YearView of a CalendarItem.
        /// </summary>
        /// <param name="item">The CalendarItem.</param>
        /// <returns>The YearView of a CalendarItem.</returns>
        public static Grid GetYearView(this GlobalCalendarItem item)
        {
            return item.GetTemplatePart<Grid>("YearView");
        }

        /// <summary>
        /// Get the HeaderButton of a CalendarItem.
        /// </summary>
        /// <param name="item">The CalendarItem.</param>
        /// <returns>The HeaderButton of a CalendarItem.</returns>
        public static Button GetHeaderButton(this GlobalCalendarItem item)
        {
            return item.GetTemplatePart<Button>("HeaderButton");
        }

        /// <summary>
        /// Get the NextButton of a CalendarItem.
        /// </summary>
        /// <param name="item">The CalendarItem.</param>
        /// <returns>The NextButton of a CalendarItem.</returns>
        public static Button GetNextButton(this GlobalCalendarItem item)
        {
            return item.GetTemplatePart<Button>("NextButton");
        }

        /// <summary>
        /// Get the PreviousButton of a CalendarItem.
        /// </summary>
        /// <param name="item">The CalendarItem.</param>
        /// <returns>The PreviousButton of a CalendarItem.</returns>
        public static Button GetPreviousButton(this GlobalCalendarItem item)
        {
            return item.GetTemplatePart<Button>("PreviousButton");
        }
        #endregion GlobalCalendarItem

        #region DateTimeHelper
        /// <summary>
        /// Compare two dates.
        /// </summary>
        /// <param name="first">The first date.</param>
        /// <param name="second">The second date.</param>
        /// <returns>A date indicating whether the dates are equal.</returns>
        public static bool CompareDates(DateTime first, DateTime second)
        {
            return first.Year == second.Year &&
                first.Month == second.Month &&
                first.Day == second.Day;
        }

        /// <summary>
        /// Get the the current date format.
        /// </summary>
        /// <returns>The current date format.</returns>
        public static DateTimeFormatInfo GetCurrentDateFormat()
        {
            if (CultureInfo.CurrentCulture.Calendar is GregorianCalendar)
            {
                return CultureInfo.CurrentCulture.DateTimeFormat;
            }
            else
            {
                foreach (Globalization.Calendar cal in CultureInfo.CurrentCulture.OptionalCalendars)
                {
                    if (cal is GregorianCalendar)
                    {
                        // if the default calendar is not Gregorian, return the
                        // first supported GregorianCalendar dtfi
                        DateTimeFormatInfo dtfi = new CultureInfo(CultureInfo.CurrentCulture.Name).DateTimeFormat;
                        dtfi.Calendar = cal;
                        return dtfi;
                    }
                }

                // if there are no GregorianCalendars in the OptionalCalendars
                // list, use the invariant dtfi
                DateTimeFormatInfo dt = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
                dt.Calendar = new GregorianCalendar();
                return dt;
            }
        }
        #endregion DateTimeHelper
    }
}