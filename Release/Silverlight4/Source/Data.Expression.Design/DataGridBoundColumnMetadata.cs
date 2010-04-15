// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// To register design time metadata for DataGridBoundColumn.
    /// </summary>
    internal class DataGridBoundColumnMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataGridBoundColumn.
        /// </summary>
        public DataGridBoundColumnMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DataGridBoundColumn),
                b =>
                {
                    // Common
                    b.AddCustomAttributes("Binding", new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes("Binding", new DataContextValueSourceAttribute("ItemsSource", @"Columns\", true));
                });
        }
    }
}
