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
    internal class HubTileMetadata : AttributeTableBuilder
    {
        private const string TextProperties = "Text";
        private const string OtherProperties = "Other";
        
        public HubTileMetadata()
        {
            // Type attributes
            AddCustomAttributes(typeof(HubTile), new DescriptionAttribute("Represents an animated tile that supports an image and a title associated with either a message or a notification."));
            AddCustomAttributes(typeof(HubTile), new FeatureAttribute(typeof(HubTileInitializer)));

            // Property attributes
            AddCustomAttributes(typeof(HubTile), "Source", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(HubTile), "Source", new DescriptionAttribute("Gets or sets the source for the image."));
            AddCustomAttributes(typeof(HubTile), "Title", new CategoryAttribute(TextProperties));
            AddCustomAttributes(typeof(HubTile), "Title", new DescriptionAttribute("Gets or sets the title associated to the tile."));
            AddCustomAttributes(typeof(HubTile), "Title", new TypeConverterAttribute(typeof(StringConverter)));
            AddCustomAttributes(typeof(HubTile), "Notification", new CategoryAttribute(TextProperties));
            AddCustomAttributes(typeof(HubTile), "Notification", new DescriptionAttribute("Gets or sets the notification displayed on the back of the tile."));
            AddCustomAttributes(typeof(HubTile), "Notification", new TypeConverterAttribute(typeof(StringConverter)));
            AddCustomAttributes(typeof(HubTile), "Message", new CategoryAttribute(TextProperties));
            AddCustomAttributes(typeof(HubTile), "Message", new DescriptionAttribute("Gets or sets the message displayed on the back of the tile."));
            AddCustomAttributes(typeof(HubTile), "Message", new TypeConverterAttribute(typeof(StringConverter)));
            AddCustomAttributes(typeof(HubTile), "DisplayNotification", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(HubTile), "DisplayNotification", new DescriptionAttribute("Gets or sets whether the notification should be displayed instead of the message."));
            AddCustomAttributes(typeof(HubTile), "DisplayNotification", new TypeConverterAttribute(typeof(BooleanConverter)));
            AddCustomAttributes(typeof(HubTile), "IsFrozen", new CategoryAttribute(MetadataStore.CommonProperties));
            AddCustomAttributes(typeof(HubTile), "IsFrozen", new DescriptionAttribute("Gets or sets whether the HubTile can be animated."));
            AddCustomAttributes(typeof(HubTile), "IsFrozen", new TypeConverterAttribute(typeof(BooleanConverter)));
            AddCustomAttributes(typeof(HubTile), "GroupTag", new CategoryAttribute(OtherProperties));
            AddCustomAttributes(typeof(HubTile), "GroupTag", new DescriptionAttribute("Gets or sets the group of HubTiles that this one is associated with."));
        }
    }
}