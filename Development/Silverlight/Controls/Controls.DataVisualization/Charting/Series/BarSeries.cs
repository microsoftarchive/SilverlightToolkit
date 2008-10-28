// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control that contains a data series to be rendered in bar format.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(BarDataPoint))]
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    public sealed partial class BarSeries : DynamicSingleSeriesWithAxes
    {
        /// <summary>
        /// Initializes a new instance of the BarSeries class.
        /// </summary>
        public BarSeries()
        {
        }

        /// <summary>
        /// Acquire a horizontal category axis and a vertical linear axis.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected override void GetAxes(DataPoint firstDataPoint)
        {
            GetCategoryAxis(
                AxisOrientation.Vertical,
                (axis) => { },
                () => IndependentAxis,
                (value) => { IndependentAxis = value; });

            GetRangeAxis(
                firstDataPoint,
                AxisOrientation.Horizontal,
                (axis) => 
                { 
                    axis.ShowGridLines = true; 
                    axis.ShouldIncludeZero = true; 
                },
                () => DependentAxis,
                (value) => { DependentAxis = value; },
                (dataPoint) => dataPoint.DependentValue,
                Properties.Resources.Series_DependentValueMustEitherBeANumericValueOrADateTime);
        }

        /// <summary>
        /// Returns the style to use for all data points.
        /// </summary>
        /// <returns>The style to use for all data points.</returns>
        protected override Style GetDataPointStyleFromHost()
        {
            return SeriesHost.TakeNextApplicableStyle(typeof(BarDataPoint), true);
        }

        /// <summary>
        /// Creates the bar data point.
        /// </summary>
        /// <returns>A bar data point.</returns>
        protected override DataPoint CreateDataPoint()
        {
            return new BarDataPoint();
        }

        /// <summary>
        /// Redraw other bar series when removed from a series host.
        /// </summary>
        /// <param name="oldValue">The old value of the series host property.
        /// </param>
        /// <param name="newValue">The new value of the series host property.
        /// </param>
        protected override void OnSeriesHostPropertyChanged(ISeriesHost oldValue, ISeriesHost newValue)
        {
            base.OnSeriesHostPropertyChanged(oldValue, newValue);

            // If being removed from series host, redraw all bar series.
            if (newValue == null || oldValue != null)
            {
                RedrawOtherBarSeries(oldValue);
            }
        }

        /// <summary>
        /// Redraws all other bar series when data points have been loaded.
        /// </summary>
        protected override void OnDataPointsLoaded()
        {
            base.OnDataPointsLoaded();

            if (this.SeriesHost != null)
            {
                RedrawOtherBarSeries(this.SeriesHost);
            }
        }

        /// <summary>
        /// Redraws other bar series to assure they allocate the right amount
        /// of space for their bars.
        /// </summary>
        /// <param name="seriesHost">The series host to update.</param>
        private void RedrawOtherBarSeries(ISeriesHost seriesHost)
        {
            // redraw all other bar series to ensure they make space for new one
            foreach (BarSeries series in seriesHost.Series.OfType<BarSeries>().Where(series => series != this))
            {
                series.UpdateAllDataPoints();
            }
        }

        /// <summary>
        /// Updates each point.
        /// </summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            if (SeriesHost == null || PlotArea == null || IndependentAxis == null || DependentAxis == null)
            {
                return;
            }

            Range<double> coordinateRange =
                IndependentAxis.GetPlotAreaCoordinateRange(dataPoint.IndependentValue ?? (this.ActiveDataPoints.IndexOf(dataPoint) + 1));
            if (coordinateRange.IsEmpty)
            {
                return;
            }

            IEnumerable<BarSeries> barSeries = SeriesHost.Series.OfType<BarSeries>();
            int numberOfSeries = barSeries.Count();
            double coordinateRangeHeight = (coordinateRange.Maximum - coordinateRange.Minimum);
            double segmentHeight = coordinateRangeHeight * 0.8;
            double columnHeight = segmentHeight / numberOfSeries;
            int seriesIndex = barSeries.IndexOf(this);

            double dataPointX = DependentAxis.GetPlotAreaCoordinate(dataPoint.ActualDependentValue);
            double zeroPointX = DependentAxis.GetPlotAreaCoordinate(0);

            double offset = seriesIndex * Math.Round(columnHeight) + coordinateRangeHeight * 0.1;
            double dataPointY = coordinateRange.Minimum + offset;

            if (ValueHelper.CanGraph(dataPointX) && ValueHelper.CanGraph(dataPointY) && ValueHelper.CanGraph(zeroPointX))
            {
                // Set the complete position
                dataPoint.Height = Math.Round(columnHeight);
                dataPoint.Width =
                    Math.Round(Math.Abs(dataPointX - zeroPointX));

                Canvas.SetTop(
                    dataPoint,
                    Math.Round(dataPointY));

                Canvas.SetLeft(
                    dataPoint,
                    Math.Round(Math.Min(dataPointX, zeroPointX)));
            }
        }
    }
}