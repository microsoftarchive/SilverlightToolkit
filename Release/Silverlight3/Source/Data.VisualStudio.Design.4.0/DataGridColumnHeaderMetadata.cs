// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Data.Design.Silverlight;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataGridBoundColumn.
    /// </summary>
    internal class DataGridColumnHeaderMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGridBoundColumn.
        /// </summary>
        public DataGridColumnHeaderMetadata()
            : base()
        {
            AddCallback(
                SilverlightTypes.DataGridColumnHeader, b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                      PlatformTypes.DataGridColumnHeader.SeparatorVisibilityProperty.Name));
                });
        }
    }
}
