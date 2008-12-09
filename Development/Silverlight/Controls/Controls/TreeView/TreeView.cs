// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Windows.Controls.Automation.Peers;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Represents a control that displays hierarchical data in a tree structure
    /// that has items that can expand and collapse.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(TreeViewItem))]
    public partial class TreeView : ItemsControl
    {
        /// <summary>
        /// A value indicating whether a read-only dependency property change
        /// handler should allow the value to be set.  This is used to ensure
        /// that read-only properties cannot be changed via SetValue, etc.
        /// </summary>
        private bool _allowWrite;

        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignorePropertyChange;

        #region public object SelectedItem
        /// <summary>
        /// Gets the selected item in a TreeView.
        /// </summary>
        /// <remarks>
        /// The default value is null.
        /// </remarks>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            private set
            {
                try
                {
                    _allowWrite = true;
                    SetValue(SelectedItemProperty, value);
                }
                finally
                {
                    _allowWrite = false;
                }
            }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(TreeView),
                new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        /// <summary>
        /// SelectedItemProperty property changed handler.
        /// </summary>
        /// <param name="d">TreeView that changed its SelectedItem.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeView source = d as TreeView;

            // Ignore the change if requested
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }

            // Ensure the property is only written when expected
            if (!source._allowWrite)
            {
                // Reset the old value before it was incorrectly written
                source._ignorePropertyChange = true;
                source.SetValue(SelectedItemProperty, e.OldValue);

                throw new InvalidOperationException(
                    Properties.Resources.TreeView_OnSelectedItemPropertyChanged_InvalidWrite);
            }

            source.UpdateSelectedValue(e.NewValue);
        }
        #endregion public object SelectedItem

        #region public object SelectedValue
        /// <summary>
        /// Gets the object that is at the specified SelectedValuePath of the
        /// SelectedItem.
        /// </summary>
        /// <remarks>
        /// The default value is null.
        /// </remarks>
        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            private set
            {
                try
                {
                    _allowWrite = true;
                    SetValue(SelectedValueProperty, value);
                }
                finally
                {
                    _allowWrite = false;
                }
            }
        }

        /// <summary>
        /// Identifies the SelectedValue dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(
                "SelectedValue",
                typeof(object),
                typeof(TreeView),
                new PropertyMetadata(null, OnSelectedValuePropertyChanged));

        /// <summary>
        /// SelectedValueProperty property changed handler.
        /// </summary>
        /// <param name="d">TreeView that changed its SelectedValue.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeView source = d as TreeView;

            // Ignore the change if requested
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }

            // Ensure the property is only written when expected
            if (!source._allowWrite)
            {
                // Reset the old value before it was incorrectly written
                source._ignorePropertyChange = true;
                source.SetValue(SelectedValueProperty, e.OldValue);

                throw new InvalidOperationException(
                    Properties.Resources.TreeView_OnSelectedValuePropertyChanged_InvalidWrite);
            }
        }
        #endregion public object SelectedValue

        #region public string SelectedValuePath
        /// <summary>
        /// Gets or sets the path that is used to get the SelectedValue of the
        /// SelectedItem in a TreeView.
        /// </summary>
        /// <remarks>
        /// The default value is String.Empty.
        /// </remarks>
        public string SelectedValuePath
        {
            get { return GetValue(SelectedValuePathProperty) as string; }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedValuePath dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(
                "SelectedValuePath",
                typeof(string),
                typeof(TreeView),
                new PropertyMetadata(string.Empty, OnSelectedValuePathPropertyChanged));

        /// <summary>
        /// SelectedValuePathProperty property changed handler.
        /// </summary>
        /// <param name="d">TreeView that changed its SelectedValuePath.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedValuePathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeView source = d as TreeView;
            source.UpdateSelectedValue(source.SelectedItem);
        }
        #endregion public string SelectedValuePath

        #region public Style ItemContainerStyle
        /// <summary>
        /// Gets or sets the Style that is applied to the container element
        /// generated for each item.
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return GetValue(ItemContainerStyleProperty) as Style; }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemContainerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(
                "ItemContainerStyle",
                typeof(Style),
                typeof(TreeView),
                new PropertyMetadata(null, OnItemContainerStylePropertyChanged));

        /// <summary>
        /// ItemContainerStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// TreeView that changed its ItemContainerStyle.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeView source = d as TreeView;
            Style value = e.NewValue as Style;
            source.ItemContainerGenerator.UpdateItemContainerStyle(value);
        }
        #endregion public Style ItemContainerStyle

        /// <summary>
        /// Gets the currently selected TreeViewItem container.
        /// </summary>
        internal TreeViewItem SelectedContainer { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the currently selected TreeViewItem
        /// container is properly hooked up to the TreeView.
        /// </summary>
        internal bool IsSelectedContainerHookedUp
        {
            get { return SelectedContainer != null && SelectedContainer.ParentTreeView == this; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected item is
        /// currently being changed.
        /// </summary>
        internal bool IsSelectionChangeActive { get; set; }

        /// <summary>
        /// Gets the ItemContainerGenerator that is associated with this
        /// control.
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Control key is currently
        /// pressed.
        /// </summary>
        internal static bool IsControlKeyDown
        {
            get { return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control; }
        }

        /// <summary>
        /// Gets a value indicating whether the Shift key is currently pressed.
        /// </summary>
        internal static bool IsShiftKeyDown
        {
            get { return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift; }
        }

        /// <summary>
        /// Occurs when the SelectedItem changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged;

        /// <summary>
        /// Initializes a new instance of the TreeView class.
        /// </summary>
        public TreeView()
        {
            DefaultStyleKey = typeof(TreeView);
            ItemContainerGenerator = new ItemContainerGenerator(this);
        }

        /// <summary>
        /// Returns a TreeViewAutomationPeer for use by the Silverlight
        /// automation infrastructure.
        /// </summary>
        /// <returns>
        /// A TreeViewAutomationPeer for the TreeView control.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new TreeViewAutomationPeer(this);
        }

        /// <summary>
        /// Builds the visual tree for the TreeView control when a new control
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            ItemContainerGenerator.OnApplyTemplate();
            base.OnApplyTemplate();
        }

        #region ItemsControl
        /// <summary>
        /// Creates a TreeViewItem to use to display content.
        /// </summary>
        /// <returns>
        /// A new TreeViewItem to use as a container for content.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItem();
        }

        /// <summary>
        /// Determines whether the specified item is its own container or can be
        /// its own container.
        /// </summary>
        /// <param name="item">The object to evaluate.</param>
        /// <returns>
        /// A value indicating whether the item is a TreeViewItem or not.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeViewItem;
        }

        /// <summary>
        /// Prepares the specified container to display the specified item.
        /// </summary>
        /// <param name="element">
        /// Container element used to display the specified item.
        /// </param>
        /// <param name="item">Specified item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            TreeViewItem node = element as TreeViewItem;
            if (node != null)
            {
                // Associate the Parent ItemsControl
                node.ParentItemsControl = this;
            }

            ItemContainerGenerator.PrepareContainerForItemOverride(element, item, ItemContainerStyle);
            base.PrepareContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Undoes the effects of PrepareContainerForItemOverride.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The contained item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            // Remove the association with the Parent ItemsControl
            TreeViewItem node = element as TreeViewItem;
            if (node != null)
            {
                node.ParentItemsControl = null;
            }

            ItemContainerGenerator.ClearContainerForItemOverride(element, item);
            base.ClearContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Provides class handling for an ItemsChanged event that occurs when
        /// there is a change in the Items collection.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Stack<TreeViewItem> addedItems = new Stack<TreeViewItem>(e.NewItems.OfType<TreeViewItem>());
                    if (addedItems.Count > 0)
                    {
                        CheckForSelectedDescendents(addedItems);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    if (SelectedItem == null || IsSelectedContainerHookedUp)
                    {
                        break;
                    }
                    SelectFirstItem();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    object selectedItem = SelectedItem;
                    if (selectedItem == null || !object.Equals(selectedItem, e.OldItems[0]))
                    {
                        break;
                    }
                    ChangeSelection(selectedItem, SelectedContainer, false);
                    break;
            }
        }

        /// <summary>
        /// Select any descendents when adding TreeViewItems to a TreeView.
        /// </summary>
        /// <param name="items">The added items.</param>
        internal void CheckForSelectedDescendents(Stack<TreeViewItem> items)
        {
            Debug.Assert(items != null, "items should not be null!");
            Debug.Assert(items.Count > 0, "items should not be empty!");

            // Recurse into subtree of each added item to ensure none of
            // its descendents are selected.
            while (items.Count > 0)
            {
                TreeViewItem item = items.Pop();
                if (item.IsSelected)
                {
                    // Make IsSelected false so that its property changed
                    // handler will be fired when it's set to true in
                    // ChangeSelection
                    item.IgnorePropertyChange = true;
                    item.IsSelected = false;

                    ChangeSelection(item, item, true);

                    // If the item is not in the visual tree, we will make sure
                    // every check for ContainsSelection will try and update the
                    // sequence of ContainsSelection flags for the
                    // SelectedContainer.
                    if (SelectedContainer.ParentItemsControl == null)
                    {
                        SelectedContainer.RequiresContainsSelectionUpdate = true;
                    }
                }
                foreach (TreeViewItem nestedItem in item.Items.OfType<TreeViewItem>())
                {
                    items.Push(nestedItem);
                }
            }
        }

        /// <summary>
        /// Get the ItemContainerGenerator for a TreeView or TreeViewItem.
        /// </summary>
        /// <param name="control">TreeView or TreeViewItem.</param>
        /// <returns>
        /// The ItemContainerGenerator for a TreeView or TreeViewItem.
        /// </returns>
        internal static ItemContainerGenerator GetGenerator(ItemsControl control)
        {
            TreeViewItem item = control as TreeViewItem;
            if (item != null)
            {
                return item.ItemContainerGenerator;
            }
            else
            {
                TreeView view = control as TreeView;
                if (view != null)
                {
                    return view.ItemContainerGenerator;
                }
            }
            return null;
        }
        #endregion ItemsControl

        #region Input Events
        /// <summary>
        /// Propagate OnKeyDown messages from the root TreeViewItems to their
        /// TreeView.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <remarks>
        /// Because Silverlight's ScrollViewer swallows many useful key events
        /// (which it can ignore on WPF if you override HandlesScrolling or use
        /// an internal only variable in Silverlight), the root TreeViewItems
        /// explicitly propagate KeyDown events to their parent TreeView.
        /// </remarks>
        internal void PropagateKeyDown(KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        /// <summary>
        /// Provides handling for the KeyDown event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity metric is inflated by the switch statements")]
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (e.Handled)
            {
                return;
            }

            // The Control key modifier is used to scroll the viewer instead of
            // the selection
            if (IsControlKeyDown)
            {
                switch (e.Key)
                {
                    case Key.Home:
                    case Key.End:
                    case Key.PageUp:
                    case Key.PageDown:
                    case Key.Left:
                    case Key.Right:
                    case Key.Up:
                    case Key.Down:
                        if (HandleScrollKeys(e.Key))
                        {
                            e.Handled = true;
                        }
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.PageUp:
                    case Key.PageDown:
                        if (SelectedContainer != null)
                        {
                            if (HandleScrollByPage(e.Key == Key.PageUp))
                            {
                                e.Handled = true;
                            }
                            break;
                        }
                        if (FocusFirstItem())
                        {
                            e.Handled = true;
                        }
                        break;
                    case Key.Home:
                        if (FocusFirstItem())
                        {
                            e.Handled = true;
                        }
                        break;
                    case Key.End:
                        if (FocusLastItem())
                        {
                            e.Handled = true;
                        }
                        break;
                    case Key.Up:
                    case Key.Down:
                        if (SelectedContainer == null && FocusFirstItem())
                        {
                            e.Handled = true;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Handle keys related to scrolling.
        /// </summary>
        /// <param name="key">The key to handle.</param>
        /// <returns>A value indicating whether the key was handled.</returns>
        private bool HandleScrollKeys(Key key)
        {
            ScrollViewer scrollHost = ItemContainerGenerator.ScrollHost;
            if (scrollHost != null)
            {
                switch (key)
                {
                    case Key.PageUp:
                        // Move horizontally if we've run out of room vertically
                        if (!NumericExtensions.IsGreaterThan(scrollHost.ExtentHeight, scrollHost.ViewportHeight))
                        {
                            scrollHost.PageLeft();
                        }
                        else
                        {
                            scrollHost.PageUp();
                        }
                        return true;
                    case Key.PageDown:
                        // Move horizontally if we've run out of room vertically
                        if (!NumericExtensions.IsGreaterThan(scrollHost.ExtentHeight, scrollHost.ViewportHeight))
                        {
                            scrollHost.PageRight();
                        }
                        else
                        {
                            scrollHost.PageDown();
                        }
                        return true;
                    case Key.Home:
                        scrollHost.ScrollToTop();
                        return true;
                    case Key.End:
                        scrollHost.ScrollToBottom();
                        return true;
                    case Key.Left:
                        scrollHost.LineLeft();
                        return true;
                    case Key.Right:
                        scrollHost.LineRight();
                        return true;
                    case Key.Up:
                        scrollHost.LineUp();
                        return true;
                    case Key.Down:
                        scrollHost.LineDown();
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Handle scrolling a page up or down.
        /// </summary>
        /// <param name="up">
        /// A value indicating whether the page should be scrolled up.
        /// </param>
        /// <returns>
        /// A value indicating whether the scroll was handled.
        /// </returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Necessary complexity")]
        private bool HandleScrollByPage(bool up)
        {
            // NOTE: This implementation assumes that items are laid out
            // vertically and the Headers of the TreeViewItems appear above
            // their ItemsPresenter.  The same assumptions are made in WPF.

            ScrollViewer scrollHost = ItemContainerGenerator.ScrollHost;
            if (scrollHost != null)
            {
                double viewportHeight = scrollHost.ViewportHeight;
                
                double top;
                double bottom;
                SelectedContainer.GetTopAndBottom(scrollHost, out top, out bottom);

                TreeViewItem selected = null;
                TreeViewItem next = SelectedContainer;
                ItemsControl parent = SelectedContainer.ParentItemsControl;

                if (parent != null)
                {
                    // We need to start at the root TreeViewItem if we're
                    // scrolling up, but can start at the SelectedItem if
                    // scrolling down.
                    if (up)
                    {
                        while (parent != this)
                        {
                            TreeViewItem parentItem = parent as TreeViewItem;
                            if (parentItem == null)
                            {
                                break;
                            }

                            ItemsControl grandparent = parentItem.ParentItemsControl;
                            if (grandparent == null)
                            {
                                break;
                            }

                            next = parentItem;
                            parent = grandparent;
                        }
                    }

                    int index = GetGenerator(parent).IndexFromContainer(next);
                    int count = parent.Items.Count;
                    while (parent != null && next != null)
                    {
                        if (next.IsEnabled)
                        {
                            double delta;
                            if (next.HandleScrollByPage(up, scrollHost, viewportHeight, top, bottom, out delta))
                            {
                                // This item or one of its children was focused
                                return true;
                            }
                            else if (NumericExtensions.IsGreaterThan(delta, viewportHeight))
                            {
                                // If the item doesn't fit on the page but it's
                                // already selected, we'll select the next item
                                // even though it doesn't completely fit into
                                // the current view
                                if (selected == SelectedContainer || selected == null)
                                {
                                    return up ?
                                        SelectedContainer.HandleUpKey() :
                                        SelectedContainer.HandleDownKey();
                                }
                                break;
                            }
                            else
                            {
                                selected = next;
                            }
                        }

                        index += up ? -1 : 1;
                        if (0 <= index && index < count)
                        {
                            next = GetGenerator(parent).ContainerFromIndex(index) as TreeViewItem;
                        }
                        else if (parent == this)
                        {
                            // We just finished with the last item in the
                            // TreeView
                            next = null;
                        }
                        else
                        {
                            // Move up the parent chain to the next item
                            while (parent != null)
                            {
                                TreeViewItem oldParent = parent as TreeViewItem;
                                parent = oldParent.ParentItemsControl;
                                if (parent != null)
                                {
                                    count = parent.Items.Count;
                                    ItemContainerGenerator parentGenerator = GetGenerator(parent);
                                    index = parentGenerator.IndexFromContainer(oldParent) + (up ? -1 : 1);
                                    if (0 <= index && index < count)
                                    {
                                        next = parentGenerator.ContainerFromIndex(index) as TreeViewItem;
                                        break;
                                    }
                                    else if (parent == this)
                                    {
                                        next = null;
                                        parent = null;
                                    }
                                }
                            }
                        }
                    }
                }

                if (selected != null)
                {
                    if (up)
                    {
                        if (selected != SelectedContainer)
                        {
                            return selected.Focus();
                        }
                    }
                    else
                    {
                        selected.FocusInto();
                    }
                }
            }
            
            return false;
        }

        /// <summary>
        /// Provides handling for the MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (!e.Handled && HandleMouseButtonDown())
            {
                e.Handled = true;
            }
            
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Provides handling for mouse button events.
        /// </summary>
        /// <returns>A value indicating whether the event was handled.</returns>
        internal bool HandleMouseButtonDown()
        {
            if (SelectedContainer != null)
            {
                if (SelectedContainer != FocusManager.GetFocusedElement())
                {
                    SelectedContainer.Focus();
                }
                return true;
            }

            return false;
        }
        #endregion Input Events

        #region Selection
        /// <summary>
        /// Raises the SelectedItemChanged event when the SelectedItem property
        /// value changes.
        /// </summary>
        /// <param name="e">
        /// Provides the item that was previously selected and the item that is
        /// currently selected for the SelectedItemChanged event.
        /// </param>
        protected virtual void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            RoutedPropertyChangedEventHandler<object> handler = SelectedItemChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Change whether a TreeViewItem is selected.
        /// </summary>
        /// <param name="itemOrContainer">
        /// Item whose selection is changing.
        /// </param>
        /// <param name="container">
        /// Container of the item whose selection is changing.
        /// </param>
        /// <param name="selected">
        /// A value indicating whether the TreeViewItem is selected.
        /// </param>
        internal void ChangeSelection(object itemOrContainer, TreeViewItem container, bool selected)
        {
            // Ignore any change notifications if we're alread in the middle of
            // changing the selection
            if (IsSelectionChangeActive)
            {
                return;
            }

            object oldValue = null;
            object newValue = null;
            bool raiseSelectionChanged = false;
            TreeViewItem element = SelectedContainer;

            // Start changing the selection
            IsSelectionChangeActive = true;
            try
            {
                if (selected && container != SelectedContainer)
                {
                    // Unselect the old value
                    oldValue = SelectedItem;
                    if (SelectedContainer != null)
                    {
                        SelectedContainer.IsSelected = false;
                        SelectedContainer.UpdateContainsSelection(false);
                    }

                    // Select the new value
                    newValue = itemOrContainer;
                    SelectedContainer = container;
                    SelectedContainer.UpdateContainsSelection(true);
                    SelectedItem = itemOrContainer;
                    UpdateSelectedValue(itemOrContainer);
                    raiseSelectionChanged = true;

                    // Scroll the selected item into view.  We only want to
                    // scroll the header into view, if possible, because an
                    // expanded TreeViewItem contains all of its child items
                    // as well.
                    ItemContainerGenerator.ScrollIntoView(container.HeaderElement ?? container);
                }
                else if (!selected && container == SelectedContainer)
                {
                    // Unselect the old value
                    SelectedContainer.UpdateContainsSelection(false);
                    SelectedContainer = null;
                    SelectedItem = null;
                    SelectedValue = null;
                    oldValue = itemOrContainer;
                    raiseSelectionChanged = true;
                }

                container.IsSelected = selected;
            }
            finally
            {
                // Finish changing the selection
                IsSelectionChangeActive = false;
            }

            // Notify when the selection changes
            if (raiseSelectionChanged)
            {
                if (SelectedContainer != null && AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
                {
                    AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(SelectedContainer);
                    if (peer != null)
                    {
                        peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
                    }
                }
                if (element != null && AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
                {
                    AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(element);
                    if (peer != null)
                    {
                        peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                    }
                }

                OnSelectedItemChanged(new RoutedPropertyChangedEventArgs<object>(oldValue, newValue));
            }
        }

        /// <summary>
        /// Update the selected value of the of the TreeView based on the value
        /// of the currently selected TreeViewItem and the SelectedValuePath.
        /// </summary>
        /// <param name="item">
        /// Value of the currently selected TreeViewItem.
        /// </param>
        private void UpdateSelectedValue(object item)
        {
            if (item != null)
            {
                string path = SelectedValuePath;
                if (string.IsNullOrEmpty(path))
                {
                    SelectedValue = item;
                }
                else
                {
                    // Since we don't have the ability to evaluate a
                    // BindingExpression, we'll just create a new temporary
                    // control to bind the value to which we can then copy out
                    Binding binding = new Binding(path) { Source = item };
                    ContentControl temp = new ContentControl();
                    temp.SetBinding(ContentControl.ContentProperty, binding);
                    SelectedValue = temp.Content;

                    // Remove the Binding once we have the value (this is
                    // especially important if the value is a UIElement because
                    // it should not exist in the visual tree once we've
                    // finished)
                    temp.ClearValue(ContentControl.ContentProperty);
                }
            }
            else
            {
                ClearValue(SelectedValueProperty);
            }
        }

        /// <summary>
        /// Select the first item of the TreeView.
        /// </summary>
        private void SelectFirstItem()
        {
            TreeViewItem container = ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
            bool found = container != null;
            if (!found)
            {
                container = SelectedContainer;
            }

            object item = found ?
                ItemContainerGenerator.ItemFromContainer(container) :
                SelectedItem;

            ChangeSelection(item, container, found);
        }
        #endregion Selection

        #region Focus Navigation
        /// <summary>
        /// Focus the first item in the TreeView.
        /// </summary>
        /// <returns>A value indicating whether the item was focused.</returns>
        private bool FocusFirstItem()
        {
            // Get the first item in the TreeView.
            TreeViewItem item = ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
            return (item != null) ?
                (item.IsEnabled && item.Focus()) || item.FocusDown() :
                false;
        }

        /// <summary>
        /// Focus the last item in the TreeView.
        /// </summary>
        /// <returns>A value indicating whether the item was focused.</returns>
        private bool FocusLastItem()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                TreeViewItem item = ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                if (item != null && item.IsEnabled)
                {
                    return item.FocusInto();
                }
            }
            return false;
        }
        #endregion Focus Navigation
    }
}