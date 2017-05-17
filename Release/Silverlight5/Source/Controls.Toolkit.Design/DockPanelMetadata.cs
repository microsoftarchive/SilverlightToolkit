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
using Microsoft.Windows.Design.Features;

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
                    b.AddCustomAttributes("GetDock", new DefaultValueAttribute(null), new AttachedPropertyBrowsableForChildrenAttribute());

                    b.AddCustomAttributes(new FeatureAttribute(typeof(DockPanelParentAdapter)));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DockPanel>(x => x.LastChildFill),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Controls, true));
#endif
                });
        }
    }
}
