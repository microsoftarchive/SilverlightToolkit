//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataClassEntityList.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// A wrapper for a list of data classes.
    /// </summary>
    public class DataClassList : IList
    {
        /// <summary>
        /// The data class list.
        /// </summary>
        private List<DataClass> dataClassList;

        /// <summary>
        /// The supported operations on the list.
        /// </summary>
        private ListOperations supportedOperations;

        /// <summary>
        /// Constructs a new DataClassList.
        /// </summary>
        public DataClassList()
        {
            this.dataClassList = new List<DataClass>();
        }

        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        public int Count
        {
            get
            {
                if (this.dataClassList != null)
                {
                    return this.dataClassList.Count;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets an item in the list.
        /// </summary>
        /// <param name="index">The index.</param>
        public DataClass this[int index]
        {
            get
            {
                if (this.dataClassList != null)
                {
                    return this.dataClassList[index];
                }

                return null;
            }

            set
            {
                if (this.dataClassList != null)
                {
                    this.dataClassList[index] = value;
                }
            }
        }

        /// <summary>
        /// Returns a new <see cref="DataClass"/> entity list.
        /// </summary>
        /// <param name="count">The number of items to be put in the list.</param>
        /// <param name="supportedOperations">The supported operations for the list.</param>
        /// <returns>The entity list.</returns>
        public static DataClassList GetDataClassList(int count, ListOperations supportedOperations)
        {
            DataClassList dataClassList = new DataClassList();

            for (int i = 0; i < count; i++)
            {
                DataClass dataClass = new DataClass()
                {
                    BoolProperty = (i % 2 == 0),
                    DateTimeProperty = new DateTime(2000 + i, 1 + (i % 12), 1 + (i % 28)),
                    IntProperty = i * 3,
                    IntPropertyWithoutAutoGenerateField = (i * 3) + 1,
                    NonGeneratedIntProperty = (i * 3) + 2,
                    StringProperty = "test string " + i.ToString(CultureInfo.CurrentCulture)
                };

                dataClass.AcceptChanges();
                dataClassList.Add(dataClass);
            }

            dataClassList.supportedOperations = supportedOperations;
            return dataClassList;
        }

        #region IEditableCollection<DataClass> Members

        /// <summary>
        /// Gets a value indicating whether or not addition is supported.
        /// </summary>
        public bool CanAdd
        {
            get
            {
                return (this.supportedOperations & ListOperations.Add) == ListOperations.Add;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not editing is supported.
        /// </summary>
        public bool CanEdit
        {
            get
            {
                return (this.supportedOperations & ListOperations.Edit) == ListOperations.Edit;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not removal is supported.
        /// </summary>
        public bool CanRemove
        {
            get
            {
                return (this.supportedOperations & ListOperations.Remove) == ListOperations.Remove;
            }
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="item">The data class.</param>
        public void Add(DataClass item)
        {
            this.dataClassList.Add(item);
        }

        /// <summary>
        /// Begins an edit.
        /// </summary>
        /// <param name="item">The data class.</param>
        public void BeginEdit(DataClass item)
        {
            item.BeginEdit();
        }

        /// <summary>
        /// Cancel an edit.
        /// </summary>
        /// <param name="item">The data class.</param>
        public void CancelEdit(DataClass item)
        {
            item.CancelEdit();
        }

        /// <summary>
        /// Creates a new data class.
        /// </summary>
        public DataClass CreateNew()
        {
            return new DataClass();
        }

        /// <summary>
        /// Ends an edit.
        /// </summary>
        /// <param name="item">The data class.</param>
        public void EndEdit(DataClass item)
        {
            item.EndEdit();
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="item">The data class.</param>
        public void Remove(DataClass item)
        {
            this.dataClassList.Remove(item);
        }

        #endregion

        #region IEditableCollection Members

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="item">The data class.</param>
        public void Add(object item)
        {
            DataClass dataClass = item as DataClass;

            if (dataClass != null)
            {
                this.dataClassList.Add(dataClass);
            }
        }

        /// <summary>
        /// Begins an edit.
        /// </summary>
        /// <param name="item">The data class.</param>
        public void BeginEdit(object item)
        {
            DataClass dataClass = item as DataClass;

            if (dataClass != null)
            {
                dataClass.BeginEdit();
            }
        }

        /// <summary>
        /// Cancels an edit.
        /// </summary>
        /// <param name="item">The data class.</param>
        public void CancelEdit(object item)
        {
            DataClass dataClass = item as DataClass;

            if (dataClass != null)
            {
                dataClass.CancelEdit();
            }
        }

        /// <summary>
        /// Ends an edit.
        /// </summary>
        /// <param name="item">The data class.</param>
        public void EndEdit(object item)
        {
            DataClass dataClass = item as DataClass;

            if (dataClass != null)
            {
                dataClass.EndEdit();
            }
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="item">The data class.</param>
        public void Remove(object item)
        {
            DataClass dataClass = item as DataClass;

            if (dataClass != null)
            {
                this.dataClassList.Remove(dataClass);
            }
        }

        #endregion

        #region IEnumerable<DataClass> Members

        public IEnumerator<DataClass> GetEnumerator()
        {
            return this.dataClassList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.dataClassList.GetEnumerator();
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            DataClass dc = value as DataClass;
            this.dataClassList.Add(dc);
            return this.dataClassList.IndexOf(dc);
        }

        public void Clear()
        {
            this.dataClassList.Clear();
        }

        public bool Contains(object value)
        {
            return this.dataClassList.Contains(value as DataClass);
        }

        public int IndexOf(object value)
        {
            return this.dataClassList.IndexOf(value as DataClass);
        }

        public void Insert(int index, object value)
        {
            this.dataClassList.Insert(index, value as DataClass);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void RemoveAt(int index)
        {
            this.dataClassList.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return this.dataClassList[index];
            }
            set
            {
                this.dataClassList[index] = value as DataClass;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
