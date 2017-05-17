// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.DataVisualization;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// TreeMap unit tests.
    /// </summary>
    [TestClass]
    [Tag("TreeMap")]
    public class DoubleInterpolatorTest : TestBase
    {
        /// <summary>
        /// Delta used for double precisions checks.
        /// </summary>
         private const double Delta = 0.000001;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleInterpolatorTest"/> class. 
        /// </summary>
        public DoubleInterpolatorTest()
        {
        }

        /// <summary>
        /// Basic verifies the DoubleInterpolator.
        /// </summary>
        [TestMethod]
        [Description("Basic verifies the DoubleInterpolator.")]
        public virtual void BasicTests()
        {
            DoubleInterpolator doubleInterpolator = new DoubleInterpolator
                                                        {
                                                            DataMaximum = 100,
                                                            DataMinimum = 0,
                                                            From = 0,
                                                            To = 0.1,
                                                        };

            // Regular tests
            Assert.AreEqual(0.001, (double)doubleInterpolator.Interpolate(1), Delta);
            Assert.AreEqual(0, (double)doubleInterpolator.Interpolate(0), Delta);
            Assert.AreEqual(0.1, (double)doubleInterpolator.Interpolate(100), Delta);

            // We aggregate values in leaves nodes so parent nodes will have 
            // values > DataMaximum.
            Assert.AreEqual(1, (double)doubleInterpolator.Interpolate(1000), Delta);

            // As we're allowing DataMinimum > DataMaximum behavior, input value
            // < DataMinimum is OK
            Assert.AreEqual(-1, (double)doubleInterpolator.Interpolate(-1000), Delta);

            // As above test but now we widening the range
            doubleInterpolator = new DoubleInterpolator
                                     {
                DataMaximum = 0.1,
                DataMinimum = 0,
                From = 0,
                To = 100,
            };
            
            // Regular tests
            Assert.AreEqual(1, (double)doubleInterpolator.Interpolate(0.001), Delta);
            Assert.AreEqual(0, (double)doubleInterpolator.Interpolate(0), Delta);
            Assert.AreEqual(100, (double)doubleInterpolator.Interpolate(0.1), Delta);
            Assert.AreEqual(1000, (double)doubleInterpolator.Interpolate(1), Delta);
            Assert.AreEqual(-1000, (double)doubleInterpolator.Interpolate(-1), Delta);
        }

        /// <summary>
        /// Verifies the DataMaximum / DataMinimum behaviors..
        /// </summary>
        [TestMethod]
        [Description("Verifies the DataMaximum / DataMinimum behaviors.")]
        public virtual void DataMaximumMinimumTests()
        {
            const double DataBorder = 100;

            // DataMinimum == DataMaximum
            DoubleInterpolator doubleInterpolator = new DoubleInterpolator
                                                        {
                DataMaximum = DataBorder,
                DataMinimum = DataBorder,
                From = 0,
                To = 0.1,
            };

            Assert.AreEqual(doubleInterpolator.From, (double)doubleInterpolator.Interpolate(0.001), Delta);
            Assert.AreEqual(doubleInterpolator.From, (double)doubleInterpolator.Interpolate(0), Delta);

            // Calculate minimum double value that (dataBorder - value) yields 
            // a different number
            double minDoubleDiff = 1;
            while ((DataBorder - (minDoubleDiff / 2)) != DataBorder)
            {
                minDoubleDiff /= 2;
            }

            // No exception (div by 0) for very close values 
            doubleInterpolator.DataMinimum = DataBorder - minDoubleDiff;
            doubleInterpolator.Interpolate(0.01);
            doubleInterpolator.DataMinimum = DataBorder - (minDoubleDiff / 2);
            doubleInterpolator.Interpolate(0.01);
         }
    }
}
