// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Collections.ObjectModel;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// This is to provide itemsource to datagrid.
    /// </summary>
    public class DataForDataGrid : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets LastName.
        /// </summary>
        public string LastName { get; set; }
     
        /// <summary>
        /// Gets or sets Age.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Generate and return a list of Data objects. 
        /// </summary>
        /// <param name="itemsCount">Number of items in the list.</param>
        /// <returns>Return the list of Data.</returns>
        public static Collection<DataForDataGrid> GenerateRecordList(int itemsCount)
        {
            Collection<DataForDataGrid> source = new Collection<DataForDataGrid>();
            Random random = new Random();
            for (int i = 0; i < itemsCount; i++)
            {
                source.Add(new DataForDataGrid()
                {
                    Id = random.Next(1234),
                    FirstName = "First",
                    LastName = "Last",
                    Age = i,
                });
            }
            return source;
        }

        /// <summary>
        /// Event PropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// SendPropertyChanged method for event.
        /// </summary>
        /// <param name="propertyName">String propertyName as parameter.</param>
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}