// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Tests for Picker.
    /// </summary>
    public abstract class PickerTest : ControlTest
    {
        #region Getting instances to test.
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return DefaultPickerToTest; }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override IEnumerable<Control> ControlsToTest
        {
            get { return (IEnumerable<Control>)PickersToTest; }
        }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { return (IEnumerable<IOverriddenControl>)OverriddenPickersToTest; }
        }

        /// <summary>
        /// Gets the default picker to test.
        /// </summary>
        /// <value>The default picker to test.</value>
        public abstract Picker DefaultPickerToTest { get; }

        /// <summary>
        /// Gets the pickers to test.
        /// </summary>
        /// <value>The pickers to test.</value>
        public abstract IEnumerable<Picker> PickersToTest { get; }

        /// <summary>
        /// Gets the overridden pickers to test.
        /// </summary>
        /// <value>The overridden pickers to test.</value>
        public abstract IEnumerable<IOverriddenControl> OverriddenPickersToTest { get; } 
        #endregion Getting instances to test.

        #region Dependency properties
        /// <summary>
        /// Gets the is drop down open property.
        /// </summary>
        protected DependencyPropertyTest<Picker, bool> IsDropDownOpenProperty { get; private set; }

        /// <summary>
        /// Gets the popup button mode property.
        /// </summary>
        protected DependencyPropertyTest<Picker, ClickMode> PopupButtonModeProperty { get; private set; }

        /// <summary>
        /// Gets the max drop down height property.
        /// </summary>
        protected DependencyPropertyTest<Picker, double> MaxDropDownHeightProperty { get; private set; } 
        #endregion Dependency properties

        /// <summary>
        /// Initializes a new instance of the <see cref="PickerTest"/> class.
        /// </summary>
        protected PickerTest()
        {
            Func<Picker> initalizer = () => DefaultPickerToTest;

            IsDropDownOpenProperty = new DependencyPropertyTest<Picker, bool>(this, "IsDropDownOpen")
                                         {
                                             Property = Picker.IsDropDownOpenProperty,
                                             DefaultValue = false,
                                             Initializer = initalizer,
                                             OtherValues = new[] { true },
                                         };

            PopupButtonModeProperty = new DependencyPropertyTest<Picker, ClickMode>(this, "PopupButtonMode")
                                          {
                                              Property = Picker.PopupButtonModeProperty,
                                              DefaultValue = ClickMode.Release,
                                              Initializer = initalizer,
                                              OtherValues = new[] { ClickMode.Release, ClickMode.Hover },
                                              InvalidValues = new Dictionary<ClickMode, Type>
                                                                  {
                                                                      { (ClickMode)23, typeof(ArgumentException) },
                                                                      { (ClickMode)65, typeof(ArgumentException) },
                                                                  }
                                          };

            MaxDropDownHeightProperty = new DependencyPropertyTest<Picker, double>(this, "MaxDropDownHeight")
                                            {
                                                Property = Picker.MaxDropDownHeightProperty,
                                                DefaultValue = double.PositiveInfinity,
                                                Initializer = initalizer,
                                                OtherValues = new[] { 1.0, 250.0, 10000.0 },
                                                InvalidValues = new Dictionary<double, Type>
                                                    {
                                                        { -1.0, typeof(ArgumentException) },
                                                        { -11234.4, typeof(ArgumentException) },
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

            // IsDropDownOpen tests
            tests.Add(IsDropDownOpenProperty.CheckDefaultValueTest);
            tests.Add(IsDropDownOpenProperty.ChangeSetValueTest);
            tests.Add(IsDropDownOpenProperty.ClearValueResetsDefaultTest);
            tests.Add(IsDropDownOpenProperty.TemplateBindTest);
            tests.Add(IsDropDownOpenProperty.ChangesVisualStateTest(false, true, "PopupOpened"));
            tests.Add(IsDropDownOpenProperty.ChangesVisualStateTest(true, false, "PopupClosed"));

            // PopupButtonMode tests
            tests.Add(PopupButtonModeProperty.CheckDefaultValueTest);
            tests.Add(PopupButtonModeProperty.ChangeSetValueTest);
            tests.Add(PopupButtonModeProperty.ClearValueResetsDefaultTest);
            tests.Add(PopupButtonModeProperty.InvalidValueFailsTest);
            tests.Add(PopupButtonModeProperty.InvalidValueIsIgnoredTest);
            tests.Add(PopupButtonModeProperty.DoesNotChangeVisualStateTest(ClickMode.Hover, ClickMode.Press));
            tests.Add(PopupButtonModeProperty.DoesNotChangeVisualStateTest(ClickMode.Press, ClickMode.Hover));
            tests.Add(PopupButtonModeProperty.DoesNotChangeVisualStateTest(ClickMode.Press, ClickMode.Release));
            tests.Add(PopupButtonModeProperty.SetXamlAttributeTest);
            tests.Add(PopupButtonModeProperty.SetXamlElementTest);
            tests.Add(PopupButtonModeProperty.CanBeStyledTest);

            // MaxDropDownHeight tests
            tests.Add(MaxDropDownHeightProperty.CheckDefaultValueTest);
            tests.Add(MaxDropDownHeightProperty.ChangeSetValueTest);
            tests.Add(MaxDropDownHeightProperty.ClearValueResetsDefaultTest);
            tests.Add(MaxDropDownHeightProperty.InvalidValueFailsTest);
            tests.Add(MaxDropDownHeightProperty.InvalidValueIsIgnoredTest);
            tests.Add(MaxDropDownHeightProperty.TemplateBindTest);
            tests.Add(MaxDropDownHeightProperty.CanBeStyledTest);

            return tests;
        }

        /// <summary>
        /// Tests Dropdown events firing.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests Dropdown events firing.")]
        [Tag("Picker")]
        public virtual void ShouldFireDropDownEvents()
        {
            Picker picker = DefaultPickerToTest;

            long? opening = null, opened = null, closing = null,  closed = null;

            picker.DropDownOpening += (sender, e) => { opening = DateTime.Now.Ticks; Thread.Sleep(5); };
            picker.DropDownOpened += (sender, e) => opened = DateTime.Now.Ticks;
            picker.DropDownClosing += (sender, e) => { closing = DateTime.Now.Ticks; Thread.Sleep(5); };
            picker.DropDownClosed += (sender, e) => closed = DateTime.Now.Ticks;

            bool isLoaded = false;
            picker.Loaded += delegate { isLoaded = true; };

            // Add the element to the test surface and wait until it's loaded
            EnqueueCallback(() => TestPanel.Children.Add(picker));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => opening = opened = closing = closed = null);
            EnqueueCallback(() => picker.IsDropDownOpen = true);
            EnqueueConditional(() => picker.IsDropDownOpen == true);
            EnqueueConditional(() => opened != null);
            EnqueueCallback(() => Assert.IsTrue(opened > opening));
            EnqueueCallback(() => picker.IsDropDownOpen = false);
            EnqueueConditional(() => closed != null);
            EnqueueCallback(() => Assert.IsTrue(closed > closing));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that the DropDownClosedEvent is not raised when showing the control.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that the DropDownClosedEvent is not raised when showing the control.")]
        public virtual void ShouldNotRaiseDropDownClosedEventsWhenInstantiating()
        {
            Picker picker = DefaultPickerToTest;

            picker.DropDownClosing += (sender, e) => Assert.Fail();
            picker.DropDownClosed += (sender, e) => Assert.Fail();

            TestAsync(picker);
        }

        /// <summary>
        /// Tests that opening of DropDown can be cancelled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that opening of DropDown can be cancelled.")]
        [Tag("Picker")]
        public virtual void ShouldBeAbleToCancelDropDownOpen()
        {
            Picker picker = DefaultPickerToTest;

            bool isLoaded = false;
            picker.Loaded += delegate { isLoaded = true; };
            bool opened = false;
            bool opening = false;
            picker.DropDownOpening += (sender, e) =>
                                          {
                                              e.Cancel = true;
                                              opening = true;
                                          };
            picker.DropDownOpened += (sender, e) => opened = true;

            EnqueueCallback(() => TestPanel.Children.Add(picker));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => picker.IsDropDownOpen = true);
            EnqueueConditional(() => opening == true);
            EnqueueCallback(() => Assert.IsFalse(opened));            
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that closing of DropDown can be cancelled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that closing of DropDown can be cancelled.")]
        [Tag("Picker")]
        [Timeout(2000)]
        public virtual void ShouldBeAbleToCancelDropDownClose()
        {
            Picker picker = DefaultPickerToTest;

            bool opened = false;
            int closecount = 0;
            bool allowedToClose = false;
            picker.DropDownOpened += (sender, e) => opened = true;
            picker.DropDownClosing += (sender, e) =>
                                          {
                                              if (!allowedToClose)
                                              {
                                                  e.Cancel = true;
                                              }
                                          };
            
            picker.DropDownClosed += (sender, e) => closecount ++;

            TestPanel.Children.Add(picker);

            EnqueueCallback(() => picker.IsDropDownOpen = true);

            EnqueueCallback(() => Assert.IsTrue(opened));
            EnqueueCallback(() => closecount = 0); // is fired by onapplytemplate, so reset it
            EnqueueCallback(() => picker.IsDropDownOpen = false);
            EnqueueCallback(() => Assert.IsTrue(picker.IsDropDownOpen));
            EnqueueCallback(() => Assert.IsTrue(closecount == 0));
            EnqueueCallback(() => allowedToClose = true);
            EnqueueCallback(() => picker.IsDropDownOpen = false);

            // need to be fully closed before we finish test.
            EnqueueConditional(() => closecount == 1);

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that popup is closed when focus is lost.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that popup is closed when focus is lost.")]
        [Tag("RequiresFocus")]
        [Tag("Picker")]
        [Timeout(2000)]
        public virtual void ShouldClosePopupOnFocusLost()
        {
            Picker picker = DefaultPickerToTest;
            Button button = new Button { Content = "A button to set focus to", VerticalAlignment = VerticalAlignment.Top };
            TestPanel.Children.Add(button);

            bool opened = false;
            bool closed = false;
            picker.DropDownOpened += (sender, e) => opened = true;
            picker.DropDownClosed += (sender, e) => closed = true;

            TestPanel.Children.Add(picker);

            EnqueueCallback(() => picker.IsDropDownOpen = true);
            EnqueueCallback(() => Assert.IsTrue(opened));
            EnqueueCallback(() => button.Focus());
            EnqueueConditional(() => closed);
            EnqueueTestComplete();
        }
    }
}
