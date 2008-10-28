// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// The ObjectHierarchy type is used as a sample hierarchical object to test
    /// HierarchicalDataTemplate.
    /// </summary>
    [ContentProperty("Children")]
    public class Hierarchy
    {
        /// <summary>
        /// Gets or sets the element name.
        /// </summary>
        public string Element { get; set; }

        /// <summary>
        /// Gets or sets the children of the element.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Required for easy initialization")]
        public Collection<Hierarchy> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the Hierarchy class.
        /// </summary>
        public Hierarchy()
        {
            Children = new Collection<Hierarchy>();
        }
    }
}