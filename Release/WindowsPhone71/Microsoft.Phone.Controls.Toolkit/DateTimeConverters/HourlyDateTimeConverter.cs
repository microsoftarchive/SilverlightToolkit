// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Text;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Date and time converter for hourly feeds.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class HourlyDateTimeConverter : IValueConverter
    {
        /// <summary>
        /// Converts a
        /// <see cref="T:System.DateTime"/>
        /// object into a string appropriately formatted for hourly feeds.
        /// This format can be found in messaging.
        /// </summary>
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

            StringBuilder result = new StringBuilder(string.Empty);

            DateTime given = (DateTime)value;

            DateTime current = DateTime.Now;

            if (DateTimeFormatHelper.IsFutureDateTime(current, given))
            {
                // Future dates and times are not supported.
                throw new NotSupportedException(Properties.Resources.NonSupportedDateTime);
            }

            if (DateTimeFormatHelper.IsAnOlderYear(current, given))
            {
                result.AppendFormat(CultureInfo.CurrentCulture, "{0}, {1}",
                                            DateTimeFormatHelper.GetShortDate(given),
                                            DateTimeFormatHelper.GetSuperShortTime(given));
            }
            else if (DateTimeFormatHelper.IsAnOlderWeek(current, given))
            {
                result.AppendFormat(CultureInfo.CurrentCulture, "{0}, {1}",
                                            DateTimeFormatHelper.GetMonthAndDay(given),
                                            DateTimeFormatHelper.GetSuperShortTime(given));
            }
            else if (DateTimeFormatHelper.IsPastDayOfWeekWithWindow(current, given))
            {
                result.AppendFormat(CultureInfo.CurrentCulture, "{0}, {1}",
                                            DateTimeFormatHelper.GetAbbreviatedDay(given),
                                            DateTimeFormatHelper.GetSuperShortTime(given));
            }
            else
            {
                // Given day time is today.
                result.Append(DateTimeFormatHelper.GetSuperShortTime(given));
            }

            if (DateTimeFormatHelper.IsCurrentUICultureFrench())
            {
                result.Replace(",", string.Empty);
            }

            return result.ToString();
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
