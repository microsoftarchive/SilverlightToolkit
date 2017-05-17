// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.DataVisualization;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// TreeMap unit tests.
    /// </summary>
    [TestClass]
    [Tag("TreeMap")]
    public class SolidColorBrushInterpolatorTest : TestBase
    {
        /// <summary>
        /// Delta used for double precision checks.
        /// </summary>
        private const double DELTA = 0.000001;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleInterpolatorTest"/> class. 
        /// </summary>
        public SolidColorBrushInterpolatorTest()
        {
        }

        /// <summary>
        /// Checks if a Solid Color Brush is as expected.
        /// </summary>
        /// <param name="brush">SolidColorBrush reference.</param>
        /// <param name="a">Alpha channel.</param>
        /// <param name="r">Red channel.</param>
        /// <param name="g">Green channel.</param>
        /// <param name="b">Blue channel.</param>
        internal static void BrushAssert(SolidColorBrush brush, byte a, byte r, byte g, byte b)
        {
            Assert.AreEqual(a, brush.Color.A);
            Assert.AreEqual(r, brush.Color.R);
            Assert.AreEqual(g, brush.Color.G);
            Assert.AreEqual(b, brush.Color.B);
        }

        /// <summary>
        /// Basic verifies the DoubleInterpolator.
        /// </summary>
        [TestMethod]
        [Description("Basic verifies the SolidColorBrushInterpolatorTest.")]
        [Bug("710577: Interpolators need to have RGB support", Fixed = true)]
        public virtual void BasicTests()
        {
            SolidColorBrushInterpolator interpolator = new SolidColorBrushInterpolator
            {
                DataMaximum = 100,
                DataMinimum = 0,
                From = Color.FromArgb(200, 0, 0, 0),
                To = Color.FromArgb(100, 100, 200, 10)
            };

            // Regular tests
            BrushAssert((SolidColorBrush)interpolator.Interpolate(1), 199, 1, 2, 0);
            BrushAssert((SolidColorBrush)interpolator.Interpolate(0), 200, 0, 0, 0);
            BrushAssert((SolidColorBrush)interpolator.Interpolate(100), 100, 100, 200, 10);

            // TODO: Right now colors are working for child level only 
            BrushAssert((SolidColorBrush)interpolator.Interpolate(200), 0, 200, 144, 20);
        }

        /// <summary>
        /// Verifies the DataMaximum / DataMinimum behaviors..
        /// </summary>
        [TestMethod]
        [Description("Verifies the DataMaximum / DataMinimum behaviors.")]
        [Bug("710577: Interpolators need to have RGB support", Fixed = true)]
        public virtual void DataMaximumMinimumTests()
        {
            const double DataBorder = 100;

            // DataMinimum == DataMaximum
            SolidColorBrushInterpolator interpolator = new SolidColorBrushInterpolator
            {
                DataMaximum = DataBorder,
                DataMinimum = DataBorder,
                From = Color.FromArgb(200, 0, 0, 0),
                To = Color.FromArgb(100, 100, 200, 10)
            };

            BrushAssert(
                (SolidColorBrush)interpolator.Interpolate(1),
                interpolator.From.A,
                interpolator.From.R,
                interpolator.From.G,
                interpolator.From.B);

            //// No exception for close values 

            // Calculate minimum double value that (dataBorder - value) yields 
            // a different number
            double minDoubleDiff = 1;
            while ((DataBorder - (minDoubleDiff / 2)) != DataBorder)
            {
                minDoubleDiff /= 2;
            }

            // No exception (div by 0) for very close values 
            interpolator.DataMinimum = DataBorder - minDoubleDiff;
            interpolator.Interpolate(0.01);
            interpolator.DataMinimum = DataBorder - (minDoubleDiff / 2);
            interpolator.Interpolate(0.01);
        }
    }
}
