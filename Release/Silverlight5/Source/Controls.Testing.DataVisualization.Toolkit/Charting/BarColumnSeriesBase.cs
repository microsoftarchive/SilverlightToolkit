// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Data;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization.Charting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the Bar/ColumnSeries classes.
    /// </summary>
    public abstract partial class BarColumnSeriesBase : DataPointSingleSeriesWithAxesBase
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
            chart.Series.Add(series);
            TestAsync(
                chart,
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
            IDataProvider series = (IDataProvider)DataPointSeriesWithAxesToTest;
            series.GetData(null);
        }

        /// <summary>
        /// Attempt to plot date time on dependent axis throws exception.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Attempt to plot date time on dependent axis throws exception.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public virtual void UseDateTimeValueAsDependentValue()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultSeriesToTest;
            DateTime[] dateTimes = new[] { DateTime.Now };
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => series.ItemsSource = dateTimes);
        }

        /// <summary>
        /// Verifies that using a DateTimeAxis for the independent axis works.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that using a DateTimeAxis for the independent axis works.")]
        [Bug("555659: Column Series Does not Support DateTime independent Axis", Fixed = true)]
        public void DateTimeAxisWorksAsIndependentAxis()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultSeriesToTest;
            series.DependentValueBinding = new Binding("Value");
            series.IndependentValueBinding = new Binding("Key");
            series.ItemsSource = new KeyValuePair<DateTime, int>[]
            {
                new KeyValuePair<DateTime, int>(new DateTime(2009, 1, 1), 1),
                new KeyValuePair<DateTime, int>(new DateTime(2009, 1, 2), 3),
                new KeyValuePair<DateTime, int>(new DateTime(2009, 1, 3), 4),
            };
            var axis = new DateTimeAxis();
            axis.Interval = 1;
            axis.IntervalType = DateTimeIntervalType.Days;
            if (series is BarSeries)
            {
                axis.Orientation = AxisOrientation.Y;
                ((BarSeries)series).IndependentAxis = axis;
            }
            else
            {
                axis.Orientation = AxisOrientation.X;
                ((ColumnSeries)series).IndependentAxis = axis;
            }
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(3, ChartTestUtilities.GetDataPointsForSeries(series).Count));
        }
    }
}