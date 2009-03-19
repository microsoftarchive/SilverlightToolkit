// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.Design.Common;
using System.Windows.Controls.Primitives;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;

[assembly: ProvideMetadata(typeof(System.Windows.Controls.Layout.Expression.Design.MetadataRegistration))]

namespace System.Windows.Controls.Layout.Expression.Design
{
    /// <summary>
    /// MetadataRegistration class.
    /// </summary>
    public class MetadataRegistration : MetadataRegistrationBase, IProvideAttributeTable
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
            builder.AddCallback(typeof(ExpandableContentControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
        }
    }
}