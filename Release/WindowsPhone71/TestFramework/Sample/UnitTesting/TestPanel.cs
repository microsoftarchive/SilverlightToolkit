// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.UnitTesting
{
    /// <summary>
    /// Tests for the TestPanel feature in the unit test framework.
    /// </summary>
    [TestClass]
    public class TestPanelTest : PresentationTest
    {
        /// <summary>
        /// Verify that bug 484164 was fixed properly.
        /// </summary>
        [TestMethod]
        [Description("Verify that bug 484164 was fixed properly.")]
        public void Bug484164()
        {
            Button b = new Button { Content = "Button" };
            string n = "Bob";
            b.SetValue(FrameworkElement.NameProperty, n);
            TestPanel.Children.Add(b);
            Assert.IsNotNull(b.FindName(n));
        }

        /// <summary>
        /// Verify that bug 484164 was fixed properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that bug 484164 was fixed properly.")]
        public void Bug484164b()
        {
            Button b = new Button { Content = "Button" };
            string n = "Bob";
            b.SetValue(FrameworkElement.NameProperty, n);
            TestPanel.Children.Add(b);
            EnqueueDelay(TimeSpan.FromSeconds(.2));
            EnqueueCallback(() => Assert.IsNotNull(b.FindName(n)));
            EnqueueTestComplete();
        }
    }
}