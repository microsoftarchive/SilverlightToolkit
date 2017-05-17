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
using System.Windows.Markup;
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

        /// <summary>
        /// Represents a "bare" Chart Template with no margins, Legend, or other chrome.
        /// </summary>
        private const string BareChartTemplate =
            @"<ControlTemplate
                xmlns=""http://schemas.microsoft.com/client/2007""
                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                xmlns:charting=""clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit""
                xmlns:chartingprimitives=""clr-namespace:System.Windows.Controls.DataVisualization.Charting.Primitives;assembly=System.Windows.Controls.DataVisualization.Toolkit""
                TargetType=""charting:Chart"">
                <chartingprimitives:EdgePanel x:Name=""ChartArea"">
                    <Grid/>
                </chartingprimitives:EdgePanel>
            </ControlTemplate>";

        /// <summary>
        /// Makes a sequence of gradual size changes to a Chart, going down to 0x0 and back again.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Makes a sequence of gradual size changes to a Chart, going down to 0x0 and back again.")]
        public void GradualChartSizeChanges()
        {
            Chart chart = new Chart { Template = (ControlTemplate)XamlReader.Load(BareChartTemplate) };
            DataPointSeries series = DefaultDataPointSeriesToTest;
            series.IndependentValueBinding = new Binding();
            series.ItemsSource = new int[] { 1, 2, 3 };
            chart.Series.Add(series);
            EnqueueCallback(() => TestPanel.Children.Add(chart));
            for (int i = 400; 0 <= i; i--)
            {
                var capture = i;
                EnqueueCallback(() => chart.Width = capture);
                EnqueueCallback(() => chart.Height = capture / 2);
            }
            for (int i = 1; i < 200; i++)
            {
                var capture = i;
                EnqueueCallback(() => chart.Width = capture);
                EnqueueCallback(() => chart.Height = capture / 2);
            }
            EnqueueTestComplete();
        }

        /// <summary>
        /// Makes a sequence of pseudo-random size changes to a Chart.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Makes a sequence of pseudo-random size changes to a Chart.")]
        public void RandomChartSizeChanges()
        {
            Chart chart = new Chart { Template = (ControlTemplate)XamlReader.Load(BareChartTemplate) };
            DataPointSeries series = DefaultDataPointSeriesToTest;
            series.IndependentValueBinding = new Binding();
            series.ItemsSource = new int[] { 1, 2, 3 };
            chart.Series.Add(series);
            Random rand = new Random(0);
            EnqueueCallback(() => TestPanel.Children.Add(chart));
            for (int i = 0; i < 100; i++)
            {
                EnqueueCallback(() =>
                {
                    chart.Width = rand.Next(0, 400);
                    chart.Height = rand.Next(0, 400);
                });
            }
            EnqueueTestComplete();
        }

        /// <summary>
        /// Verifies that clearing the Series collection with an unused Series works.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that clearing the Series collection with an unused Series works.")]
        public void ClearAnUnusedSeries()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultDataPointSeriesToTest;
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => chart.Series.Clear());
        }

        /// <summary>
        /// Verifies that item selection is correct in the presence of data object hash code collision.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that item selection is correct in the presence of data object hash code collision.")]
        public void SelectWithHashCollision()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultDataPointSeriesToTest;
            series.DependentValueBinding = new Binding("X");
            series.IndependentValueBinding = new Binding("Y");
            Point[] items = new Point[] { new Point(1, 2), new Point(2, 1) };
            series.ItemsSource = items;
            series.IsSelectionEnabled = true;
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => series.SelectedItem = items[0],
                () => ChartTestUtilities.GetDataPointsForSeries(series).Where(dp => items[0] == (Point)dp.DataContext).Single().IsSelectionEnabled = false,
                () => Assert.AreEqual(null, series.SelectedItem),
                () => series.SelectedItem = null,
                () => series.SelectedItem = items[1],
                () => ChartTestUtilities.GetDataPointsForSeries(series).Where(dp => items[1] == (Point)dp.DataContext).Single().IsSelectionEnabled = false,
                () => Assert.AreEqual(null, series.SelectedItem));
        }

        /// <summary>
        /// Contains a simple DataPointStyle without VSM states or a complex element tree.
        /// </summary>
        private const string SimpleDataPointStyle =
            @"<Style
                xmlns=""http://schemas.microsoft.com/client/2007""
                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                xmlns:charting=""clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit""
                TargetType=""charting:DataPoint"">
                <Setter Property=""Template"">
                    <Setter.Value>
                        <ControlTemplate TargetType=""charting:DataPoint"">
                            <Grid Background=""{TemplateBinding Background}""/>
                        </ControlTemplate>
                    </Setter.Value>
                </Setter>
            </Style>";

        /// <summary>
        /// Changes the state of DataPoint objects that do not define any VSM states.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the state of DataPoint objects that do not define any VSM states.")]
        [Bug("535616: DataPoint should skip animations if none are found and set state directly", Fixed = true)]
        [Bug("557085: Exception from UpdatePointsCollection when removing all points on LineSeries (axis refactoring consequence)", Fixed = true)]
        [Bug("532925: Datapoints are not removed after a refresh", Fixed = true)]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vsm", Justification = "Standard abbreviation of Visual State Manager")]
        [Priority(0)]
        public void ChangeStateOfDataPointsWithoutVsmStates()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultDataPointSeriesToTest;
            series.IndependentValueBinding = new Binding();
            ObservableCollection<int> itemsSource = new ObservableCollection<int> { 1, 2, 3 };
            series.ItemsSource = itemsSource;
            series.DataPointStyle = (Style)XamlReader.Load(SimpleDataPointStyle);
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(3, ChartTestUtilities.GetDataPointsForSeries(series).Count),
                () => itemsSource.Clear(),
                () => Assert.AreEqual(0, ChartTestUtilities.GetDataPointsForSeries(series).Count));
        }

        /// <summary>
        /// Verifies that removing and updating values in the items source results in correct behavior.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that removing and updating values in the items source results in correct behavior.")]
        public void RemoveAndUpdateCollectionValues()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultDataPointSeriesToTest;
            series.IndependentValueBinding = new Binding();
            ObservableCollection<int> itemsSource = new ObservableCollection<int> { 1, 2, 3, 4 };
            series.ItemsSource = itemsSource;
            series.DataPointStyle = (Style)XamlReader.Load(SimpleDataPointStyle);
            chart.Series.Add(series);
            TestAsync(
                chart,
                () => Assert.AreEqual(4, ChartTestUtilities.GetDataPointsForSeries(series).Count),
                () => itemsSource.RemoveAt(3),
                () => Assert.AreEqual(3, ChartTestUtilities.GetDataPointsForSeries(series).Count),
                () => itemsSource[2] = 4,
                () => itemsSource.RemoveAt(2),
                () => Assert.AreEqual(2, ChartTestUtilities.GetDataPointsForSeries(series).Count));
        }
    }
}