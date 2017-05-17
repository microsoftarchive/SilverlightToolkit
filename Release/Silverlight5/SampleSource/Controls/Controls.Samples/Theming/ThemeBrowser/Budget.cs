// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Class representing a vacation budget for use by Chart samples.
    /// </summary>
    public class Budget : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the date on which the expense was spent.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The type of expense.
        /// </summary>
        private string _expenseType;

        /// <summary>
        /// Gets or sets the type of expense.
        /// </summary>
        public string ExpenseType
        {
            get { return _expenseType; }
            set
            {
                _expenseType = value;
                int year;
                if (int.TryParse(value, out year))
                {
                    Date = new DateTime(year, 1, 1);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Volume (used for bubble chart).
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// Gets or sets the expense value.
        /// </summary>
        public double ExpenseValue
        {
            get
            {
                return _expenseValue;
            }
            set
            {
                _expenseValue = value;
                OnPropertyChanged("Count");
            }
        }

        /// <summary>
        /// Stores the expense value.
        /// </summary>
        private double _expenseValue;

        /// <summary>
        /// Fires the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Property that changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Implements the INotifyPropertyChanged interface.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}