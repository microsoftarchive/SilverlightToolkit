// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Windows.Controls.DataVisualization;
using System.Windows.Markup;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Dynamically chooses a TreeMapItemDefintion based on an item's level in the hierarchy.
    /// </summary>
    [ContentProperty("Children")]
    public class AlternatingItemDefinitionSelector : TreeMapItemDefinitionSelector
    {
        /// <summary>
        /// Gets the list of templates that this selector will choose from.
        /// </summary>
        public Collection<TreeMapItemDefinition> Children { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlternatingItemDefinitionSelector"/> class. 
        /// </summary>
        public AlternatingItemDefinitionSelector()
        {
            Children = new Collection<TreeMapItemDefinition>();
        }

        /// <summary>
        /// Returns an instance of a TreeMapItemDefinition class used to specify properties for the 
        /// current item. 
        /// </summary>
        /// <param name="treeMap">Reference to the TreeMap class.</param>
        /// <param name="item">One of the nodes in the ItemsSource hierarchy.</param>
        /// <param name="level">The level of the node in the hierarchy.</param>
        /// <returns>The TreeMapItemDefinition to be used for this node. If this method returns null 
        /// the TreeMap will use the value of its ItemDefinition property.</returns>
        public override TreeMapItemDefinition SelectItemDefinition(TreeMap treeMap, object item, int level)
        {
            if (Children.Count > 0)
            {
                int child = level % Children.Count;
                return Children[child];
            }
            else
            {
                return null;
            }
        }
    }
}
