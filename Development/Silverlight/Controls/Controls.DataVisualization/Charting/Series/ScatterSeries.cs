// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control that contains a data series to be rendered in X/Y scatter format.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(ScatterDataPoint))]
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    public sealed partial class ScatterSeries : DynamicSingleSeriesWithAxes
    {
        /// <summary>
        /// Gets or sets the height of the marker objects that follow the data 
        /// points.
        /// </summary>
        public double MarkerHeight
        {
            get { return (double)GetValue(MarkerHeightProperty); }
            set { SetValue(MarkerHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the MarkerHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerHeightProperty =
            DependencyProperty.Register(
                "MarkerHeight",
                typeof(double),
                typeof(ScatterSeries),
                new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the width of the marker objects that follow the data 
        /// points.
        /// </summary>
        public double MarkerWidth
        {
            get { return (double)GetValue(MarkerWidthProperty); }
            set { SetValue(MarkerWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the MarkerWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerWidthProperty =
            DependencyProperty.Register(
                "MarkerWidth",
                typeof(double),
                typeof(ScatterSeries),
                new PropertyMetadata(double.NaN));
        
        /// <summary>
        /// Initializes a new instance of the ScatterSeries class.
        /// </summary>
        public ScatterSeries()
        {
            this.DefaultStyleKey = typeof(ScatterSeries);
        }

        /// <summary>
        /// Acquire a horizontal linear axis and a vertical linear axis.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected override void GetAxes(DataPoint firstDataPoint)
        {
            GetRangeAxis(
                firstDataPoint,
                AxisOrientation.Vertical,
                (axis) => { axis.ShowGridLines = true; },
                () => DependentAxis,
                (value) => { DependentAxis = value; },
                (dataPoint) => dataPoint.DependentValue,
                Properties.Resources.Series_DependentValueMustEitherBeANumericValueOrADateTime);

            GetRangeAxis(
                firstDataPoint,
                AxisOrientation.Horizontal,
                (axis) => 
                {
                },
                () => IndependentAxis,
                (value) => { IndependentAxis = value; },
                (dataPoint) => dataPoint.IndependentValue,
                Properties.Resources.Series_IndependentValueMustEitherBeANumericValueOrADateTime);
        }

        /// <summary>
        /// Creates a new scatter data point.
        /// </summary>
        /// <returns>A scatter data point.</returns>
        protected override DataPoint CreateDataPoint()
        {
            return new ScatterDataPoint();
        }

        /// <summary>
        /// Returns the style to use for all data points.
        /// </summary>
        /// <returns>The style to use for all data points.</returns>
        protected override Style GetDataPointStyleFromHost()
        {
            return SeriesHost.TakeNextApplicableStyle(typeof(ScatterDataPoint), true);
        }

        /// <summary>
        /// This method updates a single data point.
        /// </summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            double PlotAreaHeight = DependentAxis.GetPlotAreaCoordinate(DependentAxis.ActualMaximum);
            double dataPointX = IndependentAxis.GetPlotAreaCoordinate(dataPoint.ActualIndependentValue);
            double dataPointY = DependentAxis.GetPlotAreaCoordinate(dataPoint.ActualDependentValue);

            if (ValueHelper.CanGraph(dataPointX) && ValueHelper.CanGraph(dataPointY))
            {
                // Set dimensions
                if (!double.IsNaN(MarkerHeight))
                {
                    dataPoint.Height = MarkerHeight;
                }

                if (!double.IsNaN(MarkerWidth))
                {
                    dataPoint.Width = MarkerWidth;
                }

                // Set the Position
                Canvas.SetLeft(
                    dataPoint,
                    Math.Round(dataPointX - (dataPoint.ActualWidth / 2)));
                Canvas.SetTop(
                    dataPoint,
                    Math.Round(PlotAreaHeight - (dataPointY + (dataPoint.ActualHeight / 2))));
            }
        }
    }
}
