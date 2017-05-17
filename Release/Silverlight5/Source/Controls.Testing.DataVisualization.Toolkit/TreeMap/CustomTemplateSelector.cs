// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.DataVisualization;
using System.Windows.Markup;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Dynamically chooses from three templates: one for level 0 (root), another one
    /// for level 1, and a third one for all the other levels.
    /// </summary>
    [ContentProperty("Children")]
    public class SampleTemplateSelector : TreeMapItemDefinitionSelector
    {
        /// <summary>
        /// Gets the collection of child templates to iterate over.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For tests only")]
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "For tests only")] 
        public Collection<TreeMapItemDefinition> Children { get; private set; }

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
            TreeMapItemDefinition template = null;
            switch (level)
            {
                case 0:
                    template = Children[0];
                    break;

                case 1:
                    template = Children[1];
                    break;

                default:
                    template = Children[2];
                    break;
            }

            return template;
        }
    }
}