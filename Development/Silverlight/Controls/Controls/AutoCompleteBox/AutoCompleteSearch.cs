// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// A predefined set of filter functions for the known, built-in 
    /// AutoCompleteSearchMode enumeration values.
    /// </summary>
    internal static class AutoCompleteSearch
    {
        /// <summary>
        /// Index function that retrieves the filter for the provided 
        /// AutoCompleteSearchMode.
        /// </summary>
        /// <param name="searchMode">The built-in search mode.</param>
        /// <returns>Returns the string-based comparison function.</returns>
        public static AutoCompleteSearchPredicate<string> GetFilter(AutoCompleteSearchMode searchMode)
        {
            switch (searchMode)
            {
                case AutoCompleteSearchMode.Contains:
                    return Contains;

                case AutoCompleteSearchMode.ContainsCaseSensitive:
                    return ContainsCaseSensitive;

                case AutoCompleteSearchMode.Equals:
                    return Equals;

                case AutoCompleteSearchMode.EqualsCaseSensitive:
                    return EqualsCaseSensitive;

                case AutoCompleteSearchMode.StartsWith:
                    return StartsWith;

                case AutoCompleteSearchMode.StartsWithCaseSensitive:
                    return StartsWithCaseSensitive;

                case AutoCompleteSearchMode.None:
                case AutoCompleteSearchMode.Custom:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Check if the string value begins with the text.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool StartsWith(string text, string value)
        {
            return value.StartsWith(text, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if the string value begins with the text.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool StartsWithCaseSensitive(string text, string value)
        {
            return value.StartsWith(text, StringComparison.Ordinal);
        }

        /// <summary>
        /// Check if the prefix is contained in the string value.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool Contains(string text, string value)
        {
            return value.ToUpper(CultureInfo.CurrentCulture).Contains(text.ToUpper(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Check if the prefix is contained in the string value.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool ContainsCaseSensitive(string text, string value)
        {
            return value.Contains(text);
        }

        /// <summary>
        /// Check if the string values are equal.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool Equals(string text, string value)
        {
            return value.Equals(text, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if the string values are equal.
        /// </summary>
        /// <param name="text">The AutoCompleteBox prefix text.</param>
        /// <param name="value">The item's string value.</param>
        /// <returns>Returns true if the condition is met.</returns>
        public static bool EqualsCaseSensitive(string text, string value)
        {
            return value.Equals(text, StringComparison.Ordinal);
        }
    }
}