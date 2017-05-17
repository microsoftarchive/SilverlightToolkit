// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// This class tests the CategoryAxis.
    /// </summary>
    [TestClass]
    public class CategoryAxisTest : DisplayAxisBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new CategoryAxis(); }
        }

        /// <summary>
        /// Assigns a string to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a string to the Title property.")]
        public void TitleChangeString()
        {
            CategoryAxis axis = (CategoryAxis)DefaultControlToTest;
            string title = "String Title";
            axis.Title = title;
            Assert.AreEqual(title, axis.Title);
        }

        /// <summary>
        /// Verify the Control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the Control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(4, properties.Count);
            Assert.AreEqual(typeof(Line), properties["MajorTickMarkStyle"]);
            Assert.AreEqual(typeof(AxisLabel), properties["AxisLabelStyle"]);
            Assert.AreEqual(typeof(Title), properties["TitleStyle"]);
            Assert.AreEqual(typeof(Line), properties["GridLineStyle"]);
        }

        /// <summary>
        /// Verify CategoryAxis does not sort its categories by default.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify CategoryAxis does not sort its categories by default.")]
        [Bug("527856: Category Axis is sorting category names when it shouldn't (please preserve the original order)", Fixed = true)]
        [Priority(0)]
        public void CategoryNamesAreNotSorted()
        {
            Chart chart = new Chart();
            ColumnSeries series = new ColumnSeries();
            series.IndependentValueBinding = new Binding("Key");
            series.DependentValueBinding = new Binding("Value");
            series.ItemsSource = new KeyValuePair<string, int>[]
            {
                new KeyValuePair<string, int>("c", 3),
                new KeyValuePair<string, int>("a", 1),
                new KeyValuePair<string, int>("b", 2),
            };
            chart.Series.Add(series);
            CategoryAxis axis = null;
            TestAsync(
                chart,
                () => axis = chart.ActualAxes.OfType<CategoryAxis>().FirstOrDefault(),
                () =>
                {
                    object[] labels = ChartTestUtilities.GetAxisLabels(axis).Select(l => l.DataContext).ToArray();
                    Assert.AreEqual("c", labels[0]);
                    Assert.AreEqual("a", labels[1]);
                    Assert.AreEqual("b", labels[2]);
                });
        }

        /////// <summary>
        /////// Verifies that elements do not stick out of the bounds in a known-problematic scenario.
        /////// </summary>
        ////[TestMethod]
        ////[Asynchronous]
        ////[Description("Verifies that elements do not stick out of the bounds in a known-problematic scenario.")]
        ////[Bug("559039: Width of vertical category axis doesn't include all category names and some end up sticking outside chart", Fixed = true)]
        ////public void CategoryNamesDoNotStickOut()
        ////{
        ////    Chart chart = new Chart();
        ////    BarSeries series = new BarSeries();
        ////    series.DependentValueBinding = new Binding("Value");
        ////    series.IndependentValueBinding = new Binding("Key");
        ////    series.ItemsSource = new KeyValuePair<string, int>[]
        ////    {
        ////        new KeyValuePair<string, int>("c", 1),
        ////        new KeyValuePair<string, int>("d", 3),
        ////        new KeyValuePair<string, int>("This is a long value that sticks out", 4),
        ////    };
        ////    chart.Series.Add(series);
        ////    TestAsync(
        ////        chart,
        ////        () => Assert.AreEqual(3, ChartTestUtilities.GetDataPointsForSeries(series).Count),
        ////        () =>
        ////        {
        ////            foreach (Axis axis in chart.GetVisualDescendents().OfType<CategoryAxis>())
        ////            {
        ////                foreach (FrameworkElement element in axis.GetVisualDescendents().OfType<FrameworkElement>())
        ////                {
        ////                    GeneralTransform transform = element.TransformToVisual(axis);
        ////                    Point topLeft = transform.Transform(new Point());
        ////                    Assert.IsTrue((0 <= topLeft.X) && (topLeft.X <= axis.ActualWidth));
        ////                    Assert.IsTrue((0 <= topLeft.Y) && (topLeft.Y <= axis.ActualHeight));
        ////                    Point bottomRight = transform.Transform(new Point(element.ActualWidth, element.ActualHeight));
        ////                    Assert.IsTrue((0 <= bottomRight.X) && (bottomRight.X <= axis.ActualWidth + 1));
        ////                    Assert.IsTrue((0 <= bottomRight.Y) && (bottomRight.Y <= axis.ActualHeight + 1));
        ////                }
        ////            }
        ////        });
        ////}

        /// <summary>
        /// Verifies FontSize changes flow down to the AxisLabels.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies FontSize changes flow down to the AxisLabels.")]
        [Bug("534268: Inheritable FontSize property doesn't seem to automatically flow down from Axis to its Labels", Fixed = true)]
        public void FontSizeChangesFlowToAxisLabels()
        {
            Chart chart = new Chart();
            chart.FontSize = 22;
            ColumnSeries series = new ColumnSeries();
            series.ItemsSource = new int[] { 1, 2, 3 };
            chart.Series.Add(series);
            CategoryAxis axis = null;
            TestAsync(
                chart,
                () => axis = chart.ActualAxes.OfType<CategoryAxis>().First(),
                () =>
                {
                    foreach (AxisLabel label in ChartTestUtilities.GetAxisLabels(axis))
                    {
                        Assert.AreEqual(chart.FontSize, label.FontSize);
                    }
                },
                () => chart.FontSize = 33,
                () =>
                {
                    foreach (AxisLabel label in ChartTestUtilities.GetAxisLabels(axis))
                    {
                        Assert.AreEqual(chart.FontSize, label.FontSize);
                    }
                });
        }
    }
}