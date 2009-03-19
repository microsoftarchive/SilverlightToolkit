//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls.Design.Common;
using System.Windows.Controls.Primitives;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;

[assembly: ProvideMetadata(typeof(System.Windows.Controls.Data.Expression.Design.MetadataRegistration))]

namespace System.Windows.Controls.Data.Expression.Design
{
    /// <summary>
    /// MetadataRegistration class.
    /// </summary>
    public partial class MetadataRegistration : MetadataRegistrationBase, IProvideAttributeTable
    {
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
            // Note: everything added here must be duplicated in VisualStudio.Design as well!
            builder.AddCallback(typeof(DataGridCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DataGridRow), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DataGridCellsPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DataGridColumnHeader), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DataGridColumnHeadersPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DataGridDetailsPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DataGridFrozenGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DataGridRowHeader), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DataGridRowsPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DataGridRowGroupHeader), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        }
    }
}
