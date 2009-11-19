// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using Microsoft.Windows.Design;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;
using Microsoft.Windows.Design.Features;
using System.Reflection;

[assembly: ProvideMetadata(typeof(System.Windows.Controls.Input.VisualStudio.Design.MetadataRegistration))]

namespace System.Windows.Controls.Input.VisualStudio.Design
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
            typeof(SSWC.AutoCompleteBox),
            b =>
            {
                GenericDefaultInitializer.AddDefault<SSWC.AutoCompleteBox>(x => x.Height, 28d);
                GenericDefaultInitializer.AddDefault<SSWC.AutoCompleteBox>(x => x.Width, 120d);

                b.AddCustomAttributes(new FeatureAttribute(typeof(GenericDefaultInitializer)));

                b.AddCustomAttributes(new DefaultPropertyAttribute(
                    Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.Text)));
                b.AddCustomAttributes(new DefaultEventAttribute("SelectionChanged"));
                b.AddCustomAttributes(new ComplexBindingPropertiesAttribute(
                    Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.ItemsSource),
                    Extensions.GetMemberName<SSWC.AutoCompleteBox>(x => x.ValueMemberPath)));
            });
        }
    }
}