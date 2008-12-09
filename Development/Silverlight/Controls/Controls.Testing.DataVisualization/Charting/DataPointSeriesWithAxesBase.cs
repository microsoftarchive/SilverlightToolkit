// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization.Charting;
using Microsoft.Silverlight.Testing;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for children of the DataPointSeries class.
    /// </summary>
    public abstract partial class DataPointSeriesWithAxesBase : DataPointSeriesBase
    {
        /// <summary>
        /// Gets a default instance of DataPointSeriesWithAxes (or a derived type) to test.
        /// </summary>
        public DataPointSeriesWithAxes DataPointSeriesWithAxesToTest
        {
            get
            {
                return DefaultSeriesToTest as DataPointSeriesWithAxes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataPointSeriesWithAxesBase class.
        /// </summary>
        protected DataPointSeriesWithAxesBase()
        {
        }

        /// <summary>
        /// Attempt to plot date time on dependent axis throws exception.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Attempt to plot date time on dependent axis throws exception.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public virtual void UseDateTimeValueAsDependentValue()
        {
            Chart chart = new Chart();
            DataPointSeriesWithAxes series = DataPointSeriesWithAxesToTest;
            DateTime[] dateTimes = new[] { DateTime.Now };
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => series.ItemsSource = dateTimes);
        }
    }
}