// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;
using SSWCP = Silverlight::System.Windows.Controls.Primitives;

[assembly: ProvideMetadata(typeof(System.Windows.Controls.VisualStudio.Design.MetadataRegistration))]

namespace System.Windows.Controls.VisualStudio.Design
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
                typeof(SSWCP.GlobalCalendarButton),
                b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(
                typeof(SSWCP.GlobalCalendarDayButton),
                b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(
                typeof(SSWCP.GlobalCalendarItem),
                b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

            builder.AddCallback(
               typeof(SSWC.TreeViewItemCheckBox),
               b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

            builder.AddCallback(
               typeof(SSWC.ListBoxDragDropTarget),
               b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(
               typeof(SSWC.TreeViewDragDropTarget),
               b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        }
    }
}