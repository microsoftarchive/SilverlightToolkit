// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a control which displays a title, a series of tick marks, and some labels.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "TickMarkStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "GridLineStyle", StyleTargetType = typeof(Line))]
    [StyleTypedProperty(Property = "LabelStyle", StyleTargetType = typeof(TextBlock))]
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
    [TemplatePart(Name = AxisContentCanvasName, Type = typeof(Canvas))]
    [TemplatePart(Name = AxisLayoutGridName, Type = typeof(Grid))]
    [TemplatePart(Name = AxisTitleName, Type = typeof(Title))]
    [TemplatePart(Name = TitleCanvasName, Type = typeof(Canvas))]
    public sealed partial class Axis : Control
    {
        /// <summary>
        /// The name of the content canvas template part.
        /// </summary>
        private const string AxisContentCanvasName = "AxisContentCanvas";

        /// <summary>
        /// The name of the layout grid template part.
        /// </summary>
        private const string AxisLayoutGridName = "AxisLayoutGrid";

        /// <summary>
        /// The name of the title template part.
        /// </summary>
        private const string AxisTitleName = "AxisTitle";

        /// <summary>
        /// The name of the title canvas template part.
        /// </summary>
        public const string TitleCanvasName = "TitleCanvas";
        
        /// <summary>
        /// Gets or sets the canvas used for labels.
        /// </summary>
        private Canvas AxisContentCanvas { get; set; }

        /// <summary>
        /// Gets or sets the size of the AxisContentCanvas.
        /// </summary>
        private Size AxisContentCanvasSize { get; set; }

        /// <summary>
        /// Gets or sets the grid used for the axis.
        /// </summary>
        private Grid AxisLayoutGrid { get; set; }

        /// <summary>
        /// Gets or sets the reference to the Axis Title.
        /// </summary>
        private Title AxisTitle { get; set; }

        /// <summary>
        /// Gets or sets the reference to the Axis Title Canvas.
        /// </summary>
        private Canvas TitleCanvas { get; set; }

        /// <summary>
        /// Maximum intervals per 200 pixels.
        /// </summary>
        private static double MaximumAxisIntervals = 8;

        /// <summary>
        /// Defines Tick Marks length.
        /// </summary>
        private static double TickMarksLength = 4;
        
        /// <summary>
        /// Retrieve template children.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AxisContentCanvas = GetTemplateChild(AxisContentCanvasName) as Canvas;
            AxisLayoutGrid = GetTemplateChild(AxisLayoutGridName) as Grid;
            AxisTitle = GetTemplateChild(AxisTitleName) as Title;
            TitleCanvas = GetTemplateChild(TitleCanvasName) as Canvas;

            if (AxisContentCanvas != null)
            {
                AxisContentCanvas.SizeChanged += new SizeChangedEventHandler(OnAxisContentSizeChanged);
            }

            if (AxisTitle != null)
            {
                AxisTitle.SizeChanged += new SizeChangedEventHandler(OnAxisTitleSizeChanged);
            }

            Invalidate();
        }

        /// <summary>
        /// Initializes a new instance of the Axis class.
        /// </summary>
        public Axis()
        {
            DefaultStyleKey = typeof(Axis);

            // By default set axis to support all types.
            CategoryType = typeof(object);
            RemoveIfUnused = true;
            _categoryList = new List<object>();
            AxisType = AxisType.Auto;
            Orientation = AxisOrientation.Vertical;
            RegisteredSeries = new List<IAxisInformationProvider>();
        }
        
        /// <summary>
        /// Updates the Axis canvas when the axis size changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void OnAxisContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AxisContentCanvasSize = e.NewSize;
            AxisContentSizeChanged(AxisType);
            Invalidate();
        }

        #region public bool ShowGridLines
        /// <summary>
        /// Gets or sets a value indicating whether grid lines should be shown.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLines", Justification = "Current casing is the expected one.")]
        public bool ShowGridLines
        {
            get { return (bool)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }

        /// <summary>
        /// Identifies the ShowGridLines dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLines", Justification = "Current casing is the expected one.")]
        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register(
                "ShowGridLines",
                typeof(bool),
                typeof(Axis),
                new PropertyMetadata(false, OnShowGridLinesPropertyChanged));

        /// <summary>
        /// ShowGridLinesProperty property changed handler.
        /// </summary>
        /// <param name="d">Axis that changed its ShowGridLines.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLines", Justification = "Current casing is the expected one.")]
        private static void OnShowGridLinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis source = (Axis)d;
            bool newValue = (bool)e.NewValue;
            source.OnShowGridLinesPropertyChanged(newValue);
        }

        /// <summary>
        /// ShowGridLinesProperty property changed handler.
        /// </summary>
        /// <param name="newValue">New value.</param>        
        private void OnShowGridLinesPropertyChanged(bool newValue)
        {
            if (newValue == true)
            {
                this.GridLines = new GridLines(this);
            }
            else
            {
                this.GridLines = null;
            }
        }
        #endregion public bool ShowGridLines

        #region internal GridLines GridLines
        /// <summary>
        /// The event raised when the GridLines property changes.
        /// </summary>
        internal event RoutedPropertyChangedEventHandler<GridLines> GridLinesPropertyChanged;

        /// <summary>
        /// Gets or sets the grid lines associated with the axis.
        /// </summary>
        internal GridLines GridLines
        {
            get { return GetValue(GridLinesProperty) as GridLines; }
            set { SetValue(GridLinesProperty, value); }
        }

        /// <summary>
        /// Identifies the GridLines dependency property.
        /// </summary>
        internal static readonly DependencyProperty GridLinesProperty =
            DependencyProperty.Register(
                "GridLines",
                typeof(GridLines),
                typeof(Axis),
                new PropertyMetadata(null, OnGridLinesPropertyChanged));

        /// <summary>
        /// GridLinesProperty property changed handler.
        /// </summary>
        /// <param name="d">Axis that changed its GridLines.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnGridLinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis source = (Axis)d;
            GridLines oldValue = (GridLines)e.OldValue;
            GridLines newValue = (GridLines)e.NewValue;
            source.OnGridLinesPropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// GridLinesProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        private void OnGridLinesPropertyChanged(GridLines oldValue, GridLines newValue)
        {
            RoutedPropertyChangedEventHandler<GridLines> handler = this.GridLinesPropertyChanged;
            if (handler != null)
            {
                handler(this, new RoutedPropertyChangedEventArgs<GridLines>(oldValue, newValue));
            }
        }
        #endregion internal GridLines GridLines

        /// <summary>
        /// Gets or sets a value indicating whether this axis can be removed if 
        /// it is not in use.
        /// </summary>
        public bool RemoveIfUnused { get; set; }

        /// <summary>
        /// Gets or sets the series host that owns the Axis.
        /// </summary>
        internal ISeriesHost ISeriesHost { get; set; }

        /// <summary>
        /// Tracks whether the Axis is changing what is externally a read-only DependencyProperty.
        /// </summary>
        private bool _changingReadOnlyDependencyProperty;

        /// <summary>
        /// Internal list of series data ranges.
        /// </summary>
        private Dictionary<IAxisInformationProvider, Range<double>> _seriesRanges = new Dictionary<IAxisInformationProvider, Range<double>>();
        
        /// <summary>
        /// Internal list of Categories.
        /// </summary>
        private IList<object> _categoryList;

        #region public AxisType AxisType
        /// <summary>
        /// Gets or sets the AxisType property of the Axis.
        /// </summary>
        public AxisType AxisType
        {
            get { return (AxisType)GetValue(AxisTypeProperty); }
            set { SetValue(AxisTypeProperty, value); }
        }

        /// <summary>
        /// Identifies the AxisType dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisTypeProperty =
            DependencyProperty.Register(
                "AxisType",
                typeof(AxisType),
                typeof(Axis),
                new PropertyMetadata(AxisType.Auto, OnAxisTypePropertyChanged));

        /// <summary>
        /// Called when the value of the AxisType property changes.
        /// </summary>
        /// <param name="d">Axis that changed its AxisType.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnAxisTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Axis source = (Axis)d;
            AxisType oldValue = (AxisType)e.OldValue;
            source.OnAxisTypePropertyChanged(oldValue);
        }

        /// <summary>
        /// Called when the value of the AxisType property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>        
        private void OnAxisTypePropertyChanged(AxisType oldValue)
        {
            if (oldValue != AxisType.Auto && RegisteredSeries.Count > 0)
            {
                throw new InvalidOperationException(Properties.Resources.Axis_OnAxisTypeChanged_CannotChangeTypeOfAnAxisWhenItIsInUseByASeries);
            }
        }
        #endregion public AxisType AxisType

        /// <summary>
        /// Gets or sets the category type from which all categories on the 
        /// axes must inherit.
        /// </summary>
        internal Type CategoryType { get; set; }

        /// <summary>
        /// Gets or sets the interval applied to the Axis.
        /// </summary>
        [TypeConverter(typeof(NullableConverter<double>))]
        public double? Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                if (_interval == null)
                {
                    _interval = new double?();
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Specifies the reference to the interval value used in an Axis.
        /// </summary>
        private double? _interval = new double?();

        /// <summary>
        /// Gets or sets the type of interval the Interval property represents.
        /// </summary>
        /// <value>One of the IntervalType values.</value>
        public AxisIntervalType IntervalType
        {
            get { return _intervalType; }
            set
            {
                _intervalType = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Specifies the reference to the interval type used in an  Axis.
        /// </summary>
        private AxisIntervalType _intervalType = AxisIntervalType.Auto;

        /// <summary>
        /// Gets or sets the offset applied to the Interval property.
        /// </summary>
        public double IntervalOffset
        {
            get { return _intervalOffset; }
            set
            {
                _intervalOffset = Double.IsNaN(value) ? 0 : value;
                Invalidate();
            }
        }

        /// <summary>
        /// Specifies the reference to the interval offset value used in an Axis.
        /// </summary>
        private double _intervalOffset;

        /// <summary>
        /// Gets or sets the IntervalType represented by the IntervalOffset 
        /// property.
        /// </summary>
        /// <value>The IntervalType of the interval offset.</value>
        public AxisIntervalType IntervalOffsetType
        {
            get { return _intervalOffsetType; }
            set
            {
                _intervalOffsetType = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Specifies the reference to the interval offset type used in an Axis.
        /// </summary>
        private AxisIntervalType _intervalOffsetType = AxisIntervalType.Auto;

        #region public IComparable Maximum
        /// <summary>
        /// Gets or sets the maximum value of the Axis.
        /// </summary>
        public IComparable Maximum
        {
            get { return (IComparable)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Identifies the Maximum dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(IComparable),
                typeof(Axis),
                new PropertyMetadata(OnMinimumMaximumChanged));

        /// <summary>
        /// MaximumProperty property changed callback.
        /// </summary>
        /// <param name="o">Axis for which the Maximum changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)o).OnMinimumMaximumChanged((IComparable)e.OldValue, (IComparable)e.NewValue);
        }

        /// <summary>
        /// Called when the Maximum property changes.
        /// </summary>
        /// <param name="oldValue">Old value of the Maximum property.</param>
        /// <param name="newValue">New value of the Maximum property.</param>
        private void OnMinimumMaximumChanged(IComparable oldValue, IComparable newValue)
        {
            // Validate newValue.  Will throw on a bad value.
            if ((oldValue != newValue) && ValidateMinimumOrMaximum(newValue))
            {
                if (AxisType == AxisType.Linear)
                {
                    RecalculateMinAndMax();
                }
                else if (AxisType == AxisType.DateTime)
                {
                    RecalculateMinAndMaxDates();
                }
                else if (AxisType == AxisType.Category)
                {
                    CreateCategoryCollection();
                }
            }
        }
        #endregion public IComparable Maximum

        #region public IComparable ActualMaximum
        /// <summary>
        /// Gets the actual maximum applied to the Axis.
        /// </summary>
        public IComparable ActualMaximum
        {
            get { return (IComparable)GetValue(ActualMaximumProperty); }
            private set
            {
                try
                {
                    _changingReadOnlyDependencyProperty = true;
                    SetValue(ActualMaximumProperty, value);
                }
                finally
                {
                    _changingReadOnlyDependencyProperty = false;
                }
            }
        }

        /// <summary>
        /// Identifies the ActualMaximum dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualMaximumProperty =
            DependencyProperty.Register(
                "ActualMaximum",
                typeof(IComparable),
                typeof(Axis),
                new PropertyMetadata(OnActualMaximumChanged));

        /// <summary>
        /// ActualMaximumProperty property changed callback.
        /// </summary>
        /// <param name="o">Axis for which the ActualMaximum changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnActualMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!((Axis)o)._changingReadOnlyDependencyProperty)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.DataVisualization_ReadOnlyDependencyPropertyChange, "ActualMaximum"));
            }
        }
        #endregion public IComparable ActualMaximum

        #region public IComparable Minimum
        /// <summary>
        /// Gets or sets the minimum value of the Axis.
        /// </summary>
        public IComparable Minimum
        {
            get { return (IComparable)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Identifies the Minimum dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(IComparable),
                typeof(Axis),
                new PropertyMetadata(OnMinimumMaximumChanged));

        #endregion public IComparable Minimum

        #region public IComparable ActualMinimum
        /// <summary>
        /// Gets the actual minimum applied to the Axis.
        /// </summary>
        public IComparable ActualMinimum
        {
            get { return (IComparable)GetValue(ActualMinimumProperty); }
            private set
            {
                try
                {
                    _changingReadOnlyDependencyProperty = true;
                    SetValue(ActualMinimumProperty, value);
                }
                finally
                {
                    _changingReadOnlyDependencyProperty = false;
                }
            }
        }

        /// <summary>
        /// Identifies the ActualMinimum dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualMinimumProperty =
            DependencyProperty.Register(
                "ActualMinimum",
                typeof(IComparable),
                typeof(Axis),
                new PropertyMetadata(OnActualMinimumChanged));

        /// <summary>
        /// ActualMinimumProperty property changed callback.
        /// </summary>
        /// <param name="o">Axis for which the ActualMinimum changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnActualMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!((Axis)o)._changingReadOnlyDependencyProperty)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.DataVisualization_ReadOnlyDependencyPropertyChange, "ActualMinimum"));
            }
        }
        #endregion public IComparable ActualMinimum

        /// <summary>
        /// Converts an Axis Minimum or Maximum value to a double.
        /// </summary>
        /// <param name="minMax">The IComparable to convert to a double.</param>
        /// <returns>The Axis Minimum or Maximum value as a double.</returns>
        private double GetMinMaxAsDouble(IComparable minMax)
        {
            double returnValue = 0.0;
            ValidateMinimumOrMaximum(ref returnValue, minMax);
            return returnValue;
        }

        /// <summary>
        /// Gets the range of dates represents by the Axis.
        /// </summary>
        /// <value>The data range represented by the Axis.</value>
        internal Range<double> DataRange
        {
            get
            {
                Range<double> result = _dataRange;
                if (result.IsEmpty)
                {
                    result = new Range<double>(0, 0);
                }
                if (AxisType != AxisType.Category)
                {
                    if (Maximum != null)
                    {
                        result = new Range<double>(result.Minimum, GetMinMaxAsDouble(Maximum));
                    }
                    if (Minimum != null)
                    {
                        result = new Range<double>(GetMinMaxAsDouble(Minimum), result.Maximum);
                    }
                }
                else
                {
                    if (_categoryList.Count > 0)
                    {
                        result = new Range<double>(0, _categoryList.Count - 1);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Specifies a reference to DataRange property.
        /// </summary>
        private Range<double> _dataRange = new Range<double>();

        /// <summary>
        /// Allows ActualMinimum and ActualMaximum to be set using a range of doubles.
        /// Will convert the minimum and maximum values to the proper form.
        /// </summary>
        /// <param name="value">The range to convert.</param>
        private void SetActualMinMax(Range<double> value)
        {
            if (!value.IsEmpty)
            {
                if (value.Maximum != GetMinMaxAsDouble(ActualMaximum))
                {
                    if (AxisType == AxisType.DateTime)
                    {
                        ActualMaximum = DateTime.FromOADate(value.Maximum);
                    }
                    else
                    {
                        ActualMaximum = value.Maximum;
                    }
                }
                if (value.Minimum != GetMinMaxAsDouble(ActualMinimum))
                {
                    if (AxisType == AxisType.DateTime)
                    {
                        ActualMinimum = DateTime.FromOADate(value.Minimum);
                    }
                    else
                    {
                        ActualMinimum = value.Minimum;
                    }
                }
            }
            else
            {
                ActualMinimum = null;
                ActualMaximum = null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the minimum value
        /// of the axis will be automatically set to zero if all data point
        /// values are positive.  If there are negative data point values,
        /// the minimum value of the data points will be used.
        /// </summary>
        /// <value>
        /// Returns <c>true</c> if this axis is started from zero; otherwise, 
        /// <c>false</c>.</value>
        /// <remarks>If there are negative data point values, the minimum value 
        /// of the data points will be used.</remarks>
        public bool ShouldIncludeZero 
        {
            get { return (bool)GetValue(ShouldIncludeZeroProperty); }
            set { SetValue(ShouldIncludeZeroProperty, value); }
        }

        /// <summary>
        /// Identifies the ShouldIncludeZero dependency property.
        /// </summary>
        public static readonly DependencyProperty ShouldIncludeZeroProperty =
            DependencyProperty.Register(
                "ShouldIncludeZero",
                typeof(bool),
                typeof(Axis),
                new PropertyMetadata(OnShouldIncludeZeroChanged));

        /// <summary>
        /// ShouldIncludeZero property changed callback.
        /// </summary>
        /// <param name="o">Axis for which the ShouldIncludeZero changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnShouldIncludeZeroChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Axis)o).OnShouldIncludeZeroChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Called when the ShouldIncludeZero property changes.
        /// </summary>
        /// <param name="oldValue">Old value of the ShouldIncludeZero property.</param>
        /// <param name="newValue">New value of the ShouldIncludeZero property.</param>
        private void OnShouldIncludeZeroChanged(bool oldValue, bool newValue)
        {
            // Validate newValue.
            if (oldValue != newValue)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the format string applied to the Axis label.
        /// </summary>
        /// <value>The format string applied to the Axis label.</value>
        public string LabelStringFormat
        {
            get { return (string)GetValue(LabelStringFormatProperty); }
            set { SetValue(LabelStringFormatProperty, value); }
        }

        /// <summary>
        /// Identifies the LabelsVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelStringFormatProperty =
            DependencyProperty.Register(
                "LabelStringFormat",
                typeof(string),
                typeof(Axis),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether the Axis labels are displayed.
        /// </summary>
        /// <value>The visibility applied to the Axis labels.</value>
        public bool ShowLabels
        {
            get { return (bool)GetValue(ShowLabelsProperty); }
            set { SetValue(ShowLabelsProperty, value); }
        }

        /// <summary>
        /// Identifies the LabelsVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowLabelsProperty =
            DependencyProperty.Register(
                "ShowLabels",
                typeof(bool),
                typeof(Axis),
                new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether tick marks are visible on 
        /// the Axis.
        /// </summary>
        /// <value>The visibility applied to the Axis tick marks.</value>
        public bool ShowTickMarks
        {
            get { return (bool)GetValue(ShowTickMarksProperty); }
            set { SetValue(ShowTickMarksProperty, value); }
        }

        /// <summary>
        /// Identifies the TickMarksVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowTickMarksProperty =
            DependencyProperty.Register(
                "ShowTickMarks",
                typeof(bool),
                typeof(Axis),
                new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the title displayed for the Axis.
        /// </summary>
        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(object),
                typeof(Axis),
                null);

        /// <summary>
        /// Gets or sets the style applied to the Axis title.
        /// </summary>
        /// <value>The Style applied to the Axis title.</value>
        public Style TitleStyle
        {
            get { return GetValue(TitleStyleProperty) as Style; }
            set { SetValue(TitleStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the TitleStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleStyleProperty =
            DependencyProperty.Register(
                "TitleStyle",
                typeof(Style),
                typeof(Axis),
                null);

        /// <summary>
        /// Gets or sets the style applied to the Axis labels.
        /// </summary>
        /// <value>The Style applied to the Axis labels.</value>
        public Style LabelStyle
        {
            get { return GetValue(LabelStyleProperty) as Style; }
            set { SetValue(LabelStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the LabelStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.Register(
                "LabelStyle",
                typeof(Style),
                typeof(Axis),
                null);

        /// <summary>
        /// Gets or sets the style applied to the Axis tick marks.
        /// </summary>
        /// <value>The Style applied to the Axis tick marks.</value>
        public Style TickMarkStyle
        {
            get { return GetValue(TickMarkStyleProperty) as Style; }
            set { SetValue(TickMarkStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the TickMarkStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TickMarkStyleProperty =
            DependencyProperty.Register(
                "TickMarkStyle",
                typeof(Style),
                typeof(Axis),
                null);

        /// <summary>
        /// Gets or sets the Style of the Axis's gridlines.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLine", Justification = "Current casing is the expected one.")]
        public Style GridLineStyle
        {
            get { return GetValue(GridLineStyleProperty) as Style; }
            set { SetValue(GridLineStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the GridlineStyle dependency property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GridLine", Justification = "Current casing is the expected one.")]
        public static readonly DependencyProperty GridLineStyleProperty =
            DependencyProperty.Register(
                "GridLineStyle",
                typeof(Style),
                typeof(Axis),
                null);

        /// <summary>
        /// Occurs when the axis is invalidated by having its range or its 
        /// categories change.
        /// </summary>
        public event RoutedEventHandler Invalidated;

        /// <summary>
        /// Method that raises Invalidated event to alert series that they
        /// should re-render.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        private void OnInvalidated(RoutedEventArgs e)
        {
            RoutedEventHandler handler = Invalidated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Gets or sets the Orientation property of the Axis.
        /// </summary>
        /// <value>One of the AxisOrientation values.</value>
        public AxisOrientation Orientation { get; set; }

        /// <summary>
        /// Gets or sets the collection of series objects that are using the 
        /// Axis.
        /// </summary>
        private IList<IAxisInformationProvider> RegisteredSeries { get; set; }

        /// <summary>
        /// Contains the logic for validating and setting the _minimum and _maximum fields.
        /// On set, Invalidates the Axis.
        /// </summary>
        /// <param name="minMaxField">Which field we're setting.</param>
        /// <param name="value">The value to validate and use when setting the field.</param>
        /// <returns>True, if the value is valid.  Throws if the value is not.</returns>
        private bool ValidateMinimumOrMaximum(ref double minMaxField, object value)
        {
            if (value == null)
            {
                minMaxField = Double.NaN;
                return true;
            }
            else
            {
                DateTime tmpDateTime;
                double tmpDouble;
                if (ValueHelper.TryConvert(value, out tmpDouble))
                {
                    minMaxField = tmpDouble;
                    return true;
                }
                else if (ValueHelper.TryConvert(value, out tmpDateTime))
                {
                    minMaxField = tmpDateTime.ToOADate();
                    return true;
                }
                else if (AxisType == AxisType.DateTime && DateTime.TryParse(value.ToString(), out tmpDateTime))
                {
                    minMaxField = tmpDateTime.ToOADate();
                    return true;
                }
                else if (Double.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.CurrentCulture, out tmpDouble))
                {
                    minMaxField = tmpDouble;
                    return true;
                }
                else
                {
                    if (AxisType == AxisType.DateTime)
                    {
                        throw new ArgumentException(Properties.Resources.Axis_DateTimeAxis_InvalidFormatArgument);
                    }
                    else
                    {
                        throw new ArgumentException(Properties.Resources.Axis_LinearAxis_NonDoubleArgument);
                    }
                }
            }
        }
        
        /// <summary>
        /// Contains the logic for validating an Axis Minimum or Maximum.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>True, if the value is valid.  Throws if the value is not.</returns>
        private bool ValidateMinimumOrMaximum(object value)
        {
            double tmpDouble = 0.0;
            return ValidateMinimumOrMaximum(ref tmpDouble, value);
        }

        /// <summary>
        /// Causes the Axis to invalidate itself and re-create all its dynamic visual elements based on a provided series collection.
        /// </summary>
        internal void Invalidate()
        {
            ResetIntervals();

            if (AxisContentCanvas != null)
            {
                // Recalculate title placement
                if (AxisTitle != null)
                {
                    if (Orientation == AxisOrientation.Horizontal)
                    {
                        if (AxisLayoutGrid != null)
                        {
                            TitleCanvas.SetValue(Grid.RowProperty, 1);
                            TitleCanvas.SetValue(Grid.ColumnProperty, 1);
                            TitleCanvas.SetValue(Grid.RowSpanProperty, 2);
                            TitleCanvas.HorizontalAlignment = HorizontalAlignment.Center;
                            TitleCanvas.VerticalAlignment = VerticalAlignment.Center;
                        }
                    }
                    else
                    {
                        if (AxisLayoutGrid != null)
                        {
                            TitleCanvas.SetValue(Grid.RowProperty, 0);
                            TitleCanvas.SetValue(Grid.ColumnProperty, 0);
                            TitleCanvas.SetValue(Grid.RowSpanProperty, 2);
                            TitleCanvas.HorizontalAlignment = HorizontalAlignment.Right;
                            TitleCanvas.VerticalAlignment = VerticalAlignment.Center;

                            // rotate vertical axis titles by 90 degrees counter-clockwise
                            TransformGroup transformGroup = new TransformGroup();
                            transformGroup.Children.Add(new RotateTransform { Angle = -90 });
                            transformGroup.Children.Add(new TranslateTransform { Y = AxisTitle.ActualWidth });
                            AxisTitle.RenderTransform = transformGroup;
                        }
                    }
                }

                // Recalculate labels
                AxisContentSizeChanged(AxisType);
                OnInvalidated(new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Update Axis canvas when the axis size changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void OnAxisTitleSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (TitleCanvas != null && AxisTitle != null)
            {
                if (Orientation == AxisOrientation.Vertical)
                {
                    TitleCanvas.Width = e.NewSize.Height;
                    TitleCanvas.Height = e.NewSize.Width;
                }
                else if (Orientation == AxisOrientation.Horizontal)
                {
                    TitleCanvas.Width = e.NewSize.Width;
                    TitleCanvas.Height = e.NewSize.Height;
                }
            }
        }

        /// <summary>
        /// Method to update various axis variables based on an ItemsSource.
        /// </summary>
        private void CreateCategoryCollection()
        {
            IList<object> categoryCaptions =
                RegisteredSeries
                    .SelectMany(series => series.GetCategories(this))
                    .Distinct()
                    .ToList();

            bool shouldInvalidate = true;
            if (categoryCaptions.Count == _categoryList.Count)
            {
                // check each if items in new list and old list equal each other
                shouldInvalidate = false;
                for (int i = 0; (i < _categoryList.Count); i++)
                {
                    if (!_categoryList[i].Equals(categoryCaptions[i]))
                    {
                        shouldInvalidate = true;
                        break;
                    }
                }
            }

            _categoryList = categoryCaptions;
            if (shouldInvalidate)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Accepts a category and returns the coordinate range of that category
        /// on the axis.
        /// </summary>
        /// <param name="category">A category for which to retrieve the 
        /// coordinate location.</param>
        /// <returns>The coordinate range of the category on the axis.</returns>        
        public Range<double> GetPlotAreaCoordinateRange(object category)
        {
            if (this.AxisType != AxisType.Category)
            {
                throw new InvalidOperationException(Properties.Resources.Axis_AxisTypeIsNotCategory);
            }
            double index = _categoryList.IndexOf(category);
            if (index != -1)
            {
                double margin = ActualInterval / 2;
                double minimum = 0.0;
                double maximum = 0.0;

                if (Orientation == AxisOrientation.Horizontal)
                {
                    minimum = GetDataPointCategoryByIndexInPixels(index - margin);
                    maximum = GetDataPointCategoryByIndexInPixels(index + margin);
                }
                else if (Orientation == AxisOrientation.Vertical)
                {
                    minimum = GetDataPointCategoryByIndexInPixels(index + margin);
                    maximum = GetDataPointCategoryByIndexInPixels(index - margin);
                }
                else
                {
                    return Range<double>.Empty;
                }

                if (minimum > maximum)
                {
                    double swap = minimum;
                    minimum = maximum;
                    maximum = swap;
                }
                return new Range<double>(minimum, maximum);
            }
            return new Range<double>();
        }

        /// <summary>
        /// Method to update various axis variables based on an ItemsSource.
        /// </summary>
        private void RecalculateMinAndMaxDates()
        {
            _seriesRanges.Clear();

            foreach (IAxisInformationProvider series in RegisteredSeries)
            {
                Range<DateTime> dateRange = series.GetDateTimeRange(this);
                Range<double> range =
                                    !dateRange.IsEmpty ?
                                    new Range<double>(dateRange.Minimum.ToOADate(), dateRange.Maximum.ToOADate()) :
                                    new Range<double>();
                _seriesRanges.Add(series, range);
            }

            Range<double> dataRange = _seriesRanges.Select(item => item.Value).Sum();

            if (_dataRange != dataRange)
            {
                _dataRange = dataRange;
            }
            Invalidate();
        }

        /// <summary>
        /// Method to update various axis variables based on an ItemsSource.
        /// </summary>
        private void RecalculateMinAndMax()
        {
            _seriesRanges.Clear();
            
            foreach (IAxisInformationProvider series in RegisteredSeries)
            {
                _seriesRanges.Add(series, series.GetNumericRange(this)); 
            }
            
            Range<double> dataRange = _seriesRanges.Select(item => item.Value).Sum();

            if (_dataRange != dataRange)
            {
                _dataRange = dataRange;
            }
            Invalidate();
        }

        /// <summary>
        /// Recalculates axis components and positions and allows overriding of AxisType.
        /// </summary>
        /// <param name="axisType">The AxisType to use in calculations.</param>
        private void AxisContentSizeChanged(AxisType axisType)
        {
            if (AxisContentCanvas != null)
            {
                switch (axisType)
                {
                    case AxisType.Linear:
                        LinearAxisAreaSizeChanged(AxisContentCanvasSize.Width, AxisContentCanvasSize.Height);
                        break;
                    case AxisType.Category:
                        CategoryAxisAreaSizeChanged(AxisContentCanvasSize.Width, AxisContentCanvasSize.Height);
                        break;
                    case AxisType.DateTime:
                        DateTimeAxisAreaSizeChanged(AxisContentCanvasSize.Width, AxisContentCanvasSize.Height);
                        break;
                }
            }
        }

        /// <summary>
        /// Recalculates Category axis components and positions.
        /// </summary>
        /// <param name="newWidth">New width of the canvas.</param>
        /// <param name="newHeight">New height of the canvas.</param>
        private void CategoryAxisAreaSizeChanged(double newWidth, double newHeight)
        {
            if (AxisContentCanvas != null)
            {
                AxisContentCanvas.Children.Clear();

                if (_categoryList.Count > 0)
                {
                    List<TextBlock> values = new List<TextBlock>();
                    double labelBucketWidth = newWidth / _categoryList.Count;
                    double labelBucketHeight = newHeight / _categoryList.Count;
                    for (int i = 0; i < _categoryList.Count; i++)
                    {
                        TextBlock value = new TextBlock();
                        value.Style = LabelStyle;

                        // Get the next category value
                        value.Text = FormatLabelValue(_categoryList[i]);

                        if (Orientation == AxisOrientation.Vertical)
                        {
                            double y = newHeight - ((i * labelBucketHeight) + (labelBucketHeight / 2));
                            value.SetValue(Canvas.TopProperty, y);
                        }
                        else if (Orientation == AxisOrientation.Horizontal)
                        {
                            double y = (i * labelBucketWidth) + (labelBucketWidth / 2);
                            value.SetValue(Canvas.LeftProperty, y);
                        }
                        AxisContentCanvas.Children.Add(value);
                        values.Add(value);
                    }
                    PopulateTickMarks(newWidth, newHeight);
                    AxisCanvasFinalAdjustmentPass(values);
                }
            }
        }

        /// <summary>
        /// Recalculates Linear axis components and positions.
        /// </summary>
        /// <param name="newWidth">New width of the canvas.</param>
        /// <param name="newHeight">New height of the canvas.</param>
        private void LinearAxisAreaSizeChanged(double newWidth, double newHeight)
        {
            if (AxisContentCanvas != null)
            {
                AxisContentCanvas.Children.Clear();
                List<TextBlock> values = new List<TextBlock>();
                if (ShowLabels)
                {
                    foreach (double currentValue in GetLabelsIntervals())
                    {
                        TextBlock value = new TextBlock();
                        value.Style = LabelStyle;
                        value.Text = FormatLabelValue(currentValue);
                        if (Orientation == AxisOrientation.Vertical)
                        {
                            value.SetValue(Canvas.TopProperty, newHeight - GetPlotAreaCoordinate(currentValue));
                        }
                        else if (Orientation == AxisOrientation.Horizontal)
                        {
                            value.SetValue(Canvas.LeftProperty, GetPlotAreaCoordinate(currentValue));
                        }

                        AxisContentCanvas.Children.Add(value);
                        values.Add(value);
                    }
                }
                PopulateTickMarks(newWidth, newHeight);
                AxisCanvasFinalAdjustmentPass(values);
            }
        }

        /// <summary>
        /// Formats the label value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The formatted label value.</returns>
        private string FormatLabelValue(object value)
        {
            IFormattable formatable = value as IFormattable;
            if (formatable != null)
            {
                string formatString = LabelStringFormat;
                if (AxisType == AxisType.DateTime && String.IsNullOrEmpty(formatString) && value is DateTime)
                {
                    switch (ActualIntervalType)
                    {
                        case AxisIntervalType.Years:
                            formatString = "yyyy";
                            break;
                        case AxisIntervalType.Hours:
                        case AxisIntervalType.Minutes:
                            formatString = "t";
                            break;
                        case AxisIntervalType.Seconds:
                            formatString = "T";
                            break;
                        case AxisIntervalType.Milliseconds:
                            formatString = "mm:ss.fff";
                            break;
                        default:
                            formatString = "d";
                            break;
                    }
                }
                return formatable.ToString(formatString, CultureInfo.CurrentCulture);
            }
            return value != null ? value.ToString() : String.Empty;
        }

        /// <summary>
        /// Populates the tick marks.
        /// </summary>
        /// <param name="newWidth">The new width of the axis.</param>
        /// <param name="newHeight">The new height of the axis.</param>
        private void PopulateTickMarks(double newWidth, double newHeight)
        {
            if (AxisContentCanvas != null && ShowTickMarks)
            {
                foreach (object currentValue in GetLinesIntervals())
                {
                    double position = AxisType == AxisType.Category ? GetDataPointCategoryByIndexInPixels((double)currentValue) : GetPlotAreaCoordinate(currentValue);
                    if (!double.IsNaN(position))
                    {
                        Line line = new Line();
                        line.Style = TickMarkStyle;

                        if (Orientation == AxisOrientation.Vertical)
                        {
                            line.Y1 = line.Y2 = Math.Round(newHeight - position);
                            line.X2 = Math.Round(TickMarksLength);
                            line.SetValue(Canvas.LeftProperty, Math.Round(newWidth - TickMarksLength));
                        }
                        else if (Orientation == AxisOrientation.Horizontal)
                        {
                            line.X1 = line.X2 = Math.Round(position);
                            line.Y2 = Math.Round(TickMarksLength);
                            line.VerticalAlignment = VerticalAlignment.Top;
                        }
                        AxisContentCanvas.Children.Add(line);
                    }
                }
            }
        }

        /// <summary>
        /// Recalculates DateTime axis components and positions.
        /// </summary>
        /// <param name="newWidth">New width of the canvas.</param>
        /// <param name="newHeight">New height of the canvas.</param>
        private void DateTimeAxisAreaSizeChanged(double newWidth, double newHeight)
        {
            if (AxisContentCanvas != null)
            {
                AxisContentCanvas.Children.Clear();

                List<TextBlock> values = new List<TextBlock>();
                if (ShowLabels)
                {
                    foreach (DateTime currentValue in GetLabelsIntervals())
                    {
                        TextBlock value = new TextBlock();
                        value.Text = FormatLabelValue(currentValue);
                        value.Style = LabelStyle;

                        if (Orientation == AxisOrientation.Vertical)
                        {
                            value.SetValue(Canvas.TopProperty, GetPlotAreaCoordinate(currentValue));
                        }
                        else if (Orientation == AxisOrientation.Horizontal)
                        {
                            value.SetValue(Canvas.LeftProperty, GetPlotAreaCoordinate(currentValue));
                        }

                        AxisContentCanvas.Children.Add(value);
                        values.Add(value);
                    }
                }
                PopulateTickMarks(newWidth, newHeight);
                AxisCanvasFinalAdjustmentPass(values);
            }
        }

        /// <summary>
        /// Adjusts the height and width of the Axis Canvas, if needed, based on the height and width of the list Labels passed in.
        /// </summary>
        /// <param name="values">The list of Labels to check when adjusting the Canvas.</param>
        private void AxisCanvasFinalAdjustmentPass(IList<TextBlock> values)
        {
            double tickMarksOffset = 0.0;
            if (ShowTickMarks)
            {
                tickMarksOffset = TickMarksLength;
            }
            if (Orientation == AxisOrientation.Vertical)
            {
                double maxvalwidth = 0.0;

                foreach (TextBlock value in values)
                {
                    maxvalwidth = Math.Max(maxvalwidth, value.ActualWidth + tickMarksOffset);
                }
                foreach (TextBlock value in values)
                {
                    value.SetValue(Canvas.LeftProperty, maxvalwidth - value.ActualWidth - tickMarksOffset);
                    value.SetValue(Canvas.TopProperty, (double)value.GetValue(Canvas.TopProperty) - (value.ActualHeight / 2));
                }
                if (AxisTitle != null && AxisTitle.Content != null && TitleCanvas != null)
                {
                    Width = Math.Min(maxvalwidth + tickMarksOffset + TitleCanvas.ActualWidth, MaxWidth);
                }
                else
                {
                    Width = Math.Min(maxvalwidth + tickMarksOffset, MaxWidth);
                }
            }
            else if (Orientation == AxisOrientation.Horizontal)
            {
                double maxvalheight = 0.0;
                bool overlappingLabels = false;
                double prevRightPosition = -1;
                double oddValue = 1;
                foreach (TextBlock value in values)
                {
                    maxvalheight = Math.Max(maxvalheight, value.ActualHeight);
                    if (prevRightPosition > 0)
                    {
                        overlappingLabels = (prevRightPosition + 4) >= (double)value.GetValue(Canvas.LeftProperty);
                    }
                    prevRightPosition = (double)value.GetValue(Canvas.LeftProperty) + value.ActualWidth;
                }

                foreach (TextBlock value in values)
                {
                    if (overlappingLabels && ((oddValue++ % 2) == 0))
                    {
                        value.SetValue(Canvas.TopProperty, maxvalheight * 2 - value.ActualHeight + tickMarksOffset);
                    }
                    else
                    {
                        value.SetValue(Canvas.TopProperty, maxvalheight - value.ActualHeight + tickMarksOffset);
                    }
                    value.SetValue(Canvas.LeftProperty, (double)value.GetValue(Canvas.LeftProperty) - (value.ActualWidth / 2));
                }
                if (AxisTitle != null && AxisTitle.Content != null && TitleCanvas != null)
                {
                    Height = Math.Min(maxvalheight * (overlappingLabels ? 2 : 1) + tickMarksOffset + TitleCanvas.ActualHeight, MaxHeight);
                }
                else
                {
                    Height = Math.Min(maxvalheight * (overlappingLabels ? 2 : 1) + tickMarksOffset, MaxHeight);
                }
            }
        }

        /// <summary>
        /// Returns the pixel offset coordinate for the specified value on 
        /// the axis for the value.
        /// </summary>
        /// <param name="value">The value to get a pixel offset coordinate for.
        /// </param>
        /// <returns>The pixel offset coordinate for the specified value on the 
        /// supplied value's axis or double.NaN if an error occurs.</returns>
        public double GetPlotAreaCoordinate(object value)
        {
            if (value != null && AxisContentCanvas != null && (AxisType == AxisType.Linear || AxisType == AxisType.DateTime))
            {
                Range<double> axisRange = AxisRange;
                double range = Math.Abs(axisRange.Maximum - axisRange.Minimum);
                double doubleValue;
                if (AxisType == AxisType.Linear)
                {
                    if (!Double.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.CurrentCulture, out doubleValue))
                    {
                        throw new ArgumentException(Properties.Resources.Axis_LinearAxis_NonDoubleArgument);
                    }
                }
                else
                {
                    doubleValue = ((DateTime)value).ToOADate();
                }

                if (range > 0)
                {
                    double delta = doubleValue - axisRange.Minimum;
                    if (Orientation == AxisOrientation.Vertical)
                    {
                        return AxisContentCanvasSize.Height * (delta / range);
                    }
                    else if (Orientation == AxisOrientation.Horizontal)
                    {
                        return AxisContentCanvasSize.Width * (delta / range);
                    }
                }
                else
                {
                    return 0;
                }
            }
            else if (AxisType == AxisType.Category && AxisContentCanvas != null && null != value)
            {
                int index = _categoryList.IndexOf(value);
                if (index != -1)
                {
                    return GetDataPointCategoryByIndexInPixels(index);
                }
            }
            return double.NaN;
        }

        /// <summary>
        /// Gets the data point category by index in pixels.
        /// </summary>
        /// <param name="value">The category index.</param>
        /// <returns>Pixel position along the axis.</returns>
        internal double GetDataPointCategoryByIndexInPixels(double value)
        {
            if (AxisContentCanvas != null && (AxisType == AxisType.Category))
            {
                if (Orientation == AxisOrientation.Vertical && AxisContentCanvasSize.Height > 0)
                {
                    double bucket = AxisContentCanvasSize.Height / _categoryList.Count;
                    return AxisContentCanvasSize.Height - ((value * bucket) + bucket / 2);
                }
                else if (AxisContentCanvasSize.Width > 0)
                {
                    double bucket = AxisContentCanvasSize.Width / _categoryList.Count;
                    return (double)value * bucket + bucket / 2;
                }
            }
            return double.NaN;
        }

        /// <summary>
        /// Gets a value indicating whether the axis is being used by any 
        /// series.
        /// </summary>
        internal bool IsUsed
        {
            get { return RegisteredSeries.Count > 0; }
        }

        /// <summary>
        /// Indicates that series is using an axis.
        /// </summary>
        /// <param name="series">The series using the axis.</param>
        internal void Register(IAxisInformationProvider series)
        {
            Debug.Assert(series != null, "series cannot be null.");
            Debug.Assert(!RegisteredSeries.Contains(series), "series has already been registered with the axis.");

            RegisteredSeries.Add(series);

            if (AxisType == AxisType.Linear)
            {
                RecalculateMinAndMax();
            }
            else if (AxisType == AxisType.DateTime)
            {
                RecalculateMinAndMaxDates();
            }
            else if (AxisType == AxisType.Category)
            {
                CreateCategoryCollection();
            }
        }

        /// <summary>
        /// Indicates that a series is no longer using an axis.
        /// </summary>
        /// <param name="series">The series no longer using the axis.</param>
        internal void Unregister(IAxisInformationProvider series)
        {
            Debug.Assert(series != null, "series cannot be null.");
            Debug.Assert(RegisteredSeries.Contains(series), "series is not registered with the axis.");

            RegisteredSeries.Remove(series);

            if (AxisType == AxisType.Linear)
            {
                RecalculateMinAndMax();
            }
            else if (AxisType == AxisType.DateTime)
            {
                RecalculateMinAndMaxDates();
            }
            else if (AxisType == AxisType.Category)
            {
                CreateCategoryCollection();
            }
        }

        /// <summary>
        /// Request that the axis plot a collection of categories.
        /// </summary>
        /// <param name="objectRequestingCategories">The object that is 
        /// requesting that categories be plotted.</param>
        /// <param name="categories">The categories to add.</param>
        public void Update(IAxisInformationProvider objectRequestingCategories, IEnumerable<object> categories)
        {
            if (objectRequestingCategories == null)
            {
                throw new ArgumentNullException("objectRequestingCategories");
            }
            if (categories == null)
            {
                throw new ArgumentNullException("categories");
            }
            if (this.AxisType != AxisType.Category)
            {
                throw new InvalidOperationException(Properties.Resources.Axis_AxisTypeIsNotCategory);
            }
            CreateCategoryCollection();
        }

        /// <summary>
        /// Request that the axis add a date time range.
        /// </summary>
        /// <param name="objectRequestingDateTimeRange">The object that is 
        /// requesting that the axis plot a date time range.</param>
        /// <param name="range">The date time range to add.</param>
        public void Update(IAxisInformationProvider objectRequestingDateTimeRange, Range<DateTime> range)
        {
            if (objectRequestingDateTimeRange == null)
            {
                throw new ArgumentNullException("objectRequestingDateTimeRange");
            }
            if (this.AxisType != AxisType.DateTime)
            {
                throw new InvalidOperationException(Properties.Resources.Axis_AxisTypeIsNotDateTime);
            }
            
            _seriesRanges[objectRequestingDateTimeRange] = !range.IsEmpty ? new Range<double>(range.Minimum.ToOADate(), range.Maximum.ToOADate()) : new Range<double>();
            
            Range<double> newRange = _seriesRanges.Select(item => item.Value).Sum();
            double maxMargin = ActualInterval / 4;

            // to avoid axis jumping we put these restrictions:
            // 1) expand axis is new range is out of axis range
            // 2) shrink axis if new range is under of two intervals + interval/4 of axis range
            if (!newRange.IsEmpty)
            {
                if (newRange.Maximum > GetMinMaxAsDouble(ActualMaximum) || newRange.Minimum < GetMinMaxAsDouble(ActualMinimum))
                {
                    RecalculateMinAndMaxDates();
                }
                else if (newRange.Maximum < (GetMinMaxAsDouble(ActualMaximum) - maxMargin - ActualInterval * 2) || newRange.Minimum > (GetMinMaxAsDouble(ActualMinimum) + maxMargin + ActualInterval * 2))
                {
                    RecalculateMinAndMaxDates();
                }
            }
            else if (!_dataRange.IsEmpty)
            {
                RecalculateMinAndMaxDates();
            }
        }

        /// <summary>
        /// Request that the axis add a numeric range.
        /// </summary>
        /// <param name="objectRequestingNumericRange">The object that is 
        /// requesting that the axis plot a numeric range be plotted.</param>
        /// <param name="range">The numeric range to add.</param>
        public void Update(IAxisInformationProvider objectRequestingNumericRange, Range<double> range)
        {
            if (objectRequestingNumericRange == null)
            {
                throw new ArgumentNullException("objectRequestingNumericRange");
            }
            if (this.AxisType != AxisType.Linear)
            {
                throw new InvalidOperationException(Properties.Resources.Axis_AxisTypeIsNotLinear);
            }
            
            _seriesRanges[objectRequestingNumericRange] = range;
            Range<double> newRange = _seriesRanges.Select(item => item.Value).Sum();
            double maxMargin = ActualInterval / 4;
            
            // to avoid axis jumping we put these restrictions:
            // 1) expand axis is new range is out of axis range
            // 2) shrink axis if new range is under of two intervals + interval/4 of axis range
            if (!newRange.IsEmpty)
            {
                if (newRange.Maximum > GetMinMaxAsDouble(ActualMaximum) || newRange.Minimum < GetMinMaxAsDouble(ActualMinimum))
                {
                    RecalculateMinAndMax();
                }
                else if (newRange.Maximum < (GetMinMaxAsDouble(ActualMaximum) - maxMargin - ActualInterval * 2) || newRange.Minimum > (GetMinMaxAsDouble(ActualMinimum) + maxMargin + ActualInterval * 2))
                {
                    RecalculateMinAndMax();
                }
            }
            else if (!_dataRange.IsEmpty)
            {
                RecalculateMinAndMaxDates();
            }
        }

        /// <summary>
        /// Gets the range of the Axis.
        /// </summary>
        /// <value>The range.</value>
        internal Range<double> AxisRange
        {
            get
            {
                this.EnsureAxisIsCalculated();
                return _axisRange;
            }
        }

        /// <summary>
        /// Specifies the axis minimum and maximum.
        /// </summary>
        private Range<double> _axisRange = new Range<double>();

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>The interval.</value>
        public double ActualInterval
        {
            get
            {
                this.EnsureAxisIsCalculated();
                return _actualInterval;
            }
        }

        /// <summary>
        /// Specifies the calculated interval.
        /// </summary>
        private double _actualInterval = Double.NaN;

        /// <summary>
        /// Gets the type of the interval.
        /// </summary>
        /// <value>The interval type.</value>
        public AxisIntervalType ActualIntervalType
        {
            get
            {
                this.EnsureAxisIsCalculated();
                return _actualIntervalType;
            }
        }

        /// <summary>
        /// Specifies a reference to ActualIntervalType property.
        /// </summary>
        private AxisIntervalType _actualIntervalType = AxisIntervalType.Auto;

        /// <summary>
        /// Gets the type of the interval offset.
        /// </summary>
        /// <value>The interval offset type.</value>
        public AxisIntervalType ActualIntervalOffsetType
        {
            get
            {
                return IntervalType == AxisIntervalType.Auto ? ActualIntervalType : IntervalOffsetType;
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        internal void ResetIntervals()
        {
            _axisRange = new Range<double>();
            ActualMinimum = null;
            ActualMaximum = null;
            _actualInterval = Double.NaN;
            _actualIntervalType = AxisIntervalType.Auto;
        }

        /// <summary>
        /// Gets the labels intervals.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable"/> which contains intervals.
        /// </returns>
        private IEnumerable<IComparable> GetLabelsIntervals()
        {
            return GetIntervals(0);
        }

        /// <summary>
        /// Gets the tick marks and gridlines intervals.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable"/> which contains intervals.
        /// </returns>
        internal IEnumerable<IComparable> GetLinesIntervals()
        {
            return GetIntervals(AxisType == AxisType.Category ? ActualInterval / 2 : 0.0);
        }

        /// <summary>
        /// Gets the intervals.
        /// </summary>
        /// <param name="categoryOffset">The category offset.</param>
        /// <returns>
        /// An <see cref="IEnumerable"/> which contains intervals.
        /// </returns>
        private IEnumerable<IComparable> GetIntervals(double categoryOffset)
        {
            if (!_dataRange.IsEmpty || _categoryList.Count > 0)
            {
                double interval = ActualInterval;
                Range<double> range = AxisRange;
                if (AxisType != AxisType.DateTime)
                {
                    for (double value = range.Minimum + IntervalOffset - categoryOffset;
                        value <= range.Maximum + categoryOffset;
                        value = Axis.RemoveNoiseFromDoubleMath(value + interval))
                    {
                        yield return value;
                    }
                }
                else
                {
                    double offset = GetNextPosition(0, true, false, IntervalOffset, ActualIntervalOffsetType);

                    for (double value = range.Minimum + offset;
                         value <= range.Maximum;
                         value = GetNextPosition(value, true, false, ActualInterval, ActualIntervalType))
                    {
                        yield return DateTime.FromOADate(value);
                    }
                }
            }
        }
        
        /// <summary>
        /// Ensures that the axis interval is calculated.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "It will be not readable if the method is optimized more.")]
        private void EnsureAxisIsCalculated()
        {
            if (_axisRange.IsEmpty || Double.IsNaN(_actualInterval) || _actualIntervalType == AxisIntervalType.Auto)
            {
                bool minIsFixedZero = false;
                bool maxIsFixedZero = false;
                double tempMinimum = DataRange.Minimum;
                double tempMaximum = DataRange.Maximum;
                
                // For category axis auto interval is 1.
                if (AxisType == AxisType.Category)
                {
                    _actualInterval = !Interval.HasValue ? 1 : Interval.Value;
                    _actualIntervalType = AxisIntervalType.Number;
                    _axisRange = new Range<double>(tempMinimum, tempMaximum);
                    SetActualMinMax(_axisRange);
                    return;
                }
                
                if (tempMinimum == tempMaximum)
                {
                    tempMinimum--;
                    tempMaximum++;
                }
                if (AxisType == AxisType.Linear)
                {
                    // fix range depending on ShouldIncludeZero flag. Auto is true for Depended axis.
                    if (ShouldIncludeZero)
                    {
                        if (Minimum == null && tempMinimum >= 0.0)
                        {
                            tempMinimum = 0.0;
                            minIsFixedZero = true;
                        }
                        if (Maximum == null && tempMaximum <= 0.0)
                        {
                            tempMaximum = 0.0;
                            maxIsFixedZero = true;
                        }
                    }
                }

                _actualInterval = !Interval.HasValue ? Double.NaN : Interval.Value;
                _actualIntervalType = IntervalType;
                if (Double.IsNaN(_actualInterval))
                {
                    _actualInterval = DetermineAutoInterval(tempMinimum, tempMaximum);
                }
                else if (_actualIntervalType == AxisIntervalType.Auto)
                {
                    if (AxisType == AxisType.DateTime)
                    {
                        DetermineAutoInterval(tempMinimum, tempMaximum);
                    }
                    else
                    {
                        _actualIntervalType = AxisIntervalType.Number;
                    }
                }

                if (Minimum == null)
                {
                    // align minimum
                    if (AxisType == AxisType.Linear)
                    {
                        tempMinimum = GetNextPosition(tempMinimum, false, true, _actualInterval, _actualIntervalType);
                    }
                    else if (AxisType == AxisType.DateTime)
                    {
                        tempMinimum = AlignIntervalStart(tempMinimum, _actualInterval, _actualIntervalType);
                    }
                }

                if (Maximum == null)
                {
                    // align maximum
                    tempMaximum = GetNextPosition(tempMaximum, true, true, _actualInterval, _actualIntervalType);
                }

                if (AxisType != AxisType.Category)
                {
                    double margin = 0;
                    if (AxisType == AxisType.Linear)
                    {
                        margin = _actualInterval;
                    }
                    else if (AxisType == AxisType.DateTime)
                    {
                        margin = GetNextPosition(0, true, false, 1, _actualIntervalType);
                    }
                    // expand with one interval if datapoins are too close to the end of axis.
                    if (!minIsFixedZero && Minimum == null && tempMinimum > (DataRange.Minimum - _actualInterval / 3))
                    {
                        // consider using margins: Margin = margin;
                        tempMinimum -= margin;
                    }
                    if (!maxIsFixedZero && Maximum == null && tempMaximum < (DataRange.Maximum + _actualInterval / 3))
                    {
                        tempMaximum += margin;
                    }
                }

                _axisRange = new Range<double>(tempMinimum, tempMaximum);
                SetActualMinMax(_axisRange);
            }
        }

        /// <summary>
        /// Gets the next position.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="forward">If set to <c>true</c> [the next position is calculated forward, otherwise backward..</param>
        /// <param name="alignToInterval">If set to <c>true</c> the position will be aligned to the interval.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="intervalType">Type of the interval.</param>
        /// <returns>The next position related to the current position.</returns>
        internal double GetNextPosition(double current, bool forward, bool alignToInterval, double interval, AxisIntervalType intervalType)
        {
            double result = current;
            if (AxisType != AxisType.DateTime)
            {
                if (!alignToInterval)
                {
                    double sign = forward ? -1d : 1d;
                    result = Axis.RemoveNoiseFromDoubleMath(current + Axis.RemoveNoiseFromDoubleMath(sign * interval));
                }
                else
                {
                    if (forward)
                    {
                        result = Axis.RemoveNoiseFromDoubleMath(Axis.RemoveNoiseFromDoubleMath(Math.Ceiling(current / interval)) * interval);
                    }
                    else
                    {
                        result = Axis.RemoveNoiseFromDoubleMath(Axis.RemoveNoiseFromDoubleMath(Math.Floor(current / interval)) * interval);
                    }
                }
            }
            else
            {
                DateTime date = DateTime.FromOADate(current);
                TimeSpan span = new TimeSpan(0);

                if (intervalType == AxisIntervalType.Days)
                {
                    span = TimeSpan.FromDays(interval);
                }
                else if (intervalType == AxisIntervalType.Hours)
                {
                    span = TimeSpan.FromHours(interval);
                }
                else if (intervalType == AxisIntervalType.Milliseconds)
                {
                    span = TimeSpan.FromMilliseconds(interval);
                }
                else if (intervalType == AxisIntervalType.Seconds)
                {
                    span = TimeSpan.FromSeconds(interval);
                }
                else if (intervalType == AxisIntervalType.Minutes)
                {
                    span = TimeSpan.FromMinutes(interval);
                }
                else if (intervalType == AxisIntervalType.Weeks)
                {
                    span = TimeSpan.FromDays(7.0 * interval);
                }
                else if (intervalType == AxisIntervalType.Months)
                {
                    // Special case handling when current date point
                    // to the last day of the month
                    bool lastMonthDay = false;
                    if (date.Day == DateTime.DaysInMonth(date.Year, date.Month))
                    {
                        lastMonthDay = true;
                    }

                    // Add specified amount of months
                    date = date.AddMonths((int)Math.Floor(interval));
                    span = TimeSpan.FromDays(30.0 * (interval - Math.Floor(interval)));

                    // Check if last month of the day was used
                    if (lastMonthDay && span.Ticks == 0)
                    {
                        // Make sure the last day of the month is selected
                        int daysInMobth = DateTime.DaysInMonth(date.Year, date.Month);
                        date = date.AddDays(daysInMobth - date.Day);
                    }
                }
                else if (intervalType == AxisIntervalType.Years)
                {
                    date = date.AddYears((int)Math.Floor(interval));
                    span = TimeSpan.FromDays(365.0 * (interval - Math.Floor(interval)));
                }

                result = date.Add(span).ToOADate();
                if (alignToInterval)
                {
                    result = AlignIntervalStart(result, interval, intervalType);
                }
            }
            return result;
        }

        /// <summary>
        /// Adjusts the beginning of the first interval depending on the type and size.
        /// </summary>
        /// <param name="start">Original start point.</param>
        /// <param name="intervalSize">Interval size.</param>
        /// <param name="type">Type of the interval (Month, Year, ...).</param>
        /// <returns>
        /// Adjusted interval start position.
        /// </returns>
        private static double AlignIntervalStart(double start, double intervalSize, AxisIntervalType type)
        {
            // Do not adjust start position for these interval type
            if (type == AxisIntervalType.Auto ||
                type == AxisIntervalType.Number)
            {
                return start;
            }

            // Get the beginning of the interval depending on type
            DateTime newStartDate = DateTime.FromOADate(start);

            // Adjust the months interval depending on size
            if (intervalSize > 0.0 && intervalSize != 1.0)
            {
                if (type == AxisIntervalType.Months && intervalSize <= 12.0 && intervalSize > 1)
                {
                    // Make sure that the beginning is aligned correctly for cases
                    // like quarters and half years
                    DateTime resultDate = newStartDate;
                    DateTime sizeAdjustedDate = new DateTime(newStartDate.Year, 1, 1, 0, 0, 0);
                    while (sizeAdjustedDate < newStartDate)
                    {
                        resultDate = sizeAdjustedDate;
                        sizeAdjustedDate = sizeAdjustedDate.AddMonths((int)intervalSize);
                    }

                    newStartDate = resultDate;
                    return newStartDate.ToOADate();
                }
            }

            // Check interval type
            switch (type)
            {
                case AxisIntervalType.Years:
                    int year = (int)((int)(newStartDate.Year / intervalSize) * intervalSize);
                    if (year <= 0)
                    {
                        year = 1;
                    }
                    newStartDate = new DateTime(year, 1, 1, 0, 0, 0);
                    break;

                case AxisIntervalType.Months:
                    int month = (int)((int)(newStartDate.Month / intervalSize) * intervalSize);
                    if (month <= 0)
                    {
                        month = 1;
                    }
                    newStartDate = new DateTime(newStartDate.Year, month, 1, 0, 0, 0);
                    break;

                case AxisIntervalType.Days:
                    int day = (int)((int)(newStartDate.Day / intervalSize) * intervalSize);
                    if (day <= 0)
                    {
                        day = 1;
                    }
                    newStartDate = new DateTime(newStartDate.Year, newStartDate.Month, day, 0, 0, 0);
                    break;

                case AxisIntervalType.Hours:
                    int hour = (int)((int)(newStartDate.Hour / intervalSize) * intervalSize);
                    newStartDate = new DateTime(
                        newStartDate.Year,
                        newStartDate.Month,
                        newStartDate.Day,
                        hour,
                        0,
                        0);
                    break;

                case AxisIntervalType.Minutes:
                    int minute = (int)((int)(newStartDate.Minute / intervalSize) * intervalSize);
                    newStartDate = new DateTime(
                        newStartDate.Year,
                        newStartDate.Month,
                        newStartDate.Day,
                        newStartDate.Hour,
                        minute,
                        0);
                    break;

                case AxisIntervalType.Seconds:
                    int second = (int)((int)(newStartDate.Second / intervalSize) * intervalSize);
                    newStartDate = new DateTime(
                        newStartDate.Year,
                        newStartDate.Month,
                        newStartDate.Day,
                        newStartDate.Hour,
                        newStartDate.Minute,
                        second,
                        0);
                    break;

                case AxisIntervalType.Milliseconds:
                    int milliseconds = (int)((int)(newStartDate.Millisecond / intervalSize) * intervalSize);
                    newStartDate = new DateTime(
                        newStartDate.Year,
                        newStartDate.Month,
                        newStartDate.Day,
                        newStartDate.Hour,
                        newStartDate.Minute,
                        newStartDate.Second,
                        milliseconds);
                    break;

                case AxisIntervalType.Weeks:

                    // Elements that have interval set to weeks should be aligned to the 
                    // nearest start of week no matter how many weeks is the interval.
                    // newStartDate = newStartDate.AddDays(-((int)newStartDate.DayOfWeek * intervalSize));
                    newStartDate = newStartDate.AddDays(-((int)newStartDate.DayOfWeek));
                    newStartDate = new DateTime(
                        newStartDate.Year,
                        newStartDate.Month,
                        newStartDate.Day,
                        0,
                        0,
                        0);
                    break;
            }

            return newStartDate.ToOADate();
        }

        /// <summary>
        /// Gets the length of the axis.
        /// </summary>
        /// <value>The length of the axis.</value>
        private double AxisLength
        {
            get
            {
                if (Orientation == AxisOrientation.Horizontal)
                {
                    if (ActualWidth > 0)
                    {
                        return ActualWidth;
                    }
                    if (Width > 0)
                    {
                        return Width;
                    }
                }
                else if (Orientation == AxisOrientation.Vertical)
                {
                    if (ActualHeight > 0)
                    {
                        return ActualHeight;
                    }
                    if (Height > 0)
                    {
                        return Height;
                    }
                }
                return 200;
            }
        }

        /// <summary>
        /// Determines the interval.
        /// </summary>
        /// <param name="minimum">The data minimum.</param>
        /// <param name="maximum">The data maximum.</param>
        /// <returns>Calculated interval.</returns>
        internal double DetermineAutoInterval(double minimum, double maximum)
        {
            if (AxisType == AxisType.Category)
            {
                _actualIntervalType = AxisIntervalType.Number;
                return 1;
            }
            else if (AxisType == AxisType.Linear)
            {
                // helper functions
                Func<double, double> Exponent = x => Math.Ceiling(Math.Log(x, 10));
                Func<double, double> Mantissa = x => x / Math.Pow(10, Exponent(x) - 1);

                _actualIntervalType = AxisIntervalType.Number;

                // reduce intervals for horizontal axis.
                double maxIntervals = Orientation == AxisOrientation.Horizontal ? MaximumAxisIntervals * 0.8 : MaximumAxisIntervals;
                // real maximum interval count
                double maxIntervalCount = AxisLength / 200 * maxIntervals;

                double range = Math.Abs(minimum - maximum);
                double interval = Math.Pow(10, Exponent(range));
                double tempInterval = interval;

                // decrease interval until interval count becomes less than maxIntervalCount
                while (true)
                {
                    int mantissa = (int)Mantissa(tempInterval);
                    if (mantissa == 5)
                    {
                        // reduce 5 to 2
                        tempInterval = Axis.RemoveNoiseFromDoubleMath(tempInterval / 2.5);
                    }
                    else if (mantissa == 2 || mantissa == 1 || mantissa == 10)
                    {
                        // reduce 2 to 1,10 to 5,1 to 0.5
                        tempInterval = Axis.RemoveNoiseFromDoubleMath(tempInterval / 2.0);
                    }
                    else
                    {
                        break;
                    }

                    if (range / tempInterval > maxIntervalCount)
                    {
                        break;
                    }

                    interval = tempInterval;
                }
                return interval;
            }
            else
            {
                return CalculateDateTimeInterval(minimum, maximum, out _actualIntervalType);
            }
        }

        /// <summary>
        /// Recalculates a DateTime interval obtained from maximum and minimum.
        /// </summary>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="type">Date time interval type.</param>
        /// <returns>Auto Interval.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The method should inspect all variations of time span (millisec to year) and contains long case. Otherwise is simple and readable.")]
        private double CalculateDateTimeInterval(double minimum, double maximum, out AxisIntervalType type)
        {
            DateTime dateTimeMin = DateTime.FromOADate(minimum);
            DateTime dateTimeMax = DateTime.FromOADate(maximum);
            TimeSpan timeSpan = dateTimeMax.Subtract(dateTimeMin);

            // this algorithm is designed to return close to 10 intervals.
            // we need to align the time span for PrefferedNumberOfIntervals
            double maxIntervals = Orientation == AxisOrientation.Horizontal ? MaximumAxisIntervals * 0.8 : MaximumAxisIntervals;
            double rangeMultiplicator = AxisLength / (200 * 10 / maxIntervals);
            timeSpan = new TimeSpan((long)((double)timeSpan.Ticks / rangeMultiplicator));

            // Minutes
            double inter = timeSpan.TotalMinutes;

            // For Range less than 60 seconds interval is 5 sec
            if (inter <= 1.0)
            {
                // Milli Seconds
                double milliSeconds = timeSpan.TotalMilliseconds;
                if (milliSeconds <= 10)
                {
                    type = AxisIntervalType.Milliseconds;
                    return 1;
                }
                if (milliSeconds <= 50)
                {
                    type = AxisIntervalType.Milliseconds;
                    return 4;
                }
                if (milliSeconds <= 200)
                {
                    type = AxisIntervalType.Milliseconds;
                    return 20;
                }
                if (milliSeconds <= 500)
                {
                    type = AxisIntervalType.Milliseconds;
                    return 50;
                }

                // Seconds
                double seconds = timeSpan.TotalSeconds;

                if (seconds <= 7)
                {
                    type = AxisIntervalType.Seconds;
                    return 1;
                }
                else if (seconds <= 15)
                {
                    type = AxisIntervalType.Seconds;
                    return 2;
                }
                else if (seconds <= 30)
                {
                    type = AxisIntervalType.Seconds;
                    return 5;
                }
                else if (seconds <= 60)
                {
                    type = AxisIntervalType.Seconds;
                    return 10;
                }
            }
            else if (inter <= 2.0)
            {
                // For Range less than 120 seconds interval is 10 sec
                type = AxisIntervalType.Seconds;
                return 20;
            }
            else if (inter <= 3.0)
            {
                // For Range less than 180 seconds interval is 30 sec
                type = AxisIntervalType.Seconds;
                return 30;
            }
            else if (inter <= 10)
            {
                // For Range less than 10 minutes interval is 1 min
                type = AxisIntervalType.Minutes;
                return 1;
            }
            else if (inter <= 20)
            {
                // For Range less than 20 minutes interval is 1 min
                type = AxisIntervalType.Minutes;
                return 2;
            }
            else if (inter <= 60)
            {
                // For Range less than 60 minutes interval is 5 min
                type = AxisIntervalType.Minutes;
                return 5;
            }
            else if (inter <= 120)
            {
                // For Range less than 120 minutes interval is 10 min
                type = AxisIntervalType.Minutes;
                return 10;
            }
            else if (inter <= 180)
            {
                // For Range less than 180 minutes interval is 30 min
                type = AxisIntervalType.Minutes;
                return 30;
            }
            else if (inter <= 60 * 12)
            {
                // For Range less than 12 hours interval is 1 hour
                type = AxisIntervalType.Hours;
                return 1;
            }
            else if (inter <= 60 * 24)
            {
                // For Range less than 24 hours interval is 4 hour
                type = AxisIntervalType.Hours;
                return 4;
            }
            else if (inter <= 60 * 24 * 2)
            {
                // For Range less than 2 days interval is 6 hour
                type = AxisIntervalType.Hours;
                return 6;
            }
            else if (inter <= 60 * 24 * 3)
            {
                // For Range less than 3 days interval is 12 hour
                type = AxisIntervalType.Hours;
                return 12;
            }
            else if (inter <= 60 * 24 * 10)
            {
                // For Range less than 10 days interval is 1 day
                type = AxisIntervalType.Days;
                return 1;
            }
            else if (inter <= 60 * 24 * 20)
            {
                // For Range less than 20 days interval is 2 day
                type = AxisIntervalType.Days;
                return 2;
            }
            else if (inter <= 60 * 24 * 30)
            {
                // For Range less than 30 days interval is 3 day
                type = AxisIntervalType.Days;
                return 3;
            }
            else if (inter <= 60 * 24 * 30.5 * 2)
            {
                // For Range less than 2 months interval is 1 week
                type = AxisIntervalType.Weeks;
                return 1;
            }
            else if (inter <= 60 * 24 * 30.5 * 5)
            {
                // For Range less than 5 months interval is 2weeks
                type = AxisIntervalType.Weeks;
                return 2;
            }
            else if (inter <= 60 * 24 * 30.5 * 12)
            {
                // For Range less than 12 months interval is 1 month
                type = AxisIntervalType.Months;
                return 1;
            }
            else if (inter <= 60 * 24 * 30.5 * 24)
            {
                // For Range less than 24 months interval is 3 month
                type = AxisIntervalType.Months;
                return 3;
            }
            else if (inter <= 60 * 24 * 30.5 * 48)
            {
                // For Range less than 48 months interval is 6 months 
                type = AxisIntervalType.Months;
                return 6;
            }

            // For Range more than 48 months interval is year 
            type = AxisIntervalType.Years;
            double years = inter / 60 / 24 / 365;
            if (years < 5)
            {
                return 1;
            }
            else if (years < 10)
            {
                return 2;
            }

            // Make a correction of the interval
            return Math.Floor(years / 5);
        }

        /// <summary>
        /// Removes the noise from double math.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A double without a noise.</returns>
        internal static double RemoveNoiseFromDoubleMath(double value)
        {
            if (value == 0.0 || Math.Abs((Math.Log10(Math.Abs(value)))) < 27)
            {
                return (double)((decimal)value);
            }
            return Double.Parse(value.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }
    }
}
