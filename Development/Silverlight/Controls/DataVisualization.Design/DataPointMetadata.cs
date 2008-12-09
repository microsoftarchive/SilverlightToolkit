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
    /// To register design time metadata for DataPoint.
    /// </summary>
    internal class DataPointMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataPoint.
        /// </summary>
        public DataPointMetadata()
            : base()
        {
            AddCallback(
                typeof(DataPoint),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<DataPoint>(x => x.ActualDependentValue), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataPoint>(x => x.ActualIndependentValue), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataPoint>(x => x.DependentValue), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataPoint>(x => x.DependentValueStringFormat), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataPoint>(x => x.FormattedDependentValue), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataPoint>(x => x.FormattedIndependentValue), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataPoint>(x => x.IndependentValue), new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(Extensions.GetMemberName<DataPoint>(x => x.IndependentValueStringFormat), new CategoryAttribute(Properties.Resources.DataVisualization));

                    b.AddCustomAttributes(Extensions.GetMemberName<DataPoint>(x => x.IsSelectionEnabled), new BrowsableAttribute(false));
                });
        }
    }
}
