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
    /// To register design time metadata for ColumnSeries.
    /// </summary>
    internal class ColumnSeriesMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for ColumnSeries.
        /// </summary>
        public ColumnSeriesMetadata()
            : base()
        {
            AddCallback(
                typeof(ColumnSeries),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<ColumnSeries>(x => x.ActualDependentRangeAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<ColumnSeries>(x => x.ActualIndependentCategoryAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<ColumnSeries>(x => x.DependentRangeAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<ColumnSeries>(x => x.IndependentCategoryAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                });
        }
    }
}
