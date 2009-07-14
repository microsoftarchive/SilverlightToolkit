// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataGrid.
    /// </summary>
    internal class DataGridColumnMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGrid.
        /// </summary>
        public DataGridColumnMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DataGridColumn),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridColumn>(x => x.Width), PropertyValueEditor.CreateEditorAttribute(typeof(DataGridLengthEditor)));
                });
        }
    }
}
