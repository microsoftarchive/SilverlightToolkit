// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample demonstrating the TreeView restyled as an organizational chart.
    /// </summary>
    [Sample("Organizational Chart", DifficultyLevel.Scenario, "TreeView")]
    public partial class OrganizationalChartSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the OrganizationalChartSample class.
        /// </summary>
        public OrganizationalChartSample()
        {
            InitializeComponent();
            DepartmentTree.ItemsSource = Department.AllDepartments;
        }
    }
}