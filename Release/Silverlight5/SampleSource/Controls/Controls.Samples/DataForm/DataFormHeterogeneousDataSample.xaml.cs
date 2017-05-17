// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the DataForm control with heterogeneous data.
    /// </summary>
    [Sample("DataForm with heterogeneous data", DifficultyLevel.Intermediate, "DataForm")]
    public partial class DataFormHeterogeneousDataSample : UserControl
    {
        /// <summary>
        /// Initializes a DataFormHeterogeneousDataSample.
        /// </summary>
        public DataFormHeterogeneousDataSample()
        {
            InitializeComponent();
            ObservableCollection<object> objects = new ObservableCollection<object>();
            foreach (object obj in Contact.People)
            {
                objects.Add(obj);
            }
            Random r = new Random();
            foreach (object obj in Airport.SampleAirports)
            {
                objects.Insert(r.Next(objects.Count), obj);
            }
            DataContext = new ReadOnlyObservableCollection<object>(objects);
        }
    }
}
