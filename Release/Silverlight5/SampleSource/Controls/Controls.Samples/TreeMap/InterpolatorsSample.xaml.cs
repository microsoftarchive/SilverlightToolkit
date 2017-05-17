// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "System.Windows.Controls.Samples.InterpolatorsSample.#itemBorder", Justification = "Artifact of using a name inside the custom control template.")]

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Example showing how to create and use Interpolators with for TreeMap. An interpolator 
    /// calculates the minimum and maximum values for the bound property across the whole tree, and
    /// then projects the value in the specified [From, To] range.
    /// </summary>
    [Sample("(2)Interpolators", DifficultyLevel.Basic, "TreeMap")]
    public partial class InterpolatorsSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the InterpolatorsSample class.
        /// </summary>
        public InterpolatorsSample()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(InterpolatorsSample_Loaded);
        }

        /// <summary>
        /// Loads the XML sample data and populates the TreeMap.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void InterpolatorsSample_Loaded(object sender, RoutedEventArgs e)
        {
            // Sample browser-specific layout change
            SampleHelpers.ChangeSampleAlignmentToStretch(this);

            treeMapControl.ItemsSource = NhlDataHelper.LoadDefaultFile();
        }
    }
}
