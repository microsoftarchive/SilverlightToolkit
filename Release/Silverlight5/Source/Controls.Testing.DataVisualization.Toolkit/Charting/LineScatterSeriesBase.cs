// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Data;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization.Charting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the Line/ScatterSeries classes.
    /// </summary>
    public abstract partial class LineScatterSeriesBase : DataPointSingleSeriesWithAxesBase
    {
        /// <summary>
        /// Initializes a new instance of the LineScatterSeriesBase class.
        /// </summary>
        protected LineScatterSeriesBase()
        {
        }

        /// <summary>
        /// Creates a ISeriesHost with a Line/ScatterSeries using value bindings.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Line/ScatterSeries using value bindings.")]
        public void SimpleChartWithBindings()
        {
            DataPointSeries series = DefaultSeriesToTest;
            series.ItemsSource =
                new[]
                {
                    new KeyValuePair<double, double>(3, 2),
                    new KeyValuePair<double, double>(8, 9)
                };
            series.IndependentValueBinding = new Binding("Key");
            series.DependentValueBinding = new Binding("Value");
            Chart chart = new Chart();
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(1, chart.Series.Count),
                () => Assert.AreEqual(2, ChartTestUtilities.GetDataPointsForSeries(series).Count));
        }

        /// <summary>
        /// Creates a ISeriesHost with a Line/ScatterSeries containing two values.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Line/ScatterSeries containing two values.")]
        public void ScatterChartTwoIntegers()
        {
            DataPointSeries series = DefaultSeriesToTest;
            series.ItemsSource = new int[] { 1, 2 };
            series.IndependentValueBinding = new Binding();
            Chart chart = new Chart();
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(2, ChartTestUtilities.GetDataPointsForSeries(series).Count));
        }

        /// <summary>
        /// Creates a ISeriesHost with a Line/ScatterSeries containing no values.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Line/ScatterSeries containing no values.")]
        public void LineChartNoValues()
        {
            DataPointSeries series = DefaultSeriesToTest;
            series.ItemsSource = new int[0];
            series.IndependentValueBinding = new Binding();
            Chart chart = new Chart();
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(0, ChartTestUtilities.GetDataPointsForSeries(series).Count));
        }
    }
}
