// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Overridden TreeView that provides access to virtual members for testing.
    /// </summary>
    public partial class OverriddenTreeView : TreeView, IOverriddenTreeView
    {
        /// <summary>
        /// Initializes a new instance of the OverriddenTreeView class.
        /// </summary>
        public OverriddenTreeView()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OverriddenTreeView class.
        /// </summary>
        /// <param name="constructorTest">
        /// Test actions to perform after the TreeView constructor.
        /// </param>
        public OverriddenTreeView(Action constructorTest)
        {
            Action invariantTest = () => AssertInvariants();
            SelectedItemChangedActions = new OverriddenMethod<RoutedPropertyChangedEventArgs<object>>(invariantTest);
            GetContainerForItemOverrideActions = new OverriddenMethod<DependencyObject>(invariantTest);
            IsItemItsOwnContainerOverrideActions = new OverriddenMethod<object, bool?>(invariantTest);
            PrepareContainerForItemOverrideActions = new OverriddenMethod<DependencyObject, object>(invariantTest);
            ClearContainerForItemOverrideActions = new OverriddenMethod<DependencyObject, object>(invariantTest);
            OnItemsChangedActions = new OverriddenMethod<NotifyCollectionChangedEventArgs>(invariantTest);
            GotFocusActions = new OverriddenMethod<RoutedEventArgs>(invariantTest);
            LostFocusActions = new OverriddenMethod<RoutedEventArgs>(invariantTest);
            KeyDownActions = new OverriddenMethod<KeyEventArgs>(invariantTest);
            KeyUpActions = new OverriddenMethod<KeyEventArgs>(invariantTest);
            MouseEnterActions = new OverriddenMethod<MouseEventArgs>(invariantTest);
            MouseLeaveActions = new OverriddenMethod<MouseEventArgs>(invariantTest);
            MouseMoveActions = new OverriddenMethod<MouseEventArgs>(invariantTest);
            MouseLeftButtonDownActions = new OverriddenMethod<MouseButtonEventArgs>(invariantTest);
            MouseLeftButtonUpActions = new OverriddenMethod<MouseButtonEventArgs>(invariantTest);
            ApplyTemplateActions = new OverriddenMethod(invariantTest);
            CreateAutomationPeerActions = new OverriddenMethod<AutomationPeer>(invariantTest);
            MeasureActions = new OverriddenMethod<Size, Size?>(invariantTest);
            ArrangeActions = new OverriddenMethod<Size, Size?>(invariantTest);

            AssertInvariants();
            if (constructorTest != null)
            {
                constructorTest();
            }
        }

        /// <summary>
        /// Ensure that TreeView invariants are satisfied.
        /// </summary>
        public void AssertInvariants()
        {
            if (SelectedItem == null)
            {
                Assert.IsNull(SelectedValue, "SelectedValue should be null when SelectedItem is null!");
            }
            if (string.IsNullOrEmpty(SelectedValuePath))
            {
                Assert.AreEqual(SelectedItem, SelectedValue, "SelectedValue should equal SelectedItem when there is no SelectedValuePath");
            }
        }

        /// <summary>
        /// Gets test actions for the OnSelectedItemChanged method.
        /// </summary>
        public OverriddenMethod<RoutedPropertyChangedEventArgs<object>> SelectedItemChangedActions { get; private set; }

        /// <summary>
        /// Raises the SelectedItemChanged event when the SelectedItem property
        /// value changes.
        /// </summary>
        /// <param name="e">
        /// Provides the item that was previously selected and the item that is
        /// currently selected for the SelectedItemChanged event.
        /// </param>
        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItemChangedActions.DoPreTest(e);
            base.OnSelectedItemChanged(e);
            SelectedItemChangedActions.DoTest(e);
        }

        /// <summary>
        /// Gets test actions for the GetContainerForItemOverride method.
        /// </summary>
        public OverriddenMethod<DependencyObject> GetContainerForItemOverrideActions { get; private set; }

        /// <summary>
        /// Creates a TreeViewItem to use to display content.
        /// </summary>
        /// <returns>
        /// A new TreeViewItem to use as a container for content.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            GetContainerForItemOverrideActions.DoPreTest(null);
            DependencyObject obj = base.GetContainerForItemOverride();
            GetContainerForItemOverrideActions.DoTest(obj);
            return obj;
        }

        /// <summary>
        /// Gets test actions for the IsItemItsOwnContainerOverride method.
        /// </summary>
        public OverriddenMethod<object, bool?> IsItemItsOwnContainerOverrideActions { get; private set; }

        /// <summary>
        /// Determines whether the specified item is its own container or can be
        /// its own container.
        /// </summary>
        /// <param name="item">The object to evaluate.</param>
        /// <returns>
        /// A value indicating whether the item is a TreeViewItem or not.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            IsItemItsOwnContainerOverrideActions.DoPreTest(item, null);
            bool result = base.IsItemItsOwnContainerOverride(item);
            IsItemItsOwnContainerOverrideActions.DoTest(item, result);
            return result;
        }

        /// <summary>
        /// Gets test actions for the PrepareContainerForItemOverride method.
        /// </summary>
        public OverriddenMethod<DependencyObject, object> PrepareContainerForItemOverrideActions { get; private set; }

        /// <summary>
        /// Prepares the specified container to display the specified item.
        /// </summary>
        /// <param name="element">
        /// Container element used to display the specified item.
        /// </param>
        /// <param name="item">Specified item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            PrepareContainerForItemOverrideActions.DoPreTest(element, item);
            base.PrepareContainerForItemOverride(element, item);
            PrepareContainerForItemOverrideActions.DoTest(element, item);
        }

        /// <summary>
        /// Gets test actions for the ClearContainerForItemOverride method.
        /// </summary>
        public OverriddenMethod<DependencyObject, object> ClearContainerForItemOverrideActions { get; private set; }

        /// <summary>
        /// Undoes the effects of PrepareContainerForItemOverride.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The contained item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            ClearContainerForItemOverrideActions.DoPreTest(element, item);
            base.ClearContainerForItemOverride(element, item);
            ClearContainerForItemOverrideActions.DoTest(element, item);
        }

        /// <summary>
        /// Gets test actions for the OnItemsChanged method.
        /// </summary>
        public OverriddenMethod<NotifyCollectionChangedEventArgs> OnItemsChangedActions { get; private set; }

        /// <summary>
        /// Provides class handling for an ItemsChanged event that occurs when
        /// there is a change in the Items collection.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            OnItemsChangedActions.DoPreTest(e);
            base.OnItemsChanged(e);
            OnItemsChangedActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnGotFocus test actions.
        /// </summary>
        public OverriddenMethod<RoutedEventArgs> GotFocusActions { get; private set; }

        /// <summary>
        /// Handle the GotFocus event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            GotFocusActions.DoPreTest(e);
            base.OnGotFocus(e);
            GotFocusActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnLostFocus test actions.
        /// </summary>
        public OverriddenMethod<RoutedEventArgs> LostFocusActions { get; private set; }

        /// <summary>
        /// Handle the LostFocus event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            LostFocusActions.DoPreTest(e);
            base.OnLostFocus(e);
            LostFocusActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnKeyDown test actions.
        /// </summary>
        public OverriddenMethod<KeyEventArgs> KeyDownActions { get; private set; }

        /// <summary>
        /// Handle the KeyDown event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            KeyDownActions.DoPreTest(e);
            base.OnKeyDown(e);
            KeyDownActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnKeyUp test actions.
        /// </summary>
        public OverriddenMethod<KeyEventArgs> KeyUpActions { get; private set; }

        /// <summary>
        /// Handle the KeyUp event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            KeyUpActions.DoPreTest(e);
            base.OnKeyUp(e);
            KeyUpActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnMouseEnter test actions.
        /// </summary>
        public OverriddenMethod<MouseEventArgs> MouseEnterActions { get; private set; }

        /// <summary>
        /// Handle the MouseEnter event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            MouseEnterActions.DoPreTest(e);
            base.OnMouseEnter(e);
            MouseEnterActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnMouseLeave test actions.
        /// </summary>
        public OverriddenMethod<MouseEventArgs> MouseLeaveActions { get; private set; }

        /// <summary>
        /// Handle the MouseLeave event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            MouseLeaveActions.DoPreTest(e);
            base.OnMouseLeave(e);
            MouseLeaveActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnMouseMove test actions.
        /// </summary>
        public OverriddenMethod<MouseEventArgs> MouseMoveActions { get; private set; }

        /// <summary>
        /// Handle the MouseMove event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseMoveActions.DoPreTest(e);
            base.OnMouseMove(e);
            MouseMoveActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnMouseLeftButtonDown test actions.
        /// </summary>
        public OverriddenMethod<MouseButtonEventArgs> MouseLeftButtonDownActions { get; private set; }

        /// <summary>
        /// Handle the MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            MouseLeftButtonDownActions.DoPreTest(e);
            base.OnMouseLeftButtonDown(e);
            MouseLeftButtonDownActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnMouseLeftButtonUp test actions.
        /// </summary>
        public OverriddenMethod<MouseButtonEventArgs> MouseLeftButtonUpActions { get; private set; }

        /// <summary>
        /// Handle the MouseLeftButtonUp event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            MouseLeftButtonUpActions.DoPreTest(e);
            base.OnMouseLeftButtonUp(e);
            MouseLeftButtonUpActions.DoTest(e);
        }

        /// <summary>
        /// Gets test actions for the OnApplyTemplate method.
        /// </summary>
        public OverriddenMethod ApplyTemplateActions { get; private set; }

        /// <summary>
        /// Apply a control template to the TreeView.
        /// </summary>
        public override void OnApplyTemplate()
        {
            ApplyTemplateActions.DoPreTest();
            base.OnApplyTemplate();
            ApplyTemplateActions.DoTest();
        }

        /// <summary>
        /// Gets test actions for the OnCreateAutomationPeer method.
        /// </summary>
        public OverriddenMethod<AutomationPeer> CreateAutomationPeerActions { get; private set; }

        /// <summary>
        /// Defines an AutomationPeer for the TreeView control.
        /// </summary>
        /// <returns>
        /// A TreeViewAutomationPeer for the TreeView control.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            CreateAutomationPeerActions.DoPreTest(null);
            AutomationPeer peer = base.OnCreateAutomationPeer();
            CreateAutomationPeerActions.DoTest(peer);
            return peer;
        }

        /// <summary>
        /// Gets test actions for the MeasureOverride method.
        /// </summary>
        public OverriddenMethod<Size, Size?> MeasureActions { get; private set; }

        /// <summary>
        /// Measure the TreeView.
        /// </summary>
        /// <param name="availableSize">Size available for the TreeView.</param>
        /// <returns>Desired size of the TreeView.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            MeasureActions.DoPreTest(availableSize, null);
            Size desired = base.MeasureOverride(availableSize);
            MeasureActions.DoTest(availableSize, desired);
            return desired;
        }

        /// <summary>
        /// Gets test actions for the ArrangeOverride method.
        /// </summary>
        public OverriddenMethod<Size, Size?> ArrangeActions { get; private set; }

        /// <summary>
        /// Arrange the TreeView.
        /// </summary>
        /// <param name="finalSize">Final size for the TreeView.</param>
        /// <returns>Final size used by the TreeView.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeActions.DoPreTest(finalSize, null);
            Size used = base.ArrangeOverride(finalSize);
            ArrangeActions.DoTest(finalSize, used);
            return used;
        }
    }
}