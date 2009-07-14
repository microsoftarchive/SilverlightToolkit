// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace System.Windows.Controls
{
    internal static class TypeHelper
    {
        internal const char PropertyNameSeparator = '.';

        // Methods
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

        /// <summary>
        /// Finds the PropertyInfo for the specified property path within this Type, and returns
        /// the value of GetShortName on its DisplayAttribute, if one exists. GetShortName will return
        /// the value of Name if there is no ShortName specified.
        /// </summary>
        /// <param name="type">Type to search</param>
        /// <param name="propertyPath">property path</param>
        /// <returns>DisplayAttribute.ShortName if it exists, null otherwise</returns>
        internal static string GetDisplayName(this Type type, string propertyPath)
        {
            PropertyInfo propertyInfo = type.GetNestedProperty(propertyPath);
            if (propertyInfo != null)
            {
                object[] attributes = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                    Debug.Assert(attributes.Length == 1);
                    DisplayAttribute displayAttribute = attributes[0] as DisplayAttribute;
                    if (displayAttribute != null)
                    {
                        return displayAttribute.GetShortName();
                    }
                }
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

        internal static PropertyInfo GetNestedProperty(this Type parentType, string propertyPath)
        {
            if (parentType == null)
            {
                return null;
            }
            PropertyInfo propertyInfo = null;
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
            return propertyInfo;
        }

        internal static Type GetNestedPropertyType(this Type parentType, string propertyPath)
        {
            if (parentType == null || String.IsNullOrEmpty(propertyPath))
            {
                return parentType;
            }

            PropertyInfo propertyInfo = parentType.GetNestedProperty(propertyPath);
            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }
            return null;
        }

        internal static Type GetNonNullableType(this Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        internal static bool IsEnumerableType(this Type enumerableType)
        {
            return (FindGenericType(typeof(IEnumerable<>), enumerableType) != null);
        }

        internal static bool IsNullableType(this Type type)
        {
            return (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        internal static bool IsNullableEnum(this Type type)
        {
            return type.IsNullableType() &&
                 type.GetGenericArguments().Length == 1 &&
                 type.GetGenericArguments()[0].IsEnum;
        }
    }
}
