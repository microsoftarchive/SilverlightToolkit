// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// HeaderedItemsControl unit tests.
    /// </summary>
    [TestClass]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
    public class HeaderedItemsControlTest : ItemsControlTest
    {
        #region ItemsControls to test
        /// <summary>
        /// Gets a default instance of ItemsControl (or a derived type) to test.
        /// </summary>
        public override ItemsControl DefaultItemsControlToTest
        {
            get
            {
                HeaderedItemsControl control = DefaultHeaderedItemsControlToTest;
                control.Header = "Test Header";
                return control;
            }
        }

        /// <summary>
        /// Gets instances of ItemsControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<ItemsControl> ItemsControlsToTest
        {
            get
            {
                return
                    HeaderedItemsControlsToTest.OfType<ItemsControl>()
                    .Concat(HeaderedItemsControlsToTest.Select(
                        c => { c.Header = "Test Content"; return (ItemsControl) c; }))
                    .Concat(HeaderedItemsControlsToTest.Select(
                        c =>
                        {
                            StackPanel p = new StackPanel();
                            p.Children.Add(new Ellipse { Fill = new SolidColorBrush(Colors.Red), Width = 20, Height = 20 });
                            p.Children.Add(new TextBlock { Text = "UIElement Content" });
                            p.Children.Add(new Ellipse { Fill = new SolidColorBrush(Colors.Blue), Width = 20, Height = 20 });
                            c.Header = p;
                            return (ItemsControl) c;
                        }));
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenItemsControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenItemsControl> OverriddenItemsControlsToTest
        {
            get { return OverriddenHeaderedItemsControlsToTest.OfType<IOverriddenItemsControl>(); }
        }
        #endregion ItemsControls to test

        #region HeaderedItemsControls to test
        /// <summary>
        /// Gets a default instance of HeaderedItemsControl (or a derived type) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
        public virtual HeaderedItemsControl DefaultHeaderedItemsControlToTest
        {
            get { return new HeaderedItemsControl(); }
        }

        /// <summary>
        /// Gets instances of HeaderedItemsControl (or derived types) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
        public virtual IEnumerable<HeaderedItemsControl> HeaderedItemsControlsToTest
        {
            get
            {
                yield return DefaultHeaderedItemsControlToTest;

                Style itemContainerStyle = new Style(typeof(Control));
                itemContainerStyle.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Red)));
                itemContainerStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.Bold));
                yield return new HeaderedItemsControl { ItemContainerStyle = itemContainerStyle };
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenHeaderedItemsControl (or derived types) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
        public virtual IEnumerable<IOverriddenHeaderedItemsControl> OverriddenHeaderedItemsControlsToTest
        {
            get { yield return new OverriddenHeaderedItemsControl(); }
        }
        #endregion HeaderedItemsControls to test

        /// <summary>
        /// Gets the Header dependency property test.
        /// </summary>
        protected DependencyPropertyTest<HeaderedItemsControl, object> HeaderProperty { get; private set; }

        /// <summary>
        /// Gets the HeaderTemplate dependency property test.
        /// </summary>
        protected DependencyPropertyTest<HeaderedItemsControl, DataTemplate> HeaderTemplateProperty { get; private set; }

        /// <summary>
        /// Gets the ItemContainerStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<HeaderedItemsControl, Style> ItemContainerStyleProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HeaderedItemsControlTest class.
        /// </summary>
        public HeaderedItemsControlTest()
            : base()
        {
            Func<HeaderedItemsControl> initializer = () => DefaultHeaderedItemsControlToTest;
            HeaderTemplateProperty = new DependencyPropertyTest<HeaderedItemsControl, DataTemplate>(this, "HeaderTemplate")
                {
                    Property = HeaderedItemsControl.HeaderTemplateProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new DataTemplate[]
                    {
                        new DataTemplate(),
                        new XamlBuilder<DataTemplate>().Load(),
                        (new XamlBuilder<DataTemplate> { Name = "Template" }).Load(),
                        (new XamlBuilder<DataTemplate> { Name = "Template", Children = new List<XamlBuilder> { new XamlBuilder<StackPanel>() } }).Load()
                    }
                };
            HeaderProperty = new DependencyPropertyTest<HeaderedItemsControl, object>(this, "Header")
                {
                    Property = HeaderedItemsControl.HeaderProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new object[] { 12, "Test Text", Environment.OSVersion, new Ellipse { Fill = new SolidColorBrush(Colors.Red), Width = 20, Height = 20 } },
                    TemplateProperty = HeaderTemplateProperty
                };
            ItemContainerStyleProperty = new DependencyPropertyTest<HeaderedItemsControl, Style>(this, "ItemContainerStyle")
                {
                    Property = HeaderedItemsControl.ItemContainerStyleProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new Style[] { new Style(typeof(HeaderedItemsControl)), new Style(typeof(ItemsControl)), new Style(typeof(Control)) }
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

            // HeaderProperty tests
            tests.Add(HeaderProperty.BindingTest);
            tests.Add(HeaderProperty.CheckDefaultValueTest);
            tests.Add(HeaderProperty.ChangeClrSetterTest);
            tests.Add(HeaderProperty.ChangeSetValueTest);
            tests.Add(HeaderProperty.SetNullTest);
            tests.Add(HeaderProperty.ClearValueResetsDefaultTest);
            tests.Add(HeaderProperty.CanBeStyledTest);
            tests.Add(HeaderProperty.TemplateBindTest.Bug("TODO: Investigate why this fails here but not for the Content property."));
            tests.Add(HeaderProperty.DoesNotChangeVisualStateTest(null, "Test"));
            tests.Add(HeaderProperty.SetXamlAttributeTest);
            tests.Add(HeaderProperty.SetXamlElementTest);
            tests.Add(HeaderProperty.DataTemplateWithIntTest);
            tests.Add(HeaderProperty.DataTemplateWithStringTest);
            tests.Add(HeaderProperty.DataTemplateWithStringAndPropertyTest);
            tests.Add(HeaderProperty.DataTemplateWithUIElementFailsTest);
            tests.Add(HeaderProperty.DataTemplateWithBusinessObjectTest);

            // HeaderTemplateProperty tests
            tests.Add(HeaderTemplateProperty.BindingTest);
            tests.Add(HeaderTemplateProperty.CheckDefaultValueTest);
            tests.Add(HeaderTemplateProperty.ChangeClrSetterTest);
            tests.Add(HeaderTemplateProperty.ChangeSetValueTest);
            tests.Add(HeaderTemplateProperty.SetNullTest);
            tests.Add(HeaderTemplateProperty.ClearValueResetsDefaultTest);
            tests.Add(HeaderTemplateProperty.CanBeStyledTest);
            tests.Add(HeaderTemplateProperty.TemplateBindTest);

            // ItemContainerStyleProperty tests
            tests.Add(ItemContainerStyleProperty.BindingTest);
            tests.Add(ItemContainerStyleProperty.CheckDefaultValueTest);
            tests.Add(ItemContainerStyleProperty.ChangeClrSetterTest);
            tests.Add(ItemContainerStyleProperty.ChangeSetValueTest);
            tests.Add(ItemContainerStyleProperty.SetNullTest);
            tests.Add(ItemContainerStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemContainerStyleProperty.CanBeStyledTest);
            tests.Add(ItemContainerStyleProperty.TemplateBindTest);
            tests.Add(ItemContainerStyleProperty.SetXamlAttributeTest.Bug("TODO: XAML Parser?"));
            tests.Add(ItemContainerStyleProperty.SetXamlElementTest.Bug("TODO: XAML Parser?"));

            return tests;
        }

        #region Control contract
        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultHeaderedItemsControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(1, properties.Count, "Incorrect number of style typed property attributes!");
            Assert.AreEqual(typeof(ContentPresenter), properties["ItemContainerStyle"], "Failed to find expected style type property ItemContainerStyle!");
        }
        #endregion

        /// <summary>
        /// Changing the Header calls the OnHeaderChanged method.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changing the Header calls the OnHeaderChanged method.")]
        public virtual void ChangingHeaderCallsOnHeaderChanged()
        {
            foreach (IOverriddenHeaderedItemsControl overriddenItem in OverriddenHeaderedItemsControlsToTest)
            {
                HeaderedItemsControl control = overriddenItem as HeaderedItemsControl;
                MethodCallMonitor monitor = overriddenItem.HeaderChangedActions.CreateMonitor();

                TestTaskAsync(
                    control,
                    () =>
                    {
                        control.Header = new object();
                        monitor.AssertCalled("OnHeaderChanged was not called when changing the header!");
                    });
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Changing the HeaderTemplate calls the OnHeaderTemplateChanged method.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changing the HeaderTemplate calls the OnHeaderTemplateChanged method.")]
        public virtual void ChangingHeaderCallsOnHeaderTemplateChanged()
        {
            foreach (IOverriddenHeaderedItemsControl overriddenItem in OverriddenHeaderedItemsControlsToTest)
            {
                HeaderedItemsControl control = overriddenItem as HeaderedItemsControl;
                MethodCallMonitor monitor = overriddenItem.HeaderTemplateChangedActions.CreateMonitor();

                TestTaskAsync(
                    control,
                    () =>
                    {
                        control.HeaderTemplate = new DataTemplate();
                        monitor.AssertCalled("OnHeaderTemplateChanged was not called when changing the header template!");
                    });
            }

            EnqueueTestComplete();
        }
    }
}