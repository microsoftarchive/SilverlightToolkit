// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for children of the DataPointSingleSeriesWithAxesBase class.
    /// </summary>
    public abstract partial class DataPointSingleSeriesWithAxesBase : DataPointSeriesWithAxesBase
    {
        /// <summary>
        /// Gets a default instance of DataPointSingleSeriesWithAxes (or a derived type) to test.
        /// </summary>
        public DataPointSingleSeriesWithAxes DataPointSingleSeriesWithAxesToTest
        {
            get
            {
                return DefaultSeriesToTest as DataPointSingleSeriesWithAxes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataPointSingleSeriesWithAxesBase class.
        /// </summary>
        protected DataPointSingleSeriesWithAxesBase()
        {
        }

        /// <summary>
        /// Creates a Chart with two Series and verifies their Titles.
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
        /// Creates a Chart with a Series that has its Title set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Series that has its Title set.")]
        public void AutomaticSeriesTitleNotApplied()
        {
            DataPointSeries series = DefaultSeriesToTest;
            string title = "Custom Series Title";
            series.Title = title;
            Chart chart = new Chart();
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => Assert.AreSame(title, series.Title));
        }

        /// <summary>
        /// Creates a Chart with a Series that has its Title unset, then set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Series that has its Title unset, then set.")]
        public void AutomaticSeriesTitleBecomesApplied()
        {
            DataPointSeries series = DefaultSeriesToTest;
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
        /// Creates a Chart with two Series and verifies their automatic Titles after changing the order.
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
        /// Creates a Chart with a Series that has its Title set, then unset.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Series that has its Title set, then unset.")]
        public void AutomaticSeriesTitleRemoved()
        {
            DataPointSeries series = DefaultSeriesToTest;
            Chart chart = new Chart();
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => Assert.AreEqual("Series 1", series.Title),
                () => chart.Series.Remove(series),
                () => Assert.IsNull(series.Title));
        }

        /// <summary>
        /// Creates a Chart with a Series that has its Title set by a Binding already in place.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a Series that has its Title set by a Binding already in place.")]
        [Bug("645384: Chart series title not binding", Fixed = true)]
        public void AutomaticSeriesTitleOverriddenByInPlaceBinding()
        {
            DataPointSeries series = DefaultSeriesToTest;
            Chart chart = new Chart();
            chart.DataContext = "Custom Title";
            series.SetBinding(Series.TitleProperty, new Binding());
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => Assert.AreEqual("Custom Title", series.Title));
        }

        /// <summary>
        /// Calls GetNumericRange with a null axis parameter.
        /// </summary>
        [TestMethod]
        [Description("Calls GetNumericRange with a null axis parameter.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNumericRangeWithNullAxis()
        {
            IRangeProvider series = (IRangeProvider)DataPointSingleSeriesWithAxesToTest;
            series.GetRange(null);
        }

        /// <summary>
        /// Verifies that the LegendItem's DataPoint has had the correct Style applied.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the LegendItem's DataPoint has had the correct Style applied.")]
        [Bug("520693: Legend DataPoint representations are missing their black border for non-Pie series", Fixed = true)]
        public void VerifyLegendItemDataPointStyleApplied()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultSeriesToTest;
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.IsNotNull(ChartTestUtilities.GetLegend(chart)),
                () =>
                {
                    foreach (var legendItem in ChartTestUtilities.GetLegend(chart).Items.OfType<LegendItem>())
                    {
                        var dataPoint = legendItem.DataContext as DataPoint;
                        if (null != dataPoint)
                        {
                            Assert.AreNotEqual(typeof(SolidColorBrush), dataPoint.Background.GetType());
                        }
                    }
                });
        }
    }
}