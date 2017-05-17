// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.DataVisualization;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Overridden TreeMapItemDefinitionSelector that provides access to virtual members for
    /// testing.
    /// </summary>
    public class OverriddenTreeMapItemDefinitionSelector : TreeMapItemDefinitionSelector
    {
        /// <summary>
        /// Initializes a new instance of the OverriddenTreeMapItemDefinitionSelector class.
        /// </summary>
        public OverriddenTreeMapItemDefinitionSelector()
        {
        }

        /// <summary>
        /// Returns an instance of a TreeMapItemDefinition class used to specify properties for the 
        /// current item. 
        /// </summary>
        /// <param name="treeMap">Reference to the TreeMap class.</param>
        /// <param name="item">One of the nodes in the ItemsSource hierarchy.</param>
        /// <param name="level">The level of the node in the hierarchy.</param>
        /// <returns>The TreeMapItemDefinition to be used for this node. If this method returns null 
        /// the TreeMap will use the value of its ItemTemplate property.</returns>
        public override TreeMapItemDefinition SelectItemDefinition(TreeMap treeMap, object item, int level)
        {
            throw new NotImplementedException();
        }
    }
}
