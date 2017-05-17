//-----------------------------------------------------------------------
// <copyright file="EnumerableCollection.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.ObjectModel;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Collection used for unit tests that does not implement IList
    /// or INotifyCollectionChanged
    /// </summary>
    public sealed class EnumerableCollection<T> : IEnumerable
        where T : new()
    {
        private bool _canAdd = true;
        private bool _canRemove = true;
        private TestClass _editItem;
        private ObservableCollection<T> _list;

        /// <summary>
        /// Default constructor
        /// </summary>
        public EnumerableCollection()
        {
            _list = new ObservableCollection<T>();
        }

        /// <summary>
        /// Clear the list of all items
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Inserts the provided item to the collection
        /// </summary>
        /// <param name="index">Index to insert into</param>
        /// <param name="item">Item to add</param>
        public void Insert(int index, object item)
        {
            _list.Insert(index, (T)item);
        }

        /// <summary>
        /// Gets the number of items we have
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        #region IEnumerable Members

        /// <summary>
        /// Gets the IEnumerator for the collection
        /// </summary>
        /// <returns>The IEnumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

        #endregion

        #region IEditableCollection<T> Members

        /// <summary>
        /// Adds the item into our collection
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Add(T item)
        {
            this._list.Add(item);
        }

        /// <summary>
        /// Start editing the item
        /// </summary>
        /// <param name="item"></param>
        public void BeginEdit(T item)
        {
            _editItem = new TestClass();

            TestClass editItem = item as TestClass;
            if (editItem != null)
            {
                _editItem.IntProperty = editItem.IntProperty;
                _editItem.StringProperty = editItem.StringProperty;
            }
        }

        /// <summary>
        /// Cancel the edit on the item
        /// </summary>
        /// <param name="item"></param>
        public void CancelEdit(T item)
        {
            TestClass editItem = item as TestClass;
            if (editItem != null)
            {
                editItem.IntProperty = _editItem.IntProperty;
                editItem.StringProperty = _editItem.StringProperty;
            }

            _editItem = null;
        }

        /// <summary>
        /// Create a new instance of type T
        /// </summary>
        /// <returns></returns>
        public T CreateNew()
        {
            return new T();
        }

        /// <summary>
        /// End editing the item
        /// </summary>
        /// <param name="item"></param>
        public void EndEdit(T item)
        {
            _editItem = null;
        }

        /// <summary>
        /// Removes the item from our collection
        /// </summary>
        /// <param name="item">Item to remove</param>
        public void Remove(T item)
        {
            this._list.Remove(item);
        }

        #endregion

        #region IEditableCollection Members

        /// <summary>
        /// Whether this collection supports adding items
        /// </summary>
        public bool CanAdd
        {
            get { return _canAdd; }
            set { _canAdd = value; }
        }

        /// <summary>
        /// Whether this collection supports editing items
        /// </summary>
        public bool CanEdit
        {
            get { return true; }
        }

        /// <summary>
        /// Whether this collection supports removing items
        /// </summary>
        public bool CanRemove
        {
            get { return _canRemove; }
            set { _canRemove = value; }
        }

        /// <summary>
        /// Adds the item into our collection
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Add(object item)
        {
            this._list.Add((T)item);
        }

        /// <summary>
        /// Starts editing an item
        /// </summary>
        /// <param name="item"></param>
        public void BeginEdit(object item)
        {
            _editItem = new TestClass();

            TestClass editItem = item as TestClass;
            if (editItem != null)
            {
                _editItem.IntProperty = editItem.IntProperty;
                _editItem.StringProperty = editItem.StringProperty;
            }
        }

        public void CancelEdit(object item)
        {
            TestClass editItem = item as TestClass;
            if (editItem != null)
            {
                editItem.IntProperty = _editItem.IntProperty;
                editItem.StringProperty = _editItem.StringProperty;
            }

            _editItem = null;
        }

        public void EndEdit(object item)
        {
            _editItem = null;
        }

        /// <summary>
        /// Removes the item from our collection
        /// </summary>
        /// <param name="item">Item to remove</param>
        public void Remove(object item)
        {
            this._list.Remove((T)item);
        }

        /// <summary>
        /// Removes the item at the specified index from 
        /// our collection
        /// </summary>
        /// <param name="index">Index of item to remove</param>
        public void RemoveAt(int index)
        {
            this._list.RemoveAt(index);
        }

        #endregion
    }
}
