// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization;
using Microsoft.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// This class tests the CategoryAxis.
    /// </summary>
    [TestClass]
    public class CategoryAxisTest : HybridAxisBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new CategoryAxis(); }
        }

        /// <summary>
        /// Assigns a string to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a string to the Title property.")]
        public void TitleChangeString()
        {
            CategoryAxis axis = (CategoryAxis)DefaultControlToTest;
            string title = "String Title";
            axis.Title = title;
            Assert.AreEqual(title, axis.Title);
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
            Assert.AreEqual(typeof(Line), properties["MajorTickMarkStyle"]);
            Assert.AreEqual(typeof(AxisLabel), properties["AxisLabelStyle"]);
            Assert.AreEqual(typeof(Title), properties["TitleStyle"]);
            Assert.AreEqual(typeof(Line), properties["GridLineStyle"]);
        }
    }
}