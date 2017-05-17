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
    /// Sample page demonstrating ColumnSeries.
    /// </summary>
    [Sample("(1)Column", DifficultyLevel.Basic, "Column Series")]
    public partial class ColumnSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the ColumnSample class.
        /// </summary>
        public ColumnSample()
        {
            InitializeComponent();

            SampleGenerators.GenerateNumericSeriesSamples(GeneratedChartsPanel, () => new ColumnSeries(), false);
            SampleGenerators.GenerateValueSeriesSamples(GeneratedChartsPanel, () => new ColumnSeries());
            SampleGenerators.GenerateCategoryValueSeriesSamples(GeneratedChartsPanel, () => new ColumnSeries());
            SampleGenerators.GenerateMultipleValueSeriesSamples(GeneratedChartsPanel, () => new ColumnSeries(), false);
        }
    }
}