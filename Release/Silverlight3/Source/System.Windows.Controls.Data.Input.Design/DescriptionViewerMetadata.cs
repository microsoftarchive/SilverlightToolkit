// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Data.Input.Design
{
    /// <summary>
    /// To register design time metadata for DescriptionViewer.
    /// </summary>
    internal class DescriptionViewerMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DescriptionViewer.
        /// </summary>
        public DescriptionViewerMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DescriptionViewer),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DescriptionViewer>(x => x.Description), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DescriptionViewer>(x => x.PropertyPath), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DescriptionViewer>(x => x.Target), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
