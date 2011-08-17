// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Date and time converter for threads.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class ThreadDateTimeConverter : IValueConverter
    {
        /// <summary>
        /// Converts a
        /// <see cref="T:System.DateTime"/>
        /// object into a string appropriately formatted for threads.
        /// This format can be found in messaging.
        /// </summary>
        /// <remarks>
        /// This format never displays the year.
        /// </remarks>
        /// <param name="value">The given date and time.</param>
        /// <param name="targetType">
        /// The type corresponding to the binding property, which must be of
        /// <see cref="T:System.String"/>.
        /// </param>
        /// <param name="parameter">(Not used).</param>
        /// <param name="culture">(Not used).</param>
        /// <returns>The given date and time as a string.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Target value must be a System.DateTime object.
            if (!(value is DateTime))
            {
                throw new ArgumentException(Properties.Resources.InvalidDateTimeArgument);
            }

            string result;

            DateTime given = (DateTime)value;

            DateTime current = DateTime.Now;

            if (DateTimeFormatHelper.IsFutureDateTime(current, given))
            {
                // Future dates and times are not supported.
                throw new NotSupportedException(Properties.Resources.NonSupportedDateTime);
            }

            if (DateTimeFormatHelper.IsAnOlderWeek(current, given))
            {
                result = DateTimeFormatHelper.GetMonthAndDay(given);
            }
            else if (DateTimeFormatHelper.IsPastDayOfWeekWithWindow(current, given))
            {
                result = DateTimeFormatHelper.GetAbbreviatedDay(given);
            }
            else
            {
                // Given day time is today.
                result = DateTimeFormatHelper.GetSuperShortTime(given);
            }

            return result;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">(Not used).</param>
        /// <param name="targetType">(Not used).</param>
        /// <param name="parameter">(Not used).</param>
        /// <param name="culture">(Not used).</param>
        /// <returns>null</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
