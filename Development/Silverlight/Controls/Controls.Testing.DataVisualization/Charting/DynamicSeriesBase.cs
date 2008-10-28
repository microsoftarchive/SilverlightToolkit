// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for children of the DynamicSeries class.
    /// </summary>
    public abstract partial class DynamicSeriesBase : SeriesBase
    {
        /// <summary>
        /// Gets a default instance of DynamicSeries (or a derived type) to test.
        /// </summary>
        public DynamicSeries DefaultDynamicSeriesToTest
        {
            get
            {
                return DefaultSeriesToTest as DynamicSeries;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DynamicSeriesBase class.
        /// </summary>
        protected DynamicSeriesBase()
        {
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Description("Verifies the initial values of all properties.")]
        public virtual void InitialValues()
        {
            DynamicSeries series = DefaultSeriesToTest as DynamicSeries;
            Assert.IsNull(series.Title);
            Assert.IsNull(series.ItemsSource);
            Assert.IsNull(series.SelectedItem);
            Assert.AreEqual(500, series.TransitionDuration.TotalMilliseconds);
        }

        /// <summary>
        /// Creates a ISeriesHost with a DynamicSeries and animates the DataPoints with the Simultaneous sequence.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DynamicSeries and animates the DataPoints with the Simultaneous sequence.")]
        public void DynamicSeriesAnimationSimultaneous()
        {
            Chart chart = new Chart();
            DynamicSeries series = DefaultDynamicSeriesToTest;
            series.IndependentValueBinding = new Binding();
            series.AnimationSequence = AnimationSequence.Simultaneous;
            TestAsync(
                chart,
                () => series.ItemsSource = new int[] { 1, 2, 3 },
                () => chart.Series.Add(series),
                () => series.Refresh());
        }

        /// <summary>
        /// Creates a ISeriesHost with a DynamicSeries and animates the DataPoints with the FirstToLast sequence.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DynamicSeries and animates the DataPoints with the FirstToLast sequence.")]
        public void DynamicSeriesAnimationFirstToLast()
        {
            Chart chart = new Chart();
            DynamicSeries series = DefaultDynamicSeriesToTest;
            series.IndependentValueBinding = new Binding();
            series.AnimationSequence = AnimationSequence.FirstToLast;
            TestAsync(
                chart,
                () => series.ItemsSource = new int[] { 1, 2, 3 },
                () => chart.Series.Add(series),
                () => series.Refresh());
        }

        /// <summary>
        /// Creates a ISeriesHost with a DynamicSeries and animates the DataPoints with the LastToFirst sequence.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DynamicSeries and animates the DataPoints with the LastToFirst sequence.")]
        public void DynamicSeriesAnimationLastToFirst()
        {
            Chart chart = new Chart();
            DynamicSeries series = DefaultDynamicSeriesToTest;
            series.IndependentValueBinding = new Binding();
            series.AnimationSequence = AnimationSequence.LastToFirst;
            TestAsync(
                chart,
                () => series.ItemsSource = new int[] { 1, 2, 3 },
                () => chart.Series.Add(series),
                () => series.Refresh());
        }
    }
}