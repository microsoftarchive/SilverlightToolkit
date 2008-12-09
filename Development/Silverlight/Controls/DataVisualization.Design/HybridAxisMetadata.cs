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
    /// To register design time metadata for HybridAxis.
    /// </summary>
    internal class HybridAxisMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for HybridAxis.
        /// </summary>
        public HybridAxisMetadata()
            : base()
        {
            AddCallback(
                typeof(HybridAxis),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<HybridAxis>(x => x.AxisTitle), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<HybridAxis>(x => x.Title), new CategoryAttribute(Properties.Resources.DataVisualization));

                    b.AddCustomAttributes(Extensions.GetMemberName<HybridAxis>(x => x.AxisLabelStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<HybridAxis>(x => x.GridLineStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<HybridAxis>(x => x.MajorTickMarkStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<HybridAxis>(x => x.TitleStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));

                    b.AddCustomAttributes(Extensions.GetMemberName<HybridAxis>(x => x.ShowGridLines), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
