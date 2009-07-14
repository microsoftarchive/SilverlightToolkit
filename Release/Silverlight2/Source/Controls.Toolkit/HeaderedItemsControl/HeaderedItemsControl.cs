// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that contains a collection of items and a header.
    /// </summary>
    /// <remarks>
    /// Use this class to create a control that contains a heading (or label) 
    /// and multiple items. The TreeViewItem class is an example of a class that 
    /// inherits from HeaderedItemsControl.
    /// <para>
    /// A HeaderedItemsControl extends the ItemsControl class and adds a Header
    ///  property for displaying label content at the top of the control. 
    /// The Header property accepts an object, which means it can contain any 
    /// kind of content. You can also set a data template to specify how the 
    /// label content should display using the HeaderTemplate property.
    /// </para>
    /// <para>
    /// A HeaderedItemsControl has a limited default style. You can create a 
    /// HeaderedItemsControl, but its appearance will be very simple. If you want 
    /// to enhance the appearance of the control, you must create a new 
    /// ControlTemplate for the control. A HeaderedItemsControl is useful 
    /// for creating custom controls because it provides a model for an items 
    /// control with a header.
    /// </para>
    /// <para>
    /// Dependency properties for this control might be set by the control’s 
    /// default style. If a property is set by a default style, 
    /// the property might change from its default value when the control 
    /// appears in the application. 
    /// </para>
    /// </remarks>
    /// <QualityBand>Stable</QualityBand>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ContentPresenter))]
    public partial class HeaderedItemsControl : ItemsControl
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Header property has been
        /// set to the item of an ItemsControl.
        /// </summary>
        internal bool HeaderIsItem { get; set; }

        #region public object Header
        /// <summary>
        /// Gets or sets the item that labels the control. 
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        /// <remarks>
        /// Note: WPF defines this property via a call to AddOwner of
        /// HeaderedContentControl's HeaderProperty.
        /// </remarks>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(object),
                typeof(HeaderedItemsControl),
                new PropertyMetadata(OnHeaderPropertyChanged));

        /// <summary>
        /// HeaderProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// HeaderedItemsControl that changed its Header.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedItemsControl source = d as HeaderedItemsControl;
            source.OnHeaderChanged(e.OldValue, e.NewValue);
        }
        #endregion public object Header

        #region public DataTemplate HeaderTemplate
        /// <summary>
        /// Gets or sets a data template that is used to display the 
        /// contents of the control's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return GetValue(HeaderTemplateProperty) as DataTemplate; }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the HeaderTemplate dependency property.
        /// </summary>
        /// <remarks>
        /// Note: WPF defines this property via a call to AddOwner of
        /// HeaderedContentControl's HeaderTemplateProperty.
        /// </remarks>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                "HeaderTemplate",
                typeof(DataTemplate),
                typeof(HeaderedItemsControl),
                new PropertyMetadata(OnHeaderTemplatePropertyChanged));

        /// <summary>
        /// HeaderTemplateProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// HeaderedItemsControl that changed its HeaderTemplate.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnHeaderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedItemsControl source = d as HeaderedItemsControl;
            DataTemplate oldHeaderTemplate = e.OldValue as DataTemplate;
            DataTemplate newHeaderTemplate = e.NewValue as DataTemplate;
            source.OnHeaderTemplateChanged(oldHeaderTemplate, newHeaderTemplate);
        }
        #endregion public DataTemplate HeaderTemplate

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
                typeof(HeaderedItemsControl),
                new PropertyMetadata(null, OnItemContainerStylePropertyChanged));

        /// <summary>
        /// ItemContainerStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// HeaderedItemsControl that changed its ItemContainerStyle.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedItemsControl source = d as HeaderedItemsControl;
            Style value = e.NewValue as Style;
            source.ItemContainerGenerator.UpdateItemContainerStyle(value);
        }
        #endregion public Style ItemContainerStyle

        /// <summary>
        /// Gets the ItemContainerGenerator that is associated with the 
        /// HeaderedItemsControl.
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HeaderedItemsControl class.
        /// </summary>
        public HeaderedItemsControl()
        {
            DefaultStyleKey = typeof(HeaderedItemsControl);
            ItemContainerGenerator = new ItemContainerGenerator(this);
        }

        /// <summary>
        /// Called when the value of the Header property changes.
        /// </summary>
        /// <param name="oldHeader">
        /// The old value of the Header property.
        /// </param>
        /// <param name="newHeader">
        /// The new value of the Header property.
        /// </param>
        /// <remarks>
        /// Override this method in a derived class to provide special 
        /// handling when the value of the Header property changes.
        /// </remarks>
        protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
        {
        }

        /// <summary>
        /// Called when the value of the HeaderTemplate property changes.
        /// </summary>
        /// <param name="oldHeaderTemplate">
        /// The old value of the HeaderTemplate property.
        /// </param>
        /// <param name="newHeaderTemplate">
        /// The new value of the HeaderTemplate property.
        /// </param>
        /// <remarks>
        /// Override this method in a derived class to provide 
        /// special handling when the value of the HeaderTemplate 
        /// property changes.
        /// </remarks>
        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
        }

        /// <summary>
        /// Builds the visual tree for the HeaderedItemsControl when 
        /// a new template is applied.
        /// </summary>
        /// <remarks>
        /// This method is invoked whenever application code or an 
        /// internal process, such as a rebuilding layout pass, 
        /// calls the ApplyTemplate method.
        /// </remarks>
        public override void OnApplyTemplate()
        {
            ItemContainerGenerator.OnApplyTemplate();
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">
        /// The container element used to display the specified item.
        /// </param>
        /// <param name="item">
        /// The content to display.
        /// </param>
        /// <remarks>
        /// Preparing the element may involve applying styles, setting bindings, 
        /// and so on.
        /// </remarks>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            ItemContainerGenerator.PrepareContainerForItemOverride(element, item, ItemContainerStyle);
            base.PrepareContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Removes any bindings and templates applied to the item container 
        /// for the specified content.
        /// </summary>
        /// <param name="element">
        /// The container element that is used to display the specified item.
        /// </param>
        /// <param name="item">
        /// The content to display.
        /// </param>
        /// <remarks>
        /// This method removes styles, binding, and other item-specific 
        /// effects that were applied to the container element to prepare 
        /// it to contain the specified content item. 
        /// </remarks>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            ItemContainerGenerator.ClearContainerForItemOverride(element, item);
            base.ClearContainerForItemOverride(element, item);
        }
    }
}