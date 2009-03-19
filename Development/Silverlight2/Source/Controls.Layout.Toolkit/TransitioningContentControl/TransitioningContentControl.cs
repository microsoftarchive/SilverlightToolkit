// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Media.Animation;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control with a single piece of content and when that content 
    /// changes performs a transition animation. 
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    /// <remarks>The API for this control will change considerably in the future.</remarks>
    [TemplateVisualState(GroupName = PresentationGroup, Name = NormalState)]
    [TemplateVisualState(GroupName = PresentationGroup, Name = DefaultTransitionState)]
    [TemplatePart(Name = PreviousContentPresentationSitePartName, Type = typeof(ContentControl))]
    [TemplatePart(Name = CurrentContentPresentationSitePartName, Type = typeof(ContentControl))]
    public class TransitioningContentControl : ContentControl
    {
        #region Visual state names
        /// <summary>
        /// The name of the group that holds the presentation states.
        /// </summary>
        private const string PresentationGroup = "PresentationStates";

        /// <summary>
        /// The name of the state that represents a normal situation where no
        /// transition is currently being used.
        /// </summary>
        private const string NormalState = "Normal";

        /// <summary>
        /// The name of the state that represents the default transition.
        /// </summary>
        public const string DefaultTransitionState = "DefaultTransition";
        #endregion Visual state names

        #region Template part names
        /// <summary>
        /// The name of the control that will display the previous content.
        /// </summary>
        internal const string PreviousContentPresentationSitePartName = "PreviousContentPresentationSite";

        /// <summary>
        /// The name of the control that will display the current content.
        /// </summary>
        internal const string CurrentContentPresentationSitePartName = "CurrentContentPresentationSite";

        #endregion Template part names

        #region TemplateParts
        /// <summary>
        /// Gets or sets the current content presentation site.
        /// </summary>
        /// <value>The current content presentation site.</value>
        private ContentControl CurrentContentPresentationSite { get; set; }

        /// <summary>
        /// Gets or sets the previous content presentation site.
        /// </summary>
        /// <value>The previous content presentation site.</value>
        private ContentControl PreviousContentPresentationSite { get; set; } 
        #endregion TemplateParts

        #region public bool IsTransitioning

        /// <summary>
        /// Indicates whether the control allows writing IsTransitioning.
        /// </summary>
        private bool _allowIsTransitioningWrite;

        /// <summary>
        /// Gets a value indicating whether this instance is currently performing
        /// a transition.
        /// </summary>
        public bool IsTransitioning
        {
            get { return (bool)GetValue(IsTransitioningProperty); }
            private set
            {
                _allowIsTransitioningWrite = true;
                SetValue(IsTransitioningProperty, value);
                _allowIsTransitioningWrite = false;
            }
        }

        /// <summary>
        /// Identifies the IsTransitioning dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTransitioningProperty =
            DependencyProperty.Register(
                "IsTransitioning",
                typeof(bool),
                typeof(TransitioningContentControl),
                new PropertyMetadata(false, OnIsTransitioningPropertyChanged));

        /// <summary>
        /// IsTransitioningProperty property changed handler.
        /// </summary>
        /// <param name="d">TransitioningContentControl that changed its IsTransitioning.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsTransitioningPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TransitioningContentControl source = (TransitioningContentControl)d;

            if (!source._allowIsTransitioningWrite)
            {
                source.IsTransitioning = (bool)e.OldValue;
                throw new InvalidOperationException(Properties.Resources.TransitiotioningContentControl_IsTransitioningReadOnly);
            }
        }
        #endregion public bool IsTransitioning

        /// <summary>
        /// The storyboard that is used to transition old and new content.
        /// </summary>
        private Storyboard _currentTransition;

        #region public string Transition
        /// <summary>
        /// Gets or sets the name of the transition to use. These correspond
        /// directly to the VisualStates inside the PresentationStates group.
        /// </summary>
        public string Transition
        {
            get { return GetValue(TransitionProperty) as string; }
            set { SetValue(TransitionProperty, value); }
        }

        /// <summary>
        /// Identifies the Transition dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register(
                "Transition",
                typeof(string),
                typeof(TransitioningContentControl),
                new PropertyMetadata(OnTransitionPropertyChanged));

        /// <summary>
        /// TransitionProperty property changed handler.
        /// </summary>
        /// <param name="d">TransitioningContentControl that changed its Transition.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTransitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TransitioningContentControl source = (TransitioningContentControl)d;
            string newTransition = e.NewValue as string;

            source.OnTransitionPropertyChanged(e.OldValue as string, newTransition);
        }

        /// <summary>
        /// Called when the TransitionProperty changes.
        /// </summary>
        /// <param name="oldTransition">The old transition.</param>
        /// <param name="newTransition">The new transition.</param>
        protected virtual void OnTransitionPropertyChanged(string oldTransition, string newTransition)
        {
            ChangeTransition(newTransition);
        }
        #endregion public string Transition

        #region public bool RestartTransitionOnContentChange
        /// <summary>
        /// Gets or sets a value indicating whether the current transition
        /// will be aborted when setting new content during a transition.
        /// </summary>
        public bool RestartTransitionOnContentChange
        {
            get { return (bool)GetValue(RestartTransitionProperty); }
            set { SetValue(RestartTransitionProperty, value); }
        }

        /// <summary>
        /// Identifies the RestartTransitionOnContentChange dependency property.
        /// </summary>
        public static readonly DependencyProperty RestartTransitionProperty =
            DependencyProperty.Register(
                "RestartTransitionOnContentChange",
                typeof(bool),
                typeof(TransitioningContentControl),
                new PropertyMetadata(false, OnRestartTransitionOnContentChangePropertyChanged));

        /// <summary>
        /// RestartTransitionProperty property changed handler.
        /// </summary>
        /// <param name="d">TransitioningContentControl that changed its RestartTransitionOnContentChange.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnRestartTransitionOnContentChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TransitioningContentControl) d).OnRestartTransitionOnContentChangeChanged((bool) e.OldValue, (bool) e.NewValue);
        }

        /// <summary>
        /// Called when the RestartTransitionProperty changes.
        /// </summary>
        /// <param name="oldValue">The old value of RestartTransitionOnContentChange.</param>
        /// <param name="newValue">The new value of RestartTransitionOnContentChange.</param>
        protected virtual void OnRestartTransitionOnContentChangeChanged(bool oldValue, bool newValue)
        {
        }
        #endregion public bool RestartTransitionOnContentChange

        #region Events
        /// <summary>
        /// Occurs when the current transition has completed.
        /// </summary>
        public event RoutedEventHandler TransitionCompleted;
        #endregion Events

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitioningContentControl"/> class.
        /// </summary>
        public TransitioningContentControl()
        {
            DefaultStyleKey = typeof(TransitioningContentControl);
        }

        /// <summary>
        /// Builds the visual tree for the TransitioningContentControl control 
        /// when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AbortTransition();

            PreviousContentPresentationSite = GetTemplateChild(PreviousContentPresentationSitePartName) as ContentControl;
            CurrentContentPresentationSite = GetTemplateChild(CurrentContentPresentationSitePartName) as ContentControl;

            VisualStateManager.GoToState(this, NormalState, false);

            ChangeTransition(Transition);

            StartTransition(null, Content);
        }

        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        /// <param name="newContent">The new value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            StartTransition(oldContent, newContent);
        }

        /// <summary>
        /// Starts the transition.
        /// </summary>
        /// <param name="oldContent">The old content.</param>
        /// <param name="newContent">The new content.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "newContent", Justification = "Should be used in the future.")]
        private void StartTransition(object oldContent, object newContent)
        {
            // both presenters must be available, otherwise a transition is useless.
            if (CurrentContentPresentationSite != null && PreviousContentPresentationSite != null)
            {
                // move old content to new previous presenter
                PreviousContentPresentationSite.Content = oldContent;

                // and start a new transition
                if (!IsTransitioning || RestartTransitionOnContentChange)
                {
                    IsTransitioning = true;
                    VisualStateManager.GoToState(this, Transition, true);
                }
            }
        }

        /// <summary>
        /// Handles the Completed event of the transition storyboard.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTransitionCompleted(object sender, EventArgs e)
        {
            AbortTransition();

            RoutedEventHandler handler = TransitionCompleted;
            if (handler != null)
            {
                handler(this, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Aborts the transition and releases the previous content.
        /// </summary>
        public void AbortTransition()
        {
            // go to normal state and release our hold on the old content.
            VisualStateManager.GoToState(this, NormalState, false);
            IsTransitioning = false;
            if (PreviousContentPresentationSite != null)
            {
                PreviousContentPresentationSite.Content = null;
            }
        }

        /// <summary>
        /// Called when the transition should be changed.
        /// </summary>
        /// <param name="newTransition">The transition to be used.</param>
        private void ChangeTransition(string newTransition)
        {
            if (_currentTransition != null)
            {
                _currentTransition.Completed -= OnTransitionCompleted;
            }

            if (IsTransitioning)
            {
                AbortTransition();
            }

            // find this transition
            VisualStateGroup presentationGroup = VisualStates.TryGetVisualStateGroup(this, PresentationGroup);
            if (presentationGroup != null)
            {
                _currentTransition = presentationGroup.States
                        .OfType<VisualState>()
                        .Where(state => state.Name == newTransition)
                        .Select(state => state.Storyboard)
                        .FirstOrDefault();
            }

            if (_currentTransition != null)
            {
                _currentTransition.Completed += OnTransitionCompleted;
            }
            else
            {
                // revert to default
                SetValue(TransitionProperty, DefaultTransitionState);
            }
        }
    }
}