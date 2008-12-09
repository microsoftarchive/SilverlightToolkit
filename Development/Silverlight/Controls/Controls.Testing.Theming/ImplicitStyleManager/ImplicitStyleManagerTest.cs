// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.Theming;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the Microsoft.Windows.Controls.ImplicitStyleManager
    /// class.
    /// </summary>
    [TestClass]
    public partial class ImplicitStyleManagerTest : TestBase
    {
        /// <summary>
        /// Stores the styles to be associated with another container nested in
        /// the outer container.
        /// </summary>
        private static readonly Style[] _innerContainerStyles;

        /// <summary>
        /// Stores the styles to be associated with the outer container.
        /// </summary>
        private static readonly Style[] _outerContainerStyles;

        /// <summary>
        /// Uri for the ResourceDictionary InnerStyleResourceDictionary.xaml.
        /// </summary>
        private static readonly Uri innerUri = new Uri("Microsoft.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/InnerStyleResourceDictionary.xaml", UriKind.Relative);

        /// <summary>
        /// Initializes static members that are used for reference equality 
        /// tests.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Complex initialization.")]
        static ImplicitStyleManagerTest()
        {
            Setter innerForegroundSetter = new Setter();
            innerForegroundSetter.Property = Control.ForegroundProperty;
            innerForegroundSetter.Value = new SolidColorBrush(Colors.Blue);

            Style innerButtonStyle = new Style { TargetType = typeof(Button) };
            innerButtonStyle.Setters.Add(innerForegroundSetter);

            _innerContainerStyles = new[] { innerButtonStyle };

            Setter outerButtonForegroundSetter = new Setter();
            outerButtonForegroundSetter.Property = Control.ForegroundProperty;
            outerButtonForegroundSetter.Value = new SolidColorBrush(Colors.Red);

            Style outerButtonStyle = new Style { TargetType = typeof(Button) };
            outerButtonStyle.Setters.Add(outerButtonForegroundSetter);

            Setter outerTextBlockForegroundSetter = new Setter();
            outerTextBlockForegroundSetter.Property = TextBlock.ForegroundProperty;
            outerTextBlockForegroundSetter.Value = new SolidColorBrush(Colors.Red);
            Style outerTextBlockStyle = new Style { TargetType = typeof(TextBlock) };
            outerTextBlockStyle.Setters.Add(outerTextBlockForegroundSetter);

            _outerContainerStyles = new[] { outerButtonStyle, outerTextBlockStyle };
        }

        // This set of tests verifies the following matrix: 
        // [ApplyMode=OneTime, ApplyMode=Auto, ApplyMode=None] 
        // x
        // [styles present in the element's resources, ResourceDictionaryUri property set, both styles sources are present (resources and ResourceDictionaryUri)]
        // It contains 4 x 3 = 12 combinations.
        #region Mode x Styles source

        /// <summary>
        /// Sets ApplyMode=OneTime, adds styles to the element's resources, and tests that:
        /// - Style is applied when it is first set.
        /// - Style is *not* applied to a control added later to the tree.
        /// - Calling Apply(...) causes the control added later to be styled appropriately.
        /// </summary>
        [TestMethod]
        [Description("Tests the scenario where ApplyMode=OneTime and styles are applied from an element's resources.")]
        [Asynchronous]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "One time is more commonly written as two words.")]
        public void TestOneTimeResources()
        {
            TestOneTime(
                (stackPanel) =>
                {
                    foreach (Style style in _outerContainerStyles)
                    {
                        stackPanel.Resources.Add(style.TargetType.FullName, style);
                    }
                    ImplicitStyleManager.SetApplyMode(stackPanel, ImplicitStylesApplyMode.OneTime);
                },
                Colors.Red);
        }

        /// <summary>
        /// Sets ApplyMode=OneTime, sets ResourceDictionaryUri to a valid Uri, and tests that:
        /// - Style is applied when it is first set.
        /// - Style is *not* applied to a control added later to the tree.
        /// - Calling Apply(...) causes the control added later to be styled appropriately.
        /// </summary>
        [TestMethod]
        [Description("Tests the scenario where ApplyMode=OneTime and styles are applied from a ResourceDictionaryUri.")]
        [Asynchronous]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "One time is more commonly written as two words.")]
        public void TestOneTimeResourceDictionaryUri()
        {
            TestOneTime(
                (stackPanel) =>
                {
                    ImplicitStyleManager.SetResourceDictionaryUri(stackPanel, innerUri);
                    ImplicitStyleManager.SetApplyMode(stackPanel, ImplicitStylesApplyMode.OneTime);
                },
                Colors.Blue);
        }

        /// <summary>
        /// Sets ApplyMode=OneTime, adds styles to the element's resources, 
        /// sets ResourceDictionaryUri to a valid Uri, and tests that the styles in the 
        /// ResourceDictionaryUri have priority.
        /// </summary>
        [TestMethod]
        [Description("Tests the scenario where ApplyMode=OneTime and styles are present both in an element's resources and in a ResourceDictionaryUri.")]
        [Asynchronous]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "One time is more commonly written as two words.")]
        public void TestOneTimeBothStyleSourcesSimultaneously()
        {
            TestOneTime(
                (stackPanel) =>
                {
                    foreach (Style style in _outerContainerStyles)
                    {
                        stackPanel.Resources.Add(style.TargetType.FullName, style);
                    }
                    ImplicitStyleManager.SetResourceDictionaryUri(stackPanel, innerUri);
                    ImplicitStyleManager.SetApplyMode(stackPanel, ImplicitStylesApplyMode.OneTime);
                },
                Colors.Blue);
        }

        /// <summary>
        /// Helper method that verifies the expected behavior for ApplyMode=OneTime.
        /// In this method's scenario, a StackPanel contains two Buttons: one added right away
        /// and another one added later.
        /// </summary>
        /// <param name="action">Action that sets the source of the styles (element's resources or ResourceDictionaryUri) and the ApplyMode.</param>
        /// <param name="color">Expected color for both Buttons after calling the Apply method.</param>
        private void TestOneTime(Action<Panel> action, Color color)
        {
            Panel stackPanel = new StackPanel();
            Button firstButton = new Button();
            Button secondButton = new Button();
            stackPanel.Children.Add(firstButton);

            action(stackPanel);

            bool hasLayoutUpdatedEventBeenRaised = false;
            stackPanel.LayoutUpdated += (source, args) => hasLayoutUpdatedEventBeenRaised = true;

            TestAsync(
                stackPanel,
                () => { EnqueueConditional(() => hasLayoutUpdatedEventBeenRaised); hasLayoutUpdatedEventBeenRaised = false; },
                () => Assert.IsNotNull(firstButton.Style, "firstButton should contain a Style (before the Apply method is called)."),
                () => stackPanel.Children.Add(secondButton),
                () => { EnqueueConditional(() => hasLayoutUpdatedEventBeenRaised); hasLayoutUpdatedEventBeenRaised = false; },
                () => Assert.IsNull(secondButton.Style, "secondButton should *not* contain a Style (before the Apply method is called)."),
                () => ImplicitStyleManager.Apply(stackPanel),
                () => Assert.IsNotNull(firstButton.Style, "firstButton should contain a Style (after the Apply method is called)."),
                () => Assert.IsNotNull(secondButton.Style, "secondButton should contain a Style (after the Apply method is called)."),
                () => Assert.AreEqual(color, ((SolidColorBrush)firstButton.Foreground).Color, String.Format(CultureInfo.CurrentCulture, "firstButton should have color {0}", color)),
                () => Assert.AreEqual(color, ((SolidColorBrush)secondButton.Foreground).Color, String.Format(CultureInfo.CurrentCulture, "secondButton should have color {0}", color)));
        }

        /// <summary>
        /// Sets ApplyMode=Auto, adds styles to the element's resources, and tests that:
        /// - Style is applied when it is first set.
        /// - Style is applied to a control added later to the tree.
        /// - Calling Apply(...) has no effect (because the Style was already applied).
        /// </summary>
        [TestMethod]
        [Description("Tests the scenario where ApplyMode=Auto and styles are applied from an element's resources.")]
        [Asynchronous]
        public void TestAutoResources()
        {
            TestAuto(
                (stackPanel) =>
                {
                    foreach (Style style in _outerContainerStyles)
                    {
                        stackPanel.Resources.Add(style.TargetType.FullName, style);
                    }
                    ImplicitStyleManager.SetApplyMode(stackPanel, ImplicitStylesApplyMode.Auto);
                },
                Colors.Red);
        }

        /// <summary>
        /// Sets ApplyMode=Auto, sets ResourceDictionaryUri to a valid Uri, and tests that:
        /// - Style is applied when it is first set.
        /// - Style is applied to a control added later to the tree.
        /// - Calling Apply(...) has no effect (because the Style was already applied).
        /// </summary>
        [TestMethod]
        [Description("Tests the scenario where ApplyMode=Auto and styles are applied from a ResourceDictionaryUri.")]
        [Asynchronous]
        public void TestAutoResourceDictionaryUri()
        {
            TestAuto(
                (stackPanel) =>
                {
                    ImplicitStyleManager.SetResourceDictionaryUri(stackPanel, innerUri);
                    ImplicitStyleManager.SetApplyMode(stackPanel, ImplicitStylesApplyMode.Auto);
                },
                Colors.Blue);
        }

        /// <summary>
        /// Sets ApplyMode=Auto, adds styles to the element's resources, 
        /// sets ResourceDictionaryUri to a valid Uri, and tests that the styles in the 
        /// ResourceDictionaryUri have priority.
        /// </summary>
        [TestMethod]
        [Description("Tests the scenario where ApplyMode=Auto and styles are present both in an element's resources and in a ResourceDictionaryUri.")]
        [Asynchronous]
        public void TestAutoBothStyleSourcesSimultaneously()
        {
            TestAuto(
                (stackPanel) =>
                {
                    foreach (Style style in _outerContainerStyles)
                    {
                        stackPanel.Resources.Add(style.TargetType.FullName, style);
                    }
                    ImplicitStyleManager.SetResourceDictionaryUri(stackPanel, innerUri);
                    ImplicitStyleManager.SetApplyMode(stackPanel, ImplicitStylesApplyMode.Auto);
                },
                Colors.Blue);
        }

        /// <summary>
        /// Helper method that verifies the expected behavior for ApplyMode=Auto.
        /// In this method's scenario, a StackPanel contains two Buttons: one added right away
        /// and another one added later.
        /// </summary>
        /// <param name="action">Action that sets the source of the styles (element's resources or ResourceDictionaryUri) and the ApplyMode.</param>
        /// <param name="color">Expected color for both Buttons after calling the Apply method.</param>
        private void TestAuto(Action<Panel> action, Color color)
        {
            Panel stackPanel = new StackPanel();
            Button firstButton = new Button();
            Button secondButton = new Button();
            stackPanel.Children.Add(firstButton);

            action(stackPanel);

            bool hasLayoutUpdatedEventBeenRaised = false;
            stackPanel.LayoutUpdated += (source, args) => hasLayoutUpdatedEventBeenRaised = true;
            TestAsync(
                stackPanel,
                () => { EnqueueConditional(() => hasLayoutUpdatedEventBeenRaised); hasLayoutUpdatedEventBeenRaised = false; },
                () => Assert.IsNotNull(firstButton.Style, "firstButton should contain a Style (before the Apply method is called)."),
                () => stackPanel.Children.Add(secondButton),
                () => { EnqueueConditional(() => hasLayoutUpdatedEventBeenRaised); hasLayoutUpdatedEventBeenRaised = false; },
                () => Assert.IsNotNull(secondButton.Style, "secondButton should contain a Style (before the Apply method is called)."),
                () => ImplicitStyleManager.Apply(stackPanel),
                () => Assert.IsNotNull(firstButton.Style, "firstButton should contain a Style (after the Apply method is called)."),
                () => Assert.IsNotNull(secondButton.Style, "secondButton should contain a Style (after the Apply method is called)."),
                () => Assert.AreEqual(color, ((SolidColorBrush)firstButton.Foreground).Color, String.Format(CultureInfo.CurrentCulture, "firstButton should have color {0}", color)),
                () => Assert.AreEqual(color, ((SolidColorBrush)secondButton.Foreground).Color, String.Format(CultureInfo.CurrentCulture, "secondButton should have color {0}", color)));
        }

        /// <summary>
        /// Sets ApplyMode=None, adds styles to the element's resources, and tests that:
        /// - Style is *not* applied when it is first set.
        /// - Style is *not* applied to a control added later to the tree.
        /// - Calling Apply(...) has no effect.
        /// </summary>
        [TestMethod]
        [Description("Tests the scenario where ApplyMode=None and styles are present in an element's resources.")]
        [Asynchronous]
        public void TestNoneResources()
        {
            TestNone(
                (stackPanel) =>
                {
                    foreach (Style style in _outerContainerStyles)
                    {
                        stackPanel.Resources.Add(style.TargetType.FullName, style);
                    }
                    ImplicitStyleManager.SetApplyMode(stackPanel, ImplicitStylesApplyMode.None);
                },
                Colors.Red);
        }

        /// <summary>
        /// Sets ApplyMode=None, sets ResourceDictionaryUri to a valid Uri, and tests that:
        /// - Style is *not* applied when it is first set.
        /// - Style is *not* applied to a control added later to the tree.
        /// - Calling Apply(...) has no effect.
        /// </summary>
        [TestMethod]
        [Description("Tests the scenario where ApplyMode=None and styles are present in a ResourceDictionaryUri.")]
        [Asynchronous]
        public void TestNoneResourceDictionaryUri()
        {
            TestNone(
                (stackPanel) =>
                {
                    ImplicitStyleManager.SetResourceDictionaryUri(stackPanel, innerUri);
                    ImplicitStyleManager.SetApplyMode(stackPanel, ImplicitStylesApplyMode.None);
                },
                Colors.Blue);
        }

        /// <summary>
        /// Sets ApplyMode=None, adds styles to the element's resources, 
        /// sets ResourceDictionaryUri to a valid Uri, and tests that style are not
        /// applied.
        /// </summary>
        [TestMethod]
        [Description("Tests the scenario where ApplyMode=None and styles are present both in an element's resources and in a ResourceDictionaryUri.")]
        [Asynchronous]
        public void TestNoneBothStyleSourcesSimultaneously()
        {
            TestNone(
                (stackPanel) =>
                {
                    foreach (Style style in _outerContainerStyles)
                    {
                        stackPanel.Resources.Add(style.TargetType.FullName, style);
                    }
                    ImplicitStyleManager.SetResourceDictionaryUri(stackPanel, innerUri);
                    ImplicitStyleManager.SetApplyMode(stackPanel, ImplicitStylesApplyMode.None);
                },
                Colors.Blue);
        }

        /// <summary>
        /// Helper method that verifies the expected behavior for ApplyMode=None.
        /// In this method's scenario, a StackPanel contains two Buttons: one added right away
        /// and another one added later.
        /// </summary>
        /// <param name="action">Action that sets the source of the styles (element's resources or ResourceDictionaryUri) and the ApplyMode.</param>
        /// <param name="color">The color to use for the styles.</param>
        private void TestNone(Action<Panel> action, Color color)
        {
            Panel stackPanel = new StackPanel();
            Button firstButton = new Button();
            Button secondButton = new Button();
            stackPanel.Children.Add(firstButton);

            action(stackPanel);

            TestAsync(
                stackPanel,
                () => Assert.IsNull(firstButton.Style, "firstButton should *not* contain a Style (before the Apply method is called)."),
                () => stackPanel.Children.Add(secondButton),
                () => Assert.IsNull(secondButton.Style, "secondButton should *not* contain a Style (before the Apply method is called)."),
                () => ImplicitStyleManager.Apply(stackPanel),
                () => Assert.IsNotNull(firstButton.Style, "firstButton should contain a Style (after the Apply method is called)."),
                () => Assert.IsNotNull(secondButton.Style, "secondButton should contain a Style (after the Apply method is called)."),
                () => Assert.AreEqual(color, ((SolidColorBrush)firstButton.Foreground).Color, String.Format(CultureInfo.CurrentCulture, "firstButton should have color {0}", color)),
                () => Assert.AreEqual(color, ((SolidColorBrush)secondButton.Foreground).Color, String.Format(CultureInfo.CurrentCulture, "secondButton should have color {0}", color)));
        }

        #endregion

        // This set of tests covers setting and getting DPs, including invalid scenarios that cause exceptions.
        #region Setting and Getting DPs

        /// <summary>
        /// Test setting and getting ApplyMode.
        /// </summary>
        [TestMethod]
        [Description("Test setting and getting ApplyMode")]
        public void TestApplyModeSetAndGet()
        {
            Panel stackPanel = new StackPanel();
            ImplicitStyleManager.SetApplyMode(stackPanel, ImplicitStylesApplyMode.Auto);
            Assert.AreEqual(ImplicitStyleManager.GetApplyMode(stackPanel), ImplicitStylesApplyMode.Auto);
        }

        /// <summary>
        /// Test getting ApplyMode from a null element throws an 
        /// exception.
        /// </summary>
        [TestMethod]
        [Description("Test getting ApplyMode from a null element throws an exception.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestApplyModeGetOnNullElementThrowsException()
        {
            ImplicitStyleManager.GetApplyMode((FrameworkElement)null);
        }

        /// <summary>
        /// Test setting ApplyMode on a null element throws an exception.
        /// </summary>
        [TestMethod]
        [Description("Test setting ApplyMode on a null element throws an exception.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestApplyModeSetOnNullElementThrowsException()
        {
            ImplicitStyleManager.SetApplyMode((FrameworkElement)null, ImplicitStylesApplyMode.Auto);
        }

        /// <summary>
        /// Test setting and getting ResourceDictionaryUri.
        /// </summary>
        [TestMethod]
        [Description("Test setting and getting ResourceDictionaryUri.")]
        public void TestResourceDictionaryUriSetAndGet()
        {
            Panel stackPanel = new StackPanel();
            Uri uri = new Uri("Microsoft.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/OuterStyleResourceDictionary.xaml", UriKind.Relative);

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
            Uri uri = new Uri("Microsoft.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/non-existantresource.xaml", UriKind.Relative);

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
            Uri uri = new Uri("Microsoft.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/OuterStyleResourceDictionary.xaml", UriKind.Relative);
            ImplicitStyleManager.SetResourceDictionaryUri((FrameworkElement)null, uri);
        }

        /// <summary>
        /// Test setting and getting ApplicationResourceDictionaryUri.
        /// </summary>
        [TestMethod]
        [Description("Test setting and getting ApplicationResourceDictionaryUri.")]
        public void TestSetGetApplicationResourceDictionaryUri()
        {
            Uri uri = new Uri("Microsoft.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/OuterStyleResourceDictionary.xaml", UriKind.Relative);
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
            Uri uri = new Uri("Microsoft.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/non-existantresource.xaml", UriKind.Relative);
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

        // This set of tests two-level nested container scenarios. 
        // These scenarios cover ApplyMode=[Auto, OneTime] x [styles in the resources, ResourceDictionaryUri]
        #region Nested containers
        /// <summary>
        /// Verify that nested containers with implicit styles apply styles 
        /// properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that nested containers with implicit styles apply styles properly.")]
        public void TestNestedContainersAutoResources()
        {
            TestStylesAreAppliedProperlyToNestedContainersWhenNotSetToApplyOnce<StackPanel>(
                (outerPanel) =>
                {
                    foreach (Style style in _outerContainerStyles)
                    {
                        outerPanel.Resources.Add(style.TargetType.FullName, style);
                    }
                },
                (innerPanel) =>
                {
                    foreach (Style style in _innerContainerStyles)
                    {
                        innerPanel.Resources.Add(style.TargetType.FullName, style);
                    }
                });
        }

        /// <summary>
        /// Verify that nested containers with implicit styles apply styles 
        /// properly when styles are loaded from a ResourceDictionary file and 
        /// associated via attached properties.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that nested containers with implicit styles apply styles properly when styles are loaded from a ResourceDictionary file and associated via attached properties.")]
        public void TestNestedContainersAutoResourceDictionaryUri()
        {
            TestStylesAreAppliedProperlyToNestedContainersWhenNotSetToApplyOnce<StackPanel>(
                (outerPanel) =>
                {
                    Uri outerResourceDictionaryLocation = new Uri("Microsoft.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/OuterStyleResourceDictionary.xaml", UriKind.Relative);
                    ImplicitStyleManager.SetResourceDictionaryUri(outerPanel, outerResourceDictionaryLocation);
                },
                (innerPanel) =>
                {
                    Uri innerResourceDictionaryLocation = innerUri;
                    ImplicitStyleManager.SetResourceDictionaryUri(innerPanel, innerResourceDictionaryLocation);
                });
        }

        /// <summary>
        /// Verify that nested containers with implicit styles apply styles 
        /// properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that nested containers with implicit styles apply styles properly.")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "One time is more commonly written as two words.")]
        public void TestNestedContainersOneTimeResources()
        {
            TestStylesAreAppliedToNestedContainersOnceProperly<StackPanel>(
                (outerPanel) =>
                {
                    foreach (Style style in _outerContainerStyles)
                    {
                        outerPanel.Resources.Add(style.TargetType.FullName, style);
                    }
                    ImplicitStyleManager.SetApplyMode(outerPanel, ImplicitStylesApplyMode.OneTime);
                },
                (innerPanel) =>
                {
                    foreach (Style style in _innerContainerStyles)
                    {
                        innerPanel.Resources.Add(style.TargetType.FullName, style);
                    }
                    ImplicitStyleManager.SetApplyMode(innerPanel, ImplicitStylesApplyMode.OneTime);
                });
        }

        /// <summary>
        /// Verify that nested containers with implicit styles apply styles 
        /// properly when styles are loaded from a ResourceDictionary file and 
        /// associated via attached properties.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that nested containers with implicit styles apply styles properly when styles are loaded from a ResourceDictionary file and associated via attached properties.")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "One time is more commonly written as two words.")]
        public void TestNestedContainersOneTimeResourceDictionaryUri()
        {
            TestStylesAreAppliedToNestedContainersOnceProperly<StackPanel>(
                (outerPanel) =>
                {
                    Uri outerResourceDictionaryLocation = new Uri("Microsoft.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/OuterStyleResourceDictionary.xaml", UriKind.Relative);
                    ImplicitStyleManager.SetApplyMode(outerPanel, ImplicitStylesApplyMode.OneTime);
                    ImplicitStyleManager.SetResourceDictionaryUri(outerPanel, outerResourceDictionaryLocation);
                },
                (innerPanel) =>
                {
                    Uri innerResourceDictionaryLocation = innerUri;
                    ImplicitStyleManager.SetApplyMode(innerPanel, ImplicitStylesApplyMode.OneTime);
                    ImplicitStyleManager.SetResourceDictionaryUri(innerPanel, innerResourceDictionaryLocation);
                });
        }

        /// <summary>
        /// Tests that styles are applied once correctly when multiple panels
        /// are nested.  The outer container contains styles that apply to a 
        /// button and a text block.  It contains a button, a text block, and
        /// another container.  The inner container contains styles that apply
        /// to a button.  It also contains a button and text block.  The test
        /// verifies that the button and text blocks in the outer container are
        /// set to use the styles defined by the outer container.  It also
        /// confirms that the style of the button in the inner container is set
        /// to the style specified by the inner container but that the text
        /// block in the inner container is set to the style of the outer 
        /// container because the inner container does not override that style
        /// with a style of its own.
        /// </summary>
        /// <typeparam name="T">The type of panel to use.</typeparam>
        /// <param name="preVisualTreeOuterContainerAction">An action that
        /// associates a text block and button style with the outer container.
        /// </param>
        /// <param name="preVisualTreeInnerContainerAction">An action that 
        /// associates a text block style with the inner container.</param>
        public void TestStylesAreAppliedToNestedContainersOnceProperly<T>(
            Action<T> preVisualTreeOuterContainerAction,
            Action<T> preVisualTreeInnerContainerAction)
            where T : Panel, new()
        {
            T outerPanel = new T();
            outerPanel.Name = "outerPanel";
            Button outerButton = new Button { Content = "This is an outer button" };
            TextBlock outerTextBlock = new TextBlock { Text = "This is an outer text block" };
            T innerPanel = new T();
            innerPanel.Name = "innerPanel";
            Button innerButton = new Button { Content = "This is a inner button" };
            TextBlock innerTextBlock = new TextBlock { Text = "This is an inner TextBlock" };

            Button unstyledOuterButton = new Button { Content = "Unstyled outer button." };
            Button unstyledInnerButton = new Button { Content = "Unstyled inner button." };

            preVisualTreeOuterContainerAction(outerPanel);
            outerPanel.Children.Add(outerButton);
            outerPanel.Children.Add(outerTextBlock);
            outerPanel.Children.Add(innerPanel);

            preVisualTreeInnerContainerAction(innerPanel);
            innerPanel.Children.Add(innerButton);
            innerPanel.Children.Add(innerTextBlock);

            TestAsync(
                outerPanel,
                () =>
                {
                    Assert.IsNotNull(outerButton.Style);
                    Assert.IsNotNull(outerTextBlock.Style);
                    Assert.IsNotNull(innerButton.Style);
                    Assert.IsNotNull(innerTextBlock.Style);
                    Assert.AreNotSame(outerButton.Style, innerButton.Style);
                    Assert.AreSame(outerTextBlock.Style, innerTextBlock.Style);
                },
                () => outerPanel.Children.Add(unstyledOuterButton),
                () => Assert.IsNull(unstyledOuterButton.Style),
                () => innerPanel.Children.Add(unstyledInnerButton),
                () => Assert.IsNull(unstyledInnerButton.Style));
        }

        /// <summary>
        /// Tests that styles are applied correctly when multiple containers
        /// are nested.  The outer container contains styles that apply to a 
        /// button and a text block.  It contains a button, a text block, and
        /// another container.  The inner container contains styles that apply
        /// to a button.  It also contains a button and text block.  The test
        /// verifies that the button and text blocks in the outer container are
        /// set to use the styles defined by the outer container.  It also
        /// confirms that the style of the button in the inner container is set
        /// to the style specified by the inner container but that the text
        /// block in the inner container is set to the style of the outer 
        /// container because the inner container does not override that style
        /// with a style of its own.
        /// </summary>
        /// <typeparam name="T">The type of panel to use.</typeparam>
        /// <param name="styleOuterContainerAction">An action that
        /// associates a text block and button style with the outer container.
        /// </param>
        /// <param name="styleInnerContainerAction">An action that 
        /// associates a text block style with the inner container.</param>
        public void TestStylesAreAppliedProperlyToNestedContainersWhenNotSetToApplyOnce<T>(Action<T> styleOuterContainerAction, Action<T> styleInnerContainerAction)
            where T : Panel, new()
        {
            T outerStackPanel = new T();
            Button outerButton = new Button { Content = "This is an outer button" };
            TextBlock outerTextBlock = new TextBlock { Text = "This is an outer text block" };
            T innerStackPanel = new T();
            Button innerButton = new Button { Content = "This is a inner button" };
            TextBlock innerTextBlock = new TextBlock { Text = "This is an inner TextBlock" };

            ImplicitStyleManager.SetApplyMode(outerStackPanel, ImplicitStylesApplyMode.Auto);
            styleOuterContainerAction(outerStackPanel);

            ImplicitStyleManager.SetApplyMode(innerStackPanel, ImplicitStylesApplyMode.Auto);
            styleInnerContainerAction(innerStackPanel);

            TestAsync(
                outerStackPanel,
                () => outerStackPanel.Children.Add(outerButton),
                () => outerStackPanel.Children.Add(outerTextBlock),
                () => outerStackPanel.Children.Add(innerStackPanel),
                () => innerStackPanel.Children.Add(innerButton),
                () => innerStackPanel.Children.Add(innerTextBlock),
                () =>
                {
                    Assert.IsNotNull(outerButton.Style);
                    Assert.IsNotNull(outerTextBlock.Style);
                    Assert.IsNotNull(innerButton.Style);
                    Assert.IsNotNull(innerTextBlock.Style);
                    Assert.AreNotSame(outerButton.Style, innerButton.Style);
                    Assert.AreSame(outerTextBlock.Style, innerTextBlock.Style);
                });
        }
        #endregion

        /// <summary>
        /// Test Apply with null Framework Element parameter.
        /// </summary>
        [TestMethod]
        [Description("Test Apply with null Framework Element parameter.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestApplyWithNullFrameworkElementParameter()
        {
            ImplicitStyleManager.Apply(null as FrameworkElement);
        }

        /// <summary>
        /// Test that styles are applied correctly after behavior is detached 
        /// from a container sandwiched between two other containers.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test that styles are applied correctly after behavior is detached from a container sandwiched between two other containers.")]
        public void TestDetachFromMiddleContainer()
        {
            Panel outerStackPanel = new StackPanel();
            outerStackPanel.Name = "outerStackPanel";
            foreach (Style style in _outerContainerStyles)
            {
                outerStackPanel.Resources.Add(style.TargetType.FullName, style);
            }
            ImplicitStyleManager.SetApplyMode(outerStackPanel, ImplicitStylesApplyMode.Auto);
            Button outerButton = new Button { Content = "This is an outer button" };

            Panel innerStackPanel = new StackPanel();
            innerStackPanel.Name = "innerStackPanel";
            foreach (Style style in _innerContainerStyles)
            {
                innerStackPanel.Resources.Add(style.TargetType.FullName, style);
            }
            ImplicitStyleManager.SetApplyMode(innerStackPanel, ImplicitStylesApplyMode.Auto);
            Button innerButton = new Button { Content = "This is a inner button" };

            Panel innerMostStackPanel = new StackPanel();

            ImplicitStyleManager.SetApplyMode(innerMostStackPanel, ImplicitStylesApplyMode.Auto);
            Button innerStyledInnerMostButton = new Button { Content = "This is a button in the innermost stackPanel styled by the innerStackPanel." };
            Button anotherInnerStyledInnerMostButton = new Button { Content = "This is a button in the innermost stackPanel that should be styled by the innerStackPanel because even though the innerStackPanel has detached from the behavior its resource dictionary should still be used." };

            bool hasLayoutUpdatedEventFired = false;
            innerStackPanel.LayoutUpdated += (source, args) => hasLayoutUpdatedEventFired = true;

            TestAsync(
                outerStackPanel,
                () => outerStackPanel.Children.Add(outerButton),
                () => outerStackPanel.Children.Add(innerStackPanel),
                () => innerStackPanel.Children.Add(innerButton),
                () => innerStackPanel.Children.Add(innerMostStackPanel),
                () => innerMostStackPanel.Children.Add(innerStyledInnerMostButton),
                () => { EnqueueConditional(() => hasLayoutUpdatedEventFired); hasLayoutUpdatedEventFired = false; },
                () => Assert.AreSame(innerButton.Style, innerStyledInnerMostButton.Style),
                () => ImplicitStyleManager.SetApplyMode(innerStackPanel, ImplicitStylesApplyMode.None),
                () => innerMostStackPanel.Children.Add(anotherInnerStyledInnerMostButton),
                () => Assert.AreSame(innerButton.Style, anotherInnerStyledInnerMostButton.Style));
        }

        /// <summary>
        /// Set ApplyMode to OneTime on a container, add an object to the 
        /// container and ensure it is styled.  Then add another object and 
        /// ensure it is not styled.  Then set ApplyMode to Auto and add another 
        /// object to ensure it is styled.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "One time is more commonly written as two words.")]
        [TestMethod]
        [Description("Set ApplyMode to OneTime on a container, add an object to the container and ensure it is styled.  Then add another object and ensure it is not styled.  Then set ApplyMode to Auto and add another object to ensure it is styled.")]
        [Asynchronous]
        public void TestTogglingOneTimeToAuto()
        {
            Setter outerButtonForegroundSetter = new Setter();
            outerButtonForegroundSetter.Property = Control.ForegroundProperty;
            outerButtonForegroundSetter.Value = new SolidColorBrush(Colors.Red);

            Style outerButtonStyle = new Style { TargetType = typeof(Button) };
            outerButtonStyle.Setters.Add(outerButtonForegroundSetter);

            Panel outerPanel = new StackPanel();
            outerPanel.Name = "outerPanel";
            outerPanel.Resources.Add(outerButtonStyle.TargetType.FullName, outerButtonStyle);

            Button styledButton = new Button { Content = "Styled button." };
            Button unstyledButton = new Button { Content = "Unstyled button." };
            Button newStyledButton = new Button { Content = "Unstyled button." };
            bool hasLayoutUpdatedEventFired = false;
            outerPanel.LayoutUpdated += (source, args) => hasLayoutUpdatedEventFired = true;

            TestAsync(
                outerPanel,
                () => ImplicitStyleManager.SetApplyMode(outerPanel, ImplicitStylesApplyMode.OneTime),
                () => outerPanel.Children.Add(styledButton),
                () => { EnqueueConditional(() => hasLayoutUpdatedEventFired); hasLayoutUpdatedEventFired = false; },
                () => Assert.IsNotNull(styledButton.Style, "Styled button has been set correctly."),
                () => outerPanel.Children.Add(unstyledButton),
                () => Assert.IsNull(unstyledButton.Style, "Newly added button has not been styled."),
                () => ImplicitStyleManager.SetApplyMode(outerPanel, ImplicitStylesApplyMode.Auto),
                () => { EnqueueConditional(() => hasLayoutUpdatedEventFired); hasLayoutUpdatedEventFired = false; },
                () => outerPanel.Children.Add(newStyledButton),
                () => { EnqueueConditional(() => hasLayoutUpdatedEventFired); hasLayoutUpdatedEventFired = false; },
                () => Assert.IsNotNull(unstyledButton.Style, "Previously added unstlyed button is now styled after mode has been toggled to Auto."),
                () => Assert.IsNotNull(newStyledButton.Style, "Newly added button is now styled because mode is Auto."));
        }

        /// <summary>
        /// Tests that when x:Key is set on a Style in the resources, it is ignored by ImplicitStyleManager.
        /// x:Key should take priority over TargetType.
        /// </summary>
        [TestMethod]
        [Description("Tests that when x:Key is set on a Style in the resources, it is ignored by ImplicitStyleManager.")]
        [Asynchronous]
        public void TestKeyHasPriorityOverTargetType()
        {
            KeyAndTargetTypeAreBothSet userControl = new KeyAndTargetTypeAreBothSet();

            TestAsync(
                userControl,
                () => Assert.IsNotNull(userControl.nud1.Style),
                () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)userControl.nud1.Foreground).Color),
                () => Assert.IsNull(userControl.nud2.Style),
                () => Assert.AreEqual(Colors.Black, ((SolidColorBrush)userControl.nud2.Foreground).Color));
        }

        /// <summary>
        /// This method tests setting the ResourceDictionaryUri property, the ApplyMode property and calling the Apply method all in different elements.
        /// </summary>
        [TestMethod]
        [Description("This method tests setting the ResourceDictionaryUri property, the ApplyMode property and calling the Apply method all in different elements.")]
        [Asynchronous]
        public void TestDifferentElements()
        {
            Button parentButton = new Button();
            StackPanel parentStackPanel = new StackPanel();
            StackPanel childStackPanel = new StackPanel();
            parentStackPanel.Children.Add(childStackPanel);
            childStackPanel.Children.Add(parentButton);

            ImplicitStyleManager.SetResourceDictionaryUri(parentButton, innerUri);
            ImplicitStyleManager.SetApplyMode(childStackPanel, ImplicitStylesApplyMode.OneTime);

            Button childButton = new Button();

            bool hasLayoutUpdatedEventFired = false;
            parentStackPanel.LayoutUpdated += (source, args) => hasLayoutUpdatedEventFired = true;

            TestAsync(
                parentStackPanel,
                () => { EnqueueConditional(() => hasLayoutUpdatedEventFired); hasLayoutUpdatedEventFired = false; },
                () => Assert.AreEqual(Colors.Blue, ((SolidColorBrush)parentButton.Foreground).Color),
                () => parentButton.Content = childButton,
                () => { EnqueueConditional(() => hasLayoutUpdatedEventFired); hasLayoutUpdatedEventFired = false; },
                () => Assert.AreEqual(Colors.Black, ((SolidColorBrush)childButton.Foreground).Color),
                () => ImplicitStyleManager.Apply(parentStackPanel),
                () => Assert.AreEqual(Colors.Blue, ((SolidColorBrush)childButton.Foreground).Color));
        }

        /// <summary>
        /// Test that ensures that controls are styled using their DefaultStyleKey.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test that ensures that controls are styled using their DefaultStyleKey.")]
        public virtual void TestDefaultStyleKeyIsAppliedToDerivedClasses()
        {
            DerivedButton derivedButton = new DerivedButton();
            Button regularButton = new Button();
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(derivedButton);
            stackPanel.Children.Add(regularButton);

            ImplicitStyleManager.SetApplyMode(stackPanel, ImplicitStylesApplyMode.OneTime);
            ImplicitStyleManager.SetResourceDictionaryUri(stackPanel, innerUri);
            bool hasLayoutUpdatedEventFired = false;
            stackPanel.LayoutUpdated += (source, args) => hasLayoutUpdatedEventFired = true;

            TestAsync(
                stackPanel,
                () => { EnqueueConditional(() => hasLayoutUpdatedEventFired); hasLayoutUpdatedEventFired = false; },
                () => Assert.IsNotNull(regularButton),
                () => Assert.AreEqual(regularButton.Style, derivedButton.Style));
       }
   }
}