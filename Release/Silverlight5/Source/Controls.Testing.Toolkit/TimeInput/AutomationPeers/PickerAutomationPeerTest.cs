// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Linq;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Tests for Picker types.
    /// </summary>
    public abstract class PickerAutomationPeerTest : TestBase
    {
        /// <summary>
        /// Gets the picker instance.
        /// </summary>
        /// <returns>A picker to test.</returns>
        protected abstract Picker PickerInstance { get; }

        /// <summary>
        /// Gets the picker automation peer.
        /// </summary>
        /// <param name="picker">The picker.</param>
        /// <returns>A PickerAutomationPeer for the picker.</returns>
        protected abstract PickerAutomationPeer CreatePickerAutomationPeer(Picker picker);

        /// <summary>
        /// Gets the expected patterns.
        /// </summary>
        /// <returns>An IList of patterns that this Picker is expected to 
        /// implement.</returns>
        protected abstract IList<PatternInterface> ExpectedPatterns { get; }

        /// <summary>
        /// Create a new AutomationPeer.
        /// </summary>
        [TestMethod]
        [Description("Create a new PickerAutomationPeer.")]
        public virtual void CreatePickerPeer()
        {
            PickerAutomationPeer peer = CreatePickerAutomationPeer(PickerInstance);
            Assert.IsNotNull(peer);
        }

        /// <summary>
        /// Create a new PickerAutomationPeer with a null object.
        /// </summary>
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Create a new PickerAutomationPeer with a null object.")]
        [TestMethod]
        public virtual void ShouldThrowExceptionWhenCreatingPickerPeerWithNull()
        {
            PickerAutomationPeer peer = CreatePickerAutomationPeer(null);

            Assert.Fail("Should have thrown exception." + peer);
        }

        /// <summary>
        /// Verify the correct patterns are supported.
        /// </summary>
        [TestMethod]
        [Description("Verify the PickerAutomationPeer supports ExpandCollapse pattern.")]
        public virtual void PickerPeerOnlySupportsCorrectPatterns()
        {
            PickerAutomationPeer peer = CreatePickerAutomationPeer(PickerInstance);
            int index = 0;
            while (Enum.IsDefined(typeof(PatternInterface), index))
            {
                PatternInterface currentInterface = (PatternInterface) Enum.ToObject(typeof(PatternInterface), index);
                object implementation = peer.GetPattern(currentInterface);
                if (ExpectedPatterns.Contains(currentInterface))
                {
                    Assert.IsNotNull(implementation);
                }
                else
                {
                    Assert.IsNull(implementation);
                }
                index++;
            }
        }

        #region IExpandCollapseProvider
        /// <summary>
        /// Verify that PickerAutomationPeer implements the
        /// IExpandCollapseProvider interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that PickerAutomationPeer implements the IExpandCollapseProvider interface.")]
        public virtual void PickerPeerIsIExpandCollapseProvider()
        {
            Picker item = PickerInstance;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as IExpandCollapseProvider,
                () => Assert.IsNotNull(provider, "PickerAutomationPeer should implement IExpandCollapseProvider!"));
        }

        /// <summary>
        /// Verify that PickerAutomationPeer supports the ExpandCollapse
        /// pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that PickerAutomationPeer supports the ExpandCollapse pattern.")]
        public virtual void PickerPeerSupportsExpandCollapse()
        {
            Picker item = PickerInstance;
            PickerAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as PickerAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.IsNotNull(provider, "IExpandCollapseProvider peer should not be null!"));
        }

        /// <summary>
        /// Verify that PickerAutomationPeer has the right
        /// ExpandCollapseState for an item that is collapsed.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that PickerAutomationPeer has the right ExpandCollapseState for an item that is collapsed.")]
        public virtual void PickerPeerExpandStateCollapsed()
        {
            Picker item = PickerInstance;
            PickerAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as PickerAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.AreEqual(ExpandCollapseState.Collapsed, provider.ExpandCollapseState, "Unexpected ExpandCollapseState!"));
        }

        /// <summary>
        /// Verify that PickerAutomationPeer has the right
        /// ExpandCollapseState for an item that is expanded.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that PickerAutomationPeer has the right ExpandCollapseState for an item that is expanded.")]
        public virtual void PickerPeerExpandStateExpanded()
        {
            Picker item = PickerInstance;
            item.IsDropDownOpen = true;

            PickerAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as PickerAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.AreEqual(ExpandCollapseState.Expanded, provider.ExpandCollapseState, "Unexpected ExpandCollapseState!"));
        }

        /// <summary>
        /// Verify that PickerAutomationPeer throws an exception when
        /// expanding a disabled item.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementNotEnabledException))]
        [Asynchronous]
        [Description("Verify that PickerAutomationPeer throws an exception when expanding a disabled item.")]
        public virtual void PickerPeerExpandDisabled()
        {
            Picker item = PickerInstance;
            item.IsEnabled = false;
            PickerAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as PickerAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand());
        }

        /// <summary>
        /// Verify that PickerAutomationPeer expands an item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that PickerAutomationPeer expands an item.")]
        public virtual void PickerPeerExpands()
        {
            Picker item = PickerInstance;
            PickerAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as PickerAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand(),
                () => Assert.IsTrue(item.IsDropDownOpen, "Item should be expanded!"));
        }

        /// <summary>
        /// Verify that PickerAutomationPeer expands an expanded item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that PickerAutomationPeer expands an expanded item.")]
        public virtual void PickerPeerExpandsExpanded()
        {
            Picker item = PickerInstance;
            item.IsDropDownOpen = true;
            PickerAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as PickerAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand(),
                () => Assert.IsTrue(item.IsDropDownOpen, "Item should be expanded!"));
        }

        /// <summary>
        /// Verify that PickerAutomationPeer throws an exception when
        /// collapsing a disabled item.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementNotEnabledException))]
        [Asynchronous]
        [Description("Verify that PickerAutomationPeer throws an exception when collapsing a disabled item.")]
        public virtual void PickerPeerCollapseDisabled()
        {
            Picker item = PickerInstance;
            item.IsEnabled = false;

            PickerAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as PickerAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse());
        }

        /// <summary>
        /// Verify that PickerAutomationPeer collapses an item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that PickerAutomationPeer collapses an item.")]
        public virtual void PickerPeerCollapse()
        {
            Picker item = PickerInstance;
            item.IsDropDownOpen = true;

            PickerAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as PickerAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse(),
                () => Assert.IsFalse(item.IsDropDownOpen, "Item should be collapsed!"));
        }

        /// <summary>
        /// Verify that PickerAutomationPeer collapses an collapsed item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that PickerAutomationPeer collapses an collapsed item.")]
        public virtual void PickerPeerCollapseCollapsed()
        {
            Picker item = PickerInstance;
            PickerAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as PickerAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse(),
                () => Assert.IsFalse(item.IsDropDownOpen, "Item should be collapsed!"));
        }
        #endregion IExpandCollapseProvider
    }
}
