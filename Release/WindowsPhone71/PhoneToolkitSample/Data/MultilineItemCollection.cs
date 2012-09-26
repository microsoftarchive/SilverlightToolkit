// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PhoneToolkitSample.Data
{
    public class MultilineItemCollection
    {
        public MultilineItemCollection()
        {
            this.Items = new ObservableCollection<MultilineItem>();
        }

        /// <summary>
        /// A collection for MultilineItem objects.
        /// </summary>
        public ObservableCollection<MultilineItem> Items { get; private set; }
    }
}
