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
    /// We set the IsSelected property at design time 
    /// when the user selects a particular tab,
    /// so that the tab becomes active and can be designed. 
    /// However we don't want to persist this value into the runtime,
    /// so we capture the value set in XAML or the Property Browser 
    /// separately from the actual value in the designer.
    /// 
    /// Works in conjunction with TabControlDesignModeValueProvider.
    /// </summary>
    internal class TabItemDesignModeValueProvider : DesignModeValueProvider
    {
        // DesignerProperty to control the IsSelected property of TabItem. 
        // We need to store the design time value of this.

        /// <summary>
        /// DesignerProperty to control the IsSelected property of TabItem. 
        /// We need to store the design time value of this.
        /// </summary>
        public static readonly DesignerProperty<bool> DesignTimeIsSelectedProperty = new DesignerProperty<bool>("DesignTimeIsSelected");

        /// <summary>
        /// Register the property we want to provider a design time version of.
        /// </summary>
        public TabItemDesignModeValueProvider()
        {
            Properties.Add(MyPlatformTypes.TabItem.IsSelectedProperty);
        }

        /// <summary>
        /// Clear the value of IsSelected and raise change notification.
        /// </summary>
        /// <param name="item">The ModelItem.</param>
        public static void ClearDesignTimeIsSelected(ModelItem item)
        {
            item.ClearDesignerProperty(DesignTimeIsSelectedProperty); // Note: ClearDesignerProperty etc. are extension methods declared on DesignerState
            Util.InvalidateProperty(item, MyPlatformTypes.TabItem.IsSelectedProperty);
        }

        /// <summary>
        /// Set the value of IsSelected, update any other TabItems,
        /// and raise change notification.
        /// </summary>
        /// <param name="item">The ModelItem representing a TabItem.</param>
        /// <param name="value">Value of the TabItem's IsSelected property.</param>
        public static void SetDesignTimeIsSelected(ModelItem item, bool value)
        {
            // If we are setting this one to true, set all the others to false.
            if (value)
            {
                UpdateIsSelectedForOtherTabs(item);
            }

            item.SetDesignerProperty(DesignTimeIsSelectedProperty, value);
            Util.InvalidateProperty(item, MyPlatformTypes.TabItem.IsSelectedProperty);
        }

        /// <summary>
        /// Only one TabItem should have DesignTimeIsSelectedProperty set to true at a time.
        /// DesignTimeSelectedIndexProperty of TabControl should match with that of active tab.
        /// </summary>
        /// <param name="item">The ModelItem representing a TabItem.</param>
        private static void UpdateIsSelectedForOtherTabs(ModelItem item)
        {
            // item.Parent is Tabcontrol
            if (item.Parent != null && item.Parent.IsItemOfType(MyPlatformTypes.TabControl.TypeId))
            {
                if (item.Parent.Content != null)
                {
                    ModelItemCollection tabItemCollection = item.Parent.Content.Collection;
                    if (tabItemCollection != null && tabItemCollection.Count > 0)
                    {
                        foreach (ModelItem tabItem in tabItemCollection)
                        {
                            if (tabItem != item)
                            { 
                                // set designer property for other tabItem in TabControl to false.
                                tabItem.SetDesignerProperty(DesignTimeIsSelectedProperty, false);
                            }
                            else
                            {
                                // This tabItem has been activated, update selectedIndex for Tabcontrol
                                tabItem.Parent.SetDesignerProperty(
                                    TabControlDesignModeValueProvider.DesignTimeSelectedIndexProperty, 
                                    tabItemCollection.IndexOf(tabItem));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the value of IsSelected.
        /// </summary>
        /// <param name="item">The ModelItem representing a TabItem.</param>
        /// <returns>The design time value for IsSelected property.</returns>
        public static bool GetDesignTimeIsSelected(ModelItem item)
        {
            return item.GetDesignerProperty(DesignTimeIsSelectedProperty);
        }

        /// <summary>
        /// Capture property changes made by the user and return the design time value.
        /// </summary>
        /// <param name="item">The ModelItem representing a TabItem.</param>
        /// <param name="identifier">The property that the user is changing the value of. </param>
        /// <param name="value">The new value that the user is giving the property.</param>
        /// <returns>The value to set the property to in the designer.</returns>
        public override object TranslatePropertyValue(ModelItem item, PropertyIdentifier identifier, object value)
        {
            if (identifier == MyPlatformTypes.TabItem.IsSelectedProperty)
            {
                bool isSelected = false;
                isSelected = GetDesignTimeIsSelected(item);

                return isSelected | (bool)value;
            }

            return base.TranslatePropertyValue(item, identifier, value);
        }
    }
}