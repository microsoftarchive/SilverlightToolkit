// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Data.Design.Silverlight;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataGridTemplateColumn.
    /// </summary>
    internal class DataGridTemplateColumnMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGridTemplateColumn.
        /// </summary>
        public DataGridTemplateColumnMetadata()
            : base()
        {
            AddCallback(
                SilverlightTypes.DataGridTemplateColumn,
                b =>
                {
                    // Common
                    b.AddCustomAttributes(PlatformTypes.DataGridTemplateColumn.CellEditingTemplateProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(PlatformTypes.DataGridTemplateColumn.CellTemplateProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(PlatformTypes.DataGridTemplateColumn.SortMemberPathProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                       PlatformTypes.DataGridTemplateColumn.HeaderProperty.Name));
                    b.AddCustomAttributes(
                        PlatformTypes.DataGridTemplateColumn.CellEditingTemplateProperty.Name,
                        new DataContextValueSourceAttribute(
                            PlatformTypes.DataGrid.ItemsSourceProperty.Name,
                            @"(DataGrid.Columns)\",
                            true));
                    b.AddCustomAttributes(
                        PlatformTypes.DataGridTemplateColumn.CellTemplateProperty.Name,
                        new DataContextValueSourceAttribute(
                            PlatformTypes.DataGrid.ItemsSourceProperty.Name,
                            @"(DataGrid.Columns)\",
                            true));               
                });
        }
    }
}
