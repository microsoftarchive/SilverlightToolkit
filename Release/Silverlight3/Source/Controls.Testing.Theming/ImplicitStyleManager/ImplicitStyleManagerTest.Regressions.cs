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
        // This set of tests covers adding styles to the local resources of the element where
        // ApplyMode is applied, with the 4 possible different values for ApplyMode.
        #region Style in local resources
        /// <summary>
        /// Tests adding a style to the local resources of the element with ApplyMode=OneTime.
        /// </summary>
        [TestMethod]
        [Description("Tests adding a style to the local resources of the element with ApplyMode=OneTime.")]
        [Asynchronous]
        [Bug("516241 - ImplicitStyleManager - It is not possible to get an implicit style from the current element", Fixed = true)]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "One time is more commonly written as two words.")]
        public void TestApplyStyleInLocalResourcesOneTime()
        {
            BasicScenario userControl = new BasicScenario();
            Button childButton = new Button();

            ImplicitStyleManager.SetApplyMode(userControl.panel, ImplicitStylesApplyMode.OneTime);

            TestAsync(
               userControl,
               () => Assert.IsInstanceOfType(userControl.btn.Foreground, typeof(SolidColorBrush)),
               () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)userControl.btn.Foreground).Color),
               () => Assert.IsNotNull(userControl.btn.Style),
               () => userControl.btn.Content = childButton,
               () => Assert.AreEqual(Colors.Black, ((SolidColorBrush)childButton.Foreground).Color),
               () => Assert.IsNull(childButton.Style));
        }

        /// <summary>
        /// Tests adding a style to the local resources of the element with ApplyMode=Auto.
        /// </summary>
        [TestMethod]
        [Description("Tests adding a style to the local resources of the element with ApplyMode=Auto.")]
        [Asynchronous]
        public void TestApplyStyleInLocalResourcesAuto()
        {
            BasicScenario userControl = new BasicScenario();
            Button childButton = new Button();

            ImplicitStyleManager.SetApplyMode(userControl.panel, ImplicitStylesApplyMode.Auto);

            TestAsync(
               userControl,
               () => Assert.IsInstanceOfType(userControl.btn.Foreground, typeof(SolidColorBrush)),
               () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)userControl.btn.Foreground).Color),
               () => Assert.IsNotNull(userControl.btn.Style),
               () => userControl.btn.Content = childButton,
               () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)childButton.Foreground).Color),
               () => Assert.IsNotNull(childButton.Style));
        }

        /// <summary>
        /// Tests adding a style to the local resources of the element with ApplyMode=None.
        /// </summary>
        [TestMethod]
        [Description("Tests adding a style to the local resources of the element with ApplyMode=None.")]
        [Asynchronous]
        public void TestApplyStyleInLocalResourcesNone()
        {
            BasicScenario userControl = new BasicScenario();
            ImplicitStyleManager.SetApplyMode(userControl.panel, ImplicitStylesApplyMode.None);

            TestAsync(
               userControl,
               () => Assert.IsInstanceOfType(userControl.btn.Foreground, typeof(SolidColorBrush)),
               () => Assert.AreEqual(Colors.Black, ((SolidColorBrush)userControl.btn.Foreground).Color),
               () => Assert.IsNull(userControl.btn.Style));
        }
        #endregion

        // This set of tests provides coverage for setting styles at the Application level, both
        // by using a ResourceDictionaryUri and by adding styles directly to the Application's resources.
        #region Styles in Application
        /// <summary>
        /// Tests a scenario where the Application's ResourceDictionaryUri is set, the UserControl
        /// contains a style in its resources and Mode is set to Auto. In this case, the element
        /// should pick up the styles from the UserControl's resources.
        /// </summary>
        [TestMethod]
        [Description("Tests a scenario where the Application's ResourceDictionaryUri is set and the UserControl contains a style in its resources.")]
        [Asynchronous]
        [Bug("528068 - ImplicitStyleManager - ApplicationResourceDictionaryUri doesn't work as expected", Fixed = true)]
        public void TestApplicationResourceDictionaryUriWithResourcesBelow()
        {
            ResourcesAboveMode userControl = new ResourcesAboveMode();

            SetApplicationResourceDictionaryUri(new Uri("System.Windows.Controls.Testing.Theming;component/ImplicitStyleManager/InnerStyleResourceDictionary.xaml", UriKind.Relative));

            TestAsync(
                userControl,
                () => Assert.IsInstanceOfType(userControl.btn.Foreground, typeof(SolidColorBrush)),
                () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)userControl.btn.Foreground).Color),
                () => SetApplicationResourceDictionaryUri(null));
        }

        /// <summary>
        /// Tests a scenario where both the Application and the UserControl
        /// contain a style in their resources and Mode is set to Auto. In this case, the element
        /// should pick up the styles from the UserControl's resources.
        /// </summary>
        [TestMethod]
        [Description("Tests a scenario where both the Application and the UserControl contain a style in their resources.")]
        [Asynchronous]
        [Bug("528068 - ImplicitStyleManager - ApplicationResourceDictionaryUri doesn't work as expected", Fixed = true)]
        public void TestApplicationResourcesWithResourcesBelow()
        {
            ResourcesAboveMode userControl = new ResourcesAboveMode();

            AddApplicationResources(Colors.Green);

            TestAsync(
                userControl,
                () => Assert.IsInstanceOfType(userControl.btn.Foreground, typeof(SolidColorBrush)),
                () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)userControl.btn.Foreground).Color),
                () => Application.Current.Resources.Clear());
        }

        /// <summary>
        /// Tests that styles in the Application's ResourceDictionaryUri are applied correctly.
        /// </summary>
        [TestMethod]
        [Description("Tests that styles in the Application's ResourceDictionaryUri are applied correctly.")]
        [Bug("528162 - ImplicitStyleManager - ResourceDictionaryUri is not working at the application level", Fixed = true)]
        [Asynchronous]
        public void TestApplicationResourceDictionaryUri()
        {
            SetApplicationResourceDictionaryUri(innerUri);

            Button btn = new Button { Content = "Hello" };
            ImplicitStyleManager.SetApplyMode(btn, ImplicitStylesApplyMode.Auto);

            TestAsync(
                btn,
                () => Assert.IsInstanceOfType(btn.Foreground, typeof(SolidColorBrush)),
                () => Assert.AreEqual(Colors.Blue, ((SolidColorBrush)btn.Foreground).Color),
                () => SetApplicationResourceDictionaryUri(null));
        }

        /// <summary>
        /// Tests that styles in the Application's resources are applied correctly.
        /// </summary>
        [TestMethod]
        [Description("Tests that styles in the Application's resources are applied correctly.")]
        [Asynchronous]
        public void TestApplicationResources()
        {
            AddApplicationResources(Colors.Green);

            Button btn = new Button { Content = "Hello" };
            ImplicitStyleManager.SetApplyMode(btn, ImplicitStylesApplyMode.Auto);

            TestAsync(
                btn,
                () => Assert.IsInstanceOfType(btn.Foreground, typeof(SolidColorBrush)),
                () => Assert.AreEqual(Colors.Green, ((SolidColorBrush)btn.Foreground).Color),
                () => Application.Current.Resources.Clear());
        }

        /// <summary>
        /// Helper method that adds a Style to the Application's resources.
        /// This style sets a Button's Foreground to the color passed as a parameter.
        /// </summary>
        /// <param name="color">Color used to set the Button's Foreground color to.</param>
        private static void AddApplicationResources(Color color)
        {
            Setter foregroundSetter = new Setter();
            foregroundSetter.Property = Control.ForegroundProperty;
            foregroundSetter.Value = new SolidColorBrush(color);

            Style foregroundButtonStyle = new Style { TargetType = typeof(Button) };
            foregroundButtonStyle.Setters.Add(foregroundSetter);

            Application.Current.Resources.Add(typeof(Button).FullName, foregroundButtonStyle);
        }
        #endregion

        /// <summary>
        /// This tests referencing a Style from within a template - the fully qualified name of the type in "TargetType" should be
        /// used as the key in this case. 
        /// This test also verified that having the ApplyMode property and the styles in the resources set in different elements.
        /// </summary>
        [TestMethod]
        [Description("This tests referencing a Style from within a template - the fully qualified name of the type in 'TargetType' should be used as the key in this case. ")]
        [Asynchronous]
        [Bug("527514 - ImplicitStyleManager - Incorrect behavior when ApplyMode and styles in resources are set in different elements", Fixed = true)]
        public void TestStyleWithinTemplate()
        {
            WithinTemplate userControl = new WithinTemplate();

            TestAsync(
                userControl,
                () => Assert.IsNotNull(userControl.contentControl.Style),
                () => Assert.IsInstanceOfType(VisualTreeHelper.GetChild(userControl.contentControl, 0), typeof(Button)),
                () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)((Button)VisualTreeHelper.GetChild(userControl.contentControl, 0)).Foreground).Color),
                () => Assert.IsNotNull(((Button)VisualTreeHelper.GetChild(userControl.contentControl, 0)).Style));
        }

        /// <summary>
        /// Tests that if a Button's Mode is OneTime, and somewhere up in the tree the Mode is set to Auto, the styles
        /// in the Panel don't get applied to the Button's new children. When calling Apply on the Button, the styles are then
        /// applied.
        /// </summary>
        [TestMethod]
        [Description("Tests a scenario where an element with OneTime mode is within the child tree of an element with mode set to Auto.")]
        [Asynchronous]
        [Bug("526896 - ImplicitStyleManager - Nested Auto and OneTime modes don't work as expected", Fixed = true)]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "One time is more commonly written as two words.")]
        public void TestAutoOneTimeNestedModes()
        {
            SubtreeDifferentMode userControl = new SubtreeDifferentMode();
            Button thirdButton = new Button { Content = "Third button" };
            TestAsync(
                userControl,
                () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)userControl.firstButton.Background).Color),
                () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)userControl.secondButton.Background).Color),
                () => userControl.secondButton.Content = thirdButton,
                () => Assert.IsNull(thirdButton.Style),
                () => ImplicitStyleManager.Apply(userControl.secondButton),
                () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)userControl.firstButton.Background).Color),
                () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)userControl.secondButton.Background).Color),
                () => Assert.AreEqual(Colors.Red, ((SolidColorBrush)thirdButton.Background).Color),
                () => Assert.IsNotNull(thirdButton.Style));
        }
    }
}