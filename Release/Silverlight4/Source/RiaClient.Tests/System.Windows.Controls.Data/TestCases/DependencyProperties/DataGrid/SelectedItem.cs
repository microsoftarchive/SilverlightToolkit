// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Reflection;
using System.Windows.Controls.Data.Test.DataClasses;
using System.Windows.Controls.Test;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Data.Test
{
    public partial class DataGrid_DependencyProperties_TestClass
    {
        private bool _rowLoaded;

        [TestMethod]
        [Asynchronous]
        [Description("Verify Dependency Property: (object) DataGrid.SelectedItem.")]
        public void SelectedItem()
        {
            Type propertyType = typeof(object);

            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 300;
            Assert.IsNotNull(dataGrid);

            // Verify dependency property member
            FieldInfo fieldInfo = typeof(DataGrid).GetField("SelectedItemProperty", BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(typeof(DependencyProperty), fieldInfo.FieldType, "DataGrid.SelectedItemProperty not expected type 'DependencyProperty'.");

            // Verify dependency property's value type
            DependencyProperty property = fieldInfo.GetValue(null) as DependencyProperty;

            Assert.IsNotNull(property);

            // 


            // Verify dependency property CLR property member
            PropertyInfo propertyInfo = typeof(DataGrid).GetProperty("SelectedItem", BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(propertyInfo, "Expected CLR property DataGrid.SelectedItem does not exist.");
            Assert.AreEqual(propertyType, propertyInfo.PropertyType, "DataGrid.SelectedItem not expected type 'object'.");

            // Verify getter/setter access
            Assert.AreEqual(true, propertyInfo.CanRead, "Unexpected value for propertyInfo.CanRead.");
            Assert.AreEqual(true, propertyInfo.CanWrite, "Unexpected value for propertyInfo.CanWrite.");

            // Verify dependency property callback
            MethodInfo methodInfo = typeof(DataGrid).GetMethod("OnSelectedItemPropertyChanged", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(methodInfo, "Expected DataGrid.SelectedItem to have static, non-public side-effect callback 'OnSelectedItemPropertyChanged'.");

            // Verify that we set what we get
            CustomerList list = new CustomerList(150);

            Assert.IsTrue(list.Count > 3, "CustomerList has too few items for this test.");

            Common.AssertExpectedException(DataGridError.DataGrid.ItemIsNotContainedInTheItemsSource("dataItem"),
                delegate
                {
                    dataGrid.SelectedItems.Add(list[2]);
                }
            );

            dataGrid.ItemsSource = list;

            dataGrid.SelectedItem = list[0];

            Assert.AreEqual(list[0], dataGrid.SelectedItem);
            Assert.AreNotEqual(list[1], dataGrid.SelectedItem);

            dataGrid.SelectedItem = list[3];

            Assert.AreEqual(list[3], dataGrid.SelectedItem);

            dataGrid.SelectedItem = list[2];

            Assert.AreEqual(list[2], dataGrid.SelectedItem);

            dataGrid.SelectedItem = list[list.Count - 1];
            dataGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_LoadingRow);
            _rowLoaded = false;

            TestPanel.Children.Add(dataGrid);

            EnqueueConditional(() => _rowLoaded);
            
            this.EnqueueYieldThread();
            this.EnqueueCallback(delegate
            {
                Assert.AreEqual(dataGrid.DisplayData.FirstScrollingSlot, 0);
            });

            EnqueueTestComplete();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            _rowLoaded = true;
        }
    }
}
