// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Windows.Controls
{
    // When adding to this enum, please update the associated constant in the 
    // AutoCompleteBox class that is used for validation.

    /// <summary>
    /// Represents the different types of built-in search modes available to 
    /// the AutoCompleteBox control.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum AutoCompleteSearchMode
    {
        /// <summary>
        /// No search mode, all elements in a collection will be included in 
        /// the results. This value would be used for a data source.
        /// </summary>
        None = 0,

        /// <summary>
        /// Matches the text value to start of the string, a case insensitive 
        /// ordinal comparison is used.
        /// </summary>
        StartsWith = 1,

        /// <summary>
        /// Matches the text value to start of the string, an ordinal 
        /// comparison is used.
        /// </summary>
        StartsWithCaseSensitive = 2,

        /// <summary>
        /// Matches the text value if it is contained inside the string, a case 
        /// insensitive comparison is used. Uses the current culture value.
        /// </summary>
        Contains = 3,

        /// <summary>
        /// Matches the text value if it is contained inside the string. Uses 
        /// the current culture value.
        /// </summary>
        ContainsCaseSensitive = 4,

        /// <summary>
        /// Matches that the text if it's value equals the string, an ordinal 
        /// case insensitive comparison is used.
        /// </summary>
        Equals = 5,

        /// <summary>
        /// Matches that the text if it's value equals the string using an 
        /// ordinal case sensitive comparison operator.
        /// </summary>
        EqualsCaseSensitive = 6,

        /// <summary>
        /// Custom search mode: setting any of the item or text delegate 
        /// dependency properties and this value will enable non-standard, 
        /// custom functions and lambdas to be used.
        /// </summary>
        Custom = 7,
    }
}