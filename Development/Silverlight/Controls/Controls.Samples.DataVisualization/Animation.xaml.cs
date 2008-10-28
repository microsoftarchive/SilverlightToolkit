// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Windows.Controls.DataVisualization.Charting;
using System.Collections;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating Animations.
    /// </summary>
    [Sample("(6)Animation")]
    public partial class AnimationSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the AnimationSample class.
        /// </summary>
        public AnimationSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the Chart is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Information about the event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Attached to event handler in XAML.")]
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Chart chart = sender as Chart;

            if (null != chart)
            {
                // Toggle each Series ItemsSource so the reveal/hide animations can be seen
                foreach (DynamicSeries series in chart.Series)
                {
                    if (null == series.Tag)
                    {
                        series.Tag = series.ItemsSource;
                        series.ItemsSource = null;
                    }
                    else
                    {
                        series.ItemsSource = series.Tag as IEnumerable;
                        series.Tag = null;
                    }
                }
            }
        }
    }
}
