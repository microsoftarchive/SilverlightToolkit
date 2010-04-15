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
                SilverlightTypes.DataGridTextColumn,
                b =>
                {
                    // Text
                    b.AddCustomAttributes(PlatformTypes.DataGridTextColumn.FontFamilyProperty.Name, new CategoryAttribute(Properties.Resources.Text));
                    b.AddCustomAttributes(PlatformTypes.DataGridTextColumn.FontSizeProperty.Name, new CategoryAttribute(Properties.Resources.Text));
                    b.AddCustomAttributes(PlatformTypes.DataGridTextColumn.FontStyleProperty.Name, new CategoryAttribute(Properties.Resources.Text));
                    b.AddCustomAttributes(PlatformTypes.DataGridTextColumn.FontWeightProperty.Name, new CategoryAttribute(Properties.Resources.Text));

                    // Brushes
                    b.AddCustomAttributes(PlatformTypes.DataGridTextColumn.ForegroundProperty.Name, new CategoryAttribute(Properties.Resources.Brushes));
                });
        }
    }
}
