// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection;
using TypeConverterAttribute = System.ComponentModel.TypeConverterAttribute;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Utiilty class contains common helper functions.
    /// </summary>
    internal static class ParseUtility
    {
        /// <summary>
        /// Try to read a value of type T from passed in TextBox. 
        /// If succeeded, return the value; 
        /// If failed, return passed in default value, and highlighted the text in red.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="tb">The TextBox whose Text value is to be parsed.</param>
        /// <param name="defaultValue">Default value to return in case of failure.</param>
        /// <returns>Value parsed from TextBox.Text or the passed in default value.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "No other common base class for all those exceptions.")]
        public static T ReadValue<T>(TextBox tb, T defaultValue)
        {
            T value = defaultValue;
            bool success = false;

            try
            {
                Type t = typeof(T);

                // first, try Convert.
                try
                {
                    value = (T)Convert.ChangeType(tb.Text, typeof(T), CultureInfo.CurrentCulture);
                    success = true;
                }
                catch (InvalidCastException)
                {
                }

                // second, try TypeConverter.
                if (!success)
                {
                    IEnumerable<TypeConverterAttribute> tcas;
                    TypeConverterAttribute tca;
                    Type tct;
                    TypeConverter tc;

                    if ((tcas = t.GetCustomAttributes(typeof(TypeConverterAttribute), true).Cast<TypeConverterAttribute>()) != null &&
                        (tca = tcas.FirstOrDefault()) != null &&
                        (tct = Type.GetType(tca.ConverterTypeName)) != null &&
                        (tc = Activator.CreateInstance(tct) as TypeConverter) != null &&
                        tc.CanConvertFrom(typeof(string)))
                    {
                        value = (T)tc.ConvertFromString(tb.Text);
                        success = true;
                    }
                }

                if (!success)
                {
                    // last, try Parse method.
                    MethodInfo mi = t.GetMethod("Parse", new Type[] { typeof(string) });
                    if (mi != null)
                    {
                        value = (T)mi.Invoke(null, new object[] { tb.Text });
                        success = true;
                    }
                }
            }
            catch (Exception)
            {
                // don't want to throw any exception.
            }

            if (success)
            {
                tb.Foreground = new SolidColorBrush(Colors.Black);
                return value;
            }
            else
            {
                tb.Foreground = new SolidColorBrush(Colors.Red);
                return defaultValue;
            }
        }
    }
}
