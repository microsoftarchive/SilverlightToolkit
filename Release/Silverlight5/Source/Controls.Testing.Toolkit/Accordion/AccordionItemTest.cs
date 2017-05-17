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
using System.Threading;
using System.Windows.Shapes;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// AccordionItem unit tests.
    /// </summary>
    [TestClass]
    [Tag("Accordion")]
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
        /// Gets the expandable content control style property.
        /// </summary>
        protected DependencyPropertyTest<AccordionItem, Style> ExpandableContentControlStyleProperty { get; private set; }
        
        /// <summary>
        /// Gets the ContentTargetSize property.
        /// </summary>
        protected DependencyPropertyTest<AccordionItem, Size> ContentTargetSizeProperty { get; private set; }

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
            VerticalContentAlignmentProperty.DefaultValue = VerticalAlignment.Top;
            HorizontalAlignmentProperty.DefaultValue = HorizontalAlignment.Stretch;

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

            ExpandableContentControlStyleProperty = new DependencyPropertyTest<AccordionItem, Style>(this, "ExpandableContentControlStyle")
            {
                Initializer = initializer,
                Property = AccordionItem.ExpandableContentControlStyleProperty,
                DefaultValue = null,
                OtherValues = new[] { new Style(typeof(ExpandableContentControl)), new Style(typeof(Control)) }
            };

            ContentTargetSizeProperty = new DependencyPropertyTest<AccordionItem, Size>(this, "ContentTargetSize")
            {
                Property = AccordionItem.ContentTargetSizeProperty,
                Initializer = initializer,
                DefaultValue = new Size(double.NaN, double.NaN),
                InvalidValues = new Dictionary<Size, Type>
                                    {
                                            { new Size(5, 5), typeof(InvalidOperationException) }
                                    }
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
            tests.Add(IsSelectedProperty.SetXamlElementTest);

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

            // ExpandableContentControlStyleProperty tests
            tests.Add(ExpandableContentControlStyleProperty.CheckDefaultValueTest);
            tests.Add(ExpandableContentControlStyleProperty.ChangeClrSetterTest);
            tests.Add(ExpandableContentControlStyleProperty.ChangeSetValueTest);
            tests.Add(ExpandableContentControlStyleProperty.SetNullTest);
            tests.Add(ExpandableContentControlStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(ExpandableContentControlStyleProperty.CanBeStyledTest);
            tests.Add(ExpandableContentControlStyleProperty.TemplateBindTest);
            tests.Add(ExpandableContentControlStyleProperty.SetXamlAttributeTest.Bug("TODO: XAML Parser?"));
            tests.Add(ExpandableContentControlStyleProperty.SetXamlElementTest.Bug("TODO: XAML Parser?"));

            // ContentTargetSize tests
            tests.Add(ContentTargetSizeProperty.CheckDefaultValueTest);

            DependencyPropertyTestMethod buggedTest = tests.FirstOrDefault(a => a.Name == HeaderProperty.TemplateBindTest.Name);
            if (buggedTest != null)
            {
                buggedTest.Bug("Find out why this fails for AccordionItem and not for HeaderedContentControl.");
            }

            DependencyPropertyTestMethod datatemplateWithUIElementFailsTest = tests.FirstOrDefault(a => a.Name == ContentProperty.DataTemplateWithUIElementFailsTest.Name);
            if (datatemplateWithUIElementFailsTest != null)
            {
                // remove this test. It fails because the content starts out not visible, thus having no elements
                // to find.
                tests.Remove(datatemplateWithUIElementFailsTest);
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
            Assert.AreEqual(2, properties.Count, "Incorrect number of style typed property attributes!");
            Assert.AreEqual(typeof(AccordionButton), properties["AccordionButtonStyle"], "Failed to find expected style type property AccordionButtonStyle!");
            Assert.AreEqual(typeof(ExpandableContentControl), properties["ExpandableContentControlStyle"], "Failed to find expected style type property ExpandableContentControlStyle!");
        }
        #endregion control contract

        /// <summary>
        /// Tests that ExpandDirection can not be set.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Description("Tests that ExpandDirection can not be set.")]
        public virtual void ShouldThrowExceptionWhenSettingExpandDirectionOnAccordionItem()
        {
            AccordionItem item = new AccordionItem();
            item.SetValue(AccordionItem.ExpandDirectionProperty, ExpandDirection.Left);
        }

        /// <summary>
        /// Tests that ContentTargetSize cannot be set.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Description("Tests that ContentTargetSize cannot be set.")]
        public virtual void ShouldThrowExceptionWhenSettingContentTargetSize()
        {
            AccordionItem item = new AccordionItem();
            item.SetValue(AccordionItem.ContentTargetSizeProperty, new Size(3, 3));
        }

        /// <summary>
        /// Tests that AccordionButtonStyle is bound from Accordion.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that AccordionButtonStyle is bound from Accordion.")]
        public virtual void ShouldBindAccordionButtonStyle()
        {
            Accordion acc = new Accordion();
            Style buttonStyle = new Style(typeof(AccordionButton));
            // todo: remove below line to test the actual switch of the style 
            acc.AccordionButtonStyle = buttonStyle;
            acc.Items.Add("item 1");
            acc.Items.Add(new AccordionItem() { Content = "item 2" });
            acc.Items.Add(new AccordionItem() { Content = "item 3", AccordionButtonStyle = new Style(typeof(AccordionButton)) });

            bool loaded = false;
            acc.Loaded += (sender, args) => loaded = true;
            EnqueueCallback(() => TestPanel.Children.Add(acc));
            EnqueueConditional(() => loaded);
            EnqueueCallback(() => acc.AccordionButtonStyle = buttonStyle);
            EnqueueCallback(() => Assert.AreSame(buttonStyle, ((AccordionItem) acc.ItemContainerGenerator.ContainerFromIndex(0)).AccordionButtonStyle));
            EnqueueCallback(() => Assert.AreSame(buttonStyle, ((AccordionItem) acc.ItemContainerGenerator.ContainerFromIndex(1)).AccordionButtonStyle));
            EnqueueCallback(() => Assert.AreNotSame(buttonStyle, ((AccordionItem) acc.ItemContainerGenerator.ContainerFromIndex(2)).AccordionButtonStyle));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that content gets space.
        /// </summary>
        /// <remarks>Schedules a few Thread.Sleeps to get the animation finished.
        /// Should rewrite to RX when that comes online.</remarks>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that content gets space.")]
        public virtual void ShouldBindContentAlignments()
        {
            Accordion acc = new Accordion();
            AccordionItem item = new AccordionItem();
            item.Content = new Button()
                               {
                                       Content = "content"
                               };
            acc.Width = 300;
            acc.Height = 300;
            acc.Items.Add(item);

            TestAsync(
                    acc,
                    () => item.IsSelected = true,
                    () => Thread.Sleep(10),
                    () => Thread.Sleep(10),
                    () => Thread.Sleep(10),
                    () => Assert.IsTrue(((Button) item.Content).ActualWidth < 200),
                    () => Assert.IsTrue(((Button) item.Content).ActualHeight < 200),
                    () => item.VerticalContentAlignment = VerticalAlignment.Stretch,
                    () => item.HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    () => Thread.Sleep(10),
                    () => Thread.Sleep(10),
                    () => Thread.Sleep(10),
                    () => Assert.IsTrue(((Button) item.Content).ActualWidth > 200),
                    () => Assert.IsTrue(((Button) item.Content).ActualHeight > 200));
        }

        /// <summary>
        /// Tests that content may take more space in a non fixed situation.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that content may take more space in a non fixed situation.")]
        public virtual void ShouldAllowContentToResizeInNonFixedSizeScenario()
        {
            Accordion acc = new Accordion();
            AccordionItem item = new AccordionItem();
            item.Content = new Shapes.Rectangle()
                               {
                                       Width = 100,
                                       Height = 100
                               };
            acc.Items.Add(item);

            TestAsync(
                    acc,
                    () => item.IsSelected = true,
                    () => Thread.Sleep(10),
                    () => Thread.Sleep(10),
                    () => Thread.Sleep(10),
                    () => Assert.IsTrue(item.ActualWidth < 110),
                    () => Assert.IsTrue(item.ActualHeight < 110),
                    () => Assert.IsTrue(item.ActualHeight > 60),
                    () => ((Shapes.Rectangle) item.Content).Height = 400,
                    () => Thread.Sleep(10),
                    () => Thread.Sleep(10),
                    () => Thread.Sleep(10),
                    () => Assert.IsTrue(item.ActualHeight > 230));
        }

        /// <summary>
        /// Verify the application of a DataTemplate with UIElement content
        /// fails.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the application of a DataTemplate with UIElement content fails.")]
        [Tag("DataTemplate")]
        [Tag("DP")]
        [Tag("DependencyProperty")]
        [Tag("VisualTree")]
        public virtual void ShouldAllowTemplateWithUIElements()
        {
            // this tests adds back the coverage lost by the exclusion of
            // test: Content*DataTemplateWithUIElementsFails (see GetDPTests())
            AccordionItem instance = new AccordionItem();
            instance.Width = 300;
            instance.Height = 300;

            TestAsync(
                    instance,
                    () =>
                        {
                            StackPanel element = new StackPanel();
                            element.SetValue(FrameworkElement.NameProperty, "UIElementContent");
                            element.Children.Add(new Ellipse
                                                     {
                                                             Fill = new SolidColorBrush(Colors.Red),
                                                             Width = 20,
                                                             Height = 20
                                                     });
                            element.Children.Add(new TextBlock
                                                     {
                                                             Text = "UIElement Content"
                                                     });
                            element.Children.Add(new Ellipse
                                                     {
                                                             Fill = new SolidColorBrush(Colors.Blue),
                                                             Width = 20,
                                                             Height = 20
                                                     });
                            instance.SetValue(ContentControl.ContentProperty, element);
                        },
                    () => instance.IsSelected = true,
                () =>
                {
                    // Create the DataTemplate
                    XamlBuilder<DataTemplate> xamlTemplate = new XamlBuilder<DataTemplate>
                    {
                        Name = "template",
                        Children = new List<XamlBuilder>
                        {
                            new XamlBuilder<StackPanel>
                            {
                                Children = new List<XamlBuilder>
                                {
                                    new XamlBuilder<StackPanel>
                                    {
                                        AttributeProperties = new Dictionary<string, string> { { "Orientation", "Horizontal" } },
                                        Children = new List<XamlBuilder>
                                        {
                                            new XamlBuilder<TextBlock>
                                            {
                                                AttributeProperties = new Dictionary<string, string> { { "Text", "{}{Binding Name}:  " } }
                                            },
                                            new XamlBuilder<TextBlock>
                                            {
                                                Name = "nameBinding",
                                                AttributeProperties = new Dictionary<string, string> { { "Text", "{Binding Name}" } }
                                            }
                                        }
                                    },
                                    new XamlBuilder<StackPanel>
                                    {
                                        AttributeProperties = new Dictionary<string, string> { { "Orientation", "Horizontal" } },
                                        Children = new List<XamlBuilder>
                                        {
                                            new XamlBuilder<TextBlock>
                                            {
                                                AttributeProperties = new Dictionary<string, string> { { "Text", "{}{Binding}:  " } }
                                            },
                                            new XamlBuilder<ContentControl>
                                            {
                                                Name = "contentBinding",
                                                AttributeProperties = new Dictionary<string, string> { { "Content", "{Binding}" } }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };

                    // Generate the DataTemplate and set it
                    instance.SetValue(ContentControl.ContentTemplateProperty, xamlTemplate.Load());
                },
                () =>
                {
                    // Verify the bindings didn't work
                    TextBlock text = instance.GetVisualChild("nameBinding") as TextBlock;
                    Assert.IsNotNull(text, "Failed to find nameBinding TextBlock!");
                    TestExtensions.AssertIsNullOrEmpty(text.Text);

                    ContentControl content = instance.GetVisualChild("contentBinding") as ContentControl;
                    Assert.IsNotNull(content, "Failed to find contentBinding ContentControl!");
                    Assert.IsNull(content.Content, "The bound Content should be null!");
                });
        }
    }
}
