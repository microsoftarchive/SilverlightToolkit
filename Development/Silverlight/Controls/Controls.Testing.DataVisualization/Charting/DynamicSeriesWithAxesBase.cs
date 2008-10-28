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
    /// Unit tests for children of the DynamicSeries class.
    /// </summary>
    public abstract partial class DynamicSeriesWithAxesBase : DynamicSeriesBase
    {
        /// <summary>
        /// Gets a default instance of DynamicSeriesWithAxes (or a derived type) to test.
        /// </summary>
        public DynamicSeriesWithAxes DynamicSeriesWithAxesToTest
        {
            get
            {
                return DefaultSeriesToTest as DynamicSeriesWithAxes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DynamicSeriesWithAxesBase class.
        /// </summary>
        protected DynamicSeriesWithAxesBase()
        {
        }

        /// <summary>
        /// Calls GetNumericRange with a null axis parameter.
        /// </summary>
        [TestMethod]
        [Description("Calls GetNumericRange with a null axis parameter.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNumericRangeWithNullAxis()
        {
            DynamicSeriesWithAxes series = DynamicSeriesWithAxesToTest;
            series.GetNumericRange(null);
        }

        /// <summary>
        /// Calls GetDateTimeRange with a null axis parameter.
        /// </summary>
        [TestMethod]
        [Description("Calls GetDateTimeRange with a null axis parameter.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDateTimeRangeWithNullAxis()
        {
            DynamicSeriesWithAxes series = DynamicSeriesWithAxesToTest;
            series.GetDateTimeRange(null);
        }

        /// <summary>
        /// Calls GetCategories with a null axis parameter.
        /// </summary>
        [TestMethod]
        [Description("Calls GetCategories with a null axis parameter.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetCategoriesWithNullAxis()
        {
            DynamicSeriesWithAxes series = DynamicSeriesWithAxesToTest;
            series.GetCategories(null);
        }

        /// <summary>
        /// Attempt to plot date time on dependent axis throws exception.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Attempt to plot date time on dependent axis throws exception.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UseDateTimeValueAsDependentValue()
        {
            Chart chart = new Chart();
            DynamicSeriesWithAxes series = DynamicSeriesWithAxesToTest;
            DateTime[] dateTimes = new[] { DateTime.Now };
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => series.ItemsSource = dateTimes);
        }
    }
}