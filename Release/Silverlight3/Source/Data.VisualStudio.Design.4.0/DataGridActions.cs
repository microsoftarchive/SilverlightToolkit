// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    ///     Provides access to actions that are shared between the MenuProvider and the CategoryEditor
    /// </summary>
    internal static class DataGridActions
    {
        /// <summary>
        ///     Opens the Edit Property-Bound Columns dialog for the given DataGrid
        /// </summary>
        public static void EditPropertyBoundColumns(ModelItem dataGridModelItem, EditingContext editingContext)
        {
            using (ModelEditingScope scope = dataGridModelItem.BeginEdit(Properties.Resources.ColumnsChanged))
            {
                PropertyColumnEditor ui = new PropertyColumnEditor(editingContext, dataGridModelItem);

                // Use Windows Forms to show the design time because Windows Forms knows about the VS message pump
                System.Windows.Forms.DialogResult result = DesignerDialog.ShowDesignerDialog(Properties.Resources.Edit_Property_Bound_Columns_Dialog_Title, ui);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    scope.Complete();
                }
                else
                {
                    scope.Revert();
                }
            }
        }

        // These methods are not used, but they are very useful when developing the Design experience
#if Development

        /// <summary>
        ///     Adds a default column for each property in the data source
        /// </summary>
        public static void GenerateColumns(ModelItem dataGridModelItem, EditingContext context)
        {
            using (ModelEditingScope scope = dataGridModelItem.BeginEdit(Properties.Resources.Generate_Columns))
            {

                // Set databinding related properties
                DataGridDesignHelper.SparseSetValue(dataGridModelItem.Properties[MyPlatformTypes.DataGrid.AutoGenerateColumnsProperty], false);

                // Get the datasource 
                object dataSource = dataGridModelItem.Properties[MyPlatformTypes.DataGrid.ItemsSourceProperty].ComputedValue;
                if (dataSource != null)
                {

                    foreach (ColumnInfo columnGenerationInfo in DataGridDesignHelper.GetColumnGenerationInfos(dataSource))
                    {

                        ModelItem dataGridColumn = null;

                        dataGridColumn = DataGridDesignHelper.CreateDefaultDataGridColumn(context, columnGenerationInfo);

                        if (dataGridColumn != null)
                        {
                            dataGridModelItem.Properties[MyPlatformTypes.DataGrid.ColumnsProperty].Collection.Add(dataGridColumn);
                        }
                    }
                }
                scope.Complete();
            }
        }

        /// <summary>
        ///     Removes all columns from the DataGrid
        /// </summary>
        public static void RemoveColumns(ModelItem dataGridModelItem, EditingContext editingContext)
        {
            using (ModelEditingScope scope = dataGridModelItem.BeginEdit(Properties.Resources.Remove_Columns))
            {
                dataGridModelItem.Properties[MyPlatformTypes.DataGrid.ColumnsProperty].Collection.Clear();
                scope.Complete();
            }
        }
#endif
    }
}
