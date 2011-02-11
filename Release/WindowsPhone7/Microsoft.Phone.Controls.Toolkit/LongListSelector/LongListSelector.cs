// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// A virtualizing list designed for grouped lists. Can also be used with flat lists.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), TemplatePart(Name = ItemsPanelName, Type = typeof(Panel))]
    [TemplatePart(Name = PanningTransformName, Type = typeof(TranslateTransform))]
    [TemplatePart(Name = VerticalScrollBarName, Type = typeof(ScrollBar))]
    public partial class LongListSelector : Control
    {
        // The names of the template parts
        private const string ItemsPanelName = "ItemsPanel";
        private const string PanningTransformName = "PanningTransform";
        private const string VerticalScrollBarName = "VerticalScrollBar";

        private Panel _itemsPanel;
        private TranslateTransform _panningTransform;
        private ScrollBar _verticalScrollbar;
        private Popup _groupSelectorPopup;

        private Storyboard _panelStoryboard;
        private DoubleAnimation _panelAnimation;
        private DateTime _gestureStart;

        // Timer that controls how long it takes before a flick will be stopped on a touch down
        private DispatcherTimer _stopTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(50) };
        private bool _ignoreNextTap;
        private bool _firstDragDuringFlick;
        private double _lastFlickVelocity;

        // Duration used for animations while panning, instead of matching finger position exactly
        private static readonly Duration _panDuration = new Duration(TimeSpan.FromMilliseconds(100));
        private readonly IEasingFunction _panEase = new ExponentialEase();

        // Time that dragging will wait before deciding that a flick is not happening
        private static readonly TimeSpan _flickStopWaitTime = TimeSpan.FromMilliseconds(20);

        private int _scrollingTowards = -1;

        private const double BufferSizeDefault = 1.0;
        private double _bufferSizeCache = BufferSizeDefault;
        private double _minimumPanelScroll = float.MinValue;
        private double _maximumPanelScroll = 0;

        private bool _isLoaded;

        private bool _isPanning;
        private bool _isFlicking;
        private bool _isStretching;

        private double _dragTarget;
        private bool _isAnimating;

        private Size _availableSize;

        private Stack<ContentPresenter> _recycledGroupHeaders = new Stack<ContentPresenter>();
        private Stack<ContentPresenter> _recycledGroupFooters = new Stack<ContentPresenter>();
        private Stack<ContentPresenter> _recycledItems = new Stack<ContentPresenter>();
        private ContentPresenter _recycledListHeader = null;
        private ContentPresenter _recycledListFooter = null;

        private List<ItemTuple> _flattenedItems;
        private object _firstGroup;

        private INotifyCollectionChanged _rootCollection;
        private List<INotifyCollectionChanged> _groupCollections;

        private int _resolvedFirstIndex;
        private int _resolvedCount;
        private int _screenFirstIndex;
        private int _screenCount;

        private bool _balanceNeededForSizeChanged;

        #region ItemsSource DependencyProperty

        /// <summary>
        /// The DataSource property. Where all of the items come from.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// The DataSource DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(LongListSelector), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((LongListSelector)obj).OnItemsSourceChanged();
        }

        private void OnItemsSourceChanged()
        {
            _flattenedItems = null;
            if (_isLoaded)
            {
                EnsureData();
            }
        }

        #endregion

        #region ListHeader DependencyProperty

        /// <summary>
        /// The ListHeader property. Will be used as the first scrollItem in the list.
        /// </summary>
        public object ListHeader
        {
            get { return (object)GetValue(ListHeaderProperty); }
            set { SetValue(ListHeaderProperty, value); }
        }

        /// <summary>
        /// The ListHeader DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ListHeaderProperty =
            DependencyProperty.Register("ListHeader", typeof(object), typeof(LongListSelector), new PropertyMetadata(null));

        #endregion

        #region ListHeaderTemplate DependencyProperty

        /// <summary>
        /// The ListHeaderTemplate provides the template for the ListHeader.
        /// </summary>
        public DataTemplate ListHeaderTemplate
        {
            get { return (DataTemplate)GetValue(ListHeaderTemplateProperty); }
            set { SetValue(ListHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// The ListHeaderTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ListHeaderTemplateProperty =
            DependencyProperty.Register("ListHeaderTemplate", typeof(DataTemplate), typeof(LongListSelector), new PropertyMetadata(null));

        #endregion

        #region ListFooter DependencyProperty

        /// <summary>
        /// The ListFooter property. Will be used as the first scrollItem in the list.
        /// </summary>
        public object ListFooter
        {
            get { return (object)GetValue(ListFooterProperty); }
            set { SetValue(ListFooterProperty, value); }
        }

        /// <summary>
        /// The ListFooter DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ListFooterProperty =
            DependencyProperty.Register("ListFooter", typeof(object), typeof(LongListSelector), new PropertyMetadata(null));

        #endregion

        #region ListFooterTemplate DependencyProperty

        /// <summary>
        /// The ListFooterTemplate provides the template for the ListFooter.
        /// </summary>
        public DataTemplate ListFooterTemplate
        {
            get { return (DataTemplate)GetValue(ListFooterTemplateProperty); }
            set { SetValue(ListFooterTemplateProperty, value); }
        }

        /// <summary>
        /// The ListFooterTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ListFooterTemplateProperty =
            DependencyProperty.Register("ListFooterTemplate", typeof(DataTemplate), typeof(LongListSelector), new PropertyMetadata(null));

        #endregion

        #region GroupHeaderTemplate DependencyProperty

        /// <summary>
        /// The GroupHeaderTemplate provides the template for the groups in the items view.
        /// </summary>
        public DataTemplate GroupHeaderTemplate
        {
            get { return (DataTemplate)GetValue(GroupHeaderProperty); }
            set { SetValue(GroupHeaderProperty, value); }
        }

        /// <summary>
        /// The GroupHeaderTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderProperty =
            DependencyProperty.Register("GroupHeaderTemplate", typeof(DataTemplate), typeof(LongListSelector), new PropertyMetadata(null));

        #endregion

        #region GroupFooterTemplate DependencyProperty

        /// <summary>
        /// The GroupFooterTemplate provides the template for the groups in the items view.
        /// </summary>
        public DataTemplate GroupFooterTemplate
        {
            get { return (DataTemplate)GetValue(GroupFooterProperty); }
            set { SetValue(GroupFooterProperty, value); }
        }

        /// <summary>
        /// The GroupFooterTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty GroupFooterProperty =
            DependencyProperty.Register("GroupFooterTemplate", typeof(DataTemplate), typeof(LongListSelector), new PropertyMetadata(null));

        #endregion

        #region ItemTemplate DependencyProperty

        /// <summary>
        /// The ItemTemplate provides the template for the items in the items view.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemsTemplateProperty); }
            set { SetValue(ItemsTemplateProperty, value); }
        }

        /// <summary>
        /// The ItemTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ItemsTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(LongListSelector), new PropertyMetadata(null));

        #endregion

        #region GroupItemTemplate DependencyProperty

        /// <summary>
        /// The GroupItemTemplate specifies the template that will be used in group view mode.
        /// </summary>
        public DataTemplate GroupItemTemplate
        {
            get { return (DataTemplate)GetValue(GroupItemTemplateProperty); }
            set { SetValue(GroupItemTemplateProperty, value); }
        }

        /// <summary>
        /// The GroupItemTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty GroupItemTemplateProperty =
            DependencyProperty.Register("GroupItemTemplate", typeof(DataTemplate), typeof(LongListSelector), new PropertyMetadata(null));

        #endregion

        #region GroupItemsPanel DependencyProperty

        /// <summary>
        /// The GroupItemsPanel specifies the panel that will be used in group view mode.
        /// </summary>
        public ItemsPanelTemplate GroupItemsPanel
        {
            get { return (ItemsPanelTemplate)GetValue(GroupItemsPanelProperty); }
            set { SetValue(GroupItemsPanelProperty, value); }
        }

        /// <summary>
        /// The GroupItemsPanel DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty GroupItemsPanelProperty =
            DependencyProperty.Register("GroupItemsPanel", typeof(ItemsPanelTemplate), typeof(LongListSelector), new PropertyMetadata(null));

        #endregion

        #region IsBouncy DependencyProperty

        /// <summary>
        /// Controls whether the list can be (temporarily) scrolled off of the ends.
        /// </summary>
        public bool IsBouncy
        {
            get { return (bool)GetValue(IsBouncyProperty); }
            set { SetValue(IsBouncyProperty, value); }
        }

        /// <summary>
        /// The IsBouncy DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsBouncyProperty =
            DependencyProperty.Register("IsBouncy", typeof(bool), typeof(LongListSelector), new PropertyMetadata(true));

        #endregion

        #region IsScrolling DependencyProperty

        /// <summary>
        /// Returns true if the user is manipulating the list, or if an inertial animation is taking place.
        /// </summary>
        public bool IsScrolling
        {
            get { return (bool)GetValue(IsScrollingProperty); }
            private set { SetValue(IsScrollingProperty, value); }
        }

        /// <summary>
        /// The IsScrolling DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsScrollingProperty =
            DependencyProperty.Register("IsScrolling", typeof(bool), typeof(LongListSelector), new PropertyMetadata(false, OnIsScrollingChanged));

        private static void OnIsScrollingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            LongListSelector control = (LongListSelector)obj;

            if ((bool) e.NewValue)
            {
                VisualStateManager.GoToState(control, "Scrolling", true);
                SafeRaise.Raise(control.ScrollingStarted, obj);
            }
            else
            {
                VisualStateManager.GoToState(control, "NotScrolling", true);
                control.BounceBack(false);
                SafeRaise.Raise(control.ScrollingCompleted, obj);
            }
        }

        #endregion

        #region ShowListHeader DependencyProperty

        /// <summary>
        /// Controls whether or not the ListHeader is shown.
        /// </summary>
        public bool ShowListHeader
        {
            get { return (bool)GetValue(ShowListHeaderProperty); }
            set { SetValue(ShowListHeaderProperty, value); }
        }

        /// <summary>
        /// The ShowListHeader DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ShowListHeaderProperty =
            DependencyProperty.Register("ShowListHeader", typeof(bool), typeof(LongListSelector), new PropertyMetadata(true, OnShowListHeaderChanged));

        private static void OnShowListHeaderChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            LongListSelector control = (LongListSelector)obj;

            if (control.HasListHeader && control._flattenedItems != null)
            {
                if (control.ShowListHeader)
                {
                    control.OnAdd(0, ItemType.ListHeader, null, new object[] {control.ListHeader});
                }
                else
                {
                    control.OnRemove(0, 1);
                }
                control.StopScrolling();
                control.ResetMinMax();
                control.Balance();
                control.BounceBack(true);
            }
        }

        #endregion

        #region ShowListFooter DependencyProperty

        /// <summary>
        /// Controls whether or not the ListFooter is shown.
        /// </summary>
        public bool ShowListFooter
        {
            get { return (bool)GetValue(ShowListFooterProperty); }
            set { SetValue(ShowListFooterProperty, value); }
        }

        /// <summary>
        /// The ShowListFooter DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ShowListFooterProperty =
            DependencyProperty.Register("ShowListFooter", typeof(bool), typeof(LongListSelector), new PropertyMetadata(true, OnShowListFooterChanged));

        private static void OnShowListFooterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            LongListSelector control = (LongListSelector)obj;

            if (control.HasListFooter && control._flattenedItems != null)
            {
                if (control.ShowListFooter)
                {
                    control.OnAdd(control._flattenedItems.Count, ItemType.ListFooter, null, new object[] { control.ListFooter });
                }
                else
                {
                    control.OnRemove(control._flattenedItems.Count - 1, 1);
                }
                control.StopScrolling();
                control.ResetMinMax();
                control.Balance();
                control.BounceBack(true);
            }
        }

        #endregion

        #region SelectedItem DependencyProperty

        private bool _setSelectionInternal;
        private bool _selectedItemChanged;
        private object[] EmptyList = { };
        private object[] _selectionList = new Object[1];

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// The SelectedItem DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(LongListSelector), new PropertyMetadata(OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((LongListSelector)obj).OnSelectedItemChanged(e);
        }

        private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            _selectedItemChanged = true;

            if (!_setSelectionInternal)
            {
                RaiseSelectionChangedEvent(e.NewValue);
            }
        }

        private void RaiseSelectionChangedEvent(object newSelection)
        {
            SelectionChangedEventHandler handler = SelectionChanged;
            if (handler != null)
            {
                _selectionList[0] = newSelection;
                handler(this, new SelectionChangedEventArgs(EmptyList, _selectionList));
            }
        }

        private void SetSelectedItemInternal(object newSelectedItem)
        {          
            _setSelectionInternal = true;
            _selectedItemChanged = false;
            SelectedItem = newSelectedItem;
            if (_selectedItemChanged)
            {
                RaiseSelectionChangedEvent(newSelectedItem);
            }
            _setSelectionInternal = false;
        }

        #endregion

        #region BufferSize DependencyProperty

        /// <summary>
        /// The number of "screens" (as defined by the ActualHeight of the LongListSelector) above and below the visible
        /// items of the list that will be filled with items.
        /// </summary>
        public double BufferSize
        {
            get { return (double)GetValue(BufferSizeProperty); }
            set { SetValue(BufferSizeProperty, value); }
        }

        /// <summary>
        /// The BufferSize DependencyProperty
        /// </summary>
        public static readonly DependencyProperty BufferSizeProperty =
            DependencyProperty.Register("BufferSize", typeof(double), typeof(LongListSelector), new PropertyMetadata(BufferSizeDefault, OnBufferSizeChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        private static void OnBufferSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            double newValue = (double)e.NewValue;

            if (newValue < 0)
            {
                throw new ArgumentOutOfRangeException("BufferSize");
            }

            ((LongListSelector)obj)._bufferSizeCache = newValue;
        }

        #endregion

        #region MaximumFlickVelocity DependencyProperty

        /// <summary>
        /// The maximum velocity for flicks, in pixels per second.
        /// </summary>
        public double MaximumFlickVelocity
        {
            get { return (double)GetValue(MaximumFlickVelocityProperty); }
            set { SetValue(MaximumFlickVelocityProperty, value); }
        }

        /// <summary>
        /// The MaximumFlickVelocity DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty MaximumFlickVelocityProperty =
            DependencyProperty.Register("MaximumFlickVelocity", typeof(double), typeof(LongListSelector), new PropertyMetadata(MotionParameters.MaximumSpeed, OnMaximumFlickVelocityChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        private static void OnMaximumFlickVelocityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            double newValue = (double)e.NewValue;

            double coercedNewValue = Math.Min(MotionParameters.MaximumSpeed, Math.Max(newValue, 1));

            if (newValue != coercedNewValue)
            {
                ((LongListSelector)obj).MaximumFlickVelocity = MotionParameters.MaximumSpeed;
                throw new ArgumentOutOfRangeException("MaximumFlickVelocity");
            }
        }

        #endregion

        #region DisplayAllGroups DependencyProperty

        /// <summary>
        /// Display all groups whether or not they have items.
        /// </summary>
        public bool DisplayAllGroups
        {
            get { return (bool)GetValue(DisplayAllGroupsProperty); }
            set { SetValue(DisplayAllGroupsProperty, value); }
        }

        /// <summary>
        /// DisplayAllGroups DependencyProperty
        /// </summary>
        public static readonly DependencyProperty DisplayAllGroupsProperty =
            DependencyProperty.Register("DisplayAllGroups", typeof(bool), typeof(LongListSelector), new PropertyMetadata(false, OnDisplayAllGroupsChanged));

        private static void OnDisplayAllGroupsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            LongListSelector lls = (LongListSelector)obj;
            lls._flattenedItems = null;
            if (lls._isLoaded)
            {
                lls.EnsureData();
            }
        }

        #endregion

        /// <summary>
        /// The SelectionChanged event.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Create a new instance of LongListSelector.
        /// </summary>
        public LongListSelector()
        {
            DefaultStyleKey = typeof(LongListSelector);

            SizeChanged += OnSizeChanged;

            GestureListener listener = GestureService.GetGestureListener(this);
            listener.GestureBegin += listener_GestureBegin;
            listener.GestureCompleted += listener_GestureCompleted;
            listener.DragStarted += listener_DragStarted;
            listener.DragDelta += listener_DragDelta;
            listener.DragCompleted += listener_DragCompleted;
            listener.Flick += listener_Flick;
            listener.Tap += listener_Tap;
            Loaded += LongListSelector_Loaded;
            Unloaded += LongListSelector_Unloaded;            
        }

        /// <summary>
        /// Raised when the user is manipulating the list.
        /// </summary>
        public event EventHandler ScrollingStarted;

        /// <summary>
        /// Raised when the user has finished a drag or a flick completes.
        /// </summary>
        public event EventHandler ScrollingCompleted;

        /// <summary>
        /// Raised when IsBouncy is true and the user has dragged the items down from the top as far as they can go.
        /// </summary>
        public event EventHandler StretchingTop;

        /// <summary>
        /// Raised when IsBouncy is true and the user has dragged the items up from the bottom as far as they can go.
        /// </summary>
        public event EventHandler StretchingBottom;

        /// <summary>
        /// Raised when the user is no longer stretching.
        /// </summary>
        public event EventHandler StretchingCompleted;

        /// <summary>
        /// Indicates that the ContentPresenter with the item is about to be "realized".
        /// </summary>
        public event EventHandler<LinkUnlinkEventArgs> Link;

        /// <summary>
        /// Indicates that the ContentPresenter with the item is being recycled and is becoming "un-realized".
        /// </summary>
        public event EventHandler<LinkUnlinkEventArgs> Unlink;

        /// <summary>
        /// Set to true when the list is flat instead of a group hierarchy.
        /// </summary>
        public bool IsFlatList { get; set; }

        /// <summary>
        /// Instantly jump to the specified item.
        /// </summary>
        /// <param name="item">The item to scroll to.</param>
        public void ScrollTo(object item)
        {
            EnsureData();
            UpdateLayout();

            ContentPresenter contentPresenter;
            int itemIndex = GetResolvedIndex(item, out contentPresenter);

            if (itemIndex != -1)
            {
                StopScrolling();
                _panningTransform.Y = -Canvas.GetTop(contentPresenter);
                Balance();
            }

            itemIndex = GetFlattenedIndex(item);

            if (itemIndex != -1)
            {
                RecycleAllItems();
                ResetMinMax();
                StopScrolling();
                _panningTransform.Y = 0;
                _resolvedFirstIndex = itemIndex;
                Balance();
                _panningTransform.Y = GetCoercedScrollPosition(_panningTransform.Y, false);
            }
        }

        /// <summary>
        /// Animate the scrolling of the list to the specified item. Scrolling speed is capped by MaximumFlickVelocity.
        /// </summary>
        /// <param name="item">The item to scroll to.</param>
        public void AnimateTo(object item)
        {
            EnsureData();
            UpdateLayout();

            ContentPresenter contentPresenter;
            int itemIndex = GetResolvedIndex(item, out contentPresenter);

            if (itemIndex != -1)
            {
                // The item we are scrolling to has already been resolved, so we can set up an animation directly
                // to it.

                double newPosition = GetCoercedScrollPosition(-Canvas.GetTop(contentPresenter), false);
                double delta = -newPosition + _panningTransform.Y;
                double seconds = Math.Abs(delta) / MaximumFlickVelocity;
                IEasingFunction ease = PhysicsConstants.GetEasingFunction(seconds);

                if (_scrollingTowards != -1)
                {
                    _scrollingTowards = -1;
                    // This is the termination of an AnimateTo call where the location was
                    // not know. Use a different ease to keep the animation smooth.
                    ease = new ExponentialEase() { EasingMode = EasingMode.EaseOut };
                }

                IsFlicking = true;
                AnimatePanel(new Duration(TimeSpan.FromSeconds(seconds)), ease, newPosition);
                return;
            }

            itemIndex = GetFlattenedIndex(item);

            if (itemIndex != -1)
            {
                // Since we don't know the pixel position of the item we are scrolling to, we just go
                // in a direction, and stop when the item is resolved.
                _scrollingTowards = itemIndex;
                ScrollTowards();
            }
        }

        /// <summary>
        /// Returns all of the items that are currently in view. This is not the same as the items that
        /// have associated visual elements: there are usually some visuals offscreen. This might be
        /// an empty list if scrolling is happening too quickly.
        /// </summary>
        /// <returns>The items in view.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public ICollection<object> GetItemsInView()
        {
            return GetItemsWithContainers(true, false);
        }

        /// <summary>
        /// Used to return either containers or items for either all items with containers or just the 
        /// visible ones, as specified by the parameters. 
        /// </summary>
        /// <param name="onlyItemsInView">When true, will return values for only items that are in view.</param>
        /// <param name="getContainers">When true, will return the containers rather than the items.</param>
        /// <returns>A collection of values as specified above.</returns>
        public ICollection<object> GetItemsWithContainers(bool onlyItemsInView, bool getContainers)
        {
            int start = onlyItemsInView ? _screenFirstIndex : _resolvedFirstIndex;
            int count = onlyItemsInView ? _screenCount : _resolvedCount;

            object[] items = new object[count];

            for (int index = start; index < start + count; ++index)
            {
                items[index - start] = getContainers ? _flattenedItems[index].ContentPresenter : _flattenedItems[index].Item;
            }
        
            return items;
        }

        private bool IsPanning
        {
            get { return _isPanning; }
            set
            {
                _isPanning = value;
                IsScrolling = IsPanning || IsFlicking;
            }
        }

        private bool IsFlicking
        {
            get { return _isFlicking; }
            set
            {
                _isFlicking = value;
                IsScrolling = IsPanning || IsFlicking;
            }
        }

        private bool IsStretching
        {
            get { return _isStretching; }
            set
            {
                if (_isStretching != value)
                {
                    _isStretching = value;
                    if (_isStretching)
                    {
                        if (_dragTarget < _minimumPanelScroll)
                        {
                            SafeRaise.Raise(StretchingBottom, this);
                        }
                        else
                        {
                            SafeRaise.Raise(StretchingTop, this);
                        }
                    }
                    else
                    {
                        SafeRaise.Raise(StretchingCompleted, this);
                    }
                }
            }
        }

        private bool HasListHeader { get { return ListHeaderTemplate != null || ListHeader is UIElement; } }

        private bool HasListFooter { get { return ListFooterTemplate != null || ListFooter is UIElement; } }

        void LongListSelector_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;

            EnsureData();
        }

        void LongListSelector_Unloaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = false;
            RecycleAllItems();
        }

        /// <summary>
        /// OnApplyTemplate override, used to locate template parts.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Find the template parts. Create dummy objects if parts are missing to avoid
            // null checks throughout the code (although we can't escape them completely.)
            _itemsPanel = GetTemplateChild(ItemsPanelName) as Panel ?? new Canvas();
            _panningTransform = GetTemplateChild(PanningTransformName) as TranslateTransform ?? new TranslateTransform();
            _verticalScrollbar = GetTemplateChild(VerticalScrollBarName) as ScrollBar ?? new ScrollBar();

            _panelAnimation = new DoubleAnimation();
            Storyboard.SetTarget(_panelAnimation, _panningTransform);
            Storyboard.SetTargetProperty(_panelAnimation, new PropertyPath("Y"));

            _panelStoryboard = new Storyboard();
            _panelStoryboard.Children.Add(_panelAnimation);
            _panelStoryboard.Completed += PanelStoryboardCompleted;

            Balance();
        }

        /// <summary>
        /// Override of the MeasureOverride function, to capture the available size.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The desired size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            _availableSize.Width = availableSize.Width;
            _availableSize.Height = double.PositiveInfinity;

            return base.MeasureOverride(availableSize);
        }

        #region Event handlers

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Clip = new RectangleGeometry() { Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height) };

            if (_isLoaded)
            {
                Balance();
            }
        }

        void listener_GestureBegin(object sender, GestureEventArgs e)
        {
            if (IsScrolling)
            {
                _ignoreNextTap = true;
            }

            _gestureStart = DateTime.Now;

            if (_isFlicking)
            {
                _stopTimer.Tick -= _stopTimer_Tick;
                _stopTimer.Tick += _stopTimer_Tick;
                _stopTimer.Start();
            }
        }

        void _stopTimer_Tick(object sender, EventArgs e)
        {
            StopScrolling();
            IsPanning = IsFlicking = false;
        }

        void listener_GestureCompleted(object sender, GestureEventArgs e)
        {
            _stopTimer.Tick -= _stopTimer_Tick;
            _ignoreNextTap = false;
        }

        private void listener_DragStarted(object sender, DragStartedGestureEventArgs e)
        {
            if (e.Direction != Orientation.Vertical)
            {
                return;
            }

            _stopTimer.Tick -= _stopTimer_Tick;
            _dragTarget = _panningTransform.Y;

            e.Handled = true;
        }

        private void listener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            if (e.Direction != Orientation.Vertical)
            {
                return;
            }

            TimeSpan elapsed = DateTime.Now - _gestureStart;

            e.Handled = true;

            if (elapsed > _flickStopWaitTime || Math.Sign(e.VerticalChange) != Math.Sign(_lastFlickVelocity))
            {
                IsPanning = true;
                IsFlicking = false;

                if (_firstDragDuringFlick)
                {
                    StopScrolling();
                    _firstDragDuringFlick = false;
                }
                else
                {
                    AnimatePanel(_panDuration, _panEase, _dragTarget += e.VerticalChange);
                    IsStretching = IsBouncy && (GetCoercedScrollPosition(_dragTarget, true) != _dragTarget);
                }
            }
        }

        private void listener_DragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            if (e.Direction != Orientation.Vertical)
            {
                return;
            }

            IsPanning = false;
            IsStretching = false;

            e.Handled = true;
        }

        private void listener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction != Orientation.Vertical)
            {
                return;
            }

            _stopTimer.Tick -= _stopTimer_Tick;
            double vMax = MaximumFlickVelocity;

            _lastFlickVelocity = Math.Min(vMax, Math.Max(e.VerticalVelocity, -vMax));

            Point velocity = new Point(0, _lastFlickVelocity);
            double flickDuration = PhysicsConstants.GetStopTime(velocity);
            Point flickEndPoint = PhysicsConstants.GetStopPoint(velocity);
            IEasingFunction flickEase = PhysicsConstants.GetEasingFunction(flickDuration);

            AnimatePanel(new Duration(TimeSpan.FromSeconds(flickDuration)), flickEase, _panningTransform.Y + flickEndPoint.Y);

            IsFlicking = true;
            _firstDragDuringFlick = true;
            _scrollingTowards = -1; 
            e.Handled = true;
        }

        void listener_Tap(object sender, GestureEventArgs e)
        {
            StopScrolling();
            IsPanning = IsFlicking = false;
        }

        private void StopScrolling()
        {
            double position = Math.Round(_panningTransform.Y);
            StopAnimation();
            _panningTransform.Y = position;
            _stopTimer.Tick -= _stopTimer_Tick;
            _scrollingTowards = -1;
        }

        private void GroupHeaderTap(object sender, GestureEventArgs e)
        {
            _stopTimer.Tick -= _stopTimer_Tick;

            if (!_ignoreNextTap)
            {
                DisplayGroupView();
            }
        }

        private void OnItemTap(object sender, GestureEventArgs e)
        {
            if (!_ignoreNextTap)
            {
                ContentPresenter cp = (ContentPresenter)sender;
                SetSelectedItemInternal(cp.Content);
            }
        }

        private void HandleGesture(object sender, GestureEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        private bool IsReady() { return _itemsPanel != null && ItemsSource != null && ActualHeight > 0; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void Balance()
        {
            if (!IsReady() || _flattenedItems.Count == 0)
            {
                // See comment in the call below. Necessary here for when the last item is removed.
                CollapseRecycledElements();
                _resolvedFirstIndex = _resolvedCount = _screenFirstIndex = _screenCount = 0;
                return;
            }

            double actualHeight = ActualHeight;
            double viewportTop = -(_bufferSizeCache * actualHeight);
            double viewportBottom = (_bufferSizeCache + 1) * actualHeight;
            double scrollPosition = _panningTransform.Y;

            ContentPresenter cc;
            double top = 0, bottom = 0;

            if (_resolvedCount > 0)
            {
                // Remove first?

                cc = FirstResolved.ContentPresenter;

                top = Canvas.GetTop(cc);
                bottom = top + cc.DesiredSize.Height;

                while (_resolvedCount > 0 && bottom + scrollPosition < viewportTop)
                {
                    RecycleFirst();

                    if (_resolvedCount > 0)
                    {
                        cc = FirstResolved.ContentPresenter;
                        top = Canvas.GetTop(cc);
                        bottom = top + cc.DesiredSize.Height;
                    }
                }

                // Remove last?

                if (_resolvedCount > 0)
                {
                    cc = LastResolved.ContentPresenter;
                    top = Canvas.GetTop(cc);
                    bottom = top + cc.DesiredSize.Height;

                    while (_resolvedCount > 0 && top + scrollPosition > viewportBottom)
                    {
                        RecycleLast();

                        if (_resolvedCount > 0)
                        {
                            cc = LastResolved.ContentPresenter;
                            top = Canvas.GetTop(cc);
                            bottom = top + cc.DesiredSize.Height;
                        }
                    }
                }

                if (_resolvedCount == 0)
                {
                    ResetMinMax();
                }
            }

            // Empty list?
            bool appendExtra = _resolvedCount == 0 && _resolvedFirstIndex == 0;

            if (_resolvedCount == 0)
            {
                //Debug.WriteLine("Adding first element");
                _resolvedFirstIndex = Math.Max(0, Math.Min(_resolvedFirstIndex, _flattenedItems.Count - 1));

                var t = _flattenedItems[_resolvedFirstIndex];
                cc = GetAndAddElementFor(t);
                cc.SetExtraData(_resolvedFirstIndex, cc.DesiredSize.Height);
                top = 0;
                Canvas.SetTop(cc, top);
                bottom = cc.DesiredSize.Height;

                if (_resolvedFirstIndex == 0)
                {
                    _maximumPanelScroll = 0;
                }
            }

            // Prepend?

            cc = FirstResolved.ContentPresenter;
            top = Canvas.GetTop(cc);
            bottom = top + cc.DesiredSize.Height;

            if (top + scrollPosition >= viewportTop && _resolvedFirstIndex == 0)
            {
                _maximumPanelScroll = -top;
                BrakeIfGoingTooFar();
            }
            else
            {
                while (top + scrollPosition >= viewportTop && _resolvedFirstIndex > 0)
                {
                    appendExtra = false;

                    _resolvedFirstIndex = Math.Max(0, Math.Min(_resolvedFirstIndex, _flattenedItems.Count - 1));
                    var t = _flattenedItems[--_resolvedFirstIndex];
                    //Debug.WriteLine("Adding {0}", t.Item);
                    cc = GetAndAddElementFor(t);
                    cc.SetExtraData(_resolvedFirstIndex, cc.DesiredSize.Height);
                    bottom = top;
                    top = bottom - cc.DesiredSize.Height;
                    Canvas.SetTop(cc, top);

                    if (_resolvedFirstIndex == 0)
                    {
                        _maximumPanelScroll = -top;
                         BrakeIfGoingTooFar();
                    }
                }
            }

            // Append?

            cc = LastResolved.ContentPresenter;
            top = Canvas.GetTop(cc);
            bottom = top + cc.DesiredSize.Height;

            if (appendExtra)
            {
                // If we went to the top of the list, generate extra items so that we don't need to generate them
                // as soon as scrolling starts.
                viewportBottom += ActualHeight * _bufferSizeCache;
            }

            while (bottom + scrollPosition <= viewportBottom && _resolvedFirstIndex + _resolvedCount < _flattenedItems.Count)
            {
                _resolvedFirstIndex = Math.Max(0, Math.Min(_resolvedFirstIndex, _flattenedItems.Count - 1));
                var t = _flattenedItems[_resolvedFirstIndex + _resolvedCount];
                //Debug.WriteLine("Adding {0}", t.Item);
                cc = GetAndAddElementFor(t);
                cc.SetExtraData(_resolvedFirstIndex + _resolvedCount - 1, cc.DesiredSize.Height);
                top = bottom;
                Canvas.SetTop(cc, top);
                bottom = top + cc.DesiredSize.Height;
            }

            if (_resolvedFirstIndex + _resolvedCount == _flattenedItems.Count)
            {
                _minimumPanelScroll = ActualHeight - bottom;
                if (_minimumPanelScroll > _maximumPanelScroll)
                {
                    _minimumPanelScroll = _maximumPanelScroll;
                }
                BrakeIfGoingTooFar();
            }

            // Determine which items are on the screen

            _screenFirstIndex = 0;
            _screenCount = 0;

            for (int itemIndex = _resolvedFirstIndex; itemIndex < _resolvedFirstIndex + _resolvedCount; ++itemIndex)
            {
                ContentPresenter cp = _flattenedItems[itemIndex].ContentPresenter;
                top = Canvas.GetTop(cp) + scrollPosition;
                bottom = top + cp.DesiredSize.Height;

                if ((top >= 0 && top <= ActualHeight) || (top < 0 && bottom > 0))
                {
                    if (_screenCount == 0)
                    {
                        _screenFirstIndex = itemIndex;
                    }
                    ++_screenCount;
                }
                else if (_screenCount != 0)
                {
                    break;
                }
            }

            // Adjust the scrollbar

            double max = Math.Max(1, _flattenedItems.Count - _screenCount);
            _verticalScrollbar.Maximum = max;
            _verticalScrollbar.Value = Math.Min(max, _screenFirstIndex);
            if (Math.Abs(_screenCount - _verticalScrollbar.Value) > 1)
            {
                _verticalScrollbar.ViewportSize = Math.Max(1, _screenCount);
            }

            // This must be done to ensure proper functionality of controls, e.g. Button.
            // It ensures that only items left in the recycle bin get collapsed, since
            // collapsing items and then immediately making them visible can have strange
            // effects, e.g. with a Button.Click on the stack.
            CollapseRecycledElements();

            // When AnimateTo is called, but the location of the item is not know (because it is not resolved) then
            // the list is just scrolled in the right direction until the item is discovered to be in the resolved 
            // items list.
            if (_scrollingTowards >= _resolvedFirstIndex && _scrollingTowards < _resolvedFirstIndex + _resolvedCount)
            {
                // Since the specified item now has a known location, scroll to it.
                AnimateTo(_flattenedItems[_scrollingTowards].Item);
            }
        }

        private ItemTuple FirstResolved
        {
            get
            {
                return _flattenedItems[_resolvedFirstIndex];
            }
        }

        private ItemTuple LastResolved
        {
            get
            {
                int index = _resolvedFirstIndex + _resolvedCount - 1;
                return _flattenedItems[index];
            }
        }

        private void RecycleFirst()
        {
            if (_resolvedCount > 0)
            {
                var t = _flattenedItems[_resolvedFirstIndex++];
                RemoveAndAddToRecycleBin(t);
            }
        }

        private void RecycleLast()
        {
            if (_resolvedCount > 0)
            {
                var t = _flattenedItems[_resolvedFirstIndex + _resolvedCount - 1];
                RemoveAndAddToRecycleBin(t);
            }
        }

        private void RemoveAndAddToRecycleBin(ItemTuple tuple)
        {
            ContentPresenter cp = tuple.ContentPresenter;

            switch (tuple.ItemType)
            {
                case ItemType.Item:
                    _recycledItems.Push(cp);
                    break;
                case ItemType.GroupHeader:
                    _recycledGroupHeaders.Push(cp);
                    break;
                case ItemType.GroupFooter:
                    _recycledGroupFooters.Push(cp);
                    break;
                case ItemType.ListHeader:
                    Debug.Assert(_recycledListHeader == null);
                    _recycledListHeader = cp;
                    break;
                case ItemType.ListFooter:
                    Debug.Assert(_recycledListFooter == null);
                    _recycledListFooter = cp;
                    break;
            }

            EventHandler<LinkUnlinkEventArgs> handler = Unlink;
            if (handler != null)
            {
                handler(this, new LinkUnlinkEventArgs(cp));
            }
            tuple.ContentPresenter = null;
            cp.Content = null;
            cp.SetExtraData(-1, 0);

            --_resolvedCount;
        }

        private void CollapseRecycledElements()
        {
            foreach (ContentPresenter cp in _recycledItems)
            {
                cp.Visibility = Visibility.Collapsed;
            }

            foreach (ContentPresenter cp in _recycledGroupHeaders)
            {
                cp.Visibility = Visibility.Collapsed;
            }

            foreach (ContentPresenter cp in _recycledGroupFooters)
            {
                cp.Visibility = Visibility.Collapsed;
            }

            if (_recycledListHeader != null)
            {
                _recycledListHeader.Visibility = Visibility.Collapsed;
            }

            if (_recycledListFooter != null)
            {
                _recycledListFooter.Visibility = Visibility.Collapsed;
            }
        }

        private void EmptyRecycleBin()
        {
            if (_recycledItems != null)
            {
                _recycledItems.Clear();
            }

            if (_recycledGroupHeaders != null)
            {
                _recycledGroupHeaders.Clear();
            }

            if (_recycledGroupFooters != null)
            {
                _recycledGroupFooters.Clear();
            }

            _recycledListHeader = null;
            _recycledListFooter = null;
        }

        private void RecycleAllItems()
        {
            while (_resolvedCount > 0)
            {
                RecycleFirst();
            }

            _resolvedFirstIndex = 0;
        }

        private ContentPresenter GetAndAddElementFor(ItemTuple tuple)
        {
            ContentPresenter cp = null;
            bool isNew = false;

            switch (tuple.ItemType)
            {
                case ItemType.Item:
                    if (_recycledItems.Count > 0)
                    {
                        cp = _recycledItems.Pop();
                    }
                    else
                    {
                        isNew = true;
                        cp = new ContentPresenter();
                        cp.ContentTemplate = ItemTemplate;

                        GestureService.GetGestureListener(cp).Tap += OnItemTap;
                    }
                    break;
                case ItemType.GroupHeader:
                    if (_recycledGroupHeaders.Count > 0)
                    {
                        cp = _recycledGroupHeaders.Pop();
                    }
                    else
                    {
                        isNew = true;
                        cp = new ContentPresenter();
                        cp.ContentTemplate = GroupHeaderTemplate;

                        GestureService.GetGestureListener(cp).Tap += GroupHeaderTap;
                    }
                    break;
                case ItemType.GroupFooter:
                    if (_recycledGroupFooters.Count > 0)
                    {
                        cp = _recycledGroupFooters.Pop();
                    }
                    else
                    {
                        isNew = true;
                        cp = new ContentPresenter();
                        cp.ContentTemplate = GroupFooterTemplate;
                    }
                    break;
                case ItemType.ListHeader:
                    if (_recycledListHeader != null)
                    {
                        cp = _recycledListHeader;
                        _recycledListHeader = null;
                    }
                    else
                    {
                        isNew = true;
                        cp = new ContentPresenter();
                        cp.ContentTemplate = ListHeaderTemplate;
                    }
                    break;
                case ItemType.ListFooter:
                    if (_recycledListFooter != null)
                    {
                        cp = _recycledListFooter;
                        _recycledListFooter = null;
                    }
                    else
                    {
                        isNew = true;
                        cp = new ContentPresenter();
                        cp.ContentTemplate = ListFooterTemplate;
                    }
                    break;
                default:
                    break;
            }

            if (isNew)
            {
                cp.CacheMode = new BitmapCache();
                _itemsPanel.Children.Add(cp);
                cp.SizeChanged += new SizeChangedEventHandler(OnItemSizeChanged);
            }

            if (cp != null)
            {
                if (cp.Width != _availableSize.Width)
                {
                    cp.Width = _availableSize.Width;
                }
                cp.Content = tuple.Item;
                cp.Visibility = Visibility.Visible;
            }

            EventHandler<LinkUnlinkEventArgs> handler = Link;
            if (handler != null)
            {
                handler(this, new LinkUnlinkEventArgs(cp));
            }
            
            tuple.ContentPresenter = cp;
            cp.Measure(_availableSize);            
            ++_resolvedCount;
            return cp;
        }

        private void OnItemSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // This will get called when items get containers as well as when an item that already
            // have containers are changed. The initial container size is saved after the Measure
            // call in Balance, and updated here if necessary. It is compared to the incoming size
            // to see if the size of the container really changed or if the event is being raised
            // because the container was recycled.

            ContentPresenter cp = sender as ContentPresenter;
            double delta = e.NewSize.Height - e.PreviousSize.Height;
            _minimumPanelScroll -= delta;

            if (cp != null)
            {
                int index;
                double lastDesiredHeight;
                cp.GetExtraData(out index, out lastDesiredHeight);

                if (lastDesiredHeight == e.NewSize.Height)
                {
                    return;
                }
                
                cp.SetExtraData(index, e.NewSize.Height);

                if (index < _screenFirstIndex)
                {
                    // Adjust the item that changed and all previous items
                    while (index >= _resolvedFirstIndex)
                    {
                        double top = Canvas.GetTop(_flattenedItems[index].ContentPresenter);
                        top -= delta;
                        Canvas.SetTop(_flattenedItems[index].ContentPresenter, top);
                        --index;
                    }
                }
                else
                {
                    // Adjust items AFTER the item that changed
                    int resolvedLastIndex = _resolvedFirstIndex + _resolvedCount - 1;
                    ++index; // The item that changed size doesn't have to move
                    while (index <= resolvedLastIndex)
                    {
                        double top = Canvas.GetTop(_flattenedItems[index].ContentPresenter);
                        top += delta;
                        Canvas.SetTop(_flattenedItems[index].ContentPresenter, top);
                        ++index;
                    }
                }

                if (!_balanceNeededForSizeChanged)
                {
                    _balanceNeededForSizeChanged = true;
                    LayoutUpdated += LongListSelector_LayoutUpdated;
                }
            }
        }

        void LongListSelector_LayoutUpdated(object sender, EventArgs e)
        {
            _balanceNeededForSizeChanged = false;
            LayoutUpdated -= LongListSelector_LayoutUpdated;

            Balance();
        }

        private void EnsureData()
        {
            if (_flattenedItems == null || _flattenedItems.Count == 0)
            {
                FlattenData();
                Balance();
            }
        }

        private void FlattenData()
        {
            bool groupHeaderExists = GroupHeaderTemplate != null;
            bool groupFooterExists = GroupFooterTemplate != null;
            bool displayAllGroups = DisplayAllGroups;

            _flattenedItems = new List<ItemTuple>();
            _firstGroup = null;

            SetSelectedItemInternal(null);

            _resolvedFirstIndex = 0;
            _resolvedCount = 0;

            _firstGroup = null;

            if (_panningTransform != null)
            {
                StopAnimation();
                _panningTransform.Y = 0;
                ResetMinMax();
            }
            
            ResetMinMax();

            if (_itemsPanel != null)
            {
                _itemsPanel.Children.Clear();
            }

            EmptyRecycleBin();

            if (_rootCollection != null)
            {
                _rootCollection.CollectionChanged -= OnRootCollectionChanged;
            }
            _rootCollection = null;

            if (_groupCollections != null)
            {
                foreach (INotifyCollectionChanged gc in _groupCollections)
                {
                    if (gc != null)
                    {
                        gc.CollectionChanged -= OnGroupCollectionChanged;
                    }
                }
            }
            _groupCollections = new List<INotifyCollectionChanged>();

            if (ItemsSource == null)
            {
                return;
            }

            if (HasListHeader && ShowListHeader)
            {
                _flattenedItems.Add(new ItemTuple() { ItemType = ItemType.ListHeader, Item = ListHeader });
            }

            foreach (object group in ItemsSource)
            {
                object groupRequiringFooter = null;

                if (IsFlatList)
                {
                    _flattenedItems.Add(new ItemTuple() { ItemType = ItemType.Item, Group = group, Item = group });
                }
                else
                {
                    IEnumerable itemEnumerable = (IEnumerable) group;
                    IEnumerator itemEnumerator = itemEnumerable.GetEnumerator();

                    bool hasItems = itemEnumerator.MoveNext();

                    if (hasItems || displayAllGroups)
                    {
                        if (groupHeaderExists)
                        {
                            _flattenedItems.Add(new ItemTuple() { ItemType = ItemType.GroupHeader, Group = group, Item = group });
                        }

                        if (groupFooterExists)
                        {
                            groupRequiringFooter = group;
                        }

                        if (_firstGroup == null)
                        {
                            _firstGroup = group;
                        }
                    }

                    if (hasItems)
                    {
                        while (hasItems)
                        {
                            _flattenedItems.Add(new ItemTuple() { ItemType = ItemType.Item, Group = group, Item = itemEnumerator.Current });
                            hasItems = itemEnumerator.MoveNext();
                        }
                    }

                    if (groupRequiringFooter != null)
                    {
                        _flattenedItems.Add(new ItemTuple() { ItemType = ItemType.GroupFooter, Group = group, Item = group });
                        groupRequiringFooter = null;
                    }

                    AddGroupNotifyCollectionChanged(group);

                }
            }

            _rootCollection = ItemsSource as INotifyCollectionChanged;
            if (_rootCollection != null)
            {
                _rootCollection.CollectionChanged += OnRootCollectionChanged;
            }

            if (HasListFooter && ShowListFooter)
            {
                _flattenedItems.Add(new ItemTuple() { ItemType = ItemType.ListFooter, Item = ListFooter });
            }

            if (_verticalScrollbar != null)
            {
                _verticalScrollbar.Maximum = _flattenedItems.Count;
            }
        }

        private void AddGroupNotifyCollectionChanged(object group)
        {
            INotifyCollectionChanged groupNCC = group as INotifyCollectionChanged;
            if (groupNCC != null)
            {
                _groupCollections.Add(groupNCC);
                groupNCC.CollectionChanged += OnGroupCollectionChanged;
            }
        }

        private void RemoveGroupNotifyCollectionChanged(object group)
        {
            INotifyCollectionChanged groupNCC = group as INotifyCollectionChanged;
            if (groupNCC != null)
            {
                _groupCollections.Remove(groupNCC);
                groupNCC.CollectionChanged -= OnGroupCollectionChanged;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void OnRootCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int headerOffset = HasListHeader && ShowListHeader ? 1 : 0;

            if (IsFlatList)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        OnAdd(headerOffset + e.NewStartingIndex, ItemType.Item, null, e.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        OnRemove(headerOffset + e.OldStartingIndex, e.OldItems.Count);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        if (e.NewItems.Count != 1)
                        {
                            throw new NotSupportedException();
                        }
                        OnReplace(headerOffset + e.NewStartingIndex, e.NewItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _flattenedItems = null;
                        EnsureData();
                        break;
                }
            }
            else
            {
                bool displayAllGroups = DisplayAllGroups;

                int groupHeaderOffset = GroupHeaderTemplate != null ? 1 : 0;
                int groupFooterOffset = GroupFooterTemplate != null ? 1 : 0;

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            IList groupList = (IList)ItemsSource;
                            object groupAtIndex = e.NewStartingIndex < groupList.Count ? groupList[e.NewStartingIndex] : null;
                            int offset = groupAtIndex != null ? GetGroupOffset(groupAtIndex) : groupList.Count;
                            foreach (object group in e.NewItems)
                            {
                                AddGroupNotifyCollectionChanged(group);

                                IList itemsList = GetItemsInGroup(group);
                                if (itemsList.Count > 0 || displayAllGroups)
                                {
                                    if (groupHeaderOffset == 1)
                                    {
                                        OnAdd(offset, ItemType.GroupHeader, group, new object[] { group });
                                        ++offset;
                                    }

                                    if (itemsList.Count > 0)
                                    {
                                        OnAdd(offset, ItemType.Item, group, itemsList);
                                        offset += itemsList.Count;
                                    }

                                    if (groupFooterOffset == 1)
                                    {
                                        OnAdd(offset, ItemType.GroupFooter, group, new object[] { group });
                                        ++offset;
                                    }
                                }
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        {
                            foreach (object group in e.OldItems)
                            {
                                RemoveGroupNotifyCollectionChanged(group);
                                IList itemsList = GetItemsInGroup(group);
                                int offset = GetGroupOffset(group);
                                int count = itemsList.Count;
                                if (displayAllGroups || itemsList.Count > 0)
                                {
                                    count += groupHeaderOffset + groupFooterOffset;
                                }

                                OnRemove(offset, count);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        throw new NotSupportedException();
                    case NotifyCollectionChangedAction.Reset:
                        _flattenedItems = null;
                        EnsureData();
                        break;
                }

            }

            ResetMinMax();
            Balance();
            if (BounceBack(true))
            {
                // The group could have been inserted at the beginning of the list, when 
                // there are not enough items to fill the screen.
                Balance();
            }
        }

        private static IList GetItemsInGroup(object group)
        {
            IList itemsList = group as IList;
            if (itemsList != null)
            {
                return itemsList;
            }

            List<object> items = new List<object>();

            IEnumerator itemEnum = ((IEnumerable)group).GetEnumerator();
            bool hasItems = itemEnum.MoveNext();

            while (hasItems)
            {
                items.Add(itemEnum.Current);
                hasItems = itemEnum.MoveNext();
            }

            return items;
        }

        /// <summary>
        /// Returns true if the group has no items
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <returns>True if the group has no items.</returns>
        private static bool IsGroupEmpty(object group)
        {
            IList itemsList = group as IList;
            if (itemsList != null)
            {
                return itemsList.Count == 0;
            }

            IEnumerator itemEnum = ((IEnumerable)group).GetEnumerator();
            return !itemEnum.MoveNext();
        }

        void OnGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            object group = sender;
            int groupHeaderOffset = GroupHeaderTemplate != null ? 1 : 0;
            int groupFooterOffset = GroupFooterTemplate != null ? 1 : 0;
            bool displayAllGroups = DisplayAllGroups;
            int itemsInGroupCount;
            
            int offset = GetGroupOffset(group, out itemsInGroupCount);

            IList groupList = group as IList;
            if (!displayAllGroups && groupList != null && e.Action == NotifyCollectionChangedAction.Add && groupList.Count == e.NewItems.Count)
            {
                if (groupHeaderOffset == 1)
                {
                    OnAdd(offset, ItemType.GroupHeader, group, new object[] { group });                    
                }

                if (groupFooterOffset == 1)
                {
                    OnAdd(offset + groupHeaderOffset, ItemType.GroupFooter, group, new object[] { group });
                }
            }

            offset += groupHeaderOffset;
          
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnAdd(e.NewStartingIndex + offset, ItemType.Item, group, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        int count = e.OldItems.Count;
                        if (IsGroupEmpty(group) && !displayAllGroups)
                        {
                            // The last items in the group are going away; remove the group, too.
                            offset -= groupHeaderOffset;
                            count += groupHeaderOffset + groupFooterOffset;
                        }
                        OnRemove(e.OldStartingIndex + offset, count);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems.Count != 1)
                    {
                        throw new NotSupportedException();
                    }
                    OnReplace(offset + e.NewStartingIndex, e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (DisplayAllGroups)
                    {
                        OnRemove(offset, itemsInGroupCount);
                    }
                    else if (itemsInGroupCount > 0)
                    {
                        OnRemove(offset - groupHeaderOffset, itemsInGroupCount + groupHeaderOffset + groupFooterOffset);
                    }
                    break;
            }

            ResetMinMax();
            Balance();
            if (BounceBack(true))
            {
                // The group could have been inserted at the beginning of the list, when 
                // there are not enough items to fill the screen.
                Balance();
            }
        }

        private int GetGroupOffset(object group)
        {
            int listHeaderOffset = HasListHeader && ShowListHeader ? 1 : 0;
            // int listFooterOffset = HasListFooter && ShowListFooter ? 1 : 0;

            bool displayAll = DisplayAllGroups;
            int groupHeaderOffset = GroupHeaderTemplate != null ? 1 : 0;
            int groupFooterOffset = GroupFooterTemplate != null ? 1 : 0;

            int offset = listHeaderOffset;
            
            foreach (var g in ItemsSource)
            {
                if (g.Equals(group))
                {
                    break;
                }

                int groupCount = 0;

                var groupList = g as IList;
                if (groupList != null)
                {
                    groupCount = groupList.Count;
                }
                else
                {
                    var groupEnum = g as IEnumerable;
                    if (groupEnum != null)
                    {
                        IEnumerator enumerator = groupEnum.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ++groupCount;
                        }
                    }
                }

                if (displayAll || groupCount > 0)
                {
                    offset += groupHeaderOffset + groupFooterOffset;
                }

                offset += groupCount;
            }

            return offset;
        }

        // Returns the first offset after all preceding groups. This could be 
        // the position of the group header, the first group item (if there is 
        // no header) or the place where the group should be inserted.
        // We have to count the items this way because this is used when clearing
        // a group, and the items will already have been removed from the group so
        // we can't get the count from there.
        private int GetGroupOffset(object group, out int itemsInGroupCount)
        {
            int offset = GetGroupOffset(group);

            int lastGroupOffset = offset;
            itemsInGroupCount = 0;
            while (lastGroupOffset < _flattenedItems.Count && group.Equals(_flattenedItems[lastGroupOffset].Group))
            {
                if (_flattenedItems[lastGroupOffset].ItemType == ItemType.Item)
                {
                    ++itemsInGroupCount;
                }

                ++lastGroupOffset;               
            }

            return offset;
        }

        private void OnAdd(int startingIndex, ItemType itemType, object group, IList newItems)
        {
            int resolvedLastIndex = _resolvedFirstIndex + _resolvedCount - 1;

            // Perform the Add operation

            ItemTuple[] newData = new ItemTuple[newItems.Count];
            for (int index = 0; index < newItems.Count; ++index)
            {
                newData[index] = new ItemTuple() { ItemType = itemType, Group = group, Item = newItems[index] };
            }

            if (startingIndex <= _resolvedFirstIndex || startingIndex > resolvedLastIndex)
            {
                // If the operation is completely outside the bounds of the resolved items, then it can just happen.
                
                if (startingIndex <= _resolvedFirstIndex)
                {
                    if (_resolvedCount > 0)
                    {
                        _resolvedFirstIndex += newItems.Count;
                    }
                    else
                    {
                        _resolvedFirstIndex = 0;
                    }
                }
            }
            else if (startingIndex < _screenFirstIndex)
            {
                int removeCount = startingIndex - _resolvedFirstIndex + 1;

                while (removeCount-- > 0)
                {
                    RecycleFirst();
                }

                _resolvedFirstIndex += newItems.Count;
            }
            else
            {
                int removeCount = _resolvedCount - startingIndex + _resolvedFirstIndex;

                while (removeCount-- > 0)
                {
                    RecycleLast();
                }
            }

            // Perform the Add operation
            _flattenedItems.InsertRange(startingIndex, newData);
        }

        private void OnRemove(int startingIndex, int count)
        {
            int endIndex = startingIndex + count - 1;

            int resolvedLastIndex = _resolvedFirstIndex + _resolvedCount - 1;

            // If the operation is completely outside the bounds of the resolved items,
            // then it can just happen.
            if (endIndex < _resolvedFirstIndex || startingIndex > resolvedLastIndex)
            {
                if (startingIndex < _resolvedFirstIndex)
                {
                    if (startingIndex + count - 1 < _resolvedFirstIndex)
                    {
                        _resolvedFirstIndex -= count;
                    }
                }
            }
            else 
            {
                if (startingIndex >= _screenFirstIndex)
                {
                    // The deletion point is either on the screen or after the screen.

                    int removeCount = resolvedLastIndex - startingIndex + 1;

                    while (removeCount-- > 0)
                    {
                        RecycleLast();
                    }
                }
                else
                {
                    // The deletion starts before the screen

                    int recycleCount = Math.Min(_resolvedCount, startingIndex - _resolvedFirstIndex + 1);

                    while (recycleCount-- > 0)
                    {
                        RecycleFirst();
                    }

                    _resolvedFirstIndex -= count;
                }
            }

            // Perform the remove operation
            _flattenedItems.RemoveRange(startingIndex, count);
        }

        /// <summary>
        /// Replaces a single object in a group
        /// </summary>
        /// <param name="index">The global index of the item.</param>
        /// <param name="item">The new item.</param>
        private void OnReplace(int index, object item)
        {
            int resolvedLastIndex = _resolvedFirstIndex + _resolvedCount - 1;

            if (index >= _resolvedFirstIndex && index <= resolvedLastIndex)
            {
                if (index < _resolvedFirstIndex)
                {
                    int count = _resolvedFirstIndex - index + 1;
                    while (count-- > 0)
                    {
                        RecycleFirst();
                    }
                }
                else
                {
                    int count = resolvedLastIndex - index + 1;
                    while (count-- > 0)
                    {
                        RecycleLast();
                    }
                }
            }

            _flattenedItems[index].Item = item;
        }


        private void ResetMinMax()
        {
            _minimumPanelScroll = float.MinValue;
            _maximumPanelScroll = float.MaxValue;
        }

        private void AnimatePanel(Duration duration, IEasingFunction ease, double to)
        {
            // Be sure not to run past the first or last items
            double newTo = GetCoercedScrollPosition(to, IsBouncy);
            if (to != newTo)
            {
                // Adjust the duration
                double originalDelta = Math.Max(Math.Abs(_panningTransform.Y - to), 1);
                double modifiedDelta = Math.Abs(_panningTransform.Y - newTo);
                double factor = modifiedDelta / originalDelta;

                // If factor > 0, the edge has been detected, but it hasn't gone too far yet, so readjust the animation. 
                // If factor < 0, somehow scrolling has gone past the edge already, so snap back.
                duration = factor <= 1 && factor >= 0 && duration.HasTimeSpan ? new Duration(TimeSpan.FromMilliseconds(duration.TimeSpan.Milliseconds * factor)) : _panDuration;

                to = newTo;
            }

            double from = _panningTransform.Y;
            StopAnimation();
            CompositionTarget.Rendering += AnimationPerFrameCallback;

            _panelAnimation.Duration = duration;
            _panelAnimation.EasingFunction = ease;
            _panelAnimation.From = from;
            _panelAnimation.To = to;
            _panelStoryboard.Begin();
            _panelStoryboard.SeekAlignedToLastTick(TimeSpan.Zero);
            _isAnimating = true;
        }

        private void BrakeIfGoingTooFar()
        {
            if (_isAnimating && _panelAnimation.To.HasValue)
            {
                double to = _panelAnimation.To.Value;
                double newTo = GetCoercedScrollPosition(to, IsBouncy);
                if (newTo != _panelAnimation.To.Value)
                {
                    double originalDelta = Math.Max(_panelAnimation.To.Value - _panelAnimation.From.Value, 1);
                    double remainingDelta = newTo - _panningTransform.Y;
                    double factor = remainingDelta / originalDelta;

                    // If factor > 0, the edge has been detected, but it hasn't gone too far yet, so readjust the animation. 
                    // If factor < 0, somehow scrolling has gone past the edge already, so snap back.
                    Duration duration = factor <= 1 && factor >= 0 && _panelAnimation.Duration.HasTimeSpan ? new Duration(TimeSpan.FromMilliseconds(_panelAnimation.Duration.TimeSpan.Milliseconds * factor)) : _panDuration;

                    AnimatePanel(duration, _panelAnimation.EasingFunction, newTo);
                }
            }
        }

        void AnimationPerFrameCallback(object sender, EventArgs e)
        {
            Balance();
        }

        void PanelStoryboardCompleted(object sender, EventArgs e)
        {
            CompositionTarget.Rendering -= AnimationPerFrameCallback;

            if (_scrollingTowards != -1)
            {
                // We are scrolling in a direction towards an item we did not know the position of.
                // We aren't there yet, so keep going in the same direction.
                ScrollTowards();
                return;
            }
            _isAnimating = false;
            IsFlicking = false;
        }

        private void StopAnimation()
        {
            _panelStoryboard.Stop();
            CompositionTarget.Rendering -= AnimationPerFrameCallback;
            _isAnimating = false;
        }

        private bool BounceBack(bool immediately)
        {
            if (_panningTransform == null || _resolvedCount == 0)
            {
                return false;
            }

            double to = _panningTransform.Y;
            double top = Canvas.GetTop(FirstResolved.ContentPresenter);

            double newTo = top > -to ? -top : GetCoercedScrollPosition(to, false);

            if (to != newTo)
            {
                if (immediately)
                {
                    _panningTransform.Y = newTo;
                }
                else
                {
                    AnimatePanel(_panDuration, _panEase, newTo);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Instantly jump to the selected group.
        /// </summary>
        /// <param name="group">The group to jump to</param>
        public void ScrollToGroup(object group)
        {
            double scrollTarget = 0;
            bool foundTarget = false;

            if (group == null)
            {
                return;
            }

            // First check to see if group is already visible
            for (int index = _resolvedFirstIndex; index < _resolvedFirstIndex + _resolvedCount; ++index)
            {
                ContentPresenter cp = _flattenedItems[index].ContentPresenter;
                if (cp.Content != null && cp.Content.Equals(group) && _flattenedItems[index].ItemType == ItemType.GroupHeader)
                {
                    scrollTarget = -Canvas.GetTop(cp);
                    foundTarget = true;
                    break;
                }
                ++index;
            }

            // Just jump to it, and replace the entire list.
            if (!foundTarget)
            {
                int newIndex = GetGroupOffset(group);
                if (newIndex == -1)
                {
                    return;
                }

                if (newIndex != -1)
                {
                    RecycleAllItems();
                    CollapseRecycledElements();
                    _resolvedFirstIndex = newIndex;
                    scrollTarget = 0;
                    ResetMinMax();
                }
            }

            scrollTarget = GetCoercedScrollPosition(scrollTarget, false);
            StopAnimation();
            _panningTransform.Y = scrollTarget;
            Balance();
            
            // Special case first group to include header
            if (group.Equals(_firstGroup))
            {
                _panningTransform.Y = _maximumPanelScroll;
            }

            BounceBack(true);
        }

        private double BounceDistance { get { return ActualHeight / 4; } }

        private double GetCoercedScrollPosition(double value, bool isBouncy)
        {
            double bounceFactor = isBouncy ? BounceDistance : 0;
            return Math.Max(_minimumPanelScroll - bounceFactor, Math.Min(_maximumPanelScroll + bounceFactor, value));
        }

        /// <summary>
        /// Scroll in the direction of an item that has not been resolved.
        /// </summary>
        private void ScrollTowards()
        {
            if (_scrollingTowards == -1)
            {
                return;
            }
            double jump = _scrollingTowards < _resolvedFirstIndex ? 10000000 : -10000000;
            double newPosition = GetCoercedScrollPosition(_panningTransform.Y + jump, false);

            double delta = newPosition - _panningTransform.Y;
            double seconds = Math.Abs(delta) / MaximumFlickVelocity;
            IsFlicking = true;
            AnimatePanel(new Duration(TimeSpan.FromSeconds(seconds)), null, newPosition);
        }

        private int GetFlattenedIndex(object item)
        {
            int count = _flattenedItems.Count;
            for (int index = 0; index < count; ++index)
            {
                if (item == _flattenedItems[index].Item)
                {
                    return index;
                }
            }
            return -1;
        }

        private int GetResolvedIndex(object item, out ContentPresenter contentPresenter)
        {
            if (_resolvedCount > 0)
            {
                for (int index = _resolvedFirstIndex; index < _resolvedFirstIndex + _resolvedCount; ++index)
                {
                    if (_flattenedItems[index].Item == item)
                    {
                        contentPresenter = _flattenedItems[index].ContentPresenter;
                        return index;
                    }
                }
            }

            contentPresenter = null;
            return -1;
        }

        private enum ItemType
        {
            Unknown,
            Item,
            GroupHeader,
            GroupFooter,
            ListHeader,
            ListFooter
        }

        private class ItemTuple
        {
            public ItemType ItemType;
            public object Group;
            public object Item;
            public ContentPresenter ContentPresenter;
        }

        
    //    private void ComputeResolvedFirstAndCount(out int first, out int count)
    //    {
    //        first = -1; count = 0;

    //        bool done = false;

    //        for (int index = 0; index < _flattenedItems.Count; ++index)
    //        {
    //            if (_flattenedItems[index].ContentPresenter != null)
    //            {
    //                if (done)
    //                {
    //                    throw new InvalidOperationException();
    //                }

    //                if (first == -1)
    //                {
    //                    first = index;
    //                }

    //                ++count;
    //            }
    //            else if (first != -1)
    //            {
    //                done = true;
    //            }
    //        }

    //        if (first == -1)
    //        {
    //            first = 0;
    //        }

    //        Debug.WriteLine("Resolved first {0} count {1}", first, count);
    //    }
    }
}