// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization;
using Microsoft.Windows.Controls.DataVisualization.Charting;

namespace Microsoft.Windows.Controls.Testing
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
    }
}