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
    /// To register design time metadata for DataGridTextColumn.
    /// </summary>
    internal class DataGridTextColumnMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGridTextColumn.
        /// </summary>
        public DataGridTextColumnMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DataGridTextColumn),
                b =>
                {
                    // Text
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridTextColumn>(x => x.FontFamily), new CategoryAttribute(Properties.Resources.Text));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridTextColumn>(x => x.FontSize), new CategoryAttribute(Properties.Resources.Text));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridTextColumn>(x => x.FontStyle), new CategoryAttribute(Properties.Resources.Text));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridTextColumn>(x => x.FontStyle), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridTextColumn>(x => x.FontWeight), new CategoryAttribute(Properties.Resources.Text));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridTextColumn>(x => x.FontWeight), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));

                    // Brushes
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridTextColumn>(x => x.Foreground), new CategoryAttribute(Properties.Resources.Brushes));
                });
        }
    }
}
