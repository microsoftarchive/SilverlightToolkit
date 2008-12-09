// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Windows.Controls.Primitives;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Bug regression tests for the Microsoft.Windows.Controls.Input.NumericUpDown class.
    /// </summary>
    public partial class NumericUpDownTest
    {
        /// <summary>
        /// Test if the actual width of NUD is equal to the specified width.
        /// </summary>
        [TestMethod]
        [Description("Test if the actual width of NUD is equal to the specified width.")]
        [Asynchronous]
        [Bug("530122 - NumericUpDown - I would expect Width to be honored.", Fixed = true)]
        public void TestActualWidthOfNumericUpDownControl()
        {
            NumericUpDown nud = new NumericUpDown();
            nud.Width = 150;
            TestAsync(
                nud,
                () => Assert.AreEqual(nud.ActualWidth, nud.Width));
        }

        /// <summary>
        /// Test when tabbing, the focus should go from NumericUpDown to the next control.
        /// </summary>
        [TestMethod]
        [Description("Test when tabbing, the focus should go from NumericUpDown to the next control.")]
        [Asynchronous]
        [Bug("530086 - NumericUpDown - when tabbing, focus should go from NumericUpDown to the next control.", Fixed = true)]
        public void TestTabbingOutNumericUpDown()
        {
            NumericUpDown nud = DefaultNumericUpDownToTest;
            nud.DecimalPlaces = 1;
            nud.Value = 50.8;
            TextBox tb = new TextBox();
            RepeatButton rb = new RepeatButton();
            TestAsync(
                nud,
                () => tb = (TextBox)TestExtensions.GetChildrenByType(nud, typeof(TextBox)),
                () => tb.Focus(),
                () => rb = (RepeatButton)TestExtensions.GetChildrenByType(nud, typeof(RepeatButton)),
                () => rb.Focus(),
                () => Assert.IsFalse(rb.IsFocused));
        }

        /// <summary>
        /// Test that the textbox text is highlighted when giving focus to NumericUpDown control.
        /// </summary>
        [TestMethod]
        [Description("Test that the textbox text is highlighted when giving focus to NumericUpDown control.")]
        [Asynchronous]
        [Tag(Tags.RequiresFocus)]
        [Bug("515485 - NumericUpDown - Giving it focus should highlight the value.", Fixed = true)]
        public void TestHighlightOfValueWhenFocused()
        {
            NumericUpDown nud = DefaultNumericUpDownToTest;
            nud.IsEnabled = true;
            nud.Maximum = 500;
            nud.Value = 244;
            TextBox tb = new TextBox(); 
            TestAsync(
                nud,
                () => tb = (TextBox)TestExtensions.GetChildrenByType(nud, typeof(TextBox)),
                () => tb.Focus(),
                () => Assert.AreEqual(nud.Value, Convert.ToDouble(tb.SelectedText, CultureInfo.CurrentCulture)));
        }

        /// <summary>
        /// Test that the actual width of the NumericUpDown control is fixed, regardless of the length of the value that is typed in.
        /// </summary>
        [TestMethod]
        [Description("Test that the actual width of the NumericUpDown control is fixed, regardless of the length of the value that is typed in.")]
        [Asynchronous]
        [Bug("530193 - NumericUpDown - Typing numbers of varying length increases and decreases the width of NUD.", Fixed = true)]
        public void TestActualWidthWithVariousDecimalPlaces()
        {
            NumericUpDown nud = DefaultNumericUpDownToTest;
            nud.Width = 85;
            nud.DecimalPlaces = 4;
            double width1 = 0.0;
            double width2 = 0.0;
            TestAsync(
                nud,
                () => nud.Value = 12.345,
                () => width1 = nud.ActualWidth,
                () => nud.Value = 12.3456789000000000000,
                () => width2 = nud.ActualWidth,
                () => Assert.AreEqual(width1, width2),
                () => Assert.AreEqual(nud.ActualWidth, nud.Width));
        }

        /// <summary>
        /// Test that the precision of value in NUD will not be lost after setting DecimalPlaces.
        /// </summary>
        [TestMethod]
        [Description("Test that the precision of value in NUD will not be lost after setting DecimalPlaces.")]
        [Asynchronous]
        [Bug("530315 - NumericUpDown - Precision of Value is lost (as is shown after setting DecimalPlaces).", Fixed = true)]
        public void TestPrecisionOfValueAfterSettingDecimalPlaces()
        {
            NumericUpDown nud = DefaultNumericUpDownToTest;
            double val = 2.46;
            nud.DecimalPlaces = 1;
            nud.Value = val;
            nud.DecimalPlaces = 2;
            TextBox tb = new TextBox();
            TestAsync(
                nud,
                () => tb = (TextBox)TestExtensions.GetChildrenByType(nud, typeof(TextBox)),
                () => Assert.AreEqual(val, nud.Value),
                () => Assert.AreEqual(val, Convert.ToDouble(tb.Text, CultureInfo.CurrentCulture)));
        }

        /// <summary>
        /// Test that if the value of NUD can be refreshed when DecimalPlaces changes.
        /// </summary>
        [TestMethod]
        [Description("Test that if the value of NUD can be refreshed when DecimalPlaces changes.")]
        [Asynchronous]
        [Bug("530319 - NumericUpDown - If DecimalPlaces changes, I would expect the value to be refreshed but it's not.", Fixed = true)]
        public void TestValueRefreshAfterDecimalPlacesChange()
        {
            NumericUpDown nud = DefaultNumericUpDownToTest;
            nud.Maximum = 1000.0;
            double val = 534.6781325968;
            nud.Value = val;
            nud.DecimalPlaces = 2;
            TextBox tb = new TextBox();
            double num1 = 0.0;
            double num2 = 0.0;
            TestAsync(
                nud,
                () => tb = (TextBox)TestExtensions.GetChildrenByType(nud, typeof(TextBox)),
                () => num1 = Convert.ToDouble(tb.Text, CultureInfo.CurrentCulture),
                () => nud.DecimalPlaces = 5,
                () => num2 = Convert.ToDouble(tb.Text, CultureInfo.CurrentCulture),
                () => Assert.AreEqual(Math.Round(val, 2), num1),
                () => Assert.AreEqual(Math.Round(val, 5), num2));
        }

        /// <summary>
        /// Test that if the NUD properties such as Foreground is honored.
        /// </summary>
        [TestMethod]
        [Description("Test that if the NUD properties such as Foreground is honored")]
        [Asynchronous]
        [Bug("515529 - NumericUpDown - Foreground property is not honored", Fixed = true)]
        public void TestForegroundPropertyOfNumericUpDown()
        {
            NumericUpDown nud = DefaultNumericUpDownToTest;
            nud.Foreground = new SolidColorBrush(Colors.Red);
            TextBox tb = new TextBox();
            TestAsync(
                nud,
                () => tb = (TextBox)TestExtensions.GetChildrenByType(nud, typeof(TextBox)),
                () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)nud.Foreground).Color));
        }
    }
}
