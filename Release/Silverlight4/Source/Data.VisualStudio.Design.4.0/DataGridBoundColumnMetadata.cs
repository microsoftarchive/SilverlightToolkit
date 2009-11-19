// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Data.Design.Silverlight;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataGridBoundColumn.
    /// </summary>
    internal class DataGridBoundColumnMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGridBoundColumn.
        /// </summary>
        public DataGridBoundColumnMetadata()
            : base()
        {
            AddCallback(
                SilverlightTypes.DataGridBoundColumn, b =>
                {
                    // Common
                    b.AddCustomAttributes(PlatformTypes.DataGridBoundColumn.BindingProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(PlatformTypes.DataGridBoundColumn.EditingElementStyleProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(PlatformTypes.DataGridBoundColumn.ElementStyleProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                       PlatformTypes.DataGridBoundColumn.BindingProperty.Name));
                    b.AddCustomAttributes(
                        PlatformTypes.DataGridBoundColumn.BindingProperty.Name,
                        new DataContextValueSourceAttribute(
                            PlatformTypes.DataGrid.ItemsSourceProperty.Name,
                            @"(DataGrid.Columns)\",
                            true));
                });
        }
    }
}
