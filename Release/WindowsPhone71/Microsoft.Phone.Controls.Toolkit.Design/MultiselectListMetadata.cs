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
    internal class MultiselectListMetadata : AttributeTableBuilder
    {
        private const string OtherProperties = "Other";

        public MultiselectListMetadata()
        {
            // Type attributes
            AddCustomAttributes(typeof(MultiselectList), new DescriptionAttribute("Represents a selectable item in a MultiselectList."));
            AddCustomAttributes(typeof(MultiselectList), new FeatureAttribute(typeof(MultiselectListInitializer)));

            // Property attributes
            AddCustomAttributes(typeof(MultiselectList), "IsSelectionEnabled", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(MultiselectList), "IsSelectionEnabled", new DescriptionAttribute("Gets or sets a value that indicates whether the MultiselectList is selecting."));
            AddCustomAttributes(typeof(MultiselectList), "IsSelectionEnabled", new TypeConverterAttribute(typeof(BooleanConverter)));            
            AddCustomAttributes(typeof(MultiselectList), "ItemInfoTemplate", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(MultiselectList), "ItemInfoTemplate", new DescriptionAttribute("Gets or sets the data template used to display the content information of each MultiselectItem."));
            AddCustomAttributes(typeof(MultiselectList), "ItemContainerStyle", new CategoryAttribute(OtherProperties));
            AddCustomAttributes(typeof(MultiselectList), "ItemContainerStyle", new DescriptionAttribute("Gets or sets the style that is applied to the container element generated for each item."));
        }
    }
}