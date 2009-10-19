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
    /// To register design time metadata for TabItem.
    /// </summary>
    internal class TabItemMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for TabItem.
        /// </summary>
        public TabItemMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.TabItem),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TabItem>(x => x.Header),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TabItem>(x => x.IsSelected),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute("Header"));

#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.BasicControls, false));
#endif
                });
        }
    }
}
