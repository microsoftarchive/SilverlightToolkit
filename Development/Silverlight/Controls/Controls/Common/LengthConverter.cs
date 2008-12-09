// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Converts instances of other types to and from instances of a Double that
    /// represent an object's length.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public partial class LengthConverter : TypeConverter
    {
        /// <summary>
        /// Conversions from units to pixels.
        /// </summary>
        private static Dictionary<string, double> UnitToPixelConversions = new Dictionary<string, double>
        {
            { "px", 1.0 },
            { "in", 96.0 },
            { "cm", 37.795275590551178 },
            { "pt", 1.3333333333333333 }
        };

        /// <summary>
        /// Initializes a new instance of the LengthConverter class.
        /// </summary>
        public LengthConverter()
        {
        }

        /// <summary>
        /// Determines whether conversion is possible from a specified type to a
        /// Double that represents an object's length.
        /// </summary>
        /// <param name="typeDescriptorContext">
        /// The type descriptor context.
        /// </param>
        /// <param name="sourceType">
        /// Identifies the data type to evaluate for conversion.
        /// </param>
        /// <returns>true if conversion is possible; otherwise, false.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            // Convert numeric types and strings
            switch (Type.GetTypeCode(sourceType))
            {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.String:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Converts instances of other data types into instances of Double that
        /// represent an object's length.
        /// </summary>
        /// <param name="typeDescriptorContext">
        /// The type descriptor context.
        /// </param>
        /// <param name="cultureInfo">The culture used to convert.</param>
        /// <param name="source">
        /// The value that is being converted to Double representing the
        /// object's length.
        /// </param>
        /// <returns>
        /// An instance of Double that is the value of the conversion.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#", Justification = "Compat with WPF.")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "2#", Justification = "Compat with WPF.")]
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
        {
            if (source == null)
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    Controls.Properties.Resources.TypeConverters_ConvertFrom_CannotConvertFromType,
                    GetType().Name,
                    "null");
                throw new NotSupportedException(message);
            }

            string text = source as string;
            if (text != null)
            {
                // Convert Auto to NaN
                if (string.Compare(text, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return double.NaN;
                }

                // Get the unit conversion factor
                string number = text;
                double conversionFactor = 1.0;
                foreach (KeyValuePair<string, double> conversion in UnitToPixelConversions)
                {
                    if (number.EndsWith(conversion.Key, StringComparison.Ordinal))
                    {
                        conversionFactor = conversion.Value;
                        number = text.Substring(0, number.Length - conversion.Key.Length);
                        break;
                    }
                }

                // Convert the value
                try
                {
                    return conversionFactor * Convert.ToDouble(number, cultureInfo);
                }
                catch (FormatException)
                {
                    string message = string.Format(
                        CultureInfo.CurrentCulture,
                        Controls.Properties.Resources.TypeConverters_Convert_CannotConvert,
                        GetType().Name,
                        text,
                        typeof(double).Name);
                    throw new FormatException(message);
                }
            }

            return Convert.ToDouble(source, cultureInfo);
        }

        /// <summary>
        /// Determines whether conversion is possible to a specified type from a
        /// Double that represents an object's length.
        /// </summary>
        /// <param name="typeDescriptorContext">
        /// The type descriptor context.
        /// </param>
        /// <param name="destinationType">
        /// Identifies the data type to evaluate for conversion.
        /// </param>
        /// <returns>
        /// A value indicating whether the conversion is possible.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            return TypeConverters.CanConvertTo<double>(destinationType);
        }

        /// <summary>
        /// Converts other types into instances of Double that represent an
        /// object's length.
        /// </summary>
        /// <param name="typeDescriptorContext">
        /// The type descriptor context.
        /// </param>
        /// <param name="cultureInfo">The culture used to convert.</param>
        /// <param name="value">
        /// The value that is being converted to a specified type.
        /// </param>
        /// <param name="destinationType">
        /// The type to convert the value to.
        /// </param>
        /// <returns>
        /// The value of the conversion to the specified type.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#", Justification = "Compat with WPF.")]
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            // Convert the length to a String
            if (value is double)
            {
                double length = (double) value;
                if (destinationType == typeof(string))
                {
                    return length.IsNaN() ?
                        "Auto" :
                        Convert.ToString(length, cultureInfo);
                }
            }

            return TypeConverters.ConvertTo(this, value, destinationType);
        }
    }
}