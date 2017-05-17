// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// This class tests the LinearAxisTest class.
    /// </summary>
    [TestClass]
    public class LinearAxisTest : NumericAxisBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new LinearAxis(); }
        }

        /// <summary>
        /// Changes the value of the Interval property.
        /// </summary>
        [TestMethod]
        [Description("Changes the value of the Interval property.")]
        public void IntervalChangeValue()
        {
            LinearAxis axis = (LinearAxis)DefaultControlToTest;
            double interval = 1.1;
            axis.Interval = interval;
            Assert.AreEqual(interval, axis.Interval);
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Description("Verifies the initial values of all properties.")]
        public override void InitialValues()
        {
            LinearAxis axis = (LinearAxis)DefaultControlToTest;
            Assert.AreEqual(null, axis.Interval);
            base.InitialValues();
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
            Assert.AreEqual(typeof(Line), properties["MinorTickMarkStyle"]);
            Assert.AreEqual(typeof(Line), properties["MajorTickMarkStyle"]);
            Assert.AreEqual(typeof(NumericAxisLabel), properties["AxisLabelStyle"]);
            Assert.AreEqual(typeof(Title), properties["TitleStyle"]);
            Assert.AreEqual(typeof(Line), properties["GridLineStyle"]);
        }

        /// <summary>
        /// Verifies that the automatic Minimum/Maximum values are reasonable.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the automatic Minimum/Maximum values are reasonable.")]
        [Bug("524275: Automatic Axis minimum/maximum values could be much improved", Fixed = true)]
        public void AutomaticMinimumAndMaximum()
        {
            Chart chart = new Chart();
            ScatterSeries series = new ScatterSeries();
            series.IndependentValueBinding = new Binding();
            series.ItemsSource = new int[] { 100 };
            chart.Series.Add(series);
            TestAsync(
                chart,
                () =>
                {
                    foreach (LinearAxis axis in chart.ActualAxes)
                    {
                        Assert.IsTrue((99 == axis.ActualMinimum) && (axis.ActualMaximum == 101));
                    }
                });
        }

        /// <summary>
        /// Verifies that user settings for Minimum/Maximum values are respected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that user settings for Minimum/Maximum values are respected.")]
        [Bug("523459: Axes don't respect user's settings for Minimum", Fixed = true)]
        public void CustomMinimumAndMaximumAreRespected()
        {
            Chart chart = new Chart();
            LinearAxis horizontalAxis = new LinearAxis { Orientation = AxisOrientation.X, Minimum = 50, Maximum = 120 };
            chart.Axes.Add(horizontalAxis);
            LinearAxis verticalAxis = new LinearAxis { Orientation = AxisOrientation.Y, Minimum = 40, Maximum = 130 };
            chart.Axes.Add(verticalAxis);
            ScatterSeries series = new ScatterSeries();
            series.IndependentValueBinding = new Binding();
            series.ItemsSource = new int[] { 100 };
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(50, horizontalAxis.ActualMinimum),
                () => Assert.AreEqual(120, horizontalAxis.ActualMaximum),
                () => Assert.AreEqual(40, verticalAxis.ActualMinimum),
                () => Assert.AreEqual(130, verticalAxis.ActualMaximum));
        }

        /// <summary>
        /// Verifies that user settings for Minimum/Maximum values are respected when set via a Style.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that user settings for Minimum/Maximum values are respected when set via a Style.")]
        [Bug("542642: Charting - Cannot set Axis.Maximum/Minimum via Style", Fixed = true)]
        [Priority(0)]
        public void CustomMinimumAndMaximumAreRespectedViaStyle()
        {
            Chart chart = new Chart();
            LinearAxis horizontalAxis = new LinearAxis { Orientation = AxisOrientation.X };
            Style horizontalStyle = new Style(typeof(LinearAxis));
            horizontalStyle.Setters.Add(new Setter(LinearAxis.MinimumProperty, 50.0));
            horizontalStyle.Setters.Add(new Setter(LinearAxis.MaximumProperty, 120.0));
            horizontalAxis.Style = horizontalStyle;
            chart.Axes.Add(horizontalAxis);
            LinearAxis verticalAxis = new LinearAxis { Orientation = AxisOrientation.Y };
            Style verticalStyle = new Style(typeof(LinearAxis));
            verticalStyle.Setters.Add(new Setter(LinearAxis.MinimumProperty, 40.0));
            verticalStyle.Setters.Add(new Setter(LinearAxis.MaximumProperty, 130.0));
            verticalAxis.Style = verticalStyle;
            chart.Axes.Add(verticalAxis);
            ScatterSeries series = new ScatterSeries();
            series.IndependentValueBinding = new Binding();
            series.ItemsSource = new int[] { 100 };
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(50, horizontalAxis.ActualMinimum),
                () => Assert.AreEqual(120, horizontalAxis.ActualMaximum),
                () => Assert.AreEqual(40, verticalAxis.ActualMinimum),
                () => Assert.AreEqual(130, verticalAxis.ActualMaximum));
        }

        /// <summary>
        /// Verifies the ability to set the Maximum and Minimum of a dependent axis.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies the ability to set the Maximum and Minimum of a dependent axis.")]
        [Bug("559809: Crash in RangeAxis.ProtectedMinimum after setting Maximum and Minumum", Fixed = true)]
        public void AbleToSetMinimumMaximumOnDependentAxis()
        {
            Chart chart = new Chart();
            ColumnSeries series = new ColumnSeries();
            series.ItemsSource = new int[] { 1, 2, 3 };
            LinearAxis dependentAxis = new LinearAxis { Orientation = AxisOrientation.Y };
            series.DependentRangeAxis = dependentAxis;
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(3, ChartTestUtilities.GetDataPointsForSeries(series).Count()),
                () => dependentAxis.Maximum = 5,
                () => Assert.AreEqual(3, ChartTestUtilities.GetDataPointsForSeries(series).Count()),
                () => dependentAxis.Minimum = 0.5,
                () => Assert.AreEqual(3, ChartTestUtilities.GetDataPointsForSeries(series).Count()));
        }

        /// <summary>
        /// Verifies that dynamic changes to the Title property are immediately applied.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that dynamic changes to the Title property are immediately applied.")]
        [Bug("539523: Dynamic changes to vertical axis Title do not get centered properly", Fixed = true)]
        public void TitleCentersProperlyAfterUpdate()
        {
            Chart chart = new Chart();
            LinearAxis horizontalAxis = new LinearAxis { Orientation = AxisOrientation.X, Title = "Filler Text" };
            chart.Axes.Add(horizontalAxis);
            LinearAxis verticalAxis = new LinearAxis { Orientation = AxisOrientation.Y, Title = "Sample Title" };
            chart.Axes.Add(verticalAxis);
            Title horizontalTitle = null;
            Title verticalTitle = null;
            TestAsync(
                chart,
                () =>
                {
                    horizontalTitle = horizontalAxis.GetVisualDescendents().OfType<Title>().First();
                    verticalTitle = verticalAxis.GetVisualDescendents().OfType<Title>().First();
                },
                () =>
                {
                    Point xLeftTop = horizontalTitle.TransformToVisual(horizontalAxis).Transform(new Point());
                    Assert.AreEqual(Math.Round(xLeftTop.X), Math.Round((horizontalAxis.ActualWidth - horizontalTitle.ActualWidth) / 2));
                    Point yLeftTop = verticalTitle.TransformToVisual(verticalAxis).Transform(new Point());
                    Assert.AreEqual(Math.Round(yLeftTop.Y), Math.Round((verticalAxis.ActualHeight + verticalTitle.ActualWidth) / 2));
                },
                () =>
                {
                    horizontalAxis.Title = "Much longer filler text";
                    verticalAxis.Title = new Rectangle { Width = 100, Height = 10 };
                },
                () =>
                {
                    Point xLeftTop = horizontalTitle.TransformToVisual(horizontalAxis).Transform(new Point());
                    Assert.AreEqual(Math.Round(xLeftTop.X), Math.Round((horizontalAxis.ActualWidth - horizontalTitle.ActualWidth) / 2));
                    Point yLeftTop = verticalTitle.TransformToVisual(verticalAxis).Transform(new Point());
                    Assert.AreEqual(Math.Round(yLeftTop.Y), Math.Round((verticalAxis.ActualHeight + verticalTitle.ActualWidth) / 2));
                });
        }

        /// <summary>
        /// Verifies that LinearAxis's ActualMinimum and ActualMaximum properties are correct.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that LinearAxis's ActualMinimum and ActualMaximum properties are correct.")]
        [Bug("604738: NumericAxis ActualMaximum, ActualMinimum always null", Fixed = true)]
        public void ActualMinimumActualMaximumCorrect()
        {
            Chart chart = new Chart();
            LinearAxis axis = new LinearAxis { Orientation = AxisOrientation.X, Minimum = 10, Maximum = 20 };
            chart.Axes.Add(axis);
            ScatterSeries series = new ScatterSeries();
            series.DependentValueBinding = new Binding();
            series.IndependentValueBinding = new Binding();
            int[] itemsSource = new int[] { 14 };
            series.ItemsSource = itemsSource;
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(axis.Minimum, axis.ActualMinimum),
                () => Assert.AreEqual(axis.Maximum, axis.ActualMaximum));
        }

        /// <summary>
        /// Verifies that adding an Axis with ShowGridLines set does not cause an exception.
        /// </summary>
        [TestMethod]
        [Description("Verifies that adding an Axis with ShowGridLines set does not cause an exception.")]
        [Bug("622901: Presence of axis in XAML without series causes Exception with ShowGridLines=true", Fixed = true)]
        public void AddingAxisWithGridlinesDoesNotCauseException()
        {
            Chart chart = new Chart();
            LinearAxis axis = new LinearAxis { Orientation = AxisOrientation.X, ShowGridLines = true };
            chart.Axes.Add(axis);
        }
    }
}