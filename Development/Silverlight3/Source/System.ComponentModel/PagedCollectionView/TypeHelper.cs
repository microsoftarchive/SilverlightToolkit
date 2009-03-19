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

namespace System.ComponentModel
{
    /// <summary>
    /// Utility class for Type related operations
    /// </summary>
    internal static class TypeHelper
    {
        #region Internal Fields
        internal const char PropertyNameSeparator = '.';
        #endregion

        /// <summary>
        /// Attempts to convert a value from type <typeparamref name="T"/> to the type of <paramref name="sourceType"/>.
        /// </summary>
        /// <typeparam name="T">Type to convert from.</typeparam>
        /// <param name="sourceType">Type to attempt to convert to.</param>
        /// <param name="value">Value to attempt to convert.</param>
        /// <returns>A converted value of type <paramref name="sourceType"/> or null.</returns>
        public static object ConvertFrom<T>(this Type sourceType, T value)
        {
            foreach (Type typeConverterType in sourceType.GetTypeConverters())
            {
                TypeConverter typeConverter = Activator.CreateInstance(typeConverterType) as TypeConverter;
                if (typeConverter != null && typeConverter.CanConvertFrom(typeof(T)))
                {
                    return typeConverter.ConvertFrom(value);
                }
            }

            return null;
        }

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

        internal static Type GetItemType(this Type enumerableType)
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
        /// <param name="exception">Potential exception</param>
        /// <returns>Property value</returns>
        internal static object GetNestedPropertyValue(object item, string propertyPath, out Exception exception)
        {
            Debug.Assert(item != null, "Unexpected null item in TypeHelper.GetNestedPropertyValue");
            object value = null;
            exception = GetOrSetNestedPropertyValue(false /*set*/, item, ref value, propertyPath);
            return value;
        }

        internal static Type GetNonNullableType(this Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// Gets or sets the value of a public instance property. The property can be nested. 
        /// </summary>
        /// <param name="set">Set to true to write the property value</param>
        /// <param name="item">Object that exposes the property</param>
        /// <param name="value">Property value</param>
        /// <param name="propertyPath">Property path</param>
        /// <returns>Potential exception</returns>
        private static Exception GetOrSetNestedPropertyValue(bool set, object item, ref object value, string propertyPath)
        {
            Debug.Assert(item != null, "Unexpected null item in TypeHelper.GetOrSetNestedPropertyValue");
            Debug.Assert(propertyPath != null, "Unexpected null propertyPath in TypeHelper.GetOrSetNestedPropertyValue");
            if (!set)
            {
                value = null;
            }
            string[] propertyNames = propertyPath.Split(PropertyNameSeparator);
            for (int i = 0; i < propertyNames.Length; i++)
            {
                if (item == null)
                {
                    Debug.Assert(i > 0, "Unexpected i==0 in TypeHelper.GetOrSetNestedPropertyValue");
                    return new InvalidOperationException(string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        PagedCollectionViewResources.InvalidPropertyAccess,
                        propertyNames[i - 1],
                        propertyNames[i]));
                }
                Type type = item.GetType();
                PropertyInfo propertyInfo = type.GetProperty(propertyNames[i]);
                if (propertyInfo == null)
                {
                    return new InvalidOperationException(string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        PagedCollectionViewResources.PropertyNotFound,
                        propertyNames[i],
                        type.GetTypeName()));
                }
                
                bool setProperty = set && i == propertyNames.Length - 1;
                if (setProperty)
                {
                    if (!propertyInfo.CanWrite)
                    {
                        return new InvalidOperationException(string.Format(
                            System.Globalization.CultureInfo.InvariantCulture,
                            PagedCollectionViewResources.PropertyNotWritable,
                            propertyNames[i],
                            type.GetTypeName()));
                    }
                }
                else if (!propertyInfo.CanRead)
                {
                    return new InvalidOperationException(string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        PagedCollectionViewResources.PropertyNotReadable,
                        propertyNames[i],
                        type.GetTypeName()));
                }

                if (setProperty)
                {
                    propertyInfo.SetValue(item, value, null);
                }
                else
                {
                    if (i == propertyNames.Length - 1)
                    {
                        value = propertyInfo.GetValue(item, null);
                    }
                    else
                    {
                        item = propertyInfo.GetValue(item, null);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the PropertyInfo corresponding to the provided propertyPath. The propertyPath can be a dotted
        /// path where each section is a public property name. Only public instance properties are searched for.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyPath"></param>
        /// <returns>The found PropertyInfo or null otherwise</returns>
        internal static PropertyInfo GetPropertyInfo(this Type type, string propertyPath)
        {
            Debug.Assert(type != null, "Unexpected null type in TypeHelper.GetPropertyOrFieldInfo");
            if (!String.IsNullOrEmpty(propertyPath))
            {
                string[] propertyNames = propertyPath.Split(PropertyNameSeparator);
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    PropertyInfo propertyInfo = type.GetProperty(propertyNames[i]);
                    if (propertyInfo == null)
                    {
                        return null;
                    }
                    if (i == propertyNames.Length - 1)
                    {
                        return propertyInfo;
                    }
                    else
                    {
                        type = propertyInfo.PropertyType;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the friendly name for a type
        /// </summary>
        /// <param name="type"></param>
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

        internal static bool IsEnumType(this Type type)
        {
            return type.GetNonNullableType().IsEnum;
        }


        internal static bool IsNullableType(this Type type)
        {
            return (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        internal static bool IsNullableEnum(this Type type)
        {
            return type != null &&
                type.IsNullableType() &&
                type.GetGenericArguments().Length == 1 &&
                type.GetGenericArguments()[0].IsEnum;
        }

        /// <summary>
        /// Returns an array of Types registered as TypeConverters for a given type.
        /// </summary>
        /// <param name="type">Type to look for a TypeConverter for.</param>
        /// <returns>An array of Types containing TypeConverter types.</returns>
        public static Type[] GetTypeConverters(this Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(TypeConverterAttribute), true);
            Type[] typeConverters = new Type[attributes.Length];

            for (int i = 0; i < attributes.Length; ++i)
            {
                TypeConverterAttribute typeConverterAttribute = attributes[i] as TypeConverterAttribute;
                Type typeConverterType = Type.GetType(typeConverterAttribute.ConverterTypeName);
                typeConverters[i] = typeConverterType;
            }

            return typeConverters;
        }

        /// <summary>
        /// Sets the value of a property. That property can be nested. 
        /// Only works on public instance properties.
        /// </summary>
        /// <param name="item">Object that exposes the property</param>
        /// <param name="value">Property value</param>
        /// <param name="propertyPath">Property path</param>
        /// <returns>Potential exception</returns>
        internal static Exception SetNestedPropertyValue(object item, object value, string propertyPath)
        {
            Debug.Assert(item != null, "Unexpected null item in TypeHelper.SetNestedPropertyValue");
            return GetOrSetNestedPropertyValue(true /*set*/, item, ref value, propertyPath);
        }
    }
}
