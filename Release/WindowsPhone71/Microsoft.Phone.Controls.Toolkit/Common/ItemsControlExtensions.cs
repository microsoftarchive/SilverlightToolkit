// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Provides helper methods to work with ItemsControl.
    /// </summary>
    internal static class ItemsControlExtensions
    {
        /// <summary>
        /// Gets the parent ItemsControl.
        /// </summary>
        /// <typeparam name="T">The type of ItemsControl.</typeparam>
        /// <param name="element">The dependency object </param>
        /// <returns>
        /// The parent ItemsControl or null if there is not.
        /// </returns>
        public static T GetParentItemsControl<T>(DependencyObject element) 
            where T : ItemsControl
        {
            var parent = VisualTreeHelper.GetParent(element);

            while (!(parent is T) && (parent != null))
            {
                parent = VisualTreeHelper.GetParent(parent as DependencyObject);
            }

            return (T)parent;
        }

        /// <summary>
        /// Gets the items that are currently in the view port
        /// of an ItemsControl with a ScrollViewer.
        /// </summary>
        /// <param name="list">The ItemsControl to search on.</param>
        /// <returns>
        /// A collection of weak references to the items in the view port.
        /// </returns>
        public static IList<WeakReference> GetItemsInViewPort(ItemsControl list)
        {
            int index;
            FrameworkElement container;
            GeneralTransform itemTransform;
            Rect boundingBox;
            IList<WeakReference> viewPortItems = new List<WeakReference>();            
            ScrollViewer scrollHost = VisualTreeHelper.GetChild(list, 0) as ScrollViewer;

            list.UpdateLayout();
            
            if (scrollHost == null)
            {
                return viewPortItems;
            }

            for (index = 0; index < list.Items.Count; index++)
            {
                container = (FrameworkElement)list.ItemContainerGenerator.ContainerFromIndex(index);
                if (container != null)
                {
                    itemTransform = null;
                    try
                    {
                        itemTransform = container.TransformToVisual(scrollHost);
                    }
                    catch (ArgumentException)
                    {
                        // Ignore failures when not in the visual tree
                        return viewPortItems;
                    }

                    boundingBox = new Rect(itemTransform.Transform(new Point()), itemTransform.Transform(new Point(container.ActualWidth, container.ActualHeight)));

                    if (boundingBox.Bottom > 0)
                    {
                        viewPortItems.Add(new WeakReference(container));
                        index++;
                        break;
                    }
                }
            }

            for (; index < list.Items.Count; index++)
            {
                container = (FrameworkElement)list.ItemContainerGenerator.ContainerFromIndex(index);
                itemTransform = null;
                try
                {
                    itemTransform = container.TransformToVisual(scrollHost);
                }
                catch (ArgumentException)
                {
                    // Ignore failures when not in the visual tree
                    return viewPortItems;
                }
                
                boundingBox = new Rect(itemTransform.Transform(new Point()), itemTransform.Transform(new Point(container.ActualWidth, container.ActualHeight)));

                if (boundingBox.Top < scrollHost.ActualHeight)
                {
                    viewPortItems.Add(new WeakReference(container));
                }
                else
                {
                    break;
                }
            }

            return viewPortItems;
        }
    }
}
