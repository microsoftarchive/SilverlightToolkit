// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides event data for various routed events that track property values 
    /// changing. Typically the events denote a cancellable action.
    /// </summary>
    /// <typeparam name="T">Type of the dependency property to be changed.</typeparam>
    /// <QualityBand>Preview</QualityBand>
    public class RoutedPropertyChangingEventArgs<T> : RoutedEventArgs
    {
        /// <summary>
        /// Gets the DependencyProperty identifier for the property that is 
        /// changing.
        /// </summary>
        public DependencyProperty Property { get; private set; }

        /// <summary>
        /// Gets a value that reports the previous value of the  changing 
        /// property.
        /// </summary>
        public T OldValue { get; private set; }

        /// <summary>
        /// Gets or sets a value that reports the new value of the changing 
        /// property,  assuming that the property change is not cancelled.
        /// </summary>
        public T NewValue { get; set; }

        /// <summary>
        /// Gets a value indicating whether the property change that originated 
        /// the RoutedPropertyChanging event is cancellable.
        /// </summary>
        public bool IsCancelable { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property change 
        /// that originated the RoutedPropertyChanging event should be cancelled.
        /// </summary>
        /// <remarks>
        /// Always check IsCancelable first in your event handling before 
        /// attempting to set Cancel to true.
        /// </remarks>
        public bool Cancel
        {
            get { return _cancel; }
            set
            {
                if (IsCancelable)
                {
                    _cancel = value;
                }
                else if (value)
                {
                    throw new InvalidOperationException(Properties.Resources.RoutedPropertyChangingEventArgs_CancelSet_InvalidOperation);
                }
            }
        }

        /// <summary>
        /// Private member variable for Cancel property.
        /// </summary>
        private bool _cancel;

        /// <summary>
        /// Gets or sets a value indicating whether internal value coercion is 
        /// acting on the property change that originated the 
        /// RoutedPropertyChanging event.
        /// </summary>
        /// <remarks>
        /// Coercion of values is not inherent in the property system in 
        /// Silverlight. Implementations might use this value as a sentinel to 
        /// denote that processing in the handler has entered a custom 
        /// routine that coerces the value based on the reported old and new 
        /// values. The coercion routine would not attempt to raise the 
        /// event again in this case.
        /// </remarks>
        public bool InCoercion { get; set; }

        /// <summary>
        /// Initializes a new instance of the RoutedPropertyChangingEventArgs.
        /// </summary>
        /// <param name="property">The dependency property whose value is changing.</param>
        /// <param name="oldValue">Old value of the property.</param>
        /// <param name="newValue">New value of the property.</param>
        /// <param name="isCancelable">A valid indicating whether the event is cancelable.</param>
        public RoutedPropertyChangingEventArgs(
            DependencyProperty property,
            T oldValue, 
            T newValue, 
            bool isCancelable)
        {
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
            IsCancelable = isCancelable;
            Cancel = false;
        }
    }
}