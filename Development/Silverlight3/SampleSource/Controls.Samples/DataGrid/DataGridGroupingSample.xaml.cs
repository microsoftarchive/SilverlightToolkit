// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the DataGrid with grouping enabled.
    /// </summary>
    [Sample("DataGrid with Grouping", DifficultyLevel.Basic)]
    [Category("DataGrid")]
    public partial class DataGridGroupingSample : UserControl
    {
        /// <summary>
        /// Initializes a DataGridGroupingSample.
        /// </summary>
        public DataGridGroupingSample()
        {
            InitializeComponent();
            DataContext = Contact.People;
        }
    }
}
