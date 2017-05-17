// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the PieSeries class.
    /// </summary>
    [TestClass]
    public partial class PieSeriesTest : DataPointSeriesBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new PieSeries(); }
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Description("Verifies the initial values of all properties.")]
        public override void InitialValues()
        {
            PieSeries series = DefaultSeriesToTest as PieSeries;
            Assert.IsNull(series.Palette);
            base.InitialValues();
        }

        /// <summary>
        /// Creates a ISeriesHost with a PieSeries containing one double.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a PieSeries containing one double.")]
        public void PieSeriesWithOneDouble()
        {
            KeyValuePair<string, double>[] objects = new KeyValuePair<string, double>[]
                {
                    new KeyValuePair<string, double>("A", 90000.0)
                };
            Chart chart = new Chart();
            DataPointSeries pieSeries = DefaultSeriesToTest;
            chart.Series.Add(pieSeries);
            pieSeries.IndependentValueBinding = new Binding("Key");
            pieSeries.DependentValueBinding = new Binding("Value");
            TestAsync(
                chart,
                () => pieSeries.ItemsSource = objects,
                () => Assert.AreEqual(1, chart.LegendItems.Cast<object>().Count()),
                () =>
                {
                    IList<PieDataPoint> pieDataPoints = ChartTestUtilities.GetDataPointsForSeries(pieSeries).Cast<PieDataPoint>().ToList();
                    Assert.AreEqual(1, pieDataPoints.Count);
                    PieDataPoint pieDataPoint = pieDataPoints[0];
                    Assert.IsNotNull(pieDataPoint.Geometry);
                    Assert.AreSame(typeof(EllipseGeometry), pieDataPoint.Geometry.GetType());
                    Assert.IsNotNull(pieDataPoint.GeometryHighlight);
                    Assert.AreSame(typeof(EllipseGeometry), pieDataPoint.GeometryHighlight.GetType());
                    Assert.IsNotNull(pieDataPoint.GeometrySelection);
                    Assert.AreSame(typeof(EllipseGeometry), pieDataPoint.GeometrySelection.GetType());
                });
        }

        /// <summary>
        /// Creates a ISeriesHost with a PieSeries containing two doubles.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a PieSeries containing two doubles.")]
        public void PieSeriesWithTwoDoubles()
        {
            KeyValuePair<string, double>[] objects = new KeyValuePair<string, double>[]
                {
                    new KeyValuePair<string, double>("A", 90000.0),
                    new KeyValuePair<string, double>("B", 80000.0),
                };
            Chart chart = new Chart();
            DataPointSeries pieSeries = DefaultSeriesToTest;
            chart.Series.Add(pieSeries);
            pieSeries.IndependentValueBinding = new Binding("Key");
            pieSeries.DependentValueBinding = new Binding("Value");
            TestAsync(
                chart,
                () => pieSeries.ItemsSource = objects,
                () => Assert.AreEqual(2, chart.LegendItems.Cast<object>().Count()),
                () =>
                {
                    IList<PieDataPoint> pieDataPoints = ChartTestUtilities.GetDataPointsForSeries(pieSeries).Cast<PieDataPoint>().ToList();
                    Assert.AreEqual(2, pieDataPoints.Count);
                    foreach (PieDataPoint pieDataPoint in pieDataPoints)
                    {
                        Assert.IsNotNull(pieDataPoint.Geometry);
                        Assert.AreSame(typeof(PathGeometry), pieDataPoint.Geometry.GetType());
                        Assert.IsNotNull(pieDataPoint.GeometryHighlight);
                        Assert.AreSame(typeof(PathGeometry), pieDataPoint.GeometryHighlight.GetType());
                        Assert.IsNotNull(pieDataPoint.GeometrySelection);
                        Assert.AreSame(typeof(PathGeometry), pieDataPoint.GeometrySelection.GetType());
                    }
                });
        }

        /// <summary>
        /// Creates a ISeriesHost with a PieSeries containing three integers.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a PieSeries containing three integers.")]
        [Priority(0)]
        public void PieSeriesWithThreeIntegers()
        {
            KeyValuePair<string, int>[] objects = new KeyValuePair<string, int>[]
                {
                    new KeyValuePair<string, int>("A", 700),
                    new KeyValuePair<string, int>("B", 800),
                    new KeyValuePair<string, int>("C", 900),
                };
            Chart chart = new Chart();
            DataPointSeries pieSeries = DefaultSeriesToTest;
            chart.Series.Add(pieSeries);
            pieSeries.IndependentValueBinding = new Binding("Key");
            pieSeries.DependentValueBinding = new Binding("Value");
            TestAsync(
                chart,
                () => pieSeries.ItemsSource = objects,
                () => Assert.AreEqual(3, chart.LegendItems.Cast<object>().Count()),
                () => Assert.AreEqual(3, ChartTestUtilities.GetDataPointsForSeries(pieSeries).Count));
        }

        /// <summary>
        /// Creates a ISeriesHost with a PieSeries containing one double with value 0.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a PieSeries containing one double with value 0.")]
        public void PieSeriesWithOneDoubleValueZero()
        {
            KeyValuePair<string, double>[] objects = new KeyValuePair<string, double>[]
                {
                    new KeyValuePair<string, double>("A", 0)
                };
            Chart chart = new Chart();
            DataPointSeries pieSeries = DefaultSeriesToTest;
            chart.Series.Add(pieSeries);
            pieSeries.IndependentValueBinding = new Binding("Key");
            pieSeries.DependentValueBinding = new Binding("Value");
            TestAsync(
                chart,
                () => pieSeries.ItemsSource = objects,
                () => Assert.AreEqual(1, chart.LegendItems.Cast<object>().Count()),
                () =>
                {
                    IList<PieDataPoint> pieDataPoints = ChartTestUtilities.GetDataPointsForSeries(pieSeries).Cast<PieDataPoint>().ToList();
                    Assert.AreEqual(1, pieDataPoints.Count);
                    Assert.IsNull(pieDataPoints[0].Geometry);
                });
        }

        /// <summary>
        /// Creates a ISeriesHost with a PieSeries containing one double with value NaN.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Creates a Chart with a PieSeries containing one double with value NaN.")]
        public void PieSeriesWithOneDoubleValueNan()
        {
            KeyValuePair<string, double>[] objects = new KeyValuePair<string, double>[]
                {
                    new KeyValuePair<string, double>("A", double.NaN)
                };
            Chart chart = new Chart();
            DataPointSeries pieSeries = DefaultSeriesToTest;
            chart.Series.Add(pieSeries);
            pieSeries.IndependentValueBinding = new Binding("Key");
            pieSeries.DependentValueBinding = new Binding("Value");
            TestAsync(
                chart,
                () => pieSeries.ItemsSource = objects,
                () => Assert.AreEqual(1, chart.LegendItems.Cast<object>().Count()),
                () =>
                {
                    IList<PieDataPoint> pieDataPoints = ChartTestUtilities.GetDataPointsForSeries(pieSeries).Cast<PieDataPoint>().ToList();
                    Assert.AreEqual(1, pieDataPoints.Count);
                    Assert.IsNull(pieDataPoints[0].Geometry);
                });
        }

        /// <summary>
        /// Changes the value of a single data item in a PieSeries to zero.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the value of a single data item in a PieSeries to zero.")]
        [Bug("532273: PieSeries crashes when single point dynamically changes to value 0", Fixed = true)]
        public void PieSeriesWithOneDoubleChangesValueToZero()
        {
            ObservableCollection<int> objects = new ObservableCollection<int>();
            objects.Add(5);
            Chart chart = new Chart();
            DataPointSeries pieSeries = DefaultSeriesToTest;
            pieSeries.ItemsSource = objects;
            chart.Series.Add(pieSeries);
            TestAsync(
                chart,
                () => Assert.AreEqual(1, ChartTestUtilities.GetDataPointsForSeries(pieSeries).Count()),
                () => objects[0] = 0,
                () => Assert.AreEqual(1, ChartTestUtilities.GetDataPointsForSeries(pieSeries).Count()));
        }

        /// <summary>
        /// Sets the Series Styles property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Sets the Series Styles property.")]
        public void SetSeriesStylesProperty()
        {
            Chart chart = new Chart();
            PieSeries series = DefaultSeriesToTest as PieSeries;
            series.IndependentValueBinding = new Binding();
            series.ItemsSource = new int[] { 4, 5, 6 };
            ObservableCollection<ResourceDictionary> styles = new ObservableCollection<ResourceDictionary>();
            styles.Add(new ResourceDictionary());
            styles.Add(new ResourceDictionary());
            series.Palette = styles;
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => series.Palette.RemoveAt(0));
        }

        /// <summary>
        /// Verify the Control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the Control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(2, properties.Count);
            Assert.AreEqual(typeof(PieDataPoint), properties["DataPointStyle"]);
            Assert.AreEqual(typeof(LegendItem), properties["LegendItemStyle"]);
        }

        /// <summary>
        /// Checks that LegendItems use 1..N numbering when IndependentValueBinding is not set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Checks that LegendItems use 1..N numbering when IndependentValueBinding is not set.")]
        [Bug("521288: PieSeries Legend Titles are inconsistent with Excel's behavior", Fixed = true)]
        public void LegendTitlesWhenIndependentValueBindingUnset()
        {
            Chart chart = new Chart();
            PieSeries series = DefaultControlToTest as PieSeries;
            series.ItemsSource = new int[] { 10, 11, 12 };
            chart.Series.Add(series);
            TestAsync(
                chart,
                () =>
                {
                    int i = 1;
                    foreach (var legendItem in ChartTestUtilities.GetLegend(chart).Items.OfType<LegendItem>())
                    {
                        Assert.AreEqual(i, legendItem.Content);
                        i++;
                    }
                });
        }

        /// <summary>
        /// Verifies that the legend text updates immediately when a slice's IndependentValueBinding changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the legend text updates immediately when a slice's IndependentValueBinding changes.")]
        [Bug("536234: Dynamic changes to IndependentValue of PieDataPoint are not reflected in the Legend (until another DataPoint is added; which may never happen)", Fixed = true)]
        public void LegendContentUpdatesImmediately()
        {
            Chart chart = new Chart();
            PieSeries series = DefaultControlToTest as PieSeries;
            series.DependentValueBinding = new Binding("Value");
            series.IndependentValueBinding = new Binding("Value");
            NotifyingDataObject<int> notifyingDataObject = new NotifyingDataObject<int> { Value = 5 };
            series.ItemsSource = new NotifyingDataObject<int>[] { notifyingDataObject };
            chart.Series.Add(series);
            TestAsync(
                chart,
                () =>
                {
                    LegendItem legendItem = ChartTestUtilities.GetLegend(chart).Items.OfType<LegendItem>().First();
                    Assert.AreEqual(5, legendItem.Content);
                },
                () => notifyingDataObject.Value = 10,
                () =>
                {
                    LegendItem legendItem = ChartTestUtilities.GetLegend(chart).Items.OfType<LegendItem>().First();
                    Assert.AreEqual(10, legendItem.Content);
                });
        }

        /// <summary>
        /// Verifies the DataPoint DataContext of the LegendItem is set to the data object.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies the DataPoint DataContext of the LegendItem is set to the data object.")]
        public void LegendItemDataPointHasDataContextSet()
        {
            Chart chart = new Chart();
            PieSeries series = DefaultControlToTest as PieSeries;
            series.DependentValueBinding = new Binding("Value");
            series.IndependentValueBinding = new Binding("Value");
            NotifyingDataObject<int> notifyingDataObjectA = new NotifyingDataObject<int> { Value = 5 };
            NotifyingDataObject<int> notifyingDataObjectB = new NotifyingDataObject<int> { Value = 7 };
            NotifyingDataObject<int>[] notifyingDataObjects = new NotifyingDataObject<int>[] { notifyingDataObjectA, notifyingDataObjectB };
            series.ItemsSource = notifyingDataObjects;
            chart.Series.Add(series);
            TestAsync(
                chart,
                () =>
                {
                    LegendItem[] legendItems = ChartTestUtilities.GetLegend(chart).Items.OfType<LegendItem>().ToArray();
                    Assert.AreEqual(notifyingDataObjects.Length, legendItems.Length);
                    for (int i = 0; i < notifyingDataObjects.Length; i++)
                    {
                        PieDataPoint legendItemDataPoint = legendItems[i].DataContext as PieDataPoint;
                        Assert.IsNotNull(legendItemDataPoint);
                        Assert.AreEqual(notifyingDataObjects[i], legendItemDataPoint.DataContext);
                    }
                });
        }
    }
}