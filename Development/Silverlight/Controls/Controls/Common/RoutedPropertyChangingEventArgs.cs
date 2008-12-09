// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Arguments for RoutedPropertyChangingEvent.
    /// It allows event handlers to cancel the event by setting Cancel property to true.
    /// It allows event thrower to decide whether the event can be canceled by setting IsCancelable.
    /// </summary>
    /// <typeparam name="T">Type of the dependency property to be changed.</typeparam>
    /// <QualityBand>Preview</QualityBand>
    public class RoutedPropertyChangingEventArgs<T> : RoutedEventArgs
    {
        /// <summary>
        /// Gets the DependencyProperty whose value is changing.
        /// </summary>
        public DependencyProperty Property { get; private set; }

        /// <summary>
        /// Gets old value of the property.
        /// </summary>
        public T OldValue { get; private set; }

        /// <summary>
        /// Gets or sets new value of the property.
        /// </summary>
        public T NewValue { get; set; }

        /// <summary>
        /// Gets a value indicating whether the changing event can be canceled. 
        /// When this is false, setting Cancel to true will cause an InvalidOperationException.
        /// </summary>
        public bool IsCancelable { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the changing event should be canceled. 
        /// If IsCancelable is false, setting this value to true will cause an InvalidOperationException.
        /// </summary>
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
        /// Gets or sets a value indicating whether we are in the middle of Value coercion.
        /// </summary>
        /// <remarks>
        /// This is a total hack to work around the class hierarchy for Value coercion in NumericUpDown.
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