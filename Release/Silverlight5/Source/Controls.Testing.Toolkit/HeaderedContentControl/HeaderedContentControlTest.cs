// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// HeaderedContentControl unit tests.
    /// </summary>
    [TestClass]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
    public partial class HeaderedContentControlTest : ContentControlTest
    {
        #region ContentControls to test
        /// <summary>
        /// Gets a default instance of ContentControl (or a derived type) to test.
        /// </summary>
        public override ContentControl DefaultContentControlToTest 
        { 
            get
            {
                return DefaultHeaderedContentControlToTest;
            }
        }

        /// <summary>
        /// Gets instances of ContentControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<ContentControl> ContentControlsToTest 
        { 
            get
            {
                return
                    HeaderedContentControlsToTest.OfType<ContentControl>()
                    .Concat(HeaderedContentControlsToTest.Select(
                        c => { c.Header = "Test Content"; return (ContentControl)c; }))
                    .Concat(HeaderedContentControlsToTest.Select(
                        c =>
                        {
                            StackPanel p = new StackPanel();
                            p.Children.Add(new Ellipse { Fill = new SolidColorBrush(Colors.Red), Width = 20, Height = 20 });
                            p.Children.Add(new TextBlock { Text = "UIElement Content" });
                            p.Children.Add(new Ellipse { Fill = new SolidColorBrush(Colors.Blue), Width = 20, Height = 20 });
                            c.Header = p;
                            return (ContentControl)c;
                        }));
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenContentControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenContentControl> OverriddenContentControlsToTest
        {
            get
            {
                return OverriddenHeaderedContentControlsToTest.OfType<IOverriddenContentControl>();
            }
        }
        #endregion ContentControls to test

        #region HeaderedContentControls to test
        /// <summary>
        /// Gets a default instance of HeaderedContentControl (or a derived type) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
        public virtual HeaderedContentControl DefaultHeaderedContentControlToTest
        {
            get
            {
                return new HeaderedContentControl();
            }
        }

        /// <summary>
        /// Gets instances of HeaderedContentControl (or derived types) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
        public virtual IEnumerable<HeaderedContentControl> HeaderedContentControlsToTest
        {
            get
            {
                yield return DefaultHeaderedContentControlToTest;

                string template = @"<DataTemplate xmlns=""http://schemas.microsoft.com/client/2007""
                                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                        xmlns:controls=""clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls"">
                                        <StackPanel>
                                            <Ellipse Fill=""Red"" Width=""20"" Height=""20""/>
                                            <Button Content=""{Binding}"" />
                                            <Ellipse Fill=""Red"" Width=""20"" Height=""20""/>
                                        </StackPanel>
                                    </DataTemplate>";
                DataTemplate dt = (DataTemplate)XamlReader.Load(template);
                yield return new HeaderedContentControl
                {
                    Header = "Test Header",
                    HeaderTemplate = dt
                };
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenHeaderedContentControl (or derived types) to test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
        public virtual IEnumerable<IOverriddenHeaderedContentControl> OverriddenHeaderedContentControlsToTest
        {
            get { yield return new OverriddenHeaderedContentControl(); }
        }
        #endregion HeaderedContentControls to test

        /// <summary>
        /// Gets the Header dependency property test.
        /// </summary>
        protected DependencyPropertyTest<HeaderedContentControl, object> HeaderProperty { get; private set; }

        /// <summary>
        /// Gets the HeaderTemplate dependency property test.
        /// </summary>
        protected DependencyPropertyTest<HeaderedContentControl, DataTemplate> HeaderTemplateProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HeaderedContentControlTest class.
        /// </summary>
        public HeaderedContentControlTest()
            : base()
        {
            HorizontalContentAlignmentProperty.DefaultValue = HorizontalAlignment.Left;
            VerticalContentAlignmentProperty.DefaultValue = VerticalAlignment.Top;

            Func<HeaderedContentControl> initializer = () => DefaultHeaderedContentControlToTest;
            HeaderTemplateProperty = new DependencyPropertyTest<HeaderedContentControl, DataTemplate>(this, "HeaderTemplate")
            {
                Property = HeaderedContentControl.HeaderTemplateProperty,
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
            HeaderProperty = new DependencyPropertyTest<HeaderedContentControl, object>(this, "Header")
            {
                Property = HeaderedContentControl.HeaderProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new object[] 
                { 
                    12, 
                    string.Empty, 
                    "Test Text", 
                    Environment.OSVersion, 
                    new Ellipse { Fill = new SolidColorBrush(Colors.Red), Width = 20, Height = 20 } 
                },
                TemplateProperty = HeaderTemplateProperty
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
            tests.Add(HeaderProperty.CheckDefaultValueTest);
            tests.Add(HeaderProperty.ChangeClrSetterTest);
            tests.Add(HeaderProperty.ChangeSetValueTest);
            tests.Add(HeaderProperty.SetNullTest);
            tests.Add(HeaderProperty.ClearValueResetsDefaultTest);
            tests.Add(HeaderProperty.CanBeStyledTest);
            // tests.Add(HeaderProperty.TemplateBindTest); // Inconsistent repro of bug 78616
            tests.Add(HeaderProperty.DoesNotChangeVisualStateTest(null, "Test"));
            tests.Add(HeaderProperty.SetXamlAttributeTest);
            tests.Add(HeaderProperty.SetXamlElementTest);
            tests.Add(HeaderProperty.DataTemplateWithIntTest);
            tests.Add(HeaderProperty.DataTemplateWithStringTest);
            tests.Add(HeaderProperty.DataTemplateWithStringAndPropertyTest);
            tests.Add(HeaderProperty.DataTemplateWithUIElementFailsTest);
            tests.Add(HeaderProperty.DataTemplateWithBusinessObjectTest);

            // HeaderTemplateProperty tests
            tests.Add(HeaderTemplateProperty.CheckDefaultValueTest);
            tests.Add(HeaderTemplateProperty.ChangeClrSetterTest);
            tests.Add(HeaderTemplateProperty.ChangeSetValueTest);
            tests.Add(HeaderTemplateProperty.SetNullTest);
            tests.Add(HeaderTemplateProperty.ClearValueResetsDefaultTest);
            tests.Add(HeaderTemplateProperty.CanBeStyledTest);
            tests.Add(HeaderTemplateProperty.TemplateBindTest);

            return tests;
        }

        /// <summary>
        /// Changing the Header calls the OnHeaderChanged method.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changing the Header calls the OnHeaderChanged method.")]
        public virtual void ChangingHeaderCallsOnHeaderChanged()
        {
            foreach (IOverriddenHeaderedContentControl overriddenItem in OverriddenHeaderedContentControlsToTest)
            {
                HeaderedContentControl control = overriddenItem as HeaderedContentControl;
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
            foreach (IOverriddenHeaderedContentControl overriddenItem in OverriddenHeaderedContentControlsToTest)
            {
                HeaderedContentControl control = overriddenItem as HeaderedContentControl;
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