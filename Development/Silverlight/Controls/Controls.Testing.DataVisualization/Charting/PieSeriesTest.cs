// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization.Charting;

namespace Microsoft.Windows.Controls.Testing
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
            Assert.IsNull(series.StylePalette);
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
            List<Style> styles = new List<Style>();
            styles.Add(new Style(typeof(Control)));
            styles.Add(new Style(typeof(DataPoint)));
            series.StylePalette = styles;
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => series.StylePalette.RemoveAt(0),
                () => chart.Refresh());
        }

        /// <summary>
        /// Verify the Control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the Control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(1, properties.Count);
            Assert.AreEqual(typeof(LegendItem), properties["LegendItemStyle"]);
        }
    }
}