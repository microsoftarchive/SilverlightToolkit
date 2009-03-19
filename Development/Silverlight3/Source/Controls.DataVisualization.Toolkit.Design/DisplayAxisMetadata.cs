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
    /// To register design time metadata for DisplayAxis.
    /// </summary>
    internal class DisplayAxisMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DisplayAxis.
        /// </summary>
        public DisplayAxisMetadata()
            : base()
        {
            AddCallback(
                typeof(DisplayAxis),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<DisplayAxis>(x => x.Title), new CategoryAttribute(Properties.Resources.DataVisualization));

                    b.AddCustomAttributes(Extensions.GetMemberName<DisplayAxis>(x => x.AxisLabelStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<DisplayAxis>(x => x.GridLineStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<DisplayAxis>(x => x.MajorTickMarkStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<DisplayAxis>(x => x.TitleStyle), new CategoryAttribute(Properties.Resources.DataVisualizationStyling));

                    b.AddCustomAttributes(Extensions.GetMemberName<DisplayAxis>(x => x.ShowGridLines), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
