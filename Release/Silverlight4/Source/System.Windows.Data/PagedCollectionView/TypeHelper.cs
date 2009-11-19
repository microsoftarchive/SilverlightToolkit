//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;

namespace System.Windows.Common
{
    /// <summary>
    /// Utility class for Type related operations
    /// </summary>
    internal static class TypeHelper
    {
        #region Internal Fields
        internal const char PropertyNameSeparator = '.';
        #endregion

        private static Type FindGenericType(Type definition, Type type)
        {
            while ((type != null) && (type != typeof(object)))
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == definition))
                {
                    return type;
                }
                if (definition.IsInterface)
                {
                    foreach (Type type2 in type.GetInterfaces())
                    {
                        Type type3 = FindGenericType(definition, type2);
                        if (type3 != null)
                        {
                            return type3;
                        }
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        internal static Type GetEnumerableItemType(this Type enumerableType)
        {
            Type type = FindGenericType(typeof(IEnumerable<>), enumerableType);
            if (type != null)
            {
                return type.GetGenericArguments()[0];
            }
            return enumerableType;
        }

        /// <summary>
        /// Extension method that returns the type of a property. That property can be nested.
        /// Each element of the path needs to be a public instance property.
        /// </summary>
        /// <param name="parentType">Type that exposes that property</param>
        /// <param name="propertyPath">Property path</param>
        /// <returns>Property type</returns>
        internal static Type GetNestedPropertyType(this Type parentType, string propertyPath)
        {
            PropertyInfo propertyInfo;
            Type propertyType = parentType;
            if (!String.IsNullOrEmpty(propertyPath))
            {
                string[] propertyNames = propertyPath.Split(PropertyNameSeparator);
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    propertyInfo = propertyType.GetProperty(propertyNames[i]);
                    if (propertyInfo == null)
                    {
                        return null;
                    }
                    propertyType = propertyInfo.PropertyType;
                }
            }
            return propertyType;
        }

        /// <summary>
        /// Retrieves the value of a property. That property can be nested.
        /// Each element of the path needs to be a public instance property.
        /// </summary>
        /// <param name="item">Object that exposes the property</param>
        /// <param name="propertyPath">Property path</param>
        /// <param name="propertyType">Property type</param>
        /// <param name="exception">Potential exception</param>
        /// <returns>Property value</returns>
        internal static object GetNestedPropertyValue(object item, string propertyPath, Type propertyType, out Exception exception)
        {
            exception = null;

            // if the item is null, treat the property value as null
            if (item == null)
            {
                return null;
            }

            // if the propertyPath is null or empty, return null
            if (String.IsNullOrEmpty(propertyPath))
            {
                return item;
            }

            string[] propertyNames = propertyPath.Split(PropertyNameSeparator);
            for (int i = 0; i < propertyNames.Length; i++)
            {
                if (item == null)
                {
                    return null;
                }
                Type type = item.GetType();

                // if we can't find the property or it is not of the correct type,
                // treat it as a null value
                PropertyInfo propertyInfo = type.GetProperty(propertyNames[i]);
                if (propertyInfo == null)
                {
                    return null;
                }

                if (!propertyInfo.CanRead)
                {
                    exception = new InvalidOperationException(string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        PagedCollectionViewResources.PropertyNotReadable,
                        propertyNames[i],
                        type.GetTypeName()));
                    return null;
                }

                if (i == propertyNames.Length - 1)
                {
                    // if the property type did not match, return null
                    if (propertyInfo.PropertyType != propertyType)
                    {
                        return null;
                    }
                    else
                    {
                        return propertyInfo.GetValue(item, null);
                    }
                }
                else
                {
                    item = propertyInfo.GetValue(item, null);
                }
            }

            return null;
        }

        internal static Type GetNonNullableType(this Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// Returns the friendly name for a type
        /// </summary>
        /// <param name="type">The type to get the name from</param>
        /// <returns>Textual representation of the input type</returns>
        internal static string GetTypeName(this Type type)
        {
            Type baseType = type.GetNonNullableType();
            string s = baseType.Name;
            if (type != baseType)
            {
                s += '?';
            }
            return s;
        }

        internal static bool IsEnumerableType(this Type enumerableType)
        {
            return FindGenericType(typeof(IEnumerable<>), enumerableType) != null;
        }

        internal static bool IsNullableType(this Type type)
        {
            return (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }
    }
}
