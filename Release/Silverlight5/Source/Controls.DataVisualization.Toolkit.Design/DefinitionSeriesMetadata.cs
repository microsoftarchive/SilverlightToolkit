// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWCDC = Silverlight::System.Windows.Controls.DataVisualization.Charting;

namespace System.Windows.Controls.DataVisualization.Design
{
    /// <summary>
    /// To register design time metadata for SSWCDC.DefinitionSeries.
    /// </summary>
    internal class DefinitionSeriesMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for SSWCDC.DefinitionSeries.
        /// </summary>
        public DefinitionSeriesMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWCDC.DefinitionSeries),
                b =>
                {
                    // Copied from SeriesMetadata.cs

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.DefinitionSeries>(x => x.SeriesHost),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        "LegendItems",
                        new CategoryAttribute(Properties.Resources.DataVisualization));

                    // Copied common attributes from [Area|Bar|Column|Line]SeriesMetadata.cs

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.DefinitionSeries>(x => x.ActualIndependentAxis),
                        new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.DefinitionSeries>(x => x.IndependentAxis),
                        new CategoryAttribute(Properties.Resources.DataVisualization));
                });
        }
    }
}