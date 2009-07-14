// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.Data.Test.DataClasses;
using System.Windows.Controls.Data.Test.DataClassSources;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Test
{
    /// <summary>
    /// Tests that mimick the DomainDataSource scenarios
    /// </summary>
    [TestClass]
    public class DataGridDdsTest : SilverlightControlTest
    {
        bool _loaded;

        /// <summary>
        /// Tests using a reset to load items when autogenerating columns is on and T is object
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests using a reset to load items when autogenerating columns is on and T is object")]
        public void AutoGenerateOnResetForListOfObjects()
        {
            DataGrid dataGrid = new DataGrid();
            Assert.IsNotNull(dataGrid);
            dataGrid.Width = 350;
            dataGrid.Height = 250;
            _loaded = false;
            dataGrid.Loaded += new RoutedEventHandler(DataGrid_Loaded);
            DataSourceINCC dataSource = new DataSourceINCC();
            dataGrid.ItemsSource = dataSource;
            TestPanel.Children.Add(dataGrid);

            EnqueueConditional(delegate { return _loaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                dataSource.Add(new Customer());
                dataSource.Add(new Customer());
                dataSource.Add(new Customer());
                dataSource.Add(new Customer());
                dataSource.RaiseReset();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.IsTrue(dataGrid.Columns.Count > 0);
                Assert.IsTrue(dataGrid.DisplayData.FirstScrollingSlot == 0);
                Assert.IsTrue(dataGrid.DisplayData.LastScrollingSlot == 3);
            });

            EnqueueTestComplete();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
        }
    }
}
