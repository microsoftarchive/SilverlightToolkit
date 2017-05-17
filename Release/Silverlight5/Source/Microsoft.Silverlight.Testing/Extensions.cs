// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Silverlight.Testing
{
    /// <summary>
    /// Miscellaneous internal extension methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Attempts to retrieve a custom assembly attribute.
        /// </summary>
        /// <typeparam name="T">The type of attribute to retrieve.</typeparam>
        /// <param name="assembly">The assembly reference.</param>
        /// <param name="attribute">An out attribute reference.</param>
        /// <returns>Returns true if the attribute is found.</returns>
        public static bool TryGetAssemblyAttribute<T>(this Assembly assembly, out T attribute)
            where T : Attribute
        {
            Type type = typeof(T);
            attribute = null;

            object[] attributes = assembly.GetCustomAttributes(type, false);
            if (attributes.Length > 0)
            {
                attribute = attributes[0] as T;
                return (attribute != null);
            }

            return false;
        }

        /// <summary>
        /// Transform the XElement into a dictionary of key/value pairs.
        /// </summary>
        /// <typeparam name="T">The type of enumeration.</typeparam>
        /// <typeparam name="K">The key type.</typeparam>
        /// <typeparam name="R">The value type.</typeparam>
        /// <param name="that">The root enumerable.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="itemSelector">The item selector.</param>
        /// <returns>Returns a new dictionary.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T", Justification = "Simple LINQ statement.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "R", Justification = "Simple LINQ statement.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "K", Justification = "Simple LINQ statement.")]
        public static Dictionary<K, R> ToTransformedDictionary<T, K, R>(this IEnumerable<T> that, Func<T, K> keySelector, Func<T, R> itemSelector)
        {
            Dictionary<K, R> dictionary = new Dictionary<K, R>();
            foreach (var item in that)
            {
                dictionary[keySelector(item)] = itemSelector(item);
            }
            return dictionary;
        }
    }
}