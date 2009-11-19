// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Label unit tests.
    /// </summary>
    [TestClass]
    public partial class LabelTest : ContentControlTest
    {
        #region ContentControls to test
        /// <summary>
        /// Gets a default instance of ContentControl (or a derived type) to
        /// test.
        /// </summary>
        public override ContentControl DefaultContentControlToTest
        {
            get { return DefaultLabelToTest; }
        }

        /// <summary>
        /// Gets instances of ContentControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<ContentControl> ContentControlsToTest
        {
            get { return LabelsToTest.OfType<ContentControl>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenContentControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenContentControl> OverriddenContentControlsToTest
        {
            get { return OverriddenLabelsToTest.OfType<IOverriddenContentControl>(); }
        }
        #endregion ContentControls to test

        #region Labels to test
        /// <summary>
        /// Gets a default instance of Label (or a derived type) to test.
        /// </summary>
        public virtual Label DefaultLabelToTest
        {
            get { return new Label(); }
        }

        /// <summary>
        /// Gets instances of Label (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<Label> LabelsToTest
        {
            get
            {
                yield return DefaultLabelToTest;
                yield return new Label { Foreground = new SolidColorBrush(Colors.Red) };
                yield return new Label { Content = new Ellipse { Fill = new SolidColorBrush(Colors.Red) } };
                yield return new Label { Style = Application.Current.Resources["CustomLabel"] as Style };

                foreach (IOverriddenContentControl overriddenControl in OverriddenLabelsToTest)
                {
                    Assert.IsInstanceOfType(overriddenControl, typeof(Label));
                    yield return (Label)overriddenControl;
                }
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenLabel (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<IOverriddenContentControl> OverriddenLabelsToTest
        {
            get { yield break; }
        }
        #endregion Labels to test

        /// <summary>
        /// Initializes a new instance of the LabelTest class.
        /// </summary>
        public LabelTest()
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
            return TagInherited(base.GetDependencyPropertyTests());
        }

        /// <summary>
        /// Ensure the Margin property is not template bound.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure the Margin property is not template bound.")]
        public virtual void MarginIsNotTemplateBound()
        {
            Grid root = new Grid();
            Label label = DefaultLabelToTest;
            ContentControl control = null;

            Point original = new Point();
            Point modified = new Point();
            root.Children.Add(label);
            Thickness margin = label.Margin;

            TestAsync(
                5,
                root,
                () => control = label.GetChildrenByType(typeof(ContentControl)) as ContentControl,
                () => Assert.IsNotNull("Could not find the ContentControl"),
                () => original = control.TransformToVisual(root).Transform(new Point()),
                () => label.Margin = new Thickness(margin.Left + 2, margin.Top + 2, margin.Right + 2, margin.Bottom + 2),
                () => modified = control.TransformToVisual(root).Transform(new Point()),
                () => Assert.AreEqual(original.X + 2, modified.X, "Unexpected X value"),
                () => Assert.AreEqual(original.Y + 2, modified.Y, "Unexpected Y value"));
        }
    }
}