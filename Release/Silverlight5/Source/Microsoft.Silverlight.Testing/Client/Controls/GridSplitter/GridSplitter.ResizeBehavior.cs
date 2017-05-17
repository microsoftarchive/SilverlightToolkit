// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows;

[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Silverlight.Testing.Controls.GridSplitter+ResizeData.#OriginalDefinition1Length", Justification = "Inherited mature control.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.Silverlight.Testing.Controls.GridSplitter+ResizeData.#OriginalDefinition2Length", Justification = "Inherited mature control.")]

namespace Microsoft.Silverlight.Testing.Controls
{
    /// <summary>
    /// Represents the control that redistributes space between columns or rows
    /// of a Grid control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class GridSplitter : Control
    {
        /// <summary>
        /// Type to hold the data for the resize operation in progress.
        /// </summary>
        /// <QualityBand>Mature</QualityBand>
        internal class ResizeData
        {
            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public PreviewControl PreviewControl { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public DefinitionAbstraction Definition1 { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public int Definition1Index { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public DefinitionAbstraction Definition2 { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public int Definition2Index { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public Grid Grid { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public double MaxChange { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public double MinChange { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public double OriginalDefinition1ActualLength { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public GridLength OriginalDefinition1Length { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public double OriginalDefinition2ActualLength { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public GridLength OriginalDefinition2Length { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public GridResizeBehavior ResizeBehavior { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public GridResizeDirection ResizeDirection { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether Inherited code: Requires comment.
            /// </summary>
            public bool ShowsPreview { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public GridSplitter.SplitBehavior SplitBehavior { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public int SplitterIndex { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public double SplitterLength { get; set; }
        }
    }
}