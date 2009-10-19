// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Data.Design.Silverlight;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataGridColumn.
    /// </summary>
    internal class DataGridColumnMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGridColumn.
        /// </summary>
        public DataGridColumnMetadata()
            : base()
        {
            AddCallback(
                SilverlightTypes.DataGridColumn,
                b =>
                {
                    // Common
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.CanUserReorderProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.CanUserResizeProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.CanUserSortProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.HeaderProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.IsReadOnlyProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));

                    // Appearance
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.VisibilityProperty.Name, new CategoryAttribute(Properties.Resources.Appearance));

                    // Layout
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.MaxWidthProperty.Name, new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.MinWidthProperty.Name, new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.WidthProperty.Name, new CategoryAttribute(Properties.Resources.Layout));

                    // DataGrid Styling
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.CellStyleProperty.Name, new CategoryAttribute(Properties.Resources.DataGridStyling));
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.DragIndicatorStyleProperty.Name, new CategoryAttribute(Properties.Resources.DataGridStyling));
                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.HeaderStyleProperty.Name, new CategoryAttribute(Properties.Resources.DataGridStyling));

                    b.AddCustomAttributes(PlatformTypes.DataGridColumn.HeaderProperty.Name, new TypeConverterAttribute(typeof(StringConverter)));

                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                       PlatformTypes.DataGridColumn.HeaderProperty.Name)); 
                });
        }
    }
}
