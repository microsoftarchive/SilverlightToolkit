// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using Microsoft.Windows.Controls.DataVisualization.Charting;
using Microsoft.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace Microsoft.Windows.Controls.DataVisualization.Design
{
    /// <summary>
    /// To register design time metadata for Axis.
    /// </summary>
    internal class AxisMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Axis.
        /// </summary>
        public AxisMetadata()
            : base()
        {
            AddCallback(
                typeof(Axis),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<Axis>(x => x.Location), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<Axis>(x => x.Orientation), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
