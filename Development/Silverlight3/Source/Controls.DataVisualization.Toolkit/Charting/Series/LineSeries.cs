// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(LineDataPoint))]
    [StyleTypedProperty(Property = "LegendItemStyle", StyleTargetType = typeof(LegendItem))]
    [StyleTypedProperty(Property = "PolylineStyle", StyleTargetType = typeof(Polyline))]
    [TemplatePart(Name = DataPointSeries.PlotAreaName, Type = typeof(Canvas))]
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Depth of hierarchy is necessary to avoid code duplication.")]
    public sealed partial class LineSeries : LineAreaBaseSeries<LineDataPoint>
    {
        #region public PointCollection Points
        /// <summary>
        /// Gets the collection of points that make up the line.
        /// </summary>
        public PointCollection Points
        {
            get { return GetValue(PointsProperty) as PointCollection; }
            private set { SetValue(PointsProperty, value); }
        }

        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                "Points",
                typeof(PointCollection),
                typeof(LineSeries),
                null);

        #endregion public PointCollection Points

        /// <summary>
        /// Gets or sets the style of the Polyline object that follows the data 
        /// points.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Polyline", Justification = "Matches System.Windows.Shapes.Polyline.")]
        public Style PolylineStyle
        {
            get { return GetValue(PolylineStyleProperty) as Style; }
            set { SetValue(PolylineStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the PolylineStyle dependency property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Polyline", Justification = "Matches System.Windows.Shapes.Polyline.")]
        public static readonly DependencyProperty PolylineStyleProperty =
            DependencyProperty.Register(
                "PolylineStyle",
                typeof(Style),
                typeof(LineSeries),
                null);

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
                typeof(LineSeries),
                new PropertyMetadata(null, OnDependentRangeAxisPropertyChanged));

        /// <summary>
        /// DependentRangeAxisProperty property changed handler.
        /// </summary>
        /// <param name="d">LineSeries that changed its DependentRangeAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDependentRangeAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSeries source = (LineSeries)d;
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
                typeof(LineSeries),
                new PropertyMetadata(null, OnIndependentAxisPropertyChanged));

        /// <summary>
        /// IndependentAxisProperty property changed handler.
        /// </summary>
        /// <param name="d">LineSeries that changed its IndependentAxis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIndependentAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineSeries source = (LineSeries)d;
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

#if !SILVERLIGHT
        /// <summary>
        /// Initializes the static members of the LineSeries class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Dependency properties are initialized in-line.")]
        static LineSeries()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineSeries), new FrameworkPropertyMetadata(typeof(LineSeries)));
        }

#endif
        /// <summary>
        /// Initializes a new instance of the LineSeries class.
        /// </summary>
        public LineSeries()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(LineSeries);
#endif
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
                (axis) => axis.Orientation == AxisOrientation.Y && axis is IRangeAxis,
                () =>
                {
                    DisplayAxis axis = (DisplayAxis)CreateRangeAxisFromData(firstDataPoint.DependentValue);
                    if (axis == null)
                    {
                        throw new InvalidOperationException(Properties.Resources.DataPointSeriesWithAxes_NoSuitableAxisAvailableForPlottingDependentValue);
                    }
                    axis.ShowGridLines = true;
                    axis.Orientation = AxisOrientation.Y;
                    return axis;
                });
        }

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
                    points = DataPointsByIndependentValue.Select(createPoint);
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
                    this.Points = new PointCollection();
                    foreach (Point point in points)
                    {
                        this.Points.Add(point);
                    }
                    return;
                }
            }
            this.Points = null;
        }
    }
}