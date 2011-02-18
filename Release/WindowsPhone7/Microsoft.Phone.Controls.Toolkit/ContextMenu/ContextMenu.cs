// (c) Copyright Microsoft Corporation.
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
#if WINDOWS_PHONE
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Shell;
#endif

#if WINDOWS_PHONE
namespace Microsoft.Phone.Controls
#else
namespace System.Windows.Controls
#endif
{
    /// <summary>
    /// Represents a pop-up menu that enables a control to expose functionality that is specific to the context of the control.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
#if WINDOWS_PHONE
    [TemplateVisualState(GroupName = VisibilityGroupName, Name = OpenVisibilityStateName)]
    [TemplateVisualState(GroupName = VisibilityGroupName, Name = ClosedVisibilityStateName)]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Code flow is reasonably clear.")]
#endif
    public class ContextMenu : MenuBase
    {
#if WINDOWS_PHONE
        /// <summary>
        /// Visibility state group.
        /// </summary>
        private const string VisibilityGroupName = "VisibilityStates";

        /// <summary>
        /// Open visibility state.
        /// </summary>
        private const string OpenVisibilityStateName = "Open";

        /// <summary>
        /// Closed visibility state.
        /// </summary>
        private const string ClosedVisibilityStateName = "Closed";

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
        private Storyboard _openingStoryboard;

        /// <summary>
        /// Tracks whether the Storyboard used to animate the ContextMenu open is active.
        /// </summary>
        private bool _openingStoryboardPlaying;

        /// <summary>
        /// Tracks the threshold for releasing contact during the ContextMenu open animation.
        /// </summary>
        private DateTime _openingStoryboardReleaseThreshold;
#endif

        /// <summary>
        /// Stores a reference to the current root visual.
        /// </summary>
#if WINDOWS_PHONE
        private PhoneApplicationFrame _rootVisual;
#else
        private FrameworkElement _rootVisual;
#endif

        /// <summary>
        /// Stores the last known mouse position (via MouseMove).
        /// </summary>
        private Point _mousePosition;

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
        /// Gets or sets the owning object for the ContextMenu.
        /// </summary>
        internal DependencyObject Owner
        {
            get { return _owner; }
            set
            {
                if (null != _owner)
                {
                    FrameworkElement ownerFrameworkElement = _owner as FrameworkElement;
                    if (null != ownerFrameworkElement)
                    {
#if WINDOWS_PHONE
                        GestureListener listener = GestureService.GetGestureListener(ownerFrameworkElement);
                        listener.Hold -= new EventHandler<GestureEventArgs>(HandleOwnerHold);
                        ownerFrameworkElement.Loaded -= new RoutedEventHandler(HandleOwnerLoaded);
                        ownerFrameworkElement.Unloaded -= new RoutedEventHandler(HandleOwnerUnloaded);
                        HandleOwnerUnloaded(null, null);
#else
                        ownerFrameworkElement.MouseRightButtonDown -= new MouseButtonEventHandler(HandleOwnerMouseRightButtonDown);
#endif
                    }
                }
                _owner = value;
                if (null != _owner)
                {
                    FrameworkElement ownerFrameworkElement = _owner as FrameworkElement;
                    if (null != ownerFrameworkElement)
                    {
#if WINDOWS_PHONE
                        GestureListener listener = GestureService.GetGestureListener(ownerFrameworkElement);
                        listener.Hold += new EventHandler<GestureEventArgs>(HandleOwnerHold);
                        ownerFrameworkElement.Loaded += new RoutedEventHandler(HandleOwnerLoaded);
                        ownerFrameworkElement.Unloaded += new RoutedEventHandler(HandleOwnerUnloaded);
                        // Owner *may* already be live and have fired its Loaded event - hook up manually if necessary
                        DependencyObject parent = ownerFrameworkElement;
                        while (parent != null)
                        {
                            parent = VisualTreeHelper.GetParent(parent);
                            if ((null != parent) && (parent == _rootVisual))
                            {
                                HandleOwnerLoaded(null, null);
                                break;
                            }
                        }
#else
                        ownerFrameworkElement.MouseRightButtonDown += new MouseButtonEventHandler(HandleOwnerMouseRightButtonDown);
#endif
                    }
                }
            }
        }

#if WINDOWS_PHONE
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
#else
        /// <summary>
        /// Gets or sets the horizontal distance between the target origin and the popup alignment point.
        /// </summary>
        [TypeConverterAttribute(typeof(LengthConverter))]
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(
            "HorizontalOffset",
            typeof(double),
            typeof(ContextMenu),
            new PropertyMetadata(0.0, OnHorizontalVerticalOffsetChanged));
#endif

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
            new PropertyMetadata(0.0, OnHorizontalVerticalOffsetChanged));

        /// <summary>
        /// Handles changes to the HorizontalOffset or VerticalOffset DependencyProperty.
        /// </summary>
        /// <param name="o">DependencyObject that changed.</param>
        /// <param name="e">Event data for the DependencyPropertyChangedEvent.</param>
        private static void OnHorizontalVerticalOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
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
                    OpenPopup(_mousePosition);
                }
                else
                {
                    ClosePopup();
                }
            }
        }

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
#if WINDOWS_PHONE
            GoToVisualState(OpenVisibilityStateName, true);
#endif
            RoutedEventHandler handler = Opened;
            if (null != handler)
            {
                handler.Invoke(this, e);
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
#if WINDOWS_PHONE
            GoToVisualState(ClosedVisibilityStateName, true);
#endif
            RoutedEventHandler handler = Closed;
            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }

        /// <summary>
        /// Initializes a new instance of the ContextMenu class.
        /// </summary>
        public ContextMenu()
        {
            DefaultStyleKey = typeof(ContextMenu);

            // Temporarily hook LayoutUpdated to find out when Application.Current.RootVisual gets set.
            LayoutUpdated += new EventHandler(HandleLayoutUpdated);
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Called when a new Template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Unhook from old Template
            if (null != _openingStoryboard)
            {
                _openingStoryboard.Completed -= new EventHandler(HandleStoryboardCompleted);
                _openingStoryboard = null;
            }
            _openingStoryboardPlaying = false;

            // Apply new template
            base.OnApplyTemplate();

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
                            if ((OpenVisibilityStateName == state.Name) && (null != state.Storyboard))
                            {
                                _openingStoryboard = state.Storyboard;
                                _openingStoryboard.Completed += new EventHandler(HandleStoryboardCompleted);
                            }
                        }
                    }
                }
            }

            // Go to correct visual state(s)
            GoToVisualState(ClosedVisibilityStateName, false);
            if (IsOpen)
            {
                // Handles initial open (where OnOpened is called before OnApplyTemplate)
                GoToVisualState(OpenVisibilityStateName, true);
            }
        }

        /// <summary>
        /// Handles the Completed event of the opening Storyboard.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleStoryboardCompleted(object sender, EventArgs e)
        {
            _openingStoryboardPlaying = false;
        }

        /// <summary>
        /// Uses VisualStateManager to go to a new visual state.
        /// </summary>
        /// <param name="stateName">The state to transition to.</param>
        /// <param name="useTransitions">true to use a System.Windows.VisualTransition to transition between states; otherwise, false.</param>
        private void GoToVisualState(string stateName, bool useTransitions)
        {
            if ((OpenVisibilityStateName == stateName) && (null != _openingStoryboard))
            {
                _openingStoryboardPlaying = true;
                _openingStoryboardReleaseThreshold = DateTime.UtcNow.AddSeconds(0.3);
            }
            VisualStateManager.GoToState(this, stateName, useTransitions);
        }
#endif

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

#if !WINDOWS_PHONE
        /// <summary>
        /// Called when the right mouse button is pressed.
        /// </summary>
        /// <param name="e">The event data for the MouseRightButtonDown event.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnMouseRightButtonDown(e);
        }
#endif

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
        private void HandleLayoutUpdated(object sender, EventArgs e)
        {
            if (null != Application.Current.RootVisual)
            {
                // Application.Current.RootVisual is valid now
                InitializeRootVisual();
                // Unhook event
                LayoutUpdated -= new EventHandler(HandleLayoutUpdated);
            }
        }

        /// <summary>
        /// Handles the RootVisual's MouseMove event to track the last mouse position.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleRootVisualMouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition = e.GetPosition(null);
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Handles the ManipulationCompleted event for the RootVisual.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleRootVisualManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
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
        private void HandleOwnerHold(object sender, GestureEventArgs e)
#else
        /// <summary>
        /// Handles the MouseRightButtonDown event for the owning element.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleOwnerMouseRightButtonDown(object sender, MouseButtonEventArgs e)
#endif
        {
            if (!IsOpen)
            {
                OpenPopup(e.GetPosition(null));
                e.Handled = true;
            }
        }

#if WINDOWS_PHONE
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
                oldValue.StateChanged -= new EventHandler<ApplicationBarStateChangedEventArgs>(HandleEventThatClosesContextMenu);
            }
            if (null != newValue)
            {
                newValue.StateChanged += new EventHandler<ApplicationBarStateChangedEventArgs>(HandleEventThatClosesContextMenu);
            }
        }

        /// <summary>
        /// Handles an event which should close an open ContextMenu.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleEventThatClosesContextMenu(object sender, EventArgs e)
        {
            // Close the ContextMenu because the elements and/or layout is likely to have changed significantly
            IsOpen = false;
        }

        /// <summary>
        /// Handles the Loaded event of the Owner.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleOwnerLoaded(object sender, RoutedEventArgs e)
        {
            if (null == _page) // Don't want to attach to BackKeyPress twice
            {
                InitializeRootVisual();
                if (null != _rootVisual)
                {
                    _page = _rootVisual.Content as PhoneApplicationPage;
                    if (_page != null)
                    {
                        _page.BackKeyPress += new EventHandler<CancelEventArgs>(HandlePageBackKeyPress);
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
        private void HandleOwnerUnloaded(object sender, RoutedEventArgs e)
        {
            if (_page != null)
            {
                _page.BackKeyPress -= new EventHandler<CancelEventArgs>(HandlePageBackKeyPress);
                ClearValue(ApplicationBarMirrorProperty);
                _page = null;
            }
        }

        /// <summary>
        /// Handles the BackKeyPress of the containing PhoneApplicationPage.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandlePageBackKeyPress(object sender, CancelEventArgs e)
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
#endif

        /// <summary>
        /// Initialize the _rootVisual property (if possible and not already done).
        /// </summary>
        private void InitializeRootVisual()
        {
            if (null == _rootVisual)
            {
                // Try to capture the Application's RootVisual
                _rootVisual = Application.Current.RootVisual as
#if WINDOWS_PHONE
                    PhoneApplicationFrame;
#else
                    FrameworkElement;
#endif
                if (null != _rootVisual)
                {
                    // Ideally, this would use AddHandler(MouseMoveEvent), but MouseMoveEvent doesn't exist
                    _rootVisual.MouseMove += new MouseEventHandler(HandleRootVisualMouseMove);

#if WINDOWS_PHONE
                    _rootVisual.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(HandleRootVisualManipulationCompleted);
                    _rootVisual.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(HandleEventThatClosesContextMenu);
#endif
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
        private void HandleContextMenuOrRootVisualSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateContextMenuPlacement();
        }

        /// <summary>
        /// Handles the MouseButtonDown events for the overlay.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void HandleOverlayMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClosePopup();
            e.Handled = true;
        }

        /// <summary>
        /// Updates the location and size of the Popup and overlay.
        /// </summary>
        private void UpdateContextMenuPlacement()
        {
            if ((null != _rootVisual) && (null != _overlay))
            {
                // Start with the current Popup alignment point
                double x = _popupAlignmentPoint.X;
                double y = _popupAlignmentPoint.Y;
                // Adjust for offset
#if !WINDOWS_PHONE
                x += HorizontalOffset;
#endif
                y += VerticalOffset;
#if WINDOWS_PHONE
                // Determine frame/page bounds
                bool portrait = (_rootVisual.Orientation & PageOrientation.Portrait) == PageOrientation.Portrait;
                double effectiveWidth = portrait ? _rootVisual.ActualWidth : _rootVisual.ActualHeight;
                double effectiveHeight = portrait ? _rootVisual.ActualHeight : _rootVisual.ActualWidth;
                Rect bounds = new Rect(0, 0, effectiveWidth, effectiveHeight);
                if (_page != null)
                {
                    bounds = SafeTransformToVisual(_page, _rootVisual).TransformBounds(new Rect(0, 0, _page.ActualWidth, _page.ActualHeight));
                }
                // Left align with full width
                x = bounds.Left;
                Width = bounds.Width;
                // Ensure the bottom is visible / ensure the top is visible
                y = Math.Min(y, bounds.Bottom - ActualHeight);
                y = Math.Max(y, bounds.Top);
#else
                // Try not to let it stick out too far to the right/bottom
                x = Math.Min(x, _rootVisual.ActualWidth - ActualWidth);
                y = Math.Min(y, _rootVisual.ActualHeight - ActualHeight);
#endif
                // Do not let it stick out too far to the left/top
                x = Math.Max(x, 0);
                y = Math.Max(y, 0);
                // Set the new location
                Canvas.SetLeft(this, x);
                Canvas.SetTop(this, y);
                // Size the overlay to match the new container
#if WINDOWS_PHONE
                _overlay.Width = effectiveWidth;
                _overlay.Height = effectiveHeight;
#else
                _overlay.Width = _rootVisual.ActualWidth;
                _overlay.Height = _rootVisual.ActualHeight;
#endif
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

            _overlay = new Canvas { Background = new SolidColorBrush(Colors.Transparent) };
            _overlay.MouseLeftButtonDown += new MouseButtonEventHandler(HandleOverlayMouseButtonDown);
#if !WINDOWS_PHONE
            _overlay.MouseRightButtonDown += new MouseButtonEventHandler(HandleOverlayMouseButtonDown);
#endif
            _overlay.Children.Add(this);

#if WINDOWS_PHONE
            if (IsZoomEnabled && (null != _rootVisual))
            {
                // Capture effective width/height
                bool portrait = PageOrientation.Portrait == (PageOrientation.Portrait & _rootVisual.Orientation);
                double width = portrait ? _rootVisual.ActualWidth : _rootVisual.ActualHeight;
                double height = portrait ? _rootVisual.ActualHeight : _rootVisual.ActualWidth;

                // Create a layer for the background brush
                UIElement backgroundLayer = new Rectangle
                {
                    Width = width,
                    Height = height,
                    Fill = (Brush)Application.Current.Resources["PhoneBackgroundBrush"],
                };
                _overlay.Children.Insert(0, backgroundLayer);

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
                };
                _overlay.Children.Insert(1, contentLayer);

                // Create a layer for the owner element and its background
                FrameworkElement ownerElement = _owner as FrameworkElement;
                if (null != ownerElement)
                {
                    Point point = SafeTransformToVisual(ownerElement, _rootVisual).Transform(new Point());

                    // Create a layer for the element's background
                    UIElement elementBackground = new Rectangle
                    {
                        Width = ownerElement.ActualWidth,
                        Height = ownerElement.ActualHeight,
                        Fill = (Brush)Application.Current.Resources["PhoneBackgroundBrush"],
                    };
                    Canvas.SetLeft(elementBackground, point.X);
                    Canvas.SetTop(elementBackground, point.Y);
                    _overlay.Children.Insert(2, elementBackground);

                    // Create a layer for the element
                    UIElement element = new Image { Source = new WriteableBitmap(ownerElement, null) };
                    Canvas.SetLeft(element, point.X);
                    Canvas.SetTop(element, point.Y);
                    _overlay.Children.Insert(3, element);
                }

                // Prepare for scale animation
                double from = 1;
                double to = 0.94;
                TimeSpan timespan = TimeSpan.FromSeconds(0.40);
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

                // Play the animation
                _backgroundResizeStoryboard.Begin();
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
                        button.Click += new EventHandler(HandleEventThatClosesContextMenu);
                        _applicationBarIconButtons.Add(button);
                    }
                }
            }
#endif

            _popup = new Popup { Child = _overlay };

            SizeChanged += new SizeChangedEventHandler(HandleContextMenuOrRootVisualSizeChanged);
            if (null != _rootVisual)
            {
                _rootVisual.SizeChanged += new SizeChangedEventHandler(HandleContextMenuOrRootVisualSizeChanged);
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

            OnOpened(new RoutedEventArgs());
        }

        /// <summary>
        /// Closes the Popup.
        /// </summary>
        private void ClosePopup()
        {
#if WINDOWS_PHONE
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
#endif
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
#if WINDOWS_PHONE
            }
#endif
            SizeChanged -= new SizeChangedEventHandler(HandleContextMenuOrRootVisualSizeChanged);
            if (null != _rootVisual)
            {
                _rootVisual.SizeChanged -= new SizeChangedEventHandler(HandleContextMenuOrRootVisualSizeChanged);
            }

#if WINDOWS_PHONE
            // Remove Click handler for ApplicationBar Buttons
            foreach (ApplicationBarIconButton button in _applicationBarIconButtons)
            {
                button.Click -= new EventHandler(HandleEventThatClosesContextMenu);
            }
            _applicationBarIconButtons.Clear();
#endif

            // Update IsOpen
            _settingIsOpen = true;
            IsOpen = false;
            _settingIsOpen = false;

            OnClosed(new RoutedEventArgs());
        }
    }
}
