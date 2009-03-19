// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Automation.Peers;
using System.Windows.Data;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a collection of collapsed and expanded AccordionItem controls.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(AccordionItem))]
    public class Accordion : ItemsControl
    {
        /// <summary>
        /// Determines whether the SelectedItemsProperty or 
        /// SelectedIndicesProperty may be written.
        /// </summary>
        private readonly bool _isAllowedToWrite;

        /// <summary>
        /// The item that is currently visually performing an action.
        /// </summary>
        /// <remarks>An action can be expanding, resizing or collapsing.</remarks>
        private AccordionItem _currentActioningItem;

        /// <summary>
        /// The items that are currently waiting to perform an action.
        /// </summary>
        /// <remarks>An action can be expanding, resizing or collapsing.</remarks>
        private List<AccordionItem> _scheduledActions;

        /// <summary>
        /// The ItemContainerGenerator that is associated with this control.
        /// </summary>
        private AccordionItemContainerGenerator _itemContainerGenerator;

        /// <summary>
        /// Gets the IItemContainerGenerator that is associated with this
        /// control.
        /// </summary>
        public IItemContainerGenerator ItemContainerGenerator
        {
            get { return _itemContainerGenerator; }
        }

        #region public ExpandDirection ExpandDirection
        /// <summary>
        /// Gets or sets the ExpandDirection property of each 
        /// AccordionItem in the Accordion control and the direction in which
        /// the Accordion does layout.
        /// </summary>
        /// <remarks>Setting the ExpandDirection will set the expand direction 
        /// on the accordionItems.</remarks>
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the ExpandDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register(
                "ExpandDirection",
                typeof(ExpandDirection),
                typeof(Accordion),
                new PropertyMetadata(ExpandDirection.Down, OnExpandDirectionPropertyChanged));

        /// <summary>
        /// ExpandDirectionProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its ExpandDirection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnExpandDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion source = (Accordion)d;
            ExpandDirection expandDirection = (ExpandDirection)e.NewValue;

            if (expandDirection != ExpandDirection.Down &&
                expandDirection != ExpandDirection.Up &&
                expandDirection != ExpandDirection.Left &&
                expandDirection != ExpandDirection.Right)
            {
                // revert to old value
                source.SetValue(ExpandDirectionProperty, e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.Accordion_OnExpandDirectionPropertyChanged_InvalidValue,
                    expandDirection);

                throw new ArgumentOutOfRangeException("e", message);
            }

            // force this change to all AccordionItems
            foreach (object item in source.Items)
            {
                AccordionItem accordionItem = source.ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;

                if (accordionItem != null)
                {
                    accordionItem.ExpandDirection = expandDirection;
                }
            }

            // set panel to align to the change
            source.SetPanelOrientation();

            // schedule a layout pass after this panel has had time to rearrange.
            source.Dispatcher.BeginInvoke(source.LayoutChildren);
        }
        #endregion public ExpandDirection ExpandDirection

        #region public AccordionSelectionMode SelectionMode
        /// <summary>
        /// Gets or sets the AccordionSelectionMode used to determine the minimum 
        /// and maximum selected AccordionItems allowed in the Accordion.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "WPF has a MultiSelector class that will be used in the future.")]
        public AccordionSelectionMode SelectionMode
        {
            get { return (AccordionSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                "SelectionMode",
                typeof(AccordionSelectionMode),
                typeof(Accordion),
                new PropertyMetadata(AccordionSelectionMode.One, OnSelectionModePropertyChanged));

        /// <summary>
        /// SelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its SelectionMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion source = (Accordion)d;

            AccordionSelectionMode newValue = (AccordionSelectionMode)e.NewValue;

            if (newValue != AccordionSelectionMode.One &&
                newValue != AccordionSelectionMode.OneOrMore &&
                newValue != AccordionSelectionMode.ZeroOrMore &&
                newValue != AccordionSelectionMode.ZeroOrOne)
            {
                // revert to old value
                source.SetValue(SelectionModeProperty, e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.Accordion_OnSelectionModePropertyChanged_InvalidValue,
                    newValue);
                throw new ArgumentOutOfRangeException("e", message);
            }

            // unlock all items
            // a selectionmode change is expected to change the locks.
            for (int i = 0; i < source.Items.Count; i++)
            {
                AccordionItem item = source.ItemContainerGenerator.ContainerFromIndex(i) as AccordionItem;
                if (item != null)
                {
                    item.IsLocked = false;
                }
            }

            // single selection coercion
            if (source.IsMinimumOneSelected)
            {
                // a minimum of one item should be selected
                if (source.GetValue(SelectedItemProperty) == null && source.Items.Count > 0)
                {
                    // select first accordionitem
                    source.SetValue(SelectedItemProperty, source.Items[0]);
                }
            }

            // multi selection coeercion
            if (source.IsMaximumOneSelected)
            {
                // allow at most one item.
                if (source.SelectedItems.Count > 1)
                {
                    foreach (object item in source.Items)
                    {
                        // unselect all items except the currently selected item.
                        if (!item.Equals(source.SelectedItem))
                        {
                            AccordionItem accordionItem = source.ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;
                            if (accordionItem != null)
                            {
                                accordionItem.IsSelected = false;
                            }
                        }
                    }
                }
            }

            // re-evaluate the locking status of the items in this new configuration
            source.SetLockedProperties();
        }

        /// <summary>
        /// Gets a value indicating whether at least one item is selected at 
        /// all times.
        /// </summary>
        private bool IsMinimumOneSelected
        {
            get
            {
                return SelectionMode == AccordionSelectionMode.One || SelectionMode == AccordionSelectionMode.OneOrMore;
            }
        }

        /// <summary>
        /// Gets a value indicating whether at most one item is selected at all times.
        /// </summary>
        private bool IsMaximumOneSelected
        {
            get
            {
                return SelectionMode == AccordionSelectionMode.One || SelectionMode == AccordionSelectionMode.ZeroOrOne;
            }
        }
        #endregion public AccordionSelectionMode SelectionMode

        #region public object SelectedItem
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <remarks>
        /// The default value is null.
        /// When multiple items are allowed (IsMaximumOneSelected false), 
        /// return the first of the selectedItems.
        /// </remarks>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(Accordion),
                new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        /// <summary>
        /// SelectedItemProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its SelectedItem.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion source = (Accordion)d;

            object oldValue = e.OldValue;
            object newValue = e.NewValue;

            if (oldValue != null && oldValue.Equals(newValue))
            {
                // when value types are used as items, there is a possibility of getting a change notification.
                return;
            }

            if (!source.IsValidItemForSelection(newValue))
            {
                // reset to oldvalue
                source._selectedItemNestedLevel++;
                source.SetValue(SelectedItemProperty, oldValue);
                source._selectedItemNestedLevel--;
            }
            else if (source._selectedItemNestedLevel == 0)
            {
                source.ChangeSelectedItem(oldValue, newValue);
            }
        }

        /// <summary>
        /// Determines whether the new value can be selected.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// 	<c>True</c> if this item can be selected; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidItemForSelection(object newValue)
        {
            // setting to null is supported.
            if (newValue == null && IsMinimumOneSelected == false)
            {
                return true;
            }

            // item should be contained inside the items collection.
            return Items.OfType<object>().Contains(newValue);
        }

        /// <summary>
        /// Nested level for SelectedItemCoercion.
        /// </summary>
        private int _selectedItemNestedLevel;

        #endregion public object SelectedItem

        #region public int SelectedIndex
        /// <summary>
        /// Gets or sets the index of the currently selected AccordionItem.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(Accordion),
                new PropertyMetadata(-1, OnSelectedIndexPropertyChanged));

        /// <summary>
        /// SelectedIndexProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its SelectedIndex.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion source = (Accordion)d;
            int oldValue = (int)e.OldValue;
            int newValue = (int)e.NewValue;

            if (!source.IsValidIndexForSelection(newValue))
            {
                // we have determined that the oldvalue is can be set, so do not throw events for it
                source._selectedIndexNestedLevel++;
                source.SetValue(SelectedIndexProperty, oldValue);
                source._selectedIndexNestedLevel--;
            }
            else if (source._selectedIndexNestedLevel == 0)
            {
                source.SelectedItem = source.Items.ElementAtOrDefault(newValue);
            }
        }

        /// <summary>
        /// Determines whether the new value can be selected.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// 	<c>True</c> if this item can be selected; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidIndexForSelection(int newValue)
        {
            // setting to null is supported.
            if (newValue == -1 && IsMinimumOneSelected == false)
            {
                return true;
            }

            // index should be contained inside the items collection.
            return newValue >= 0 && newValue < Items.Count;
        }

        /// <summary>
        /// Coercion level.
        /// </summary>
        private int _selectedIndexNestedLevel;
        #endregion public int SelectedIndex

        #region public SelectionSequence SelectionSequence
        /// <summary>
        /// Gets or sets the SelectionSequence used to determine 
        /// the order of AccordionItem selection.
        /// </summary>
        public SelectionSequence SelectionSequence
        {
            get { return (SelectionSequence)GetValue(SelectionSequenceProperty); }
            set { SetValue(SelectionSequenceProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionSequence dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionSequenceProperty =
            DependencyProperty.Register(
                "SelectionSequence",
                typeof(SelectionSequence),
                typeof(Accordion),
                new PropertyMetadata(SelectionSequence.Simultaneous, OnSelectionSequencePropertyChanged));

        /// <summary>
        /// Called when SelectionSequenceProperty changed.
        /// </summary>
        /// <param name="d">Accordion that changed its SelectionSequence property.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private static void OnSelectionSequencePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectionSequence newValue = (SelectionSequence)e.NewValue;

            if (newValue != SelectionSequence.CollapseBeforeExpand &&
                newValue != SelectionSequence.Simultaneous)
            {
                // revert to old value
                d.SetValue(Accordion.SelectionSequenceProperty, e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.Accordion_OnSelectionSequencepropertyChanged_InvalidValue,
                    newValue);
                throw new ArgumentOutOfRangeException("e", message);
            }
        }
        #endregion public SelectionSequence SelectionSequence

        #region public IList SelectedItems
        /// <summary>
        /// Gets the selected items.
        /// </summary>
        /// <remarks>Does not allow setting.</remarks>
        public IList SelectedItems
        {
            get { return GetValue(SelectedItemsProperty) as IList; }
        }

        /// <summary>
        /// Identifies the SelectedItems dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                "SelectedItems",
                typeof(IList),
                typeof(Accordion),
                new PropertyMetadata(OnSelectedItemsChanged));

        /// <summary>
        /// Property changed handler of SelectedItems.
        /// </summary>
        /// <param name="d">Accordion that changed the collection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion accordion = (Accordion)d;

            if (!accordion._isAllowedToWrite)
            {
                // revert to old value
                accordion.SetValue(SelectedItemsProperty, e.OldValue);

                throw new InvalidOperationException(Properties.Resources.Accordion_OnSelectedItemsChanged_InvalidWrite);
            }
        }
        #endregion public IList SelectedItems

        #region public IList<int> SelectedIndices
        /// <summary>
        /// Gets the indices of the currently selected AccordionItems.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "Framework uses indices.")]
        public IList<int> SelectedIndices
        {
            get { return GetValue(SelectedIndicesProperty) as IList<int>; }
        }

        /// <summary>
        /// Identifies the SelectedIndices dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "Framework uses indices.")]
        public static readonly DependencyProperty SelectedIndicesProperty =
            DependencyProperty.Register(
                "SelectedIndices",
                typeof(IList<int>),
                typeof(Accordion),
                new PropertyMetadata(null, OnSelectedIndicesChanged));

        /// <summary>
        /// Property changed handler of SelectedIndices.
        /// </summary>
        /// <param name="d">Accordion that changed the collection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedIndicesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion accordion = (Accordion)d;

            if (!accordion._isAllowedToWrite)
            {
                // revert to old value
                accordion.SetValue(SelectedIndicesProperty, e.OldValue);

                throw new InvalidOperationException(Properties.Resources.Accordion_OnSelectedIndicesChanged_InvalidWrite);
            }
        }
        #endregion public IList<int> SelectedIndices

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
                typeof(Accordion),
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
            Accordion source = (Accordion)d;
            Style value = e.NewValue as Style;
            source._itemContainerGenerator.UpdateItemContainerStyle(value);
        }
        #endregion public Style ItemContainerStyle

        #region public DataTemplate ContentTemplate
        /// <summary>
        /// Gets or sets the DataTemplate used to display the content 
        /// of each generated AccordionItem. 
        /// </summary>
        /// <remarks>Either ContentTemplate or ItemTemplate is used. 
        /// Setting both will result in an exception.</remarks>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
                "ContentTemplate",
                typeof(DataTemplate),
                typeof(Accordion),
                new PropertyMetadata(null));
        #endregion public DataTemplate ContentTemplate

        #region public DataTemplate HeaderTemplate
        /// <summary>
        /// Gets or sets the DataTemplate used to display the 
        /// header of each generated AccordionItem.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the HeaderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                "HeaderTemplate",
                typeof(DataTemplate),
                typeof(Accordion),
                new PropertyMetadata(null));
        #endregion public DataTemplate HeaderTemplate

        /// <summary>
        /// Occurs when the SelectedItem or SelectedItems property value changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when the SelectedItems collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler SelectedItemsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Accordion"/> class.
        /// </summary>
        public Accordion()
        {
            DefaultStyleKey = typeof(Accordion);
            _itemContainerGenerator = new AccordionItemContainerGenerator(this);

            ObservableCollection<object> items = new ObservableCollection<object>();
            ObservableCollection<int> indices = new ObservableCollection<int>();

            _isAllowedToWrite = true;
            SetValue(SelectedItemsProperty, items);
            SetValue(SelectedIndicesProperty, indices);
            _isAllowedToWrite = false;

            items.CollectionChanged += OnSelectedItemsCollectionChanged;
            indices.CollectionChanged += OnSelectedIndicesCollectionChanged;

            _scheduledActions = new List<AccordionItem>();
        }

        /// <summary>
        /// Builds the visual tree for the Accordion control when a 
        /// new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _itemContainerGenerator.OnApplyTemplate();
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Returns a AccordionAutomationPeer for use by the Silverlight
        /// automation infrastructure.
        /// </summary>
        /// <returns>A AccordionAutomationPeer object for the Accordion.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new AccordionAutomationPeer(this);
        }

        #region ItemsControl
        /// <summary>
        /// Creates or identifies the element that is used to display the given 
        /// item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            // return a new item and already set the header and content template on them
            AccordionItem item = new AccordionItem() { HeaderTemplate = this.HeaderTemplate, ContentTemplate = this.ContentTemplate };
            item.SetBinding(AccordionItem.HeaderProperty, new Binding() { Source = DataContext });
            return item;
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own 
        /// container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// True if the item is (or is eligible to be) its own container; 
        /// otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is AccordionItem;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (ItemTemplate != null && ContentTemplate != null)
            {
                throw new InvalidOperationException(Properties.Resources.Accordion_PrepareContainerForItemOverride_InvalidTemplates);
            }

            AccordionItem accordionItem = element as AccordionItem;

            if (accordionItem != null)
            {
                DataTemplate priorityContentTemplate = accordionItem.ContentTemplate;

                // after base.prepare, item template has replaced contenttemplate
                base.PrepareContainerForItemOverride(element, item);
                _itemContainerGenerator.PrepareContainerForItemOverride(accordionItem, item, ItemContainerStyle);

                DataTemplate displayMemberTemplate = accordionItem.ContentTemplate;

                if (priorityContentTemplate != null && !accordionItem.ContentTemplate.Equals(priorityContentTemplate))
                {
                    // set original template back. It takes precendence over a generated itemtemplate.
                    accordionItem.ContentTemplate = priorityContentTemplate;
                }

                // possibly set a displaymemberPath on header or content.
                if (displayMemberTemplate != null && !string.IsNullOrEmpty(DisplayMemberPath))
                {
                    if (accordionItem.ContentTemplate == null)
                    {
                        accordionItem.ContentTemplate = displayMemberTemplate;
                    }
                    if (accordionItem.HeaderTemplate == null)
                    {
                        accordionItem.HeaderTemplate = displayMemberTemplate;
                    }
                }

                // give accordionItem a reference back to the parent Accordion.
                accordionItem.ParentAccordion = this;
                // SelectedItem is expected to be set while adding items.
                // but could also be adding an item with the IsSelected set to true.
                if (item == SelectedItem)
                {
                    accordionItem.IsSelected = true;
                }

                if (accordionItem.IsSelected)
                {
                    SelectedItem = item;
                }

                // item might have been preselected when added to the item collection. 
                // at that point the parent had not been registered yet, so no notification was done.
                if (accordionItem.IsSelected)
                {
                    if (!SelectedItems.OfType<object>().Contains(item))
                    {
                        SelectedItems.Add(item);
                    }

                    int index = Items.OfType<object>().ToList().IndexOf(item);
                    if (!SelectedIndices.Contains(index))
                    {
                        SelectedIndices.Add(index);
                    }
                }
                accordionItem.ExpandDirection = ExpandDirection;
            }
            else
            {
                base.PrepareContainerForItemOverride(element, item);
                _itemContainerGenerator.PrepareContainerForItemOverride(element, item, ItemContainerStyle);
            }

            // The panel will register itself when it has had a child to add.
            SetPanelOrientation();

            // change has occured, re-evaluate the locked status on items
            SetLockedProperties();

            // At this moment this item has not been added to the panel yet, so we schedule a layoutpass
            Dispatcher.BeginInvoke(LayoutChildren);
        }

        /// <summary>
        /// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> 
        /// method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item that should be cleared.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            AccordionItem accordionItem = element as AccordionItem;
            if (accordionItem != null)
            {
                // release the parent child relationship.
                accordionItem.ParentAccordion = null;
            }

            _itemContainerGenerator.ClearContainerForItemOverride(element, item);
            base.ClearContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> 
        /// property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (SelectedItem == null && IsMinimumOneSelected)
                    {
                        if (!SelectedItems.OfType<object>().Contains(e.NewItems[0]))
                        {
                            SelectedItems.Add(e.NewItems[0]);
                        }
                        SelectedItem = e.NewItems[0];
                    }
                    break;
            }

            SetPanelOrientation();
        }
        #endregion ItemsControl

        #region Selection handling
        /// <summary>
        /// Called when an AccordionItem is unselected.
        /// </summary>
        /// <param name="accordionItem">The accordion item that was unselected.</param>
        internal void OnAccordionItemUnselected(AccordionItem accordionItem)
        {
            object item = ItemContainerGenerator.ItemFromContainer(accordionItem);

            if (SelectedItem != null && SelectedItem.Equals(item))
            {
                // this item is no longer the selected item. 
                // in order to keep the amount of thrown events down, will select a new selected item here.
                object newSelectedItem = ProposeSelectedItemCandidate(item);
                if (newSelectedItem != null && newSelectedItem.Equals(item))
                {
                    // no cancelling possible, undo the action.
                    // current template makes sure accordionheader does not allow this unselect
                    // that behavior is not enforced.
                    accordionItem.IsSelected = true;
                }
                else
                {
                    // select new item
                    SelectedItem = newSelectedItem;
                }
            }

            // update selecteditems collection
            if (SelectedItems.OfType<object>().Contains(item))
            {
                // remove from selected items
                SelectedItems.Remove(item);
            }

            // update selectedindexes collection
            int index = Items.OfType<object>().ToList().IndexOf(item);
            if (SelectedIndices.Contains(index))
            {
                SelectedIndices.Remove(index);
            }
        }

        /// <summary>
        /// Called when an AccordionItem selected.
        /// </summary>
        /// <param name="accordionItem">The accordion item that was selected.</param>
        internal void OnAccordionItemSelected(AccordionItem accordionItem)
        {
            object item = ItemContainerGenerator.ItemFromContainer(accordionItem);

            if (item != null)
            {
                SelectedItem = item;

                // update selecteditems collection
                if (!SelectedItems.OfType<object>().Contains(item))
                {
                    SelectedItems.Add(item);
                }

                // update selectedindexes collection
                int index = Items.OfType<object>().ToList().IndexOf(item);
                if (!SelectedIndices.Contains(index))
                {
                    SelectedIndices.Add(index);
                }
            }
        }

        /// <summary>
        /// Gets an item that is suitable for selection.
        /// </summary>
        /// <param name="nonCandidate">Item that should not be considered if 
        /// possible.</param>
        /// <returns>An item that should be selected. This could be nonCandidate, 
        /// if no other possibility was found.</returns>
        private object ProposeSelectedItemCandidate(object nonCandidate)
        {
            object FromSelectedItems = SelectedItems
                .OfType<object>()
                .Where(item => item != nonCandidate)
                .FirstOrDefault();

            if (FromSelectedItems != null)
            {
                return FromSelectedItems;
            }

            if (IsMinimumOneSelected && Items.Count > 0)
            {
                return Items[0];
            }

            return null;
        }

        /// <summary>
        /// Changes the selected item.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void ChangeSelectedItem(object oldValue, object newValue)
        {
            AccordionItem oldAccordionItem = oldValue as AccordionItem ?? ItemContainerGenerator.ContainerFromItem(oldValue) as AccordionItem;

            AccordionItem newAccordionItem = newValue as AccordionItem ?? ItemContainerGenerator.ContainerFromItem(newValue) as AccordionItem;

            if (IsMaximumOneSelected)
            {
                if (oldAccordionItem != null)
                {
                    // unselection can be triggered by selection of another item.
                    oldAccordionItem.IsLocked = false;
                    oldAccordionItem.IsSelected = false;
                }

                // raise event for UIAutomation.
                if (newAccordionItem != null && AutomationPeer.ListenerExists(
                        AutomationEvents.SelectionItemPatternOnElementSelected))
                {
                    AutomationPeer peer =
                        FrameworkElementAutomationPeer.CreatePeerForElement(newAccordionItem);
                    if (peer != null)
                    {
                        peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
                    }
                }
            }

            if (newAccordionItem != null)
            {
                newAccordionItem.IsSelected = true;
            }

            SelectedItem = newValue;
            SelectedIndex = newValue == null ? -1 : Items.ToList().IndexOf(newValue);

            object[] newValues = newValue == null ? new object[0] : new[] { newValue };
            object[] oldValues = oldValue == null ? new object[0] : new[] { oldValue };
            OnSelectedItemChanged(new SelectionChangedEventArgs(oldValues, newValues));
        }

        /// <summary>
        /// Raises the SelectedItemChanged event when the SelectedItem 
        /// property value changes.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        protected virtual void OnSelectedItemChanged(SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = SelectionChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when selected items collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // bundle up the actions that need to be performed so they can be scheduled on the dispatcher in one go.
            List<Action> actions = new List<Action>();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (object item in e.NewItems)
                        {
                            // possibly select them
                            AccordionItem accordionItem =
                                ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;
                            if (accordionItem != null && !accordionItem.IsSelected)
                            {
                                actions.Add(() => accordionItem.IsSelected = true);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (object item in e.OldItems)
                        {
                            // possibly unselect them
                            AccordionItem accordionItem =
                                ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;
                            if (accordionItem != null && accordionItem.IsSelected)
                            {
                                actions.Add(() => accordionItem.IsSelected = false);
                            }
                        }
                    }
                    break;
                default:
                    {
                        string message = string.Format(
                            CultureInfo.InvariantCulture,
                            Properties.Resources.Accordion_UnsupportedCollectionAction,
                            e.Action);

                        throw new NotSupportedException(message);
                    }
            }

            Action maintainance = () =>
            {
                // change has occured, re-evaluate the locked status on items
                SetLockedProperties();

                // do a layout pass.
                LayoutChildren();

                // let the outside world know
                RaiseOnSelectedItemsCollectionChanged(e);
            };

            if (actions.Count > 0)
            {
                // actions are scheduled only when the appropriate item was not yet set to the correct Selected state
                // thus signalling that we are modifying this collection directly
                Dispatcher.BeginInvoke(delegate
                {
                    foreach (Action a in actions)
                    {
                        a();
                    }

                    maintainance();
                });
            }
            else
            {
                maintainance();
            }
        }

        /// <summary>
        /// Raise the SelectedItemsCollectionChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        /// <remarks>This event is raised after the changes to the collection 
        /// have been processed.</remarks>
        private void RaiseOnSelectedItemsCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = SelectedItemsChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when selected indices collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private void OnSelectedIndicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // bundle up the actions that need to be performed so they can be scheduled on the dispatcher in one go.
            List<Action> actions = new List<Action>();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (int index in e.NewItems)
                        {
                            // possibly select them
                            AccordionItem accordionItem =
                                ItemContainerGenerator.ContainerFromIndex(index) as AccordionItem;
                            if (accordionItem != null && !accordionItem.IsSelected)
                            {
                                actions.Add(() => accordionItem.IsSelected = true);
                            }

                            // raise event for UIAutomation, which uses SelectedIndices to query SelectedItems.
                            if (AutomationPeer.ListenerExists(
                                    AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
                            {
                                AutomationPeer peer =
                                    FrameworkElementAutomationPeer.CreatePeerForElement(this);
                                if (peer != null)
                                {
                                    peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
                                }
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (int index in e.OldItems)
                        {
                            // possibly unselect them
                            AccordionItem accordionItem =
                                ItemContainerGenerator.ContainerFromIndex(index) as AccordionItem;
                            if (accordionItem != null && accordionItem.IsSelected)
                            {
                                actions.Add(() => accordionItem.IsSelected = false);
                            }

                            // raise event for UIAutomation, which uses SelectedIndices to query SelectedItems.
                            if (AutomationPeer.ListenerExists(
                                    AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
                            {
                                AutomationPeer peer =
                                    FrameworkElementAutomationPeer.CreatePeerForElement(this);
                                if (peer != null)
                                {
                                    peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                                }
                            }
                        }
                    }
                    break;
                default:
                    {
                        string message = string.Format(
                            CultureInfo.InvariantCulture,
                            Properties.Resources.Accordion_UnsupportedCollectionAction,
                            e.Action);

                        throw new NotSupportedException(message);
                    }
            }

            // layout pass is triggered by SelectedItemsChanged.
            if (actions.Count > 0)
            {
                Dispatcher.BeginInvoke(delegate
                {
                    foreach (Action a in actions)
                    {
                        a();
                    }
                });
            }
        }

        /// <summary>
        /// Selects all the AccordionItems in the Accordion control.
        /// </summary>
        /// <remarks>If the Accordion SelectionMode is OneOrMore or ZeroOrMore all 
        /// AccordionItems would be selected. If the Accordion SelectionMode is 
        /// One or ZeroOrOne all items would be selected and unselected. Only 
        /// the last AccordionItem would remain selected. </remarks>
        public void SelectAll()
        {
            UpdateAccordionItemsSelection(true);
        }

        /// <summary>
        /// Unselects all the AccordionItems in the Accordion control.
        /// </summary>
        /// <remarks>If the Accordion SelectionMode is Zero or ZeroOrMore all 
        /// AccordionItems would be Unselected. If SelectionMode is One or  
        /// OneOrMode  than all items would be Unselected and selected. Only the 
        /// first AccordionItem would still be selected.</remarks>
        public void UnselectAll()
        {
            UpdateAccordionItemsSelection(false);
        }

        /// <summary>
        /// Updates all accordionItems to be selected or unselected.
        /// </summary>
        /// <param name="selectedValue">True to select all items, false to unselect.</param>
        /// <remarks>Will not attempt to change a locked accordionItem.</remarks>
        private void UpdateAccordionItemsSelection(bool selectedValue)
        {
            foreach (object item in Items)
            {
                AccordionItem accordionItem = ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;

                if (accordionItem != null && !accordionItem.IsLocked)
                {
                    accordionItem.IsSelected = selectedValue;
                }
            }
        }

        /// <summary>
        /// Sets the locked properties on all the items.
        /// </summary>
        private void SetLockedProperties()
        {
            // an item that can not be unselected is locked.
            // This happens in 'One' or 'OneOrMore' selection mode, when the first item is selected.

            for (int i = 0; i < Items.Count; i++)
            {
                AccordionItem accordionItem = ItemContainerGenerator.ContainerFromIndex(i) as AccordionItem;

                if (accordionItem != null)
                {
                    accordionItem.IsLocked = (accordionItem.IsSelected && IsMinimumOneSelected && SelectedItems.Count == 1);
                }
            }
        }
        #endregion Selection handling

        #region Layout
        /// <summary>
        /// Allows an AccordionItem to signal the need for a visual action 
        /// (resize, collapse, expand).
        /// </summary>
        /// <param name="item">The AccordionItem that signals for a schedule.</param>
        /// <param name="action">The action it is scheduling for.</param>
        /// <returns>True if the item is allowed to proceed without scheduling, 
        /// false if the item needs to wait for a signal to execute the action.</returns>
        internal virtual bool ScheduleAction(AccordionItem item, AccordionAction action)
        {
            if (SelectionSequence == SelectionSequence.CollapseBeforeExpand)
            {
                lock (this)
                {
                    if (!_scheduledActions.Contains(item))
                    {
                        _scheduledActions.Add(item);
                    }
                }
                if (_currentActioningItem == null)
                {
                    Dispatcher.BeginInvoke(StartNextAction);
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Signals the finish of an action by an item.
        /// </summary>
        /// <param name="item">The AccordionItem that finishes an action.</param>
        /// <remarks>An AccordionItem should always signal a finish, for this call
        /// will start the next scheduled action.</remarks>
        internal virtual void OnActionFinish(AccordionItem item)
        {
            if (SelectionSequence == SelectionSequence.CollapseBeforeExpand)
            {
                lock (this)
                {
                    if (!_currentActioningItem.Equals(item))
                    {
                        throw new InvalidOperationException(Properties.Resources.Accordion_OnActionFinish_InvalidFinish);
                    }
                    _currentActioningItem = null;

                    StartNextAction();
                }
            }
        }

        /// <summary>
        /// Starts the next action in the list, in a particular order.
        /// </summary>
        /// <remarks>An AccordionItem is should always signal that it is 
        /// finished with an action.</remarks>
        private void StartNextAction()
        {
            if (_currentActioningItem != null)
            {
                return;
            }

            // First do collapses, then resizes and finally expands.
            AccordionItem next = _scheduledActions.FirstOrDefault(item => item.ScheduledAction == AccordionAction.Collapse);
            if (next == null)
            {
                next = _scheduledActions.FirstOrDefault(item => item.ScheduledAction == AccordionAction.Resize);
            }
            if (next == null)
            {
                next = _scheduledActions.FirstOrDefault(item => item.ScheduledAction == AccordionAction.Expand);
            }
            if (next != null)
            {
                _currentActioningItem = next;
                _scheduledActions.Remove(next);
                next.StartAction();
            }
        }

        /// <summary>
        /// Determines and sets the height of the accordion items.
        /// </summary>
        private void LayoutChildren()
        {
            ScrollViewer root = _itemContainerGenerator.ScrollHost;
            Size targetSize = new Size(double.NaN, double.NaN);
            if (root != null && _itemContainerGenerator.ItemsHost != null)
            {
                if (IsShouldFillWidth)
                {
                    // selected items should fill the remaining width of the container.
                    targetSize.Width = Math.Max(0, root.ViewportWidth - _itemContainerGenerator.ItemsHost.ActualWidth);

                    // calculate space currently occupied by items. This space will be redistributed.
                    foreach (object item in Items)
                    {
                        AccordionItem accordionItem = ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;
                        if (accordionItem != null)
                        {
                            targetSize.Width += accordionItem.RelevantContentSize.Width;
                        }
                    }

                    // calculated the targetsize for all selected items. Because of rounding issues, the
                    // actual space taken sometimes exceeds the appropriate amount by a fraction. 
                    if (targetSize.Width > 1)
                    {
                        targetSize.Width -= 1;
                    }

                    // possibly we are bigger than we would want, the items
                    // are overflowing. Always try to fit in current viewport.
                    if (root.ExtentWidth > root.ViewportWidth)
                    {
                        targetSize.Width = Math.Max(0, targetSize.Width - (root.ExtentWidth - root.ViewportWidth));
                    }

                    // calculate targetsize per selected item. This is redistribution.
                    targetSize.Width = SelectedItems.Count > 0 ? targetSize.Width / SelectedItems.Count : targetSize.Width;
                }
                else if (IsShouldFillHeight)
                {
                    // selected items should fill the remaining width of the container.
                    targetSize.Height = Math.Max(0, root.ViewportHeight - _itemContainerGenerator.ItemsHost.ActualHeight);

                    // calculate space currently occupied by items. This space will be redistributed.
                    foreach (object item in Items)
                    {
                        AccordionItem accordionItem = ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;
                        if (accordionItem != null)
                        {
                            targetSize.Height += accordionItem.RelevantContentSize.Height;
                        }
                    }

                    // calculated the targetsize for all selected items. Because of rounding issues, the
                    // actual space taken sometimes exceeds the appropriate amount by a fraction. 
                    if (targetSize.Height > 1)
                    {
                        targetSize.Height -= 1;
                    }

                    // calculate targetsize per selected item. This is redistribution.
                    targetSize.Height = SelectedItems.Count > 0 ? targetSize.Height / SelectedItems.Count : targetSize.Height;
                }

                // set that targetsize
                foreach (object item in Items)
                {
                    AccordionItem accordionItem = ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;
                    if (accordionItem != null)
                    {
                        // the calculated target size is calculated for the selected items.
                        if (accordionItem.IsSelected)
                        {
                            accordionItem.SetTargetContentSize(targetSize);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the accordion fills width.
        /// </summary>
        private bool IsShouldFillWidth
        {
            get
            {
                return (ExpandDirection == ExpandDirection.Left || ExpandDirection == ExpandDirection.Right) &&
                       (!Double.IsNaN(Width) || HorizontalAlignment == HorizontalAlignment.Stretch);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the accordion fills height.
        /// </summary>
        private bool IsShouldFillHeight
        {
            get
            {
                return (ExpandDirection == ExpandDirection.Down || ExpandDirection == ExpandDirection.Up) &&
                       (!Double.IsNaN(Height) || VerticalAlignment == VerticalAlignment.Stretch);
            }
        }

        /// <summary>
        /// Sets the orientation of the panel.
        /// </summary>
        private void SetPanelOrientation()
        {
            StackPanel panel = _itemContainerGenerator.ItemsHost as StackPanel;
            if (panel != null)
            {
                switch (ExpandDirection)
                {
                    case ExpandDirection.Down:
                    case ExpandDirection.Up:
                        panel.HorizontalAlignment = HorizontalAlignment.Stretch;
                        panel.VerticalAlignment = ExpandDirection == ExpandDirection.Down ? VerticalAlignment.Top : VerticalAlignment.Bottom;
                        panel.Orientation = Orientation.Vertical;
                        break;
                    case ExpandDirection.Left:
                    case ExpandDirection.Right:
                        panel.VerticalAlignment = VerticalAlignment.Stretch;
                        panel.HorizontalAlignment = ExpandDirection == ExpandDirection.Left ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                        panel.Orientation = Orientation.Horizontal;
                        break;
                }
            }
        }
        #endregion Layout
    }
}