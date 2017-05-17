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
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.Data.DataForm.Toolkit.Design
{
    /// <summary>
    /// To register design time metadata for DataForm.
    /// </summary>
    internal class DataFormMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataForm.
        /// </summary>
        public DataFormMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DataForm), b =>
                {
                    // Defaults
                    GenericDefaultInitializer.AddDefault<SSWC.DataForm>(x => x.Height, 100d);
                    GenericDefaultInitializer.AddDefault<SSWC.DataForm>(x => x.Width, 200d);

                    b.AddCustomAttributes(new FeatureAttribute(typeof(GenericDefaultInitializer)));

                    // Appearance
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.CommandButtonsVisibility), new CategoryAttribute(Properties.Resources.Appearance));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.HeaderVisibility), new CategoryAttribute(Properties.Resources.Appearance));

                    // Common Properties
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.AutoCommit), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.AutoEdit), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.CurrentIndex), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.CurrentItem), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.EditTemplate), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.Header), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.IsReadOnly), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.ItemsSource), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.NewItemTemplate), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.NewItemTemplate), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.ReadOnlyTemplate), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.ReadOnlyTemplate), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));

                    // Layout
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.DescriptionViewerPosition), new CategoryAttribute(Properties.Resources.Layout));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.LabelPosition), new CategoryAttribute(Properties.Resources.Layout));

                    // DataForm Styling
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.CancelButtonStyle), new CategoryAttribute(Properties.Resources.DataFormStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.CommitButtonStyle), new CategoryAttribute(Properties.Resources.DataFormStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.DataFieldStyle), new CategoryAttribute(Properties.Resources.DataFormStyling));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.ValidationSummaryStyle), new CategoryAttribute(Properties.Resources.DataFormStyling));

                    // DataContextValueSource
                    string currentItemName = Extensions.GetMemberName<SSWC.DataForm>(x => x.CurrentItem);
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.EditTemplate), new DataContextValueSourceAttribute(currentItemName, true));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.HeaderTemplate), new DataContextValueSourceAttribute(Extensions.GetMemberName<SSWC.DataForm>(x => x.Header), true));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.NewItemTemplate), new DataContextValueSourceAttribute(currentItemName, true));
                    b.AddCustomAttributes(Extensions.GetMemberName<SSWC.DataForm>(x => x.ReadOnlyTemplate), new DataContextValueSourceAttribute(currentItemName, true));

                    // DefaultPropertyAttribute
                    b.AddCustomAttributes(new DefaultPropertyAttribute(Extensions.GetMemberName<SSWC.DataForm>(x => x.ItemsSource)));

                    // DefaultEventAttribute
                    b.AddCustomAttributes(new DefaultEventAttribute("ItemEditEnded"));

                    // ComplexBindingPropertiesAttribute
                    b.AddCustomAttributes(new ComplexBindingPropertiesAttribute(
                        Extensions.GetMemberName<SSWC.DataForm>(x => x.ItemsSource)));

                    // TypeConverterAttribute
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataForm>(x => x.CancelButtonContent),
                        new TypeConverterAttribute(typeof(StringConverter)));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataForm>(x => x.CommitButtonContent),
                        new TypeConverterAttribute(typeof(StringConverter)));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DataForm>(x => x.Header),
                        new TypeConverterAttribute(typeof(StringConverter)));
                });
        }
    }
}
