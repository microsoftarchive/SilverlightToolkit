// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the ScrollViewerExtensions.
    /// </summary>
    [TestClass]
    [Tag("ScrollViewerExtensions")]
    [Tag("TreeViewExtensions")]
    public partial class ScrollViewerExtensionsTest : TestBase
    {
        /// <summary>
        /// Number of milliseconds to wait between steps for scrolling to take
        /// place.
        /// </summary>
        private const int ScrollDelay = 25;

        /// <summary>
        /// The number of pixels scrolled in a line.
        /// </summary>
        private const double LineScrollAmount = 16;

        /// <summary>
        /// The number of pixels scrolled in a page in our standard test
        /// scroll viewer.
        /// </summary>
        private const double TestPageScrollAmount = 73;

        /// <summary>
        /// Initializes a new instance of the ScrollViewerExtensionsTest class.
        /// </summary>
        public ScrollViewerExtensionsTest()
        {            
        }

        #region CreateTestScrollViewer
        /// <summary>
        /// Create a ScrollViewer that can be used by the tests.
        /// </summary>
        /// <returns>A ScrollViewer that can be used by the tests.</returns>
        private static ScrollViewer CreateTestScrollViewer()
        {
            LinearGradientBrush brush = new LinearGradientBrush
            {
                StartPoint = new Point(),
                EndPoint = new Point(1, 1)
            };
            brush.GradientStops.Add(new GradientStop { Offset = 0, Color = Colors.Red });
            brush.GradientStops.Add(new GradientStop { Offset = 1, Color = Colors.Blue });

            Canvas panel = new Canvas
            {
                Height = 400,
                Width = 400,
                Background = brush
            };

            return new ScrollViewer
            {
                Height = 100,
                Width = 100,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = panel
            };
        }

        /// <summary>
        /// Create a ScrollViewer that can be used by the tests.
        /// </summary>
        /// <param name="element">Element to place in the ScrollViewer.</param>
        /// <param name="left">Left coordinate of the element.</param>
        /// <param name="top">Top coordinate of the element.</param>
        /// <returns>A ScrollViewer that can be used by the tests.</returns>
        private static ScrollViewer CreateTestScrollViewer(UIElement element, double left, double top)
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            Canvas panel = viewer.Content as Canvas;
            Assert.IsNotNull(panel, "ScrollViewer.Content should be a Canvas!");
            panel.Children.Add(element);
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
            return viewer;
        }
        #endregion CreateTestScrollViewer

        /// <summary>
        /// Determine whether an element is currently in the view of the
        /// ScrollViewer.
        /// </summary>
        /// <param name="viewer">The ScrollViewer.</param>
        /// <param name="element">The element.</param>
        /// <returns>
        /// A value indicating whether the element is currently in the view of
        /// the ScrollViewer.
        /// </returns>
        private static bool IsInView(ScrollViewer viewer, FrameworkElement element)
        {
            Assert.IsNotNull(viewer, "viewer should not be null!");
            Assert.IsNotNull(element, "element should not be null!");

            Rect? itemBounds = element.GetBoundsRelativeTo(viewer);
            if (itemBounds == null)
            {
                return false;
            }

            double viewBottom = viewer.ViewportHeight;
            double viewRight = viewer.ViewportWidth;

            return
                itemBounds.Value.Top >= 0 && itemBounds.Value.Top <= viewBottom &&
                itemBounds.Value.Bottom >= 0 && itemBounds.Value.Bottom <= viewBottom &&
                itemBounds.Value.Left >= 0 && itemBounds.Value.Left <= viewRight &&
                itemBounds.Value.Right >= 0 && itemBounds.Value.Right <= viewRight;
        }

        #region Scroll by Line
        /// <summary>
        /// Verify LineUp throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify LineUp throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "LineUp", Justification = "Test method")]
        public virtual void LineUpThrows()
        {
            ScrollViewer viewer = null;
            viewer.LineUp();
        }

        /// <summary>
        /// Verify LineDown throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify LineDown throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DownThrows", Justification = "Test method")]
        public virtual void LineDownThrows()
        {
            ScrollViewer viewer = null;
            viewer.LineDown();
        }

        /// <summary>
        /// Verify LineLeft throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify LineLeft throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void LineLeftThrows()
        {
            ScrollViewer viewer = null;
            viewer.LineLeft();
        }

        /// <summary>
        /// Verify LineRight throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify LineRight throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void LineRightThrows()
        {
            ScrollViewer viewer = null;
            viewer.LineRight();
        }

        /// <summary>
        /// Verify LineUp moves by a line.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify LineUp moves by a line.")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "LineUp", Justification = "Test method")]
        public virtual void ScrollLineUp()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.LineUp(),
                () => Assert.AreEqual(200 - LineScrollAmount, viewer.VerticalOffset, "LineUp did not scroll vertically!"),
                () => Assert.AreEqual(200, viewer.HorizontalOffset, "LineUp scrolled horizontally!"));
        }

        /// <summary>
        /// Verify LineDown moves by a line.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify LineDown moves by a line.")]
        public virtual void ScrollLineDown()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.LineDown(),
                () => Assert.AreEqual(200 + LineScrollAmount, viewer.VerticalOffset, "LineDown did not scroll vertically!"),
                () => Assert.AreEqual(200, viewer.HorizontalOffset, "LineDown scrolled horizontally!"));
        }

        /// <summary>
        /// Verify LineLeft moves by a line.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify LineLeft moves by a line.")]
        public virtual void ScrollLineLeft()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.LineLeft(),
                () => Assert.AreEqual(200, viewer.VerticalOffset, "LineLeft scrolled vertically!"),
                () => Assert.AreEqual(200 - LineScrollAmount, viewer.HorizontalOffset, "LineLeft did not scroll horizontally!"));
        }

        /// <summary>
        /// Verify LineRight moves by a line.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify LineRight moves by a line.")]
        public virtual void ScrollLineRight()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.LineRight(),
                () => Assert.AreEqual(200, viewer.VerticalOffset, "LineRight scrolled vertically!"),
                () => Assert.AreEqual(200 + LineScrollAmount, viewer.HorizontalOffset, "LineRight did not scroll horizontally!"));
        }
        #endregion Scroll by Line

        #region Scroll by Page
        /// <summary>
        /// Verify PageUp throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify PageUp throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void PageUpThrows()
        {
            ScrollViewer viewer = null;
            viewer.PageUp();
        }

        /// <summary>
        /// Verify PageDown throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify PageDown throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DownThrows", Justification = "Test method")]
        public virtual void PageDownThrows()
        {
            ScrollViewer viewer = null;
            viewer.PageDown();
        }

        /// <summary>
        /// Verify PageLeft throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify PageLeft throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void PageLeftThrows()
        {
            ScrollViewer viewer = null;
            viewer.PageLeft();
        }

        /// <summary>
        /// Verify PageRight throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify PageRight throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void PageRightThrows()
        {
            ScrollViewer viewer = null;
            viewer.PageRight();
        }

        /// <summary>
        /// Verify PageUp moves by a line.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify PageUp moves by a line.")]
        public virtual void ScrollPageUp()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.PageUp(),
                () => Assert.AreEqual(200 - TestPageScrollAmount, viewer.VerticalOffset, "PageUp did not scroll vertically!"),
                () => Assert.AreEqual(200, viewer.HorizontalOffset, "PageUp scrolled horizontally!"));
        }

        /// <summary>
        /// Verify PageDown moves by a line.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify PageDown moves by a line.")]
        public virtual void ScrollPageDown()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.PageDown(),
                () => Assert.AreEqual(200 + TestPageScrollAmount, viewer.VerticalOffset, "PageDown did not scroll vertically!"),
                () => Assert.AreEqual(200, viewer.HorizontalOffset, "PageDown scrolled horizontally!"));
        }

        /// <summary>
        /// Verify PageLeft moves by a line.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify PageLeft moves by a line.")]
        public virtual void ScrollPageLeft()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.PageLeft(),
                () => Assert.AreEqual(200, viewer.VerticalOffset, "PageLeft scrolled vertically!"),
                () => Assert.AreEqual(200 - TestPageScrollAmount, viewer.HorizontalOffset, "PageLeft did not scroll horizontally!"));
        }

        /// <summary>
        /// Verify PageRight moves by a line.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify PageRight moves by a line.")]
        public virtual void ScrollPageRight()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.PageRight(),
                () => Assert.AreEqual(200, viewer.VerticalOffset, "PageRight scrolled vertically!"),
                () => Assert.AreEqual(200 + TestPageScrollAmount, viewer.HorizontalOffset, "PageRight did not scroll horizontally!"));
        }
        #endregion Scroll by Page

        #region Scroll to Top/Bottom/Left/Right
        /// <summary>
        /// Verify ScrollToTop throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify ScrollToTop throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ScrollToTopThrows()
        {
            ScrollViewer viewer = null;
            viewer.ScrollToTop();
        }

        /// <summary>
        /// Verify ScrollToBottom throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify ScrollToBottom throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ScrollToBottomThrows()
        {
            ScrollViewer viewer = null;
            viewer.ScrollToBottom();
        }

        /// <summary>
        /// Verify ScrollToLeft throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify ScrollToLeft throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ScrollToLeftThrows()
        {
            ScrollViewer viewer = null;
            viewer.ScrollToLeft();
        }

        /// <summary>
        /// Verify ScrollToRight throws with a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify ScrollToRight throws with a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ScrollToRightThrows()
        {
            ScrollViewer viewer = null;
            viewer.ScrollToRight();
        }

        /// <summary>
        /// Verify ScrollToTop moves to the top.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify ScrollToTop moves to the top.")]
        public virtual void ScrollToTop()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.ScrollToTop(),
                () => Assert.AreEqual(0, viewer.VerticalOffset, "ScrollToTop did not scroll vertically!"),
                () => Assert.AreEqual(200, viewer.HorizontalOffset, "ScrollToTop scrolled horizontally!"),
                () => viewer.ScrollToTop(),
                () => Assert.AreEqual(0, viewer.VerticalOffset, "ScrollToTop did not stay at the top!"));
        }

        /// <summary>
        /// Verify ScrollToBottom moves to the bottom.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify ScrollToBottom moves to the bottom.")]
        public virtual void ScrollToBottom()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.ScrollToBottom(),
                () => Assert.AreEqual(400 - TestPageScrollAmount, viewer.VerticalOffset, "ScrollToBottom did not scroll vertically!"),
                () => Assert.AreEqual(200, viewer.HorizontalOffset, "ScrollToBottom scrolled horizontally!"),
                () => viewer.ScrollToBottom(),
                () => Assert.AreEqual(400 - TestPageScrollAmount, viewer.VerticalOffset, "ScrollToBottom did not stay at the bottom!"));
        }

        /// <summary>
        /// Verify ScrollToLeft moves to the left.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify ScrollToLeft moves to the left.")]
        public virtual void ScrollToLeft()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToLeft(),
                () => Assert.AreEqual(0, viewer.HorizontalOffset, "ScrollToLeft did not scroll horizontally!"),
                () => Assert.AreEqual(200, viewer.VerticalOffset, "ScrollToLeft scrolled vertically!"),
                () => viewer.ScrollToLeft(),
                () => Assert.AreEqual(0, viewer.HorizontalOffset, "ScrollToLeft did not stay at the left!"));
        }

        /// <summary>
        /// Verify ScrollToRight moves to the right.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify ScrollToRight moves to the right.")]
        public virtual void ScrollToRight()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToHorizontalOffset(200),
                () => viewer.ScrollToVerticalOffset(200),
                () => viewer.ScrollToRight(),
                () => Assert.AreEqual(400 - TestPageScrollAmount, viewer.HorizontalOffset, "ScrollToRight did not scroll horizontally!"),
                () => Assert.AreEqual(200, viewer.VerticalOffset, "ScrollToRight scrolled vertically!"),
                () => viewer.ScrollToRight(),
                () => Assert.AreEqual(400 - TestPageScrollAmount, viewer.HorizontalOffset, "ScrollToRight did not stay at the right!"));
        }
        #endregion Scroll to Top/Bottom/Left/Right

        #region ScrollIntoView
        /// <summary>
        /// Verify ScrollIntoView throws on a null ScrollViewer.
        /// </summary>
        [TestMethod]
        [Description("Verify ScrollIntoView throws on a null ScrollViewer.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ScrollIntoViewRequiresViewer()
        {
            ScrollViewer viewer = null;
            viewer.ScrollIntoView(new Button());
        }

        /// <summary>
        /// Verify ScrollIntoView throws on a null element.
        /// </summary>
        [TestMethod]
        [Description("Verify ScrollIntoView throws on a null element.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ScrollIntoViewRequiresElement()
        {
            ScrollViewer viewer = new ScrollViewer();
            viewer.ScrollIntoView(null);
        }

        /// <summary>
        /// Attempt to ScrollIntoView when not in the visual tree.
        /// </summary>
        [TestMethod]
        [Description("Attempt to ScrollIntoView when not in the visual tree.")]
        public virtual void ScrollIntoViewNotInVisualTree()
        {
            Button button = new Button { Content = "Test" };
            ScrollViewer viewer = CreateTestScrollViewer(button, 200, 200);
            viewer.ScrollIntoView(button);
            Assert.AreEqual(0, viewer.VerticalOffset, "Should not have not scroll vertically!");
            Assert.AreEqual(0, viewer.HorizontalOffset, "Should not have not scroll horizontally!");
        }

        /// <summary>
        /// Attempt to ScrollIntoView when the element is not in the visual
        /// tree.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Attempt to ScrollIntoView when the element is not in the visual tree.")]
        public virtual void ScrollIntoViewElementNotInVisualTree()
        {
            Button button = new Button { Content = "Test" };
            ScrollViewer viewer = CreateTestScrollViewer();

            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollIntoView(button),
                () => Assert.AreEqual(0, viewer.VerticalOffset, "Should not have not scroll vertically!"),
                () => Assert.AreEqual(0, viewer.HorizontalOffset, "Should not have not scroll horizontally!"));
        }

        /// <summary>
        /// Basic ScrollIntoView test.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic ScrollIntoView test.")]
        public virtual void ScrollIntoViewBasic()
        {
            Button button = new Button { Content = "Test" };
            ScrollViewer viewer = CreateTestScrollViewer(button, 200, 200);

            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollIntoView(button),
                () => Assert.AreNotEqual(0, viewer.VerticalOffset, "Should have scrolled vertically!"),
                () => Assert.AreNotEqual(0, viewer.HorizontalOffset, "Should have scrolled horizontally!"),
                () => Assert.IsTrue(IsInView(viewer, button), "Button should be in view!"));
        }

        /// <summary>
        /// Scroll an element into view that is too large.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Scroll an element into view that is too large.")]
        public virtual void ScrollIntoViewTooLarge()
        {
            Button a = new Button
            {
                Content = "a",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            Button b = new Button
            {
                Content = "b",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Grid grid = new Grid { Width = 200, Height = 200 };
            grid.Children.Add(a);
            grid.Children.Add(b);

            ScrollViewer viewer = CreateTestScrollViewer(grid, 150, 150);

            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollIntoView(grid),
                () => Assert.IsFalse(IsInView(viewer, grid), "Grid should not fit in the view!"),
                () => Assert.IsTrue(IsInView(viewer, a), "The a Button should be in view!"),
                () => Assert.IsFalse(IsInView(viewer, b), "The b Button should not fit in the view!"));
        }

        /// <summary>
        /// Scroll an element into view that's already in view.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Scroll an element into view that's already in view.")]
        public virtual void ScrollIntoViewAlreadyInView()
        {
            Button button = new Button { Content = "Test" };
            ScrollViewer viewer = CreateTestScrollViewer(button, 105, 105);

            TestAsync(
                ScrollDelay,
                viewer,
                () => viewer.ScrollToVerticalOffset(100),
                () => viewer.ScrollToHorizontalOffset(100),
                () => Assert.IsTrue(IsInView(viewer, button), "Button should start in view!"),
                () => viewer.ScrollIntoView(button),
                () => Assert.IsTrue(IsInView(viewer, button), "Button is still in view!"),
                () => Assert.AreEqual(100, viewer.VerticalOffset, "Should not have scrolled vertically!"),
                () => Assert.AreEqual(100, viewer.HorizontalOffset, "Should not have scrolled horizontally!"));
        }
        #endregion ScrollIntoView

        /// <summary>
        /// Ensure we never scroll off the edges.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure we never scroll off the edges.")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "InBounds", Justification = "Test method")]
        public virtual void ScrollingStaysInBounds()
        {
            ScrollViewer viewer = CreateTestScrollViewer();
            TestAsync(
                ScrollDelay,
                viewer,
                () => Assert.AreEqual(0, viewer.VerticalOffset, "Should start at the top!"),
                () => Assert.AreEqual(0, viewer.HorizontalOffset, "Short start at the left!"),
                () => viewer.LineUp(),
                () => Assert.AreEqual(0, viewer.VerticalOffset, "Still at the top!"),
                () => viewer.LineLeft(),
                () => Assert.AreEqual(0, viewer.HorizontalOffset, "Still at the left!"),
                () => viewer.ScrollToBottom(),
                () => Assert.AreEqual(400 - TestPageScrollAmount, viewer.VerticalOffset, "Should have moved to the bottom!"),
                () => viewer.LineDown(),
                () => Assert.AreEqual(400 - TestPageScrollAmount, viewer.VerticalOffset, "Should still be at the bottom!"),
                () => viewer.ScrollToHorizontalOffset(327),
                () => Assert.AreEqual(400 - TestPageScrollAmount, viewer.HorizontalOffset, "Should have moved to the right!"),
                () => viewer.LineRight(),
                () => Assert.AreEqual(400 - TestPageScrollAmount, viewer.HorizontalOffset, "Should still be at the right!"));
        }

        #region IsMouseWheelScrollingEnabled
        /// <summary>
        /// Verify GetIsMouseWheelScrollingEnabled throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify GetIsMouseWheelScrollingEnabled throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetIsMouseWheelScrollingEnabledThrows()
        {
            ScrollViewer viewer = null;
            viewer.GetIsMouseWheelScrollingEnabled();
        }

        /// <summary>
        /// Verify SetIsMouseWheelScrollingEnabled throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify SetIsMouseWheelScrollingEnabled throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void SetIsMouseWheelScrollingEnabledThrows()
        {
            ScrollViewer viewer = null;
            viewer.SetIsMouseWheelScrollingEnabled(true);
        }

        /// <summary>
        /// Enable mouse wheel scrolling.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Enable mouse wheel scrolling.")]
        public virtual void EnableMouseWheelScrolling()
        {
            ScrollViewer viewer = CreateTestScrollViewer();

            TestAsync(
                ScrollDelay,
                viewer,
                () => Assert.IsFalse(viewer.GetIsMouseWheelScrollingEnabled(), "IsMouseWheelScrollingEnabled should start false!"),
                () => viewer.SetIsMouseWheelScrollingEnabled(true),
                () => Assert.IsTrue(viewer.GetIsMouseWheelScrollingEnabled(), "IsMouseWheelScrollingEnabled be true!"));
        }
        #endregion IsMouseWheelScrollingEnabled

        /// <summary>
        /// Get the ScrollHost after setting IsMouseWheelScrollingEnabled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the ScrollHost after setting IsMouseWheelScrollingEnabled.")]
        [Bug("719038 - ScrollViewerExtensions - GetScrollHost() returns null after setting SetIsMouseWheelScrollingEnabled(true) in code", Fixed = true)]
        public virtual void GetScrollHostAfterSettingIsMouseWheelScrollingEnabled()
        {
            ListBox items = new ListBox
            {
                ItemsSource = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                Height = 100
            };
            ScrollViewer viewer = null;
            TestAsync(
                25,
                items,
                () => viewer = items.GetScrollHost(),
                () => Assert.IsNotNull(viewer, "Failed to find ScrollHost the first time!"),
                () => viewer.SetIsMouseWheelScrollingEnabled(true),
                () => viewer = items.GetScrollHost(),
                () => Assert.IsNotNull(viewer, "Failed to find ScrollHost the second time!"));
        }
    }
}