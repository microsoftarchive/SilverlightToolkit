// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// Interaction logic for PropertyColumnEditor.xaml
    /// </summary>
    internal partial class PropertyColumnEditor : UserControl
    {
        private ModelItem _dataGrid;
        private bool _changingSelectAll;

        public PropertyColumnEditor(EditingContext context, ModelItem dataGrid)
        {
            _dataGrid = dataGrid;
            this.EditingContext = context;

            this.Columns = new PropertyColumnDataModelCollection(this, _dataGrid);

            this.PropertyColumnsCVS = new CollectionViewSource();
            this.PropertyColumnsCVS.Source = Columns;
            this.PropertyColumnsCVS.Filter += new FilterEventHandler(PropertyColumnsCVS_Filter);

            this.DataContext = this;
            InitializeComponent();

            EnsureSelectAllChecked();
            LoadStringResources();

// WPF does not use the Display Attribute metadata
#if WPF
            MetadataCheckBox.Visibility = Visibility.Collapsed;
            MetadataTextBlock.Visibility = Visibility.Collapsed;
#endif
        }

        private PropertyColumnDataModelCollection Columns
        {
            get;
            set;
        }

        public CollectionViewSource PropertyColumnsCVS
        {
            get;
            private set;
        }

        internal EditingContext EditingContext 
        { 
            get; 
            private set; 
        }

        private void EnsureSelectAllChecked()
        {
            if (_changingSelectAll || this.SelectAllCheckBox == null)
            {
                return;
            }

            bool shouldCheck = false;
            if (this.Columns != null && this.Columns.Count > 0)
            {
                foreach (PropertyColumnDataModel columnModel in this.Columns)
                {
                    if (columnModel.HasProperty)
                    {
                        shouldCheck = true;
                        if (!columnModel.HasColumn)
                        {
                            shouldCheck = false;
                            break;
                        }
                    }
                }
            }

            if (this.SelectAllCheckBox.IsChecked != shouldCheck)
            {
                _changingSelectAll = true;
                try
                {
                    this.SelectAllCheckBox.IsChecked = shouldCheck;
                }
                finally
                {
                    _changingSelectAll = false;
                }
            }
        }

        private void LoadStringResources()
        {
            this.PropertiesGroupBox.Header = Properties.Resources.Properties;
            this.SelectAllCheckBox.Content = Properties.Resources.SelectAll;
            this.ColumnPropertiesGroupBox.Header = Properties.Resources.ColumnProperties;
            this.MetadataCheckBox.Content = Properties.Resources.UseMetadaValue;
        }

        internal void OnHasColumnChanged()
        {
            EnsureSelectAllChecked();
            DataGridDesignHelper.SparseSetValue(_dataGrid.Properties[PlatformTypes.DataGrid.AutoGenerateColumnsProperty], false);
        }

        /// <summary>
        ///     Only show those columns that have simple Properties as Binding
        /// </summary>
        private void PropertyColumnsCVS_Filter(object sender, FilterEventArgs e)
        {
            var column = e.Item as PropertyColumnDataModel;
            e.Accepted = column.HasProperty;
        }

        private void SelectAllChecked(object sender, RoutedEventArgs e)
        {
            if (!_changingSelectAll && Columns != null)
            {
                foreach (PropertyColumnDataModel column in Columns)
                {
                    if (column.HasProperty && !column.HasColumn)
                    {
                        column.HasColumn = true;
                    }
                }
            }
        }

        private void MoveUp(object sender, RoutedEventArgs e)
        {
            int itemIndex = this.PropertyList.SelectedIndex;
            if (itemIndex > 0)
            {
                itemIndex = this.Columns.PropertyIndexToColumnIndex(itemIndex);
                int newIndex = itemIndex;

                // swap it with the previous column with a property binding
                do
                {
                    newIndex--;
                } 
                while (newIndex >= 0 && !this.Columns[newIndex].HasProperty);

                if (newIndex >= 0)
                {
                    this.Columns.Move(itemIndex, newIndex);
                    this.PropertyList.SelectedIndex = this.Columns.ColumnIndexToPropertyIndex(newIndex);
                }
            }
        }

        private void MoveDown(object sender, RoutedEventArgs e)
        {
            int itemIndex = this.PropertyList.SelectedIndex;
            if (itemIndex >= 0 && itemIndex < this.Columns.Count - 1)
            {
                itemIndex = this.Columns.PropertyIndexToColumnIndex(itemIndex);
                int newIndex = itemIndex;

                // swap it with the previous column with a property binding
                do
                {
                    newIndex++;
                } 
                while (newIndex < this.Columns.Count && !Columns[newIndex].HasProperty);

                if (newIndex < Columns.Count)
                {
                    this.Columns.Move(itemIndex, newIndex);
                    this.PropertyList.SelectedIndex = Columns.ColumnIndexToPropertyIndex(newIndex);
                }
            }
        }

        private void SelectAllUnchecked(object sender, RoutedEventArgs e)
        {
            if (!_changingSelectAll && Columns != null)
            {
                foreach (PropertyColumnDataModel column in Columns)
                {
                    if (column.HasProperty && column.HasColumn)
                    {
                        column.HasColumn = false;
                    }
                }
            }
        }

        private void SelectItem(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = e.Source as CheckBox;
            if (checkBox != null)
            {
                this.PropertyList.SelectedItem = checkBox.DataContext;
            }
        }
    }
}
