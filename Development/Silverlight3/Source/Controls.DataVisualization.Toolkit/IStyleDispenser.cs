// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;

namespace System.Windows.Controls.DataVisualization
{
    /// <summary>
    /// Represents a service that dispenses Styles.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public interface IStyleDispenser
    {
        /// <summary>
        /// Returns a rotating enumerator of Style objects that coordinates with 
        /// the style dispenser object to ensure that no two enumerators are
        /// currently on the same style if possible.  If the style
        /// dispenser is reset or its collection of styles is changed then
        /// the enumerators will also be reset.
        /// </summary>
        /// <param name="stylePredicate">A predicate that returns a value
        /// indicating whether to return a style.</param>
        /// <returns>An enumerator of styles.</returns>
        IEnumerator<Style> GetStylesWhere(Func<Style, bool> stylePredicate);

        /// <summary>
        /// Resets the style dispenser.
        /// </summary>
        void ResetStyles();
    }
}