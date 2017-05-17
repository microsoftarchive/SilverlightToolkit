// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Implements a simple data object subclass for testing.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    public class NotifyingDataObject<T> : DataObject<T>, INotifyPropertyChanged
    {
        /// <summary>
        /// Sets the value property.
        /// </summary>
        /// <param name="value">Value to set.</param>
        protected override void SetValue(T value)
        {
            base.SetValue(value);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }

        /// <summary>
        /// Implements the INotifyPropertyChanged interface.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}