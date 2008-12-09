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
    /// To register design time metadata for Chart.
    /// </summary>
    internal class ChartMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Chart.
        /// </summary>
        public ChartMetadata()
            : base()
        {
            AddCallback(
                typeof(Chart),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<Chart>(x => x.ActualAxes), new BrowsableAttribute(false));

                    b.AddCustomAttributes(Extensions.GetMemberName<Chart>(x => x.Axes), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<Chart>(x => x.LegendItems), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<Chart>(x => x.LegendTitle), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<Chart>(x => x.Title), new CategoryAttribute(Properties.Resources.DataVisualization));

                    b.AddCustomAttributes(Extensions.GetMemberName<Chart>(x => x.ChartAreaStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<Chart>(x => x.LegendStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<Chart>(x => x.PlotAreaStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<Chart>(x => x.StylePalette), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<Chart>(x => x.TitleStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));

                    b.AddCustomAttributes(Extensions.GetMemberName<Chart>(x => x.Series), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
