//-----------------------------------------------------------------------
// <copyright file="TestEnumerableCollection.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace System.ComponentModel.UnitTests
{
    public sealed class TestEnumerableCollection : IEnumerable, INotifyCollectionChanged
    {
        private ObservableCollection<object> _list;

        public TestEnumerableCollection()
        {
            _list = new ObservableCollection<object>();
            _list.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public void Add(object item)
        {
            this._list.Add(item);
        }

        public void Remove(object item)
        {
            this._list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this._list.RemoveAt(index);
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        #endregion
    }
}
