// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization.Charting.Primitives;

#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace System.Windows.Controls.Testing
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
            PaddingProperty.DefaultValue = new Thickness(10);
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
            Assert.AreEqual(2, templateParts.Count);

            Assert.AreSame(typeof(EdgePanel), templateParts["ChartArea"]);
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
            Assert.AreEqual(typeof(EdgePanel), properties["ChartAreaStyle"]);
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
                () => Assert.IsNotNull(chart.Palette),
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
        [Priority(0)]
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
        /// Sets the Styles property.
        /// </summary>
        [TestMethod]
        [Description("Sets the Styles property.")]
        public void SetSeriesStyles()
        {
            Chart chart = new Chart();
            Collection<ResourceDictionary> seriesStyles = new Collection<ResourceDictionary> { new ResourceDictionary(), new ResourceDictionary() };
            chart.Palette = seriesStyles;
            Assert.AreSame(seriesStyles, chart.Palette);
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
        /// Creates a ISeriesHost with a LineSeries that uses an Axis from the ISeriesHost's Axis collection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a LineSeries that uses an Axis from the Chart's Axis collection.")]
        [Priority(0)]
        public void AxisInAxisCollection()
        {
            Chart chart = new Chart();
            DateTimeAxis dateTimeAxis = new DateTimeAxis();
            dateTimeAxis.Orientation = AxisOrientation.X;
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
                () => chart.UpdateLayout());
        }

        /// <summary>
        /// Verifies that calling the Chart.Axis setter throws a NotSupportedException.
        /// </summary>
        [TestMethod]
        [Description("Verifies that calling the Chart.Axes setter throws a NotSupportedException.")]
        [ExpectedException(typeof(NotSupportedException))]
        [Bug("530728: Chart.Axis and .Series should have a public setter that throws NotSupportedException so that these properties will show up in Visual Studio Intellisense", Fixed = true)]
        public void AxisSetterNotSupported()
        {
            Chart chart = new Chart();
            chart.Axes = new Collection<IAxis>();
        }

        /// <summary>
        /// Verifies that calling the Chart.Series setter throws a NotSupportedException.
        /// </summary>
        [TestMethod]
        [Description("Verifies that calling the Chart.Series setter throws a NotSupportedException.")]
        [ExpectedException(typeof(NotSupportedException))]
        [Bug("530728: Chart.Axis and .Series should have a public setter that throws NotSupportedException so that these properties will show up in Visual Studio Intellisense", Fixed = true)]
        public void SeriesSetterNotSupported()
        {
            Chart chart = new Chart();
            chart.Series = new Collection<ISeries>();
        }

        /// <summary>
        /// Removes an Axis from an empty Chart to exercise a problematic scenario.
        /// </summary>
        [TestMethod]
        [Description("Removes an Axis from an empty Chart to exercise a problematic scenario.")]
        [Bug("564186: NullReferenceException in Chart.OnAxesCollectionChanged when removing any Axis: Chart.cs line 744, e.NewItems should be e.OldItems", Fixed = true)]
        public void RemoveAxisFromEmptyChart()
        {
            Chart chart = new Chart();
            LinearAxis axis = new LinearAxis { Orientation = AxisOrientation.X };
            chart.Axes.Add(axis);
            chart.Axes.Remove(axis);
        }

        /// <summary>
        /// Adds a LineSeries and a ColumnSeries that share the same independent axis.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Adds a LineSeries and a ColumnSeries that share the same independent axis.")]
        [Bug("700372: Charting exception on mixed Column & Line Series", Fixed = true)]
        public void LineAndColumnSeriesSharingIndependentAxis()
        {
            List<KeyValuePair<int, double>> itemsSource = new List<KeyValuePair<int, double>>();
            itemsSource.Add(new KeyValuePair<int, double>(1, 2.0));
            Chart chart = new Chart();
            LineSeries lineSeries = new LineSeries();
            lineSeries.DependentValuePath = "Value";
            lineSeries.IndependentValuePath = "Key";
            lineSeries.ItemsSource = itemsSource;
            chart.Series.Add(lineSeries);
            ColumnSeries columnSeries = new ColumnSeries();
            columnSeries.DependentValuePath = "Value";
            columnSeries.IndependentValuePath = "Key";
            columnSeries.ItemsSource = itemsSource;
            chart.Series.Add(columnSeries);
            TestAsync(
                chart,
                () => chart.UpdateLayout());
        }

        /// <summary>
        /// Adds a ColumnSeries and a LineSeries that share the same independent axis.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Adds a ColumnSeries and a LineSeries that share the same independent axis.")]
        [Bug("700372: Charting exception on mixed Column & Line Series", Fixed = true)]
        public void ColumnAndLineSeriesSharingIndependentAxis()
        {
            List<KeyValuePair<int, double>> itemsSource = new List<KeyValuePair<int, double>>();
            itemsSource.Add(new KeyValuePair<int, double>(1, 2.0));
            Chart chart = new Chart();
            ColumnSeries columnSeries = new ColumnSeries();
            columnSeries.DependentValuePath = "Value";
            columnSeries.IndependentValuePath = "Key";
            columnSeries.ItemsSource = itemsSource;
            chart.Series.Add(columnSeries);
            LineSeries lineSeries = new LineSeries();
            lineSeries.DependentValuePath = "Value";
            lineSeries.IndependentValuePath = "Key";
            lineSeries.ItemsSource = itemsSource;
            chart.Series.Add(lineSeries);
            TestAsync(
                chart,
                () => chart.UpdateLayout());
        }

        /// <summary>
        /// Verifies the ability to add a simple ISeries implementation to the Series collection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies the ability to add a simple ISeries implementation to the Series collection.")]
        public void AddSimpleISeries()
        {
            Chart chart = new Chart();
            SimpleISeries simpleISeries = new SimpleISeries();
            TestAsync(
                chart,
                () => chart.Series.Add(simpleISeries),
                () => Assert.IsNotNull(simpleISeries.SeriesHost),
                () => chart.Series.Remove(simpleISeries),
                () => Assert.IsNull(simpleISeries.SeriesHost));
        }

        /// <summary>
        /// Simple ISeries implementation.
        /// </summary>
        internal class SimpleISeries : ISeries
        {
            /// <summary>
            /// Gets an empty collection of legend items.
            /// </summary>
            public ObservableCollection<object> LegendItems
            {
                get { return new ObservableCollection<object>(); }
            }

            /// <summary>
            /// Gets or sets the SeriesHost.
            /// </summary>
            public ISeriesHost SeriesHost { get; set; }
        }

        /// <summary>
        /// Verifies the ability to add a simple IAxis implementation to the Axis collection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies the ability to add a simple IAxis implementation to the Axis collection.")]
        public void AddSimpleIAxis()
        {
            Chart chart = new Chart();
            SimpleIAxis simpleIAxis = new SimpleIAxis();
            TestAsync(
                chart,
                () => chart.Axes.Add(simpleIAxis),
                () => Assert.IsTrue(chart.ActualAxes.Contains(simpleIAxis)),
                () => chart.Axes.Remove(simpleIAxis),
                () => Assert.IsFalse(chart.ActualAxes.Contains(simpleIAxis)));
        }

        /// <summary>
        /// Simple IAxis implementation.
        /// </summary>
        internal class SimpleIAxis : IAxis
        {
            /// <summary>
            /// Gets or sets the orientation.
            /// </summary>
            public AxisOrientation Orientation
            {
                get
                {
                    return AxisOrientation.None;
                }
                set
                {
                    RoutedPropertyChangedEventHandler<AxisOrientation> handler = OrientationChanged;
                    if (null != handler)
                    {
                        handler.Invoke(this, new RoutedPropertyChangedEventArgs<AxisOrientation>(AxisOrientation.None, AxisOrientation.None));
                    }
                }
            }

            /// <summary>
            /// Event that is invoked when the Orientation property is changed.
            /// </summary>
            public event RoutedPropertyChangedEventHandler<AxisOrientation> OrientationChanged;

            /// <summary>
            /// Indicates the ability to plot the specified value.
            /// </summary>
            /// <param name="value">Specified value.</param>
            /// <returns>True if it can be plotted.</returns>
            public bool CanPlot(object value)
            {
                return false;
            }

            /// <summary>
            /// Returns the plot area coordinate of the specified value.
            /// </summary>
            /// <param name="value">Specified value.</param>
            /// <returns>Plot area coordinate.</returns>
            public UnitValue GetPlotAreaCoordinate(object value)
            {
                return UnitValue.NaN();
            }

            /// <summary>
            /// Gets a collection of the registerd listeners.
            /// </summary>
            public ObservableCollection<IAxisListener> RegisteredListeners
            {
                get { return new ObservableCollection<IAxisListener>(); }
            }

            /// <summary>
            /// Gets a collection of the dependent axes.
            /// </summary>
            public ObservableCollection<IAxis> DependentAxes
            {
                get { return new ObservableCollection<IAxis>(); }
            }
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
