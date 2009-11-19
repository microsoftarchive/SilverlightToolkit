// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Input;

namespace System.Windows.Controls
{
    /// <summary>
    /// A child item of TabControl.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class TabItem : ContentControl
    {
        /// <summary>
        /// Exposes test hooks to unit tests with internal access.
        /// </summary>
        private InternalTestHook _testHook;

        /// <summary>
        /// Gets a test hook for unit tests with internal access.
        /// </summary>
        internal InternalTestHook TestHook
        {
            get
            {
                if (_testHook == null)
                {
                    _testHook = new InternalTestHook(this);
                }
                return _testHook;
            }
        }

        /// <summary>
        /// Expose test hooks for internal and private members of the TabItem.
        /// </summary>
        internal class InternalTestHook
        {
            /// <summary>
            /// Reference to the outer 'parent' TabItem.
            /// </summary>
            private TabItem _tabItem;

            /// <summary>
            /// Initializes a new instance of the InternalTestHook class.
            /// </summary>
            /// <param name="tabItem">The TabItem to hook.</param>
            internal InternalTestHook(TabItem tabItem)
            {
                _tabItem = tabItem;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the TabItem has the
            /// mouse over it.
            /// </summary>
            internal bool IsMouseOver
            {
                get { return _tabItem._isMouseOver; }
                set { _tabItem._isMouseOver = value; }
            }

            /// <summary>
            /// Gets the TabItem's TabControlParent property.
            /// </summary>
            internal TabControl TabControlParent
            {
                get { return _tabItem.TabControlParent; }
            }

            /// <summary>
            /// Simulate the MouseLeftButtonDown event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">Event arguments.</param>
            internal void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                _tabItem.OnMouseLeftButtonDown(sender, e);
            }

            /// <summary>
            /// Simulate the MouseLeave event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">Event arguments.</param>
            internal void OnMouseLeave(object sender, MouseEventArgs e)
            {
                _tabItem.OnMouseLeave(sender, e);
            }

            /// <summary>
            /// Simulate the MouseEnter event.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">Event arguments.</param>
            internal void OnMouseEnter(object sender, MouseEventArgs e)
            {
                _tabItem.OnMouseEnter(sender, e);
            }
        }
    }
}