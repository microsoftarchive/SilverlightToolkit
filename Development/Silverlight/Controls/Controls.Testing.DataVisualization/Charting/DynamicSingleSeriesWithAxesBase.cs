// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for children of the DynamicSingleSeriesWithAxesBase class.
    /// </summary>
    public abstract partial class DynamicSingleSeriesWithAxesBase : DynamicSeriesBase
    {
        /// <summary>
        /// Gets a default instance of DynamicSingleSeriesWithAxes (or a derived type) to test.
        /// </summary>
        public DynamicSingleSeriesWithAxes DynamicSingleSeriesWithAxesToTest
        {
            get
            {
                return DefaultSeriesToTest as DynamicSingleSeriesWithAxes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DynamicSingleSeriesWithAxesBase class.
        /// </summary>
        protected DynamicSingleSeriesWithAxesBase()
        {
        }

        /// <summary>
        /// Creates a ISeriesHost with two Series and verifies their Titles.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with two Series and verifies their Titles.")]
        public void AutomaticSeriesTitle()
        {
            Series series1 = DefaultSeriesToTest;
            Series series2 = DefaultSeriesToTest;
            Chart chart = new Chart();
            TestAsync(
                chart,
                () => chart.Series.Add(series1),
                () => chart.Series.Add(series2),
                () => Assert.AreEqual("Series 1", series1.Title),
                () => Assert.AreEqual("Series 2", series2.Title));
        }

        /// <summary>
        /// Creates a ISeriesHost with a Series that has its Title set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Series that has its Title set.")]
        public void AutomaticSeriesTitleNotApplied()
        {
            DynamicSeries series = DefaultSeriesToTest;
            string title = "Custom Series Title";
            series.Title = title;
            Chart chart = new Chart();
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => Assert.AreSame(title, series.Title));
        }

        /// <summary>
        /// Creates a ISeriesHost with a Series that has its Title unset, then set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Series that has its Title unset, then set.")]
        public void AutomaticSeriesTitleBecomesApplied()
        {
            DynamicSeries series = DefaultSeriesToTest;
            string title = "Custom Series Title";
            Chart chart = new Chart();
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => Assert.AreEqual("Series 1", series.Title),
                () => series.Title = title,
                () => Assert.AreSame(title, series.Title));
        }

        /// <summary>
        /// Creates a ISeriesHost with two Series and verifies their automatic Titles after changing the order.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with two Series and verifies their automatic Titles after changing the order.")]
        public void AutomaticSeriesTitleAfterOrderChange()
        {
            Series series1 = DefaultSeriesToTest;
            Series series2 = DefaultSeriesToTest;
            Chart chart = new Chart();
            TestAsync(
                chart,
                () => chart.Series.Add(series1),
                () => chart.Series.Add(series2),
                () => Assert.AreEqual("Series 1", series1.Title),
                () => Assert.AreEqual("Series 2", series2.Title),
                () => chart.Series.Remove(series1),
                () => chart.Series.Add(series1),
                () => Assert.AreEqual("Series 1", series2.Title),
                () => Assert.AreEqual("Series 2", series1.Title));
        }
        
        /// <summary>
        /// Creates a ISeriesHost with a Series that has its Title set, then unset.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Series that has its Title set, then unset.")]
        public void AutomaticSeriesTitleRemoved()
        {
            DynamicSeries series = DefaultSeriesToTest;
            Chart chart = new Chart();
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => Assert.AreEqual("Series 1", series.Title),
                () => chart.Series.Remove(series),
                () => Assert.IsNull(series.Title));
        }
    }
}

