// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Class representing an untyped pair of values.
    /// </summary>
    public class Pair : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the first value.
        /// </summary>
        public object First
        {
            get
            {
                return _first;
            }
            set
            {
                _first = value;
                OnPropertyChanged("First");
            }
        }

        /// <summary>
        /// Stores the value of the First property.
        /// </summary>
        private object _first;

        /// <summary>
        /// Gets or sets the second value.
        /// </summary>
        public object Second
        {
            get
            {
                return _second;
            }
            set
            {
                _second = value;
                OnPropertyChanged("Second");
            }
        }

        /// <summary>
        /// Stores the value of the Second property.
        /// </summary>
        private object _second;

        /// <summary>
        /// Implements the INotifyPropertyChanged interface.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}