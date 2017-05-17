// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Implements a simple data object class for testing.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    public class DataObject<T>
    {
        /// <summary>
        /// Gets or sets the value of the data object.
        /// </summary>
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// Stores the value of the data object.
        /// </summary>
        private T _value;

        /// <summary>
        /// Sets the value property.
        /// </summary>
        /// <param name="value">Value to set.</param>
        protected virtual void SetValue(T value)
        {
            _value = value;
        }
    }
}