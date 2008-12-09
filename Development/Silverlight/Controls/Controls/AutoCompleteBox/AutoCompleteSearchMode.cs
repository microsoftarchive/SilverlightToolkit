// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Windows.Controls
{
    // When adding to this enum, please update the OnSearchModePropertyChanged
    // in the AutoCompleteBox class that is used for validation.

    /// <summary>
    /// Represents the different types of built-in search modes available to 
    /// the AutoCompleteBox control.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public enum AutoCompleteSearchMode
    {
        /// <summary>
        /// No search mode, all elements in a collection will be included in 
        /// the results. This value would be used for a data source.
        /// </summary>
        None = 0,

        /// <summary>
        /// Matches the text value to start of the string, a current culture 
        /// case insensitive comparison is used.
        /// </summary>
        StartsWith = 1,

        /// <summary>
        /// Matches the text value to start of the string, the current culture 
        /// comparison is used.
        /// </summary>
        StartsWithCaseSensitive = 2,

        /// <summary>
        /// Matches the text value to start of the string, an ordinal comparison
        /// is used.
        /// </summary>
        StartsWithOrdinal = 3,

        /// <summary>
        /// Matches the text value to start of the string, an ordinal, case
        /// insensitive comparison is used.
        /// </summary>
        StartsWithOrdinalCaseSensitive = 4,

        /// <summary>
        /// Matches the text value if it is contained inside the string, a case 
        /// insensitive comparison is used. Uses the current culture value.
        /// </summary>
        Contains = 5,

        /// <summary>
        /// Matches the text value if it is contained inside the string. Uses 
        /// the current culture, case-sensitive comparison.
        /// </summary>
        ContainsCaseSensitive = 6,

        /// <summary>
        /// Matches the text value if it is contained inside the string. Uses 
        /// an ordinal comparison.
        /// </summary>
        ContainsOrdinal = 7,

        /// <summary>
        /// Matches the text value if it is contained inside the string. Uses 
        /// an ordinal comparison that is case-sensitive.
        /// </summary>
        ContainsOrdinalCaseSensitive = 8,

        /// <summary>
        /// Matches that the text if it's value equals the string, the current
        /// culture, case insensitive comparison is used.
        /// </summary>
        Equals = 9,

        /// <summary>
        /// Matches that the text if it's value equals the string using the 
        /// current culture's case sensitive comparison operator.
        /// </summary>
        EqualsCaseSensitive = 10,

        /// <summary>
        /// Matches that the text if it's value equals the string using an 
        /// ordinal case insensitive comparison operator.
        /// </summary>
        EqualsOrdinal = 11,

        /// <summary>
        /// Matches that the text if it's value equals the string using an 
        /// ordinal case sensitive comparison operator.
        /// </summary>
        EqualsOrdinalCaseSensitive = 12,

        /// <summary>
        /// Custom search mode: setting any of the item or text delegate 
        /// dependency properties and this value will enable non-standard, 
        /// custom functions and lambdas to be used.
        /// </summary>
        Custom = 13,
    }
}