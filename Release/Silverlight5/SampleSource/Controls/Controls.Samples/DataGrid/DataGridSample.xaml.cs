// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the DataGrid.
    /// </summary>
    [Sample("(0)DataGrid", DifficultyLevel.Basic, "DataGrid")]
    public partial class DataGridSample : UserControl
    {
        /// <summary>
        /// Initializes a DataGridSample.
        /// </summary>
        public DataGridSample()
        {
            InitializeComponent();
            DataContext = new CustomerCollection();
        }
    }
}
