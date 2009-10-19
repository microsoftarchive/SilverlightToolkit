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
    internal class DataGridRowMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGridBoundColumn.
        /// </summary>
        public DataGridRowMetadata()
            : base()
        {
            AddCallback(
                SilverlightTypes.DataGridRow, b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                      PlatformTypes.DataGridRow.HeaderProperty.Name));

                    //TypeConverterAttribute
                    b.AddCustomAttributes(PlatformTypes.DataGridRow.HeaderProperty.Name, new TypeConverterAttribute(typeof(StringConverter)));
                });
        }
    }
}
