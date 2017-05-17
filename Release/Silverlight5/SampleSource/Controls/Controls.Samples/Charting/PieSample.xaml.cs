// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the Pie Chart.
    /// </summary>
    [Sample("(1)Pie", DifficultyLevel.Basic, "Pie Series")]
    public partial class PieSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the PieSample class.
        /// </summary>
        public PieSample()
        {
            InitializeComponent();

            SampleGenerators.GenerateNumericSeriesSamples(GeneratedChartsPanel, () => new PieSeries(), false);
            SampleGenerators.GenerateValueSeriesSamples(GeneratedChartsPanel, () => new PieSeries());
            SampleGenerators.GenerateCategoryValueSeriesSamples(GeneratedChartsPanel, () => new PieSeries());
            SampleGenerators.GenerateMultipleValueSeriesSamples(GeneratedChartsPanel, () => new PieSeries(), false);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the "Drill-Down" sample Chart.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Attached to event handler in XAML.")]
        private void DrillDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InformationPanel.DataContext = (1 <= e.AddedItems.Count) ? e.AddedItems[0] : null;
        }

        /// <summary>
        /// Handles the Click event of the "Unselect" button for the "Drill-Down" sample Chart.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Attached to event handler in XAML.")]
        private void Unselect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (DrillDownChart.Series[0] as PieSeries).SelectedItem = null;
        }
    }
}