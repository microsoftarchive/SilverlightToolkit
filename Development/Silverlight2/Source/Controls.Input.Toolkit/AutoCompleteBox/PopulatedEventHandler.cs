// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents the method that will handle the Populated event of a 
    /// AutoCompleteBox control.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event arguments.</param>
    /// <QualityBand>Stable</QualityBand>
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification = "There is no generic RoutedEventHandler.")]
    public delegate void PopulatedEventHandler(object sender, PopulatedEventArgs e);
}