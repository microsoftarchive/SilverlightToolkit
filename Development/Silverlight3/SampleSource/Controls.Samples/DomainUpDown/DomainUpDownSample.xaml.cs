// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;
using System.Reflection;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating DomainUpDown.
    /// </summary>
    [Sample("(0)DomainUpDown", DifficultyLevel.Basic)]
    [Category("DomainUpDown")]
    public partial class DomainUpDownSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainUpDownSample"/> class.
        /// </summary>
        public DomainUpDownSample()
        {
            InitializeComponent();

            Loaded += DomainUpDownSample_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the DomainUpDownSample control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DomainUpDownSample_Loaded(object sender, RoutedEventArgs e)
        {
            IEnumerable airports = Airport.SampleAirports;
            DataContext = airports;

            // fairly random list of cultures
            CultureInfo[] cultures = new[]
                                         {
                                             new CultureInfo("zh-Hans"),    // chinese simplified
                                             new CultureInfo("da"),         // danish
                                             new CultureInfo("nl-NL"),      // dutch
                                             new CultureInfo("en-US"),      // english us
                                             new CultureInfo("fr"),         // french
                                             new CultureInfo("de"),         // german
                                             new CultureInfo("he"),         // hebrew
                                             new CultureInfo("it"),         // italian
                                             new CultureInfo("ru"),         // russian
                                             new CultureInfo("es-ES")       // spanish
                                         };
            cultureList.ItemsSource = cultures;
            cultureList.SelectedIndex = 3;
        }

        /// <summary>
        /// Handles the ParseError event of the DomainUpDown control.
        /// If a color can be found to, a new border will be added to our items collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Branched.UpDownParseErrorEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by event defined in Xaml.")]
        private void DomainUpDown_ParseError(object sender, UpDownParseErrorEventArgs e)
        {
            DomainUpDown dud = (DomainUpDown)sender;

            // get the text that was unable to parse.
            string text = e.Text;

            SolidColorBrush backgroundColor = null;

            // is might be a known color string, like "Yellow" 
            // get by looking at the Colors class.
            PropertyInfo colorPropertyInfo = typeof(Colors).GetProperty(text, BindingFlags.Static | BindingFlags.Public);
            if (colorPropertyInfo != null)
            {
                backgroundColor = new SolidColorBrush((Color)colorPropertyInfo.GetValue(null, new object[] { }));
            }
            else
            {
                // it might be rgba code, like #aarrggbb
                if (text.StartsWith("#", StringComparison.OrdinalIgnoreCase) && text.Length == 9)
                {
                    // rrggbbaa
                    text = text.Substring(1);

                    // test to see if it can be parsed to an int
                    int result;
                    if (Int32.TryParse(text, out result))
                    {
                        byte[] rgba = new byte[4];
                        for (int i = 0; i < 4; i++)
                        {
                            rgba[i] = Byte.Parse(text.Substring(i * 2, 2), CultureInfo.CurrentCulture);
                        }
                        backgroundColor = new SolidColorBrush(new Color { A = rgba[0], B = rgba[3], G = rgba[2], R = rgba[1] });
                    }
                }
            }

            if (backgroundColor != null)
            {
                dud.Items.Add(new Border
                {
                    Width = 120,
                    Height = 80,
                    Background = backgroundColor,
                    BorderBrush = new SolidColorBrush(Colors.Yellow),
                    BorderThickness = new Thickness(4)
                });
                dud.CurrentIndex = dud.Items.Count - 1;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the CultureList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by event defined in Xaml.")]
        private void CultureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CultureInfo c = cultureList.SelectedItem as CultureInfo;
            if (c != null)
            {
                cultureDependentDUD.ConverterCulture = c;
            }
        }
    }
}
