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
    /// To register design time metadata for HeaderedItemsControl.
    /// </summary>
    internal class HeaderedItemsControlMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for HeaderedItemsControl.
        /// </summary>
        public HeaderedItemsControlMetadata()
            : base()
        {
            AddCallback(
                typeof(HeaderedItemsControl),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<HeaderedItemsControl>(x => x.Header), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
