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
    /// Sample page demonstrating AreaSeries.
    /// </summary>
    [Sample("Area Series", DifficultyLevel.Basic, "Area Series")]
    public partial class AreaSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the AreaSample class.
        /// </summary>
        public AreaSample()
        {
            InitializeComponent();

            SampleGenerators.GenerateNumericSeriesSamples(GeneratedChartsPanel, () => new AreaSeries(), true);
            SampleGenerators.GenerateDateTimeValueSeriesSamples(GeneratedChartsPanel, () => new AreaSeries());
            SampleGenerators.GenerateMultipleValueSeriesSamples(GeneratedChartsPanel, () => new AreaSeries(), true);
        }
    }
}