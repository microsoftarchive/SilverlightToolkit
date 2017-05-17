// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
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
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.Series)));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.LegendTitle),
                        new TypeConverterAttribute(typeof(StringConverter)));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCDC.Chart>(x => x.Title),
                        new TypeConverterAttribute(typeof(StringConverter)));
                });
        }
    }
}