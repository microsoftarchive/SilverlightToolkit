// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls
{
    // When adding to this enum, please update the OnSearchModePropertyChanged
    // in the AutoCompleteBox class that is used for validation.

    /// <summary>
    /// Specifies how text in the text box portion of the AutoCompleteBox 
    /// control is used to filter items specified by the ItemsSource 
    /// property for display in the drop-down.
    /// </summary>
    /// <remarks>
    /// An ordinal filter is a filter that uses the Unicode values of strings 
    /// when performing comparisons.
    /// Use None when the data source performs its own filtering.
    /// </remarks>
    /// <QualityBand>Stable</QualityBand>
    public enum AutoCompleteSearchMode
    {
        /// <summary>
        /// Specifies that no filter is used. All items are returned.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies a culture-sensitive, case-insensitive filter where the 
        /// returned items start with the specified text. The filter uses the 
        /// String.StartsWith method and the 
        /// StringComparer.CurrentCultureIgnoreCase property.
        /// </summary>
        StartsWith = 1,

        /// <summary>
        /// Specifies a culture-sensitive, case-sensitive filter where the 
        /// returned items start with the specified text. 
        /// The filter uses the String.StartsWith method and the 
        /// StringComparer.CurrentCulture property.
        /// </summary>
        StartsWithCaseSensitive = 2,

        /// <summary>
        /// Specifies an ordinal, case-insensitive filter where the returned 
        /// items start with the specified text. The filter uses the 
        /// String.StartsWith method and the StringComparer.OrdinalIgnoreCase 
        /// property.
        /// </summary>
        StartsWithOrdinal = 3,

        /// <summary>
        /// Specifies an ordinal, case-sensitive filter where the returned items 
        /// start with the specified text. 
        /// The filter uses the String.StartsWith method and the 
        /// StringComparer.Ordinal property.
        /// </summary>
        StartsWithOrdinalCaseSensitive = 4,

        /// <summary>
        /// Specifies a culture-sensitive, case-insensitive filter 
        /// where the returned items contain the specified text.
        /// </summary>
        Contains = 5,

        /// <summary>
        /// MSpecifies a culture-sensitive, case-sensitive filter where 
        /// the returned items contain the specified text.
        /// </summary>
        ContainsCaseSensitive = 6,

        /// <summary>
        /// Specifies an ordinal, case-insensitive filter where the 
        /// returned items contain the specified text.
        /// </summary>
        ContainsOrdinal = 7,

        /// <summary>
        /// Specifies an ordinal, case-sensitive filter where the returned 
        /// items contain the specified text.
        /// </summary>
        ContainsOrdinalCaseSensitive = 8,

        /// <summary>
        /// Specifies a culture-sensitive, case-insensitive filter where the 
        /// returned items equal the specified text. The filter uses the 
        /// String.Equals method and the 
        /// StringComparer.CurrentCultureIgnoreCase property.
        /// </summary>
        Equals = 9,

        /// <summary>
        /// Specifies a culture-sensitive, case-sensitive filter where the 
        /// returned items equals the specified text. The filter uses the 
        /// String.Equals method and the StringComparer.CurrentCulture property.
        /// </summary>
        EqualsCaseSensitive = 10,

        /// <summary>
        /// Specifies an ordinal, case-insensitive filter where the returned 
        /// items equal the specified text. The filter uses the String.Equals 
        /// method and the StringComparer.OrdinalIgnoreCase property.
        /// </summary>
        EqualsOrdinal = 11,

        /// <summary>
        /// Specifies an ordinal, case-sensitive filter where the returned items
        ///  equal the specified text. The filter uses the String.Equals 
        /// method and the StringComparer.Ordinal property.
        /// </summary>
        EqualsOrdinalCaseSensitive = 12,

        /// <summary>
        /// Specifies that a custom filter is used. This mode is used when the 
        /// AutoCompleteBox.TextFilter or AutoCompleteBox.ItemFilter 
        /// properties are set. 
        /// </summary>
        Custom = 13,
    }
}