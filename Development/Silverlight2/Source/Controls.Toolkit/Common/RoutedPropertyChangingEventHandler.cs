// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents methods that handle various routed events that track property 
    /// values changing. Typically the events denote a cancellable action.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the dependency property that is changing.
    /// </typeparam>
    /// <param name="sender">
    /// The object where the event handler is attached.
    /// </param>
    /// <param name="e">The event arguments.</param>
    /// <remarks>
    /// <para>
    /// Examples of events that use type-constrained delegates based on 
    /// RoutedPropertyChangingEventHandler(T) include DropDownClosing and 
    /// DropDownClosing.
    /// </para>
    /// <para>
    /// The difference between RoutedPropertyChanged events and 
    /// RoutedPropertyChanging events is that RoutedPropertyChanging events 
    /// typically give the application an opportunity to cancel the property 
    /// change, by setting Cancel to true in the event data.
    /// </para>
    /// </remarks>
    /// <QualityBand>Preview</QualityBand>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification = "To match pattern of RoutedPropertyChangedEventHandler<T>")]
    public delegate void RoutedPropertyChangingEventHandler<T>(object sender, RoutedPropertyChangingEventArgs<T> e);
}