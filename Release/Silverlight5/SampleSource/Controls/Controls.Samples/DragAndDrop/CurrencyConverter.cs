// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Windows.Data;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Converts a number into currency format.
    /// </summary>
    public class CurrencyConverter : IValueConverter
    {
        /// <summary>
        /// Converts from a number to a currency format.
        /// </summary>
        /// <param name="value">The value of amount.</param>
        /// <param name="targetType">The type to convert the value to.</param>
        /// <param name="parameter">Information about the conversion.</param>
        /// <param name="culture">The culture to use for the conversion.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format(CultureInfo.CurrentUICulture, "{0:c}", value);
        }

        /// <summary>
        /// Converts to a number from a currency format.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <param name="targetType">The type to convert the value to.</param>
        /// <param name="parameter">Information about the conversion.</param>
        /// <param name="culture">The culture to use for the conversion.</param>
        /// <returns>The converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}