// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides useful extensions to ItemsControl instances.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public static class ItemsControlExtensions
    {
        /// <summary>
        /// Gets the Panel that contains the containers of an ItemsControl.
        /// </summary>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>
        /// The Panel that contains the containers of an ItemsControl, or null
        /// if the Panel could not be found.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        public static Panel GetItemsHost(this ItemsControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            // Get the first live container
            DependencyObject container = control.ItemContainerGenerator.ContainerFromIndex(0);
            
            // Get the parent of the container
            return (container != null) ?
                VisualTreeHelper.GetParent(container) as Panel :
                null;
        }

        /// <summary>
        /// Gets the ScrollViewer that contains the containers of an
        /// ItemsControl.
        /// </summary>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>
        /// The ScrollViewer that contains the containers of an ItemsControl, or
        /// null if a ScrollViewer could not be found.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        public static ScrollViewer GetScrollHost(this ItemsControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            Panel itemsHost = GetItemsHost(control);
            if (itemsHost == null)
            {
                return null;
            }

            // Walk up the visual tree from the ItemsHost to the
            // ItemsControl looking for a ScrollViewer that wraps
            // the ItemsHost.
            return itemsHost
                .GetVisualAncestors()
                .Where(c => c != control)
                .OfType<ScrollViewer>()
                .FirstOrDefault();
        }

        #region GetContainers
        /// <summary>
        /// Get the item containers of an ItemsControl.
        /// </summary>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The item containers of an ItemsControl.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        public static IEnumerable<DependencyObject> GetContainers(this ItemsControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            return GetContainersIterator<DependencyObject>(control);
        }

        /// <summary>
        /// Get the item containers of an ItemsControl.
        /// </summary>
        /// <typeparam name="TContainer">
        /// The type of the item containers.
        /// </typeparam>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The item containers of an ItemsControl.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Common pattern for extensions that cast.")]
        public static IEnumerable<TContainer> GetContainers<TContainer>(this ItemsControl control)
            where TContainer : DependencyObject
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            return GetContainersIterator<TContainer>(control);
        }

        /// <summary>
        /// Get the item containers of an ItemsControl.
        /// </summary>
        /// <typeparam name="TContainer">
        /// The type of the item containers.
        /// </typeparam>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The item containers of an ItemsControl.</returns>
        private static IEnumerable<TContainer> GetContainersIterator<TContainer>(ItemsControl control)
            where TContainer : DependencyObject
        {
            Debug.Assert(control != null, "control should not be null!");
            return control.GetItemsAndContainers<TContainer>().Select(p => p.Value);
        }
        #endregion GetContainers

        #region GetItemsAndContainers
        /// <summary>
        /// Get the items and item containers of an ItemsControl.
        /// </summary>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The items and item containers of an ItemsControl.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        public static IEnumerable<KeyValuePair<object, DependencyObject>> GetItemsAndContainers(this ItemsControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            return GetItemsAndContainersIterator<DependencyObject>(control);
        }

        /// <summary>
        /// Get the items and item containers of an ItemsControl.
        /// </summary>
        /// <typeparam name="TContainer">
        /// The type of the item containers.
        /// </typeparam>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The items and item containers of an ItemsControl.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Returns a generic type.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using a sequence of pairs.")]
        public static IEnumerable<KeyValuePair<object, TContainer>> GetItemsAndContainers<TContainer>(this ItemsControl control)
            where TContainer : DependencyObject
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            return GetItemsAndContainersIterator<TContainer>(control);
        }

        /// <summary>
        /// Get the items and item containers of an ItemsControl.
        /// </summary>
        /// <typeparam name="TContainer">
        /// The type of the item containers.
        /// </typeparam>
        /// <param name="control">The ItemsControl.</param>
        /// <returns>The items and item containers of an ItemsControl.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control" /> is null.
        /// </exception>
        private static IEnumerable<KeyValuePair<object, TContainer>> GetItemsAndContainersIterator<TContainer>(ItemsControl control)
            where TContainer : DependencyObject
        {
            Debug.Assert(control != null, "control should not be null!");

            int count = control.Items.Count;
            for (int i = 0; i < count; i++)
            {
                DependencyObject container = control.ItemContainerGenerator.ContainerFromIndex(i);
                if (container == null)
                {
                    continue;
                }

                yield return new KeyValuePair<object, TContainer>(
                    control.Items[i],
                    container as TContainer);
            }
        }
        #endregion GetItemsAndContainers
    }
}