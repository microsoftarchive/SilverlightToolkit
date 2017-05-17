// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A set of simple time helper methods.
    /// </summary>
    internal static class TimeHelper
    {
        /// <summary>
        /// Returns a human-readable formatting of the time different between 
        /// two DateTime instances.
        /// </summary>
        /// <param name="start">The starting time.</param>
        /// <param name="finish">The finishing time.</param>
        /// <returns>Returns a human-readable string.</returns>
        public static string ElapsedReadableTime(DateTime start, DateTime finish)
        {
            TimeSpan ts = new TimeSpan(finish.Ticks - start.Ticks);
            return ElapsedReadableTime(ts);
        }

        /// <summary>
        /// Returns a human-readable formatting of the time different between 
        /// two DateTime instances.
        /// </summary>
        /// <param name="ts">The time span instance.</param>
        /// <returns>Returns a human-readable string.</returns>
        public static string ElapsedReadableTime(TimeSpan ts)
        {
            List<string> parts = new List<string>();

            if (ts.Milliseconds > 0 && ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 0)
            {
                parts.Add(ts.Milliseconds.ToString(CultureInfo.CurrentCulture) + " ms");
            }
            if (ts.Seconds > 0 && ts.Days == 0 && ts.Hours == 0)
            {
                parts.Add(ts.Seconds.ToString(CultureInfo.CurrentCulture) + " second".Plural(ts.Seconds));
            }
            if (ts.Minutes > 0)
            {
                parts.Add(ts.Minutes.ToString(CultureInfo.CurrentCulture) + " minute".Plural(ts.Minutes));
            }
            if (ts.Hours > 0)
            {
                parts.Add(ts.Hours.ToString(CultureInfo.CurrentCulture) + " hour".Plural(ts.Hours));
            }
            if (ts.Days > 0)
            {
                parts.Add(ts.Days.ToString(CultureInfo.CurrentCulture) + " day".Plural(ts.Days));
            }
            parts.Reverse();
            return string.Join(" ", parts.ToArray());
        }

        /// <summary>
        /// A plural 's' as the suffix, when not equal to one.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="number">The number to check.</param>
        /// <returns>Returns an empty string or the English plural 's'.</returns>
        private static string Plural(this string value, int number)
        {
            return number != 1 ? value + "s" : value;
        }
    }
}