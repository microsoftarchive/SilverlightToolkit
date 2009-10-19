// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using System.Windows;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for TreeView.
    /// </summary>
    internal class TreeViewMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for TreeView.
        /// </summary>
        /// 
        public TreeViewMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.TreeView),
                b =>
                {
#if MWD40
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TreeView>(x => x.SelectedValue),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWC.TreeView>(x => x.SelectedValuePath),
                            false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TreeView>(x => x.SelectedItem),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWC.TreeView>(x => x.ItemsSource),
                            true));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TreeView>(x => x.SelectedValuePath),
                        new DataContextValueSourceAttribute(
                            Extensions.GetMemberName<SSWC.TreeView>(x => x.ItemsSource),
                            true));
                    b.AddCustomAttributes(
                    Extensions.GetMemberName<HierarchicalDataTemplate>(x => x.ItemContainerStyle),
                    new DataContextValueSourceAttribute(
                        Extensions.GetMemberName<HierarchicalDataTemplate>(x => x.ItemsSource),
                        true));
                    b.AddCustomAttributes(
                    Extensions.GetMemberName<HierarchicalDataTemplate>(x => x.ItemTemplate),
                    new DataContextValueSourceAttribute(
                        Extensions.GetMemberName<HierarchicalDataTemplate>(x => x.ItemsSource),
                        true));
#endif

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TreeView>(x => x.Items),
                        new NewItemTypesAttribute(typeof(SSWC.TreeViewItem)));
                });
        }
    }
}
