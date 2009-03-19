//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Controls
{
    using System.Windows.Controls.Common;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Separator for the DataForm.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormSeparator : DataFormField
    {
        /// <summary>
        /// Identifies the LineStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register(
                "LineStyle",
                typeof(Style),
                typeof(DataFormSeparator),
                new PropertyMetadata(OnLineStylePropertyChanged));

        /// <summary>
        /// Identifies the Stroke dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                "Stroke",
                typeof(Brush),
                typeof(DataFormSeparator),
                new PropertyMetadata(new SolidColorBrush(Colors.Black), OnStrokePropertyChanged));

        /// <summary>
        /// Identifies the StrokeDashArray dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(
                "StrokeDashArray",
                typeof(DoubleCollection),
                typeof(DataFormSeparator),
                new PropertyMetadata(OnStrokeDashArrayPropertyChanged));

        /// <summary>
        /// Identifies the StrokeDashOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeDashOffsetProperty =
            DependencyProperty.Register(
                "StrokeDashOffset",
                typeof(double),
                typeof(DataFormSeparator),
                new PropertyMetadata(OnStrokeDashOffsetPropertyChanged));

        /// <summary>
        /// Identifies the StrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                "StrokeThickness",
                typeof(double),
                typeof(DataFormSeparator),
                new PropertyMetadata(1.0, OnStrokeThicknessPropertyChanged));

        /// <summary>
        /// The line that was created.
        /// </summary>
        Line _line;

        /// <summary>
        /// The presenter for the line.  Used to determine how large
        /// to make the line.
        /// </summary>
        ContentPresenter _linePresenter;

        /// <summary>
        /// Constructs a new DataFormSeparator.
        /// </summary>
        public DataFormSeparator()
        {
        }

        /// <summary>
        /// Gets or sets the line style to use for this separator.
        /// </summary>
        public Style LineStyle
        {
            get
            {
                return this.GetValue(LineStyleProperty) as Style;
            }

            set
            {
                this.SetValue(LineStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke to use for this separator.
        /// </summary>
        public Brush Stroke
        {
            get
            {
                return this.GetValue(StrokeProperty) as Brush;
            }

            set
            {
                this.SetValue(StrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke dash array to use for this separator.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The property should be settable.")]
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return this.GetValue(StrokeDashArrayProperty) as DoubleCollection;
            }

            set
            {
                this.SetValue(StrokeDashArrayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke dash offset to use for this separator.
        /// </summary>
        public double StrokeDashOffset
        {
            get
            {
                return (double)this.GetValue(StrokeDashOffsetProperty);
            }

            set
            {
                this.SetValue(StrokeDashOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke thickness to use for this separator.
        /// </summary>
        public double StrokeThickness
        {
            get
            {
                return (double)this.GetValue(StrokeThicknessProperty);
            }

            set
            {
                this.SetValue(StrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Generates the display input control.
        /// </summary>
        /// <returns>The display input control.</returns>
        protected override FrameworkElement GenerateElement()
        {
            return this.GenerateSeparator();
        }

        /// <summary>
        /// Generates the edit input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The edit input control.</returns>
        protected override FrameworkElement GenerateEditingElement(bool isReadOnly)
        {
            return this.GenerateSeparator();
        }

        /// <summary>
        /// Generates the insert input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The insert input control.</returns>
        protected override FrameworkElement GenerateInsertElement(bool isReadOnly)
        {
            return this.GenerateSeparator();
        }

        /// <summary>
        /// LineStyle property changed handler.
        /// </summary>
        /// <param name="d">DataFormSeparator that changed its LineStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormSeparator separator = d as DataFormSeparator;
            if (separator != null && !separator.AreHandlersSuspended())
            {
                if (separator._line != null)
                {
                    separator._line.Style = separator.LineStyle;
                }
            }
        }

        /// <summary>
        /// Stroke property changed handler.
        /// </summary>
        /// <param name="d">DataFormSeparator that changed its Stroke value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormSeparator separator = d as DataFormSeparator;
            if (separator != null && !separator.AreHandlersSuspended() && separator._line != null)
            {
                separator._line.Stroke = separator.Stroke;
            }
        }

        /// <summary>
        /// StrokeDashArray property changed handler.
        /// </summary>
        /// <param name="d">DataFormSeparator that changed its StrokeDashArray value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnStrokeDashArrayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormSeparator separator = d as DataFormSeparator;
            if (separator != null && !separator.AreHandlersSuspended() && separator._line != null)
            {
                separator._line.StrokeDashArray = separator.StrokeDashArray;
            }
        }

        /// <summary>
        /// StrokeDashOffset property changed handler.
        /// </summary>
        /// <param name="d">DataFormSeparator that changed its StrokeDashOffset value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnStrokeDashOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormSeparator separator = d as DataFormSeparator;
            if (separator != null && !separator.AreHandlersSuspended() && separator._line != null)
            {
                separator._line.StrokeDashOffset = separator.StrokeDashOffset;
            }
        }

        /// <summary>
        /// StrokeThickness property changed handler.
        /// </summary>
        /// <param name="d">DataFormSeparator that changed its StrokeThickness value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnStrokeThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormSeparator separator = d as DataFormSeparator;
            if (separator != null && !separator.AreHandlersSuspended() && separator._line != null)
            {
                separator._line.StrokeThickness = separator.StrokeThickness;
            }
        }

        /// <summary>
        /// Generates the line to be used as a separator.
        /// </summary>
        /// <returns>The generated line.</returns>
        private FrameworkElement GenerateSeparator()
        {
            this._line = new Line();
            this._line.Stroke = this.Stroke;

            if (this.StrokeDashArray != null)
            {
                foreach (double d in this.StrokeDashArray)
                {
                    this._line.StrokeDashArray.Add(d);
                }
            }

            this._line.StrokeDashOffset = this.StrokeDashOffset;
            this._line.StrokeThickness = this.StrokeThickness;
            this._line.Style = this.LineStyle;

            if (this._linePresenter != null)
            {
                this._linePresenter.Content = null;
            }

            this._linePresenter = new ContentPresenter();
            this._linePresenter.SizeChanged += new SizeChangedEventHandler(this.OnPresenterSizeChanged);
            this._linePresenter.Content = this._line;

            this._linePresenter.Margin = new Thickness(0, 4, 0, 4);
            return this._linePresenter;
        }

        /// <summary>
        /// Updates the size of the line when the size of the presenter changes.
        /// </summary>
        /// <param name="sender">The presenter.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnPresenterSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this._line != null)
            {
                this._linePresenter.SizeChanged -= new SizeChangedEventHandler(this.OnPresenterSizeChanged);
                this._line.X2 = this._linePresenter.ActualWidth - 2;
            }
        }
    }
}
