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
    internal class MultiselectItemMetadata : AttributeTableBuilder
    {
        private const string OtherProperties = "Other";

        public MultiselectItemMetadata()
        {
            // Type attributes
            AddCustomAttributes(typeof(MultiselectItem), new DescriptionAttribute("Represents a selectable item in a MultiselectList."));
            AddCustomAttributes(typeof(MultiselectItem), new FeatureAttribute(typeof(MultiselectItemInitializer)));

            // Property attributes
            AddCustomAttributes(typeof(MultiselectItem), "IsSelected", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(MultiselectItem), "IsSelected", new DescriptionAttribute("Gets or sets a value that indicates whether the MultiselectItem is selected."));
            AddCustomAttributes(typeof(MultiselectItem), "IsSelected", new TypeConverterAttribute(typeof(BooleanConverter)));
            AddCustomAttributes(typeof(MultiselectItem), "HintPanelHeight", new CategoryAttribute(OtherProperties));
            AddCustomAttributes(typeof(MultiselectItem), "HintPanelHeight", new DescriptionAttribute("Gets or sets the height of the hint panel."));
            AddCustomAttributes(typeof(MultiselectItem), "HintPanelHeight", new TypeConverterAttribute(typeof(DoubleConverter)));
            AddCustomAttributes(typeof(MultiselectItem), "ContentInfo", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(MultiselectItem), "ContentInfo", new DescriptionAttribute("Gets or sets the content information of a MultiselectItem."));
            AddCustomAttributes(typeof(MultiselectItem), "ContentInfoTemplate", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(MultiselectItem), "ContentInfoTemplate", new DescriptionAttribute("Gets or sets the data template used to display the content information of a MultiselectItem."));
        }
    }
}