// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// A rating control implementation that allows for a parameterized
    /// number of RatingItems, which can be arbitrarily styled using the
    /// FilledItemStyle and UnfilledItemStyle properties, or templated
    /// using an applied ControlTemplate.
    /// </summary>
    [TemplatePart(Name = Rating.FilledClipElement, Type = typeof(UIElement))]
    [TemplatePart(Name = Rating.FilledGridElement, Type = typeof(Grid))]
    [TemplatePart(Name = Rating.UnfilledGridElement, Type = typeof(Grid))]
    [TemplatePart(Name = Rating.DragBorderElement, Type = typeof(Border))]
    [TemplatePart(Name = Rating.DragTextBlockElement, Type = typeof(TextBlock))]
    [TemplateVisualState(Name = Rating.DragHelperCollapsed, GroupName = Rating.DragHelperStates)]
    [TemplateVisualState(Name = Rating.DragHelperVisible, GroupName = Rating.DragHelperStates)]
    public class Rating : Control
    {
        private const string FilledClipElement = "FilledClipElement";
        private const string FilledGridElement = "FilledGridElement";
        private const string UnfilledGridElement = "UnfilledGridElement";
        private const string DragBorderElement = "DragBorderElement";
        private const string DragTextBlockElement = "DragTextBlockElement";

        private const string DragHelperStates = "DragHelperStates";
        private const string DragHelperCollapsed = "Collapsed";
        private const string DragHelperVisible = "Visible";

        private UIElement _filledClipElement;
        private Grid _filledGridElement;
        private Grid _unfilledGridElement;
        private Border _dragBorderElement;
        private TextBlock _dragTextBlockElement;
        private Geometry _clippingMask;

        private List<RatingItem> _filledItemCollection = new List<RatingItem>();
        private List<RatingItem> _unfilledItemCollection = new List<RatingItem>();

        /// <summary>
        /// Occures when the value of this control changes, either from user interaction or by directly setting
        /// the property.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Initializes a new instance of the Rating type.
        /// </summary>
        public Rating()
        {
            DefaultStyleKey = typeof(Rating);
            SizeChanged += OnSizeChanged;
            ManipulationStarted += OnManipulationStarted;
            ManipulationDelta += OnManipulationDelta;
            ManipulationCompleted += OnManipulationCompleted;

            // Setup the control with the default number of
            // items.
            AdjustNumberOfRatingItems();
            SynchronizeGrids();
            UpdateClippingMask();
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (!ReadOnly)
            {
                PerformValueCalculation(e.ManipulationOrigin, e.ManipulationContainer);
                UpdateDragHelper();
                if (ShowSelectionHelper)
                {
                    ChangeDragHelperVisibility(true);
                }
            }
        }

        private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (!ReadOnly)
            {
                PerformValueCalculation(e.ManipulationOrigin, e.ManipulationContainer);
                UpdateDragHelper();
            }
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (!ReadOnly)
            {
                PerformValueCalculation(e.ManipulationOrigin, e.ManipulationContainer);
            }
            ChangeDragHelperVisibility(false);
        }

        private void OnSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            UpdateClippingMask();
        }

        /// <summary>
        /// Applies the template and extracts both a visual state and template
        /// parts.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _filledClipElement = GetTemplateChild(FilledClipElement) as UIElement;
            _filledGridElement = GetTemplateChild(FilledGridElement) as Grid;
            _unfilledGridElement = GetTemplateChild(UnfilledGridElement) as Grid;
            _dragBorderElement = GetTemplateChild(DragBorderElement) as Border;
            _dragTextBlockElement = GetTemplateChild(DragTextBlockElement) as TextBlock;

            if (_filledClipElement != null)
            {
                // Controls the visibility of the "Filled" RatingItems.
                _filledClipElement.Clip = _clippingMask;
            }

            if (_dragBorderElement != null)
            {
                _dragBorderElement.RenderTransform = new TranslateTransform();
            }
            VisualStateManager.GoToState(this, "Collapsed", false);
            SynchronizeGrids();
        }

        #region Grid Modifiers and Helpers

        /// <summary>
        /// Checks for existence of a drag text helper, and sets the visibility of this element.
        /// </summary>
        /// <param name="isVisible">Indicates whether the UIElement should be visible.</param>
        private void ChangeDragHelperVisibility(bool isVisible)
        {
            if (_dragBorderElement == null)
            {
                return;
            }

            if (isVisible)
            {
                VisualStateManager.GoToState(this, DragHelperVisible, true);
            }
            else
            {
                VisualStateManager.GoToState(this, DragHelperCollapsed, true);
            }
        }

        /// <summary>
        /// Updates both the text and the positioning of the drag text helper.
        /// </summary>
        private void UpdateDragHelper()
        {
            // If the RatingItemCount is set to zero the DragHelper has no
            // valid position so we simply return.
            if (RatingItemCount == 0)
            {
                return;
            }

            // if we only allow for whole stars to be selected, we don't need to
            // bother displaying the decimal point.
            string textBlockFormatString;
            if (AllowHalfItemIncrement)
            {
                textBlockFormatString = "F1";
            }
            else
            {
                textBlockFormatString = "F0";
            }

            if (_dragTextBlockElement != null)
            {
                _dragTextBlockElement.Text = Value.ToString(textBlockFormatString, CultureInfo.CurrentCulture);
            }

            if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                if (_dragBorderElement != null)
                {
                    double distanceToCenterOfDragBorder = (_dragBorderElement.ActualWidth) / 2;
                    double distanceToCenterOfRatingItem = (_filledItemCollection[0].ActualWidth) / 2;

                    // Because we explicitly set the RenderTransform to TranslateTransform we don't
                    // need to worry about this cast failing.
                    TranslateTransform t = (TranslateTransform)_dragBorderElement.RenderTransform;

                    if (!AllowHalfItemIncrement && !AllowSelectingZero)
                    {
                        t.X = (Value / RatingItemCount) * ActualWidth - distanceToCenterOfDragBorder - distanceToCenterOfRatingItem;
                    }
                    else
                    {
                        t.X = (Value / RatingItemCount) * ActualWidth - distanceToCenterOfDragBorder;
                    }

                    // 15 is a small amount of padding for the hover-over dragging number
                    // box to lift it above the main items control.
                    t.Y = -(ActualHeight / 2 + 15);
                }
            }
            else
            {
                if (_dragBorderElement != null)
                {
                    double distanceToCenterOfDragBorder = (_dragBorderElement.ActualHeight) / 2;
                    double distanceToCenterOfRatingItem = (_filledItemCollection[0].ActualHeight) / 2;

                    // Because we explicitly set the RenderTransform to TranslateTransform we don't
                    // need to worry about this cast failing.
                    TranslateTransform t = (TranslateTransform)_dragBorderElement.RenderTransform;

                    if (!AllowHalfItemIncrement && !AllowSelectingZero)
                    {
                        t.Y = (Value / RatingItemCount) * ActualHeight - distanceToCenterOfDragBorder - distanceToCenterOfRatingItem;
                    }
                    else
                    {
                        t.Y = (Value / RatingItemCount) * ActualHeight - distanceToCenterOfDragBorder;
                    }

                    t.X = -(ActualWidth / 2 + 15);
                }
            }
        }

        /// <summary>
        /// Calculates the new Value of this control using a location and relative source.
        /// The location is translated to the control, and then used to set the value.
        /// </summary>
        /// <param name="location">A point.</param>
        /// <param name="locationRelativeSource">The UIElement that the point originated from.</param>
        private void PerformValueCalculation(Point location, UIElement locationRelativeSource)
        {
            GeneralTransform gt = locationRelativeSource.TransformToVisual(this);
            location = gt.Transform(location);

            int numberOfPositions = _filledItemCollection.Count;

            if (AllowHalfItemIncrement)
            {
                numberOfPositions *= 2;
            }

            double newValue;
            if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                newValue = Math.Ceiling(location.X / ActualWidth * numberOfPositions);
            }
            else
            {
                newValue = Math.Ceiling(location.Y / ActualHeight * numberOfPositions);
            }

            if (!AllowSelectingZero && newValue <= 0)
            {
                newValue = 1;
            }

            Value = newValue;
        }

        /// <summary>
        /// Updates the clipping mask on _filledClipElement, if present, to reflect the current Value of the control.
        /// </summary>
        private void UpdateClippingMask()
        {
            Rect newRect;

            if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                double widthMinusBorder = ActualWidth - BorderThickness.Right - BorderThickness.Left;
                newRect = new Rect(0, 0, widthMinusBorder * (Value / RatingItemCount), ActualHeight);
            }
            else
            {
                double heightMinusBorder = ActualHeight - BorderThickness.Top - BorderThickness.Bottom;
                newRect = new Rect(0, 0, ActualWidth, heightMinusBorder * (Value / RatingItemCount));
            }

            RectangleGeometry rGeo = _clippingMask as RectangleGeometry;

            // we recycle the RectangleGeometry if its already been set here
            if (rGeo != null)
            {
                rGeo.Rect = newRect;
            }
            else
            {
                rGeo = new RectangleGeometry();
                rGeo.Rect = newRect;
                _clippingMask = rGeo;
            }
        }

        /// <summary>
        /// Builds a new RatingItem, taking in an optional Style to be applied to it.
        /// </summary>
        /// <param name="s">An optional style to apply to the RatingItem</param>
        /// <returns></returns>
        private static RatingItem BuildNewRatingItem(Style s)
        {
            RatingItem ri = new RatingItem();
            if (s != null)
            {
                ri.Style = s;
            }
            return ri;
        }

        /// <summary>
        /// Adds or removes RatingItem objects in the _filledItemCollection and _unfilledItemCollection lists to
        /// reflect the value of RatingItemCount.
        /// </summary>
        private void AdjustNumberOfRatingItems()
        {
            // Bring the item count for each collection up or down
            // to match the RatingItemCount property.

            while (_filledItemCollection.Count > RatingItemCount)
            {
                _filledItemCollection.RemoveAt(0);
            }

            while (_unfilledItemCollection.Count > RatingItemCount)
            {
                _unfilledItemCollection.RemoveAt(0);
            }

            while (_filledItemCollection.Count < RatingItemCount)
            {
                _filledItemCollection.Add(BuildNewRatingItem(FilledItemStyle));
            }

            while (_unfilledItemCollection.Count < RatingItemCount)
            {
                _unfilledItemCollection.Add(BuildNewRatingItem(UnfilledItemStyle));
            }
        }

        /// <summary>
        /// Adjusts the RowDefinition and ColumnDefinition collections on a Grid to be consistent with the
        /// number of RatingItems present in the collections, and finally adds these RatingItems to the 
        /// grid's Children, setting their Row and Column dependency properties. 
        /// </summary>
        /// <param name="grid">The grid to fix up.</param>
        /// <param name="ratingItemList">A rating list source to add to the grid.</param>
        private void SynchronizeGrid(Grid grid, IList<RatingItem> ratingItemList)
        {
            if (grid == null)
            {
                return;
            }

            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            if (Orientation == System.Windows.Controls.Orientation.Horizontal)
            {

                while (grid.ColumnDefinitions.Count < ratingItemList.Count)
                {
                    ColumnDefinition cD = new ColumnDefinition();
                    cD.Width = new GridLength(1, GridUnitType.Star);
                    grid.ColumnDefinitions.Add(cD);
                }

                grid.Children.Clear();
                for (int i = 0; i < ratingItemList.Count; i++)
                {
                    grid.Children.Add(ratingItemList[i]);
                    Grid.SetColumn(ratingItemList[i], i);
                    Grid.SetRow(ratingItemList[i], 0);
                }
            }
            else
            {
                while (grid.RowDefinitions.Count < ratingItemList.Count)
                {
                    RowDefinition rD = new RowDefinition();
                    rD.Height = new GridLength(1, GridUnitType.Star);
                    grid.RowDefinitions.Add(rD);
                }

                grid.Children.Clear();
                for (int i = 0; i < ratingItemList.Count; i++)
                {
                    grid.Children.Add(ratingItemList[i]);
                    Grid.SetRow(ratingItemList[i], i);
                    Grid.SetColumn(ratingItemList[i], 0);
                }

            }
        }

        /// <summary>
        /// Helper function to call SynchronizeGrid on both Grids captured from the ControlTemplate.
        /// </summary>
        private void SynchronizeGrids()
        {
            SynchronizeGrid(_unfilledGridElement, _unfilledItemCollection);
            SynchronizeGrid(_filledGridElement, _filledItemCollection);
        }

        #endregion

        #region public Style FilledItemStyle
        /// <summary>
        /// Gets or sets the style that will be applied to filled RatingItems.
        /// This style can only be applied once to a RatingItem.
        /// </summary>
        public Style FilledItemStyle
        {
            get { return (Style)GetValue(FilledItemStyleProperty); }
            set { SetValue(FilledItemStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the FilledItemStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty FilledItemStyleProperty =
            DependencyProperty.Register("FilledItemStyle",
            typeof(Style), typeof(Rating),
            new PropertyMetadata(OnFilledItemStyleChanged));

        private static void OnFilledItemStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)dependencyObject;
            source.OnFilledItemStyleChanged();
        }

        private void OnFilledItemStyleChanged()
        {
            foreach (RatingItem ri in _filledItemCollection)
            {
                // A style can only be set once in code.
                ri.Style = FilledItemStyle;
            }
        }
        #endregion

        #region public style UnfilledItemStyle
        /// <summary>
        /// Gets or sets the style that will be applied to unfilled RatingItems.
        /// This style can only be applied once to a RatingItem.
        /// </summary>
        public Style UnfilledItemStyle
        {
            get { return (Style)GetValue(UnfilledItemStyleProperty); }
            set { SetValue(UnfilledItemStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the UnfilledItemStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty UnfilledItemStyleProperty =
            DependencyProperty.Register("UnfilledItemStyle",
            typeof(Style), typeof(Rating),
            new PropertyMetadata(OnUnfilledItemStyleChanged));
        private static void OnUnfilledItemStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)dependencyObject;
            source.OnUnfilledItemStyleChanged();
        }

        private void OnUnfilledItemStyleChanged()
        {
            foreach (RatingItem ri in _unfilledItemCollection)
            {
                // A style can only be set once in code.
                ri.Style = UnfilledItemStyle;
            }
        }
        #endregion

        #region public int RatingItemCount
        /// <summary>
        /// Gets or sets the number of RatingItems that will be displayed
        /// by the control. Changing this property will cause elements to be
        /// added or removed from the FilledItemsCollection 
        /// and UnfilledItemsCollection.
        /// </summary>
        public int RatingItemCount
        {
            get { return (int)GetValue(RatingItemCountProperty); }
            set { SetValue(RatingItemCountProperty, value); }
        }

        /// <summary>
        /// Identifies the RatingItemCount dependency property.
        /// </summary>
        public static readonly DependencyProperty RatingItemCountProperty =
            DependencyProperty.Register("RatingItemCount",
                typeof(int), typeof(Rating),
                new PropertyMetadata(5, OnRatingItemCountChanged));

        private static void OnRatingItemCountChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)dependencyObject;
            source.OnRatingItemCountChanged();
        }

        private void OnRatingItemCountChanged()
        {
            if (RatingItemCount <= 0)
            {
                RatingItemCount = 0;
            }

            AdjustNumberOfRatingItems();
            SynchronizeGrids();
        }
        #endregion

        #region public double Value
        /// <summary>
        /// Gets or sets the current value of the control. This value
        /// corresponds to the percentage of filled RatingItems displayed,
        /// as determiend by a Clipping mask.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Property traditionally named value")]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                if (ValueChanged != null)
                {
                    ValueChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                typeof(double), typeof(Rating),
                new PropertyMetadata(0.0, OnValueChanged));

        private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)dependencyObject;
            source.OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (Value > RatingItemCount || Value < 0)
            {
                Value = Math.Max(0, Math.Min(RatingItemCount, Value));
            }
            UpdateClippingMask();
        }
        #endregion

        #region public boolean ReadOnly
        /// <summary>
        /// Gets or sets the value indicating whether this control will
        /// process user input and update the Value property.
        /// </summary>
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        /// <summary>
        /// Identifies the ReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(Rating), new PropertyMetadata(false));
        #endregion

        #region public boolean AllowHalfItemIncrement
        /// <summary>
        /// Gets or sets the value indicating whether this control will
        /// allow users to select half items when dragging or touching.
        /// </summary>
        public bool AllowHalfItemIncrement
        {
            get { return (bool)GetValue(AllowHalfItemIncrementProperty); }
            set { SetValue(AllowHalfItemIncrementProperty, value); }
        }

        /// <summary>
        /// Identifies the AllowHalfItemIncrement dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowHalfItemIncrementProperty =
            DependencyProperty.Register("AllowHalfItemIncrement", typeof(bool), typeof(Rating), new PropertyMetadata(false));
        #endregion

        #region public boolean AllowSelectingZero
        /// <summary>
        /// Gets or sets the value indicating whether this control will
        /// allow users to drag the selected items to zero.
        /// </summary>
        public bool AllowSelectingZero
        {
            get { return (bool)GetValue(AllowSelectingZeroProperty); }
            set { SetValue(AllowSelectingZeroProperty, value); }
        }

        /// <summary>
        /// Identifies the AllowNoItemsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowSelectingZeroProperty =
            DependencyProperty.Register("AllowSelectingZero", typeof(bool), typeof(Rating), new PropertyMetadata(false));
        #endregion

        #region public boolean ShowSelectionHelper
        /// <summary>
        /// Gets or sets the value indicating whether this control will
        /// display the drag selection helper.
        /// </summary>
        public bool ShowSelectionHelper
        {
            get { return (bool)GetValue(ShowSelectionHelperProperty); }
            set { SetValue(ShowSelectionHelperProperty, value); }
        }

        /// <summary>
        /// Identifies the AllowNoItemsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowSelectionHelperProperty =
            DependencyProperty.Register("ShowSelectionHelperItems", typeof(bool), typeof(Rating), new PropertyMetadata(false));
        #endregion

        #region public Orientation Orientation
        /// <summary>
        /// Gets or sets the value indicating the current orientation of the
        /// control.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Rating), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)dependencyObject;
            source.OnOrientationChanged();
        }

        private void OnOrientationChanged()
        {
            SynchronizeGrids();
            UpdateClippingMask();
        }
        #endregion
    }
}