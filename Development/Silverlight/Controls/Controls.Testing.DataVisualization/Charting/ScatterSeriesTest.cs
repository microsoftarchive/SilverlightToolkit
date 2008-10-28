// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization.Charting;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the ScatterSeries class.
    /// </summary>
    [TestClass]
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
    }
}
