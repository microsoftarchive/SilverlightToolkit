// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Stores the information for a single LayoutTransformer test scenario.
    /// </summary>
    public class LayoutTransformerScenario
    {
        /// <summary>
        /// Gets the preferred size of the test control.
        /// </summary>
        public Size PreferredSize { get; private set; }

        /// <summary>
        /// Gets the size to pass in to the call to Measure.
        /// </summary>
        public Size MeasureSize { get; private set; }

        /// <summary>
        /// Gets the expected DesiredSize after the call to Measure.
        /// </summary>
        public Size DesiredSize { get; private set; }

        /// <summary>
        /// Gets the size to pass in to the call to Arrange.
        /// </summary>
        public Size ArrangeSize { get; private set; }

        /// <summary>
        /// Gets the expected RenderSize after the call to Arrange.
        /// </summary>
        public Size RenderSize { get; private set; }

        /// <summary>
        /// Gets the Transform to use for the scenario.
        /// </summary>
        public Transform Transform { get; private set; }

        /// <summary>
        /// Gets the child Control to use for the scenario.
        /// </summary>
        public Control Child
        {
            get
            {
                return new LayoutTestControl(PreferredSize, _measureAtPreferredSize, _arrangeAtPreferredSize);
            }
        }

        /// <summary>
        /// Whether the child Control should force its preferred size during Measure.
        /// </summary>
        private bool _measureAtPreferredSize;

        /// <summary>
        /// Whether the child Control should force its preferred size during Arrange.
        /// </summary>
        private bool _arrangeAtPreferredSize;

        /// <summary>
        /// Initializes a new instance of the LayoutTransformerScenario class.
        /// </summary>
        /// <param name="preferredWidth">Preferred width of the test control.</param>
        /// <param name="preferredHeight">Preferred height of the test control.</param>
        /// <param name="measureAtPreferredSize">Whether the child control should force its preferred size during Measure.</param>
        /// <param name="arrangeAtPreferredSize">Whether the child control should force its preferred size during Arrange.</param>
        /// <param name="measureWidth">Width to pass to Measure.</param>
        /// <param name="measureHeight">Height to pass to Measure.</param>
        /// <param name="desiredWidth">Expected DesiredSize.Width.</param>
        /// <param name="desiredHeight">Expected DesiredSize.Height.</param>
        /// <param name="arrangeWidth">Width to pass to Arrange.</param>
        /// <param name="arrangeHeight">Height to pass to Arrange.</param>
        /// <param name="renderWidth">Expected RenderSize.Width.</param>
        /// <param name="renderHeight">Expected RenderSize.Height.</param>
        /// <param name="transform">Transform to use.</param>
        public LayoutTransformerScenario(double preferredWidth, double preferredHeight, bool measureAtPreferredSize, bool arrangeAtPreferredSize, double measureWidth, double measureHeight, double desiredWidth, double desiredHeight, double arrangeWidth, double arrangeHeight, double renderWidth, double renderHeight, Transform transform)
        {
            PreferredSize = new Size(preferredWidth, preferredHeight);
            _measureAtPreferredSize = measureAtPreferredSize;
            _arrangeAtPreferredSize = arrangeAtPreferredSize;
            MeasureSize = new Size(measureWidth, measureHeight);
            DesiredSize = new Size(desiredWidth, desiredHeight);
            ArrangeSize = new Size(arrangeWidth, arrangeHeight);
            RenderSize = new Size(renderWidth, renderHeight);
            Transform = transform;
        }
    }
}