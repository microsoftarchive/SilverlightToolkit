// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWCD = Silverlight::System.Windows.Controls.DataVisualization;

namespace System.Windows.Controls.DataVisualization.Design
{
    /// <summary>
    /// To register design time metadata for SSWCD.Legend.
    /// </summary>
    internal class LegendMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for SSWCD.Legend.
        /// </summary>
        public LegendMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWCD.Legend),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWCD.Legend>(x => x.Header)));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCD.Legend>(x => x.Header),
                        new TypeConverterAttribute(typeof(StringConverter)));

                    b.AddCustomAttributes(
                       new ToolboxBrowsableAttribute(false));
                });
        }
    }
}