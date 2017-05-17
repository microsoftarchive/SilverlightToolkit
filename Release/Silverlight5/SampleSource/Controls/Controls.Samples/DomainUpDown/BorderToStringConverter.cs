// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Reflection;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Converts a border to a friendly name (the color of its background).
    /// </summary>
    /// <remarks>Used in DomainUpDownSample.</remarks>
    public class BorderToStringConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // expecting a border
            Border element = value as Border;
            if (element != null)
            {
                SolidColorBrush b = element.Background as SolidColorBrush;

                if (b != null)
                {
                    // use the colors class to find a friendly name for this color.
                    string colorname = (from c in typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static)
                                        where c.GetValue(null, new object[] { }).Equals(b.Color)
                                        select c.Name).FirstOrDefault();

                    // no friendly name found, use the rgb code.
                    if (String.IsNullOrEmpty(colorname))
                    {
                        colorname = b.Color.ToString();
                    }
                    return colorname;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
