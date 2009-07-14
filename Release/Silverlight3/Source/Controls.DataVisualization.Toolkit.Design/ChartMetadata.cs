// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWCDC = Silverlight::System.Windows.Controls.DataVisualization.Charting;

namespace System.Windows.Controls.DataVisualization.Design
{
    /// <summary>
    /// To register design time metadata for SSWCDC.Chart.
    /// </summary>
    internal class ChartMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for SSWCDC.Chart.
        /// </summary>
        public ChartMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWCDC.Chart),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.ActualAxes),
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.LegendItems),
                        new BrowsableAttribute(false));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.Title),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    // Note: Not using GetMemberName here because that pulls in INotifyPropertyChanged 
                    // from Silverlight's System which conflicts with the desktop's System
                    b.AddCustomAttributes(
                        "Series",
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        "Axes",
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.LegendItems),
                        new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.LegendTitle),
                        new CategoryAttribute(Properties.Resources.DataVisualization));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.ChartAreaStyle),
                        new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.LegendStyle),
                        new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.PlotAreaStyle),
                        new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.StylePalette),
                        new CategoryAttribute(Properties.Resources.DataVisualizationStyling));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.TitleStyle),
                        new CategoryAttribute(Properties.Resources.DataVisualizationStyling));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.LegendTitle),
                        PropertyValueEditor.CreateEditorAttribute(typeof(TextBoxEditor)));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.Title),
                        PropertyValueEditor.CreateEditorAttribute(typeof(TextBoxEditor)));

#if MWD40
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.LegendTitle),
                        new AlternateContentPropertyAttribute());
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.Title),
                        new AlternateContentPropertyAttribute());

                    b.AddCustomAttributes(
                        "Series",
                        new AlternateContentPropertyAttribute());
                    b.AddCustomAttributes(
                        "Axes",
                        new AlternateContentPropertyAttribute());

                    b.AddCustomAttributes(new FeatureAttribute(typeof(ChartDefaultInitializer)));

                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.DataVisualization, true));
#endif
                });
        }
    }
}