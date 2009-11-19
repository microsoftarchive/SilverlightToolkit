// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Data.Design.Silverlight;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SWC = System.Windows.Controls;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataGridBoundColumn.
    /// </summary>
    internal class DataGridRowHeaderMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGridBoundColumn.
        /// </summary>
        public DataGridRowHeaderMetadata()
            : base()
        {
            AddCallback(
                SilverlightTypes.DataGridRowHeader, b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                      PlatformTypes.DataGridRowHeader.SeparatorVisibilityProperty.Name));
                });
        }
    }
}
