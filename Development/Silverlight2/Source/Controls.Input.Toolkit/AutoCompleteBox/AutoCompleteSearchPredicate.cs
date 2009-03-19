// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents the filter used by the AutoCompleteBox control to determine 
    /// whether an item is a possible match for the specified text.
    /// </summary>
    /// <param name="search">The string used as the basis for filtering.</param>
    /// <param name="item">
    /// The item that is compared with the search parameter. 
    /// </param>
    /// <typeparam name="T">
    /// The type used for filtering the AutoCompleteBox. This type can be either 
    /// a string or an object.
    /// </typeparam>
    /// <returns>
    /// True to indicate item is a possible match for search; otherwise false.
    /// </returns>
    /// <QualityBand>Mature</QualityBand>
    public delegate bool AutoCompleteSearchPredicate<T>(string search, T item);
}