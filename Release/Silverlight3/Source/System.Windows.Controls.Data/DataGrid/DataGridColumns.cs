// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace System.Windows.Controls
{
    public partial class DataGrid
    {
        #region Private Properties
        #endregion Private Properties

        #region Protected Methods

        protected virtual void OnColumnDisplayIndexChanged(DataGridColumnEventArgs e)
        {
            EventHandler<DataGridColumnEventArgs> handler = this.ColumnDisplayIndexChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected internal virtual void OnColumnReordered(DataGridColumnEventArgs e)
        {
            this.EnsureVerticalGridLines();

            EventHandler<DataGridColumnEventArgs> handler = this.ColumnReordered;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected internal virtual void OnColumnReordering(DataGridColumnReorderingEventArgs e)
        {
            EventHandler<DataGridColumnReorderingEventArgs> handler = this.ColumnReordering;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion Protected Methods

        #region Internal Methods

        internal bool ColumnRequiresRightGridLine(DataGridColumn dataGridColumn, bool includeLastRightGridLineWhenPresent)
        {
            return (this.GridLinesVisibility == DataGridGridLinesVisibility.Vertical || this.GridLinesVisibility == DataGridGridLinesVisibility.All) && this.VerticalGridLinesBrush != null &&
                   (dataGridColumn != this.ColumnsInternal.LastVisibleColumn || (includeLastRightGridLineWhenPresent && this.ColumnsInternal.FillerColumn.IsActive));
        }

        internal DataGridColumnCollection CreateColumnsInstance()
        {
            return new DataGridColumnCollection(this);
        }

        internal bool GetColumnReadOnlyState(DataGridColumn dataGridColumn, bool isReadOnly)
        {
            Debug.Assert(dataGridColumn != null);

            DataGridBoundColumn dataGridBoundColumn = dataGridColumn as DataGridBoundColumn;
            if (dataGridBoundColumn != null && dataGridBoundColumn.Binding != null)
            {
                string path = null;
                if (dataGridBoundColumn.Binding.Path != null)
                {
                    path = dataGridBoundColumn.Binding.Path.Path;
                }

                if (!string.IsNullOrEmpty(path))
                {
                    return this.DataConnection.GetPropertyIsReadOnly(path) || isReadOnly;
                }
            }

            return isReadOnly;
        }

        // Returns the column's width
        internal static double GetEdgedColumnWidth(DataGridColumn dataGridColumn)
        {
            Debug.Assert(dataGridColumn != null);
            return dataGridColumn.ActualWidth;
        }

        internal void OnClearingColumns()
        {
            // Rows need to be cleared first. There cannot be rows without also having columns.
            ClearRows(false);

            // Removing all the column header cells
            RemoveDisplayedColumnHeaders();

            this._horizontalOffset = this._negHorizontalOffset = 0;
            if (this._hScrollBar != null && this._hScrollBar.Visibility == Visibility.Visible) // 
            {
                this._hScrollBar.Value = 0;
            }
        }

        internal void OnColumnCellStyleChanged(DataGridColumn column, Style previousStyle)
        {
            // Set HeaderCell.Style for displayed rows if HeaderCell.Style is not already set
            foreach (DataGridRow row in GetAllRows())
            {
                row.Cells[column.Index].EnsureStyle(previousStyle);
            }
            InvalidateRowHeightEstimate();
        }

        internal void OnColumnCollectionChanged_PostNotification(bool columnsGrew)
        {
            if (columnsGrew &&
                this.CurrentColumnIndex == -1)
            {
                MakeFirstDisplayedCellCurrentCell();
            }

            if (_autoGeneratingColumnOperationCount == 0)
            {
                EnsureRowsPresenterVisibility();
                InvalidateRowHeightEstimate();
            }
        }

        internal void OnColumnCollectionChanged_PreNotification(bool columnsGrew)
        {
            // dataGridColumn==null means the collection was refreshed.

            if (columnsGrew && _autoGeneratingColumnOperationCount == 0 && this.ColumnsItemsInternal.Count == 1)
            {
                RefreshRows(false /*recycleRows*/, true /*clearRows*/);
            }
            else
            {
                InvalidateMeasure();
            }
        }

        internal void OnColumnDisplayIndexChanged(DataGridColumn dataGridColumn)
        {
            Debug.Assert(dataGridColumn != null);
            DataGridColumnEventArgs e = new DataGridColumnEventArgs(dataGridColumn);

            // Call protected method to raise event
            if (dataGridColumn != this.ColumnsInternal.RowGroupSpacerColumn)
            {
                OnColumnDisplayIndexChanged(e);
            }
        }

        internal void OnColumnDisplayIndexChanged_PostNotification()
        {
            // Notifications for adjusted display indexes.
            FlushDisplayIndexChanged(true /*raiseEvent*/);

            // Our displayed columns may have changed so recompute them
            UpdateDisplayedColumns();

            // Invalidate layout
            CorrectColumnFrozenStates();
            EnsureHorizontalLayout();
        }

        internal void OnColumnDisplayIndexChanging(DataGridColumn targetColumn, int newDisplayIndex)
        {
            Debug.Assert(targetColumn != null);
            Debug.Assert(newDisplayIndex != targetColumn.DisplayIndexWithFiller);

            if (InDisplayIndexAdjustments)
            {
                // We are within columns display indexes adjustments. We do not allow changing display indexes while adjusting them.
                throw DataGridError.DataGrid.CannotChangeColumnCollectionWhileAdjustingDisplayIndexes();
            }

            try
            {
                InDisplayIndexAdjustments = true;

                bool trackChange = targetColumn != this.ColumnsInternal.RowGroupSpacerColumn;

                DataGridColumn column;
                // Move is legal - let's adjust the affected display indexes.
                if (newDisplayIndex < targetColumn.DisplayIndexWithFiller)
                {
                    // DisplayIndex decreases. All columns with newDisplayIndex <= DisplayIndex < targetColumn.DisplayIndex
                    // get their DisplayIndex incremented.
                    for (int i = newDisplayIndex; i < targetColumn.DisplayIndexWithFiller; i++)
                    {
                        column = this.ColumnsInternal.GetColumnAtDisplayIndex(i);
                        column.DisplayIndexWithFiller = column.DisplayIndexWithFiller + 1;
                        if (trackChange)
                        {
                            column.DisplayIndexHasChanged = true; // OnColumnDisplayIndexChanged needs to be raised later on
                        }
                    }
                }
                else
                {
                    // DisplayIndex increases. All columns with targetColumn.DisplayIndex < DisplayIndex <= newDisplayIndex
                    // get their DisplayIndex decremented.
                    for (int i = newDisplayIndex; i > targetColumn.DisplayIndexWithFiller; i--)
                    {
                        column = this.ColumnsInternal.GetColumnAtDisplayIndex(i);
                        column.DisplayIndexWithFiller = column.DisplayIndexWithFiller - 1;
                        if (trackChange)
                        {
                            column.DisplayIndexHasChanged = true; // OnColumnDisplayIndexChanged needs to be raised later on
                        }
                    }
                }
                // Now let's actually change the order of the DisplayIndexMap
                if (targetColumn.DisplayIndexWithFiller != -1)
                {
                    this.ColumnsInternal.DisplayIndexMap.Remove(targetColumn.Index);
                }
                this.ColumnsInternal.DisplayIndexMap.Insert(newDisplayIndex, targetColumn.Index);
            }
            finally
            {
                InDisplayIndexAdjustments = false;
            }

            // Note that displayIndex of moved column is updated by caller.
        }

        internal void OnColumnBindingChanged(DataGridBoundColumn column)
        {
            // Update Binding in Displayed rows by regenerating the affected elements
            if (_rowsPresenter != null)
            {
                foreach (DataGridRow row in GetAllRows())
                {
                    PopulateCellContent(false /*isCellEdited*/, column, row, row.Cells[column.Index]);
                }
            }
        }
        
        internal void OnColumnElementStyleChanged(DataGridBoundColumn column)
        {
            // Update Element Style in Displayed rows
            foreach (DataGridRow row in GetAllRows())
            {
                FrameworkElement element = column.GetCellContent(row);
                if (element != null)
                {
                    element.SetStyleWithType(column.ElementStyle);
                }
            }
            InvalidateRowHeightEstimate();
        }

        internal void OnColumnHeaderDragStarted(DragStartedEventArgs e)
        {
            if (this.ColumnHeaderDragStarted != null)
            {
                this.ColumnHeaderDragStarted(this, e);
            }
        }

        internal void OnColumnHeaderDragDelta(DragDeltaEventArgs e)
        {
            if (this.ColumnHeaderDragDelta != null)
            {
                this.ColumnHeaderDragDelta(this, e);
            }
        }

        internal void OnColumnHeaderDragCompleted(DragCompletedEventArgs e)
        {
            if (this.ColumnHeaderDragCompleted != null)
            {
                this.ColumnHeaderDragCompleted(this, e);
            }
        }

        internal void OnColumnReadOnlyStateChanging(DataGridColumn dataGridColumn, bool isReadOnly)
        {
            Debug.Assert(dataGridColumn != null);
            if (isReadOnly && this.CurrentColumnIndex == dataGridColumn.Index)
            {
                // Edited column becomes read-only. Exit editing mode.
                if (!EndCellEdit(DataGridEditAction.Commit, true /*exitEditingMode*/, this.ContainsFocus /*keepFocus*/, true /*raiseEvents*/))
                {
                    EndCellEdit(DataGridEditAction.Cancel, true /*exitEditingMode*/, this.ContainsFocus /*keepFocus*/, false /*raiseEvents*/);
                }
            }
        }

        internal void OnColumnVisibleStateChanged(DataGridColumn updatedColumn)
        {
            Debug.Assert(updatedColumn != null);

            CorrectColumnFrozenStates();
            UpdateDisplayedColumns();
            EnsureRowsPresenterVisibility();
            EnsureHorizontalLayout();
            InvalidateColumnHeadersMeasure();

            if (updatedColumn.Visibility == Visibility.Visible && 
                this.ColumnsInternal.VisibleColumnCount == 1 && this.CurrentColumnIndex == -1)
            {
                Debug.Assert(this.SelectedIndex == this.DataConnection.IndexOf(this.SelectedItem));
                if (this.SelectedIndex != -1)
                {
                    SetAndSelectCurrentCell(updatedColumn.Index, this.SelectedIndex, true /*forceCurrentCellSelection*/);
                }
                else
                {
                    MakeFirstDisplayedCellCurrentCell();
                }
            }

            // We need to explicitly collapse the cells of the invisible column because layout only goes through
            // visible ones
            if (updatedColumn.Visibility == Visibility.Collapsed)
            {
                foreach (DataGridRow row in GetAllRows())
                {
                    row.Cells[updatedColumn.Index].Visibility = Visibility.Collapsed;
                }
            }
        }

        internal void OnColumnVisibleStateChanging(DataGridColumn targetColumn)
        {
            Debug.Assert(targetColumn != null);

            if (targetColumn.Visibility == Visibility.Visible && 
                this.CurrentColumn == targetColumn)
            {
                // Column of the current cell is made invisible. Trying to move the current cell to a neighbor column. May throw an exception.
                DataGridColumn dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(targetColumn);
                if (dataGridColumn == null)
                {
                    dataGridColumn = this.ColumnsInternal.GetPreviousVisibleNonFillerColumn(targetColumn);
                }
                if (dataGridColumn == null)
                {
                    SetCurrentCellCore(-1, -1);
                }
                else
                {
                    SetCurrentCellCore(dataGridColumn.Index, this.CurrentSlot);
                }
            }
        }

        internal void OnColumnWidthChanged(DataGridColumn updatedColumn)
        {
            Debug.Assert(updatedColumn != null);
            if (updatedColumn.Visibility == Visibility.Visible)
            {
                EnsureHorizontalLayout();
            }
        }

        internal void OnFillerColumnWidthNeeded(double finalWidth)
        {
            DataGridFillerColumn fillerColumn = this.ColumnsInternal.FillerColumn;
            double totalColumnsWidth = this.ColumnsInternal.VisibleEdgedColumnsWidth;
            if (finalWidth > totalColumnsWidth)
            {
                fillerColumn.FillerWidth = finalWidth - totalColumnsWidth;
            }
            else
            {
                fillerColumn.FillerWidth = 0;
            }
        }

        internal void OnInsertedColumn_PostNotification(DataGridCellCoordinates newCurrentCellCoordinates, int newDisplayIndex)
        {
            // Update current cell if needed
            if (newCurrentCellCoordinates.ColumnIndex != -1)
            {
                Debug.Assert(this.CurrentColumnIndex == -1);
                SetAndSelectCurrentCell(newCurrentCellCoordinates.ColumnIndex, 
                                        newCurrentCellCoordinates.Slot, 
                                        this.ColumnsInternal.VisibleColumnCount == 1 /*forceCurrentCellSelection*/);

                if (newDisplayIndex < this.FrozenColumnCountWithFiller)
                {
                    CorrectColumnFrozenStates();
                }
            }
        }

        internal void OnInsertedColumn_PreNotification(DataGridColumn insertedColumn)
        {
            // Fix the Index of all following columns
            CorrectColumnIndexesAfterInsertion(insertedColumn, 1);

            Debug.Assert(insertedColumn.Index >= 0);
            Debug.Assert(insertedColumn.Index < this.ColumnsItemsInternal.Count);
            Debug.Assert(insertedColumn.OwningGrid == this);

            CorrectColumnDisplayIndexesAfterInsertion(insertedColumn);

            InsertDisplayedColumnHeader(insertedColumn);

            // Insert the missing data cells
            if (this.SlotCount > 0)
            {
                int newColumnCount = this.ColumnsItemsInternal.Count;

                foreach (DataGridRow row in GetAllRows())
                {
                    if (row.Cells.Count < newColumnCount)
                    {
                        AddNewCellPrivate(row, insertedColumn);
                    }
                }
            }

            if (insertedColumn.Visibility == Visibility.Visible)
            {
                EnsureHorizontalLayout();
            }

            DataGridBoundColumn boundColumn = insertedColumn as DataGridBoundColumn;
            if (boundColumn != null && !boundColumn.IsAutoGenerated)
            {
                boundColumn.SetHeaderFromBinding();
            }
        }

        internal DataGridCellCoordinates OnInsertingColumn(int columnIndexInserted, DataGridColumn insertColumn)
        {
            DataGridCellCoordinates newCurrentCellCoordinates;
            Debug.Assert(insertColumn != null);

            if (insertColumn.OwningGrid != null && insertColumn != this.ColumnsInternal.RowGroupSpacerColumn)
            {
                throw DataGridError.DataGrid.ColumnCannotBeReassignedToDifferentDataGrid();
            }

            // Reset current cell if there is one, no matter the relative position of the columns involved
            if (this.CurrentColumnIndex != -1)
            {
                this._temporarilyResetCurrentCell = true;
                newCurrentCellCoordinates = new DataGridCellCoordinates(columnIndexInserted <= this.CurrentColumnIndex ? this.CurrentColumnIndex + 1 : this.CurrentColumnIndex,
                     this.CurrentSlot);
                ResetCurrentCellCore();
            }
            else
            {
                newCurrentCellCoordinates = new DataGridCellCoordinates(-1, -1);
            }
            return newCurrentCellCoordinates;
        }

        internal void OnRemovedColumn_PostNotification(DataGridCellCoordinates newCurrentCellCoordinates)
        {
            // Update current cell if needed
            if (newCurrentCellCoordinates.ColumnIndex != -1)
            {
                Debug.Assert(this.CurrentColumnIndex == -1);
                SetAndSelectCurrentCell(newCurrentCellCoordinates.ColumnIndex, newCurrentCellCoordinates.Slot, false /*forceCurrentCellSelection*/);
            }
        }

        internal void OnRemovedColumn_PreNotification(DataGridColumn removedColumn)
        {
            Debug.Assert(removedColumn.Index >= 0);
            Debug.Assert(removedColumn.OwningGrid == null);

            // Intentionally keep the DisplayIndex intact after detaching the column.
            CorrectColumnIndexesAfterDeletion(removedColumn);

            CorrectColumnDisplayIndexesAfterDeletion(removedColumn);

            // If the detached column was frozen, a new column needs to take its place
            if (removedColumn.IsFrozen)
            {
                removedColumn.IsFrozen = false;
                CorrectColumnFrozenStates();
            }

            UpdateDisplayedColumns();

            // Fix the existing rows by removing cells at correct index
            int newColumnCount = this.ColumnsItemsInternal.Count;

            if (this._rowsPresenter != null)
            {
                foreach (DataGridRow row in GetAllRows())
                {
                    if (row.Cells.Count > newColumnCount)
                    {
                        row.Cells.RemoveAt(removedColumn.Index);
                    }
                }
                _rowsPresenter.InvalidateArrange();
            }

            RemoveDisplayedColumnHeader(removedColumn);
        }

        internal DataGridCellCoordinates OnRemovingColumn(DataGridColumn dataGridColumn)
        {
            Debug.Assert(dataGridColumn != null);
            Debug.Assert(dataGridColumn.Index >= 0 && dataGridColumn.Index < this.ColumnsItemsInternal.Count);

            DataGridCellCoordinates newCurrentCellCoordinates;

            this._temporarilyResetCurrentCell = false;
            int columnIndex = dataGridColumn.Index;

            // Reset the current cell's address if there is one.
            if (this.CurrentColumnIndex != -1)
            {
                int newCurrentColumnIndex = this.CurrentColumnIndex;
                if (columnIndex == newCurrentColumnIndex)
                {
                    DataGridColumn dataGridColumnNext = this.ColumnsInternal.GetNextVisibleColumn(this.ColumnsItemsInternal[columnIndex]);
                    if (dataGridColumnNext != null)
                    {
                        if (dataGridColumnNext.Index > columnIndex)
                        {
                            newCurrentColumnIndex = dataGridColumnNext.Index - 1;
                        }
                        else
                        {
                            newCurrentColumnIndex = dataGridColumnNext.Index;
                        }
                    }
                    else
                    {
                        DataGridColumn dataGridColumnPrevious = this.ColumnsInternal.GetPreviousVisibleNonFillerColumn(this.ColumnsItemsInternal[columnIndex]);
                        if (dataGridColumnPrevious != null)
                        {
                            if (dataGridColumnPrevious.Index > columnIndex)
                            {
                                newCurrentColumnIndex = dataGridColumnPrevious.Index - 1;
                            }
                            else
                            {
                                newCurrentColumnIndex = dataGridColumnPrevious.Index;
                            }
                        }
                        else
                        {
                            newCurrentColumnIndex = -1;
                        }
                    }
                }
                else if (columnIndex < newCurrentColumnIndex)
                {
                    newCurrentColumnIndex--;
                }
                newCurrentCellCoordinates = new DataGridCellCoordinates(newCurrentColumnIndex, (newCurrentColumnIndex == -1) ? -1 : this.CurrentSlot);
                if (columnIndex == this.CurrentColumnIndex)
                {
                    // If the commit fails, force a cancel edit
                    if (!this.CommitEdit(DataGridEditingUnit.Row, false /*exitEditingMode*/))
                    {
                        this.CancelEdit(DataGridEditingUnit.Row, false /*raiseEvents*/);
                    }
                    bool success = this.SetCurrentCellCore(-1, -1);
                    Debug.Assert(success);
                }
                else
                {
                    // Underlying data of deleted column is gone. It cannot be accessed anymore.
                    // Do not end editing mode so that CellValidation doesn't get raised, since that event needs the current formatted value.
                    this._temporarilyResetCurrentCell = true;
                    bool success = SetCurrentCellCore(-1, -1);
                    Debug.Assert(success);
                }
            }
            else
            {
                newCurrentCellCoordinates = new DataGridCellCoordinates(-1, -1);
            }

            // If the last column is removed, delete all the rows first.
            if (this.ColumnsItemsInternal.Count == 1)
            {
                ClearRows(false);
            }

            // Is deleted column scrolled off screen?
            if (dataGridColumn.Visibility == Visibility.Visible &&
                !dataGridColumn.IsFrozen &&
                this.DisplayData.FirstDisplayedScrollingCol >= 0)
            {
                // Deleted column is part of scrolling columns.
                if (this.DisplayData.FirstDisplayedScrollingCol == dataGridColumn.Index)
                {
                    // Deleted column is first scrolling column
                    this._horizontalOffset -= this._negHorizontalOffset;
                    this._negHorizontalOffset = 0;
                }
                else if (!this.ColumnsInternal.DisplayInOrder(this.DisplayData.FirstDisplayedScrollingCol, dataGridColumn.Index))
                {
                    // Deleted column is displayed before first scrolling column
                    Debug.Assert(this._horizontalOffset >= GetEdgedColumnWidth(dataGridColumn));
                    this._horizontalOffset -= GetEdgedColumnWidth(dataGridColumn);
                }

                if (this._hScrollBar != null && this._hScrollBar.Visibility == Visibility.Visible) // 
                {
                    this._hScrollBar.Value = this._horizontalOffset;
                }
            }
            
            return newCurrentCellCoordinates;
        }

        /// <summary>
        /// Called when a column property changes, and its cells need to 
        /// adjust that that column change.
        /// </summary>
        internal void RefreshColumnElements(DataGridColumn dataGridColumn, string propertyName)
        {
            Debug.Assert(dataGridColumn != null);

            // Take care of the non-displayed loaded rows
            for (int index = 0; index < this._loadedRows.Count; )
            {
                DataGridRow dataGridRow = this._loadedRows[index];
                Debug.Assert(dataGridRow != null);
                if (!this.IsSlotVisible(dataGridRow.Slot))
                {
                    RefreshCellElement(dataGridColumn, dataGridRow, propertyName);
                }
                index++;
            }

            // Take care of the displayed rows
            if (this._rowsPresenter != null)
            {
                foreach (DataGridRow row in GetAllRows())
                {
                    RefreshCellElement(dataGridColumn, row, propertyName);
                }
                // This update could change layout so we need to update our estimate and invalidate
                InvalidateRowHeightEstimate();
                InvalidateMeasure();
            }
        }

        #endregion Internal Methods
        
        #region Private Methods

        private bool AddGeneratedColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            // Raise the AutoGeneratingColumn event in case the user wants to Cancel or Replace the
            // column being generated
            OnAutoGeneratingColumn(e);
            if (e.Cancel)
            {
                return false;
            }
            else
            {
                this.ColumnsInternal.Add(e.Column);
                this.ColumnsInternal.AutogeneratedColumnCount++;
                return true;
            }
        }

        private void AutoGenerateColumnsPrivate()
        {
            if (!_measured || (_autoGeneratingColumnOperationCount > 0))
            {
                // Reading the DataType when we generate columns could cause the CollectionView to 
                // raise a Reset if its Enumeration changed.  In that case, we don't want to generate again.
                return;
            }
            
            _autoGeneratingColumnOperationCount++;
            try
            {
                // Always remove existing autogenerated columns before generating new ones
                RemoveAutoGeneratedColumns();
                GenerateColumnsFromProperties();
                EnsureRowsPresenterVisibility();
                InvalidateRowHeightEstimate();
            }
            finally
            {
                _autoGeneratingColumnOperationCount--;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private bool ComputeDisplayedColumns()
        {
            bool invalidate = false;
            int numVisibleScrollingCols = 0;
            int visibleScrollingColumnsTmp = 0;
            double displayWidth = this.CellsWidth;
            double cx = 0;
            int firstDisplayedFrozenCol = -1;
            int firstDisplayedScrollingCol = this.DisplayData.FirstDisplayedScrollingCol;

            // the same problem with negative numbers:
            // if the width passed in is negative, then return 0
            if (displayWidth <= 0 || this.ColumnsInternal.VisibleColumnCount == 0)
            {
                this.DisplayData.FirstDisplayedScrollingCol = -1;
                this.DisplayData.LastTotallyDisplayedScrollingCol = -1;
                return invalidate;
            }

            foreach (DataGridColumn dataGridColumn in this.ColumnsInternal.GetVisibleFrozenColumns())
            {
                if (firstDisplayedFrozenCol == -1)
                {
                    firstDisplayedFrozenCol = dataGridColumn.Index;
                }
                cx += GetEdgedColumnWidth(dataGridColumn);
                if (cx >= displayWidth)
                {
                    break;
                }
            }

            Debug.Assert(cx <= this.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth());

            if (cx < displayWidth && firstDisplayedScrollingCol >= 0)
            {
                DataGridColumn dataGridColumn = this.ColumnsItemsInternal[firstDisplayedScrollingCol];
                if (dataGridColumn.IsFrozen)
                {
                    dataGridColumn = this.ColumnsInternal.FirstVisibleScrollingColumn;
                    this._negHorizontalOffset = 0;
                    if (dataGridColumn == null)
                    {
                        this.DisplayData.FirstDisplayedScrollingCol = this.DisplayData.LastTotallyDisplayedScrollingCol = -1;
                        return invalidate;
                    }
                    else
                    {
                        firstDisplayedScrollingCol = dataGridColumn.Index;
                    }
                }

                cx -= this._negHorizontalOffset;
                while (cx < displayWidth && dataGridColumn != null)
                {
                    cx += GetEdgedColumnWidth(dataGridColumn);
                    visibleScrollingColumnsTmp++;
                    dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(dataGridColumn);
                }
                numVisibleScrollingCols = visibleScrollingColumnsTmp;

                // if we inflate the data area then we paint columns to the left of firstDisplayedScrollingCol
                if (cx < displayWidth)
                {
                    Debug.Assert(firstDisplayedScrollingCol >= 0);
                    //first minimize value of this._negHorizontalOffset
                    if (this._negHorizontalOffset > 0)
                    {
                        invalidate = true;
                        if (displayWidth - cx > this._negHorizontalOffset)
                        {
                            cx += this._negHorizontalOffset;
                            this._horizontalOffset -= this._negHorizontalOffset;
                            this._negHorizontalOffset = 0;
                        }
                        else
                        {
                            this._horizontalOffset -= displayWidth - cx;
                            this._negHorizontalOffset -= displayWidth - cx;
                            cx = displayWidth;
                        }
                    }
                    // second try to scroll entire columns
                    if (cx < displayWidth && this._horizontalOffset > 0)
                    {
                        Debug.Assert(this._negHorizontalOffset == 0);
                        dataGridColumn = this.ColumnsInternal.GetPreviousVisibleScrollingColumn(this.ColumnsItemsInternal[firstDisplayedScrollingCol]);
                        while (dataGridColumn != null && cx + GetEdgedColumnWidth(dataGridColumn) <= displayWidth)
                        {
                            cx += GetEdgedColumnWidth(dataGridColumn);
                            visibleScrollingColumnsTmp++;
                            invalidate = true;
                            firstDisplayedScrollingCol = dataGridColumn.Index;
                            this._horizontalOffset -= GetEdgedColumnWidth(dataGridColumn);
                            dataGridColumn = this.ColumnsInternal.GetPreviousVisibleScrollingColumn(dataGridColumn);
                        }
                    }
                    // third try to partially scroll in first scrolled off column
                    if (cx < displayWidth && this._horizontalOffset > 0)
                    {
                        Debug.Assert(this._negHorizontalOffset == 0);
                        dataGridColumn = this.ColumnsInternal.GetPreviousVisibleScrollingColumn(this.ColumnsItemsInternal[firstDisplayedScrollingCol]);
                        Debug.Assert(dataGridColumn != null);
                        Debug.Assert(GetEdgedColumnWidth(dataGridColumn) > displayWidth - cx);
                        firstDisplayedScrollingCol = dataGridColumn.Index;
                        this._negHorizontalOffset = GetEdgedColumnWidth(dataGridColumn) - displayWidth + cx;
                        this._horizontalOffset -= displayWidth - cx;
                        visibleScrollingColumnsTmp++;
                        invalidate = true;
                        cx = displayWidth;
                        Debug.Assert(this._negHorizontalOffset == GetNegHorizontalOffsetFromHorizontalOffset(this._horizontalOffset));
                    }

                    // update the number of visible columns to the new reality
                    Debug.Assert(numVisibleScrollingCols <= visibleScrollingColumnsTmp, "the number of displayed columns can only grow");
                    numVisibleScrollingCols = visibleScrollingColumnsTmp;
                }

                int jumpFromFirstVisibleScrollingCol = numVisibleScrollingCols - 1;
                if (cx > displayWidth)
                {
                    jumpFromFirstVisibleScrollingCol--;
                }

                Debug.Assert(jumpFromFirstVisibleScrollingCol >= -1);

                if (jumpFromFirstVisibleScrollingCol < 0)
                {
                    this.DisplayData.LastTotallyDisplayedScrollingCol = -1; // no totally visible scrolling column at all
                }
                else
                {
                    Debug.Assert(firstDisplayedScrollingCol >= 0);
                    dataGridColumn = this.ColumnsItemsInternal[firstDisplayedScrollingCol];
                    for (int jump = 0; jump < jumpFromFirstVisibleScrollingCol; jump++)
                    {
                        dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(dataGridColumn);
                        Debug.Assert(dataGridColumn != null);
                    }
                    this.DisplayData.LastTotallyDisplayedScrollingCol = dataGridColumn.Index;
                }
            }
            else
            {
                this.DisplayData.LastTotallyDisplayedScrollingCol = -1;
            }
            this.DisplayData.FirstDisplayedScrollingCol = firstDisplayedScrollingCol;

            return invalidate;
        }

        private int ComputeFirstVisibleScrollingColumn()
        {
            if (this.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth() >= this.CellsWidth)
            {
                // Not enough room for scrolling columns.
                this._negHorizontalOffset = 0;
                return -1;
            }

            DataGridColumn dataGridColumn = this.ColumnsInternal.FirstVisibleScrollingColumn;

            if (this._horizontalOffset == 0)
            {
                this._negHorizontalOffset = 0;
                return (dataGridColumn == null) ? -1 : dataGridColumn.Index;
            }

            double cx = 0;
            while (dataGridColumn != null)
            {
                cx += GetEdgedColumnWidth(dataGridColumn);
                if (cx > this._horizontalOffset)
                {
                    break;
                }
                dataGridColumn = this.ColumnsInternal.GetNextVisibleColumn(dataGridColumn);
            }

            if (dataGridColumn == null)
            {
                Debug.Assert(cx <= this._horizontalOffset);
                dataGridColumn = this.ColumnsInternal.FirstVisibleScrollingColumn;
                if (dataGridColumn == null)
                {
                    this._negHorizontalOffset = 0;
                    return -1;
                }
                else
                {
                    if (this._negHorizontalOffset != this._horizontalOffset)
                    {
                        this._negHorizontalOffset = 0;
                    }
                    return dataGridColumn.Index;
                }
            }
            else
            {
                this._negHorizontalOffset = GetEdgedColumnWidth(dataGridColumn) - (cx - this._horizontalOffset);
                return dataGridColumn.Index;
            }
        }

        private void CorrectColumnDisplayIndexesAfterDeletion(DataGridColumn deletedColumn)
        {
            // Column indexes have already been adjusted.
            // This column has already been detached and has retained its old Index and DisplayIndex

            Debug.Assert(deletedColumn != null);
            Debug.Assert(deletedColumn.OwningGrid == null);
            Debug.Assert(deletedColumn.Index >= 0);
            Debug.Assert(deletedColumn.DisplayIndexWithFiller >= 0);

            try
            {
                InDisplayIndexAdjustments = true;

                // The DisplayIndex of columns greater than the deleted column need to be decremented,
                // as do the DisplayIndexMap values of modified column Indexes
                DataGridColumn column;
                this.ColumnsInternal.DisplayIndexMap.RemoveAt(deletedColumn.DisplayIndexWithFiller);
                for (int displayIndex = 0; displayIndex < this.ColumnsInternal.DisplayIndexMap.Count; displayIndex++ )
                {
                    if (this.ColumnsInternal.DisplayIndexMap[displayIndex] > deletedColumn.Index)
                    {
                        this.ColumnsInternal.DisplayIndexMap[displayIndex]--;
                    }
                    if (displayIndex >= deletedColumn.DisplayIndexWithFiller)
                    {
                        column = this.ColumnsInternal.GetColumnAtDisplayIndex(displayIndex);
                        column.DisplayIndexWithFiller = column.DisplayIndexWithFiller - 1;
                        column.DisplayIndexHasChanged = true; // OnColumnDisplayIndexChanged needs to be raised later on
                    }
                }

#if DEBUG
                Debug.Assert(this.ColumnsInternal.Debug_VerifyColumnDisplayIndexes());
#endif 
                // Now raise all the OnColumnDisplayIndexChanged events
                FlushDisplayIndexChanged(true /*raiseEvent*/);
            }
            finally
            {
                InDisplayIndexAdjustments = false;
                FlushDisplayIndexChanged(false /*raiseEvent*/);
            }
        }

        private void CorrectColumnDisplayIndexesAfterInsertion(DataGridColumn insertedColumn)
        {
            Debug.Assert(insertedColumn != null);
            Debug.Assert(insertedColumn.OwningGrid == this);
            if (insertedColumn.DisplayIndexWithFiller == -1 || insertedColumn.DisplayIndexWithFiller >= this.ColumnsItemsInternal.Count)
            {
                // Developer did not assign a DisplayIndex or picked a large number.
                // Choose the Index as the DisplayIndex.
                insertedColumn.DisplayIndexWithFiller = insertedColumn.Index;
            }

            try
            {
                InDisplayIndexAdjustments = true;

                // The DisplayIndex of columns greater than the inserted column need to be incremented,
                // as do the DisplayIndexMap values of modified column Indexes
                DataGridColumn column;
                for (int displayIndex = 0; displayIndex < this.ColumnsInternal.DisplayIndexMap.Count; displayIndex++)
                {
                    if (this.ColumnsInternal.DisplayIndexMap[displayIndex] >= insertedColumn.Index)
                    {
                        this.ColumnsInternal.DisplayIndexMap[displayIndex]++;
                    }
                    if (displayIndex >= insertedColumn.DisplayIndexWithFiller)
                    {
                        column = this.ColumnsInternal.GetColumnAtDisplayIndex(displayIndex);
                        column.DisplayIndexWithFiller++;
                        column.DisplayIndexHasChanged = true; // OnColumnDisplayIndexChanged needs to be raised later on
                    }
                }
                this.ColumnsInternal.DisplayIndexMap.Insert(insertedColumn.DisplayIndexWithFiller, insertedColumn.Index);

#if DEBUG
                Debug.Assert(this.ColumnsInternal.Debug_VerifyColumnDisplayIndexes());
#endif
                // Now raise all the OnColumnDisplayIndexChanged events
                FlushDisplayIndexChanged(true /*raiseEvent*/);
            }
            finally
            {
                InDisplayIndexAdjustments = false;
                FlushDisplayIndexChanged(false /*raiseEvent*/);
            }
        }

        private void CorrectColumnFrozenStates()
        {
            int index = 0;
            double frozenColumnWidth = 0;
            double oldFrozenColumnWidth = 0;
            foreach (DataGridColumn column in this.ColumnsInternal.GetDisplayedColumns())
            {
                if (column.IsFrozen)
                {
                    oldFrozenColumnWidth += column.ActualWidth;
                }
                column.IsFrozen = index < this.FrozenColumnCountWithFiller;
                if (column.IsFrozen)
                {
                    frozenColumnWidth += column.ActualWidth;
                }
                index++;
            }
            if (this.HorizontalOffset > Math.Max(0, frozenColumnWidth - oldFrozenColumnWidth))
            {
                UpdateHorizontalOffset(this.HorizontalOffset - frozenColumnWidth + oldFrozenColumnWidth);
            }
            else
            {
                UpdateHorizontalOffset(0);
            }
        }

        private void CorrectColumnIndexesAfterDeletion(DataGridColumn deletedColumn)
        {
            Debug.Assert(deletedColumn != null);
            for (int columnIndex = deletedColumn.Index; columnIndex < this.ColumnsItemsInternal.Count; columnIndex++)
            {
                this.ColumnsItemsInternal[columnIndex].Index = this.ColumnsItemsInternal[columnIndex].Index - 1;
                Debug.Assert(this.ColumnsItemsInternal[columnIndex].Index == columnIndex);
            }
        }

        private void CorrectColumnIndexesAfterInsertion(DataGridColumn insertedColumn, int insertionCount)
        {
            Debug.Assert(insertedColumn != null);
            Debug.Assert(insertionCount > 0);
            for (int columnIndex = insertedColumn.Index + insertionCount; columnIndex < this.ColumnsItemsInternal.Count; columnIndex++)
            {
                this.ColumnsItemsInternal[columnIndex].Index = columnIndex;
            }
        }

        private void FlushDisplayIndexChanged(bool raiseEvent)
        {
            foreach (DataGridColumn column in this.ColumnsItemsInternal)
            {
                if (column.DisplayIndexHasChanged)
                {
                    column.DisplayIndexHasChanged = false;
                    if (raiseEvent)
                    {
                        Debug.Assert(column != this.ColumnsInternal.RowGroupSpacerColumn);
                        OnColumnDisplayIndexChanged(column);
                    }
                }
            }
        }

        private static DataGridAutoGeneratingColumnEventArgs GenerateColumn(Type propertyType, string propertyName, string header)
        {
            // Create a new DataBoundColumn for the Property
            DataGridBoundColumn newColumn = GetDataGridColumnFromType(propertyType);
            newColumn.Binding = new Binding(propertyName);
            newColumn.Header = header;
            newColumn.IsAutoGenerated = true;
            return new DataGridAutoGeneratingColumnEventArgs(propertyName, propertyType, newColumn);
        }

        private void GenerateColumnsFromProperties()
        {
            // Autogenerated Columns are added at the end so the user columns appear first
            if (this.DataConnection.DataProperties != null && this.DataConnection.DataProperties.Length > 0)
            {
                List<KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs>> columnOrderPairs = new List<KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs>>();

                // Generate the columns
                foreach (PropertyInfo propertyInfo in this.DataConnection.DataProperties)
                {
                    string columnHeader = propertyInfo.Name;
                    int columnOrder = DATAGRID_defaultColumnDisplayOrder;

                    // Check if DisplayAttribute is defined on the property
                    object[] attributes = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
                    if (attributes != null && attributes.Length > 0)
                    {
                        DisplayAttribute displayAttribute = attributes[0] as DisplayAttribute;
                        Debug.Assert(displayAttribute != null);

                        bool? autoGenerateField = displayAttribute.GetAutoGenerateField();
                        if (autoGenerateField.HasValue && autoGenerateField.Value == false)
                        {
                            // Abort column generation because we aren't supposed to auto-generate this field
                            continue;
                        }

                        string header = displayAttribute.GetShortName();
                        if (header != null)
                        {
                            columnHeader = header;
                        }

                        int? order = displayAttribute.GetOrder();
                        if (order.HasValue)
                        {
                            columnOrder = order.Value;
                        }
                    }

                    // Generate a single column and determine its relative order
                    int insertIndex = 0;
                    if (columnOrder == int.MaxValue)
                    {
                        insertIndex = columnOrderPairs.Count;
                    }
                    else
                    {
                        foreach (KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs> columnOrderPair in columnOrderPairs)
                        {
                            if (columnOrderPair.Key > columnOrder)
                            {
                                break;
                            }
                            insertIndex++;
                        }
                    }
                    DataGridAutoGeneratingColumnEventArgs columnArgs = GenerateColumn(propertyInfo.PropertyType, propertyInfo.Name, columnHeader);
                    columnOrderPairs.Insert(insertIndex, new KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs>(columnOrder, columnArgs));
                }

                // Add the columns to the DataGrid in the correct order
                foreach (KeyValuePair<int, DataGridAutoGeneratingColumnEventArgs> columnOrderPair in columnOrderPairs)
                {
                    AddGeneratedColumn(columnOrderPair.Value);
                }
            }
            else if (this.DataConnection.DataIsPrimitive)
            {
                AddGeneratedColumn(GenerateColumn(this.DataConnection.DataType, string.Empty, this.DataConnection.DataType.Name));
            }
        }

        private bool GetColumnEffectiveReadOnlyState(DataGridColumn dataGridColumn)
        {
            Debug.Assert(dataGridColumn != null);

            return this.IsReadOnly || dataGridColumn.IsReadOnly || dataGridColumn is DataGridFillerColumn;
        }

        /// <devdoc>
        ///      Returns the absolute coordinate of the left edge of the given column (including
        ///      the potential gridline - that is the left edge of the gridline is returned). Note that
        ///      the column does not need to be in the display area.
        /// </devdoc>
        private double GetColumnXFromIndex(int index)
        {
            Debug.Assert(index < this.ColumnsItemsInternal.Count);
            Debug.Assert(this.ColumnsItemsInternal[index].Visibility == Visibility.Visible);

            double x = 0;
            foreach (DataGridColumn column in this.ColumnsInternal.GetVisibleColumns())
            {
                if (index == column.Index)
                {
                    break;
                }
                x += GetEdgedColumnWidth(column);
            }
            return x;
        }

        private static DataGridBoundColumn GetDataGridColumnFromType(Type type)
        {
            Debug.Assert(type != null);
            if (type == typeof(bool))
            {
                return new DataGridCheckBoxColumn();
            }
            else if (type == typeof(bool?))
            {
                DataGridCheckBoxColumn column = new DataGridCheckBoxColumn();
                column.IsThreeState = true;
                return column;
            }
            return new DataGridTextColumn();
        }

        private double GetNegHorizontalOffsetFromHorizontalOffset(double horizontalOffset)
        {
            foreach (DataGridColumn column in this.ColumnsInternal.GetVisibleScrollingColumns())
            {
                if (GetEdgedColumnWidth(column) > horizontalOffset)
                {
                    break;
                }
                horizontalOffset -= GetEdgedColumnWidth(column);
            }
            return horizontalOffset;
        }

        private void InsertDisplayedColumnHeader(DataGridColumn dataGridColumn)
        {
            Debug.Assert(dataGridColumn != null);
            if (_columnHeadersPresenter != null)
            {
                dataGridColumn.HeaderCell.Visibility = dataGridColumn.Visibility;
                Debug.Assert(!this._columnHeadersPresenter.Children.Contains(dataGridColumn.HeaderCell));
                _columnHeadersPresenter.Children.Insert(dataGridColumn.DisplayIndexWithFiller, dataGridColumn.HeaderCell);
            }
        }

        private static void RefreshCellElement(DataGridColumn dataGridColumn, DataGridRow dataGridRow, string propertyName)
        {
            Debug.Assert(dataGridColumn != null);
            Debug.Assert(dataGridRow != null);

            DataGridCell dataGridCell = dataGridRow.Cells[dataGridColumn.Index];
            Debug.Assert(dataGridCell != null);
            FrameworkElement element = dataGridCell.Content as FrameworkElement;
            if (element != null)
            {
                dataGridColumn.RefreshCellContent(element, propertyName);
            }
        }

        private void RemoveAutoGeneratedColumns()
        {
            int index = 0;
            _autoGeneratingColumnOperationCount++;
            try
            {
                while (index < this.ColumnsInternal.Count)
                {
                    // Skip over the user columns
                    while (index < this.ColumnsInternal.Count && !this.ColumnsInternal[index].IsAutoGenerated)
                    {
                        index++;
                    }
                    // Remove the autogenerated columns
                    while (index < this.ColumnsInternal.Count && this.ColumnsInternal[index].IsAutoGenerated)
                    {
                        this.ColumnsInternal.RemoveAt(index);
                    }
                }
                this.ColumnsInternal.AutogeneratedColumnCount = 0;
            }
            finally
            {
                _autoGeneratingColumnOperationCount--;
            }
        }

        private bool ScrollColumnIntoView(int columnIndex)
        {
            Debug.Assert(columnIndex >= 0 && columnIndex < this.ColumnsItemsInternal.Count);

            if (this.DisplayData.FirstDisplayedScrollingCol != -1 &&
                !this.ColumnsItemsInternal[columnIndex].IsFrozen &&
                (columnIndex != this.DisplayData.FirstDisplayedScrollingCol || this._negHorizontalOffset > 0))
            {
                int columnsToScroll;
                if (this.ColumnsInternal.DisplayInOrder(columnIndex, this.DisplayData.FirstDisplayedScrollingCol))
                {
                    columnsToScroll = this.ColumnsInternal.GetColumnCount(true /* isVisible */, false /* isFrozen */, columnIndex, this.DisplayData.FirstDisplayedScrollingCol);
                    if (this._negHorizontalOffset > 0)
                    {
                        columnsToScroll++;
                    }
                    ScrollColumns(-columnsToScroll);
                }
                else if (columnIndex == this.DisplayData.FirstDisplayedScrollingCol && this._negHorizontalOffset > 0)
                {
                    ScrollColumns(-1);
                }
                else if (this.DisplayData.LastTotallyDisplayedScrollingCol == -1 ||
                         (this.DisplayData.LastTotallyDisplayedScrollingCol != columnIndex &&
                          this.ColumnsInternal.DisplayInOrder(this.DisplayData.LastTotallyDisplayedScrollingCol, columnIndex)))
                {
                    double xColumnLeftEdge = GetColumnXFromIndex(columnIndex);
                    double xColumnRightEdge = xColumnLeftEdge + GetEdgedColumnWidth(this.ColumnsItemsInternal[columnIndex]);
                    double change = xColumnRightEdge - this.HorizontalOffset - this.CellsWidth;
                    double widthRemaining = change;

                    DataGridColumn newFirstDisplayedScrollingCol = this.ColumnsItemsInternal[this.DisplayData.FirstDisplayedScrollingCol];
                    DataGridColumn nextColumn = this.ColumnsInternal.GetNextVisibleColumn(newFirstDisplayedScrollingCol);
                    double newColumnWidth = GetEdgedColumnWidth(newFirstDisplayedScrollingCol) - this._negHorizontalOffset;
                    while (nextColumn != null && widthRemaining >= newColumnWidth)
                    {
                        widthRemaining -= newColumnWidth;
                        newFirstDisplayedScrollingCol = nextColumn;
                        newColumnWidth = GetEdgedColumnWidth(newFirstDisplayedScrollingCol);
                        nextColumn = this.ColumnsInternal.GetNextVisibleColumn(newFirstDisplayedScrollingCol);
                        this._negHorizontalOffset = 0;
                    }
                    this._negHorizontalOffset += widthRemaining;
                    this.DisplayData.LastTotallyDisplayedScrollingCol = columnIndex;
                    if (newFirstDisplayedScrollingCol.Index == columnIndex)
                    {
                        this._negHorizontalOffset = 0;
                        double frozenColumnWidth = this.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();
                        // If the entire column cannot be displayed, we want to start showing it from its LeftEdge
                        if (newColumnWidth > (this.CellsWidth - frozenColumnWidth))
                        {
                            this.DisplayData.LastTotallyDisplayedScrollingCol = -1;
                            change = xColumnLeftEdge - this.HorizontalOffset - frozenColumnWidth;
                        }
                    }
                    this.DisplayData.FirstDisplayedScrollingCol = newFirstDisplayedScrollingCol.Index;
                    
                    // At this point DisplayData.FirstDisplayedScrollingColumn and LastDisplayedScrollingColumn 
                    // should be correct
                    if (change != 0)
                    {
                        UpdateHorizontalOffset(this.HorizontalOffset + change);
                    }
                }
            }
            return true;
        }

        private void ScrollColumns(int columns)
        {
            DataGridColumn newFirstVisibleScrollingCol = null;
            DataGridColumn dataGridColumnTmp;
            int colCount = 0;
            if (columns > 0)
            {
                if (this.DisplayData.LastTotallyDisplayedScrollingCol >= 0)
                {
                    dataGridColumnTmp = this.ColumnsItemsInternal[this.DisplayData.LastTotallyDisplayedScrollingCol];
                    while (colCount < columns && dataGridColumnTmp != null)
                    {
                        dataGridColumnTmp = this.ColumnsInternal.GetNextVisibleColumn(dataGridColumnTmp);
                        colCount++;
                    }

                    if (dataGridColumnTmp == null)
                    {
                        // no more column to display on the right of the last totally seen column
                        return;
                    }
                }
                Debug.Assert(this.DisplayData.FirstDisplayedScrollingCol >= 0);
                dataGridColumnTmp = this.ColumnsItemsInternal[this.DisplayData.FirstDisplayedScrollingCol];
                colCount = 0;
                while (colCount < columns && dataGridColumnTmp != null)
                {
                    dataGridColumnTmp = this.ColumnsInternal.GetNextVisibleColumn(dataGridColumnTmp);
                    colCount++;
                }
                newFirstVisibleScrollingCol = dataGridColumnTmp;
            }

            if (columns < 0)
            {
                Debug.Assert(this.DisplayData.FirstDisplayedScrollingCol >= 0);
                dataGridColumnTmp = this.ColumnsItemsInternal[this.DisplayData.FirstDisplayedScrollingCol];
                if (this._negHorizontalOffset > 0)
                {
                    colCount++;
                }
                while (colCount < -columns && dataGridColumnTmp != null)
                {
                    dataGridColumnTmp = this.ColumnsInternal.GetPreviousVisibleScrollingColumn(dataGridColumnTmp);
                    colCount++;
                }
                newFirstVisibleScrollingCol = dataGridColumnTmp;
                if (newFirstVisibleScrollingCol == null)
                {
                    if (this._negHorizontalOffset == 0)
                    {
                        // no more column to display on the left of the first seen column
                        return;
                    }
                    else
                    {
                        newFirstVisibleScrollingCol = this.ColumnsItemsInternal[this.DisplayData.FirstDisplayedScrollingCol];
                    }
                }
            }

            double newColOffset = 0;
            foreach (DataGridColumn dataGridColumn in this.ColumnsInternal.GetVisibleScrollingColumns())
            {
                if (dataGridColumn == newFirstVisibleScrollingCol)
                {
                    break;
                }
                newColOffset += GetEdgedColumnWidth(dataGridColumn);
            }

            UpdateHorizontalOffset(newColOffset);
        }

        private void UpdateDisplayedColumns()
        {
            this.DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
            ComputeDisplayedColumns();
        }

        #endregion Private Methods
    }
}
