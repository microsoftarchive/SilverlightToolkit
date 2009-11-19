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
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

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
            // DefaultBindingPropertyAttribute
            builder.AddCallback(typeof(SSWC.DataGrid), b => b.AddCustomAttributes(new DefaultBindingPropertyAttribute(Extensions.GetMemberName<SSWC.DataGrid>(x => x.ItemsSource))));
            builder.AddCallback(typeof(SSWC.DataGridBoundColumn), b => b.AddCustomAttributes(new DefaultBindingPropertyAttribute(Extensions.GetMemberName<SSWC.DataGridBoundColumn>(x => x.Binding))));
        }
    }
}
