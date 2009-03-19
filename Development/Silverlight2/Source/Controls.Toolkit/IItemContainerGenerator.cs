// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls
{
    /// <summary>
    /// IItemContainerGenerator provides useful utilities for mapping between
    /// the items of an ItemsControl and their generated containers.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public interface IItemContainerGenerator
    {
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
        DependencyObject ContainerFromIndex(int index);

        /// <summary>
        /// Returns the container corresponding to the given item.
        /// </summary>
        /// <param name="item">The item to find the container for.</param>
        /// <returns>
        /// A container that corresponds to the given item.  Returns null if the
        /// item does not belong to the item collection, or if a container has
        /// not been generated for it.
        /// </returns>
        DependencyObject ContainerFromItem(object item);

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
        int IndexFromContainer(DependencyObject container);

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
        object ItemFromContainer(DependencyObject container);
    }
}