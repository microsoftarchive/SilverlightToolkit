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
    /// To register design time metadata for DataGridCheckBoxColumn.
    /// </summary>
    internal class DataGridCheckBoxColumnMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGridCheckBoxColumn.
        /// </summary>
        public DataGridCheckBoxColumnMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DataGridCheckBoxColumn),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataGridCheckBoxColumn>(x => x.IsThreeState), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
