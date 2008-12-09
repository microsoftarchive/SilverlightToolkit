// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization.Charting;
using Microsoft.Silverlight.Testing;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the BubbleSeries class.
    /// </summary>
    [TestClass]
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
        /// Attempt to plot date time on dependent axis throws exception.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Attempt to plot date time on dependent axis throws exception.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public override void UseDateTimeValueAsDependentValue()
        {
            Chart chart = new Chart();
            BubbleSeries series = (BubbleSeries) DataPointSeriesWithAxesToTest;
            series.SizeValueBinding = new Binding("Day");
            DateTime[] dateTimes = new[] { DateTime.Now };
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => series.ItemsSource = dateTimes);
        }
    }
}