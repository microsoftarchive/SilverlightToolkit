//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Browser;

namespace System.Windows.Navigation
{
    /// <summary>
    /// Used to programmatically initiate navigation, primarily from within a <see cref="Page"/>.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public sealed class NavigationService
    {
        #region Fields

        private Frame _host;
        private Uri _currentSource;
        private Uri _currentSourceAfterMapping;
        private Uri _source;
        private Journal _journal;
        private ContentLoaderBase _contentLoader;
        private UriMapperBase _uriMapper;
        private NavigationOperation _currentNavigation;
        private bool _lastNavigationWasError;

        #endregion

        #region Constructors

        /// <summary>
        /// Internal class used to host content and handles all navigations
        /// </summary>
        /// <param name="nav">
        /// Parent navigator that uses and owns this NS. (It's either NavigationWindow or Frame.)
        /// </param>
        internal NavigationService(Frame nav)
        {
            Guard.ArgumentNotNull(nav, "nav");
            this._host = nav;

            // The following lines are to ensure the UriFragmentHelper, and the BrowserNavigationProxy
            // get their static members initialized at this time, which ensures the scripts are written out
            // to the page before anyone uses a NavigationService.
            if (UriFragmentHelper.IsEnabled)
            {
                UriFragmentHelper.BrowserNavigationProxy current = UriFragmentHelper.BrowserNavigationProxy.Current;
            }
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// This event is fired when an error is encountered during a navigation
        /// </summary>
        public event NavigationFailedEventHandler NavigationFailed;

        /// <summary>
        /// event NavigatingCancelEventHandler NavigationService.Navigating
        /// </summary>
        /// <value></value>
        public event NavigatingCancelEventHandler Navigating;

        /// <summary>
        /// event NavigatedEventHandler NavigationService.Navigated
        /// </summary>
        /// <value></value>
        public event NavigatedEventHandler Navigated;

        /// <summary>
        /// event NavigationStoppedEventHandler NavigationService.NavigationStopped
        /// </summary>
        /// <value></value>
        public event NavigationStoppedEventHandler NavigationStopped;

        /// <summary>
        /// event FragmentNavigationEventHandler NavigationService.FragmentNavigation
        /// </summary>
        public event FragmentNavigationEventHandler FragmentNavigation;

        #endregion

        #region NavigationService Attached Property

        /// <summary>
        /// Attached DependencyProperty. It gives an element the NavigationService of the navigation container it's in.
        /// </summary>
        internal static readonly DependencyProperty NavigationServiceProperty =
                DependencyProperty.RegisterAttached(
                        "NavigationService",
                        typeof(NavigationService),
                        typeof(NavigationService),
                        new PropertyMetadata(null));

        /// <summary>
        /// Gets NavigationService of the navigation container the given dependencyObject is in.
        /// </summary>
        /// <param name="dependencyObject">The object to retrieve the attached <see cref="NavigationService"/> for</param>
        /// <returns>The <see cref="NavigationService"/> attached to the <paramref name="dependencyObject"/></returns>
        public static NavigationService GetNavigationService(DependencyObject dependencyObject)
        {
            Guard.ArgumentNotNull(dependencyObject, "dependencyObject");

            return dependencyObject.GetValue(NavigationServiceProperty) as NavigationService;
        }

        #endregion

        #region Properties

        internal Journal Journal
        {
            get { return this._journal; }
        }

        internal ContentLoaderBase ContentLoader
        {
            get { return this._contentLoader; }
        }

        internal UriMapperBase UriMapper
        {
            get { return this._uriMapper; }
        }

        internal Frame Host
        {
            get { return this._host; }
        }

        /// <summary>
        /// Gets or sets the source Uri
        /// </summary>
        /// <value></value>
        public Uri Source
        {
            get
            {
                return this._source;
            }

            set
            {
                this._source = value;
                this.Navigate(value);
            }
        }

        /// <summary>
        /// Gets the Current Source Uri
        /// </summary>
        /// <value></value>
        public Uri CurrentSource
        {
            get { return this._currentSource; }
            internal set { this._currentSource = value; }
        }

        /// <summary>
        /// Gets a value used to determine if there are any entries on the forward stack
        /// </summary>
        /// <value></value>
        public bool CanGoForward
        {
            get { return this._journal.CanGoForward; }
        }

        /// <summary>
        /// Gets a value used to determine if there are any entries on the back stack
        /// </summary>
        /// <value></value>
        public bool CanGoBack
        {
            get { return this._journal.CanGoBack; }
        }

        #endregion

        #region Methods

        internal void InitializeJournal()
        {
            Journal originalJournal = this._journal;

            //Find the outer frame (if there is one)
            Frame outerFrame = null;
            DependencyObject walker = VisualTreeHelper.GetParent(this.Host);

            // Walk up tree to find a parent navigator.
            while (walker != null)
            {
                outerFrame = walker as Frame;
                if (outerFrame != null)
                {
                    break;
                }

                walker = VisualTreeHelper.GetParent(walker);
            }

            if (!UriFragmentHelper.IsEnabled || this.Host.JournalOwnership == JournalOwnership.OwnsJournal)
            {
                this._journal = new Journal();
            }
            // If we're set to use the parent journal, one of the following is true:
            // 1) We're a top-level Frame (i.e. not nested within another Frame) - in this case, integrate with the browser, as it is logically our "parent"
            // 2) We're not a top-level Frame, in which case browser integration is not supported, so throw an exception.  If graceful fall-back is desired, they can use JournalOwnership.Automatic instead.
            else if (this.Host.JournalOwnership == JournalOwnership.UsesParentJournal &&
                outerFrame != null)
            {
                throw new InvalidOperationException(Resource.NavigationService_JournalOwnership_UsesParentJournal_OnlyTopLevel);
            }
            // If we're set to automatic (the default), then one of the following is true:
            // 1) We're a top-level Frame (i.e. not nested within another Frame) - in this case, integrate with the browser, as it is logically our "parent"
            // 2) We're not a top-level Frame, in which case browser integration is not supported, so fall-back to an internal journal silently.
            else if (this.Host.JournalOwnership == JournalOwnership.Automatic &&
                outerFrame != null)
            {
                this._journal = new Journal();
            }
            else
            {
                // If we get this far, then we must be integrating with the browser:
                this._journal = new Journal() { BrowserIntegrated = true };
            }

            if (this._journal != originalJournal)
            {
                if (originalJournal != null)
                {
                    originalJournal.Navigated -= this.Journal_Navigated;
                }
                this._journal.Navigated += this.Journal_Navigated;
            }
        }

        internal void InitializeUriMapper()
        {

            this._uriMapper = this.WalkResourceTreeForKey("uriMapper") as UriMapperBase;
        }

        internal void InitializeContentLoader()
        {

            this._contentLoader = (this.WalkResourceTreeForKey("contentLoader") as ContentLoaderBase) ??
                                  new PageResourceContentLoader();
        }

        private object WalkResourceTreeForKey(string key)
        {
            FrameworkElement fe = this.Host;

            while (fe != null)
            {
                if (fe.Resources.Contains(key))
                {
                    return fe.Resources[key];
                }
                fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
            }

            if (Application.Current.Resources.Contains(key))
            {
                return Application.Current.Resources[key];
            }

            return null;
        }

        /// <summary>
        /// Navigate to source
        /// </summary>
        /// <param name="source">The Uri to begin navigating to</param>
        /// <returns>True if the navigation was begun successfully, false if it was not.</returns>
        public bool Navigate(Uri source)
        {
            return this.NavigateCore(source, NavigationMode.New, false);
        }

        private void Journal_Navigated(object sender, JournalEventArgs args)
        {
            NavigationOperation navOp = this._currentNavigation;
            if (navOp == null || navOp.SuppressNotifications == false)
            {
                this.NavigateCore(args.Uri, args.NavigationMode, true);
            }
        }

        private bool NavigateCore(Uri uri, NavigationMode mode, bool suppressJournalAdd)
        {
            if (uri == null)
            {
                this.RaiseNavigationFailed(uri, new ArgumentNullException("uri", Resource.NavigationService_NavigationToANullUriIsNotSupported));
                return false;
            }

            // Make sure we're on the UI thread because of the DependencyProperties we use.
            if (!this.Host.Dispatcher.CheckAccess())
            {
                // Move to UI thread
                this.Host.Dispatcher.BeginInvoke(() => this.NavigateCore(uri, mode, suppressJournalAdd));
                return true; 
            }

            // Stop in-progress navigation
            this.StopLoading();

            Uri mappedUri = uri;
            // If the Uri is only a fragment, mapping does not take place
            if (!UriParsingHelper.InternalUriIsFragment(uri))
            {
                UriMapperBase mapper = this._uriMapper;
                if (mapper != null)
                {
                    mappedUri = mapper.MapUri(uri);
                }
            }

            Uri mergedUriAfterMapping = UriParsingHelper.InternalUriMerge(this._currentSourceAfterMapping, mappedUri) ?? mappedUri;
            Uri mergedUri = UriParsingHelper.InternalUriMerge(this._currentSource, uri) ?? uri;

            // If we're navigating to just a fragment (i.e. "#frag1") or to a page which differs only in the fragment
            // (i.e. "Page.xaml?id=123" to "Page.xaml?id=123#frag1") then complete navigation without involving the content loader
            bool isFragmentNavigationOnly =
                    UriParsingHelper.InternalUriIsFragment(mappedUri) ||
                    UriParsingHelper.InternalUriGetAllButFragment(mappedUri) == UriParsingHelper.InternalUriGetAllButFragment(this._currentSourceAfterMapping);

            // It's only fragment navigation if there is currently content in the Host
            isFragmentNavigationOnly = isFragmentNavigationOnly && this._host.Content != null;

            // Check to see if anyone wants to cancel
            if (mode == NavigationMode.New)
            {
                if (this.RaiseNavigating(mergedUri, mode, isFragmentNavigationOnly) == true)
                {
                    // Someone stopped us
                    this.RaiseNavigationStopped(null, mergedUri);
                    return false;
                }
            }

            if (!UriParsingHelper.InternalUriIsNavigable(mappedUri))
            {
                this.RaiseNavigationFailed(uri, new ArgumentException(Resource.NavigationService_UriNotNavigable, "uri"));
                return false;
            }

            // Start a new navigation load...
            this._currentNavigation = new NavigationOperation(mergedUriAfterMapping, mergedUri, uri, mode, suppressJournalAdd);
            
            // If the last navigation was an error, even if the Uri's match, we should do a full navigation to avoid leaving the
            // error visuals in the Frame.
            if (isFragmentNavigationOnly && !this._lastNavigationWasError)
            {
                // If we're navigating only to a fragment (e.g. "#frag2") then the Uri to journal should be that merged with the base uri
                if (UriParsingHelper.InternalUriIsFragment(uri))
                {
                    this._currentNavigation.UriForJournal = mergedUri;
                }
                this.CompleteNavigation(null, NavigationMode.New);
            }
            else
            {
                this._contentLoader.BeginLoad(mergedUriAfterMapping, this.ContentLoader_BeginLoad_Callback, this._currentNavigation);
            }
            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The exception should always be caught and used to raise a failure event instead.")]
        private void ContentLoader_BeginLoad_Callback(IAsyncResult result)
        {
            DependencyObject content = null;
            NavigationOperation asyncNavigationOperationCompleted = result.AsyncState as NavigationOperation;

            NavigationOperation navOp = this._currentNavigation;
            if (navOp == null || navOp.Uri != asyncNavigationOperationCompleted.Uri)
            {
                // We already fired NavigationStopped in NavigateCore(), so just return without doing anything
                return;
            }

            try
            {
                content = this._contentLoader.EndLoad(result) as DependencyObject;
            }
            catch (Exception ex)
            {
                // Content loader experienced an error, so throw Failed
                this.RaiseNavigationFailed(asyncNavigationOperationCompleted.UriBeforeMapping, ex);
                return;
            }

            // Content loader was successful, so complete navigation

            // Create a new navigation context
            JournalEntry.SetNavigationContext(content, new NavigationContext(asyncNavigationOperationCompleted.UriBeforeMapping, UriParsingHelper.InternalUriParseQueryStringToDictionary(asyncNavigationOperationCompleted.Uri)));
            content.SetValue(NavigationServiceProperty, this);

            // Complete navigation operation
            this.CompleteNavigation(content, navOp.Mode);
        }

        private void CompleteNavigation(DependencyObject content, NavigationMode mode)
        {
            Uri uri = null;
            string pageTitle = null;
            Page existingContentPage = this._host.Content as Page;
            Page newContentPage = content as Page;

            pageTitle = JournalEntry.GetName(content ?? this._host.Content as DependencyObject);

            NavigationOperation navOp = this._currentNavigation;
            if (navOp != null)
            {
                // Set uri
                uri = navOp.UriBeforeMapping;

                navOp.SuppressNotifications = true;

                this.CurrentSource = navOp.UriForJournal;
                this._source = navOp.UriBeforeMapping;
                this._currentSourceAfterMapping = navOp.Uri;
                this.Host.UpdateSourceFromNavigationService(navOp.UriForJournal);

                // Check if this is a 'New' operation
                if (mode == NavigationMode.New && navOp.Uri != null && navOp.SuppressJournalAdd == false)
                {
                    JournalEntry je = new JournalEntry(pageTitle ?? uri.OriginalString, navOp.UriForJournal);
                    this.Journal.AddHistoryPoint(je);
                }

                navOp.SuppressNotifications = false;
            }

            this._lastNavigationWasError = false;
            if (this.Journal.BrowserIntegrated)
            {
                HtmlPage.Document.SetProperty("title", pageTitle ?? uri.OriginalString);
            }
            if (content == null)
            {
                // We're navigating to a fragment in the current page, so for WPF compatibility, fire FragmentNavigation THEN Navigated

                this.RaiseFragmentNavigation(UriParsingHelper.InternalUriGetFragment(uri));
                this.RaiseNavigated(content, uri);
            }
            else
            {
                // We're navigating to a fragment in the new content, so let the host load content, then for WPF compatibility,
                // fire Navigated THEN FragmentNavigation
                this.Host.Content = content;
                this.RaiseNavigated(content, uri);
                string fragment = UriParsingHelper.InternalUriGetFragment(uri);
                if (!String.IsNullOrEmpty(fragment))
                {
                    this.RaiseFragmentNavigation(fragment);
                }
            }

            if (existingContentPage != null && content != null)
            {
                existingContentPage.InternalOnNavigatedFrom(new NavigationEventArgs(content, uri));
            }

            if (newContentPage != null)
            {
                newContentPage.InternalOnNavigatedTo(new NavigationEventArgs(content, uri));
            }

            // Release reference
            this._currentNavigation = null;
        }
        
        /// <summary>
        /// Navigate to the next entry in the Journal
        /// </summary>
        /// <value></value>
        public void GoForward()
        {
            this.GoForwardBackCore(NavigationMode.Forward, this.CanGoForward, this.Journal.ForwardStack, Resource.NavigationService_CannotGoForward);
        }

        /// <summary>
        /// Navigate to the next entry in the Journal
        /// </summary>
        /// <value></value>
        public void GoBack()
        {
            this.GoForwardBackCore(NavigationMode.Back, this.CanGoBack, this.Journal.BackStack, Resource.NavigationService_CannotGoBack);
        }

        /// <summary>
        /// StopLoading aborts asynchronous navigations that haven't been processed yet or that are
        /// still being downloaded. SopLoading does not abort parsing of the downloaded streams.
        /// The NavigationStopped event is fired only if the navigation was aborted.
        /// </summary>
        /// <value></value>
        public void StopLoading()
        {
            NavigationOperation navOp = this._currentNavigation;
            if (navOp != null)
            {
                this.RaiseNavigationStopped(null, navOp.Uri);

                // Release current context
                this._currentNavigation = null;
            }
        }

        private void GoForwardBackCore(NavigationMode mode, bool canDoIt, Stack<JournalEntry> entries, string onFailureText)
        {
            if (canDoIt)
            {
                JournalEntry entry = entries.Peek();

                bool isFragmentNavigationOnly =
                    UriParsingHelper.InternalUriIsFragment(entry.Source) ||
                    UriParsingHelper.InternalUriGetAllButFragment(entry.Source) == UriParsingHelper.InternalUriGetAllButFragment(this._currentSourceAfterMapping);

                if (this.RaiseNavigating(entry.Source, mode, isFragmentNavigationOnly) == false)
                {
                    if (mode == NavigationMode.Back)
                    {
                        this.Journal.GoBack();
                    }
                    else
                    {
                        this.Journal.GoForward();
                    }
                }
                else
                {
                    this.RaiseNavigationStopped(null, entry.Source);
                }
            }
            else
            {
                Exception ex = new InvalidOperationException(onFailureText);
                this.RaiseNavigationFailed(null, ex);
            }
        }

        #region Event handlers

        /// <summary>
        /// Raises the Navigated event asynchronously.
        /// </summary>
        /// <param name="content">A reference to the object content that is being navigated to.</param>
        /// <param name="uri">A URI value representing the navigation content.</param>
        private void RaiseNavigated(object content, Uri uri)
        {
            NavigatedEventHandler eventHandler = this.Navigated;

            if (eventHandler != null)
            {
                NavigationEventArgs eventArgs = new NavigationEventArgs(content, uri);
                this.Host.Dispatcher.BeginInvoke(() => eventHandler(this, eventArgs));
            }
        }

        /// <summary>
        /// Raises the Navigating event synchronously.
        /// </summary>
        /// <param name="uri">A URI value representing the navigation content.</param>
        /// <param name="mode">The mode of navigation being initiated (New, Forward or Back)</param>
        /// <param name="isFragmentNavigationOnly">True if this navigation is only a fragment navigation on the existing page, false if it is any other type of navigation</param>
        /// <returns>A value indicating whether or not to cancel the navigation operation.</returns>
        private bool RaiseNavigating(Uri uri, NavigationMode mode, bool isFragmentNavigationOnly)
        {
            NavigatingCancelEventHandler eventHandler = this.Navigating;
            bool canceled = false;

            if (eventHandler != null)
            {
                NavigatingCancelEventArgs eventArgs = new NavigatingCancelEventArgs(uri, mode);

                eventHandler(this, eventArgs);

                canceled = eventArgs.Cancel;
            }

            if (!isFragmentNavigationOnly)
            {
                Page p = this._host.Content as Page;
                if (p != null)
                {
                    NavigatingCancelEventArgs eventArgs = new NavigatingCancelEventArgs(uri, mode);
                    p.InternalOnNavigatingFrom(eventArgs);
                    canceled |= eventArgs.Cancel;
                }
            }

            return canceled;
        }

        /// <summary>
        /// Raises the Failed event synchronously.
        /// </summary>
        /// <param name="uri">A URI value representing the navigation content.</param>
        /// <param name="exception">The error that occurred</param>
        private void RaiseNavigationFailed(Uri uri, Exception exception)
        {
            this._lastNavigationWasError = true;

            NavigationFailedEventHandler eventHandler = this.NavigationFailed;
            NavigationFailedEventArgs eventArgs = new NavigationFailedEventArgs(uri, exception);

            if (eventHandler != null)
            {
                eventHandler(this, eventArgs);
            }

            if (!eventArgs.Handled)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Raises the Stopped event asynchronously.
        /// </summary>
        /// <param name="content">A reference to the object content that is being navigated to.</param>
        /// <param name="uri">A URI value representing the navigation content.</param>
        private void RaiseNavigationStopped(object content, Uri uri)
        {
            NavigationStoppedEventHandler eventHandler = this.NavigationStopped;

            if (eventHandler != null)
            {
                NavigationEventArgs eventArgs = new NavigationEventArgs(content, uri);
                this.Host.Dispatcher.BeginInvoke(() => eventHandler(this, eventArgs));
            }
        }

        /// <summary>
        /// Raises the Fragment Navigation event asynchronously
        /// </summary>
        /// <param name="fragment">The fragment that was navigated to</param>
        private void RaiseFragmentNavigation(string fragment)
        {
            FragmentNavigationEventHandler eventHandler = this.FragmentNavigation;

            if (eventHandler != null)
            {
                FragmentNavigationEventArgs eventArgs = new FragmentNavigationEventArgs(fragment);
                this.Host.Dispatcher.BeginInvoke(() => eventHandler(this, eventArgs));
            }

            Page p = this._host.Content as Page;
            if (p != null)
            {
                FragmentNavigationEventArgs eventArgs = new FragmentNavigationEventArgs(fragment);
                this.Host.Dispatcher.BeginInvoke(() => p.InternalOnFragmentNavigation(eventArgs));
            }
        }

        #endregion

        #endregion

        #region Nested Classes, Structs

        /// <summary>
        /// Class used within the Frame to manage navigation operations.
        /// </summary>
        private class NavigationOperation
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="uri"></param>
            /// <param name="uriBeforeMapping"></param>
            /// <param name="uriForJournal"></param>
            /// <param name="mode"></param>
            /// <param name="suppressJournalUpdate"></param>
            public NavigationOperation(Uri uri, Uri uriBeforeMapping, Uri uriForJournal, NavigationMode mode, bool suppressJournalUpdate)
            {
                this.Uri = uri;
                this.UriBeforeMapping = uriBeforeMapping;
                this.UriForJournal = uriForJournal;
                this.Mode = mode;
                this.SuppressJournalAdd = suppressJournalUpdate;
            }

            /// <summary>
            /// Gets or sets Uri used in the navigation operation, after passing through the DefaultUriMapper
            /// </summary>
            public Uri Uri
            {
                get;
                set;
            }

            public Uri UriBeforeMapping
            {
                get;
                set;
            }

            public Uri UriForJournal { get; set; }

            /// <summary>
            /// Gets or sets NavigationMode used in the current operation.
            /// </summary>
            public NavigationMode Mode
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether or not the operation is altering the Source property.
            /// </summary>
            public bool SuppressNotifications
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the Journal should be updated based on this navigation operation
            /// </summary>
            public bool SuppressJournalAdd
            {
                get;
                set;
            }
        }

        #endregion Nested Classes, Structs
    }
}
