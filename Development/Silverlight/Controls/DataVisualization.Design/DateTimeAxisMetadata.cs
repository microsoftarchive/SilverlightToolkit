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
    /// To register design time metadata for DateTimeAxis.
    /// </summary>
    internal class DateTimeAxisMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DateTimeAxis.
        /// </summary>
        public DateTimeAxisMetadata()
            : base()
        {
            AddCallback(
                typeof(DateTimeAxis),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<DateTimeAxis>(x => x.Interval), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<DateTimeAxis>(x => x.IntervalType), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<DateTimeAxis>(x => x.Maximum), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<DateTimeAxis>(x => x.Minimum), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
