// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls
{
    /// <summary>
    /// TabControl allows a developer to arrange visual content in a compacted
    /// and organized form.  The real-world analog of the control might be a
    /// tabbed notebook, in which visual content is displayed in discreet pages
    /// which are accessed by selecting the appropriate tab.  Each tab/page is
    /// encapsulated by a TabItem, the generated item of TabControl.  A TabItem
    /// has a Header property which corresponds to the content in the tab button
    /// and a Content property which corresponds to the content in the tab page.
    /// This control is useful for minimizing screen space usage while allowing
    /// an application to expose a large amount of data.  The user navigates
    /// through TabItems by clicking on a tab button using the mouse or by using
    /// the keyboard.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class TabControl : ItemsControl
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
        /// Expose test hooks for internal and private members of the
        /// TabControl.
        /// </summary>
        internal class InternalTestHook
        {
            /// <summary>
            /// Reference to the outer 'parent' TabControl.
            /// </summary>
            private TabControl _tabControl;

            /// <summary>
            /// Initializes a new instance of the InternalTestHook class.
            /// </summary>
            /// <param name="tabControl">The TabControl to hook.</param>
            internal InternalTestHook(TabControl tabControl)
            {
                _tabControl = tabControl;
            }

            /// <summary>
            /// Find the next tab item.
            /// </summary>
            /// <param name="startIndex">Initial index.</param>
            /// <param name="direction">Direction to search.</param>
            /// <returns>The next tab item.</returns>
            internal TabItem FindNextTabItem(int startIndex, int direction)
            {
                return _tabControl.FindNextTabItem(startIndex, direction);
            }
        }
    }
}