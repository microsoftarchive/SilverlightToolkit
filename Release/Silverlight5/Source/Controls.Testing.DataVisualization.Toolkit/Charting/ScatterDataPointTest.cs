// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media;

namespace System.Windows.Controls.Testing
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
            BorderBrushProperty.DefaultValue = new SolidColorBrush(Color.FromArgb(0xff, 0x68, 0x68, 0x68));
            BorderThicknessProperty.DefaultValue = new Thickness(0);
        }
    }
}