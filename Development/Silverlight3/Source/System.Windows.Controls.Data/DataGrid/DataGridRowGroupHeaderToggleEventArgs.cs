// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace System.Windows.Controls
{
    /// <summary>
    /// EventArgs used for the DataGrid's ExpandingRowGroup and CollapsingRowGroup events
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataGridRowGroupHeaderToggleEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Constructs a DataGridRowGroupHeaderToggleEventArgs instance
        /// </summary>
        /// <param name="rowGroupHeader"></param>
        public DataGridRowGroupHeaderToggleEventArgs(DataGridRowGroupHeader rowGroupHeader)
        {
            this.RowGroupHeader = rowGroupHeader;
        }

        /// <summary>
        /// DataGridRowGroupHeader associated with this instance
        /// </summary>
        public DataGridRowGroupHeader RowGroupHeader
        {
            get;
            private set;
        }
    }
}
