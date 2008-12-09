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
    /// To register design time metadata for BubbleSeries.
    /// </summary>
    internal class BubbleSeriesMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for BubbleSeries.
        /// </summary>
        public BubbleSeriesMetadata()
            : base()
        {
            AddCallback(
                typeof(BubbleSeries),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<BubbleSeries>(x => x.ActualDependentRangeAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<BubbleSeries>(x => x.ActualIndependentRangeAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<BubbleSeries>(x => x.DependentRangeAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<BubbleSeries>(x => x.IndependentRangeAxis), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<BubbleSeries>(x => x.SizeValueBinding), new CategoryAttribute(Properties.Resources.DataVisualization));
                });
        }
    }
}
