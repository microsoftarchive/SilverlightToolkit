// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Phone.Controls;
using System;

namespace PhoneToolkitSample.Samples
{
    public partial class PerformanceProgressBarSample : PhoneApplicationPage
    {
        public PerformanceProgressBarSample()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // This blocks the user interface thread for 4 seconds.
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(4));
        }

    }
}