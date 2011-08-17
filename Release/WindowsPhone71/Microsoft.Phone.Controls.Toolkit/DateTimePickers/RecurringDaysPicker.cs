// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Phone.Controls.LocalizedResources;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Represents a control that allows the user to choose days of the week.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public class RecurringDaysPicker : ListPicker
    {
        private const string CommaSpace = ", ";

        private const string EnglishLanguage = "en";

        private string[] DayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
        private string[] ShortestDayNames = CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames;

        /// <summary>
        /// Initializes a new instance of the RecurringDaysPicker control.
        /// </summary>
        public RecurringDaysPicker()
        {
            if (CultureInfo.CurrentCulture.Name.StartsWith(EnglishLanguage, StringComparison.OrdinalIgnoreCase))
            {
                // The shortestDayNames array shortens English weekdays to two letters.
                // The native experience has 3 letters, so we initialize it correclty here.
                ShortestDayNames = new string[] { "Sun",
                                                  "Mon",
                                                  "Tue",
                                                  "Wed",
                                                  "Thu",
                                                  "Fri",
                                                  "Sat" };
            }

            DayOfWeek dow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            for (int i = 0; i < DayNames.Count(); i++)
            {
                Items.Add(DayNames[((int)dow + i) % DayNames.Count()]);
            }

            SelectionMode = SelectionMode.Multiple;

            // Add a custom sumarizer, which will take a list of days and return a shortened string representation
            SummaryForSelectedItemsDelegate = SummarizeDaysOfWeek;
        }

        /// <summary>
        /// Sumarizes a list of days into a shortened string representation.
        /// If all days, all weekdays, or all weekends are in the list, then the string includes 
        ///     the corresponding name rather than listing out all of those days separately.
        /// If individual days are listed, they are abreviated.
        /// If the list is null or empty, "only once" is returned.
        /// </summary>
        /// <param name="selection">The list of days. Can be empty or null.</param>
        /// <returns>A string representation of the list of days.</returns>
        protected string SummarizeDaysOfWeek(IList selection)
        {
            string str = ControlResources.RepeatsOnlyOnce;

            if (null != selection)
            {
                List<string> contents = new List<string>();
                foreach (object o in selection)
                {
                    contents.Add((string)o);
                }
                str = DaysOfWeekToString(contents);
            }

            return str;
        }

        /// <summary>
        /// Sumarizes a list of days into a shortened string representation.
        /// If all days, all weekdays, or all weekends are in the list, then the string includes 
        ///     the corresponding name rather than listing out all of those days separately.
        /// If individual days are listed, they are abreviated.
        /// If the list is empty, "only once" is returned.
        /// </summary>
        /// <param name="daysList">The list of days. Can be empty.</param>
        /// <returns>A string representation of the list of days.</returns>
        private string DaysOfWeekToString(List<string> daysList)
        {
            List<string> days = new List<string>();

            foreach (string day in daysList)
            {
                // Only include unique days of the week. 
                // Though a list *should* never have duplicate days, protect against it anyways.
                if (!days.Contains(day))
                {
                    days.Add(day);
                }
            }

            // No days chosen, return the 'only once' string
            if (days.Count == 0)
            {
                return ControlResources.RepeatsOnlyOnce;
            }

            StringBuilder builder = new StringBuilder();

            IEnumerable<string> unhandledDays;

            builder.Append(HandleGroups(days, out unhandledDays));

            if (builder.Length > 0)
            {
                builder.Append(CommaSpace);
            }

            DayOfWeek dow = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            for (int i = 0; i < DayNames.Count(); i++)
            {
                int index = ((int)dow + i) % DayNames.Count();
                string day = DayNames[index];

                if (unhandledDays.Contains(day))
                {
                    builder.Append(ShortestDayNames[index]);
                    builder.Append(CommaSpace);
                }
            }

            // trim off the remaining ", " characters, as it was the last day
            builder.Length -= CommaSpace.Length;
            return builder.ToString();
        }

        /// <summary>
        /// Finds a group (weekends, weekdays, every day) within a list of days and returns a string representing that group.
        /// Days that are not in a group are set in the unhandledDays out parameter.
        /// </summary>
        /// <param name="days">List of days</param>
        /// <param name="unhandledDays">Out parameter which will be written to with the list of days that were not in a group.</param>
        /// <returns>String of any group found.</returns>
        private static string HandleGroups(List<string> days, out IEnumerable<string> unhandledDays)
        {
            // First do a check for all of the days of the week, and replace it with the 'every day' string
            if (days.Count == 7)
            {
                unhandledDays = new List<string>();
                return ControlResources.RepeatsEveryDay;
            }

            var weekdays = CultureInfo.CurrentCulture.Weekdays();
            var weekends = CultureInfo.CurrentCulture.Weekends();

            if (days.Intersect(weekdays).Count() == weekdays.Count)
            {
                unhandledDays = days.Where(day => !weekdays.Contains(day));
                return ControlResources.RepeatsOnWeekdays;
            }
            else if (days.Intersect(weekends).Count() == weekends.Count)
            {
                unhandledDays = days.Where(day => !weekends.Contains(day));
                return ControlResources.RepeatsOnWeekends;
            }
            else
            {
                unhandledDays = days;
                return string.Empty;
            }
        }
    }
}
