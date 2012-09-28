// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls.Primitives;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// The lockable Pivot extends the base Pivot control with a property that
    /// disables navigation between Pivot items.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    [TemplatePart(Name = HeadersListElement, Type = typeof(PivotHeadersControl))]
    [TemplatePart(Name = PivotItemPresenterElement, Type = typeof(ItemsPresenter))]
    [StyleTypedProperty(Property = ItemContainerStyleName, StyleTargetType = typeof(PivotItem))]
    public class LockablePivot : Pivot
    {
        /// <summary>
        /// Headers list element template part.
        /// </summary>
        private const string HeadersListElement = "HeadersListElement";

        /// <summary>
        /// Content element template part.
        /// </summary>
        private const string PivotItemPresenterElement = "PivotItemPresenter";

        /// <summary>
        /// The item container style property name.
        /// </summary>
        internal const string ItemContainerStyleName = "ItemContainerStyle";

        private bool _isLocked   = false;
        private bool _isUpdating = false;

        #region public bool IsLocked
        /// <summary>
        /// Gets or sets a value indicating whether the control is locked.
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)GetValue(IsLockedProperty); }
            set { SetValue(IsLockedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsLocked dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLockedProperty =
            DependencyProperty.Register(
                "IsLocked",
                typeof(bool),
                typeof(LockablePivot),
                new PropertyMetadata(false, OnIsLockedPropertyChanged));

        /// <summary>
        /// IsLockedProperty property changed handler.
        /// </summary>
        /// <param name="d">LockablePivot that changed its IsLocked.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsLockedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LockablePivot source = d as LockablePivot;
            if (source != null)
            {
                source.OnIsLockedChanged((bool)e.NewValue);
            }
        }
        #endregion public bool IsLocked

        private struct HeaderAnimationInfo
        {
            public double opacity;
            public double originalX;
            public OpacityAnimator opacityAnimator;
            public TransformAnimator transformAnimator;
            public TranslateTransform transform;
            public PivotHeaderItem headerItem;
        }

        private PivotItem[]           _savedItems     = null;
        private HeaderAnimationInfo[] _animInfo       = null;
        private PivotHeadersControl   _header         = null;
        private int                   _savedIndex;
        
        private static Duration _animTime =
            new Duration(TimeSpan.FromMilliseconds(200));

        private const double _animOffset = 20.0;

        /// <summary>
        /// Initializes a new instance of the LockablePivot type.
        /// </summary>
        public LockablePivot()
        {
            SelectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// Invoked when the Items property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_isUpdating)
            {
                if (_isLocked)
                {
                    throw new System.InvalidOperationException(
                        "Pivot Items cannot be modified when locked"
                        );
                }

                // Items have been changed, so animation information
                // will need to be recreated.
                _animInfo = null;

                base.OnItemsChanged(e);
            }
        }

        private PivotHeadersControl FindHeader(UIElement start)
        {
            UIElement target = null;

            int childCount = VisualTreeHelper.GetChildrenCount(start);
            
            for (int i = 0; i < childCount; i++)
            {
                UIElement e = VisualTreeHelper.GetChild(start, i) as UIElement;

                if (e is PivotHeadersControl)
                {
                    target = e;
                }
                else
                {
                    target = FindHeader(e);
                }

                if (target != null)
                {
                    break;
                }
            }

            return target as PivotHeadersControl;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // We've transitioned to another pivot item, so existing
            // animation information is no longer valid.
            _animInfo = null;
        }

        private void OnIsLockedChanged(bool newValue)
        {
            _isLocked = newValue;
            _isUpdating = true;

            if (_isLocked)
            {
                _savedIndex = SelectedIndex;

                FadeOutHeaders();
                SaveAndRemoveItems();
            }
            else
            {
                RestoreItems();
                FadeInHeaders();
            }

            _isUpdating = false;
        }

        // We create and store the animation information for the current
        // state of the header the first time we need to perform an animation.
        // It's stored so that an initial fade-out animation can be
        // interrupted and resumed from its current state by the fade-in
        // animation.
        private void CreateAnimationInformation()
        {
            if (_animInfo == null)
            {
                int count = _header.Items.Count;

                // We won't animate the currently selected item,
                // so ensure we don't create information for it.
                _animInfo = new HeaderAnimationInfo[count - 1];

                int i = 0;

                foreach (PivotHeaderItem headerItem in _header.Items)
                {
                    if (!headerItem.IsSelected)
                    {
                        _animInfo[i].opacity = headerItem.Opacity;
                        _animInfo[i].opacityAnimator = new OpacityAnimator(headerItem);
                        _animInfo[i].transform =
                            TransformAnimator.GetTranslateTransform(headerItem);
                        _animInfo[i].transformAnimator =
                            new TransformAnimator(_animInfo[i].transform);
                        _animInfo[i].originalX = _animInfo[i].transform.X;
                        _animInfo[i].headerItem = headerItem;

                        i++;
                    }
                }
            }
        }

        private void SaveAndRemoveItems()
        {
            _savedItems = new PivotItem[Items.Count];
            Items.CopyTo(_savedItems, 0);

            for (int i = Items.Count - 1; i > _savedIndex; i--)
            {
                Items.RemoveAt(i);
            }

            for (int i = 0; i < _savedIndex; i++)
            {
                Items.RemoveAt(0);
            }
        }

        private void RestoreItems()
        {
            for (int i = 0; i < _savedIndex; i++)
            {
                Items.Insert(i, _savedItems[i]);
            }

            for (int i = _savedIndex + 1; i < _savedItems.Length; i++)
            {
                Items.Add(_savedItems[i]);
            }

            _savedItems = null;
        }
       
        private void FadeOutHeaders()
        {
            if (_header == null)
            {
                _header = FindHeader(this);
            }

            CreateAnimationInformation();
           
            foreach (HeaderAnimationInfo anim in _animInfo)
            {
                // headerItem should not be hittestable when it is faded out.
                anim.headerItem.IsHitTestVisible = false;

                anim.opacityAnimator.GoTo(
                    0.0, 
                    _animTime
                    );
                anim.transformAnimator.GoTo(
                    _animOffset + anim.originalX,
                    _animTime
                    );
            }
        }
        
        private void FadeInHeaders()
        {
            foreach (HeaderAnimationInfo anim in _animInfo)
            {
                // Enable hit test on the headerItem when it is faded in.
                anim.headerItem.IsHitTestVisible = true;

                anim.opacityAnimator.GoTo(
                    anim.opacity,
                    _animTime
                    );
                anim.transformAnimator.GoTo(
                    anim.originalX,
                    _animTime
                    );
            }    
        }
    }
}
