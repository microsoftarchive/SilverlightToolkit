// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// Data model for property bound columns.  This object represents the union of both properties and columns.
    /// </summary>
    internal class PropertyColumnDataModel : DependencyObject
    {
        private ColumnInfo _columnGenerationInfo;
        private ModelItem _columnModelItem;
        private bool _isSyncing;
        private ModelItem _oldColumnModelItem;
        private PropertyColumnDataModelCollection _parentCollection;
        private bool _restoreColumn;

        /// <summary>
        /// Constructs a PropertyColumnDataModel
        /// </summary>
        /// <param name="parentCollection">Parent collection</param>
        public PropertyColumnDataModel(PropertyColumnDataModelCollection parentCollection)
        {
            if (parentCollection == null)
            {
                throw new ArgumentNullException("parentCollection");
            }

            _parentCollection = parentCollection;
            SyncColumnProperties();
        }

        /// <summary>
        ///     This is used to link Properties with Columns
        /// </summary>
        internal string BindingPath 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Property information used for creating the column
        /// </summary>
        public ColumnInfo ColumnGenerationInfo
        {
            get
            {
                return _columnGenerationInfo;
            }
            set
            {
                _columnGenerationInfo = value;
                if (_columnGenerationInfo != null && _columnGenerationInfo.PropertyInfo != null)
                {
                    if (_columnGenerationInfo.HeaderFromAttribute != null)
                    {
                        this.ColumnUseHeaderMetadata = this.ColumnHeader == null;
                    }
                }
            }
        }

        /// <summary>
        /// The ModelItem of the column that this DataModel represents
        /// </summary>
        public ModelItem Column
        {
            get
            {
                return _columnModelItem;
            }
            set
            {
                if (!object.ReferenceEquals(_columnModelItem, value))
                {
                    // The collection needs to know when we're changing the column because it must
                    // keep the ModelItems for this column in sync.
                    _parentCollection.OnColumnChanging(this, _columnModelItem, value);
                }

                _columnModelItem = value;

                SyncColumnProperties();
            }
        }

        #region ColumnCanUserReorder
        /// <summary>
        /// CanUserReorder property that the PropertyColumnEditor binds to
        /// </summary>
        public bool? ColumnCanUserReorder
        {
            get
            {
                return (bool?)GetValue(ColumnCanUserReorderProperty);
            }
            set
            {
                SetValue(ColumnCanUserReorderProperty, value);
            }
        }

        public static DependencyProperty ColumnCanUserReorderProperty = 
            DependencyProperty.Register(
                "ColumnCanUserReorder", 
                typeof(bool?), 
                typeof(PropertyColumnDataModel), 
                new FrameworkPropertyMetadata(OnColumnCanUserReorderPropertyChanged));

        private static void OnColumnCanUserReorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyColumnDataModel columnModel = (PropertyColumnDataModel)d;
            if (columnModel.Column != null)
            {
                DataGridDesignHelper.SparseSetValue(columnModel.Column.Properties[PlatformTypes.DataGridColumn.CanUserReorderProperty], e.NewValue);
            }
        }
        #endregion ColumnCanUserReorder

        #region ColumnCanUserResize
        /// <summary>
        /// CanUserResize property that the PropertyColumnEditor binds to
        /// </summary>
        public bool? ColumnCanUserResize
        {
            get
            {
                return (bool?)GetValue(ColumnCanUserResizeProperty);
            }
            set
            {
                SetValue(ColumnCanUserResizeProperty, value);
            }
        }

        public static DependencyProperty ColumnCanUserResizeProperty = 
            DependencyProperty.Register(
                "ColumnCanUserResize", 
                typeof(bool?), 
                typeof(PropertyColumnDataModel), 
                new FrameworkPropertyMetadata(OnColumnCanUserResizeChanged));

        private static void OnColumnCanUserResizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyColumnDataModel columnModel = (PropertyColumnDataModel)d;
            if (columnModel.Column != null)
            {
                DataGridDesignHelper.SparseSetValue(columnModel.Column.Properties[PlatformTypes.DataGridColumn.CanUserResizeProperty], e.NewValue);
            }
        }
        #endregion ColumnCanUserResize

        #region ColumnCanUserSort
        /// <summary>
        /// CanUserSort property that the PropertyColumnEditor binds to
        /// </summary>
        public bool? ColumnCanUserSort
        {
            get
            {
                return (bool?)GetValue(ColumnCanUserSortProperty);
            }
            set
            {
                SetValue(ColumnCanUserSortProperty, value);
            }
        }

        public static DependencyProperty ColumnCanUserSortProperty = 
            DependencyProperty.Register(
                "ColumnCanUserSort", 
                typeof(bool?), 
                typeof(PropertyColumnDataModel), 
                new FrameworkPropertyMetadata(OnColumnCanUserSortChanged));

        private static void OnColumnCanUserSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyColumnDataModel columnModel = (PropertyColumnDataModel)d;
            if (columnModel.Column != null)
            {
                DataGridDesignHelper.SparseSetValue(columnModel.Column.Properties[PlatformTypes.DataGridColumn.CanUserSortProperty], e.NewValue);
            }
        }
        #endregion ColumnCanUserSort

        #region ColumnIsReadOnly
        /// <summary>
        /// IsReadOnly property that the PropertyColumnEditor binds to
        /// </summary>
        public bool? ColumnIsReadOnly
        {
            get
            {
                return (bool?)GetValue(ColumnIsReadOnlyProperty);
            }
            set
            {
                SetValue(ColumnIsReadOnlyProperty, value);
            }
        }

        public static DependencyProperty ColumnIsReadOnlyProperty = 
            DependencyProperty.Register(
                "ColumnIsReadOnly", 
                typeof(bool?), 
                typeof(PropertyColumnDataModel), 
                new FrameworkPropertyMetadata(OnColumnIsReadOnlyChanged));

        private static void OnColumnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyColumnDataModel columnModel = (PropertyColumnDataModel)d;
            if (columnModel.Column != null)
            {
                DataGridDesignHelper.SparseSetValue(columnModel.Column.Properties[PlatformTypes.DataGridColumn.IsReadOnlyProperty], e.NewValue);
            }
        }
        #endregion ColumnIsReadOnly

        #region ColumnHeader
        /// <summary>
        /// Header that the PropertyColumnEditor dialog is bound to
        /// </summary>
        public string ColumnHeader
        {
            get
            {
                return (string)GetValue(ColumnHeaderProperty);
            }
            set
            {
                SetValue(ColumnHeaderProperty, value);
            }
        }

        public static DependencyProperty ColumnHeaderProperty =
            DependencyProperty.Register(
                "ColumnHeader",
                typeof(string),
                typeof(PropertyColumnDataModel),
                new FrameworkPropertyMetadata(OnColumnHeaderPropertyChanged));

        private static void OnColumnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyColumnDataModel columnModel = (PropertyColumnDataModel)d;
            if (columnModel.Column != null)
            {
                if (e.NewValue == null)
                {
                    columnModel.Column.Properties[PlatformTypes.DataGridColumn.HeaderProperty].ClearValue();
                }
                else
                {
                    DataGridDesignHelper.SparseSetValue(columnModel.Column.Properties[PlatformTypes.DataGridColumn.HeaderProperty], e.NewValue);
                }
            }
        }
        #endregion ColumnHeader

        #region ColumnUseHeaderMetadata
        /// <summary>
        /// UserHeaderMetadata property that the PropertyColumnEditor binds to
        /// </summary>
        public bool ColumnUseHeaderMetadata
        {
            get
            {
                return (bool)GetValue(ColumnUseHeaderMetadataProperty);
            }
            set
            {
                SetValue(ColumnUseHeaderMetadataProperty, value);
            }
        }

        public static DependencyProperty ColumnUseHeaderMetadataProperty = 
            DependencyProperty.Register(
                "ColumnUseHeaderMetadata", 
                typeof(bool), 
                typeof(PropertyColumnDataModel), 
                new FrameworkPropertyMetadata(OnColumnUseHeaderMetadataChanged));

        private static void OnColumnUseHeaderMetadataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyColumnDataModel columnModel = (PropertyColumnDataModel)d;
            bool newValue = (bool)e.NewValue;
            if (newValue)
            {
                // The DataGrid runtime should be updated to re-fetch the Header when this happens at Design time.
                // Currently, the header will be blank if you go from Custom header to use metadata until you reload
                // the DataGrid.  This is because the DataGrid only populates from metadata when the column is added.
                columnModel.ColumnHeader = null;
            }
        }
        #endregion ColumnUseHeaderMetadata

        #region ColumnTypeName
        /// <summary>
        /// Name of the type of column represented
        /// </summary>
        public string ColumnTypeName
        {
            get
            {
                return (string)GetValue(ColumnTypeNameProperty);
            }
            set
            {
                SetValue(ColumnTypeNameProperty, value);
            }
        }

        public static DependencyProperty ColumnTypeNameProperty =
            DependencyProperty.Register(
                "ColumnTypeName",
                typeof(string),
                typeof(PropertyColumnDataModel),
                new FrameworkPropertyMetadata(OnColumnTypeNameChanged));

        private static void OnColumnTypeNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyColumnDataModel columnModel = (PropertyColumnDataModel)d;
            if (!columnModel._isSyncing &&
                columnModel.Column != null &&
                columnModel._columnGenerationInfo != null &&
                columnModel._columnGenerationInfo.PropertyInfo != null &&
                !string.IsNullOrEmpty(columnModel.ColumnTypeName))
            {
                TypeIdentifier columnTypeId = DataGridDesignHelper.GetColumnSystemType(columnModel.ColumnTypeName);
                // Update the column with the model properties.  If someone changed the header,
                // we want to keep this value when they change the type.
                columnModel._restoreColumn = true;
                columnModel.Column = DataGridDesignHelper.CreateColumnFromColumnType(
                    columnModel._parentCollection.PropertyColumnEditor.EditingContext,
                    columnTypeId,
                    columnModel._columnGenerationInfo);
            }
        }
        #endregion ColumnTypeName

        #region HasColumn
        /// <summary>
        /// Whether or not the DataGrid has a column for this property
        /// </summary>
        public bool HasColumn
        {
            get
            {
                return (bool)GetValue(HasColumnProperty);
            }
            set
            {
                SetValue(HasColumnProperty, value);
            }
        }

        public static DependencyProperty HasColumnProperty =
            DependencyProperty.Register(
                "HasColumn",
                typeof(bool),
                typeof(PropertyColumnDataModel),
                new FrameworkPropertyMetadata(OnHasColumnPropertyChanged));

        private static void OnHasColumnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyColumnDataModel columnModel = (PropertyColumnDataModel)d;
            bool newValue = (bool)e.NewValue;

            if (newValue && columnModel.Column == null)
            {
                columnModel.CreateColumn();
            }
            else if (!newValue && columnModel.Column != null)
            {
                columnModel.ClearColumn();
            }

            // The editor needs to know when the column has changed so it can
            // refresh it's "Select All" checkbox.
            columnModel._parentCollection.PropertyColumnEditor.OnHasColumnChanged();
        }
        #endregion HasColumn

        /// <summary>
        /// Returns true when there is a property associates with this PropertyColumnDataModel
        /// </summary>
        internal bool HasProperty
        {
            get
            {
                return (_columnGenerationInfo != null) && (_columnGenerationInfo.PropertyInfo != null);
            }
        }

        private void ClearColumn()
        {
            // If the user makes a change to the column (like changing the Header) and then un-checks the property
            // we want to remember the changes in case they later re-check the property.
            _oldColumnModelItem = Column;
            Column = null;
        }

        private void CreateColumn()
        {
            // The user may have already had a column for this property.  If so we want to restore it so any changes
            // they made will be restored.
            if (_oldColumnModelItem != null)
            {
                // bring back the properties & column that we had before.
                // pull the model properties from the column
                _restoreColumn = false;
                Column = _oldColumnModelItem;
            }
            else
            {
                // Update the column with the model properties.
                _restoreColumn = true;
                ModelItem column = DataGridDesignHelper.CreateDefaultDataGridColumn(_parentCollection.PropertyColumnEditor.EditingContext, _columnGenerationInfo);
                if (_columnGenerationInfo.HeaderFromAttribute == null)
                {
                    this.ColumnHeader = _columnGenerationInfo.PropertyInfo.Name;
                }
                Column = column;
            }
        }

        /// <summary>
        ///     Sets BindingPath which will be set to the name of the property that the binding refers to if one exists.
        ///     Discovers Binding.Path.Path using ModelItem access to avoid platform specific types
        /// </summary>
        private void SetBindingPath()
        {
            string propertyName = string.Empty;

            //To get this working for DataGridTemplateColumns, we need to be able to serialize designer only variables
            //which is not available at this time

            //Get the Binding Property from DataGridBoundColumn
            ModelProperty bindingProperty = _columnModelItem.Properties.Find(PlatformTypes.DataGridBoundColumn.BindingProperty);
            if (bindingProperty != null)
            {

                //Get the Model for the value of the Binding property
                ModelItem bindingItem = bindingProperty.Value;

                //If there is one and its a Binding then get the PropertyPath and extract Path from that
                if (bindingItem != null && bindingItem.IsItemOfType(PlatformTypes.Binding.TypeId))
                {
                    ModelProperty pathProperty = bindingItem.Properties[PlatformTypes.Binding.PathProperty];
                    if (pathProperty != null)
                    {
                        propertyName = pathProperty.Value.Properties[PlatformTypes.PropertyPath.PathProperty].ComputedValue as string;
                    }
                }
            }
            BindingPath = propertyName;
        }

        /// <summary>
        ///     Set the initial column model and pulls in relevant properties.
        /// </summary>
        public void SetInitialColumn(ModelItem column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            // pull the model properties from the column
            _restoreColumn = false;
            Column = column;
        }

        /// <summary>
        ///     When the column changes we need to sync other properties to keep our databinding in sync.
        /// </summary>
        private void SyncColumnProperties()
        {
            if (!_isSyncing)
            {
                _isSyncing = true;
                try
                {
                    if (Column == null)
                    {
                        BindingPath = string.Empty;
                        ColumnTypeName = string.Empty;
                        HasColumn = false;
                    }
                    else
                    {
                        SetBindingPath();
                        ColumnTypeName = DataGridDesignHelper.GetColumnStringType(Column);
                        HasColumn = true;

                        // We sometimes want to pull the properties from the column, and other times we want to
                        // push the models properties down to the column.
                        if (_restoreColumn)
                        {
                            DataGridDesignHelper.SparseSetValue(Column.Properties[PlatformTypes.DataGridColumn.HeaderProperty], ColumnHeader);
                            DataGridDesignHelper.SparseSetValue(Column.Properties[PlatformTypes.DataGridColumn.CanUserReorderProperty], ColumnCanUserReorder);
                            DataGridDesignHelper.SparseSetValue(Column.Properties[PlatformTypes.DataGridColumn.CanUserResizeProperty], ColumnCanUserResize);
                            DataGridDesignHelper.SparseSetValue(Column.Properties[PlatformTypes.DataGridColumn.CanUserSortProperty], ColumnCanUserSort);
                            DataGridDesignHelper.SparseSetValue(Column.Properties[PlatformTypes.DataGridColumn.IsReadOnlyProperty], ColumnIsReadOnly);
                        }
                        else
                        {
                            // Update based on what's set in the ModelItem
                            ModelProperty modelProperty = Column.Properties[PlatformTypes.DataGridColumn.HeaderProperty];
                            ColumnHeader = modelProperty.IsSet ? (string)modelProperty.ComputedValue : null;

                            modelProperty = Column.Properties[PlatformTypes.DataGridColumn.CanUserReorderProperty];
                            ColumnCanUserReorder = modelProperty.IsSet ? (bool?)modelProperty.ComputedValue : null;

                            modelProperty = Column.Properties[PlatformTypes.DataGridColumn.CanUserResizeProperty];
                            ColumnCanUserResize = modelProperty.IsSet ? (bool?)modelProperty.ComputedValue : null;

                            modelProperty = Column.Properties[PlatformTypes.DataGridColumn.CanUserSortProperty];
                            ColumnCanUserSort = modelProperty.IsSet ? (bool?)modelProperty.ComputedValue : null;

                            modelProperty = Column.Properties[PlatformTypes.DataGridColumn.IsReadOnlyProperty];
                            ColumnIsReadOnly = modelProperty.IsSet ? (bool?)modelProperty.ComputedValue : null;
                        }
                    }
                }
                finally
                {
                    _isSyncing = false;
                }
            }
        }
    }
}
