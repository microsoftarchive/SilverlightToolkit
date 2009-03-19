using System.Reflection;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

[assembly: ProvideMetadata(typeof(System.Windows.Controls.Input.Expression.Design.MetadataRegistration))]

namespace System.Windows.Controls.Input.Expression.Design
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
        }
    }
}
