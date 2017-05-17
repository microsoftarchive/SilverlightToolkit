// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating navigation using Pages and a Frame.
    /// </summary>
    [Sample("(0)Navigation", DifficultyLevel.Basic, "Navigation")]
    public partial class NavigationSample : UserControl
    {
        /// <summary>
        /// Initializes a NavigationSample.
        /// </summary>
        public NavigationSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Causes navigation when a button is clicked.
        /// </summary>
        /// <param name="sender">The clicked button.</param>
        /// <param name="e">Event arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by event defined in Xaml.")]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            navFrame.Navigate(new Uri(((Button)sender).Tag.ToString(), UriKind.Relative));
        }
    }
}
