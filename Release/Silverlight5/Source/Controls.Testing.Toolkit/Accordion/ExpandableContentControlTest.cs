// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// ExpandableContentControl unit tests.
    /// </summary>
    [TestClass]
    [Tag("Accordion")]
    public class ExpandableContentControlTest : ContentControlTest
    {
        /// <summary>
        /// Gets a default instance of ContentControl (or a derived type) to test.
        /// </summary>
        /// <value></value>
        public override ContentControl DefaultContentControlToTest
        {
            get { return DefaultExpandableContentControlToTest; }
        }

        /// <summary>
        /// Gets instances of ContentControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<ContentControl> ContentControlsToTest
        {
            get { return ExpandableContentControlsToTest.OfType<ContentControl>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenContentControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<IOverriddenContentControl> OverriddenContentControlsToTest
        {
            get { return OverriddenExpandableContentControlsToTest.OfType<IOverriddenContentControl>(); }
        }

        #region ExpandableContentControls to test
        /// <summary>
        /// Gets a default instance of ExpandableContentControl (or a derived type) to test.
        /// </summary>
        public virtual ExpandableContentControl DefaultExpandableContentControlToTest
        {
            get { return new ExpandableContentControl(); }
        }

        /// <summary>
        /// Gets instances of ExpandableContentControl (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<ExpandableContentControl> ExpandableContentControlsToTest
        {
            get
            {
                yield return DefaultExpandableContentControlToTest;

                yield return new ExpandableContentControl
                {
                    Content = "content"
                };
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenExpandableContentControl (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<IOverriddenControl> OverriddenExpandableContentControlsToTest
        {
            get { yield break; }
        }
        #endregion ExpandableContentControls to test

        /// <summary>
        /// Gets the RevealMode dependency property test.
        /// </summary>
        protected DependencyPropertyTest<ExpandableContentControl, ExpandDirection> RevealModeProperty { get; private set; }

        /// <summary>
        /// Gets the Percentage dependency property test.
        /// </summary>
        protected DependencyPropertyTest<ExpandableContentControl, double> PercentageProperty { get; private set; }

        /// <summary>
        /// Gets the TargetSize dependency property test.
        /// </summary>
        protected DependencyPropertyTest<ExpandableContentControl, Size> TargetSizeProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ExpandableContentControlTest"/> class.
        /// </summary>
        public ExpandableContentControlTest()
        {
            Func<ExpandableContentControl> initializer = () => DefaultExpandableContentControlToTest;

            RevealModeProperty = new DependencyPropertyTest<ExpandableContentControl, ExpandDirection>(this, "RevealMode")
                                     {
                                         Property = ExpandableContentControl.RevealModeProperty,
                                         Initializer = initializer,
                                         DefaultValue = ExpandDirection.Down,
                                         OtherValues =
                                             new[] { ExpandDirection.Left, ExpandDirection.Right, ExpandDirection.Up }
                                     };
            PercentageProperty = new DependencyPropertyTest<ExpandableContentControl, double>(this, "Percentage")
                                     {
                                         Property = ExpandableContentControl.PercentageProperty,
                                         Initializer = initializer,
                                         DefaultValue = 0,
                                         OtherValues = new[] { 1.0 }
                                     };
            TargetSizeProperty = new DependencyPropertyTest<ExpandableContentControl, Size>(this, "TargetSize")
                                     {
                                         Property = ExpandableContentControl.TargetSizeProperty,
                                         Initializer = initializer,
                                         DefaultValue = new Size(double.NaN, double.NaN),
                                         OtherValues = new[] { new Size(10, 10) }
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

            // RevealMode tests
            tests.Add(RevealModeProperty.CheckDefaultValueTest);
            tests.Add(RevealModeProperty.ChangeClrSetterTest);
            tests.Add(RevealModeProperty.ChangeSetValueTest);
            tests.Add(RevealModeProperty.ClearValueResetsDefaultTest);
            tests.Add(RevealModeProperty.CanBeStyledTest);
            tests.Add(RevealModeProperty.TemplateBindTest);
            tests.Add(RevealModeProperty.SetXamlAttributeTest);
            tests.Add(RevealModeProperty.SetXamlElementTest);

            // Percentage tests
            tests.Add(PercentageProperty.CheckDefaultValueTest);
            tests.Add(PercentageProperty.ChangeClrSetterTest);
            tests.Add(PercentageProperty.ChangeSetValueTest);
            tests.Add(PercentageProperty.ClearValueResetsDefaultTest);

            // TargetSize tests
            tests.Add(TargetSizeProperty.CheckDefaultValueTest);
            tests.Add(TargetSizeProperty.ChangeClrSetterTest);
            tests.Add(TargetSizeProperty.ChangeSetValueTest);
            tests.Add(TargetSizeProperty.ClearValueResetsDefaultTest);

            return tests;
        }

        #region Control contract
        /// <summary>
        /// Verify the control's template parts.
        /// </summary>
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> parts = DefaultFrameworkElementToTest.GetType().GetTemplateParts();
            Assert.AreEqual(1, parts.Count);
            Assert.AreEqual(typeof(ContentPresenter), parts["ContentSite"], "Failed to find expected part ContentSite!");
        }
        #endregion Control contract

        /// <summary>
        /// Tests that TargetStyle can be styled.
        /// </summary>
        [TestMethod]
        [Description("Tests that TargetStyle can be styled.")]
        public virtual void ShouldAllowStylingTargetStyle()
        {
            Style s = new Style(typeof(ExpandableContentControl));
            s.Setters.Add(new Setter(ExpandableContentControl.TargetSizeProperty, new Size(400, 300)));

            ExpandableContentControl ecc = new ExpandableContentControl();
            ecc.Style = s;
        }

        /// <summary>
        /// Tests that ExpandableContentControl allows retemplating with no templateparts.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that ExpandableContentControl allows retemplating with no templateparts.")]
        public virtual void ShouldAllowNoTemplateParts()
        {
            ExpandableContentControl ecc = new ExpandableContentControl();
            ecc.Template = new ControlTemplate();

            // touch all the public api.
            ecc.TargetSize = new Size(4, 4);
            ecc.RevealMode = ExpandDirection.Left;
            ecc.Percentage = 0.5;

            // show on screen
            TestAsync(ecc);
        }
    }
}
