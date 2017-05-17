// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Windows.Controls;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// The SampleTreeItem represents a node in the TreeView.  
    /// </summary>
    [ContentProperty("Items")]
    public class SampleTreeItem
    {
        /// <summary>
        /// Gets or sets the name of the TreeView node.
        /// </summary>
        public string TreeItemName { get; set; }

        /// <summary>
        ///  Gets a collection of SampleTreeItems.
        /// </summary>
        public Collection<SampleTreeItem> Items { get; private set; }

        /// <summary>
        /// Initialize a SampleTreeItem.
        /// </summary>
        public SampleTreeItem()
        {
            Items = new Collection<SampleTreeItem>();
        }

        /// <summary>
        /// Gets or sets the resource name of the Icon representing this 
        /// node.
        /// </summary>
        public string IconResourceName { get; set; }

        /// <summary>
        /// Gets the icon representing this type. 
        /// </summary>
        public Image Icon
        {
            get
            {
                return SharedResources.GetIcon(IconResourceName);
            }
        }
    }
}
