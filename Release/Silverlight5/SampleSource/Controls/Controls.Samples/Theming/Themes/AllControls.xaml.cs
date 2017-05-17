// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Data;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// A user control with examples of every control to demonstrate themes.
    /// </summary>
    public partial class AllControls : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the AllControls class.
        /// </summary>
        public AllControls()
        {
            InitializeComponent();

            SampleDataGrid.Loaded += delegate
            {
                // Set this in Loaded because DataGrid doesn't always handle themed Template changes well
                SampleDataGrid.ItemsSource = Employee.Executives;
            };
            SampleAutoComplete.ItemsSource = Catalog.VacationMediaItems;
            SampleDataForm.ItemsSource = Employee.Executives;

            PagedCollectionView pcv = new PagedCollectionView(Employee.Executives);
            pcv.PageSize = 1;
            DataContext = pcv;
        }
    }
}
