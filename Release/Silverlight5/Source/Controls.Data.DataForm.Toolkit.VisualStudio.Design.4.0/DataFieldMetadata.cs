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
using Microsoft.Windows.Design.Features;

namespace System.Windows.Controls.Data.DataForm.Toolkit.Design
{
    /// <summary>
    /// To register design time metadata for DataField.
    /// </summary>
    internal class DataFieldMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataField.
        /// </summary>
        public DataFieldMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DataField),
                b =>
                {
                    // Defaults
                    GenericDefaultInitializer.AddDefault<SSWC.DataField>(x => x.Height, 28d);
                    GenericDefaultInitializer.AddDefault<SSWC.DataField>(x => x.Width, 200d);

                    b.AddCustomAttributes(new FeatureAttribute(typeof(GenericDefaultInitializer)));

                    b.AddCustomAttributes(new ComplexBindingPropertiesAttribute("DataSource", Extensions.GetMemberName<SSWC.DataField>(x => x.Content)));
                    b.AddCustomAttributes(new DefaultEventAttribute("CurrentItemChanged"));

                    // Common
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.Content), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.Description), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.IsRequired), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.Label), new CategoryAttribute(Properties.Resources.CommonProperties));

                    // Layout
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.DescriptionViewerPosition), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.LabelPosition), new CategoryAttribute(Properties.Resources.Layout));

                    // Appearance
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.DescriptionViewerVisibility), new CategoryAttribute(Properties.Resources.Appearance));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.LabelVisibility), new CategoryAttribute(Properties.Resources.Appearance));

                    // DataFormStyling
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.DescriptionViewerStyle), new CategoryAttribute(Properties.Resources.DataFormStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.LabelStyle), new CategoryAttribute(Properties.Resources.DataFormStyling));

                    // DataContextValueSource
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.DescriptionViewerStyle), new DataContextValueSourceAttribute(Extensions.GetMemberName<SSWC.DataField>(x => x.Description), true));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataField>(x => x.LabelStyle), new DataContextValueSourceAttribute(Extensions.GetMemberName<SSWC.DataField>(x => x.Label), true));

                    // DefaultPropertyAttribute
                    b.AddCustomAttributes(new DefaultPropertyAttribute(Extensions.GetMemberName<SSWC.DataField>(x => x.Label)));

                    // ComplexBindingPropertiesAttribute
                    b.AddCustomAttributes(new ComplexBindingPropertiesAttribute(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.DataContext),
                        Extensions.GetMemberName<SSWC.DataField>(x => x.PropertyPath)));

                    // TypeConverterAttribute
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataField>(x => x.Label),
                        new TypeConverterAttribute(typeof(StringConverter)));
                });
        }
    }
}
