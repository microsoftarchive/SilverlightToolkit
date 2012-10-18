﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Shell;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Represents a pop-up menu that enables a control to expose functionality that is specific to the context of the control.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(GroupName = VisibilityGroupName, Name = OpenVisibilityStateName)]
    [TemplateVisualState(GroupName = VisibilityGroupName, Name = OpenReversedVisibilityStateName)]
    [TemplateVisualState(GroupName = VisibilityGroupName, Name = ClosedVisibilityStateName)]
    [TemplateVisualState(GroupName = VisibilityGroupName, Name = OpenLandscapeVisibilityStateName)]
    [TemplateVisualState(GroupName = VisibilityGroupName, Name = OpenLandscapeReversedVisibilityStateName)]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Code flow is reasonably clear.")]
    public class ContextMenu : MenuBase
    {
        /// <summary>
        /// Width of the Menu in Landscape
        /// </summary>
        private const double LandscapeWidth = 480;

        /// <summary>
        /// Width of the system tray in Landscape Mode
        /// </summary>
        private const double SystemTrayLandscapeWidth = 72;

        /// <summary>
        /// Width of the application bar in Landscape mode
        /// </summary>
        private const double ApplicationBarLandscapeWidth = 72;

        /// <summary>
        /// Width of the borders around the menu
        /// </summary>
        private const double TotalBorderWidth = 8;

        /// <summary>
        /// Visibility state group.
        /// </summary>
        private const string VisibilityGroupName = "VisibilityStates";

        /// <summary>
        /// Open visibility state.
        /// </summary>
        private const string OpenVisibilityStateName = "Open";

        /// <summary>
        /// Open state when the context menu grows upwards.
        /// </summary>
        private const string OpenReversedVisibilityStateName = "OpenReversed";

        /// <summary>
        /// Closed visibility state.
        /// </summary>
        private const string ClosedVisibilityStateName = "Closed";

        /// <summary>
        /// Open landscape visibility state.
        /// </summary>
        private const string OpenLandscapeVisibilityStateName = "OpenLandscape";

        /// <summary>
        /// Open landscape state when the context menu grows leftwards.
        /// </summary>
        private const string OpenLandscapeReversedVisibilityStateName = "OpenLandscapeReversed";

        /// <summary>
        /// The panel that holds all the content
        /// </summary>
        private StackPanel _outerPanel;

        /// <summary>
        /// The grid that contains the item presenter 
        /// </summary>
        private Grid _innerGrid;

        /// <summary>
        /// Stores a reference to the PhoneApplicationPage that contains the owning object.
        /// </summary>
        private PhoneApplicationPage _page;

        /// <summary>
        /// Stores a reference to a list of ApplicationBarIconButtons for which the Click event is being handled.
        /// </summary>
        private readonly List<ApplicationBarIconButton> _applicationBarIconButtons = new List<ApplicationBarIconButton>();

        /// <summary>
        /// Stores a reference to the Storyboard used to animate the background resize.
        /// </summary>
        private Storyboard _backgroundResizeStoryboard;

        /// <summary>
        /// Stores a reference to the Storyboard used to animate the ContextMenu open.
        /// </summary>
        private List<Storyboard> _openingStoryboard;

        /// <summary>
        /// Tracks whether the Storyboard used to animate the ContextMenu open is active.
        /// </summary>
        private bool _openingStoryboardPlaying;

        /// <summary>
        /// Tracks the threshold for releasing contact during the ContextMenu open animation.
        /// </summary>
        private DateTime _openingStoryboardReleaseThreshold;

        /// <summary>
        /// Stores a reference to the current root visual.
        /// </summary>
        private PhoneApplicationFrame _rootVisual;

        /// <summary>
        /// Stores a reference to the object that owns the ContextMenu.
        /// </summary>
        private DependencyObject _owner;

        /// <summary>
        /// Stores a reference to the current Popup.
        /// </summary>
        private Popup _popup;

        /// <summary>
        /// Stores a reference to the current overlay.
        /// </summary>
        private Panel _overlay;

        /// <summary>
        /// Stores a reference to the current Popup alignment point.
        /// </summary>
        private Point _popupAlignmentPoint;

        /// <summary>
        /// Stores a value indicating whether the IsOpen property is being updated by ContextMenu.
        /// </summary>
        private bool _settingIsOpen;

        /// <summary>
        /// Whether the opening animation is reversed (bottom to top or right to left).
        /// </summary>
        private bool _reversed;

        /// <summary>
        /// Gets or sets the owning object for the ContextMenu.
        /// </summary>
        public DependencyObject Owner
        {
            get { return _owner; }
            internal set
            {
                if (null != _owner)
                {
                    FrameworkElement ownerFrameworkElement = _owner as FrameworkElement;
                    if (null != ownerFrameworkElement)
                    {
                        ownerFrameworkElement.Hold -= OnOwnerHold;
                        ownerFrameworkElement.Loaded -= OnOwnerLoaded;
                        ownerFrameworkElement.Unloaded -= OnOwnerUnloaded;

                        OnOwnerUnloaded(null, null);
                    }
                }
                _owner = value;
                if (null != _owner)
                {
                    FrameworkElement ownerFrameworkElement = _owner as FrameworkElement;
                    if (null != ownerFrameworkElement)
                    {
                        ownerFrameworkElement.Hold += OnOwnerHold;
                        ownerFrameworkElement.Loaded += OnOwnerLoaded;
                        ownerFrameworkElement.Unloaded += OnOwnerUnloaded;

                        // Owner *may* already be live and have fired its Loaded event - hook up manually if necessary
                        DependencyObject parent = ownerFrameworkElement;
                        while (parent != null)
                        {
                            parent = VisualTreeHelper.GetParent(parent);
                            if ((null != parent) && (parent == _rootVisual))
                            {
                                OnOwnerLoaded(null, null);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the background will zoom out when the ContextMenu is open.
        /// </summary>
        public bool IsZoomEnabled
        {
            get { return (bool)GetValue(IsZoomEnabledProperty); }
            set { SetValue(IsZoomEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the IsZoomEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(
            "IsZoomEnabled",
            typeof(bool),
            typeof(ContextMenu),
            new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the background will fade when the ContextMenu is open.
        /// IsZoomEnabled must be true for this value to take effect.
        /// </summary>
        public bool IsFadeEnabled
        {
            get { return (bool)GetValue(IsFadeEnabledProperty); }
            set { SetValue(IsFadeEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the IsFadeEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFadeEnabledProperty = DependencyProperty.Register(
            "IsFadeEnabled",
            typeof(bool),
            typeof(ContextMenu),
            new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the vertical distance between the target origin and the popup alignment point.
        /// </summary>
        [TypeConverterAttribute(typeof(LengthConverter))]
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(
            "VerticalOffset",
            typeof(double),
            typeof(ContextMenu),
            new PropertyMetadata(0.0, OnVerticalOffsetChanged));

        /// <summary>
        /// Handles changes to the VerticalOffset DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnVerticalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ContextMenu)o).UpdateContextMenuPlacement();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ContextMenu is visible.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Identifies the IsOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            "IsOpen",
            typeof(bool),
            typeof(ContextMenu),
            new PropertyMetadata(false, OnIsOpenChanged));

        /// <summary>
        /// Handles changes to the IsOpen DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ContextMenu)o).OnIsOpenChanged(/*(bool)e.OldValue,*/ (bool)e.NewValue);
        }

        /// <summary>
        /// Handles changes to the IsOpen property.
        /// </summary>
        /// <param name="newValue">New value.</param>
        private void OnIsOpenChanged(/*bool oldValue,*/ bool newValue)
        {
            if (!_settingIsOpen)
            {
                if (newValue)
                {
                    // User is trying to set IsOpen property to true to show the ContextMenu,
                    // this property can be set anywhere so we don't know the exact position the user wants to show.
                    // Passing negative numbers so we can put it around the current element
                    OpenPopup(new Point(-1, -1));
                }
                else
                {
                    ClosePopup();
                }
            }
        }

        /// <summary>
        /// Gets or sets the region of interest expressed in the coordinate system of the root visual. 
        /// A context menu will try to position itself outside the region of interest.
        /// If null, the owner's bounding box is considered the region of interest.
        /// </summary>
        public Rect? RegionOfInterest
        {
            get { return (Rect?)GetValue(RegionOfInterestProperty); }
            set { SetValue(RegionOfInterestProperty, value); }
        }

        /// <summary>
        /// Identifies the RegionOfInterest dependency property.
        /// </summary>
        public static readonly DependencyProperty RegionOfInterestProperty =
            DependencyProperty.Register("RegionOfInterest", typeof(Rect?), typeof(ContextMenu), null);

        /// <summary>
        /// Occurs when a particular instance of a ContextMenu opens.
        /// </summary>
        public event RoutedEventHandler Opened;

        /// <summary>
        /// Called when the Opened event occurs.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnOpened(RoutedEventArgs e)
        {
            UpdateContextMenuPlacement();

            // Handles initial open (where OnOpened is called before OnApplyTemplate)
            SetRenderTransform();
            UpdateVisualStates(true);

            var handler = Opened;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        private void SetRenderTransform()
        {
            if (DesignerProperties.IsInDesignTool || _rootVisual.Orientation.IsPortrait())
            {
                double x = 0.5;
                if (null != _popupAlignmentPoint)
                {
                    x = _popupAlignmentPoint.X / Width;
                }

                if (_outerPanel != null)
                {
                    _outerPanel.RenderTransformOrigin = new Point(x, 0);
                }
                if (_innerGrid != null)
                {
                    double pointY = _reversed ? 1 : 0;
                    _innerGrid.RenderTransformOrigin = new Point(0, pointY);
                }
            }
            else
            {
                if (_outerPanel != null)
                {
                    _outerPanel.RenderTransformOrigin = new Point(0, 0.5);
                }
                if (_innerGrid != null)
                {
                    double pointX = _reversed ? 1 : 0;
                    _innerGrid.RenderTransformOrigin = new Point(pointX, 0);
                }
            }
        }

        /// <summary>
        /// Occurs when a particular instance of a ContextMenu closes.
        /// </summary>
        public event RoutedEventHandler Closed;

        /// <summary>
        /// Called when the Closed event occurs.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnClosed(RoutedEventArgs e)
        {
            UpdateVisualStates(true);

            var handler = Closed;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Initializes a new instance of the ContextMenu class.
        /// </summary>
        public ContextMenu()
        {
            DefaultStyleKey = typeof(ContextMenu);

            _openingStoryboard = new List<Storyboard>();

            if (null == Application.Current.RootVisual)
            {
                // Temporarily hook LayoutUpdated to find out when Application.Current.RootVisual gets set.
                LayoutUpdated += OnLayoutUpdated;
            }
            else
            {
                // We've already missed the LayoutUpdated event, so we are safe to call InitializeRootVisual() to compensate.
                InitializeRootVisual();
            }
        }

        /// <summary>
        /// Called when a new Template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Unhook from old Template
            if (null != _openingStoryboard)
            {
                foreach (Storyboard sb in _openingStoryboard)
                {
                    sb.Completed -= OnStoryboardCompleted;
                }
                _openingStoryboard.Clear();
            }
            _openingStoryboardPlaying = false;

            // Apply new template
            base.OnApplyTemplate();

            SetDefaultStyle();

            // Hook up to new template
            FrameworkElement templateRoot = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
            if (null != templateRoot)
            {
                foreach (VisualStateGroup group in VisualStateManager.GetVisualStateGroups(templateRoot))
                {
                    if (VisibilityGroupName == group.Name)
                    {
                        foreach (VisualState state in group.States)
                        {
                            if ((OpenVisibilityStateName == state.Name || OpenLandscapeVisibilityStateName == state.Name || OpenReversedVisibilityStateName == state.Name || OpenLandscapeReversedVisibilityStateName == state.Name) && (null != state.Storyboard))
                            {
                                _openingStoryboard.Add(state.Storyboard);
                                state.Storyboard.Completed += OnStoryboardCompleted;
                            }
                        }
                    }
                }
            }

            _outerPanel = GetTemplateChild("OuterPanel") as StackPanel;
            _innerGrid = GetTemplateChild("InnerGrid") as Grid;

            // Go to correct visual state(s)
            bool portrait = DesignerProperties.IsInDesignTool || _rootVisual.Orientation.IsPortrait();

            SetRenderTransform();

            if (IsOpen)
            {
                // Handles initial open (where OnOpened is called before OnApplyTemplate)
                if (null != _innerGrid)
                {
                    // if landscape to the full height. NOTE: device is rotated so use the ActualWidth
                    _innerGrid.MinHeight = portrait ? 0 : _rootVisual.ActualWidth;
                }

                UpdateVisualStates(true);
            }
        }

        /// <summary>
        /// Set up the background and border default styles
        /// </summary>
        private void SetDefaultStyle()
        {
            // These styles are not defined in the XAML because according to spec,
            // the background color should be white (opaque) in Dark Theme and black (opaque) in Light Theme.
            // There are no StaticResource brushes that have this property (the black is transparent).
            // We define these in code, because we need to check the current theme to define the colors.

            SolidColorBrush backgroundBrush;
            SolidColorBrush borderBrush;
            if (DesignerProperties.IsInDesignTool || Resources.IsDarkThemeActive())
            {
                backgroundBrush = new SolidColorBrush(Colors.White);
                borderBrush = new SolidColorBrush(Colors.Black);
            }
            else
            {
                backgroundBrush = new SolidColorBrush(Colors.Black);
                borderBrush = new SolidColorBrush(Colors.White);
            }

            Style newStyle = new Style(typeof(ContextMenu));

            Setter setterBackground = new Setter(ContextMenu.BackgroundProperty, backgroundBrush);
            Setter settterBorderBrush = new Setter(ContextMenu.BorderBrushProperty, borderBrush);

            if (null == Style)
            {
                newStyle.Setters.Add(setterBackground);
                newStyle.Setters.Add(settterBorderBrush);
            }
            else
            {
                // Merge the currently existing style with the new styles we want
                bool foundBackground = false;
                bool foundBorderBrush = false;

                foreach (Setter s in Style.Setters)
                {
                    if (s.Property == ContextMenu.BackgroundProperty)
                    {
                        foundBackground = true;
                    }
                    else if (s.Property == ContextMenu.BorderBrushProperty)
                    {
                        foundBorderBrush = true;
                    }

                    newStyle.Setters.Add(new Setter(s.Property, s.Value));
                }

                if (!foundBackground)
                {
                    newStyle.Setters.Add(setterBackground);
                }
                if (!foundBorderBrush)
                {
                    newStyle.Setters.Add(settterBorderBrush);
                }
            }

            Style = newStyle;
        }

        /// <summary>
        /// Handles the Completed event of the opening Storyboard.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnStoryboardCompleted(object sender, EventArgs e)
        {
            _openingStoryboardPlaying = false;
        }

        /// <summary>
        /// Uses VisualStateManager to go to the appropriate visual state.
        /// </summary>
        /// <param name="useTransitions">true to use a System.Windows.VisualTransition to 
        ///                              transition between states; otherwise, false.</param>
        private void UpdateVisualStates(bool useTransitions)
        {
            string stateName;

            if (IsOpen)
            {
                if (null != _openingStoryboard)
                {
                    _openingStoryboardPlaying = true;
                    _openingStoryboardReleaseThreshold = DateTime.UtcNow.AddSeconds(0.3);
                }

                if (_rootVisual != null && _rootVisual.Orientation.IsPortrait())
                {
                    if (_outerPanel != null)
                    {
                        _outerPanel.Orientation = Orientation.Vertical;
                    }

                    stateName = _reversed ? OpenReversedVisibilityStateName : OpenVisibilityStateName;
                }
                else
                {
                    if (_outerPanel != null)
                    {
                        _outerPanel.Orientation = Orientation.Horizontal;
                    }

                    stateName = _reversed ? OpenLandscapeReversedVisibilityStateName : OpenLandscapeVisibilityStateName;
                }

                if (null != _backgroundResizeStoryboard)
                {
                    _backgroundResizeStoryboard.Begin();
                }
            }
            else
            {
                stateName = ClosedVisibilityStateName;
            }

            VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        /// <summary>
        /// Whether the position is on the right half of the screen. 
        /// Only supports landscape mode.
        /// This is used to determine which side of the screen the context menu will display on.
        /// </summary>
        /// <param name="position">Position to check for</param>
        private bool PositionIsOnScreenRight(double position)
        {
            return (PageOrientation.LandscapeLeft == _rootVisual.Orientation ? 
                (position > _rootVisual.ActualHeight / 2) :
                (position < _rootVisual.ActualHeight / 2));
        }

        /// <summary>
        /// Called when the left mouse button is pressed.
        /// </summary>
        /// <param name="e">The event data for the MouseLeftButtonDown event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            e.Handled = true;
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Responds to the KeyDown event.
        /// </summary>
        /// <param name="e">The event data for the KeyDown event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            switch (e.Key)
            {
                case Key.Up:
                    FocusNextItem(false);
                    e.Handled = true;
                    break;
                case Key.Down:
                    FocusNextItem(true);
                    e.Handled = true;
                    break;
                case Key.Escape:
                    ClosePopup();
                    e.Handled = true;
                    break;
                // case Key.Apps: // Key.Apps not defined by Silverlight 4
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Handles the LayoutUpdated event to capture Application.Current.RootVisual.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (null != Application.Current.RootVisual)
            {
                // Application.Current.RootVisual is valid now
                InitializeRootVisual();
                // Unhook event
                LayoutUpdated -= OnLayoutUpdated;
            }
        }

        /// <summary>
        /// Handles the ManipulationCompleted event for the RootVisual.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnRootVisualManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            // Breaking contact during the ContextMenu show animation should cancel the ContextMenu
            if (_openingStoryboardPlaying && (DateTime.UtcNow <= _openingStoryboardReleaseThreshold))
            {
                IsOpen = false;
            }
        }

        /// <summary>
        /// Handles the Hold event for the owning element.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOwnerHold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!IsOpen)
            {
                OpenPopup(e.GetPosition(null));
                e.Handled = true;
            }
        }

        /// <summary>
        /// Identifies the ApplicationBarMirror dependency property.
        /// </summary>
        private static readonly DependencyProperty ApplicationBarMirrorProperty = DependencyProperty.Register(
            "ApplicationBarMirror",
            typeof(IApplicationBar),
            typeof(ContextMenu),
            new PropertyMetadata(OnApplicationBarMirrorChanged));

        /// <summary>
        /// Handles changes to the ApplicationBarMirror DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnApplicationBarMirrorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ContextMenu)o).OnApplicationBarMirrorChanged((IApplicationBar)e.OldValue, (IApplicationBar)e.NewValue);
        }

        /// <summary>
        /// Handles changes to the ApplicationBarMirror property.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnApplicationBarMirrorChanged(IApplicationBar oldValue, IApplicationBar newValue)
        {
            if (null != oldValue)
            {
                oldValue.StateChanged -= OnEventThatClosesContextMenu;
            }
            if (null != newValue)
            {
                newValue.StateChanged += OnEventThatClosesContextMenu;
            }
        }

        /// <summary>
        /// Handles an event which should close an open ContextMenu.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnEventThatClosesContextMenu(object sender, EventArgs e)
        {
            // Close the ContextMenu because the elements and/or layout is likely to have changed significantly
            IsOpen = false;
        }

        /// <summary>
        /// Handles the Loaded event of the Owner.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOwnerLoaded(object sender, RoutedEventArgs e)
        {
            if (null == _page) // Don't want to attach to BackKeyPress twice
            {
                InitializeRootVisual();
                if (null != _rootVisual)
                {
                    _page = _rootVisual.Content as PhoneApplicationPage;
                    if (_page != null)
                    {
                        _page.BackKeyPress += OnPageBackKeyPress;
                        SetBinding(ApplicationBarMirrorProperty, new Binding { Source = _page, Path = new PropertyPath("ApplicationBar") });
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Unloaded event of the Owner.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOwnerUnloaded(object sender, RoutedEventArgs e)
        {
            if (null != _rootVisual)
            {
                _rootVisual.ManipulationCompleted -= OnRootVisualManipulationCompleted;
                _rootVisual.OrientationChanged -= OnEventThatClosesContextMenu;
            }
            if (_page != null)
            {
                _page.BackKeyPress -= OnPageBackKeyPress;
                ClearValue(ApplicationBarMirrorProperty);
                _page = null;
            }
        }

        /// <summary>
        /// Handles the BackKeyPress of the containing PhoneApplicationPage.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPageBackKeyPress(object sender, CancelEventArgs e)
        {
            if (IsOpen)
            {
                IsOpen = false;
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Calls TransformToVisual on the specified element for the specified visual, suppressing the ArgumentException that can occur in some cases.
        /// </summary>
        /// <param name="element">Element on which to call TransformToVisual.</param>
        /// <param name="visual">Visual to pass to the call to TransformToVisual.</param>
        /// <returns>Resulting GeneralTransform object.</returns>
        private static GeneralTransform SafeTransformToVisual(UIElement element, UIElement visual)
        {
            GeneralTransform result;
            try
            {
                result = element.TransformToVisual(visual);
            }
            catch (ArgumentException)
            {
                // Not perfect, but better than throwing an exception
                result = new TranslateTransform();
            }
            return result;
        }

        /// <summary>
        /// Initialize the _rootVisual property (if possible and not already done).
        /// </summary>
        private void InitializeRootVisual()
        {
            if (null == _rootVisual)
            {
                // Try to capture the Application's RootVisual
                _rootVisual = Application.Current.RootVisual as
                    PhoneApplicationFrame;
                if (null != _rootVisual)
                {
                    _rootVisual.ManipulationCompleted -= OnRootVisualManipulationCompleted;
                    _rootVisual.ManipulationCompleted += OnRootVisualManipulationCompleted;

                    _rootVisual.OrientationChanged -= OnEventThatClosesContextMenu;
                    _rootVisual.OrientationChanged += OnEventThatClosesContextMenu;
                }
            }
        }

        /// <summary>
        /// Sets focus to the next item in the ContextMenu.
        /// </summary>
        /// <param name="down">True to move the focus down; false to move it up.</param>
        private void FocusNextItem(bool down)
        {
            int count = Items.Count;
            int startingIndex = down ? -1 : count;
            MenuItem focusedMenuItem = FocusManager.GetFocusedElement() as MenuItem;
            if (null != focusedMenuItem && (this == focusedMenuItem.ParentMenuBase))
            {
                startingIndex = ItemContainerGenerator.IndexFromContainer(focusedMenuItem);
            }
            int index = startingIndex;
            do
            {
                index = (index + count + (down ? 1 : -1)) % count;
                MenuItem container = ItemContainerGenerator.ContainerFromIndex(index) as MenuItem;
                if (null != container)
                {
                    if (container.IsEnabled && container.Focus())
                    {
                        break;
                    }
                }
            }
            while (index != startingIndex);
        }

        /// <summary>
        /// Called when a child MenuItem is clicked.
        /// </summary>
        internal void ChildMenuItemClicked()
        {
            ClosePopup();
        }

        /// <summary>
        /// Handles the SizeChanged event for the ContextMenu or RootVisual.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnContextMenuOrRootVisualSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateContextMenuPlacement();
        }

        /// <summary>
        /// Handles the MouseButtonUp events for the overlay.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOverlayMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            // If they clicked in the context menu, then don't close
            List<UIElement> list = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), _rootVisual) as List<UIElement>;
            if (!list.Contains(this))
            {
                ClosePopup();
            }

            e.Handled = true;
        }

        /// <summary>
        /// Adjust the position (Y) of ContextMenu for Portrait Mode.
        /// </summary>
        private double AdjustContextMenuPositionForPortraitMode(Rect bounds, double roiY, double roiHeight, ref bool reversed)
        {
            double y = 0.0;
            bool notEnoughRoom = false;     // if we have enough room to place the menu without moving.

            double lowestTopOfMenu = bounds.Bottom - ActualHeight;
            double highestBottomOfMenu = bounds.Top + ActualHeight;

            if (bounds.Height <= ActualHeight)
            {
                notEnoughRoom = true;
            }
            else if (roiY + roiHeight <= lowestTopOfMenu)           // there is enough room below the owner.
            {
                y = roiY + roiHeight;
                reversed = false;
            }
            else if (roiY >= highestBottomOfMenu)                   // there is enough room above the owner.
            {
                y = roiY - ActualHeight;
                reversed = true;
            }
            else if (_popupAlignmentPoint.Y >= 0)                   // menu is displayed by Tap&Hold gesture, will try to place the menu at touch position                                                                            
            {
                y = _popupAlignmentPoint.Y;
                if (y <= lowestTopOfMenu)
                {
                    reversed = false;
                }
                else if (y >= highestBottomOfMenu)
                {
                    y -= ActualHeight;
                    reversed = true;
                }
                else
                {
                    notEnoughRoom = true;
                }
            }
            else                                                    // menu is displayed by calling "IsOpen = true", the point will be (-1, -1)
            {
                notEnoughRoom = true;
            }

            if (notEnoughRoom)                                      // failed to place menu in above scenraios, try to align it to Bottom.
            {
                y = lowestTopOfMenu;                              // align to bottom
                reversed = true;

                if (y <= bounds.Top)                              // if the menu can't be fully displayed, make sure we truncate the bottom items, not the top items.
                {
                    y = bounds.Top;
                    reversed = false;
                }
            }
            return y;
        }

        /// <summary>
        /// Updates the location and size of the Popup and overlay.
        /// </summary>
        private void UpdateContextMenuPlacement()
        {
            if ((null != _rootVisual) && (null != _overlay))
            {
                Point p = new Point(_popupAlignmentPoint.X, _popupAlignmentPoint.Y);

                // Determine frame/page bounds
                bool portrait = _rootVisual.Orientation.IsPortrait();

                double effectiveWidth = portrait ? _rootVisual.ActualWidth : _rootVisual.ActualHeight;
                double effectiveHeight = portrait ? _rootVisual.ActualHeight : _rootVisual.ActualWidth;
                Rect bounds = new Rect(0, 0, effectiveWidth, effectiveHeight);
                if (_page != null)
                {
                    bounds = SafeTransformToVisual(_page, _rootVisual).TransformBounds(new Rect(0, 0, _page.ActualWidth, _page.ActualHeight));
                }

                if (portrait && null != _rootVisual && null != bounds)
                {
                    double roiY;
                    double roiHeight;

                    if (RegionOfInterest.HasValue)
                    {
                        roiY = RegionOfInterest.Value.Y;
                        roiHeight = RegionOfInterest.Value.Height;
                    }
                    else if (Owner is FrameworkElement)
                    {
                        FrameworkElement el = (FrameworkElement)Owner;
                        GeneralTransform t = el.TransformToVisual(_rootVisual);

                        roiY = t.Transform(new Point(0, 0)).Y;
                        roiHeight = el.ActualHeight;
                    }
                    else
                    {
                        roiY = _popupAlignmentPoint.Y;
                        roiHeight = 0;
                    }
                                  
                    p.Y = AdjustContextMenuPositionForPortraitMode(bounds, roiY, roiHeight, ref _reversed);
                }
                    

                // Start with the current Popup alignment point
                double x = p.X;
                double y = p.Y;

                // Adjust for offset
                y += VerticalOffset;

                if (portrait)
                {
                    // Left align with full width
                    x = bounds.Left;
                    Width = bounds.Width;
                    if (null != _innerGrid)
                    {
                        _innerGrid.Width = Width;
                    }
                }
                else
                {                    
                    if (PositionIsOnScreenRight(y))
                    {
                        Width = (SystemTray.IsVisible) ? LandscapeWidth - SystemTrayLandscapeWidth : LandscapeWidth;
                        x = (SystemTray.IsVisible) ? SystemTrayLandscapeWidth : 0;
                        _reversed = true;
                    }
                    else
                    {
                        Width = (null != _page.ApplicationBar && _page.ApplicationBar.IsVisible) ? LandscapeWidth - ApplicationBarLandscapeWidth : LandscapeWidth;
                        x = bounds.Width - Width + ((SystemTray.IsVisible) ? SystemTrayLandscapeWidth : 0);
                        _reversed = false;
                    }

                    if (null != _innerGrid)
                    {
                        _innerGrid.Width = Width - TotalBorderWidth;
                    }

                    y = 0;
                }

                // Do not let it stick out too far to the left/top
                x = Math.Max(x, 0);

                // Set the new location
                Canvas.SetLeft(this, x);
                Canvas.SetTop(this, y);

                // Size the overlay to match the new container
                _overlay.Width = effectiveWidth;
                _overlay.Height = effectiveHeight;
            }
        }

        /// <summary>
        /// Opens the Popup.
        /// </summary>
        /// <param name="position">Position to place the Popup.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Code flow is reasonably clear.")]
        private void OpenPopup(Point position)
        {
            _popupAlignmentPoint = position;

            InitializeRootVisual();

            bool portrait = _rootVisual.Orientation.IsPortrait();

            if (portrait)
            {
                if (_innerGrid != null)
                {
                    _innerGrid.MinHeight = 0;
                }
            }
            else
            {
                if (_innerGrid != null)
                {
                    // if landscape to the full height. NOTE: device is rotated so use the ActualWidth
                    _innerGrid.MinHeight = _rootVisual.ActualWidth;
                }
            }

            _overlay = new Canvas { Background = new SolidColorBrush(Colors.Transparent) };
            _overlay.MouseLeftButtonUp += OnOverlayMouseButtonUp;

            if (IsZoomEnabled && (null != _rootVisual))
            {
                // Capture effective width/height
                double width = portrait ? _rootVisual.ActualWidth : _rootVisual.ActualHeight;
                double height = portrait ? _rootVisual.ActualHeight : _rootVisual.ActualWidth;

                // Create a layer for the background brush
                UIElement backgroundLayer = new Rectangle
                {
                    Width = width,
                    Height = height,
                    Fill = (Brush)Application.Current.Resources["PhoneBackgroundBrush"],
                    CacheMode = new BitmapCache(),
                };
                _overlay.Children.Insert(0, backgroundLayer);

                // Hide the owner for the snapshot we will take
                FrameworkElement ownerElement = _owner as FrameworkElement;
                if (null != ownerElement)
                {
                    ownerElement.Opacity = 0;
                }

                // Create a layer for the page content
                WriteableBitmap writeableBitmap = new WriteableBitmap((int)width, (int)height);
                writeableBitmap.Render(_rootVisual, null);
                writeableBitmap.Invalidate();
                Transform scaleTransform = new ScaleTransform
                {
                    CenterX = width / 2,
                    CenterY = height / 2,
                };
                UIElement contentLayer = new Image
                {
                    Source = writeableBitmap,
                    RenderTransform = scaleTransform,
                    CacheMode = new BitmapCache(),
                };
                _overlay.Children.Insert(1, contentLayer);

                // Create a layer for the background brush
                UIElement backgroundFadeLayer = new Rectangle
                {
                    Width = width,
                    Height = height,
                    Fill = (Brush)Application.Current.Resources["PhoneBackgroundBrush"],
                    Opacity = 0,
                    CacheMode = new BitmapCache(),
                };
                _overlay.Children.Insert(2, backgroundFadeLayer);


                // Create a layer for the owner element and its background
                
                if (null != ownerElement)
                {
                    ((FrameworkElement)Owner).Opacity = 1;

                    // If the owner's flow direction is right-to-left, then (0, 0) is situated at the
                    // top-right corner of the element instead of its top-left corner.
                    // We need for the translated point to be in the top-left corner since we want these elements
                    // to be drawn on top of the owner's position from left to right,
                    // so to achieve that, we'll translate (0, ActualWidth) instead if its flow direction is right-to-left.
                    Point point = SafeTransformToVisual(ownerElement, _rootVisual).Transform(new Point(ownerElement.FlowDirection == System.Windows.FlowDirection.RightToLeft ? ownerElement.ActualWidth : 0, 0));

                    // Create a layer for the element's background
                    UIElement elementBackground = new Rectangle
                    {
                        Width = ownerElement.ActualWidth,
                        Height = ownerElement.ActualHeight,
                        Fill = new SolidColorBrush(Colors.Transparent),
                        CacheMode = new BitmapCache(),
                    };
                    Canvas.SetLeft(elementBackground, point.X);
                    Canvas.SetTop(elementBackground, point.Y);
                    _overlay.Children.Insert(3, elementBackground);

                    // Create a layer for the element
                    UIElement element = new Image { Source = new WriteableBitmap(ownerElement, null) };
                    Canvas.SetLeft(element, point.X);
                    Canvas.SetTop(element, point.Y);
                    _overlay.Children.Insert(4, element);
                }

                // Prepare for scale animation
                double from = 1;
                double to = 0.94;
                TimeSpan timespan = TimeSpan.FromSeconds(0.42);
                IEasingFunction easingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut };
                _backgroundResizeStoryboard = new Storyboard();

                // Create an animation for the X scale
                DoubleAnimation animationX = new DoubleAnimation { From = from, To = to, Duration = timespan, EasingFunction = easingFunction };
                Storyboard.SetTarget(animationX, scaleTransform);
                Storyboard.SetTargetProperty(animationX, new PropertyPath(ScaleTransform.ScaleXProperty));
                _backgroundResizeStoryboard.Children.Add(animationX);

                // Create an animation for the Y scale
                DoubleAnimation animationY = new DoubleAnimation { From = from, To = to, Duration = timespan, EasingFunction = easingFunction };
                Storyboard.SetTarget(animationY, scaleTransform);
                Storyboard.SetTargetProperty(animationY, new PropertyPath(ScaleTransform.ScaleYProperty));
                _backgroundResizeStoryboard.Children.Add(animationY);

                if (IsFadeEnabled)
                {
                    DoubleAnimation animationFade = new DoubleAnimation { From = 0, To = .3, Duration = timespan, EasingFunction = easingFunction };
                    Storyboard.SetTarget(animationFade, backgroundFadeLayer);
                    Storyboard.SetTargetProperty(animationFade, new PropertyPath(Rectangle.OpacityProperty));
                    _backgroundResizeStoryboard.Children.Add(animationFade);
                }
            }

            // Create transforms for handling rotation
            TransformGroup transforms = new TransformGroup();
            if (null != _rootVisual)
            {
                switch (_rootVisual.Orientation)
                {
                    case PageOrientation.LandscapeLeft:
                        transforms.Children.Add(new RotateTransform { Angle = 90 });
                        transforms.Children.Add(new TranslateTransform { X = _rootVisual.ActualWidth });
                        break;
                    case PageOrientation.LandscapeRight:
                        transforms.Children.Add(new RotateTransform { Angle = -90 });
                        transforms.Children.Add(new TranslateTransform { Y = _rootVisual.ActualHeight });
                        break;
                }
            }
            _overlay.RenderTransform = transforms;

            // Add Click handler for ApplicationBar Buttons
            if ((null != _page) && (null != _page.ApplicationBar) && (null != _page.ApplicationBar.Buttons))
            {
                foreach (object obj in _page.ApplicationBar.Buttons)
                {
                    ApplicationBarIconButton button = obj as ApplicationBarIconButton;
                    if (null != button)
                    {
                        button.Click += OnEventThatClosesContextMenu;
                        _applicationBarIconButtons.Add(button);
                    }
                }
            }

            _overlay.Children.Add(this);

            _popup = new Popup { Child = _overlay };

            _popup.Opened += (s, e) =>
            {
                // When the popup is actually opened, call our OnOpened method
                OnOpened(new RoutedEventArgs());
            };

            SizeChanged += OnContextMenuOrRootVisualSizeChanged;
            if (null != _rootVisual)
            {
                _rootVisual.SizeChanged += OnContextMenuOrRootVisualSizeChanged;
            }

            UpdateContextMenuPlacement();

            if (ReadLocalValue(DataContextProperty) == DependencyProperty.UnsetValue)
            {
                DependencyObject dataContextSource = Owner ?? _rootVisual;
                SetBinding(DataContextProperty, new Binding("DataContext") { Source = dataContextSource });
            }

            _popup.IsOpen = true;
            Focus();

            // Update IsOpen
            _settingIsOpen = true;
            IsOpen = true;
            _settingIsOpen = false;
        }

        /// <summary>
        /// Closes the Popup.
        /// </summary>
        private void ClosePopup()
        {
            if (null != _backgroundResizeStoryboard)
            {
                // Swap all the From/To values to reverse the animation
                foreach (DoubleAnimation animation in _backgroundResizeStoryboard.Children)
                {
                    double temp = animation.From.Value;
                    animation.From = animation.To;
                    animation.To = temp;
                }

                // Capture member variables for delegate closure
                Popup popup = _popup;
                Panel overlay = _overlay;
                _backgroundResizeStoryboard.Completed += delegate
                {
                    // Clear/close popup and overlay
                    if (null != popup)
                    {
                        popup.IsOpen = false;
                        popup.Child = null;
                    }
                    if (null != overlay)
                    {
                        overlay.Children.Clear();
                    }
                };

                // Begin the reverse animation
                _backgroundResizeStoryboard.Begin();

                // Reset member variables
                _backgroundResizeStoryboard = null;
                _popup = null;
                _overlay = null;
            }
            else
            {
                if (null != _popup)
                {
                    _popup.IsOpen = false;
                    _popup.Child = null;
                    _popup = null;
                }
                if (null != _overlay)
                {
                    _overlay.Children.Clear();
                    _overlay = null;
                }
            }
            SizeChanged -= OnContextMenuOrRootVisualSizeChanged;
            if (null != _rootVisual)
            {
                _rootVisual.SizeChanged -= OnContextMenuOrRootVisualSizeChanged;
            }

            // Remove Click handler for ApplicationBar Buttons
            foreach (ApplicationBarIconButton button in _applicationBarIconButtons)
            {
                button.Click -= OnEventThatClosesContextMenu;
            }
            _applicationBarIconButtons.Clear();

            // Update IsOpen
            _settingIsOpen = true;
            IsOpen = false;
            _settingIsOpen = false;

            OnClosed(new RoutedEventArgs());
        }
    }
}
