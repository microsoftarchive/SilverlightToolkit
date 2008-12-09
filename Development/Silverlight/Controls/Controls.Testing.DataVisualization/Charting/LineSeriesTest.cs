// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization.Charting;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the LineSeries class.
    /// </summary>
    [TestClass]
    public partial class LineSeriesTest : LineScatterSeriesBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new LineSeries(); }
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
        /// Initializes a new instance of the LineSeriesTest class.
        /// </summary>
        public LineSeriesTest()
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
            Assert.AreEqual(3, properties.Count);
            Assert.AreEqual(typeof(LineDataPoint), properties["DataPointStyle"]);
            Assert.AreEqual(typeof(LegendItem), properties["LegendItemStyle"]);
            Assert.AreEqual(typeof(Polyline), properties["PolylineStyle"]);
        }
    }
}