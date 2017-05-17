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
    /// A specialized data template selector.
    /// </summary>
    public sealed class DataTemplateSelector : IValueConverter
    {
        /// <summary>
        /// Gets or sets the default data template.
        /// </summary>
        public DataTemplate DefaultDataTemplate { get; set; }

        /// <summary>
        /// Gets or sets the test method template.
        /// </summary>
        public DataTemplate TestMethodTemplate { get; set; }

        /// <summary>
        /// Gets or sets the test class template.
        /// </summary>
        public DataTemplate TestClassTemplate { get; set; }

        /// <summary>
        /// Initializes a new instance of the DataTemplateSelector type.
        /// </summary>
        public DataTemplateSelector()
        {
        }

        /// <summary>
        /// Convert a value to a data template.
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
                Type type = value.GetType();

                if (typeof(TestMethodData).TypeHandle == type.TypeHandle)
                {
                    return TestMethodTemplate;
                }
                else if (typeof(TestClassData).TypeHandle == type.TypeHandle)
                {
                    return TestClassTemplate;
                }
            }

            return DefaultDataTemplate;
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