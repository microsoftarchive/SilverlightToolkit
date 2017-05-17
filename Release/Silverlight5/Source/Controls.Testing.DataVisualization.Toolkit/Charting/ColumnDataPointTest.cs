// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.DataVisualization.Charting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the ColumnDataPoint class.
    /// </summary>
    [TestClass]
    public partial class ColumnDataPointTest : DataPointBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new ColumnDataPoint(); }
        }

        /// <summary>
        /// Initializes a new instance of the ColumnDataPointTest class.
        /// </summary>
        public ColumnDataPointTest()
        {
        }
    }
}