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
    internal class ExpanderViewMetadata : AttributeTableBuilder
    {
        private const string TextProperties = "Text";
        private const string OtherProperties = "Other";

        public ExpanderViewMetadata()
        {
            // Type attributes
            AddCustomAttributes(typeof(ExpanderView), new DescriptionAttribute("Represents a collection of items that can be expanded and collapsed."));
            AddCustomAttributes(typeof(ExpanderView), new FeatureAttribute(typeof(ExpanderViewInitializer)));

            // Property attributes
            AddCustomAttributes(typeof(ExpanderView), "Expander", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(ExpanderView), "Expander", new DescriptionAttribute("Gets or sets the expander object."));
            AddCustomAttributes(typeof(MultiselectItem), "ExpanderTemplate", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(MultiselectItem), "ExpanderTemplate", new DescriptionAttribute("Gets or sets the data template used to display the expander of an ExpanderView."));
            AddCustomAttributes(typeof(ExpanderView), "NonExpandableHeader", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(ExpanderView), "NonExpandableHeader", new DescriptionAttribute("Gets or sets the non-expandable header object."));
            AddCustomAttributes(typeof(ExpanderView), "NonExpandableHeaderTemplate", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(ExpanderView), "NonExpandableHeaderTemplate", new DescriptionAttribute("Gets or sets the data template used to display the non-expandable header of an ExpanderView."));
            AddCustomAttributes(typeof(ExpanderView), "IsExpanded", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(ExpanderView), "IsExpanded", new DescriptionAttribute("Gets or sets whether the ExpanderView is expanded."));
            AddCustomAttributes(typeof(ExpanderView), "IsExpanded", new TypeConverterAttribute(typeof(BooleanConverter)));
            AddCustomAttributes(typeof(ExpanderView), "HasItems", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(ExpanderView), "HasItems", new DescriptionAttribute("Gets or sets whether the ExpanderView has items."));
            AddCustomAttributes(typeof(ExpanderView), "HasItems", new TypeConverterAttribute(typeof(BooleanConverter)));
            AddCustomAttributes(typeof(ExpanderView), "IsNonExpandable", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(ExpanderView), "IsNonExpandable", new DescriptionAttribute("Gets or sets whether the ExpanderView is non-expandable."));
            AddCustomAttributes(typeof(ExpanderView), "IsNonExpandable", new TypeConverterAttribute(typeof(BooleanConverter)));
        }
    }
}