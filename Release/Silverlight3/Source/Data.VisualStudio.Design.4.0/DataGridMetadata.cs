// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Data.Design.Silverlight;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataGrid.
    /// </summary>
    internal class DataGridMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGrid.
        /// </summary>
        public DataGridMetadata()
            : base()
        {
            AddCallback(
                SilverlightTypes.DataGrid, b =>
                {
                    b.AddCustomAttributes(new ComplexBindingPropertiesAttribute(PlatformTypes.DataGrid.ItemsSourceProperty.Name));
                    b.AddCustomAttributes(new DefaultEventAttribute("SelectionChanged"));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(PlatformTypes.DataGrid.ColumnsProperty.Name));

                    // Appearance
                    b.AddCustomAttributes(PlatformTypes.DataGrid.HorizontalScrollBarVisibilityProperty.Name, new CategoryAttribute(Properties.Resources.Appearance));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.VerticalScrollBarVisibilityProperty.Name, new CategoryAttribute(Properties.Resources.Appearance));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.GridLinesVisibilityProperty.Name, new CategoryAttribute(Properties.Resources.Appearance));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.HeadersVisibilityProperty.Name, new CategoryAttribute(Properties.Resources.Appearance));

                    // Brushes
                    b.AddCustomAttributes(PlatformTypes.DataGrid.HorizontalGridLinesBrushProperty.Name, new CategoryAttribute(Properties.Resources.Brushes));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.VerticalGridLinesBrushProperty.Name, new CategoryAttribute(Properties.Resources.Brushes));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.AlternatingRowBackgroundProperty.Name, new CategoryAttribute(Properties.Resources.Brushes));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.RowBackgroundProperty.Name, new CategoryAttribute(Properties.Resources.Brushes));

                    // Columns
                    b.AddCustomAttributes(PlatformTypes.DataGrid.AutoGenerateColumnsProperty.Name, new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.CanUserReorderColumnsProperty.Name, new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.CanUserResizeColumnsProperty.Name, new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.CanUserSortColumnsProperty.Name, new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.ColumnsProperty.Name, new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.ColumnWidthProperty.Name, new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.ColumnHeaderHeightProperty.Name, new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.FrozenColumnCountProperty.Name, new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.MaxColumnWidthProperty.Name, new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.MinColumnWidthProperty.Name, new CategoryAttribute(Properties.Resources.Columns));

                    // Common
                    b.AddCustomAttributes(PlatformTypes.DataGrid.IsReadOnlyProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.ItemsSourceProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.SelectedIndexProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));

                    // DataGrid Styling
                    b.AddCustomAttributes(PlatformTypes.DataGrid.CellStyleProperty.Name, new CategoryAttribute(Properties.Resources.DataGridStyling));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.DragIndicatorStyleProperty.Name, new CategoryAttribute(Properties.Resources.DataGridStyling));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.DropLocationIndicatorStyleProperty.Name, new CategoryAttribute(Properties.Resources.DataGridStyling));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.ColumnHeaderStyleProperty.Name, new CategoryAttribute(Properties.Resources.DataGridStyling));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.RowGroupHeaderStylesProperty.Name, new CategoryAttribute(Properties.Resources.DataGridStyling));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.RowHeaderStyleProperty.Name, new CategoryAttribute(Properties.Resources.DataGridStyling));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.RowStyleProperty.Name, new CategoryAttribute(Properties.Resources.DataGridStyling));

                    //Rows
                    b.AddCustomAttributes(PlatformTypes.DataGrid.RowHeaderWidthProperty.Name, new CategoryAttribute(Properties.Resources.Rows));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.AreRowDetailsFrozenProperty.Name, new CategoryAttribute(Properties.Resources.Rows));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.RowDetailsTemplateProperty.Name, new CategoryAttribute(Properties.Resources.Rows));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.RowDetailsVisibilityModeProperty.Name, new CategoryAttribute(Properties.Resources.Rows));
                    b.AddCustomAttributes(PlatformTypes.DataGrid.RowHeightProperty.Name, new CategoryAttribute(Properties.Resources.Rows));

                    // Browsable Attribute
                    b.AddCustomAttributes(PlatformTypes.DataGrid.SelectedItemProperty.Name, BrowsableAttribute.No);
                    b.AddCustomAttributes(PlatformTypes.DataGrid.CurrentColumnProperty.Name, BrowsableAttribute.No);

                    // MenuActions
                    b.AddCustomAttributes(new FeatureAttribute(typeof(DataGridMenuProvider)));

                    // When it is created from the toolbox
                    b.AddCustomAttributes(new FeatureAttribute(typeof(DataGridInitializer)));

#if MWD40
                    b.AddCustomAttributes(
                        PlatformTypes.DataGrid.RowDetailsTemplateProperty.Name,
                        new DataContextValueSourceAttribute(
                            PlatformTypes.DataGrid.ItemsSourceProperty.Name,
                            true));
                    b.AddCustomAttributes(
                        PlatformTypes.DataGrid.SelectedItemProperty.Name,
                        new DataContextValueSourceAttribute(
                            PlatformTypes.DataGrid.ItemsSourceProperty.Name,
                            true));
                    b.AddCustomAttributes(
                        PlatformTypes.DataGrid.RowStyleProperty.Name,
                        new DataContextValueSourceAttribute(
                            PlatformTypes.DataGrid.ItemsSourceProperty.Name,
                            true));
                    b.AddCustomAttributes(
                        PlatformTypes.DataGrid.CellStyleProperty.Name,
                        new DataContextValueSourceAttribute(
                            PlatformTypes.DataGrid.ItemsSourceProperty.Name,
                            true));
#endif
                });
        }
    }
}
