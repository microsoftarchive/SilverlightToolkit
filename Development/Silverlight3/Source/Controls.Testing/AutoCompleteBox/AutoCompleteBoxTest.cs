// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// AutoCompleteBox control tests.
    /// </summary>
    [TestClass]
    public partial class AutoCompleteBoxTest : ControlTest
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get
            {
                return new AutoCompleteBox();
            }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override IEnumerable<Control> ControlsToTest
        {
            get
            {
                yield return new AutoCompleteBox();
                yield return new AutoCompleteBox
                {
                    IsTextCompletionEnabled = false
                };

                // A few sample modes
                List<AutoCompleteSearchMode> modes = new List<AutoCompleteSearchMode>
                {
                    AutoCompleteSearchMode.Contains,
                    AutoCompleteSearchMode.StartsWithCaseSensitive
                };
                foreach (AutoCompleteSearchMode mode in modes)
                {
                    yield return new AutoCompleteBox 
                    { 
                        SearchMode = mode
                    };
                }
            }
        }

        /// <summary>
        /// Gets the overridden controls to test.
        /// </summary>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the AutoCompleteBoxTest class.
        /// </summary>
        public AutoCompleteBoxTest()
        {
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());
            return tests;
        }

        /// <summary>
        /// Creates an AutoCompleteBox control instance with a large set of 
        /// string data.
        /// </summary>
        /// <returns>Returns a new AutoCompleteBox with a set height and 
        /// ItemsSource.</returns>
        private AutoCompleteBox CreateSimpleStringAutoComplete()
        {
            AutoCompleteBox ac = (AutoCompleteBox)DefaultControlToTest;
            ac.ItemsSource = CreateSimpleStringArray();
            ac.Height = 32;
            return ac;
        }

        /// <summary>
        /// Creates a testable AutoCompleteBox instance.
        /// </summary>
        /// <returns>Returns a new AutoCompleteBox instance.</returns>
        private static OverriddenAutoCompleteBox GetDerivedAutoComplete()
        {
            return new OverriddenAutoCompleteBox()
            {
                ItemsSource = CreateSimpleStringArray(),
                Height = 32,
            };
        }

        /// <summary>
        /// Retrieves a defined predicate filter through a new AutoCompleteBox 
        /// control instance.
        /// </summary>
        /// <param name="mode">The SearchMode of interest.</param>
        /// <returns>Returns the predicate instance.</returns>
        private static AutoCompleteSearchPredicate<string> GetFilter(AutoCompleteSearchMode mode)
        {
            return new AutoCompleteBox { SearchMode = mode }
                .TextFilter;
        }

        /// <summary>
        /// Create a new SelectorSelectionAdapter.
        /// </summary>
        [TestMethod]
        [Description("Create a new SelectorSelectionAdapter.")]
        public void SelectionAdapterConstructor()
        {
            SelectorSelectionAdapter adapter = new SelectorSelectionAdapter();
            Assert.IsNull(adapter.SelectorControl, "A selection control was present.");
        }

        /// <summary>
        /// Validate all the standard filters.
        /// </summary>
        [TestMethod]
        [Description("Validate the standard filters.")]
        public void TestSearchFilters()
        {
            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.Contains)("am", "name"));
            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.Contains)("AME", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.Contains)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.ContainsCaseSensitive)("na", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.ContainsCaseSensitive)("AME", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.ContainsCaseSensitive)("hello", "name"));

            Assert.IsNull(GetFilter(AutoCompleteSearchMode.Custom));
            Assert.IsNull(GetFilter(AutoCompleteSearchMode.None));

            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.Equals)("na", "na"));
            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.Equals)("na", "NA"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.Equals)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.EqualsCaseSensitive)("na", "na"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.EqualsCaseSensitive)("na", "NA"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.EqualsCaseSensitive)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.StartsWith)("na", "name"));
            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.StartsWith)("NAM", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.StartsWith)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.StartsWithCaseSensitive)("na", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.StartsWithCaseSensitive)("NAM", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.StartsWithCaseSensitive)("hello", "name"));
        }

        /// <summary>
        /// Validate the ordinal filters.
        /// </summary>
        [TestMethod]
        [Description("Validate the ordinal filters.")]
        public void TestOrdinalSearchFilters()
        {
            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.ContainsOrdinal)("am", "name"));
            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.ContainsOrdinal)("AME", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.ContainsOrdinal)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.ContainsOrdinalCaseSensitive)("na", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.ContainsOrdinalCaseSensitive)("AME", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.ContainsOrdinalCaseSensitive)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.EqualsOrdinal)("na", "na"));
            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.EqualsOrdinal)("na", "NA"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.EqualsOrdinal)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.EqualsOrdinalCaseSensitive)("na", "na"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.EqualsOrdinalCaseSensitive)("na", "NA"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.EqualsOrdinalCaseSensitive)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.StartsWithOrdinal)("na", "name"));
            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.StartsWithOrdinal)("NAM", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.StartsWithOrdinal)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteSearchMode.StartsWithOrdinalCaseSensitive)("na", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.StartsWithOrdinalCaseSensitive)("NAM", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteSearchMode.StartsWithOrdinalCaseSensitive)("hello", "name"));
        }

        /// <summary>
        /// Tests that invalid search mode values throw.
        /// </summary>
        [TestMethod]
        [Description("Tests that invalid search mode values throw.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidSearchMode()
        {
            AutoCompleteBox control = new AutoCompleteBox();
            AutoCompleteSearchMode invalid = (AutoCompleteSearchMode)4321;
            control.SetValue(AutoCompleteBox.SearchModeProperty, invalid);
        }

        /// <summary>
        /// Attach to the standard, non-cancelable drop down events.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "We must check for string.Empty and not null.")]
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Description("Attach to the standard, non-cancelable drop down events.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void DropDownEvents()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool openEvent = false;
            bool closeEvent = false;

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.DropDownOpened += (s, e) => openEvent = true;
            control.DropDownClosed += (s, e) => closeEvent = true;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => Assert.IsTrue(openEvent, "The DropDownOpened event did not fire."));

            EnqueueCallback(() => control.TextBox.Text = string.Empty);
            EnqueueConditional(() => control.Text == string.Empty);

            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));
            EnqueueCallback(() => Assert.IsTrue(closeEvent, "The DropDownClosed event did not fire."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Right before calling PopulateComplete, clear the view.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Description("Right before calling PopulateComplete, clear the view.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void PopulateAndClearView()
        {
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            bool populating = false;
            control.SearchMode = AutoCompleteSearchMode.None;
            control.Populating += (s, e) =>
            {
                e.Cancel = true;
                populating = true;
            };

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "accounti");

            EnqueueConditional(() => control.Text == control.TextBox.Text);

            EnqueueCallback(() => Assert.IsTrue(populating, "The populating event did not fire."));

            EnqueueCallback(() =>
                {
                    // Clear the view
                    OverriddenSelectionAdapter.Current.ItemsSource = null;

                    // Call populate
                    control.PopulateComplete();
                });

            EnqueueCallback(() => Assert.IsNotNull(OverriddenSelectionAdapter.Current.ItemsSource, "The ItemsSource is still null."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Check that IsTextCompletionEnabled through the Text property works.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Description("Check that IsTextCompletionEnabled through the Text property works.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void TextCompletionViaTextProperty()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            control.IsTextCompletionEnabled = true;

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => Assert.AreEqual<string>(string.Empty, control.Text));
            EnqueueCallback(() => control.Text = "close");
            EnqueueCallback(() => Assert.IsNotNull(control.SelectedItem, "The SelectedItem was null. IsTextCompletionEnabled result did not match the item."));
            
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test the IsTextCompletionEnabled selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Description("Test the IsTextCompletionEnabled selection.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void TextCompletionSelection()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            control.IsTextCompletionEnabled = true;

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Focus());
            EnqueueCallback(() => 
                {
                    // Set text and move the caret to the end
                    control.TextBox.Text = "ac";
                    control.TextBox.SelectionStart = 2;
                });
            EnqueueConditional(() => control.SearchText == "ac");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => Assert.IsTrue(control.TextBox.SelectionLength > 2, "The selection length did not increase."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Attach to the TextChanged event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Description("Attach to the TextChanged event.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void TextChangedEvent()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool textChanged = false;

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.TextChanged += (s, e) => textChanged = true;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => Assert.IsTrue(textChanged, "The TextChanged event never fired."));

            EnqueueCallback(() => textChanged = false);
            EnqueueCallback(() => control.Text = "conversati");
            EnqueueConditional(() => textChanged);
            EnqueueCallback(() => Assert.IsTrue(textChanged, "The TextChanged event never fired."));

            EnqueueCallback(() => textChanged = false);
            EnqueueCallback(() => control.Text = null);
            EnqueueConditional(() => textChanged);
            EnqueueCallback(() => Assert.IsTrue(textChanged, "The TextChanged event never fired."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that the minimum prefix length property is working.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "Important test scenario.")]
        [TestMethod]
        [Asynchronous]
        [Description("Verify that the minimum prefix length property is working.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void VerifyMinimumPrefixLength()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.IsTextCompletionEnabled = false;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = string.Empty);
            EnqueueConditional(() => control.Text == string.Empty);

            EnqueueCallback(() => control.MinimumPrefixLength = 3);

            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.Text == "a");
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "acc");
            EnqueueConditional(() => control.Text == "acc");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that the population delay can be set back to 0.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "Important to test for the empty string.")]
        [TestMethod]
        [Description("MinimumPopulateDelayChangeToNull")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Asynchronous]
        public void MinimumPopulateDelayChangeToNull()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.IsTextCompletionEnabled = false;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = string.Empty);
            EnqueueConditional(() => control.Text == string.Empty);

            EnqueueCallback(() => control.MinimumPopulateDelay = 1);

            EnqueueCallback(() =>
            {
                control.TextBox.Text = "acc";
            });
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => control.MinimumPopulateDelay = 0);
            EnqueueCallback(() => control.TextBox.Text = "accou");

            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that the population delay cannot be set to a negative delay.
        /// </summary>
        [TestMethod]
        [Description("Verify that the population delay cannot be set to a negative delay.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [ExpectedException(typeof(ArgumentException))]
        public void MinimumPopulateDelayChangeToNegative()
        {
            AutoCompleteBox ac = (AutoCompleteBox)DefaultControlToTest;
            ac.MinimumPopulateDelay = -100;
        }

        /// <summary>
        /// Verify that the population delay reverts after a negative set.
        /// </summary>
        [TestMethod]
        [Description("Verify that the population delay reverts after a negative set.")]
        public void MinimumPopulateDelayChangeToNegativeReverts()
        {
            AutoCompleteBox ac = (AutoCompleteBox)DefaultControlToTest;
            int currentDelay = ac.MinimumPopulateDelay;
            try
            {
                ac.MinimumPopulateDelay = -100;
            }
            catch (ArgumentException)
            {
            }
            Assert.AreEqual(currentDelay, ac.MinimumPopulateDelay);
        }

        /// <summary>
        /// Verify that the minimum populate delay is being used.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "Important test scenario.")]
        [TestMethod]
        [Asynchronous]
        [Description("Verify that the minimum populate delay is being used.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Bug("535601", Fixed = true)]
        public void VerifyMinimumPopulateDelay()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.IsTextCompletionEnabled = false;
            DateTime started = DateTime.MinValue;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = string.Empty);
            EnqueueConditional(() => control.Text == string.Empty);

            EnqueueCallback(() => control.MinimumPopulateDelay = 500);
            EnqueueCallback(() => Assert.AreEqual(500, control.MinimumPopulateDelay, 0.1, "The MinimumPopulateDelay was not changed to 500ms."));

            EnqueueCallback(() => 
                {
                    control.TextBox.Text = "acc";
                    started = DateTime.Now;
                });
            EnqueueConditional(() => ((TimeSpan)(DateTime.Now - started)).TotalMilliseconds < 500);
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));

            EnqueueConditional(() => ((TimeSpan)(DateTime.Now - started)).TotalMilliseconds > 500);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Set the ItemTemplate.
        /// </summary>
        [TestMethod]
        [Description("Set the ItemTemplate.")]
        public void SetItemTemplate()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.ItemTemplate = new DataTemplate();
            Assert.IsNotNull(ac.ItemTemplate, "The ItemTemplate was not set.");
        }

        /// <summary>
        /// Set the ItemContainerStyle.
        /// </summary>
        [TestMethod]
        [Description("Set the ItemContainerStyle.")]
        public void SetItemContainerStyle()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.ItemContainerStyle = new Style();
            Assert.IsNotNull(ac.ItemContainerStyle, "The ItemContainerStyle was not set.");
        }

        /// <summary>
        /// Initialize a new control with the Text property already set.
        /// </summary>
        [TestMethod]
        [Description("Initialize a new control with the Text property already set.")]
        public void StartingWithText()
        {
            AutoCompleteBox control = new AutoCompleteBox
            {
                Text = "Starting text."
            };
            control.ItemsSource = CreateSimpleStringArray();
        }

        /// <summary>
        /// Set the TextBoxStyle.
        /// </summary>
        [TestMethod]
        [Description("Set the TextBoxStyle.")]
        public void SetTextBoxStyle()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.TextBoxStyle = new Style();
            Assert.IsNotNull(ac.TextBoxStyle, "The TextBoxStyle was not set.");
        }

        /// <summary>
        /// Cancels the drop down opening.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Cancels the drop down opening.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void CancelDropDownOpening()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            control.DropDownOpening += (s, e) => e.Cancel = true;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));
            
            EnqueueTestComplete();
        }

        /// <summary>
        /// Cancels the drop down closing.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Cancels the drop down closing.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void CancelDropDownClosing()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            control.DropDownClosing += (s, e) => e.Cancel = true;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => control.IsDropDownOpen = false);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Cancels the Population event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Description("Cancels the Population event.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void CancelPopulation()
        {
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            bool populating = false;
            control.SearchMode = AutoCompleteSearchMode.None;
            control.Populating += (s, e) =>
                {
                    e.Cancel = true;
                    populating = true;
                };

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "accounti");

            EnqueueConditional(() => control.Text == control.TextBox.Text);

            EnqueueCallback(() => Assert.IsTrue(populating, "The populating event did not fire."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests using the Population event to change the ItemsSource.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests using the Population event to change the ItemsSource.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void Population()
        {
            string custom = "Custom!";
            string search = "accounti";
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            OverriddenSelectionAdapter tsa = null;
            bool populated = false;
            bool populatedOk = false;
            control.SearchMode = AutoCompleteSearchMode.None;
            control.Populating += (s, e) =>
                {
                    control.ItemsSource = new string[] { custom };
                    Assert.AreEqual(search, e.Parameter, "The parameter value was incorrect.");
                };
            control.Populated += (s, e) =>
            {
                populated = true;
                ReadOnlyCollection<object> collection = e.Data as ReadOnlyCollection<object>;
                populatedOk = collection != null && collection.Count == 1;
            };

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = search);
            EnqueueConditional(() => control.IsDropDownOpen);

            EnqueueCallback(() => Assert.IsTrue(populated, "The populated event did not fire."));
            EnqueueCallback(() => Assert.IsTrue(populatedOk, "The populated event data was incorrect."));

            EnqueueCallback(() => tsa = OverriddenSelectionAdapter.Current);
            EnqueueCallback(() => Assert.IsNotNull(tsa, "The testable selection adapter was not found."));
            EnqueueCallback(() => tsa.SelectFirst());
            EnqueueConditional(() => control.SelectedItem != null);
            EnqueueCallback(() => Assert.AreEqual(custom, control.SelectedItem));
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => 
                {
                    tsa.TestCommit();
                });
            EnqueueConditional(() => !control.IsDropDownOpen);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test using a custom adapter.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test using a custom adapter.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void AdapterInXaml()
        {
            string xmlns = " xmlns=\"http://schemas.microsoft.com/client/2007\" " +
                " xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" " +
                " xmlns:testing=\"clr-namespace:System.Windows.Controls.Testing;assembly=System.Windows.Controls.Testing\" " +
                " xmlns:input=\"clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input\" ";
            string xmlChild = @"<Style " + xmlns + @" TargetType=""input:AutoCompleteBox"">
        <Setter Property=""Text"" Value=""Custom..."" />
        <Setter Property=""Template"">
            <Setter.Value>
                <ControlTemplate TargetType=""input:AutoCompleteBox"">
                    <Grid Margin=""{TemplateBinding Padding}"" Background=""{TemplateBinding Background}"">
                        <TextBox IsTabStop=""True"" x:Name=""Text"" Style=""{TemplateBinding TextBoxStyle}"" Margin=""0"" />
                        <ToggleButton x:Name=""DropDownToggle"" Height=""16"" Content=""Toggle DropDown"" />
                        <Popup x:Name=""Popup"">
                            <Border x:Name=""PopupBorder"">
                                    <testing:XamlSelectionAdapter x:Name=""SelectionAdapter"" />
                            </Border>
                        </Popup>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>";
            Style customTemplate = XamlReader.Load(xmlChild) as Style;
            AutoCompleteBox control = new AutoCompleteBox
            {
                Style = customTemplate,
                ItemsSource = CreateSimpleStringArray()
            };

            bool isLoaded = false;
            OverriddenSelectionAdapter.Current = null;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueTestComplete();
        }

        /// <summary>
        /// Test using a custom template with a ToggleButton present.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test using a custom template with a ToggleButton present.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void ToggleButtonPart()
        {
            string xmlns = " xmlns=\"http://schemas.microsoft.com/client/2007\" " +
                " xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" " +
                " xmlns:input=\"clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input\" ";
            string xmlChild = @"<Style " + xmlns + @" TargetType=""input:AutoCompleteBox"">
        <Setter Property=""Text"" Value=""Custom..."" />
        <Setter Property=""Template"">
            <Setter.Value>
                <ControlTemplate TargetType=""input:AutoCompleteBox"">
                    <Grid Margin=""{TemplateBinding Padding}"" Background=""{TemplateBinding Background}"">
                        <TextBox IsTabStop=""True"" x:Name=""Text"" Style=""{TemplateBinding TextBoxStyle}"" Margin=""0"" />
                        <ToggleButton x:Name=""DropDownToggle"" Height=""16"" Content=""Toggle DropDown"" />
                        <Popup x:Name=""Popup"">
                            <Border x:Name=""PopupBorder"">
                                    <ListBox x:Name=""SelectionAdapter"" 
                                            ScrollViewer.HorizontalScrollBarVisibility=""Auto"" 
                                            ScrollViewer.VerticalScrollBarVisibility=""Auto"" 
                                            ItemContainerStyle=""{TemplateBinding ItemContainerStyle}""
                                            ItemTemplate=""{TemplateBinding ItemTemplate}"" />
                            </Border>
                        </Popup>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>";
            Style customTemplate = XamlReader.Load(xmlChild) as Style;
            OverriddenAutoCompleteBox control = new OverriddenAutoCompleteBox
            {
                Style = customTemplate,
                ItemsSource = CreateSimpleStringArray()
            };

            string search = "c";

            bool isLoaded = false;
            OverriddenSelectionAdapter.Current = null;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => Assert.IsNotNull(control.DropDownToggle, "The DropDownToggle part from the custom control template was not found."));
            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = search);
            EnqueueConditional(() => control.IsDropDownOpen);

            EnqueueCallback(() => Assert.IsNotNull(OverriddenSelectionAdapter.Current, "The testable selection adapter was not found."));
            EnqueueCallback(() => OverriddenSelectionAdapter.Current.SelectFirst());
            EnqueueConditional(() => control.SelectedItem != null);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            
            EnqueueCallback(() => OverriddenSelectionAdapter.Current.TestCancel());
            EnqueueConditional(() => !control.IsDropDownOpen);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify the simple observable collection add operation works.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Navigate up and down inside the drop down and validate the selected items.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void ObservableCollectionAdd()
        {
            string search = "Data";
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            ObservableCollection<string> data = new ObservableCollection<string>
            {
                "Data1",
                "Data2",
            };
            control.ItemsSource = data;
            ObservableCollection<object> view = null;
            OverriddenSelectionAdapter tsa = null;

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(
                () => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."),
                () => control.TextBox.Text = search);
            EnqueueConditional(() => control.IsDropDownOpen);

            // Assert the 2 items, then add an item, and assert the new value
            EnqueueCallback(
                () => tsa = OverriddenSelectionAdapter.Current,
                () => Assert.IsNotNull(tsa, "The testable selection adapter was not found."),
                () => view = tsa.ItemsSource as ObservableCollection<object>,
                () => Assert.AreEqual(2, view.Count),
                () => data.Add("Data3"),
                () => Assert.AreEqual(3, view.Count));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Navigate up and down inside the drop down and validate the selected 
        /// items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Navigate up and down inside the drop down and validate the selected items.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void UpDownNavigation()
        {
            string search = "acc";
            bool selectionChanged = false;
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            ObservableCollection<object> view = null;
            OverriddenSelectionAdapter tsa = null;

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.SelectionChanged += (s, e) => selectionChanged = true;
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = search);
            EnqueueConditional(() => control.IsDropDownOpen);

            EnqueueCallback(() => tsa = OverriddenSelectionAdapter.Current);
            EnqueueCallback(() => Assert.IsNotNull(tsa, "The testable selection adapter was not found."));
            EnqueueCallback(() => tsa.SelectFirst());
            EnqueueConditional(() => control.SelectedItem != null);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => tsa.SelectNext());
            EnqueueCallback(() => 
                {
                    view = tsa.ItemsSource as ObservableCollection<object>;
                    Assert.IsNotNull(view, "The ObservableCollection could not be cast.");
                });
            EnqueueCallback(() => Assert.AreEqual(view[1], control.SelectedItem, "The SelectedItem was not the expected value."));
            EnqueueCallback(() => tsa.SelectNext());
            EnqueueCallback(() => Assert.AreEqual(view[2], control.SelectedItem, "The SelectedItem was not the expected value."));
            EnqueueCallback(() => tsa.SelectPrevious());
            EnqueueCallback(() => Assert.AreEqual(view[1], control.SelectedItem, "The SelectedItem was not the expected value."));
            EnqueueCallback(() =>
            {
                tsa.TestCommit();
            });
            EnqueueConditional(() => !control.IsDropDownOpen);
            EnqueueCallback(() => Assert.AreEqual(view[1], control.SelectedItem));
            EnqueueCallback(() => Assert.IsTrue(selectionChanged, "The SelectedItemChanged event handler never fired."));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Cancels the selection adapter and verifies that the text value is 
        /// reverted.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Cancels the selection adapter and verifies that the text value is reverted.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void CancelSelection()
        {
            string custom = "Custom!";
            string search = "accounti";
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            OverriddenSelectionAdapter tsa = null;
            bool populated = false;
            bool populatedOk = false;
            control.SearchMode = AutoCompleteSearchMode.None;
            control.Populating += (s, e) => control.ItemsSource = new string[] { custom };
            control.Populated += (s, e) =>
            {
                populated = true;
                ReadOnlyCollection<object> roc = e.Data as ReadOnlyCollection<object>;
                populatedOk = roc != null && roc.Count == 1;
            };

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = search);
            EnqueueConditional(() => control.IsDropDownOpen);

            EnqueueCallback(() => Assert.IsTrue(populated, "The populated event did not fire."));
            EnqueueCallback(() => Assert.IsTrue(populatedOk, "The populated event data was incorrect."));

            EnqueueCallback(() => tsa = OverriddenSelectionAdapter.Current);
            EnqueueCallback(() => Assert.IsNotNull(tsa, "The testable selection adapter was not found."));
            EnqueueCallback(() => tsa.SelectFirst());
            EnqueueConditional(() => control.SelectedItem != null);
            EnqueueCallback(() => Assert.AreEqual(custom, control.SelectedItem));
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => tsa.TestCancel());
            EnqueueConditional(() => !control.IsDropDownOpen);
            EnqueueCallback(() => Assert.AreEqual(search, control.TextBox.Text, "The original value was not restored in the text box"));
            EnqueueCallback(() => Assert.AreEqual(search, control.Text, "The original value was not restored in the text property"));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Gain coverage by moving the focus from the control to another 
        /// control in the test panel.
        /// </summary>
        [TestMethod]
        [Description("Gain coverage by moving the focus from the control to another control in the test panel.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Tag(Tags.RequiresFocus)]
        [Asynchronous]
        public void LostFocus()
        {
            // TODO: Evaluate improvements to remove the Sleep.

            Button button = new Button { Content = "This is a Button" };
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            TestPanel.Children.Add(button);

            bool lostFocus = false;
            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.LostFocus += (s, e) => lostFocus = true;
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.Focus());
            EnqueueDelay(TimeSpan.FromSeconds(0.1));
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));
            EnqueueDelay(TimeSpan.FromSeconds(0.1));
            EnqueueCallback(() => button.Focus());
            EnqueueDelay(TimeSpan.FromSeconds(0.1));
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));
            EnqueueCallback(() => Assert.IsTrue(lostFocus, "Focus was not lost. The Silverlight host may not have had focus itself."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that focus being lost with the drop down open will close it.
        /// </summary>
        [TestMethod]
        [Description("Verify that focus being lost with the drop down open will close it")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Tag(Tags.RequiresFocus)]
        [Asynchronous]
        public void LostFocusWithDropDown()
        {
            // TODO: Evaluate improvements to remove the Sleep.

            Button button = new Button { Content = "This is a Button" };
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            TestPanel.Children.Add(button);

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.Focus());
            EnqueueCallback(() => control.TextBox.Text = "accounti");
            EnqueueConditional(() => control.SearchText == "accounti");
            EnqueueCallback(() => StringAssert.Equals(control.TextBox.Text, "accounting"));
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => button.Focus());
            
            // This will prevent a timeout
            EnqueueDelay(TimeSpan.FromSeconds(0.1));
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen, "The drop down did not close. Please check that the focus did change."));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test changing the minimum prefix length.
        /// </summary>
        [TestMethod]
        [Description("Test changing the minimum prefix length.")]
        public void ChangeMinimumPrefixLength()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.MinimumPrefixLength = 10;
            Assert.AreEqual(10, ac.MinimumPrefixLength);
            ac.MinimumPrefixLength = 1;
            Assert.AreEqual(1, ac.MinimumPrefixLength);
        }

        /// <summary>
        /// Test changing the minimum prefix length to a large negative value.
        /// </summary>
        [TestMethod]
        [Description("Test changing the minimum prefix length to a large negative value.")]
        public void ChangeMinimumPrefixLengthCoerce()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.MinimumPrefixLength = 10;
            Assert.AreEqual(10, ac.MinimumPrefixLength);
            
            // Validate that the value was coerced to -1
            ac.MinimumPrefixLength = -99;
            Assert.AreEqual(-1, ac.MinimumPrefixLength);
        }

        /// <summary>
        /// Change the maximum drop down height dependency property.
        /// </summary>
        [TestMethod]
        [Description("Change the maximum drop down height dependency property.")]
        public void ChangeMaxDropDownHeight()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.MaxDropDownHeight = 60;
            Assert.AreEqual(60, ac.MaxDropDownHeight);
        }

        /// <summary>
        /// Change the maximum drop down height dependency property with an 
        /// invalid value.
        /// </summary>
        [TestMethod]
        [Description("Change the maximum drop down height dependency property with an invalid value.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [ExpectedException(typeof(ArgumentException))]
        public void ChangeMaxDropDownHeightInvalid()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.MaxDropDownHeight = -10;
        }

        /// <summary>
        /// Validate that the IsTextCompletionEnabled property is updated in 
        /// the standard AutoCompleteBox scenario.
        /// </summary>
        [TestMethod]
        [Description("Validate that the IsTextCompletionEnabled property is updated in the standard AutoCompleteBox scenario.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Asynchronous]
        public void TextCompletionValidation()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "accounti");
            EnqueueConditional(() => control.SearchText == "accounti");
            EnqueueCallback(() => StringAssert.Equals(control.TextBox.Text, "accounting"));
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueTestComplete();
        }

        /// <summary>
        /// A typical string search scenario.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Functional test.")]
        [TestMethod]
        [Asynchronous]
        [Description("A typical string search scenario.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void StringSearch()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            // Add the element to the test surface and wait until it's loaded
            
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "acc");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "zoo");
            EnqueueConditional(() => control.Text == control.TextBox.Text);

            EnqueueCallback(() => control.TextBox.Text = "accept");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "cook");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            // Remove the element from the test surface and finish the test
            EnqueueCallback(() => TestPanel.Children.Remove(control));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the custom ItemFilter workflow.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Functional test scenario.")]
        [TestMethod]
        [Asynchronous]
        [Description("Tests the custom ItemFilter workflow.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void ItemSearch()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            control.SearchMode = AutoCompleteSearchMode.Custom;
            control.ItemFilter = (search, item) =>
                {
                    string s = item as string;
                    return s == null ? false : true;
                };

            // Just set to null briefly to exercise that code path
            AutoCompleteSearchPredicate<object> filter = control.ItemFilter;
            Assert.IsNotNull(filter, "The ItemFilter was null.");
            control.ItemFilter = null;
            Assert.IsNull(control.ItemFilter, "The ItemFilter should be null.");
            control.ItemFilter = filter;
            Assert.IsNotNull(control.ItemFilter, "The ItemFilter was null.");

            // Add the element to the test surface and wait until it's loaded

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "acc");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "zoo");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            
            EnqueueCallback(() => control.TextBox.Text = "accept");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "cook");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            // Remove the element from the test surface and finish the test
            EnqueueCallback(() => TestPanel.Children.Remove(control));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that the ItemsSource changed handler works.
        /// </summary>
        [TestMethod]
        [Description("Verify that the ItemsSource changed handler works.")]
        public void ChangeItemsSource()
        {
            AutoCompleteBox ac = CreateSimpleStringAutoComplete();
            ac.ItemsSource = null;
            ac.ItemsSource = new List<object> { DateTime.Now, "hello", Guid.NewGuid() };
            ac.ItemsSource = CreateSimpleStringArray();
        }

        /// <summary>
        /// Validate the the SearchText property is read only.
        /// </summary>
        [TestMethod]
        [Description("Validate the the SearchText property is read only.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Asynchronous]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlySearchText()
        {
            AutoCompleteBox ac = CreateSimpleStringAutoComplete();
            TestAsync(
                ac,
                () => ac.SetValue(AutoCompleteBox.SearchTextProperty, "a"));
        }

        /// <summary>
        /// Validate that the SelectedItem property is no longer read only.
        /// </summary>
        /// <remarks>This was a functional change after the December 2008
        /// Silverlight Toolkit release.</remarks>
        [TestMethod]
        [Description("Validate that the SelectedItem property is no longer read only.")]
        [Bug("568174 - AutoCompleteBox - Make SelectedItem property settable", Fixed = true)]
        public void SetSelectedItem()
        {
            AutoCompleteBox ac = CreateSimpleStringAutoComplete();
            ac.SetValue(AutoCompleteBox.SelectedItemProperty, "a");
        }

        #region control contract
        /// <summary>
        /// Verifies the Control's TemplateParts.
        /// </summary>
        [TestMethod]
        [Description("Verifies the Control's TemplateParts.")]
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> templateParts = DefaultControlToTest.GetType().GetTemplateParts();
            Assert.AreEqual(4, templateParts.Count);
            Assert.AreSame(typeof(Popup), templateParts["Popup"]);
            Assert.AreSame(typeof(TextBox), templateParts["Text"]);

            // By default:
            // - Selection adapter is wrapped at runtime
            // Assert.AreSame(typeof(ISelectionAdapter), templateParts["SelectionAdapter"]);

            // The toggle is not in the default control template:
            // Assert.AreSame(typeof(ToggleButton), templateParts["DropDownToggle"]);
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> visualStates = DefaultControlToTest.GetType().GetVisualStates();
            Assert.AreEqual(11, visualStates.Count);

            Assert.AreEqual<string>("CommonStates", visualStates["Normal"]);
            Assert.AreEqual<string>("CommonStates", visualStates["MouseOver"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Pressed"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Disabled"]);

            Assert.AreEqual<string>("FocusStates", visualStates["Focused"]);
            Assert.AreEqual<string>("FocusStates", visualStates["Unfocused"]);

            Assert.AreEqual<string>("ValidationStates", visualStates["Valid"]);
            Assert.AreEqual<string>("ValidationStates", visualStates["InvalidFocused"]);
            Assert.AreEqual<string>("ValidationStates", visualStates["InvalidUnfocused"]);

            // + Popups
        }

        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(2, properties.Count, "Incorrect number of style typed property attributes!");
            Assert.AreEqual(typeof(TextBox), properties["TextStyle"], "Failed to find expected style type property TextStyle!");
            Assert.AreEqual(typeof(ListBox), properties["ContainerStyle"], "Failed to find expected style type property ContainerStyle!");
        }
        #endregion

        #region Sample data
        
        /// <summary>
        /// Creates a large list of strings for AutoCompleteBox testing.
        /// </summary>
        /// <returns>Returns a new List of string values.</returns>
        protected static IList<string> CreateSimpleStringArray()
        {
            return new List<string>
            {
            "a", 
            "abide", 
            "able", 
            "about", 
            "above", 
            "absence", 
            "absurd", 
            "accept", 
            "acceptance", 
            "accepted", 
            "accepting", 
            "access", 
            "accessed", 
            "accessible", 
            "accident", 
            "accidentally", 
            "accordance", 
            "account", 
            "accounting", 
            "accounts", 
            "accusation", 
            "accustomed", 
            "ache", 
            "across", 
            "act", 
            "active", 
            "actual", 
            "actually", 
            "ada", 
            "added", 
            "adding", 
            "addition", 
            "additional", 
            "additions", 
            "address", 
            "addressed", 
            "addresses", 
            "addressing", 
            "adjourn", 
            "adoption", 
            "advance", 
            "advantage", 
            "adventures", 
            "advice", 
            "advisable", 
            "advise", 
            "affair", 
            "affectionately", 
            "afford", 
            "afore", 
            "afraid", 
            "after", 
            "afterwards", 
            "again", 
            "against", 
            "age", 
            "aged", 
            "agent", 
            "ago", 
            "agony", 
            "agree", 
            "agreed", 
            "agreement", 
            "ah", 
            "ahem", 
            "air", 
            "airs", 
            "ak", 
            "alarm", 
            "alarmed", 
            "alas", 
            "alice", 
            "alive", 
            "all", 
            "allow", 
            "almost", 
            "alone", 
            "along", 
            "aloud", 
            "already", 
            "also", 
            "alteration", 
            "altered", 
            "alternate", 
            "alternately", 
            "altogether", 
            "always", 
            "am", 
            "ambition", 
            "among", 
            "an", 
            "ancient", 
            "and", 
            "anger", 
            "angrily", 
            "angry", 
            "animal", 
            "animals", 
            "ann", 
            "annoy", 
            "annoyed", 
            "another", 
            "answer", 
            "answered", 
            "answers", 
            "antipathies", 
            "anxious", 
            "anxiously", 
            "any", 
            "anyone", 
            "anything", 
            "anywhere", 
            "appealed", 
            "appear", 
            "appearance", 
            "appeared", 
            "appearing", 
            "appears", 
            "applause", 
            "apple", 
            "apples", 
            "applicable", 
            "apply", 
            "approach", 
            "arch", 
            "archbishop", 
            "arches", 
            "archive", 
            "are", 
            "argue", 
            "argued", 
            "argument", 
            "arguments", 
            "arise", 
            "arithmetic", 
            "arm", 
            "arms", 
            "around", 
            "arranged", 
            "array", 
            "arrived", 
            "arrow", 
            "arrum", 
            "as", 
            "ascii", 
            "ashamed", 
            "ask", 
            "askance", 
            "asked", 
            "asking", 
            "asleep", 
            "assembled", 
            "assistance", 
            "associated", 
            "at", 
            "ate", 
            "atheling", 
            "atom", 
            "attached", 
            "attempt", 
            "attempted", 
            "attempts", 
            "attended", 
            "attending", 
            "attends", 
            "audibly", 
            "australia", 
            "author", 
            "authority", 
            "available", 
            "avoid", 
            "away", 
            "awfully", 
            "axes", 
            "axis", 
            "b", 
            "baby", 
            "back", 
            "backs", 
            "bad", 
            "bag", 
            "baked", 
            "balanced", 
            "bank", 
            "banks", 
            "banquet", 
            "bark", 
            "barking", 
            "barley", 
            "barrowful", 
            "based", 
            "bat", 
            "bathing", 
            "bats", 
            "bawled", 
            "be", 
            "beak", 
            "bear", 
            "beast", 
            "beasts", 
            "beat", 
            "beating", 
            "beau", 
            "beauti", 
            "beautiful", 
            "beautifully", 
            "beautify", 
            "became", 
            "because", 
            "become", 
            "becoming", 
            "bed", 
            "beds", 
            "bee", 
            "been", 
            "before", 
            "beg", 
            "began", 
            "begged", 
            "begin", 
            "beginning", 
            "begins", 
            "begun", 
            "behead", 
            "beheaded", 
            "beheading", 
            "behind", 
            "being", 
            "believe", 
            "believed", 
            "bells", 
            "belong", 
            "belongs", 
            "beloved", 
            "below", 
            "belt", 
            "bend", 
            "bent", 
            "besides", 
            "best", 
            "better", 
            "between", 
            "bill", 
            "binary", 
            "bird", 
            "birds", 
            "birthday", 
            "bit", 
            "bite", 
            "bitter", 
            "blacking", 
            "blades", 
            "blame", 
            "blasts", 
            "bleeds", 
            "blew", 
            "blow", 
            "blown", 
            "blows", 
            "body", 
            "boldly", 
            "bone", 
            "bones", 
            "book", 
            "books", 
            "boon", 
            "boots", 
            "bore", 
            "both", 
            "bother", 
            "bottle", 
            "bottom", 
            "bough", 
            "bound", 
            "bowed", 
            "bowing", 
            "box", 
            "boxed", 
            "boy", 
            "brain", 
            "branch", 
            "branches", 
            "brandy", 
            "brass", 
            "brave", 
            "breach", 
            "bread", 
            "break", 
            "breath", 
            "breathe", 
            "breeze", 
            "bright", 
            "brightened", 
            "bring", 
            "bringing", 
            "bristling", 
            "broke", 
            "broken", 
            "brother", 
            "brought", 
            "brown", 
            "brush", 
            "brushing", 
            "burn", 
            "burning", 
            "burnt", 
            "burst", 
            "bursting", 
            "busily", 
            "business", 
            "business@pglaf", 
            "busy", 
            "but", 
            "butter", 
            "buttercup", 
            "buttered", 
            "butterfly", 
            "buttons", 
            "by", 
            "bye", 
            "c", 
            "cackled", 
            "cake", 
            "cakes", 
            "calculate", 
            "calculated", 
            "call", 
            "called", 
            "calling", 
            "calmly", 
            "came", 
            "camomile", 
            "can", 
            "canary", 
            "candle", 
            "cannot", 
            "canterbury", 
            "canvas", 
            "capering", 
            "capital", 
            "card", 
            "cardboard", 
            "cards", 
            "care", 
            "carefully", 
            "cares", 
            "carried", 
            "carrier", 
            "carroll", 
            "carry", 
            "carrying", 
            "cart", 
            "cartwheels", 
            "case", 
            "cat", 
            "catch", 
            "catching", 
            "caterpillar", 
            "cats", 
            "cattle", 
            "caucus", 
            "caught", 
            "cauldron", 
            "cause", 
            "caused", 
            "cautiously", 
            "cease", 
            "ceiling", 
            "centre", 
            "certain", 
            "certainly", 
            "chain", 
            "chains", 
            "chair", 
            "chance", 
            "chanced", 
            "change", 
            "changed", 
            "changes", 
            "changing", 
            "chapter", 
            "character", 
            "charge", 
            "charges", 
            "charitable", 
            "charities", 
            "chatte", 
            "cheap", 
            "cheated", 
            "check", 
            "checked", 
            "checks", 
            "cheeks", 
            "cheered", 
            "cheerfully", 
            "cherry", 
            "cheshire", 
            "chief", 
            "child", 
            "childhood", 
            "children", 
            "chimney", 
            "chimneys", 
            "chin", 
            "choice", 
            "choke", 
            "choked", 
            "choking", 
            "choose", 
            "choosing", 
            "chop", 
            "chorus", 
            "chose", 
            "christmas", 
            "chrysalis", 
            "chuckled", 
            "circle", 
            "circumstances", 
            "city", 
            "civil", 
            "claim", 
            "clamour", 
            "clapping", 
            "clasped", 
            "classics", 
            "claws", 
            "clean", 
            "clear", 
            "cleared", 
            "clearer", 
            "clearly", 
            "clever", 
            "climb", 
            "clinging", 
            "clock", 
            "close", 
            "closed", 
            "closely", 
            "closer", 
            "clubs", 
            "coast", 
            "coaxing", 
            "codes", 
            "coils", 
            "cold", 
            "collar", 
            "collected", 
            "collection", 
            "come", 
            "comes", 
            "comfits", 
            "comfort", 
            "comfortable", 
            "comfortably", 
            "coming", 
            "commercial", 
            "committed", 
            "common", 
            "commotion", 
            "company", 
            "compilation", 
            "complained", 
            "complaining", 
            "completely", 
            "compliance", 
            "comply", 
            "complying", 
            "compressed", 
            "computer", 
            "computers", 
            "concept", 
            "concerning", 
            "concert", 
            "concluded", 
            "conclusion", 
            "condemn", 
            "conduct", 
            "confirmation", 
            "confirmed", 
            "confused", 
            "confusing", 
            "confusion", 
            "conger", 
            "conqueror", 
            "conquest", 
            "consented", 
            "consequential", 
            "consider", 
            "considerable", 
            "considered", 
            "considering", 
            "constant", 
            "consultation", 
            "contact", 
            "contain", 
            "containing", 
            "contempt", 
            "contemptuous", 
            "contemptuously", 
            "content", 
            "continued", 
            "contract", 
            "contradicted", 
            "contributions", 
            "conversation", 
            "conversations", 
            "convert", 
            "cook", 
            "cool", 
            "copied", 
            "copies", 
            "copy", 
            "copying", 
            "copyright", 
            "corner", 
            "corners", 
            "corporation", 
            "corrupt", 
            "cost", 
            "costs", 
            "could", 
            "couldn", 
            "counting", 
            "countries", 
            "country", 
            "couple", 
            "couples", 
            "courage", 
            "course", 
            "court", 
            "courtiers", 
            "coward", 
            "crab", 
            "crash", 
            "crashed", 
            "crawled", 
            "crawling", 
            "crazy", 
            "created", 
            "creating", 
            "creation", 
            "creature", 
            "creatures", 
            "credit", 
            "creep", 
            "crept", 
            "cried", 
            "cries", 
            "crimson", 
            "critical", 
            "crocodile", 
            "croquet", 
            "croqueted", 
            "croqueting", 
            "cross", 
            "crossed", 
            "crossly", 
            "crouched", 
            "crowd", 
            "crowded", 
            "crown", 
            "crumbs", 
            "crust", 
            "cry", 
            "crying", 
            "cucumber", 
            "cunning", 
            "cup", 
            "cupboards", 
            "cur", 
            "curiosity", 
            "curious", 
            "curiouser", 
            "curled", 
            "curls", 
            "curly", 
            "currants", 
            "current", 
            "curtain", 
            "curtsey", 
            "curtseying", 
            "curving", 
            "cushion", 
            "custard", 
            "custody", 
            "cut", 
            "cutting", 
            };
        }
#endregion
    }
}