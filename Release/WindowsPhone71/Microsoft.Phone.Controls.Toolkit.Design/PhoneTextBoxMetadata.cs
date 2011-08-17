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
    internal class PhoneTextBoxMetadata : AttributeTableBuilder
    {
        private const string TextProperties = "Text";
        private const string OtherProperties = "Other";

        public PhoneTextBoxMetadata()
        {
            // Type attributes
            AddCustomAttributes(typeof(PhoneTextBox), new DescriptionAttribute("Represents a text box with additional features, including hint text, an action icon, and a length indicator."));
            AddCustomAttributes(typeof(PhoneTextBox), new FeatureAttribute(typeof(PhoneTextBoxInitializer)));

            // Property attributes
            AddCustomAttributes(typeof(PhoneTextBox), "Hint", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(PhoneTextBox), "Hint", new DescriptionAttribute("Gets or sets the hint text associated with the control."));
            AddCustomAttributes(typeof(PhoneTextBox), "Hint", new TypeConverterAttribute(typeof(StringConverter)));
            AddCustomAttributes(typeof(PhoneTextBox), "HintStyle", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(PhoneTextBox), "HintStyle", new DescriptionAttribute("Gets or sets the style of the Hint associated with the control."));
            AddCustomAttributes(typeof(PhoneTextBox), "LengthIndicatorVisible", new CategoryAttribute(OtherProperties));
            AddCustomAttributes(typeof(PhoneTextBox), "LengthIndicatorVisible", new DescriptionAttribute("Gets or sets whether the length indicator should be visible."));
            AddCustomAttributes(typeof(PhoneTextBox), "LengthIndicatorVisible", new TypeConverterAttribute(typeof(BooleanConverter)));
            AddCustomAttributes(typeof(PhoneTextBox), "LengthIndicatorThreshold", new CategoryAttribute(OtherProperties));
            AddCustomAttributes(typeof(PhoneTextBox), "LengthIndicatorThreshold", new DescriptionAttribute("Gets or sets the threshold after which the length indicator appears."));
            AddCustomAttributes(typeof(PhoneTextBox), "LengthIndicatorThreshold", new TypeConverterAttribute(typeof(Int16Converter)));
            AddCustomAttributes(typeof(PhoneTextBox), "DisplayedMaxLength", new CategoryAttribute(OtherProperties));
            AddCustomAttributes(typeof(PhoneTextBox), "DisplayedMaxLength", new DescriptionAttribute("Gets or sets the displayed max length for the length indicator"));
            AddCustomAttributes(typeof(PhoneTextBox), "DisplayedMaxLength", new TypeConverterAttribute(typeof(StringConverter)));
            AddCustomAttributes(typeof(PhoneTextBox), "ActionIcon", new CategoryAttribute(OtherProperties));
            AddCustomAttributes(typeof(PhoneTextBox), "ActionIcon", new DescriptionAttribute("Gets or sets the icon displayed on the right side of the control."));
        }
    }
}