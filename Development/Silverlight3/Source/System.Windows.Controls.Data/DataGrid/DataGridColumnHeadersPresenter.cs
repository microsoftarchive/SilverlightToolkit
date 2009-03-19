// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Media;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Used within the template of a <see cref="T:System.Windows.Controls.DataGrid" /> to specify the 
    /// location in the control's visual tree where the column headers are to be added.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public sealed class DataGridColumnHeadersPresenter : Panel
    {
        #region Properties

        internal DataGrid OwningGrid
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Arranges the content of the <see cref="T:System.Windows.Controls.Primitives.DataGridColumnHeadersPresenter" />.
        /// </summary>
        /// <returns>
        /// The actual size used by the <see cref="T:System.Windows.Controls.Primitives.DataGridColumnHeadersPresenter" />.
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
            foreach (DataGridColumn dataGridColumn in this.OwningGrid.ColumnsInternal.GetVisibleColumns())
            {
                DataGridColumnHeader columnHeader = dataGridColumn.HeaderCell;
                Debug.Assert(columnHeader.OwningColumn == dataGridColumn);
                if (dataGridColumn.IsFrozen)
                {
                    columnHeader.Arrange(new Rect(frozenLeftEdge, 0, dataGridColumn.ActualWidth, finalSize.Height));
                    columnHeader.Clip = null; // The layout system could have clipped this becaues it's not aware of our render transform
                    frozenLeftEdge += dataGridColumn.ActualWidth;
                }
                else
                {
                    columnHeader.Arrange(new Rect(scrollingLeftEdge, 0, dataGridColumn.ActualWidth, finalSize.Height));
                    EnsureColumnHeaderClip(columnHeader, dataGridColumn.ActualWidth, finalSize.Height, frozenLeftEdge, scrollingLeftEdge);
                }
                scrollingLeftEdge += dataGridColumn.ActualWidth;
            }

            // Arrange filler
            this.OwningGrid.OnFillerColumnWidthNeeded(finalSize.Width);
            DataGridFillerColumn fillerColumn = this.OwningGrid.ColumnsInternal.FillerColumn;
            if (fillerColumn.FillerWidth > 0)
            {
                fillerColumn.HeaderCell.Visibility = Visibility.Visible;
                fillerColumn.HeaderCell.Arrange(new Rect(scrollingLeftEdge, 0, fillerColumn.FillerWidth, finalSize.Height));
            }
            else
            {
                fillerColumn.HeaderCell.Visibility = Visibility.Collapsed;
            }

            // This needs to be updated after the filler column is configured
            DataGridColumn lastVisibleColumn = this.OwningGrid.ColumnsInternal.LastVisibleColumn;
            if (lastVisibleColumn != null)
            {
                lastVisibleColumn.HeaderCell.UpdateSeparatorVisibility(lastVisibleColumn);
            }
            return finalSize;
        }

        private static void EnsureColumnHeaderClip(DataGridColumnHeader columnHeader, double width, double height, double frozenLeftEdge, double columnHeaderLeftEdge)
        {
            // Clip the cell only if it's scrolled under frozen columns.  Unfortunately, we need to clip in this case
            // because cells could be transparent
            if (frozenLeftEdge > columnHeaderLeftEdge)
            {
                RectangleGeometry rg = new RectangleGeometry();
                double xClip = Math.Min(width, frozenLeftEdge - columnHeaderLeftEdge);
                rg.Rect = new Rect(xClip, 0, width - xClip, height);
                columnHeader.Clip = rg;
            }
            else
            {
                columnHeader.Clip = null;
            }
        }

        /// <summary>
        /// Measures the children of a <see cref="T:System.Windows.Controls.Primitives.DataGridColumnHeadersPresenter" /> to 
        /// prepare for arranging them during the <see cref="M:System.Windows.FrameworkElement.ArrangeOverride(System.Windows.Size)" /> pass.
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements. Indicates an upper limit that child elements should not exceed.
        /// </param>
        /// <returns>
        /// The size that the <see cref="T:System.Windows.Controls.Primitives.DataGridColumnHeadersPresenter" /> determines it needs during layout, based on its calculations of child object allocated sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.OwningGrid == null)
            {
                return base.MeasureOverride(availableSize);
            }
            if (!this.OwningGrid.AreColumnHeadersVisible)
            {
                return Size.Empty;
            }
            double height = this.OwningGrid.ColumnHeaderHeight;
            bool autoSizeHeight;
            if (double.IsNaN(height))
            {
                // No explicit height values were set so we can autosize
                height = 0;
                autoSizeHeight = true;
            }
            else
            {
                autoSizeHeight = false;
            }

            double measureWidth;
            DataGridColumn lastVisibleColumn = this.OwningGrid.ColumnsInternal.LastVisibleColumn;
            foreach (DataGridColumn column in this.OwningGrid.ColumnsInternal.GetVisibleColumns())
            {
                DataGridLength columnWidth = column.EffectiveWidth;
                measureWidth = columnWidth.IsAbsolute ? column.ActualWidth : column.ActualMaxWidth;
                bool autoGrowWidth = columnWidth.IsAuto || columnWidth.IsSizeToHeader;

                DataGridColumnHeader columnHeader = column.HeaderCell;
                if (column != lastVisibleColumn)
                {
                    columnHeader.UpdateSeparatorVisibility(lastVisibleColumn);
                }
                columnHeader.Measure(new Size(measureWidth, double.PositiveInfinity));
                if (autoSizeHeight)
                {
                    height = Math.Max(height, columnHeader.DesiredSize.Height);
                }
                if (autoGrowWidth && columnHeader.DesiredSize.Width > column.DesiredWidth)
                {
                    column.DesiredWidth = columnHeader.DesiredSize.Width;
                }
            }

            // Add the filler column if it's not represented.  We won't know whether we need it or not until Arrange
            DataGridFillerColumn fillerColumn = this.OwningGrid.ColumnsInternal.FillerColumn;
            if (!fillerColumn.IsRepresented)
            {
                Debug.Assert(!this.Children.Contains(fillerColumn.HeaderCell));
                fillerColumn.HeaderCell.SeparatorVisibility = Visibility.Collapsed;
                this.Children.Insert(this.OwningGrid.ColumnsInternal.Count, fillerColumn.HeaderCell);
                fillerColumn.IsRepresented = true;
                // Optimize for the case where we don't need the filler cell 
                fillerColumn.HeaderCell.Visibility = Visibility.Collapsed;
            }
            fillerColumn.HeaderCell.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return new Size(this.OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth, height);
        }

        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridColumnHeadersPresenterAutomationPeer(this);
        }

        #endregion Methods
    }
}
