// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Collections.Generic;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample showing how to use different item definitions per branch of a tree.
    /// </summary>
    [Sample("(6) Segmented TreeMap", DifficultyLevel.Advanced, "TreeMap")]
    public partial class SegmentedSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the SegmentedSample class.
        /// </summary>
        public SegmentedSample()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SegmentedSample_Loaded);
        }

        /// <summary>
        /// Generate a tree with a given segment ID.
        /// </summary>
        /// <param name="depth">The depth of the tree.</param>
        /// <param name="maxChildren">The maximum number of children nodes allowed.</param>
        /// <param name="maxValue"> The maximum value allowed for the node's metrics.</param>
        /// <param name="segmentID">The ID to segment to which the leaves belong.</param>
        /// <param name="name">The name of the node (for making node names).</param>
        /// <param name="random">A random number generator for controlling tree generation.</param>
        /// <returns>A SegmentNode representing the root of the tree.</returns>
        private SegmentNode GenerateTree(int depth, int maxChildren, int maxValue, int segmentID, string name, Random random)
        {
            SegmentNode node = new SegmentNode();
            node.Name = name;

            if (depth <= 0)
            {
                node.Value = random.Next(1, maxValue);
                node.Value2 = random.Next(1, maxValue);
                node.Segment = segmentID;
                node.Children = new SegmentNode[0];
            }
            else
            {
                int numChildren = random.Next(2, maxChildren);
                
                node.Children = new List<SegmentNode>();
                for (int i = 0; i < numChildren; i++)
                {
                    node.Children.Add(GenerateTree(depth - 1, maxChildren, maxChildren, segmentID, name + "." + i, random));
                }
            }

            return node;
        }

        /// <summary>
        /// Loads the XML sample data and populates the TreeMap.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void SegmentedSample_Loaded(object sender, RoutedEventArgs e)
        {
            // Sample browser-specific layout change.
            SampleHelpers.ChangeSampleAlignmentToStretch(this);

            // Construct the tree.
            Random r = new Random();
            treeMapControl.ItemsSource = new List<SegmentNode>()
            {
                GenerateTree(2, 9, 10, 1, "A", r),
                GenerateTree(2, 7, 15, 2, "B", r),
                GenerateTree(2, 5, 20, 3, "C", r),
            };
        }
    }
}

