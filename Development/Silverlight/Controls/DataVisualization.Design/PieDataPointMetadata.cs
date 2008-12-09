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
    /// To register design time metadata for PieDataPoint.
    /// </summary>
    internal class PieDataPointMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for PieDataPoint.
        /// </summary>
        public PieDataPointMetadata()
            : base()
        {
            AddCallback(
                typeof(PieDataPoint),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<PieDataPoint>(x => x.ActualOffsetRatio), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<PieDataPoint>(x => x.ActualRatio), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<PieDataPoint>(x => x.FormattedRatio), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<PieDataPoint>(x => x.Geometry), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<PieDataPoint>(x => x.GeometryHighlight), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<PieDataPoint>(x => x.GeometrySelection), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<PieDataPoint>(x => x.OffsetRatio), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<PieDataPoint>(x => x.Ratio), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<PieDataPoint>(x => x.RatioStringFormat), new CategoryAttribute(Properties.Resources.DataVisualization));
                });
        }
    }
}
