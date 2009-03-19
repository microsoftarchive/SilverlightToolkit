// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for children of the DataPointSeries class.
    /// </summary>
    public abstract partial class DataPointSeriesBase : SeriesBase
    {
        /// <summary>
        /// Gets a default instance of DataPointSeries (or a derived type) to test.
        /// </summary>
        public DataPointSeries DefaultDataPointSeriesToTest
        {
            get
            {
                return DefaultSeriesToTest as DataPointSeries;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataPointSeriesBase class.
        /// </summary>
        protected DataPointSeriesBase()
        {
        }

        /// <summary>
        /// Verify the control's template parts.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template parts.")]
        public override void TemplatePartsAreDefined()
        {
            Assert.AreEqual(1, DefaultFrameworkElementToTest.GetType().GetTemplateParts().Count);
            Assert.AreSame(typeof(Canvas), DefaultFrameworkElementToTest.GetType().GetTemplateParts()["PlotArea"]);
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Description("Verifies the initial values of all properties.")]
        [Bug("530063: Charts are always hoverable/selectable and that seems a silly default - recommend adding a property IsSelectable or IsInteractive (default=false)", Fixed = true)]
        public virtual void InitialValues()
        {
            DataPointSeries series = DefaultSeriesToTest as DataPointSeries;
            Assert.IsNull(series.Title);
            Assert.IsNull(series.ItemsSource);
            Assert.IsFalse(series.IsSelectionEnabled);
            Assert.IsNull(series.SelectedItem);
            Assert.AreEqual(500, series.TransitionDuration.TotalMilliseconds);
            Assert.AreEqual(null, series.LegendItemStyle);
            Assert.IsNull(series.DependentValueBinding);
            Assert.IsNull(series.DependentValuePath);
            Assert.IsNull(series.IndependentValueBinding);
            Assert.IsNull(series.IndependentValuePath);
        }

        /// <summary>
        /// Verifies that setting DependentValueBinding and DependentValuePath results in consistent behavior.
        /// </summary>
        [TestMethod]
        [Description("Verifies that setting DependentValueBinding and DependentValuePath results in consistent behavior.")]
        public void DependentValueBindingConsistentWithDependentValuePath()
        {
            DataPointSeries series = DefaultSeriesToTest as DataPointSeries;

            string path = "Path1";
            series.DependentValueBinding = new Binding(path);
            Assert.IsNotNull(series.DependentValueBinding);
            Assert.AreEqual(path, series.DependentValueBinding.Path.Path);
            Assert.AreEqual(path, series.DependentValuePath);

            series.DependentValueBinding = null;
            Assert.IsNull(series.DependentValueBinding);
            Assert.IsNull(series.DependentValuePath);

            path = "Path2";
            series.DependentValuePath = path;
            Assert.AreEqual(path, series.DependentValuePath);
            Assert.IsNotNull(series.DependentValueBinding);
            Assert.AreEqual(path, series.DependentValueBinding.Path.Path);

            series.DependentValuePath = null;
            Assert.IsNull(series.DependentValuePath);
            Assert.IsNull(series.DependentValueBinding);

            path = "";
            series.DependentValuePath = path;
            Assert.AreEqual(path, series.DependentValuePath);
            Assert.IsNotNull(series.DependentValueBinding);
            Assert.AreEqual(path, series.DependentValueBinding.Path.Path);
        }

        /// <summary>
        /// Verifies that setting IndependentValueBinding and IndependentValuePath results in consistent behavior.
        /// </summary>
        [TestMethod]
        [Description("Verifies that setting IndependentValueBinding and IndependentValuePath results in consistent behavior.")]
        public void IndependentValueBindingConsistentWithIndependentValuePath()
        {
            DataPointSeries series = DefaultSeriesToTest as DataPointSeries;

            string path = "Path1";
            series.IndependentValueBinding = new Binding(path);
            Assert.IsNotNull(series.IndependentValueBinding);
            Assert.AreEqual(path, series.IndependentValueBinding.Path.Path);
            Assert.AreEqual(path, series.IndependentValuePath);

            series.IndependentValueBinding = null;
            Assert.IsNull(series.IndependentValueBinding);
            Assert.IsNull(series.IndependentValuePath);

            path = "Path2";
            series.IndependentValuePath = path;
            Assert.AreEqual(path, series.IndependentValuePath);
            Assert.IsNotNull(series.IndependentValueBinding);
            Assert.AreEqual(path, series.IndependentValueBinding.Path.Path);

            series.IndependentValuePath = null;
            Assert.IsNull(series.IndependentValuePath);
            Assert.IsNull(series.IndependentValueBinding);

            path = "";
            series.IndependentValuePath = path;
            Assert.AreEqual(path, series.IndependentValuePath);
            Assert.IsNotNull(series.IndependentValueBinding);
            Assert.AreEqual(path, series.IndependentValueBinding.Path.Path);
        }

        /// <summary>
        /// Creates a ISeriesHost with a DataPointSeries and animates the DataPoints with the Simultaneous sequence.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DataPointSeries and animates the DataPoints with the Simultaneous sequence.")]
        public void DataPointSeriesAnimationSimultaneous()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultDataPointSeriesToTest;
            series.IndependentValueBinding = new Binding();
            series.AnimationSequence = AnimationSequence.Simultaneous;
            TestAsync(
                chart,
                () => series.ItemsSource = new int[] { 1, 2, 3 },
                () => chart.Series.Add(series),
                () => series.Refresh());
        }

        /// <summary>
        /// Creates a ISeriesHost with a DataPointSeries and animates the DataPoints with the FirstToLast sequence.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DataPointSeries and animates the DataPoints with the FirstToLast sequence.")]
        public void DataPointSeriesAnimationFirstToLast()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultDataPointSeriesToTest;
            series.IndependentValueBinding = new Binding();
            series.AnimationSequence = AnimationSequence.FirstToLast;
            TestAsync(
                chart,
                () => series.ItemsSource = new int[] { 1, 2, 3 },
                () => chart.Series.Add(series),
                () => series.Refresh());
        }

        /// <summary>
        /// Creates a ISeriesHost with a DataPointSeries and animates the DataPoints with the LastToFirst sequence.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a DataPointSeries and animates the DataPoints with the LastToFirst sequence.")]
        public void DataPointSeriesAnimationLastToFirst()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultDataPointSeriesToTest;
            series.IndependentValueBinding = new Binding();
            series.AnimationSequence = AnimationSequence.LastToFirst;
            TestAsync(
                chart,
                () => series.ItemsSource = new int[] { 1, 2, 3 },
                () => chart.Series.Add(series),
                () => series.Refresh());
        }

        /// <summary>
        /// Verifies that setting LegendItemStyle after Title successfully applies the style.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that setting LegendItemStyle after Title successfully applies the style.")]
        [Bug("579946: Changes to Series.LegendItemStyle after Series.Title do not take effect", Fixed = true)]
        public void SettingLegendItemStyleAfterTitleWorks()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultDataPointSeriesToTest;
            series.IndependentValueBinding = new Binding();
            series.ItemsSource = new int[] { 1, 2, 3 };
            series.Title = "Title";
            chart.Series.Add(series);
            TestAsync(
                chart,
                () =>
                {
                    Style style = new Style(typeof(LegendItem));
                    style.Setters.Add(new Setter(Control.FontSizeProperty, 50.0));
                    series.LegendItemStyle = style;
                },
                () =>
                {
                    foreach (LegendItem item in chart.GetVisualDescendents().OfType<LegendItem>())
                    {
                        Assert.AreEqual(50, item.FontSize);
                    }
                });
        }

        /// <summary>
        /// Verifies that using DependentValuePath/IndependentValuePath works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that using DependentValuePath/IndependentValuePath works properly.")]
        public void UsingValuePathsWorks()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultDataPointSeriesToTest;
            series.DependentValuePath = "Value";
            series.IndependentValuePath = "Key";
            series.ItemsSource = new KeyValuePair<int, int>[] { new KeyValuePair<int, int>(1, 2), new KeyValuePair<int, int>(3, 4), new KeyValuePair<int, int>(5, 6) };
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(3, ChartTestUtilities.GetDataPointsForSeries(series).Count()));
        }

        /// <summary>
        /// Verifies that the event handler for an INotifyCollectionChanged collection won't keep the DataPointSeries alive.
        /// </summary>
        [TestMethod]
        [Description("Verifies that the event handler for an INotifyCollectionChanged collection won't keep the DataPointSeries alive.")]
        [Bug("587275: Charting should be using the WeakEvent pattern for attaching events to any user-provided object/collection", Fixed = true)]
        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Deliberately calling method to verify scenario.")]
        public void CollectionChangedHandlerDoesNotKeepSeriesAlive()
        {
            ObservableCollection<int> collection = new ObservableCollection<int>();
            WeakReference weakReference = new WeakReference(DefaultDataPointSeriesToTest);
            ((DataPointSeries)weakReference.Target).ItemsSource = collection;
            GC.Collect();
            Assert.IsNull(weakReference.Target);
        }
    }
}