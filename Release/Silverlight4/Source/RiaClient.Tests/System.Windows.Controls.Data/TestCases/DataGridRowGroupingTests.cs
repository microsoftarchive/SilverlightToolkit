// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls.Data.Test.DataClasses;
using System.Windows.Controls.Primitives;
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
        private int _currentCellChangedCount;
        private int? _currentSlotOnCurrentCellChanged;
        private int _rowGroupHeadersLoaded;
        private int _rowGroupHeadersUnloaded;
        private int _rowsLoaded;
        private int _rowsUnloaded;
        private int _selectionChangedCount;
        private SelectionChangedEventArgs _selectionChangedEventArgs;

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
        /// Tests setting GroupDescriptions while the DataGrid visuals are loading
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests setting GroupDescriptions while the DataGrid visuals are loading")]
        public void GroupDescriptionsWhileLoading()
        {
            DataGrid dataGrid = new DataGrid();
            PagedCollectionView collectionView = new PagedCollectionView(CreateOrders());

            //
            // Test case where group descriptions are set while the DataGrid's rows are loading.
            //
            Assert.AreEqual(0, collectionView.GroupDescriptions.Count, "CollectionView should not have any GroupDescriptions");
            TestPanel.Children.Add(dataGrid);
            dataGrid.ItemsSource = collectionView;
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Groups should have been created
                Assert.AreEqual(dataGrid.SlotCount, collectionView.Count + collectionView.Groups.Count);
                Assert.AreEqual(collectionView.Groups.Count, dataGrid.RowGroupHeadersTable.IndexCount);
            });

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
            dataGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
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

                EnsureElements(dataGrid);
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

                EnsureElements(dataGrid);
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

                EnsureElements(dataGrid);

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

                EnsureElements(dataGrid);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests adding a new item to an empty PagedCollectionView")]
        public void AddInitialItem()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 500;
            ObservableCollection<Order> orders = new ObservableCollection<Order>();
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

            EnqueueCallback(delegate
            {
                Assert.AreEqual(0, collectionView.Groups.Count, "Incorrect number of ICollectionView.Groups");
                Assert.AreEqual(0, dataGrid.SlotCount, "Initial SlotCount incorrect");
                Assert.AreEqual(0, dataGrid.VisibleSlotCount, "Initial VisibleSlotCount incorrect");
                Assert.AreEqual(-1, dataGrid.DisplayData.FirstScrollingSlot, "Initial FirstScrollingSlot incorrect");
                Assert.AreEqual(-1, dataGrid.DisplayData.LastScrollingSlot, "Initial LastScrollingSlot incorrect");

                // Adding a new item should add a new row which is not a part of any group
                collectionView.AddNew();
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, collectionView.Groups.Count, "Incorrect number of ICollectionView.Groups");
                Assert.AreEqual(1, dataGrid.SlotCount, "SlotCount incorrect after adding new item");
                Assert.AreEqual(1, dataGrid.VisibleSlotCount, "VisibleSlotCount incorrect after adding new item");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "FirstScrollingSlot incorrect after adding new item");
                Assert.AreEqual(0, dataGrid.DisplayData.LastScrollingSlot, "LastScrollingSlot incorrect after adding new item");

                // Committing the new item should add a new row within groups
                collectionView.CommitNew();
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, collectionView.Groups.Count, "Incorrect number of ICollectionView.Groups");
                Assert.AreEqual(4, dataGrid.SlotCount, "SlotCount incorrect after committing new item");
                Assert.AreEqual(4, dataGrid.VisibleSlotCount, "VisibleSlotCount incorrect after committing new item");
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot, "FirstScrollingSlot incorrect after committing new item");
                Assert.AreEqual(3, dataGrid.DisplayData.LastScrollingSlot, "LastScrollingSlot incorrect after committing new item");

                EnsureElements(dataGrid);
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
                orders.Add(new Order(2001, "United States", "California", "San Jose", "(650)555-0118"));
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
                orders.Add(new Order(2002, "United States", "California", "Mountain View", "(650)555-0119"));
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
                orders.Add(new Order(2003, "United States", "California", "Mountain View", "(650)555-0119"));
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

                collectionView.SortDescriptions.Add(new CompMod.SortDescription("ID", CompMod.ListSortDirection.Descending));
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

                EnsureElements(dataGrid);
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
                Assert.AreEqual("Country/Region", groupHeader.PropertyName);

                groupHeader = dataGrid.DisplayData.GetDisplayedElement(1) as DataGridRowGroupHeader;
                Assert.IsNotNull(groupHeader);
                brush = groupHeader.Foreground as SolidColorBrush;
                Assert.IsNotNull(brush);
                Assert.AreEqual(brush.Color, Colors.Green);
                Assert.AreEqual(newSubLevelIndent, groupHeader.SublevelIndent);
                Assert.AreEqual("State/Province", groupHeader.PropertyName);

                groupHeader = dataGrid.DisplayData.GetDisplayedElement(2) as DataGridRowGroupHeader;
                Assert.IsNotNull(groupHeader);
                brush = groupHeader.Foreground as SolidColorBrush;
                Assert.IsNotNull(brush);
                Assert.AreEqual(brush.Color, Colors.Green);
                Assert.AreEqual(newSubLevelIndent, groupHeader.SublevelIndent);
                Assert.AreEqual("City", groupHeader.PropertyName);

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

                EnsureElements(dataGrid);
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

                EnsureElements(dataGrid);
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

                EnsureElements(dataGrid);
            });
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests pressing enter to commit an edit
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests pressing enter to commit an edit")]
        public void EditEnterTest()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 200;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = CreateOrders();

            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });
            object expectedItem = null;

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                dataGrid.SelectedItem = collectionView[0];
                expectedItem = collectionView[1];
                dataGrid.CurrentColumn = dataGrid.Columns[2];

                Assert.AreEqual(collectionView[0], dataGrid.SelectedItem);
                dataGrid.BeginEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                dataGrid.TestHook.ProcessEnterKey();

                // We should have selected the next item after pressing enter
                Assert.AreEqual(expectedItem, dataGrid.SelectedItem);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                dataGrid.SelectedItem = collectionView[2];
                expectedItem = collectionView[2];

                Assert.AreEqual(collectionView[2], dataGrid.SelectedItem);
                dataGrid.BeginEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                // Ctrl-enter commits the row without going to the next item
                dataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                // We should still have the same item selected after simulating a ctrl-enter
                Assert.AreEqual(expectedItem, dataGrid.SelectedItem);
            });
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests tabbing through RowGroupHeaders when editing
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests tabbing through RowGroupHeaders when editing")]
        public void EditTabbingTest()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = CreateOrders();

            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            dataGrid.ItemsSource = collectionView;

            bool handled = false;
            DataGridColumn lastColumn = null;
            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                dataGrid.SelectedItem = OrderFromCity("San Francisco", orders);
                lastColumn = dataGrid.Columns[dataGrid.Columns.Count - 1];
                dataGrid.CurrentColumn = lastColumn;

                Assert.AreEqual(9, dataGrid.CurrentSlot);
                dataGrid.BeginEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                handled = dataGrid.TestHook.ProcessTabKey(null);
                // We skipped over a RowGroupHeader so we went from slot 9 to slot 11
                Assert.AreEqual(11, dataGrid.CurrentSlot);
                Assert.IsTrue(handled, "Tab key was not handled by the DataGrid");

                // Start editing the last cell in the DataGrid
                dataGrid.SelectedIndex = dataGrid.DataConnection.Count - 1;
                dataGrid.CurrentColumn = lastColumn;
                dataGrid.BeginEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                // The tab key should not be handled by the DataGrid when editing the last item
                Assert.IsNotNull(dataGrid.EditingRow, "DataGrid should be editing the last row");
                handled = dataGrid.TestHook.ProcessTabKey(null);
                Assert.IsFalse(handled, "Tab key should not have been handled by the DataGrid");
            });
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests adding an item to a collapsed group
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests adding an item to a collapsed group")]
        public void AddItemToCollapsedGroup()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = CreateOrders();

            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            CollectionViewGroup californiaGroup = dataGrid.GetGroupFromItem(OrderFromCity("San Mateo", orders), 1);
            DataGridRowGroupInfo californiaGroupInfo = null;

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(30, dataGrid.SlotCount);
                Assert.AreEqual(30, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(13, dataGrid.DisplayData.LastScrollingSlot);
                californiaGroupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(californiaGroup);
                Assert.AreEqual(6, californiaGroupInfo.Slot);
                Assert.AreEqual(9, californiaGroupInfo.LastSubItemSlot);

                dataGrid.CollapseRowGroup(californiaGroup, false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(30, dataGrid.SlotCount);
                Assert.AreEqual(27, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(16, dataGrid.DisplayData.LastScrollingSlot);

                orders.Add(new Order(1003, "United States", "California", "Mountain View", "(650)-555-0119"));
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(31, dataGrid.SlotCount);
                Assert.AreEqual(27, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(17, dataGrid.DisplayData.LastScrollingSlot);
                Assert.AreEqual(6, californiaGroupInfo.Slot);
                Assert.AreEqual(10, californiaGroupInfo.LastSubItemSlot);

                dataGrid.ExpandRowGroup(californiaGroup, false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(31, dataGrid.SlotCount);
                Assert.AreEqual(31, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(13, dataGrid.DisplayData.LastScrollingSlot);

                dataGrid.CollapseRowGroup(californiaGroup, false);
            });

            CollectionViewGroup usGroup = dataGrid.GetGroupFromItem(OrderFromCity("San Mateo", orders), 0);

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(31, dataGrid.SlotCount);
                Assert.AreEqual(27, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(17, dataGrid.DisplayData.LastScrollingSlot);

                dataGrid.CollapseRowGroup(usGroup, false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(31, dataGrid.SlotCount);
                Assert.AreEqual(15, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(30, dataGrid.DisplayData.LastScrollingSlot);

                // Add an item beyond the last slot of the collapsed parent group
                orders.Add(new Order(1003, "United States", "Massachusetts", "ZZZ", "(555)-555-0120"));
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(32, dataGrid.SlotCount);
                Assert.AreEqual(15, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(31, dataGrid.DisplayData.LastScrollingSlot);

                dataGrid.ExpandRowGroup(usGroup, false);
            });

            EnqueueCallback(delegate
            {
                Assert.AreEqual(32, dataGrid.SlotCount);
                Assert.AreEqual(28, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(17, dataGrid.DisplayData.LastScrollingSlot);

                EnsureElements(dataGrid);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests editing an item where the edit causes the item to move to a different group
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests editing an item where the edit causes the item to move to a different group")]
        public void EditToAnotherGroup()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = CreateOrders();

            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            Order bellevue = OrderFromCity("Bellevue", orders);
            CollectionViewGroup washingtonGroup = dataGrid.GetGroupFromItem(bellevue, 1);
            DataGridRowGroupInfo washingtonGroupInfo = null;

            CollectionViewGroup californiaGroup = dataGrid.GetGroupFromItem(OrderFromCity("San Mateo", orders), 1);
            DataGridRowGroupInfo californiaGroupInfo = null;

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(0, bellevue.BeginEditCount);
                Assert.AreEqual(0, bellevue.CancelEditCount);
                Assert.AreEqual(0, bellevue.EndEditCount);

                Assert.AreEqual(4, washingtonGroup.ItemCount);
                washingtonGroupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(washingtonGroup);
                Assert.AreEqual(1, washingtonGroupInfo.Slot);
                Assert.AreEqual(5, washingtonGroupInfo.LastSubItemSlot);

                Assert.AreEqual(californiaGroup.ItemCount, 3);
                californiaGroupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(californiaGroup);
                Assert.AreEqual(6, californiaGroupInfo.Slot);
                Assert.AreEqual(9, californiaGroupInfo.LastSubItemSlot);

                // Edit Bellevue and move it to California
                dataGrid.SelectedItem = bellevue;
                dataGrid.CurrentColumn = dataGrid.Columns[2];
                collectionView.EditItem(bellevue);
                // dataGrid.BeginEdit should not call BeginEdit on the item since the item is editing
                dataGrid.BeginEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                TextBox editingTextBox = dataGrid.EditingRow.Cells[dataGrid.EditingColumnIndex].Content as TextBox;
                editingTextBox.Text = "California";
                dataGrid.CommitEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, bellevue.BeginEditCount);
                Assert.AreEqual(0, bellevue.CancelEditCount);
                Assert.AreEqual(1, bellevue.EndEditCount);

                Assert.AreEqual(3, washingtonGroup.ItemCount);
                Assert.AreEqual(1, washingtonGroupInfo.Slot);
                Assert.AreEqual(4, washingtonGroupInfo.LastSubItemSlot);

                Assert.AreEqual(californiaGroup.ItemCount, 4);
                Assert.AreEqual(5, californiaGroupInfo.Slot);
                Assert.AreEqual(9, californiaGroupInfo.LastSubItemSlot);

                collectionView.SortDescriptions.Add(new CompMod.SortDescription("ID", CompMod.ListSortDirection.Ascending));
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                // Check the Groups again after the Sort
                washingtonGroup = dataGrid.GetGroupFromItem(OrderFromCity("Seattle", orders), 1);
                washingtonGroupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(washingtonGroup);
                Assert.AreEqual(washingtonGroup.ItemCount, 3);
                Assert.AreEqual(1, washingtonGroupInfo.Slot);
                Assert.AreEqual(4, washingtonGroupInfo.LastSubItemSlot);

                californiaGroup = dataGrid.GetGroupFromItem(OrderFromCity("San Mateo", orders), 1);
                californiaGroupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(californiaGroup);
                Assert.AreEqual(californiaGroup.ItemCount, 4);
                Assert.AreEqual(5, californiaGroupInfo.Slot);
                Assert.AreEqual(9, californiaGroupInfo.LastSubItemSlot);

                // Edit Bellevue, put it back in Washington at the start of the group based on the ID
                // Edit directly on the CollectionView this time
                collectionView.EditItem(bellevue);
                bellevue.StateProvince = "Washington";
                bellevue.ID = 1;
                collectionView.CommitEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(2, bellevue.BeginEditCount);
                Assert.AreEqual(0, bellevue.CancelEditCount);
                Assert.AreEqual(2, bellevue.EndEditCount);

                Assert.AreEqual(4, washingtonGroup.ItemCount);
                Assert.AreEqual(1, washingtonGroupInfo.Slot);
                Assert.AreEqual(5, washingtonGroupInfo.LastSubItemSlot);

                // Check that Bellevue was put at the start of Washington
                Assert.AreEqual(bellevue, collectionView[dataGrid.RowIndexFromSlot(washingtonGroupInfo.Slot + 1)]);

                californiaGroup = dataGrid.GetGroupFromItem(OrderFromCity("San Mateo", orders), 1);
                californiaGroupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(californiaGroup);

                Assert.AreEqual(3, californiaGroup.ItemCount);
                Assert.AreEqual(6, californiaGroupInfo.Slot);
                Assert.AreEqual(9, californiaGroupInfo.LastSubItemSlot);

                // Test CancelEdit
                dataGrid.SelectedItem = bellevue;
                dataGrid.BeginEdit();
                bellevue.StateProvince = "Foo";
                dataGrid.CancelEdit();

                Assert.AreEqual("Washington", bellevue.StateProvince);
                Assert.AreEqual(3, bellevue.BeginEditCount);
                Assert.AreEqual(1, bellevue.CancelEditCount);
                Assert.AreEqual(2, bellevue.EndEditCount);

                EnsureElements(dataGrid);
            });

            EnqueueTestComplete();
        }

        // 










        /// <summary>
        /// Tests adding an item that adds a new group when the last element is a collapsed group and there is room in the DataGrid for the new group
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests adding an item that adds a new group when the last element is a collapsed group and there is room in the DataGrid for the new group")]
        public void AddGroupBelowCollapsedGroupWithSpaceAvailable()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = CreateOrders();

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
                Assert.AreEqual(30, dataGrid.SlotCount);
                Assert.AreEqual(30, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(13, dataGrid.DisplayData.LastScrollingSlot);

                CollectionViewGroup usGroup = dataGrid.GetGroupFromItem(OrderFromCity("Seattle", orders), 0);
                dataGrid.CollapseRowGroup(usGroup, false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(30, dataGrid.SlotCount);
                Assert.AreEqual(15, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(29, dataGrid.DisplayData.LastScrollingSlot);

                CollectionViewGroup chinaGroup = dataGrid.GetGroupFromItem(OrderFromCity("ShangHai", orders), 0);
                dataGrid.CollapseRowGroup(chinaGroup, false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(30, dataGrid.SlotCount);
                Assert.AreEqual(7, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(21, dataGrid.DisplayData.LastScrollingSlot);

                orders.Add(new Order(2000, "ZZZZ", "ZZZZ", "ZZZZ", "555555555"));
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(33, dataGrid.SlotCount);
                Assert.AreEqual(10, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(32, dataGrid.DisplayData.LastScrollingSlot);

                EnsureElements(dataGrid);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests externally committing the current item and verifies that currency and selection do not change
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests externally committing the current item and verifies that currency and selection do not change")]
        public void CommitCurrentItemExternally()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = CreateOrders();

            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.MoveCurrentToPosition(2);
            dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(DataGrid_CurrentCellChanged);
            dataGrid.SelectionChanged += new SelectionChangedEventHandler(DataGrid_SelectionChanged);
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(2, dataGrid.SelectedIndex);
                Assert.AreEqual(collectionView[2], dataGrid.SelectedItem);
                Assert.AreEqual(2, collectionView.CurrentPosition);

                Assert.IsNotNull(this._selectionChangedEventArgs);
                Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count);
                Assert.AreEqual(collectionView[2], this._selectionChangedEventArgs.AddedItems[0]);
                Assert.AreEqual(0, this._selectionChangedEventArgs.RemovedItems.Count);
                Assert.AreEqual(3, this._currentSlotOnCurrentCellChanged);

                this._currentCellChangedCount = 0;
                this._currentSlotOnCurrentCellChanged = -1;
                dataGrid.CurrentColumn = dataGrid.Columns[2];

                Assert.AreEqual(1, this._currentCellChangedCount);
                Assert.AreEqual(3, this._currentSlotOnCurrentCellChanged);
                Assert.AreEqual(dataGrid.Columns[2], dataGrid.CurrentColumn);

                this._currentCellChangedCount = 0;
                this._currentSlotOnCurrentCellChanged = -1;
                this._selectionChangedCount = 0;
                this._selectionChangedEventArgs = null;

                Order editItem = dataGrid.SelectedItem as Order;
                Assert.IsNotNull(editItem);
                collectionView.EditItem(editItem);
                editItem.CountryRegion = "Canada";
                collectionView.CommitEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(10, dataGrid.SelectedIndex);
                Assert.AreEqual(collectionView[10], dataGrid.SelectedItem);
                Assert.AreEqual(10, collectionView.CurrentPosition);
                Assert.AreEqual(dataGrid.Columns[2], dataGrid.CurrentColumn);

                Assert.AreEqual(0, this._selectionChangedCount);
                Assert.AreEqual(0, this._currentCellChangedCount);

                dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(DataGrid_CurrentCellChanged);
                dataGrid.SelectionChanged -= new SelectionChangedEventHandler(DataGrid_SelectionChanged);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests removing the current item and verifies that currency and selection update
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests removing the current item and verifies that currency and selection update")]
        public void RemoveCurrentItem()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = CreateOrders();

            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.MoveCurrentToPosition(2);
            dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(DataGrid_CurrentCellChanged);
            dataGrid.SelectionChanged += new SelectionChangedEventHandler(DataGrid_SelectionChanged);
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            object oldSelectedItem = null;
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(2, dataGrid.SelectedIndex);
                Assert.AreEqual(collectionView[2], dataGrid.SelectedItem);
                Assert.AreEqual(2, collectionView.CurrentPosition);

                Assert.IsNotNull(this._selectionChangedEventArgs);
                Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count);
                Assert.AreEqual(collectionView[2], this._selectionChangedEventArgs.AddedItems[0]);
                Assert.AreEqual(0, this._selectionChangedEventArgs.RemovedItems.Count);
                Assert.AreEqual(3, this._currentSlotOnCurrentCellChanged);

                this._currentCellChangedCount = 0;
                this._currentSlotOnCurrentCellChanged = -1;
                dataGrid.CurrentColumn = dataGrid.Columns[2];

                Assert.AreEqual(1, this._currentCellChangedCount);
                Assert.AreEqual(3, this._currentSlotOnCurrentCellChanged);
                Assert.AreEqual(dataGrid.Columns[2], dataGrid.CurrentColumn);

                this._currentCellChangedCount = 0;
                this._currentSlotOnCurrentCellChanged = -1;
                this._selectionChangedCount = 0;
                this._selectionChangedEventArgs = null;

                oldSelectedItem = dataGrid.SelectedItem;
                collectionView.RemoveAt(2);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(2, dataGrid.SelectedIndex);
                Assert.AreEqual(collectionView[2], dataGrid.SelectedItem);
                Assert.AreEqual(2, collectionView.CurrentPosition);

                Assert.AreEqual(1, this._selectionChangedCount);
                Assert.IsNotNull(this._selectionChangedEventArgs);
                Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count);
                Assert.AreEqual(collectionView[2], this._selectionChangedEventArgs.AddedItems[0]);
                Assert.AreEqual(1, this._selectionChangedEventArgs.RemovedItems.Count);
                Assert.AreEqual(oldSelectedItem, this._selectionChangedEventArgs.RemovedItems[0]);
                Assert.AreEqual(1, this._currentCellChangedCount);
                Assert.AreEqual(3, this._currentSlotOnCurrentCellChanged);

                dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(DataGrid_CurrentCellChanged);
                dataGrid.SelectionChanged -= new SelectionChangedEventHandler(DataGrid_SelectionChanged);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests removing from a collapsed group
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests removing from a collapsed group")]
        public void RemoveFromCollapsedGroup()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = CreateOrders();

            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            Order sanMateo = OrderFromCity("San Mateo", orders);
            CollectionViewGroup californiaGroup = dataGrid.GetGroupFromItem(sanMateo, 1);
            DataGridRowGroupInfo californiaGroupInfo = null;

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(30, dataGrid.SlotCount);
                Assert.AreEqual(30, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(13, dataGrid.DisplayData.LastScrollingSlot);
                californiaGroupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(californiaGroup);
                Assert.AreEqual(6, californiaGroupInfo.Slot);
                Assert.AreEqual(9, californiaGroupInfo.LastSubItemSlot);

                dataGrid.CollapseRowGroup(californiaGroup, false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(30, dataGrid.SlotCount);
                Assert.AreEqual(27, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(16, dataGrid.DisplayData.LastScrollingSlot);
                Assert.AreEqual(6, californiaGroupInfo.Slot);
                Assert.AreEqual(9, californiaGroupInfo.LastSubItemSlot);

                orders.Remove(sanMateo);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(29, dataGrid.SlotCount);
                Assert.AreEqual(27, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(15, dataGrid.DisplayData.LastScrollingSlot);
                Assert.AreEqual(6, californiaGroupInfo.Slot);
                Assert.AreEqual(8, californiaGroupInfo.LastSubItemSlot);

                orders.Remove(OrderFromCity("Redwood City", orders));
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(28, dataGrid.SlotCount);
                Assert.AreEqual(27, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(14, dataGrid.DisplayData.LastScrollingSlot);
                Assert.AreEqual(6, californiaGroupInfo.Slot);
                Assert.AreEqual(7, californiaGroupInfo.LastSubItemSlot);

                // Remove the last order in California
                orders.Remove(OrderFromCity("San Francisco", orders));
            });

            CollectionViewGroup usGroup = null; 
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(26, dataGrid.SlotCount);
                Assert.AreEqual(26, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(13, dataGrid.DisplayData.LastScrollingSlot);
                Assert.IsNull(dataGrid.RowGroupInfoFromCollectionViewGroup(californiaGroup));

                usGroup = dataGrid.GetGroupFromItem(OrderFromCity("Seattle", orders), 0);
                dataGrid.CollapseRowGroup(usGroup, false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(26, dataGrid.SlotCount);
                Assert.AreEqual(15, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(25, dataGrid.DisplayData.LastScrollingSlot);

                dataGrid.ExpandRowGroup(usGroup, false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(26, dataGrid.SlotCount);
                Assert.AreEqual(26, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(13, dataGrid.DisplayData.LastScrollingSlot);

                EnsureElements(dataGrid);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests removing an item above the current slot and verifies that selection and currency do not change
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests removing an item above the current slot and verifies that selection and currency do not change")]
        public void RemoveItemAboveCurrentSlot()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = CreateOrders();

            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("StateProvince"));
            collectionView.MoveCurrentToPosition(3);
            dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(DataGrid_CurrentCellChanged);
            dataGrid.SelectionChanged+= new SelectionChangedEventHandler(DataGrid_SelectionChanged);
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(3, dataGrid.SelectedIndex);
                Assert.AreEqual(collectionView[3], dataGrid.SelectedItem);
                Assert.AreEqual(3, collectionView.CurrentPosition);

                Assert.IsNotNull(this._selectionChangedEventArgs);
                Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count);
                Assert.AreEqual(collectionView[3], this._selectionChangedEventArgs.AddedItems[0]);
                Assert.AreEqual(0, this._selectionChangedEventArgs.RemovedItems.Count);
                Assert.AreEqual(5, this._currentSlotOnCurrentCellChanged);

                this._currentCellChangedCount = 0;
                this._currentSlotOnCurrentCellChanged = -1;
                dataGrid.CurrentColumn = dataGrid.Columns[2];

                Assert.AreEqual(1, this._currentCellChangedCount);
                Assert.AreEqual(5, this._currentSlotOnCurrentCellChanged);
                Assert.AreEqual(dataGrid.Columns[2], dataGrid.CurrentColumn);

                this._currentCellChangedCount = 0;
                this._currentSlotOnCurrentCellChanged = -1;
                this._selectionChangedCount = 0;
                this._selectionChangedEventArgs = null;

                collectionView.RemoveAt(1);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(2, dataGrid.SelectedIndex);
                Assert.AreEqual(collectionView[2], dataGrid.SelectedItem);
                Assert.AreEqual(2, collectionView.CurrentPosition);

                Assert.AreEqual(0, this._selectionChangedCount);
                Assert.AreEqual(0, this._currentCellChangedCount);

                dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(DataGrid_CurrentCellChanged);
                dataGrid.SelectionChanged -= new SelectionChangedEventHandler(DataGrid_SelectionChanged);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests removing the last item from the DataGrid in the case where the last item is collapsed
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests removing the last item from the DataGrid in the case where the last item is collapsed")]
        public void RemoveLastItemWhenCollapsed()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = new ObservableCollection<Order>()
            {
                new Order(1000, "United States", "Washington", "Seattle", "(206)555-0100"),
                new Order(1001, "United States", "Washington", "Bellevue", "(425)555-0101")
            };

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
                Assert.AreEqual(4, dataGrid.SlotCount);
                Assert.AreEqual(4, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(3, dataGrid.DisplayData.LastScrollingSlot);

                // Collapse Washington
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(orders[0], 1), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(4, dataGrid.SlotCount);
                Assert.AreEqual(2, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(1, dataGrid.DisplayData.LastScrollingSlot);

                // Remove Seattle
                orders.RemoveAt(0);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(3, dataGrid.SlotCount);
                Assert.AreEqual(2, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(1, dataGrid.DisplayData.LastScrollingSlot);

                // Remove Bellevue
                orders.RemoveAt(0);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                // Nothing displayed in the DataGrid
                Assert.AreEqual(0, dataGrid.SlotCount);
                Assert.AreEqual(0, dataGrid.VisibleSlotCount);
                Assert.AreEqual(-1, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(-1, dataGrid.DisplayData.LastScrollingSlot);

                // Add a new order; should create 2 new groups and 1 item
                orders.Add(new Order(1000, "United States", "Washington", "Seattle", "(206)-555-0121"));
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(3, dataGrid.SlotCount);
                Assert.AreEqual(3, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(2, dataGrid.DisplayData.LastScrollingSlot);

                EnsureElements(dataGrid);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests editing an item to be in a new group below multiple collapsed groups
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests editing an item to be in a new group below multiple collapsed groups")]
        public void EditToNewGroupBelowMultipleCollapsedGroups()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = new ObservableCollection<Order>()
            {
                new Order(1000, "United States", "Washington", "Seattle", "(206)555-0100"),
                new Order(1001, "United States", "Washington", "Bellevue", "(425)555-0101"),
                new Order(1002, "United States", "California", "San Mateo", "(650)555-0102"),
                new Order(1003, "United States", "California", "Redwood City", "(650)555-0103"),
                new Order(1008, "United States", "Massachusetts", "Cambridge", "(617)555-0108"),
                new Order(1008, "United States", "Massachusetts", "Boston", "(617)-555-0109"),
            };

            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.SortDescriptions.Add(new CompMod.SortDescription("City", CompMod.ListSortDirection.Ascending));
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
                Assert.AreEqual(10, dataGrid.SlotCount);
                Assert.AreEqual(10, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(9, dataGrid.DisplayData.LastScrollingSlot);

                // Collapse Massachusetts
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Boston", orders), 1), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(10, dataGrid.SlotCount);
                Assert.AreEqual(8, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(9, dataGrid.DisplayData.LastScrollingSlot);

                // Collapse California
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("San Mateo", orders), 1), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(10, dataGrid.SlotCount);
                Assert.AreEqual(6, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(7, dataGrid.DisplayData.LastScrollingSlot);

                dataGrid.SelectedItem = OrderFromCity("Seattle", orders);
                dataGrid.CurrentColumn = dataGrid.Columns[2];
                dataGrid.BeginEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                // Edit Seattle to a new group at the end of the groups
                TextBox editingTextBox = dataGrid.EditingRow.Cells[dataGrid.EditingColumnIndex].Content as TextBox;
                editingTextBox.Text = "ZZZZ";
                dataGrid.CommitEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(11, dataGrid.SlotCount);
                Assert.AreEqual(7, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(10, dataGrid.DisplayData.LastScrollingSlot);

                EnsureElements(dataGrid);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests expanding a group by moving currency to collapsed rows
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests expanding a group by moving currency to collapsed rows")]
        public void MoveCurrencyToCollapsedRows()
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
            EnqueueConditional(delegate { return isLoaded; });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(DataGrid_CurrentCellChanged);
                dataGrid.SelectionChanged += new SelectionChangedEventHandler(DataGrid_SelectionChanged);

                // Collapse the first group
                this._selectionChangedEventArgs = null;
                this._currentSlotOnCurrentCellChanged = null;
                dataGrid.CollapseRowGroup(collectionView.Groups[0] as CollectionViewGroup, false);
                dataGrid.CollapseRowGroup(collectionView.Groups[1] as CollectionViewGroup, false);
            });
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                DataGridRowGroupInfo groupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(collectionView.Groups[0] as CollectionViewGroup);
                Assert.IsNotNull(groupInfo, "RowGroupInfo is null");
                Assert.AreEqual(Visibility.Collapsed, groupInfo.Visibility, "Group 0 should be collapsed");

                groupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(collectionView.Groups[1] as CollectionViewGroup);
                Assert.IsNotNull(groupInfo, "RowGroupInfo is null");
                Assert.AreEqual(Visibility.Collapsed, groupInfo.Visibility, "Group 1 should be collapsed");

                Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChanged event was not raised");
                Assert.AreEqual(0, this._selectionChangedEventArgs.AddedItems.Count, "No items should be selected after moving currency to a RowGroupHeader");
                Assert.AreEqual(1, this._selectionChangedEventArgs.RemovedItems.Count, "All items should be unselected after moving currency to a RowGroupHeader");
                Assert.AreEqual(collectionView[0], this._selectionChangedEventArgs.RemovedItems[0], "Wrong RemovedItem in SelectionChanged event");
                Assert.AreEqual(0, this._currentSlotOnCurrentCellChanged, "Incorrect currency change when a group is collapsed");

                // Move currency to the collapsed row
                this._selectionChangedEventArgs = null;
                this._currentSlotOnCurrentCellChanged = null;
                collectionView.MoveCurrentToPosition(0);
            });
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                DataGridRowGroupInfo groupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(collectionView.Groups[0] as CollectionViewGroup);
                Assert.IsNotNull(groupInfo, "RowGroupInfo is null");
                Assert.AreEqual(Visibility.Visible, groupInfo.Visibility, "Group should be visible");

                Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChanged event was not raised");
                Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count, "An item should be selected after moving currency to a collapsed row");
                Assert.AreEqual(0, this._selectionChangedEventArgs.RemovedItems.Count, "No items should should have been unselected after moving currency to a collapsed row");
                Assert.AreEqual(collectionView[0], this._selectionChangedEventArgs.AddedItems[0], "Wrong AddedItem in SelectionChanged event");
                Assert.AreEqual(1, this._currentSlotOnCurrentCellChanged, "Incorrect CurrentSlot when currency moved to a collapsed row");

                // Select an item within a different collapsed group
                this._selectionChangedEventArgs = null;
                this._currentSlotOnCurrentCellChanged = null;
                CollectionViewGroup group = collectionView.Groups[1] as CollectionViewGroup;
                dataGrid.SelectedItem = group.Items[0];
            });
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                DataGridRowGroupInfo groupInfo = dataGrid.RowGroupInfoFromCollectionViewGroup(collectionView.Groups[1] as CollectionViewGroup);
                Assert.IsNotNull(groupInfo, "RowGroupInfo is null");
                Assert.AreEqual(Visibility.Visible, groupInfo.Visibility, "Group should be visible");

                CollectionViewGroup group = collectionView.Groups[1] as CollectionViewGroup;
                Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChanged event was not raised");
                Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count, "An item should be selected after moving currency to a collapsed row");
                Assert.AreEqual(1, this._selectionChangedEventArgs.RemovedItems.Count, "No items should should have been unselected after moving currency to a collapsed row");
                Assert.AreEqual(collectionView[0], this._selectionChangedEventArgs.RemovedItems[0], "Wrong RemovedItem in SelectionChanged event");
                Assert.AreEqual(group.Items[0], this._selectionChangedEventArgs.AddedItems[0], "Wrong AddedItem in SelectionChanged event");
                Assert.AreEqual(dataGrid.SlotFromRowIndex(collectionView.IndexOf(group.Items[0])), this._currentSlotOnCurrentCellChanged, "Incorrect CurrentSlot when currency moved to a new row");

                dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(DataGrid_CurrentCellChanged);
                dataGrid.SelectionChanged -= new SelectionChangedEventHandler(DataGrid_SelectionChanged);
            });
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests editing an item to be the first item in a collapsed group below it
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests editing an item to be the first item in a collapsed group below it")]
        public void EditToFirstItemOfCollapsedGroupBelow()
        {
            DataGrid dataGrid = new DataGrid();

            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = new ObservableCollection<Order>()
            {

                new Order(1000, "United States", "Washington", "Seattle", "(206)555-0100"),
                new Order(1001, "United States", "Washington", "Bellevue", "(425)555-0101"),
                new Order(1002, "United States", "California", "San Mateo", "(650)555-0102"),
                new Order(1003, "United States", "California", "Redwood City", "(650)555-0103"),
                new Order(1008, "United States", "Massachusetts", "Cambridge", "(617)555-0104"),
                new Order(1008, "United States", "Massachusetts", "Boston", "(617)555-0105"),
            };

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
                Assert.AreEqual(10, dataGrid.SlotCount);
                Assert.AreEqual(10, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(9, dataGrid.DisplayData.LastScrollingSlot);

                // Collapse Massachusetts
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Boston", orders), 1), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(10, dataGrid.SlotCount);
                Assert.AreEqual(8, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(7, dataGrid.DisplayData.LastScrollingSlot);

                dataGrid.SelectedItem = OrderFromCity("Seattle", orders);
                dataGrid.CurrentColumn = dataGrid.Columns[2];
                dataGrid.BeginEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                // Edit Seattle to a new group at the end of the groups
                TextBox editingTextBox = dataGrid.EditingRow.Cells[dataGrid.EditingColumnIndex].Content as TextBox;
                editingTextBox.Text = "Massachusetts";
                dataGrid.ProcessEnterKey();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(10, dataGrid.SlotCount);
                Assert.AreEqual(7, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(6, dataGrid.DisplayData.LastScrollingSlot);

                EnsureElements(dataGrid);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests removing an item that causes multiple levels of groups to be removed
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests removing an item that causes multiple levels of groups to be removed")]
        public void RemoveMultipleGroupLevelsByDeletingItem()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = new ObservableCollection<Order>()
            {
                new Order(1000, "United States", "Washington", "Seattle", "(206)555-0100"),
                new Order(1013, "Canada", "British Columbia", "Richmond", "(572)555-0101"),
            };

            PagedCollectionView collectionView = new PagedCollectionView(orders);
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
                Assert.AreEqual(8, dataGrid.SlotCount);
                Assert.AreEqual(8, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(7, dataGrid.DisplayData.LastScrollingSlot);

                orders.RemoveAt(0);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(4, dataGrid.SlotCount);
                Assert.AreEqual(4, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(3, dataGrid.DisplayData.LastScrollingSlot);

                DataGridRow row = dataGrid.DisplayData.GetDisplayedRow(0);
                Assert.AreEqual(row.DataContext, orders[0]);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that the vertical offset is updated when there is a collapse and the last visible slot is visible
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that the vertical offset is updated when there is a collapse and the last visible slot is visible")]
        public void VerticalOffsetOnCollapseWhenLastVisibleSlotDisplayed()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 350;
            dataGrid.Width = 350;
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

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(46, dataGrid.SlotCount);
                Assert.AreEqual(46, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(14, dataGrid.DisplayData.LastScrollingSlot);

                dataGrid.ScrollIntoView(orders[orders.Count - 1], null);
            });

            double verticalOffset = double.NaN;
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(46, dataGrid.SlotCount);
                Assert.AreEqual(46, dataGrid.VisibleSlotCount);
                Assert.AreEqual(31, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(45, dataGrid.DisplayData.LastScrollingSlot);

                verticalOffset = dataGrid.VerticalScrollBar.Value;
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Shanghai", orders), 0), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(46, dataGrid.SlotCount);
                Assert.AreEqual(34, dataGrid.VisibleSlotCount);
                Assert.AreEqual(19, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(33, dataGrid.DisplayData.LastScrollingSlot);

                Assert.IsTrue(verticalOffset > dataGrid.VerticalScrollBar.Value);

                verticalOffset = dataGrid.VerticalScrollBar.Value;
                dataGrid.CollapseRowGroup(dataGrid.GetGroupFromItem(OrderFromCity("Vancouver", orders), 0), false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(46, dataGrid.SlotCount);
                Assert.AreEqual(27, dataGrid.VisibleSlotCount);
                Assert.AreEqual(12, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(33, dataGrid.DisplayData.LastScrollingSlot);

                Assert.IsTrue(verticalOffset > dataGrid.VerticalScrollBar.Value);

                EnsureElements(dataGrid);
            });

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests using the up key to commit edit when the edit causes the target RowGroupHeader above the item
        /// to disappear.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests using the up key to commit edit when the edit causes the target RowGroupHeader above the item to disappear.")]
        public void UpKeyToCommitWhenTargetGroupDisappears()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = new ObservableCollection<Order>()
            {
                new Order(1000, "United States", "Washington", "Seattle", "(206)555-0100"),
                new Order(1001, "United States", "Washington", "Bellevue", "(425)555-0101"),
                new Order(1002, "United States", "California", "San Mateo", "(650)555-0102"),
                new Order(1003, "United States", "California", "Redwood City", "(650)555-0103"),
                new Order(1006, "United States", "California", "San Francisco", "(415)555-0106"),
                new Order(1011, "Canada", "British Columbia", "Vancouver", "(473)555-0111"),
            };

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
                Assert.AreEqual(11, dataGrid.SlotCount);
                Assert.AreEqual(11, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(10, dataGrid.DisplayData.LastScrollingSlot);

                CollectionViewGroup usGroup = dataGrid.GetGroupFromItem(OrderFromCity("Seattle", orders), 0);
                dataGrid.CollapseRowGroup(usGroup, false/*collapseAllSubgroups*/);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(11, dataGrid.SlotCount);
                Assert.AreEqual(4, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(10, dataGrid.DisplayData.LastScrollingSlot);

                dataGrid.SelectedItem = orders[5];
                dataGrid.CurrentColumn = dataGrid.Columns[1];
                dataGrid.BeginEdit();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                // Edit Vancouver in a way that makes the Canada Group and the Vancouver Group go away
                orders[5].StateProvince = "Washington";
                TextBox editingTextBox = dataGrid.EditingRow.Cells[dataGrid.EditingColumnIndex].Content as TextBox;
                editingTextBox.Text = "United States";
                dataGrid.ProcessUpKey();
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(9, dataGrid.SlotCount);
                Assert.AreEqual(1, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(0, dataGrid.DisplayData.LastScrollingSlot);
                Assert.AreEqual(0, dataGrid.CurrentSlot);
            });
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests a collapse operation that changes the vertical offset even though the last slot is not visible
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests a collapse operation that changes the vertical offset even though the last slot is not visible")]
        public void CollapseThatChangesVerticalOffsetWhenLastSlotIsNotVisible()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Height = 350;
            dataGrid.Width = 350;
            ObservableCollection<Order> orders = CreateOrders();

            PagedCollectionView collectionView = new PagedCollectionView(orders);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("CountryRegion"));
            dataGrid.ItemsSource = collectionView;

            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            CollectionViewGroup chinaGroup = null;
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(21, dataGrid.SlotCount);
                Assert.AreEqual(21, dataGrid.VisibleSlotCount);
                Assert.AreEqual(0, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(13, dataGrid.DisplayData.LastScrollingSlot);

                Order hunan = OrderFromCity("Changsha", orders);
                chinaGroup = dataGrid.GetGroupFromItem(hunan, 0);
                dataGrid.ScrollIntoView(hunan, null);
            });

            double verticalOffset = double.NaN;
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(21, dataGrid.SlotCount);
                Assert.AreEqual(21, dataGrid.VisibleSlotCount);
                Assert.AreEqual(4, dataGrid.DisplayData.FirstScrollingSlot);
                Assert.AreEqual(17, dataGrid.DisplayData.LastScrollingSlot);

                verticalOffset = dataGrid.VerticalScrollBar.Value;
                dataGrid.CollapseRowGroup(chinaGroup, false);
            });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(21, dataGrid.SlotCount);
                Assert.AreEqual(17, dataGrid.VisibleSlotCount);

                Assert.IsTrue(verticalOffset > dataGrid.VerticalScrollBar.Value);

                EnsureElements(dataGrid);
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
                new Order(1000, "United States", "Washington", "Seattle", "(206)555-0100"),
                new Order(1001, "United States", "Washington", "Bellevue", "(425)555-0101"),
                new Order(1002, "United States", "California", "San Mateo", "(650)555-0102"),
                new Order(1003, "United States", "California", "Redwood City", "(650)555-0103"),
                new Order(1004, "United States", "Texas", "Katy", "(281)555-0104"),
                new Order(1005, "United States", "Texas", "Houston", "(713)555-0105"),
                new Order(1006, "United States", "California", "San Francisco", "(415)555-0106"),
                new Order(1007, "United States", "Washington", "Seattle", "(206)555-0107"),
                new Order(1008, "United States", "Massachusetts", "Cambridge", "(617)555-0108"),
                new Order(1009, "United States", "Washington", "Seattle", "(206)555-0109"),
                new Order(1010, "United States", "Texas", "Austin", "(502)555-0110"),
                new Order(1011, "Canada", "British Columbia", "Vancouver", "(473)555-0111"),
                new Order(1012, "Canada", "British Columbia", "Victoria", "(347)555-0112"),
                new Order(1013, "Canada", "British Columbia", "Richmond", "(572)555-0113"),
                new Order(1014, "China", "Hunan", "Changsha", "(221)555-0114"),
                new Order(1015, "China", "Fujian", "Fuzhou", "(212)555-0115"),
                new Order(1016, "China", "Jianxi", "Nanchang", "(219)555-0116"),
                new Order(1017, "China", "Shanghai", "Shanghai", "(918)555-0117"),
            };
        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            this._currentCellChangedCount++;
            this._currentSlotOnCurrentCellChanged = ((DataGrid)sender).CurrentSlot;
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._selectionChangedCount++;
            this._selectionChangedEventArgs = e;
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

        // Check to make sure the visuals in the DataGrid are not displaying duplicated
        // items or groups nor do they contain duplicated slots
        private void EnsureElements(DataGrid dataGrid)
        {
            DataGridRowsPresenter rowsPresenter = null;
            Queue<DependencyObject> visuals = new Queue<DependencyObject>();
            visuals.Enqueue(dataGrid);
            while ((rowsPresenter == null) && (visuals.Count > 0))
            {
                DependencyObject visual = visuals.Dequeue();
                rowsPresenter = visual as DataGridRowsPresenter;
                if ((rowsPresenter == null) && (visual != null))
                {
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
                    {
                        visuals.Enqueue(VisualTreeHelper.GetChild(visual, i));
                    }
                }
            }
            Assert.IsNotNull(rowsPresenter);

            // Check for slot collisions or duplicated items
            Dictionary<int, object> itemSlots = new Dictionary<int, object>();
            foreach (UIElement element in rowsPresenter.Children)
            {
                DataGridRow row = element as DataGridRow;
                if (row != null)
                {
                    if ((row.Slot != -1) && (row.Visibility == Visibility.Visible))
                    {
                        Assert.IsFalse(itemSlots.ContainsKey(row.Slot));
                        Assert.IsFalse(itemSlots.ContainsValue(row.DataContext));
                        itemSlots.Add(row.Slot, row.DataContext);
                    }
                    else
                    {
                        DataGridRowGroupHeader rowGroupHeader = element as DataGridRowGroupHeader;
                        if ((rowGroupHeader != null) && (rowGroupHeader.Visibility == Visibility.Visible))
                        {
                            Assert.IsNotNull(rowGroupHeader.RowGroupInfo);
                            Assert.IsFalse(itemSlots.ContainsKey(rowGroupHeader.RowGroupInfo.Slot));
                            Assert.IsFalse(itemSlots.ContainsValue(rowGroupHeader.DataContext));
                            itemSlots.Add(rowGroupHeader.RowGroupInfo.Slot, rowGroupHeader.DataContext);
                        }
                    }
                }
            }
        }

        #endregion Helpers
    }
}
