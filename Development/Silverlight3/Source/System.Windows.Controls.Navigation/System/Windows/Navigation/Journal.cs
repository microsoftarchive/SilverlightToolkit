//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Navigation
{
    /// <summary>
    /// Journal used to manage a history list of JournalEntry items.
    /// </summary>
    internal class Journal
    {
        #region Fields

        /// <summary>
        /// Synchronization lock object.
        /// </summary>
        private readonly object _syncLock = new object();

        private JournalEntry _currentEntry;

        private Stack<JournalEntry> _forwardStack = new Stack<JournalEntry>();

        private Stack<JournalEntry> _backStack = new Stack<JournalEntry>();

        /// <summary>
        /// Used to indicate whether or not to suppress navigation events.
        /// </summary>
        /// <remarks>
        /// This is used internally to avoid redundant browser navigation calls after deep link values are detected.
        /// </remarks>
        private bool _suppressNavigationEvent;

        /// <summary>
        /// Internal event handler reference used to sign up to the UriFragmentHelper.Navigated event.
        /// </summary>
        /// <remarks>
        /// The event handler constructed here will use a weak reference to self in order to allow for this instance to be collected.
        /// </remarks>
        private EventHandler _weakRefEventHandler;


        #endregion Fields

        #region Constructors & Destructor

        internal Journal()
        {
            this.InitializeNavigationHandler();
        }

        ~Journal()
        {
            if (this._weakRefEventHandler != null)
            {
                UriFragmentHelper.Navigated -= this._weakRefEventHandler;
            }
        }

        #endregion

        #region Events

        internal event EventHandler<JournalEventArgs> Navigated;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets a value indicating whether or not this journal instance is
        /// integrated with the browser.
        /// </summary>
        internal bool BrowserIntegrated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether or not the Journal instance
        /// can navigate backward.
        /// </summary>
        internal bool CanGoBack
        {
            get { return this._backStack.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the Journal instance
        /// can navigate forward.
        /// </summary>
        internal bool CanGoForward
        {
            get { return (this._forwardStack.Count > 0); }
        }

        /// <summary>
        /// Gets the current JournalEntry or null if no history items exist.
        /// </summary>
        internal JournalEntry CurrentEntry
        {
            get
            {
                return this._currentEntry;
            }
        }

        /// <summary>
        /// Gets a stack of back entries in this journal
        /// </summary>
        internal Stack<JournalEntry> BackStack
        {
            get { return this._backStack; }
        }

        /// <summary>
        /// Gets a stack of forward entries in this journal
        /// </summary>
        internal Stack<JournalEntry> ForwardStack
        {
            get { return this._forwardStack; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds a new JournalEntry to the history stack.
        /// </summary>
        /// <param name="journalEntry">A new JournalEntry to add to the history stack.</param>
        /// <remarks>
        /// Any JournalEntry items existing on the ForwardStack will be removed.
        /// </remarks>
        internal void AddHistoryPoint(JournalEntry journalEntry)
        {
            Guard.ArgumentNotNull(journalEntry, "journalEntry");

            lock (this._syncLock)
            {
                this._forwardStack.Clear();

                if (this._currentEntry != null)
                {
                    this._backStack.Push(this._currentEntry);
                }

                this._currentEntry = journalEntry;
            }

            this.UpdateObservables(journalEntry, NavigationMode.New);

            this.NavigateBrowser(this.CurrentEntry);
        }

        /// <summary>
        /// Forces the BrowserJournal to check for deep-link values in 
        /// the browser address URI.
        /// </summary>
        /// <returns>
        /// A boolean indicating whether or not a deep-link value was found.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Deeplinks", Justification = "This is the correct spelling.")]
        internal bool CheckForDeeplinks()
        {
            if (this.BrowserIntegrated)
            {
                string currentState = UriParsingHelper.InternalUriFromExternalValue(UriFragmentHelper.CurrentFragment);
                if (!String.IsNullOrEmpty(currentState))
                {
                    this.AddHistoryPointIfDifferent(currentState);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Navigates the Journal instance back to the previous 
        /// JournalEntry item in the history stack.
        /// </summary>
        /// <remarks>
        /// If CanGoBack is false, this method will throw an InvalidOperationException.
        /// </remarks>
        internal void GoBack()
        {
            if (this.CanGoBack == false)
            {
                throw new InvalidOperationException(Resource.Journal_CannotGoBack);
            }

            lock (this._syncLock)
            {
                this._forwardStack.Push(this._currentEntry);
                this._currentEntry = this._backStack.Pop();
            }

            this.UpdateObservables(this._currentEntry, NavigationMode.Back);

            this.NavigateBrowser(this.CurrentEntry);
        }

        /// <summary>
        /// Navigates the Journal instance forward to the next 
        /// JournalEntry item in the history stack.
        /// </summary>
        /// <remarks>
        /// If CanGoForward is false, this method will throw an InvalidOperationException.
        /// </remarks>
        internal void GoForward()
        {
            if (this.CanGoForward == false)
            {
                throw new InvalidOperationException(Resource.Journal_CannotGoForward);
            }

            lock (this._syncLock)
            {
                this._backStack.Push(this._currentEntry);
                this._currentEntry = this._forwardStack.Pop();
            }

            this.UpdateObservables(this._currentEntry, NavigationMode.Forward);

            this.NavigateBrowser(this.CurrentEntry);
        }

        /// <summary>
        /// Updates the browser location to reflect the Journal state, if the Journal is browser integrated.
        /// </summary>
        /// <param name="journalEntry">JournalEntry used to update the browser location.</param>
        private void NavigateBrowser(JournalEntry journalEntry)
        {
            if (this.BrowserIntegrated)
            {
                if (this._suppressNavigationEvent == false)
                {
                    // Verify browser integration is enabled
                    if (UriFragmentHelper.IsEnabled == false)
                    {
                        Debug.WriteLine(Resource.Journal_NavigateBrowserFailed);
                        return;
                    }

                    string state = journalEntry.Source == null
                                    ? string.Empty
                                    : UriParsingHelper.InternalUriToExternalValue(journalEntry.Source);
                    UriFragmentHelper.Navigate(state, journalEntry.Name);
                }
            }
        }

        /// <summary>
        /// Occurs when the UriFragmentHelper has navigated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">Empty event args.</param>
        private void Browser_Navigated(object sender, EventArgs eventArgs)
        {
            if (this.BrowserIntegrated)
            {
                this.AddHistoryPointIfDifferent(UriParsingHelper.InternalUriFromExternalValue(UriFragmentHelper.CurrentFragment));
            }
        }

        /// <summary>
        /// Conditionally adds a new history point if the new state information differs from the current journal entry Uri value.
        /// </summary>
        /// <param name="newState">An updated state value to examine.</param>
        private void AddHistoryPointIfDifferent(string newState)
        {
            // Check if different from our current state
            string currentState = String.Empty;
            if (this.CurrentEntry != null && this.CurrentEntry.Source != null)
            {
                currentState = this.CurrentEntry.Source.OriginalString;
            }

            if (string.Equals(newState, currentState, StringComparison.Ordinal) == false)
            {
                this._suppressNavigationEvent = true;
                this.AddHistoryPoint(new JournalEntry(string.Empty, new Uri(newState, UriKind.RelativeOrAbsolute)));
                this._suppressNavigationEvent = false;
            }
        }

        /// <summary>
        /// Signs up for events on UriFragmentHelper.Navigated using a weak-reference based event handler.
        /// </summary>
        private void InitializeNavigationHandler()
        {
            WeakReference thisWeak = new WeakReference(this);
            this._weakRefEventHandler =
                (sender, args) =>
                {
                    var journal = thisWeak.Target as Journal;
                    if (journal != null)
                    {
                        journal.Browser_Navigated(sender, args);
                    }
                };

            UriFragmentHelper.Navigated += this._weakRefEventHandler;
        }

        /// <summary>
        /// Raises the Navigated event.
        /// </summary>
        /// <param name="name">A value representing a journal entry name.</param>
        /// <param name="uri">A value representing a journal entry URI.</param>
        /// <param name="mode">A value representing a journal entry navigation mode.</param>
        protected void OnNavigated(string name, Uri uri, NavigationMode mode)
        {
            EventHandler<JournalEventArgs> eventHandler = this.Navigated;
            if (eventHandler != null)
            {
                JournalEventArgs args = new JournalEventArgs(name, uri, mode);
                eventHandler(this, args);
            }
        }

        /// <summary>
        /// Updates observable properties of the journal.
        /// </summary>
        /// <param name="currentEntry">The current journal entry.</param>
        /// <param name="mode">The mode of navigation that triggered the update.</param>
        private void UpdateObservables(JournalEntry currentEntry, NavigationMode mode)
        {
            this.OnNavigated(currentEntry.Name, currentEntry.Source, mode);
        }

        #endregion Methods
    }
}
