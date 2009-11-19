//-----------------------------------------------------------------------
// <copyright file="FixedSizeList.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.ComponentModel.UnitTests
{
    public sealed class FixedSizeCollection<T> : Collection<T>, IList
    {
        #region IList Members

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Clear()
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        int IList.IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        bool IList.IsFixedSize
        {
            get { return true; }
        }

        bool IList.IsReadOnly
        {
            get { return (this as IList).IsReadOnly; }
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int ICollection.Count
        {
            get { return base.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return (this as ICollection).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return (this as ICollection).SyncRoot; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
