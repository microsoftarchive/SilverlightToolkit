// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using System.Windows.Controls.Primitives;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataGridRowHeader.
    /// </summary>
    public class DataGridRowHeaderMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGridRowHeader.
        /// </summary>
        public DataGridRowHeaderMetadata()
            : base()
        {
            AddCallback(
                typeof(DataGridRowHeader),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<DataGridRowHeader>(x => x.SeparatorVisibility), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
