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
using SSW = Silverlight::System.Windows;
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

                    // Use property name strings to avoid ISupportInitialize system.dll collision.
                    b.AddCustomAttributes("ItemContainerStyle", new DataContextValueSourceAttribute("ItemsSource", true));
                    b.AddCustomAttributes("ItemTemplate", new DataContextValueSourceAttribute("ItemsSource", true));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TreeView>(x => x.Items),
                        new NewItemTypesAttribute(typeof(SSWC.TreeViewItem)));
                });
        }
    }
}
