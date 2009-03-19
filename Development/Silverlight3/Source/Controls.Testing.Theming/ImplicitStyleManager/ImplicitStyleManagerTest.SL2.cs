// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Theming;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the System.Windows.Controls.ImplicitStyleManager
    /// class.
    /// </summary>
    public partial class ImplicitStyleManagerTest : TestBase
    {
        /// <summary>
        /// Associates a resource dictionary uri with an element.
        /// </summary>
        /// <param name="element">The element to associate the uri with.</param>
        /// <param name="uri">The uri of a resource dictionary.</param>
        public static void SetResourceDictionaryUri(FrameworkElement element, Uri uri)
        {
            ImplicitStyleManager.SetResourceDictionaryUri(element, uri);
        }

        // This set of tests covers setting and getting DPs, including invalid scenarios that cause exceptions.
        #region Setting and Getting DPs

        /// <summary>
        /// Test setting and getting ResourceDictionaryUri.
        /// </summary>
        [TestMethod]
        [Description("Test setting and getting ResourceDictionaryUri.")]
        public void TestResourceDictionaryUriSetAndGet()
        {
            Panel stackPanel = new StackPanel();
            Uri uri = new Uri("System.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/OuterStyleResourceDictionary.xaml", UriKind.Relative);

            ImplicitStyleManager.SetResourceDictionaryUri(stackPanel, uri);
            Assert.AreSame(ImplicitStyleManager.GetResourceDictionaryUri(stackPanel), uri);
        }

        /// <summary>
        /// Test that setting resource dictionary uri to a missing resource
        /// throws an exception.
        /// </summary>
        [TestMethod]
        [Description("Test that setting resource dictionary uri to a missing resource throws an exception.")]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void TestResourceDictionaryUriSetToMissingResourceThrowsException()
        {
            Panel stackPanel = new StackPanel();
            Uri uri = new Uri("System.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/non-existantresource.xaml", UriKind.Relative);

            ImplicitStyleManager.SetResourceDictionaryUri(stackPanel, uri);
        }

        /// <summary>
        /// Ensures that attempting to get ResourceDictionaryUri from a null 
        /// value throws an exception.
        /// </summary>
        [TestMethod]
        [Description("Ensures that attempting to get ResourceDictionaryUri from a null value throws an exception.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestResourceDictionaryUriGetOnNullElementThrowsException()
        {
            ImplicitStyleManager.GetResourceDictionaryUri((FrameworkElement)null);
        }

        /// <summary>
        /// This method tests that an ArgumentNullException is thrown if the ResourceDictionaryUri
        /// property is set on a null value.
        /// </summary>
        [TestMethod]
        [Description("Ensure that attempting to set ResourceDictionaryUri on a null value throws an exception.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestResourceDictionaryUriSetOnNullElementThrowsException()
        {
            Uri uri = new Uri("System.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/OuterStyleResourceDictionary.xaml", UriKind.Relative);
            ImplicitStyleManager.SetResourceDictionaryUri((FrameworkElement)null, uri);
        }

        /// <summary>
        /// Test setting and getting ApplicationResourceDictionaryUri.
        /// </summary>
        [TestMethod]
        [Description("Test setting and getting ApplicationResourceDictionaryUri.")]
        public void TestSetGetApplicationResourceDictionaryUri()
        {
            Uri uri = new Uri("System.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/OuterStyleResourceDictionary.xaml", UriKind.Relative);
            ImplicitStyleManager.ApplicationResourceDictionaryUri = uri;

            Assert.AreSame(ImplicitStyleManager.ApplicationResourceDictionaryUri, uri);
            ImplicitStyleManager.ApplicationResourceDictionaryUri = null;
        }

        /// <summary>
        /// Test that setting ApplicationResourceDictionaryUri to a missing resource
        /// throws an exception.
        /// </summary>
        [TestMethod]
        [Description("Test that setting ApplicationResourceDictionaryUri to a missing resource throws an exception.")]
        [ExpectedException(typeof(ResourceNotFoundException))]
        public void TestApplicationResourceDictionaryUriInvalidUri()
        {
            Uri uri = new Uri("System.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/non-existantresource.xaml", UriKind.Relative);
            try
            {
                ImplicitStyleManager.ApplicationResourceDictionaryUri = uri;
            }
            finally
            {
                Assert.AreSame(null, ImplicitStyleManager.ApplicationResourceDictionaryUri);
            }
        }
        #endregion
    }
}