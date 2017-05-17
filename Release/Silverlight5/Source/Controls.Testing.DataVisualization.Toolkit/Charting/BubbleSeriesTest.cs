// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization.Charting;
using Microsoft.Silverlight.Testing;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the BubbleSeries class.
    /// </summary>
    [TestClass]
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Test class inheritance mirrors Charting inheritance.")]
    public partial class BubbleSeriesTest : LineScatterSeriesBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new BubbleSeries(); }
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
            tests.RemoveTests(BackgroundProperty.CheckDefaultValueTest);
            tests.RemoveTests(BackgroundProperty.CanBeStyledTest);

            return tests;
        }

        /// <summary>
        /// Initializes a new instance of the BubbleSeriesTest class.
        /// </summary>
        public BubbleSeriesTest()
        {
        }

        /// <summary>
        /// Verify the Control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the Control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(2, properties.Count);
            Assert.AreEqual(typeof(BubbleDataPoint), properties["DataPointStyle"]);
            Assert.AreEqual(typeof(LegendItem), properties["LegendItemStyle"]);
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Description("Verifies the initial values of all properties.")]
        public override void InitialValues()
        {
            base.InitialValues();
            BubbleSeries series = DefaultControlToTest as BubbleSeries;
            Assert.IsNull(series.SizeValueBinding);
            Assert.IsNull(series.SizeValuePath);
        }

        /// <summary>
        /// Verifies that setting SizeValueBinding and SizeValuePath results in consistent behavior.
        /// </summary>
        [TestMethod]
        [Description("Verifies that setting SizeValueBinding and SizeValuePath results in consistent behavior.")]
        public void SizeValueBindingConsistentWithSizeValuePath()
        {
            BubbleSeries series = DefaultControlToTest as BubbleSeries;

            string path = "Path1";
            series.SizeValueBinding = new Binding(path);
            Assert.IsNotNull(series.SizeValueBinding);
            Assert.AreEqual(path, series.SizeValueBinding.Path.Path);
            Assert.AreEqual(path, series.SizeValuePath);

            series.SizeValueBinding = null;
            Assert.IsNull(series.SizeValueBinding);
            Assert.IsNull(series.SizeValuePath);

            path = "Path2";
            series.SizeValuePath = path;
            Assert.AreEqual(path, series.SizeValuePath);
            Assert.IsNotNull(series.SizeValueBinding);
            Assert.AreEqual(path, series.SizeValueBinding.Path.Path);

            series.SizeValuePath = null;
            Assert.IsNull(series.SizeValuePath);
            Assert.IsNull(series.SizeValueBinding);

            path = "";
            series.SizeValuePath = path;
            Assert.AreEqual(path, series.SizeValuePath);
            Assert.IsNotNull(series.SizeValueBinding);
            Assert.AreEqual(path, series.SizeValueBinding.Path.Path);
        }

        /// <summary>
        /// Verifies that the DependentAxis can be a DateTimeAxis.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the DependentAxis can be a DateTimeAxis.")]
        [Bug("527881: Dependent Axis should allow DateTime", Fixed = true)]
        public void DependentAxisCanBeDateTimeAxis()
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

        /// <summary>
        /// Verifies that using SizeValuePath works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that using SizeValuePath works properly.")]
        public void UsingSizeValuePathWorks()
        {
            Chart chart = new Chart();
            BubbleSeries series = DefaultControlToTest as BubbleSeries;
            series.DependentValuePath = "Value";
            series.IndependentValuePath = "Key";
            series.SizeValuePath = "Value";
            series.ItemsSource = new KeyValuePair<int, int>[] { new KeyValuePair<int, int>(1, 2), new KeyValuePair<int, int>(3, 4), new KeyValuePair<int, int>(5, 6) };
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(3, ChartTestUtilities.GetDataPointsForSeries(series).Count()));
        }
    }
}