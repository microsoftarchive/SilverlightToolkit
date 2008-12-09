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
    /// To register design time metadata for Series.
    /// </summary>
    internal class SeriesMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Series.
        /// </summary>
        public SeriesMetadata()
            : base()
        {
            AddCallback(
                typeof(Series),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<Series>(x => x.Title), new CategoryAttribute(Properties.Resources.DataVisualization));
                    ////b.AddCustomAttributes(Extensions.GetMemberName<Series>(x => x.LegendItems), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes("LegendItems", new CategoryAttribute(Properties.Resources.DataVisualization));
                });
        }
    }
}
