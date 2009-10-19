// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
using SSWC = Silverlight::System.Windows.Controls;

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
                typeof(SSWC.DataGridTemplateColumn),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridTemplateColumn>(x => x.CellEditingTemplate), new DataContextValueSourceAttribute("ItemsSource", @"Columns\", true));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridTemplateColumn>(x => x.CellTemplate), new DataContextValueSourceAttribute("ItemsSource", @"Columns\", true));
                });
        }
    }
}
