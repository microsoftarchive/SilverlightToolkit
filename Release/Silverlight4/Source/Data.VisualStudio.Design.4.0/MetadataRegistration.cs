// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Windows.Design.Metadata;
using System.Windows.Controls.Data.Design.Silverlight;
using Microsoft.Windows.Design;

[assembly: ProvideMetadata(typeof(System.Windows.Controls.Data.VisualStudio.Design.MetadataRegistration))]

namespace System.Windows.Controls.Data.VisualStudio.Design
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
            builder.AddCallback(SilverlightTypes.DataGridCell, b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(SilverlightTypes.DataGridRow, b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(SilverlightTypes.DataGridCellsPresenter, b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(SilverlightTypes.DataGridColumnHeader, b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(SilverlightTypes.DataGridColumnHeadersPresenter, b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(SilverlightTypes.DataGridDetailsPresenter, b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(SilverlightTypes.DataGridFrozenGrid, b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(SilverlightTypes.DataGridRowHeader, b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(SilverlightTypes.DataGridRowsPresenter, b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(SilverlightTypes.DataGridRowGroupHeader, b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        }
    }
}
