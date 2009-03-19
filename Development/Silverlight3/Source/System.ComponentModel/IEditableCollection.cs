//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;

namespace System.ComponentModel
{
    /// <summary>
    /// Exposes the basic collection operations of a collection type. 
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public interface IEditableCollection
    {
        /// <summary>
        /// Gets a value indicating whether the collection allows new items to be added
        /// </summary>
        bool CanAdd
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether members of the collection can be edited
        /// </summary>
        bool CanEdit
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the collection allows items to be removed.
        /// Note that newly added items can always be removed.
        /// </summary>
        bool CanRemove
        {
            get;
        }

        /// <summary>
        /// Add a new item to the collection. If CanAdd is false, an <see cref="InvalidOperationException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="item">The item to add</param>
        void Add(object item);

        /// <summary>
        /// Begin editing the specified item. If the item implements <see cref="IEditableObject"/>
        /// then this method should delegate to that implementation. If CanEdit is false, 
        /// an <see cref="InvalidOperationException"/> will be thrown.
        /// </summary>
        /// <param name="item">The item to begin editing</param>
        void BeginEdit(object item);

        /// <summary>
        /// Cancel editing on the specified item, reverting the changes. If 
        /// the item implements <see cref="IEditableObject"/>
        /// then this method should delegate to that implementation.
        /// </summary>
        /// <param name="item">The item to cancel edits for</param>
        void CancelEdit(object item);

        /// <summary>
        /// Create and return a new instance of the item type of the collection.
        /// The returned instance is not added to the collection automatically.
        /// </summary>
        /// <returns>The new instance</returns>
        object CreateNew();

        /// <summary>
        /// End editing on the specified item, committing the changes. If 
        /// the item implements <see cref="IEditableObject"/>
        /// then this method should delegate to that implementation.
        /// </summary>
        /// <param name="item"></param>
        void EndEdit(object item);

        /// <summary>
        /// Remove the specified item from the collection. If CanRemove is false
        /// and the specified item is not newly added, an <see cref="InvalidOperationException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="item">The item to remove</param>
        void Remove(object item);
    }

    /// <summary>
    /// Exposes the basic collection operations of a collection type. 
    /// </summary>
    /// <typeparam name="T">The item type of the collection</typeparam>
    /// <QualityBand>Preview</QualityBand>
    public interface IEditableCollection<T> : IEditableCollection
    {
        /// <summary>
        /// Gets a value indicating whether the collection allows new items to be added
        /// </summary>
        bool CanAdd
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether members of the collection can be edited
        /// </summary>
        bool CanEdit
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the collection allows items to be removed.
        /// Note that newly added items can always be removed.
        /// </summary>
        bool CanRemove
        {
            get;
        }

        /// <summary>
        /// Add a new item to the collection. If CanAdd is false, an <see cref="InvalidOperationException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="item">The item to add</param>
        void Add(T item);

        /// <summary>
        /// Begin editing the specified item. If the item implements <see cref="IEditableObject"/>
        /// then this method should delegate to that implementation. If CanEdit is false, 
        /// an <see cref="InvalidOperationException"/> will be thrown.
        /// </summary>
        /// <param name="item">The item to begin editing</param>
        void BeginEdit(T item);

        /// <summary>
        /// Cancel editing on the specified item, reverting the changes. If 
        /// the item implements <see cref="IEditableObject"/>
        /// then this method should delegate to that implementation.
        /// </summary>
        /// <param name="item">The item to cancel edits for</param>
        void CancelEdit(T item);

        /// <summary>
        /// Create and return a new instance of the item type of the collection.
        /// The returned instance is not added to the collection automatically.
        /// </summary>
        /// <returns>The new instance</returns>
        T CreateNew();

        /// <summary>
        /// End editing on the specified item, committing the changes. If 
        /// the item implements <see cref="IEditableObject"/>
        /// then this method should delegate to that implementation.
        /// </summary>
        /// <param name="item"></param>
        void EndEdit(T item);

        /// <summary>
        /// Remove the specified item from the collection. If CanRemove is false
        /// and the specified item is not newly added, an <see cref="InvalidOperationException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="item">The item to remove</param>
        void Remove(T item);
    }
}
