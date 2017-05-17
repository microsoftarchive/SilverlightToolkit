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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// ContentControl unit tests.
    /// </summary>
    public abstract partial class ContentControlTest : ControlTest
    {
        #region Controls to test
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get
            {
                ContentControl c = DefaultContentControlToTest;
                c.Content = "Test Content";
                return c;
            }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override IEnumerable<Control> ControlsToTest
        {
            get
            {
                return
                    ContentControlsToTest.OfType<Control>()
                    .Concat(ContentControlsToTest.Select(
                        c => { c.Content = "Test Content"; return (Control) c; }))
                    .Concat(ContentControlsToTest.Select(
                        c =>
                        {
                            StackPanel p = new StackPanel();
                            p.Children.Add(new Ellipse { Fill = new SolidColorBrush(Colors.Red), Width = 20, Height = 20 });
                            p.Children.Add(new TextBlock { Text = "UIElement Content" });
                            p.Children.Add(new Ellipse { Fill = new SolidColorBrush(Colors.Blue), Width = 20, Height = 20 });
                            c.Content = p;
                            return (Control) c;
                        }));
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { return OverriddenContentControlsToTest.OfType<IOverriddenControl>(); }
        }
        #endregion Controls to test

        #region ContentControls to test
        /// <summary>
        /// Gets a default instance of ContentControl (or a derived type) to test.
        /// </summary>
        public abstract ContentControl DefaultContentControlToTest { get; }

        /// <summary>
        /// Gets instances of ContentControl (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<ContentControl> ContentControlsToTest { get; }

        /// <summary>
        /// Gets instances of IOverriddenContentControl (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<IOverriddenContentControl> OverriddenContentControlsToTest { get; }
        #endregion ContentControls to test

        /// <summary>
        /// Gets the Content dependency property test.
        /// </summary>
        protected DependencyPropertyTest<ContentControl, object> ContentProperty { get; private set; }

        /// <summary>
        /// Gets the ContentTemplate dependency property test.
        /// </summary>
        protected DependencyPropertyTest<ContentControl, DataTemplate> ContentTemplateProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ContentControlTest class.
        /// </summary>
        protected ContentControlTest()
        {
            Func<ContentControl> initializer = () => DefaultContentControlToTest;
            ContentTemplateProperty = new DependencyPropertyTest<ContentControl, DataTemplate>(this, "ContentTemplate")
                {
                    Property = ContentControl.ContentTemplateProperty,
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
            ContentProperty = new DependencyPropertyTest<ContentControl, object>(this, "Content")
                {
                    Property = ContentControl.ContentProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new object[] { 12, "Test Text", Environment.OSVersion, new Ellipse { Fill = new SolidColorBrush(Colors.Red), Width = 20, Height = 20 } },
                    TemplateProperty = ContentTemplateProperty
                };
            HorizontalContentAlignmentProperty.DefaultValue = HorizontalAlignment.Left;
            VerticalContentAlignmentProperty.DefaultValue = VerticalAlignment.Top;
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // ContentProperty tests
            tests.Add(ContentProperty.CheckDefaultValueTest);
            tests.Add(ContentProperty.ChangeClrSetterTest);
            tests.Add(ContentProperty.ChangeSetValueTest);
            tests.Add(ContentProperty.SetNullTest);
            tests.Add(ContentProperty.ClearValueResetsDefaultTest);
            tests.Add(ContentProperty.CanBeStyledTest);
            tests.Add(ContentProperty.TemplateBindTest);
            tests.Add(ContentProperty.DoesNotChangeVisualStateTest(null, "Test"));
            tests.Add(ContentProperty.SetXamlAttributeTest);
            tests.Add(ContentProperty.SetXamlElementTest);
            tests.Add(ContentProperty.SetXamlContentTest);
            tests.Add(ContentProperty.IsContentPropertyTest);
            tests.Add(ContentProperty.DataTemplateWithIntTest);
            tests.Add(ContentProperty.DataTemplateWithStringTest);
            tests.Add(ContentProperty.DataTemplateWithStringAndPropertyTest);
            tests.Add(ContentProperty.DataTemplateWithUIElementFailsTest);
            tests.Add(ContentProperty.DataTemplateWithBusinessObjectTest);

            // ContentTemplateProperty tests
            tests.Add(ContentTemplateProperty.CheckDefaultValueTest);
            tests.Add(ContentTemplateProperty.ChangeClrSetterTest);
            tests.Add(ContentTemplateProperty.ChangeSetValueTest);
            tests.Add(ContentTemplateProperty.SetNullTest);
            tests.Add(ContentTemplateProperty.ClearValueResetsDefaultTest);
            tests.Add(ContentTemplateProperty.CanBeStyledTest);
            tests.Add(ContentTemplateProperty.TemplateBindTest);

            return tests;
        }
    }
}