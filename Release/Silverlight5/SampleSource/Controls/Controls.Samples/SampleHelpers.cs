// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Media;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Helper class for general sample-related utility methods.
    /// </summary>
    internal static class SampleHelpers
    {
        /// <summary>
        /// Changes the alignment of a sample to Stretch so that its contents
        /// will automatically fill the available area.
        /// </summary>
        /// <param name="sample">Sample control to modify.</param>
        public static void ChangeSampleAlignmentToStretch(UserControl sample)
        {
            // Set sample to stretch
            sample.HorizontalAlignment = HorizontalAlignment.Stretch;
            sample.VerticalAlignment = VerticalAlignment.Stretch;

            // Find ContentPresenter parent (i.e., TabItem in Silverlight world)...
            FrameworkElement parent = sample;
            while (parent != null)
            {
                ContentPresenter tabItem = parent as ContentPresenter;
                if (tabItem != null)
                {
                    // Set to stretch as well
                    tabItem.HorizontalAlignment = HorizontalAlignment.Stretch;
                    tabItem.VerticalAlignment = VerticalAlignment.Stretch;
                    break;
                }

                // Move up a level
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }
        }
    }
}