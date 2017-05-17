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
    /// Unit tests for the System.Windows.Controls.DockPanel class.
    /// </summary>
    [TestClass]
    public partial class DockPanelTest : PanelTest
    {
        #region Panels to test
        /// <summary>
        /// Gets a default instance of Panel (or a derived type) to test.
        /// </summary>
        public override Panel DefaultPanelToTest
        {
            get { return DefaultDockPanelToTest; }
        }

        /// <summary>
        /// Gets instances of Panel (or derived types) to test.
        /// </summary>
        public override IEnumerable<Panel> PanelsToTest
        {
            get { return DockPanelsToTest.OfType<Panel>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenPanel (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenPanel> OverriddenPanelsToTest
        {
            get { yield break; }
        }
        #endregion Panels to test

        #region DockPanels to test
        /// <summary>
        /// Gets a default instance of DockPanel (or a derived type) to test.
        /// </summary>
        public virtual DockPanel DefaultDockPanelToTest
        {
            get { return new DockPanel(); }
        }

        /// <summary>
        /// Gets instances of DockPanel (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<DockPanel> DockPanelsToTest
        {
            get { yield return DefaultDockPanelToTest; }
        }
        #endregion Panels to test

        /// <summary>
        /// Gets the Dock dependency property test.
        /// </summary>
        protected DependencyPropertyTest<DockPanel, Dock> DockProperty { get; private set; }

        /// <summary>
        /// Gets the LastChildFill dependency property test.
        /// </summary>
        protected DependencyPropertyTest<DockPanel, bool> LastChildFillProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DockPanelTest class.
        /// </summary>
        public DockPanelTest()
            : base()
        {
            Func<DockPanel> initializer = () => DefaultDockPanelToTest;
            DockProperty = new DependencyPropertyTest<DockPanel, Dock>(this, "Dock")
                {
                    Property = DockPanel.DockProperty,
                    Initializer = initializer,
                    IsAttached = true,
                    AttachedInitializer = () => new Button { Content = "Test" },
                    DefaultValue = Dock.Left,
                    OtherValues = new Dock[] { Dock.Top, Dock.Right, Dock.Bottom },
                    InvalidValues = new Dictionary<Dock, Type>
                    {
                        { (Dock)(-1), typeof(ArgumentException) },
                        { (Dock)4, typeof(ArgumentException) },
                        { (Dock)10, typeof(ArgumentException) },
                        { (Dock)27, typeof(ArgumentException) },
                        { (Dock)int.MaxValue, typeof(ArgumentException) },
                        { (Dock)int.MinValue, typeof(ArgumentException) }
                    }
                };
            LastChildFillProperty = new DependencyPropertyTest<DockPanel, bool>(this, "LastChildFill")
                {
                    Property = DockPanel.LastChildFillProperty,
                    Initializer = initializer,
                    DefaultValue = true,
                    OtherValues = new bool[] { false }
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

            // DockProperty tests
            tests.Add(DockProperty.CheckDefaultValueTest);
            tests.Add(DockProperty.ChangeClrSetterTest);
            tests.Add(DockProperty.ChangeSetValueTest);
            tests.Add(DockProperty.ClearValueResetsDefaultTest);
            tests.Add(DockProperty.CanBeStyledTest);
            tests.Add(DockProperty.SetXamlAttributeTest);
            tests.Add(DockProperty.SetXamlElementTest);
            tests.Add(DockProperty.AttachedGetNullFailsTest);
            tests.Add(DockProperty.AttachedSetNullFailsTest);
            tests.Add(DockProperty.InvalidValueFailsTest);
            tests.Add(DockProperty.InvalidValueIsIgnoredTest);

            // LastChildFillProperty tests
            tests.Add(LastChildFillProperty.CheckDefaultValueTest);
            tests.Add(LastChildFillProperty.ChangeClrSetterTest);
            tests.Add(LastChildFillProperty.ChangeSetValueTest);
            tests.Add(LastChildFillProperty.ClearValueResetsDefaultTest);
            tests.Add(LastChildFillProperty.CanBeStyledTest.Bug("488577", true));
            tests.Add(LastChildFillProperty.SetXamlAttributeTest);
            tests.Add(LastChildFillProperty.SetXamlElementTest);

            return tests;
        }

        /// <summary>
        /// Prepare a ContentControl to be used in a layout test.
        /// </summary>
        /// <param name="control">Control to prepare.</param>
        /// /// <param name="dock">Side to dock the control.</param>
        /// <param name="minWidth">Minimum width of the control.</param>
        /// <param name="minHeight">Minimum height of the control.</param>
        /// <returns>The prepared Control.</returns>
        protected virtual ContentControl Prepare(ContentControl control, Dock dock, double minWidth, double minHeight)
        {
            DockPanel.SetDock(control, dock);
            return Prepare(control, minWidth, minHeight);
        }

        /// <summary>
        /// Verify that arrange is called when changing the DockProperty.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that arrange is called when changing the DockProperty.")]
        public virtual void DockInvalidatesArrange()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            ContentControl control = new ContentControl { Content = 0 };
            TestAsync(
                Prepare(panel, 300, 300),
                () => panel.Children.Add(Prepare(control, 100, 100)),
                () => DockPanel.SetDock(control, Dock.Right));
        }

        /// <summary>
        /// Verify the availableSize provided during measuring.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the availableSize provided during measuring.")]
        public virtual void MeasureAvailableSize()
        {
            DockPanel panel = DefaultDockPanelToTest;
            TestAsync(
                Prepare(panel, 300, 300),
                () => panel.Children.Add(Prepare(new LayoutTestContentControl { Content = 0 }, 100, 100)),
                () => panel.Children.Add(Prepare(new LayoutTestContentControl { Content = 1 }, 100, 100)),
                () => Assert.AreEqual(
                    new Size(panel.Width, panel.Height),
                    (panel.Children[0] as LayoutTestContentControl).SuggestedAvailableSize),
                () => Assert.AreEqual(
                    new Size(panel.Width - 100, panel.Height),
                    (panel.Children[1] as LayoutTestContentControl).SuggestedAvailableSize));
        }

        #region Layout Left Sequences
        /// <summary>
        /// Test simple DockPanel layout with a single element on the left.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a single element on the left.")]
        public virtual void LayoutLeftSingle()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 400, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a single element on the left with
        /// the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a single element on the left with the LastChildFill property set to false.")]
        public virtual void LayoutLeftSingleWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a short sequence of elements on
        /// the left.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a short sequence of elements on the left.")]
        public virtual void LayoutLeftShortSequence()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(100, 0, 300, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a short sequence of elements on
        /// the left with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a short sequence of elements on the left with the LastChildFill property set to false.")]
        public virtual void LayoutLeftShortSequenceWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(100, 0, 100, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the
        /// left.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the left.")]
        public virtual void LayoutLeftSequence()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(100, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(200, 0, 200, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the left
        /// with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the left with the LastChildFill property set to false.")]
        public virtual void LayoutLeftSequenceWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(100, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(200, 0, 100, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the
        /// left that get clipped.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the left that get clipped.")]
        public virtual void LayoutLeftSequenceClipped()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 350, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(100, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(200, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(300, 0, 100, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the left
        /// with the LastChildFill property set to false that get clipped.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the left with the LastChildFill property set to false that get clipped.")]
        public virtual void LayoutLeftSequenceClippedWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 350, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(100, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(200, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(300, 0, 100, 400); });
        }
        #endregion Layout Left Sequences

        #region Layout Right Sequences
        /// <summary>
        /// Test simple DockPanel layout with a single element on the right.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a single element on the right.")]
        public virtual void LayoutRightSingle()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(0, 0, 400, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a single element on the right with
        /// the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a single element on the right with the LastChildFill property set to false.")]
        public virtual void LayoutRightSingleWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 0, 100, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a short sequence of elements on
        /// the right.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a short sequence of elements on the right.")]
        public virtual void LayoutRightShortSequence()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(0, 0, 300, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a short sequence of elements on
        /// the right with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a short sequence of elements on the right with the LastChildFill property set to false.")]
        public virtual void LayoutRightShortSequenceWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(200, 0, 100, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the
        /// right.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the right.")]
        public virtual void LayoutRightSequence()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(200, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(0, 0, 200, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the
        /// right with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the right with the LastChildFill property set to false.")]
        public virtual void LayoutRightSequenceWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(200, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(100, 0, 100, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the
        /// right that get clipped.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the right that get clipped.")]
        public virtual void LayoutRightSequenceClipped()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 350, 400),
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(250, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(150, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(50, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(0, 0, 100, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the
        /// right with the LastChildFill property set to false that get clipped.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the right with the LastChildFill property set to false that get clipped.")]
        public virtual void LayoutRightSequenceClippedWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 350, 400),
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(250, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(150, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(50, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(0, 0, 100, 400); });
        }
        #endregion Layout Right Sequences

        #region Layout Top Sequences
        /// <summary>
        /// Test simple DockPanel layout with a single element on the top.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a single element on the top.")]
        public virtual void LayoutTopSingle()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a single element on the top with
        /// the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a single element on the top with the LastChildFill property set to false.")]
        public virtual void LayoutTopSingleWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 100); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a short sequence of elements on
        /// the top.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a short sequence of elements on the top.")]
        public virtual void LayoutTopShortSequence()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 100, 400, 300); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a short sequence of elements on
        /// the top with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a short sequence of elements on the top with the LastChildFill property set to false.")]
        public virtual void LayoutTopShortSequenceWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 100, 400, 100); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the top.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the top.")]
        public virtual void LayoutTopSequence()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 100, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 200, 400, 200); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the top
        /// with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the top with the LastChildFill property set to false.")]
        public virtual void LayoutTopSequenceWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 100, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 200, 400, 100); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the top
        /// that get clipped.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the top that get clipped.")]
        public virtual void LayoutTopSequenceClipped()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 350),
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 100, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 200, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 300, 400, 100); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the top
        /// with the LastChildFill property set to false that get clipped.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the top with the LastChildFill property set to false that get clipped.")]
        public virtual void LayoutTopSequenceClippedWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 350),
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 100, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 200, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 300, 400, 100); });
        }
        #endregion Layout Top Sequences

        #region Layout Bottom Sequences
        /// <summary>
        /// Test simple DockPanel layout with a single element on the bottom.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a single element on the bottom.")]
        public virtual void LayoutBottomSingle()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 0, 400, 400); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a single element on the bottom
        /// with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a single element on the bottom with the LastChildFill property set to false.")]
        public virtual void LayoutBottomSingleWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 300, 400, 100); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a short sequence of elements on
        /// the bottom.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a short sequence of elements on the bottom.")]
        public virtual void LayoutBottomShortSequence()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 300, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 0, 400, 300); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a short sequence of elements on
        /// the bottom with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a short sequence of elements on the bottom with the LastChildFill property set to false.")]
        public virtual void LayoutBottomShortSequenceWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 300, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 200, 400, 100); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the
        /// bottom.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the bottom.")]
        public virtual void LayoutBottomSequence()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 400),
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 300, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 200, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 0, 400, 200); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the
        /// bottom with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the bottom with the LastChildFill property set to false.")]
        public virtual void LayoutBottomSequenceWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 300, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 200, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 100, 400, 100); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the
        /// bottom that get clipped.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the bottom that get clipped.")]
        public virtual void LayoutBottomSequenceClipped()
        {
            TestLayoutAsync(
                Prepare(DefaultDockPanelToTest, 400, 350),
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 250, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 150, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 50, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 0, 400, 100); });
        }

        /// <summary>
        /// Test simple DockPanel layout with a sequence of elements on the
        /// bottom with the LastChildFill property set to false that get clipped.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test simple DockPanel layout with a sequence of elements on the bottom with the LastChildFill property set to false that get clipped.")]
        public virtual void LayoutBottomSequenceClippedWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 350),
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 250, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 150, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 50, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 0, 400, 100); });
        }
        #endregion Layout Bottom Sequences

        #region Layout Opposite Sides
        /// <summary>
        /// Test the DockPanel layout with elements docked left and right.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with element docked left and right.")]
        public virtual void LayoutMixedLeftRight()
        {
            DockPanel panel = DefaultDockPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(100, 0, 300, 400); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked left and right with
        /// the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with element docked left and right with the LastChildFill property set to false.")]
        public virtual void LayoutMixedLeftRightWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 0, 100, 400); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked right and left.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked right and left.")]
        public virtual void LayoutMixedRightLeft()
        {
            DockPanel panel = DefaultDockPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 300, 400); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked right and left with
        /// the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked right and left with the LastChildFill property set to false.")]
        public virtual void LayoutMixedRightLeftWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 0, 100, 400); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked top and bottom.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Text the DockPanel layout with elements docked top and bottom.")]
        public virtual void LayoutMixedTopBottom()
        {
            DockPanel panel = DefaultDockPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 100, 400, 300); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked top and bottom with
        /// the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Text the DockPanel layout with elements docked top and bottom with the LastChildFill property set to false.")]
        public virtual void LayoutMixedTopBottomWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 300, 400, 100); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked bottom and top.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked bottom and top.")]
        public virtual void LayoutMixedBottomTop()
        {
            DockPanel panel = DefaultDockPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 300, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 300); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked bottom and top with
        /// the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked bottom and top with the LastChildFill property set to false.")]
        public virtual void LayoutMixedBottomTopWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(0, 300, 400, 100); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(0, 0, 400, 100); });
        }
        #endregion Layout Opposite Sides

        #region Layout Mixed
        /// <summary>
        /// Test the DockPanel layout with elements docked left, top, and
        /// bottom.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked left, top, and bottom.")]
        public virtual void LayoutMixedLeftTopBottom()
        {
            DockPanel panel = DefaultDockPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(100, 0, 300, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(100, 100, 300, 300); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked left, top, and bottom
        /// with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked left, top, and bottom with the LastChildFill property set to false.")]
        public virtual void LayoutMixedLeftTopBottomWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(100, 0, 300, 100); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(100, 300, 300, 100); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked left, top, and right.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked left, top, and right.")]
        public virtual void LayoutMixedLeftTopRight()
        {
            DockPanel panel = DefaultDockPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(100, 0, 300, 100); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(100, 100, 300, 300); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked left, top, and right
        /// with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked left, top, and right with the LastChildFill property set to false.")]
        public virtual void LayoutMixedLeftTopRightWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(100, 0, 300, 100); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 100, 100, 300); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked left, top, right, and
        /// bottom.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked left, top, right, and bottom.")]
        public virtual void LayoutMixedLeftTopRightBottom()
        {
            DockPanel panel = DefaultDockPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(100, 0, 300, 100); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 100, 100, 300); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(100, 100, 200, 300); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked left, top, right, and
        /// bottom with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked left, top, right, and bottom with the LastChildFill property set to false.")]
        public virtual void LayoutMixedLeftTopRightBottomWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(100, 0, 300, 100); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 100, 100, 300); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(100, 300, 200, 100); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked left, top, right, bottom, and left.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked left, top, right, bottom, and left.")]
        public virtual void LayoutMixedLeftTopRightBottomLeft()
        {
            DockPanel panel = DefaultDockPanelToTest;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(100, 0, 300, 100); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 100, 100, 300); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(100, 300, 200, 100); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(100, 100, 200, 200); });
        }

        /// <summary>
        /// Test the DockPanel layout with elements docked left, top, right, bottom, and left with the LastChildFill property set to false.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test the DockPanel layout with elements docked left, top, right, bottom, and left with the LastChildFill property set to false.")]
        public virtual void LayoutMixedLeftTopRightBottomLeftWithoutFill()
        {
            DockPanel panel = DefaultDockPanelToTest;
            panel.LastChildFill = false;
            TestLayoutAsync(
                Prepare(panel, 400, 400),
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(0, 0, 100, 400); },
                c => { Prepare(c, Dock.Top, 100, 100); return new Rect(100, 0, 300, 100); },
                c => { Prepare(c, Dock.Right, 100, 100); return new Rect(300, 100, 100, 300); },
                c => { Prepare(c, Dock.Bottom, 100, 100); return new Rect(100, 300, 200, 100); },
                c => { Prepare(c, Dock.Left, 100, 100); return new Rect(100, 100, 100, 200); });
        }
        #endregion Layout Mixed
    }
}