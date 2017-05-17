// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A user control that should be used as the root visual for a Silverlight 
    /// plugin if developers would like to use the advanced TestSurface 
    /// functionality within Microsoft.Silverlight.Testing.
    /// 
    /// The TestSurface is automatically cleared after each test scenario 
    /// completes, eliminating the need for many additional cleanup methods.
    /// </summary>
    public partial class ClassicTestPage : UserControl, ITestPage
    {
        /// <summary>
        /// Initializes the TestPage object.
        /// </summary>
        public ClassicTestPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the test surface, a dynamic Panel that removes its children 
        /// elements after each test completes.
        /// </summary>
        public Panel TestPanel
        {
            get { return TestLayoutRoot; }
        }
    }
}