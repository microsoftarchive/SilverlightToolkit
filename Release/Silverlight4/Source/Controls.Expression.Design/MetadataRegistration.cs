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

[assembly: ProvideMetadata(typeof(System.Windows.Controls.Expression.Design.MetadataRegistration))]

namespace System.Windows.Controls.Expression.Design
{
    /// <summary>
    /// MetadataRegistration class.
    /// </summary>
    public partial class MetadataRegistration : MetadataRegistrationBase, IProvideAttributeTable
    {
        /// <summary>
        /// Gets the AttributeTable for design time metadata.
        /// </summary>
        public AttributeTable AttributeTable
        {
            get
            {
                return BuildAttributeTable();
            }
        }

        /// <summary>
        /// Provide a place to add custom attributes without creating a AttributeTableBuilder subclass.
        /// </summary>
        /// <param name="builder">The assembly attribute table builder.</param>
        protected override void AddAttributes(AttributeTableBuilder builder)
        {
            builder.AddCallback(
                typeof(SSWC.Primitives.TabPanel),
                b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));

            builder.AddCallback(
                typeof(SSWC.TabControl),
                b => b.AddCustomAttributes(new DefaultBindingPropertyAttribute(String.Empty)));
            builder.AddCallback(
                typeof(SSWC.TreeView),
                b => b.AddCustomAttributes(new DefaultBindingPropertyAttribute(
                    Extensions.GetMemberName<SSWC.TreeView>(x => x.ItemsSource))));
            builder.AddCallback(
                typeof(SSWC.TreeViewItem),
                b => b.AddCustomAttributes(new DefaultBindingPropertyAttribute(
                    Extensions.GetMemberName<SSWC.TreeViewItem>(x => x.ItemsSource))));
            builder.AddCallback(
                typeof(SSWC.HeaderedItemsControl),
                b => b.AddCustomAttributes(new DefaultBindingPropertyAttribute(
                    Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.Items))));
        }
    }
}
