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
    /// To register design time metadata for WrapPanel.
    /// </summary>
    internal class WrapPanelMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for WrapPanel.
        /// </summary>
        public WrapPanelMetadata()
            : base()
        {
            AddCallback(
                typeof(WrapPanel),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<WrapPanel>(x => x.Orientation), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<WrapPanel>(x => x.ItemHeight), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<WrapPanel>(x => x.ItemWidth), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
