// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the System.Windows.Controls.BusyIndicator class.
    /// </summary>
    [TestClass]
    public partial class BusyIndicatorTest : ControlTest
    {
        #region Controls to test
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return DefaultBusyIndicatorToTest; }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override IEnumerable<Control> ControlsToTest
        {
            get { return BusyIndicatorsToTest.OfType<Control>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { yield break; }
        }
        #endregion Controls to test

        #region BusyIndicators to test
        /// <summary>
        /// Gets a default instance of BusyIndicator (or a derived type) to test.
        /// </summary>
        public virtual BusyIndicator DefaultBusyIndicatorToTest
        {
            get { return new BusyIndicator(); }
        }

        /// <summary>
        /// Gets instances of BusyIndicator (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<BusyIndicator> BusyIndicatorsToTest
        {
            get
            {
                yield return DefaultBusyIndicatorToTest;
            }
        }
        #endregion BusyIndicators to test

        /// <summary>
        /// Gets the IsBusy dependency property test.
        /// </summary>
        protected DependencyPropertyTest<BusyIndicator, bool> IsBusyProperty { get; private set; }

        /// <summary>
        /// Gets the BusyContent dependency property test.
        /// </summary>
        protected DependencyPropertyTest<BusyIndicator, object> BusyContentProperty { get; private set; }

        /// <summary>
        /// Gets the BusyTemplate dependency property test.
        /// </summary>
        protected DependencyPropertyTest<BusyIndicator, DataTemplate> BusyContentTemplateProperty { get; private set; }

        /// <summary>
        /// Gets the DisplayAfter dependency property test.
        /// </summary>
        protected DependencyPropertyTest<BusyIndicator, TimeSpan> DisplayAfterProperty { get; private set; }

        /// <summary>
        /// Gets the OverlayStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<BusyIndicator, Style> OverlayStyleProperty { get; private set; }

        /// <summary>
        /// Gets the ProgressBarStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<BusyIndicator, Style> ProgressBarStyleProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the BusyIndicatorTest class.
        /// </summary>
        public BusyIndicatorTest()
            : base()
        {
            IsBusyProperty = new DependencyPropertyTest<BusyIndicator, bool>(this, "IsBusy")
            {
                Property = BusyIndicator.IsBusyProperty,
                Initializer = () => DefaultBusyIndicatorToTest,
                DefaultValue = false,
                OtherValues = new bool[] { false },
            };
            BusyContentProperty = new DependencyPropertyTest<BusyIndicator, object>(this, "BusyContent")
            {
                Property = BusyIndicator.BusyContentProperty,
                Initializer = () => DefaultBusyIndicatorToTest,
                DefaultValue = "Please wait...",
                OtherValues = new object[] { 12, "Test Text", Environment.OSVersion },
            };
            BusyContentTemplateProperty = new DependencyPropertyTest<BusyIndicator, DataTemplate>(this, "BusyContentTemplate")
            {
                Property = BusyIndicator.BusyContentTemplateProperty,
                Initializer = () => DefaultBusyIndicatorToTest,
                DefaultValue = null,
                OtherValues = new DataTemplate[]
                {
                    new DataTemplate(),
                    new XamlBuilder<DataTemplate>().Load(),
                    (new XamlBuilder<DataTemplate> { Name = "Template" }).Load(),
                    (new XamlBuilder<DataTemplate> { Name = "Template", Children = new List<XamlBuilder> { new XamlBuilder<StackPanel>() } }).Load()
                }
            };
            DisplayAfterProperty = new DependencyPropertyTest<BusyIndicator, TimeSpan>(this, "DisplayAfter")
            {
                Property = BusyIndicator.DisplayAfterProperty,
                Initializer = () => DefaultBusyIndicatorToTest,
                DefaultValue = TimeSpan.FromSeconds(0.1),
                OtherValues = new TimeSpan[] { TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(1) },
            };
            OverlayStyleProperty = new DependencyPropertyTest<BusyIndicator, Style>(this, "OverlayStyle")
            {
                Property = BusyIndicator.OverlayStyleProperty,
                Initializer = () => DefaultBusyIndicatorToTest,
                DefaultValue = null,
                OtherValues = new Style[] { new Style { TargetType = typeof(Rectangle) } },
            };
            ProgressBarStyleProperty = new DependencyPropertyTest<BusyIndicator, Style>(this, "ProgressBarStyle")
            {
                Property = BusyIndicator.ProgressBarStyleProperty,
                Initializer = () => DefaultBusyIndicatorToTest,
                DefaultValue = null,
                OtherValues = new Style[] { new Style { TargetType = typeof(ProgressBar) } },
            };
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // IsBusyProperty tests
            tests.Add(IsBusyProperty.BindingTest);
            tests.Add(IsBusyProperty.CheckDefaultValueTest);
            tests.Add(IsBusyProperty.ChangeClrSetterTest);
            tests.Add(IsBusyProperty.ChangeSetValueTest);
            tests.Add(IsBusyProperty.ClearValueResetsDefaultTest);
            tests.Add(IsBusyProperty.CanBeStyledTest);
            tests.Add(IsBusyProperty.TemplateBindTest);
            tests.Add(IsBusyProperty.SetXamlAttributeTest);
            tests.Add(IsBusyProperty.SetXamlElementTest);

            // BusyContentProperty tests
            tests.Add(BusyContentProperty.BindingTest);
            tests.Add(BusyContentProperty.CheckDefaultValueTest);
            tests.Add(BusyContentProperty.ChangeClrSetterTest);
            tests.Add(BusyContentProperty.ChangeSetValueTest);
            tests.Add(BusyContentProperty.SetNullTest);
            tests.Add(BusyContentProperty.ClearValueResetsDefaultTest);
            tests.Add(BusyContentProperty.CanBeStyledTest);
            tests.Add(BusyContentProperty.TemplateBindTest);
            tests.Add(BusyContentProperty.SetXamlAttributeTest);
            tests.Add(BusyContentProperty.SetXamlElementTest);

            // BusyContentTemplateProperty tests
            tests.Add(BusyContentTemplateProperty.BindingTest);
            tests.Add(BusyContentTemplateProperty.CheckDefaultValueTest);
            tests.Add(BusyContentTemplateProperty.ChangeClrSetterTest);
            tests.Add(BusyContentTemplateProperty.ChangeSetValueTest);
            tests.Add(BusyContentTemplateProperty.ClearValueResetsDefaultTest);
            tests.Add(BusyContentTemplateProperty.SetNullTest);
            tests.Add(BusyContentTemplateProperty.CanBeStyledTest);
            tests.Add(BusyContentTemplateProperty.TemplateBindTest);

            // DisplayAfterProperty tests
            tests.Add(DisplayAfterProperty.BindingTest);
            tests.Add(DisplayAfterProperty.CheckDefaultValueTest);
            tests.Add(DisplayAfterProperty.ChangeClrSetterTest);
            tests.Add(DisplayAfterProperty.ChangeSetValueTest);
            tests.Add(DisplayAfterProperty.ClearValueResetsDefaultTest);
            tests.Add(DisplayAfterProperty.CanBeStyledTest);
            tests.Add(DisplayAfterProperty.TemplateBindTest);
            tests.Add(DisplayAfterProperty.SetXamlAttributeTest);

            // OverlayStyleProperty tests
            tests.Add(OverlayStyleProperty.BindingTest);
            tests.Add(OverlayStyleProperty.ChangeClrSetterTest);
            tests.Add(OverlayStyleProperty.ChangeSetValueTest);
            tests.Add(OverlayStyleProperty.SetNullTest);
            tests.Add(OverlayStyleProperty.CanBeStyledTest);
            tests.Add(OverlayStyleProperty.TemplateBindTest);

            // ProgressBarStyleProperty tests
            tests.Add(ProgressBarStyleProperty.BindingTest);
            tests.Add(ProgressBarStyleProperty.ChangeClrSetterTest);
            tests.Add(ProgressBarStyleProperty.ChangeSetValueTest);
            tests.Add(ProgressBarStyleProperty.SetNullTest);
            tests.Add(ProgressBarStyleProperty.CanBeStyledTest);
            tests.Add(ProgressBarStyleProperty.TemplateBindTest);

            // Tweak base class tests
            HorizontalAlignmentProperty.DefaultValue = HorizontalAlignment.Stretch;
            VerticalAlignmentProperty.DefaultValue = VerticalAlignment.Stretch;
            HorizontalContentAlignmentProperty.DefaultValue = HorizontalAlignment.Stretch;
            VerticalContentAlignmentProperty.DefaultValue = VerticalAlignment.Stretch;

            return tests;
        }

        /// <summary>
        /// Verify the control's visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the Control Template's visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> visualStates = DefaultControlToTest.GetType().GetVisualStates();
            Assert.AreEqual(4, visualStates.Count);

            Assert.AreEqual<string>("BusyStatusStates", visualStates["Busy"]);
            Assert.AreEqual<string>("BusyStatusStates", visualStates["Idle"]);
            Assert.AreEqual<string>("VisibilityStates", visualStates["Visible"]);
            Assert.AreEqual<string>("VisibilityStates", visualStates["Hidden"]);
        }

        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(2, properties.Count);
            Assert.AreEqual(typeof(Rectangle), properties["OverlayStyle"]);
            Assert.AreEqual(typeof(ProgressBar), properties["ProgressBarStyle"]);
        }

        /// <summary>
        /// Verifies that BusyIndicator doesn't stay alive after being removed from the visual tree.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that BusyIndicator doesn't stay alive after being removed from the visual tree.")]
        [Bug("88589 - BusyIndicator has a leak because it hooks DispatcherTimer and doesn't unhook it if unloaded - should use Unloaded event on SL4 to remedy this", Fixed = true)]
        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Deliberately calling method to verify scenario.")]
        public void BusyIndicatorLeaksWhenRemoved()
        {
            WeakReference weakReference = new WeakReference(new BusyIndicator { IsBusy = true });
            Grid grid = new Grid();
            grid.Children.Add((UIElement)weakReference.Target);
            TestAsync(
              grid,
              () => Assert.IsNotNull(weakReference.Target),
              () => grid.Children.Clear(),
              () => GC.Collect(),
              () => Assert.IsNull(weakReference.Target));
        }
    }
}