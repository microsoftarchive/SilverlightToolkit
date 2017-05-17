// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// An axis that contains logic to layout all axes.
    /// </summary>
    public abstract class DisplayAxisBase : AxisBase
    {
        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Description("Verifies the initial values of all properties.")]
        public virtual void InitialValues()
        {
            DisplayAxis axis = (DisplayAxis)DefaultControlToTest;
            Assert.AreEqual(AxisOrientation.None, axis.Orientation);
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