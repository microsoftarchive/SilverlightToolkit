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
    /// To register design time metadata for SSWCDC.SeriesDefinition.
    /// </summary>
    internal class SeriesDefinitionMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for SSWCDC.SeriesDefinition.
        /// </summary>
        public SeriesDefinitionMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWCDC.SeriesDefinition),
                b =>
                {
                    // Copied from SeriesMetadata.cs
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.Title),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        "LegendItems",
                        new CategoryAttribute(Properties.Resources.DataVisualization));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.Title),
                        PropertyValueEditor.CreateEditorAttribute(typeof(TextBoxEditor)));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.Title),
                        new AlternateContentPropertyAttribute());

                    // Copied from DataPointSeriesMetadata.cs
                    b.AddCustomAttributes(
                        "DependentValueBinding",
                        new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(
                        "IndependentValueBinding",
                        new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.TransitionDuration),
                        new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.TransitionEasingFunction),
                        new CategoryAttribute(Properties.Resources.DataVisualization));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.LegendItemStyle),
                        new CategoryAttribute(Properties.Resources.DataVisualizationStyling));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.DependentValuePath),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.IndependentValuePath),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.ItemsSource),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.IndependentValuePath),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.ItemsSource),
                            true));
                    b.AddCustomAttributes(
                        "IndependentValueBinding",
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.ItemsSource),
                            true));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.DependentValuePath),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.ItemsSource),
                            true));
                    b.AddCustomAttributes(
                        "DependentValueBinding",
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWCDC.SeriesDefinition>(x => x.ItemsSource),
                            true));
                });
        }
    }
}