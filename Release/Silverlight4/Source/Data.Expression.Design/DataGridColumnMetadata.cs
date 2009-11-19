// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

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
                typeof(SSWC.DataGridColumn),
                b =>
                {
                    // Common
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.CanUserReorder), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.CanUserResize), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.CanUserSort), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.Header), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.DisplayIndex), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.IsReadOnly), new CategoryAttribute(Properties.Resources.CommonProperties));

                    // Appearance
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.Visibility), new CategoryAttribute(Properties.Resources.Appearance));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.SortMemberPath), new CategoryAttribute(Properties.Resources.Appearance));

                    // Layout
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.MaxWidth), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.MaxWidth), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.MaxWidth), new DefaultValueAttribute(double.PositiveInfinity));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.MinWidth), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.MinWidth), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.Width), new CategoryAttribute(Properties.Resources.Layout));
                });
        }
    }
}
