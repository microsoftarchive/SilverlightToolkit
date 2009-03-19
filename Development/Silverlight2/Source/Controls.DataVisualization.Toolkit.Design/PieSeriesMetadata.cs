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
    /// To register design time metadata for PieSeries.
    /// </summary>
    internal class PieSeriesMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for PieSeries.
        /// </summary>
        public PieSeriesMetadata()
            : base()
        {
            AddCallback(
                typeof(PieSeries),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<PieSeries>(x => x.StylePalette), 
                        new CategoryAttribute(Properties.Resources.DataVisualizationStyling));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<PieSeries>(x => x.Title),
                        PropertyValueEditor.CreateEditorAttribute(typeof(TextBoxEditor)));
                });
        }
    }
}
