// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Represents a method that filters items and determines whether they are 
    /// valid suggestions given the search parameter.
    /// </summary>
    /// <param name="search">The search string.</param>
    /// <param name="item">The string representation of the current item being 
    /// evaluated by the predicate.</param>
    /// <typeparam name="T">Whether the predicate will operate on object or 
    /// string values.</typeparam>
    /// <returns>Returns true if the item should be included as a suggestion 
    /// given the search term.</returns>
    /// <QualityBand>Stable</QualityBand>
    public delegate bool AutoCompleteSearchPredicate<T>(string search, T item);
}