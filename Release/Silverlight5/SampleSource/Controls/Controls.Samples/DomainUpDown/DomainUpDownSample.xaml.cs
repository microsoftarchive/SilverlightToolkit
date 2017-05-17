// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Generic;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating DomainUpDown.
    /// </summary>
    [Sample("(0)DomainUpDown", DifficultyLevel.Basic, "DomainUpDown")]
    public partial class DomainUpDownSample : UserControl
    {
        #region Cultures
        /// <summary>
        /// Cultures that are recognized on most configurations.
        /// </summary>
        private readonly string[] _cultureNames = new[]
                                             {
                                                     "af", "af-ZA", "ar", "ar-AE", "ar-BH", "ar-DZ", "ar-EG", "ar-IQ", "ar-JO", "ar-KW", "ar-LB", "ar-LY",
                                                     "ar-MA", "ar-OM", "ar-QA", "ar-SA", "ar-SY", "ar-TN", "ar-YE", "az", "az-Cyrl-AZ", "az-Latn-AZ", "be",
                                                     "be-BY", "bg", "bg-BG", "ca", "ca-ES", "cs", "cs-CZ", "da", "da-DK", "de", "de-AT", "de-CH", "de-DE",
                                                     "de-LI", "de-LU", "dv", "dv-MV", "el", "el-GR", "en", "en-029", "en-AU", "en-BZ", "en-CA", "en-GB", "en-IE", 
                                                     "en-JM", "en-NZ", "en-PH", "en-TT", "en-US", "en-ZA", "en-ZW", "es", "es-AR", "es-BO", "es-CL", "es-CO",
                                                     "es-CR", "es-DO", "es-EC", "es-ES", "es-GT", "es-HN", "es-MX", "es-NI", "es-PA", "es-PE", "es-PR", "es-PY",
                                                     "es-SV", "es-UY", "es-VE", "et", "et-EE", "eu", "eu-ES", "fa", "fa-IR", "fi", "fi-FI", "fo", "fo-FO", "fr",
                                                     "fr-BE", "fr-CA", "fr-CH", "fr-FR", "fr-LU", "fr-MC", "gl", "gl-ES", "gu", "gu-IN", "he", "he-IL", "hi",
                                                     "hi-IN", "hr", "hr-HR", "hu", "hu-HU", "hy", "hy-AM", "id", "id-ID", "is", "is-IS", "it", "it-CH", "it-IT",
                                                     "ja", "ja-JP", "ka", "ka-GE", "kk", "kk-KZ", "kn", "kn-IN", "ko", "kok", "kok-IN", "ko-KR", "ky", "ky-KG",
                                                     "lt", "lt-LT", "lv", "lv-LV", "mk", "mk-MK", "mn", "mn-MN", "mr", "mr-IN", "ms", "ms-BN", "ms-MY", "nb-NO",
                                                     "nl", "nl-BE", "nl-NL", "nn-NO", "no", "pa", "pa-IN", "pl", "pl-PL", "pt", "pt-BR", "pt-PT", "ro", "ro-RO",
                                                     "ru", "ru-RU", "sa", "sa-IN", "sk", "sk-SK", "sl", "sl-SI", "sq", "sq-AL", "sr", "sr-Cyrl-CS", "sr-Latn-CS", 
                                                     "sv", "sv-FI", "sv-SE", "sw", "sw-KE", "syr", "syr-SY", "ta", "ta-IN", "te", "te-IN", "th", "th-TH", "tr", 
                                                     "tr-TR", "tt", "tt-RU", "uk", "uk-UA", "ur", "ur-PK", "uz", "uz-Cyrl-UZ", "uz-Latn-UZ", "vi", "vi-VN",
                                                     "zh-CHS", "zh-CHT", "zh-CN", "zh-HK", "zh-MO", "zh-SG", "zh-TW"
                                             };
        #endregion

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

            List<CultureInfo> cultures = new List<CultureInfo>();

            // work through long list of cultures and check if it is actually 
            // allowed in this configuration.
            foreach (string cultureName in _cultureNames)
            {
                try
                {
                    CultureInfo c = new CultureInfo(cultureName);
                    cultures.Add(c);
                }
                catch (ArgumentException)
                {
                }
            }

            cultureList.ItemsSource = cultures;
            // preselect dutch, if allowed.
            cultureList.SelectedItem = cultures.FirstOrDefault(info => info.Name == "nl-NL");
        }

        /// <summary>
        /// Handles the ParseError event of the DomainUpDown control.
        /// If a color can be found to, a new border will be added to our items collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Branched.UpDownParseErrorEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by event defined in Xaml.")]
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
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by event defined in Xaml.")]
        private void CultureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CultureInfo c = cultureList.SelectedItem as CultureInfo;
            if (c != null)
            {
                Binding copy = new Binding(cultureDependentDUD.ValueMemberPath)
                {
                    Converter = cultureDependentDUD.ValueMemberBinding.Converter,
                    ConverterCulture = c
                };
                cultureDependentDUD.ValueMemberBinding = copy;
            }
        }
    }
}
