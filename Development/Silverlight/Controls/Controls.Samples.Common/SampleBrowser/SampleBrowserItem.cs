// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// The SampleBrowserItem class represents an individual sample in a tree of
    /// samples.  It is built from the paths provided by SampleAttributes.
    /// </summary>
    public class SampleBrowserItem
    {
        /// <summary>
        /// Gets the name of the sample.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the name of the sample to use for sorting.
        /// </summary>
        private string OriginalName { get; set; }

        /// <summary>
        /// Gets the sample type that was declared using the SampleAttribute.
        /// </summary>
        public Type SampleType { get; private set; }

        /// <summary>
        /// Gets the parent sample in the tree.
        /// </summary>
        public SampleBrowserItem Parent { get; private set; }

        /// <summary>
        /// Gets the child sample items of this sample.
        /// </summary>
        public IList<SampleBrowserItem> Items { get; private set; }

        /// <summary>
        /// Gets or sets the TreeViewItem associated with this sample.
        /// </summary>
        internal TreeViewItem TreeViewItem { get; set; }

        /// <summary>
        /// Initializes a new instance of the SampleBrowserItem class.
        /// </summary>
        private SampleBrowserItem()
        {
            Items = new List<SampleBrowserItem>();
        }

        /// <summary>
        /// Add a new sample to a collection of a samples.
        /// </summary>
        /// <param name="samples">The collection of samples.</param>
        /// <param name="sampleType">The sample type.</param>
        /// <param name="sampleAttribute">
        /// The SampleAttribute on the sample type.
        /// </param>
        public static void AddSample(IList<SampleBrowserItem> samples, Type sampleType, SampleAttribute sampleAttribute)
        {
            if (samples == null)
            {
                throw new ArgumentNullException("samples");
            }
            if (sampleType == null)
            {
                throw new ArgumentNullException("sampleType");
            }
            if (sampleAttribute == null)
            {
                throw new ArgumentNullException("sampleAttribute");
            }
            if (string.IsNullOrEmpty(sampleAttribute.Path))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "SampleAttribute on type {0} did not define a Path!",
                        sampleType));
            }

            // Get the path parts
            string[] parts = sampleAttribute.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts == null || parts.Length == 0)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "SampleAttribute on type {0} provided an invalid Path '{1}'!",
                        sampleType,
                        sampleAttribute.Path));
            }

            // Navigate down the path to where the sample should be added
            SampleBrowserItem newItem = null;
            SampleBrowserItem lastItem = null;
            IList<SampleBrowserItem> currentItems = samples;
            for (int i = 0; i < parts.Length; i++)
            {
                lastItem = newItem;
                newItem = GetSampleBrowserItem(lastItem, currentItems, parts[i]);
                currentItems = newItem.Items;
            }

            // Provide the details of the current item
            newItem.SampleType = sampleType;
        }

        /// <summary>
        /// Find or create the SampleBrowserItem for the given name in the
        /// current list of items.
        /// </summary>
        /// <param name="parent">The parent of the item to get.</param>
        /// <param name="items">The list containing the item to get.</param>
        /// <param name="name">The name of the item to get.</param>
        /// <returns>The desired SampleBrowserItem.</returns>
        private static SampleBrowserItem GetSampleBrowserItem(SampleBrowserItem parent, IList<SampleBrowserItem> items, string name)
        {
            // Find any existing item
            SampleBrowserItem item = null;
            if (items != null)
            {
                foreach (SampleBrowserItem other in items)
                {
                    if (string.Compare(other.OriginalName, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        item = other;
                        break;
                    }
                }
            }

            // Create the item if not found
            if (item == null)
            {
                // Escape any name sorting
                string escapedName = name;
                if (name[0] == '(')
                {
                    int index = name.IndexOf(')');
                    if (index > 0)
                    {
                        escapedName = name.Substring(index + 1, name.Length - index - 1);
                    }
                }

                item = new SampleBrowserItem { Name = escapedName, OriginalName = name };
                
                // Insert the item into the list in sorted order (linearly
                // because no item should have very many children)
                int sortedIndex = 0;
                foreach (SampleBrowserItem other in items)
                {
                    if (string.Compare(other.OriginalName, name, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        break;
                    }
                    else
                    {
                        sortedIndex++;
                    }
                }
                items.Insert(sortedIndex, item);

                item.Parent = parent;
            }

            return item;
        }

        /// <summary>
        /// Gets the sample represented by this item.
        /// </summary>
        /// <returns>The sample represented by this item.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Creates a new instance each time it is accessed.")]
        public FrameworkElement GetSample()
        {
            if (SampleType != null)
            {
                // Create the sample from the type if we have one
                return Activator.CreateInstance(SampleType) as FrameworkElement;
            }
            else
            {
                // Create a page with links to the samples
                StackPanel root = new StackPanel();
                root.Children.Add(new TextBlock
                {
                    Text = Name,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 10)
                });
                foreach (SampleBrowserItem item in Items)
                {
                    HyperlinkButton button = new HyperlinkButton
                    {
                        Content = item.Name,
                        FontSize = 12,
                        Margin = new Thickness(10, 5, 5, 5),
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    button.Click += item.OnSelectSampleLink;
                    root.Children.Add(button);
                }
                return root;
            }
        }

        /// <summary>
        /// Select a sample using the generated link.
        /// </summary>
        /// <param name="sender">Sample button.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSelectSampleLink(object sender, RoutedEventArgs e)
        {
            // We need to make sure the TreeViewItem is visible to properly
            // select it.  Since it was shown in a description page, its parent
            // must be visible, so we only need to check one level up in the
            // hierarchy.
            if (!Parent.TreeViewItem.IsExpanded)
            {
                Parent.TreeViewItem.IsExpanded = true;
                TreeViewItem.Dispatcher.BeginInvoke(() => TreeViewItem.IsSelected = true);
            }
            else
            {
                TreeViewItem.IsSelected = true;
            }
        }
    }
}