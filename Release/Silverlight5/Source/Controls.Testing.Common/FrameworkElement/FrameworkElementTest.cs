// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using Microsoft.Silverlight.Testing;
using Microsoft.Silverlight.Testing.Harness;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Base class used to author unit tests for FrameworkElements.
    /// </summary>
    public abstract partial class FrameworkElementTest : TestBase, IProvideDynamicTestMethods
    {
        #region FrameworkElements to test
        /// <summary>
        /// Gets a default instance of FrameworkElement (or a derived type) to
        /// test.
        /// </summary>
        public abstract FrameworkElement DefaultFrameworkElementToTest { get; }

        /// <summary>
        /// Gets instances of FrameworkElement (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<FrameworkElement> FrameworkElementsToTest { get; }

        /// <summary>
        /// Gets instances of IOverriddenFrameworkElement (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<IOverriddenFrameworkElement> OverriddenFrameworkElementsToTest { get; }
        #endregion FrameworkElements to test

        /// <summary>
        /// Gets the HorizontalAlignment dependency property test.
        /// </summary>
        protected DependencyPropertyTest<FrameworkElement, HorizontalAlignment> HorizontalAlignmentProperty { get; private set; }

        /// <summary>
        /// Gets the VerticalAlignment dependency property test.
        /// </summary>
        protected DependencyPropertyTest<FrameworkElement, VerticalAlignment> VerticalAlignmentProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the FrameworkElementTest class.
        /// </summary>
        protected FrameworkElementTest()
            : base()
        {
            Func<FrameworkElement> initializer = () => DefaultFrameworkElementToTest;
            HorizontalAlignmentProperty = new DependencyPropertyTest<FrameworkElement, HorizontalAlignment>(this, "HorizontalAlignment")
                {
                    Property = FrameworkElement.HorizontalAlignmentProperty,
                    Initializer = initializer,
                    DefaultValue = HorizontalAlignment.Stretch,
                    OtherValues = new HorizontalAlignment[] { HorizontalAlignment.Right, HorizontalAlignment.Left, HorizontalAlignment.Stretch, HorizontalAlignment.Center },
                    InvalidValues = new Dictionary<HorizontalAlignment, Type>
                    {
                        { (HorizontalAlignment)(-1), typeof(OverflowException) },
                        { (HorizontalAlignment)4, typeof(ArgumentException) },
                        { (HorizontalAlignment)10, typeof(ArgumentException) },
                        { (HorizontalAlignment)27, typeof(ArgumentException) },
                        { (HorizontalAlignment)int.MaxValue, typeof(OverflowException) },
                        { (HorizontalAlignment)int.MinValue, typeof(OverflowException) }
                    }
                };
            VerticalAlignmentProperty = new DependencyPropertyTest<FrameworkElement, VerticalAlignment>(this, "VerticalAlignment")
                {
                    Property = FrameworkElement.VerticalAlignmentProperty,
                    Initializer = initializer,
                    DefaultValue = VerticalAlignment.Stretch,
                    OtherValues = new VerticalAlignment[] { VerticalAlignment.Bottom, VerticalAlignment.Top, VerticalAlignment.Center, VerticalAlignment.Stretch },
                    InvalidValues = new Dictionary<VerticalAlignment, Type>
                    {
                        { (VerticalAlignment)(-1), typeof(OverflowException) },
                        { (VerticalAlignment)4, typeof(ArgumentException) },
                        { (VerticalAlignment)10, typeof(ArgumentException) },
                        { (VerticalAlignment)27, typeof(ArgumentException) },
                        { (VerticalAlignment)int.MaxValue, typeof(OverflowException) },
                        { (VerticalAlignment)int.MinValue, typeof(OverflowException) }
                    }
                };
        }

        /// <summary>
        /// Get any dynamically created test methods.
        /// </summary>
        /// <returns>Dynamically created test methods.</returns>
        public IEnumerable<ITestMethod> GetDynamicTestMethods()
        {
            return GetDependencyPropertyTests().OfType<ITestMethod>();
        }

        /// <summary>
        /// Get any dependency property tests.
        /// </summary>
        /// <returns>Dependency property tests.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Does more work than a property should")]
        public virtual IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // HorizontalAlignmentProperty tests
            yield return HorizontalAlignmentProperty.BindingTest;
            yield return HorizontalAlignmentProperty.CheckDefaultValueTest;
            yield return HorizontalAlignmentProperty.ChangeClrSetterTest;
            yield return HorizontalAlignmentProperty.ChangeSetValueTest;
            yield return HorizontalAlignmentProperty.ClearValueResetsDefaultTest;
            yield return HorizontalAlignmentProperty.InvalidValueFailsTest.Bug("Core property is not validated");
            yield return HorizontalAlignmentProperty.InvalidValueIsIgnoredTest.Bug("Core property is not validated");
            yield return HorizontalAlignmentProperty.InvalidValueDoesNotChangeVisualStateTest;
            yield return HorizontalAlignmentProperty.CanBeStyledTest;
            yield return HorizontalAlignmentProperty.TemplateBindTest;
            yield return HorizontalAlignmentProperty.DoesNotChangeVisualStateTest(HorizontalAlignment.Left, HorizontalAlignment.Right);
            yield return HorizontalAlignmentProperty.SetXamlAttributeTest;
            yield return HorizontalAlignmentProperty.SetXamlElementTest;

            // HorizontalAlignmentProperty tests
            yield return VerticalAlignmentProperty.BindingTest;
            yield return VerticalAlignmentProperty.CheckDefaultValueTest;
            yield return VerticalAlignmentProperty.ChangeClrSetterTest;
            yield return VerticalAlignmentProperty.ChangeSetValueTest;
            yield return VerticalAlignmentProperty.ClearValueResetsDefaultTest;
            yield return VerticalAlignmentProperty.InvalidValueFailsTest.Bug("Core property is not validated");
            yield return VerticalAlignmentProperty.InvalidValueIsIgnoredTest.Bug("Core property is not validated");
            yield return VerticalAlignmentProperty.InvalidValueDoesNotChangeVisualStateTest;
            yield return VerticalAlignmentProperty.CanBeStyledTest;
            yield return VerticalAlignmentProperty.TemplateBindTest;
            yield return VerticalAlignmentProperty.DoesNotChangeVisualStateTest(VerticalAlignment.Bottom, VerticalAlignment.Center);
            yield return VerticalAlignmentProperty.SetXamlAttributeTest;
            yield return VerticalAlignmentProperty.SetXamlElementTest;
        }

        /// <summary>
        /// Associate an Inherited tag with the test methods.
        /// </summary>
        /// <param name="methods">Test methods.</param>
        /// <returns>List of inherited DependencyPropertyTestMethod.</returns>
        protected static IList<DependencyPropertyTestMethod> TagInherited(IEnumerable<DependencyPropertyTestMethod> methods)
        {
            List<DependencyPropertyTestMethod> tests = new List<DependencyPropertyTestMethod>();
            foreach (DependencyPropertyTestMethod method in methods)
            {
                tests.Add(method);

                // Only add an Inherited tag if it wasn't already added
                bool exists = false;
                foreach (TagAttribute tag in method.Tags)
                {
                    if (string.CompareOrdinal(tag.Tag, "Inherited") == 0)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    method.Tags.Add(new TagAttribute("Inherited"));
                }
            }
            return tests;
        }

        #region Control contract
        /// <summary>
        /// Verify the control's template parts.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template parts.")]
        public virtual void TemplatePartsAreDefined()
        {
            Assert.AreEqual(0, DefaultFrameworkElementToTest.GetType().GetTemplateParts().Count);
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template visual states.")]
        public virtual void TemplateVisualStatesAreDefined()
        {
            Assert.AreEqual(0, DefaultFrameworkElementToTest.GetType().GetVisualStates().Count);
        }

        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's style typed properties.")]
        public virtual void StyleTypedPropertiesAreDefined()
        {
            Assert.AreEqual(0, DefaultFrameworkElementToTest.GetType().GetStyleTypedProperties().Count);
        }
        #endregion Control contract
    }
}