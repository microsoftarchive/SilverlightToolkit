// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample demonstrating CheckBoxes in a TreeView.
    /// </summary>    
    [Sample("(2)Using CheckBoxes", DifficultyLevel.Scenario, "TreeView")]
    public partial class CheckedTreeViewItemSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the CheckedTreeViewSample class.
        /// </summary>
        public CheckedTreeViewItemSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handle the ItemCheckbox.Click event.
        /// </summary>
        /// <param name="sender">The CheckBox.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called from an event declared in XAML")]
        private void ItemCheckbox_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = GetParentTreeViewItem((DependencyObject)sender);
            if (item != null)
            {
                Feature feature = item.DataContext as Feature;
                if (feature != null)
                {
                    UpdateChildrenCheckedState(feature);
                    UpdateParentCheckedState(item);
                }
            }
        }

        /// <summary>
        /// Gets the parent TreeViewItem of the passed in dependancy object.
        /// </summary>
        /// <param name="item">Item whose parent to wish to find.</param>
        /// <returns>
        /// If item is a TreeViewItem then returns its parent TreeViewItem,
        /// else returns the TreeViewItem containing the item.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called from an event declared in XAML")]
        private static TreeViewItem GetParentTreeViewItem(DependencyObject item)
        {
            if (item != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(item);
                TreeViewItem parentTreeViewItem = parent as TreeViewItem;
                return (parentTreeViewItem != null) ? parentTreeViewItem : GetParentTreeViewItem(parent);
            }
            return null;
        }

        /// <summary>
        /// Sets the Feature bound to the item's parent to the combined
        /// check state of all the children.
        /// </summary>
        /// <param name="item">Item whose parent should be adjust.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called from an event declared in XAML")]
        private static void UpdateParentCheckedState(TreeViewItem item)
        {
            TreeViewItem parent = GetParentTreeViewItem(item);
            if (parent != null)
            {
                Feature feature = parent.DataContext as Feature;
                if (feature != null)
                {
                    // Get the combined checked state of all the children,
                    // determing if they're all checked, all unchecked or a
                    // combination.
                    bool? childrenCheckedState = feature.Subcomponents.First<Feature>().ShouldInstall;
                    for (int i = 1; i < feature.Subcomponents.Count(); i++)
                    {
                        if (childrenCheckedState != feature.Subcomponents[i].ShouldInstall)
                        {
                            childrenCheckedState = null;
                            break;
                        }
                    }

                    // Set the parent to the combined state of the children.
                    feature.ShouldInstall = childrenCheckedState;

                    // Continue up the tree updating each parent with the
                    // correct combined state.
                    UpdateParentCheckedState(parent);
                }
            }
        }

        /// <summary>
        /// Sets the feature's children checked states, including subcomponents,
        /// to match the state of feature.
        /// </summary>
        /// <param name="feature">Feature whose children should be set.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called from an event declared in XAML")]
        private static void UpdateChildrenCheckedState(Feature feature)
        {
            if (feature.ShouldInstall.HasValue)
            {
                foreach (Feature childFeature in feature.Subcomponents)
                {
                    childFeature.ShouldInstall = feature.ShouldInstall;
                    if (childFeature.Subcomponents.Count() > 0)
                    {
                        UpdateChildrenCheckedState(childFeature);
                    }
                }
            }
        }
    }
}