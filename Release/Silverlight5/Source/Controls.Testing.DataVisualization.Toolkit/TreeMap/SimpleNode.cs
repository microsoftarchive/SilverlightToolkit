// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Represents ts imple node structure used for tests.
    /// </summary>
    public class SimpleNode
    {
        /// <summary>
        /// Gets or sets a value representing the name of the entity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value representing the number of points associated 
        /// with a node.
        /// </summary>
        public double Points { get; set; }

        /// <summary>
        /// Gets or sets a value representing the number of points associated 
        /// with a node.
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Gets or sets a value representing the children of this a node.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Simplifies tests.")]
        public IList<SimpleNode> Children { get; set; }
    }
}
