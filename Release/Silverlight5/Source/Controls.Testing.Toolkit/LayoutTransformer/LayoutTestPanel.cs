// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Implements a test-friendly Panel that offers the dimensions specified.
    /// </summary>
    public class LayoutTestPanel : Panel
    {
        /// <summary>
        /// Size to offer during measure.
        /// </summary>
        private Size _measureSize;

        /// <summary>
        /// Size to offer during arrange.
        /// </summary>
        private Size _arrangeSize;

        /// <summary>
        /// Initializes a new instance of the TestPanel class.
        /// </summary>
        /// <param name="measureSize">Size to offer during measure.</param>
        /// <param name="arrangeSize">Size to offer during arrange.</param>
        public LayoutTestPanel(Size measureSize, Size arrangeSize)
        {
            _measureSize = measureSize;
            _arrangeSize = arrangeSize;
        }

        /// <summary>
        /// Overrides MeasureOverride to offer the specified size.
        /// </summary>
        /// <param name="availableSize">Available size.</param>
        /// <returns>Measured size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement c in Children)
            {
                c.Measure(_measureSize);
            }
            return availableSize;
        }

        /// <summary>
        /// Overrides ArrangeOverride to offer the specified size.
        /// </summary>
        /// <param name="finalSize">Final size.</param>
        /// <returns>Arranged size.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement c in Children)
            {
                c.Arrange(new Rect(new Point(), _arrangeSize));
            }
            return finalSize;
        }
    }
}