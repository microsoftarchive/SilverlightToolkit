// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Represents a control with a single piece of content that expands or 
    /// collapses in a sliding motion to a specified desired size.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = ExpandableContentControl.ElementScrollViewerName, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = ExpandableContentControl.ElementContentSiteName, Type = typeof(ContentControl))]
    public class ExpandableContentControl : ContentControl
    {
        #region TemplateParts
        /// <summary>
        /// The name of the ScrollSite template part.
        /// </summary>
        private const string ElementScrollViewerName = "ScrollSite";

        /// <summary>
        /// The name of the ContentSite template part.
        /// </summary>
        private const string ElementContentSiteName = "ContentSite";

        /// <summary>
        /// Gets or sets the ContentSite template part.
        /// </summary>
        private ContentControl ContentSite { get; set; }

        /// <summary>
        /// Gets or sets the ScrollSite template part.
        /// </summary>
        private ScrollViewer ScrollSite { get; set; }
        #endregion TemplateParts

        #region public ExpandDirection RevealMode
        /// <summary>
        /// Gets or sets the direction in which the ExpandableContentControl 
        /// content window opens.
        /// </summary>
        public ExpandDirection RevealMode
        {
            get { return (ExpandDirection)GetValue(RevealModeProperty); }
            set { SetValue(RevealModeProperty, value); }
        }

        /// <summary>
        /// Identifies the RevealMode dependency property.
        /// </summary>
        public static readonly DependencyProperty RevealModeProperty =
            DependencyProperty.Register(
                "RevealMode",
                typeof(ExpandDirection),
                typeof(ExpandableContentControl),
                new PropertyMetadata(ExpandDirection.Down, OnRevealModePropertyChanged));

        /// <summary>
        /// RevealModeProperty property changed handler.
        /// </summary>
        /// <param name="d">ExpandableContentControl that changed its RevealMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnRevealModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExpandableContentControl source = (ExpandableContentControl)d;
            ExpandDirection value = (ExpandDirection)e.NewValue;

            if (value != ExpandDirection.Down &&
                value != ExpandDirection.Left &&
                value != ExpandDirection.Right &&
                value != ExpandDirection.Up)
            {
                // revert to old value
                source.RevealMode = (ExpandDirection) e.OldValue;

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.Expander_OnExpandDirectionPropertyChanged_InvalidValue,
                    value);
                throw new ArgumentException(message, "e");
            }

            // set the non-reveal dimension
            source.SetNonRevealDimension();

            // calculate the reveal dimension
            source.SetRevealDimension();
        }

        /// <summary>
        /// Gets a value indicating whether the content should be revealed vertically.
        /// </summary>
        private bool IsVerticalRevealMode
        {
            get { return RevealMode == ExpandDirection.Down || RevealMode == ExpandDirection.Up; }
        }

        /// <summary>
        /// Gets a value indicating whether the content should be revealed horizontally.
        /// </summary>
        private bool IsHorizontalRevealMode
        {
            get { return RevealMode == ExpandDirection.Left || RevealMode == ExpandDirection.Right; }
        }
        #endregion public ExpandDirection RevealMode

        #region public double Percentage
        /// <summary>
        /// Gets or sets the relative percentage of the content that is 
        /// currently visible. 
        /// </summary>
        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        /// <summary>
        /// Identifies the Percentage dependency property.
        /// </summary>
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register(
                "Percentage",
                typeof(double),
                typeof(ExpandableContentControl),
                new PropertyMetadata(0.0, OnPercentagePropertyChanged));

        /// <summary>
        /// PercentageProperty property changed handler.
        /// </summary>
        /// <param name="d">Page that changed its Percentage.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPercentagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExpandableContentControl source = (ExpandableContentControl)d;
            source.SetRevealDimension();

            if (source.ScrollSite != null)
            {
                if ((double) e.NewValue >= 1)
                {
                    source.ScrollSite.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    source.ScrollSite.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
                else
                {
                    source.ScrollSite.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    source.ScrollSite.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                }
            }
        }
        #endregion public double Percentage

        #region public Size TargetSize
        /// <summary>
        /// Gets or sets the desired size of the ExpandableContentControl content.
        /// </summary>
        /// <remarks>Use the percentage property to animate to this size.</remarks>
        public Size TargetSize
        {
            get { return (Size)GetValue(TargetSizeProperty); }
            set { SetValue(TargetSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the TargetSize dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetSizeProperty =
            DependencyProperty.Register(
                "TargetSize",
                typeof(Size),
                typeof(ExpandableContentControl),
                new PropertyMetadata(new Size(double.NaN, double.NaN), OnTargetSizePropertyChanged));

        /// <summary>
        /// TargetSizeProperty property changed handler.
        /// </summary>
        /// <param name="d">ExpandableContentControl that changed its TargetSize.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTargetSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExpandableContentControl source = (ExpandableContentControl)d;
            Size value = (Size)e.NewValue;

            // note: changed logic to disregard desired size 14 march. 

            // recalculate percentage based on this new targetsize.
            // for instance, percentage was at 1 and width was 100. Now width was changed to 200, this means
            // that the percentage needs to be set to 0.5 so that a reveal animation can be started again.

            if (source.IsVerticalRevealMode)
            {
                source.Percentage = source.ScrollSite.ViewportHeight / (double.IsNaN(value.Height) ? source.ContentSite.DesiredSize.Height : value.Height);
            }
            else if (source.IsHorizontalRevealMode)
            {
                source.Percentage = source.ScrollSite.ViewportWidth / (double.IsNaN(value.Width) ? source.ContentSite.DesiredSize.Width : value.Width);
            }
        }
        #endregion public Size TargetSize

        /// <summary>
        /// Sets the dimensions according to the current percentage.
        /// </summary>
        private void SetRevealDimension()
        {
            if (ContentSite == null || ScrollSite == null)
            {
                return;
            }

            if (IsHorizontalRevealMode)
            {
                double targetWidth = TargetSize.Width;
                if (Double.IsNaN(targetWidth))
                {
                    // NaN has the same meaning as autosize, which in this context means the desired size
                    targetWidth = ContentSite.DesiredSize.Width;
                }

                ScrollSite.Width = Percentage * targetWidth;
            }

            if (IsVerticalRevealMode)
            {
                double targetHeight = TargetSize.Height;
                if (Double.IsNaN(targetHeight))
                {
                    // NaN has the same meaning as autosize, which in this context means the desired size
                    targetHeight = ContentSite.DesiredSize.Height;
                }

                ScrollSite.Height = Percentage * targetHeight;
            }
        }

        /// <summary>
        /// Sets the opposite dimension.
        /// </summary>
        private void SetNonRevealDimension()
        {
            if (ScrollSite == null)
            {
                return;
            }
            if (IsHorizontalRevealMode)
            {
                // reset height to target size height. This can be double.NaN (auto size)
                ScrollSite.Height = TargetSize.Height;
            }

            if (IsVerticalRevealMode)
            {
                // reset width to target size width. This can be double.NaN (auto size)
                ScrollSite.Width = TargetSize.Width;
            }
        }

        /// <summary>
        /// Gets the content current visible size.
        /// </summary>
        public Size RelevantContentSize
        {
            get
            {
                return ScrollSite == null ? new Size(0, 0) : new Size(
                    (IsHorizontalRevealMode ? ScrollSite.ActualWidth : 0),
                    (IsVerticalRevealMode ? ScrollSite.ActualHeight : 0));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandableContentControl"/> class.
        /// </summary>
        public ExpandableContentControl()
        {
            DefaultStyleKey = typeof(ExpandableContentControl);
        }

        /// <summary>
        /// Builds the visual tree for the ExpandableContentControl control when a 
        /// new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            ScrollSite = GetTemplateChild(ElementScrollViewerName) as ScrollViewer;

            ContentSite = GetTemplateChild(ElementContentSiteName) as ContentControl;

            SetRevealDimension();

            SetNonRevealDimension();
        }
    }
}