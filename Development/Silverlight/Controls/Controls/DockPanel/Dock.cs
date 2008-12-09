// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Specifies the Dock position of a child element that is inside a
    /// DockPanel.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public enum Dock
    {
        /// <summary>
        /// A child element that is positioned on the left side of the
        /// DockPanel.
        /// </summary>
        Left = 0,

        /// <summary>
        /// A child element that is positioned at the top of the DockPanel.
        /// </summary>
        Top = 1,
        
        /// <summary>
        /// A child element that is positioned on the right side of the
        /// DockPanel.
        /// </summary>
        Right = 2,

        /// <summary>
        /// A child element that is positioned at the bottom of the DockPanel.
        /// </summary>
        Bottom = 3
    }
}