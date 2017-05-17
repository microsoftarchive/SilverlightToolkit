// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// ItemsControl unit tests.
    /// </summary>
    public abstract partial class ItemsControlTest : ControlTest
    {
        #region Controls to test
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get
            {
                ItemsControl control = DefaultItemsControlToTest;
                control.ItemsSource = new string[] { "Test Item 1", "Test Item 2", "Test Item 3" };
                return control;
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
                    ItemsControlsToTest.OfType<Control>()
                    .Concat(ItemsControlsToTest.Select(
                        control =>
                        {
                            control.ItemsSource = new string[] { "Test Item 1", "Test Item 2", "Test Item 3" };
                            return (Control) control;
                        }));
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { return OverriddenItemsControlsToTest.OfType<IOverriddenControl>(); }
        }
        #endregion Controls to test

        #region ItemsControls to test
        /// <summary>
        /// Gets a default instance of ItemsControl (or a derived type) to test.
        /// </summary>
        public abstract ItemsControl DefaultItemsControlToTest { get; }

        /// <summary>
        /// Gets instances of ItemsControl (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<ItemsControl> ItemsControlsToTest { get; }

        /// <summary>
        /// Gets instances of IOverriddenItemsControl (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<IOverriddenItemsControl> OverriddenItemsControlsToTest { get; }
        #endregion ItemsControls to test

        /// <summary>
        /// Gets the DisplayMemberPath dependency property test.
        /// </summary>
        protected DependencyPropertyTest<ItemsControl, string> DisplayMemberPathProperty { get; private set; }

        /// <summary>
        /// Gets the ItemsPanel dependency property test.
        /// </summary>
        protected DependencyPropertyTest<ItemsControl, ItemsPanelTemplate> ItemsPanelProperty { get; private set; }

        /// <summary>
        /// Gets the ItemsSource dependency property test.
        /// </summary>
        protected DependencyPropertyTest<ItemsControl, IEnumerable> ItemsSourceProperty { get; private set; }

        /// <summary>
        /// Gets the ItemTemplate dependency property test.
        /// </summary>
        protected DependencyPropertyTest<ItemsControl, DataTemplate> ItemTemplateProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ItemsControlTest class.
        /// </summary>
        protected ItemsControlTest()
            : base()
        {
            Func<ItemsControl> initializer = () => DefaultItemsControlToTest;
            DisplayMemberPathProperty = new DependencyPropertyTest<ItemsControl, string>(this, "DisplayMemberPath")
                {
                    Property = ItemsControl.DisplayMemberPathProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new string[] { "Value" }
                };
            ItemsPanelProperty = new DependencyPropertyTest<ItemsControl, ItemsPanelTemplate>(this, "ItemsPanel")
                {
                    Property = ItemsControl.ItemsPanelProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new ItemsPanelTemplate[] { new ItemsPanelTemplate() }
                };
            ItemsSourceProperty = new DependencyPropertyTest<ItemsControl, IEnumerable>(this, "ItemsSource")
                {
                    Property = ItemsControl.ItemsSourceProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new IEnumerable[] { new int[] { 1, 2, 3 }, new string[] { "hello", "world" } }
                };
            ItemTemplateProperty = new DependencyPropertyTest<ItemsControl, DataTemplate>(this, "ItemTemplate")
                {
                    Property = ItemsControl.ItemTemplateProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new DataTemplate[] { new DataTemplate() }
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

            // DisplayMemberPathProperty tests
            tests.Add(DisplayMemberPathProperty.BindingTest);
            tests.Add(DisplayMemberPathProperty.CheckDefaultValueTest);
            tests.Add(DisplayMemberPathProperty.ChangeClrSetterTest);
            tests.Add(DisplayMemberPathProperty.ChangeSetValueTest);
            tests.Add(DisplayMemberPathProperty.SetNullTest);
            tests.Add(DisplayMemberPathProperty.ClearValueResetsDefaultTest);
            tests.Add(DisplayMemberPathProperty.CanBeStyledTest);
            tests.Add(DisplayMemberPathProperty.TemplateBindTest);
            tests.Add(DisplayMemberPathProperty.SetXamlAttributeTest);
            tests.Add(DisplayMemberPathProperty.SetXamlElementTest);

            // ItemsPanelProperty tests
            tests.Add(ItemsPanelProperty.BindingTest);
            tests.Add(ItemsPanelProperty.CheckDefaultValueTest);
            tests.Add(ItemsPanelProperty.ChangeClrSetterTest);
            tests.Add(ItemsPanelProperty.ChangeSetValueTest);
            tests.Add(ItemsPanelProperty.SetNullTest);
            tests.Add(ItemsPanelProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemsPanelProperty.CanBeStyledTest);
            tests.Add(ItemsPanelProperty.TemplateBindTest);
            tests.Add(ItemsPanelProperty.SetXamlAttributeTest.Bug("XamlConverter cannot serialize"));
            tests.Add(ItemsPanelProperty.SetXamlElementTest.Bug("XamlConverter cannot serialize"));

            // ItemsSourceProperty tests
            tests.Add(ItemsSourceProperty.BindingTest);
            tests.Add(ItemsSourceProperty.CheckDefaultValueTest);
            tests.Add(ItemsSourceProperty.ChangeClrSetterTest);
            tests.Add(ItemsSourceProperty.ChangeSetValueTest);
            tests.Add(ItemsSourceProperty.SetNullTest);
            tests.Add(ItemsSourceProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemsSourceProperty.CanBeStyledTest.Bug("TODO: Why is this failing?"));
            tests.Add(ItemsSourceProperty.TemplateBindTest);
            tests.Add(ItemsSourceProperty.SetXamlAttributeTest);
            tests.Add(ItemsSourceProperty.SetXamlElementTest);

            // ItemTemplateProperty tests
            tests.Add(ItemTemplateProperty.BindingTest);
            tests.Add(ItemTemplateProperty.CheckDefaultValueTest);
            tests.Add(ItemTemplateProperty.ChangeClrSetterTest);
            tests.Add(ItemTemplateProperty.ChangeSetValueTest);
            tests.Add(ItemTemplateProperty.SetNullTest);
            tests.Add(ItemTemplateProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemTemplateProperty.CanBeStyledTest);
            tests.Add(ItemTemplateProperty.TemplateBindTest);
            tests.Add(ItemTemplateProperty.SetXamlAttributeTest.Bug("XamlConverter cannot serialize"));
            tests.Add(ItemTemplateProperty.SetXamlElementTest.Bug("XamlConverter cannot serialize"));

            return tests;
        }
    }
}