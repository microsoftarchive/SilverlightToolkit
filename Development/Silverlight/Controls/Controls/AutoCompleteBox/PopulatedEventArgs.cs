// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Windows;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Provides data for the AutoCompleteBox Populated event.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public class PopulatedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the list of suggestions populated in the AutoCompleteBox selection
        /// adapter.
        /// </summary>
        public IEnumerable Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the PopulatedEventArgs.
        /// </summary>
        /// <param name="data">The IEnumerable of populated data.</param>
        public PopulatedEventArgs(IEnumerable data)
        {
            Data = data;
        }
    }
}