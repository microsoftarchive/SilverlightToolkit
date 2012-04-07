// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Phone.Controls.LocalizedResources;
using Microsoft.Phone.Controls.Properties;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Converts bool? values to "Off" and "On" strings.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class OffOnConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean to the equivalent On or Off string.
        /// </summary>
        /// <param name="value">The given boolean.</param>
        /// <param name="targetType">
        /// The type corresponding to the binding property, which must be of
        /// <see cref="T:System.String"/>.
        /// </param>
        /// <param name="parameter">(Not used).</param>
        /// <param name="culture">(Not used).</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            if (targetType != typeof(object))
            {
                throw new ArgumentException(Resources.UnexpectedType, "targetType");
            }
            if (value is bool? || value == null)
            {
                return (bool?)value == true ? ControlResources.On : ControlResources.Off;
            }
            throw new ArgumentException(Resources.UnexpectedType, "value");
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