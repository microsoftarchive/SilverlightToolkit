// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PhoneToolkitSample.Data
{
    /// <summary>
    /// A converter that takes a name of an accent color and returns a SolidColorBrush.
    /// </summary>
    public class AccentColorNameToBrush : IValueConverter
    {
        private static Dictionary<string, SolidColorBrush> ColorNameToBrush = new Dictionary<string, SolidColorBrush>()
        {
            { "magenta", 0xFFFF0097.ToSolidColorBrush() },
            { "purple",  0xFFA200FF.ToSolidColorBrush() },
            { "teal",    0xFF00ABA9.ToSolidColorBrush() },
            { "lime",    0xFF8CBF26.ToSolidColorBrush() },
            { "brown",   0xFFA05000.ToSolidColorBrush() },
            { "pink",    0xFFE671B8.ToSolidColorBrush() },
            { "orange",  0xFFF09609.ToSolidColorBrush() },
            { "blue",    0xFF1BA1E2.ToSolidColorBrush() },
            { "red",     0xFFE51400.ToSolidColorBrush() },
            { "green",   0xFF339933.ToSolidColorBrush() },
            { "mango",   0xFFF09609.ToSolidColorBrush() },
        };

        /// <summary>
        /// Converts a name of an accent color to a SolidColorBrush.
        /// </summary>
        /// <param name="value">The accent color as a string.</param>
        /// <param name="targetType">The target type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>A SolidColorBrush representing the accent color.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "By design")]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string v = value as string;
            if (null == v)
            {
                throw new ArgumentNullException("value");
            }

            SolidColorBrush brush = null;
            if (ColorNameToBrush.TryGetValue(v.ToLowerInvariant(), out brush))
            {
                return brush;
            }

            return null;
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}