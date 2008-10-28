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
    /// Unit tests for the Axis class.
    /// </summary>
    [TestClass]
    public partial class AxisTest : ControlTest
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new Axis(); }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override IEnumerable<Control> ControlsToTest
        {
            get { yield return DefaultControlToTest; }
        }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { yield break; }
        }

        /// <summary>
        /// Initializes a new instance of the AxisTest class.
        /// </summary>
        public AxisTest()
        {
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            return TagInherited(base.GetDependencyPropertyTests());
        }

        /// <summary>
        /// Verifies the Control's TemplateParts.
        /// </summary>
        [TestMethod]
        [Description("Verifies the Control's TemplateParts.")]
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> templateParts = DefaultControlToTest.GetType().GetTemplateParts();
            Assert.AreEqual(4, templateParts.Count);
            Assert.AreSame(typeof(Canvas), templateParts["AxisContentCanvas"]);
            Assert.AreSame(typeof(Grid), templateParts["AxisLayoutGrid"]);
            Assert.AreSame(typeof(Title), templateParts["AxisTitle"]);
            Assert.AreSame(typeof(Canvas), templateParts["TitleCanvas"]);
        }

        /// <summary>
        /// Verify the Control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the Control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(4, properties.Count);
            Assert.AreEqual(typeof(Line), properties["TickMarkStyle"]);
            Assert.AreEqual(typeof(TextBlock), properties["LabelStyle"]);
            Assert.AreEqual(typeof(Title), properties["TitleStyle"]);
            Assert.AreEqual(typeof(Line), properties["GridLineStyle"]);
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        [TestMethod]
        [Description("Creates a new instance.")]
        public void NewInstance()
        {
            Axis axis = new Axis();
            Assert.IsNotNull(axis);
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Description("Verifies the initial values of all properties.")]
        public void InitialValues()
        {
            Axis axis = new Axis();
            Assert.AreEqual(AxisType.Auto, axis.AxisType);
            Assert.AreEqual(null, axis.Interval);
            Assert.AreEqual(null, axis.Maximum);
            Assert.AreEqual(null, axis.Minimum);
            Assert.AreEqual(AxisOrientation.Vertical, axis.Orientation);
            Assert.IsNull(axis.Title);
        }

        /// <summary>
        /// Assigns all values to the AxisType property.
        /// </summary>
        [TestMethod]
        [Description("Assigns all values to the AxisType property.")]
        public void AssignAllAxisTypeValues()
        {
            Axis axis = new Axis();
            foreach (AxisType axisType in new AxisType[] { AxisType.Category, AxisType.DateTime, AxisType.Linear })
            {
                axis.AxisType = axisType;
                Assert.AreEqual(axisType, axis.AxisType);
            }
        }

        /// <summary>
        /// Changes the value of the Interval property.
        /// </summary>
        [TestMethod]
        [Description("Changes the value of the Interval property.")]
        public void IntervalChangeValue()
        {
            Axis axis = new Axis();
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
            Axis axis = new Axis();
            IComparable minimum = 1.1;
            IComparable maximum = 2.2;
            axis.Minimum = minimum;
            axis.Maximum = maximum;
            Assert.AreEqual(minimum, axis.Minimum);
            Assert.AreEqual(maximum, axis.Maximum);
            minimum = 3.3;
            axis.Minimum = minimum;
            Assert.AreEqual(minimum, axis.Minimum);
            minimum = null;
            axis.Minimum = minimum;
            Assert.AreEqual(minimum, axis.Minimum);
            maximum = null;
            axis.Maximum = maximum;
            Assert.AreEqual(maximum, axis.Maximum);
        }

        /// <summary>
        /// Changes the value of the orientation property.
        /// </summary>
        [TestMethod]
        [Description("Changes the value of the AxisOrientation property.")]
        public void OrientationChangeValue()
        {
            Axis axis = new Axis();
            AxisOrientation orientation = AxisOrientation.Horizontal;
            axis.Orientation = orientation;
            Assert.AreEqual(orientation, axis.Orientation);
        }

        /// <summary>
        /// Assigns a string to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a string to the Title property.")]
        public void TitleChangeString()
        {
            Axis axis = new Axis();
            string title = "String Title";
            axis.Title = title;
            Assert.AreEqual(title, axis.Title);
        }

        /// <summary>
        /// Adds a new point to a Category Axis, causing the Axis to update.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Adds a new point to a Category Axis, causing the Axis to update.")]
        [Bug("527206, Axis exception when adding new category", Fixed = true)]
        public void AddNewPointToCategoryAxis()
        {
            Chart chart = new Chart();
            DynamicSeries series = new ColumnSeries();
            series.IndependentValueBinding = new Binding();
            ObservableCollection<int> itemsSource = new ObservableCollection<int>();
            itemsSource.Add(5);
            series.ItemsSource = itemsSource;
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => itemsSource.Add(10),
                () => chart.Refresh());
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
        /// Create a ISeriesHost with a DateTime Axis and a TimeSpan so it can pick the appropriate interval.
        /// </summary>
        /// <param name="timeSpan">Time span for the data.</param>
        /// <returns>ISeriesHost for testing.</returns>
        private static Chart CreateDateTimeAxisWithIntervalChart(TimeSpan timeSpan)
        {
            Chart chart = new Chart();
            Axis dateTimeAxis = new Axis();
            dateTimeAxis.Orientation = AxisOrientation.Horizontal;
            dateTimeAxis.AxisType = AxisType.DateTime;
            chart.Axes.Add(dateTimeAxis);
            DynamicSeries series = new LineSeries();
            series.DependentValueBinding = new Binding("Day");
            series.IndependentValueBinding = new Binding();
            DateTime start = new DateTime(2008, 1, 1);
            series.ItemsSource = new DateTime[] { start, start + timeSpan };
            chart.Series.Add(series);
            return chart;
        }
    }
}