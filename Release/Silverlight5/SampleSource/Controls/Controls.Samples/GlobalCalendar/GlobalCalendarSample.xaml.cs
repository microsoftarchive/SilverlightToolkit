// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the GlobalCalendar.
    /// </summary>
    [Sample("(1)GlobalCalendar", DifficultyLevel.Basic, "GlobalCalendar")]
    public partial class GlobalCalendarSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the GlobalCalendarSample class.
        /// </summary>
        public GlobalCalendarSample()
        {
            InitializeComponent();
            CultureOptions.SelectedIndex = 0;
        }

        /// <summary>
        /// Update the culture when the drop down changes.
        /// </summary>
        /// <param name="sender">The culture ComboBox.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Attached in XAML.")]
        private void OnCultureChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selected = CultureOptions.SelectedItem as ComboBoxItem;
            if (selected == null)
            {
                return;
            }

            CultureInfo culture = new CultureInfo(selected.Tag as string);
            CulturedCalendar.CalendarInfo = new CultureCalendarInfo(culture);
        }
    }
}