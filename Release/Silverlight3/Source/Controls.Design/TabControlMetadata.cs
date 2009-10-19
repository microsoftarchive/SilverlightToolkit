// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for TabControl.
    /// </summary>
    internal class TabControlMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for TabControl.
        /// </summary>
        public TabControlMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.TabControl),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TabControl>(x => x.TabStripPlacement),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TabControl>(x => x.SelectedIndex),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TabControl>(x => x.Items),
                        new NewItemTypesAttribute(typeof(SSWC.TabItem)));

#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Controls, true));
#endif
                });
        }
    }
}
