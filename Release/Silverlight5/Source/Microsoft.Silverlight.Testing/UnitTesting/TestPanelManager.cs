// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// A manager for the underlying TestSurface Panel.
    /// </summary>
    public class TestPanelManager
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        public TestPanelManager()
        {
        }

        /// <summary>
        /// The test page object.
        /// </summary>
        private ITestPage _testPage;

        /// <summary>
        /// A value indicating whether the panel is dirty.
        /// </summary>
        private bool _dirty;

        /// <summary>
        /// Gets or sets the Reference to the TestPage user control.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Provided for future framework use.")]
        internal ITestPage TestPage
        {
            get { return _testPage; }
            set { _testPage = value; }
        }

        /// <summary>
        /// Gets the TestSurface Panel, and tracks the use for the 
        /// current test method.  When the test completes, the panel children 
        /// will be cleared automatically.
        /// </summary>
        public Panel TestPanel
        {
            get
            {
                _dirty = true;
                return _testPage.TestPanel;
            }
        }

        /// <summary>
        /// Remove the children from the test surface, if it has 
        /// been used.
        /// </summary>
        public void ClearUsedChildren()
        {
            if (_dirty)
            {
                ClearChildren();
            }
        }

        /// <summary>
        /// Remove the children from the test surface.
        /// </summary>
        public void ClearChildren()
        {
            if (_testPage != null && _testPage.TestPanel != null)
            {
                _testPage.TestPanel.Children.Clear();
            }

            _dirty = false;
        }
    }
}