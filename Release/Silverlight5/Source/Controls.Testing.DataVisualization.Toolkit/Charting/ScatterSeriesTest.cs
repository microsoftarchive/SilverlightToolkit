// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the ScatterSeries class.
    /// </summary>
    [TestClass]
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Test class inheritance mirrors Charting inheritance.")]
    public partial class ScatterSeriesTest : LineScatterSeriesBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new ScatterSeries(); }
        }

        /// <summary>
        /// Initializes a new instance of the ScatterSeriesTest class.
        /// </summary>
        public ScatterSeriesTest()
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
            Assert.AreEqual(typeof(ScatterDataPoint), properties["DataPointStyle"]);
            Assert.AreEqual(typeof(LegendItem), properties["LegendItemStyle"]);
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
    }
}