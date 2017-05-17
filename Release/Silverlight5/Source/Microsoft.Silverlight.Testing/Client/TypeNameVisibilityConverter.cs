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
    /// A value converter for collapsing or showing elements based on the bound
    /// object's type name. Does not walk the hierarchy - it is explicit to the
    /// most specific class for the value.
    /// </summary>
    public class TypeNameVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets a value indicating whether the visibility value should be
        /// inverted.
        /// </summary>
        protected virtual bool IsInverted { get { return false; } }

        /// <summary>
        /// Convert a value based on CLR type to a Visibility value. Does not
        /// walk the type tree, however.
        /// </summary>
        /// <param name="value">The value instance.</param>
        /// <param name="targetType">The target parameter.</param>
        /// <param name="parameter">ConverterParameter is the name of the type,
        /// both short and full names are checked, short name first.</param>
        /// <param name="culture">The culture parameter.</param>
        /// <returns>Returns the object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = false;
            if (parameter != null && value != null)
            {
                string name = parameter as string;
                if (name == null)
                {
                    throw new InvalidOperationException("The parameter must be of type System.String.");
                }

                Type t = value.GetType();
                isVisible = t.Name == name || t.FullName == name;
            }

            // If visibility is inverted by the converter parameter, then invert our value
            if (IsInverted)
            {
                isVisible = !isVisible;
            }

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Convert back, not supported with this value converter.
        /// </summary>
        /// <param name="value">The value instance.</param>
        /// <param name="targetType">The target parameter.</param>
        /// <param name="parameter">ConverterParameter is of type Visibility.</param>
        /// <param name="culture">The culture parameter.</param>
        /// <returns>Returns the object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}