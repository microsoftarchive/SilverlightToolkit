//-----------------------------------------------------------------------
// <copyright file="TestEditableCollection.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// IEditableCollection implementation used for unit testing
    /// </summary>
    /// <typeparam name="T">Newable type</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface", Justification = "Test Class")]
    public class TestEditableCollection<T> : ObservableCollection<T>, IEditableCollection<T> where T : new()
    {
        /// <summary>
        /// Copy of the item we are editing in the case of an item that does not implement IEditableObject
        /// </summary>
        private TestClass _editItem;

        /// <summary>
        /// Initializes a new instance of the TestEditableCollection class.
        /// </summary>
        public TestEditableCollection()
        {
        }

        #region IEditableCollection Members

        /// <summary>
        /// Gets a value indicating whether we can add a new item
        /// </summary>
        public bool CanAdd
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether we can edit an existing item
        /// </summary>
        public bool CanEdit
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether we can remove an existing item
        /// </summary>
        public bool CanRemove
        {
            get { return true; }
        }

        /// <summary>
        /// Adds the provided item to the collection
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Add(object item)
        {
            this.Add((T)item);
        }

        /// <summary>
        /// Enters editing mode for the provided item
        /// </summary>
        /// <param name="item">Item to edit</param>
        public void BeginEdit(object item)
        {
            this.BeginEdit((T)item);
        }

        /// <summary>
        /// Cancels editing mode for the provided item
        /// </summary>
        /// <param name="item">Item to cancel</param>
        public void CancelEdit(object item)
        {
            this.CancelEdit((T)item);
        }

        /// <summary>
        /// Creates a new item for future addition
        /// </summary>
        /// <returns>Created item</returns>
        object IEditableCollection.CreateNew()
        {
            return new T();
        }

        /// <summary>
        /// Commits the edits for the provided item
        /// </summary>
        /// <param name="item">Item to commit</param>
        public void EndEdit(object item)
        {
            this.EndEdit((T)item);
        }

        /// <summary>
        /// Removes the provided item from the collection
        /// </summary>
        /// <param name="item">Item to remove</param>
        public void Remove(object item)
        {
            this.Remove((T)item);
        }

        #endregion IEditableCollection Members

        #region IEditableCollection<T> Members

        /// <summary>
        /// Adds the provided item to the collection
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Add(T item)
        {
            this.InsertItem(this.Count, item);
        }

        /// <summary>
        /// Enters editing mode for the provided item
        /// </summary>
        /// <param name="item">Item to edit</param>
        public void BeginEdit(T item)
        {
            IEditableObject ieo = item as IEditableObject;
            if (ieo != null)
            {
                ieo.BeginEdit();
            }
            else
            {
                TestClass tc = item as TestClass;
                this._editItem = new TestClass() { IntProperty = tc.IntProperty, StringProperty = tc.StringProperty };
            }
        }

        /// <summary>
        /// Cancels editing mode for the provided item
        /// </summary>
        /// <param name="item">Item to cancel</param>
        public void CancelEdit(T item)
        {
            IEditableObject ieo = item as IEditableObject;
            if (ieo != null)
            {
                ieo.CancelEdit();
            }
            else
            {
                TestClass tc = item as TestClass;
                tc.IntProperty = this._editItem.IntProperty;
                tc.StringProperty = this._editItem.StringProperty;
            }
        }

        /// <summary>
        /// Creates a new item for future addition
        /// </summary>
        /// <returns>Created item</returns>
        T IEditableCollection<T>.CreateNew()
        {
            return new T();
        }

        /// <summary>
        /// Commits the edits for the provided item
        /// </summary>
        /// <param name="item">Item to commit</param>
        public void EndEdit(T item)
        {
            IEditableObject ieo = item as IEditableObject;
            if (ieo != null)
            {
                ieo.EndEdit();
            }
            else
            {
                this._editItem = null;
            }
        }

        /// <summary>
        /// Removes the provided item from the collection
        /// </summary>
        /// <param name="item">Item to remove</param>
        public void Remove(T item)
        {
            this.RemoveItem(this.IndexOf(item));
        }

        #endregion IEditableCollection<T> Members
    }
}
