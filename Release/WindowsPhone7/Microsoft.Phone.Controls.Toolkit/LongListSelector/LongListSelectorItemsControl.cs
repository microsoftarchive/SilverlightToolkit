// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Partial definition of LongListSelector. Includes ItemsControl subclass.
    /// </summary>
    public partial class LongListSelector : Control
    {
        private class GroupSelectedEventArgs : EventArgs
        {
            public GroupSelectedEventArgs(object group)
            {
                Group = group;
            }

            public object Group { get; private set; }
        }

        private delegate void GroupSelectedEventHandler(object sender, GroupSelectedEventArgs e);

        private class LongListSelectorItemsControl : ItemsControl
        {
            public event GroupSelectedEventHandler GroupSelected;

            protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
            {
                base.PrepareContainerForItemOverride(element, item);

                GestureService.GetGestureListener(element).Tap += LongListSelectorItemsControl_Tap;
            }

            protected override void ClearContainerForItemOverride(DependencyObject element, object item)
            {
                base.ClearContainerForItemOverride(element, item);

                GestureService.GetGestureListener(element).Tap -= LongListSelectorItemsControl_Tap;
            }

            private void LongListSelectorItemsControl_Tap(object sender, GestureEventArgs e)
            {
                ContentPresenter cc = sender as ContentPresenter;
                if (cc != null)
                {
                    GroupSelectedEventHandler handler = GroupSelected;
                    if (handler != null)
                    {
                        GroupSelectedEventArgs args = new GroupSelectedEventArgs(cc.Content);
                        handler(this, args);
                    }
                }
            }
        }
    }
}
