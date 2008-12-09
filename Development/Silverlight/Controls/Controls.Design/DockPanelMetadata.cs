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
    /// To register design time metadata for DockPanel.
    /// </summary>
    internal class DockPanelMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DockPanel.
        /// </summary>
        public DockPanelMetadata()
            : base()
        {
            AddCallback(
                typeof(DockPanel),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<DockPanel>(x => x.LastChildFill), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
