// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.DataVisualization.Design
{
    /// <summary>
    /// To register design time metadata for AxisLabel.
    /// </summary>
    internal class AxisLabelMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for AxisLabel.
        /// </summary>
        public AxisLabelMetadata()
            : base()
        {
            AddCallback(
                typeof(AxisLabel),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<AxisLabel>(x => x.StringFormat), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
