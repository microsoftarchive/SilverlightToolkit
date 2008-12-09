// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Provides data for the AutoCompleteBox Populating event.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public class PopulatingEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the value of the text parameter used to determine suggestions.
        /// </summary>
        public string Parameter { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the changing event should 
        /// be canceled.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Initializes a new instance of the PopulatingEventArgs.
        /// </summary>
        /// <param name="parameter">The population parameter, provided to 
        /// observers to be used in filtering suggestions.</param>
        public PopulatingEventArgs(string parameter)
        {
            Parameter = parameter;
        }
    }
}