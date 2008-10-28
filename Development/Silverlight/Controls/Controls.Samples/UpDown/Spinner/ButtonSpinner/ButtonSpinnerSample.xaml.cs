// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// ButtonSpinner sample page.
    /// </summary>
    [Sample("UpDown/Spinner/ButtonSpinner")]
    public partial class ButtonSpinnerSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the ButtonSpinnerSample class.
        /// </summary>
        public ButtonSpinnerSample()
        {
            InitializeComponent();
            spinner.Spin += (s, e) => { OnSpin(e); };
        }

        /// <summary>
        /// Display the interactive ButtonSpinner control's properties.
        /// </summary>
        /// <param name="e">The SpinEventArgs.</param>
        private void OnSpin(SpinEventArgs e)
        {
            string formatString = "\n\n" +
                " SpinEventArgs.Direction: \t{0}\n " +
                " ButtonSpinner.Content: :\t{1}\n ";
            output.Text = string.Format(
                CultureInfo.InvariantCulture, 
                formatString, 
                e.Direction,
                spinner.Content);
        }
    }
}