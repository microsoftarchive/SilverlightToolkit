// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Data;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page that demonstrates using a DataGrid, DataForm, and DataPager
    /// to create a paged Master-Details view.
    /// </summary>
    [Sample("Master-Details with DataGrid and DataForm", DifficultyLevel.Scenario, "DataGrid")]
    public partial class DataGridMasterDetailSample : UserControl
    {
        /// <summary>
        /// Initializes a DataGridMasterDetailsSample.
        /// </summary>
        public DataGridMasterDetailSample()
        {
            InitializeComponent();
            PagedCollectionView pcv = new PagedCollectionView(Contact.People);
            DataContext = pcv;
        }
    }
}
