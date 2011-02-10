// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Phone.Shell;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Partial definition of LongListSelector. Includes group view code.
    /// </summary>
    public partial class LongListSelector : Control
    {
        private PhoneApplicationPage _page;

        private bool _systemTrayVisible;
        private bool _applicationBarVisible;
        private Border _border;
        private LongListSelectorItemsControl _itemsControl;

        private static readonly double _screenWidth = Application.Current.Host.Content.ActualWidth;
        private static readonly double _screenHeight = Application.Current.Host.Content.ActualHeight;

        /// <summary>
        /// Will invoke the group view, if a GroupItemTemplate has been defined.
        /// </summary>
        public void DisplayGroupView()
        {
            StopScrolling();

            if (GroupItemTemplate != null && !IsFlatList)
            {
                OpenPopup();
            }
        }

        /// <summary>
        /// Close the group view unconditionally (do not raise the GroupViewClosingEventArgs event.)
        /// </summary>
        public void CloseGroupView()
        {
            ClosePopup(null, false);
        }

        /// <summary>
        /// This event will be raised when the group Popup's IsOpen has been set to true.
        /// </summary>
        public event EventHandler<GroupViewOpenedEventArgs> GroupViewOpened;
        
        /// <summary>
        /// This event will be raised then the group Popup is about to close.
        /// </summary>
        public event EventHandler<GroupViewClosingEventArgs> GroupViewClosing;

        private void OpenPopup()
        {
            SaveSystemState(false, false);
            BuildPopup();
            AttachToPageEvents();
            _groupSelectorPopup.IsOpen = true;

            // This has to happen eventually anyway, and this forces the ItemsControl to 
            // expand it's template, populate it's items etc.
            UpdateLayout(); 


            SafeRaise.Raise(GroupViewOpened, this, () => { return new GroupViewOpenedEventArgs(_itemsControl); });
        }

        /// <summary>
        /// Close the group popup.
        /// </summary>
        /// <param name="selectedGroup">The selected group.</param>
        /// <param name="raiseEvent">Should the GroupPopupClosing event be raised.</param>
        /// <returns>True if the event was not raised or if it was raised and e.Handled is false.</returns>
        private bool ClosePopup(object selectedGroup, bool raiseEvent)
        {
            if (raiseEvent)
            {
                GroupViewClosingEventArgs args = null;
                
                SafeRaise.Raise(GroupViewClosing, this, () => { return args = new GroupViewClosingEventArgs(_itemsControl, selectedGroup); });

                if (args != null && args.Cancel)
                {
                    return false;
                }
            }

            if (_groupSelectorPopup != null)
            {
                RestoreSystemState();
                _groupSelectorPopup.IsOpen = false;
                DetachFromPageEvents();
                _groupSelectorPopup.Child = null;
                _border = null;
                _itemsControl = null;
                _groupSelectorPopup = null;
            }

            return true;
        }

        private void BuildPopup()
        {
            _groupSelectorPopup = new Popup();

            var bg = (Color)Resources["PhoneBackgroundColor"];
            _border = new Border
            { 
                Background = new SolidColorBrush(
                    Color.FromArgb(0xa0, bg.R, bg.G, bg.B))
            };
            
            GestureListener listener = GestureService.GetGestureListener(_border);
            listener.GestureBegin += HandleGesture;
            listener.GestureCompleted += HandleGesture;
            listener.DoubleTap += HandleGesture;
            listener.DragCompleted += HandleGesture;
            listener.DragDelta += HandleGesture;
            listener.DragStarted += HandleGesture;
            listener.Flick += HandleGesture;
            listener.Hold += HandleGesture;
            listener.PinchCompleted += HandleGesture;
            listener.PinchDelta += HandleGesture;
            listener.PinchStarted += HandleGesture;
            listener.Tap += HandleGesture;

            _itemsControl = new LongListSelectorItemsControl();
            _itemsControl.ItemTemplate = GroupItemTemplate;
            _itemsControl.ItemsPanel = GroupItemsPanel;
            _itemsControl.ItemsSource = ItemsSource;

            _itemsControl.GroupSelected += itemsControl_GroupSelected;

            _groupSelectorPopup.Child = _border;
            ScrollViewer sv = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };

            _border.Child = sv;
            sv.Content = _itemsControl;

            SetItemsControlSize();
        }

        private void SetItemsControlSize()
        {
            Rect client = GetTransformedRect();
            if (_border != null)
            {
                _border.Padding = new Thickness(18);
                _border.RenderTransform = GetTransform();

                _border.Width = client.Width;
                _border.Height = client.Height;
            }
        }

        private void itemsControl_GroupSelected(object sender, LongListSelector.GroupSelectedEventArgs e)
        {
            if (ClosePopup(e.Group, true))
            {
                ScrollToGroup(e.Group);
            }
        }

        private void AttachToPageEvents()
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                _page = frame.Content as PhoneApplicationPage;
                if (_page != null)
                {
                    _page.BackKeyPress += page_BackKeyPress;
                    _page.OrientationChanged += page_OrientationChanged;
                }
            }
        }

        private void DetachFromPageEvents()
        {
            if (_page != null)
            {
                _page.BackKeyPress -= page_BackKeyPress;
                _page.OrientationChanged -= page_OrientationChanged;
                _page = null;
            }
        }

        private void page_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            ClosePopup(null, true);
        }

        void page_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            SetItemsControlSize();
        }

        private static Rect GetTransformedRect()
        {
            bool isLandscape = IsLandscape(GetPageOrientation());

            return new Rect(0, 0,
                isLandscape ? _screenHeight : _screenWidth,
                isLandscape ? _screenWidth : _screenHeight);
        }

        private static Transform GetTransform()
        {
            PageOrientation orientation = GetPageOrientation();

            switch (orientation)
            {
                case PageOrientation.LandscapeLeft:
                case PageOrientation.Landscape:
                    return new CompositeTransform() { Rotation = 90, TranslateX = _screenWidth };
                case PageOrientation.LandscapeRight:
                    return new CompositeTransform() { Rotation = -90, TranslateY = _screenHeight };
                default:
                    return null;
            }
        }

        private static bool IsLandscape(PageOrientation orientation)
        {
            return orientation == PageOrientation.Landscape || orientation == PageOrientation.LandscapeLeft || orientation == PageOrientation.LandscapeRight;
        }

        private static PageOrientation GetPageOrientation()
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                PhoneApplicationPage page = frame.Content as PhoneApplicationPage;
                return page.Orientation;
            }

            return PageOrientation.None;
        }

        private void SaveSystemState(bool newSystemTrayVisible, bool newApplicationBarVisible)
        {
            _systemTrayVisible = SystemTray.IsVisible;
            SystemTray.IsVisible = newSystemTrayVisible;

            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                PhoneApplicationPage page = frame.Content as PhoneApplicationPage;
                if (page != null && page.ApplicationBar != null)
                {
                    _applicationBarVisible = page.ApplicationBar.IsVisible;
                    page.ApplicationBar.IsVisible = newApplicationBarVisible;
                }
            }
        }

        private void RestoreSystemState()
        {
            SystemTray.IsVisible = _systemTrayVisible;

            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                PhoneApplicationPage page = frame.Content as PhoneApplicationPage;
                if (page != null && page.ApplicationBar != null)
                {
                    page.ApplicationBar.IsVisible = _applicationBarVisible;
                }
            }
        }
    }

}
