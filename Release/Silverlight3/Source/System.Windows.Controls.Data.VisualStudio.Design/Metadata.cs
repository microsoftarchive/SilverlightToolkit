//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using System.Windows.Controls.Primitives;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;
using SSWCP = Silverlight::System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Data.VisualStudio.Design
{
    /// <summary>
    /// MetadataRegistration class.
    /// </summary>
    public partial class MetadataRegistration : MetadataRegistrationBase, IRegisterMetadata
    {
        /// <summary>
        /// Borrowed from System.Windows.Controls.Toolbox.Design.MetadataRegistration:
        /// use a static flag to ensure metadata is registered only once.
        /// </summary>
        private static bool _initialized;

        /// <summary>
        /// Called by tools to register design time metadata.
        /// </summary>
        public void Register()
        {
            if (!_initialized)
            {
                MetadataStore.AddAttributeTable(BuildAttributeTable());
                _initialized = true;
            }
        }

        /// <summary>
        /// Provide a place to add custom attributes without creating a AttributeTableBuilder subclass.
        /// </summary>
        /// <param name="builder">The assembly attribute table builder.</param>
        protected override void AddAttributes(AttributeTableBuilder builder)
        {
            builder.AddCallback(typeof(SSWC.DataPager), b => b.AddCustomAttributes(new DefaultBindingPropertyAttribute("Source")));
            builder.AddCallback(typeof(SSWC.DataGridCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SSWC.DataGridRow), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SSWCP.DataGridCellsPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SSWCP.DataGridColumnHeader), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SSWCP.DataGridColumnHeadersPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SSWCP.DataGridDetailsPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SSWCP.DataGridFrozenGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SSWCP.DataGridRowHeader), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SSWCP.DataGridRowsPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SSWC.DataGridRowGroupHeader), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        }
    }
}
