//-----------------------------------------------------------------------
// <copyright file="CollectionViewGroup.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Windows.Data
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// A CollectionViewGroup, as created by a PagedCollectionView according to a GroupDescription.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public abstract class CollectionViewGroup : INotifyPropertyChanged
    {
        #region Private Fields

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        /// <summary>
        /// The number of data items (leaves) in the subtree under this group.
        /// </summary>
        private int _itemCount;

        /// <summary>
        /// The immediate children of the group.
        /// These may be data items (leaves) or subgroups.
        /// </summary>
        private ReadOnlyObservableCollection<object> _itemsRO;

        /// <summary>
        /// A writable copy of the collection of children of the group.
        /// </summary>
        private ObservableCollection<object> _itemsRW;

        /// <summary>
        /// The name of the group, which is the common value of the
        /// property used to divide data items into groups.
        /// </summary>
        private object _name;

        #endregion Private Fields

        #region Constructors

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the CollectionViewGroup class.
        /// </summary>
        /// <param name="name">Name of the CollectionViewGroup</param>
        protected CollectionViewGroup(object name)
        {
            this._name = name;
            this._itemsRW = new ObservableCollection<object>();
            this._itemsRO = new ReadOnlyObservableCollection<object>(this._itemsRW);
        }

        #endregion Constructors

        #region Public Properties

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        /// <summary>
        /// Gets a value indicating whether this group is at the 
        /// bottom level (not further sub-grouped).
        /// </summary>
        public abstract bool IsBottomLevel
        {
            get;
        }

        /// <summary>
        /// Gets the immediate children of the group.
        /// These may be data items (leaves) or subgroups.
        /// </summary>
        public ReadOnlyObservableCollection<object> Items
        {
            get { return this._itemsRO; }
        }

        /// <summary>
        /// Gets the number of data items (leaves) in the subtree under this group.
        /// </summary>
        public int ItemCount
        {
            get { return this._itemCount; }
        }

        /// <summary>
        /// Gets the name of the group, which is the common value of the
        /// property used to divide data items into groups.
        /// </summary>
        public object Name
        {
            get { return this._name; }
        }

        #endregion Public Properties

        #region Protected Properties

        //------------------------------------------------------
        //
        //  Protected Properties
        //
        //------------------------------------------------------

        /// <summary>
        /// Gets or sets the number of data items (leaves) in the subtree 
        /// under this group. Derived classes can change the count using 
        /// this property; the changes will be reflected in the public 
        /// ItemCount property.
        /// </summary>
        protected int ProtectedItemCount
        {
            get
            {
                return this._itemCount;
            }

            set
            {
                this._itemCount = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("ItemCount"));
            }
        }

        /// <summary>
        /// Gets the items of the group.
        /// Derived classes can add or remove items using this property;
        /// the changes will be reflected in the public Items property.
        /// </summary>
        protected ObservableCollection<object> ProtectedItems
        {
            get { return this._itemsRW; }
        }

        #endregion Protected Properties

        #region INotifyPropertyChanged

        /// <summary>
        /// Raises a PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        /// <param name="e">EventArgs for the PropertyChange</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        /// <summary>
        /// PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this.PropertyChanged += value; }
            remove { this.PropertyChanged -= value; }
        }

        /// <summary>
        /// PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        protected virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged
    }
}

