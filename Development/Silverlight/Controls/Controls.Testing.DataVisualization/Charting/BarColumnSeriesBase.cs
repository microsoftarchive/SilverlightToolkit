// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Data;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization.Charting;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the Bar/ColumnSeries classes.
    /// </summary>
    public abstract partial class BarColumnSeriesBase : DataPointSeriesWithAxesBase
    {
        /// <summary>
        /// Initializes a new instance of the BarColumnSeriesBase class.
        /// </summary>
        protected BarColumnSeriesBase()
        {
        }

        /// <summary>
        /// Creates a ISeriesHost with a Bar/ColumnSeries containing no values.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Bar/ColumnSeries containing no values.")]
        public void SimpleChartNoValues()
        {
            DataPointSeries series = DefaultSeriesToTest;
            series.ItemsSource = new int[0];
            Chart chart = new Chart();
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(0, ChartTestUtilities.GetDataPointsForSeries(series).Count));
        }

        /// <summary>
        /// Creates a ISeriesHost with a Bar/ColumnSeries containing one int with value 0.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Bar/ColumnSeries containing one int with value 0.")]
        public void SimpleChartOneIntegerValueZero()
        {
            DataPointSeries series = DefaultSeriesToTest;
            series.ItemsSource = new int[] { 0 };
            Chart chart = new Chart();
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(1, ChartTestUtilities.GetDataPointsForSeries(series).Count));
        }

        /// <summary>
        /// Creates a ISeriesHost with a Bar/ColumnSeries containing two ints.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Bar/ColumnSeries containing two ints.")]
        public void SimpleChartTwoIntegers()
        {
            DataPointSeries series = DefaultSeriesToTest;
            series.ItemsSource = new List<int>(new int[] { 1, -1 });
            Chart chart = new Chart();
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(2, ChartTestUtilities.GetDataPointsForSeries(series).Count));
        }

        /// <summary>
        /// Creates a ISeriesHost with a Series using value bindings.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Series using value bindings.")]
        public void SimpleChartWithBindings()
        {
            DataPointSeries series = DefaultSeriesToTest;
            series.ItemsSource = new[] { new KeyValuePair<string, double>("A", 2) };
            series.IndependentValueBinding = new Binding("Key");
            series.DependentValueBinding = new Binding("Value");
            Chart chart = new Chart();
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => Assert.AreEqual(1, chart.Series.Count),
                () => Assert.AreEqual(1, ChartTestUtilities.GetDataPointsForSeries(series).Count));
        }

        /// <summary>
        /// Calls GetCategories with a null axis parameter.
        /// </summary>
        [TestMethod]
        [Description("Calls GetCategories with a null axis parameter.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetCategoriesWithNullAxis()
        {
            ICategoryAxisInformationProvider series = (ICategoryAxisInformationProvider)DataPointSeriesWithAxesToTest;
            series.GetCategories(null);
        }
    }
}