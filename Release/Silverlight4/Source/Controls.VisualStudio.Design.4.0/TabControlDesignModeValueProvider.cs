// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using System.Windows.Controls.Design;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// We set the SelectedIndex property at design time when the user selects a particular tab so that the tab becomes 
    /// active and can be designed. However we don't want to persist this value at into the runtime so we capture the 
    /// value set in XAML or the Property Browser separately from the actual value in the designer.
    /// 
    /// The "meat" of this class is in TranslatePropertyValue.
    /// 
    /// Works in conjunction with TabItemDesignModeValueProvider.
    /// </summary>
    internal class TabControlDesignModeValueProvider : DesignModeValueProvider
    {
        /// <summary>
        /// DesignerProperty to control the SelectedIndex property of TabControl. 
        /// We need to store the design time value of this property separately from the runtime state.
        /// </summary>
        public static readonly DesignerProperty<int> DesignTimeSelectedIndexProperty = new DesignerProperty<int>("DesignTimeSelectedIndex");

        /// <summary>
        /// DesignerProperty to track whether user has updated SelectedIndex property value from XAML/PB.
        /// We don't want to override the user's choice when resetting SelectedIndex at design time.
        /// </summary>
        public static readonly DesignerProperty<int> CurrentSelectedIndexValueProperty = new DesignerProperty<int>("CurrentSelectedIndexValue");

        /// <summary>
        /// Register the property we want to provider a design time version of.
        /// </summary>
        public TabControlDesignModeValueProvider()
        {
            Properties.Add(MyPlatformTypes.TabControl.SelectedIndexProperty);
        }

        /// <summary>
        /// Clear the value of SelectedIndex and raise change notification.
        /// </summary>
        /// <param name="item">Model item.</param>
        public static void ClearDesignTimeSelectedIndex(ModelItem item)
        {
            item.ClearDesignerProperty(DesignTimeSelectedIndexProperty);    // Note: ClearDesignerProperty etc. are extension methods declared on DesignerState
            Util.InvalidateProperty(item, MyPlatformTypes.TabControl.SelectedIndexProperty);
        }

        /// <summary>
        /// Set the value of SelectedIndex and raise change notification.
        /// Make the selected TabItem the Active TabItem.
        /// </summary>
        /// <param name="item">Model item for the TabControl.</param>
        /// <param name="value">The SelectedIndex value.</param>
        public static void SetDesignTimeSelectedIndex(ModelItem item, int value)
        {
            item.SetDesignerProperty(DesignTimeSelectedIndexProperty, value);
            Util.InvalidateProperty(item, MyPlatformTypes.TabControl.SelectedIndexProperty);

            ModelProperty content = item.Content;
            ModelItemCollection tabControlChildCollection = null;

            // activate the corresponding TabItem
            if (content != null)
            {
                tabControlChildCollection = content.Collection;
                if (tabControlChildCollection != null && value <= tabControlChildCollection.Count)
                {
                    TabItemDesignModeValueProvider.SetDesignTimeIsSelected(tabControlChildCollection[value], true);
                }
            }
        }

        /// <summary>
        /// Get the value of SelectedIndex.
        /// </summary>
        /// <param name="item">The model item for a TabControl.</param>
        /// <returns>SelectedIndex of the TabControl.</returns>
        public static int GetDesignTimeSelectedIndex(ModelItem item)
        {
            int selectedIndex = item.GetDesignerProperty(DesignTimeSelectedIndexProperty);

            // check if that tab is really active. If not, then return -1 (none of the tabs are active
            if (item.Content != null &&
                item.Content.Collection.Count > 0 &&
                selectedIndex < item.Content.Collection.Count)
            {
                if (TabItemDesignModeValueProvider.GetDesignTimeIsSelected(item.Content.Collection[selectedIndex]))
                {
                    return selectedIndex;
                }
            }
            return -1;
        }

        /// <summary>
        /// Get the value of CurrentSelectedIndex. 
        /// </summary>
        /// <param name="item">The model item for a TabControl.</param>
        /// <returns>CurrentSelectedIndex of the TabControl.</returns>
        public static int GetCurrentSelectedIndexPropertyValue(ModelItem item)
        {
            return item.GetDesignerProperty(CurrentSelectedIndexValueProperty);
        }

        /// <summary>
        /// Set the value of CurrentSelectedIndex.
        /// </summary>
        /// <param name="item">The model item for a TabControl.</param>
        /// <param name="value">CurrentSelectedIndex of the TabControl.</param>
        public static void SetCurrentSelectedIndexPropertyValue(ModelItem item, int value)
        {
            item.SetDesignerProperty(CurrentSelectedIndexValueProperty, value);
        }

        /// <summary>
        /// Capture property changes made by the user and return the design time value.
        /// </summary>
        /// <param name="item">The model item for a TabControl.</param>
        /// <param name="identifier">The property that the user is changing the value of.</param>
        /// <param name="value">The new value that the user is giving the property.</param>
        /// <returns>The value to set the property to in the designer.</returns>
        public override object TranslatePropertyValue(ModelItem item, PropertyIdentifier identifier, object value)
        {
            if (identifier == MyPlatformTypes.TabControl.SelectedIndexProperty)
            {
                // if SelectedIndex has been modified from XAML/PB, we use that as the value for selectedIndex
                // otherwise we determine the index of active tabItem and return that as the selected Index   

                // TabControl's DMVP gets called when user changes SelectedIndex from XAML/PB and whenever TabControl gets created/recreated.
                // TabControl gets recreated when its items collection is updated (adding/removing tabs)

                // **************Scenario 1:TabItem is added/removed
                // When a tabItem is added or removed, TabItemAdornerProvider updates the selection for TabItems and we want to retain that selection
                // This is done by updating DesignTimeSelectedIndex value each time selection for Tabitem is updated and we use this value in TabControls DMVP

                // *************Scenario 2:User hasa updated SelectedIndex value from XAML/PB
                // Here we want to use the updated value of SelectedIndex that gets passed into TabControl DMVP
                // To distinguish between the scenario where Tabcontrol is recreated(Scenario 1) and the case where user has simply updated SelectedIndex property value
                // we maintain another DesignerProperty called as CurrentSelectedIndexValueProperty, which stores current value of SelectedIndex that has been serialized
                // We compare CurrentSelectedIndexValueProperty with new value that is passed in to determine whether user has updated SelectedIndex from XAML/PB
                // If this is the case, we should use updated value of SelectedIndex that user has set and update TabItem selection too

                if (GetCurrentSelectedIndexPropertyValue(item) != (int)value)
                {
                    // this is Scenario 2: User has updated SelectedIndex value from XAML/PB
                    SetCurrentSelectedIndexPropertyValue(item, (int)value);
                    if (item.Content.Collection != null && (int)value >= 0 && (int)value < item.Content.Collection.Count)
                    {
                        ModelItem tabItem = item.Content.Collection[(int)value];
                        TabItemDesignModeValueProvider.SetDesignTimeIsSelected(tabItem, true);
                        return value;
                    }
                }

                // If control flows till here, then its Scenario 1: TabItem is added/removed
                int selectedIndex = GetDesignTimeSelectedIndex(item);

                if (selectedIndex != -1)
                {
                    return selectedIndex;
                }
                else if (item.Properties[identifier].IsSet)
                {
                    return value;
                }
                else
                {
                    return 0;
                }
            }

            return base.TranslatePropertyValue(item, identifier, value);
        }
    }
}