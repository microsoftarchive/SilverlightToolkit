// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    ///     This class represents the union of all Properties (from ItemsSource) and DataGrid Columns.
    ///     It must translate index's to syncronize with the DataGrid Columns ModelProperty Collection.
    /// </summary>
    internal class PropertyColumnDataModelCollection : ObservableCollection<PropertyColumnDataModel>
    {
        private ModelItem _dataGrid;
        private bool _initializing;

        /// <summary>
        ///     Fills this collection from a DataGrid.  Creates one item for each Column, then one for each property.
        /// </summary>
        public PropertyColumnDataModelCollection(PropertyColumnEditor propertyColumnEditor, ModelItem dataGrid)
        {
            PropertyColumnEditor = propertyColumnEditor;
            _dataGrid = dataGrid;
            _initializing = true;
            try
            {
                object dataSource = _dataGrid.Properties[PlatformTypes.DataGrid.ItemsSourceProperty].ComputedValue;
                if (dataSource != null)
                {
                    // Create a PropertyColumnDataModel for each column in the DataGrid's Columns Collection
                    foreach (ModelItem dataGridColumn in _dataGrid.Properties[PlatformTypes.DataGrid.ColumnsProperty].Collection)
                    {
                        PropertyColumnDataModel columnModel = new PropertyColumnDataModel(this);
                        columnModel.SetInitialColumn(dataGridColumn);
                        this.Add(columnModel);
                    }

                    // Look through the column info the DataSource's properties and see if any match existing columns
                    foreach (ColumnInfo columnGenerationInfo in DataGridDesignHelper.GetColumnGenerationInfos(dataSource))
                    {
                        bool found = false;

                        // only create a new item if we dont already have an item that matches based on the column binding.
                        foreach (PropertyColumnDataModel columnModel in this)
                        {
                            if (columnModel.ColumnGenerationInfo == null && columnModel.BindingPath == columnGenerationInfo.PropertyInfo.Name)
                            {
                                // if we already have a column bound to this property, then must associate the property descriptor with it.
                                columnModel.ColumnGenerationInfo = columnGenerationInfo;
                                found = true;
                                break;
                            }
                        }

                        // if we dont have a column that binds to this property, add a new data object to represent this potential column
                        if (!found)
                        {
                            this.Add(new PropertyColumnDataModel(this) { ColumnGenerationInfo = columnGenerationInfo });
                        }
                    }
                }
            }
            finally
            {
                _initializing = false;
            }
        }

        internal PropertyColumnEditor PropertyColumnEditor
        {
            get;
            private set;
        }

        public int ColumnIndexToPropertyIndex(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= this.Count || !this[columnIndex].HasProperty)
            {
                return -1;
            }

            int propertyIndex = 0;
            for (int i = 0; i < columnIndex; i++)
            {
                if (this[i].HasProperty)
                {
                    propertyIndex++;
                }
            }

            return propertyIndex;
        }

        public int ColumnIndexToDataGridColumnIndex(int columnIndex)
        {
            int dataGridColumnIndex = -1;
            if (columnIndex >= 0 && columnIndex < this.Count)
            {
                for (int i = 0; i <= columnIndex; i++)
                {
                    if (this[i].Column != null)
                    {
                        dataGridColumnIndex++;
                    }
                }
            }

            return dataGridColumnIndex;
        }

        // Override update methods to keep the underlying model object in sync
        protected override void ClearItems()
        {
            if (!this._initializing)
            {
                _dataGrid.Properties[PlatformTypes.DataGrid.ColumnsProperty].Collection.Clear();
            }

            base.ClearItems();
        }

        public int PropertyIndexToColumnIndex(int propertyIndex)
        {
            if (propertyIndex >= 0)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].HasProperty)
                    {
                        propertyIndex--;
                    }

                    if (propertyIndex < 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        protected override void RemoveItem(int index)
        {
            if (!this._initializing)
            {
                int dgindex = ColumnIndexToDataGridColumnIndex(index);
                if (dgindex >= 0)
                {
                    _dataGrid.Properties[PlatformTypes.DataGrid.ColumnsProperty].Collection.RemoveAt(dgindex);
                }
            }

            base.RemoveItem(index);
        }

        protected override void InsertItem(int index, PropertyColumnDataModel item)
        {
            if (!this._initializing)
            {
                int dgindex = ColumnIndexToDataGridColumnIndex(index);
                if (dgindex >= 0)
                {
                    _dataGrid.Properties[PlatformTypes.DataGrid.ColumnsProperty].Collection.Insert(dgindex, item.Column);
                }
            }

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, PropertyColumnDataModel item)
        {
            if (!this._initializing)
            {
                int dgindex = ColumnIndexToDataGridColumnIndex(index);
                if (dgindex >= 0)
                {
                    _dataGrid.Properties[PlatformTypes.DataGrid.ColumnsProperty].Collection[dgindex] = item.Column;
                }
            }

            base.SetItem(index, item);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (!this._initializing)
            {
                int dgindexOld = ColumnIndexToDataGridColumnIndex(oldIndex);
                int dgindexNew = ColumnIndexToDataGridColumnIndex(newIndex);
                if (dgindexOld >= 0 && dgindexNew >= 0)
                {
                    _dataGrid.Properties[PlatformTypes.DataGrid.ColumnsProperty].Collection.Move(dgindexOld, dgindexNew);
                }
            }

            base.MoveItem(oldIndex, newIndex);
        }

        /// <summary>
        ///   When the column changes on a property it could mean we either need to add/remove/update the column for that item
        /// </summary>
        internal void OnColumnChanging(PropertyColumnDataModel propertyColumnDataModel, ModelItem oldColumn, ModelItem newColumn)
        {
            if (!_initializing)
            {
                int index = this.IndexOf(propertyColumnDataModel);
                if (index < 0)
                {
                    return;
                }


                // removing column
                if (newColumn == null)
                {
                    int dgindex = ColumnIndexToDataGridColumnIndex(index);
                    if (dgindex >= 0)
                    {
                        _dataGrid.Properties[PlatformTypes.DataGrid.ColumnsProperty].Collection.RemoveAt(dgindex);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("propertyColumnDataModel");
                    }
                }
                // adding column
                else if (oldColumn == null)
                {
                    int dgindex = ColumnIndexToDataGridColumnIndex(index) + 1;
                    if (dgindex >= 0 && dgindex < _dataGrid.Properties[PlatformTypes.DataGrid.ColumnsProperty].Collection.Count)
                    {
                        _dataGrid.Properties[PlatformTypes.DataGrid.ColumnsProperty].Collection.Insert(dgindex, newColumn);
                    }
                    else
                    {
                        _dataGrid.Properties[PlatformTypes.DataGrid.ColumnsProperty].Collection.Add(newColumn);
                    }
                }
                // replacing column
                else
                {
                    int dgindex = ColumnIndexToDataGridColumnIndex(index);
                    if (dgindex >= 0)
                    {
                        _dataGrid.Properties[PlatformTypes.DataGrid.ColumnsProperty].Collection[dgindex] = newColumn;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("propertyColumnDataModel");
                    }
                }
            }
        }
    }
}
