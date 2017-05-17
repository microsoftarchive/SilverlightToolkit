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
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// An axis that can be used to plot numeric values.
    /// </summary>
    public abstract class NumericAxisBase : RangeAxisBase
    {
        /// <summary>
        /// Changes the value of the Minimum and Maximum properties.
        /// </summary>
        [TestMethod]
        [Description("Changes the value of the Minimum and Maximum properties.")]
        public void MinimumMaximumChangeValues()
        {
            NumericAxis axis = (NumericAxis)DefaultControlToTest;
            double? minimum = 0.0;
            double? maximum = 100.0;
            axis.Minimum = minimum;
            axis.Maximum = maximum;
            Assert.AreEqual(minimum, axis.Minimum);
            Assert.AreEqual(maximum, axis.Maximum);
        }

        /// <summary>
        /// Tests that setting the minimum larger than the maximum throws.
        /// </summary>
        [TestMethod]
        [Description("Tests that setting the minimum larger than the maximum throws.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MinimumValueLargerThanMaximumThrowsException()
        {
            NumericAxis axis = (NumericAxis)DefaultControlToTest;
            double? minimum = 0.0;
            double? maximum = 100.0;
            axis.Minimum = minimum;
            axis.Maximum = maximum;
            minimum = 200.0;
            axis.Minimum = minimum;
        }

        /// <summary>
        /// Tests that setting the maximum smaller than the minimum throws.
        /// </summary>
        [TestMethod]
        [Description("Tests that setting the maximum smaller than the minimum throws.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MaximumValueSmallerThanMinimumThrowsException()
        {
            NumericAxis axis = (NumericAxis)DefaultControlToTest;
            double? minimum = 0.0;
            double? maximum = 100.0;
            axis.Minimum = minimum;
            axis.Maximum = maximum;
            axis.Maximum = -100.0;
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Description("Verifies the initial values of all properties.")]
        public override void InitialValues()
        {
            NumericAxis axis = (NumericAxis)DefaultControlToTest;
            Assert.AreEqual(null, axis.Maximum);
            Assert.AreEqual(null, axis.Minimum);
            Assert.AreEqual(AxisOrientation.None, axis.Orientation);
            base.InitialValues();
        }
    }
}
