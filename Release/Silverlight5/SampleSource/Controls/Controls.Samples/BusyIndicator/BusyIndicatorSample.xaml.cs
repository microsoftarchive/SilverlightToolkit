// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the BusyIndicator.
    /// </summary>
    [Sample("BusyIndicator", DifficultyLevel.Basic, "BusyIndicator")]
    public partial class BusyIndicatorSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the BusyIndicatorSample class.
        /// </summary>
        public BusyIndicatorSample()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(BusyIndicatorSample_Loaded);
            DataContext = false;
        }

        /// <summary>
        /// Handles the UserControl's Loaded event.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void BusyIndicatorSample_Loaded(object sender, RoutedEventArgs e)
        {
            // Sample browser-specific layout change
            SampleHelpers.ChangeSampleAlignmentToStretch(this);
        }

        /// <summary>
        /// Handles clicks on the "Get Busy!" button.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Hooked up in XAML.")]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int busySeconds = (int)BusySeconds.SelectedItem;
            int delayMilliseconds = (int)DelayMilliseconds.SelectedItem;
            SampleIndicator.DisplayAfter = TimeSpan.FromMilliseconds(delayMilliseconds);

            // Simulate a long-running task by sleeping on a worker thread
            DataContext = true;
            ThreadPool.QueueUserWorkItem((state) =>
            {
                Thread.Sleep(busySeconds * 1000);
                Dispatcher.BeginInvoke(() => DataContext = false);
            });
        }
    }
}