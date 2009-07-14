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
using System.Windows.Media;
using System.Windows.Shapes;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// The SampleBrowserItem class represents an individual sample in tab control
    /// samples. 
    /// </summary>
    public class SampleBrowserItem
    {
        /// <summary>
        /// Gets or sets the name of the sample.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the sample to use for sorting.
        /// </summary>
        private string OriginalName { get; set; }

        /// <summary>
        /// Gets or sets the sample type that was declared using the SampleAttribute.
        /// </summary>
        public Type SampleType { get; set; }

        /// <summary>
        /// Gets or sets the level difficulty of the sample.
        /// </summary>
        public DifficultyLevel SampleLevel { get; set; }

        /// <summary>
        /// Initializes a new instance of the SampleBrowserItem class.
        /// </summary>
        public SampleBrowserItem()
        {       
        }

        /// <summary>
        /// Overrides the ToString method to return the Name of the Sample.
        /// </summary>
        /// <returns>Returns the Name of the sample.</returns>
        public override string ToString()
        {
            return Name;
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
                FrameworkElement sample = Activator.CreateInstance(SampleType) as FrameworkElement;
                sample.VerticalAlignment = VerticalAlignment.Top;
                sample.HorizontalAlignment = HorizontalAlignment.Left;
                sample.Margin = new Thickness(10, 0, 0, 0);
                return sample;
            }
            return null;
        }

        /// <summary>
        /// Create a SampleBrowserItem and inserts into the right location.
        /// </summary>
        /// <param name="Samples">Collections of Samples.</param>
        /// <param name="type">Type of Sample.</param>
        /// <param name="attribute">Attributes for the Sample.</param>
        internal static void AddSample(IList<SampleBrowserItem> Samples, Type type, SampleAttribute attribute)
        {
            string name = attribute.Name;
            // Find any existing item
            SampleBrowserItem item = null;
            if (Samples != null)
            {
                foreach (SampleBrowserItem other in Samples)
                {
                    if (string.Compare(other.OriginalName, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        item = other;
                        break;
                    }
                }
            }
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

            item = new SampleBrowserItem { Name = escapedName, OriginalName = name, SampleType = type, SampleLevel = attribute.DifficultyLevel };

            // Insert the item into the list in sorted order (linearly
            // because no item should have very many children)
            int sortedIndex = 0;
            foreach (SampleBrowserItem other in Samples)
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
            Samples.Insert(sortedIndex, item);       
        }
    }
}