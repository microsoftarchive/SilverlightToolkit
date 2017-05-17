//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Media;

namespace System.Windows.Controls.UnitTests
{
    internal static class DependencyObjectExtensions
    {
        #region Methods

        /// <summary>
        /// Returns an array containing the DependencyObject children.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject to return children of.</param>
        /// <returns>An array of DependencyObjects.</returns>
        internal static DependencyObject[] GetCurrentChildren(this DependencyObject dependencyObject)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(dependencyObject);
            DependencyObject[] children = new DependencyObject[childCount];

            for (int i = 0; i < childCount; ++i)
            {
                children[i] = VisualTreeHelper.GetChild(dependencyObject, i);
            }

            return children;
        }

        /// <summary>
        /// Attempts to find a child DependencyObject by name.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject to search.</param>
        /// <param name="name">Name of the child element to find.</param>
        /// <returns>A child DependencyObject with the provided name or null if not found.</returns>
        internal static DependencyObject FindChildByName(this DependencyObject dependencyObject, string name)
        {
            Queue<DependencyObject> queue = new Queue<DependencyObject>();

            // Enqueue starting point
            queue.Enqueue(dependencyObject);

            // Do a BFS search on children
            while (queue.Count > 0)
            {
                DependencyObject node = queue.Dequeue();

                // Check for a match
                FrameworkElement frameworkElement = node as FrameworkElement;
                if (frameworkElement != null)
                {
                    if (frameworkElement.Name.Equals(name, StringComparison.Ordinal))
                    {
                        return frameworkElement;
                    }
                }

                // Add children
                foreach (DependencyObject child in node.GetCurrentChildren())
                {
                    queue.Enqueue(child);
                }
            }

            return null;
        }

        #endregion
    }
}
