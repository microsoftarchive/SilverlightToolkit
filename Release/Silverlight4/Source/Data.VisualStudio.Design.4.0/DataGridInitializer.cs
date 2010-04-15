// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.Data.Design
{
    internal class DataGridInitializer : DefaultInitializer
    {
        public DataGridInitializer()
            : base()
        {
        }

        // Set any property defaults here
        public override void InitializeDefaults(ModelItem item)
        {
            if (item != null)
            {
                DataGridDesignHelper.SparseSetValue(item.Properties[PlatformTypes.DataGrid.WidthProperty], 200.0);
                DataGridDesignHelper.SparseSetValue(item.Properties[PlatformTypes.DataGrid.HeightProperty], 200.0);

                DataGridDesignHelper.SparseSetValue(item.Properties[PlatformTypes.DataGrid.AutoGenerateColumnsProperty], false);
            }
        }
    }
}
