// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.DataVisualization;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Overridden TreeMap that provides access to virtual members for testing.
    /// </summary>
    public class OverriddenTreeMap : TreeMap
    {
        /// <summary>
        /// Initializes a new instance of the OverriddenTreeMap class.
        /// </summary>
        public OverriddenTreeMap()
        {
        }

        /// <summary>
        /// Initializes a new instance of the OverriddenTreeMap class.
        /// </summary>
        /// <param name="constructorTest">
        /// Test actions to perform after the TreeMap constructor.
        /// </param>
        public OverriddenTreeMap(Action constructorTest)
        {
            Action invariantTest = () => AssertInvariants();
            GetContainerForItemOverrideActions = new OverriddenMethod<FrameworkElement, object, int>(invariantTest);
            ApplyTemplateActions = new OverriddenMethod(invariantTest);
            MeasureActions = new OverriddenMethod<Size, Size?>(invariantTest);
            ArrangeActions = new OverriddenMethod<Size, Size?>(invariantTest);

            AssertInvariants();
            if (constructorTest != null)
            {
                constructorTest();
            }
        }

        /// <summary>
        /// Ensure that TreeMap invariants are satisfied.
        /// </summary>
        public void AssertInvariants()
        {
            Assert.IsNotNull(ItemDefinition, "ItemDefinition should not be null!");
        }

        /// <summary>
        /// Gets test actions for the GetContainerForItemOverride method.
        /// </summary>
        public OverriddenMethod<FrameworkElement, object, int> GetContainerForItemOverrideActions { get; private set; }

        /// <summary>
        /// Gets test actions for the OnApplyTemplate method.
        /// </summary>
        public OverriddenMethod ApplyTemplateActions { get; private set; }

        /// <summary>
        /// Apply a control template to the TreeMap.
        /// </summary>
        public override void OnApplyTemplate()
        {
            ApplyTemplateActions.DoPreTest();
            base.OnApplyTemplate();
            ApplyTemplateActions.DoTest();
        }

        /// <summary>
        /// Gets test actions for the MeasureOverride method.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for testing")]
        public OverriddenMethod<Size, Size?> MeasureActions { get; private set; }

        /// <summary>
        /// Measure the TreeMap.
        /// </summary>
        /// <param name="availableSize">Size available for the TreeMap.</param>
        /// <returns>Desired size of the TreeMap.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            MeasureActions.DoPreTest(availableSize, null);
            Size desired = base.MeasureOverride(availableSize);
            MeasureActions.DoTest(availableSize, desired);
            return desired;
        }

        /// <summary>
        /// Gets test actions for the ArrangeOverride method.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for testing")]
        public OverriddenMethod<Size, Size?> ArrangeActions { get; private set; }

        /// <summary>
        /// Arrange the TreeMap.
        /// </summary>
        /// <param name="finalSize">Final size for the TreeMap.</param>
        /// <returns>Final size used by the TreeMap.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeActions.DoPreTest(finalSize, null);
            Size used = base.ArrangeOverride(finalSize);
            ArrangeActions.DoTest(finalSize, used);
            return used;
        }
    }
}
