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
    /// To register design time metadata for DataGridRow.
    /// </summary>
    public class DataGridRowMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGridRow.
        /// </summary>
        public DataGridRowMetadata()
            : base()
        {
            AddCallback(
                typeof(DataGridRow),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGridRow>(x => x.DetailsVisibility), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGridRow>(x => x.Header), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGridRow>(x => x.HeaderStyle), new CategoryAttribute(Properties.Resources.DataGridStyling));
                });
        }
    }
}
