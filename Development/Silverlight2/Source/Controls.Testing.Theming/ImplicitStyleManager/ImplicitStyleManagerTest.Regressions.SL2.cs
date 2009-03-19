// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.Theming;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Regression tests for the System.Windows.Controls.ImplicitStyleManager
    /// class.
    /// </summary>
    public partial class ImplicitStyleManagerTest
    {
        /// <summary>
        /// Sets the application resource dictionary.
        /// </summary>
        /// <param name="uri">The uri of the resource dictionary.</param>
        private static void SetApplicationResourceDictionaryUri(Uri uri)
        {
            ImplicitStyleManager.ApplicationResourceDictionaryUri = uri;
        }

        /// <summary>
        /// Tests that styles in the Application's ResourceDictionaryUri are not applied when 
        /// UseApplicationResources = false.
        /// </summary>
        [TestMethod]
        [Description("Tests that styles in the Application's ResourceDictionaryUri are not applied when UseApplicationResources = false.")]
        [Asynchronous]
        public void TestApplicationResourceDictionaryUriNotApplied()
        {
            SetApplicationResourceDictionaryUri(innerUri);
            ImplicitStyleManager.UseApplicationResources = false;

            Button btn = new Button { Content = "Hello" };
            ImplicitStyleManager.SetApplyMode(btn, ImplicitStylesApplyMode.Auto);

            TestAsync(
                btn,
                () => Assert.IsInstanceOfType(btn.Foreground, typeof(SolidColorBrush)),
                () => Assert.AreEqual(Colors.Black, ((SolidColorBrush)btn.Foreground).Color),
                () => SetApplicationResourceDictionaryUri(null),
                () => ImplicitStyleManager.UseApplicationResources = true);
        }

        /// <summary>
        /// Tests that styles in the Application's resources are not applied when UseApplicationResource=false.
        /// </summary>
        [TestMethod]
        [Description("Tests that styles in the Application's resources are not applied when UseApplicationResource=false.")]
        [Asynchronous]
        public void TestApplicationResourcesNotApplied()
        {
            AddApplicationResources(Colors.Green);
            ImplicitStyleManager.UseApplicationResources = false;

            Button btn = new Button { Content = "Hello" };
            ImplicitStyleManager.SetApplyMode(btn, ImplicitStylesApplyMode.Auto);

            TestAsync(
                btn,
                () => Assert.IsInstanceOfType(btn.Foreground, typeof(SolidColorBrush)),
                () => Assert.AreEqual(Colors.Black, ((SolidColorBrush)btn.Foreground).Color),
                () => Application.Current.Resources.Clear(),
                () => ImplicitStyleManager.UseApplicationResources = true);
        }
    }
}