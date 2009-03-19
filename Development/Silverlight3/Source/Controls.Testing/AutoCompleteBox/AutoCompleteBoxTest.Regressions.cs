// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Regression tests for AutoCompleteBox.
    /// </summary>
    public partial class AutoCompleteBoxTest
    {
        /// <summary>
        /// Test that if the Basic style inherited from Control(Background,BorderBrush,BorderThickness,etc) work properly.
        /// </summary>
        [TestMethod]
        [Description("Test that if the Basic style inherited from Control(Background,BorderBrush,BorderThickness,etc) work properly.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Asynchronous]
        [Bug("530456 - AutoComplete: some properties inherit from Control (Background,BorderBrush,BorderThickness,etc) don't work", Fixed = true)]
        public void TestAutoCompleteBoxBasicStyle()
        {
            AutoCompleteBox autoComplete = new AutoCompleteBox();

            SolidColorBrush background = new SolidColorBrush(Colors.Green);
            SolidColorBrush borderBrush = new SolidColorBrush(Colors.Orange);
            Thickness borderThickness = new Thickness(1);
            Thickness padding = new Thickness(2);

            TestAsync(
                autoComplete,
                () => autoComplete.Background = background,
                () => autoComplete.BorderBrush = borderBrush,
                () => autoComplete.BorderThickness = borderThickness,
                () => autoComplete.Padding = padding,
                () => Assert.AreEqual(Colors.Green, (autoComplete.Background as SolidColorBrush).Color),
                () => Assert.AreEqual(Colors.Orange, (autoComplete.BorderBrush as SolidColorBrush).Color),
                () => Assert.AreEqual(borderThickness, autoComplete.BorderThickness),
                () => Assert.AreEqual(padding, autoComplete.Padding));
        }

        /// <summary>
        /// Update the Text property and validate that the SelectedItem is also 
        /// updated.
        /// </summary>
        [TestMethod]
        [Priority(0)]
        [Bug("531153 - SelectionChanged doesn't fire when entering text matching item in list", Fixed = true)]
        [Bug("556120 - AutoCompleteBox: DCR: Text updates should always update SelectedItem, if present in Items", Fixed = true)]
        [Description("Update the Text property and validate that the SelectedItem is also updated.")]
        public void UpdatedTextUpdatesSelectedItem()
        {
            bool selectionChanged = false;
            string value = "String1";
            AutoCompleteBox acb = new AutoCompleteBox();
            acb.SelectionChanged += (e, o) => selectionChanged = true;
            acb.ItemsSource = new List<string> { value, "String2" };
            acb.Text = value;
            Assert.AreEqual(value, acb.SelectedItem, "The SelectedItem value does not equal the matched item value.");
            Assert.IsTrue(selectionChanged, "The SelectionChanged event was not fired.");
        }

        /// <summary>
        /// Update the Text property and validate that TextChanged fires.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Bug("530455 - Autocomplete: TextChanged event doesn't work", Fixed = true)]
        [Bug("Silverlight 29497", Fixed = true)]
        [Description("Update the Text property and validate that TextChanged fires.")]
        public void UpdatedTextFiresTextChanged()
        {
            bool selectionChanged = false;
            bool textChanged = false;
            bool isLoaded = false;
            string value = "String1";
            AutoCompleteBox acb = new AutoCompleteBox();
            acb.TextChanged += (e, o) => textChanged = true;
            acb.SelectionChanged += (e, o) => selectionChanged = true;
            acb.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(acb));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(
                () => acb.Text = "Hello world",
                () => acb.ItemsSource = new List<string> { value, "String2" },
                () => acb.Text = value,
                () => Assert.AreEqual(value, acb.SelectedItem, "The SelectedItem value does not equal the matched item value."),
                () => Assert.IsTrue(selectionChanged, "The SelectionChanged event was not fired."));

            // Effectively an assert: test will run forever if this is not fired
            EnqueueConditional(() => textChanged);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Cancel the selection adapter and verify that the selecteditem
        /// reverts.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Bug("556122 - AutoComplete: Cancel the selection adapter and verify that the selecteditem reverts.", Fixed = true)]
        [Bug("Silverlight 29497", Fixed = true)]
        [Description("Cancel the selection adapter and verify that the selecteditem reverts.")]
        public void CancelAndRevert()
        {
            string search = "Data1";
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
                () => control.SelectedItem = search,
                () => control.TextBox.Text = "Dat");
            EnqueueConditional(() => control.IsDropDownOpen);

            // Assert the 2 items, then add an item, and assert the new value
            EnqueueCallback(
                () => tsa = OverriddenSelectionAdapter.Current,
                () => Assert.IsNotNull(tsa, "The testable selection adapter was not found."),
                () => view = tsa.ItemsSource as ObservableCollection<object>,
                () => Assert.AreEqual(2, view.Count),
                () => tsa.SelectFirst(),
                () => tsa.SelectNext(), // control.TextBox.Text = "Data2",
                () => tsa.TestCancel());

            // Wait for the drop down to close
            EnqueueConditional(() => control.IsDropDownOpen == false);

            // Assert the revert
            EnqueueCallback(() => Assert.AreEqual("Dat", control.Text, "The text was not reverted to the search text value."));

            EnqueueTestComplete();
        }
    }
}
