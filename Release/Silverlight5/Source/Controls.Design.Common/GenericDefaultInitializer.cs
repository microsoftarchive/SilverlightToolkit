// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Windows.Design.Model;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Windows.Design;

namespace System.Windows.Controls.Design.Common
{
    /// <summary>
    /// An initializer that uses a simple table driven approach to setting
    /// defaults on items.
    /// </summary>
    internal class GenericDefaultInitializer : DefaultInitializer
    {
        /// <summary>
        /// Locking object.
        /// </summary>
        private static object lockThis = new object();

        /// <summary>
        /// Table of properties to set on the item.
        /// </summary>
        private static Dictionary<Type, Dictionary<string, object>> _properties = new Dictionary<Type, Dictionary<string, object>>();

        /// <summary>
        /// Action to perform when initializing this item.
        /// </summary>
        private static Dictionary<Type, Action<ModelItem, EditingContext>> _actions = new Dictionary<Type, Action<ModelItem, EditingContext>>();

        /// <summary>
        /// Adds a default property value.
        /// </summary>
        /// <typeparam name="T">Type of the control the property will be set on.</typeparam>
        /// <param name="propertyName">Expression that is used in Extensions.GetMemberName
        /// to get the propertyName.</param>
        /// <param name="value">The value the property should be set to.</param>
        internal static void AddDefault<T>(System.Linq.Expressions.Expression<Func<T, object>> propertyName, object value)
        {
            lock (lockThis)
            {
                if (!_properties.ContainsKey(typeof(T)))
                {
                    _properties[typeof(T)] = new Dictionary<string, object>();
                }

                Dictionary<string, object> properties = _properties[typeof(T)];
                properties[System.Windows.Controls.Design.Common.Extensions.GetMemberName<T>(propertyName)] = value;
            }
        }

        /// <summary>
        /// Adds a default property value.
        /// </summary>
        /// <param name="T">Type of the control the property will be set on.</param>
        /// <param name="propertyName">The property on the item that should be set.</param>
        /// <param name="value">The value the property should be set to.</param>
        internal static void AddDefault(Type T, string propertyName, object value)
        {
            lock (lockThis)
            {
                if (!_properties.ContainsKey(T))
                {
                    _properties[T] = new Dictionary<string, object>();
                }

                Dictionary<string, object> properties = _properties[T];
                properties[propertyName] = value;
            }
        }

        /// <summary>
        /// Adds an action to be performed during initialization.
        /// </summary>
        /// <param name="T">Type of the control that the action will be performed on.</param>
        /// <param name="action">An action to be performed.</param>
        internal static void SetAction(Type T, Action<ModelItem, EditingContext> action)
        {
            _actions[T] = action;
        }

        /// <summary>
        /// Called during initialization of an item.
        /// </summary>
        /// <param name="item">The item being initialized.</param>
        /// <param name="context">The EditingContext during initialization.</param>
        public override void InitializeDefaults(ModelItem item, EditingContext context)
        {
            Type t = item.ItemType;
            bool actionApplied = false;

            List<Type> reverseTypes = new List<Type>();

            // build reverse list of types so we can apply default properties in the correct order.
            // while building it, apply the action, which needs to be sub -> base
            while (t != typeof(object))
            {
                // only apply an action once, and only for the first found
                // this means that actions registered for baseclasses are not
                // executed, which is the only safe approach. 
                if (!actionApplied && _actions.ContainsKey(t))
                {
                    actionApplied = true;
                    _actions[t](item, context);
                }

                reverseTypes.Insert(0, t);
                t = t.BaseType;
            }

            foreach (Type type in reverseTypes)
            {
                // will run through all the properties of the full hierarchy
                if (_properties.ContainsKey(type))
                {
                    Dictionary<string, object> properties = _properties[type];
                    foreach (KeyValuePair<string, object> keyvalue in properties)
                    {
                        ModelProperty property = item.Properties[keyvalue.Key];
                        Util.SparseSetValue(property, keyvalue.Value);
                    }
                }
            }
        }
    }
}
