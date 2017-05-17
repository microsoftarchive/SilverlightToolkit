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
    /// Dynamically chooses a TreeMapItemDefinitionSelector based on segment number in the node.
    /// </summary>
    [ContentProperty("Children")]
    public class SegmentItemDefinitionSelector : TreeMapItemDefinitionSelector
    {
        /// <summary>
        /// Gets the list of templates that this selector will choose from.
        /// </summary>
        public Collection<TreeMapItemDefinition> Children { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentItemDefinitionSelector"/> class. 
        /// </summary>
        public SegmentItemDefinitionSelector()
        {
            Children = new Collection<TreeMapItemDefinition>();
        }

        /// <summary>
        /// Returns an instance of a SegmentItemDefinitionSelector class used to specify properties for the 
        /// current item. 
        /// </summary>
        /// <param name="treeMap">Reference to the TreeMap class.</param>
        /// <param name="item">One of the nodes in the ItemsSource hierarchy.</param>
        /// <param name="level">The level of the node in the hierarchy.</param>
        /// <returns>The TreeMapItemDefinition to be used for this node. If this method returns null 
        /// the TreeMap will use the value of its ItemDefinition property.</returns>
        public override TreeMapItemDefinition SelectItemDefinition(TreeMap treeMap, object item, int level)
        {
            SegmentNode node = item as SegmentNode;

            if (Children.Count > 0 && node != null && node.Segment >= 0)
            {
                int child = node.Segment % Children.Count;
                return Children[child];
            }
            else
            {
                return null;
            }
        }
    }
}
