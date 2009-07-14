// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.Data.Test.DataClasses;
using System.Windows.Controls.Test;
using System.Windows.Data;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Data.Test
{
    /// <summary>
    /// This class runs the unit tests for DataGrid editing events
    /// </summary>
    [TestClass]
    public class DataGridTests_EditingEvents : DataGridUnitTest<Customer>
    {
        #region EditEvents

        private bool _cancelCellEvent;
        private bool _cancelRowEvent;
        private int _currentCellChangedCount;
        private int _eventCount;
        private int _selectionChangedCount;

        void dataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            this._currentCellChangedCount++;
        }

        void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._selectionChangedCount++;
        }

        #region EditFirstCell

        public virtual void EditFirstCell(DataGridEditAction cellEditAction,
                                          DataGridEditAction? rowEditAction,
                                          DataGridDelegate subscribeToEvent,
                                          DataGridCellValidateDelegate validateEvent,
                                          DataGridDelegate unsubscribeToEvent)
        {
            // The first customer property should always be a string
            if (properties[0].PropertyType == typeof(string))
            {
                DataGrid dataGrid = new DataGrid();
                string originalValue;
                string updatedValue;
                dataGrid.ItemsSource = null;
                dataGrid.SelectedItems.Clear();
                rowLoaded = false;
                dataGridRow = null;
                isLoaded = false;
                dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
                dataGrid.ColumnWidth = new DataGridLength(50);
                dataGrid.Width = 650;
                dataGrid.Height = 250;
                CustomerList customerList = new CustomerList(1);
                customerList[0].LastName = "A";
                PagedCollectionView pagedCollectionView = new PagedCollectionView(customerList);
                pagedCollectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("LastName", System.ComponentModel.ListSortDirection.Ascending));

                TestPanel.Children.Add(dataGrid);
                EnqueueConditional(delegate { return isLoaded; });
                this.EnqueueYieldThread();
                EnqueueCallback(delegate
                {
                    dataGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid_LoadingRowGetRow);
                    subscribeToEvent(dataGrid);

                    dataGrid.ItemsSource = pagedCollectionView;
                });
                EnqueueConditional(delegate { return rowLoaded; });

                this.EnqueueYieldThread();
                EnqueueCallback(delegate
                {
                    dataGrid.LoadingRow -= new EventHandler<DataGridRowEventArgs>(dataGrid_LoadingRowGetRow);

                    bool success = dataGrid.BeginEdit();
                    Assert.IsTrue(success, "BeginEdit was not successful");
                });
                this.EnqueueYieldThread();
                //}
                EnqueueCallback(delegate
                {
                    //Set column to valid value
                    Assert.IsTrue(dataGrid.Columns[0].GetCellContent(customerList[0]) is TextBox, "Not a TextBox");
                    TextBox cell = ((TextBox)dataGrid.CurrentColumn.GetCellContent(customerList[0]));
                    originalValue = cell.Text;
                    ((TextBox)dataGrid.CurrentColumn.GetCellContent(customerList[0])).Text = Common.RandomString(10);
                    updatedValue = cell.Text;

                    // Either commit or cancel the cell edit
                    if (cellEditAction == DataGridEditAction.Commit)
                    {
                        dataGrid.CommitEdit(DataGridEditingUnit.Cell, true /*exitEditing*/);
                    }
                    else
                    {
                        dataGrid.CancelEdit(DataGridEditingUnit.Cell);
                    }

                    // Either commit or cancel the row edit
                    if (rowEditAction == DataGridEditAction.Commit)
                    {
                        dataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/);
                    }
                    else if (rowEditAction == DataGridEditAction.Cancel)
                    {
                        dataGrid.CancelEdit(DataGridEditingUnit.Row);
                    }

                    updatedValue = properties[0].GetValue(customerList[0], null) as String;
                    validateEvent(dataGrid, originalValue, updatedValue);
                    unsubscribeToEvent(dataGrid);
                });
            }
        }

        #endregion EditFirstCell

        #region BeginningEdit

        DataGridBeginningEditEventArgs _beginningEditEventArgs;
        void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            Assert.IsNotNull(dataGrid);
            this._beginningEditEventArgs = e;
            this._eventCount++;
        }

        [TestMethod]
        [Asynchronous]
        [Description("Check the BeginningEdit event.")]
        public virtual void BeginningEditEvent()
        {
            this._cancelCellEvent = false;
            this._cancelRowEvent = false;
            this._beginningEditEventArgs = null;
            this._eventCount = 0;
            EditFirstCell(
                DataGridEditAction.Commit /*cellEditAction*/,
                DataGridEditAction.Commit /*rowEditAction*/,
                new DataGridDelegate(
                    delegate(DataGrid dataGrid)
                    {
                        dataGrid.BeginningEdit -= new EventHandler<DataGridBeginningEditEventArgs>(dataGrid_BeginningEdit);
                        dataGrid.BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(dataGrid_BeginningEdit);
                        this._currentCellChangedCount = 0;
                        dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
                        this._selectionChangedCount = 0;
                        dataGrid.SelectionChanged += new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                    }),
                new DataGridCellValidateDelegate(
                    delegate(DataGrid dataGrid, object beforeValue, object afterValue)
                    {
                        // Ensure that the BeginningEdit event was raised
                        Assert.IsNotNull(this._beginningEditEventArgs, "BeginningEdit wasn't raised");
                        Assert.AreEqual(1, this._eventCount, "Event was raised too many times");
                        Assert.AreEqual(0, this._beginningEditEventArgs.Column.Index, "Incorrect BeginningEdit Column");
                        Assert.AreEqual(0, this._beginningEditEventArgs.Row.Index, "Incorrect BeginningEdit Row");
                    }),
                new DataGridDelegate(
                    delegate(DataGrid dataGrid)
                    {
                        dataGrid.BeginningEdit -= new EventHandler<DataGridBeginningEditEventArgs>(dataGrid_BeginningEdit);
                    }));
            EnqueueTestComplete();
        }

        #endregion BeginningEdit

        #region CellEditEnded

        private DataGridCellEditEndedEventArgs _cellEditEndedEventArgs;
        private void dataGrid_CellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            Assert.IsNotNull(dataGrid);
            this._cellEditEndedEventArgs = e;
            this._eventCount++;
        }

        private void EnqueueCellEditEndedEvent(DataGridEditAction editAction)
        {
            EditFirstCell(
                editAction /*cellEditAction*/,
                null /*rowEditAction*/,
                new DataGridDelegate(
                    delegate(DataGrid dataGrid)
                    {
                        this._cellEditEndedEventArgs = null;
                        this._eventCount = 0;
                        dataGrid.CellEditEnded += new EventHandler<DataGridCellEditEndedEventArgs>(dataGrid_CellEditEnded);
                        this._currentCellChangedCount = 0;
                        dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
                        this._selectionChangedCount = 0;
                        dataGrid.SelectionChanged += new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                    }),
                new DataGridCellValidateDelegate(
                    delegate(DataGrid dataGrid, object beforeValue, object afterValue)
                    {
                        // Ensure that the CellEditEndedEvent was raised
                        Assert.IsNotNull(this._cellEditEndedEventArgs, "CellEditEnded wasn't raised");
                        Assert.AreEqual(1, this._eventCount, "Event was raised too many times");
                        Assert.AreEqual(1, this._currentCellChangedCount, "Event was raised too many times");
                        Assert.AreEqual(1, this._selectionChangedCount, "Event was raised too many times");
                        Assert.AreEqual(0, this._cellEditEndedEventArgs.Column.Index, "Incorrect CellEditEnded Column");
                        Assert.AreEqual(editAction, this._cellEditEndedEventArgs.EditAction, "Incorrect CellEditEnded EditAction");
                        Assert.AreEqual(0, this._cellEditEndedEventArgs.Row.Index, "Incorrect CellEditEnded Row");

                        if (editAction == DataGridEditAction.Commit)
                        {
                            Assert.AreNotEqual(beforeValue, afterValue, "New value was not committed");
                        }
                        else
                        {
                            Assert.AreEqual(beforeValue, afterValue, "New value was committed");
                        }
                    }),
                new DataGridDelegate(
                    delegate(DataGrid dataGrid)
                    {
                        dataGrid.CellEditEnded -= new EventHandler<DataGridCellEditEndedEventArgs>(dataGrid_CellEditEnded);
                        dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
                        dataGrid.SelectionChanged -= new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                    }));
        }

        [TestMethod]
        [Asynchronous]
        [Description("Check the CellEditEndedEvent event.")]
        public virtual void CellEditEndedEvent()
        {
            EnqueueCellEditEndedEvent(DataGridEditAction.Commit);
            EnqueueCellEditEndedEvent(DataGridEditAction.Cancel);
            EnqueueTestComplete();
        }

        #endregion CellEditEnded

        #region CellEditEnding

        private DataGridCellEditEndingEventArgs _cellEditEndingEventArgs;
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            Assert.IsNotNull(dataGrid);
            e.Cancel = this._cancelCellEvent;
            this._cellEditEndingEventArgs = e;
            this._eventCount++;
        }

        private void EnqueueCellEditEndingEvent(DataGridEditAction editAction, bool cancelEvent)
        {
            EditFirstCell(
                editAction /*cellEditAction*/,
                null /*rowEditAction*/,
                new DataGridDelegate(
                    delegate(DataGrid dataGrid)
                    {
                        this._cancelCellEvent = cancelEvent;
                        this._cancelRowEvent = false;
                        this._cellEditEndingEventArgs = null;
                        this._eventCount = 0;
                        dataGrid.CellEditEnding -= new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid_CellEditEnding);
                        dataGrid.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid_CellEditEnding);
                        this._currentCellChangedCount = 0;
                        dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
                        this._selectionChangedCount = 0;
                        dataGrid.SelectionChanged += new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                    }),
                new DataGridCellValidateDelegate(
                    delegate(DataGrid dataGrid, object beforeValue, object afterValue)
                    {
                        // Ensure that the CellEditEndingEvent was raised
                        Assert.IsNotNull(this._cellEditEndingEventArgs, "CellEditEnding wasn't raised");
                        Assert.AreEqual(1, this._eventCount, "Event was raised too many times");
                        Assert.AreEqual(1, this._currentCellChangedCount, "Event was raised too many times");
                        Assert.AreEqual(1, this._selectionChangedCount, "Event was raised too many times");
                        Assert.AreEqual(0, this._cellEditEndingEventArgs.Column.Index, "Incorrect CellEditEnding Column");
                        Assert.AreEqual(editAction, this._cellEditEndingEventArgs.EditAction, "Incorrect CellEditEnding EditAction");
                        Assert.AreEqual(0, this._cellEditEndingEventArgs.Row.Index, "Incorrect CellEditEnding Row");

                        if (editAction == DataGridEditAction.Commit && !cancelEvent)
                        {
                            Assert.AreNotEqual(beforeValue, afterValue, "New value was not committed");
                        }
                        else
                        {
                            Assert.AreEqual(beforeValue, afterValue, "New value was committed");
                        }
                    }),
                new DataGridDelegate(
                    delegate(DataGrid dataGrid)
                    {
                        dataGrid.CellEditEnding -= new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid_CellEditEnding);
                        dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
                        dataGrid.SelectionChanged -= new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                    }));
        }

        [TestMethod]
        [Asynchronous]
        [Description("Check the CellEditEndingEvent event.")]
        public virtual void CellEditEndingEvent()
        {
            EnqueueCellEditEndingEvent(DataGridEditAction.Commit, false);
            EnqueueCellEditEndingEvent(DataGridEditAction.Commit, true);
            EnqueueCellEditEndingEvent(DataGridEditAction.Cancel, false);
            EnqueueCellEditEndingEvent(DataGridEditAction.Cancel, true);
            EnqueueTestComplete();
        }

        #endregion CellEditEnding

        #region RowEditEnded

        private DataGridRowEditEndedEventArgs _rowEditEndedEventArgs;
        void dataGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            Assert.IsNotNull(dataGrid);
            this._rowEditEndedEventArgs = e;
            this._eventCount++;
        }

        private void EnqueueRowEditEndedEvent(DataGridEditAction editAction)
        {
            EditFirstCell(
                DataGridEditAction.Commit /*cellEditAction*/,
                editAction /*rowEditAction*/,
                new DataGridDelegate(
                    delegate(DataGrid dataGrid)
                    {
                        this._cancelCellEvent = false;
                        this._cancelRowEvent = false;
                        this._rowEditEndingEventArgs = null;
                        this._eventCount = 0;
                        dataGrid.RowEditEnded -= new EventHandler<DataGridRowEditEndedEventArgs>(dataGrid_RowEditEnded);
                        dataGrid.RowEditEnded += new EventHandler<DataGridRowEditEndedEventArgs>(dataGrid_RowEditEnded);
                        this._currentCellChangedCount = 0;
                        dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
                        this._selectionChangedCount = 0;
                        dataGrid.SelectionChanged += new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                    }),
                new DataGridCellValidateDelegate(
                    delegate(DataGrid dataGrid, object beforeValue, object afterValue)
                    {
                        Assert.IsNull(dataGrid.EditingRow);
                        if (editAction == DataGridEditAction.Commit)
                        {
                            Assert.AreNotEqual(beforeValue, afterValue, "New value was not committed");
                        }
                        else
                        {
                            Assert.AreEqual(beforeValue, afterValue, "New value was committed");
                        }

                        Assert.IsNotNull(this._rowEditEndedEventArgs, "RowEditEnded wasn't raised");
                        Assert.AreEqual(1, this._eventCount, "Event was raised too many times");
                        Assert.AreEqual(1, this._currentCellChangedCount, "Event was raised too many times");
                        Assert.AreEqual(1, this._selectionChangedCount, "Event was raised too many times");
                        Assert.AreEqual(editAction, this._rowEditEndedEventArgs.EditAction, "Incorrect RowEditEnded EditAction");
                        Assert.AreEqual(dataGridRow.Index, this._rowEditEndedEventArgs.Row.Index, "Incorrect RowEditEnded Row");
                    }),
                new DataGridDelegate(
                    delegate(DataGrid dataGrid)
                    {
                        dataGrid.RowEditEnded -= new EventHandler<DataGridRowEditEndedEventArgs>(dataGrid_RowEditEnded);
                        dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
                        dataGrid.SelectionChanged -= new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                    }));
        }

        [TestMethod]
        [Asynchronous]
        [Description("Check the RowEditEndedEvent event.")]
        public virtual void RowEditEndedEvent()
        {
            EnqueueRowEditEndedEvent(DataGridEditAction.Commit);
            EnqueueRowEditEndedEvent(DataGridEditAction.Cancel);
            EnqueueTestComplete();
        }

        #endregion RowEditEnded

        #region RowEditEnding

        private DataGridRowEditEndingEventArgs _rowEditEndingEventArgs;
        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            Assert.IsNotNull(dataGrid);
            e.Cancel = this._cancelRowEvent;
            this._rowEditEndingEventArgs = e;
            this._eventCount++;
        }

        private void EnqueueRowEditEndingEvent(DataGridEditAction editAction, bool cancelEvent)
        {
            EditFirstCell(
                DataGridEditAction.Commit /*cellEditAction*/,
                editAction /*rowEditAction*/,
                new DataGridDelegate(
                    delegate(DataGrid dataGrid)
                    {
                        this._cancelCellEvent = false;
                        this._cancelRowEvent = cancelEvent;
                        this._rowEditEndingEventArgs = null;
                        this._eventCount = 0;
                        dataGrid.RowEditEnding -= new EventHandler<DataGridRowEditEndingEventArgs>(dataGrid_RowEditEnding);
                        dataGrid.RowEditEnding += new EventHandler<DataGridRowEditEndingEventArgs>(dataGrid_RowEditEnding);
                        this._currentCellChangedCount = 0;
                        dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
                        dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
                        this._selectionChangedCount = 0;
                        dataGrid.SelectionChanged -= new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                        dataGrid.SelectionChanged += new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                    }),
                new DataGridCellValidateDelegate(
                    delegate(DataGrid dataGrid, object beforeValue, object afterValue)
                    {
                        if (cancelEvent)
                        {
                            Assert.IsNotNull(dataGrid.EditingRow);
                        }
                        else
                        {
                            Assert.IsNull(dataGrid.EditingRow);
                        }

                        // Ensure that the RowEditEndingEvents were raised
                        Assert.IsNotNull(this._rowEditEndingEventArgs, "RowEditEnding wasn't raised");
                        Assert.AreEqual(1, this._eventCount, "Event was raised too many times");
                        Assert.AreEqual(1, this._currentCellChangedCount, "Event was raised too many times");
                        Assert.AreEqual(1, this._selectionChangedCount, "Event was raised too many times");
                        Assert.AreEqual(editAction, this._rowEditEndingEventArgs.EditAction, "Incorrect RowEditEnding EditAction");
                        Assert.AreEqual(dataGridRow.Index, this._rowEditEndingEventArgs.Row.Index, "Incorrect RowEditEnding Row");
                    }),
                new DataGridDelegate(
                    delegate(DataGrid dataGrid)
                    {
                        dataGrid.RowEditEnding -= new EventHandler<DataGridRowEditEndingEventArgs>(dataGrid_RowEditEnding);
                        dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
                        dataGrid.SelectionChanged -= new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                    }));
        }

        [TestMethod]
        [Asynchronous]
        [Description("Check the RowEditEndingEvent event.")]
        public virtual void RowEditEndingEvent()
        {
            EnqueueRowEditEndingEvent(DataGridEditAction.Commit, false);
            EnqueueRowEditEndingEvent(DataGridEditAction.Commit, true);
            EnqueueRowEditEndingEvent(DataGridEditAction.Cancel, false);
            EnqueueRowEditEndingEvent(DataGridEditAction.Cancel, true);
            EnqueueTestComplete();
        }

        #endregion RowEditEnding

        #endregion EditEvents

    }
}
