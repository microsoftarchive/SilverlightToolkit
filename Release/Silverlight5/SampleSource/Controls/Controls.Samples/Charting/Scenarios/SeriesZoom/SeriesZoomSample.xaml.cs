// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Diagnostics;
using System.Windows.Media;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Charting sample that demonstrates zooming.
    /// </summary>
    [Sample("Zoom", DifficultyLevel.Scenario, "DataVisualization")]
    public partial class SeriesZoomSample : UserControl
    {
        /// <summary>
        /// Caching of the ChartArea template part.
        /// </summary>
        private Panel chartArea;

        /// <summary>
        /// Gets the ChartArea.
        /// </summary>
        /// <returns>TemplatePart ChartArea</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by ZoomChanged.")]
        private Panel ChartArea
        {
            get
            {
                if (chartArea == null)
                {
                    chartArea = GetLogicalChildrenBreadthFirst(ZoomChart).Where(element => element.Name.Equals("ChartArea")).FirstOrDefault() as Panel;
                }

                return chartArea;
            }
        }

        /// <summary>
        /// Caching of the ScrollArea template part.
        /// </summary>
        private ScrollViewer scrollArea;

        /// <summary>
        /// Gets the ScrollArea.
        /// </summary>
        /// <returns>TemplatePart ScrollArea</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by an event handler in XAML.")]
        private ScrollViewer ScrollArea
        {
            get
            {
                if (scrollArea == null)
                {
                    scrollArea = GetLogicalChildrenBreadthFirst(ZoomChart).Where(element => element.Name.Equals("ScrollArea")).FirstOrDefault() as ScrollViewer;
                }
                return scrollArea;
            }
        } 

        /// <summary>
        /// Initializes a new instance of the ZoomIntoChartSample class.
        /// </summary>
        public SeriesZoomSample()
        {
            InitializeComponent();

            this.Loaded += ZoomIntoChartSample_Loaded;
        }

        /// <summary>
        /// Force an update of the chart.
        /// </summary>
        /// <param name="sender">The ZoomIntoChartSample instance.</param>
        /// <param name="e">Event arguments.</param>
        private void ZoomIntoChartSample_Loaded(object sender, RoutedEventArgs e)
        {
            // force synchronous layout pass
            ZoomChart.UpdateLayout();

            // and force initial zoom 
            UpdateChart(0);
        }
        
        /// <summary>
        /// Handles the changing of the zoomlevel.
        /// </summary>
        /// <param name="sender">The zoom slider.</param>
        /// <param name="e">Event arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by an event handler in XAML.")]
        private void ZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Assert(ChartArea != null && ScrollArea != null, "Zoom should not be called before layout has occurred");

            double zoom = e.NewValue;

            UpdateChart(zoom);
        }

        /// <summary>
        /// Updates the chart to zoom with the correct zoom factor.
        /// </summary>
        /// <param name="zoom">The percentage of zoom we wish to apply.</param>
        private void UpdateChart(double zoom)
        {
            ChartArea.Width = ScrollArea.ViewportWidth + (ScrollArea.ViewportWidth * zoom / 100.0);
        }

        /// <summary>
        /// Helper function that returns a list of the visual children.
        /// </summary>
        /// <param name="parent">Element whose visual children will be returned.</param>
        /// <returns>A collection of visualchildren.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by ChartArea and ScrollArea.")]
        private IEnumerable<FrameworkElement> GetLogicalChildrenBreadthFirst(FrameworkElement parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null.");

            Queue<FrameworkElement> queue =
                new Queue<FrameworkElement>(GetVisualChildren(parent).OfType<FrameworkElement>());

            while (queue.Count > 0)
            {
                FrameworkElement element = queue.Dequeue();
                yield return element;

                foreach (FrameworkElement visualChild in GetVisualChildren(element).OfType<FrameworkElement>())
                {
                    queue.Enqueue(visualChild);
                }
            }
        }

        /// <summary>
        /// Helper function that returns the direct visual children of an element.
        /// </summary>
        /// <param name="parent">The element whose visual children will be returned.</param>
        /// <returns>A collection of visualchildren.</returns>
        private IEnumerable<DependencyObject> GetVisualChildren(DependencyObject parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null.");

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int counter = 0; counter < childCount; counter++)
            {
                yield return VisualTreeHelper.GetChild(parent, counter);
            }
        }
    }
}