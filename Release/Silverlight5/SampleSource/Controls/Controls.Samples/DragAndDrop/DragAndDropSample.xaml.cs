// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.DataVisualization.Charting;
using System.Linq;
namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// A sample that demonstrates the use of the drag and drop functionality
    /// in the Silverlight Toolkit.
    /// </summary>
    [Sample("Drag and Drop", DifficultyLevel.Advanced, "Drag and Drop")]
    public partial class DragAndDropSample : UserControl
    {
        /// <summary>
        /// Flattens a tech employee.
        /// </summary>
        /// <param name="techEmployee">The tech employee.</param>
        /// <returns>A list including the tech employee and all reports.</returns>
        private IEnumerable<TechEmployee> FlattenTechEmployee(TechEmployee techEmployee)
        {
            yield return techEmployee;
            foreach (TechEmployee employee in techEmployee.Reports.SelectMany(emp => FlattenTechEmployee(emp)))
            {
                yield return employee;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DragAndDropSample class.
        /// </summary>
        public DragAndDropSample()
        {
            InitializeComponent();

            treeView.ItemsSource = TechEmployee.AllTechEmployees;

            ObservableCollection<TechEmployee> allEmployees = new ObservableCollection<TechEmployee>();

            foreach (TechEmployee employee in TechEmployee.AllTechEmployees.SelectMany(emp => FlattenTechEmployee(emp)))
            {
                allEmployees.Add(employee);
            }

            fromListBox.ItemsSource = allEmployees;

            ObservableCollection<TechEmployee> bugsCollection = new ObservableCollection<TechEmployee>();
            (bugsChart.Series[0] as DataPointSeries).ItemsSource = bugsCollection;
            (bugsChart.Series[1] as DataPointSeries).ItemsSource = bugsCollection;
            (salaryChart.Series[0] as DataPointSeries).ItemsSource = new ObservableCollection<TechEmployee>();
        }
    }
}
