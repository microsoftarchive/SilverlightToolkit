// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls.Data.Test.DataClasses;
using System.Windows.Controls.Test;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompMod = System.ComponentModel;

namespace System.Windows.Controls.Data.Test
{
    [TestClass]
    public class DataGridRowGroupingTests : DataGridUnitTest<Order>
    {
        private int _rowGroupHeadersLoaded;
        private int _rowGroupHeadersUnloaded;
        private int _rowsLoaded;
        private int _rowsUnloaded;

        /// <summary>
        /// Tests the ColumnCollection when RowGrouping is enabled
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the ColumnCollection when RowGrouping is enabled")]
        public void ColumnCollectionTest()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 400;
            ObservableCollection<Order> orders = CreateOrders();
            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("City"));
            dataGrid.ItemsSource = collectionView;

            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });
            int columnCount = 6;

            Action CheckColumnIndexes = () =>
            {
                int spacerAdjustment = dataGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented ? 1 : 0;

                for (int i = 0; i < dataGrid.Columns.Count; i++)
                {
                    DataGridColumn column = dataGrid.Columns[i];
                    Assert.AreEqual(i, column.DisplayIndex);
                    Assert.AreEqual(i + spacerAdjustment, column.DisplayIndexWithFiller);
                    Assert.AreEqual(i + spacerAdjustment, column.Index);
                }

                Assert.AreEqual(columnCount, dataGrid.Columns.Count);
                Assert.AreEqual(columnCount + spacerAdjustment, dataGrid.ColumnsItemsInternal.Count);
                Assert.AreEqual(0, dataGrid.ColumnsInternal.RowGroupSpacerColumn.Index);
            };

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.IsTrue(dataGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented);
                CheckColumnIndexes();

                DataGridTextColumn newColumn = new DataGridTextColumn();
                newColumn.Binding = new Binding();
                dataGrid.Columns.Insert(0, newColumn);
                columnCount++;
                Assert.AreEqual(newColumn, dataGrid.Columns[0]);

                CheckColumnIndexes();
                Assert.AreEqual(dataGrid.ColumnsInternal.RowGroupSpacerColumn, dataGrid.ColumnsItemsInternal[0]);

                DataGridColumn column = dataGrid.CurrentColumn;
                dataGrid.Columns.Remove(column);
                columnCount--;
                CheckColumnIndexes();

                column.DisplayIndexWithFiller = -1;
                dataGrid.Columns.Add(column);
                columnCount++;
                CheckColumnIndexes();

                // 

            });

            //this.EnqueueYieldThread();
            //EnqueueCallback(delegate
            //{
            //    CheckColumnIndexes();
            //});

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the GroupDescriptions property
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the GroupDescriptions property")]
        public void GroupDescriptions()
        {
            //
            // Test case where group descriptions are set on CollectionView but not on DataGrid
            // DataGrid should reflect CollectionView's group descriptions.
            //
            DataGrid dataGrid = new DataGrid();
            PagedCollectionView collectionView = new PagedCollectionView(CreateOrders());
            Assert.AreNotEqual(dataGrid.GroupDescriptions, collectionView.GroupDescriptions);
            Assert.AreEqual(0, dataGrid.GroupDescriptions.Count, "DataGrid should not have any GroupDescriptions");
            Assert.AreEqual(0, collectionView.GroupDescriptions.Count, "CollectionView should not have any GroupDescriptions");

            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("City"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            dataGrid.ItemsSource = collectionView;

            Assert.AreEqual(dataGrid.GroupDescriptions, collectionView.GroupDescriptions);
            Assert.AreEqual(dataGrid.GroupDescriptions.Count, 2);

            PropertyGroupDescription propertyGroupDescription = collectionView.GroupDescriptions[0] as PropertyGroupDescription;
            Assert.IsNotNull(propertyGroupDescription);
            Assert.AreEqual(propertyGroupDescription.PropertyName, "City", "PagedCollectionView original GroupDescriptions were lost");

            propertyGroupDescription = collectionView.GroupDescriptions[1] as PropertyGroupDescription;
            Assert.IsNotNull(propertyGroupDescription);
            Assert.AreEqual(propertyGroupDescription.PropertyName, "StateProvince", "PagedCollectionView original GroupDescriptions were lost");

            //
            // Test case where group descriptions are set on DataGrid but not on CollectionView.
            // DataGrid's group descriptions should be transferred to the CollectionView.
            //
            dataGrid.ItemsSource = null;
            collectionView.GroupDescriptions.Clear();
            Assert.AreNotEqual(dataGrid.GroupDescriptions, collectionView.GroupDescriptions);
            Assert.AreEqual(0, dataGrid.GroupDescriptions.Count, "DataGrid should not have any GroupDescriptions");
            Assert.AreEqual(0, collectionView.GroupDescriptions.Count, "CollectionView should not have any GroupDescriptions");

            dataGrid.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            dataGrid.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            dataGrid.ItemsSource = collectionView;

            Assert.AreEqual(dataGrid.GroupDescriptions, collectionView.GroupDescriptions);
            Assert.AreEqual(dataGrid.GroupDescriptions.Count, 2);

            propertyGroupDescription = collectionView.GroupDescriptions[0] as PropertyGroupDescription;
            Assert.IsNotNull(propertyGroupDescription);
            Assert.AreEqual(propertyGroupDescription.PropertyName, "StateProvince", "PagedCollectionView did not get DataGrid's GroupDescriptions");

            propertyGroupDescription = collectionView.GroupDescriptions[1] as PropertyGroupDescription;
            Assert.IsNotNull(propertyGroupDescription);
            Assert.AreEqual(propertyGroupDescription.PropertyName, "CountryRegion", "PagedCollectionView did not get DataGrid's GroupDescriptions");

            //
            // Test case where group descriptions are set on both the DataGrid and the CollectionView.
            // The CollectionView should win, and those set on the DataGrid should be lost.
            //
            dataGrid.ItemsSource = null;
            collectionView.GroupDescriptions.Clear();
            Assert.AreNotEqual(dataGrid.GroupDescriptions, collectionView.GroupDescriptions);
            Assert.AreEqual(0, dataGrid.GroupDescriptions.Count, "DataGrid should not have any GroupDescriptions");
            Assert.AreEqual(0, collectionView.GroupDescriptions.Count, "CollectionView should not have any GroupDescriptions");

            dataGrid.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            dataGrid.ItemsSource = collectionView;

            Assert.AreEqual(dataGrid.GroupDescriptions, collectionView.GroupDescriptions);
            Assert.AreEqual(dataGrid.GroupDescriptions.Count, 1);

            propertyGroupDescription = collectionView.GroupDescriptions[0] as PropertyGroupDescription;
            Assert.IsNotNull(propertyGroupDescription);
            Assert.AreEqual(propertyGroupDescription.PropertyName, "CountryRegion", "PagedCollectionView original GroupDescriptions were lost");

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the SortDescriptions property
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the SortDescriptions property")]
        public void SortDescriptions()
        {
            //
            // Test case where sort descriptions are set on CollectionView but not on DataGrid
            // DataGrid should reflect CollectionView's sort descriptions.
            //
            DataGrid dataGrid = new DataGrid();
            PagedCollectionView collectionView = new PagedCollectionView(CreateOrders());
            Assert.AreNotEqual(dataGrid.SortDescriptions, collectionView.SortDescriptions);
            Assert.AreEqual(0, dataGrid.SortDescriptions.Count, "DataGrid should not have any SortDescriptions");
            Assert.AreEqual(0, collectionView.SortDescriptions.Count, "CollectionView should not have any SortDescriptions");

            collectionView.SortDescriptions.Add(new CompMod.SortDescription("City", CompMod.ListSortDirection.Ascending));
            collectionView.SortDescriptions.Add(new CompMod.SortDescription("StateProvince", CompMod.ListSortDirection.Ascending));
            dataGrid.ItemsSource = collectionView;

            Assert.AreEqual(dataGrid.SortDescriptions, collectionView.SortDescriptions);
            Assert.AreEqual(dataGrid.SortDescriptions.Count, 2);

            CompMod.SortDescription sortDescription = collectionView.SortDescriptions[0];
            Assert.IsNotNull(sortDescription);
            Assert.AreEqual(sortDescription.PropertyName, "City", "PagedCollectionView original SortDescriptions were lost");

            sortDescription = collectionView.SortDescriptions[1];
            Assert.IsNotNull(sortDescription);
            Assert.AreEqual(sortDescription.PropertyName, "StateProvince", "PagedCollectionView original SortDescriptions were lost");

            //
            // Test case where sort descriptions are set on DataGrid but not on CollectionView.
            // DataGrid's sort descriptions should be transferred to the CollectionView.
            //
            dataGrid.ItemsSource = null;
            collectionView.SortDescriptions.Clear();
            Assert.AreNotEqual(dataGrid.SortDescriptions, collectionView.SortDescriptions);
            Assert.AreEqual(0, dataGrid.SortDescriptions.Count, "DataGrid should not have any SortDescriptions");
            Assert.AreEqual(0, collectionView.SortDescriptions.Count, "CollectionView should not have any SortDescriptions");

            dataGrid.SortDescriptions.Add(new CompMod.SortDescription("StateProvince", CompMod.ListSortDirection.Ascending));
            dataGrid.SortDescriptions.Add(new CompMod.SortDescription("CountryRegion", CompMod.ListSortDirection.Ascending));
            dataGrid.ItemsSource = collectionView;

            Assert.AreEqual(dataGrid.SortDescriptions, collectionView.SortDescriptions);
            Assert.AreEqual(dataGrid.SortDescriptions.Count, 2);

            sortDescription = collectionView.SortDescriptions[0];
            Assert.IsNotNull(sortDescription);
            Assert.AreEqual(sortDescription.PropertyName, "StateProvince", "PagedCollectionView did not get DataGrid's SortDescriptions");

            sortDescription = collectionView.SortDescriptions[1];
            Assert.IsNotNull(sortDescription);
            Assert.AreEqual(sortDescription.PropertyName, "CountryRegion", "PagedCollectionView did not get DataGrid's SortDescriptions");

            //
            // Test case where sort descriptions are set on both the DataGrid and the CollectionView.
            // The CollectionView should win, and those set on the DataGrid should be lost.
            //
            dataGrid.ItemsSource = null;
            collectionView.SortDescriptions.Clear();
            Assert.AreNotEqual(dataGrid.SortDescriptions, collectionView.SortDescriptions);
            Assert.AreEqual(0, dataGrid.SortDescriptions.Count, "DataGrid should not have any SortDescriptions");
            Assert.AreEqual(0, collectionView.SortDescriptions.Count, "CollectionView should not have any SortDescriptions");

            dataGrid.SortDescriptions.Add(new CompMod.SortDescription("StateProvince", CompMod.ListSortDirection.Ascending));
            collectionView.SortDescriptions.Add(new CompMod.SortDescription("CountryRegion", CompMod.ListSortDirection.Ascending));
            dataGrid.ItemsSource = collectionView;

            Assert.AreEqual(dataGrid.SortDescriptions, collectionView.SortDescriptions);
            Assert.AreEqual(dataGrid.SortDescriptions.Count, 1);

            sortDescription = collectionView.SortDescriptions[0];
            Assert.IsNotNull(sortDescription);
            Assert.AreEqual(sortDescription.PropertyName, "CountryRegion", "PagedCollectionView original SortDescriptions were lost");

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the GroupDescriptions property
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the GroupDescriptions property")]
        public void FlatDisplay()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 500;
            ObservableCollection<Order> orders = CreateOrders();
            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            _rowGroupHeadersLoaded = 0;
            dataGrid.LoadingRowGroup += DataGrid_LoadingRowGroup;
            dataGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_LoadingRow);
            _rowsLoaded = 0;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });
            this.EnqueueYieldThread();

            int groupCount = 0;
            EnqueueCallback(delegate
            {
                groupCount = CountCollectionViewGroups(collectionView.Groups);
                Assert.AreEqual(groupCount, collectionView.Groups.Count, "Unexpected number of Groups from PagedCollectionView for Country/Region grouping");
                Assert.AreEqual(_rowGroupHeadersLoaded, groupCount, "Wrong number of Loaded RowGroupHeaders for Country/Region grouping");
                Assert.AreEqual(_rowsLoaded, collectionView.ItemCount, "Wrong number of items loaded for Country/Region grouping");
                Assert.AreEqual(dataGrid.SlotCount, orders.Count + collectionView.Groups.Count);
                Assert.AreEqual(groupCount, dataGrid.RowGroupHeadersTable.IndexCount);
                _rowGroupHeadersLoaded = 0;
                _rowsLoaded = 0;
                collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                groupCount = CountCollectionViewGroups(collectionView.Groups);
                Assert.IsTrue(collectionView.Groups.Count < _rowGroupHeadersLoaded, "New RowGroupHeaders were not added");
                Assert.AreEqual(7, _rowGroupHeadersLoaded, "Wrong number of Loaded RowGroupHeaders for CountryRegion,StateProvince grouping");
                Assert.AreEqual(18, collectionView.ItemCount, "Wrong number of items loaded for CountryRegion,StateProvince grouping");
                Assert.AreEqual(groupCount, dataGrid.RowGroupHeadersTable.IndexCount);
                Assert.Equals(dataGrid.SlotCount, orders.Count + groupCount);
                _rowGroupHeadersLoaded = 0;
                _rowsLoaded = 0;
                collectionView.GroupDescriptions.RemoveAt(1);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                int newGroupCount = CountCollectionViewGroups(collectionView.Groups);
                Assert.IsTrue(newGroupCount < groupCount, "CollectionViewGroups were not removed");
                Assert.AreEqual(newGroupCount, _rowGroupHeadersLoaded, "Wrong number of Loaded RowGroupHeaders after removing state/province grouping");
                Assert.AreEqual(_rowsLoaded, collectionView.ItemCount, "Wrong number of items loaded after removing state/province grouping");
                Assert.AreEqual(newGroupCount, dataGrid.RowGroupHeadersTable.IndexCount);
                Assert.Equals(dataGrid.SlotCount, orders.Count + newGroupCount);

                _rowGroupHeadersLoaded = 0;

                // Set the Itemsource to something different without any groups
                dataGrid.ItemsSource = new List<int>() { 1, 2, 3, 4 };
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(0, _rowGroupHeadersLoaded);
                Assert.AreEqual(false, dataGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented);
                Assert.AreEqual(dataGrid.ColumnsItemsInternal.Count, dataGrid.Columns.Count);
                Assert.AreEqual(dataGrid.ColumnsInternal.DisplayIndexMap.Count, dataGrid.Columns.Count);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the expanding and collapsing of RowGroups in a DataGrid with a single GroupDescription
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the expanding and collapsing of RowGroups in a DataGrid with a single GroupDescription")]
        public void ExpandCollapseSingleLevel()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 350;
            ObservableCollection<Order> orders = CreateOrders();
            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            dataGrid.LoadingRowGroup += DataGrid_LoadingRowGroup;
            dataGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_LoadingRow);
            dataGrid.UnloadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_UnloadingRow);
            dataGrid.UnloadingRowGroup += new EventHandler<DataGridRowGroupHeaderEventArgs>(DataGrid_UnloadingRowGroup);
            EnqueueConditional(delegate { return isLoaded; });
            this.EnqueueYieldThread();

            double verticalMax = 0;
            EnqueueCallback(delegate
            {
                Assert.AreEqual(21, dataGrid.SlotCount);
                Assert.AreEqual(21, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(13, dataGrid.DisplayData.LastScrollingSlot);
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible);

                verticalMax = dataGrid.VerticalScrollBar.Maximum;
                _rowGroupHeadersUnloaded = 0;
                _rowsUnloaded = 0;

                // Collapse Canada
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Victoria", orders), 0), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, _rowsUnloaded, "Canada was not collapsed properly");
                Assert.AreEqual(0, _rowGroupHeadersUnloaded, "Canada was not collapsed properly");
                Assert.AreEqual(21, dataGrid.SlotCount, "Canada was not collapsed properly");
                Assert.AreEqual(18, dataGrid.VisibleSlotCount, "Canada was not collapsed properly");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "Canada was not collapsed properly");
                Assert.AreEqual(16, dataGrid.DisplayData.LastScrollingSlot, "Canada was not collapsed properly");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "Canada was not collapsed properly");
                Assert.IsTrue(verticalMax > dataGrid.VerticalScrollBar.Maximum, "Canada was not collapsed properly");

                _rowsUnloaded = 0;
                _rowGroupHeadersUnloaded = 0;

                // Collapse United States
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Seattle", orders), 0), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(11, _rowsUnloaded, "United States was not collapsed properly");
                Assert.AreEqual(0, _rowGroupHeadersUnloaded, "United States was not collapsed properly");
                Assert.AreEqual(21, dataGrid.SlotCount, "United States was not collapsed properly");
                Assert.AreEqual(7, dataGrid.VisibleSlotCount, "United States was not collapsed properly");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "United States was not collapsed properly");
                Assert.AreEqual(20, dataGrid.DisplayData.LastScrollingSlot, "United States was not collapsed properly");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Collapsed, "United States was not collapsed properly");

                _rowsUnloaded = 0;
                _rowGroupHeadersUnloaded = 0;

                // Collapse China
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Shanghai", orders), 0), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(4, _rowsUnloaded, "China was not collapsed properly");
                Assert.AreEqual(0, _rowGroupHeadersUnloaded, "China was not collapsed properly");
                Assert.AreEqual(21, dataGrid.SlotCount, "China was not collapsed properly");
                Assert.AreEqual(3, dataGrid.VisibleSlotCount, "China was not collapsed properly");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "China was not collapsed properly");
                Assert.AreEqual(16, dataGrid.DisplayData.LastScrollingSlot, "China was not collapsed properly");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Collapsed, "China was not collapsed properly");

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;
                _rowGroupHeadersUnloaded = 0;

                // Expand United States
                dataGrid.ExpandRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Katy", orders), 0), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(11, _rowsLoaded, "United States was not expanded properly");
                Assert.AreEqual(0, _rowGroupHeadersLoaded - _rowGroupHeadersUnloaded, "United States was not expanded properly");
                Assert.AreEqual(21, dataGrid.SlotCount, "United States was not expanded properly");
                Assert.AreEqual(14, dataGrid.VisibleSlotCount, "United States was not expanded properly");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "United States was not expanded properly");
                Assert.AreEqual(16, dataGrid.DisplayData.LastScrollingSlot, "United States was not expanded properly");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Collapsed, "United States was not expanded properly");
                Assert.IsTrue(verticalMax > dataGrid.VerticalScrollBar.Maximum, "United States was not expanded properly");

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;

                // Expand Canada
                dataGrid.ExpandRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Vancouver", orders), 0), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, _rowsLoaded, "Canada was not expanded properly");
                Assert.AreEqual(0, _rowGroupHeadersLoaded, "Canada was not expanded properly");
                Assert.AreEqual(21, dataGrid.SlotCount, "Canada was not expanded properly");
                Assert.AreEqual(17, dataGrid.VisibleSlotCount, "Canada was not expanded properly");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "Canada was not expanded properly");
                Assert.AreEqual(13, dataGrid.DisplayData.LastScrollingSlot, "Canada was not expanded properly");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "Canada was not expanded properly");
                Assert.IsTrue(verticalMax > dataGrid.VerticalScrollBar.Maximum, "Canada was not expanded properly");

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;

                // Expand China
                dataGrid.ExpandRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Fuzhou", orders), 0), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(0, _rowsLoaded, "China was not expanded properly");
                Assert.AreEqual(0, _rowGroupHeadersLoaded, "China was not expanded properly");
                Assert.AreEqual(21, dataGrid.SlotCount, "China was not expanded properly");
                Assert.AreEqual(21, dataGrid.VisibleSlotCount, "China was not expanded properly");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "China was not expanded properly");
                Assert.AreEqual(13, dataGrid.DisplayData.LastScrollingSlot, "China was not expanded properly");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "China was not expanded properly");
            });
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the expanding and collapsing of RowGroups in a DataGrid with multiple GroupDescriptions
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the expanding and collapsing of RowGroups in a DataGrid with multiple GroupDescriptions")]
        public void ExpandCollapseMultiLevel()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 350;
            ObservableCollection<Order> orders = CreateOrders();
            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("City"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            dataGrid.LoadingRowGroup += DataGrid_LoadingRowGroup;
            dataGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_LoadingRow);
            dataGrid.UnloadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_UnloadingRow);
            dataGrid.UnloadingRowGroup += new EventHandler<DataGridRowGroupHeaderEventArgs>(DataGrid_UnloadingRowGroup);
            EnqueueConditional(delegate { return isLoaded; });
            this.EnqueueYieldThread();

            double verticalMax = 0;
            EnqueueCallback(delegate
            {
                Assert.AreEqual(46, dataGrid.SlotCount);
                Assert.AreEqual(46, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(14, dataGrid.DisplayData.LastScrollingSlot);

                verticalMax = dataGrid.VerticalScrollBar.Maximum;
                _rowGroupHeadersUnloaded = 0;
                _rowsUnloaded = 0;

                // Collapse UnitedStates and all subgroups
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Austin", orders), 0), true);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(7, _rowsUnloaded, "United States was not collapsed properly");
                Assert.AreEqual(7, _rowGroupHeadersUnloaded, "United States was not collapsed properly");
                Assert.AreEqual(46, dataGrid.SlotCount, "United States was not collapsed properly");
                Assert.AreEqual(22, dataGrid.VisibleSlotCount, "United States was not collapsed properly");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "United States was not collapsed properly");
                Assert.AreEqual(39, dataGrid.DisplayData.LastScrollingSlot, "United States was not collapsed properly");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "United States was not collapsed properly");
                Assert.IsTrue(verticalMax > dataGrid.VerticalScrollBar.Maximum, "United States was not collapsed properly");

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;

                // Expand United States
                dataGrid.ExpandRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Houston", orders), 0), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(0, _rowsLoaded, "United States was not collapsed properly");
                Assert.AreEqual(4, _rowGroupHeadersLoaded, "United States was not collapsed properly");
                Assert.AreEqual(46, dataGrid.SlotCount, "United States was not collapsed properly");
                Assert.AreEqual(26, dataGrid.VisibleSlotCount, "United States was not collapsed properly");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "United States was not collapsed properly");
                Assert.AreEqual(35, dataGrid.DisplayData.LastScrollingSlot, "United States was not collapsed properly");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "United States was not collapsed properly");
                Assert.IsTrue(verticalMax > dataGrid.VerticalScrollBar.Maximum, "United States was not collapsed properly");

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;

                // Expand Cambridge (should no-opt visually since its parent is collapsed)
                dataGrid.ExpandRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Cambridge", orders), 2), true);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(0, _rowsLoaded, "United States was not collapsed properly");
                Assert.AreEqual(0, _rowGroupHeadersLoaded, "United States was not collapsed properly");
                Assert.AreEqual(46, dataGrid.SlotCount, "United States was not collapsed properly");
                Assert.AreEqual(26, dataGrid.VisibleSlotCount, "United States was not collapsed properly");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "United States was not collapsed properly");
                Assert.AreEqual(35, dataGrid.DisplayData.LastScrollingSlot, "United States was not collapsed properly");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "United States was not collapsed properly");
                Assert.IsTrue(verticalMax > dataGrid.VerticalScrollBar.Maximum, "United States was not collapsed properly");

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;

                // Expand Massachusetts (both Mass and Cambridge should be expanded)
                dataGrid.ExpandRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Cambridge", orders), 1), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, _rowsLoaded, "United States was not collapsed properly");
                Assert.AreEqual(1, _rowGroupHeadersLoaded, "United States was not collapsed properly");
                Assert.AreEqual(46, dataGrid.SlotCount, "United States was not collapsed properly");
                Assert.AreEqual(28, dataGrid.VisibleSlotCount, "United States was not collapsed properly");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "United States was not collapsed properly");
                Assert.AreEqual(33, dataGrid.DisplayData.LastScrollingSlot, "United States was not collapsed properly");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "United States was not collapsed properly");
                Assert.IsTrue(verticalMax > dataGrid.VerticalScrollBar.Maximum, "United States was not collapsed properly");

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the expanding on a small DataGrid to make sure we don't create too many rows
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the expanding on a small DataGrid to make sure we don't create too many rows")]
        public void ExpandSmallDataGrid()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 100;
            ObservableCollection<Order> orders = CreateOrders();
            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            dataGrid.LoadingRowGroup += DataGrid_LoadingRowGroup;
            dataGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_LoadingRow);
            dataGrid.UnloadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_UnloadingRow);
            dataGrid.UnloadingRowGroup += new EventHandler<DataGridRowGroupHeaderEventArgs>(DataGrid_UnloadingRowGroup);
            EnqueueConditional(delegate { return isLoaded; });
            this.EnqueueYieldThread();

            double verticalMax = 0;
            EnqueueCallback(delegate
            {
                Assert.AreEqual(21, dataGrid.SlotCount);
                Assert.AreEqual(21, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(3, dataGrid.DisplayData.LastScrollingSlot);

                verticalMax = dataGrid.VerticalScrollBar.Maximum;
                _rowGroupHeadersUnloaded = 0;
                _rowsUnloaded = 0;

                // Collapse UnitedStates
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Austin", orders), 0), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(3, _rowsUnloaded, "United States was not collapsed properly");
                Assert.AreEqual(0, _rowGroupHeadersUnloaded, "United States was not collapsed properly");
                Assert.AreEqual(21, dataGrid.SlotCount, "United States was not collapsed properly");
                Assert.AreEqual(10, dataGrid.VisibleSlotCount, "United States was not collapsed properly");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "United States was not collapsed properly");
                Assert.AreEqual(14, dataGrid.DisplayData.LastScrollingSlot, "United States was not collapsed properly");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "United States was not collapsed properly");
                Assert.IsTrue(verticalMax > dataGrid.VerticalScrollBar.Maximum, "United States was not collapsed properly");

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;

                // Expand United States
                dataGrid.ExpandRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Houston", orders), 0), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(3, _rowsLoaded);
                Assert.AreEqual(0, _rowGroupHeadersLoaded);
                Assert.AreEqual(21, dataGrid.SlotCount);
                Assert.AreEqual(21, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(3, dataGrid.DisplayData.LastScrollingSlot);
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible);
            });
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests adding and removing rows when row are grouped
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests adding and removing rows when row are grouped")]
        public void AddRemove()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 500;
            ObservableCollection<Order> orders = CreateOrders();
            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("City"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            dataGrid.LoadingRowGroup += DataGrid_LoadingRowGroup;
            dataGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_LoadingRow);
            dataGrid.UnloadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_UnloadingRow);
            dataGrid.UnloadingRowGroup += new EventHandler<DataGridRowGroupHeaderEventArgs>(DataGrid_UnloadingRowGroup);
            EnqueueConditional(delegate { return isLoaded; });
            this.EnqueueYieldThread();

            DataGridRowGroupInfo usGroupInfo = null;
            DataGridRowGroupInfo canadaGroupInfo = null;
            DataGridRowGroupInfo chinaGroupInfo = null;
            double verticalMax = 0;
            EnqueueCallback(delegate
            {
                Assert.AreEqual(3, collectionView.Groups.Count, "Incorrect number of ICollectionView.Groups");
                Assert.AreEqual(46, dataGrid.SlotCount, "SlotCount incorrect after removal");
                Assert.AreEqual(46, dataGrid.VisibleSlotCount, "VisibleSlotCount incorrect after removal");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "FirstScrollingSlot incorrect after removal");
                Assert.AreEqual(21, dataGrid.DisplayData.LastScrollingSlot, "LastScrollingSlot incorrect after removal");

                usGroupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(collectionView.Groups[0] as CollectionViewGroup);
                Assert.IsNotNull(usGroupInfo);
                Assert.AreEqual(24, usGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removal");
                Assert.AreEqual(0, usGroupInfo.Slot, "Slot incorrect after removal");

                canadaGroupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(collectionView.Groups[1] as CollectionViewGroup);
                Assert.IsNotNull(canadaGroupInfo);
                Assert.AreEqual(32, canadaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removal");
                Assert.AreEqual(25, canadaGroupInfo.Slot, "Slot incorrect after removal");

                chinaGroupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(collectionView.Groups[2] as CollectionViewGroup);
                Assert.IsNotNull(chinaGroupInfo);
                Assert.AreEqual(45, chinaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removal");
                Assert.AreEqual(33, chinaGroupInfo.Slot, "Slot incorrect after removal");

                // Adding a new item should add a new row which is not a part of any group
                collectionView.AddNew();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(4, collectionView.Groups.Count, "New item did not get added to ICollectionView.Groups");
                Assert.AreEqual(47, dataGrid.SlotCount, "SlotCount incorrect after adding new item");
                Assert.AreEqual(47, dataGrid.VisibleSlotCount, "VisibleSlotCount incorrect after adding new item");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "FirstScrollingSlot incorrect after adding new item");
                Assert.AreEqual(21, dataGrid.DisplayData.LastScrollingSlot, "LastScrollingSlot incorrect after adding new item");

                Assert.IsNotNull(usGroupInfo);
                Assert.AreEqual(24, usGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after adding new item");
                Assert.AreEqual(0, usGroupInfo.Slot, "Slot incorrect after adding new item");

                Assert.IsNotNull(canadaGroupInfo);
                Assert.AreEqual(32, canadaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after adding new item");
                Assert.AreEqual(25, canadaGroupInfo.Slot, "Slot incorrect after adding new item");

                Assert.IsNotNull(chinaGroupInfo);
                Assert.AreEqual(45, chinaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after adding new item");
                Assert.AreEqual(33, chinaGroupInfo.Slot, "Slot incorrect after adding new item");

                // Cancelling new should remove the new item row and still not affect grouping
                collectionView.CancelNew();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(3, collectionView.Groups.Count, "New item did not get removed from ICollectionView.Groups");
                Assert.AreEqual(46, dataGrid.SlotCount, "SlotCount incorrect after removing new item");
                Assert.AreEqual(46, dataGrid.VisibleSlotCount, "VisibleSlotCount incorrect after removing new item");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "FirstScrollingSlot incorrect after removing new item");
                Assert.AreEqual(21, dataGrid.DisplayData.LastScrollingSlot, "LastScrollingSlot incorrect after removing new item");

                Assert.IsNotNull(usGroupInfo);
                Assert.AreEqual(24, usGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removing new item");
                Assert.AreEqual(0, usGroupInfo.Slot, "Slot incorrect after removing new item");

                Assert.IsNotNull(canadaGroupInfo);
                Assert.AreEqual(32, canadaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removing new item");
                Assert.AreEqual(25, canadaGroupInfo.Slot, "Slot incorrect after removing new item");

                Assert.IsNotNull(chinaGroupInfo);
                Assert.AreEqual(45, chinaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removing new item");
                Assert.AreEqual(33, chinaGroupInfo.Slot, "Slot incorrect after removing new item");

                verticalMax = dataGrid.VerticalScrollBar.Maximum;
                _rowGroupHeadersUnloaded = 0;
                _rowsUnloaded = 0;

                // Delete San Mateo, Redwood City, and San Francisco (should remove California as well)
                orders.Remove(OrderFromCity("San Mateo", orders));
                orders.Remove(OrderFromCity("Redwood City", orders));
                orders.Remove(OrderFromCity("San Francisco", orders));
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(3, _rowsUnloaded, "Incorrect number of orders removed");
                Assert.AreEqual(4, _rowGroupHeadersUnloaded, "Incorrect number of RowGroupHeaders removed");
                Assert.AreEqual(39, dataGrid.SlotCount, "SlotCount incorrect after removal");
                Assert.AreEqual(39, dataGrid.VisibleSlotCount, "VisibleSlotCount incorrect after removal");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "FirstScrollingSlot incorrect after removal");
                Assert.AreEqual(21, dataGrid.DisplayData.LastScrollingSlot, "LastScrollingSlot incorrect after removal");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "Vertical scrollbar incorrect after removal");
                Assert.IsTrue(verticalMax > dataGrid.VerticalScrollBar.Maximum, "Vertical scrollbar incorrect after removal");

                Assert.IsNotNull(usGroupInfo);
                Assert.AreEqual(17, usGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removal");
                Assert.AreEqual(0, usGroupInfo.Slot, "Slot incorrect after removal");

                Assert.IsNotNull(canadaGroupInfo);
                Assert.AreEqual(25, canadaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removal");
                Assert.AreEqual(18, canadaGroupInfo.Slot, "Slot incorrect after removal");

                Assert.IsNotNull(chinaGroupInfo);
                Assert.AreEqual(38, chinaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removal");
                Assert.AreEqual(26, chinaGroupInfo.Slot, "Slot incorrect after removal");

                _rowGroupHeadersUnloaded = 0;
                _rowsUnloaded = 0;

                // Collapse Washington
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Bellevue", orders), 1), true);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(4, _rowsUnloaded, "Incorrect number of orders removed");
                Assert.AreEqual(2, _rowGroupHeadersUnloaded, "Incorrect number of RowGroupHeaders removed");
                Assert.AreEqual(39, dataGrid.SlotCount, "SlotCount incorrect after removal");
                Assert.AreEqual(33, dataGrid.VisibleSlotCount, "VisibleSlotCount incorrect after removal");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "FirstScrollingSlot incorrect after removal");
                Assert.AreEqual(28, dataGrid.DisplayData.LastScrollingSlot, "LastScrollingSlot incorrect after removal");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "Vertical scrollbar incorrect after removal");
                Assert.IsTrue(verticalMax > dataGrid.VerticalScrollBar.Maximum, "Vertical scrollbar incorrect after removal");

                Assert.IsNotNull(usGroupInfo);
                Assert.AreEqual(17, usGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removal");
                Assert.AreEqual(0, usGroupInfo.Slot, "Slot incorrect after removal");

                Assert.IsNotNull(canadaGroupInfo);
                Assert.AreEqual(25, canadaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removal");
                Assert.AreEqual(18, canadaGroupInfo.Slot, "Slot incorrect after removal");

                Assert.IsNotNull(chinaGroupInfo);
                Assert.AreEqual(38, chinaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after removal");
                Assert.AreEqual(26, chinaGroupInfo.Slot, "Slot incorrect after removal");

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;
                verticalMax = dataGrid.VerticalScrollBar.Maximum;

                // Add San Jose (should add a new California back)
                orders.Add(new Order(2001, "United States", "California", "San Jose", "(650)313-2813"));
            });

            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, _rowsLoaded, "Incorrect number of orders added");
                Assert.AreEqual(2, _rowGroupHeadersLoaded, "Incorrect number of RowGroupHeaders added");
                Assert.AreEqual(42, dataGrid.SlotCount, "SlotCount incorrect after addition");
                Assert.AreEqual(36, dataGrid.VisibleSlotCount, "VisibleSlotCount incorrect after addition");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "FirstScrollingSlot incorrect after addition");
                Assert.AreEqual(28, dataGrid.DisplayData.LastScrollingSlot, "LastScrollingSlot incorrect after addition");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "Vertical scrollbar incorrect after addition");
                Assert.IsTrue(verticalMax < dataGrid.VerticalScrollBar.Maximum, "Vertical scrollbar incorrect after addition");

                Assert.IsNotNull(usGroupInfo);
                Assert.AreEqual(20, usGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after addition");
                Assert.AreEqual(0, usGroupInfo.Slot, "Slot incorrect after addition");

                Assert.IsNotNull(canadaGroupInfo);
                Assert.AreEqual(28, canadaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after addition");
                Assert.AreEqual(21, canadaGroupInfo.Slot, "Slot incorrect after addition");

                Assert.IsNotNull(chinaGroupInfo);
                Assert.AreEqual(41, chinaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after addition");
                Assert.AreEqual(29, chinaGroupInfo.Slot, "Slot incorrect after addition");

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;

                // Add Mountain View
                orders.Add(new Order(2002, "United States", "California", "Mountain View", "(650)821-2813"));
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, _rowsLoaded, "Incorrect number of orders added");
                Assert.AreEqual(1, _rowGroupHeadersLoaded, "Incorrect number of RowGroupHeaders added");
                Assert.AreEqual(44, dataGrid.SlotCount, "SlotCount incorrect after addition");
                Assert.AreEqual(38, dataGrid.VisibleSlotCount, "VisibleSlotCount incorrect after addition");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "FirstScrollingSlot incorrect after addition");
                Assert.AreEqual(28, dataGrid.DisplayData.LastScrollingSlot, "LastScrollingSlot incorrect after addition");
                Assert.IsTrue(dataGrid.VerticalScrollBar.Visibility == Visibility.Visible, "Vertical scrollbar incorrect after addition");

                Assert.IsNotNull(usGroupInfo);
                Assert.AreEqual(22, usGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after addition");
                Assert.AreEqual(0, usGroupInfo.Slot, "Slot incorrect after addition");

                Assert.IsNotNull(canadaGroupInfo);
                Assert.AreEqual(30, canadaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after addition");
                Assert.AreEqual(23, canadaGroupInfo.Slot, "Slot incorrect after addition");

                Assert.IsNotNull(chinaGroupInfo);
                Assert.AreEqual(43, chinaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after addition");
                Assert.AreEqual(31, chinaGroupInfo.Slot, "Slot incorrect after addition");

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;

                // Add another Mountain View
                orders.Add(new Order(2003, "United States", "California", "Mountain View", "(650)120-2219"));
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, _rowsLoaded, "Incorrect number of orders added");
                Assert.AreEqual(0, _rowGroupHeadersLoaded, "Incorrect number of RowGroupHeaders added");
                Assert.AreEqual(45, dataGrid.SlotCount, "SlotCount incorrect after addition");
                Assert.AreEqual(39, dataGrid.VisibleSlotCount, "VisibleSlotCount incorrect after addition");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "FirstScrollingSlot incorrect after addition");
                Assert.AreEqual(28, dataGrid.DisplayData.LastScrollingSlot, "LastScrollingSlot incorrect after addition");

                Assert.IsNotNull(usGroupInfo);
                Assert.AreEqual(23, usGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after addition");
                Assert.AreEqual(0, usGroupInfo.Slot, "Slot incorrect after addition");

                Assert.IsNotNull(canadaGroupInfo);
                Assert.AreEqual(31, canadaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after addition");
                Assert.AreEqual(24, canadaGroupInfo.Slot, "Slot incorrect after addition");

                Assert.IsNotNull(chinaGroupInfo);
                Assert.AreEqual(44, chinaGroupInfo.LastSubItemSlot, "LastSubItemSlot incorrect after addition");
                Assert.AreEqual(32, chinaGroupInfo.Slot, "Slot incorrect after addition");

                dataGrid.SortDescriptions.Add(new CompMod.SortDescription("ID", CompMod.ListSortDirection.Descending));
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(45, dataGrid.SlotCount);
                orders.RemoveAt(orders.Count - 1);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(44, dataGrid.SlotCount);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the RowGroupHeaderStyles property
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the RowGroupHeaderStyles property")]
        public void RowGroupHeaderStyles()
        {
            DataGrid dataGrid = new DataGrid();

            Style style0 = new Style(typeof(DataGridRowGroupHeader));
            style0.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.Red)));
            dataGrid.RowGroupHeaderStyles.Add(style0);

            Style style1 = new Style(typeof(DataGridRowGroupHeader));
            double newSubLevelIndent = 179;
            style1.Setters.Add(new Setter(DataGridRowGroupHeader.SublevelIndentProperty, newSubLevelIndent));
            style1.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Green)));
            dataGrid.RowGroupHeaderStyles.Add(style1);

            dataGrid.Height = 350;
            PagedCollectionView collectionView = new PagedCollectionView(CreateOrders());
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("City"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                DataGridRowGroupHeader groupHeader = dataGrid.DisplayData.GetDisplayedElement(0) as DataGridRowGroupHeader;
                Assert.IsNotNull(groupHeader);
                SolidColorBrush brush = groupHeader.Background as SolidColorBrush;
                Assert.IsNotNull(brush);
                Assert.AreEqual(brush.Color, Colors.Red);

                groupHeader = dataGrid.DisplayData.GetDisplayedElement(1) as DataGridRowGroupHeader;
                Assert.IsNotNull(groupHeader);
                brush = groupHeader.Foreground as SolidColorBrush;
                Assert.IsNotNull(brush);
                Assert.AreEqual(brush.Color, Colors.Green);
                Assert.AreEqual(newSubLevelIndent, groupHeader.SublevelIndent);

                groupHeader = dataGrid.DisplayData.GetDisplayedElement(2) as DataGridRowGroupHeader;
                Assert.IsNotNull(groupHeader);
                brush = groupHeader.Foreground as SolidColorBrush;
                Assert.IsNotNull(brush);
                Assert.AreEqual(brush.Color, Colors.Green);
                Assert.AreEqual(newSubLevelIndent, groupHeader.SublevelIndent);

                Assert.IsTrue(dataGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented);
                Assert.AreEqual((2 * newSubLevelIndent) + DataGrid.DATAGRID_defaultRowGroupSublevelIndent, dataGrid.ColumnsInternal.RowGroupSpacerColumn.Width.Value);
            });
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the expanding and collapsing of RowGroups using the keyboard
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests the expanding and collapsing of RowGroups using the keyboard")]
        public void Keyboard()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            ObservableCollection<Order> orders = CreateOrders();
            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("City"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            dataGrid.LoadingRowGroup += DataGrid_LoadingRowGroup;
            dataGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_LoadingRow);
            dataGrid.UnloadingRow += new EventHandler<DataGridRowEventArgs>(DataGrid_UnloadingRow);
            dataGrid.UnloadingRowGroup += new EventHandler<DataGridRowGroupHeaderEventArgs>(DataGrid_UnloadingRowGroup);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(46, dataGrid.SlotCount);
                Assert.AreEqual(46, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(14, dataGrid.DisplayData.LastScrollingSlot);

                dataGrid.ProcessUpKey();

                _rowsUnloaded = 0;
                _rowGroupHeadersUnloaded = 0;

                // Collapses Seattle
                dataGrid.ProcessLeftKey();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(3, _rowsUnloaded);
                Assert.AreEqual(0, _rowGroupHeadersUnloaded);
                Assert.AreEqual(46, dataGrid.SlotCount);
                Assert.AreEqual(43, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(18, dataGrid.DisplayData.LastScrollingSlot);
                Assert.IsNull(dataGrid.SelectedItem);
                Assert.AreEqual(-1, dataGrid.SelectedIndex);
                Assert.IsNull(collectionView.CurrentItem);

                dataGrid.ProcessUpKey();

                _rowsUnloaded = 0;
                _rowGroupHeadersUnloaded = 0;

                // Collapses Washington
                dataGrid.ProcessLeftKey();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, _rowsUnloaded);
                Assert.AreEqual(2, _rowGroupHeadersUnloaded);
                Assert.AreEqual(46, dataGrid.SlotCount);
                Assert.AreEqual(40, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(21, dataGrid.DisplayData.LastScrollingSlot);

                _rowsLoaded = 0;
                _rowGroupHeadersLoaded = 0;

                // Expands Washington
                dataGrid.ProcessRightKey();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, _rowsLoaded);
                Assert.AreEqual(2, _rowGroupHeadersLoaded);
                Assert.AreEqual(46, dataGrid.SlotCount);
                Assert.AreEqual(43, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(18, dataGrid.DisplayData.LastScrollingSlot);

                dataGrid.ProcessPriorKey();

                _rowsUnloaded = 0;
                _rowGroupHeadersUnloaded = 0;

                // Collapses United States
                dataGrid.ProcessLeftKey();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(5, _rowsUnloaded);
                Assert.AreEqual(10, _rowGroupHeadersUnloaded);
                Assert.AreEqual(46, dataGrid.SlotCount);
                Assert.AreEqual(22, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(39, dataGrid.DisplayData.LastScrollingSlot);
                Assert.IsNull(dataGrid.SelectedItem);
                Assert.AreEqual(-1, dataGrid.SelectedIndex);
                Assert.IsNull(collectionView.CurrentItem);

                dataGrid.ProcessNextKey();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.IsNotNull(dataGrid.SelectedItem);
                Assert.AreEqual(OrderFromCity("Fuzhou", orders), dataGrid.SelectedItem);
                            
                dataGrid.ProcessPriorKey();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                // A GroupHeader is current so the CollectionView's current position is -1
                Assert.AreEqual(-1, dataGrid.DataConnection.CollectionView.CurrentPosition);

                dataGrid.DataConnection.CollectionView.GroupDescriptions.Clear();                
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(0, dataGrid.RowGroupHeadersTable.IndexCount);
                Assert.AreEqual(false, dataGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests removing an item from a 1 item group, collapsing the parent group, and expanding it again
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests removing an item from a 1 item group, collapsing the parent group, and expanding it again")]
        public void RemoveSingleGroupItemCollapseExpandParent()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            ObservableCollection<Order> orders = CreateOrders();
            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("City"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            CollectionViewGroup waGroup = null;
            DataGridRowGroupInfo rowGroupInfo = null;
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(46, dataGrid.SlotCount);
                Assert.AreEqual(46, dataGrid.VisibleSlotCount);
                Assert.AreEqual(28, CountCollectionViewGroups(collectionView.Groups));

                waGroup = dataGrid.GetGroupFromItem(OrderFromCity("Seattle", orders), 1);
                Assert.IsNotNull(waGroup);
                rowGroupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(waGroup);
                Assert.IsNotNull(rowGroupInfo);
                Assert.AreEqual(7, rowGroupInfo.LastSubItemSlot);
                
                Order order = OrderFromCity("Bellevue", orders);
                orders.Remove(order);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(44, dataGrid.SlotCount);
                Assert.AreEqual(44, dataGrid.VisibleSlotCount);
                Assert.AreEqual(27, CountCollectionViewGroups(collectionView.Groups));
                Assert.AreEqual(5, rowGroupInfo.LastSubItemSlot);

                dataGrid.CollapseRowGroup(waGroup, false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(44, dataGrid.SlotCount);
                Assert.AreEqual(40, dataGrid.VisibleSlotCount);

                dataGrid.ExpandRowGroup(waGroup, false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(44, dataGrid.SlotCount);
                Assert.AreEqual(44, dataGrid.VisibleSlotCount);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests removing the last item from a group when there are collapsed groups below but not above
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests removing the last item from a group when there are collapsed groups below but not above")]
        public void DeleteLastGroupItemAboveCollapsedGroup()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 250;
            ObservableCollection<Order> orders = CreateOrders();
            while (orders[0].CountryRegion.Equals("United States", StringComparison.OrdinalIgnoreCase))
            {
                orders.RemoveAt(0);
            }
            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Shanghai", orders), 0), false);
                orders.RemoveAt(0);
                orders.RemoveAt(0);
                orders.RemoveAt(0);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(9, dataGrid.SlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(0, dataGrid.DisplayData.LastScrollingSlot);
            });
            EnqueueTestComplete();
        }

        #region Helpers

        private int CountCollectionViewGroups(ReadOnlyObservableCollection<object> groups)
        {
            if (groups == null)
            {
                return 0;
            }
            int count = 0;
            foreach (object groupObj in groups)
            {
                CollectionViewGroup collectionViewGroup = groupObj as CollectionViewGroup;
                if (collectionViewGroup != null)
                {
                    count += CountCollectionViewGroups(collectionViewGroup.Items) + 1;
                }
            }
            return count;
        }

        private ObservableCollection<Order> CreateOrders()
        {
            return new ObservableCollection<Order>()
            {
                new Order(1000, "United States", "Washington", "Seattle", "(206)-122-2382"),
                new Order(1001, "United States", "Washington", "Bellevue", "(425)-878-9018"),
                new Order(1002, "United States", "California", "San Mateo", "(650)-733-0192"),
                new Order(1003, "United States", "California", "Redwood City", "(650)-132-0128"),
                new Order(1004, "United States", "Texas", "Katy", "(281)-170-8210"),
                new Order(1005, "United States", "Texas", "Houston", "(713)-382-5422"),
                new Order(1006, "United States", "California", "San Francisco", "(415)-876-2133"),
                new Order(1007, "United States", "Washington", "Seattle", "(206)-122-2382"),
                new Order(1008, "United States", "Massachusetts", "Cambridge", "(617)-239-4738"),
                new Order(1009, "United States", "Washington", "Seattle", "(206)-122-2382"),
                new Order(1010, "United States", "Texas", "Austin", "(502)-721-4423"),
                new Order(1011, "Canada", "British Columbia", "Vancouver", "(473)-908-3782"),
                new Order(1012, "Canada", "British Columbia", "Victoria", "(347)-809-4238"),
                new Order(1013, "Canada", "British Columbia", "Richmond", "(572)-308-1389"),
                new Order(1014, "China", "Hunan", "Changsha", "(121)-7308-3128"),
                new Order(1015, "China", "Fujian", "Fuzhou", "(212)-3218-5018"),
                new Order(1016, "China", "Jianxi", "Nanchang", "(219)-3129-4527"),
                new Order(1017, "China", "Shanghai", "Shanghai", "(918)1029-9089"),
            };
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            _rowsLoaded++;
        }

        private void DataGrid_LoadingRowGroup(object sender, DataGridRowGroupHeaderEventArgs e)
        {
            _rowGroupHeadersLoaded++;   
        }

        private void DataGrid_UnloadingRowGroup(object sender, DataGridRowGroupHeaderEventArgs e)
        {
            _rowGroupHeadersUnloaded++;   
        }

        private void DataGrid_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
            _rowsUnloaded++;
        }

        private Order OrderFromCity(string city, ObservableCollection<Order> orders)
        {
            foreach (Order order in orders)
            {
                if (String.Equals(order.City, city, StringComparison.OrdinalIgnoreCase))
                {
                    return order;
                }
            }
            return null;
        }

        #endregion Helpers
    }
}
