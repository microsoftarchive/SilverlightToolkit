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
    /// Represents a control that contains a data series to be rendered in column format.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(ColumnDataPoint))]
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    public sealed partial class ColumnSeries : DynamicSingleSeriesWithAxes
    {
        /// <summary>
        /// Initializes a new instance of the ColumnSeries class.
        /// </summary>
        public ColumnSeries()
        {
        }

        /// <summary>
        /// Acquire a horizontal category axis and a vertical linear axis.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected override void GetAxes(DataPoint firstDataPoint)
        {
            GetCategoryAxis(
                AxisOrientation.Horizontal,
                (axis) => { },
                () => IndependentAxis,
                (value) => { IndependentAxis = value; });

            GetRangeAxis(
                firstDataPoint,
                AxisOrientation.Vertical,
                (axis) => { axis.ShouldIncludeZero = true; axis.ShowGridLines = true; },
                () => DependentAxis,
                (value) => { DependentAxis = value; },
                (dataPoint) => dataPoint.DependentValue,
                Properties.Resources.Series_DependentValueMustEitherBeANumericValueOrADateTime);
        }

        /// <summary>
        /// Creates the column data point.
        /// </summary>
        /// <returns>A column data point.</returns>
        protected override DataPoint CreateDataPoint()
        {
            return new ColumnDataPoint();
        }

        /// <summary>
        /// Redraw other column series when removed from a series host.
        /// </summary>
        /// <param name="oldValue">The old value of the series host property.</param>
        /// <param name="newValue">The new value of the series host property.</param>
        protected override void OnSeriesHostPropertyChanged(ISeriesHost oldValue, ISeriesHost newValue)
        {
            base.OnSeriesHostPropertyChanged(oldValue, newValue);

            // If being removed from series host, redraw all column series.
            if (newValue == null || oldValue != null)
            {
                RedrawOtherColumnSeries(oldValue);
            }
        }

        /// <summary>
        /// Redraws all other column series when data points have been loaded.
        /// </summary>
        protected override void OnDataPointsLoaded()
        {
            base.OnDataPointsLoaded();

            if (this.SeriesHost != null)
            {
                RedrawOtherColumnSeries(this.SeriesHost);
            }
        }

        /// <summary>
        /// Redraws other column series to assure they allocate the right amount
        /// of space for their columns.
        /// </summary>
        /// <param name="seriesHost">The series host to update.</param>
        private void RedrawOtherColumnSeries(ISeriesHost seriesHost)
        {
            // redraw all other column series to ensure they make space for new one
            foreach (ColumnSeries series in seriesHost.Series.OfType<ColumnSeries>().Where(series => series != this))
            {
                series.UpdateAllDataPoints();
            }  
        }

        /// <summary>
        /// Updates a data point when its actual dependent value has changed.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointActualDependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            UpdateDataPoint(dataPoint);
            base.OnDataPointActualDependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Updates a data point when its actual independent value has changed.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointActualIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            UpdateDataPoint(dataPoint);
            base.OnDataPointActualIndependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Returns the style to use for all data points.
        /// </summary>
        /// <returns>The style to use for all data points.</returns>
        protected override Style GetDataPointStyleFromHost()
        {
            return SeriesHost.TakeNextApplicableStyle(typeof(ColumnDataPoint), true);
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

            double PlotAreaHeight = DependentAxis.GetPlotAreaCoordinate(DependentAxis.ActualMaximum);
            IEnumerable<ColumnSeries> columnSeries = SeriesHost.Series.OfType<ColumnSeries>();
            int numberOfSeries = columnSeries.Count();
            double coordinateRangeWidth = (coordinateRange.Maximum - coordinateRange.Minimum);
            double segmentWidth = coordinateRangeWidth * 0.8;
            double columnWidth = segmentWidth / numberOfSeries;
            int seriesIndex = columnSeries.IndexOf(this);
            
            double dataPointX = DependentAxis.GetPlotAreaCoordinate(dataPoint.ActualDependentValue);
            double zeroPointX = DependentAxis.GetPlotAreaCoordinate(0);

            double offset = seriesIndex * Math.Round(columnWidth) + coordinateRangeWidth * 0.1;
            double dataPointY = coordinateRange.Minimum + offset;

            if (!double.IsNaN(dataPointX) && !double.IsNaN(dataPointY) && !double.IsNaN(zeroPointX))
            {
                // Set the complete position
                dataPoint.Width = Math.Round(columnWidth);
                dataPoint.Height =
                    Math.Round(Math.Abs(dataPointX - zeroPointX));

                Canvas.SetLeft(
                    dataPoint,
                    Math.Round(dataPointY));

                Canvas.SetTop(
                    dataPoint,
                    Math.Round(PlotAreaHeight - Math.Max(dataPointX, zeroPointX)));
            }
        }
    }
}