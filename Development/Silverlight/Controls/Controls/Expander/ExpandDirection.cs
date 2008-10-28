// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Specifies the direction in which an Expander control opens.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    /// <remarks>
    /// To specify the direction that an Expander control expands, 
    /// set the ExpandDirection property to one of the enumeration members.
    /// </remarks>
    public enum ExpandDirection
    {
        /// <summary>
        /// Expander will expand to the down direction.
        /// </summary>
        Down = 0,

        /// <summary>
        /// Expander will expand to the up direction.
        /// </summary>
        Up = 1,
        
        /// <summary>
        /// Expander will expand to the left direction.
        /// </summary>
        Left = 2,
        
        /// <summary>
        /// Expander will expand to the right direction.
        /// </summary>
        Right = 3,
    }
}
