// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.Controls
{
    /// <summary>
    /// Specifies values that control the behavior of a control positioned
    /// inside another control.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public enum Dock
    {
        /// <summary>
        /// Specifies that the control should be positioned on the left of the
        /// control.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Specifies that the control should be positioned on top of the
        /// control.
        /// </summary>
        Top = 1,
        
        /// <summary>
        /// Specifies that the control should be positioned on the right of the
        /// control.
        /// </summary>
        Right = 2,

        /// <summary>
        /// Specifies that the control should be positioned at the bottom of 
        /// control.
        /// </summary>
        Bottom = 3
    }
}