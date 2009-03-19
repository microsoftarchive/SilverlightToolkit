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
    /// To register design time metadata for DataPointSeries.
    /// </summary>
    internal class DataPointSeriesMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataPointSeries.
        /// </summary>
        public DataPointSeriesMetadata()
            : base()
        {
            AddCallback(
                typeof(DataPointSeries),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<DataPointSeries>(x => x.SelectedItem),
                        new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<DataPointSeries>(x => x.TransitionDuration),
                        new CategoryAttribute(Properties.Resources.DataVisualization));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<DataPointSeries>(x => x.LegendItemStyle),
                        new CategoryAttribute(Properties.Resources.DataVisualizationStyling));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<DataPointSeries>(x => x.AnimationSequence),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<DataPointSeries>(x => x.DependentValueBinding),
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<DataPointSeries>(x => x.DependentValuePath),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<DataPointSeries>(x => x.IndependentValueBinding),
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<DataPointSeries>(x => x.IndependentValuePath),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<DataPointSeries>(x => x.IsSelectionEnabled),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<DataPointSeries>(x => x.ItemsSource),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
