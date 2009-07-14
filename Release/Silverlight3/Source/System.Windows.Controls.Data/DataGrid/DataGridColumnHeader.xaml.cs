// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Represents an individual <see cref="T:System.Windows.Controls.DataGrid" /> column header.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnsorted, GroupName = VisualStates.GroupSort)]
    [TemplateVisualState(Name = VisualStates.StateSortAscending, GroupName = VisualStates.GroupSort)]
    [TemplateVisualState(Name = VisualStates.StateSortDescending, GroupName = VisualStates.GroupSort)]
    public partial class DataGridColumnHeader : ContentControl
    {
        private enum DragMode
        {
            None = 0,
            MouseDown = 1,
            Drag = 2,
            Resize = 3,
            Reorder = 4
        }

        #region Constants

        private const int DATAGRIDCOLUMNHEADER_resizeRegionWidth = 5;
        private const double DATAGRIDCOLUMNHEADER_separatorThickness = 1;

        #endregion Constants

        #region Data

        private bool _autoSizeDropLocationIndicatorHeight;
        private static DragMode _dragMode;
        private static Point? _lastMousePositionGrid;
        private static double _lastResizeWidth;
        private static Cursor _originalCursor;
        private static double _originalHorizontalOffset;
        private static double _originalWidth;
        private Visibility _desiredSeparatorVisibility;
        private static Point? _dragStart;
        private static Point? _dragStartParent;
        private static DataGridColumn _dragColumn;
        private static double _frozenColumnsWidth;
        private static Popup _reorderingThumb;

        #endregion Data

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.Primitives.DataGridColumnHeader" /> class. 
        /// </summary>
        public DataGridColumnHeader()
        {
            this.LostMouseCapture += new MouseEventHandler(DataGridColumnHeader_LostMouseCapture);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(DataGridColumnHeader_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(DataGridColumnHeader_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(DataGridColumnHeader_MouseMove);
            this.MouseEnter += new MouseEventHandler(DataGridColumnHeader_MouseEnter);
            this.MouseLeave += new MouseEventHandler(DataGridColumnHeader_MouseLeave);

            DefaultStyleKey = typeof(DataGridColumnHeader);
        }

        #region Dependency Properties

        #region SeparatorBrush

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush" /> used to paint the column header separator lines. 
        /// </summary>
        public Brush SeparatorBrush
        {
            get { return GetValue(SeparatorBrushProperty) as Brush; }
            set { SetValue(SeparatorBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.Primitives.DataGridColumnHeader.SeparatorBrush" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeparatorBrushProperty = 
            DependencyProperty.Register("SeparatorBrush", 
                typeof(Brush),
                typeof(DataGridColumnHeader), 
                null);

        #endregion SeparatorBrush

        #region SeparatorVisibility

        /// <summary>
        /// Gets or sets a value indicating whether the column header separator lines are visible.
        /// </summary>
        public Visibility SeparatorVisibility
        {
            get { return (Visibility) GetValue(SeparatorVisibilityProperty); }
            set { SetValue(SeparatorVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.Primitives.DataGridColumnHeader.SeparatorVisibility" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeparatorVisibilityProperty =
            DependencyProperty.Register("SeparatorVisibility",
                typeof(Visibility),
                typeof(DataGridColumnHeader),
                new PropertyMetadata(OnSeparatorVisibilityPropertyChanged));

        private static void OnSeparatorVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumnHeader columnHeader = (DataGridColumnHeader)d;
            
            if (!columnHeader.AreHandlersSuspended())
            {
                columnHeader._desiredSeparatorVisibility = (Visibility)e.NewValue;
                if (columnHeader.OwningGrid != null)
                {
                    columnHeader.UpdateSeparatorVisibility(columnHeader.OwningGrid.ColumnsInternal.LastVisibleColumn);
                }
                else
                {
                    columnHeader.UpdateSeparatorVisibility(null);
                }
            }
        }

        #endregion SeparatorVisibility

        #endregion Dependency Properties


        #region Public Properties

        #endregion Public Properties


        #region Protected Properties

        #endregion Protected Properties

        #region Internal Properties

        internal int ColumnIndex
        {
            get
            {
                if (this.OwningColumn == null)
                {
                    return -1;
                }
                return this.OwningColumn.Index;
            }
        }

        internal ListSortDirection? CurrentSortingState
        {
            get;
            private set;
        }

        internal DataGrid OwningGrid
        {
            get
            {
                if (this.OwningColumn != null && this.OwningColumn.OwningGrid != null)
                {
                    return this.OwningColumn.OwningGrid;
                }
                return null;
            }
        }

        internal DataGridColumn OwningColumn
        {
            get;
            set;
        }

        #endregion Internal Properties


        #region Private Properties

        private bool IsMouseOver
        {
            get;
            set;
        }

        private bool IsPressed
        {
            get;
            set;
        }

        #endregion Private Properties


        #region Public Methods

        #endregion Public Methods


        #region Protected Methods

        /// <summary>
        /// Builds the visual tree for the column header when a new template is applied. 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ApplyState(false);
        }
        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ContentControl.Content" /> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="P:System.Windows.Controls.ContentControl.Content" /> property.</param>
        /// <param name="newContent">The new value of the <see cref="P:System.Windows.Controls.ContentControl.Content" /> property.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// <paramref name="newContent" /> is not a UIElement.
        /// </exception>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent is UIElement)
            {
                throw DataGridError.DataGridColumnHeader.ContentDoesNotSupportUIElements();
            }
            base.OnContentChanged(oldContent, newContent);
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            if (this.OwningGrid != null && this.OwningColumn != this.OwningGrid.ColumnsInternal.FillerColumn)
            {
                return new DataGridColumnHeaderAutomationPeer(this);
            }
            return base.OnCreateAutomationPeer();
        }

        #endregion Protected Methods


        #region Internal Methods

        internal void ApplyState(bool useTransitions)
        {
            // Common States
            if (this.IsPressed && DataGridColumnHeader._dragMode != DragMode.Resize)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StatePressed, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else if (this.IsMouseOver && DataGridColumnHeader._dragMode != DragMode.Resize)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            // Sort States
            this.CurrentSortingState = null;
            if (this.OwningGrid != null
                && this.OwningGrid.DataConnection != null
                && this.OwningGrid.DataConnection.AllowSort)
            {
                SortDescription? sort = this.OwningColumn.GetSortDescription();

                if (sort.HasValue)
                {
                    this.CurrentSortingState = sort.Value.Direction;
                    if (this.CurrentSortingState == ListSortDirection.Ascending)
                    {
                        VisualStates.GoToState(this, useTransitions, VisualStates.StateSortAscending, VisualStates.StateUnsorted);
                    }
                    if (this.CurrentSortingState == ListSortDirection.Descending)
                    {
                        VisualStates.GoToState(this, useTransitions, VisualStates.StateSortDescending, VisualStates.StateUnsorted);
                    }
                }
                else
                {
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateUnsorted);
                }
            }
        }

        /// <summary>
        /// Ensures that the correct Style is applied to this object.
        /// </summary>
        /// <param name="previousStyle">Caller's previous associated Style</param>
        internal void EnsureStyle(Style previousStyle)
        {
            if (this.Style != null
                && (this.OwningColumn == null || this.Style != this.OwningColumn.HeaderStyle)
                && (this.OwningGrid == null || this.Style != this.OwningGrid.ColumnHeaderStyle)
                && (this.Style != previousStyle))
            {
                return;
            }

            Style style = null;
            if (this.OwningColumn != null)
            {
                style = this.OwningColumn.HeaderStyle;
            }
            if (style == null && this.OwningGrid != null)
            {
                style = this.OwningGrid.ColumnHeaderStyle;
            }
            this.SetStyleWithType(style);
        }

        internal void InvokeProcessSort()
        {
            if (this.OwningGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditingMode*/))
            {
                this.Dispatcher.BeginInvoke(new Action(ProcessSort));
            }
        }

        internal void OnMouseLeftButtonDown(ref bool handled, Point mousePosition)
        {
            this.IsPressed = true;

            if (this.OwningGrid != null)
            {
                this.CaptureMouse();

                _dragMode = DragMode.MouseDown;
                _frozenColumnsWidth = this.OwningGrid.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();
                _lastMousePositionGrid = this.Translate(this.OwningGrid, mousePosition);

                double distanceFromLeft = mousePosition.X;
                double distanceFromRight = this.ActualWidth - distanceFromLeft;
                DataGridColumn currentColumn = this.OwningColumn;
                DataGridColumn previousColumn = null;
                if (!(this.OwningColumn is DataGridFillerColumn))
                {
                    previousColumn = this.OwningGrid.ColumnsInternal.GetPreviousVisibleNonFillerColumn(currentColumn);
                }

                if (_dragMode == DragMode.MouseDown && _dragColumn == null && (distanceFromRight <= DATAGRIDCOLUMNHEADER_resizeRegionWidth))
                {
                    handled = TrySetResizeColumn(currentColumn);
                }
                else if (_dragMode == DragMode.MouseDown && _dragColumn == null && distanceFromLeft <= DATAGRIDCOLUMNHEADER_resizeRegionWidth && previousColumn != null)
                {
                    handled = TrySetResizeColumn(previousColumn);
                }

                if (_dragMode == DragMode.Resize && _dragColumn != null)
                {
                    _dragStart = _lastMousePositionGrid;
                    _originalWidth = _dragColumn.ActualWidth;
                    _originalHorizontalOffset = this.OwningGrid.HorizontalOffset;

                    _lastResizeWidth = _originalWidth;
                    handled = true;
                }
            }
        }

        internal void OnMouseLeftButtonUp(ref bool handled, Point mousePosition)
        {
            this.IsPressed = false;

            if (_dragMode == DragMode.MouseDown)
            {
               OnMouseLeftButtonUp_Click(ref handled);
            }
            else if (_dragMode == DragMode.Reorder)
            {
                // Find header we're hovering over
                int? targetIndex = this.GetReorderingTargetDisplayIndex(mousePosition, true /* ignoreVertical */, true /* clipToVisible */);

                if (targetIndex.HasValue &&
                    ((!this.OwningColumn.IsFrozen && targetIndex.Value >= this.OwningGrid.FrozenColumnCount)
                      || (this.OwningColumn.IsFrozen && targetIndex.Value < this.OwningGrid.FrozenColumnCount)))
                {
                    this.OwningColumn.DisplayIndex = targetIndex.Value;

                    DataGridColumnEventArgs ea = new DataGridColumnEventArgs(this.OwningColumn);
                    this.OwningGrid.OnColumnReordered(ea);
                }

                DragCompletedEventArgs dragCompletedEventArgs = new DragCompletedEventArgs(mousePosition.X - _dragStart.Value.X, mousePosition.Y - _dragStart.Value.Y, false);
                this.OwningGrid.OnColumnHeaderDragCompleted(dragCompletedEventArgs);

                // If we autosized the indicator's height, set it back to NaN
                if (this._autoSizeDropLocationIndicatorHeight)
                {
                    this._autoSizeDropLocationIndicatorHeight = false;
                    this.OwningGrid.ColumnDropLocationIndicator.Height = double.NaN;
                }
            }
            else if (_dragMode == DragMode.Drag)
            {
                DragCompletedEventArgs dragCompletedEventArgs = new DragCompletedEventArgs(0, 0, false);
                this.OwningGrid.OnColumnHeaderDragCompleted(dragCompletedEventArgs);
            }

            SetDragCursor(mousePosition);

            // Variables that track drag mode states get reset in DataGridColumnHeader_LostMouseCapture
            ReleaseMouseCapture();
            DataGridColumnHeader._dragMode = DragMode.None;
            handled = true;
        }

        internal void OnMouseLeftButtonUp_Click(ref bool handled)
        {
            // completed a click without dragging, so we're sorting
            InvokeProcessSort();
            handled = true;
        }

        internal void OnMouseMove(ref bool handled, Point mousePosition, Point mousePositionGridParent)
        {
            if (handled)
            {
                return;
            }

            Debug.Assert(this.OwningGrid.Parent is UIElement);

            // various useful positions
            Point mousePositionHeaders = this.Translate(this.OwningGrid.ColumnHeaders, mousePosition);
            Point mousePositionGrid = this.Translate(this.OwningGrid, mousePosition);

            double distanceFromLeft = mousePosition.X;
            double distanceFromRight = this.ActualWidth - distanceFromLeft;

            OnMouseMove_Resize(ref handled, mousePositionGrid);

            OnMouseMove_Reorder(ref handled, mousePosition, mousePositionHeaders, mousePositionGrid, mousePositionGridParent, distanceFromLeft, distanceFromRight);

            // if we still haven't done anything about moving the mouse while 
            // the button is down, we remember that we're dragging, but we don't 
            // claim to have actually handled the event
            if (_dragMode == DragMode.MouseDown)
            {
                _dragMode = DragMode.Drag;
            }

            if (_dragMode == DragMode.Drag)
            {
                DragDeltaEventArgs dragDeltaEventArgs = new DragDeltaEventArgs(mousePositionGrid.X - _lastMousePositionGrid.Value.X, mousePositionGrid.Y - _lastMousePositionGrid.Value.Y);
                this.OwningGrid.OnColumnHeaderDragDelta(dragDeltaEventArgs);
            }

            _lastMousePositionGrid = mousePositionGrid;

            SetDragCursor(mousePosition);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal void ProcessSort()
        {
            // if we can sort:
            //  - DataConnection.AllowSort is true, and
            //  - AllowUserToSortColumns and CanSort are true, and
            //  - OwningColumn is bound, and
            //  - SortDescriptionsCollection exists, and
            //  - the column's data type is comparable
            // then try to sort
            if (this.OwningColumn != null
                && this.OwningGrid != null
                && this.OwningGrid.EditingRow == null
                && this.OwningColumn != this.OwningGrid.ColumnsInternal.FillerColumn
                && this.OwningGrid.DataConnection.AllowSort
                && this.OwningGrid.CanUserSortColumns
                && this.OwningColumn.CanUserSort
                && this.OwningGrid.DataConnection.SortDescriptions != null)
            {
                DataGrid owningGrid = this.OwningGrid;
                ListSortDirection newSortDirection;
                SortDescription newSort;

                bool ctrl;
                bool shift;

                KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

                SortDescription? sort = this.OwningColumn.GetSortDescription();
                ICollectionView collectionView = owningGrid.DataConnection.CollectionView;
                Debug.Assert(collectionView != null);
                using (collectionView.DeferRefresh())
                {

                    // if shift is held down, we multi-sort, therefore if it isn't, we'll clear the sorts beforehand
                    if (!shift || owningGrid.DataConnection.SortDescriptions.Count == 0)
                    {
                        if (collectionView.CanGroup && collectionView.GroupDescriptions != null)
                        {
                            // Make sure we sort by the GroupDescriptions first
                            for (int i = 0; i < collectionView.GroupDescriptions.Count; i++)
                            {
                                PropertyGroupDescription groupDescription = collectionView.GroupDescriptions[i] as PropertyGroupDescription;
                                if (groupDescription != null && collectionView.SortDescriptions.Count <= i || collectionView.SortDescriptions[i].PropertyName != groupDescription.PropertyName)
                                {
                                    collectionView.SortDescriptions.Insert(Math.Min(i, collectionView.SortDescriptions.Count), new SortDescription(groupDescription.PropertyName, ListSortDirection.Ascending));
                                }
                            }
                            while (collectionView.SortDescriptions.Count > collectionView.GroupDescriptions.Count)
                            {
                                collectionView.SortDescriptions.RemoveAt(collectionView.GroupDescriptions.Count);
                            }
                        }
                        else if (!shift)
                        {
                            owningGrid.DataConnection.SortDescriptions.Clear();
                        }
                    }

                    if (sort.HasValue)
                    {
                        // swap direction
                        switch (sort.Value.Direction)
                        {
                            case ListSortDirection.Ascending:
                                newSortDirection = ListSortDirection.Descending;
                                break;
                            default:
                                newSortDirection = ListSortDirection.Ascending;
                                break;
                        }

                        newSort = new SortDescription(sort.Value.PropertyName, newSortDirection);

                        // changing direction should not affect sort order, so we replace this column's
                        // sort description instead of just adding it to the end of the collection
                        int oldIndex = owningGrid.DataConnection.SortDescriptions.IndexOf(sort.Value);
                        if (oldIndex >= 0)
                        {
                            owningGrid.DataConnection.SortDescriptions.Remove(sort.Value);
                            owningGrid.DataConnection.SortDescriptions.Insert(oldIndex, newSort);
                        }
                        else
                        {
                            owningGrid.DataConnection.SortDescriptions.Add(newSort);
                        }
                    }
                    else
                    {
                        // start new sort
                        newSortDirection = ListSortDirection.Ascending;

                        string propertyName = this.OwningColumn.GetSortPropertyName();
                        // no-opt if we couldn't find a property to sort on
                        if (string.IsNullOrEmpty(propertyName))
                        {
                            return;
                        }

                        newSort = new SortDescription(propertyName, newSortDirection);

                        owningGrid.DataConnection.SortDescriptions.Add(newSort);
                    }
                }

                // We've completed the sort, so send the Invoked event for the column header's automation peer
                if (AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
                {
                    AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this);
                    if (peer != null)
                    {
                        peer.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
                    }
                }

            }
        }

        internal void UpdateSeparatorVisibility(DataGridColumn lastVisibleColumn)
        {
            Visibility newVisibility = _desiredSeparatorVisibility;

            // Collapse separator for the last column if there is no filler column
            if (this.OwningColumn != null &&
                this.OwningGrid != null &&
                _desiredSeparatorVisibility == Visibility.Visible &&
                this.OwningColumn == lastVisibleColumn &&
                !this.OwningGrid.ColumnsInternal.FillerColumn.IsActive)
            {
                newVisibility = Visibility.Collapsed;
            }

            // Update the public property if it has changed
            if (this.SeparatorVisibility != newVisibility)
            {
                this.SetValueNoCallback(DataGridColumnHeader.SeparatorVisibilityProperty, newVisibility);
            }
        }

        #endregion Internal Methods


        #region Private Methods

        private bool CanReorderColumn(DataGridColumn column)
        {
            return this.OwningGrid.CanUserReorderColumns 
                && !(column is DataGridFillerColumn)
                && (column.CanUserReorderInternal.HasValue && column.CanUserReorderInternal.Value || !column.CanUserReorderInternal.HasValue);
        }

        private void DataGridColumnHeader_LostMouseCapture(object sender, MouseEventArgs e)
        {
            this.OnLostMouseCapture();
        }

        private void DataGridColumnHeader_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            Point mousePosition = e.GetPosition(this);
            this.OnMouseEnter(mousePosition);
            ApplyState(true);
        }

        private void DataGridColumnHeader_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            this.OnMouseLeave();
            ApplyState(true);
        }

        private void DataGridColumnHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.OwningColumn == null || e.Handled || !this.IsEnabled)
            {
                return;
            }

            Point mousePosition = e.GetPosition(this);
            bool handled = e.Handled;
            OnMouseLeftButtonDown(ref handled, mousePosition);
            e.Handled = handled;

            ApplyState(true);
        }

        private void DataGridColumnHeader_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.OwningColumn == null || e.Handled || !this.IsEnabled)
            {
                return;
            }

            Point mousePosition = e.GetPosition(this);
            bool handled = e.Handled;
            OnMouseLeftButtonUp(ref handled, mousePosition);
            e.Handled = handled;

            ApplyState(true);
        }

        private void DataGridColumnHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.OwningColumn == null || !this.IsEnabled)
            {
                return;
            }

            Point mousePosition = e.GetPosition(this);
            Point absolutePosition = e.GetPosition(null);

            bool handled = false;

            OnMouseMove(ref handled, mousePosition, absolutePosition);

        }

        /// <summary>
        /// Returns the column against whose top-left the reordering caret should be positioned
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="ignoreVertical"></param>
        /// <param name="clipToVisible"></param>
        /// <returns></returns>
        private DataGridColumn GetReorderingTargetColumn(Point mousePosition, bool ignoreVertical, bool clipToVisible)
        {
            if (!clipToVisible || (mousePosition.Within(this, this.OwningGrid, ignoreVertical) && mousePosition.Within(this, this.OwningGrid.ColumnHeaders, ignoreVertical)))
            {
                DataGridColumnHeader result = this.OwningGrid.ColumnHeaders.Children
                    .OfType<DataGridColumnHeader>()
                    .Where(header => header.OwningGrid != null && header.OwningColumn != this.OwningGrid.ColumnsInternal.FillerColumn)
                    .OrderBy(header => header.OwningColumn.DisplayIndex)
                    .FirstOrDefault(header => IsReorderTargeted(mousePosition, header, ignoreVertical));

                if (result == null)
                {
                    return this.OwningGrid.ColumnsInternal.FillerColumn;
                }
                else
                {
                    return result.OwningColumn;
                }
            }


            return null;
        }

        /// <summary>
        /// Returns the display index to set the column to
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="ignoreVertical"></param>
        /// <param name="clipToVisible"></param>
        /// <returns></returns>
        private int? GetReorderingTargetDisplayIndex(Point mousePosition, bool ignoreVertical, bool clipToVisible)
        {
            if (!clipToVisible || (mousePosition.Within(this, this.OwningGrid, ignoreVertical) && mousePosition.Within(this, this.OwningGrid.ColumnHeaders, ignoreVertical)))
            {
                DataGridColumnHeader result = this.OwningGrid.ColumnHeaders.Children
                    .OfType<DataGridColumnHeader>()
                    .Where(header => header.OwningGrid != null && header.OwningColumn != this.OwningGrid.ColumnsInternal.FillerColumn)
                    .OrderBy(header => header.OwningColumn.DisplayIndex)
                    .FirstOrDefault(header => IsReorderTargeted(mousePosition, header, ignoreVertical));

                if (result != null)
                {
                    if (result.OwningColumn.DisplayIndex > this.OwningColumn.DisplayIndex)
                    {
                        // later column, but we need to adjust for the index shift that would happen
                        return result.OwningColumn.DisplayIndex - 1;
                    }
                    else
                    {
                        // earlier column
                        return result.OwningColumn.DisplayIndex;
                    }
                }
                else
                {
                    // last column
                    return this.OwningGrid.Columns.Count - 1;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns true if the mouse is 
        /// - to the left of the element, or within the left half of the element
        /// and
        /// - within the vertical range of the element, or ignoreVertical == true
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="element"></param>
        /// <param name="ignoreVertical"></param>
        /// <returns></returns>
        private bool IsReorderTargeted(Point mousePosition, FrameworkElement element, bool ignoreVertical)
        {
            Point position = this.Translate(element, mousePosition);

            return (position.X < 0 || (position.X >= 0 && position.X <= element.ActualWidth / 2))
                && (ignoreVertical || (position.Y >= 0 && position.Y <= element.ActualHeight))
                ;
        }

        /// <summary>
        /// Resets the static DataGridColumnHeader properties when a header loses mouse capture
        /// </summary>
        private void OnLostMouseCapture()
        {
            // When we stop interacting with the column headers, we need to reset the drag mode
            // and close any popups if they are open.

            if (DataGridColumnHeader._dragColumn != null && DataGridColumnHeader._dragColumn.HeaderCell != null)
            {
                DataGridColumnHeader._dragColumn.HeaderCell.Cursor = DataGridColumnHeader._originalCursor;
            }
            _dragMode = DragMode.None;
            _dragColumn = null;
            _dragStart = null;
            _lastMousePositionGrid = null;

            if (_reorderingThumb != null)
            {
                _reorderingThumb.IsOpen = false;
            }

            if (this.OwningGrid != null)
            {
                this.OwningGrid.ColumnDropLocationIndicatorPopup.IsOpen = false;
            }
        }

        /// <summary>
        /// Sets up the DataGridColumnHeader for the MouseEnter event
        /// </summary>
        /// <param name="mousePosition">mouse position relative to the DataGridColumnHeader</param>
        private void OnMouseEnter(Point mousePosition)
        {
            this.IsMouseOver = true;
            this.SetDragCursor(mousePosition);
        }

        /// <summary>
        /// Sets up the DataGridColumnHeader for the MouseLeave event
        /// </summary>
        private void OnMouseLeave()
        {
            this.IsMouseOver = false;
        }

        private void OnMouseMove_BeginReorder(Point mousePosition, Point mousePositionGridParent)
        {
            DataGridColumnHeader dragIndicator = new DataGridColumnHeader();
            dragIndicator.OwningColumn = this.OwningColumn;
            dragIndicator.IsEnabled = false;
            dragIndicator.Content = this.Content;
            dragIndicator.ContentTemplate = this.ContentTemplate;

            if (this.OwningColumn.DragIndicatorStyle != null)
            {
                dragIndicator.SetStyleWithType(this.OwningColumn.DragIndicatorStyle);
            }
            else if (this.OwningGrid.DragIndicatorStyle != null)
            {
                dragIndicator.SetStyleWithType(this.OwningGrid.DragIndicatorStyle);
            }

            // If the user didn't style the dragIndicator's Width, default it to the column header's width
            if (double.IsNaN(dragIndicator.Width))
            {
                dragIndicator.Width = this.ActualWidth;
            }

            // If the user didn't style the dropLocationIndicator's Height, default to the column header's height
            if (double.IsNaN(this.OwningGrid.ColumnDropLocationIndicator.Height))
            {
                this._autoSizeDropLocationIndicatorHeight = true;
                this.OwningGrid.ColumnDropLocationIndicator.Height = this.ActualHeight;
            }

            // pass the caret's data template to the user for modification
            DataGridColumnReorderingEventArgs columnReorderingEventArgs = new DataGridColumnReorderingEventArgs(this.OwningColumn)
            {
                DropLocationIndicator = this.OwningGrid.ColumnDropLocationIndicator,
                DragIndicator = dragIndicator
            };
            this.OwningGrid.OnColumnReordering(columnReorderingEventArgs);
            if (columnReorderingEventArgs.Cancel)
            {
                return;
            }

            // The user didn't cancel, so prepare for the reorder
            _dragColumn = this.OwningColumn;
            _dragMode = DragMode.Reorder;
            _dragStartParent = mousePositionGridParent;
            // the mouse position relative to the ColumnHeader needs to be scaled to be in the same
            // dimensions as the DataGrid, so that it doesn't get out of sync later on
            _dragStart = this.OwningGrid.RenderTransform.Transform(mousePosition);

            // Display the reordering thumb
            if (_reorderingThumb == null)
            {
                _reorderingThumb = new Popup();
            }

            _reorderingThumb.Child = columnReorderingEventArgs.DragIndicator;
            _reorderingThumb.IsOpen = true;

            // use the data template to populate the caret
            if (columnReorderingEventArgs.DropLocationIndicator != null)
            {
                Control child = columnReorderingEventArgs.DropLocationIndicator;

                this.OwningGrid.ColumnDropLocationIndicatorPopup.Child = child;
                this.OwningGrid.ColumnDropLocationIndicatorPopup.Height = child.ActualHeight;
                this.OwningGrid.ColumnDropLocationIndicatorPopup.Width = child.ActualWidth;

                this.OwningGrid.ColumnDropLocationIndicatorPopup.IsOpen = false;
            }
        }

        private void OnMouseMove_Reorder(ref bool handled, Point mousePosition, Point mousePositionHeaders, Point mousePositionGrid, Point mousePositionGridParent, double distanceFromLeft, double distanceFromRight)
        {
            if (handled)
            {
                return;
            }

            #region handle entry into reorder mode
            if (_dragMode == DragMode.MouseDown && _dragColumn == null && (distanceFromRight > DATAGRIDCOLUMNHEADER_resizeRegionWidth && distanceFromLeft > DATAGRIDCOLUMNHEADER_resizeRegionWidth))
            {
                DragStartedEventArgs dragStartedEventArgs = new DragStartedEventArgs(mousePositionGrid.X - _lastMousePositionGrid.Value.X, mousePositionGrid.Y - _lastMousePositionGrid.Value.Y);
                this.OwningGrid.OnColumnHeaderDragStarted(dragStartedEventArgs);

                handled = CanReorderColumn(this.OwningColumn);

                if (handled)
                {
                    OnMouseMove_BeginReorder(mousePosition, mousePositionGridParent);
                }
            }
            #endregion

            #region handle reorder mode (eg, positioning of the popup)
            if (_dragMode == DragMode.Reorder && _reorderingThumb != null)
            {
                DragDeltaEventArgs dragDeltaEventArgs = new DragDeltaEventArgs(mousePositionGrid.X - _lastMousePositionGrid.Value.X, mousePositionGrid.Y - _lastMousePositionGrid.Value.Y);
                this.OwningGrid.OnColumnHeaderDragDelta(dragDeltaEventArgs);

                _reorderingThumb.HorizontalOffset = mousePositionGridParent.X - _dragStart.Value.X;
                _reorderingThumb.VerticalOffset = _dragStartParent.Value.Y - _dragStart.Value.Y;

                // the mouse position relative to the ColumnHeadersPresenter can be scaled differently than
                // the same position relative to the DataGrid, so we'll have to apply the grid's RenderTransform
                Point scaledMousePositionHeaders = mousePositionHeaders;
                if (!this.OwningColumn.IsFrozen)
                {
                    scaledMousePositionHeaders.X -= _frozenColumnsWidth;
                }
                scaledMousePositionHeaders = this.OwningGrid.RenderTransform.Transform(scaledMousePositionHeaders);

                // prepare some variables for clipping/hiding
                double dgX = mousePositionGridParent.X - scaledMousePositionHeaders.X;
                double dgY = mousePositionGridParent.Y - scaledMousePositionHeaders.Y;
                double dgW = this.OwningColumn.IsFrozen ? _frozenColumnsWidth : this.OwningGrid.CellsWidth - _frozenColumnsWidth;
                double dgH = this.OwningGrid.ActualRowsPresenterHeight + this.OwningGrid.ColumnHeaders.ActualHeight;

                // we need to transform the size of the clipping rectangle if the datagrid has a rendertransform set
                Point clipSize = new Point(dgW, dgH);
                clipSize = this.OwningGrid.RenderTransform.Transform(clipSize);

                // clip the thumb to the column headers region
                _reorderingThumb.Child.Clip = new RectangleGeometry
                {
                    Rect = new Rect(
                        dgX - _reorderingThumb.HorizontalOffset,
                        dgY - _reorderingThumb.VerticalOffset,
                        clipSize.X,
                        clipSize.Y
                        )
                };

                // if the datagrid has a scale transform, apply the inverse to the popup's clipping rectangle
                ScaleTransform scaleTransform = null;
                ScaleTransform gridScaleTransform = _reorderingThumb.Child.RenderTransform as ScaleTransform;
                if (gridScaleTransform != null && gridScaleTransform.ScaleY != 0.0 && gridScaleTransform.ScaleX != 0.0)
                {
                    scaleTransform = new ScaleTransform();
                    scaleTransform.ScaleX = 1.0 / gridScaleTransform.ScaleX;
                    scaleTransform.ScaleY = 1.0 / gridScaleTransform.ScaleY;
                }
                if (scaleTransform != null)
                {
                    _reorderingThumb.Child.Clip.Transform = scaleTransform;
                }

                // Find header we're hovering over
                DataGridColumn targetColumn = this.GetReorderingTargetColumn(mousePosition, true /* ignoreVertical */, true /* clipToVisible */);

                if (this.OwningGrid.ColumnDropLocationIndicator != null)
                {
                    if (targetColumn == this.OwningGrid.ColumnsInternal.FillerColumn)
                    {
                        // change target column to last column but position caret at the end

                        targetColumn = this.OwningGrid.ColumnsInternal.LastVisibleColumn;

                        Point targetPosition = this.OwningGrid.RenderTransform.Transform(this.Translate(targetColumn.HeaderCell, mousePosition));

                        this.OwningGrid.ColumnDropLocationIndicatorPopup.IsOpen = true;
                        this.OwningGrid.ColumnDropLocationIndicatorPopup.HorizontalOffset = mousePositionGridParent.X - targetPosition.X + targetColumn.HeaderCell.ActualWidth;
                        this.OwningGrid.ColumnDropLocationIndicatorPopup.VerticalOffset = mousePositionGridParent.Y - targetPosition.Y;
                    }
                    else if (targetColumn != null)
                    {
                        // try to position caret

                        Point targetPosition = this.OwningGrid.RenderTransform.Transform(this.Translate(targetColumn.HeaderCell, mousePosition));

                        this.OwningGrid.ColumnDropLocationIndicatorPopup.IsOpen = true;
                        this.OwningGrid.ColumnDropLocationIndicatorPopup.HorizontalOffset = mousePositionGridParent.X - targetPosition.X;
                        this.OwningGrid.ColumnDropLocationIndicatorPopup.VerticalOffset = mousePositionGridParent.Y - targetPosition.Y;
                    }
                    else
                    {
                        this.OwningGrid.ColumnDropLocationIndicatorPopup.IsOpen = false;
                    }

                    // hide the caret if it's off the grid -- this is not the same as clipping
                    if (this.OwningGrid.ColumnDropLocationIndicatorPopup.HorizontalOffset < dgX
                        || dgX + clipSize.X < this.OwningGrid.ColumnDropLocationIndicatorPopup.HorizontalOffset)
                    {
                        this.OwningGrid.ColumnDropLocationIndicatorPopup.IsOpen = false;
                    }
                }

                handled = true;
            }
            #endregion
        }

        private void OnMouseMove_Resize(ref bool handled, Point mousePositionGrid)
        {
            if (handled)
            {
                return;
            }

            if (_dragMode == DragMode.Resize && _dragColumn != null && _dragStart.HasValue)
            {
                // resize column

                double mouseDelta = mousePositionGrid.X - _dragStart.Value.X;
                double newWidth = _originalWidth + mouseDelta;

                if (_lastResizeWidth >= _dragColumn.MinWidth && newWidth != _lastResizeWidth)
                {
                    newWidth = Math.Max(newWidth, 0);

                    _dragColumn.Width = new DataGridLength(newWidth);
                    _lastResizeWidth = newWidth;
                    this.OwningGrid.UpdateHorizontalOffset(_originalHorizontalOffset);
                }

                handled = true;
            }
        }

        private void SetDragCursor(Point mousePosition)
        {
            if (_dragMode != DragMode.None || this.OwningGrid == null || this.OwningColumn == null)
            {
                return;
            }

            // set mouse if we can resize column

            double distanceFromLeft = mousePosition.X;
            double distanceFromRight = this.ActualWidth - distanceFromLeft;
            DataGridColumn currentColumn = this.OwningColumn;
            DataGridColumn previousColumn = null;

            if (!(this.OwningColumn is DataGridFillerColumn))
            {
                previousColumn = this.OwningGrid.ColumnsInternal.GetPreviousVisibleNonFillerColumn(currentColumn);
            }

            if ((distanceFromRight <= DATAGRIDCOLUMNHEADER_resizeRegionWidth && currentColumn != null && currentColumn.ActualCanUserResize) ||
                (distanceFromLeft <= DATAGRIDCOLUMNHEADER_resizeRegionWidth && previousColumn != null && previousColumn.ActualCanUserResize))
            {
                if (this.Cursor != Cursors.SizeWE)
                {
                    DataGridColumnHeader._originalCursor = this.Cursor;
                    this.Cursor = Cursors.SizeWE;
                }
            }
            else
            {
                this.Cursor = DataGridColumnHeader._originalCursor;
            }
        }

        private static bool TrySetResizeColumn(DataGridColumn column)
        {
            // If datagrid.CanUserResizeColumns == false, then the column can still override it
            if (column.ActualCanUserResize)
            {
                _dragColumn = column;

                _dragMode = DragMode.Resize;

                return true;
            }
            return false;
        }

        #endregion Private Methods
    }
}
