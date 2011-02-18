// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Shell;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Class that implements a flexible list-picking experience with a custom interface for few/many items.
    /// </summary>
    [TemplatePart(Name = ItemsPresenterPartName, Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = ItemsPresenterTranslateTransformPartName, Type = typeof(TranslateTransform))]
    [TemplatePart(Name = ItemsPresenterHostPartName, Type = typeof(Canvas))]
    [TemplatePart(Name = FullModePopupPartName, Type = typeof(Popup))]
    [TemplatePart(Name = FullModeSelectorPartName, Type = typeof(Selector))]
    [TemplateVisualState(GroupName = PickerStatesGroupName, Name = PickerStatesNormalStateName)]
    [TemplateVisualState(GroupName = PickerStatesGroupName, Name = PickerStatesExpandedStateName)]
    public class ListPicker : ItemsControl
    {
        private const string ItemsPresenterPartName = "ItemsPresenter";
        private const string ItemsPresenterTranslateTransformPartName = "ItemsPresenterTranslateTransform";
        private const string ItemsPresenterHostPartName = "ItemsPresenterHost";
        private const string FullModePopupPartName = "FullModePopup";
        private const string FullModeSelectorPartName = "FullModeSelector";

        private const string PickerStatesGroupName = "PickerStates";
        private const string PickerStatesNormalStateName = "Normal";
        private const string PickerStatesExpandedStateName = "Expanded";

        private readonly DoubleAnimation _heightAnimation = new DoubleAnimation();
        private readonly DoubleAnimation _translateAnimation = new DoubleAnimation();
        private readonly Storyboard _storyboard = new Storyboard();

        private PhoneApplicationFrame _frame;
        private PhoneApplicationPage _page;
        private FrameworkElement _itemsPresenterHostParent;
        private Canvas _itemsPresenterHostPart;
        private ItemsPresenter _itemsPresenterPart;
        private Popup _fullModePopupPart;
        private Selector _fullModeSelectorPart;
        private TranslateTransform _itemsPresenterTranslateTransformPart;
        private bool _updatingSelection;
        private bool _savedSystemTrayIsVisible;
        private bool _savedApplicationBarIsVisible;
        private int _deferredSelectedIndex = -1;

        /// <summary>
        /// Event that is raised when the selection changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Gets or sets the ListPickerMode (ex: Normal/Expanded/Full).
        /// </summary>
        public ListPickerMode ListPickerMode
        {
            get { return (ListPickerMode)GetValue(ListPickerModeProperty); }
            set { SetValue(ListPickerModeProperty, value); }
        }

        /// <summary>
        /// Identifies the ListPickerMode DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ListPickerModeProperty =
            DependencyProperty.Register("ListPickerMode", typeof(ListPickerMode), typeof(ListPicker), new PropertyMetadata(ListPickerMode.Normal, OnListPickerModeChanged));

        private static void OnListPickerModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ListPicker)o).OnListPickerModeChanged((ListPickerMode)e.OldValue, (ListPickerMode)e.NewValue);
        }

        private void OnListPickerModeChanged(ListPickerMode oldValue, ListPickerMode newValue)
        {
            // Hook up to frame if not already done
            if (null == _frame)
            {
                _frame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (null != _frame)
                {
                    _frame.AddHandler(ManipulationCompletedEvent, new EventHandler<ManipulationCompletedEventArgs>(HandleFrameManipulationCompleted), true);
                }
            }

            // Restore state
            if ((ListPickerMode.Full == oldValue) && !DesignerProperties.IsInDesignTool)
            {
                if (null != _fullModePopupPart)
                {
                    _fullModePopupPart.IsOpen = false;
                }
                if (null != _fullModeSelectorPart)
                {
                    _fullModeSelectorPart.SelectionChanged -= HandleFullModeSelectorPartSelectionChanged;
                    _fullModeSelectorPart.Loaded -= HandleFullModeSelectorPartLoaded;
                    _fullModeSelectorPart.ItemsSource = null;
                }

                Action restoreSystemTray = () =>
                    {
                        try
                        {
                            SystemTray.IsVisible = _savedSystemTrayIsVisible;
                        }
                        catch (InvalidOperationException)
                        {
                        }
                    };
                try
                {
                    restoreSystemTray();
                }
                catch (InvalidOperationException)
                {
                    Dispatcher.BeginInvoke(restoreSystemTray);
                }
                if (null != _page)
                {
                    if (null != _page.ApplicationBar)
                    {
                        _page.ApplicationBar.IsVisible = _savedApplicationBarIsVisible;
                    }
                }
                if (null != _frame)
                {
                    _frame.OrientationChanged -= HandleFrameOrientationChanged;
                }
            }
            if ((ListPickerMode.Expanded == oldValue) || (ListPickerMode.Full == oldValue))
            {
                if (null != _page)
                {
                    _page.BackKeyPress -= HandlePageBackKeyPress;
                    _page = null;
                }
            }

            // Hook up to relevant events
            if ((ListPickerMode.Expanded == newValue) || (ListPickerMode.Full == newValue))
            {
                if (null != _frame)
                {
                    _page = _frame.Content as PhoneApplicationPage;
                    if (null != _page)
                    {
                        _page.BackKeyPress += HandlePageBackKeyPress;
                    }
                }
            }
            if ((ListPickerMode.Full == newValue) && !DesignerProperties.IsInDesignTool)
            {
                Action saveTrayAndHide = () =>
                    {
                        try
                        {
                            _savedSystemTrayIsVisible = SystemTray.IsVisible;
                            SystemTray.IsVisible = false;
                        }
                        catch (InvalidOperationException)
                        {
                        }
                    };
                try
                {
                    saveTrayAndHide();
                }
                catch (InvalidOperationException)
                {
                    Dispatcher.BeginInvoke(saveTrayAndHide);
                }
                if (null != _frame)
                {
                    AdjustPopupChildForCurrentOrientation(_frame);
                    _frame.OrientationChanged += HandleFrameOrientationChanged;
                    if (null != _page)
                    {
                        if (null != _page.ApplicationBar)
                        {
                            _savedApplicationBarIsVisible = _page.ApplicationBar.IsVisible;
                            _page.ApplicationBar.IsVisible = false;
                        }
                    }
                }
                if (null != _fullModeSelectorPart)
                {
                    _fullModeSelectorPart.ItemsSource = Items;
                    _fullModeSelectorPart.SelectionChanged += HandleFullModeSelectorPartSelectionChanged;
                    _fullModeSelectorPart.Loaded += HandleFullModeSelectorPartLoaded;
                }
                if (null != _fullModePopupPart)
                {
                    _fullModePopupPart.IsOpen = true;
                }
            }

            // Resize for new view and go to relevant visual state(s)
            SizeForAppropriateView(ListPickerMode.Full != oldValue);
            GoToStates(true);
        }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedIndex DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(ListPicker), new PropertyMetadata(-1, OnSelectedIndexChanged));

        private static void OnSelectedIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ListPicker)o).OnSelectedIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SelectedIndex", Justification = "Property name.")]
        private void OnSelectedIndexChanged(int oldValue, int newValue)
        {
            // Validate new value
            if ((Items.Count <= newValue) ||
                ((0 < Items.Count) && (newValue < 0)) ||
                ((0 == Items.Count) && (newValue != -1)))
            {
                if ((null == Template) && (0 <= newValue))
                {
                    // Can't set the value now; remember it for later
                    _deferredSelectedIndex = newValue;
                    return;
                }
                throw new InvalidOperationException(Properties.Resources.InvalidSelectedIndex);
            }

            // Synchronize SelectedItem property
            if (!_updatingSelection)
            {
                _updatingSelection = true;
                SelectedItem = (-1 != newValue) ? Items[newValue] : null;
                _updatingSelection = false;
            }

            if (-1 != oldValue)
            {
                // Toggle container selection
                ListPickerItem oldContainer = (ListPickerItem)ItemContainerGenerator.ContainerFromIndex(oldValue);
                if (null != oldContainer)
                {
                    oldContainer.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ListPicker), new PropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ListPicker)o).OnSelectedItemChanged(e.OldValue, e.NewValue);
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SelectedItem", Justification = "Property name.")]
        private void OnSelectedItemChanged(object oldValue, object newValue)
        {
            // Validate new value
            int newValueIndex = newValueIndex = (null != newValue) ? Items.IndexOf(newValue) : -1;
            if ((-1 == newValueIndex) && (0 < Items.Count))
            {
                throw new InvalidOperationException(Properties.Resources.InvalidSelectedItem);
            }

            // Synchronize SelectedIndex property
            if (!_updatingSelection)
            {
                _updatingSelection = true;
                SelectedIndex = newValueIndex;
                _updatingSelection = false;
            }

            // Switch to Normal mode or size for current item
            if (ListPickerMode.Normal != ListPickerMode)
            {
                ListPickerMode = ListPickerMode.Normal;
            }
            else
            {
                SizeForAppropriateView(false);
            }

            // Fire SelectionChanged event
            SelectionChangedEventHandler handler = SelectionChanged;
            if (null != handler)
            {
                IList removedItems = (null == oldValue) ? new object[0] : new object[] { oldValue };
                IList addedItems = (null == newValue) ? new object[0] : new object[] { newValue };
                handler(this, new SelectionChangedEventArgs(removedItems, addedItems));
            }
        }

        private static readonly DependencyProperty ShadowItemTemplateProperty =
            DependencyProperty.Register("ShadowItemTemplate", typeof(DataTemplate), typeof(ListPicker), new PropertyMetadata(null, OnShadowOrFullModeItemTemplateChanged));

        /// <summary>
        /// Gets or sets the DataTemplate used to display each item when ListPickerMode is set to Full.
        /// </summary>
        public DataTemplate FullModeItemTemplate
        {
            get { return (DataTemplate)GetValue(FullModeItemTemplateProperty); }
            set { SetValue(FullModeItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the FullModeItemTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty FullModeItemTemplateProperty =
            DependencyProperty.Register("FullModeItemTemplate", typeof(DataTemplate), typeof(ListPicker), new PropertyMetadata(null, OnShadowOrFullModeItemTemplateChanged));

        private static void OnShadowOrFullModeItemTemplateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ListPicker)o).OnShadowOrFullModeItemTemplateChanged(/*(DataTemplate)e.OldValue, (DataTemplate)e.NewValue*/);
        }

        private void OnShadowOrFullModeItemTemplateChanged(/*DataTemplate oldValue, DataTemplate newValue*/)
        {
            // Set ActualFullModeItemTemplate accordingly
            SetValue(ActualFullModeItemTemplateProperty, FullModeItemTemplate ?? ItemTemplate);
        }

        private static readonly DependencyProperty ActualFullModeItemTemplateProperty =
            DependencyProperty.Register("ActualFullModeItemTemplate", typeof(DataTemplate), typeof(ListPicker), null);

        /// <summary>
        /// Gets or sets the header of the control.
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the Header DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ListPicker), null);

        /// <summary>
        /// Gets or sets the template used to display the control's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the HeaderTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ListPicker), null);

        /// <summary>
        /// Gets or sets the header to use when ListPickerMode is set to Full.
        /// </summary>
        public object FullModeHeader
        {
            get { return (object)GetValue(FullModeHeaderProperty); }
            set { SetValue(FullModeHeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the FullModeHeader DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty FullModeHeaderProperty =
            DependencyProperty.Register("FullModeHeader", typeof(object), typeof(ListPicker), null);

        /// <summary>
        /// Gets or sets the maximum number of items for which Expanded mode will be used (default: 5).
        /// </summary>
        public int ItemCountThreshold
        {
            get { return (int)GetValue(ItemCountThresholdProperty); }
            set { SetValue(ItemCountThresholdProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemCountThreshold DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ItemCountThresholdProperty =
            DependencyProperty.Register("ItemCountThreshold", typeof(int), typeof(ListPicker), new PropertyMetadata(5, OnItemCountThresholdChanged));

        private static void OnItemCountThresholdChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ListPicker)o).OnItemCountThresholdChanged(/*(int)e.OldValue,*/ (int)e.NewValue);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Following DependencyProperty property changed handler convention.")]
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Providing the DependencyProperty name is preferred here.")]
        private void OnItemCountThresholdChanged(/*int oldValue,*/ int newValue)
        {
            if (newValue < 0)
            {
                throw new ArgumentOutOfRangeException("ItemCountThreshold");
            }
        }

        /// <summary>
        /// Initializes a new instance of the ListPicker class.
        /// </summary>
        public ListPicker()
        {
            DefaultStyleKey = typeof(ListPicker);

            Storyboard.SetTargetProperty(_heightAnimation, new PropertyPath(FrameworkElement.HeightProperty));
            Storyboard.SetTargetProperty(_translateAnimation, new PropertyPath(TranslateTransform.YProperty));

            // Would be nice if these values were customizable (ex: as DependencyProperties or in Template as VSM states)
            Duration duration = TimeSpan.FromSeconds(0.2);
            _heightAnimation.Duration = duration;
            _translateAnimation.Duration = duration;
            IEasingFunction easingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut, Exponent = 4 };
            _heightAnimation.EasingFunction = easingFunction;
            _translateAnimation.EasingFunction = easingFunction;

            Unloaded += delegate
            {
                // Unhook any remaining event handlers
                if (null != _frame)
                {
                    _frame.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(HandleFrameManipulationCompleted);
                    _frame = null;
                }
            };
        }

        /// <summary>
        /// Builds the visual tree for the control when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Unhook from old elements
            if (null != _itemsPresenterHostParent)
            {
                _itemsPresenterHostParent.SizeChanged -= HandleItemsPresenterHostParentSizeChanged;
            }
            _storyboard.Stop();

            base.OnApplyTemplate();

            // Hook up to new elements
            _itemsPresenterPart = GetTemplateChild(ItemsPresenterPartName) as ItemsPresenter;
            _itemsPresenterTranslateTransformPart = GetTemplateChild(ItemsPresenterTranslateTransformPartName) as TranslateTransform;
            _itemsPresenterHostPart = GetTemplateChild(ItemsPresenterHostPartName) as Canvas;
            _fullModePopupPart = GetTemplateChild(FullModePopupPartName) as Popup;
            _fullModeSelectorPart = GetTemplateChild(FullModeSelectorPartName) as Selector;
            _itemsPresenterHostParent = (null != _itemsPresenterHostPart) ? _itemsPresenterHostPart.Parent as FrameworkElement : null;
            if (null != _itemsPresenterHostParent)
            {
                _itemsPresenterHostParent.SizeChanged += HandleItemsPresenterHostParentSizeChanged;
            }
            if (null != _itemsPresenterHostPart)
            {
                Storyboard.SetTarget(_heightAnimation, _itemsPresenterHostPart);
                if (!_storyboard.Children.Contains(_heightAnimation))
                {
                    _storyboard.Children.Add(_heightAnimation);
                }
            }
            else
            {
                if (_storyboard.Children.Contains(_heightAnimation))
                {
                    _storyboard.Children.Remove(_heightAnimation);
                }
            }
            if (null != _itemsPresenterTranslateTransformPart)
            {
                Storyboard.SetTarget(_translateAnimation, _itemsPresenterTranslateTransformPart);
                if (!_storyboard.Children.Contains(_translateAnimation))
                {
                    _storyboard.Children.Add(_translateAnimation);
                }
            }
            else
            {
                if (_storyboard.Children.Contains(_translateAnimation))
                {
                    _storyboard.Children.Remove(_translateAnimation);
                }
            }
            if (null != _fullModePopupPart)
            {
                UIElement child = _fullModePopupPart.Child;
                _fullModePopupPart.Child = null;
                _fullModePopupPart = new Popup();
                _fullModePopupPart.Child = child;
            }
            SetBinding(ShadowItemTemplateProperty, new Binding("ItemTemplate") { Source = this });

            // Commit deferred SelectedIndex (if any)
            if (-1 != _deferredSelectedIndex)
            {
                SelectedIndex = _deferredSelectedIndex;
                _deferredSelectedIndex = -1;
            }

            // Go to current state(s)
            GoToStates(false);
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own item container.
        /// </summary>
        /// <param name="item">The specified item.</param>
        /// <returns>True if the item is its own item container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ListPickerItem;
        }

        /// <summary>
        /// Creates or identifies the element used to display a specified item.
        /// </summary>
        /// <returns>A container corresponding to a specified item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListPickerItem();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            // Hook up to interesting events
            ContentControl container = (ContentControl)element;
            container.ManipulationCompleted += HandleContainerManipulationCompleted;
            container.SizeChanged += HandleListPickerItemSizeChanged;

            // Size for selected item if it's this one
            if (object.Equals(item, SelectedItem))
            {
                SizeForAppropriateView(false);
            }
        }

        /// <summary>
        /// Undoes the effects of the PrepareContainerForItemOverride method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            // Unhook from events
            ContentControl container = (ContentControl)element;
            container.ManipulationCompleted -= HandleContainerManipulationCompleted;
            container.SizeChanged -= HandleListPickerItemSizeChanged;
        }

        /// <summary>
        /// Provides handling for the ItemContainerGenerator.ItemsChanged event.
        /// </summary>
        /// <param name="e">A NotifyCollectionChangedEventArgs that contains the event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if ((0 < Items.Count) && (null == SelectedItem))
            {
                // Nothing selected (and no pending Binding); select the first item
                if ((null == GetBindingExpression(SelectedIndexProperty)) &&
                    (null == GetBindingExpression(SelectedItemProperty)))
                {
                    SelectedIndex = 0;
                }
            }
            else if (0 == Items.Count)
            {
                // No items; select nothing
                SelectedIndex = -1;
                ListPickerMode = ListPickerMode.Normal;
            }
            else if (Items.Count <= SelectedIndex)
            {
                // Selected item no longer present; select the last item
                SelectedIndex = Items.Count - 1;
            }
            else
            {
                // Re-synchronize SelectedIndex with SelectedItem if necessary
                if (!object.Equals(Items[SelectedIndex], SelectedItem))
                {
                    int selectedItemIndex = Items.IndexOf(SelectedItem);
                    if (-1 == selectedItemIndex)
                    {
                        SelectedItem = Items[0];
                    }
                    else
                    {
                        SelectedIndex = selectedItemIndex;
                    }
                }
            }

            // Translate it into view once layout has been updated for the added/removed item(s)
            Dispatcher.BeginInvoke(() => SizeForAppropriateView(false));
        }

        /// <summary>
        /// Called when the ManipulationCompleted event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            if (null == e)
            {
                throw new ArgumentNullException("e");
            }

            base.OnManipulationCompleted(e);

            // Interaction needs to be on the _itemsPresenterHostPart or its children
            DependencyObject element = e.OriginalSource as DependencyObject;
            while (null != element)
            {
                if (_itemsPresenterHostPart == element)
                {
                    // On interaction, switch to Expanded/Full mode
                    if ((ListPickerMode.Normal == ListPickerMode) && (0 < Items.Count))
                    {
                        ListPickerMode = (Items.Count <= ItemCountThreshold) ? ListPickerMode.Expanded : ListPickerMode.Full;
                        e.Handled = true;
                    }
                    break;
                }
                element = VisualTreeHelper.GetParent(element);
            }
        }

        private void HandleItemsPresenterHostParentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Pass width through the Canvas
            if (null != _itemsPresenterPart)
            {
                _itemsPresenterPart.Width = e.NewSize.Width;
            }

            // Update clip to show only the selected item in Normal mode
            _itemsPresenterHostParent.Clip = new RectangleGeometry { Rect = new Rect(new Point(), e.NewSize) };
        }

        private void HandleListPickerItemSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update size accordingly
            ContentControl container = (ContentControl)sender;
            if (object.Equals(ItemContainerGenerator.ItemFromContainer(container), SelectedItem))
            {
                SizeForAppropriateView(false);
            }
        }

        private void HandleFullModeSelectorPartSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != _fullModeSelectorPart)
            {
                // Commit selected item
                if (SelectedItem != _fullModeSelectorPart.SelectedItem)
                {
                    SelectedItem = _fullModeSelectorPart.SelectedItem;
                }
                else
                {
                    // User selected the already-selected item; just switch back to Normal view
                    ListPickerMode = ListPickerMode.Normal;
                }
            }
        }

        private void HandleFullModeSelectorPartLoaded(object sender, RoutedEventArgs e)
        {
            if (null != _fullModeSelectorPart)
            {
                // Find the relevant container and make it look selected
                // Note: Selector.SelectedItem is left null so *any* selection will trigger the SelectionChanged event.
                // However, this doesn't highlight the "currently selected" item; the following technique fakes that.
                ContentControl container = _fullModeSelectorPart.ItemContainerGenerator.ContainerFromItem(SelectedItem) as ContentControl;
                if (null == container)
                {
                    // Container isn't always available; defer until it is
                    // Note: Assumes the container eventually WILL be available (which is why
                    // the default Template replaces VirtualizingStackPanel with StackPanel)
                    Dispatcher.BeginInvoke(() => HandleFullModeSelectorPartLoaded(sender, e));
                }
                else
                {
                    Brush phoneAccentBrush = Application.Current.Resources["PhoneAccentBrush"] as Brush;
                    if (null != phoneAccentBrush)
                    {
                        container.Foreground = phoneAccentBrush;
                    }
                }
                // Scroll item into view if possible
                ListBox listBox = _fullModeSelectorPart as ListBox;
                if (null != listBox)
                {
                    listBox.ScrollIntoView(SelectedItem);
                }
            }
        }

        private void HandlePageBackKeyPress(object sender, CancelEventArgs e)
        {
            // Revert to Normal mode
            ListPickerMode = ListPickerMode.Normal;
            e.Cancel = true;
        }

        private void HandleFrameOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            AdjustPopupChildForCurrentOrientation((PhoneApplicationFrame)sender);
        }

        private void AdjustPopupChildForCurrentOrientation(PhoneApplicationFrame frame)
        {
            if (null != _fullModePopupPart)
            {
                FrameworkElement child = _fullModePopupPart.Child as FrameworkElement;
                if (null != child)
                {
                    // Transform child according to current orientation
                    double actualWidth = frame.ActualWidth;
                    double actualHeight = frame.ActualHeight;
                    bool portrait = PageOrientation.Portrait == (PageOrientation.Portrait & frame.Orientation);
                    TransformGroup transformGroup = new TransformGroup();
                    switch (frame.Orientation)
                    {
                        case PageOrientation.LandscapeLeft:
                            transformGroup.Children.Add(new RotateTransform { Angle = 90 });
                            transformGroup.Children.Add(new TranslateTransform { X = actualWidth });
                            break;
                        case PageOrientation.LandscapeRight:
                            transformGroup.Children.Add(new RotateTransform { Angle = -90 });
                            transformGroup.Children.Add(new TranslateTransform { Y = actualHeight });
                            break;
                    }
                    child.RenderTransform = transformGroup;

                    // Size child to frame
                    child.Width = portrait ? actualWidth : actualHeight;
                    child.Height = portrait ? actualHeight : actualWidth;

                    // Adjust padding if possible
                    Border border = child as Border;
                    if (null != border)
                    {
                        switch (frame.Orientation)
                        {
                            case PageOrientation.PortraitUp:
                                border.Padding = new Thickness(0, 32, 0, 0);
                                break;
                            case PageOrientation.LandscapeLeft:
                                border.Padding = new Thickness(72, 0, 0, 0);
                                break;
                            case PageOrientation.LandscapeRight:
                                border.Padding = new Thickness(0, 0, 72, 0);
                                break;
                        }
                    }
                }
            }
        }

        private void SizeForAppropriateView(bool animate)
        {
            switch (ListPickerMode)
            {
                case ListPickerMode.Normal:
                    SizeForNormalMode(animate);
                    break;
                case ListPickerMode.Expanded:
                    SizeForExpandedMode();
                    break;
                case ListPickerMode.Full:
                    // Nothing to do
                    break;
            }

            // Play the height/translation animations
            _storyboard.Begin();
            if (!animate)
            {
                _storyboard.SkipToFill();
            }
        }

        private void SizeForNormalMode(bool animate)
        {
            ContentControl container = (ContentControl)ItemContainerGenerator.ContainerFromItem(SelectedItem);
            if (null != container)
            {
                // Set height/translation to show just the selected item
                if (0 < container.ActualHeight)
                {
                    SetContentHeight(container.ActualHeight + container.Margin.Top + container.Margin.Bottom);
                }
                if (null != _itemsPresenterTranslateTransformPart)
                {
                    if (!animate)
                    {
                        _itemsPresenterTranslateTransformPart.Y = 0;
                    }
                    _translateAnimation.To = container.Margin.Top - LayoutInformation.GetLayoutSlot(container).Top;
                    _translateAnimation.From = animate ? null : _translateAnimation.To;
                }
            }
            else
            {
                // Resize to minimum height
                SetContentHeight(0);
            }

            // Clear highlight of previously selected container
            ListPickerItem oldContainer = (ListPickerItem)ItemContainerGenerator.ContainerFromIndex(SelectedIndex);
            if (null != oldContainer)
            {
                oldContainer.IsSelected = false;
            }
        }

        private void SizeForExpandedMode()
        {
            // Set height and align first element at top
            if (null != _itemsPresenterPart)
            {
                SetContentHeight(_itemsPresenterPart.ActualHeight);
            }
            if (null != _itemsPresenterTranslateTransformPart)
            {
                _translateAnimation.To = 0;
            }

            // Highlight selected container
            ListPickerItem container = (ListPickerItem)ItemContainerGenerator.ContainerFromIndex(SelectedIndex);
            if (null != container)
            {
                container.IsSelected = true;
            }
        }

        private void SetContentHeight(double height)
        {
            if ((null != _itemsPresenterHostPart) && !double.IsNaN(height))
            {
                double canvasHeight = _itemsPresenterHostPart.Height;
                _heightAnimation.From = double.IsNaN(canvasHeight) ? height : canvasHeight;
                _heightAnimation.To = height;
            }
        }

        private void HandleFrameManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (ListPickerMode.Expanded == ListPickerMode)
            {
                // Manipulation outside an Expanded ListPicker reverts to Normal mode
                DependencyObject element = e.OriginalSource as DependencyObject;
                DependencyObject cancelElement = (DependencyObject)_itemsPresenterHostPart ?? (DependencyObject)this;
                while (null != element)
                {
                    if (cancelElement == element)
                    {
                        return;
                    }
                    element = VisualTreeHelper.GetParent(element);
                }
                ListPickerMode = ListPickerMode.Normal;
            }
        }

        private void HandleContainerManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (ListPickerMode.Expanded == ListPickerMode)
            {
                // Manipulation of a container selects the item and reverts to Normal mode
                ContentControl container = (ContentControl)sender;
                SelectedItem = ItemContainerGenerator.ItemFromContainer(container);
                ListPickerMode = ListPickerMode.Normal;
                e.Handled = true;
            }
        }

        private void GoToStates(bool useTransitions)
        {
            switch (ListPickerMode)
            {
                case ListPickerMode.Normal:
                    VisualStateManager.GoToState(this, PickerStatesNormalStateName, useTransitions);
                    break;
                case ListPickerMode.Expanded:
                    VisualStateManager.GoToState(this, PickerStatesExpandedStateName, useTransitions);
                    break;
                case ListPickerMode.Full:
                    // Nothing to do
                    break;
            }
        }
    }
}
