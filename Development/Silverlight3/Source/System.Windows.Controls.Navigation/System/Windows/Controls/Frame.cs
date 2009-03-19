//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls
{
    /// <summary>
    /// A frame control with the ability to navigate to and from content.
    /// </summary>
    /// <seealso cref="Page"/>
    /// <QualityBand>Stable</QualityBand>
    [TemplatePart(Name = Frame.PART_FrameNextButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = Frame.PART_FramePreviousButton, Type = typeof(ButtonBase))]
    public class Frame : ContentControl
    {
        #region Static Fields and Constants

        private const string PART_FrameNextButton = "NextButton";
        private const string PART_FramePreviousButton = "PrevButton";

        #endregion

        #region Fields

        private ButtonBase _nextButton;
        private ButtonBase _previousButton;
        private NavigationService _navigationService;
        private bool _loaded;
        private bool _updatingSourceFromNavigationService;

        #endregion  Fields

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Frame()
        {
            this.DefaultStyleKey = typeof(Frame);
            this.Loaded += new RoutedEventHandler(this.Frame_Loaded);
            this._navigationService = new NavigationService(this);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when the Frame has navigated.
        /// </summary>
        public event NavigatedEventHandler Navigated
        {
            add { this._navigationService.Navigated += value; }
            remove { this._navigationService.Navigated -= value; }
        }

        /// <summary>
        /// Occurs when the Frame is starting to navigate.
        /// </summary>
        public event NavigatingCancelEventHandler Navigating
        {
            add { this._navigationService.Navigating += value; }
            remove { this._navigationService.Navigating -= value; }
        }

        /// <summary>
        /// Occurs when the an exception is raised during navigation.
        /// </summary>
        public event NavigationFailedEventHandler NavigationFailed
        {
            add { this._navigationService.NavigationFailed += value; }
            remove { this._navigationService.NavigationFailed -= value; }
        }

        /// <summary>
        /// Occurs when a navigation operation has been cancelled.
        /// </summary>
        public event NavigationStoppedEventHandler NavigationStopped
        {
            add { this._navigationService.NavigationStopped += value; }
            remove { this._navigationService.NavigationStopped -= value; }
        }

        /// <summary>
        /// Occurs when a navigation occurs within a page
        /// </summary>
        public event FragmentNavigationEventHandler FragmentNavigation
        {
            add { this._navigationService.FragmentNavigation += value; }
            remove { this._navigationService.FragmentNavigation -= value; }
        }

        #endregion Events

        #region Source Dependency Property

        /// <summary>
        /// The DependencyProperty for the Source property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(Frame), new PropertyMetadata(SourcePropertyChanged));

        /// <summary>
        /// Gets or sets the Uri of the content currently hosted in the Frame.
        /// </summary>
        /// <remarks>
        /// This value may be different from CurrentSource if you set Source and the
        /// navigation has not yet completed.  CurrentSource reflects the page currently
        /// in the frame at all times, even when an async loading operation is in progress.
        /// </remarks>
        public Uri Source
        {
            get { return this.GetValue(SourceProperty) as Uri; }
            set { this.SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Called when Source property is changed
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="e"></param>
        private static void SourcePropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Frame frame = depObj as Frame;

            // Verify frame reference is valid and we're not in design mode.
            if (frame != null &&
                !frame.IsInDesignMode() &&
                frame._loaded &&
                frame._updatingSourceFromNavigationService == false)
            {
                frame.Navigate(e.NewValue as Uri);
            }
        }

        #endregion

        #region JournalOwnership Dependency Property

        /// <summary>
        /// The DependencyProperty for the JournalOwnership property.
        /// </summary>
        public static readonly DependencyProperty JournalOwnershipProperty = DependencyProperty.Register("JournalOwnership", typeof(JournalOwnership), typeof(Frame), new PropertyMetadata(JournalOwnership.Automatic, JournalOwnershipPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating if this <see cref="Frame"/> owns its own journal, or attempts
        /// to integrate with the browser journal.  See <see cref="JournalOwnership"/> for a description of
        /// the options.
        /// </summary>
        public JournalOwnership JournalOwnership
        {
            get { return (JournalOwnership)this.GetValue(JournalOwnershipProperty); }
            set { this.SetValue(JournalOwnershipProperty, value); }
        }

        private static void JournalOwnershipPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Frame frame = depObj as Frame;
            if (depObj != null)
            {
                try
                {
                    frame.NavigationService.InitializeJournal();
                }
                catch (Exception)
                {
                    frame.JournalOwnership = (JournalOwnership)e.OldValue;
                    throw;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether or not the Frame can navigate backward.
        /// </summary>
        public bool CanGoBack
        {
            get { return this._navigationService.CanGoBack; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the Frame can navigate forward.
        /// </summary>
        public bool CanGoForward
        {
            get { return this._navigationService.CanGoForward; }
        }

        /// <summary>
        /// Gets the Uri of the content currently hosted in the Frame.
        /// </summary>
        /// <remarks>
        /// This value may be different from Source if you set Source and the
        /// navigation has not yet completed.  CurrentSource reflects the page currently
        /// in the frame at all times, even when an async loading operation is in progress.
        /// </remarks>
        public Uri CurrentSource
        {
            get { return this._navigationService.CurrentSource; }
        }

        internal NavigationService NavigationService
        {
            get { return this._navigationService; }
        }

        #endregion Properties

        #region Methods

        internal bool IsInDesignMode()
        {

            return Application.Current == null ||
                   Application.Current.GetType() == typeof(Application) ||
                   DesignerProperties.GetIsInDesignMode(this) == true;
        }

        /// <summary>
        /// This will check for deep link values in the URL if the Frame's 
        /// IJournal is integrated with the browser.
        /// </summary>
        /// <returns>A value indicating whether or not deep links were found.</returns>
        internal bool ApplyDeepLinks()
        {
            return this.NavigationService.Journal.CheckForDeeplinks();
        }

        /// <summary>
        /// StopLoading aborts asynchronous navigations that haven't been processed yet.
        /// The Stopped event is fired only if the navigation was aborted.
        /// </summary>
        public void StopLoading()
        {
            this._navigationService.StopLoading();
        }

        /// <summary>
        /// Navigates the Frame to the previous journal entry in the history stack.
        /// </summary>
        public void GoBack()
        {
            this._navigationService.GoBack();
        }

        /// <summary>
        /// Navigates the Frame to the next journal entry in the history stack.
        /// </summary>
        public void GoForward()
        {
            this._navigationService.GoForward();
        }

        /// <summary>
        /// Navigates to the provided URI.
        /// </summary>
        /// <param name="uri">A URI representing an IPageControl instance to display in the Frame.</param>
        /// <returns>True if the navigation was begun successfully, false if it was not.</returns>
        public bool Navigate(Uri uri)
        {
            return this._navigationService.Navigate(uri);
        }

        /// <summary>
        /// Hook our template parts and store them off for easy access later
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Unhook and rehook our Next button
            if (this._nextButton != null)
            {
                this._nextButton.Click -= new RoutedEventHandler(this.PART_nextButton_Click);
            }

            this._nextButton = GetTemplateChild(Frame.PART_FrameNextButton) as ButtonBase;
            if (this._nextButton != null)
            {
                this._nextButton.Click += new RoutedEventHandler(this.PART_nextButton_Click);
            }

            if (this._previousButton != null)
            {
                this._previousButton.Click -= new RoutedEventHandler(this.PART_previousButton_Click);
            }

            this._previousButton = GetTemplateChild(Frame.PART_FramePreviousButton) as ButtonBase;

            if (this._previousButton != null)
            {
                this._previousButton.Click += new RoutedEventHandler(this.PART_previousButton_Click);
            }
        }

        internal void UpdateSourceFromNavigationService(Uri newSource)
        {
            if (this.Source != newSource)
            {
                this._updatingSourceFromNavigationService = true;
                this.SetValue(SourceProperty, newSource);
                this._updatingSourceFromNavigationService = false;
            }
        }

        /// <summary>
        /// Called when the Frame.Loaded event fires.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            // Set loaded flag
            this._loaded = true;

            this._navigationService.InitializeJournal();
            this._navigationService.InitializeUriMapper();
            this._navigationService.InitializeContentLoader();

            // Check if source property was set
            if (!this.ApplyDeepLinks() &&
                this.Source != null &&
                !this.IsInDesignMode())
            {
                this.Navigate(this.Source);
            }
        }

        /// <summary>
        /// Next button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_nextButton_Click(object sender, RoutedEventArgs e)
        {
            this.GoForward();
        }

        /// <summary>
        /// Previous button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_previousButton_Click(object sender, RoutedEventArgs e)
        {
            this.GoBack();
        }

        #endregion Methods
    }
}
