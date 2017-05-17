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
    /// A color selection converter for translating a bool result into
    /// a color.
    /// </summary>
    public sealed class BrushSelector : IValueConverter
    {
        /// <summary>
        /// Gets or sets the color to use for true values.
        /// </summary>
        public Brush TrueBrush { get; set; }

        /// <summary>
        /// Gets or sets the color to use for false values.
        /// </summary>
        public Brush FalseBrush { get; set; }

        /// <summary>
        /// Initializes a new instance of the ColorSelectorConverter type.
        /// </summary>
        public BrushSelector()
        {
        }

        /// <summary>
        /// Convert a boolean value to a Color value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target parameter.</param>
        /// <param name="parameter">ConverterParameter is of type Visibility.</param>
        /// <param name="culture">The culture parameter.</param>
        /// <returns>Returns the object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isTrueColor = true; // Default to true for this converter
            if (value is bool)
            {
                isTrueColor = (bool)value;
            }
            return isTrueColor ? TrueBrush : FalseBrush;
        }

        /// <summary>
        /// Support 2-way databinding of the VisibilityConverter, converting 
        /// Visibility to a boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target parameter.</param>
        /// <param name="parameter">ConverterParameter is of type Visibility.</param>
        /// <param name="culture">The culture parameter.</param>
        /// <returns>Returns the object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}