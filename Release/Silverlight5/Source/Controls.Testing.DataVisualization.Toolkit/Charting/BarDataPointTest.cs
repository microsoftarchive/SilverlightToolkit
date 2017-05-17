// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization.Charting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the BarDataPoint class.
    /// </summary>
    [TestClass]
    public partial class BarDataPointTest : DataPointBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new BarDataPoint(); }
        }

        /// <summary>
        /// Initializes a new instance of the BarDataPointTest class.
        /// </summary>
        public BarDataPointTest()
        {
        }
    }
}