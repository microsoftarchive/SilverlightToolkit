// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls.Test;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Data.Test
{
    public partial class DataGrid_DependencyProperties_TestClass
    {
        [TestMethod]
        [Asynchronous]
        [Microsoft.VisualStudio.TestTools.UnitTesting.Description("Verify Dependency Property: (bool) DataGrid.AutoGenerateColumns.")]
        public void AutoGenerateColumns()
        {
            Type propertyType = typeof(bool);
            bool expectGet = true;
            bool expectSet = true;
            bool hasSideEffects = true;

            DataGrid control = new DataGrid();
            Assert.IsNotNull(control);
            TestPanel.Children.Add(control);
            this.EnqueueYieldThread();
            this.EnqueueCallback(delegate
            {
                // Verify DP Property member
                FieldInfo fieldInfo = typeof(DataGrid).GetField("AutoGenerateColumnsProperty", BindingFlags.Static | BindingFlags.Public);
                Assert.AreEqual(typeof(DependencyProperty), fieldInfo.FieldType, "DataGrid.AutoGenerateColumnsProperty not expected type 'DependencyProperty'.");

                // Verify DP Property's value type
                DependencyProperty property = fieldInfo.GetValue(null) as DependencyProperty;

                Assert.IsNotNull(property);

                // 


                // Verify DP CLR property member
                PropertyInfo propertyInfo = typeof(DataGrid).GetProperty("AutoGenerateColumns", BindingFlags.Instance | BindingFlags.Public);
                Assert.IsNotNull(propertyInfo, "Expected CLR property DataGrid.AutoGenerateColumns does not exist.");
                Assert.AreEqual(propertyType, propertyInfo.PropertyType, "DataGrid.AutoGenerateColumns not expected type 'bool'.");

                // Verify getter/setter access
                Assert.AreEqual(expectGet, propertyInfo.CanRead, "Unexpected value for propertyInfo.CanRead.");
                Assert.AreEqual(expectSet, propertyInfo.CanWrite, "Unexpected value for propertyInfo.CanWrite.");

                // Verify that we set what we get
                if (expectSet) // if expectSet == false, this block can be removed
                {
                    Assert.AreEqual(true, control.AutoGenerateColumns);

                    control.AutoGenerateColumns = false;

                    Assert.AreEqual(false, control.AutoGenerateColumns);

                    control.AutoGenerateColumns = true;

                    Assert.AreEqual(true, control.AutoGenerateColumns);
                }

                // Verify DP callback
                if (hasSideEffects)
                {
                    MethodInfo methodInfo = typeof(DataGrid).GetMethod("OnAutoGenerateColumnsPropertyChanged", BindingFlags.Static | BindingFlags.NonPublic);
                    Assert.IsNotNull(methodInfo, "Expected DataGrid.AutoGenerateColumns to have static, non-public side-effect callback 'OnAutoGenerateColumnsPropertyChanged'.");

                    // 
                }
                else
                {
                    MethodInfo methodInfo = typeof(DataGrid).GetMethod("OnAutoGenerateColumnsPropertyChanged", BindingFlags.Static | BindingFlags.NonPublic);
                    Assert.IsNull(methodInfo, "Expected DataGrid.AutoGenerateColumns NOT to have static side-effect callback 'OnAutoGenerateColumnsPropertyChanged'.");
                }
            });
            EnqueueTestComplete();
       }

        [TestMethod]
        [Asynchronous]
        [Microsoft.VisualStudio.TestTools.UnitTesting.Description("Verify Bindable Attribute: BindableAttribute is not defined on the class")]
        public void BindableAttributeGeneral()
        {
            DataGrid dataGrid = new DataGrid();
            Assert.IsNotNull(dataGrid);
            _isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);

            List<BindableAttrClass> list = new List<BindableAttrClass>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new BindableAttrClass());
            }

            dataGrid.ItemsSource = list;

            this.TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return _isLoaded; });
            EnqueueCallback(delegate
            {
                EnsureTwoWay(dataGrid);
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Microsoft.VisualStudio.TestTools.UnitTesting.Description("Verify Bindable Attribute: [Bindable(false)]")]
        public void UnBindable()
        {
            DataGrid dataGrid = new DataGrid();
            Assert.IsNotNull(dataGrid);
            _isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);

            List<UnBindableClass> list = new List<UnBindableClass>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new UnBindableClass());
            }

            dataGrid.ItemsSource = list;

            this.TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return _isLoaded; });

            EnqueueCallback(delegate
            {
                Assert.AreEqual(0, dataGrid.Columns.Count);
                Assert.AreEqual(0, dataGrid.SlotCount);
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Microsoft.VisualStudio.TestTools.UnitTesting.Description("Verify Bindable Attribute: [Bindable(true)]")]
        public void DefaultBindable()
        {
            DataGrid dataGrid = new DataGrid();
            Assert.IsNotNull(dataGrid);
            _isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);

            List<DefaultBindableClass> list = new List<DefaultBindableClass>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new DefaultBindableClass());
            }

            dataGrid.ItemsSource = list;

            this.TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return _isLoaded; });

            EnqueueCallback(delegate
            {
                EnsureDefault(dataGrid);
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Microsoft.VisualStudio.TestTools.UnitTesting.Description("Verify Bindable Attribute: [Bindable(true, BindingDirection.OneWay)]")]
        public void OneWayBindable()
        {
            DataGrid dataGrid = new DataGrid();
            Assert.IsNotNull(dataGrid);
            _isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);

            List<OneWayBindableClass> list = new List<OneWayBindableClass>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new OneWayBindableClass());
            }

            dataGrid.ItemsSource = list;

            this.TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return _isLoaded; });

            EnqueueCallback(delegate
            {
                EnsureDefault(dataGrid);
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Microsoft.VisualStudio.TestTools.UnitTesting.Description("Verify Bindable Attribute: [Bindable(true, BindingDirection.TwoWay)]")]
        public void TwoWayBindable()
        {
            DataGrid dataGrid = new DataGrid();
            Assert.IsNotNull(dataGrid);
            _isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);

            List<TwoWayBindableClass> list = new List<TwoWayBindableClass>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(new TwoWayBindableClass());
            }

            dataGrid.ItemsSource = list;

            this.TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return _isLoaded; });

            EnqueueCallback(delegate
            {
                EnsureTwoWay(dataGrid);
            });

            EnqueueTestComplete();
        }

        private void EnsureTwoWay(DataGrid dataGrid)
        {
            Assert.AreEqual(4, dataGrid.Columns.Count);
            Assert.AreEqual(10, dataGrid.SlotCount);

            foreach (DataGridColumn col in dataGrid.Columns)
            {
                switch (col.Header.ToString())
                {
                    case "Name1":
                    case "Name4":
                        {
                            Assert.IsFalse(col.IsReadOnly);
                            break;
                        }
                    case "Name2":
                    case "Name3":
                        {
                            Assert.IsTrue(col.IsReadOnly);
                            break;
                        }
                    default:
                        {
                            Assert.Fail("The column should not be autogenerated");
                            break;
                        }
                }
            }
        }

        private void EnsureDefault(DataGrid dataGrid)
        {
            Assert.AreEqual(4, dataGrid.Columns.Count);
            Assert.AreEqual(10, dataGrid.SlotCount);

            foreach (DataGridColumn col in dataGrid.Columns)
            {
                switch (col.Header.ToString())
                {
                    case "Name4":
                        {
                            Assert.IsFalse(col.IsReadOnly);
                            break;
                        }
                    case "Name1":
                    case "Name2":
                    case "Name3":
                        {
                            Assert.IsTrue(col.IsReadOnly);
                            break;
                        }
                    default:
                        {
                            Assert.Fail("The column should not be autogenerated");
                            break;
                        }
                }
            }
        }

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }

        #region Data

        private bool _isLoaded = false;

        public class BindableAttrClass
        {
            public string Name1
            {
                get;
                set;
            }

            [Bindable(true)]
            public string Name2
            {
                get;
                set;
            }

            [Bindable(true, BindingDirection.OneWay)]
            public string Name3
            {
                get;
                set;
            }

            [Bindable(true, BindingDirection.TwoWay)]
            public string Name4
            {
                get;
                set;
            }

            [Bindable(false)]
            public string Name5
            {
                get;
                set;
            }
        }

        [Bindable(false)]
        public class UnBindableClass : BindableAttrClass
        {
        }

        [Bindable(true)]
        public class DefaultBindableClass : BindableAttrClass
        {
        }

        [Bindable(true, BindingDirection.OneWay)]
        public class OneWayBindableClass : BindableAttrClass
        {
        }

        [Bindable(true, BindingDirection.TwoWay)]
        public class TwoWayBindableClass : BindableAttrClass
        {
        }
        #endregion
    }
}
