// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// The XamlConverter class is used as a very simple XamlWriter for the
    /// purpose of testing.  It allows writing values as either attributes or
    /// elements.
    /// </summary>
    public static class XamlConverter
    {
        /// <summary>
        /// Determine whether the object can be converted to a XAML attribute
        /// using the XamlConverter.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>
        /// A value indicating whether the object can be converted.
        /// </returns>
        public static bool CanConvertToAttribute(object value)
        {
            if (value is SolidColorBrush)
            {
                return true;
            }
            else if (value is Brush || value is UIElement || value is Array || value is OperatingSystem)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Convert an object to a XAML attribute.
        /// </summary>
        /// <param name="value">Object to convert.</param>
        /// <returns>XAML representation.</returns>
        public static string ConvertToAttribute(object value)
        {
            SolidColorBrush b = value as SolidColorBrush;
            if (b != null)
            {
                return b.Color.ToString(CultureInfo.InvariantCulture);
            }
            else if (value is Brush)
            {
                throw new ArgumentException("Can only convert from SolidColorBrush.");
            }
            else if (value is DateTime)
            {
                DateTime date = (DateTime)value;
                return date.ToShortDateString();
            }
            else
            {
                return value != null ?
                    Convert.ToString(value, CultureInfo.InvariantCulture) :
                    null;
            }
        }

        /// <summary>
        /// Determine whether the object can be converted to a XAML element
        /// using the XamlConverter.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>
        /// A value indicating whether the object can be converted.
        /// </returns>
        public static bool CanConvertToElement(object value)
        {
            if (value is SolidColorBrush)
            {
                return true;
            }
            else if (value is Brush || value is UIElement || value is Array || value is OperatingSystem)
            {
                return false;
            }
            else
            {
                return value != null;
            }
        }

        /// <summary>
        /// Convert an object to a XAML element.
        /// </summary>
        /// <param name="value">Object to convert.</param>
        /// <returns>XAML representation.</returns>
        public static XamlBuilder ConvertToElement(object value)
        {
            SolidColorBrush b = value as SolidColorBrush;
            if (b != null)
            {
                return new XamlBuilder<SolidColorBrush>
                    {
                        AttributeProperties = new Dictionary<string, string> { { "Color", b.Color.ToString(CultureInfo.InvariantCulture) } }
                    };
            }
            else if (value is Brush)
            {
                throw new ArgumentException("Can only convert from SolidColorBrush.");
            }
            else
            {
                bool isLiteral =
                    value is bool ||
                    value is FontStretch ||
                    value is FontStyle ||
                    value is FontWeight ||
                    value is HorizontalAlignment ||
                    value is VerticalAlignment;
                return (value == null) ?
                    null :
                    new XamlBuilder
                    {
                        ElementType = value.GetType(),
                        Content = Convert.ToString(value, CultureInfo.InvariantCulture),
                        IsLiteral = isLiteral
                    };
            }
        }
    }
}