// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents product sales data.
    /// </summary>
    public class SalesData
    {
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Product { set; get; }

        /// <summary>
        /// Gets or sets the quantity of the product sold.
        /// </summary>
        public int Quantity { set; get; }

        /// <summary>
        /// Gets the desired tooltip content.
        /// </summary>
        public string ToolTip
        {
            get
            {
                return Product + ": " + Quantity;
            }
        }
    }
}
