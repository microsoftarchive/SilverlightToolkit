// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for System.Windows.Controls.WrapPanel.
    /// </summary>
    [TestClass]
    public partial class WrapPanelTest : PanelTest
    {
        #region Panels to test
        /// <summary>
        /// Gets a default instance of Panel (or a derived type) to test.
        /// </summary>
        public override Panel DefaultPanelToTest
        {
            get { return DefaultWrapPanelToTest; }
        }

        /// <summary>
        /// Gets instances of Panel (or derived types) to test.
        /// </summary>
        public override IEnumerable<Panel> PanelsToTest
        {
            get { return WrapPanelsToTest.OfType<Panel>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenPanel (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenPanel> OverriddenPanelsToTest
        {
            get { yield break; }
        }
        #endregion Panels to test

        #region WrapPanels to test
        /// <summary>
        /// Gets a default instance of WrapPanel (or a derived type) to test.
        /// </summary>
        public virtual WrapPanel DefaultWrapPanelToTest
        {
            get { return new WrapPanel(); }
        }

        /// <summary>
        /// Gets instances of WrapPanel (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<WrapPanel> WrapPanelsToTest
        {
            get { yield return DefaultWrapPanelToTest; }
        }
        #endregion Panels to test

        /// <summary>
        /// Gets the ItemHeight dependency property test.
        /// </summary>
        protected DependencyPropertyTest<WrapPanel, double> ItemHeightProperty { get; private set; }

        /// <summary>
        /// Gets the ItemWidth dependency property test.
        /// </summary>
        protected DependencyPropertyTest<WrapPanel, double> ItemWidthProperty { get; private set; }

        /// <summary>
        /// Gets the Orientation dependency property test.
        /// </summary>
        protected DependencyPropertyTest<WrapPanel, Orientation> OrientationProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the WrapPanelTest class.
        /// </summary>
        public WrapPanelTest()
            : base()
        {
            Func<WrapPanel> initializer = () => DefaultWrapPanelToTest;
            ItemHeightProperty = new DependencyPropertyTest<WrapPanel, double>(this, "ItemHeight")
                {
                    Property = WrapPanel.ItemHeightProperty,
                    Initializer = initializer,
                    DefaultValue = double.NaN,
                    OtherValues = new double[] { 50.0, 100.0, 5000.0 },
                    InvalidValues = new Dictionary<double, Type>
                    {
                        { 0.0, typeof(ArgumentException) },
                        { -1.0, typeof(ArgumentException) },
                        { double.PositiveInfinity, typeof(ArgumentException) },
                        { double.NegativeInfinity, typeof(ArgumentException) }
                    }
                };
            ItemWidthProperty = new DependencyPropertyTest<WrapPanel, double>(this, "ItemWidth")
                {
                    Property = WrapPanel.ItemWidthProperty,
                    Initializer = initializer,
                    DefaultValue = double.NaN,
                    OtherValues = new double[] { 50.0, 100.0, 5000.0 },
                    InvalidValues = new Dictionary<double, Type>
                    {
                        { 0.0, typeof(ArgumentException) },    
                        { -1.0, typeof(ArgumentException) },
                        { double.PositiveInfinity, typeof(ArgumentException) },
                        { double.NegativeInfinity, typeof(ArgumentException) }
                    }
                };
            OrientationProperty = new DependencyPropertyTest<WrapPanel, Orientation>(this, "Orientation")
                {
                    Property = WrapPanel.OrientationProperty,
                    Initializer = initializer,
                    DefaultValue = Orientation.Horizontal,
                    OtherValues = new Orientation[] { Orientation.Vertical },
                    InvalidValues = new Dictionary<Orientation, Type>
                    {
                        { (Orientation)(-1), typeof(ArgumentException) },
                        { (Orientation)2, typeof(ArgumentException) },
                        { (Orientation)3, typeof(ArgumentException) },
                        { (Orientation)500, typeof(ArgumentException) },
                        { (Orientation)int.MaxValue, typeof(ArgumentException) },
                        { (Orientation)int.MinValue, typeof(ArgumentException) }
                    }
                };
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // ItemHeightProperty tests
            tests.Add(ItemHeightProperty.CheckDefaultValueTest);
            tests.Add(ItemHeightProperty.ChangeClrSetterTest);
            tests.Add(ItemHeightProperty.ChangeSetValueTest);
            tests.Add(ItemHeightProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemHeightProperty.InvalidValueFailsTest);
            tests.Add(ItemHeightProperty.InvalidValueIsIgnoredTest);
            tests.Add(ItemHeightProperty.CanBeStyledTest.Bug("488580", true));
            tests.Add(ItemHeightProperty.SetXamlAttributeTest);
            tests.Add(ItemHeightProperty.SetXamlElementTest);
            tests.Add(ItemHeightProperty.HasTypeConverterTest(typeof(LengthConverter)));

            // ItemWidthProperty tests
            tests.Add(ItemWidthProperty.CheckDefaultValueTest);
            tests.Add(ItemWidthProperty.ChangeClrSetterTest);
            tests.Add(ItemWidthProperty.ChangeSetValueTest);
            tests.Add(ItemWidthProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemWidthProperty.InvalidValueFailsTest);
            tests.Add(ItemWidthProperty.InvalidValueIsIgnoredTest);
            tests.Add(ItemWidthProperty.CanBeStyledTest.Bug("488583", true));
            tests.Add(ItemWidthProperty.SetXamlAttributeTest);
            tests.Add(ItemWidthProperty.SetXamlElementTest);
            tests.Add(ItemWidthProperty.HasTypeConverterTest(typeof(LengthConverter)));

            // OrientationProperty tests
            tests.Add(OrientationProperty.CheckDefaultValueTest);
            tests.Add(OrientationProperty.ChangeClrSetterTest);
            tests.Add(OrientationProperty.ChangeSetValueTest);
            tests.Add(OrientationProperty.ClearValueResetsDefaultTest);
            tests.Add(OrientationProperty.InvalidValueFailsTest);
            tests.Add(OrientationProperty.InvalidValueIsIgnoredTest);
            tests.Add(OrientationProperty.CanBeStyledTest.Bug("488582", true));
            tests.Add(OrientationProperty.SetXamlAttributeTest);
            tests.Add(OrientationProperty.SetXamlElementTest);

            return tests;
        }

        #region Measure
        /// <summary>
        /// Verify the availableSize provided during measuring.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the availableSize provided during measuring.")]
        public virtual void MeasureAvailableSize()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            TestAsync(
                Prepare(panel, 300, 300),
                () => panel.Children.Add(Prepare(new LayoutTestContentControl { Content = 0 }, 25, 25)),
                () => panel.Children.Add(Prepare(new LayoutTestContentControl { Content = 1 }, 25, 25)),
                () => Assert.AreEqual(
                    new Size(panel.Width, panel.Height),
                    (panel.Children[0] as LayoutTestContentControl).SuggestedAvailableSize),
                () => Assert.AreEqual(
                    new Size(panel.Width, panel.Height),
                    (panel.Children[1] as LayoutTestContentControl).SuggestedAvailableSize));
        }

        /// <summary>
        /// Verify the availableSize provided during measuring when using
        /// ItemWidth.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the availableSize provided during measuring when using ItemWidth.")]
        public virtual void MeasureAvailableSizeItemWidth()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 30;
            TestAsync(
                Prepare(panel, 300, 300),
                () => panel.Children.Add(Prepare(new LayoutTestContentControl { Content = 0 }, 25, 25)),
                () => panel.Children.Add(Prepare(new LayoutTestContentControl { Content = 1 }, 25, 25)),
                () => Assert.AreEqual(
                    new Size(30, panel.Height),
                    (panel.Children[0] as LayoutTestContentControl).SuggestedAvailableSize),
                () => Assert.AreEqual(
                    new Size(30, panel.Height),
                    (panel.Children[1] as LayoutTestContentControl).SuggestedAvailableSize));
        }

        /// <summary>
        /// Verify the availableSize provided during measuring when using
        /// ItemHeight.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the availableSize provided during measuring when using ItemHeight.")]
        public virtual void MeasureAvailableSizeItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemHeight = 30;
            TestAsync(
                Prepare(panel, 300, 300),
                () => panel.Children.Add(Prepare(new LayoutTestContentControl { Content = 0 }, 25, 25)),
                () => panel.Children.Add(Prepare(new LayoutTestContentControl { Content = 1 }, 25, 25)),
                () => Assert.AreEqual(
                    new Size(panel.Width, 30),
                    (panel.Children[0] as LayoutTestContentControl).SuggestedAvailableSize),
                () => Assert.AreEqual(
                    new Size(panel.Width, 30),
                    (panel.Children[1] as LayoutTestContentControl).SuggestedAvailableSize));
        }

        /// <summary>
        /// Verify the availableSize provided during measuring when using
        /// ItemWidth.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the availableSize provided during measuring when using ItemWidth.")]
        public virtual void MeasureAvailableSizeItemWidthAndHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 30;
            panel.ItemHeight = 40;
            TestAsync(
                Prepare(panel, 300, 300),
                () => panel.Children.Add(Prepare(new LayoutTestContentControl { Content = 0 }, 25, 25)),
                () => panel.Children.Add(Prepare(new LayoutTestContentControl { Content = 1 }, 25, 25)),
                () => Assert.AreEqual(
                    new Size(30, 40),
                    (panel.Children[0] as LayoutTestContentControl).SuggestedAvailableSize),
                () => Assert.AreEqual(
                    new Size(30, 40),
                    (panel.Children[1] as LayoutTestContentControl).SuggestedAvailableSize));
        }
        #endregion

        #region Uniform Layout
        /// <summary>
        /// Basic horizontal layout test with a single element.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a single element.")]
        public virtual void LayoutHorizontalSingleElement()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 100, 100); return new Rect(0, 0, 100, 100); });
        }

        /// <summary>
        /// Basic vertical layout test with a single element.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a single element.")]
        public virtual void LayoutVerticalSingleElement()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 100, 100); return new Rect(0, 0, 100, 100); });
        }

        /// <summary>
        /// Basic horizontal layout test with a short sequence of uniform
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a short sequence of uniform elements.")]
        public virtual void LayoutHorizontalUniformShortSequence()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 100, 100); return new Rect(0, 0, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(100, 0, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(200, 0, 100, 100); });
        }

        /// <summary>
        /// Basic vertical layout test with a short sequence of uniform
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a short sequence of uniform elements.")]
        public virtual void LayoutVerticalUniformShortSequence()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 100, 100); return new Rect(0, 0, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(0, 100, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(0, 200, 100, 100); });
        }

        /// <summary>
        /// Basic horizontal layout test with a sequence of uniform elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a sequence of uniform elements.")]
        public virtual void LayoutHorizontalUniformSequence()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 100, 100); return new Rect(0, 0, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(100, 0, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(200, 0, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(300, 0, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(0, 100, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(100, 100, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(200, 100, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(300, 100, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(0, 200, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(100, 200, 100, 100); });
        }

        /// <summary>
        /// Basic vertical layout test with a sequence of uniform elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a sequence of uniform elements.")]
        public virtual void LayoutVerticalUniformSequence()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 100, 100); return new Rect(0, 0, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(0, 100, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(0, 200, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(0, 300, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(100, 0, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(100, 100, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(100, 200, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(100, 300, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(200, 0, 100, 100); },
                c => { Prepare(c, 100, 100); return new Rect(200, 100, 100, 100); });
        }
        #endregion Uniform Layout

        #region Uniform Layout with ItemWidth
        /// <summary>
        /// Basic horizontal layout test with a single element with the
        /// ItemWidth property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a single element with the ItemWidth property set.")]
        public virtual void LayoutHorizontalSingleElementWithItemWidth()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 30); });
        }

        /// <summary>
        /// Basic vertical layout test with a single element with the ItemWidth
        /// property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a single element with the ItemWidth property set.")]
        public virtual void LayoutVerticalSingleElementWithItemWidth()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemWidth = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 30); });
        }

        /// <summary>
        /// Basic horizontal layout test with a short sequence of uniform
        /// elements with the ItemWidth property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a short sequence of uniform elements with the ItemWidth property set.")]
        public virtual void LayoutHorizontalUniformShortSequenceWithItemWidth()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(50, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(100, 0, 50, 30); });
        }

        /// <summary>
        /// Basic vertical layout test with a short sequence of uniform
        /// elements with the ItemWidth property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a short sequence of uniform elements with the ItemWidth property set.")]
        public virtual void LayoutVerticalUniformShortSequenceWithItemWidth()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemWidth = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 30, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 60, 50, 30); });
        }

        /// <summary>
        /// Basic horizontal layout test with a sequence of uniform elements
        /// with the ItemWidth property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a sequence of uniform elements with the ItemWidth property set.")]
        public virtual void LayoutHorizontalUniformSequenceWithItemWidth()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(50, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(100, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(150, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(200, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(250, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(300, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(350, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 30, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(50, 30, 50, 30); });
        }

        /// <summary>
        /// Basic vertical layout test with a sequence of uniform elements with
        /// the ItemWidth property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a sequence of uniform elements with the ItemWidth property set.")]
        public virtual void LayoutVerticalUniformSequenceWithItemWidth()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemWidth = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 30, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 60, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 90, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 120, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 150, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 180, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 210, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 240, 50, 30); },
                c => { Prepare(c, 30, 30); return new Rect(0, 270, 50, 30); });
        }
        #endregion Uniform Layout with ItemWidth

        #region Uniform Layout with ItemHeight
        /// <summary>
        /// Basic horizontal layout test with a single element with the
        /// ItemHeight property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a single element with the ItemHeight property set.")]
        public virtual void LayoutHorizontalSingleElementWithItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 30, 50); });
        }

        /// <summary>
        /// Basic vertical layout test with a single element with the ItemHeight
        /// property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a single element with the ItemHeight property set.")]
        public virtual void LayoutVerticalSingleElementWithItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 30, 50); });
        }

        /// <summary>
        /// Basic horizontal layout test with a short sequence of uniform
        /// elements with the ItemHeight property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a short sequence of uniform elements with the ItemHeight property set.")]
        public virtual void LayoutHorizontalUniformShortSequenceWithItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(30, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(60, 0, 30, 50); });
        }

        /// <summary>
        /// Basic vertical layout test with a short sequence of uniform
        /// elements with the ItemHeight property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a short sequence of uniform elements with the ItemHeight property set.")]
        public virtual void LayoutVerticalUniformShortSequenceWithItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 50, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 100, 30, 50); });
        }

        /// <summary>
        /// Basic horizontal layout test with a sequence of uniform elements
        /// with the ItemHeight property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a sequence of uniform elements with the ItemHeight property set.")]
        public virtual void LayoutHorizontalUniformSequenceWithItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(30, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(60, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(90, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(120, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(150, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(180, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(210, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(240, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(270, 0, 30, 50); });
        }

        /// <summary>
        /// Basic vertical layout test with a sequence of uniform elements with
        /// the ItemHeight property set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a sequence of uniform elements with the ItemHeight property set.")]
        public virtual void LayoutVerticalUniformSequenceWithItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 50, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 100, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 150, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 200, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 250, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 300, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 350, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(30, 0, 30, 50); },
                c => { Prepare(c, 30, 30); return new Rect(30, 50, 30, 50); });
        }
        #endregion Uniform Layout with ItemHeight

        #region Uniform Layout with ItemWidth and ItemHeight
        /// <summary>
        /// Basic horizontal layout test with a single element with the
        /// ItemWidth and ItemHeight properties set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a single element with the ItemWidth and ItemHeight properties set.")]
        public virtual void LayoutHorizontalSingleElementWithItemWidthAndHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 50;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 50); });
        }

        /// <summary>
        /// Basic vertical layout test with a single element with the ItemWidth
        /// and ItemHeight properties set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a single element with the ItemWidth and ItemHeight properties set.")]
        public virtual void LayoutVerticalSingleElementWithItemWidthAndHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemWidth = 50;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 50); });
        }

        /// <summary>
        /// Basic horizontal layout test with a short sequence of uniform
        /// elements with the ItemWidth and ItemHeight properties set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a short sequence of uniform elements with the ItemWidth and ItemHeight properties set.")]
        public virtual void LayoutHorizontalUniformShortSequenceWithItemWidthAndHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 50;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(50, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(100, 0, 50, 50); });
        }

        /// <summary>
        /// Basic vertical layout test with a short sequence of uniform
        /// elements with the ItemWidth and ItemHeight properties set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a short sequence of uniform elements with the ItemWidth and ItemHeight properties set.")]
        public virtual void LayoutVerticalUniformShortSequenceWithItemWidthAndHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemWidth = 50;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 50, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 100, 50, 50); });
        }

        /// <summary>
        /// Basic horizontal layout test with a sequence of uniform elements
        /// with the ItemWidth and ItemHeight properties set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a sequence of uniform elements with the ItemWidth and ItemHeight properties set.")]
        public virtual void LayoutHorizontalUniformSequenceWithItemWidthAndHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 50;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(50, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(100, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(150, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(200, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(250, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(300, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(350, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 50, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(50, 50, 50, 50); });
        }

        /// <summary>
        /// Basic vertical layout test with a sequence of uniform elements with
        /// the ItemWidth and ItemHeight properties set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a sequence of uniform elements with the ItemWidth and ItemHeight properties set.")]
        public virtual void LayoutVerticalUniformSequenceWithItemWidthAndHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemWidth = 50;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 30, 30); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 50, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 100, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 150, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 200, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 250, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 300, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(0, 350, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(50, 0, 50, 50); },
                c => { Prepare(c, 30, 30); return new Rect(50, 50, 50, 50); });
        }
        #endregion Uniform Layout with ItemWidth and ItemHeight

        #region Non-uniform Layout
        /// <summary>
        /// Basic horizontal layout test with a short sequence of elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a short sequence of elements.")]
        public virtual void LayoutHorizontalShortSequence()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 100); },
                c => { Prepare(c, 100, 50); return new Rect(50, 0, 100, 100); },
                c => { Prepare(c, 75, 100); return new Rect(150, 0, 75, 100); });
        }

        /// <summary>
        /// Basic vertical layout test with a short sequence of elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a short sequence of elements.")]
        public virtual void LayoutVerticalShortSequence()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 100, 50); },
                c => { Prepare(c, 100, 50); return new Rect(0, 50, 100, 50); },
                c => { Prepare(c, 75, 100); return new Rect(0, 100, 100, 100); });
        }

        /// <summary>
        /// Basic horizontal layout test with a sequence of elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a sequence of elements.")]
        public virtual void LayoutHorizontalSequence()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 120); },
                c => { Prepare(c, 100, 50); return new Rect(50, 0, 100, 120); },
                c => { Prepare(c, 75, 100); return new Rect(150, 0, 75, 120); },
                c => { Prepare(c, 30, 30); return new Rect(225, 0, 30, 120); },
                c => { Prepare(c, 100, 120); return new Rect(255, 0, 100, 120); },
                c => { Prepare(c, 200, 50); return new Rect(0, 120, 200, 50); },
                c => { Prepare(c, 50, 30); return new Rect(200, 120, 50, 50); },
                c => { Prepare(c, 40, 40); return new Rect(250, 120, 40, 50); },
                c => { Prepare(c, 100, 40); return new Rect(290, 120, 100, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 170, 50, 50); });
        }

        /// <summary>
        /// Basic vertical layout test with a sequence of elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a sequence of elements.")]
        public virtual void LayoutVerticalSequence()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 200, 50); },
                c => { Prepare(c, 100, 50); return new Rect(0, 50, 200, 50); },
                c => { Prepare(c, 75, 100); return new Rect(0, 100, 200, 100); },
                c => { Prepare(c, 30, 30); return new Rect(0, 200, 200, 30); },
                c => { Prepare(c, 100, 120); return new Rect(0, 230, 200, 120); },
                c => { Prepare(c, 200, 50); return new Rect(0, 350, 200, 50); },
                c => { Prepare(c, 50, 30); return new Rect(200, 0, 100, 30); },
                c => { Prepare(c, 40, 40); return new Rect(200, 30, 100, 40); },
                c => { Prepare(c, 100, 40); return new Rect(200, 70, 100, 40); },
                c => { Prepare(c, 50, 50); return new Rect(200, 110, 100, 50); });
        }
        #endregion Non-uniform Layout

        #region Non-uniform Layout with ItemWidth
        /// <summary>
        /// Basic horizontal layout with ItemWidth test with a short sequence of
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout with ItemWidth test with a short sequence of elements.")]
        public virtual void LayoutHorizontalShortSequenceWithItemWidth()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 100); },
                c => { Prepare(c, 100, 50); return new Rect(50, 0, 100, 100); },
                c => { Prepare(c, 75, 100); return new Rect(100, 0, 75, 100); });
        }

        /// <summary>
        /// Basic vertical layout with ItemWidth test with a short sequence of
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout with ItemWidth test with a short sequence of elements.")]
        public virtual void LayoutVerticalShortSequenceWithItemWidth()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemWidth = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 100, 50); return new Rect(0, 50, 100, 50); },
                c => { Prepare(c, 75, 100); return new Rect(0, 100, 75, 100); });
        }

        /// <summary>
        /// Basic horizontal layout with ItemWidth test with a sequence of
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout with ItemWidth test with a sequence of elements.")]
        public virtual void LayoutHorizontalSequenceWithItemWidth()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 120); },
                c => { Prepare(c, 100, 50); return new Rect(50, 0, 100, 120); },
                c => { Prepare(c, 75, 100); return new Rect(100, 0, 75, 120); },
                c => { Prepare(c, 30, 30); return new Rect(150, 0, 50, 120); },
                c => { Prepare(c, 100, 120); return new Rect(200, 0, 100, 120); },
                c => { Prepare(c, 200, 50); return new Rect(250, 0, 200, 120); },
                c => { Prepare(c, 50, 30); return new Rect(300, 0, 50, 120); },
                c => { Prepare(c, 40, 40); return new Rect(350, 0, 50, 120); },
                c => { Prepare(c, 100, 40); return new Rect(0, 120, 100, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 120, 50, 50); });
        }

        /// <summary>
        /// Basic vertical layout with ItemWidth test with a sequence of
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout with ItemWidth test with a sequence of elements.")]
        public virtual void LayoutVerticalSequenceWithItemWidth()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemWidth = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 100, 50); return new Rect(0, 50, 100, 50); },
                c => { Prepare(c, 75, 100); return new Rect(0, 100, 75, 100); },
                c => { Prepare(c, 30, 30); return new Rect(0, 200, 50, 30); },
                c => { Prepare(c, 100, 120); return new Rect(0, 230, 100, 120); },
                c => { Prepare(c, 200, 50); return new Rect(0, 350, 200, 50); },
                c => { Prepare(c, 50, 30); return new Rect(50, 0, 50, 30); },
                c => { Prepare(c, 40, 40); return new Rect(50, 30, 50, 40); },
                c => { Prepare(c, 100, 40); return new Rect(50, 70, 100, 40); },
                c => { Prepare(c, 50, 50); return new Rect(50, 110, 50, 50); });
        }
        #endregion Non-uniform Layout with ItemWidth

        #region Non-uniform Layout with ItemHeight
        /// <summary>
        /// Basic horizontal layout with ItemHeight test with a short sequence
        /// of elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout with ItemHeight test with a short sequence of elements.")]
        public virtual void LayoutHorizontalShortSequenceWithItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 100, 50); return new Rect(50, 0, 100, 50); },
                c => { Prepare(c, 75, 100); return new Rect(150, 0, 75, 100); });
        }

        /// <summary>
        /// Basic vertical layout with ItemHeight test with a short sequence of
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout with ItemHeight test with a short sequence of elements.")]
        public virtual void LayoutVerticalShortSequenceWithItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 100, 50); },
                c => { Prepare(c, 100, 50); return new Rect(0, 50, 100, 50); },
                c => { Prepare(c, 75, 100); return new Rect(0, 100, 100, 100); });
        }

        /// <summary>
        /// Basic horizontal layout with ItemHeight test with a sequence of
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout with ItemHeight test with a sequence of elements.")]
        public virtual void LayoutHorizontalSequenceWithItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 100, 50); return new Rect(50, 0, 100, 50); },
                c => { Prepare(c, 75, 100); return new Rect(150, 0, 75, 100); },
                c => { Prepare(c, 30, 30); return new Rect(225, 0, 30, 50); },
                c => { Prepare(c, 100, 120); return new Rect(255, 0, 100, 120); },
                c => { Prepare(c, 200, 50); return new Rect(0, 50, 200, 50); },
                c => { Prepare(c, 50, 30); return new Rect(200, 50, 50, 50); },
                c => { Prepare(c, 40, 40); return new Rect(250, 50, 40, 50); },
                c => { Prepare(c, 100, 40); return new Rect(290, 50, 100, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 100, 50, 50); });
        }

        /// <summary>
        /// Basic vertical layout with ItemHeight test with a sequence of
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout with ItemHeight test with a sequence of elements.")]
        public virtual void LayoutVerticalSequenceWithItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemHeight = 50;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 200, 50); },
                c => { Prepare(c, 100, 50); return new Rect(0, 50, 200, 50); },
                c => { Prepare(c, 75, 100); return new Rect(0, 100, 200, 100); },
                c => { Prepare(c, 30, 30); return new Rect(0, 150, 200, 50); },
                c => { Prepare(c, 100, 120); return new Rect(0, 200, 200, 120); },
                c => { Prepare(c, 200, 50); return new Rect(0, 250, 200, 50); },
                c => { Prepare(c, 50, 30); return new Rect(0, 300, 200, 50); },
                c => { Prepare(c, 40, 40); return new Rect(0, 350, 200, 50); },
                c => { Prepare(c, 100, 40); return new Rect(200, 0, 100, 50); },
                c => { Prepare(c, 50, 50); return new Rect(200, 50, 100, 50); });
        }
        #endregion Non-uniform Layout with ItemHeight

        #region Non-uniform Layout with ItemWidth and ItemHeight
        /// <summary>
        /// Basic horizontal layout with ItemWidth and ItemHeight test with a short sequence of
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout with ItemWidth and ItemHeight test with a short sequence of elements.")]
        public virtual void LayoutHorizontalShortSequenceWithItemWidthAndItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 50;
            panel.ItemHeight = 40;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 100, 50); return new Rect(50, 0, 100, 50); },
                c => { Prepare(c, 75, 100); return new Rect(100, 0, 75, 100); });
        }

        /// <summary>
        /// Basic vertical layout with ItemWidth and ItemHeight test with a short sequence of
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout with ItemWidth and ItemHeight test with a short sequence of elements.")]
        public virtual void LayoutVerticalShortSequenceWithItemWidthAndItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemWidth = 50;
            panel.ItemHeight = 40;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 100, 50); return new Rect(0, 40, 100, 50); },
                c => { Prepare(c, 75, 100); return new Rect(0, 80, 75, 100); });
        }

        /// <summary>
        /// Basic horizontal layout with ItemWidth and ItemHeight test with a sequence of
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout with ItemWidth and ItemHeight test with a sequence of elements.")]
        public virtual void LayoutHorizontalSequenceWithItemWidthAndItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 50;
            panel.ItemHeight = 40;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 100, 50); return new Rect(50, 0, 100, 50); },
                c => { Prepare(c, 75, 100); return new Rect(100, 0, 75, 100); },
                c => { Prepare(c, 30, 30); return new Rect(150, 0, 50, 40); },
                c => { Prepare(c, 100, 120); return new Rect(200, 0, 100, 120); },
                c => { Prepare(c, 200, 50); return new Rect(250, 0, 200, 50); },
                c => { Prepare(c, 50, 30); return new Rect(300, 0, 50, 40); },
                c => { Prepare(c, 40, 40); return new Rect(350, 0, 50, 40); },
                c => { Prepare(c, 100, 40); return new Rect(0, 40, 100, 40); },
                c => { Prepare(c, 50, 50); return new Rect(50, 40, 50, 50); });
        }

        /// <summary>
        /// Basic vertical layout with ItemWidth and ItemHeight test with a sequence of
        /// elements.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout with ItemWidth and ItemHeight test with a sequence of elements.")]
        public virtual void LayoutVerticalSequenceWithItemWidthAndItemHeight()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemWidth = 50;
            panel.ItemHeight = 40;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 100, 50); return new Rect(0, 40, 100, 50); },
                c => { Prepare(c, 75, 100); return new Rect(0, 80, 75, 100); },
                c => { Prepare(c, 30, 30); return new Rect(0, 120, 50, 40); },
                c => { Prepare(c, 100, 120); return new Rect(0, 160, 100, 120); },
                c => { Prepare(c, 200, 50); return new Rect(0, 200, 200, 50); },
                c => { Prepare(c, 50, 30); return new Rect(0, 240, 50, 40); },
                c => { Prepare(c, 40, 40); return new Rect(0, 280, 50, 40); },
                c => { Prepare(c, 100, 40); return new Rect(0, 320, 100, 40); },
                c => { Prepare(c, 50, 50); return new Rect(0, 360, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 0, 50, 50); },
                c => { Prepare(c, 100, 50); return new Rect(50, 40, 100, 50); },
                c => { Prepare(c, 75, 100); return new Rect(50, 80, 75, 100); });
        }
        #endregion Non-uniform Layout with ItemWidth and ItemHeight

        #region Too Large
        /// <summary>
        /// Basic horizontal layout test with a single element that is too
        /// large.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test with a single element that is too large.")]
        public virtual void LayoutHorizontalLargeSingleElement()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.ItemWidth = 500;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 100, 100); return new Rect(0, 0, 500, 100); });
        }

        /// <summary>
        /// Basic vertical layout test with a single element that is too large.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test with a single element that is too large.")]
        public virtual void LayoutVerticalLargeSingleElement()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            panel.ItemHeight = 500;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, 100, 100); return new Rect(0, 0, 100, 500); });
        }
        #endregion Too Large

        #region Inside ScrollViewer
        /// <summary>
        /// Basic horizontal layout test inside a ScrollViewer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test inside a ScrollViewer.")]
        public virtual void InsideScrollViewerHorizontal()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            TestLayoutAsync(
                new ScrollViewer
                {
                    Content = panel,
                    Width = 400,
                    Height = 400,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                },
                panel,
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(100, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(150, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(200, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(250, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(300, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(350, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(400, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(450, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(500, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(550, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(600, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(650, 0, 50, 50); });
        }

        /// <summary>
        /// Basic horizontal layout test inside a ScrollViewer with wrapping.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test inside a ScrollViewer with wrapping.")]
        public virtual void InsideScrollViewerHorizontalWithWrapping()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            TestLayoutAsync(
                new ScrollViewer
                {
                    Content = panel,
                    Width = 400,
                    Height = 400,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                },
                panel,
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(100, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(150, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(200, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(250, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(300, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 50, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 50, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(100, 50, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(150, 50, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(200, 50, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(250, 50, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(300, 50, 50, 50); });
        }

        /// <summary>
        /// Basic vertical layout test inside a ScrollViewer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test inside a ScrollViewer.")]
        public virtual void InsideScrollViewerVertical()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            TestLayoutAsync(
                new ScrollViewer
                {
                    Content = panel,
                    Width = 400,
                    Height = 400,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                },
                panel,
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 50, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 100, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 150, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 200, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 250, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 300, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 350, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 400, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 450, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 500, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 550, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 600, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 650, 50, 50); });
        }

        /// <summary>
        /// Basic vertical layout test inside a ScrollViewer with wrapping.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test inside a ScrollViewer with wrapping.")]
        public virtual void InsideScrollViewerVerticalWithWrapping()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            TestLayoutAsync(
                new ScrollViewer
                {
                    Content = panel,
                    Width = 400,
                    Height = 400,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Disabled
                },
                panel,
                c => { Prepare(c, 50, 50); return new Rect(0, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 50, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 100, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 150, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 200, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 250, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(0, 300, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 0, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 50, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 100, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 150, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 200, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 250, 50, 50); },
                c => { Prepare(c, 50, 50); return new Rect(50, 300, 50, 50); });
        }
        #endregion Inside ScrollViewer

        #region Inside other containers
        /// <summary>
        /// Basic horizontal layout test inside a Button.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic horizontal layout test inside a Button.")]
        public virtual void InsideButtonHorizontal()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            TestLayoutAsync(
                new Button { Content = panel, Width = 70 },
                panel,
                c => { Prepare(c, 25, 25); return new Rect(0, 0, 25, 25); },
                c => { Prepare(c, 25, 25); return new Rect(25, 0, 25, 25); },
                c => { Prepare(c, 25, 25); return new Rect(0, 25, 25, 25); },
                c => { Prepare(c, 25, 25); return new Rect(25, 25, 25, 25); });
        }

        /// <summary>
        /// Basic vertical layout test inside a Button.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Basic vertical layout test inside a Button.")]
        public virtual void InsideButtonVertical()
        {
            WrapPanel panel = DefaultWrapPanelToTest;
            panel.Orientation = Orientation.Vertical;
            TestLayoutAsync(
                new Button { Content = panel, Height = 70 },
                panel,
                c => { Prepare(c, 25, 25); return new Rect(0, 0, 25, 25); },
                c => { Prepare(c, 25, 25); return new Rect(0, 25, 25, 25); },
                c => { Prepare(c, 25, 25); return new Rect(25, 0, 25, 25); },
                c => { Prepare(c, 25, 25); return new Rect(25, 25, 25, 25); });
        }
        #endregion Inside other containers
    }
}