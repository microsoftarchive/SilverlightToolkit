//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Controls
{
    using System.Windows.Controls.Common;

    /// <summary>
    /// Date field for the DataForm.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormDateField : DataFormBoundField
    {
        /// <summary>
        /// Identifies the BlackoutDates dependency property.
        /// </summary>
        public static readonly DependencyProperty BlackoutDatesProperty =
            DependencyProperty.Register(
                "BlackoutDates",
                typeof(CalendarBlackoutDatesCollection),
                typeof(DataFormDateField),
                new PropertyMetadata(OnBlackoutDatesPropertyChanged));

        /// <summary>
        /// Identifies the DisplayDateEnd dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateEndProperty =
            DependencyProperty.Register(
                "DisplayDateEnd",
                typeof(DateTime?),
                typeof(DataFormDateField),
                new PropertyMetadata(OnDisplayDateEndPropertyChanged));

        /// <summary>
        /// Identifies the DisplayDateStart dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateStartProperty =
            DependencyProperty.Register(
                "DisplayDateStart",
                typeof(DateTime?),
                typeof(DataFormDateField),
                new PropertyMetadata(OnDisplayDateStartPropertyChanged));

        /// <summary>
        /// Identifies the SelectedDateFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateFormatProperty =
            DependencyProperty.Register(
                "SelectedDateFormat",
                typeof(DatePickerFormat),
                typeof(DataFormDateField),
                new PropertyMetadata(OnSelectedDateFormatPropertyChanged));

        /// <summary>
        /// Constructs a new DataFormDateField.
        /// </summary>
        public DataFormDateField()
        {
            this.BlackoutDates = new CalendarBlackoutDatesCollection(new Calendar());
        }

        /// <summary>
        /// Gets or sets the collection of blackout dates.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The property should be settable.")]
        public CalendarBlackoutDatesCollection BlackoutDates
        {
            get
            {
                return this.GetValue(BlackoutDatesProperty) as CalendarBlackoutDatesCollection;
            }

            set
            {
                this.SetValue(BlackoutDatesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the end display date.
        /// </summary>
        public DateTime? DisplayDateEnd
        {
            get
            {
                return this.GetValue(DisplayDateEndProperty) as DateTime?;
            }

            set
            {
                this.SetValue(DisplayDateEndProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the start display date.
        /// </summary>
        public DateTime? DisplayDateStart
        {
            get
            {
                return this.GetValue(DisplayDateStartProperty) as DateTime?;
            }

            set
            {
                this.SetValue(DisplayDateStartProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected date format.
        /// </summary>
        public DatePickerFormat SelectedDateFormat
        {
            get
            {
                return (DatePickerFormat)this.GetValue(SelectedDateFormatProperty);
            }

            set
            {
                this.SetValue(SelectedDateFormatProperty, value);
            }
        }

        /// <summary>
        /// Generates the display input control.
        /// </summary>
        /// <returns>The display input control.</returns>
        protected override FrameworkElement GenerateElement()
        {
            return this.GenerateDatePicker(true /* isReadOnly */);
        }

        /// <summary>
        /// Generates the edit input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The edit input control.</returns>
        protected override FrameworkElement GenerateEditingElement(bool isReadOnly)
        {
            return this.GenerateDatePicker(isReadOnly);
        }

        /// <summary>
        /// Generates the insert input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>THe insert input control.</returns>
        protected override FrameworkElement GenerateInsertElement(bool isReadOnly)
        {
            return this.GenerateDatePicker(isReadOnly);
        }

        /// <summary>
        /// BlackoutDates property changed handler.
        /// </summary>
        /// <param name="d">DataFormDateField that changed its BlackoutDates value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnBlackoutDatesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormDateField dateField = d as DataFormDateField;
            if (dateField != null && !dateField.AreHandlersSuspended())
            {
                DatePicker datePicker = dateField.Element as DatePicker;

                if (datePicker != null)
                {
                    foreach (CalendarDateRange dateRange in dateField.BlackoutDates)
                    {
                        datePicker.BlackoutDates.Add(dateRange);
                    }
                }
            }
        }

        /// <summary>
        /// DisplayDateEnd property changed handler.
        /// </summary>
        /// <param name="d">DataFormDateField that changed its BlackoutDates value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDisplayDateEndPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormDateField dateField = d as DataFormDateField;
            if (dateField != null && !dateField.AreHandlersSuspended())
            {
                DatePicker datePicker = dateField.Element as DatePicker;

                if (datePicker != null)
                {
                    datePicker.DisplayDateEnd = dateField.DisplayDateEnd;
                }
            }
        }

        /// <summary>
        /// DisplayDateStart property changed handler.
        /// </summary>
        /// <param name="d">DataFormDateField that changed its DisplayDateStart value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDisplayDateStartPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormDateField dateField = d as DataFormDateField;
            if (dateField != null && !dateField.AreHandlersSuspended())
            {
                DatePicker datePicker = dateField.Element as DatePicker;

                if (datePicker != null)
                {
                    datePicker.DisplayDateStart = dateField.DisplayDateStart;
                }
            }
        }

        /// <summary>
        /// SelectedDateFormat property changed handler.
        /// </summary>
        /// <param name="d">DataFormDateField that changed its SelectedDateFormat value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnSelectedDateFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormDateField dateField = d as DataFormDateField;
            if (dateField != null && !dateField.AreHandlersSuspended())
            {
                DatePicker datePicker = dateField.Element as DatePicker;

                if (datePicker != null)
                {
                    datePicker.SelectedDateFormat = dateField.SelectedDateFormat;
                }
            }
        }

        /// <summary>
        /// Generates a date picker.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the date picker should be read-only.</param>
        /// <returns>The date picker.</returns>
        private DatePicker GenerateDatePicker(bool isReadOnly)
        {
            DatePicker datePicker = new DatePicker() { IsEnabled = !isReadOnly, VerticalAlignment = VerticalAlignment.Center };

            if (this.Binding != null)
            {
                datePicker.SetBinding(DatePicker.SelectedDateProperty, this.Binding);
            }

            foreach (CalendarDateRange dateRange in this.BlackoutDates)
            {
                datePicker.BlackoutDates.Add(dateRange);
            }

            datePicker.DisplayDateEnd = this.DisplayDateEnd;
            datePicker.DisplayDateStart = this.DisplayDateStart;
            datePicker.SelectedDateFormat = this.SelectedDateFormat;
            datePicker.CalendarClosed += new RoutedEventHandler(this.OnDatePickerCalendarClosed);

            return datePicker;
        }

        /// <summary>
        /// Handles the case where the date picker's calendar was closed.
        /// </summary>
        /// <param name="sender">The date picker.</param>
        /// <param name="e">The event args.</param>
        private void OnDatePickerCalendarClosed(object sender, RoutedEventArgs e)
        {
            this.CommitEdit();
        }
    }
}
