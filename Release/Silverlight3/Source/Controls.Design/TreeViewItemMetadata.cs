// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for TreeViewItem.
    /// </summary>
    internal class TreeViewItemMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for TreeViewItem.
        /// </summary>
        public TreeViewItemMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.TreeViewItem),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TreeViewItem>(x => x.Header),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TreeViewItem>(x => x.IsExpanded),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TreeViewItem>(x => x.IsSelected),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TreeViewItem>(x => x.Items),
                        new NewItemTypesAttribute(typeof(SSWC.TreeViewItem)));

#if MWD40
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TreeViewItem>(x => x.Header),
                        new AlternateContentPropertyAttribute());

                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.BasicControls, false));

                    b.AddCustomAttributes(new FeatureAttribute(typeof(TreeViewItemIsExpandedDesignModeValueProvider)));
                    b.AddCustomAttributes(new FeatureAttribute(typeof(TreeViewItemIsExpandedDesignModeValueProvider.AdornerProxy)));
#endif
                });
        }
    }
}
