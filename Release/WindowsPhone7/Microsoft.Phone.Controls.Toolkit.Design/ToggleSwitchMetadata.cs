// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;

namespace Microsoft.Phone.Controls.Design
{
    internal class ToggleSwitchMetadata : AttributeTableBuilder
    {
        public ToggleSwitchMetadata()
        {
            // Type attributes
            AddCustomAttributes(typeof(ToggleSwitch), new DescriptionAttribute("Represents a switch that can be toggled between two states."));
            AddCustomAttributes(typeof(ToggleSwitch), new FeatureAttribute(typeof(ToggleSwitchInitializer)));

            // Property attributes
            AddCustomAttributes(typeof(ToggleSwitch), "IsChecked", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(ToggleSwitch), "IsChecked", new DescriptionAttribute("Gets or sets whether the ToggleSwitch is checked."));
            AddCustomAttributes(typeof(ToggleSwitch), "IsChecked", new TypeConverterAttribute(typeof(NullableBoolConverter)));
            AddCustomAttributes(typeof(ToggleSwitch), "Header", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(ToggleSwitch), "Header", new DescriptionAttribute("Gets or sets the header."));
            AddCustomAttributes(typeof(ToggleSwitch), "Header", new TypeConverterAttribute(typeof(StringConverter)));
            AddCustomAttributes(typeof(ToggleSwitch), "HeaderTemplate", new DescriptionAttribute("Gets or sets the template used to display the control's header."));
            AddCustomAttributes(typeof(ToggleSwitch), "Checked", new DescriptionAttribute("Occurs when a ToggleSwitch is checked."));
            AddCustomAttributes(typeof(ToggleSwitch), "Unchecked", new DescriptionAttribute("Occurs when a ToggleSwitch is unchecked."));
        }
    }
}