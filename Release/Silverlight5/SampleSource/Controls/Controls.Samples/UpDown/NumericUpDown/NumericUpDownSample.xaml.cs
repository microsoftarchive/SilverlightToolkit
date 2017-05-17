// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// NumericUpDown sample page.
    /// </summary>
    [Sample("NumericUpDown", DifficultyLevel.Basic, "NumericUpDown")]
    public partial class NumericUpDownSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the NumericUpDownSample class.
        /// </summary>
        public NumericUpDownSample()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            nud.ValueChanged += (s, e) => { OutputNUD(); };
        }

        /// <summary>
        /// Load the demo page.
        /// </summary>
        /// <param name="sender">Sample page.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            btnChange.Click += ChangeSettings;
        }

        /// <summary>
        /// Change ContentControl settings (Header, HeaderTemplate, Content, ContentTemplate).
        /// </summary>
        /// <param name="sender">Sender Button.</param>
        /// <param name="e">Event args.</param>
        private void ChangeSettings(object sender, RoutedEventArgs e)
        {
            nud.Minimum = ParseUtility.ReadValue(tbMininum, nud.Minimum);
            nud.Maximum = ParseUtility.ReadValue(tbMaximum, nud.Maximum);
            nud.Value = ParseUtility.ReadValue(tbValue, nud.Value);
            nud.Increment = ParseUtility.ReadValue(tbIncrement, nud.Increment);
            nud.DecimalPlaces = ParseUtility.ReadValue(tbDecimalPlaces, nud.DecimalPlaces);
            nud.IsEditable = cbIsEditable.IsChecked ?? false;
            nud.IsEnabled = cbIsEnabled.IsChecked ?? true;

            OutputNUD();
        }

        /// <summary>
        /// Display the interactive NumericUpDown control's properties.
        /// </summary>
        private void OutputNUD()
        {
            string formatString = "\n\n" +
                " Minimum:\t{0}\n Maximum:\t{1}\n Value:\t{2}\n" + 
                " Increment:\t{3}\n DecimalPlaces:\t{4}\n" +
                " IsEditable:\t{5}\n IsEnabled:\t{6}\n";
            output.Text = string.Format(
                CultureInfo.InvariantCulture, 
                formatString, 
                nud.Minimum, 
                nud.Maximum, 
                nud.Value, 
                nud.Increment, 
                nud.DecimalPlaces, 
                nud.IsEditable, 
                nud.IsEnabled);
        }
    }
}
