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
using System.Collections.Generic;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides an interface for collections with an indexer and Count property.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public interface IIndexableCollection : IEnumerable
    {
        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to return.</param>
        /// <returns>The element at the specified index.</returns>
        object this[int index]
        {
            get;
        }

        /// <summary>
        /// Gets the number of elements in this collection.
        /// </summary>
        int Count
        {
            get;
        }
    }

    /// <summary>
    /// Provides an interface for collections with an indexer and Count property.
    /// </summary>
    /// <typeparam name="T">The element type of the collection.</typeparam>
    /// <QualityBand>Preview</QualityBand>
    public interface IIndexableCollection<T> : IIndexableCollection, IEnumerable<T>
    {
        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element to return.</param>
        /// <returns>The element at the specified index.</returns>
        new T this[int index]
        {
            get;
        }
    }
}
