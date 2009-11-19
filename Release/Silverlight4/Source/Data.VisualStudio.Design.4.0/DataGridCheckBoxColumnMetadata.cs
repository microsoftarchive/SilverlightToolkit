// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Data.Design.Silverlight;
using Microsoft.Windows.Design.Metadata;

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
                SilverlightTypes.DataGridCheckBoxColumn,
                b =>
                {
                    b.AddCustomAttributes(PlatformTypes.DataGridCheckBoxColumn.IsThreeStateProperty.Name, new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                       PlatformTypes.DataGridCheckBoxColumn.HeaderProperty.Name));               
                });
        }
    }
}
