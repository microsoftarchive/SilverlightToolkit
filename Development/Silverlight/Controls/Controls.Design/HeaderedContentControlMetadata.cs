// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using Microsoft.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace Microsoft.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for HeaderedContentControl.
    /// </summary>
    internal class HeaderedContentControlMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for HeaderedContentControl.
        /// </summary>
        public HeaderedContentControlMetadata()
            : base()
        {
            AddCallback(
                typeof(HeaderedContentControl),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<HeaderedContentControl>(x => x.Header), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
