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
    /// A value converter for collapsing or showing elements.
    /// </summary>
    public sealed class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert a boolean value to a Visibility value.
        /// </summary>
        /// <param name="value">The value instance.</param>
        /// <param name="targetType">The target parameter.</param>
        /// <param name="parameter">ConverterParameter is of type Visibility.</param>
        /// <param name="culture">The culture parameter.</param>
        /// <returns>Returns the object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value is a bool, then it is cast and used. Else, the 
            // condition is whether the object is null or not.
            bool isVisible;
            if (value is bool)
            {
                isVisible = (bool)value;
            }
            else
            {
                isVisible = value != null;
            }

            // If visibility is inverted by the converter parameter, then invert our value
            if (IsVisibilityInverted(parameter))
            {
                isVisible = !isVisible;
            }

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Support two-way databinding of the VisibilityConverter, converting 
        /// Visibility to a bool.
        /// </summary>
        /// <param name="value">The value instance.</param>
        /// <param name="targetType">The target parameter.</param>
        /// <param name="parameter">ConverterParameter is of type Visibility.</param>
        /// <param name="culture">The culture parameter.</param>
        /// <returns>Returns the object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isVisible = ((Visibility)value == Visibility.Visible);

            // If visibility is inverted by the converter parameter, then invert
            // our value
            if (IsVisibilityInverted(parameter))
            {
                isVisible = !isVisible;
            }

            return isVisible;
        }

        /// <summary>
        /// Determine the visibility mode based on a converter parameter. This
        /// parameter is of type Visibility,and specifies what visibility value
        /// to return when the boolean value is true.
        /// </summary>
        /// <param name="parameter">The parameter object.</param>
        /// <returns>Returns a Visibility value.</returns>
        private static Visibility GetVisibilityMode(object parameter)
        {
            // Default to Visible
            Visibility mode = Visibility.Visible;

            // If a parameter is specified, then we'll try to understand it as a
            // Visibility value
            if (parameter != null)
            {
                // If it's already a Visibility value, then just use it
                if (parameter is Visibility)
                {
                    mode = (Visibility)parameter;
                }
                else
                {
                    // Try to parse the parameter as a Visibility value,
                    // throwing an exception when the parsing fails
                    try
                    {
                        mode = (Visibility)Enum.Parse(typeof(Visibility), parameter.ToString(), true);
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException("Invalid Visibility specified as the ConverterParameter. Use Visible or Collapsed.", e);
                    }
                }
            }

            // Return the detected mode
            return mode;
        }

        /// <summary>
        /// Determine whether or not visibility is inverted based on a converter
        /// parameter. When the parameter is specified as Collapsed, that means
        /// that when the boolean value is true, we should return Collapsed,
        /// which is inverted.
        /// </summary>
        /// <param name="parameter">The parameter object.</param>
        /// <returns>Returns a value indicating whether the visibility is 
        /// inverted.</returns>
        private static bool IsVisibilityInverted(object parameter)
        {
            return (GetVisibilityMode(parameter) == Visibility.Collapsed);
        }
    }
}