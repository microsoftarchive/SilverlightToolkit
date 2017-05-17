// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Expander unit tests.
    /// </summary>
    [TestClass]
    public partial class ExpanderTest : HeaderedContentControlTest
    {
        #region HeaderedContentControls to test
        /// <summary>
        /// Gets a default instance of HeaderedContentControl (or a derived type) to test.
        /// </summary>
        public override HeaderedContentControl DefaultHeaderedContentControlToTest
        {
            get
            {
                return DefaultExpanderToTest;
            }
        }

        /// <summary>
        /// Gets instances of HeaderedContentControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<HeaderedContentControl> HeaderedContentControlsToTest
        {
            get
            {
                return ExpandersToTest.OfType<HeaderedContentControl>();
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenContentControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenHeaderedContentControl> OverriddenHeaderedContentControlsToTest
        {
            get
            {
                return OverriddenExpandersToTest.OfType<IOverriddenHeaderedContentControl>();
            }
        }
        #endregion HeaderedContentControls to test

        #region Expanders to test
        /// <summary>
        /// Gets a default instance of Expander (or a derived type) to test.
        /// </summary>
        public virtual Expander DefaultExpanderToTest
        {
            get
            {
                return new Expander();
            }
        }

        /// <summary>
        /// Gets instances of Expander (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<Expander> ExpandersToTest
        {
            get
            {
                yield return DefaultExpanderToTest;

                // This may cause ExpanderTest to go extremely slow.
                for (int i = 0; i < 4; i++)
                {
                    Expander expander = new Expander
                    {
                        ExpandDirection = (ExpandDirection)i,
                        IsExpanded = (i % 2 == 0)
                    };
                    yield return expander;
                }
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenContentControl (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<IOverriddenExpander> OverriddenExpandersToTest
        {
            get
            {
                yield break;
            }
        }
        #endregion Expanders to test

        /// <summary>
        /// Gets the IsExpanded dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Expander, bool> IsExpandedProperty { get; private set; }

        /// <summary>
        /// Gets ExpandDirection dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Expander, ExpandDirection> ExpandDirectionProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ExpanderTest class.
        /// </summary>
        public ExpanderTest()
            : base()
        {
            BorderThicknessProperty.DefaultValue = new Thickness(1);
            HorizontalContentAlignmentProperty.DefaultValue = HorizontalAlignment.Stretch;
            VerticalContentAlignmentProperty.DefaultValue = VerticalAlignment.Stretch;

            Func<ContentControl> expandedInitializer =
                () =>
                {
                    Expander expander = DefaultExpanderToTest;
                    expander.IsExpanded = true;
                    return expander;
                };
            ContentProperty.Initializer = expandedInitializer;

            Func<Expander> initializer = () => DefaultExpanderToTest;
            IsExpandedProperty = new DependencyPropertyTest<Expander, bool>(this, "IsExpanded")
            {
                Property = Expander.IsExpandedProperty,
                Initializer = initializer,
                DefaultValue = false,
                OtherValues = new bool[] { true }
            };
            ExpandDirectionProperty = new DependencyPropertyTest<Expander, ExpandDirection>(this, "ExpandDirection")
            {
                Property = Expander.ExpandDirectionProperty,
                Initializer = initializer,
                DefaultValue = ExpandDirection.Down,
                OtherValues = new ExpandDirection[] { ExpandDirection.Up, ExpandDirection.Left, ExpandDirection.Right },
                InvalidValues = new Dictionary<ExpandDirection, Type>
                {
                    { (ExpandDirection)(-1), typeof(ArgumentException) },
                    { (ExpandDirection)4, typeof(ArgumentException) },
                    { (ExpandDirection)5, typeof(ArgumentException) },
                    { (ExpandDirection)500, typeof(ArgumentException) },
                    { (ExpandDirection)int.MaxValue, typeof(ArgumentException) },
                    { (ExpandDirection)int.MinValue, typeof(ArgumentException) }
                }
            };
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            tests.Add(IsEnabledProperty.ChangesVisualStateTest(false, true, "Normal"));
            tests.Add(IsEnabledProperty.ChangesVisualStateTest(true, false, "Disabled"));
            tests.Add(IsEnabledProperty.DoesNotChangeVisualStateTest(true, true));
            tests.Add(IsEnabledProperty.DoesNotChangeVisualStateTest(false, false));

            // IsExpandedProperty tests
            tests.Add(IsExpandedProperty.CheckDefaultValueTest);
            tests.Add(IsExpandedProperty.ChangeClrSetterTest);
            tests.Add(IsExpandedProperty.ChangeSetValueTest);
            tests.Add(IsExpandedProperty.ClearValueResetsDefaultTest);
            tests.Add(IsExpandedProperty.CanBeStyledTest);
            DependencyPropertyTestMethod focusTest = IsExpandedProperty.TemplateBindTest;
            focusTest.Tags.Add(new TagAttribute(Tags.RequiresFocus));
            tests.Add(focusTest);
            tests.Add(IsExpandedProperty.ChangesVisualStateTest(false, true, "Expanded"));
            tests.Add(IsExpandedProperty.ChangesVisualStateTest(true, false, "Collapsed"));
            tests.Add(IsExpandedProperty.SetXamlAttributeTest);
            tests.Add(IsExpandedProperty.SetXamlElementTest);

            // ExpandDirectionProperty tests
            tests.Add(ExpandDirectionProperty.CheckDefaultValueTest);
            tests.Add(ExpandDirectionProperty.ChangeClrSetterTest);
            tests.Add(ExpandDirectionProperty.ChangeSetValueTest);
            tests.Add(ExpandDirectionProperty.ClearValueResetsDefaultTest);
            tests.Add(ExpandDirectionProperty.InvalidValueFailsTest);
            tests.Add(ExpandDirectionProperty.InvalidValueIsIgnoredTest);
            tests.Add(ExpandDirectionProperty.CanBeStyledTest);
            tests.Add(ExpandDirectionProperty.TemplateBindTest);
            tests.Add(ExpandDirectionProperty.ChangesVisualStateTest(ExpandDirection.Down, ExpandDirection.Up, "ExpandUp"));
            tests.Add(ExpandDirectionProperty.ChangesVisualStateTest(ExpandDirection.Up, ExpandDirection.Left, "ExpandLeft"));
            tests.Add(ExpandDirectionProperty.ChangesVisualStateTest(ExpandDirection.Left, ExpandDirection.Right, "ExpandRight"));
            tests.Add(ExpandDirectionProperty.ChangesVisualStateTest(ExpandDirection.Right, ExpandDirection.Down, "ExpandDown"));
            tests.Add(ExpandDirectionProperty.SetXamlAttributeTest);
            tests.Add(ExpandDirectionProperty.SetXamlElementTest);

            return tests;
        }

        #region Control contract
        /// <summary>
        /// Verifies the Control's TemplateParts.
        /// </summary>
        [TestMethod]
        [Description("Verifies the Control's TemplateParts.")]
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> templateParts = DefaultControlToTest.GetType().GetTemplateParts();
            Assert.AreEqual(1, templateParts.Count);
            Assert.AreSame(typeof(ToggleButton), templateParts["ExpanderButton"]);
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> visualStates = DefaultControlToTest.GetType().GetVisualStates();
            Assert.AreEqual(12, visualStates.Count);

            Assert.AreEqual<string>("CommonStates", visualStates["Normal"]);
            Assert.AreEqual<string>("CommonStates", visualStates["MouseOver"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Pressed"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Disabled"]);

            Assert.AreEqual<string>("FocusStates", visualStates["Focused"]);
            Assert.AreEqual<string>("FocusStates", visualStates["Unfocused"]);

            Assert.AreEqual<string>("ExpansionStates", visualStates["Expanded"]);
            Assert.AreEqual<string>("ExpansionStates", visualStates["Collapsed"]);

            Assert.AreEqual<string>("ExpandDirectionStates", visualStates["ExpandDown"]);
            Assert.AreEqual<string>("ExpandDirectionStates", visualStates["ExpandUp"]);
            Assert.AreEqual<string>("ExpandDirectionStates", visualStates["ExpandLeft"]);
            Assert.AreEqual<string>("ExpandDirectionStates", visualStates["ExpandRight"]);
        }
        #endregion Control contract
    }
}
