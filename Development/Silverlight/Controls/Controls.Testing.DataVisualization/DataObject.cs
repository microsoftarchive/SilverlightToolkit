// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Implements a simple data object class for testing.
    /// </summary>
    public class DataObject
    {
        /// <summary>
        /// Gets or sets the value of the data object.
        /// </summary>
        public int Value
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
        private int _value;

        /// <summary>
        /// Sets the value property.
        /// </summary>
        /// <param name="value">Value to set.</param>
        protected virtual void SetValue(int value)
        {
            _value = value;
        }
    }
}