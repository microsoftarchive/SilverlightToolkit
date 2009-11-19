// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls.Test;
using System.Windows.Data;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Data.Test
{
    public partial class DataGrid_DependencyProperties_TestClass
    {
        #region BindItemsSource Test

        [TestMethod]
        [Asynchronous]
        [Description("Verify that OneWay binding to the ItemsSourceDependency Property works.")]
        public void BindItemsSource()
        {
            // Create the original DataContext object
            StringsContainer stringsContainer1 = new StringsContainer();
            stringsContainer1.Strings = new ObservableCollection<string> { "first", "second", "third" };

            // Create the next object to be used as DataContext
            StringsContainer stringsContainer2 = new StringsContainer();
            stringsContainer2.Strings = new ObservableCollection<string> { "one", "two", "three", "four", "five" };

            // Create the DataGrid and setup its binding
            DataGrid dataGrid = new DataGrid();
            dataGrid.DataContext = stringsContainer1;
            Binding binding = new Binding("Strings");
            binding.Mode = BindingMode.OneWay;
            dataGrid.SetBinding(DataGrid.ItemsSourceProperty, binding);
            TestPanel.Children.Add(dataGrid);

            this.EnqueueCallback(delegate
            {
                Assert.AreEqual(stringsContainer1.Strings, dataGrid.ItemsSource, "ItemsSource was not set from the original DataContext");
                Assert.AreEqual(3, dataGrid.DataConnection.Count, "ItemsSource was not set from the original DataContext");
                dataGrid.DataContext = stringsContainer2;
            });
            this.EnqueueYieldThread();

            this.EnqueueCallback(delegate
            {
                Assert.AreEqual(stringsContainer2.Strings, dataGrid.ItemsSource, "ItemsSource did not change along with the DataContext");
                Assert.AreEqual(5, dataGrid.DataConnection.Count, "ItemsSource did not change along with the DataContext");
            });
            this.EnqueueTestComplete();
        }

        // Helper class for BindItemsSource test, needs to be public for Binding to work
        public class StringsContainer
        {
            public ObservableCollection<string> Strings
            {
                get;
                set;
            }
        }

        #endregion BindItemsSource Test

        #region ItemsSource Test

        [TestMethod]
        [Asynchronous]
        [Description("Verify Dependency Property: (IEnumerable) DataGrid.ItemsSource.")]
        public void ItemsSource()
        {
            Type propertyType = typeof(IEnumerable);
            bool expectGet = true;
            bool expectSet = true;
            bool hasSideEffects = true;

            DataGrid control = new DataGrid();
            TestPanel.Children.Add(control);
            Assert.IsNotNull(control);

            // Verify dependency property member
            FieldInfo fieldInfo = typeof(DataGrid).GetField("ItemsSourceProperty", BindingFlags.Static | BindingFlags.Public);
            Assert.AreEqual(typeof(DependencyProperty), fieldInfo.FieldType, "DataGrid.ItemsSourceProperty not expected type 'DependencyProperty'.");

            // Verify dependency property's value type
            DependencyProperty property = fieldInfo.GetValue(null) as DependencyProperty;

            Assert.IsNotNull(property);

            // 


            // Verify dependency property CLR property member
            PropertyInfo propertyInfo = typeof(DataGrid).GetProperty("ItemsSource", BindingFlags.Instance | BindingFlags.Public);
            Assert.IsNotNull(propertyInfo, "Expected CLR property DataGrid.ItemsSource does not exist.");
            Assert.AreEqual(propertyType, propertyInfo.PropertyType, "DataGrid.ItemsSource not expected type 'IEnumerable'.");

            // Verify getter/setter access
            Assert.AreEqual(expectGet, propertyInfo.CanRead, "Unexpected value for propertyInfo.CanRead.");
            Assert.AreEqual(expectSet, propertyInfo.CanWrite, "Unexpected value for propertyInfo.CanWrite.");

            // Verify that we set what we get
            if (expectSet) // if expectSet == false, this block can be removed
            {
                control.SelectionChanged += new SelectionChangedEventHandler(control_SelectionChanged);

                ObservableCollection<int> data1 = new ObservableCollection<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                ObservableCollection<string> data2 = new ObservableCollection<string> { "zero", "one", "two" };
                ObservableCollection<double> data3 = new ObservableCollection<double> { 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
                PagedCollectionView pcv1 = new PagedCollectionView(data1);
                PagedCollectionView pcv2 = new PagedCollectionView(data2);
                PagedCollectionView pcv3 = new PagedCollectionView(data3);

                this.EnqueueCallback(delegate
                {
                    control.ItemsSource = data1;
                });
                this.EnqueueYieldThread();
                
                this.EnqueueCallback(delegate
                {
                    Assert.AreEqual(data1, control.ItemsSource);
                    Assert.IsNull(control.SelectedItem, "No items should be selected by default if ItemsSource was not set to an ICollectionView");
                    Assert.AreEqual(-1, control.SelectedIndex, "No items should be selected by default if ItemsSource was not set to an ICollectionView");
                    Assert.AreEqual(0, control.CurrentColumnIndex, "The first cell should be current by default");
                    Assert.AreEqual(control.Columns[0], control.CurrentColumn, "The first cell should be current by default");

                    Assert.AreEqual(0, this._counter, "SelectionChanged should not have been raised yet");
                    Assert.IsNull(this._selectionChangedEventArgs, "SelectionChanged should not have been raised yet");

                    control.ItemsSource = data2;
                    control.SelectedIndex = 1;

                    // Force a CollectionChanged.Reset by adding and removing a GroupDescription, in order to
                    // verify that resetting the collection while a SelectionChanged event is pending will still work correctly.
                    Assert.IsNotNull(control.DataConnection.CollectionView);
                    control.DataConnection.CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Length"));
                    control.DataConnection.CollectionView.GroupDescriptions.Clear();
                });
                this.EnqueueYieldThread();

                this.EnqueueCallback(delegate
                {
                    Assert.AreEqual(data2, control.ItemsSource);
                    Assert.AreEqual(data2[1], control.SelectedItem, "Setting SelectedIndex after ItemsSource did not work");
                    Assert.AreEqual(1, control.SelectedIndex, "Setting SelectedIndex after ItemsSource did not work");
                    Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChanged should have been raised");
                    Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count);
                    Assert.AreEqual(0, control.CurrentColumnIndex, "The first cell should be current by default");
                    Assert.AreEqual(control.Columns[0], control.CurrentColumn, "The first cell should be current by default");

                    Assert.AreEqual(1, this._counter, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs.AddedItems);
                    Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count, "A single item should have been added");
                    Assert.AreEqual(data2[1], this._selectionChangedEventArgs.AddedItems[0], "A single item should have been added");
                    Assert.IsNotNull(this._selectionChangedEventArgs.RemovedItems);
                    Assert.AreEqual(0, this._selectionChangedEventArgs.RemovedItems.Count, "No items should have been removed");

                    control.ItemsSource = data3;
                    control.SelectedItem = data3[3];
                });
                this.EnqueueYieldThread();

                this.EnqueueCallback(delegate
                {
                    Assert.AreEqual(data3, control.ItemsSource);
                    Assert.AreEqual(data3[3], control.SelectedItem, "Setting SelectedItem after ItemsSource did not work");
                    Assert.AreEqual(3, control.SelectedIndex, "Setting SelectedItem after ItemsSource did not work");
                    Assert.AreEqual(0, control.CurrentColumnIndex, "The first cell should be current by default");
                    Assert.AreEqual(control.Columns[0], control.CurrentColumn, "The first cell should be current by default");

                    Assert.AreEqual(2, this._counter, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs.AddedItems);
                    Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count, "A single item should have been added");
                    Assert.AreEqual(data3[3], this._selectionChangedEventArgs.AddedItems[0], "A single item should have been added");
                    Assert.IsNotNull(this._selectionChangedEventArgs.RemovedItems);
                    Assert.AreEqual(1, this._selectionChangedEventArgs.RemovedItems.Count, "A single item should have been removed");
                    Assert.AreEqual(data2[1], this._selectionChangedEventArgs.RemovedItems[0], "A single item should have been removed");

                    control.ItemsSource = pcv1;
                });
                this.EnqueueYieldThread();

                this.EnqueueCallback(delegate
                {
                    Assert.AreEqual(pcv1, control.ItemsSource);
                    Assert.AreEqual(pcv1[0], control.SelectedItem, "First item was not selected when setting ItemsSource to an ICollectionView");
                    Assert.AreEqual(0, control.SelectedIndex, "First item was not selected when setting ItemsSource to an ICollectionView");
                    Assert.AreEqual(0, control.CurrentColumnIndex, "The first cell should be current by default");
                    Assert.AreEqual(control.Columns[0], control.CurrentColumn, "The first cell should be current by default");

                    Assert.AreEqual(3, this._counter, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs.AddedItems);
                    Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count, "A single item should have been added");
                    Assert.AreEqual(pcv1[0], this._selectionChangedEventArgs.AddedItems[0], "A single item should have been added");
                    Assert.IsNotNull(this._selectionChangedEventArgs.RemovedItems);
                    Assert.AreEqual(1, this._selectionChangedEventArgs.RemovedItems.Count, "A single item should have been removed");
                    Assert.AreEqual(data3[3], this._selectionChangedEventArgs.RemovedItems[0], "A single item should have been removed");

                    pcv2.MoveCurrentToPosition(1);
                    control.ItemsSource = pcv2;
                });
                this.EnqueueYieldThread();

                this.EnqueueCallback(delegate
                {
                    Assert.AreEqual(pcv2, control.ItemsSource);
                    Assert.AreEqual(pcv2[1], control.SelectedItem, "CollectionView.CurrentItem was not selected by default");
                    Assert.AreEqual(1, control.SelectedIndex, "CollectionView.CurrentItem was not selected by default");
                    Assert.AreEqual(0, control.CurrentColumnIndex, "The first cell should be current by default");
                    Assert.AreEqual(control.Columns[0], control.CurrentColumn, "The first cell should be current by default");

                    Assert.AreEqual(4, this._counter, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs.AddedItems);
                    Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count, "A single item should have been added");
                    Assert.AreEqual(pcv2[1], this._selectionChangedEventArgs.AddedItems[0], "A single item should have been added");
                    Assert.IsNotNull(this._selectionChangedEventArgs.RemovedItems);
                    Assert.AreEqual(1, this._selectionChangedEventArgs.RemovedItems.Count, "A single item should have been removed");
                    Assert.AreEqual(pcv1[0], this._selectionChangedEventArgs.RemovedItems[0], "A single item should have been removed");

                    pcv3.MoveCurrentToPosition(-1);
                    control.ItemsSource = pcv3;
                });
                this.EnqueueYieldThread();

                this.EnqueueCallback(delegate
                {
                    Assert.AreEqual(pcv3, control.ItemsSource);
                    Assert.IsNull(control.SelectedItem, "No items should be selected if CollectionView.CurrentItem is null");
                    Assert.AreEqual(-1, control.SelectedIndex, "No items should be selected if CollectionView.CurrentItem is null");
                    Assert.AreEqual(-1, control.CurrentColumnIndex, "There should be no current cell if CollectionView.CurrentItem is null");
                    Assert.IsNull(control.CurrentColumn, "There should be no current cell if CollectionView.CurrentItem is null");

                    Assert.AreEqual(5, this._counter, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs.AddedItems);
                    Assert.AreEqual(0, this._selectionChangedEventArgs.AddedItems.Count, "No items should have been added");
                    Assert.IsNotNull(this._selectionChangedEventArgs.RemovedItems);
                    Assert.AreEqual(1, this._selectionChangedEventArgs.RemovedItems.Count, "A single item should have been removed");
                    Assert.AreEqual(pcv2[1], this._selectionChangedEventArgs.RemovedItems[0], "A single item should have been removed");

                    control.ProcessDownKey();
                });
                this.EnqueueYieldThread();

                this.EnqueueCallback(delegate
                {
                    Assert.AreEqual(pcv3[0], control.SelectedItem, "The first item should be selected after pressing the down key");
                    Assert.AreEqual(0, control.SelectedIndex, "The first item should be selected after pressing the down key");
                    Assert.AreEqual(0, control.CurrentColumnIndex, "The first cell should be current after pressing the down key");
                    Assert.AreEqual(control.Columns[0], control.CurrentColumn, "The first cell should be current after pressing the down key");

                    Assert.AreEqual(6, this._counter, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChanged should have been raised");
                    Assert.IsNotNull(this._selectionChangedEventArgs.AddedItems);
                    Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count, "A single item should have been added");
                    Assert.AreEqual(pcv3[0], this._selectionChangedEventArgs.AddedItems[0], "A single item should have been added");
                    Assert.IsNotNull(this._selectionChangedEventArgs.RemovedItems);
                    Assert.AreEqual(0, this._selectionChangedEventArgs.RemovedItems.Count, "No items should have been removed");
                });
            }

            // Verify dependency property callback
            if (hasSideEffects)
            {
                MethodInfo methodInfo = typeof(DataGrid).GetMethod("OnItemsSourcePropertyChanged", BindingFlags.Static | BindingFlags.NonPublic);
                Assert.IsNotNull(methodInfo, "Expected DataGrid.ItemsSource to have static, non-public side-effect callback 'OnItemsSourcePropertyChanged'.");

                // 
            }
            else
            {
                MethodInfo methodInfo = typeof(DataGrid).GetMethod("OnItemsSourcePropertyChanged", BindingFlags.Static | BindingFlags.NonPublic);
                Assert.IsNull(methodInfo, "Expected DataGrid.ItemsSource NOT to have static side-effect callback 'OnItemsSourcePropertyChanged'.");
            }

            this.EnqueueTestComplete();
        }

        #endregion ItemsSource Test

        #region SetItemsSourceAndChangePageSize Test

        [TestMethod]
        [Asynchronous]
        [Description("Change the PagedCollectionView's PageSize immediately after setting the ItemsSource and before the visuals have loaded.")]
        public void SetItemsSourceAndChangePageSize()
        {
            // Create the ItemsSource
            List<string> strings = new List<string>() { "one", "two", "three", "four" };
            PagedCollectionView pagedCollectionView = new PagedCollectionView(strings);
            pagedCollectionView.MoveCurrentToLast();

            // Create the DataGrid
            bool isLoaded = false;
            DataGrid dataGrid = new DataGrid();
            dataGrid.Loaded += delegate { isLoaded = true; };
            dataGrid.ItemsSource = pagedCollectionView;
            TestPanel.Children.Add(dataGrid);
            this.EnqueueConditional(delegate { return isLoaded; });

            // Initially, the DG's selected item should be the last item
            Assert.AreEqual(4, dataGrid.SlotCount, "Incorrect SlotCount after adding and removing items");
            Assert.AreEqual(4, dataGrid.VisibleSlotCount, "Incorrect VisibleSlotCount after adding and removing items");
            Assert.AreEqual(3, dataGrid.SelectedIndex, "SelectedIndex was not updated when collection changed");
            Assert.AreEqual("four", dataGrid.SelectedItem, "SelectedItem was not updated when collection changed");

            // Changing the PageSize should cause the current and selected item to change
            pagedCollectionView.PageSize = 2;
            this.EnqueueYieldThread();
            this.EnqueueCallback(delegate
            {
                Assert.AreEqual(2, dataGrid.SlotCount, "Incorrect SlotCount after adding and removing items");
                Assert.AreEqual(2, dataGrid.VisibleSlotCount, "Incorrect VisibleSlotCount after adding and removing items");
                Assert.AreEqual(0, dataGrid.SelectedIndex, "SelectedIndex was not updated when collection changed");
                Assert.AreEqual("one", dataGrid.SelectedItem, "SelectedItem was not updated when collection changed");
            });
            this.EnqueueTestComplete();
        }

        #endregion SetItemsSourceAndChangePageSize Test

        #region SetItemsSourceAndModifyCollection Test

        [TestMethod]
        [Asynchronous]
        [Description("Modify the collection immediately after setting the ItemsSource and before the visuals have loaded.")]
        public void SetItemsSourceAndModifyCollection()
        {
            // Create the DataGrid and setup its binding
            ObservableCollection<string> strings = new ObservableCollection<string>() { "one", "two", "three" };
            bool isLoaded = false;
            DataGrid dataGrid = new DataGrid();
            dataGrid.Loaded += delegate { isLoaded = true; };

            // Set the ItemsSource and verify the slot counts
            dataGrid.ItemsSource = strings;
            dataGrid.SelectedIndex = 0;
            Assert.AreEqual(3, dataGrid.SlotCount, "Incorrect SlotCount after setting ItemsSource");
            Assert.AreEqual(3, dataGrid.VisibleSlotCount, "Incorrect VisibleSlotCount after setting ItemsSource");
            Assert.AreEqual(0, dataGrid.SelectedIndex, "SelectedIndex incorrect");

            // Remove an item and verify that the DataGrid is updated
            strings.RemoveAt(1);
            Assert.AreEqual(2, dataGrid.SlotCount, "Incorrect SlotCount after removing an item");
            Assert.AreEqual(2, dataGrid.VisibleSlotCount, "Incorrect VisibleSlotCount after removing an item");
            Assert.AreEqual(0, dataGrid.SelectedIndex, "SelectedIndex incorrect after removing an item");

            // Add an item and verify that the DataGrid is updated
            strings.Add("four");
            Assert.AreEqual(3, dataGrid.SlotCount, "Incorrect SlotCount after adding an item");
            Assert.AreEqual(3, dataGrid.VisibleSlotCount, "Incorrect VisibleSlotCount after adding an item");
            Assert.AreEqual(0, dataGrid.SelectedIndex, "SelectedIndex incorrect after adding an item");

            // Insert an item an dverify that the DataGrid is updated
            strings.Insert(0, "zero");
            Assert.AreEqual(4, dataGrid.SlotCount, "Incorrect SlotCount after inserting an item");
            Assert.AreEqual(4, dataGrid.VisibleSlotCount, "Incorrect VisibleSlotCount after inserting an item");
            Assert.AreEqual(1, dataGrid.SelectedIndex, "SelectedIndex incorrect after inserting an item");

            // Add the DataGrid to the visual tree
            TestPanel.Children.Add(dataGrid);
            this.EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            this.EnqueueCallback(delegate
            {
                Assert.AreEqual(4, dataGrid.SlotCount, "Incorrect SlotCount after adding and removing items");
                Assert.AreEqual(4, dataGrid.VisibleSlotCount, "Incorrect VisibleSlotCount after adding and removing items");
                Assert.AreEqual(1, dataGrid.SelectedIndex, "SelectedIndex was not updated when collection changed");
                Assert.AreEqual("one", dataGrid.SelectedItem, "SelectedItem was not updated when collection changed");

                DataGridRow row = dataGrid.DisplayData.GetDisplayedRow(0);
                Assert.IsNotNull(row);
                Assert.AreEqual(0, row.Slot, "Incorrect Row.Slot");
                Assert.AreEqual(0, row.Index, "Incorrect Row.Index");
                Assert.AreEqual("zero", row.DataContext, "Incorrect Row.DataContext");

                row = dataGrid.DisplayData.GetDisplayedRow(1);
                Assert.IsNotNull(row);
                Assert.AreEqual(1, row.Slot, "Incorrect Row.Slot");
                Assert.AreEqual(1, row.Index, "Incorrect Row.Index");
                Assert.AreEqual("one", row.DataContext, "Incorrect Row.DataContext");

                row = dataGrid.DisplayData.GetDisplayedRow(2);
                Assert.IsNotNull(row);
                Assert.AreEqual(2, row.Slot, "Incorrect Row.Slot");
                Assert.AreEqual(2, row.Index, "Incorrect Row.Index");
                Assert.AreEqual("three", row.DataContext, "Incorrect Row.DataContext");

                row = dataGrid.DisplayData.GetDisplayedRow(3);
                Assert.IsNotNull(row);
                Assert.AreEqual(3, row.Slot, "Incorrect Row.Slot");
                Assert.AreEqual(3, row.Index, "Incorrect Row.Index");
                Assert.AreEqual("four", row.DataContext, "Incorrect Row.DataContext");
            });
            this.EnqueueTestComplete();
        }

        #endregion SetItemsSourceAndModifyCollection Test
    }
}
