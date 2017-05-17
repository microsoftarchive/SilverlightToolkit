// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating navigation with query strings.
    /// </summary>
    [Sample("(1)Using a Query String", DifficultyLevel.Intermediate, "Navigation")]
    public partial class QueryNavigationSample : UserControl
    {
        /// <summary>
        /// Initializes a QueryNavigationSample.
        /// </summary>
        public QueryNavigationSample()
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
            navFrame.Navigate(new Uri(string.Format(CultureInfo.CurrentCulture, "/Navigation/QueryPage.xaml?x={0}&y={1}&z={2}", Uri.EscapeDataString(x.Text), Uri.EscapeDataString(y.Text), Uri.EscapeDataString(z.Text)), UriKind.Relative));
        }
    }
}
