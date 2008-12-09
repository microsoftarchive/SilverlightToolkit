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
    /// To register design time metadata for LineSeries.
    /// </summary>
    internal class LineSeriesMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for LineSeries.
        /// </summary>
        public LineSeriesMetadata()
            : base()
        {
            AddCallback(
                typeof(LineSeries),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<LineSeries>(x => x.ActualDependentRangeAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<LineSeries>(x => x.ActualIndependentRangeAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<LineSeries>(x => x.DependentRangeAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<LineSeries>(x => x.IndependentRangeAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<LineSeries>(x => x.MarkerHeight), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<LineSeries>(x => x.MarkerWidth), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<LineSeries>(x => x.Points), new CategoryAttribute(Properties.Resources.DataVisualization));

                    b.AddCustomAttributes(Extensions.GetMemberName<LineSeries>(x => x.PolylineStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                });
        }
    }
}
