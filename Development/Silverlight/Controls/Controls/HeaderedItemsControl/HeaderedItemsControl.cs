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

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Represents a control that contains multiple items and have a header.
    /// </summary>
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
        /// Gets or sets an object that labels the HeaderedItemsControl.  The
        /// default value is null.  The header can be a string, UIElement, or
        /// any other content.
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
        /// Gets or sets a data template used to display a control's header. The
        /// default is null.
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
        /// Gets the ItemContainerGenerator that is associated with this
        /// control.
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
        /// Called when the Header property of a HeaderedItemsControl changes.
        /// </summary>
        /// <param name="oldHeader">
        /// The old value of the Header property.
        /// </param>
        /// <param name="newHeader">
        /// The new value of the Header property.
        /// </param>
        protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
        {
        }

        /// <summary>
        /// Called when the HeaderTemplate property changes.
        /// </summary>
        /// <param name="oldHeaderTemplate">
        /// The old value of the HeaderTemplate property.
        /// </param>
        /// <param name="newHeaderTemplate">
        /// The new value of the HeaderTemplate property.
        /// </param>
        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
        }

        /// <summary>
        /// Apply a control template to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            ItemContainerGenerator.OnApplyTemplate();
            base.OnApplyTemplate();
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
            ItemContainerGenerator.ClearContainerForItemOverride(element, item);
            base.ClearContainerForItemOverride(element, item);
        }
    }
}