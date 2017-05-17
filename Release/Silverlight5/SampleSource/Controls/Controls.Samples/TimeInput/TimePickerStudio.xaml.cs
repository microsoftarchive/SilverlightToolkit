// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample that allows setting properties on a TimePicker.
    /// </summary>
    [Sample("(3)TimePicker Studio", DifficultyLevel.Intermediate, "TimePicker")]
    public partial class TimePickerStudio : UserControl
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
        /// Initializes a new instance of the <see cref="TimePickerStudio"/> class.
        /// </summary>
        public TimePickerStudio()
        {
            InitializeComponent();

            Loaded += TimePickerStudio_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the TimePickerStudio control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void TimePickerStudio_Loaded(object sender, RoutedEventArgs e)
        {
            // init
            cmbPopupSelectionMode.ItemsSource = typeof(PopupTimeSelectionMode)
                .GetMembers()
                .ToList()
                .Where(m =>
                    m.DeclaringType.Equals(typeof(PopupTimeSelectionMode)) &&
                    !m.Name.StartsWith("_", StringComparison.Ordinal) &&
                    !m.Name.EndsWith("_", StringComparison.Ordinal))
                .Select(m => m.Name)
                .ToList();

            cmbPopup.ItemsSource = new Dictionary<string, Type>()
                                       {
                                           { "ListTimePicker", typeof(ListTimePickerPopup) },
                                           { "RangeTimePicker", typeof(RangeTimePickerPopup) },
                                       };

            cmbFormat.ItemsSource = new Dictionary<string, ITimeFormat>()
                                        {
                                            { "ShortTimeFormat", new ShortTimeFormat() },
                                            { "LongTimeFormat", new LongTimeFormat() },
                                            { "Custom: hh:mm:ss", new CustomTimeFormat("hh:mm:ss") },
                                            { "Custom: hh.mm", new CustomTimeFormat("hh.mm") },
                                        };

            cmbTimeParser.ItemsSource = new Dictionary<string, TimeParser>()
                                            {
                                                { "+/- hours, try +3h", new PlusMinusHourTimeParser() },
                                                { "+/- minutes, try +3m", new PlusMinusMinuteTimeInputParser() },
                                            };

            // defaults
            cmbFormat.SelectedIndex = 0;
            cmbPopupSecondsInterval.SelectedIndex = 1;
            cmbPopupMinutesInterval.SelectedIndex = 3;
            cmbPopupSelectionMode.SelectedIndex = cmbPopupSelectionMode.Items.ToList().IndexOf(tp.PopupTimeSelectionMode.ToString());
            cmbPopup.SelectedIndex = 0;

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

            cmbCultures.ItemsSource = cultures;
            // preselect current culture.
            cmbCultures.SelectedItem = cultures.FirstOrDefault(info => info.Name == tp.ActualCulture.Name);
        }

        /// <summary>
        /// Called when Minimum ComboBox is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in Xaml.")]
        private void MinimumChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            tp.Minimum = e.NewValue;
        }

        /// <summary>
        /// Called when Maximum ComboBox is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in Xaml.")]
        private void MaximumChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            tp.Maximum = e.NewValue;
        }

        /// <summary>
        /// Called when Popup ComboBox is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in Xaml.")]
        private void PopupChanged(object sender, SelectionChangedEventArgs e)
        {
            tp.Popup = Activator.CreateInstance(((KeyValuePair<string, Type>)cmbPopup.SelectedItem).Value) as TimePickerPopup;
        }

        /// <summary>
        /// Called when PopupSecondsInterval ComboBox is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in Xaml.")]
        private void PopupSecondsIntervalChanged(object sender, SelectionChangedEventArgs e)
        {
            tp.PopupSecondsInterval = (int)e.AddedItems[0];
        }

        /// <summary>
        /// Called when PopupMinutesInterval ComboBox is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in Xaml.")]
        private void PopupMinutesIntervalChanged(object sender, SelectionChangedEventArgs e)
        {
            tp.PopupMinutesInterval = (int)e.AddedItems[0];
        }

        /// <summary>
        /// Called when the PopupSelectionMode ComboBox is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in Xaml.")]
        private void PopupSelectionModeChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                tp.PopupTimeSelectionMode = (PopupTimeSelectionMode)Enum.Parse(typeof(PopupTimeSelectionMode), e.AddedItems[0].ToString(), false);
            }
            catch (ArgumentOutOfRangeException)
            {
                Dispatcher.BeginInvoke(() =>
                                       cmbPopupSelectionMode.SelectedIndex =
                                       cmbPopupSelectionMode.Items.ToList().IndexOf(e.RemovedItems[0]));
            }
        }

        /// <summary>
        /// Called when Culture ComboBox has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Wish to catch all."), SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in Xaml.")]
        private void CultureChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCultures.SelectedItem != null)
            {
                tp.Culture = (CultureInfo)cmbCultures.SelectedItem;
            }
        }

        /// <summary>
        /// Called when Format ComboBox has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in Xaml.")]
        private void FormatChanged(object sender, SelectionChangedEventArgs e)
        {
            tp.Format = ((KeyValuePair<string, ITimeFormat>)cmbFormat.SelectedItem).Value;
        }

        /// <summary>
        /// Called when Timeparsers ComboBox is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in Xaml.")]
        private void TimeparserChanged(object sender, SelectionChangedEventArgs e)
        {
            tp.TimeParsers = new TimeParserCollection
                                 {
                                     ((KeyValuePair<string, TimeParser>)cmbTimeParser.SelectedItem).Value
                                 };
        }
    }
}
