// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Collections.Generic;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// DesignerState and DesignerProperty are used to store the value of properties 
    /// that are only used at design time and are not persisted to XAML. 
    /// It is used in the implementation of DesignModeValueProvider classes. 
    /// For example we set TabControl.SelectedIndex in the designer to allow the user 
    /// to design the active tab but we don't want that value persisted into the XAML. 
    /// 
    /// It is a dictionary of ModelItem->Properties where "Properties" is a dictionary of Property->Value.
    /// DesignerProperty exists to allow typed access to property values. For example:
    /// 
    ///        int selectedIndex = item.GetDesignerProperty(DesignTimeSelectedIndexProperty);
    /// 
    /// DesignerState also adds the following extension methods to Model Item:
    /// 
    ///     GetDesignerProperty
    ///     SetDesignerProperty
    ///     ClearDesignerProperty
    ///     
    /// See TabControlDesignModeValueProvider for a usage example.
    /// </summary>
    internal static class DesignerState
    {
        /// <summary>
        /// Dictionary of DesignerProperty->Value.
        /// </summary>
        private class StateTable : Dictionary<object, object> { }

        /// <summary>
        /// Dictionary of ModelItem->DesignerProperty.
        /// </summary>
        private class DesignerStateDictionary : Dictionary<ModelItem, StateTable> { }

        /// <summary>
        /// Get the value of the designer property for this model item.
        /// </summary>
        /// <typeparam name="T">Property value type.</typeparam>
        /// <param name="item">Model item.</param>
        /// <param name="property">Designer property.</param>
        /// <returns>Designer property value.</returns>
        public static T GetDesignerProperty<T>(this ModelItem item, DesignerProperty<T> property)
        {
            return GetDesignerProperty<T>(item, property, null);
        }

        /// <summary>
        /// Get the value of the designer property for this model item.
        /// If there is not a value then create one using the factory.
        /// </summary>
        /// <typeparam name="T">Property value type.</typeparam>
        /// <param name="item">Model item.</param>
        /// <param name="property">Designer property.</param>
        /// <param name="factory">A function to initialize the designer property value.</param>
        /// <returns>Designer property value.</returns>
        public static T GetDesignerProperty<T>(this ModelItem item, DesignerProperty<T> property, Func<ModelItem, T> factory)
        {
            DesignerStateDictionary dictionary = item.Context.Services.GetService<DesignerStateDictionary>();
            if (dictionary == null)
            {
                dictionary = new DesignerStateDictionary();
                item.Context.Services.Publish<DesignerStateDictionary>(dictionary);
            }

            StateTable table;
            if (dictionary.TryGetValue(item, out table))
            {
                object value;
                if (table.TryGetValue(property, out value))
                {
                    return (T)value;
                }
                else if (factory != null)
                {
                    T v = factory(item);
                    table[property] = v;
                    return v;
                }
            }
            return default(T);
        }

        /// <summary>
        /// Set the value of the designer property for this model item.
        /// </summary>
        /// <typeparam name="T">Property value type.</typeparam>
        /// <param name="item">Model item.</param>
        /// <param name="property">Designer property.</param>
        /// <param name="value">Designer property value.</param>
        public static void SetDesignerProperty<T>(this ModelItem item, DesignerProperty<T> property, T value)
        {
            DesignerStateDictionary dictionary = item.Context.Services.GetService<DesignerStateDictionary>();
            if (dictionary == null)
            {
                dictionary = new DesignerStateDictionary();
                item.Context.Services.Publish<DesignerStateDictionary>(dictionary);
            }

            StateTable table;
            if (!dictionary.TryGetValue(item, out table))
            {
                table = new StateTable();
                dictionary[item] = table;
            }

            table[property] = value;
        }

        /// <summary>
        /// Clear the value of the designer property for this model item.
        /// </summary>
        /// <typeparam name="T">Property value type.</typeparam>
        /// <param name="item">Model item.</param>
        /// <param name="property">Designer property.</param>
        public static void ClearDesignerProperty<T>(this ModelItem item, DesignerProperty<T> property)
        {
            DesignerStateDictionary dictionary = item.Context.Services.GetService<DesignerStateDictionary>();
            if (dictionary != null)
            {
                StateTable table;
                if (dictionary.TryGetValue(item, out table))
                {
                    table.Remove(property);
                }
                if (table != null && table.Count == 0)
                {
                    dictionary.Remove(item);
                }
            }
        }
    }
}