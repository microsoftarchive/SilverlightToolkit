// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Represents a DataTemplate that supports HeaderedItemsControl, such as
    /// TreeViewItem or MenuItem.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public partial class HierarchicalDataTemplate : DataTemplate
    {
        /// <summary>
        /// Gets or sets the binding for this data template, which indicates
        /// where to find the collection that represents the next level in the
        /// data hierarchy. 
        /// </summary>
        /// <remarks>
        ///  The default value is null.
        /// </remarks>
        public Binding ItemsSource { get; set; }

        #region public DataTemplate ItemTemplate
        /// <summary>
        /// The DataTemplate to apply to the ItemTemplate property on a
        /// generated HeaderedItemsControl (such as a MenuItem or a
        /// TreeViewItem), to indicate how to display items from the next level
        /// in the data hierarchy.
        /// </summary>
        private DataTemplate _itemTemplate;

        /// <summary>
        /// Gets a value indicating whether the ItemTemplate property was set on
        /// the template.
        /// </summary>
        internal bool IsItemTemplateSet { get; private set; }

        /// <summary>
        /// Gets or sets the DataTemplate to apply to the ItemTemplate property
        /// on a generated HeaderedItemsControl (such as a MenuItem or a
        /// TreeViewItem), to indicate how to display items from the next level
        /// in the data hierarchy.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return _itemTemplate; }
            set
            {
                IsItemTemplateSet = true;
                _itemTemplate = value;
            }
        }
        #endregion public DataTemplate ItemTemplate

        #region public Style ItemContainerStyle
        /// <summary>
        /// The Style to apply to the ItemContainerStyle property on a generated
        /// HeaderedItemsControl (such as a MenuItem or a TreeViewItem), to
        /// indicate how to style items from the next level in the data
        /// hierarchy.
        /// </summary>
        private Style _itemContainerStyle;

        /// <summary>
        /// Gets a value indicating whether the ItemContainerStyle property was
        /// set on the template.
        /// </summary>
        internal bool IsItemContainerStyleSet { get; private set; }

        /// <summary>
        /// Gets or sets the Style that is applied to the item container for each child item.
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return _itemContainerStyle; }
            set
            {
                IsItemContainerStyleSet = true;
                _itemContainerStyle = value;
            }
        }
        #endregion public Style ItemContainerStyle

        /// <summary>
        /// Initializes a new instance of the HierarchicalDataTemplate class.
        /// </summary>
        public HierarchicalDataTemplate()
        {
        }
    }
}