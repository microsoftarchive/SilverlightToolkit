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
using System.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDescription = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using System.Globalization;

namespace System.Windows.Navigation.UnitTests
{
    /// <summary>
    /// Unit tests for the Journal type.
    /// </summary>
    [TestClass]
    public sealed class JournalTests : SilverlightTest
    {
        #region Fields

        private EventHandler<JournalEventArgs> _eventHandler;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets an Journal instance.
        /// </summary>
        internal Journal Journal
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a count of the number of times the Navigated event was raised.
        /// </summary>
        public int NavigatedCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the value of the browser location fragment (hash) before running UriFragmentHelper tests.
        /// </summary>
        public string OriginalBrowserHash
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the value of the browser title before running UriFragmentHelper tests.
        /// </summary>
        public string OriginalBrowserTitle
        {
            get;
            private set;
        }


        #endregion Properties

        #region Methods

        #region Setup and Teardown methods

        /// <summary>
        /// Initializes the class for testing.
        /// </summary>
        [ClassInitialize]
        public void ClassInitialize()
        {
            this.OriginalBrowserHash = BrowserHelper.Location.Hash;
            this.OriginalBrowserTitle = BrowserHelper.Title;
            this._eventHandler = (sender, args) => this.NavigatedCount++;
        }

        /// <summary>
        /// Performs cleanup after running UriFragmentHelper tests.
        /// </summary>
        [ClassCleanup]
        public void ClassCleanup()
        {
            this.CleanUp();
        }

        /// <summary>
        /// Cleans up resources for the next test.
        /// </summary>
        private void CleanUp()
        {
            this.ResetBrowser();
            if (this.Journal != null)
            {
                this.Journal.Navigated -= this._eventHandler;
            }
            this.NavigatedCount = 0;
        }

        /// <summary>
        /// Initializes class properties to run a test method.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.CleanUp();
        }
        
        /// <summary>
        /// Resets the browser location fragment (hash) value to be empty.
        /// </summary>
        private void ResetBrowser()
        {
            Application.Current.Host.NavigationState = "#";
            BrowserHelper.Title = this.OriginalBrowserTitle;
        }

        private void CreateTestJournal(bool useNavigationState)
        {
            this.Journal = new Journal(useNavigationState);
            this.Journal.Navigated += this._eventHandler;
        }

        #endregion

        [TestMethod]
        [TestDescription("Verifies the initial state of the Journal.")]
        public void InitialState()
        {
            this.CreateTestJournal(false);
            EventHandler<JournalEventArgs> navigatedEvent = (sender, args) => Assert.Fail();

            // Add event handlers
            this.Journal.Navigated += navigatedEvent;

            // Verify 'current' behaviors.
            Assert.IsNull(this.Journal.CurrentEntry);

            // Verify 'back' behaviors.
            Assert.IsFalse(this.Journal.CanGoBack);
            Assert.IsNotNull(this.Journal.BackStack);
            Assert.AreEqual<int>(0, this.Journal.BackStack.Count);

            // Verify 'forward' behaviors.
            Assert.IsFalse(this.Journal.CanGoForward);
            Assert.IsNotNull(this.Journal.ForwardStack);
            Assert.AreEqual<int>(0, this.Journal.ForwardStack.Count);

            // Remove event handlers
            this.Journal.Navigated -= navigatedEvent;
        }

        #region AddHistoryPoint

        [TestMethod]
        [Asynchronous]
        [TestDescription("Verifies the behavior of Journal's AddHistoryPoint method.")]
        public void AddHistoryPoint()
        {
            this.CreateTestJournal(false);
            AddHistoryPointCore();
        }

        [TestMethod]
        [Asynchronous]
        [TestDescription("Verifies the behavior of Journal's AddHistoryPoint method when browser integrated.")]
        public void AddHistoryPointBrowserIntegrated()
        {
            this.CreateTestJournal(true);
            AddHistoryPointCore();
        }

        private void AddHistoryPointCore()
        {
            JournalEntry journalEntry = this.CreateJournalEntry();

            this.EnqueueCallback(() => this.Journal.AddHistoryPoint(journalEntry));
            this.EnqueueConditional(() => this.NavigatedCount == 1);
            this.EnqueueCallback(() =>
            {
                // Verify 'current' behaviors.
                Assert.AreEqual<JournalEntry>(journalEntry, this.Journal.CurrentEntry);
                if (this.Journal.UseNavigationState)
                {
                    Assert.AreEqual<string>(journalEntry.Name, BrowserHelper.Title);
                    Assert.AreEqual<string>(journalEntry.Source.OriginalString, BrowserHelper.Location.Hash);
                }

                // Verify 'forward' behaviors.
                Assert.IsFalse(this.Journal.CanGoForward);
                Assert.IsNotNull(this.Journal.ForwardStack);
                Assert.AreEqual<int>(0, this.Journal.ForwardStack.Count);

                // Verify 'back' behaviors.
                Assert.IsFalse(this.Journal.CanGoBack);
                Assert.IsNotNull(this.Journal.BackStack);
                Assert.AreEqual<int>(0, this.Journal.BackStack.Count);
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [TestDescription("Verifies the behavior of Journal's AddHistoryPoint method being called multiple times in succession.")]
        public void AddHistoryPointStressTest()
        {
            this.CreateTestJournal(false);
            AddHistoryPointStressTestCore();
        }

        [TestMethod]
        [Asynchronous]
        [TestDescription("Verifies the behavior of Journal's AddHistoryPoint method being called multiple times in succession when browser integrated.")]
        public void AddHistoryPointStressTestBrowserIntegrated()
        {
            this.CreateTestJournal(true);
            AddHistoryPointStressTestCore();
        }

        private void AddHistoryPointStressTestCore()
        {
            // Create a collection of test items
            const int totalItems = 100;
            List<JournalEntry> journalEntries = new List<JournalEntry>(totalItems);
            for (int i = 0; i < totalItems; i++)
            {
                journalEntries.Add(this.CreateJournalEntry());
            }

            // Add multiple journal entries
            foreach (JournalEntry journalEntry in journalEntries)
            {
                var scopedRef = journalEntry;
                this.EnqueueCallback(() => this.Journal.AddHistoryPoint(scopedRef));
            }

            this.EnqueueConditional(() => this.NavigatedCount == totalItems);
            this.EnqueueCallback(() =>
            {
                JournalEntry currentEntry = journalEntries[totalItems - 1];

                // Verify 'current' behaviors.
                Assert.AreEqual<JournalEntry>(currentEntry, this.Journal.CurrentEntry);
                if (this.Journal.UseNavigationState)
                {
                    Assert.AreEqual<string>(currentEntry.Name, BrowserHelper.Title);
                    Assert.AreEqual<string>(currentEntry.Source.OriginalString, BrowserHelper.Location.Hash);
                }

                // Verify 'forward' behaviors.
                Assert.IsFalse(this.Journal.CanGoForward);
                Assert.IsNotNull(this.Journal.ForwardStack);
                Assert.AreEqual<int>(0, this.Journal.ForwardStack.Count);

                // Verify 'back' behaviors.
                Assert.IsTrue(this.Journal.CanGoBack);
                Assert.IsNotNull(this.Journal.BackStack);
                Assert.AreEqual<int>((totalItems - 1), this.Journal.BackStack.Count);
            });
            // Move back to the very first item.
            for (int i = 0; i < totalItems - 1; i++)
            {
                this.EnqueueCallback(() => this.Journal.GoBack());
            }

            this.EnqueueConditional(() => this.NavigatedCount == ((totalItems * 2) - 1));
            this.EnqueueCallback(() =>
            {
                JournalEntry currentEntry = journalEntries[0];

                // Verify 'current' behaviors.
                Assert.AreEqual<JournalEntry>(currentEntry, this.Journal.CurrentEntry);
                if (this.Journal.UseNavigationState)
                {
                    Assert.AreEqual<string>(currentEntry.Name, BrowserHelper.Title);
                    Assert.AreEqual<string>(currentEntry.Source.OriginalString, BrowserHelper.Location.Hash);
                }

                // Verify 'forward' behaviors.
                Assert.IsTrue(this.Journal.CanGoForward);
                Assert.IsNotNull(this.Journal.ForwardStack);
                Assert.AreEqual<int>((totalItems - 1), this.Journal.ForwardStack.Count);

                // Verify 'back' behaviors.
                Assert.IsFalse(this.Journal.CanGoBack);
                Assert.IsNotNull(this.Journal.BackStack);
                Assert.AreEqual<int>(0, this.Journal.BackStack.Count);
            });

            // Move foward to the very last item
            for (int i = 0; i < totalItems - 1; i++)
            {
                this.EnqueueCallback(() => this.Journal.GoForward());
            }

            this.EnqueueConditional(() => this.NavigatedCount == ((totalItems * 3) - 2));
            this.EnqueueCallback(() =>
            {
                JournalEntry currentEntry = journalEntries[totalItems - 1];

                // Verify 'current' behaviors.
                Assert.AreEqual<JournalEntry>(currentEntry, this.Journal.CurrentEntry);
                if (this.Journal.UseNavigationState)
                {
                    Assert.AreEqual<string>(currentEntry.Name, BrowserHelper.Title);
                    Assert.AreEqual<string>(currentEntry.Source.OriginalString, BrowserHelper.Location.Hash);
                }

                // Verify 'forward' behaviors.
                Assert.IsFalse(this.Journal.CanGoForward);
                Assert.IsNotNull(this.Journal.ForwardStack);
                Assert.AreEqual<int>(0, this.Journal.ForwardStack.Count);

                // Verify 'back' behaviors.
                Assert.IsTrue(this.Journal.CanGoBack);
                Assert.IsNotNull(this.Journal.BackStack);
                Assert.AreEqual<int>((totalItems - 1), this.Journal.BackStack.Count);
            });

            this.EnqueueTestComplete();
        }

        #endregion AddHistoryPoint

        #region GoBack

        [TestMethod]
        [Asynchronous]
        [TestDescription("Verifies the behavior of Journal's GoBack method.")]
        public void GoBack()
        {
            this.CreateTestJournal(false);
            GoBackCore();
        }

        [TestMethod]
        [Asynchronous]
        [TestDescription("Verifies the behavior of Journal's GoBack method when browser integrated.")]
        public void GoBackBrowserIntegrated()
        {
            this.CreateTestJournal(true);
            GoBackCore();
        }

        private void GoBackCore()
        {
            JournalEntry journalEntry1 = this.CreateJournalEntry();
            JournalEntry journalEntry2 = this.CreateJournalEntry();
            JournalEntry journalEntry3 = this.CreateJournalEntry();

            this.EnqueueCallback(() =>
            {
                // Add 3 history points.
                this.Journal.AddHistoryPoint(journalEntry1);
                this.Journal.AddHistoryPoint(journalEntry2);
                this.Journal.AddHistoryPoint(journalEntry3);
            });

            // Navigate back to the second entry
            this.EnqueueCallback(() => this.Journal.GoBack());

            this.EnqueueConditional(() => this.NavigatedCount == 4);

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual<JournalEntry>(journalEntry2, this.Journal.CurrentEntry);
                if (this.Journal.UseNavigationState)
                {
                    Assert.AreEqual<string>(journalEntry2.Name, BrowserHelper.Title);
                    Assert.AreEqual<string>(journalEntry2.Source.OriginalString, BrowserHelper.Location.Hash);
                }
                Assert.AreEqual<int>(1, this.Journal.BackStack.Count);
                Assert.AreEqual<int>(1, this.Journal.ForwardStack.Count);
                Assert.IsTrue(this.Journal.CanGoBack);
                Assert.IsTrue(this.Journal.CanGoForward);
            });

            // Navigate back to first entry.
            this.EnqueueCallback(() => this.Journal.GoBack());
            this.EnqueueConditional(() => this.NavigatedCount == 5);
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual<JournalEntry>(journalEntry1, this.Journal.CurrentEntry);
                if (this.Journal.UseNavigationState)
                {
                    Assert.AreEqual<string>(journalEntry1.Name, BrowserHelper.Title);
                    Assert.AreEqual<string>(journalEntry1.Source.OriginalString, BrowserHelper.Location.Hash);
                }
                Assert.AreEqual<int>(5, this.NavigatedCount);
                Assert.AreEqual<int>(0, this.Journal.BackStack.Count);
                Assert.AreEqual<int>(2, this.Journal.ForwardStack.Count);
                Assert.IsFalse(this.Journal.CanGoBack);
                Assert.IsTrue(this.Journal.CanGoForward);
            });

            this.EnqueueTestComplete();
        }

        #endregion GoBack

        #region GoForward

        [TestMethod]
        [Asynchronous]
        [TestDescription("Verifies the behavior of Journal's GoBack method.")]
        public void GoForward()
        {
            this.CreateTestJournal(false);
            GoForwardCore();
        }

        [TestMethod]
        [Asynchronous]
        [TestDescription("Verifies the behavior of Journal's GoBack method when browser integrated.")]
        public void GoForwardBrowserIntegrated()
        {
            this.CreateTestJournal(true);
            GoForwardCore();
        }

        private void GoForwardCore()
        {
            JournalEntry journalEntry1 = this.CreateJournalEntry();
            JournalEntry journalEntry2 = this.CreateJournalEntry();
            JournalEntry journalEntry3 = this.CreateJournalEntry();

            this.EnqueueCallback(() =>
            {
                // Add 3 history points.
                this.Journal.AddHistoryPoint(journalEntry1);
                this.Journal.AddHistoryPoint(journalEntry2);
                this.Journal.AddHistoryPoint(journalEntry3);

                // Navigate back to first entry.
                this.Journal.GoBack();
                this.Journal.GoBack();

                // Navigate forward to second entry.
                this.Journal.GoForward();
            });

            this.EnqueueConditional(() => this.NavigatedCount == 6);
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual<JournalEntry>(journalEntry2, this.Journal.CurrentEntry);
                if (this.Journal.UseNavigationState)
                {
                    Assert.AreEqual<string>(journalEntry2.Name, BrowserHelper.Title);
                    Assert.AreEqual<string>(journalEntry2.Source.OriginalString, BrowserHelper.Location.Hash);
                }
                Assert.AreEqual<int>(1, this.Journal.BackStack.Count);
                Assert.AreEqual<int>(1, this.Journal.ForwardStack.Count);
                Assert.IsTrue(this.Journal.CanGoBack);
                Assert.IsTrue(this.Journal.CanGoForward);
            });

            // Navigate forward to third entry.
            this.EnqueueCallback(() => this.Journal.GoForward());
            this.EnqueueConditional(() => this.NavigatedCount == 7);
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual<JournalEntry>(journalEntry3, this.Journal.CurrentEntry);
                if (this.Journal.UseNavigationState)
                {
                    Assert.AreEqual<string>(journalEntry3.Name, BrowserHelper.Title);
                    Assert.AreEqual<string>(journalEntry3.Source.OriginalString, BrowserHelper.Location.Hash);
                }
                Assert.AreEqual<int>(2, this.Journal.BackStack.Count);
                Assert.AreEqual<int>(0, this.Journal.ForwardStack.Count);
                Assert.IsTrue(this.Journal.CanGoBack);
                Assert.IsFalse(this.Journal.CanGoForward);
            });

            this.EnqueueTestComplete();
        }

        #endregion GoForward

        [TestMethod]
        [Asynchronous]
        [TestDescription("Verifies the behavior of CheckForDeeplinks method when browser integrated.")]
        public void CheckForDeeplinksBrowserIntegrated()
        {
            int localNavigationCount = 0;
            string value1 = Guid.NewGuid().ToString();

            // Seed a deeplink value
            this.EnqueueCallback(() => BrowserHelper.Location.Hash = value1);
            this.EnqueueConditional(() => BrowserHelper.Location.Hash == value1);
            this.EnqueueCallback(() =>
            {
                // Create a new journal using the key provided
                this.CreateTestJournal(true);
                this.Journal.Navigated += (sender, args) => localNavigationCount++;

                // Check for deeplinks
                Assert.IsTrue(this.Journal.CheckForDeeplinks());
            });
            this.EnqueueConditional(() => localNavigationCount == 1);
            this.EnqueueCallback(() => Assert.AreEqual<string>(value1, this.Journal.CurrentEntry.Source.OriginalString));
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [TestDescription("Verifies the Navigated event is raised as expected and that its event arguments are correct.")]
        public void Navigated()
        {
            this.CreateTestJournal(false);
            NavigatedCore();
        }

        [TestMethod]
        [Asynchronous]
        [TestDescription("Verifies the Navigated event is raised as expected and that its event arguments are correct when browser integrated.")]
        public void NavigatedBrowserIntegrated()
        {
            this.CreateTestJournal(true);
            NavigatedCore();
        }

        private void NavigatedCore()
        {
            JournalEventArgs eventArguments = null;
            JournalEntry journalEntry1 = this.CreateJournalEntry();
            JournalEntry journalEntry2 = this.CreateJournalEntry();

            this.Journal.Navigated += (sender, args) => eventArguments = args;

            this.EnqueueCallback(() => this.Journal.AddHistoryPoint(journalEntry1));
            this.EnqueueConditional(() => eventArguments != null && this.NavigatedCount == 1);
            this.EnqueueCallback(() =>
            {
                // Validate the new operation is reported correctly.
                Assert.AreEqual<JournalEntry>(journalEntry1, this.Journal.CurrentEntry);
                Assert.AreEqual<NavigationMode>(NavigationMode.New, eventArguments.NavigationMode);
                Assert.AreEqual<string>(journalEntry1.Name, eventArguments.Name);
                Assert.AreEqual<Uri>(journalEntry1.Source, eventArguments.Uri);

                eventArguments = null;
            });

            this.EnqueueCallback(() => this.Journal.AddHistoryPoint(journalEntry2));
            this.EnqueueConditional(() => eventArguments != null && this.NavigatedCount == 2);
            this.EnqueueCallback(() =>
            {
                // Validate the new operation is reported correctly.
                Assert.AreEqual<JournalEntry>(journalEntry2, this.Journal.CurrentEntry);
                Assert.AreEqual<NavigationMode>(NavigationMode.New, eventArguments.NavigationMode);
                Assert.AreEqual<string>(journalEntry2.Name, eventArguments.Name);
                Assert.AreEqual<Uri>(journalEntry2.Source, eventArguments.Uri);

                eventArguments = null;
            });
            this.EnqueueCallback(() => this.Journal.GoBack());
            this.EnqueueConditional(() => eventArguments != null && this.NavigatedCount == 3);
            this.EnqueueCallback(() =>
            {
                // Validate the back operation is reported correctly.
                Assert.AreEqual<JournalEntry>(journalEntry1, this.Journal.CurrentEntry);
                Assert.AreEqual<NavigationMode>(NavigationMode.Back, eventArguments.NavigationMode);
                Assert.AreEqual<string>(journalEntry1.Name, eventArguments.Name);
                Assert.AreEqual<Uri>(journalEntry1.Source, eventArguments.Uri);

                eventArguments = null;
            });
            this.EnqueueCallback(() => this.Journal.GoForward());
            this.EnqueueConditional(() => eventArguments != null && this.NavigatedCount == 4);
            this.EnqueueCallback(() =>
            {
                // Validate the forward operation is reported correctly.
                Assert.AreEqual<JournalEntry>(journalEntry2, this.Journal.CurrentEntry);
                Assert.AreEqual<NavigationMode>(NavigationMode.Forward, eventArguments.NavigationMode);
                Assert.AreEqual<string>(journalEntry2.Name, eventArguments.Name);
                Assert.AreEqual<Uri>(journalEntry2.Source, eventArguments.Uri);

                eventArguments = null;
            });

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Creates a unique JournalEntry instance for use in testing.
        /// </summary>
        /// <returns>A unique JournalEntry instance for use in testing.</returns>
        private JournalEntry CreateJournalEntry()
        {
            string guid = Guid.NewGuid().ToString();
            return new JournalEntry(guid, new Uri(guid, UriKind.Relative));
        }

        #endregion Methods
    }
}
