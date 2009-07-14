// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Reflection;
using System.Windows.Controls.Data.Test.DataClasses;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Test;
using System.Windows.Markup;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Data.Test
{
    public partial class DataGrid_DependencyProperties_TestClass
    {
        [TestMethod]
        [Description("Verify Dependency Property: (DataTemplate) DataGrid.RowDetailsTemplate.")]
        public void RowDetailsTemplate()
        {
            Type propertyType = typeof(DataTemplate);
            bool expectGet = true;  
            bool expectSet = true;
            bool hasSideEffects = true;

            DataGrid control = new DataGrid();
            Assert.IsNotNull(control);

            // Verify dependency property member
            FieldInfo fieldInfo = typeof(DataGrid).GetField("RowDetailsTemplateProperty", BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(typeof(DependencyProperty), fieldInfo.FieldType, "DataGrid.RowDetailsTemplateProperty not expected type 'DependencyProperty'.");

            // Verify dependency property's value type
            DependencyProperty property = fieldInfo.GetValue(null) as DependencyProperty;

            Assert.IsNotNull(property);

            // 


            // Verify dependency property CLR property member
            PropertyInfo propertyInfo = typeof(DataGrid).GetProperty("RowDetailsTemplate", BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(propertyInfo, "Expected CLR property DataGrid.RowDetailsTemplate does not exist.");
            Assert.AreEqual(propertyType, propertyInfo.PropertyType, "DataGrid.RowDetailsTemplate not expected type 'DataTemplate'.");

            // Verify getter/setter access
            Assert.AreEqual(expectGet, propertyInfo.CanRead, "Unexpected value for propertyInfo.CanRead.");
            Assert.AreEqual(expectSet, propertyInfo.CanWrite, "Unexpected value for propertyInfo.CanWrite.");

            // Verify that we set what we get
            if (expectSet) // if expectSet == false, this block can be removed
            {
                DataTemplate template = new DataTemplate();

                control.RowDetailsTemplate = template;

                Assert.AreEqual(template, control.RowDetailsTemplate);
            }

            // Verify dependency property callback
            if (hasSideEffects)
            {
                MethodInfo methodInfo = typeof(DataGrid).GetMethod("OnRowDetailsTemplatePropertyChanged", BindingFlags.Static | BindingFlags.NonPublic);
                Assert.IsNotNull(methodInfo, "Expected DataGrid.RowDetailsTemplate to have static, non-public side-effect callback 'OnRowDetailsTemplatePropertyChanged'.");

                // 
            }
            else
            {
                MethodInfo methodInfo = typeof(DataGrid).GetMethod("OnRowDetailsTemplatePropertyChanged", BindingFlags.Static | BindingFlags.NonPublic);
                Assert.IsNull(methodInfo, "Expected DataGrid.RowDetailsTemplate NOT to have static side-effect callback 'OnRowDetailsTemplatePropertyChanged'.");
            }
        }

        /// <summary>
        /// Tests setting a RowDetails template on a DataGrid with selection after load when the previous template was null
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests setting a RowDetails template on a DataGrid with selection after load when the previous template was null")]
        public void SetRowDetailsTemplateWithSelectionAfterLoad()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 350;
            dataGrid.Width = 350;
            this._isLoaded = false;

            CustomerList customers = new CustomerList(20);
            dataGrid.ItemsSource = customers;
            dataGrid.Loaded += this.dataGrid_Loaded;
            TestPanel.Children.Add(dataGrid);
            this.EnqueueConditional(delegate {return this._isLoaded;});

            double scrollBarMax = 0;
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                scrollBarMax = dataGrid.VerticalScrollBar.Maximum;
                dataGrid.SelectedItem = customers[0];
                dataGrid.RowDetailsTemplate = XamlReader.Load(@"
                    <DataTemplate  
                        xmlns:data=""clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data""
                        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" >
                        <Button Height=""50"" />
                    </DataTemplate>") as DataTemplate;
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                double heightIncrease = dataGrid.VerticalScrollBar.Maximum - scrollBarMax;
                Assert.IsTrue(DoubleUtil.AreClose(50, heightIncrease));
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that the Details Content information is cleared when a row is recycled
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that the Details Content information is cleared when a row is recycled")]
        public void ClearDetailsContentInformationOnRecycle()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 350;
            dataGrid.Width = 350;
            this._isLoaded = false;

            CustomerList customers = new CustomerList(20);
            dataGrid.ItemsSource = customers;
            dataGrid.Loaded += this.dataGrid_Loaded;
            TestPanel.Children.Add(dataGrid);
            this.EnqueueConditional(delegate { return this._isLoaded; });

            double scrollBarMax = 0;
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                scrollBarMax = dataGrid.VerticalScrollBar.Maximum;
                dataGrid.SelectedItem = customers[0];
                dataGrid.RowDetailsTemplate = XamlReader.Load(@"
                    <DataTemplate  
                        xmlns:data=""clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Data""
                        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" >
                        <Button Height=""50"" />
                    </DataTemplate>") as DataTemplate;
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                DataGridRow row = dataGrid.DisplayData.GetDisplayedElement(0) as DataGridRow;
                Assert.IsNotNull(row);
                double contentHeight = row.TestHook.DetailsPresenter.ContentHeight;
                Assert.AreNotEqual(0.0, contentHeight);

                // Unfortuantely, we cannot reliably get a row to be recycled through normal means but guarantee
                // that it will not be reused
                row.DetachFromDataGrid(true);
                contentHeight = row.TestHook.DetailsPresenter.ContentHeight;
                Assert.AreEqual(0.0, contentHeight);
            });

            EnqueueTestComplete();
        }
    }
}
