// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
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
        /// The first <see cref="T:System.Windows.Controls.ContentPresenter"/>.
        /// </summary>
        private ContentPresenter _firstContentPresenter;

        /// <summary>
        /// The second <see cref="T:System.Windows.Controls.ContentPresenter"/>.
        /// </summary>
        private ContentPresenter _secondContentPresenter;

        /// <summary>
        /// Determines whether to set the new content to the first or second
        /// <see cref="T:System.Windows.Controls.ContentPresenter"/>.
        /// </summary>
        private bool _useFirstContentPresenter;

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
        /// Gets the backward in
        /// <see cref="T:Microsoft.Phone.Controls.ITransition"/>
        /// for the
        /// <see cref="T:System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Windows.UIElement"/>.</param>
        /// <returns>The <see cref="T:Microsoft.Phone.Controls.ITransition"/>.</returns>
        private static TransitionElement BackwardIn(UIElement element)
        {
            NavigationTransition navigationTransition = TransitionService.GetNavigationInTransition(element);
            if (navigationTransition == null)
            {
                return null;
            }
            return navigationTransition.Backward;
        }

        /// <summary>
        /// Gets the backward out
        /// <see cref="T:Microsoft.Phone.Controls.ITransition"/>
        /// for the
        /// <see cref="T:System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Windows.UIElement"/>.</param>
        /// <returns>The <see cref="T:Microsoft.Phone.Controls.ITransition"/>.</returns>
        private static TransitionElement BackwardOut(UIElement element)
        {
            NavigationTransition navigationTransition = TransitionService.GetNavigationOutTransition(element);
            if (navigationTransition == null)
            {
                return null;
            }
            return navigationTransition.Backward;
        }

        /// <summary>
        /// Gets the forward in
        /// <see cref="T:Microsoft.Phone.Controls.ITransition"/>
        /// for the
        /// <see cref="T:System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Windows.UIElement"/>.</param>
        /// <returns>The <see cref="T:Microsoft.Phone.Controls.ITransition"/>.</returns>
        private static TransitionElement ForwardIn(UIElement element)
        {
            NavigationTransition navigationTransition = TransitionService.GetNavigationInTransition(element);
            if (navigationTransition == null)
            {
                return null;
            }
            return navigationTransition.Forward;
        }

        /// <summary>
        /// Gets the forward out
        /// <see cref="T:Microsoft.Phone.Controls.ITransition"/>
        /// for the
        /// <see cref="T:System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Windows.UIElement"/>.</param>
        /// <returns>The <see cref="T:Microsoft.Phone.Controls.ITransition"/>.</returns>
        private static TransitionElement ForwardOut(UIElement element)
        {
            NavigationTransition navigationTransition = TransitionService.GetNavigationOutTransition(element);
            if (navigationTransition == null)
            {
                return null;
            }
            return navigationTransition.Forward;
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
            TransitionElement oldTransitionElement = null;
            TransitionElement newTransitionElement = null;
            if (_isForwardNavigation)
            {
                if (oldElement != null)
                {
                    oldTransitionElement = ForwardOut(oldElement);
                }
                newTransitionElement = ForwardIn(newElement);
            }
            else
            {
                if (oldElement != null)
                {
                    oldTransitionElement = BackwardOut(oldElement);
                }
                newTransitionElement = BackwardIn(newElement);
            }
            ContentPresenter oldContentPresenter = null;
            ContentPresenter newContentPresenter = null;
            if (_useFirstContentPresenter)
            {
                newContentPresenter = _firstContentPresenter;
                oldContentPresenter = _secondContentPresenter;
            }
            else
            {
                newContentPresenter = _secondContentPresenter;
                oldContentPresenter = _firstContentPresenter;
            }
            _useFirstContentPresenter = !_useFirstContentPresenter;
            newContentPresenter.Opacity = 0;
            newContentPresenter.Visibility = Visibility.Visible;
            newContentPresenter.Content = newElement;
            oldContentPresenter.Opacity = 1;
            oldContentPresenter.Visibility = Visibility.Visible;
            ITransition newTransition = null;
            if (newTransitionElement != null)
            {
                newElement.UpdateLayout();
                newTransition = newTransitionElement.GetTransition(newElement);
            }
            if (oldTransitionElement != null)
            {
                ITransition oldTransition = oldTransitionElement.GetTransition(oldElement);
                if (oldTransition.GetCurrentState() != ClockState.Stopped)
                {
                    oldTransition.Stop();
                }
                oldTransition.Completed += delegate
                {
                    oldTransition.Stop();
                    TransitionNewElement(oldContentPresenter, newContentPresenter, newTransition);
                };
                oldTransition.Begin();
            }
            else
            {
                TransitionNewElement(oldContentPresenter, newContentPresenter, newTransition);
            }
        }

        /// <summary>
        /// Transitions the new <see cref="T:System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="oldContentPresenter">The old <see cref="T:System.Windows.Controls.ContentPresenter"/>.</param>
        /// <param name="newContentPresenter">The new <see cref="T:System.Windows.Controls.ContentPresenter"/>.</param>
        /// <param name="transition">The <see cref="T:Microsoft.Phone.Controls.ITransition"/>.</param>
        private static void TransitionNewElement(ContentPresenter oldContentPresenter, ContentPresenter newContentPresenter, ITransition transition)
        {
            oldContentPresenter.Visibility = Visibility.Collapsed;
            oldContentPresenter.Content = null;
            if (transition == null)
            {
                newContentPresenter.Opacity = 1;
            }
            else
            {
                if (transition.GetCurrentState() != ClockState.Stopped)
                {
                    transition.Stop();
                }
                transition.Completed += delegate
                {
                    transition.Stop();
                };
                newContentPresenter.Opacity = 1;
                transition.Begin();
            }
        }
    }
}