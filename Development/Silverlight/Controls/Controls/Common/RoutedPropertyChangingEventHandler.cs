// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Represents methods that handle various routed events that track property
    /// values changing.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the dependency property that is changing.
    /// </typeparam>
    /// <param name="sender">
    /// The object where the event handler is attached.
    /// </param>
    /// <param name="e">The event arguments.</param>
    /// <QualityBand>Preview</QualityBand>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification = "To match pattern of RoutedPropertyChangedEventHandler<T>")]
    public delegate void RoutedPropertyChangingEventHandler<T>(object sender, RoutedPropertyChangingEventArgs<T> e);
}