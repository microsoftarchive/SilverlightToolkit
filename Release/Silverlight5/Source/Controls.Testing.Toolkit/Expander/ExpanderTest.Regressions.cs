// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Bug regression tests for the System.Windows.Controls.Expander class.
    /// </summary>
    public partial class ExpanderTest
    {
        /// <summary>
        /// Tests that the width of the the expander header is equal to what is specified by the Expander Width.
        /// </summary>
        [TestMethod]
        [Description("Tests that the width of the the expander header is equal to what is specified by the Expander Width.")]
        [Asynchronous]
        [Bug("526627 - Expander -  Incompatbility with WPF, the width of expander header is overruled by the width of its textblock content.", Fixed = true)]
        public void TestHeaderWidthOfExpander()
        {
            Expander exp = DefaultExpanderToTest;
            exp.Width = 400;
            TextBlock tb = new TextBlock();
            tb.Width = 300;
            tb.Text = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua";
            tb.TextWrapping = TextWrapping.Wrap;
            exp.Content = tb;
            ToggleButton tgbn = new ToggleButton();
            TestAsync(
                exp,
                () => tgbn = (ToggleButton)TestExtensions.GetChildrenByType(exp, typeof(ToggleButton)),
                () => Assert.IsTrue(TestExtensions.AreClose(exp.ActualWidth, tgbn.ActualWidth)));
        }

        /// <summary>
        /// Tests that the header of expander always stays at the top-left corner of the expander itself.
        /// </summary>
        [TestMethod]
        [Description("Tests that the header of expander always stays at the top-left corner of the expander itself.")]
        [Asynchronous]
        [Bug("526658 - Expander - HorizontalContentAlignment and VerticalContentAlignment are not working for Expander.", Fixed = true)]
        public void TestHeaderAlignmentOfExpander()
        {
            Expander exp = DefaultExpanderToTest;
            exp.Width = 400;
            exp.Height = 300;
            exp.HorizontalContentAlignment = HorizontalAlignment.Center;
            exp.Padding = new Thickness(0.0);
            Ellipse elps = new Ellipse();
            Point pt = new Point();
            TestAsync(
                exp,
                () => elps = (Ellipse)TestExtensions.GetChildrenByType(exp, typeof(Ellipse)),
                () => pt = elps.TransformToVisual(exp).Transform(new Point(0, 0)),
                () => Assert.IsTrue(pt.X < 5), //// Make sure the ellipse of header/toggle button is at the top left corner of expander. 
                () => Assert.IsTrue(pt.Y < 5)); //// The pixel difference is less than 5.
        }

        /// <summary>
        /// Tests that when HorizontalContentAlignment and VerticalContentAlignment are set to 'Right' and 'Bottom' respectively (StretchDirection = Down),
        /// the content is at the right-bottom position.  
        /// </summary>
        [TestMethod]
        [Description("Tests that when HorizontalContentAlignment and VerticalContentAlignment are set to 'Right' and 'Bottom' respectively, the content is at the right-bottom position.")]
        [Asynchronous]
        [Bug("526658 - Expander - HorizontalContentAlignment and VerticalContentAlignment are not working for Expander", Fixed = true)]
        public void TestHorizontalAndVerticalContentAlignmentOfExpander()
        {
            Expander exp = DefaultExpanderToTest;
            exp.Width = 400;
            exp.Height = 300;
            TextBlock tb = new TextBlock();
            tb.Width = 300;
            tb.Height = 100;
            tb.Text = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua";
            tb.TextWrapping = TextWrapping.Wrap;
            exp.Content = tb;
            exp.HorizontalContentAlignment = HorizontalAlignment.Right;
            exp.VerticalContentAlignment = VerticalAlignment.Bottom;
            exp.IsExpanded = true;
            Point pt = new Point();
            TestAsync(
                exp,
                () => pt = tb.TransformToVisual(exp).Transform(new Point(0, 0)),
                () => Assert.AreEqual(exp.ActualWidth - tb.Width - 1, pt.X), // -1 to account default BoarderThickness of 1
                () => Assert.AreEqual(exp.ActualHeight - tb.Height - 1, pt.Y));
        }

        /// <summary>
        /// Tests that when setting HorizontalContentAlignment and VerticalContentAlignment both to strech and the width of content being specified
        /// the expander content is is located in the center, as what WPF does.  
        /// </summary>
        [TestMethod]
        [Description("Tests that when setting HorizontalContentAlignment and VerticalContentAlignment both to strech and the width of content being specified, the expander content is located in the center, as what WPF does.")]
        [Asynchronous]
        [Bug("526658 - Expander - HorizontalContentAlignment and VerticalContentAlignment are not working for Expander", Fixed = true)]
        public void TestStretchContentAlignmentOfExpander()
        {
            Expander exp = DefaultExpanderToTest;
            exp.Width = 400;
            exp.Height = 300;
            exp.ExpandDirection = ExpandDirection.Down;
            TextBlock tb = new TextBlock();
            tb.Width = 300;
            tb.Text = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua";
            tb.TextWrapping = TextWrapping.Wrap;
            exp.Content = tb;
            exp.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            exp.VerticalContentAlignment = VerticalAlignment.Stretch;
            exp.IsExpanded = true;
            Point pt = new Point();
            TestAsync(
                exp,
                () => pt = tb.TransformToVisual(exp).Transform(new Point(0, 0)),
                () => Assert.AreEqual((exp.ActualWidth - tb.Width) / 2, pt.X));
        }

        /// <summary>
        /// Tests that the content of expander fills the available space as in WPF, using a button as example.
        /// </summary>
        [TestMethod]
        [Description("Tests that the content of expander fills the available space as in WPF, using a button as example.")]
        [Asynchronous]
        [Bug("526583 - Expander - Content does not fill available space", Fixed = true)]
        public void TestContentFillSpaceOfExpander()
        {
            Expander exp = DefaultExpanderToTest;
            exp.Width = 258;
            exp.Height = 480;
            Button bn = new Button();
            bn.Content = "Button";
            exp.Content = bn;
            exp.HorizontalAlignment = HorizontalAlignment.Left;
            exp.IsExpanded = true;
            ToggleButton tb = new ToggleButton();
            TestAsync(
                exp,
                () => tb = (ToggleButton)TestExtensions.GetChildrenByType(exp, typeof(ToggleButton)),
                () => Assert.AreEqual(exp.ActualWidth - 2, bn.ActualWidth), // -2 to account for default BorderThickness of 1
                () => Assert.IsTrue(TestExtensions.AreClose(exp.ActualHeight - tb.ActualHeight, bn.ActualHeight))); //// The difference in height is less than 5 percent
        }

        /// <summary>
        /// Tests that if the behavior of ScrollViewer is consistent with WPF.
        /// </summary>
        [TestMethod]
        [Description("Tests that if the behavior of ScrollViewer is consistent with WPF.")]
        [Asynchronous]
        [Bug("526719 - Expander - Incompatbility with WPF, ScrollViewer won't show up unless proper width for the TextBlock is specified", Fixed = true)]
        public void TestScrollViewerOnExpander()
        {
            Expander exp = DefaultExpanderToTest;
            exp.Width = 200;
            exp.Height = 400;
            exp.Header = "MyExpander";
            exp.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            ScrollViewer sv = new ScrollViewer();
            sv.Height = 50;
            TextBlock tb = new TextBlock();
            tb.Width = 300;
            tb.Height = 100;
            tb.Text = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua";
            tb.TextWrapping = TextWrapping.Wrap;
            sv.Content = tb;
            exp.Content = sv;
            exp.IsExpanded = true;
            Point pt = new Point();
            ToggleButton tgbn = new ToggleButton();
            TestAsync(
                exp,
                () => Assert.IsTrue(sv.Visibility == Visibility.Visible),
                () => Assert.IsTrue(sv.VerticalScrollBarVisibility == ScrollBarVisibility.Visible),
                () => pt = sv.TransformToVisual(exp).Transform(new Point(0, 0)),
                () => tgbn = (ToggleButton)TestExtensions.GetChildrenByType(exp, typeof(ToggleButton)),
                () => Assert.IsTrue(TestExtensions.AreClose((exp.ActualHeight - tgbn.ActualHeight - sv.ActualHeight) / 2, pt.Y - tgbn.ActualHeight)));
        }

        /// <summary>
        /// Tests the alignment of togglebutton relative to the header of expander.
        /// </summary>
        [TestMethod]
        [Description("Test the alignment of togglebutton relative to the header of expander.")]
        [Asynchronous]
        [Bug("526830 - Expander  - ToggleButton aligned To Left/Right on ExpandDirection.Left instead of Center", Fixed = true)]

        public void TestAlignmentOfToggleButtonToExpanderHeader()
        {
            Expander exp = DefaultExpanderToTest;
            exp.Width = 200;
            exp.Header = "Header of Expander";
            exp.ExpandDirection = ExpandDirection.Left;
            exp.Padding = new Thickness(0.0);
            ToggleButton tgbn = new ToggleButton();
            Ellipse elps = new Ellipse();
            Point pt = new Point();
            TestAsync(
                exp,
                () => tgbn = (ToggleButton)TestExtensions.GetChildrenByType(exp, typeof(ToggleButton)),
                () => elps = (Ellipse)TestExtensions.GetChildrenByType(exp, typeof(Ellipse)),
                () => pt = elps.TransformToVisual(tgbn).Transform(new Point(0, 0)),
                () => Assert.IsTrue(TestExtensions.AreClose(tgbn.ActualWidth / 2, pt.X - elps.ActualWidth / 2)));
        }
    }
}