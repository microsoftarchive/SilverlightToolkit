// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// TreeViewItemCheckBox unit tests.
    /// </summary>
    [TestClass]
    [Tag("TreeView")]
    [Tag("TreeViewItem")]
    [Tag("TreeViewExtensions")]
    public partial class TreeViewItemCheckBoxTest : ContentControlTest
    {
        #region ContentControls to test
        /// <summary>
        /// Gets a default instance of ContentControl (or a derived type) to
        /// test.
        /// </summary>
        public override ContentControl DefaultContentControlToTest
        {
            get { return DefaultTreeViewItemCheckBoxToTest; }
        }

        /// <summary>
        /// Gets instances of ContentControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<ContentControl> ContentControlsToTest
        {
            get { return TreeViewItemCheckBoxesToTest.OfType<ContentControl>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenContentControl (or derived types) to
        /// test.
        /// </summary>
        public override IEnumerable<IOverriddenContentControl> OverriddenContentControlsToTest
        {
            get { yield break; }
        }
        #endregion ContentControls to test

        #region TreeViewItemCheckBoxes to test
        /// <summary>
        /// Gets a default instance of TreeViewItemCheckBox (or a derived type)
        /// to test.
        /// </summary>
        public virtual TreeViewItemCheckBox DefaultTreeViewItemCheckBoxToTest
        {
            get { return new TreeViewItemCheckBox(); }
        }

        /// <summary>
        /// Gets instances of TreeViewItemCheckBox (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<TreeViewItemCheckBox> TreeViewItemCheckBoxesToTest
        {
            get
            {
                yield return DefaultTreeViewItemCheckBoxToTest;
                yield return new TreeViewItemCheckBox { Content = "Checked?" };
            }
        }
        #endregion TreeViewItemCheckBoxes to test

        /// <summary>
        /// Initializes a new instance of the TreeViewItemCheckBoxTest class.
        /// </summary>
        public TreeViewItemCheckBoxTest()
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(.5, 0);
            brush.EndPoint = new Point(.5, 1);
            brush.GradientStops.Add(new GradientStop { Offset = 0, Color = Color.FromArgb(0xFF, 0xA3, 0xAE, 0xB9) });
            brush.GradientStops.Add(new GradientStop { Offset = 0.375, Color = Color.FromArgb(0xFF, 0x83, 0x99, 0xA9) });
            brush.GradientStops.Add(new GradientStop { Offset = 0.375, Color = Color.FromArgb(0xFF, 0x71, 0x85, 0x97) });
            brush.GradientStops.Add(new GradientStop { Offset = 1, Color = Color.FromArgb(0xFF, 0x61, 0x75, 0x84) });
            BorderBrushProperty.DefaultValue = brush;

            BackgroundProperty.DefaultValue = new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x8D, 0xCA));
            BorderThicknessProperty.DefaultValue = new Thickness(1);
            PaddingProperty.DefaultValue = new Thickness(4, 1, 0, 0);
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            List<DependencyPropertyTestMethod> tests = new List<DependencyPropertyTestMethod>(base.GetDependencyPropertyTests());
            tests.RemoveTests(ContentProperty.DoesNotChangeVisualStateTest(null, "Test"));
            return tests;
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> states = DefaultTreeViewItemCheckBoxToTest.GetType().GetVisualStates();
            Assert.AreEqual(12, states.Count, "Incorrect number of template states");
            Assert.AreEqual("CommonStates", states["Normal"], "Failed to find expected state Normal!");
            Assert.AreEqual("CommonStates", states["MouseOver"], "Failed to find expected state MouseOver!");
            Assert.AreEqual("CommonStates", states["Pressed"], "Failed to find expected state Pressed!");
            Assert.AreEqual("CommonStates", states["Disabled"], "Failed to find expected state Disabled!");
            Assert.AreEqual("FocusStates", states["Focused"], "Failed to find expected state Focused!");
            Assert.AreEqual("FocusStates", states["Unfocused"], "Failed to find expected state Unfocused!");
            Assert.AreEqual("ValidationStates", states["Valid"], "Failed to find expected state Valid!");
            Assert.AreEqual("ValidationStates", states["InvalidFocused"], "Failed to find expected state InvalidFocused!");
            Assert.AreEqual("ValidationStates", states["InvalidUnfocused"], "Failed to find expected state InvalidUnfocused!");
            Assert.AreEqual("CheckStates", states["Unchecked"], "Failed to find expected state Unchecked!");
            Assert.AreEqual("CheckStates", states["Indeterminate"], "Failed to find expected state Indeterminate!");
            Assert.AreEqual("CheckStates", states["Checked"], "Failed to find expected state Checked!");
        }

        /// <summary>
        /// Verify the TreeViewItemCheckBoxes associated with their parent
        /// TreeViewItems.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemCheckBoxes associated with their parent TreeViewItems.")]
        public virtual void AssociateWithTreeViewItems()
        {
            TreeViewItemCheckBox c1 = new TreeViewItemCheckBox { Content = "First" };
            TreeViewItemCheckBox c2 = new TreeViewItemCheckBox { Content = "Second" };
            TreeViewItemCheckBox c3 = new TreeViewItemCheckBox { Content = "Third" };
            TreeViewItemCheckBox c4 = new TreeViewItemCheckBox { Content = "Fourth" };
            TreeViewItemCheckBox c5 = new TreeViewItemCheckBox { Content = "Fifth" };
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items(c1).Expand().Named(out t1)
                    .Items(c2).Expand().Named(out t2)
                        .Item(c3).Expand().Named(out t3).Set(t => t.SetIsChecked(true))
                        .Item(c4).Expand().Named(out t4)
                    .EndItems()
                .EndItems()
                .Item(c5).Named(out t5).Set(t => t.SetIsChecked(true));

            TestAsync(
                view,
                () => Assert.AreEqual(t1.GetIsChecked(), c1.IsChecked, "First didn't match!"),
                () => Assert.AreEqual(t2.GetIsChecked(), c2.IsChecked, "Second didn't match!"),
                () => Assert.AreEqual(t3.GetIsChecked(), c3.IsChecked, "Third didn't match!"),
                () => Assert.AreEqual(t4.GetIsChecked(), c4.IsChecked, "Fourth didn't match!"),
                () => Assert.AreEqual(t5.GetIsChecked(), c5.IsChecked, "Fifth didn't match!"));
        }
    }
}