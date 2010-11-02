// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Microsoft.Phone.Controls.Primitives
{
    /// <summary>
    /// Represents a switch that can be toggled between two states.
    /// </summary>
    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplateVisualState(Name = CheckedState, GroupName = CheckStates)]
    [TemplateVisualState(Name = DraggingState, GroupName = CheckStates)]
    [TemplateVisualState(Name = UncheckedState, GroupName = CheckStates)]
    [TemplatePart(Name = SwitchRootPart, Type = typeof(Grid))]
    [TemplatePart(Name = SwitchBackgroundPart, Type = typeof(UIElement))]
    [TemplatePart(Name = SwitchTrackPart, Type = typeof(Grid))]
    [TemplatePart(Name = SwitchThumbPart, Type = typeof(FrameworkElement))]
    public class ToggleSwitchButton : ToggleButton
    {
        /// <summary>
        /// Common visual states.
        /// </summary>
        private const string CommonStates = "CommonStates";

        /// <summary>
        /// Normal visual state.
        /// </summary>
        private const string NormalState = "Normal";

        /// <summary>
        /// Disabled visual state.
        /// </summary>
        private const string DisabledState = "Disabled";

        /// <summary>
        /// Check visual states.
        /// </summary>
        private const string CheckStates = "CheckStates";

        /// <summary>
        /// Checked visual state.
        /// </summary>
        private const string CheckedState = "Checked";

        /// <summary>
        /// Dragging visual state.
        /// </summary>
        private const string DraggingState = "Dragging";

        /// <summary>
        /// Unchecked visual state.
        /// </summary>
        private const string UncheckedState = "Unchecked";

        /// <summary>
        /// Switch root template part name.
        /// </summary>
        private const string SwitchRootPart = "SwitchRoot";

        /// <summary>
        /// Switch background template part name.
        /// </summary>
        private const string SwitchBackgroundPart = "SwitchBackground";

        /// <summary>
        /// Switch track template part name.
        /// </summary>
        private const string SwitchTrackPart = "SwitchTrack";

        /// <summary>
        /// Switch thumb template part name.
        /// </summary>
        private const string SwitchThumbPart = "SwitchThumb";

        /// <summary>
        /// Identifies the SwitchForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty SwitchForegroundProperty =
            DependencyProperty.Register("SwitchForeground", typeof(Brush), typeof(ToggleSwitchButton), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the switch foreground.
        /// </summary>
        public Brush SwitchForeground
        {
            get
            {
                return (Brush)GetValue(SwitchForegroundProperty);
            }
            set
            {
                SetValue(SwitchForegroundProperty, value);
            }
        }

        /// <summary>
        /// The background TranslateTransform.
        /// </summary>
        private TranslateTransform _backgroundTranslation;

        /// <summary>
        /// The thumb TranslateTransform.
        /// </summary>
        private TranslateTransform _thumbTranslation;

        /// <summary>
        /// The root template part.
        /// </summary>
        private Grid _root;

        /// <summary>
        /// The track template part.
        /// </summary>
        private Grid _track;

        /// <summary>
        /// The thumb template part.
        /// </summary>
        private FrameworkElement _thumb;

        /// <summary>
        /// The minimum translation.
        /// </summary>
        private const double _uncheckedTranslation = 0;

        /// <summary>
        /// The maximum translation.
        /// </summary>
        private double _checkedTranslation;

        /// <summary>
        /// The drag translation.
        /// </summary>
        private double _dragTranslation;

        /// <summary>
        /// Whether the translation ever changed during the drag.
        /// </summary>
        private bool _wasDragged;

        /// <summary>
        /// Whether the dragging state is current.
        /// </summary>
        private bool _isDragging;

        /// <summary>
        /// Initializes a new instance of the ToggleSwitch class.
        /// </summary>
        public ToggleSwitchButton()
        {
            DefaultStyleKey = typeof(ToggleSwitchButton);
        }

        /// <summary>
        /// Gets and sets the thumb and background translation.
        /// </summary>
        /// <returns>The translation.</returns>
        private double Translation
        {
            get
            {
                return _backgroundTranslation == null ? _thumbTranslation.X : _backgroundTranslation.X;
            }
            set
            {
                if (_backgroundTranslation != null)
                {
                    _backgroundTranslation.X = value;
                }

                if (_thumbTranslation != null)
                {
                    _thumbTranslation.X = value;
                }
            }
        }

        /// <summary>
        /// Change the visual state.
        /// </summary>
        /// <param name="useTransitions">Indicates whether to use animation transitions.</param>
        private void ChangeVisualState(bool useTransitions)
        {
            if (IsEnabled)
            {
                VisualStateManager.GoToState(this, NormalState, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, DisabledState, useTransitions);
            }

            if (_isDragging)
            {
                VisualStateManager.GoToState(this, DraggingState, useTransitions);
            }
            else if (IsChecked == true)
            {
                VisualStateManager.GoToState(this, CheckedState, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, UncheckedState, useTransitions);
            }
        }

        /// <summary>
        /// Called by the OnClick method to implement toggle behavior.
        /// </summary>
        protected override void OnToggle()
        {
            IsChecked = IsChecked == true ? false : true;
            ChangeVisualState(true);
        }

        /// <summary>
        /// Gets all the template parts and initializes the corresponding state.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (_track != null)
            {
                _track.SizeChanged -= SizeChangedHandler;
            }
            if (_thumb != null)
            {
                _thumb.SizeChanged -= SizeChangedHandler;
            }
            base.OnApplyTemplate();
            _root = GetTemplateChild(SwitchRootPart) as Grid;
            UIElement background = GetTemplateChild(SwitchBackgroundPart) as UIElement;
            _backgroundTranslation = background == null ? null : background.RenderTransform as TranslateTransform;
            _track = GetTemplateChild(SwitchTrackPart) as Grid;
            _thumb = GetTemplateChild(SwitchThumbPart) as Border;
            _thumbTranslation = _thumb == null ? null : _thumb.RenderTransform as TranslateTransform;
            if (_root != null && _track != null && _thumb != null && (_backgroundTranslation != null || _thumbTranslation != null))
            {
                GestureListener gestureListener = GestureService.GetGestureListener(_root);
                gestureListener.DragStarted += DragStartedHandler;
                gestureListener.DragDelta += DragDeltaHandler;
                gestureListener.DragCompleted += DragCompletedHandler;
                _track.SizeChanged += SizeChangedHandler;
                _thumb.SizeChanged += SizeChangedHandler;
            }
            ChangeVisualState(false);
        }

        /// <summary>
        /// Handles started drags on the root.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void DragStartedHandler(object sender, DragStartedGestureEventArgs e)
        {
            e.Handled = true;
            _isDragging = true;
            _dragTranslation = Translation;
            ChangeVisualState(true);
            Translation = _dragTranslation;
        }

        /// <summary>
        /// Handles drags on the root.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void DragDeltaHandler(object sender, DragDeltaGestureEventArgs e)
        {
            e.Handled = true;
            if (e.Direction == Orientation.Horizontal && e.HorizontalChange != 0)
            {
                _wasDragged = true;
                _dragTranslation += e.HorizontalChange;
                Translation = Math.Max(_uncheckedTranslation, Math.Min(_checkedTranslation, _dragTranslation));
            }
        }

        /// <summary>
        /// Handles completed drags on the root.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void DragCompletedHandler(object sender, DragCompletedGestureEventArgs e)
        {
            e.Handled = true;
            _isDragging = false;
            bool click = false;
            if (_wasDragged)
            {
                double edge = IsChecked == true ? _checkedTranslation : _uncheckedTranslation;
                if (Translation != edge)
                {
                    click = true;
                }
            }
            else
            {
                click = true;
            }
            if (click)
            {
                OnClick();
            }
            _wasDragged = false;
        }

        /// <summary>
        /// Handles changed sizes for the track and the thumb.
        /// Sets the clip of the track and computes the indeterminate and checked translations.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            _track.Clip = new RectangleGeometry { Rect = new Rect(0, 0, _track.ActualWidth, _track.ActualHeight) };
            _checkedTranslation = _track.ActualWidth - _thumb.ActualWidth - _thumb.Margin.Left - _thumb.Margin.Right;
        }
    }
}