// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Extends the CultureInfo class to add Weekdays and Weekends methods.
    /// </summary>
    public static class CultureInfoExtensions
    {
        private static string[] CulturesWithTFWeekends = { "ar-SA" };
        private static string[] CulturesWithFSWeekends = { "he-IL", "ar-EG" };

        /// <summary>
        /// Returns a list of days that are weekdays in the given culture.
        /// </summary>
        /// <param name="culture">The culture to lookup.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Extension method.")]
        public static ReadOnlyCollection<string> Weekdays(this CultureInfo culture)
        {
            DayOfWeek[] daysOfWeek;

            if (CulturesWithTFWeekends.Contains(culture.Name))
            {
                daysOfWeek = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Saturday, DayOfWeek.Sunday };
            }
            else if (CulturesWithFSWeekends.Contains(culture.Name))
            {
                daysOfWeek = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Sunday };
            }
            else
            {
                daysOfWeek = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            }

            List<string> weekdays = new List<string>();

            foreach (DayOfWeek day in daysOfWeek)
            {
                weekdays.Add(culture.DateTimeFormat.GetDayName(day));
            }

            return new ReadOnlyCollection<string>(weekdays);
        }

        /// <summary>
        /// Returns a list of days that are weekends in the given culture.
        /// </summary>
        /// <param name="culture">The culture to lookup.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Extension method.")]
        public static ReadOnlyCollection<string> Weekends(this CultureInfo culture)
        {
            DayOfWeek[] daysOfWeek;

            if (CulturesWithTFWeekends.Contains(culture.Name))
            {
                DayOfWeek[] d = { DayOfWeek.Thursday, DayOfWeek.Friday };
                daysOfWeek = d;
            }
            else if (CulturesWithFSWeekends.Contains(culture.Name))
            {
                DayOfWeek[] d = { DayOfWeek.Friday, DayOfWeek.Saturday };
                daysOfWeek = d;
            }
            else
            {
                DayOfWeek[] d = { DayOfWeek.Saturday, DayOfWeek.Sunday };
                daysOfWeek = d;
            }

            List<string> weekends = new List<string>();

            foreach (DayOfWeek day in daysOfWeek)
            {
                weekends.Add(culture.DateTimeFormat.GetDayName(day));
            }

            return new ReadOnlyCollection<string>(weekends);
        }
    }
}
