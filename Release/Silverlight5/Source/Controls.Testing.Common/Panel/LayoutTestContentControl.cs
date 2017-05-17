// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// A ContentControl that records details about its last layout pass.
    /// </summary>
    public sealed partial class LayoutTestContentControl : ContentControl
    {
        /// <summary>
        /// Gets the available size suggested during the last measure pass.
        /// </summary>
        public Size SuggestedAvailableSize { get; private set; }

        /// <summary>
        /// Gets the final size suggested during the last arrange pass.
        /// </summary>
        public Size SuggestedFinalSize { get; private set; }

        /// <summary>
        /// Initializes a new instance of the LayoutTestContentControl class.
        /// </summary>
        public LayoutTestContentControl()
            : base()
        {
        }

        /// <summary>
        /// Measures the control.
        /// </summary>
        /// <param name="availableSize">A maximum Size to not exceed.</param>
        /// <returns>The desired size of the control.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            SuggestedAvailableSize = availableSize;
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Arranges the control.
        /// </summary>
        /// <param name="finalSize">The final Size of the control.</param>
        /// <returns>The arranged size of the control.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            SuggestedFinalSize = finalSize;
            return base.ArrangeOverride(finalSize);
        }
    }
}