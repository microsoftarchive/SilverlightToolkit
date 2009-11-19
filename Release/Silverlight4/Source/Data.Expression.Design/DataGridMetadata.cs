// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

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
                typeof(SSWC.DataGrid),
                b =>
                {
                    // Common
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.CanUserReorderColumns), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.CanUserReorderColumns), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.CanUserResizeColumns), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.CanUserResizeColumns), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.CanUserSortColumns), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.CanUserSortColumns), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.RowDetailsVisibilityMode), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.RowDetailsVisibilityMode), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.AutoGenerateColumns), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes("Columns", new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.ItemsSource), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.SelectedIndex), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.HeadersVisibility), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.GridLinesVisibility), new CategoryAttribute(Properties.Resources.CommonProperties));

                    // Layout
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.ColumnWidth), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.MaxColumnWidth), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.MaxColumnWidth), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.MinColumnWidth), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.MinColumnWidth), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.RowHeight), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.RowHeaderWidth), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.HorizontalScrollBarVisibility), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.HorizontalScrollBarVisibility), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.VerticalScrollBarVisibility), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.VerticalScrollBarVisibility), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));

                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.RowDetailsTemplate), new DataContextValueSourceAttribute("ItemsSource", true));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.RowStyle), new DataContextValueSourceAttribute("ItemsSource", true));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGrid>(x => x.CellStyle), new DataContextValueSourceAttribute("ItemsSource", true));

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute(Extensions.GetMemberName<SSWC.DataGrid>(x => x.ItemsSource)));
                });
        }
    }
}
