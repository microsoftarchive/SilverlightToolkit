// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents a node within a tree with value, name and belonging to a particular data segment.
    /// </summary>
    public class SegmentNode
    {
        /// <summary>
        /// Gets or sets a value representing the segment to which a node belongs.
        /// </summary>
        public int Segment { get; set; }

        /// <summary>
        /// Gets or sets the primary value associated with the node.
        /// </summary>
        public double Value { get; set; }
        
        /// <summary>
        /// Gets or sets the second value associated with the node.
        /// </summary>
        public double Value2 { get; set; }

        /// <summary>
        /// Gets or sets the name associated with the node.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value representing the children of this division, conference, or league. Empty for teams.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Simplifies samples.")]
        public IList<SegmentNode> Children { get; set; }

        /// <summary>
        /// Gets the desired tooltip content.
        /// </summary>
        public string ToolTip
        {
            get
            {
                StringBuilder outStr = new StringBuilder();
                outStr.Append("Name: ").Append(Name);
                outStr.Append("\nValue: ").Append(Value);
                outStr.Append("\nValue2: ").Append(Value2);
                outStr.Append("\nSegment: ").Append(Segment);
                return outStr.ToString();
            }
        }
    }
}
