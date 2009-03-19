// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Data.DataForm.Design
{
    /// <summary>
    /// To register design time metadata for DescriptionViewer.
    /// </summary>
    public class DescriptionViewerMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DescriptionViewer.
        /// </summary>
        public DescriptionViewerMetadata()
            : base()
        {
            AddCallback(
                typeof(DescriptionViewer),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<DescriptionViewer>(x => x.Description), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<DescriptionViewer>(x => x.PropertyPath), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<DescriptionViewer>(x => x.Target), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
