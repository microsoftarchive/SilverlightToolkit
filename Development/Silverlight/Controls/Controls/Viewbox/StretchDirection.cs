// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Describes how scaling applies to content and restricts scaling to named
    /// axis types. 
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum StretchDirection
    {
        /// <summary>
        /// The content scales upward only when it is smaller than the parent.
        /// If the content is larger, no scaling downward is performed.
        /// </summary>
        UpOnly = 0,

        /// <summary>
        /// The content scales downward only when it is larger than the parent.
        /// If the content is smaller, no scaling upward is performed.
        /// </summary>
        DownOnly = 1,

        /// <summary>
        /// The content stretches to fit the parent according to the Stretch
        /// mode.
        /// </summary>
        Both = 2
    }
}