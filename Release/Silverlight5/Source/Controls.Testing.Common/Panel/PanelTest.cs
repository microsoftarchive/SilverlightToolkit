// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Base class used to author unit tests for layout containers.
    /// </summary>
    public abstract partial class PanelTest : FrameworkElementTest
    {
        #region FrameworkElements to test
        /// <summary>
        /// Gets a default instance of FrameworkElement (or a derived type) to
        /// test.
        /// </summary>
        public override FrameworkElement DefaultFrameworkElementToTest
        {
            get { return DefaultPanelToTest; }
        }

        /// <summary>
        /// Gets instances of FrameworkElement (or derived types) to test.
        /// </summary>
        public override IEnumerable<FrameworkElement> FrameworkElementsToTest
        {
            get { return PanelsToTest.OfType<FrameworkElement>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenFrameworkElement (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenFrameworkElement> OverriddenFrameworkElementsToTest
        {
            get { return OverriddenPanelsToTest.OfType<IOverriddenFrameworkElement>(); }
        }
        #endregion FrameworkElements to test

        #region Panels to test
        /// <summary>
        /// Gets a default instance of Panel (or a derived type) to test.
        /// </summary>
        public abstract Panel DefaultPanelToTest { get; }

        /// <summary>
        /// Gets instances of Panel (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<Panel> PanelsToTest { get; }

        /// <summary>
        /// Gets instances of IOverriddenPanel (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<IOverriddenPanel> OverriddenPanelsToTest { get; }
        #endregion Panels to test

        /// <summary>
        /// Initializes a new instance of the PanelTest class.
        /// </summary>
        protected PanelTest()
            : base()
        {
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // Remove the tests requiring templates inherited from properties
            // on FrameworkElementTest
            tests.RemoveTests(HorizontalAlignmentProperty.TemplateBindTest);
            tests.RemoveTests(HorizontalAlignmentProperty.DoesNotChangeVisualStateTest(HorizontalAlignment.Left, HorizontalAlignment.Right));
            tests.RemoveTests(HorizontalAlignmentProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.RemoveTests(VerticalAlignmentProperty.TemplateBindTest);
            tests.RemoveTests(VerticalAlignmentProperty.DoesNotChangeVisualStateTest(VerticalAlignment.Bottom, VerticalAlignment.Center));
            tests.RemoveTests(VerticalAlignmentProperty.InvalidValueDoesNotChangeVisualStateTest);

            return tests;
        }

        #region TestLayoutAsync
        /// <summary>
        /// Create a layout test that adds a series of elements to a panel and
        /// then verifies their positions once they have all been added.  An
        /// element will be created and added for each function used to compute
        /// a position.
        /// </summary>
        /// <param name="panel">Panel to test.</param>
        /// <param name="computePositions">
        /// Functions to compute the final positions of the elements that are
        /// added to the panel.  The functions can also customize the element
        /// (by setting size, etc.) before returning the desired final position.
        /// </param>
        public void TestLayoutAsync(Panel panel, params Func<ContentControl, Rect>[] computePositions)
        {
            // If the panel is not contained by anything, use it as the root
            TestLayoutAsync(panel, panel, computePositions);
        }

        /// <summary>
        /// Create a layout test that adds a series of elements to a panel and
        /// then verifies their positions once they have all been added.  An
        /// element will be created and added for each function used to compute
        /// a position.
        /// </summary>
        /// <param name="root">Root object containing the panel to test.</param>
        /// <param name="panel">Panel to test.</param>
        /// <param name="computePositions">
        /// Functions to compute the final positions of the elements that are
        /// added to the panel.  The functions can also customize the element
        /// (by setting size, etc.) before returning the desired final position.
        /// </param>
        public void TestLayoutAsync(UIElement root, Panel panel, params Func<ContentControl, Rect>[] computePositions)
        {
            Assert.IsNotNull(root);
            Assert.IsNotNull(panel);
            computePositions = computePositions ?? new Func<ContentControl, Rect>[] { };

            // Add a handler to determine when the panel is loaded (note: we
            // check the panel instead of the root as it's deeper in the tree)
            bool isLoaded = false;
            panel.Loaded += delegate { isLoaded = true; };

            // Add the root to the test surface and wait until its child Panel
            // is loaded before testing
            EnqueueCallback(() => TestPanel.Children.Add(root));
            EnqueueConditional(() => isLoaded);

            // Add the elements to the Panel
            List<Rect> positions = new List<Rect>();
            int count = 0;
            foreach (Func<ContentControl, Rect> computePosition in computePositions)
            {
                LayoutTestContentControl control = new LayoutTestContentControl { Content = count++ };
                panel.Children.Add(control);
                positions.Add(computePosition(control));
            }
            Assert.AreEqual(computePositions.Length, positions.Count);

            // Allow the visual tree to refresh before verifying positions
            EnqueueVisualDelay(DefaultVisualDelayInMilliseconds);

            // Verify the final positions of the elements
            EnqueueCallback(() =>
                {
                    for (int i = 0; i < positions.Count; i++)
                    {
                        LayoutTestContentControl element = panel.Children[i] as LayoutTestContentControl;
                        Rect desired = positions[i];
                        Point offset = element.TransformToVisual(panel).Transform(new Point(0, 0));
                        Rect actual = new Rect(offset.X, offset.Y, element.ActualWidth, element.ActualHeight);
                        Assert.AreEqual(desired, actual, "Element {0} expected to be arranged at <{1}> instead of <{2}>.", i, desired, actual);
                    }
                });

            // Remove the panel from the test surface and finish the test
            EnqueueCallback(() => TestPanel.Children.Remove(root));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Prepare a Panel to be used in a layout test.
        /// </summary>
        /// <param name="panel">Panel to prepare.</param>
        /// <returns>The prepared Panel.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design.")]
        protected virtual Panel Prepare(Panel panel)
        {
            if (panel.Style == null)
            {
                panel.Style = Application.Current.Resources["LayoutTestPanel"] as Style;
            }
            return panel;
        }

        /// <summary>
        /// Prepare a Panel to be used in a layout test.
        /// </summary>
        /// <param name="panel">Panel to prepare.</param>
        /// <param name="width">Width of the control.</param>
        /// <param name="height">Height of the control.</param>
        /// <returns>The prepared Panel.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design.")]
        protected virtual Panel Prepare(Panel panel, double width, double height)
        {
            Prepare(panel);
            panel.Width = width;
            panel.Height = height;
            return panel;
        }

        /// <summary>
        /// Prepare a ContentControl to be used in a layout test.
        /// </summary>
        /// <param name="control">Control to prepare.</param>
        /// <param name="minWidth">Minimum width of the control.</param>
        /// <param name="minHeight">Minimum height of the control.</param>
        /// <returns>The prepared Control.</returns>
        protected virtual ContentControl Prepare(ContentControl control, double minWidth, double minHeight)
        {
            if (control.Style == null)
            {
                control.Style = Application.Current.Resources["LayoutTestControl"] as Style;
            }
            control.MinWidth = minWidth;
            control.MinHeight = minHeight;
            return control;
        }
        #endregion TestLayoutAsync
    }
}