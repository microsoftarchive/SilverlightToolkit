// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// UpDownBase unit tests.
    /// </summary>
    /// <typeparam name="T">Type of Value property.</typeparam>
    public abstract class UpDownBaseTest<T> : UpDownBaseTest
    {
        #region UpDownBases to test
        /// <summary>
        /// Gets a default instance of UpDownBase (or a derived type) to test.
        /// </summary>
        public override UpDownBase DefaultUpDownBaseToTest
        {
            get { return DefaultUpDownBaseTToTest; }
        }

        /// <summary>
        /// Gets instances of UpDownBase (or derived types) to test.
        /// </summary>
        public override IEnumerable<UpDownBase> UpDownBasesToTest
        {
            get { return UpDownBaseTsToTest.OfType<UpDownBase>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenUpDownBase (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenUpDownBase> OverriddenUpDownBasesToTest
        {
            get { return OverriddenUpDownBaseTsToTest.OfType<IOverriddenUpDownBase>(); }
        }
        #endregion UpDownBases to test

        #region UpDownBase<T>s to test
        /// <summary>
        /// Gets a default instance of UpDownBase&lt;T&gt; (or a derived type) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ts", Justification = "To avoid name collision with non generic UpDownBaseTest.")]
        public abstract UpDownBase<T> DefaultUpDownBaseTToTest { get; }

        /// <summary>
        /// Gets instances of UpDownBase&lt;T&gt; (or derived types) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ts", Justification = "To avoid name collision with non generic UpDownBaseTest.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public abstract IEnumerable<UpDownBase<T>> UpDownBaseTsToTest { get; }

        /// <summary>
        /// Gets instances of IOverriddenUpDownBase&lt;T&gt; (or derived types) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ts", Justification = "To avoid name collision with non generic UpDownBaseTest.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public abstract IEnumerable<IOverriddenUpDownBase<T>> OverriddenUpDownBaseTsToTest { get; }
        #endregion UpDownBase<T>s to test

        /// <summary>
        /// Gets the Value dependency property test.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design for testing.")]
        protected DependencyPropertyTest<UpDownBase<T>, T> ValueProperty { get; private set; }

        /// <summary>
        /// Gets the IsEditable dependency property test.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design for testing.")]
        protected DependencyPropertyTest<UpDownBase<T>, bool> IsEditableProperty { get; private set; }

        /////// <summary>
        /////// Gets the IsCyclic dependency property test.
        /////// </summary>
        ////[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design for testing.")]
        ////protected DependencyPropertyTest<UpDownBase<T>, bool> IsCyclicProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the UpDownBaseTest&lt;T&gt; class.
        /// </summary>
        protected UpDownBaseTest()
        {
            Func<UpDownBase<T>> initializer = () => DefaultUpDownBaseTToTest;
            ValueProperty = new DependencyPropertyTest<UpDownBase<T>, T>(this, "Value")
            {
                Property = UpDownBase<T>.ValueProperty,
                Initializer = initializer,
                DefaultValue = default(T),
                OtherValues = new T[] { }
            };
            IsEditableProperty = new DependencyPropertyTest<UpDownBase<T>, bool>(this, "IsEditable")
            {
                Property = UpDownBase<T>.IsEditableProperty,
                Initializer = initializer,
                DefaultValue = true,
                OtherValues = new bool[] { false }
            };
            ////IsCyclicProperty = new DependencyPropertyTest<UpDownBase<T>, bool>(this, "IsCyclic")
            ////{
            ////    Property = UpDownBase<T>.IsCyclicProperty,
            ////    Initializer = initializer,
            ////    DefaultValue = false,
            ////    OtherValues = new bool[] { true }
            ////};
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // ValueProperty tests
            tests.Add(ValueProperty.CheckDefaultValueTest);
            tests.Add(ValueProperty.ChangeClrSetterTest);
            tests.Add(ValueProperty.ChangeSetValueTest);
            tests.Add(ValueProperty.ClearValueResetsDefaultTest);
            ////tests.Add(ValueProperty.SetNullTest);
            tests.Add(ValueProperty.CanBeStyledTest);
            tests.Add(ValueProperty.TemplateBindTest);
            ////tests.Add(ValueProperty.DoesNotChangeVisualStateTest(default(T), default(T)));
            tests.Add(ValueProperty.SetXamlAttributeTest);
            tests.Add(ValueProperty.SetXamlElementTest);

            // IsEditableProperty tests
            tests.Add(IsEditableProperty.CheckDefaultValueTest);
            tests.Add(IsEditableProperty.ChangeClrSetterTest);
            tests.Add(IsEditableProperty.ChangeSetValueTest);
            tests.Add(IsEditableProperty.ClearValueResetsDefaultTest);
            tests.Add(IsEditableProperty.CanBeStyledTest);
            tests.Add(IsEditableProperty.TemplateBindTest);
            tests.Add(IsEditableProperty.DoesNotChangeVisualStateTest(true, false));
            tests.Add(IsEditableProperty.SetXamlAttributeTest);
            tests.Add(IsEditableProperty.SetXamlElementTest);

            ////// IsCyclicProperty tests
            ////tests.Add(IsCyclicProperty.CheckDefaultValueTest);
            ////tests.Add(IsCyclicProperty.ChangeClrSetterTest);
            ////tests.Add(IsCyclicProperty.ChangeSetValueTest);
            ////tests.Add(IsCyclicProperty.ClearValueResetsDefaultTest);
            ////tests.Add(IsCyclicProperty.CanBeStyledTest);
            ////tests.Add(IsCyclicProperty.TemplateBindTest);
            ////tests.Add(IsCyclicProperty.DoesNotChangeVisualStateTest(true, false));
            ////tests.Add(IsCyclicProperty.SetXamlAttributeTest);
            ////tests.Add(IsCyclicProperty.SetXamlElementTest.Bug("TODO: XAML Parser?"));

            return tests;
        }
    }
}