// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Windows.Controls.Automation.Peers;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Represents a control that combines a text box and a drop down popup 
    /// containing a selection control. AutoCompleteBox allows users to filter 
    /// an items list.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    [TemplatePart(Name = AutoCompleteBox.ElementSelectionAdapter, Type = typeof(ISelectionAdapter))]
    [TemplatePart(Name = AutoCompleteBox.ElementTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = AutoCompleteBox.ElementPopup, Type = typeof(Popup))]
    [TemplatePart(Name = AutoCompleteBox.ElementDropDownToggle, Type = typeof(ToggleButton))]
    [StyleTypedProperty(Property = AutoCompleteBox.ElementTextBoxStyle, StyleTargetType = typeof(TextBox))]
    [StyleTypedProperty(Property = AutoCompleteBox.ElementItemContainerStyle, StyleTargetType = typeof(ListBox))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StatePopupClosed, GroupName = VisualStates.GroupPopup)]
    [TemplateVisualState(Name = VisualStates.StatePopupOpened, GroupName = VisualStates.GroupPopup)]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Large implementation keeps the components contained.")]
    public partial class AutoCompleteBox : Control, IUpdateVisualState
    {
        #region Template part and style names

        /// <summary>
        /// Specifies the name of the ListBox TemplatePart.
        /// </summary>
        private const string ElementSelectionAdapter = "SelectionAdapter";

        /// <summary>
        /// Specifies the name of the ToggleButton TemplatePart.
        /// </summary>
        private const string ElementDropDownToggle = "DropDownToggle";

        /// <summary>
        /// Specifies the name of the Popup TemplatePart.
        /// </summary>
        private const string ElementPopup = "Popup";
        
        /// <summary>
        /// The name for the text box part.
        /// </summary>
        private const string ElementTextBox = "Text";

        /// <summary>
        /// The name for the text box style.
        /// </summary>
        private const string ElementTextBoxStyle = "TextStyle";

        /// <summary>
        /// The name for the adapter's item container style.
        /// </summary>
        private const string ElementItemContainerStyle = "ContainerStyle";

        #endregion

        /// <summary>
        /// Gets or sets a local cached copy of the items data.
        /// </summary>
        private List<object> Items { get; set; }

        /// <summary>
        /// Gets or sets the observable collection that contains references to 
        /// all of the items in the generated view of data that is provided to 
        /// the selection-style control adapter.
        /// </summary>
        private ObservableCollection<object> View { get; set; }

        /// <summary>
        /// Gets or sets a value to ignore a number of pending change handlers. 
        /// The value is decremented after each use. This is used to reset the 
        /// value of properties without performing any of the actions in their 
        /// change handlers.
        /// </summary>
        /// <remarks>The int is important as a value because the TextBox 
        /// TextChanged event does not immediately fire, and this will allow for
        /// nested property changes to be ignored.</remarks>
        private int IgnoreTextPropertyChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore calling a pending 
        /// change handlers. 
        /// </summary>
        private bool IgnorePropertyChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the selection 
        /// changed event.
        /// </summary>
        private bool IgnoreTextSelectionChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to skip the text update 
        /// processing when the selected item is updated.
        /// </summary>
        private bool SkipSelectedItemTextUpdate { get; set; }

        /// <summary>
        /// Gets or sets the last observed text box selection start location.
        /// </summary>
        private int TextSelectionStart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user initiated the 
        /// current populate call.
        /// </summary>
        private bool UserCalledPopulate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a visual popup state is 
        /// being used in the current template for the Closed state. Setting 
        /// this value to true will delay the actual setting of Popup.IsOpen 
        /// to false until after the visual state's transition for Closed is 
        /// complete.
        /// </summary>
        private bool PopupClosedVisualState { get; set; }

        /// <summary>
        /// Gets or sets the DispatcherTimer used for the MinimumPopulateDelay 
        /// condition for auto completion.
        /// </summary>
        private DispatcherTimer DelayTimer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a read-only dependency 
        /// property change handler should allow the value to be set.  This is 
        /// used to ensure that read-only properties cannot be changed via 
        /// SetValue, etc.
        /// </summary>
        private bool AllowWrite { get; set; }

        /// <summary>
        /// Gets or sets the helper that provides all of the standard
        /// interaction functionality. Making it internal for subclass access.
        /// </summary>
        internal InteractionHelper Interaction { get; set; }

        #region public int MinimumPrefixLength
        /// <summary>
        /// Gets or sets the minimum text length before the AutoCompleteBox can 
        /// display suggestions.
        /// </summary>
        /// <remarks>
        /// The default MinimumPrefixLength value is 1 character. Valid integers
        /// range from -1 to any reasonable maximum. -1 effectively disables the 
        /// AutoCompleteBox functionality of the control.
        /// </remarks>
        public int MinimumPrefixLength
        {
            get { return (int)GetValue(MinimumPrefixLengthProperty); }
            set { SetValue(MinimumPrefixLengthProperty, value); }
        }

        /// <summary>
        /// Identifies the MinimumPrefixLength dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumPrefixLengthProperty =
            DependencyProperty.Register(
                "MinimumPrefixLength",
                typeof(int),
                typeof(AutoCompleteBox),
                new PropertyMetadata(1, OnMinimumPrefixLengthPropertyChanged));

        /// <summary>
        /// MinimumPrefixLengthProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its MinimumPrefixLength.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPrefixLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = d as AutoCompleteBox;
            int newValue = (int)e.NewValue;

            // If negative, coerce the value to -1
            if (newValue < 0 && newValue != -1)
            {
                source.SetValue(e.Property, -1);
            }
        }
        #endregion public int MinimumPrefixLength

        #region public int MinimumPopulateDelay
        /// <summary>
        /// Gets or sets the minimum delay required, in milliseconds, before 
        /// the AutoCompleteBox control will lookup and provide suggestions for 
        /// the current Text.
        /// </summary>
        public int MinimumPopulateDelay
        {
            get { return (int)GetValue(MinimumPopulateDelayProperty); }
            set { SetValue(MinimumPopulateDelayProperty, value); }
        }

        /// <summary>
        /// Identifies the MinimumPopulateDelay dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumPopulateDelayProperty =
            DependencyProperty.Register(
                "MinimumPopulateDelay",
                typeof(int),
                typeof(AutoCompleteBox),
                new PropertyMetadata(OnMinimumPopulateDelayPropertyChanged));

        // TODO: PLANNING: We should consider MinimumPopulateDelay as a TimeSpan

        /// <summary>
        /// MinimumPopulateDelayProperty property changed handler. Any current 
        /// dispatcher timer will be stopped. The timer will not be restarted 
        /// until the next TextUpdate call by the user.
        /// </summary>
        /// <param name="d">AutoCompleteTextBox that changed its 
        /// MinimumPopulateDelay.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception is most likely to be called through the CLR property setter.")]
        private static void OnMinimumPopulateDelayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = d as AutoCompleteBox;

            if (source.IgnorePropertyChange)
            {
                source.IgnorePropertyChange = false;
                return;
            }

            int newValue = (int)e.NewValue;
            if (newValue < 0)
            {
                source.IgnorePropertyChange = true;
                d.SetValue(e.Property, e.OldValue);

                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.AutoComplete_OnMinimumPopulateDelayPropertyChanged_InvalidValue, newValue), "value");
            }

            // Stop any existing timer
            if (source.DelayTimer != null)
            {
                source.DelayTimer.Stop();
                
                if (newValue == 0)
                {
                    source.DelayTimer = null;
                }
            }

            // Create or clear a dispatcher timer instance
            if (newValue > 0 && source.DelayTimer == null)
            {
                source.DelayTimer = new DispatcherTimer();
                source.DelayTimer.Tick += source.PopulateDropDown;
            }

            // Set the new tick interval
            if (newValue > 0 && source.DelayTimer != null)
            {
                source.DelayTimer.Interval = TimeSpan.FromMilliseconds(newValue);
            }
        }
        #endregion public int MinimumPopulateDelay
        
        #region public bool IsTextCompletionEnabled
        /// <summary>
        /// Gets or sets a value indicating whether the first suggestion found 
        /// during a lookup will be automatically displayed in the TextBox.
        /// </summary>
        /// <remarks>
        /// Additionally, performs a lookup for the associated item value 
        /// belonging to the first suggestion.
        /// </remarks>
        public bool IsTextCompletionEnabled
        {
            get { return (bool)GetValue(IsTextCompletionEnabledProperty); }
            set { SetValue(IsTextCompletionEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the IsTextCompletionEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTextCompletionEnabledProperty =
            DependencyProperty.Register(
                "IsTextCompletionEnabled",
                typeof(bool),
                typeof(AutoCompleteBox),
                new PropertyMetadata(false, null));

        #endregion public bool IsTextCompletionEnabled

        #region public DataTemplate ItemTemplate
        /// <summary>
        /// Gets or sets the DataTemplate used to display each item in the 
        /// drop down.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return GetValue(ItemTemplateProperty) as DataTemplate; }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                "ItemTemplate",
                typeof(DataTemplate),
                typeof(AutoCompleteBox),
                new PropertyMetadata(null));

        #endregion public DataTemplate ItemTemplate

        #region public Style ItemContainerStyle
        /// <summary>
        /// Gets or sets the Style that is applied to the selection adapter.
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
                typeof(AutoCompleteBox),
                new PropertyMetadata(null, null));

        #endregion public Style ItemContainerStyle

        #region public Style TextBoxStyle
        /// <summary>
        /// Gets or sets the Style that is applied to the TextBox.
        /// </summary>
        public Style TextBoxStyle
        {
            get { return GetValue(TextBoxStyleProperty) as Style; }
            set { SetValue(TextBoxStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the TextBoxStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TextBoxStyleProperty =
            DependencyProperty.Register(
                "TextBoxStyle",
                typeof(Style),
                typeof(AutoCompleteBox),
                new PropertyMetadata(null));

        #endregion public Style TextBoxStyle

        #region public double MaxDropDownHeight
        /// <summary>
        /// Gets or sets the maximum drop down height.
        /// </summary>
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the MaxDropDownHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register(
                "MaxDropDownHeight",
                typeof(double),
                typeof(AutoCompleteBox),
                new PropertyMetadata(double.PositiveInfinity, OnMaxDropDownHeightPropertyChanged));

        /// <summary>
        /// MaxDropDownHeightProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteTextBox that changed its MaxDropDownHeight.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception will be called through a CLR setter in most cases.")]
        private static void OnMaxDropDownHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = d as AutoCompleteBox;
            if (source.IgnorePropertyChange)
            {
                source.IgnorePropertyChange = false;
                return;
            }

            double newValue = (double)e.NewValue;
            
            // Revert to the old value if invalid (negative)
            if (newValue < 0)
            {
                source.IgnorePropertyChange = true;
                source.SetValue(e.Property, e.OldValue);

                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.AutoComplete_OnMaxDropDownHeightPropertyChanged_InvalidValue, e.NewValue), "value");
            }

            source.OnMaxDropDownHeightChanged(newValue);
        }
        #endregion public double MaxDropDownHeight

        #region public bool IsDropDownOpen
        /// <summary>
        /// Gets or sets a value indicating whether the drop down is open.
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Identifies the IsDropDownOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
                "IsDropDownOpen",
                typeof(bool),
                typeof(AutoCompleteBox),
                new PropertyMetadata(false, OnIsDropDownOpenPropertyChanged));

        /// <summary>
        /// IsDropDownOpenProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteTextBox that changed its IsDropDownOpen.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsDropDownOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = d as AutoCompleteBox;

            // Ignore the change if requested
            if (source.IgnorePropertyChange)
            {
                source.IgnorePropertyChange = false;
                return;
            }

            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            bool delayedClosingVisual = source.PopupClosedVisualState;
            RoutedPropertyChangingEventArgs<bool> args = new RoutedPropertyChangingEventArgs<bool>(e.Property, oldValue, newValue, true);

            AutoCompleteBoxAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(source) as AutoCompleteBoxAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent(oldValue, newValue);
            }

            if (newValue)
            {
                // Opening
                source.OnDropDownOpening(args);
                
                // Opened
                if (!args.Cancel)
                {
                    source.OpenDropDown(oldValue, newValue);
                }
            }
            else
            {
                // Closing
                source.OnDropDownClosing(args);

                if (source.View == null || source.View.Count == 0)
                {
                    delayedClosingVisual = false;
                }

                // Immediately close the drop down window:
                // When a popup closed visual state is present, the code path is 
                // slightly different and the actual call to CloseDropDown will 
                // be called only after the visual state's transition is done
                if (!args.Cancel && !delayedClosingVisual)
                {
                    source.CloseDropDown(oldValue, newValue);
                }
            }

            // If canceled, revert the value change
            if (args.Cancel)
            {
                source.IgnorePropertyChange = true;
                source.SetValue(e.Property, oldValue);
            }
            
            // Closing call when visual states are in use
            if (delayedClosingVisual)
            {
                source.UpdateVisualState(true);
            }
        }
        #endregion public bool IsDropDownOpen

        #region public IEnumerable ItemsSource
        /// <summary>
        /// Gets or sets a collection that is used to generate the content of 
        /// the control.
        /// </summary>
        /// <remarks>
        /// AutoCompleteBox does not properly support the INotifyCollectionChanged 
        /// interface. Directly set ItemsSource to a new value if the source 
        /// data has changed.</remarks>
        public IEnumerable ItemsSource
        {
            get { return GetValue(ItemsSourceProperty) as IEnumerable; }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(AutoCompleteBox),
                new PropertyMetadata(OnItemsSourcePropertyChanged));

        /// <summary>
        /// ItemsSourceProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its ItemsSource.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox autoComplete = d as AutoCompleteBox;
            autoComplete.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        #endregion public IEnumerable ItemsSource

        #region public object SelectedItem
        /// <summary>
        /// Gets or sets the selected item's value.
        /// </summary>
        /// <remarks>
        /// The IsTextCompletionEnabled property of the control impacts the 
        /// SelectedItem behavior: if the property is set to false, and the 
        /// user enters a valid items' textual representation, without 
        /// selection, the SelectedItem value will be null. The lookup between 
        /// text and items is only done when the IsTextCompletionEnabled property 
        /// is true (the default value). This is the same behavior that the 
        /// ComboBox control has in WPF.
        /// </remarks>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty) as object; }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(AutoCompleteBox),
                new PropertyMetadata(OnSelectedItemPropertyChanged));

        /// <summary>
        /// SelectedItemProperty property changed handler. Fires the 
        /// SelectionChanged event. The event data will contain any non-null
        /// removed items and non-null additions.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its SelectedItem.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = d as AutoCompleteBox;

            if (source.IgnorePropertyChange)
            {
                source.IgnorePropertyChange = false;
                return;
            }

            // Update the text display
            if (source.SkipSelectedItemTextUpdate)
            {
                source.SkipSelectedItemTextUpdate = false;
            }
            else
            {
                source.OnSelectedItemChanged(e.NewValue);
            }

            // Fire the SelectionChanged event
            List<object> removed = new List<object>();
            if (e.OldValue != null)
            {
                removed.Add(e.OldValue);
            }
            
            List<object> added = new List<object>();
            if (e.NewValue != null)
            {
                added.Add(e.NewValue);
            }

            source.OnSelectionChanged(new SelectionChangedEventArgs(removed, added));
        }

        /// <summary>
        /// Called when the selected item is changed, updates the text value
        /// that is displayed in the text box part.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        private void OnSelectedItemChanged(object newItem)
        {
            string text;

            if (newItem == null)
            {
                text = SearchText;
            }
            else
            {
                text = FormatValue(newItem);
            }

            // Update the Text property and the TextBox values
            UpdateTextValue(text);

            // Move the caret to the end of the text box
            if (TextBox != null && Text != null)
            {
                TextBox.SelectionStart = Text.Length;
            }
        }

        #endregion public object SelectedItem

        #region public string Text
        /// <summary>
        /// Gets or sets the contents of the TextBox.
        /// </summary>
        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(AutoCompleteBox),
                new PropertyMetadata(OnTextPropertyChanged));

        /// <summary>
        /// TextProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its Text.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = d as AutoCompleteBox;
            source.TextUpdated((string)e.NewValue, false);
        }

        #endregion public string Text

        #region public string SearchText
        /// <summary>
        /// Gets the text value used to search. This is a read-only dependency 
        /// property.
        /// </summary>
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }

            private set
            {
                try
                {
                    AllowWrite = true;
                    SetValue(SearchTextProperty, value);
                }
                finally
                {
                    AllowWrite = false;
                }
            }
        }

        /// <summary>
        /// Identifies the SearchText dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(
                "SearchText",
                typeof(string),
                typeof(AutoCompleteBox),
                new PropertyMetadata(string.Empty, OnSearchTextPropertyChanged));

        /// <summary>
        /// OnSearchTextProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its SearchText.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSearchTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = d as AutoCompleteBox;
            if (source.IgnorePropertyChange)
            {
                source.IgnorePropertyChange = false;
                return;
            }

            // Ensure the property is only written when expected
            if (!source.AllowWrite)
            {
                // Reset the old value before it was incorrectly written
                source.IgnorePropertyChange = true;
                source.SetValue(e.Property, e.OldValue);

                throw new InvalidOperationException(Properties.Resources.AutoComplete_OnSearchTextPropertyChanged_InvalidWrite);
            }
        }
        #endregion public string SearchText

        #region public AutoCompleteSearchMode SearchMode
        /// <summary>
        /// Gets or sets the built-in, predefined search mode to use for 
        /// searching the ItemsSource.
        /// </summary>
        public AutoCompleteSearchMode SearchMode
        {
            get { return (AutoCompleteSearchMode)GetValue(SearchModeProperty); }
            set { SetValue(SearchModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SearchMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchModeProperty =
            DependencyProperty.Register(
                "SearchMode",
                typeof(AutoCompleteSearchMode),
                typeof(AutoCompleteBox),
                new PropertyMetadata(AutoCompleteSearchMode.StartsWith, OnSearchModePropertyChanged));

        /// <summary>
        /// SearchModeProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its SearchMode.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The exception will be thrown when the CLR setter is used in most situations.")]
        private static void OnSearchModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = d as AutoCompleteBox;
            AutoCompleteSearchMode mode = (AutoCompleteSearchMode)e.NewValue;

            if (mode != AutoCompleteSearchMode.Contains &&
                mode != AutoCompleteSearchMode.ContainsCaseSensitive &&
                mode != AutoCompleteSearchMode.ContainsOrdinal &&
                mode != AutoCompleteSearchMode.ContainsOrdinalCaseSensitive &&
                mode != AutoCompleteSearchMode.Custom && 
                mode != AutoCompleteSearchMode.Equals &&
                mode != AutoCompleteSearchMode.EqualsCaseSensitive &&
                mode != AutoCompleteSearchMode.EqualsOrdinal &&
                mode != AutoCompleteSearchMode.EqualsOrdinalCaseSensitive &&
                mode != AutoCompleteSearchMode.None &&
                mode != AutoCompleteSearchMode.StartsWith &&
                mode != AutoCompleteSearchMode.StartsWithCaseSensitive &&
                mode != AutoCompleteSearchMode.StartsWithOrdinal &&
                mode != AutoCompleteSearchMode.StartsWithOrdinalCaseSensitive)
            {
                source.SetValue(e.Property, e.OldValue);

                throw new ArgumentException(Properties.Resources.AutoComplete_OnSearchModePropertyChanged_InvalidValue, "value");
            }

            // Sets the filter predicate for the new value
            AutoCompleteSearchMode newValue = (AutoCompleteSearchMode)e.NewValue;
            source.TextFilter = AutoCompleteSearch.GetFilter(newValue);
        }
        #endregion public AutoCompleteSearchMode SearchMode

        #region public AutoCompleteSearchPredicate ItemFilter
        /// <summary>
        /// Gets or sets a search filter that determines whether an item object 
        /// is a valid suggestion given the search string.
        /// </summary>
        public AutoCompleteSearchPredicate<object> ItemFilter
        {
            get { return GetValue(ItemFilterProperty) as AutoCompleteSearchPredicate<object>; }
            set { SetValue(ItemFilterProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemFilter dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemFilterProperty =
            DependencyProperty.Register(
                "ItemFilter",
                typeof(AutoCompleteSearchPredicate<object>),
                typeof(AutoCompleteBox),
                new PropertyMetadata(OnItemFilterPropertyChanged));

        /// <summary>
        /// ItemFilterProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its ItemFilter.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemFilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = d as AutoCompleteBox;
            AutoCompleteSearchPredicate<object> value = e.NewValue as AutoCompleteSearchPredicate<object>;
            
            // If null, revert to the "None" predicate
            if (value == null)
            {
                source.SearchMode = AutoCompleteSearchMode.None;
            }
            else
            {
                source.SearchMode = AutoCompleteSearchMode.Custom;
                source.TextFilter = null;
            }
        }
        #endregion public AutoCompleteSearchPredicate ItemFilter

        #region public AutoCompleteStringFilterPredicate TextFilter
        /// <summary>
        /// Gets or sets a search filter that determines whether a string is a 
        /// valid suggestion given the search text.
        /// </summary>
        public AutoCompleteSearchPredicate<string> TextFilter
        {
            get { return GetValue(TextFilterProperty) as AutoCompleteSearchPredicate<string>; }
            set { SetValue(TextFilterProperty, value); }
        }

        /// <summary>
        /// Identifies the TextFilter dependency property.
        /// </summary>
        public static readonly DependencyProperty TextFilterProperty =
            DependencyProperty.Register(
                "TextFilter",
                typeof(AutoCompleteSearchPredicate<string>),
                typeof(AutoCompleteBox),
                new PropertyMetadata(AutoCompleteSearch.GetFilter(AutoCompleteSearchMode.StartsWith)));
        #endregion public AutoCompleteStringFilterPredicate TextFilter

        #region public IValueConverter Converter
        /// <summary>
        /// Gets or sets the value converter used to convert item instances to 
        /// string values.
        /// </summary>
        /// <remarks>
        /// This enables high performance lookups. The conversion is from object 
        /// to type of string. 
        /// </remarks>
        public IValueConverter Converter
        {
            get { return GetValue(ConverterProperty) as IValueConverter; }
            set { SetValue(ConverterProperty, value); }
        }

        /// <summary>
        /// Identifies the Converter dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register(
                "Converter",
                typeof(IValueConverter),
                typeof(AutoCompleteBox),
                new PropertyMetadata(null, null));

        #endregion public IValueConverter Converter

        #region public object ConverterParameter
        /// <summary>
        /// Gets or sets the parameter used with the converter property.
        /// </summary>
        public object ConverterParameter
        {
            get { return GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the ConverterParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register(
                "ConverterParameter",
                typeof(object),
                typeof(AutoCompleteBox),
                new PropertyMetadata(null, OnConverterParameterPropertyChanged));

        /// <summary>
        /// ConverterParameterProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// AutoCompleteBox that changed its ConverterParameter.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnConverterParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = d as AutoCompleteBox;
            if (source.View != null && source.View.Count > 0)
            {
                source.ToggleDropDown(d, new RoutedEventArgs());
            }
        }
        #endregion public object ConverterParameter

        #region public CultureInfo ConverterCulture
        /// <summary>
        /// Gets or sets the culture used in with the converter property.
        /// </summary>
        public CultureInfo ConverterCulture
        {
            get { return GetValue(ConverterCultureProperty) as CultureInfo; }
            set { SetValue(ConverterCultureProperty, value); }
        }

        /// <summary>
        /// Identifies the ConverterCulture dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterCultureProperty =
            DependencyProperty.Register(
                "ConverterCulture",
                typeof(CultureInfo),
                typeof(AutoCompleteBox),
                new PropertyMetadata(CultureInfo.CurrentUICulture, OnConverterCulturePropertyChanged));

        /// <summary>
        /// ConverterCultureProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// AutoCompleteBox that changed its ConverterCulture.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnConverterCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteBox source = d as AutoCompleteBox;
            if (source.View != null && source.View.Count > 0)
            {
                source.ToggleDropDown(d, new RoutedEventArgs());
            }
        }
        #endregion public CultureInfo ConverterCulture

        #region Template parts and Popup elements

        /// <summary>
        /// Gets or sets the template's ToggleButton part.
        /// </summary>
        private ToggleButton DropDownToggleButton
        {
            get { return _toggleButton; }
            set
            {
                if (_toggleButton != null)
                {
                    DropDownToggleButton.Click -= ToggleDropDown;
                }

                _toggleButton = value;

                if (_toggleButton != null)
                {
                    DropDownToggleButton.Click += ToggleDropDown;
                }
            }
        }

        /// <summary>
        /// Gets or sets the drop down popup control.
        /// </summary>
        private Popup DropDownPopup { get; set; }

        /// <summary>
        /// The toggle button template part.
        /// </summary>
        private ToggleButton _toggleButton;

        /// <summary>
        /// The TextBox template part.
        /// </summary>
        private TextBox _text;

        /// <summary>
        /// The SelectionAdapter.
        /// </summary>
        private ISelectionAdapter _adapter;

        /// <summary>
        /// Gets or sets the Text template part.
        /// </summary>
        internal TextBox TextBox
        {
            get { return _text; }
            set
            {
                // Detach existing handlers
                if (_text != null)
                {
                    _text.SelectionChanged -= OnTextBoxSelectionChanged;
                    _text.TextChanged -= OnTextBoxTextChanged;
                }

                _text = value;

                // Attach handlers
                if (_text != null)
                {
                    _text.SelectionChanged += OnTextBoxSelectionChanged;
                    _text.TextChanged += OnTextBoxTextChanged;

                    if (Text != null)
                    {
                        UpdateTextValue(Text);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the adapter that represents a selection control. Now 
        /// internal or protected so that automation peers and derived classes 
        /// can access it.
        /// </summary>
        protected internal ISelectionAdapter SelectionAdapter 
        { 
            get { return _adapter; }
            set
            {
                if (_adapter != null)
                {
                    _adapter.SelectionChanged -= OnAdapterSelectionChanged;
                    _adapter.Commit -= OnAdapterSelectionComplete;
                    _adapter.Cancel -= OnAdapterSelectionCanceled;
                    _adapter.Cancel -= OnAdapterSelectionComplete;
                    _adapter.ItemsSource = null;
                }

                _adapter = value;

                if (_adapter != null)
                {
                    _adapter.SelectionChanged += OnAdapterSelectionChanged;
                    _adapter.Commit += OnAdapterSelectionComplete;
                    _adapter.Cancel += OnAdapterSelectionCanceled;
                    _adapter.Cancel += OnAdapterSelectionComplete;
                    _adapter.ItemsSource = View;
                }
            }
        }

        /// <summary>
        /// Gets or sets the popup child content.
        /// </summary>
        private FrameworkElement PopupChild { get; set; }

        /// <summary>
        /// Gets or sets the expansive area outside of the popup.
        /// </summary>
        private Canvas OutsidePopupCanvas { get; set; }

        /// <summary>
        /// Gets or sets the canvas for the popup child.
        /// </summary>
        private Canvas PopupChildCanvas { get; set; }

        #endregion

        /// <summary>
        /// Occurs when the text changes.
        /// </summary>
        public event RoutedEventHandler TextChanged;

        /// <summary>
        /// Occurs when the AutoCompleteBox is populating the selection adapter 
        /// with suggestions based on the text property. 
        /// </summary>
        /// <remarks>
        /// If Canceled, the control will not continue the automatic suggestion 
        /// process, which will be left to the handler.
        /// </remarks>
        public event PopulatingEventHandler Populating;

        /// <summary>
        /// Occurs when the AutoCompleteBox has populated the selection adapter 
        /// with suggestions based on the text property.
        /// </summary>
        public event PopulatedEventHandler Populated;

        /// <summary>
        /// Occurs when the IsDropDownOpen property is changing from false to 
        /// true. The event can be cancelled.
        /// </summary>
        public event RoutedPropertyChangingEventHandler<bool> DropDownOpening;

        /// <summary>
        /// Occurs when the IsDropDownOpen property was changed from false to true.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> DropDownOpened;

        /// <summary>
        /// Occurs when the IsDropDownOpen property is changing from true to 
        /// false. The event can be cancelled.
        /// </summary>
        public event RoutedPropertyChangingEventHandler<bool> DropDownClosing;

        /// <summary>
        /// Occurs when the IsDropDownOpen property was changed from true to 
        /// false.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> DropDownClosed;

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Gets a value indicating whether the content is editable. The 
        /// default Silverlight Toolkit AutoCompleteBox is always editable.
        /// </summary>
        public virtual bool IsEditable { get { return true; } }

        /// <summary>
        /// Initializes a new instance of the AutoCompleteBox control class.
        /// </summary>
        public AutoCompleteBox()
        {
            DefaultStyleKey = typeof(AutoCompleteBox);
            
            Loaded += (sender, e) => ApplyTemplate();
            IsEnabledChanged += ControlIsEnabledChanged;

            Interaction = new InteractionHelper(this);

            // Creating the view here ensures that View is always != null
            ClearView();
        }

        /// <summary>
        /// Arranges and sizes the auto complete control and its contents.
        /// </summary>
        /// <param name="finalSize">The provided arrangement bounds object.</param>
        /// <returns>Returns the arrangement bounds, unchanged.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size r = base.ArrangeOverride(finalSize);
            ArrangePopup();
            return r;
        }

        /// <summary>
        /// Builds the visual tree for the AutoCompleteBox control when a 
        /// new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Unhook the event handler for the popup closed visual state group.
            // This code is used to enable visual state transitions before 
            // actually setting the underlying Popup.IsOpen property to false.
            VisualStateGroup groupPopupClosed = VisualStates.TryGetVisualStateGroup(this, VisualStates.GroupPopup);
            if (null != groupPopupClosed)
            {
                groupPopupClosed.CurrentStateChanged -= OnPopupClosedStateChanged;
                PopupClosedVisualState = false;
            }

            base.OnApplyTemplate();

            groupPopupClosed = VisualStates.TryGetVisualStateGroup(this, VisualStates.GroupPopup);
            if (null != groupPopupClosed)
            {
                groupPopupClosed.CurrentStateChanged += OnPopupClosedStateChanged;
                PopupClosedVisualState = true;
            }

            // Explicit set of the popup to closed. This is in line with other 
            // drop down controls, including the Silverlight ComboBox's 
            // implementation.
            IsDropDownOpen = false;

            // Set the template parts. Individual part setters remove and add 
            // any event handlers.
            DropDownToggleButton = GetTemplateChild(ElementDropDownToggle) as ToggleButton;
            DropDownPopup = GetTemplateChild(ElementPopup) as Popup;
            SelectionAdapter = TryGetSelectionAdapter(GetTemplateChild(ElementSelectionAdapter));
            TextBox = GetTemplateChild(AutoCompleteBox.ElementTextBox) as TextBox;

            // TODO: Consider moving to the DropDownPopup setter
            // TODO: Although in line with other implementations, what happens 
            // when the template is swapped out?
            if (DropDownPopup != null)
            {
                OutsidePopupCanvas = new Canvas();
                PopupChildCanvas = new Canvas();
                PopupChild = DropDownPopup.Child as FrameworkElement;
                OutsidePopupCanvas.Background = new SolidColorBrush(Colors.Transparent);

                if (PopupChild != null)
                {
                    PopupChildCanvas.Children.Add(OutsidePopupCanvas);
                    PopupChildCanvas.Children.Add(PopupChild);
                    DropDownPopup.Child = PopupChildCanvas;
                    PopupChild.GotFocus += PopupChild_GotFocus;
                    PopupChild.LostFocus += PopupChild_LostFocus;
                    PopupChild.MouseEnter += PopupChild_MouseEnter;
                    PopupChild.MouseLeave += PopupChild_MouseLeave;
                    PopupChild.SizeChanged += PopupChild_SizeChanged;
                    OutsidePopupCanvas.MouseLeftButtonDown += OutsidePopup_MouseLeftButtonDown;
                }
            }

            Interaction.OnApplyTemplateBase();
        }

        /// <summary>
        /// Creates the AutomationPeer for the AutoCompleteBox.
        /// </summary>
        /// <returns>Returns a new AutoCompleteBoxAutomationPeer.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new AutoCompleteBoxAutomationPeer(this);
        }

        #region Focus

        /// <summary>
        /// Handles the FocusChanged event.
        /// </summary>
        /// <param name="hasFocus">A value indicating whether the control 
        /// currently has the focus.</param>
        private void FocusChanged(bool hasFocus)
        {
            // The OnGotFocus & OnLostFocus are asynchronously and cannot 
            // reliably tell you that have the focus.  All they do is let you 
            // know that the focus changed sometime in the past.  To determine 
            // if you currently have the focus you need to do consult the 
            // FocusManager (see HasFocus()).

            if (hasFocus)
            {
                if (TextBox != null && TextBox.SelectionLength == 0)
                {
                    TextBox.SelectAll();
                }
            }
            else
            {
                IsDropDownOpen = false;
                UserCalledPopulate = false;
                if (TextBox != null)
                {
                    TextBox.Select(TextBox.Text.Length, 0);
                }
            }
        }

        /// <summary>
        /// Checks to see if the control has focus currently.
        /// </summary>
        /// <returns>Returns a value indicating whether the control or its popup
        /// have focus.</returns>
        private bool HasFocus()
        {
            DependencyObject focused = FocusManager.GetFocusedElement() as DependencyObject;
            while (focused != null)
            {
                if (object.ReferenceEquals(focused, this))
                {
                    return true;
                }

                // This helps deal with popups that may not be in the same 
                // visual tree
                DependencyObject parent = VisualTreeHelper.GetParent(focused);
                if (parent == null)
                {
                    // Try the logical parent.
                    FrameworkElement element = focused as FrameworkElement;
                    if (element != null)
                    {
                        parent = element.Parent;
                    }
                }
                focused = parent;
            }
            return false;
        }

        /// <summary>
        /// Provides class handling for the GotFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            FocusChanged(HasFocus());
        }

        /// <summary>
        /// Provides handling for the LostFocus event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            FocusChanged(HasFocus());
        }

        #endregion

        /// <summary>
        /// Handle the change of the IsEnabled property.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void ControlIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isEnabled = (bool)e.NewValue;
            if (!isEnabled)
            {
                IsDropDownOpen = false;
            }
        }

        /// <summary>
        /// Actually closes the popup after the VSM state animation completes.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPopupClosedStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            switch (e.NewState.Name)
            {
                // Delayed closing of the popup until now
                case VisualStates.StatePopupClosed:
                    CloseDropDown(true, false);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Attempts to return an ISelectionAdapter wrapper for a specified object.
        /// </summary>
        /// <param name="value">The object value.</param>
        /// <returns>Returns an IItemsSelector wrapping the value.</returns>
        /// <remarks>
        /// The specified object will be returned if it implements 
        /// ISelectionAdapter. If the specified object can be placed in a known 
        /// implementation of ISelectionAdapter, one containing the specified 
        /// object will be returned. Otherwise null will be returned.
        /// 
        /// Custom adapters can be added by deriving a new control from 
        /// AutoCompleteBox and overriding the TryGetSelectionAdapter method.</remarks>
        protected virtual ISelectionAdapter TryGetSelectionAdapter(object value)
        {
            // Check if it is already an IItemsSelector
            ISelectionAdapter asAdapter = value as ISelectionAdapter;
            if (asAdapter != null)
            {
                return asAdapter;
            }

            // Built in support for wrapping a Selector control
            Selector asSelector = value as Selector;
            if (asSelector != null)
            {
                return new SelectorSelectionAdapter(asSelector);
            }

            // TODO: PLANNING: Consider an Attribute to expose extensibility

            return null;
        }

        /// <summary>
        /// Handles the timer tick when using a populate delay.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void PopulateDropDown(object sender, EventArgs e)
        {
            if (DelayTimer != null)
            {
                DelayTimer.Stop();
            }

            // Update the prefix/search text.
            SearchText = Text;

            // The Populated event enables advanced, custom filtering. The 
            // client needs to directly update the ItemsSource collection or
            // call the Populate method on the control to continue the 
            // display process if Cancel is set to true.
            PopulatingEventArgs populating = new PopulatingEventArgs(SearchText);
            OnPopulating(populating);
            if (!populating.Cancel)
            {
                PopulateComplete();
            }
        }

        /// <summary>
        /// Raises the Populating event when the AutoCompleteBox is populating the 
        /// selection adapter with suggestions based on the text property.
        /// </summary>
        /// <param name="e">The populating event data.</param>
        protected virtual void OnPopulating(PopulatingEventArgs e)
        {
            PopulatingEventHandler handler = Populating;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the Populated event when the AutoCompleteBox has populated the 
        /// selection adapter with suggestions based on the text property.
        /// </summary>
        /// <param name="e">The populated event data.</param>
        protected virtual void OnPopulated(PopulatedEventArgs e)
        {
            PopulatedEventHandler handler = Populated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the SelectionChanged event when the selected item has
        /// changed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises an DropDownOpening event when the IsDropDownOpen property is 
        /// changing from false to true.
        /// </summary>
        /// <param name="e">
        /// Provides any observers the opportunity to cancel the operation and 
        /// halt opening the drop down.
        /// </param>
        protected virtual void OnDropDownOpening(RoutedPropertyChangingEventArgs<bool> e)
        {
            RoutedPropertyChangingEventHandler<bool> handler = DropDownOpening;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises an DropDownOpened event when the IsDropDownOpen property 
        /// changed from false to true.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDropDownOpened(RoutedPropertyChangedEventArgs<bool> e)
        {
            RoutedPropertyChangedEventHandler<bool> handler = DropDownOpened;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises an DropDownClosing event when the IsDropDownOpen property is 
        /// changing from true to false.
        /// </summary>
        /// <param name="e">
        /// Provides any observers the opportunity to cancel the operation 
        /// and halt closing the drop down.
        /// </param>
        protected virtual void OnDropDownClosing(RoutedPropertyChangingEventArgs<bool> e)
        {
            RoutedPropertyChangingEventHandler<bool> handler = DropDownClosing;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises an DropDownClosed event when the IsDropDownOpen property 
        /// changed from true to false.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnDropDownClosed(RoutedPropertyChangedEventArgs<bool> e)
        {
            RoutedPropertyChangedEventHandler<bool> handler = DropDownClosed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Formats an Item for text comparisons based on Converter 
        /// and ConverterCulture properties.
        /// </summary>
        /// <param name="value">The object to format.</param>
        /// <returns>Formatted Value.</returns>
        protected virtual string FormatValue(object value)
        {
            // TODO: PLANNING: In the future, check for and use TypeConverters

            if (Converter != null)
            {
                return Converter.Convert(value, typeof(string), ConverterParameter, ConverterCulture) as string ?? string.Empty;
            }
            else
            {
                return value == null ? string.Empty : value.ToString();
            }
        }

        /// <summary>
        /// Raises the TextChanged event when the text has changed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnTextChanged(RoutedEventArgs e)
        {
            RoutedEventHandler handler = TextChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Handle the TextChanged event that is directly attached to the 
        /// TextBox part. This ensures that only user initiated actions will 
        /// result in an AutoCompleteBox suggestion and operation.
        /// </summary>
        /// <param name="sender">The source TextBox object.</param>
        /// <param name="e">The TextChanged event data.</param>
        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            // Call the central updated text method as a user-initiated action
            TextUpdated(_text.Text, true);
        }

        /// <summary>
        /// When selection changes, save the location of the selection start.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            // If ignoring updates. This happens after text is updated, and 
            // before the PopulateComplete method is called. Required for the 
            // IsTextCompletionEnabled feature.
            if (IgnoreTextSelectionChange)
            {
                return;
            }

            TextSelectionStart = _text.SelectionStart;
        }

        /// <summary>
        /// Updates both the text box value and underlying text dependency 
        /// property value if and when they change. Automatically fires the 
        /// text changed events when there is a change.
        /// </summary>
        /// <param name="value">The new string value.</param>
        private void UpdateTextValue(string value)
        {
            UpdateTextValue(value, null);
        }

        /// <summary>
        /// Updates both the text box value and underlying text dependency 
        /// property value if and when they change. Automatically fires the 
        /// text changed events when there is a change.
        /// </summary>
        /// <param name="value">The new string value.</param>
        /// <param name="userInitiated">A nullable bool value indicating whether
        /// the action was user initiated. In a user initiated mode, the 
        /// underlying text dependency property is updated. In a non-user 
        /// interaction, the text box value is updated. When user initiated is 
        /// null, all values are updated.</param>
        private void UpdateTextValue(string value, bool? userInitiated)
        {
            // Update the Text dependency property
            if ((userInitiated == null || userInitiated == true) && Text != value)
            {
                IgnoreTextPropertyChange++;
                Text = value;
                OnTextChanged(new RoutedEventArgs());
            }

            // Update the TextBox's Text dependency property
            if ((userInitiated == null || userInitiated == false) && TextBox != null && TextBox.Text != value)
            {
                IgnoreTextPropertyChange++;
                TextBox.Text = value ?? string.Empty;

                // Text dependency property value was set, fire event
                if (Text == value || Text == null)
                {
                    OnTextChanged(new RoutedEventArgs());
                }
            }
        }

        /// <summary>
        /// Handle the update of the text for the control from any source, 
        /// including the TextBox part and the Text dependency property.
        /// </summary>
        /// <param name="newText">The new text.</param>
        /// <param name="userInitiated">A value indicating whether the update 
        /// is a user-initiated action. This should be a True value when the 
        /// TextUpdated method is called from a TextBox event handler.</param>
        private void TextUpdated(string newText, bool userInitiated)
        {
            // Only process this event if it is coming from someone outside 
            // setting the Text dependency property directly.
            if (IgnoreTextPropertyChange > 0)
            {
                IgnoreTextPropertyChange--;
                return;
            }

            if (newText == null)
            {
                newText = string.Empty;
            }

            // The TextBox.TextChanged event was not firing immediately and 
            // was causing an immediate update, even with wrapping. If there is 
            // a selection currently, no update should happen.
            if (IsTextCompletionEnabled && TextBox != null && TextBox.SelectionLength > 0 && TextBox.SelectionStart != TextBox.Text.Length)
            {
                return;
            }

            // Evaluate the conditions needed for completion.
            // 1. Minimum prefix length
            // 2. If a delay timer is in use, use it
            bool populateReady = newText.Length >= MinimumPrefixLength && MinimumPrefixLength >= 0;
            UserCalledPopulate = populateReady ? userInitiated : false;

            // Update the interface and values only as necessary
            UpdateTextValue(newText, userInitiated);

            if (populateReady)
            {
                IgnoreTextSelectionChange = true;

                if (DelayTimer != null)
                {
                    DelayTimer.Start();
                }
                else
                {
                    PopulateDropDown(this, EventArgs.Empty);
                }
            }
            else
            {
                SearchText = null;
                if (SelectedItem != null)
                {
                    SkipSelectedItemTextUpdate = true;
                }
                SelectedItem = null;
                if (IsDropDownOpen)
                {
                    IsDropDownOpen = false;
                }
            }
        }

        /// <summary>
        /// Notifies AutoCompleteBox that ItemsSource has been populated and 
        /// suggestions can now be computed using that data.
        /// </summary>
        /// <remarks>
        /// Allows a developer to continue the population event after setting 
        /// the Cancel property to True. This allows for custom, 
        /// developer-driven AutoCompleteBox scenarios.
        /// </remarks>
        public void PopulateComplete()
        {
            // Apply the search filter
            RefreshView();

            // Fire the Populated event containing the read-only view data.
            PopulatedEventArgs populated = new PopulatedEventArgs(new ReadOnlyCollection<object>(View));
            OnPopulated(populated);

            if (SelectionAdapter != null && SelectionAdapter.ItemsSource != View)
            {
                SelectionAdapter.ItemsSource = View;
            }

            IsDropDownOpen = UserCalledPopulate && (View.Count > 0);
            if (IsDropDownOpen)
            {
                ArrangePopup();
            }

            UpdateTextCompletion(UserCalledPopulate);
        }

        /// <summary>
        /// Performs text completion, if enabled, and a lookup on the underlying
        /// item values for an exact match. Will update the SelectedItem value.
        /// </summary>
        /// <param name="userInitiated">A value indicating whether the operation
        /// was user initiated. Text completion will not be performed when not 
        /// directly initiated by the user.</param>
        private void UpdateTextCompletion(bool userInitiated)
        {
            // By default this method will clear the selected value
            object newSelectedItem = null;
            string text = Text;

            // Text search is StartsWith explicit and only when enabled, in 
            // line with WPF's ComboBox lookup. When in use it will associate 
            // a Value with the Text if it is found in ItemsSource. This is 
            // only valid when there is data and the user initiated the action.
            if (View.Count > 0)
            {
                if (IsTextCompletionEnabled && TextBox != null && userInitiated)
                {
                    int currentLength = TextBox.Text.Length;
                    int selectionStart = TextBox.SelectionStart;
                    if (selectionStart == text.Length && selectionStart > TextSelectionStart)
                    {
                        // When the SearchMode dependency property is set to 
                        // either StartsWith or StartsWithCaseSensitive, the 
                        // first item in the view is used. This will improve 
                        // performance on the lookup. It assumes that the 
                        // SearchMode the user has selected is an acceptable 
                        // case sensitive matching function for their scenario.
                        object top = SearchMode == AutoCompleteSearchMode.StartsWith || SearchMode == AutoCompleteSearchMode.StartsWithCaseSensitive
                            ? View[0]
                            : TryGetMatch(text, View, AutoCompleteSearch.GetFilter(AutoCompleteSearchMode.StartsWith));

                        // If the search was successful, update SelectedItem
                        if (top != null)
                        {
                            newSelectedItem = top;
                            string topString = FormatValue(top);

                            // Only replace partially when the two words being the same
                            int minLength = Math.Min(topString.Length, Text.Length);
                            if (AutoCompleteSearch.Equals(Text.Substring(0, minLength), topString.Substring(0, minLength)))
                            {
                                // Update the text
                                UpdateTextValue(topString);

                                // Select the text past the user's caret
                                TextBox.SelectionStart = currentLength;
                                TextBox.SelectionLength = topString.Length - currentLength;
                            }
                        }
                    }
                }
                else
                {
                    // Perform an exact string lookup for the text. This is a 
                    // design change from the original Toolkit release when the 
                    // IsTextCompletionEnabled property behaved just like the 
                    // WPF ComboBox's IsTextSearchEnabled property.
                    //
                    // This change provides the behavior that most people expect
                    // to find: a lookup for the value is always performed.
                    newSelectedItem = TryGetMatch(text, View, AutoCompleteSearch.GetFilter(AutoCompleteSearchMode.EqualsCaseSensitive));
                }
            }

            // Update the selected item property

            if (SelectedItem != newSelectedItem)
            {
                SkipSelectedItemTextUpdate = true;
            }
            SelectedItem = newSelectedItem;

            // Restore updates for TextSelection
            if (IgnoreTextSelectionChange)
            {
                IgnoreTextSelectionChange = false;
                if (TextBox != null)
                {
                    TextSelectionStart = TextBox.SelectionStart;
                }
            }
        }

        /// <summary>
        /// Attempts to look through the view and locate the specific exact 
        /// text match.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="view">The view reference.</param>
        /// <param name="predicate">The predicate to use for the partial or 
        /// exact match.</param>
        /// <returns>Returns the object or null.</returns>
        private object TryGetMatch(string searchText, ObservableCollection<object> view, AutoCompleteSearchPredicate<string> predicate)
        {
            if (view != null && view.Count > 0)
            {
                foreach (object o in view)
                {
                    if (predicate(searchText, FormatValue(o)))
                    {
                        return o;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// A simple helper method to clear the view and ensure that a view 
        /// object is always present and not null.
        /// </summary>
        private void ClearView()
        {
            if (View == null)
            {
                View = new ObservableCollection<object>();
            }
            else
            {
                View.Clear();
            }
        }

        /// <summary>
        /// Walks through the items enumeration. Performance is not going to be 
        /// perfect with the current implementation.
        /// </summary>
        private void RefreshView()
        {
            if (Items == null)
            {
                ClearView();
                return;
            }

            // Cache the current text value
            string text = Text ?? string.Empty;

            // Determine if any filtering mode is on
            bool stringFiltering = TextFilter != null;
            bool objectFiltering = SearchMode == AutoCompleteSearchMode.Custom && TextFilter == null;

            int view_index = 0;
            int view_count = View.Count;
            List<object> items = Items;
            foreach (object item in items)
            {
                bool inResults = !(stringFiltering || objectFiltering);
                if (!inResults)
                {
                    inResults = stringFiltering ? TextFilter(text, FormatValue(item)) : ItemFilter(text, item);
                }

                if (view_count > view_index && inResults && View[view_index] == item)
                {
                    // Item is still in the view
                    view_index++;
                }
                else if (inResults)
                {
                    // Insert the item
                    if (view_count > view_index && View[view_index] != item)
                    {
                        // Replace item
                        // Unfortunately replacing via index throws a fatal 
                        // exception: View[view_index] = item;
                        // Cost: O(n) vs O(1)
                        View.RemoveAt(view_index);
                        View.Insert(view_index, item);
                        view_index++;
                    }
                    else
                    {
                        // Add the item
                        if (view_index == view_count)
                        {
                            // Constant time is preferred (Add).
                            View.Add(item);
                        }
                        else
                        {
                            View.Insert(view_index, item);
                        }
                        view_index++;
                        view_count++;
                    }
                }
                else if (view_count > view_index && View[view_index] == item)
                {
                    // Remove the item
                    View.RemoveAt(view_index);
                    view_count--;
                }
            }
        }

        /// <summary>
        /// Handle any change to the ItemsSource dependency property, update 
        /// the underlying ObservableCollection view, and set the selection 
        /// adapter's ItemsSource to the view if appropriate.
        /// </summary>
        /// <param name="oldValue">The old enumerable reference.</param>
        /// <param name="newValue">The new enumerable reference.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "oldValue", Justification = "This makes it easy to add validation or other changes in the future.")]
        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // Remove handler for oldValue.CollectionChanged (if present)
            INotifyCollectionChanged oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;
            if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(ItemsSourceCollectionChanged);
            }

            // Add handler for newValue.CollectionChanged (if possible)
            INotifyCollectionChanged newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(ItemsSourceCollectionChanged);
            }

            // Store a local cached copy of the data
            Items = newValue == null ? null : new List<object>(newValue.Cast<object>().ToList());

            // Clear and set the view on the selection adapter
            ClearView();
            if (SelectionAdapter != null && SelectionAdapter.ItemsSource != View)
            {
                SelectionAdapter.ItemsSource = View;
            }
        }

        /// <summary>
        /// Method that handles the ObservableCollection.CollectionChanged event for the ItemsSource property.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Update the cache
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                for (int index = 0; index < e.OldItems.Count; index++)
                {
                    Items.RemoveAt(e.OldStartingIndex);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null && Items.Count >= e.NewStartingIndex)
            {
                for (int index = 0; index < e.NewItems.Count; index++)
                {
                    Items.Insert(e.NewStartingIndex + index, e.NewItems[index]);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems != null && e.OldItems != null)
            {
                for (int index = 0; index < e.NewItems.Count; index++)
                {
                    Items[e.NewStartingIndex] = e.NewItems[index];
                }
            }

            // Update the view
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                for (int index = 0; index < e.OldItems.Count; index++)
                {
                    View.Remove(e.OldItems[index]);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearView();
            }
            else
            {
                RefreshView();
            }
        }

        #region Selection Adapter

        /// <summary>
        /// Handles the SelectionChanged event of the selection adapter.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The selection changed event data.</param>
        private void OnAdapterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = _adapter.SelectedItem;
        }

        /// <summary>
        /// Handles the Commit event on the selection adapter.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnAdapterSelectionComplete(object sender, RoutedEventArgs e)
        {
            if (IsDropDownOpen)
            {
                IsDropDownOpen = false;
            }

            // Completion will update the selected value
            UpdateTextCompletion(false);

            // Text should not be selected
            if (TextBox != null)
            {
                TextBox.Select(TextBox.Text.Length, 0);
            }
        }

        /// <summary>
        /// Handles the Cancel event on the selection adapter.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnAdapterSelectionCanceled(object sender, RoutedEventArgs e)
        {
            UpdateTextValue(SearchText);

            // Completion will update the selected value
            UpdateTextCompletion(false);
        }

        #endregion

        #region Popup

        /// <summary>
        /// Handles MaxDropDownHeightChanged by re-arranging and updating the 
        /// popup arrangement.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newValue", Justification = "This makes it easy to add validation or other changes in the future.")]
        private void OnMaxDropDownHeightChanged(double newValue)
        {
            ArrangePopup();
            UpdateVisualState(true);
        }

        /// <summary>
        /// Private method that directly opens the popup, checks the expander 
        /// button, and then fires the Opened event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OpenDropDown(bool oldValue, bool newValue)
        {
            if (DropDownPopup != null)
            {
                DropDownPopup.IsOpen = true;
            }
            if (DropDownToggleButton != null)
            {
                DropDownToggleButton.IsChecked = true;
            }

            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue);
            OnDropDownOpened(e);
        }

        /// <summary>
        /// Private method that directly closes the popup, flips the Checked 
        /// value, and then fires the Closed event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void CloseDropDown(bool oldValue, bool newValue)
        {
            if (SelectionAdapter != null)
            {
                SelectionAdapter.SelectedItem = null;
            }
            if (DropDownPopup != null)
            {
                DropDownPopup.IsOpen = false;
            }
            if (DropDownToggleButton != null)
            {
                DropDownToggleButton.IsChecked = false;
            }

            RoutedPropertyChangedEventArgs<bool> e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue);
            OnDropDownClosed(e);
        }

        /// <summary>
        /// The popup child has received focus.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusChanged(HasFocus());
        }

        /// <summary>
        /// The popup child has lost focus.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_LostFocus(object sender, RoutedEventArgs e)
        {
            FocusChanged(HasFocus());
        }

        /// <summary>
        /// The popup child has had the mouse enter its bounds.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_MouseEnter(object sender, MouseEventArgs e)
        {
            UpdateVisualState(true);
        }

        /// <summary>
        /// The mouse has left the popup child's bounds.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_MouseLeave(object sender, MouseEventArgs e)
        {
            UpdateVisualState(true);
        }

        /// <summary>
        /// The size of the popup child has changed.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void PopupChild_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ArrangePopup();
        }

        /// <summary>
        /// The mouse has clicked outside of the popup.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OutsidePopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsDropDownOpen = false;
        }

        /// <summary>
        /// Arrange the drop down popup.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This try-catch pattern is used by other popup controls to keep the runtime up.")]
        private void ArrangePopup()
        {
            if (DropDownPopup == null || PopupChild == null || OutsidePopupCanvas == null || Application.Current == null || Application.Current.Host == null || Application.Current.Host.Content == null)
            {
                return;
            }

            Content hostContent = Application.Current.Host.Content;
            double rootWidth = hostContent.ActualWidth;
            double rootHeight = hostContent.ActualHeight;

            double popupContentWidth = PopupChild.ActualWidth;
            double popupContentHeight = PopupChild.ActualHeight;

            if (rootHeight == 0 || rootWidth == 0 || popupContentWidth == 0 || popupContentHeight == 0)
            {
                return;
            }

            // Getting the transform will throw if the popup is no longer in 
            // the visual tree.  This can happen if you first open the popup 
            // and then click on something else on the page that removes it 
            // from the live tree.
            MatrixTransform mt = null;
            try
            {
                mt = TransformToVisual(null) as MatrixTransform;
            }
            catch
            {
                IsDropDownOpen = false;
            }
            if (mt == null)
            {
                return;
            }

            double rootOffsetX = mt.Matrix.OffsetX;
            double rootOffsetY = mt.Matrix.OffsetY;

            double myControlHeight = ActualHeight;
            double myControlWidth = ActualWidth;

            // TODO: Revisit the magic numbers

            // Use or come up with a maximum popup height.
            double popupMaxHeight = MaxDropDownHeight;
            if (double.IsInfinity(popupMaxHeight) || double.IsNaN(popupMaxHeight))
            {
                popupMaxHeight = (rootHeight - myControlHeight) * 3 / 5;
            }

            popupContentWidth = Math.Min(popupContentWidth, rootWidth);
            popupContentHeight = Math.Min(popupContentHeight, popupMaxHeight);
            popupContentWidth = Math.Max(myControlWidth, popupContentWidth);

            // We prefer to align the popup box with the left edge of the 
            // control, if it will fit.
            double popupX = rootOffsetX;
            if (rootWidth < popupX + popupContentWidth)
            {
                // Since it doesn't fit when strictly left aligned, we shift it 
                // to the left until it does fit.
                popupX = rootWidth - popupContentWidth;
                popupX = Math.Max(0, popupX);
            }

            // We prefer to put the popup below the combobox if it will fit.
            bool below = true;
            double popupY = rootOffsetY + myControlHeight;
            if (rootHeight < popupY + popupContentHeight)
            {
                below = false;
                // It doesn't fit below the combobox, lets try putting it above 
                // the combobox.
                popupY = rootOffsetY - popupContentHeight;
                if (popupY < 0)
                {
                    // doesn't really fit below either.  Now we just pick top 
                    // or bottom based on wich area is bigger.
                    if (rootOffsetY < (rootHeight - myControlHeight) / 2)
                    {
                        below = true;
                        popupY = rootOffsetY + myControlHeight;
                    }
                    else
                    {
                        below = false;
                        popupY = rootOffsetY - popupContentHeight;
                    }
                }
            }

            // Now that we have positioned the popup we may need to truncate 
            // its size.
            popupMaxHeight = below ? Math.Min(rootHeight - popupY, popupMaxHeight) : Math.Min(rootOffsetY, popupMaxHeight);

            DropDownPopup.HorizontalOffset = 0;
            DropDownPopup.VerticalOffset = 0;

            OutsidePopupCanvas.Width = rootWidth;
            OutsidePopupCanvas.Height = rootHeight;

            // Transform the transparent canvas to the plugin's coordinate 
            // space origin.
            Matrix transformToRootMatrix = mt.Matrix;
            Matrix newMatrix;
            transformToRootMatrix.Invert(out newMatrix);
            mt.Matrix = newMatrix;
            OutsidePopupCanvas.RenderTransform = mt;

            PopupChild.MinWidth = myControlWidth;
            PopupChild.MaxWidth = rootWidth;
            PopupChild.MinHeight = 0;
            PopupChild.MaxHeight = Math.Max(0, popupMaxHeight);

            PopupChild.Width = popupContentWidth;
            // TODO: RESEARCH: This next line was commented out previously
            // PopupChild.Height = popupContentHeight;
            PopupChild.HorizontalAlignment = HorizontalAlignment.Left;
            PopupChild.VerticalAlignment = VerticalAlignment.Top;

            // Set the top left corner for the actual drop down.
            Canvas.SetLeft(PopupChild, popupX - rootOffsetX);
            Canvas.SetTop(PopupChild, popupY - rootOffsetY);
        }

        #endregion

        /// <summary>
        /// Toggles the drop down visibility. If visible, the text updated 
        /// method will be called, eventually refreshing the selection adapter's
        /// view and making the appropriate visibility call on the drop down. 
        /// If no items are in the filtered view, the drop down will not be 
        /// displayed.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void ToggleDropDown(object sender, RoutedEventArgs e)
        {
            if (IsEnabled)
            {
                if (IsDropDownOpen)
                {
                    IsDropDownOpen = false;
                }
                else
                {
                    // If the drop down is current closed, the TextUpdated 
                    // method is called to re-evaluate the Text and display the 
                    // drop down only if items are available in the view.
                    TextUpdated(Text, true);
                }
            }
        }

        #region Keyboard

        /// <summary>
        /// Provides class handling for the KeyDown event that occurs when a 
        /// key is pressed while the control has focus.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            base.OnKeyDown(e);

            if (e.Handled || !IsEnabled)
            {
                return;
            }

            // TODO: CONSIDER: What about the Adapter interface: should it 
            // offer the ability to always handle events from the text box 
            // key down?
            if (IsDropDownOpen)
            {
                OnDropDownKeyDown(e);
            }
            else
            {
                OnTextBoxKeyDown(e);
            }
        }

        /// <summary>
        /// Occurs when the KeyDown event fires and the drop down is not open.
        /// </summary>
        /// <param name="e">The key event data.</param>
        protected void OnTextBoxKeyDown(KeyEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            else if (e.Handled)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Down:
                    if (!IsDropDownOpen)
                    {
                        ToggleDropDown(this, e);
                        e.Handled = true;
                    }
                    break;

                case Key.F4:
                    ToggleDropDown(this, e);
                    e.Handled = true;
                    break;

                case Key.Enter:
                    OnAdapterSelectionComplete(this, new RoutedEventArgs());
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Raises the DropDownKeyDown event when a key down event occurs 
        /// when the drop down is open.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected void OnDropDownKeyDown(KeyEventArgs e)
        {
            if (e == null || e.Handled)
            {
                return;
            }

            if (SelectionAdapter != null)
            {
                SelectionAdapter.HandleKeyDown(e);
                if (e.Handled)
                {
                    return;
                }
            }

            switch (e.Key)
            {
                case Key.Enter:
                    OnAdapterSelectionComplete(this, new RoutedEventArgs());
                    e.Handled = true;
                    break;

                case Key.Escape:
                    OnAdapterSelectionCanceled(this, new RoutedEventArgs());
                    e.Handled = true;
                    break;

                case Key.F4:
                    ToggleDropDown(this, e);
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            UpdateVisualState(useTransitions);
        }

        /// <summary>
        /// Update the current visual state of the button.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            // Popup
            VisualStateManager.GoToState(this, IsDropDownOpen ? VisualStates.StatePopupOpened : VisualStates.StatePopupClosed, useTransitions);

            // Handle the Common and Focused states
            Interaction.UpdateVisualStateBase(useTransitions);
        }
    }
}
