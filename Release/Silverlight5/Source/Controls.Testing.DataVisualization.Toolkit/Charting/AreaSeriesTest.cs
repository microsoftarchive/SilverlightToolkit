// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the AreaSeries class.
    /// </summary>
    [TestClass]
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Test class inheritance mirrors Charting inheritance.")]
    public partial class AreaSeriesTest : LineScatterSeriesBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new AreaSeries(); }
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // Remove the tests that react to the Background being the line color
            tests.RemoveTests(BackgroundProperty.BindingTest);
            tests.RemoveTests(BackgroundProperty.ChangeSetValueTest);
            tests.RemoveTests(BackgroundProperty.CheckDefaultValueTest);
            tests.RemoveTests(BackgroundProperty.CanBeStyledTest);
            tests.RemoveTests(BackgroundProperty.SetNullTest);

            return tests;
        }

        /// <summary>
        /// Initializes a new instance of the AreaSeriesTest class.
        /// </summary>
        public AreaSeriesTest()
        {
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies the initial values of all properties.")]
        public override void InitialValues()
        {
            AreaSeries series = (AreaSeries)DefaultControlToTest;
            TestAsync(
                series,
                () => Assert.IsNull(series.Geometry),
                () => Assert.IsNotNull(series.PathStyle));
        }

        /// <summary>
        /// Verify the Control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the Control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(3, properties.Count);
            Assert.AreEqual(typeof(AreaDataPoint), properties["DataPointStyle"]);
            Assert.AreEqual(typeof(LegendItem), properties["LegendItemStyle"]);
            Assert.AreEqual(typeof(Path), properties["PathStyle"]);
        }

        /// <summary>
        /// Verifies that the brush used to draw the path is not the default brush.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the brush used to draw the path is not the default brush.")]
        public void PathFillIsNotDefaultBrush()
        {
            Chart chart = new Chart();
            AreaSeries series = (AreaSeries)DefaultControlToTest;
            series.IndependentValueBinding = new Binding();
            ObservableCollection<int> itemsSource = new ObservableCollection<int> { 1, 2, 3 };
            series.ItemsSource = itemsSource;
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(3, ChartTestUtilities.GetDataPointsForSeries(series).Count()),
                () =>
                {
                    Path area = chart.GetVisualDescendents().OfType<Path>().FirstOrDefault();
                    Assert.AreNotEqual(typeof(SolidColorBrush), area.Fill.GetType());
                });
        }

        /// <summary>
        /// Verifies the ability to change a DateTime independent value.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies the ability to change a DateTime independent value.")]
        public void ChangeIndependentDateTimeValue()
        {
            Chart chart = new Chart();
            AreaSeries series = (AreaSeries)DefaultControlToTest;
            series.DependentValueBinding = new Binding("Value.Day");
            series.IndependentValueBinding = new Binding("Value");
            var dataObject = new NotifyingDataObject<DateTime> { Value = new DateTime(2009, 1, 10) };
            NotifyingDataObject<DateTime>[] itemsSource = new NotifyingDataObject<DateTime>[] { dataObject };
            series.ItemsSource = itemsSource;
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(1, ChartTestUtilities.GetDataPointsForSeries(series).Count),
                () => dataObject.Value = dataObject.Value.AddDays(1),
                () => Assert.AreEqual(1, ChartTestUtilities.GetDataPointsForSeries(series).Count));
        }

        /// <summary>
        /// Verifies that the DependentAxis cannot be a DateTimeAxis.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the DependentAxis cannot be a DateTimeAxis.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DependentAxisCannotBeDateTimeAxis()
        {
            DataPointSeries series = DefaultSeriesToTest;
            series.ItemsSource = new[] { new KeyValuePair<int, DateTime>(5, new DateTime(2009, 1, 10)) };
            series.IndependentValueBinding = new Binding("Key");
            series.DependentValueBinding = new Binding("Value");
            Chart chart = new Chart();
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(1, chart.Series.Count));
        }
    }
}