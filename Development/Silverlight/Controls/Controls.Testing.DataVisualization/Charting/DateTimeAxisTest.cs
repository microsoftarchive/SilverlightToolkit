// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization;
using Microsoft.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// An axis that can be used to plot DateTime values.
    /// </summary>
    [TestClass]
    public class DateTimeAxisTest : RangeAxisBase
    {
        /// <summary>
        /// Create a ISeriesHost with a DateTime Axis and a TimeSpan so it can pick the appropriate interval.
        /// </summary>
        /// <param name="timeSpan">Time span for the data.</param>
        /// <returns>ISeriesHost for testing.</returns>
        private static Chart CreateDateTimeAxisWithIntervalChart(TimeSpan timeSpan)
        {
            Chart chart = new Chart();
            DateTimeAxis dateTimeAxis = new DateTimeAxis();
            dateTimeAxis.Orientation = AxisOrientation.Horizontal;
            chart.Axes.Add(dateTimeAxis);
            DataPointSeries series = new LineSeries();
            series.DependentValueBinding = new Binding("Day");
            series.IndependentValueBinding = new Binding();
            DateTime start = new DateTime(2008, 1, 1);
            series.ItemsSource = new DateTime[] { start, start + timeSpan };
            chart.Series.Add(series);
            return chart;
        }

        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new DateTimeAxis(); }
        }

        /// <summary>
        /// Changes the value of the Interval property.
        /// </summary>
        [TestMethod]
        [Description("Changes the value of the Interval property.")]
        public void IntervalChangeValue()
        {
            DateTimeAxis axis = (DateTimeAxis)DefaultControlToTest;
            double interval = 1.1;
            axis.Interval = interval;
            Assert.AreEqual(interval, axis.Interval);
        }

        /// <summary>
        /// Changes the value of the Minimum and Maximum properties.
        /// </summary>
        [TestMethod]
        [Description("Changes the value of the Minimum and Maximum properties.")]
        public void MinimumMaximumChangeValues()
        {
            DateTimeAxis axis = (DateTimeAxis)DefaultControlToTest;
            DateTime? minimum = new DateTime(1999, 11, 1);
            DateTime? maximum = new DateTime(2000, 11, 1);
            axis.Minimum = minimum;
            axis.Maximum = maximum;
            Assert.AreEqual(minimum, axis.Minimum);
            Assert.AreEqual(maximum, axis.Maximum);
        }

        /// <summary>
        /// Tests that setting the minimum larger than the maximum throws.
        /// </summary>
        [TestMethod]
        [Description("Tests that setting the minimum larger than the maximum throws.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MinimumValueLargerThanMaximumThrowsException()
        {
            DateTimeAxis axis = (DateTimeAxis)DefaultControlToTest;
            DateTime? minimum = new DateTime(1999, 11, 1);
            DateTime? maximum = new DateTime(2000, 11, 1);
            axis.Minimum = minimum;
            axis.Maximum = maximum;
            minimum = new DateTime(2001, 11, 1);
            axis.Minimum = minimum;
        }

        /// <summary>
        /// Tests that setting the maximum smaller than the minimum throws.
        /// </summary>
        [TestMethod]
        [Description("Tests that setting the maximum smaller than the minimum throws.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MaximumValueSmallerThanMinimumThrowsException()
        {
            DateTimeAxis axis = (DateTimeAxis)DefaultControlToTest;
            DateTime? minimum = new DateTime(1999, 11, 1);
            DateTime? maximum = new DateTime(2000, 11, 1);
            axis.Minimum = minimum;
            axis.Maximum = maximum;
            axis.Maximum = new DateTime(1999, 1, 1);
        }

        /// <summary>
        /// Creates a ISeriesHost with a DateTime Axis and Year intervals.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DateTime Axis and Year intervals.")]
        public void DateTimeAxisWithYearIntervals()
        {
            Chart chart = CreateDateTimeAxisWithIntervalChart(TimeSpan.FromDays(10 * 365));
            TestAsync(
                chart,
                () => chart.Refresh());
        }

        /// <summary>
        /// Creates a ISeriesHost with a DateTime Axis and Month intervals.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DateTime Axis and Month intervals.")]
        public void DateTimeAxisWithMonthIntervals()
        {
            Chart chart = CreateDateTimeAxisWithIntervalChart(TimeSpan.FromDays(10 * 31));
            TestAsync(
                chart,
                () => chart.Refresh());
        }

        /// <summary>
        /// Creates a ISeriesHost with a DateTime Axis and Week intervals.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DateTime Axis and Week intervals.")]
        public void DateTimeAxisWithWeekIntervals()
        {
            Chart chart = CreateDateTimeAxisWithIntervalChart(TimeSpan.FromDays(10 * 7));
            TestAsync(
                chart,
                () => chart.Refresh());
        }

        /// <summary>
        /// Creates a ISeriesHost with a DateTime Axis and Day intervals.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DateTime Axis and Day intervals.")]
        public void DateTimeAxisWithDayIntervals()
        {
            Chart chart = CreateDateTimeAxisWithIntervalChart(TimeSpan.FromDays(10));
            TestAsync(
                chart,
                () => chart.Refresh());
        }

        /// <summary>
        /// Creates a ISeriesHost with a DateTime Axis and Hour intervals.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DateTime Axis and Hour intervals.")]
        public void DateTimeAxisWithHourIntervals()
        {
            Chart chart = CreateDateTimeAxisWithIntervalChart(TimeSpan.FromHours(10));
            TestAsync(
                chart);
        }

        /// <summary>
        /// Creates a ISeriesHost with a DateTime Axis and Minute intervals.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DateTime Axis and Minute intervals.")]
        public void DateTimeAxisWithMinuteIntervals()
        {
            Chart chart = CreateDateTimeAxisWithIntervalChart(TimeSpan.FromMinutes(10));
            TestAsync(
                chart,
                () => chart.Refresh());
        }

        /// <summary>
        /// Creates a ISeriesHost with a DateTime Axis and Second intervals.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DateTime Axis and Second intervals.")]
        public void DateTimeAxisWithSecondIntervals()
        {
            Chart chart = CreateDateTimeAxisWithIntervalChart(TimeSpan.FromSeconds(10));
            TestAsync(
                chart,
                () => chart.Refresh());
        }

        /// <summary>
        /// Creates a ISeriesHost with a DateTime Axis and Millisecond intervals.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DateTime Axis and Millisecond intervals.")]
        public void DateTimeAxisWithMillisecondIntervals()
        {
            Chart chart = CreateDateTimeAxisWithIntervalChart(TimeSpan.FromMilliseconds(10));
            TestAsync(
                chart,
                () => chart.Refresh());
        }

        /// <summary>
        /// Verify the Control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the Control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(5, properties.Count);
            Assert.AreEqual(typeof(Line), properties["MajorTickMarkStyle"]);
            Assert.AreEqual(typeof(Line), properties["MinorTickMarkStyle"]);
            Assert.AreEqual(typeof(DateTimeAxisLabel), properties["AxisLabelStyle"]);
            Assert.AreEqual(typeof(Title), properties["TitleStyle"]);
            Assert.AreEqual(typeof(Line), properties["GridLineStyle"]);
        }
    }
}