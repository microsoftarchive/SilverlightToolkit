// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Enables navigation transitions for
    /// <see cref="T:Microsoft.Phone.Controls.PhoneApplicationPage"/>s.
    /// </summary>
    [TemplatePart(Name = FirstTemplatePartName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = SecondTemplatePartName, Type = typeof(ContentPresenter))]
    public class TransitionFrame : PhoneApplicationFrame
    {
        /// <summary>
        /// The new
        /// <see cref="T:System.Windows.Controls.ContentPresenter"/>
        /// template part name.
        /// </summary>
        private const string FirstTemplatePartName = "FirstContentPresenter";

        /// <summary>
        /// The old
        /// <see cref="T:System.Windows.Controls.ContentPresenter"/>
        /// template part name.
        /// </summary>
        private const string SecondTemplatePartName = "SecondContentPresenter";

        /// <summary>
        /// Indicates whether a navigation is forward.
        /// </summary>
        private bool _isForwardNavigation;

        /// <summary>
        /// Determines whether to set the new content to the first or second
        /// <see cref="T:System.Windows.Controls.ContentPresenter"/>.
        /// </summary>
        private bool _useFirstAsNew;
        
        /// <summary>
        /// The first <see cref="T:System.Windows.Controls.ContentPresenter"/>.
        /// </summary>
        private ContentPresenter _firstContentPresenter;

        /// <summary>
        /// The second <see cref="T:System.Windows.Controls.ContentPresenter"/>.
        /// </summary>
        private ContentPresenter _secondContentPresenter;

        /// <summary>
        /// The old <see cref="T:System.Windows.Controls.ContentPresenter"/>.
        /// </summary>
        private ContentPresenter _oldContentPresenter;

        /// <summary>
        /// The new <see cref="T:System.Windows.Controls.ContentPresenter"/>.
        /// </summary>
        private ContentPresenter _newContentPresenter;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TransitionFrame()
            : base()
        {
            DefaultStyleKey = typeof(TransitionFrame);
            Navigating += (s, e) =>
            {
                _isForwardNavigation = e.NavigationMode != NavigationMode.Back;
            };
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call
        /// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// In simplest terms, this means the method is called just before a UI element displays in an application.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _firstContentPresenter = GetTemplateChild(FirstTemplatePartName) as ContentPresenter;
            _secondContentPresenter = GetTemplateChild(SecondTemplatePartName) as ContentPresenter;
            if (Content != null)
            {
                OnContentChanged(null, Content);
            }
        }

        /// <summary>
        /// Called when the value of the
        /// <see cref="P:System.Windows.Controls.ContentControl.Content"/>
        /// property changes.
        /// </summary>
        /// <param name="oldContent">The old <see cref="T:System.Object"/>.</param>
        /// <param name="newContent">The new <see cref="T:System.Object"/>.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            UIElement oldElement = oldContent as UIElement;
            UIElement newElement = newContent as UIElement;
            if (_firstContentPresenter == null || _secondContentPresenter == null || newElement == null)
            {
                return;
            }
            if (_useFirstAsNew)
            {
                _newContentPresenter = _firstContentPresenter;
                _oldContentPresenter = _secondContentPresenter;
            }
            else
            {
                _newContentPresenter = _secondContentPresenter;
                _oldContentPresenter = _firstContentPresenter;
            }
            _useFirstAsNew = !_useFirstAsNew;
            NavigationOutTransition navigationOutTransition = null;
            NavigationInTransition navigationInTransition = null;
            ITransition oldTransition = null;
            ITransition newTransition = null;
            if (oldElement != null)
            {
                navigationOutTransition = TransitionService.GetNavigationOutTransition(oldElement);
                TransitionElement oldTransitionElement = null;
                if (navigationOutTransition != null)
                {
                    oldTransitionElement = _isForwardNavigation ? navigationOutTransition.Forward : navigationOutTransition.Backward;
                }
                if (oldTransitionElement != null)
                {
                    oldTransition = oldTransitionElement.GetTransition(oldElement);
                    _oldContentPresenter.CacheMode = new BitmapCache();
                    _oldContentPresenter.IsHitTestVisible = false;
                }
            }
            if (newElement != null)
            {
                navigationInTransition = TransitionService.GetNavigationInTransition(newElement);
                TransitionElement newTransitionElement = null;
                if (navigationInTransition != null)
                {
                    newTransitionElement = _isForwardNavigation ? navigationInTransition.Forward : navigationInTransition.Backward;
                }
                if (newTransitionElement != null)
                {
                    newElement.UpdateLayout();
                    newTransition = newTransitionElement.GetTransition(newElement);
                    _newContentPresenter.CacheMode = new BitmapCache();
                    _newContentPresenter.IsHitTestVisible = false;
                }
            }
            _newContentPresenter.Opacity = 0;
            _newContentPresenter.Visibility = Visibility.Visible;
            _newContentPresenter.Content = newElement;
            _oldContentPresenter.Opacity = 1;
            _oldContentPresenter.Visibility = Visibility.Visible;
            _oldContentPresenter.Content = oldElement;
            if (oldTransition != null)
            {
                if (oldTransition.GetCurrentState() != ClockState.Stopped)
                {
                    oldTransition.Stop();
                }
                oldTransition.Completed += delegate
                {
                    oldTransition.Stop();
                    _oldContentPresenter.CacheMode = null;
                    _oldContentPresenter.IsHitTestVisible = true;
                    if (navigationOutTransition != null)
                    {
                        navigationOutTransition.OnEndTransition();
                    }
                    TransitionNewElement(newTransition, navigationInTransition);
                };
                Dispatcher.BeginInvoke(delegate
                {
                    Dispatcher.BeginInvoke(delegate
                    {
                        if (navigationOutTransition != null)
                        {
                            navigationOutTransition.OnBeginTransition();
                        }
                        oldTransition.Begin();
                    });
                });
            }
            else
            {
                TransitionNewElement(newTransition, navigationInTransition);
            }
        }

        /// <summary>
        /// Transitions the new <see cref="T:System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="newTransition">The <see cref="T:Microsoft.Phone.Controls.ITransition"/> for the new <see cref="T:System.Windows.UIElement"/>.</param>
        /// <param name="navigationInTransition">The <see cref="T:Microsoft.Phone.Controls.NavigationInTransition"/>  for the new <see cref="T:System.Windows.UIElement"/>.</param>
        private void TransitionNewElement(ITransition newTransition, NavigationInTransition navigationInTransition)
        {
            _oldContentPresenter.Visibility = Visibility.Collapsed;
            _oldContentPresenter.Content = null;
            if (newTransition == null)
            {
                _newContentPresenter.IsHitTestVisible = true;
                _newContentPresenter.Opacity = 1;
                return;
            }
            if (newTransition.GetCurrentState() != ClockState.Stopped)
            {
                newTransition.Stop();
            }
            newTransition.Completed += delegate
            {
                newTransition.Stop();
                _newContentPresenter.CacheMode = null;
                _newContentPresenter.IsHitTestVisible = true;
                if (navigationInTransition != null)
                {
                    navigationInTransition.OnEndTransition();
                }
            };
            Dispatcher.BeginInvoke(delegate
            {
                if (navigationInTransition != null)
                {
                    navigationInTransition.OnBeginTransition();
                }
                _newContentPresenter.Opacity = 1;
                newTransition.Begin();
            });
        }
    }
}