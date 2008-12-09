// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization;
using Microsoft.Windows.Controls.DataVisualization.Charting;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// An axis that contains logic to layout all axes.
    /// </summary>
    public abstract class HybridAxisBase : AxisBase
    {
        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Description("Verifies the initial values of all properties.")]
        public virtual void InitialValues()
        {
            HybridAxis axis = (HybridAxis)DefaultControlToTest;
            Assert.AreEqual(AxisOrientation.Vertical, axis.Orientation);
            Assert.IsNull(axis.Title);
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
            Assert.AreSame(typeof(Grid), templateParts["AxisGrid"]);
            Assert.AreSame(typeof(Title), templateParts["AxisTitle"]);
        }
    }
}