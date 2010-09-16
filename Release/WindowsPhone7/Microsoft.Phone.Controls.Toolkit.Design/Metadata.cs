// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;

namespace Microsoft.Phone.Controls.Design
{
    /// <summary>
    /// Provides design metadata.
    /// </summary>
    public class MetadataStore : IProvideAttributeTable
    {
        /// <summary>
        /// Stores the string used to refer to the "Common Properties" section.
        /// </summary>
        private const string CommonProperties = "Common Properties";

        /// <summary>
        /// Gets the attribute table.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Simple method happens to be highly coupled.")]
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder attributeTableBuilder = new AttributeTableBuilder();

                // Add attributes for ContextMenuService
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenuService), "ContextMenu", new AttachedPropertyBrowsableForTypeAttribute(typeof(UIElement)));
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenuService), "ContextMenu", new DisplayNameAttribute(typeof(ContextMenu).Name));
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenuService), "ContextMenu", new CategoryAttribute(CommonProperties));
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenuService), "ContextMenu", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));

                // Add attributes for ContextMenu
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenu), new DescriptionAttribute("Represents a pop-up menu that enables a control to expose functionality that is specific to the context of the control."));

                // Add attributes for ContextMenu properties
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenu), "IsZoomEnabled", new CategoryAttribute(CommonProperties));
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenu), "VerticalOffset", new CategoryAttribute("Layout"));
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenu), "IsOpen", new DescriptionAttribute("Gets or sets a value indicating whether the ContextMenu is visible."));
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenu), "IsZoomEnabled", new DescriptionAttribute(" Gets or sets a value indicating whether the background will zoom out when the ContextMenu is open."));
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenu), "ItemContainerStyle", new DescriptionAttribute("Gets or sets the Style that is applied to the container element generated for each item."));
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenu), "VerticalOffset", new DescriptionAttribute("Gets or sets the vertical distance between the target origin and the popup alignment point."));
                NewItemTypesAttribute menuItemType = new NewItemTypesAttribute(typeof(MenuItem));
                menuItemType.FactoryType = typeof(MenuItemFactory);
                NewItemTypesAttribute separatorType = new NewItemTypesAttribute(typeof(Separator));
                attributeTableBuilder.AddCustomAttributes(typeof(ContextMenu), "Items", menuItemType, separatorType);

                // Add attributes for MenuItem
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), new DescriptionAttribute("Represents a selectable item inside a Menu or ContextMenu."));
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), new FeatureAttribute(typeof(MenuItemInitializer)));

                // Add attributes for MenuItem properties
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), "Command", new CategoryAttribute(CommonProperties));
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), "CommandParameter", new CategoryAttribute(CommonProperties));
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), "Header", new CategoryAttribute(CommonProperties));
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), "Command", new DescriptionAttribute("Gets or sets the command associated with the menu item."));
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), "CommandParameter", new DescriptionAttribute("Gets or sets the parameter to pass to the Command property of a MenuItem."));
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), "Header", new DescriptionAttribute("Gets or sets the item that labels the control."));
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), "HeaderTemplate", new DescriptionAttribute("Gets or sets a data template that is used to display the contents of the control's header."));
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), "ItemContainerStyle", new DescriptionAttribute("Gets or sets the Style that is applied to the container element generated for each item."));
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), "Header", new TypeConverterAttribute(typeof(StringConverter)));

                // Add attributes for Separator
                attributeTableBuilder.AddCustomAttributes(typeof(Separator), new DescriptionAttribute("Control that is used to separate items in items controls."));

                // Add attributes for ToggleSwitch
                attributeTableBuilder.AddCustomAttributes(typeof(ToggleSwitch), new DescriptionAttribute("Represents a switch that can be toggled between two states."));
                attributeTableBuilder.AddCustomAttributes(typeof(ToggleSwitch), new FeatureAttribute(typeof(ToggleSwitchInitializer)));

                // Add attributes for ToggleSwitch properties
                attributeTableBuilder.AddCustomAttributes(typeof(ToggleSwitch), "Header", new CategoryAttribute(CommonProperties));
                attributeTableBuilder.AddCustomAttributes(typeof(ToggleSwitch), "Header", new DescriptionAttribute("Gets or sets the header."));
                attributeTableBuilder.AddCustomAttributes(typeof(ToggleSwitch), "Header", new TypeConverterAttribute(typeof(StringConverter)));
                attributeTableBuilder.AddCustomAttributes(typeof(ToggleSwitch), "SwitchForeground", new CategoryAttribute("Brushes"));
                attributeTableBuilder.AddCustomAttributes(typeof(ToggleSwitch), "SwitchForeground", new DescriptionAttribute("The switch foreground."));

                // Add attributes for WrapPanel
                attributeTableBuilder.AddCustomAttributes(typeof(WrapPanel), new DescriptionAttribute("Positions child elements sequentially from left to right or top to bottom. When elements extend beyond the panel edge, elements are positioned in the next row or column."));
                
                // Add attributes for WrapPanel properties
                attributeTableBuilder.AddCustomAttributes(typeof(WrapPanel), "Orientation", new CategoryAttribute(CommonProperties));
                attributeTableBuilder.AddCustomAttributes(typeof(WrapPanel), "Orientation", new DescriptionAttribute("Gets or sets the direction in which child elements are arranged."));
                attributeTableBuilder.AddCustomAttributes(typeof(WrapPanel), "ItemWidth", new CategoryAttribute(CommonProperties));
                attributeTableBuilder.AddCustomAttributes(typeof(WrapPanel), "ItemWidth", new DescriptionAttribute("Gets or sets the width of the layout area for each item."));
                attributeTableBuilder.AddCustomAttributes(typeof(WrapPanel), "ItemHeight", new CategoryAttribute(CommonProperties));
                attributeTableBuilder.AddCustomAttributes(typeof(WrapPanel), "ItemHeight", new DescriptionAttribute("Gets or sets the height of the layout area for each item."));

                // Add attributes for DatePicker
                attributeTableBuilder.AddCustomAttributes(typeof(DatePicker), new DescriptionAttribute("Represents a control that allows the user to choose a date (day/month/year)."));

                // Add attributes for TimePicker
                attributeTableBuilder.AddCustomAttributes(typeof(TimePicker), new DescriptionAttribute("Represents a control that allows the user to choose a time (hour/minute/am/pm)."));

                // Add attributes for DatePicker/TimePicker properties
                attributeTableBuilder.AddCustomAttributes(typeof(DateTimePickerBase), "Value", new CategoryAttribute(CommonProperties));
                attributeTableBuilder.AddCustomAttributes(typeof(DateTimePickerBase), "Header", new CategoryAttribute(CommonProperties));
                attributeTableBuilder.AddCustomAttributes(typeof(DateTimePickerBase), "Value", new DescriptionAttribute("Gets or sets the DateTime value."));
                attributeTableBuilder.AddCustomAttributes(typeof(DateTimePickerBase), "ValueChanged", new DescriptionAttribute("Event that is invoked when the Value property changes."));
                attributeTableBuilder.AddCustomAttributes(typeof(DateTimePickerBase), "ValueStringFormat", new DescriptionAttribute("Gets or sets the format string to use when converting the Value property to a string."));
                attributeTableBuilder.AddCustomAttributes(typeof(DateTimePickerBase), "Header", new DescriptionAttribute("Gets or sets the header of the control."));
                attributeTableBuilder.AddCustomAttributes(typeof(DateTimePickerBase), "HeaderTemplate", new DescriptionAttribute("Gets or sets the template used to display the control's header."));
                attributeTableBuilder.AddCustomAttributes(typeof(DateTimePickerBase), "PickerPageUri", new DescriptionAttribute("Gets or sets the Uri to use for loading the IDateTimePickerPage instance when the control is clicked."));
                attributeTableBuilder.AddCustomAttributes(typeof(DateTimePickerBase), "Header", new TypeConverterAttribute(typeof(StringConverter)));

                // Hide primitives
                attributeTableBuilder.AddCustomAttributes(typeof(DateTimePickerBase), new ToolboxBrowsableAttribute(false));
                attributeTableBuilder.AddCustomAttributes(typeof(DatePickerPage), new ToolboxBrowsableAttribute(false));
                attributeTableBuilder.AddCustomAttributes(typeof(TimePickerPage), new ToolboxBrowsableAttribute(false));
                attributeTableBuilder.AddCustomAttributes(typeof(HeaderedItemsControl), new ToolboxBrowsableAttribute(false));
                attributeTableBuilder.AddCustomAttributes(typeof(LoopingSelector), new ToolboxBrowsableAttribute(false));
                attributeTableBuilder.AddCustomAttributes(typeof(LoopingSelectorItem), new ToolboxBrowsableAttribute(false));
                attributeTableBuilder.AddCustomAttributes(typeof(ToggleSwitchButton), new ToolboxBrowsableAttribute(false));

                // Add back subclasses
                attributeTableBuilder.AddCustomAttributes(typeof(DatePicker), new ToolboxBrowsableAttribute(true));
                attributeTableBuilder.AddCustomAttributes(typeof(TimePicker), new ToolboxBrowsableAttribute(true));
                attributeTableBuilder.AddCustomAttributes(typeof(MenuItem), new ToolboxBrowsableAttribute(true));

                return attributeTableBuilder.CreateTable();
            }
        }
    }
}