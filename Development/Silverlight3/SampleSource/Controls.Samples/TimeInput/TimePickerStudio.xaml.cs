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
    [Sample("(3)TimePicker Studio", DifficultyLevel.Intermediate)]
    [Category("TimePicker")]
    public partial class TimePickerStudio : UserControl
    {
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
            tbCultures.Text = tp.ActualCulture.Name;
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
            tp.Popup = Activator.CreateInstance(((KeyValuePair<string, Type>) cmbPopup.SelectedItem).Value) as TimePickerPopup;
        }

        /// <summary>
        /// Called when PopupSecondsInterval ComboBox is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in Xaml.")]
        private void PopupSecondsIntervalChanged(object sender, SelectionChangedEventArgs e)
        {
            tp.PopupSecondsInterval = (int) e.AddedItems[0];
        }

        /// <summary>
        /// Called when PopupMinutesInterval ComboBox is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in Xaml.")]
        private void PopupMinutesIntervalChanged(object sender, SelectionChangedEventArgs e)
        {
            tp.PopupMinutesInterval = (int) e.AddedItems[0];
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
        private void CultureChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                CultureInfo newCulture = new CultureInfo(tbCultures.Text);
                tp.Culture = newCulture;
            }
            catch
            {
                tbCultures.Text = tp.ActualCulture.Name;
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
