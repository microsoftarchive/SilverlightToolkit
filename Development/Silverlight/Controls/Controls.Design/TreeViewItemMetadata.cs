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
                typeof(TreeViewItem),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<TreeViewItem>(x => x.Header), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<TreeViewItem>(x => x.IsExpanded), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<TreeViewItem>(x => x.IsSelected), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
