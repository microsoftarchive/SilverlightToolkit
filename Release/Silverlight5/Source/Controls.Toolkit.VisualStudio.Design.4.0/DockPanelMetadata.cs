// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
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
                typeof(SSWC.DockPanel),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.DockPanel>(x => x.LastChildFill)));
                });
        }
    }
}
