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
    /// Sample page demonstrating the Calendar.
    /// </summary>
    [Sample("Calendar", DifficultyLevel.Basic, "Calendar")]
    public partial class CalendarSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the CalendarSample class.
        /// </summary>
        public CalendarSample()
        {
            InitializeComponent();

            // Setting the DatePickers
            txtDisplayDate.Text = sampleCalendar.DisplayDate.ToShortDateString();
            txtSelectedDate.Text = sampleCalendar.SelectedDate.ToString();
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
            if (sampleCalendar == null)
            {
                return;
            }

            if ((bool)chkPastDateSelection.IsChecked)
            {
                sampleCalendar.BlackoutDates.Clear();
            }
            else
            {
                try
                {
                    sampleCalendar.BlackoutDates.AddDatesInPast();
                }
                catch
                {
                    chkPastDateSelection.IsChecked = true;
                }                
            }
        }
        
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Event handler attached in XAML.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Simplifies sample.")]
        private void OnDisplayDateStartSelected(object sender, SelectionChangedEventArgs e)
        {
            if (sampleCalendar == null)
            {
                return;
            }

            try
            {
                sampleCalendar.DisplayDateStart = e.AddedItems[0] as DateTime?;
            }
            catch
            {
                sampleCalendar.DisplayDateStart = null;
                dateStart.Text = "";
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Event handler attached in XAML.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Simplifies sample.")]
        private void OnDisplayDateEndSelected(object sender, SelectionChangedEventArgs e)
        {
            if (sampleCalendar == null)
            {
                return;
            }

            try
            {
                sampleCalendar.DisplayDateEnd = e.AddedItems[0] as DateTime?;
            }
            catch
            {
                sampleCalendar.DisplayDateEnd = null;
                dateEnd.Text = "";
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Event handler attached in XAML.")]
        private void OnDisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            if (sampleCalendar != null)
            {
                txtDisplayDate.Text = sampleCalendar.DisplayDate.ToShortDateString();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Event handler attached in XAML.")]
        private void OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sampleCalendar != null && sampleCalendar.SelectedDate != null)
            {
                txtSelectedDate.Text = sampleCalendar.SelectedDate.ToString();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment 1.</param>
        /// <param name="e">Inherited code: Requires comment 2.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Event handler attached in XAML.")]
        private void OnSelectionModeChanged(object sender, RoutedEventArgs e)
        {
            if (sampleCalendar != null)
            {
                sampleCalendar.SelectionMode =
                    (sender == radioNone) ? CalendarSelectionMode.None :
                    (sender == radioSingleRange) ? CalendarSelectionMode.SingleRange :
                    (sender == radioMultiRange) ? CalendarSelectionMode.MultipleRange :
                    CalendarSelectionMode.SingleDate;
            }
        }
    }
}