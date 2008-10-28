// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization.Charting;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the ScatterDataPointTest class.
    /// </summary>
    [TestClass]
    public partial class ScatterDataPointTest : DataPointBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new ScatterDataPoint(); }
        }

        /// <summary>
        /// Initializes a new instance of the ScatterDataPointTest class.
        /// </summary>
        public ScatterDataPointTest()
        {
        }
    }
}