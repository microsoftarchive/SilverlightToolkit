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
    /// Sample page demonstrating ScatterSeries.
    /// </summary>
    [Sample("Bubble", DifficultyLevel.Basic, "Bubble Series")]
    public partial class BubbleSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the ScatterSample class.
        /// </summary>
        public BubbleSample()
        {
            InitializeComponent();

            SampleGenerators.GenerateNumericSeriesSamples(GeneratedChartsPanel, () => new BubbleSeries(), true);
            SampleGenerators.GenerateDateTimeValueSeriesSamples(GeneratedChartsPanel, () => new BubbleSeries());
            SampleGenerators.GenerateMultipleValueSeriesSamples(GeneratedChartsPanel, () => new BubbleSeries(), true);
        }
    }
}