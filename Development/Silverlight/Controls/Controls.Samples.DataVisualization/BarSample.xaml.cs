// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;
using Microsoft.Windows.Controls.DataVisualization.Charting;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating BarSeries.
    /// </summary>
    [Sample("(2)Bar")]
    public partial class BarSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the BarSample class.
        /// </summary>
        public BarSample()
        {
            InitializeComponent();

            SampleGenerators.GenerateNumericSeriesSamples(GeneratedChartsPanel, () => new BarSeries(), false);
            SampleGenerators.GenerateValueSeriesSamples(GeneratedChartsPanel, () => new BarSeries());
            SampleGenerators.GenerateCategoryValueSeriesSamples(GeneratedChartsPanel, () => new BarSeries());
            SampleGenerators.GenerateMultipleValueSeriesSamples(GeneratedChartsPanel, () => new BarSeries(), false);
        }
    }
}