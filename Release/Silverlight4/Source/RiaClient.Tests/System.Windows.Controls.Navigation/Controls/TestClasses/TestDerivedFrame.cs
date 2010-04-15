// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// A Frame control which navigates in its constructor to verify that deferred navigation takes place
    /// correctly.
    /// </summary>
    public sealed class TestDerivedFrame : Frame
    {
        private static readonly string TestPagesPath = @"/System.Windows.Controls.Navigation/Controls/TestPages/";
        public static readonly string DefaultPage = TestPagesPath + "Page1.xaml";
        public static readonly string DefaultSource = TestPagesPath + "Page2.xaml";

        public TestDerivedFrame()
        {
            this.Navigate(new Uri(DefaultPage, UriKind.Relative));
        }
    }
}
