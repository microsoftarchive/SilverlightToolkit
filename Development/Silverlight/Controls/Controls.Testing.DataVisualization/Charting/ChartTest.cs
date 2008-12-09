// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization;
using Microsoft.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the ISeriesHost class.
    /// </summary>
    [TestClass]
    public partial class ChartTest : ControlTest
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new Chart(); }
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
        /// Initializes a new instance of the ChartTest class.
        /// </summary>
        public ChartTest()
        {
            BorderBrushProperty.DefaultValue = new SolidColorBrush(Colors.Black);
            BorderThicknessProperty.DefaultValue = new Thickness(1);
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
            Assert.AreEqual(5, templateParts.Count);
            Assert.AreSame(typeof(Grid), templateParts["ChartArea"]);
            Assert.AreSame(typeof(Grid), templateParts["PlotArea"]);
            Assert.AreSame(typeof(Panel), templateParts["SeriesContainer"]);
            Assert.AreSame(typeof(Panel), templateParts["GridLinesContainer"]);
            Assert.AreSame(typeof(Legend), templateParts["Legend"]);
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
            Assert.AreEqual(typeof(Title), properties["TitleStyle"]);
            Assert.AreEqual(typeof(Legend), properties["LegendStyle"]);
            Assert.AreEqual(typeof(Grid), properties["ChartAreaStyle"]);
            Assert.AreEqual(typeof(Grid), properties["PlotAreaStyle"]);
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        [TestMethod]
        [Description("Creates a new instance.")]
        public void NewInstance()
        {
            Chart chart = new Chart();
            Assert.IsNotNull(chart);
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies the initial values of all properties.")]
        public void InitialValues()
        {
            Chart chart = new Chart();
            TestAsync(
                chart,
                () => Assert.IsNull(chart.Title),
                () => Assert.IsNull(chart.LegendTitle),
                () => Assert.IsNotNull(chart.Series),
                () => Assert.AreEqual(0, chart.Series.Count),
                () => Assert.IsNotNull(chart.LegendItems),
                () => Assert.AreEqual(0, (new ObjectCollection(chart.LegendItems)).Count),
                () => Assert.IsNotNull(chart.StylePalette),
                () => Assert.IsNotNull(chart.TitleStyle),
                () => Assert.IsNotNull(chart.LegendStyle),
                () => Assert.IsNotNull(chart.ChartAreaStyle),
                () => Assert.IsNotNull(chart.PlotAreaStyle));
        }

        /// <summary>
        /// Changes the TitleStyle property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the TitleStyle property.")]
        public void TitleStyleChange()
        {
            Chart chart = new Chart();
            // Change ISeriesHost's Template because Silverlight only allows setting Style properties once.
            chart.Template = new ControlTemplate();
            Style style = new Style(typeof(Title));
            TestAsync(
                chart,
                () => chart.TitleStyle = style,
                () => Assert.AreSame(style, chart.TitleStyle));
        }

        /// <summary>
        /// Changes the LegendStyle property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the LegendStyle property.")]
        public void LegendStyleChange()
        {
            Chart chart = new Chart();
            // Change ISeriesHost's Template because Silverlight only allows setting Style properties once.
            chart.Template = new ControlTemplate();
            Style style = new Style(typeof(Legend));
            TestAsync(
                chart,
                () => chart.LegendStyle = style,
                () => Assert.AreSame(style, chart.LegendStyle));
        }

        /// <summary>
        /// Changes the ChartAreaStyle property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the ChartAreaStyle property.")]
        public void ChartAreaStyleChange()
        {
            Chart chart = new Chart();
            // Change ISeriesHost's Template because Silverlight only allows setting Style properties once.
            chart.Template = new ControlTemplate();
            Style style = new Style(typeof(Panel));
            TestAsync(
                chart,
                () => chart.ChartAreaStyle = style,
                () => Assert.AreSame(style, chart.ChartAreaStyle));
        }

        /// <summary>
        /// Changes the PlotAreaStyle property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the PlotAreaStyle property.")]
        public void PlotAreaStyleChange()
        {
            Chart chart = new Chart();
            // Change ISeriesHost's Template because Silverlight only allows setting Style properties once.
            chart.Template = new ControlTemplate();
            Style style = new Style(typeof(Panel));
            TestAsync(
                chart,
                () => chart.PlotAreaStyle = style,
                () => Assert.AreSame(style, chart.PlotAreaStyle));
        }

        /// <summary>
        /// Assigns a string to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a string to the Title property.")]
        public void TitleChangeString()
        {
            Chart chart = new Chart();
            string title = "String Title";
            chart.Title = title;
            Assert.AreSame(title, chart.Title);
        }

        /// <summary>
        /// Assigns an object to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns an object to the Title property.")]
        public void TitleChangeObject()
        {
            Chart chart = new Chart();
            object title = new object();
            chart.Title = title;
            Assert.AreSame(title, chart.Title);
        }

        /// <summary>
        /// Assigns a Button to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a Button to the Title property.")]
        public void TitleChangeButton()
        {
            Chart chart = new Chart();
            Button title = new Button { Content = "Button Title" };
            chart.Title = title;
            Assert.AreSame(title, chart.Title);
        }

        /// <summary>
        /// Assigns a string to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a string to the Title property.")]
        public void LegendTitleChangeString()
        {
            Chart chart = new Chart();
            string legendTitle = "String Legend Title";
            chart.LegendTitle = legendTitle;
            Assert.AreSame(legendTitle, chart.LegendTitle);
        }

        /// <summary>
        /// Assigns an object to the LegendTitle property.
        /// </summary>
        [TestMethod]
        [Description("Assigns an object to the LegendTitle property.")]
        public void LegendTitleChangeObject()
        {
            Chart chart = new Chart();
            object legendTitle = new object();
            chart.LegendTitle = legendTitle;
            Assert.AreSame(legendTitle, chart.LegendTitle);
        }

        /// <summary>
        /// Assigns a Button to the LegendTitle property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a Button to the LegendTitle property.")]
        public void LegendTitleChangeButton()
        {
            Chart chart = new Chart();
            Button legendTitle = new Button { Content = "Button Legend Title" };
            chart.LegendTitle = legendTitle;
            Assert.AreSame(legendTitle, chart.LegendTitle);
        }

        /// <summary>
        /// Verifies that adding an object instance to the Series property collection is not allowed.
        /// </summary>
        [TestMethod]
        [Description("Verifies that adding an object instance to the Series property collection is not allowed.")]
        [ExpectedException(typeof(ArgumentException))]
        public void SeriesAddObjectException()
        {
            Chart chart = new Chart();
            chart.Series.Add(new object());
        }

        /// <summary>
        /// Assigns a single Series to the Series property collection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Assigns a single Series to the Series property collection.")]
        public void SeriesAddOneSeries()
        {
            Chart chart = new Chart();
            DataPointSeries series = new ColumnSeries();
            series.ItemsSource = new int[] { 2, 4, 6 };
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => AssertSeriesCorrect(chart, series));
        }

        /// <summary>
        /// Adds and removes a single Series to the Series property collection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Adds and removes a single Series to the Series property collection.")]
        public void SeriesRemoveOneSeries()
        {
            Chart chart = new Chart();
            DataPointSeries series = new ColumnSeries();
            series.ItemsSource = new int[] { 2, 4, 6 };
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => AssertSeriesCorrect(chart, series),
                () => chart.Series.Remove(series),
                () => AssertSeriesCorrect(chart),
                () => Assert.IsNull(series.SeriesHost));
        }

        /// <summary>
        /// Changes the ItemsSource for the ISeriesHost's Series.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the ItemsSource for the Chart's Series.")]
        public void SeriesItemsSourceChange()
        {
            Chart chart = new Chart();
            DataPointSeries series = new ColumnSeries();
            TestAsync(
                chart,
                () => series.ItemsSource = new int[] { 1, 2 },
                () => chart.Series.Add(series),
                () => AssertSeriesCorrect(chart, series),
                () => series.ItemsSource = new double[] { 1.1, 2.1 },
                () => AssertSeriesCorrect(chart, series));
        }

        /// <summary>
        /// Verifies the LegendItems property is read-only.
        /// </summary>
        [TestMethod]
        [Description("Verifies the LegendItems property is read-only.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LegendItemsIsReadOnly()
        {
            Chart chart = new Chart();
            chart.SetValue(Chart.LegendItemsProperty, new ObservableCollection<UIElement>());
        }

        /// <summary>
        /// Sets the Styles property.
        /// </summary>
        [TestMethod]
        [Description("Sets the Styles property.")]
        public void SetSeriesStyles()
        {
            Chart chart = new Chart();
            IList seriesStyles = new Style[] { new Style(typeof(Control)), new Style(typeof(Series)) };
            chart.StylePalette = seriesStyles;
            Assert.AreSame(seriesStyles, chart.StylePalette);
        }

        /// <summary>
        /// Verifies that adding a Series to two Charts at the same time is not allowed.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that adding a Series to two Charts at the same time is not allowed.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SeriesInMultipleCharts()
        {
            Grid grid = new Grid();
            Chart chartA = new Chart();
            grid.Children.Add(chartA);
            Chart chartB = new Chart();
            grid.Children.Add(chartB);
            DataPointSeries series = new ColumnSeries();
            TestAsync(
                grid,
                () => chartA.Series.Add(series),
                () => chartB.Series.Add(series));
        }

        /// <summary>
        /// Calls Refresh with no Series present.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Calls Refresh with no Series present.")]
        public void RefreshNoSeries()
        {
            Chart chart = new Chart();
            TestAsync(
                chart,
                () => chart.Refresh(),
                () => AssertSeriesCorrect(chart));
        }

        /// <summary>
        /// Calls Refresh with one Series present.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Calls Refresh with one Series present.")]
        public void RefreshOneSeries()
        {
            Chart chart = new Chart();
            DataPointSeries series = new ColumnSeries();
            series.ItemsSource = new int[] { 0 };
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => chart.Refresh(),
                () => AssertSeriesCorrect(chart, series));
        }

        /// <summary>
        /// Creates a ISeriesHost with a LineSeries that uses an Axis from the ISeriesHost's Axis collection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a LineSeries that uses an Axis from the Chart's Axis collection.")]
        public void AxisInAxisCollection()
        {
            Chart chart = new Chart();
            DateTimeAxis dateTimeAxis = new DateTimeAxis();
            dateTimeAxis.Orientation = AxisOrientation.Horizontal;
            dateTimeAxis.IntervalType = DateTimeIntervalType.Days;
            dateTimeAxis.Interval = 1;
            chart.Axes.Add(dateTimeAxis);
            DataPointSeries series = new LineSeries();
            series.DependentValueBinding = new Binding("Day");
            series.IndependentValueBinding = new Binding();
            series.ItemsSource = new DateTime[] { new DateTime(2008, 1, 1), new DateTime(2008, 1, 2) };
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => chart.Refresh());
        }

        /// <summary>
        /// Asserts that the content of a ISeriesHost's Series property collection is correct.
        /// </summary>
        /// <param name="chart">ISeriesHost instance to use.</param>
        /// <param name="expected">Expected collection.</param>
        private static void AssertSeriesCorrect(Chart chart, params Series[] expected)
        {
            Assert.AreEqual(expected.Length, chart.Series.Count);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreSame(expected[i], chart.Series[i]);
                Assert.AreSame(chart, expected[i].SeriesHost);
            }
        }
    }
}
