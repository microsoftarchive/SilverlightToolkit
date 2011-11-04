// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Holds information about an item for use in the LongListSelector.
    /// </summary>
    public class LongListSelectorItem
    {
        /// <summary>
        /// Gets or sets the item type.
        /// </summary>
        public LongListSelectorItemType ItemType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the associated group for the item.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Assists in debugging.")]
        public object Group
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the underlying item instance.
        /// </summary>
        public object Item
        {
            get;
            set;
        }
    }
}
