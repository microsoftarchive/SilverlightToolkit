// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Collections;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control that contains a data series to be rendered in X/Y 
    /// line format.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(AreaDataPoint))]
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    [StyleTypedProperty(Property = "PathStyle", StyleTargetType = typeof(Path))]
    [TemplatePart(Name = DataPointSeries.PlotAreaName, Type = typeof(Canvas))]
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Depth of hierarchy is necessary to avoid code duplication.")]
    public sealed partial class AreaSeries : LineAreaBaseSeries<AreaDataPoint>, IAnchoredToOrigin
    {
        #region public Geometry Geometry
        /// <summary>
        /// Gets the geometry property.
        /// </summary>
        public Geometry Geometry
        {
            get { return GetValue(GeometryProperty) as Geometry; }
            private set { SetValue(GeometryProperty, value); }
        }

        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register(
                "Geometry",
                typeof(Geometry),
                typeof(AreaSeries),
                null);

        #endregion public Geometry Geometry

        /// <summary>
        /// Gets or sets the style of the Path object that follows the data 
        /// points.
        /// </summary>
        public Style PathStyle
        {
            get { return GetValue(PathStyleProperty) as Style; }
            set { SetValue(PathStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the PathStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty PathStyleProperty =
            DependencyProperty.Register(
                "PathStyle",
                typeof(Style),
                typeof(AreaSeries),
                null);

#if !SILVERLIGHT
        /// <summary>
        /// Initializes the static members of the AreaSeries class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Dependency properties are initialized in-line.")]
        static AreaSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AreaSeries), new FrameworkPropertyMetadata(typeof(AreaSeries)));
        }

#endif
        /// <summary>
        /// Initializes a new instance of the AreaSeries class.
        /// </summary>
        public AreaSeries()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(AreaSeries);
#endif
        }

        /// <summary>
        /// Acquire a horizontal linear axis and a vertical linear axis.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected override void GetAxes(DataPoint firstDataPoint)
        {
            GetAxes(
                firstDataPoint,
                (axis) => axis.Orientation == AxisOrientation.X,
                () =>
                {
                    IAxis axis = CreateRangeAxisFromData(firstDataPoint.IndependentValue);
                    if (axis == null)
                    {
                        axis = new CategoryAxis();
                    }
                    axis.Orientation = AxisOrientation.X;
                    return axis;
                },
                (axis) => 
                    {
                        IRangeAxis rangeAxis = axis as IRangeAxis;
                        return rangeAxis != null && rangeAxis.Origin != null && axis.Orientation == AxisOrientation.Y;
                    },
                () =>
                {
                    DisplayAxis axis = (DisplayAxis)CreateRangeAxisFromData(firstDataPoint.DependentValue);
                    if (axis == null || (axis as IRangeAxis).Origin == null)
                    {
                        throw new InvalidOperationException(Properties.Resources.DataPointSeriesWithAxes_NoSuitableAxisAvailableForPlottingDependentValue);
                    }
                    axis.ShowGridLines = true;
                    axis.Orientation = AxisOrientation.Y;
                    return axis;
                });
        }

        /// <summary>
        /// Creates a DataPoint for determining the line color.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (null != PlotArea)
            {
                Grid grid = new Grid();
                DataPoint dataPoint = CreateDataPoint();
                dataPoint.Visibility = Visibility.Collapsed;
                dataPoint.Loaded += delegate
                {
                    dataPoint.SetStyle(ActualDataPointStyle);
                    Background = dataPoint.Background;
                    if (null != PlotArea)
                    {
                        PlotArea.Children.Remove(grid);
                    }
                };
                grid.Children.Add(dataPoint);
                PlotArea.Children.Add(grid);
            }
        }

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
        public static readonly DependencyProperty DependentRangeAxisProperty =
            DependencyProperty.Register(
                "DependentRangeAxis",
                typeof(IRangeAxis),
                typeof(AreaSeries),
                new PropertyMetadata(null, OnDependentRangeAxisPropertyChanged));

        /// <summary>
        /// DependentRangeAxisProperty property changed handler.
        /// </summary>
        /// <param name="d">AreaSeries that changed its DependentRangeAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDependentRangeAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AreaSeries source = (AreaSeries)d;
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
        /// Gets or sets the independent range axis.
        /// </summary>
        public IAxis IndependentAxis
        {
            get { return GetValue(IndependentAxisProperty) as IAxis; }
            set { SetValue(IndependentAxisProperty, value); }
        }

        /// <summary>
        /// Identifies the IndependentAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty IndependentAxisProperty =
            DependencyProperty.Register(
                "IndependentAxis",
                typeof(IAxis),
                typeof(AreaSeries),
                new PropertyMetadata(null, OnIndependentAxisPropertyChanged));

        /// <summary>
        /// IndependentAxisProperty property changed handler.
        /// </summary>
        /// <param name="d">AreaSeries that changed its IndependentAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIndependentAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AreaSeries source = (AreaSeries)d;
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
        /// Updates the visual representation of the data point.
        /// </summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            double maximum = ActualDependentRangeAxis.GetPlotAreaCoordinate(ActualDependentRangeAxis.Range.Maximum).Value.Value;
            if (ValueHelper.CanGraph(maximum))
            {
                double x = ActualIndependentAxis.GetPlotAreaCoordinate(dataPoint.ActualIndependentValue).Value.Value;
                double y = ActualDependentRangeAxis.GetPlotAreaCoordinate(dataPoint.ActualDependentValue).Value.Value;

                if (ValueHelper.CanGraph(x) && ValueHelper.CanGraph(y))
                {
                    double coordinateY = Math.Round(maximum - (y + (dataPoint.ActualHeight / 2)));
                    Canvas.SetTop(dataPoint, coordinateY);
                    double coordinateX = Math.Round(x - (dataPoint.ActualWidth / 2));
                    Canvas.SetLeft(dataPoint, coordinateX);
                }
            }

            if (!UpdatingDataPoints)
            {
                UpdateShape();
            }
        }

        /// <summary>
        /// Updates the point collection object.
        /// </summary>
        protected override void UpdateShape()
        {
            double maximum = ActualDependentRangeAxis.GetPlotAreaCoordinate(ActualDependentRangeAxis.Range.Maximum).Value.Value;

            Func<DataPoint, Point> createPoint =
                dataPoint =>
                    new Point(
                        ActualIndependentAxis.GetPlotAreaCoordinate(dataPoint.ActualIndependentValue).Value.Value,
                        maximum - ActualDependentRangeAxis.GetPlotAreaCoordinate(dataPoint.ActualDependentValue).Value.Value);

            IEnumerable<Point> points = null;
            if (ValueHelper.CanGraph(maximum))
            {
                if (ActualIndependentAxis is IRangeAxis)
                {
                    points = this.DataPointsByIndependentValue.Select(createPoint);
                }
                else
                {
                    points =
                        ActiveDataPoints
                            .Select(createPoint)
                            .OrderBy(point => point.X);
                }

                if (!points.IsEmpty())
                {
                    double originY = Math.Floor(ActualDependentRangeAxis.GetPlotAreaCoordinate(ActualDependentRangeAxis.Origin).Value.Value);
                    PathFigure figure = new PathFigure();
                    figure.IsClosed = true;
                    figure.IsFilled = true;

                    Point startPoint;
                    IEnumerator<Point> pointEnumerator = points.GetEnumerator();
                    if (pointEnumerator.MoveNext())
                    {
                        startPoint = new Point(pointEnumerator.Current.X, maximum - originY);
                        figure.StartPoint = startPoint;
                    }
                    else
                    {
                        this.Geometry = null;
                        return;
                    }

                    Point lastPoint;
                    do
                    {
                        lastPoint = pointEnumerator.Current;
                        figure.Segments.Add(new LineSegment { Point = pointEnumerator.Current });
                    } 
                    while (pointEnumerator.MoveNext());
                    figure.Segments.Add(new LineSegment { Point = new Point(lastPoint.X, maximum - originY) });

                    if (figure.Segments.Count > 1)
                    {
                        PathGeometry geometry = new PathGeometry();
                        geometry.Figures.Add(figure);
                        this.Geometry = geometry;
                        return;
                    }
                }
            }
            this.Geometry = null;
        }

        /// <summary>
        /// Remove value margins from the side of the data points to ensure
        /// that area chart is flush against the edge of the chart.
        /// </summary>
        /// <param name="consumer">The value margin consumer.</param>
        /// <returns>A sequence of value margins.</returns>
        protected override IEnumerable<ValueMargin> GetValueMargins(IValueMarginConsumer consumer)
        {
            if (consumer == ActualIndependentAxis)
            {
                return Enumerable.Empty<ValueMargin>();
            }
            return base.GetValueMargins(consumer);
        }

        /// <summary>
        /// Gets the axis to which the series is anchored.
        /// </summary>
        IRangeAxis IAnchoredToOrigin.AnchoredAxis
        {
            get { return ActualDependentRangeAxis;  }
        }
    }
}