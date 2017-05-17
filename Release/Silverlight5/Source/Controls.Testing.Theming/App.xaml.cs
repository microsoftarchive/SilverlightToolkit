// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using Microsoft.Silverlight.Testing;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for System.Windows.Controls.Theming.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the App class.
        /// </summary>
        public App()
        {
            Startup += delegate
            {
                UnitTestSettings settings = UnitTestSystem.CreateDefaultSettings();
                settings.SampleTags = new System.Collections.Generic.List<string>
                {
                    "All",
                    "ImplicitStyleManagerTest",
                    "ImplicitStyleManagerTest-DP",
                    "ImplicitStyleManagerTest+InvalidResourceExceptionTest",
                    "(ImplicitStyleManagerTest+InvalidResourceExceptionTest)-DP",
                    "ImplicitStyleManagerTest*DP",
                    "!DP",
                };
                RootVisual = UnitTestSystem.CreateTestPage(settings);
            };
        }
    }
}