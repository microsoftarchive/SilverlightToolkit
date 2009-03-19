// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;

namespace System.Windows.Controls.DataVisualization.Design
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
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<BubbleSeries>(x => x.ActualDependentRangeAxis), 
                        new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<BubbleSeries>(x => x.ActualIndependentAxis), 
                        new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<BubbleSeries>(x => x.DependentRangeAxis), 
                        new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<BubbleSeries>(x => x.IndependentAxis), 
                        new CategoryAttribute(Properties.Resources.DataVisualization));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<BubbleSeries>(x => x.SizeValueBinding), 
                        new BrowsableAttribute(false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<BubbleSeries>(x => x.SizeValuePath), 
                        new CategoryAttribute(Properties.Resources.DataVisualization));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<BubbleSeries>(x => x.DataPointStyle), 
                        new CategoryAttribute(Properties.Resources.DataVisualizationStyling));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<BubbleSeries>(x => x.DependentRangeAxis), 
                        new TypeConverterAttribute(typeof(ExpandableObjectConverter)));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<BubbleSeries>(x => x.IndependentAxis), 
                        new TypeConverterAttribute(typeof(ExpandableObjectConverter)));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<BubbleSeries>(x => x.Title),
                        PropertyValueEditor.CreateEditorAttribute(typeof(TextBoxEditor)));
                });
        }
    }
}
