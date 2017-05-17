// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace System.Windows.Controls.Testing
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
            dateTimeAxis.Orientation = AxisOrientation.X;
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
                () => chart.UpdateLayout());
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
                () => chart.UpdateLayout());
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
                () => chart.UpdateLayout());
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
                () => chart.UpdateLayout());
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
                () => chart.UpdateLayout());
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
                () => chart.UpdateLayout());
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
                () => chart.UpdateLayout());
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

        /// <summary>
        /// Verifies that DateTimeAxisLabel gives precedence to its StringFormat property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that DateTimeAxisLabel gives precedence to its StringFormat property.")]
        [Bug("555496: Charting: DateTimeAxisLabel doesn't use StringFormat", Fixed = true)]
        public void DateTimeAxisLabelHonorsStringFormat()
        {
            Chart chart = new Chart();
            string dateTimeFormat = "|{0:MM,dd}|";
            DateTimeAxis axis = new DateTimeAxis { Orientation = AxisOrientation.X };
            Style labelStyle = new Style(typeof(DateTimeAxisLabel));
            labelStyle.Setters.Add(new Setter(DateTimeAxisLabel.StringFormatProperty, dateTimeFormat));
            axis.AxisLabelStyle = labelStyle;
            chart.Axes.Add(axis);
            ScatterSeries series = new ScatterSeries();
            series.DependentValueBinding = new Binding("Day");
            series.IndependentValueBinding = new Binding();
            DateTime[] itemsSource = new DateTime[] { new DateTime(2009, 1, 23) };
            series.ItemsSource = itemsSource;
            chart.Series.Add(series);
            TestAsync(
                chart,
                () =>
                {
                    AxisLabel label = ChartTestUtilities.GetAxisLabels(axis).First();
                    Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, dateTimeFormat, label.DataContext), label.FormattedContent);
                });
        }

        /// <summary>
        /// Verifies that DateTimeAxis's ActualMinimum and ActualMaximum properties are correct.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that DateTimeAxis's ActualMinimum and ActualMaximum properties are correct.")]
        [Bug("604738: NumericAxis ActualMaximum, ActualMinimum always null", Fixed = true)]
        public void ActualMinimumActualMaximumCorrect()
        {
            Chart chart = new Chart();
            DateTimeAxis axis = new DateTimeAxis { Orientation = AxisOrientation.X, Minimum = new DateTime(2009, 1, 20), Maximum = new DateTime(2009, 1, 30) }; 
            chart.Axes.Add(axis);
            ScatterSeries series = new ScatterSeries();
            series.DependentValueBinding = new Binding("Day");
            series.IndependentValueBinding = new Binding();
            DateTime[] itemsSource = new DateTime[] { new DateTime(2009, 1, 23) };
            series.ItemsSource = itemsSource;
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(axis.Minimum, axis.ActualMinimum),
                () => Assert.AreEqual(axis.Maximum, axis.ActualMaximum));
        }
    }
}