// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// The ItemContainerGenerator provides useful utilities for mapping between
    /// the items of an ItemsControl and their generated containers.
    /// </summary>
    /// <remarks>
    /// WPF actually uses the ItemContainerGenerator to create all of the
    /// containers used by an ItemsControl.  The container generation in
    /// Silverlight happens in the native implementation of ItemsControl, so
    /// this ItemContainerGenerator serves only to expose the mapping between
    /// items and containers in a subset of the WPF API.
    /// </remarks>
    /// <QualityBand>Preview</QualityBand>
    public sealed partial class ItemContainerGenerator
    {
        /// <summary>
        /// Gets or sets the ItemsControl being tracked by the
        /// ItemContainerGenerator.
        /// </summary>
        private ItemsControl ItemsControl { get; set; }

        /// <summary>
        /// Gets or sets a dictionary that maps containers to the items they
        /// display.
        /// </summary>
        private IDictionary<DependencyObject, object> ContainersToItems { get; set; }

        /// <summary>
        /// A Panel that is used as the ItemsHost of the ItemsControl.  This
        /// property will only be valid when the ItemsControl is live in the
        /// tree and has generated containers for some of its items.
        /// </summary>
        private Panel _itemsHost;

        /// <summary>
        /// Gets a Panel that is used as the ItemsHost of the ItemsControl.
        /// This property will only be valid when the ItemsControl is live in
        /// the tree and has generated containers for some of its items.
        /// </summary>
        internal Panel ItemsHost
        {
            get
            {
                // Lookup the ItemsHost if we haven't already cached it.
                if (_itemsHost == null)
                {
                    // Get any live container
                    if (ContainersToItems.Count <= 0)
                    {
                        return null;
                    }
                    DependencyObject container = ContainersToItems.First().Key;

                    // Get the parent of the container
                    _itemsHost = VisualTreeHelper.GetParent(container) as Panel;
                }

                return _itemsHost;
            }
        }

        /// <summary>
        /// A ScrollViewer that is used to scroll the items in the ItemsHost.
        /// </summary>
        private ScrollViewer _scrollHost;

        /// <summary>
        /// Gets a ScrollViewer that is used to scroll the items in the
        /// ItemsHost.
        /// </summary>
        internal ScrollViewer ScrollHost
        {
            get
            {
                if (_scrollHost == null)
                {
                    Panel itemsHost = ItemsHost;
                    if (itemsHost != null)
                    {
                        for (DependencyObject obj = itemsHost; obj != ItemsControl && obj != null; obj = VisualTreeHelper.GetParent(obj))
                        {
                            ScrollViewer viewer = obj as ScrollViewer;
                            if (viewer != null)
                            {
                                _scrollHost = viewer;
                                break;
                            }
                        }
                    }
                }
                return _scrollHost;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ItemContainerGenerator.
        /// </summary>
        /// <param name="control">
        /// The ItemsControl being tracked by the ItemContainerGenerator.
        /// </param>
        internal ItemContainerGenerator(ItemsControl control)
        {
            Debug.Assert(control != null, "control cannot be null!");
            ItemsControl = control;
            ContainersToItems = new Dictionary<DependencyObject, object>();
        }

        /// <summary>
        /// Apply a control template to the ItemsControl.
        /// </summary>
        internal void OnApplyTemplate()
        {
            // Clear the cached ItemsHost, ScrollHost, and containers
            _itemsHost = null;
            _scrollHost = null;
            ContainersToItems.Clear();
        }

        #region PrepareContainer
        /// <summary>
        /// Prepares the specified container to display the specified item.
        /// </summary>
        /// <param name="element">
        /// Container element used to display the specified item.
        /// </param>
        /// <param name="item">Specified item to display.</param>
        /// <param name="parentItemContainerStyle">
        /// The ItemContainerStyle for the parent ItemsControl.
        /// </param>
        internal void PrepareContainerForItemOverride(DependencyObject element, object item, Style parentItemContainerStyle)
        {
            // Associate the container with the item it contains
            ContainersToItems[element] = item;

            // Apply the ItemContainerStyle to the item
            Control control = element as Control;
            if (parentItemContainerStyle != null && control != null && control.Style == null)
            {
                control.SetValue(Control.StyleProperty, parentItemContainerStyle);
            }

            // Note: WPF also does preparation for ContentPresenter,
            // ContentControl, HeaderedContentControl, and ItemsControl.  Since
            // we don't have any other ItemsControls using this
            // ItemContainerGenerator, we've removed that code for now.  It
            // should be added back later when necessary.

            HeaderedItemsControl headeredItemsControl = element as HeaderedItemsControl;
            if (headeredItemsControl != null)
            {
                PrepareHeaderedItemsControlContainer(headeredItemsControl, item, ItemsControl, parentItemContainerStyle);
            }
        }

        /// <summary>
        /// Check whether a control has the default value for a property.
        /// </summary>
        /// <param name="control">The control to check.</param>
        /// <param name="property">The property to check.</param>
        /// <returns>
        /// True if the property has the default value; false otherwise.
        /// </returns>
        private static bool HasDefaultValue(Control control, DependencyProperty property)
        {
            Debug.Assert(control != null, "control should not be null!");
            Debug.Assert(property != null, "property should not be null!");
            return control.ReadLocalValue(property) == DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Prepare a PrepareHeaderedItemsControlContainer container for an
        /// item.
        /// </summary>
        /// <param name="control">Container to prepare.</param>
        /// <param name="item">Item to be placed in the container.</param>
        /// <param name="parentItemsControl">The parent ItemsControl.</param>
        /// <param name="parentItemContainerStyle">
        /// The ItemContainerStyle for the parent ItemsControl.
        /// </param>
        private static void PrepareHeaderedItemsControlContainer(HeaderedItemsControl control, object item, ItemsControl parentItemsControl, Style parentItemContainerStyle)
        {
            if (control != item)
            {
                // Copy the ItemsControl properties from parent to child
                DataTemplate parentItemTemplate = parentItemsControl.ItemTemplate;
                if (parentItemTemplate != null)
                {
                    control.SetValue(HeaderedItemsControl.ItemTemplateProperty, parentItemTemplate);
                }
                if (parentItemContainerStyle != null && HasDefaultValue(control, HeaderedItemsControl.ItemContainerStyleProperty))
                {
                    control.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, parentItemContainerStyle);
                }

                // Copy the Header properties from parent to child
                if (control.HeaderIsItem || HasDefaultValue(control, HeaderedItemsControl.HeaderProperty))
                {
                    control.Header = item;
                    control.HeaderIsItem = true;
                }
                if (parentItemTemplate != null)
                {
                    control.SetValue(HeaderedItemsControl.HeaderTemplateProperty, parentItemTemplate);
                }
                if (parentItemContainerStyle != null && control.Style == null)
                {
                    control.SetValue(HeaderedItemsControl.StyleProperty, parentItemContainerStyle);
                }

                // Note: this is where we would apply the HeaderTemplateSelector
                // (if implemented) or attempt to lookup the implicit template
                // for the type of the item if the headerTemplate were null.

                // Setup a hierarchical template
                HierarchicalDataTemplate headerTemplate = parentItemTemplate as HierarchicalDataTemplate;
                if (headerTemplate != null)
                {
                    if (headerTemplate.ItemsSource != null && HasDefaultValue(control, HeaderedItemsControl.ItemsSourceProperty))
                    {
                        control.SetBinding(
                            HeaderedItemsControl.ItemsSourceProperty,
                            new Binding
                            {
                                Converter = headerTemplate.ItemsSource.Converter,
                                ConverterCulture = headerTemplate.ItemsSource.ConverterCulture,
                                ConverterParameter = headerTemplate.ItemsSource.ConverterParameter,
                                Mode = headerTemplate.ItemsSource.Mode,
                                NotifyOnValidationError = headerTemplate.ItemsSource.NotifyOnValidationError,
                                Path = headerTemplate.ItemsSource.Path,
                                Source = control.Header,
                                ValidatesOnExceptions = headerTemplate.ItemsSource.ValidatesOnExceptions
                            });
                    }
                    if (headerTemplate.IsItemTemplateSet && control.ItemTemplate == parentItemTemplate)
                    {
                        control.ClearValue(HeaderedItemsControl.ItemTemplateProperty);
                        if (headerTemplate.ItemTemplate != null)
                        {
                            control.ItemTemplate = headerTemplate.ItemTemplate;
                        }
                    }
                    if (headerTemplate.IsItemContainerStyleSet && control.ItemContainerStyle == parentItemContainerStyle)
                    {
                        control.ClearValue(HeaderedItemsControl.ItemContainerStyleProperty);
                        if (headerTemplate.ItemContainerStyle != null)
                        {
                            control.ItemContainerStyle = headerTemplate.ItemContainerStyle;
                        }
                    }
                }
            }
        }
        #endregion PrepareContainer

        /// <summary>
        /// Undoes the effects of PrepareContainerForItemOverride.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The contained item.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "item", Justification = "Matching ItemsControl signature.")]
        internal void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            ContainersToItems.Remove(element);
        }

        /// <summary>
        /// Update the style of any generated items when the ItemContainerStyle
        /// has been changed.
        /// </summary>
        /// <param name="itemContainerStyle">The ItemContainerStyle.</param>
        /// <remarks>
        /// Silverlight does not support setting a Style multiple times, so we
        /// only attempt to set styles on elements whose style hasn't already
        /// been set.
        /// </remarks>
        internal void UpdateItemContainerStyle(Style itemContainerStyle)
        {
            if (itemContainerStyle == null)
            {
                return;
            }

            Panel itemsHost = ItemsHost;
            if (itemsHost == null || itemsHost.Children == null)
            {
                return;
            }

            foreach (UIElement element in itemsHost.Children)
            {
                FrameworkElement obj = element as FrameworkElement;
                if (obj.Style == null)
                {
                    obj.Style = itemContainerStyle;
                }
            }
        }

        /// <summary>
        /// Returns the element corresponding to the item at the given index
        /// within the ItemCollection.
        /// </summary>
        /// <param name="index">The index of the desired item.</param>
        /// <returns>
        /// Returns the element corresponding to the item at the given index
        /// within the ItemCollection or returns null if the item is not
        /// realized.
        /// </returns>
        public DependencyObject ContainerFromIndex(int index)
        {
            Panel host = ItemsHost;
            if (host == null || host.Children == null || index < 0 || index >= host.Children.Count)
            {
                return null;
            }

            return host.Children[index];
        }

        /// <summary>
        /// Returns the container corresponding to the given item.
        /// </summary>
        /// <param name="item">The item to find the container for.</param>
        /// <returns>
        /// A container that corresponds to the given item.  Returns null if the
        /// item does not belong to the item collection, or if a container has
        /// not been generated for it.
        /// </returns>
        public DependencyObject ContainerFromItem(object item)
        {
            // TODO: Consider replacing dictionary with a list so that search
            // will be consistent regardless of dictionary entries
            foreach (KeyValuePair<DependencyObject, object> mapping in ContainersToItems)
            {
                if (object.Equals(item, mapping.Value))
                {
                    return mapping.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the index to an item that corresponds to the specified,
        /// generated container. 
        /// </summary>
        /// <param name="container">
        /// The DependencyObject that corresponds to the item to the index to be
        /// returned.
        /// </param>
        /// <returns>
        /// An Int32 index to an item that corresponds to the specified,
        /// generated container.
        /// </returns>
        public int IndexFromContainer(DependencyObject container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            UIElement element = container as UIElement;
            if (element == null)
            {
                return -1;
            }

            Panel host = ItemsHost;
            if (host == null || host.Children == null)
            {
                return -1;
            }

            return host.Children.IndexOf(element);
        }

        /// <summary>
        /// Returns the item that corresponds to the specified, generated
        /// container.
        /// </summary>
        /// <param name="container">
        /// The DependencyObject that corresponds to the item to be returned.
        /// </param>
        /// <returns>
        /// The contained item, or the container if it had no associated item.
        /// </returns>
        public object ItemFromContainer(DependencyObject container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            object item = container;
            ContainersToItems.TryGetValue(container, out item);
            return item;
        }

        /// <summary>
        /// Scroll the desired element into the ScrollHost's viewport.
        /// </summary>
        /// <param name="element">Element to scroll into view.</param>
        internal void ScrollIntoView(FrameworkElement element)
        {
            // Get the ScrollHost
            ScrollViewer scrollHost = ScrollHost;
            if (scrollHost == null)
            {
                return;
            }

            // Get the position of the element relative to the ScrollHost
            GeneralTransform transform = null;
            try
            {
                transform = element.TransformToVisual(scrollHost);
            }
            catch (ArgumentException)
            {
                // Ignore failures when not in the visual tree
                return;
            }
            Rect itemRect = new Rect(
                transform.Transform(new Point()),
                transform.Transform(new Point(element.ActualWidth, element.ActualHeight)));

            // Scroll vertically
            double verticalOffset = scrollHost.VerticalOffset;
            double verticalDelta = 0;
            double hostBottom = scrollHost.ViewportHeight;
            double itemBottom = itemRect.Bottom;
            if (hostBottom < itemBottom)
            {
                verticalDelta = itemBottom - hostBottom;
                verticalOffset += verticalDelta;
            }
            double itemTop = itemRect.Top;
            if (itemTop - verticalDelta < 0)
            {
                verticalOffset -= verticalDelta - itemTop;
            }
            scrollHost.ScrollToVerticalOffset(verticalOffset);

            // Scroll horizontally
            double horizontalOffset = scrollHost.HorizontalOffset;
            double horizontalDelta = 0;
            double hostRight = scrollHost.ViewportWidth;
            double itemRight = itemRect.Right;
            if (hostRight < itemRight)
            {
                horizontalDelta = itemBottom - hostBottom;
                horizontalOffset += horizontalDelta;
            }
            double itemLeft = itemRect.Top;
            if (itemTop - horizontalDelta < 0)
            {
                horizontalOffset -= horizontalDelta - itemLeft;
            }
            scrollHost.ScrollToHorizontalOffset(horizontalOffset);
        }
    }
}