// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A converter for modifying the font weight based on a parameter.
    /// </summary>
    public sealed class FontWeightConverter : IValueConverter
    {
        /// <summary>
        /// Convert a boolean value to a FontWeight value.
        /// </summary>
        /// <param name="value">The value object.</param>
        /// <param name="targetType">The target parameter.</param>
        /// <param name="parameter">ConverterParameter is of type Visibility.</param>
        /// <param name="culture">The culture parameter.</param>
        /// <returns>Returns the object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isBold = (bool)value;

            // If visibility is inverted by the converter parameter, invert.
            if (IsFontWeightInverted(parameter))
            {
                isBold = !isBold;
            }

            return isBold ? FontWeights.Bold : FontWeights.Normal;
        }

        /// <summary>
        /// Support 2-way databinding of the VisibilityConverter, converting 
        /// Visibility to a bool.
        /// </summary>
        /// <param name="value">The value object.</param>
        /// <param name="targetType">The target parameter.</param>
        /// <param name="parameter">ConverterParameter is of type Visibility.</param>
        /// <param name="culture">The culture parameter.</param>
        /// <returns>Returns the object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isBold = ((Visibility)value == Visibility.Visible);

            // If visibility is inverted by the converter parameter, then
            // invert our value
            if (IsFontWeightInverted(parameter))
            {
                isBold = !isBold;
            }

            return isBold;
        }

        /// <summary>
        /// Determine the visibility mode based on a converter parameter. This
        /// parameter is of type Visibility, and specifies what visibility value
        /// to return when the boolean value is true.
        /// </summary>
        /// <param name="parameter">The parameter object.</param>
        /// <returns>Returns a FontWeight value.</returns>
        private static FontWeight GetFontWeightMode(object parameter)
        {
            // Default to Visible
            FontWeight mode = FontWeights.Bold;

            // If a parameter is specified, then we'll try to understand it as a
            // FontWeight value
            if (parameter != null)
            {
                // If it's already a FontWeight value, then just use it
                if (parameter is FontWeight)
                {
                    mode = (FontWeight)parameter;
                }
                else
                {
                    // Let's try to parse the parameter as a FontWeight value,
                    // throwing an exception when the parsing fails
                    try
                    {
                        if (parameter is string)
                        {
                            string p = (string)parameter;
                            if (p == "Normal")
                            {
                                mode = FontWeights.Normal;
                            }
                        }
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException("Invalid FontWeight specified as the ConverterParameter.  Use Bold or Normal.", e);
                    }
                }
            }

            return mode;
        }

        /// <summary>
        /// Determine whether or not weight is inverted based on a converter
        /// parameter.
        /// </summary>
        /// <param name="parameter">The parameter instance.</param>
        /// <returns>Returns a value indicating whether the instance is 
        /// inverting.</returns>
        private static bool IsFontWeightInverted(object parameter)
        {
            return (GetFontWeightMode(parameter) == FontWeights.Normal);
        }
    }
}