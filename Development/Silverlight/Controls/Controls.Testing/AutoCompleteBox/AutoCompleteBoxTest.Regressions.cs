// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Regression tests for AutoCompleteBox.
    /// </summary>
    public partial class AutoCompleteBoxTest
    {
        /// <summary>
        /// Test that if the Basic style inherited from Control(Background,BorderBrush,BorderThickness,etc) work properly.
        /// </summary>
        [TestMethod]
        [Description("Test that if the Basic style inherited from Control(Background,BorderBrush,BorderThickness,etc) work properly.")]
        [Asynchronous]
        [Bug("530456 - AutoComplete: some properties inherit from Control (Background,BorderBrush,BorderThickness,etc) don't work", Fixed = true)]
        public void TestAutoCompleteBoxBasicStyle()
        {
            AutoCompleteBox autoComplete = new AutoCompleteBox();

            SolidColorBrush background = new SolidColorBrush(Colors.Green);
            SolidColorBrush borderBrush = new SolidColorBrush(Colors.Orange);
            Thickness borderThickness = new Thickness(1);
            Thickness padding = new Thickness(2);

            TestAsync(
                autoComplete,
                () => autoComplete.Background = background,
                () => autoComplete.BorderBrush = borderBrush,
                () => autoComplete.BorderThickness = borderThickness,
                () => autoComplete.Padding = padding,
                () => Assert.AreEqual(Colors.Green, (autoComplete.Background as SolidColorBrush).Color),
                () => Assert.AreEqual(Colors.Orange, (autoComplete.BorderBrush as SolidColorBrush).Color),
                () => Assert.AreEqual(borderThickness, autoComplete.BorderThickness),
                () => Assert.AreEqual(padding, autoComplete.Padding));
        }
    }
}
