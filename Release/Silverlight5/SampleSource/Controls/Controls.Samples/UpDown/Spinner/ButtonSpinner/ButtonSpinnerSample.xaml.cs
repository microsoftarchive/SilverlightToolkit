// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Controls;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// ButtonSpinner sample page.
    /// </summary>
    [Sample("ButtonSpinner", DifficultyLevel.Basic, "ButtonSpinner")]
    public partial class ButtonSpinnerSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the ButtonSpinnerSample class.
        /// </summary>
        public ButtonSpinnerSample()
        {
            InitializeComponent();
            spinner1.Spin += (s, e) => { OnSpin(e); };
            spinner2.Spin += (s, e) => { OnSpin(e); };
        }

        /// <summary>
        /// Display the interactive ButtonSpinner control's properties.
        /// </summary>
        /// <param name="e">The SpinEventArgs.</param>
        private void OnSpin(SpinEventArgs e)
        {
            string s = string.Format(CultureInfo.InvariantCulture, "SpinEventArgs.Direction: \t{0}", e.Direction);
            display.Items.Add(s);
            display.SelectedIndex = display.Items.Count - 1;
            Dispatcher.BeginInvoke(() => display.ScrollIntoView(s));
        }
    }
}