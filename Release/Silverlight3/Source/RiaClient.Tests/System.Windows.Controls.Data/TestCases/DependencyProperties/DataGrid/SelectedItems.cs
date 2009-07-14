// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Reflection;
using System.Windows.Controls.Data.Test.DataClasses;
using System.Windows.Controls.Test;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Data.Test
{
    public partial class DataGrid_DependencyProperties_TestClass
    {
        [TestMethod]
        [Asynchronous]
        [Description("Verify SelectedItems Property: (IList) DataGrid.SelectedItems.")]
        public void SelectedItems()
        {
            Type propertyType = typeof(IList);
            bool expectGet = true;
            bool expectSet = false;

            DataGrid control = new DataGrid();
            Assert.IsNotNull(control);
            TestPanel.Children.Add(control);

            this._counter = 0;
            control.SelectionChanged += control_SelectionChanged;

            // Verify dependency property CLR property member
            PropertyInfo propertyInfo = typeof(DataGrid).GetProperty("SelectedItems", BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(propertyInfo, "Expected CLR property DataGrid.SelectedItems does not exist.");
            Assert.AreEqual(propertyType, propertyInfo.PropertyType, "DataGrid.SelectedItems not expected type 'IList'.");

            // Verify getter/setter access
            Assert.AreEqual(expectGet, propertyInfo.CanRead, "Unexpected value for propertyInfo.CanRead.");
            Assert.AreEqual(expectSet, propertyInfo.CanWrite, "Unexpected value for propertyInfo.CanWrite.");

            CustomerList list = new CustomerList();

            Assert.IsTrue(list.Count > 3, "CustomerList has too few items for this test.");

            Assert.AreEqual(0, this._counter, "Wrong initial counter state.");

            Common.AssertExpectedException(DataGridError.DataGrid.ItemIsNotContainedInTheItemsSource("dataItem"),
                delegate
                {
                    control.SelectedItems.Add(list[2]);
                }
            );

            Assert.AreEqual(0, this._counter, "Counter should not have changed after illegal item selection.");

            control.ItemsSource = list;
            control.SelectionMode = DataGridSelectionMode.Extended;

            // Verify that you can modify SelectedItems while the DataGrid is loading
            control.SelectedItem = null;
            for (int i = 1; i < 4; i++)
            {
                control.SelectedItems.Add(list[i]);
            }
            control.SelectedItems.Remove(list[2]);

            // 







            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, this._counter, "Test0: SelectionChanged should have been called once");
                Assert.AreEqual(control.SelectedItem, list[1], "Test0: wrong selected item");
                Assert.AreEqual(2, control.SelectedItems.Count, "Test0: wrong number of selected items");

                control.SelectedItem = list[0];
                control.SelectedItems.Add(list[2]);

                Assert.AreEqual(3, this._counter, "Test0: Counter should have incremented.");

                Assert.AreEqual(list[0], control.SelectedItem, "Test1: wrong selected item -- SelectedItem should have remained the same");
                Assert.AreEqual(2, control.SelectedItems.Count, "Test1: wrong selected item count");

                control.SelectedItem = list[2];

                Assert.AreEqual(4, this._counter, "Test3: Counter should have incremented.");

                Assert.AreEqual(list[2], control.SelectedItem, "Test3: wrong selected item");
                Assert.AreEqual(1, control.SelectedItems.Count, "Test3: wrong selected item count");

                control.SelectedItems.Clear();

                Assert.AreEqual(5, this._counter, "Test4: Counter should have incremented.");

                Assert.IsNull(control.SelectedItem, "Test4: selected item not null");
                Assert.AreEqual(0, control.SelectedItems.Count, "Test4: wrong selected item count");
            });
            EnqueueTestComplete();
        }
    }
}
