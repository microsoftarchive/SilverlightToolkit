// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// A static class providing methods for working with the visual tree.  
    /// </summary>
    internal static class VisualTreeExtensions
    {
        /// <summary>
        /// Gets or sets an identity integer that ensures that names are unique.
        /// </summary>
        private static long UniqueId { get; set; }

        /// <summary>
        /// Ensures that a framework element has a unique name.
        /// </summary>
        /// <param name="element">The element to set the name of.
        /// </param>
        private static void EnsureName(FrameworkElement element)
        {
            Debug.Assert(element != null, "The framework element cannot be null.");

            if (string.IsNullOrEmpty(element.Name))
            {
                string newName;
                do
                {
                    newName = string.Format(CultureInfo.InvariantCulture, "__UniqueId{0}__", UniqueId++);
                }
                while (element.FindName(newName) != null);

                element.SetValue(FrameworkElement.NameProperty, newName);
            }
        }

        /// <summary>
        /// Returns the visual tree ancestors of an element.
        /// </summary>
        /// <param name="element">The descendent of the ancestors.</param>
        /// <returns>The visual tree ancestors of the element.</returns>
        internal static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement element)
        {
            Debug.Assert(element != null, "element cannot be null.");

            DependencyObject parent = null;
            parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                FrameworkElement frameworkElementParent = parent as FrameworkElement;
                if (frameworkElementParent != null)
                {
                    yield return frameworkElementParent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        /// <summary>
        /// Retrieves all the logical children of a framework element using a 
        /// depth-first search.  A visual element is assumed to be a logical 
        /// child of another visual element if they are in the same namescope.
        /// For performance reasons this method manually manages the stack 
        /// instead of using recursion.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <returns>The logical children of the framework element.</returns>
        internal static IEnumerable<FrameworkElement> GetLogicalChildrenDepthFirst(this FrameworkElement parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null.");

            EnsureName(parent);

            Popup popup = parent as Popup;
            if (popup != null)
            {
                FrameworkElement popupChild = popup.Child as FrameworkElement;
                if (popupChild != null)
                {
                    yield return popupChild;
                }
            }

            // If control is an items control find the items host panel and 
            // return it if an ItemsPanel template is specified.  This works
            // around a bug which moves the items host into a different name
            // scope than the items control when an items template is provided.
            ItemsControl itemsControl = parent as ItemsControl;
            if (itemsControl != null && itemsControl.ItemsPanel != null)
            {
                ItemsPresenter itemsPresenter =
                    itemsControl
                        .GetVisualChildren()
                        .SelectMany(child =>
                            FunctionalProgramming.TraverseBreadthFirst(
                                child,
                                node => node.GetVisualChildren(),
                                node => !(node is ItemsControl)))
                        .OfType<ItemsPresenter>()
                        .FirstOrDefault();

                if (itemsPresenter != null)
                {
                    Panel itemsHost = itemsPresenter.GetVisualChildren().OfType<Panel>().FirstOrDefault();
                    if (itemsHost != null)
                    {
                        yield return itemsHost;
                    }
                }
            }

            string parentName = parent.Name;
            Stack<FrameworkElement> stack =
                new Stack<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());

            while (stack.Count > 0)
            {
                FrameworkElement element = stack.Pop();
                if (element.FindName(parentName) == parent || element is UserControl)
                {
                    yield return element;
                }
                else
                {
                    foreach (FrameworkElement visualChild in element.GetVisualChildren().OfType<FrameworkElement>())
                    {
                        stack.Push(visualChild);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all the visual children of a framework element.
        /// </summary>
        /// <param name="parent">The parent framework element.</param>
        /// <returns>The visual children of the framework element.</returns>
        internal static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
        {
            Debug.Assert(parent != null, "The parent cannot be null.");

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int counter = 0; counter < childCount; counter++)
            {
                yield return VisualTreeHelper.GetChild(parent, counter);
            }
        }
    }
}