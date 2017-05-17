// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Converts a number into the sequence of its factors.
    /// </summary>
    /// <remarks>
    /// This is used to demonstrate a hierarchical data source where items
    /// appear in the tree multiple times.
    /// </remarks>
    public class FactorizationValueConverter : IValueConverter
    {
        /// <summary>
        /// Convert a number into a sequence of its factors.
        /// </summary>
        /// <param name="value">The number.</param>
        /// <param name="targetType">An ignored target type.</param>
        /// <param name="parameter">An ignored parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>A sequence of the number's factors.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int number = (int)value;
            return Enumerable.Range(2, (number + 1) / 2 - 1).Where(i => number % i == 0);
        }

        /// <summary>
        /// Throws a NotSupportedException.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The conversion parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>Always throws a NotSupportedException.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Get a HierarchicalDataTemplate that can be used to bind the
        /// factorization.
        /// </summary>
        /// <returns>
        /// A HierarchicalDataTemplate that can be used to bind the
        /// factorization.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not used like a property.")]
        public static HierarchicalDataTemplate GetDataTemplate()
        {
            string templateKey = "template";

            // Wrap the template in a Grid because the XAML parser doesn't like
            // namespaces declared on a DataTemplate
            XamlBuilder<Grid> builder = new XamlBuilder<Grid>
            {
                ExplicitNamespaces = new Dictionary<string, string>
                {
                    { "common", XamlBuilder.GetNamespace(typeof(HierarchicalDataTemplate)) },
                    { "local", XamlBuilder.GetNamespace(typeof(FactorizationValueConverter)) }
                },
                ElementProperties = new Dictionary<string, XamlBuilder>
                {
                    {
                        "Resources",
                        new XamlBuilder<FactorizationValueConverter>
                        {
                            Key = "Factorization"
                        }
                    }
                },
                Children = new List<XamlBuilder>
                {
                    new XamlBuilder<Grid>
                    {
                        ElementProperties = new Dictionary<string, XamlBuilder>
                        {
                            {
                                "Resources",
                                new XamlBuilder<HierarchicalDataTemplate>
                                {
                                    Key = templateKey,
                                    AttributeProperties = new Dictionary<string, string>
                                    {
                                        { "ItemsSource", "{Binding Converter={StaticResource Factorization}}" }
                                    },
                                    Children = new List<XamlBuilder>
                                    {
                                        new XamlBuilder<ContentControl>
                                        {
                                            Name = "TemplateContent",
                                            AttributeProperties = new Dictionary<string, string>
                                            {
                                                { "Content", "{Binding}" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }                
            };
            
            Grid outer = builder.Load();
            Grid inner = outer.Children[0] as Grid;
            return inner.Resources[templateKey] as HierarchicalDataTemplate;
        }
    }
}