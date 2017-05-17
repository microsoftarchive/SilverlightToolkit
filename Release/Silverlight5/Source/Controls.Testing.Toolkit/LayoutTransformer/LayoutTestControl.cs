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
    /// Implements a test-friendly Control that requests the dimensions specified.
    /// </summary>
    public class LayoutTestControl : Control
    {
        /// <summary>
        /// Preferred size of the control.
        /// </summary>
        private Size _preferredSize;

        /// <summary>
        /// Whether the Control should force its preferred size during Measure.
        /// </summary>
        private bool _measureAtPreferredSize;

        /// <summary>
        /// Whether the Control should force its preferred size during Arrange.
        /// </summary>
        private bool _arrangeAtPreferredSize;

        /// <summary>
        /// Initializes a new instance of the TestControl class.
        /// </summary>
        /// <param name="preferredSize">Desired size.</param>
        /// <param name="measureAtPreferredSize">Whether the child control should force its preferred size during Measure.</param>
        /// <param name="arrangeAtPreferredSize">Whether the child control should force its preferred size during Arrange.</param>
        public LayoutTestControl(Size preferredSize, bool measureAtPreferredSize, bool arrangeAtPreferredSize)
        {
            _preferredSize = preferredSize;
            _measureAtPreferredSize = measureAtPreferredSize;
            _arrangeAtPreferredSize = arrangeAtPreferredSize;
        }

        /// <summary>
        /// Overrides the MeasureOverride method to return the desired size.
        /// </summary>
        /// <param name="availableSize">Constraining size to measure against.</param>
        /// <returns>Desired size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_measureAtPreferredSize)
            {
                return _preferredSize;
            }
            else
            {
                return new Size(Math.Min(availableSize.Width, _preferredSize.Width), Math.Min(availableSize.Height, _preferredSize.Height));
            }
        }

        /// <summary>
        /// Overrides the ArrangeOverride method to return the used size.
        /// </summary>
        /// <param name="finalSize">Constraining bounds to arrange within.</param>
        /// <returns>Render size.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_arrangeAtPreferredSize)
            {
                return new Size(Math.Max(finalSize.Width, _preferredSize.Width), Math.Max(finalSize.Height, _preferredSize.Height));
            }
            else
            {
                return finalSize;
            }
        }
    }
}