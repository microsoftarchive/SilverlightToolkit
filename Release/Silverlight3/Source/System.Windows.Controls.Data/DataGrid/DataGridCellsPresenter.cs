// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using System.Windows.Media;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Used within the template of a <see cref="T:System.Windows.Controls.DataGrid" />
    /// to specify the location in the control's visual tree where the cells are to be added. 
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public sealed class DataGridCellsPresenter : Panel
    {
        private double _fillerLeftEdge;

        #region Properties

        // The desired height needs to be cached due to column virtualization; otherwise, the cells
        // would grow and shrink as the DataGrid scrolls horizontally
        private double DesiredHeight
        {
            get;
            set;
        }

        private DataGrid OwningGrid
        {
            get
            {
                if (this.OwningRow != null)
                {
                    return this.OwningRow.OwningGrid;
                }
                return null;
            }
        }

        internal DataGridRow OwningRow
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Arranges the content of the <see cref="T:System.Windows.Controls.Primitives.DataGridCellsPresenter" />.
        /// </summary>
        /// <returns>
        /// The actual size used by the <see cref="T:System.Windows.Controls.Primitives.DataGridCellsPresenter" />.
        /// </returns>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.OwningGrid == null)
            {
                return base.ArrangeOverride(finalSize);
            }
            double frozenLeftEdge = 0;
            double scrollingLeftEdge = -this.OwningGrid.HorizontalOffset;

            double cellLeftEdge;
            foreach (DataGridColumn column in this.OwningGrid.ColumnsInternal.GetVisibleColumns())
            {
                DataGridCell cell = this.OwningRow.Cells[column.Index];
                Debug.Assert(cell.OwningColumn == column);
                Debug.Assert(column.Visibility == Visibility.Visible);
                
                if (column.IsFrozen)
                {
                    cellLeftEdge = frozenLeftEdge;
                    // This can happen before or after clipping because frozen cells aren't clipped
                    frozenLeftEdge += column.ActualWidth;
                }
                else
                {
                    cellLeftEdge = scrollingLeftEdge;
                }
                if (cell.Visibility == Visibility.Visible)
                {
                    cell.Arrange(new Rect(cellLeftEdge, 0, column.ActualWidth, finalSize.Height));
                    EnsureCellClip(cell, column.ActualWidth, finalSize.Height, frozenLeftEdge, scrollingLeftEdge);
                }
                scrollingLeftEdge += column.ActualWidth;
            }

            _fillerLeftEdge = scrollingLeftEdge;
            // FillerColumn.Width == 0 when the filler column is not active
            this.OwningRow.FillerCell.Arrange(new Rect(_fillerLeftEdge, 0, this.OwningGrid.ColumnsInternal.FillerColumn.FillerWidth, finalSize.Height));

            return finalSize;
        }

        private static void EnsureCellClip(DataGridCell cell, double width, double height, double frozenLeftEdge, double cellLeftEdge)
        {
            // Clip the cell only if it's scrolled under frozen columns.  Unfortunately, we need to clip in this case
            // because cells could be transparent
            if (!cell.OwningColumn.IsFrozen && frozenLeftEdge > cellLeftEdge)
            {
                RectangleGeometry rg = new RectangleGeometry();
                double xClip = Math.Round(Math.Min(width, frozenLeftEdge - cellLeftEdge));
                rg.Rect = new Rect(xClip, 0, width - xClip, height);
                cell.Clip = rg;
            }
            else
            {
                cell.Clip = null;
            }
        }

        private static void EnsureCellDisplay(DataGridCell cell, bool displayColumn)
        {
            if (cell.IsCurrent)
            {
                if (displayColumn)
                {
                    cell.Visibility = Visibility.Visible;
                    cell.Clip = null;
                }
                else
                {
                    // Clip
                    RectangleGeometry rg = new RectangleGeometry();
                    rg.Rect = Rect.Empty;
                    cell.Clip = rg;
                }
            }
            else
            {
                cell.Visibility = displayColumn ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        internal void EnsureFillerVisibility()
        {
            DataGridFillerColumn fillerColumn = this.OwningGrid.ColumnsInternal.FillerColumn;
            Visibility newVisibility = fillerColumn.IsActive ? Visibility.Visible : Visibility.Collapsed;
            if (this.OwningRow.FillerCell.Visibility != newVisibility)
            {
                this.OwningRow.FillerCell.Visibility = newVisibility;
                if (newVisibility == Visibility.Visible)
                {
                    this.OwningRow.FillerCell.Arrange(new Rect(_fillerLeftEdge, 0, fillerColumn.FillerWidth, this.ActualHeight));
                }
            }

            // This must be done after the Filler visibility is determined.  This also must be done
            // regardless of whether or not the filler visibility actually changed values because
            // we could scroll in a cell that didn't have EnsureGridLine called yet
            DataGridColumn lastVisibleColumn = this.OwningGrid.ColumnsInternal.LastVisibleColumn;
            if (lastVisibleColumn != null)
            {
                DataGridCell cell = this.OwningRow.Cells[lastVisibleColumn.Index];
                cell.EnsureGridLine(lastVisibleColumn);
            }
        }

        /// <summary>
        /// Measures the children of a <see cref="T:System.Windows.Controls.Primitives.DataGridCellsPresenter" /> to 
        /// prepare for arranging them during the <see cref="M:System.Windows.FrameworkElement.ArrangeOverride(System.Windows.Size)" /> pass.
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements. Indicates an upper limit that child elements should not exceed.
        /// </param>
        /// <returns>
        /// The size that the <see cref="T:System.Windows.Controls.Primitives.DataGridCellsPresenter" /> determines it needs during layout, based on its calculations of child object allocated sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.OwningGrid == null)
            {
                return base.MeasureOverride(availableSize);
            }
            bool autoSizeHeight;
            double measureHeight;
            if (double.IsNaN(this.OwningGrid.RowHeight))
            {
                // No explicit height values were set so we can autosize
                autoSizeHeight = true;
                measureHeight = double.PositiveInfinity;
            }
            else
            {
                this.DesiredHeight = this.OwningGrid.RowHeight;
                measureHeight = this.DesiredHeight;
                autoSizeHeight = false;
            }

            DataGridColumn lastVisibleColumn = this.OwningGrid.ColumnsInternal.LastVisibleColumn;
            double totalCellsWidth = this.OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth;
            double measureWidth;

            double frozenLeftEdge = 0;
            double scrollingLeftEdge = -this.OwningGrid.HorizontalOffset;
            foreach (DataGridColumn column in this.OwningGrid.ColumnsInternal.GetVisibleColumns())
            {
                DataGridCell cell = this.OwningRow.Cells[column.Index];
                // Measure the entire first row to make the horizontal scrollbar more accurate
                bool shouldDisplayCell = ShouldDisplayCell(column, frozenLeftEdge, scrollingLeftEdge) || this.OwningRow.Index == 0;
                EnsureCellDisplay(cell, shouldDisplayCell);
                if (shouldDisplayCell)
                {
                    DataGridLength columnWidth = column.EffectiveWidth;
                    measureWidth = columnWidth.IsAbsolute ? column.ActualWidth : column.ActualMaxWidth;
                    bool autoGrowWidth = columnWidth.IsSizeToCells || columnWidth.IsAuto;
                    cell.Measure(new Size(measureWidth, measureHeight));
                    if (column != lastVisibleColumn)
                    {
                        cell.EnsureGridLine(lastVisibleColumn);
                    }
                    if (autoSizeHeight)
                    {
                        this.DesiredHeight = Math.Max(this.DesiredHeight, cell.DesiredSize.Height);
                    }
                    if (autoGrowWidth && cell.DesiredSize.Width > column.DesiredWidth)
                    {
                        column.DesiredWidth = cell.DesiredSize.Width;
                    }
                }
                if (column.IsFrozen)
                {
                    frozenLeftEdge += column.ActualWidth;
                }
                scrollingLeftEdge += column.ActualWidth;
            }

            // Measure FillerCell, we're doing it unconditionally here because we don't know if we'll need the filler
            // column and we don't want to cause another Measure if we do
            this.OwningRow.FillerCell.Measure(new Size(double.PositiveInfinity, this.DesiredHeight));

            return new Size(totalCellsWidth, this.DesiredHeight);
        }

        internal void Recycle()
        {
            // Clear out the cached desired height so it is not reused for other rows
            this.DesiredHeight = 0;
        }

        private bool ShouldDisplayCell(DataGridColumn column, double frozenLeftEdge, double scrollingLeftEdge)
        {
            Debug.Assert(this.OwningGrid != null);

            if (column.Visibility != Visibility.Visible)
            {
                return false;
            }
            double leftEdge = column.IsFrozen ? frozenLeftEdge : scrollingLeftEdge;
            double rightEdge = leftEdge + column.ActualWidth;
            return DoubleUtil.GreaterThan(rightEdge, 0) &&
                DoubleUtil.LessThanOrClose(leftEdge, this.OwningGrid.CellsWidth) &&
                DoubleUtil.GreaterThan(rightEdge, frozenLeftEdge); // scrolling column covered up by frozen column(s)
        }

        #endregion Methods
    }
}
