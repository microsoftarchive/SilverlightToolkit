// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample showing how an ItemDefinitionSelector can be used to return a different
    /// template for each level in the tree.
    /// </summary>
    [Sample("(1)ItemDefinitionSelector", DifficultyLevel.Intermediate, "TreeMap")]
    public partial class FlexibleTemplateSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the FlexibleTemplateSample class.
        /// </summary>
        public FlexibleTemplateSample()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FlexibleTemplateSample_Loaded);
        }

        /// <summary>
        /// Loads the XML sample data and populates the TreeMap.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void FlexibleTemplateSample_Loaded(object sender, RoutedEventArgs e)
        {
            // Sample browser-specific layout change
            SampleHelpers.ChangeSampleAlignmentToStretch(this);

            treeMapControl.ItemsSource = NhlDataHelper.LoadDefaultFile();
        }
    }
}
