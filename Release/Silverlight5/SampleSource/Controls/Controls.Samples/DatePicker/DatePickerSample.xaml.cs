// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the DatePicker.
    /// </summary>
    [Sample("DatePicker", DifficultyLevel.Basic, "DatePicker")]
    public partial class DatePickerSample : UserControl
    {
        /// <summary>
        /// Flag indicating whether to ignore the next DatePicker update.
        /// </summary>
        private bool _ignoreNextUpdate = true;

        /// <summary>
        /// Initializes a new instance of the DatePickerSample class.
        /// </summary>
        public DatePickerSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Event handler attached in XAML.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Simplifies sample.")]
        private void OnPastDatesChanged(object sender, RoutedEventArgs e)
        {
            if (sampleDatePicker != null && sampleLongDatePicker != null)
            {
                if (chkPastDateSelection.IsChecked == true)
                {
                    sampleDatePicker.BlackoutDates.Clear();
                    sampleLongDatePicker.BlackoutDates.Clear();
                }
                else
                {
                    try
                    {
                        sampleDatePicker.BlackoutDates.AddDatesInPast();
                        sampleLongDatePicker.BlackoutDates.AddDatesInPast();
                    }
                    catch
                    {
                        chkPastDateSelection.IsChecked = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handle changes to the selected date.
        /// </summary>
        /// <param name="sender">The DatePicker.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Event handler attached in XAML.")]
        private void OnDateSelected(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedDate(sampleDatePicker, sampleLongDatePicker);
        }

        /// <summary>
        /// Handle changes to the selected date.
        /// </summary>
        /// <param name="sender">The long DatePicker.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Event handler attached in XAML.")]
        private void OnLongDateSelected(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedDate(sampleLongDatePicker, sampleDatePicker);
        }

        /// <summary>
        /// Update the selected date.
        /// </summary>
        /// <param name="current">The current DatePicker.</param>
        /// <param name="other">The other DatePicker.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called via event hanlders attached in XAML.")]
        private void UpdateSelectedDate(DatePicker current, DatePicker other)
        {
            if (_ignoreNextUpdate)
            {
                _ignoreNextUpdate = false;
                other.SelectedDate = current.SelectedDate;

                if (current.SelectedDate != null)
                {
                    txtSelectedDate.Text = current.SelectedDate.ToString();
                }
                else
                {
                    txtSelectedDate.Text = "";
                }
            }
            else
            {
                _ignoreNextUpdate = true;
            }
        }
    }
}