// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataGrid.
    /// </summary>
    public class DataGridMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGrid.
        /// </summary>
        public DataGridMetadata()
            : base()
        {
            AddCallback(
                typeof(DataGrid),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.CanUserReorderColumns), new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.CanUserResizeColumns), new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.CanUserSortColumns), new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes("Columns", new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.ColumnWidth), new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.FrozenColumnCount), new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.MaxColumnWidth), new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.MinColumnWidth), new CategoryAttribute(Properties.Resources.Columns));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.SelectedIndex), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.GridLinesVisibility), new CategoryAttribute(Properties.Resources.GridLines));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.HorizontalGridLinesBrush), new CategoryAttribute(Properties.Resources.GridLines));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.VerticalGridLinesBrush), new CategoryAttribute(Properties.Resources.GridLines));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.ColumnHeaderHeight), new CategoryAttribute(Properties.Resources.Headers));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.HeadersVisibility), new CategoryAttribute(Properties.Resources.Headers));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.RowHeaderWidth), new CategoryAttribute(Properties.Resources.Headers));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.AlternatingRowBackground), new CategoryAttribute(Properties.Resources.Rows));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.AreRowDetailsFrozen), new CategoryAttribute(Properties.Resources.Rows));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.RowBackground), new CategoryAttribute(Properties.Resources.Rows));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.RowDetailsVisibilityMode), new CategoryAttribute(Properties.Resources.Rows));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.RowHeight), new CategoryAttribute(Properties.Resources.Rows));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGrid>(x => x.SelectionMode), new CategoryAttribute(Properties.Resources.Rows));
                });
        }
    }
}
