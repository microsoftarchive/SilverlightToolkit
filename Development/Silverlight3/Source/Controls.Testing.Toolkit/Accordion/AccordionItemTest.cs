// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// AccordionItem unit tests.
    /// </summary>
    [TestClass]
    [Tag("AccordionTest")]
    public class AccordionItemTest : HeaderedContentControlTest
    {
        #region HeaderedContentControls to test
        /// <summary>
        /// Gets a default instance of AccordionItem (or a derived type) to test.
        /// </summary>
        /// <value></value>
        public override HeaderedContentControl DefaultHeaderedContentControlToTest
        {
            get
            {
                return DefaultAccordionItemToTest;
            }
        }

        /// <summary>
        /// Gets instances of AccordionItem (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<HeaderedContentControl> HeaderedContentControlsToTest
        {
            get
            {
                return AccordionItemsToTest.OfType<HeaderedContentControl>();
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenHeaderedContentControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<IOverriddenHeaderedContentControl> OverriddenHeaderedContentControlsToTest
        {
            get
            {
                return OverriddenAccordionItemsToTest.OfType<IOverriddenHeaderedContentControl>();
            }
        } 
        #endregion HeaderedContentControls to test

        #region AccordionItems to test
        /// <summary>
        /// Gets a default instance of AccordionItem (or a derived type) to test.
        /// </summary>
        public virtual AccordionItem DefaultAccordionItemToTest
        {
            get { return new AccordionItem(); }
        }

        /// <summary>
        /// Gets instances of AccordionItem (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<AccordionItem> AccordionItemsToTest
        {
            get
            {
                yield return DefaultAccordionItemToTest;

                yield return new AccordionItem
                {
                    Header = "Test Header",
                    Content = "Test Content"
                };
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenAccordionItem (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<IOverriddenControl> OverriddenAccordionItemsToTest
        {
            get { yield break; }
        }
        #endregion AccordionItems to test

        /// <summary>
        /// Gets the ExpandDirection dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AccordionItem, ExpandDirection> ExpandDirectionProperty { get; private set; }

        /// <summary>
        /// Gets the IsSelected dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AccordionItem, bool> IsSelectedProperty { get; private set; }

        /// <summary>
        /// Gets the accordion button style property.
        /// </summary>
        protected DependencyPropertyTest<AccordionItem, Style> AccordionButtonStyleProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionItemTest"/> class.
        /// </summary>
        public AccordionItemTest()
        {
            BackgroundProperty.DefaultValue = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xff, 0xff));
            BorderBrushProperty.DefaultValue = new SolidColorBrush(Color.FromArgb(0xff, 0xec, 0xec, 0xec));
            BorderThicknessProperty.DefaultValue = new Thickness(1);
            PaddingProperty.DefaultValue = new Thickness(0);
            HorizontalContentAlignmentProperty.DefaultValue = HorizontalAlignment.Left;
            VerticalContentAlignmentProperty.DefaultValue = VerticalAlignment.Center;

            Func<AccordionItem> initializer = () => DefaultAccordionItemToTest;

            ExpandDirectionProperty = new DependencyPropertyTest<AccordionItem, ExpandDirection>(this, "ExpandDirection")
                                          {
                                              Initializer = initializer,
                                              Property = AccordionItem.ExpandDirectionProperty,
                                              DefaultValue = ExpandDirection.Down,
                                              OtherValues =
                                                  new[] { ExpandDirection.Up, ExpandDirection.Left, ExpandDirection.Right },
                                              InvalidValues = new Dictionary<ExpandDirection, Type>
                                                                  {
                                                                      { (ExpandDirection)99, typeof(ArgumentOutOfRangeException) },
                                                                      { (ExpandDirection)66, typeof(ArgumentOutOfRangeException) }
                                                                  }
                                          };
            IsSelectedProperty = new DependencyPropertyTest<AccordionItem, bool>(this, "IsSelected")
                                     {
                                         Initializer = initializer,
                                         Property = AccordionItem.IsSelectedProperty,
                                         DefaultValue = false,
                                         OtherValues = new[] { true }
                                     };

            AccordionButtonStyleProperty = new DependencyPropertyTest<AccordionItem, Style>(this, "AccordionButtonStyle")
                                               {
                                                   Initializer = initializer,
                                                   Property = AccordionItem.AccordionButtonStyleProperty,
                                                   DefaultValue = null,
                                                   OtherValues = new[] { new Style(typeof(AccordionButton)), new Style(typeof(Control)) }
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

            tests.Add(IsEnabledProperty.ChangesVisualStateTest(false, true, "Normal"));
            tests.Add(IsEnabledProperty.ChangesVisualStateTest(true, false, "Disabled"));
            tests.Add(IsEnabledProperty.DoesNotChangeVisualStateTest(true, true));
            tests.Add(IsEnabledProperty.DoesNotChangeVisualStateTest(false, false));

            // ExpandDirection tests
            // TODO: uncomment these tests if we decide to open up ExpandDirection.
            // tests.Add(ExpandDirectionProperty.CheckDefaultValueTest);
            // tests.Add(ExpandDirectionProperty.ChangeClrSetterTest);
            // tests.Add(ExpandDirectionProperty.ChangeSetValueTest);
            // tests.Add(ExpandDirectionProperty.ClearValueResetsDefaultTest);
            // tests.Add(ExpandDirectionProperty.InvalidValueFailsTest);
            // tests.Add(ExpandDirectionProperty.InvalidValueIsIgnoredTest);
            // tests.Add(ExpandDirectionProperty.CanBeStyledTest);
            // tests.Add(ExpandDirectionProperty.TemplateBindTest);
            // tests.Add(ExpandDirectionProperty.ChangesVisualStateTest(ExpandDirection.Down, ExpandDirection.Up, "ExpandUp"));
            // tests.Add(ExpandDirectionProperty.ChangesVisualStateTest(ExpandDirection.Up, ExpandDirection.Left, "ExpandLeft"));
            // tests.Add(ExpandDirectionProperty.ChangesVisualStateTest(ExpandDirection.Left, ExpandDirection.Right, "ExpandRight"));
            // tests.Add(ExpandDirectionProperty.ChangesVisualStateTest(ExpandDirection.Right, ExpandDirection.Down, "ExpandDown"));
            // tests.Add(ExpandDirectionProperty.SetXamlAttributeTest);
            // tests.Add(ExpandDirectionProperty.SetXamlElementTest);

            // IsSelected tests
            tests.Add(IsSelectedProperty.CheckDefaultValueTest);
            tests.Add(IsSelectedProperty.ChangeClrSetterTest);
            tests.Add(IsSelectedProperty.ChangeSetValueTest);
            tests.Add(IsSelectedProperty.ClearValueResetsDefaultTest);
            tests.Add(IsSelectedProperty.CanBeStyledTest);
            tests.Add(IsSelectedProperty.TemplateBindTest.Tag(Tags.RequiresFocus));
            tests.Add(IsSelectedProperty.ChangesVisualStateTest(false, true, "Expanded"));
            tests.Add(IsSelectedProperty.ChangesVisualStateTest(true, false, "Collapsed"));
            tests.Add(IsSelectedProperty.SetXamlAttributeTest);
            tests.Add(IsSelectedProperty.SetXamlElementTest.Bug("TODO: XAML Parser?"));

            // AccordionButtonStyleProperty tests
            tests.Add(AccordionButtonStyleProperty.CheckDefaultValueTest);
            tests.Add(AccordionButtonStyleProperty.ChangeClrSetterTest);
            tests.Add(AccordionButtonStyleProperty.ChangeSetValueTest);
            tests.Add(AccordionButtonStyleProperty.SetNullTest);
            tests.Add(AccordionButtonStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(AccordionButtonStyleProperty.CanBeStyledTest);
            tests.Add(AccordionButtonStyleProperty.TemplateBindTest);
            tests.Add(AccordionButtonStyleProperty.SetXamlAttributeTest.Bug("TODO: XAML Parser?"));
            tests.Add(AccordionButtonStyleProperty.SetXamlElementTest.Bug("TODO: XAML Parser?"));

            DependencyPropertyTestMethod buggedTest = tests.FirstOrDefault(a => a.Name == HeaderProperty.TemplateBindTest.Name);
            if (buggedTest != null)
            {
                buggedTest.Bug("Find out why this fails for AccordionItem and not for HeaderedContentControl.");
            }

            return tests;
        }

        #region control contract
        /// <summary>
        /// Verify the control's template parts.
        /// </summary>
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> parts = DefaultFrameworkElementToTest.GetType().GetTemplateParts();
            Assert.AreEqual(2, parts.Count);
            Assert.AreEqual(typeof(ExpandableContentControl), parts["ExpandSite"], "Failed to find expected part ExpandSite!");
            Assert.AreEqual(typeof(AccordionButton), parts["ExpanderButton"], "Failed to find expected part ExpanderButton!");
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> states = DefaultAccordionItemToTest.GetType().GetVisualStates();
            Assert.AreEqual(14, states.Count, "Incorrect number of template states");
            Assert.AreEqual("CommonStates", states["Normal"], "Failed to find expected state Normal!");
            Assert.AreEqual("CommonStates", states["MouseOver"], "Failed to find expected state MouseOver!");
            Assert.AreEqual("CommonStates", states["Pressed"], "Failed to find expected state Pressed!");
            Assert.AreEqual("CommonStates", states["Disabled"], "Failed to find expected state Disabled!");
            Assert.AreEqual("FocusStates", states["Focused"], "Failed to find expected state Focused!");
            Assert.AreEqual("FocusStates", states["Unfocused"], "Failed to find expected state Unfocused!");
            Assert.AreEqual("ExpansionStates", states["Expanded"], "Failed to find expected state Expanded!");
            Assert.AreEqual("ExpansionStates", states["Collapsed"], "Failed to find expected state Collapsed!");
            Assert.AreEqual("LockedStates", states["Locked"], "Failed to find expected state Locked!");
            Assert.AreEqual("LockedStates", states["Unlocked"], "Failed to find expected state Unlocked!");
            Assert.AreEqual("ExpandDirectionStates", states["ExpandDown"], "Failed to find expected state ExpandDown!");
            Assert.AreEqual("ExpandDirectionStates", states["ExpandUp"], "Failed to find expected state ExpandUp!");
            Assert.AreEqual("ExpandDirectionStates", states["ExpandLeft"], "Failed to find expected state ExpandLeft!");
            Assert.AreEqual("ExpandDirectionStates", states["ExpandRight"], "Failed to find expected state ExpandRight!");
        }

        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultAccordionItemToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(1, properties.Count, "Incorrect number of style typed property attributes!");
            Assert.AreEqual(typeof(AccordionButton), properties["AccordionButtonStyle"], "Failed to find expected style type property AccordionButtonStyle!");
        }
        #endregion control contract
    }
}