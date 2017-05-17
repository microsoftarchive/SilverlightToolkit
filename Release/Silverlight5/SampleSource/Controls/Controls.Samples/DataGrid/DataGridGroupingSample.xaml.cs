// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Data;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the DataGrid with grouping enabled.
    /// </summary>
    [Sample("DataGrid with Grouping", DifficultyLevel.Intermediate, "DataGrid")]
    public partial class DataGridGroupingSample : UserControl
    {
        /// <summary>
        /// Initializes a DataGridGroupingSample.
        /// </summary>
        public DataGridGroupingSample()
        {
            InitializeComponent();
            PagedCollectionView pcv = new PagedCollectionView(Contact.People);
            pcv.GroupDescriptions.Add(new PropertyGroupDescription("State"));
            pcv.GroupDescriptions.Add(new PropertyGroupDescription("City"));
            DataContext = pcv;
        }
    }
}
