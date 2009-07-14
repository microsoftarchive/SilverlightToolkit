// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control that contains a data series to be rendered in column format.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Depth of hierarchy is necessary to avoid code duplication.")]
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(ColumnDataPoint))]
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    [TemplatePart(Name = DataPointSeries.PlotAreaName, Type = typeof(Canvas))]
    public sealed partial class ColumnSeries : ColumnBarBaseSeries<ColumnDataPoint>
    {
        #region public IRangeAxis DependentRangeAxis
        /// <summary>
        /// Gets or sets the dependent range axis.
        /// </summary>
        public IRangeAxis DependentRangeAxis
        {
            get { return GetValue(DependentRangeAxisProperty) as IRangeAxis; }
            set { SetValue(DependentRangeAxisProperty, value); }
        }

        /// <summary>
        /// Identifies the DependentRangeAxis dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This member is necessary because the base classes need to share this dependency property.")]
        public static readonly DependencyProperty DependentRangeAxisProperty =
            DependencyProperty.Register(
                "DependentRangeAxis",
                typeof(IRangeAxis),
                typeof(ColumnSeries),
                new PropertyMetadata(null, OnDependentRangeAxisPropertyChanged));

        /// <summary>
        /// DependentRangeAxisProperty property changed handler.
        /// </summary>
        /// <param name="d">ColumnBarBaseSeries that changed its DependentRangeAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDependentRangeAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColumnSeries source = (ColumnSeries)d;
            IRangeAxis newValue = (IRangeAxis)e.NewValue;
            source.OnDependentRangeAxisPropertyChanged(newValue);
        }

        /// <summary>
        /// DependentRangeAxisProperty property changed handler.
        /// </summary>
        /// <param name="newValue">New value.</param>
        private void OnDependentRangeAxisPropertyChanged(IRangeAxis newValue)
        {
            this.InternalDependentAxis = (IAxis)newValue;
        }
        #endregion public IRangeAxis DependentRangeAxis

        #region public IAxis IndependentAxis
        /// <summary>
        /// Gets or sets the independent category axis.
        /// </summary>
        public IAxis IndependentAxis
        {
            get { return GetValue(IndependentAxisProperty) as IAxis; }
            set { SetValue(IndependentAxisProperty, value); }
        }

        /// <summary>
        /// Identifies the IndependentAxis dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This member is necessary because the base classes need to share this dependency property.")]
        public static readonly DependencyProperty IndependentAxisProperty =
            DependencyProperty.Register(
                "IndependentAxis",
                typeof(IAxis),
                typeof(ColumnSeries),
                new PropertyMetadata(null, OnIndependentAxisPropertyChanged));

        /// <summary>
        /// IndependentAxisProperty property changed handler.
        /// </summary>
        /// <param name="d">ColumnBarBaseSeries that changed its IndependentAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIndependentAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColumnSeries source = (ColumnSeries)d;
            IAxis newValue = (IAxis)e.NewValue;
            source.OnIndependentAxisPropertyChanged(newValue);
        }

        /// <summary>
        /// IndependentAxisProperty property changed handler.
        /// </summary>
        /// <param name="newValue">New value.</param>
        private void OnIndependentAxisPropertyChanged(IAxis newValue)
        {
            this.InternalIndependentAxis = (IAxis)newValue;
        }
        #endregion public IAxis IndependentAxis

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
            GetAxes(
                firstDataPoint,
                (axis) => axis.Orientation == AxisOrientation.X,
                () => new CategoryAxis { Orientation = AxisOrientation.X },
                (axis) =>
                {
                    IRangeAxis rangeAxis = axis as IRangeAxis;
                    return rangeAxis != null && rangeAxis.Origin != null && axis.Orientation == AxisOrientation.Y;
                },
                () =>
                {
                    IRangeAxis rangeAxis = CreateRangeAxisFromData(firstDataPoint.DependentValue);
                    rangeAxis.Orientation = AxisOrientation.Y;
                    if (rangeAxis == null || rangeAxis.Origin == null)
                    {
                        throw new InvalidOperationException(Properties.Resources.DataPointSeriesWithAxes_NoSuitableAxisAvailableForPlottingDependentValue);
                    }
                    DisplayAxis axis = rangeAxis as DisplayAxis;
                    if (axis != null)
                    {
                        axis.ShowGridLines = true;
                    }
                    return rangeAxis;
                });
        }

        /// <summary>
        /// Updates each point.
        /// </summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            if (SeriesHost == null || PlotArea == null)
            {
                return;
            }

            object category = dataPoint.ActualIndependentValue ?? (this.ActiveDataPoints.IndexOf(dataPoint) + 1);
            Range<UnitValue> coordinateRange = GetCategoryRange(category);
            if (!coordinateRange.HasData)
            {
                return;
            }
            else if (coordinateRange.Maximum.Unit != Unit.Pixels || coordinateRange.Minimum.Unit != Unit.Pixels)
            {
                throw new InvalidOperationException(Properties.Resources.DataPointSeriesWithAxes_ThisSeriesDoesNotSupportRadialAxes);
            }

            double minimum = (double)coordinateRange.Minimum.Value;
            double maximum = (double)coordinateRange.Maximum.Value;

            double plotAreaHeight = ActualDependentRangeAxis.GetPlotAreaCoordinate(ActualDependentRangeAxis.Range.Maximum).Value.Value;
            IEnumerable<ColumnSeries> columnSeries = SeriesHost.Series.OfType<ColumnSeries>().Where(series => series.ActualIndependentAxis == ActualIndependentAxis);
            int numberOfSeries = columnSeries.Count();
            double coordinateRangeWidth = (maximum - minimum);
            double segmentWidth = coordinateRangeWidth * 0.8;
            double columnWidth = segmentWidth / numberOfSeries;
            int seriesIndex = columnSeries.IndexOf(this);

            double dataPointY = ActualDependentRangeAxis.GetPlotAreaCoordinate(ValueHelper.ToDouble(dataPoint.ActualDependentValue)).Value.Value;
            double zeroPointY = ActualDependentRangeAxis.GetPlotAreaCoordinate(ActualDependentRangeAxis.Origin).Value.Value;

            double offset = seriesIndex * Math.Round(columnWidth) + coordinateRangeWidth * 0.1;
            double dataPointX = minimum + offset;

            if (GetIsDataPointGrouped(category))
            {
                // Multiple DataPoints share this category; offset and overlap them appropriately
                IGrouping<object, DataPoint> categoryGrouping = GetDataPointGroup(category);
                int index = categoryGrouping.IndexOf(dataPoint);
                dataPointX += (index * (columnWidth * 0.2)) / (categoryGrouping.Count() - 1);
                columnWidth *= 0.8;
                Canvas.SetZIndex(dataPoint, -index);
            }

            if (ValueHelper.CanGraph(dataPointY) && ValueHelper.CanGraph(dataPointX) && ValueHelper.CanGraph(zeroPointY))
            {
                double left = Math.Round(dataPointX);
                double width = Math.Round(columnWidth);

                double top = Math.Round(plotAreaHeight - Math.Max(dataPointY, zeroPointY) + 0.5);
                double bottom = Math.Round(plotAreaHeight - Math.Min(dataPointY, zeroPointY) + 0.5);
                double height = bottom - top + 1;

                Canvas.SetLeft(dataPoint, left);
                Canvas.SetTop(dataPoint, top);
                dataPoint.Width = width;
                dataPoint.Height = height;
            }
        }
    }
}