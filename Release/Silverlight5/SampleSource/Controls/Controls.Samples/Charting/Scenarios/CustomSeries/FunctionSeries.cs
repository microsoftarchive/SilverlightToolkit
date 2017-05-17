// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// FunctionSeries is used to single variable functions on a chart.
    /// </summary>
    [TemplatePart(Name = FunctionSeries.PlotAreaName, Type = typeof(Canvas))]
    public sealed partial class FunctionSeries : Series, IRangeProvider, IAxisListener
    {
        /// <summary>
        /// The default control template would normally reside in generic.xaml,
        /// but the sample project is an application and doesn't have that file.
        /// We're just putting it here, but a real control project wouldn't.
        /// </summary>
        private const string DefaultTemplate =
@"<ControlTemplate
  xmlns='http://schemas.microsoft.com/client/2007'
  xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
  xmlns:samples='clr-namespace:System.Windows.Controls.Samples;assembly=System.Windows.Controls.Samples'
  TargetType='samples:FunctionSeries'>
    <Canvas x:Name='PlotArea'>
        <Path
          Stroke='{TemplateBinding LineBrush}'
          StrokeThickness='{TemplateBinding LineThickness}'
          Data='{TemplateBinding Geometry}' />
    </Canvas>
</ControlTemplate>";

        #region Template Parts
        /// <summary>
        /// Name of the plot area canvas.
        /// </summary>
        private const string PlotAreaName = "PlotArea";

        /// <summary>
        /// Gets or sets the plot area canvas.
        /// </summary>
        private Canvas PlotArea { get; set; }
        #endregion Template Parts

        #region public Func<double, double> Function
        /// <summary>
        /// Gets or sets the function to plot.
        /// </summary>
        [TypeConverter(typeof(SimpleFunctionTypeConverter))]
        public Func<double, double> Function
        {
            get { return GetValue(FunctionProperty) as Func<double, double>; }
            set { SetValue(FunctionProperty, value); }
        }

        /// <summary>
        /// Identifies the Function dependency property.
        /// </summary>
        public static readonly DependencyProperty FunctionProperty =
            DependencyProperty.Register(
                "Function",
                typeof(Func<double, double>),
                typeof(FunctionSeries),
                new PropertyMetadata(null, OnFunctionPropertyChanged));

        /// <summary>
        /// FunctionProperty property changed handler.
        /// </summary>
        /// <param name="d">FunctionSeries that changed its Function.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnFunctionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FunctionSeries source = d as FunctionSeries;
            source.Refresh();
        }
        #endregion public Func<double, double> Function

        #region public Geometry Geometry
        /// <summary>
        /// Gets or sets the geometry of the line object rendering the function.
        /// </summary>
        public Geometry Geometry
        {
            get { return GetValue(GeometryProperty) as Geometry; }
            set { SetValue(GeometryProperty, value); }
        }

        /// <summary>
        /// Identifies the Geometry dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register(
                "Geometry",
                typeof(Geometry),
                typeof(FunctionSeries),
                new PropertyMetadata(null));
        #endregion public Geometry Geometry

        #region public Brush LineBrush
        /// <summary>
        /// Gets or sets the brush used to plot the function.
        /// </summary>
        public Brush LineBrush
        {
            get { return GetValue(LineBrushProperty) as Brush; }
            set { SetValue(LineBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the LineBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register(
                "LineBrush",
                typeof(Brush),
                typeof(FunctionSeries),
                new PropertyMetadata(null, OnLineBrushPropertyChanged));

        /// <summary>
        /// LineBrushProperty property changed handler.
        /// </summary>
        /// <param name="d">FunctionSeries that changed its LineBrush.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnLineBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FunctionSeries source = d as FunctionSeries;
            Brush value = e.NewValue as Brush;
            source.LegendItem.DataContext = new ContentControl { Background = value };
        }
        #endregion public Brush LineBrush

        #region public double LineThickness
        /// <summary>
        /// Gets or sets the thickness of the line used to plot the function.
        /// </summary>
        public double LineThickness
        {
            get { return (double) GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the LineThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register(
                "LineThickness",
                typeof(double),
                typeof(FunctionSeries),
                new PropertyMetadata(1.0));
        #endregion public double LineThickness

        #region private IRangeAxis IndependentAxis
        /// <summary>
        /// Gets or sets the value of the independent axis.
        /// </summary>
        private IRangeAxis IndependentAxis
        {
            get { return GetValue(IndependentAxisProperty) as IRangeAxis; }
            set { SetValue(IndependentAxisProperty, value); }
        }

        /// <summary>
        /// Identifies the IndependentAxis dependency property.
        /// </summary>
        private static readonly DependencyProperty IndependentAxisProperty =
            DependencyProperty.Register(
                "IndependentAxis",
                typeof(IRangeAxis),
                typeof(FunctionSeries),
                null);
        #endregion protected IRangeAxis IndependentAxis

        #region private IRangeAxis DependentAxis
        /// <summary>
        /// Gets or sets the value of the dependent axis.
        /// </summary>
        private IRangeAxis DependentAxis
        {
            get { return GetValue(DependentAxisProperty) as IRangeAxis; }
            set { SetValue(DependentAxisProperty, value); }
        }

        /// <summary>
        /// Identifies the DependentAxis dependency property.
        /// </summary>
        private static readonly DependencyProperty DependentAxisProperty =
            DependencyProperty.Register(
                "DependentAxis",
                typeof(IRangeAxis),
                typeof(FunctionSeries),
                null);
        #endregion protected IRangeAxis DependentAxis

        /// <summary>
        /// Gets or sets the single chart legend item associated with the
        /// series.
        /// </summary>
        private LegendItem LegendItem { get; set; }

        /// <summary>
        /// Gets or sets the Geometry used to clip the line to the PlotArea
        /// bounds.
        /// </summary>
        private RectangleGeometry ClipGeometry { get; set; }

        /// <summary>
        /// Initializes a new instance of the FunctionSeries class.
        /// </summary>
        public FunctionSeries()
        {
            LegendItem = new LegendItem();
            LegendItems.Add(LegendItem);
            Clip = ClipGeometry = new RectangleGeometry();
            SizeChanged += OnSizeChanged;

            // Explicitly load the default template since the samples project
            // is an application and does not have a generic.xaml file.
            Template = XamlReader.Load(DefaultTemplate) as ControlTemplate;
            LineBrush = new SolidColorBrush(Colors.Purple);
        }

        /// <summary>
        /// Refreshes data from data source and renders the series.
        /// </summary>
        private void Refresh()
        {
            if (SeriesHost == null || ActualWidth == 0)
            {
                return;
            }

            // Ensure we have a function to plot
            Func<double, double> function = Function;
            if (function == null)
            {
                return;
            }

            // Ensure we have axes
            IRangeAxis independent = GetAxis(AxisOrientation.X, IndependentAxis);
            IndependentAxis = independent;
            IRangeAxis dependent = GetAxis(AxisOrientation.Y, DependentAxis);
            DependentAxis = dependent;
            if (!independent.Range.HasData)
            {
                return;
            }

            // Create a geometry that matches the function to plot
            PathGeometry path = new PathGeometry();
            PathFigure figure = new PathFigure();

            // Get the range over which we will 
            double start = (double) independent.Range.Minimum;
            double end = (double) independent.Range.Maximum;

            // Adjust the line at each pixel
            double delta = (end - start) / ActualWidth;
            
            // We'll only add a new line segment when the slope is changing
            // between points
            Point last = GetPoint(start, function, independent, dependent);
            figure.StartPoint = last;
            double slope = double.NaN;
            for (double x = start + delta; x <= end; x += delta)
            {
                Point next = GetPoint(x, function, independent, dependent);
                double newSlope = (next.Y - last.Y) / (next.X - last.X);

                if (slope != newSlope)
                {
                    figure.Segments.Add(new LineSegment { Point = last });
                }

                slope = newSlope;
                last = next;
            }
            figure.Segments.Add(new LineSegment { Point = last });

            path.Figures.Add(figure);
            Geometry = path;
        }

        /// <summary>
        /// Get a point in screen coordinates.
        /// </summary>
        /// <param name="x">Independent value.</param>
        /// <param name="function">The function.</param>
        /// <param name="independent">The independent axis.</param>
        /// <param name="dependent">The dependent axis.</param>
        /// <returns>The point in screen coordinates.</returns>
        private Point GetPoint(double x, Func<double, double> function, IRangeAxis independent, IRangeAxis dependent)
        {
            // Get the dependent value
            double y = double.NaN;
            try
            {
                y = function(x);
            }
            catch (DivideByZeroException)
            {
            }

            // Map the actual values into coordinate values
            return new Point(
                independent.GetPlotAreaCoordinate(x).Value,
                Math.Min(
                    Math.Max(
                        ActualHeight - dependent.GetPlotAreaCoordinate(y).Value,
                        -1),
                    ActualHeight + 1));
        }

        /// <summary>
        /// Get the plot area after loading it from XAML.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PlotArea = GetTemplateChild("PlotArea") as Canvas;
        }

        /// <summary>
        /// Updates the visual appearance of all the data points when the size
        /// changes. 
        /// </summary>
        /// <param name="sender">The series.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update the clip geometry
            ClipGeometry.Rect = new Rect(0.0, 0.0, e.NewSize.Width, e.NewSize.Height);

            // Update the PlotArea size and refresh.
            if (PlotArea != null)
            {
                PlotArea.Width = e.NewSize.Width;
                PlotArea.Height = e.NewSize.Height;
                Refresh();
            }
        }

        /// <summary>
        /// Sets all the text the legend items to the title.
        /// </summary>
        /// <param name="oldValue">The old title.</param>
        /// <param name="newValue">The new title.</param>
        protected override void OnTitleChanged(object oldValue, object newValue)
        {
            base.OnTitleChanged(oldValue, newValue);
            LegendItem.Content = Title;
        }

        /// <summary>
        /// Get or create a linear numeric axis in the correct dimension.
        /// </summary>
        /// <param name="orientation">Dimension of the axis to create.</param>
        /// <param name="oldAxis">
        /// Old value of the axis in this dimension.
        /// </param>
        /// <returns>New value of the axis in this dimension.</returns>
        private IRangeAxis GetAxis(AxisOrientation orientation, IRangeAxis oldAxis)
        {
            // Check the existing axes for a potential axis
            IRangeAxis axis =
                (from IRangeAxis a in SeriesHost.Axes.OfType<IRangeAxis>()
                 where a.Orientation == orientation
                 select a)
                .FirstOrDefault();

            if (axis == null)
            {
                // Create a new axis if not found
                axis = new LinearAxis
                {
                    Orientation = orientation,
                };
            }

            if (oldAxis != axis)
            {
                // Unregister any existing axis
                if (oldAxis != null)
                {
                    oldAxis.RegisteredListeners.Remove(this);
                }
                
                // Register the new axis
                if (!axis.RegisteredListeners.Contains(this))
                {
                    axis.RegisteredListeners.Add(this);
                }
            }

            return axis;
        }

        /// <summary>
        /// Updates the series when the axis is invalidated.
        /// </summary>
        /// <param name="axis">The axis that was invalidated.</param>
        public void AxisInvalidated(IAxis axis)
        {
            if (DependentAxis != null && IndependentAxis != null)
            {
                Refresh();
            }
        }

        /// <summary>
        /// Ensures that chart and series are kept in a consistent state when a
        /// series is added or removed from a chart. 
        /// </summary>
        /// <param name="oldValue">Old chart.</param>
        /// <param name="newValue">New chart.</param>
        protected override void OnSeriesHostPropertyChanged(ISeriesHost oldValue, ISeriesHost newValue)
        {
            IRangeAxis axis = null;

            // Unregister the axes from the old chart
            if (oldValue != null)
            {
                axis = IndependentAxis;
                if (axis != null)
                {
                    axis.RegisteredListeners.Remove(this);
                    IndependentAxis = null;
                }

                axis = DependentAxis;
                if (axis != null)
                {
                    axis.RegisteredListeners.Remove(this);
                    DependentAxis = null;
                }
            }

            // Register the axes with new chart
            if (newValue != null)
            {
                axis = IndependentAxis;
                if (axis != null)
                {
                    axis.RegisteredListeners.Add(this);
                }

                axis = DependentAxis;
                if (axis != null)
                {
                    axis.RegisteredListeners.Add(this);
                }
            }

            base.OnSeriesHostPropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// If data is found returns the minimum and maximum dependent numeric
        /// values. 
        /// </summary>
        /// <param name="rangeConsumer">IRangeConsumer that needs the data.</param>
        /// <returns>
        /// The range of values or empty if no data is present.
        /// </returns>
        public Range<IComparable> GetRange(IRangeConsumer rangeConsumer)
        {
            // Use an empty range so we only plot over the area used by other
            // axes.
            return new Range<IComparable>();
        }
    }
}
