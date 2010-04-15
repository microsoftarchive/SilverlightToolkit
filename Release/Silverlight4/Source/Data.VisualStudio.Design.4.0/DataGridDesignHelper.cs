// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Windows.Controls.Data.Design.Silverlight;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.Data.Design
{
    internal static class DataGridDesignHelper
    {
        private const string ImageLocationBase = @"/System.Windows.Controls.Data.VisualStudio.Design.4.0;component/Images/";

        /// <summary>
        ///     Creates a DataGridColumn derived ModelItem
        /// </summary>
        internal static ModelItem CreateColumnFromColumnType(EditingContext context, TypeIdentifier dataGridColumnTypeId, ColumnInfo columnGenerationInfo)
        {
            ModelItem dataGridColumn = ModelFactory.CreateItem(context, dataGridColumnTypeId);
            if (columnGenerationInfo.HeaderFromAttribute == null)
            {
                // Default the Header to the property name if there's no attribute tied to it
                dataGridColumn.Properties[PlatformTypes.DataGridColumn.HeaderProperty].SetValue(columnGenerationInfo.PropertyInfo.Name);
            }

            if (dataGridColumnTypeId == PlatformTypes.DataGridTemplateColumn.TypeId)
            {
                return CreateTemplateColumn(context, columnGenerationInfo, dataGridColumn);
            }

            ModelItem binding = ModelFactory.CreateItem(context, PlatformTypes.Binding.TypeId);
            binding.Properties[PlatformTypes.Binding.PathProperty].SetValue(columnGenerationInfo.PropertyInfo.Name);

            // ReadOnly Properties are handled by the runtime so we don't need to alter the Binding here
            // to account for them.

            // Whether or not a column can sort is handled by the runtime as well

            dataGridColumn.Properties[PlatformTypes.DataGridBoundColumn.BindingProperty].SetValue(binding);

            return dataGridColumn;
        }

        /// <summary>
        ///     Create a DataTemplate for the given control bound to the given data source property
        /// </summary>
        internal static ModelItem CreateDataTemplate(EditingContext context, TypeIdentifier controlToTemplateTypeId, PropertyIdentifier controlPropertyId, string datasourcePropertyName)
        {
            ModelItem controlToTemplate = ModelFactory.CreateItem(context, controlToTemplateTypeId);
            ModelItem controlBinding = ModelFactory.CreateItem(context, PlatformTypes.Binding.TypeId);
            controlBinding.Properties[PlatformTypes.Binding.PathProperty].SetValue(datasourcePropertyName);
            controlToTemplate.Properties[controlPropertyId].SetValue(controlBinding);
            ModelItem dataTemplate = ModelFactory.CreateItem(context, PlatformTypes.DataTemplate.TypeId);
            dataTemplate.Content.SetValue(controlToTemplate);
            return dataTemplate;
        }

        /// <summary>
        ///     Creates a DataGridColumn derived ModelItem based on the PropertyDescriptor's PropertyType.
        /// </summary>
        internal static ModelItem CreateDefaultDataGridColumn(EditingContext context, ColumnInfo columnGenerationInfo)
        {
            TypeIdentifier dataGridColumnTypeId;

            if (columnGenerationInfo.PropertyInfo.PropertyType == typeof(bool))
            {
                dataGridColumnTypeId = PlatformTypes.DataGridCheckBoxColumn.TypeId;
            }
            else
            {
                dataGridColumnTypeId = PlatformTypes.DataGridTextColumn.TypeId;
            }
            // Add checks here for other column types when they come online

            return CreateColumnFromColumnType(context, dataGridColumnTypeId, columnGenerationInfo);
        }

        /// <summary>
        ///     Creates a DataGridTemplateColumn ModelItem with default templates for this property
        /// </summary>
        internal static ModelItem CreateTemplateColumn(EditingContext context, ColumnInfo columnGenerationInfo, ModelItem dataGridColumn)
        {
            ModelItem displayDataTemplate = CreateDataTemplate(context, PlatformTypes.TextBlock.TypeId, PlatformTypes.TextBlock.TextProperty, columnGenerationInfo.PropertyInfo.Name);
            dataGridColumn.Properties[PlatformTypes.DataGridTemplateColumn.CellTemplateProperty].SetValue(displayDataTemplate);

            ModelItem editingDataTemplate = CreateDataTemplate(context, PlatformTypes.TextBox.TypeId, PlatformTypes.TextBox.TextProperty, columnGenerationInfo.PropertyInfo.Name);
            dataGridColumn.Properties[PlatformTypes.DataGridTemplateColumn.CellEditingTemplateProperty].SetValue(editingDataTemplate);

            return dataGridColumn;
        }

        /// <summary>
        ///     Get the properties on the ItemSource or if the ItemsSource is an IEnumerable then the properties on 
        ///     the items in the IEnumerable. See comments on the SilverlightHelpers.GetItemsSourceProperties method
        ///     for an explanation 
        /// </summary>
        internal static IEnumerable<ColumnInfo> GetColumnGenerationInfos(object dataSource)
        {
            List<ColumnInfo> columnGenerationInfos = new List<ColumnInfo>();
            foreach (SilverlightColumnInfo silverlightColumnGenerationInfo in SilverlightHelpers.GetColumnGenerationInfo(dataSource))
            {
                columnGenerationInfos.Add(new ColumnInfo(silverlightColumnGenerationInfo.HeaderFromAttribute, silverlightColumnGenerationInfo.PropertyInfo));
            }

            return columnGenerationInfos;
        }

        /// <summary>
        ///     Returns the string that represents a given DataGridColumn type
        /// </summary>
        internal static string GetColumnStringType(ModelItem column)
        {
            if (column.IsItemOfType(PlatformTypes.DataGridTextColumn.TypeId))
            {
                return Properties.Resources.Text_Column;
            }

            if (column.IsItemOfType(PlatformTypes.DataGridCheckBoxColumn.TypeId))
            {
                return Properties.Resources.CheckBox_Column;
            }

            if (column.IsItemOfType(PlatformTypes.DataGridTemplateColumn.TypeId))
            {
                return Properties.Resources.Template_Column;
            }

            return string.Empty;
        }

        /// <summary>
        ///     Returns the DataGridColumn type based on a string
        /// </summary>
        internal static TypeIdentifier GetColumnSystemType(string typeName)
        {
            if (typeName == Properties.Resources.Text_Column)
            {
                return PlatformTypes.DataGridTextColumn.TypeId;
            }

            if (typeName == Properties.Resources.CheckBox_Column)
            {
                return PlatformTypes.DataGridCheckBoxColumn.TypeId;
            }

            return PlatformTypes.DataGridTemplateColumn.TypeId;
        }

        /// <summary>
        ///     Sets the value of a ModelProperty if it's not the default value.  This makes better looking XAML.
        /// </summary>
        internal static void SparseSetValue(ModelProperty property, object value)
        {
            object defaultValue = property.DefaultValue;
            if (object.Equals(defaultValue, value))
            {
                property.ClearValue();
            }
            else
            {
                property.SetValue(value);
            }
        }
    }
}
