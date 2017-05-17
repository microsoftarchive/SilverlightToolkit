// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A specialized bool inversion selector.
    /// </summary>
    public sealed class InvertValueConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the InvertValueConverter class.
        /// </summary>
        public InvertValueConverter()
        {
        }

        /// <summary>
        /// Convert a value to its opposite. Defines that null value will also
        /// return false.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target parameter.</param>
        /// <param name="parameter">ConverterParameter value.</param>
        /// <param name="culture">The culture parameter.</param>
        /// <returns>Returns the object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                bool b = (bool)value;
                return !b;
            }

            // Defined that null will return false.
            return false;
        }

        /// <summary>
        /// No 2-way databinding support.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target parameter.</param>
        /// <param name="parameter">ConverterParameter value.</param>
        /// <param name="culture">The culture parameter.</param>
        /// <returns>Returns the object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}