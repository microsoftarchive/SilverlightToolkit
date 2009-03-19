//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading;
using System.Windows.Browser;
using System.Windows.Navigation;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Navigation.UnitTests;

namespace System.Windows.Controls.UnitTests
{
    [TestClass]
    public class FrameTests : SilverlightTest
    {
        #region Static fields and constants

        private static readonly string TestPagesPath = @"/System.Windows.Controls.Navigation/Controls/TestPages/";

        #endregion

        #region Test Fields and Properties

        private string _pageTitle = string.Empty;

        public Frame Frame { get; set; }
        public int NavigationOperationCount { get; set; }
        public int NavigationStoppedCount { get; set; }

        #endregion

        #region Test initializers, cleanup, etc.

        [ClassInitialize]
        public void ClassInitialize()
        {
            this._pageTitle = HtmlPage.Document.GetProperty("title") as string;
        }

        [ClassCleanup]
        public void Cleanup()
        {
            if (this.Frame != null)
            {
                this.Frame.NavigationService.Journal.BrowserIntegrated = false;
                this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
                this.Frame = null;
            }

            HtmlPage.Window.CurrentBookmark = String.Empty;
            HtmlPage.Document.SetProperty("title", this._pageTitle);

            // Clear stage
            this.TestPanel.Children.Clear();
            this.TestPanel.UpdateLayout();
        }

        [TestInitialize]
        public void Reset()
        {
            if (this.Frame != null)
            {
                this.Frame.NavigationService.Journal.BrowserIntegrated = false;
                this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            }

            HtmlPage.Window.CurrentBookmark = String.Empty;

            // Create a new navigator
            this.Frame = new Frame();

            // Wire up event counters
            this.Frame.Navigated += (sender, args) => this.NavigationOperationCount++;
            this.Frame.NavigationStopped += (sender, args) => this.NavigationStoppedCount++;

            // Reset counter
            this.NavigationOperationCount = 0;
            this.NavigationStoppedCount = 0;

            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
        }

        #endregion

        #region Test methods

        [TestMethod]
        [Description("Verifies the JournalOwnership property behaves correctly.")]
        public void JournalOwnershipBehavior()
        {
            // Verify JournalOwnership is Automatic by default
            Assert.AreEqual(JournalOwnership.Automatic, this.Frame.JournalOwnership);

            // Set to OwnsJournal and verify results
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            Assert.AreEqual(JournalOwnership.OwnsJournal, this.Frame.JournalOwnership);
            Assert.IsInstanceOfType(this.Frame.NavigationService.Journal, typeof(Journal));
            Assert.IsFalse(this.Frame.NavigationService.Journal.BrowserIntegrated);

            // Set to UsesParentJournal and verify results
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            Assert.AreEqual(JournalOwnership.UsesParentJournal, this.Frame.JournalOwnership);
            Assert.IsInstanceOfType(this.Frame.NavigationService.Journal, typeof(Journal));
            Assert.IsTrue(this.Frame.NavigationService.Journal.BrowserIntegrated);
        }

        [TestMethod]
        [Description("Verifies the JournalOwnership property behaves correctly in a nested frame situation.")]
        public void JournalOwnershipBehaviorForNestedFrame()
        {
            Frame innerFrame = new Frame();
            this.Frame.Content = innerFrame;

            this.TestPanel.UpdateLayout();

            // Verify JournalOwnership is Automatic by default
            Assert.AreEqual(JournalOwnership.Automatic, this.Frame.JournalOwnership);

            // Set to OwnsJournal and verify results
            innerFrame.JournalOwnership = JournalOwnership.OwnsJournal;
            Assert.AreEqual(JournalOwnership.OwnsJournal, innerFrame.JournalOwnership);
            Assert.IsInstanceOfType(innerFrame.NavigationService.Journal, typeof(Journal));
            Assert.IsFalse(innerFrame.NavigationService.Journal.BrowserIntegrated);

            // Set to UsesParentJournal and verify results
            try
            {
                innerFrame.JournalOwnership = JournalOwnership.UsesParentJournal;
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                Assert.AreEqual(JournalOwnership.OwnsJournal, innerFrame.JournalOwnership);
            }
        }

        [TestMethod]
        [Description("Tests the Frame's default properties.")]
        public void DefaultProperties()
        {
            Assert.IsInstanceOfType(this.Frame.NavigationService.ContentLoader, typeof(PageResourceContentLoader));

            Assert.IsNotNull(this.Frame.NavigationService);

            Assert.AreEqual(JournalOwnership.Automatic, this.Frame.JournalOwnership);

            Assert.IsNull(this.Frame.Source);

            Assert.IsNull(this.Frame.CurrentSource);

            Assert.IsFalse(this.Frame.CanGoBack);

            Assert.IsFalse(this.Frame.CanGoForward);

            Assert.IsNull(this.Frame.NavigationService.UriMapper);
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame Navigation method without browser integration.")]
        public void Navigation()
        {
            int completed = 0;
            string page1String = TestPagesPath + "Page1.xaml";
            Uri page1Uri = new Uri(page1String, UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Verify contents of event args
                    Assert.IsTrue(args.Content is Page1);

                    // Verify contents in navigator content presenter
                    Assert.IsTrue(Frame.Content is Page1);

                    // Verify Uri
                    Assert.AreEqual<Uri>(page1Uri, args.Uri);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);

                    // Verify navigation context
                    Page1 frameContent = Frame.Content as Page1;
                    Assert.IsNotNull(frameContent.NavigationContext);
                    Assert.IsNotNull(frameContent.NavigationContext.QueryString);

                    Assert.AreEqual<int>(0, frameContent.NavigationContext.QueryString.Count);

                    Assert.ReferenceEquals(frameContent.NavigationService, this.Frame.NavigationService);

                    Assert.AreEqual<int>(completed, this.Frame.NavigationService.Journal.BackStack.Count);
                    Assert.AreEqual<int>(0, this.Frame.NavigationService.Journal.ForwardStack.Count);

                    // Set completion flag
                    completed++;
                };

            // Perform a navigation operation by Uri
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => completed == 1);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame Navigation method while integrated with browser.")]
        public void NavigationBrowserIntegrated()
        {
            int completed = 0;
            string page1String = TestPagesPath + "Page1.xaml";
            Uri page1Uri = new Uri(page1String, UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Verify contents of event args
                    Assert.IsTrue(args.Content is Page1);

                    // Verify contents in navigator content presenter
                    Assert.IsTrue(Frame.Content is Page1);

                    // Verify Uri
                    Assert.AreEqual<Uri>(page1Uri, args.Uri);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);

                    // Verify navigation context
                    Page1 frameContent = Frame.Content as Page1;
                    Assert.IsNotNull(frameContent.NavigationContext);
                    Assert.IsNotNull(frameContent.NavigationContext.QueryString);

                    Assert.AreEqual<int>(0, frameContent.NavigationContext.QueryString.Count);

                    Assert.ReferenceEquals(frameContent.NavigationService, this.Frame.NavigationService);

                    Assert.AreEqual<int>(completed, this.Frame.NavigationService.Journal.BackStack.Count);
                    Assert.AreEqual<int>(0, this.Frame.NavigationService.Journal.ForwardStack.Count);

                    // Set completion flag
                    completed++;
                };

            // Perform a navigation operation by Uri
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => completed == 1);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame Navigation method without browser integration, when a query string is passed to the page.")]
        public void NavigationWithQueryString()
        {
            int completed = 0;
            string page1String = TestPagesPath + "Page1.xaml?field1=val1&field2=val2";
            Uri page1Uri = new Uri(page1String, UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Verify contents of event args
                    Assert.IsTrue(args.Content is Page1);

                    // Verify contents in navigator content presenter
                    Assert.IsTrue(Frame.Content is Page1);

                    // Verify Uri
                    Assert.AreEqual<Uri>(page1Uri, args.Uri);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);

                    // Verify navigation context
                    Page1 frameContent = Frame.Content as Page1;
                    Assert.IsNotNull(frameContent.NavigationContext);
                    Assert.IsNotNull(frameContent.NavigationContext.QueryString);

                    Assert.AreEqual<int>(2, frameContent.NavigationContext.QueryString.Count);

                    Assert.AreEqual<string>("val1", frameContent.NavigationContext.QueryString["field1"]);
                    Assert.AreEqual<string>("val2", frameContent.NavigationContext.QueryString["field2"]);

                    Assert.ReferenceEquals(frameContent.NavigationService, this.Frame.NavigationService);

                    Assert.AreEqual<int>(completed, this.Frame.NavigationService.Journal.BackStack.Count);
                    Assert.AreEqual<int>(0, this.Frame.NavigationService.Journal.ForwardStack.Count);

                    // Set completion flag
                    completed++;
                };

            // Perform a navigation operation by Uri
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => completed == 1);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame Navigation method while integrated with browser, when a query string is passed to the page.")]
        public void NavigationWithQueryStringBrowserIntegrated()
        {
            int completed = 0;
            string page1String = TestPagesPath + "Page1.xaml?field1=val1&field2=val2";
            Uri page1Uri = new Uri(page1String, UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Verify contents of event args
                    Assert.IsTrue(args.Content is Page1);

                    // Verify contents in navigator content presenter
                    Assert.IsTrue(Frame.Content is Page1);

                    // Verify Uri
                    Assert.AreEqual<Uri>(page1Uri, args.Uri);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);

                    // Verify navigation context
                    Page1 frameContent = Frame.Content as Page1;
                    Assert.IsNotNull(frameContent.NavigationContext);
                    Assert.IsNotNull(frameContent.NavigationContext.QueryString);

                    Assert.AreEqual<int>(2, frameContent.NavigationContext.QueryString.Count);

                    Assert.AreEqual<string>("val1", frameContent.NavigationContext.QueryString["field1"]);
                    Assert.AreEqual<string>("val2", frameContent.NavigationContext.QueryString["field2"]);

                    Assert.ReferenceEquals(frameContent.NavigationService, this.Frame.NavigationService);

                    Assert.AreEqual<int>(completed, this.Frame.NavigationService.Journal.BackStack.Count);
                    Assert.AreEqual<int>(0, this.Frame.NavigationService.Journal.ForwardStack.Count);

                    // Set completion flag
                    completed++;
                };

            // Perform a navigation operation by Uri
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => completed == 1);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests canceling navigation from a Navigating event handler.")]
        public void NavigationCancellation()
        {
            bool completed = false;

            // This event is expected to be fired if we cancel.
            Frame.NavigationStopped += (sender, args) => completed = true;

            // This event should not be fired if we cancel.
            Frame.Navigated += (sender, args) => Assert.Fail();

            // This event should not be fired.
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);

            // Here is where we can cancel the navigation operation.
            Frame.Navigating +=
                (sender, args) =>
                {
                    // Attempt to cancel navigation
                    args.Cancel = true;
                };

            // Start a navigation operation.
            this.EnqueueCallback(() => Frame.Navigate(new Uri("/Page1.xaml", UriKind.Relative)));

            this.EnqueueConditional(() => completed);

            this.EnqueueCallback(() =>
            {
                Assert.IsNull(this.Frame.CurrentSource);
                Assert.IsNull(this.Frame.Source);
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame StopLoading method.")]
        public void StopLoading()
        {
            bool completed = false;

            // This event is expected to be fired if we cancel.
            Frame.NavigationStopped += (sender, args) => completed = true;

            // This event should not be fired if we cancel.
            Frame.Navigated += (sender, args) => Assert.Fail();

            // This event should not be fired.
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);

            // Start a navigation operation, and immediately cancel.
            this.EnqueueCallback(() =>
                {
                    Frame.Navigate(new Uri("/Page1.xaml", UriKind.Relative));
                    Frame.StopLoading();
                });

            this.EnqueueConditional(() => completed);

            this.EnqueueCallback(() =>
            {
                Assert.IsNull(this.Frame.CurrentSource);
                Assert.IsNull(this.Frame.Source);
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame GoBack method.")]
        public void NavigationBack()
        {
            bool verifyFirstBackOperation = false,
                 verifySecondBackOperation = false;
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;

            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            Uri page3Uri = new Uri(TestPagesPath + "Page3.xaml", UriKind.Relative);

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for third navigation
                    if (this.NavigationOperationCount == 4)
                    {
                        // Verify contents of event args.
                        // This should be of type Page2 when we go back initially.
                        Assert.IsTrue(args.Content is Page2);

                        // Verify contents in navigator content presenter.
                        // This should be of type Page2 when we go back initially.
                        Assert.IsTrue(Frame.Content is Page2);

                        Assert.AreEqual<Uri>(page2Uri, this.Frame.CurrentSource);
                        Assert.AreEqual<Uri>(page2Uri, this.Frame.Source);

                        Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                        Assert.AreEqual<int>(1, this.Frame.NavigationService.Journal.BackStack.Count);
                        Assert.AreEqual<int>(1, this.Frame.NavigationService.Journal.ForwardStack.Count);

                        verifyFirstBackOperation = true;
                    }
                    else if (this.NavigationOperationCount == 5)
                    {
                        // Verify contents of event args.
                        // This should be of type Page1 when we go back a second time.
                        Assert.IsTrue(args.Content is Page1);

                        // Verify contents in navigator content presenter.
                        // This should be of type Page1 when we go back a second time.
                        Assert.IsTrue(Frame.Content is Page1);

                        Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);
                        Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);

                        Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                        Assert.AreEqual<int>(0, this.Frame.NavigationService.Journal.BackStack.Count);
                        Assert.AreEqual<int>(2, this.Frame.NavigationService.Journal.ForwardStack.Count);

                        verifySecondBackOperation = true;
                    }
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 2);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page3Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 3);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => verifyFirstBackOperation);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => verifySecondBackOperation);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame GoBack method with browser history integration.")]
        public void NavigationBackBrowserIntegrated()
        {
            bool verifyFirstBackOperation = false,
                 verifySecondBackOperation = false;

            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            Uri page3Uri = new Uri(TestPagesPath + "Page3.xaml", UriKind.Relative);

            // This should be enabled by default but let's be explicit.
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for third navigation
                    if (this.NavigationOperationCount == 4)
                    {
                        // Verify contents of event args.
                        // This should be of type Page2 when we go back initially.
                        Assert.IsTrue(args.Content is Page2);

                        // Verify contents in navigator content presenter.
                        // This should be of type Page2 when we go back initially.
                        Assert.IsTrue(Frame.Content is Page2);

                        Assert.AreEqual<Uri>(page2Uri, this.Frame.CurrentSource);
                        Assert.AreEqual<Uri>(page2Uri, this.Frame.Source);

                        Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                        Assert.AreEqual<int>(1, this.Frame.NavigationService.Journal.BackStack.Count);
                        Assert.AreEqual<int>(1, this.Frame.NavigationService.Journal.ForwardStack.Count);

                        verifyFirstBackOperation = true;
                    }
                    else if (this.NavigationOperationCount == 5)
                    {
                        // Verify contents of event args.
                        // This should be of type Page1 when we go back a second time.
                        Assert.IsTrue(args.Content is Page1);

                        // Verify contents in navigator content presenter.
                        // This should be of type Page1 when we go back a second time.
                        Assert.IsTrue(Frame.Content is Page1);

                        Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);
                        Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);

                        Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                        Assert.AreEqual<int>(0, this.Frame.NavigationService.Journal.BackStack.Count);
                        Assert.AreEqual<int>(2, this.Frame.NavigationService.Journal.ForwardStack.Count);

                        verifySecondBackOperation = true;
                    }
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 2);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page3Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 3);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => verifyFirstBackOperation);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => verifySecondBackOperation);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame GoBack method and cancellation.")]
        public void NavigationBackCancellation()
        {
            bool completed = false;
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for third navigation
                    if (this.NavigationOperationCount > 2)
                    {
                        // Failure
                        Assert.Fail();
                    }
                };
            Frame.Navigating +=
                (sender, args) =>
                {
                    if (this.NavigationOperationCount == 2)
                    {
                        args.Cancel = true;
                    }
                };
            Frame.NavigationStopped +=
                (sender, args) =>
                {
                    // Verify contents of event args.
                    Assert.AreEqual<object>(null, args.Content);
                    Assert.AreEqual<Uri>(page1Uri, args.Uri);

                    // Verify the source is correctly still at page2
                    Assert.AreEqual<Uri>(page2Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page2Uri, this.Frame.CurrentSource);

                    // Verify contents in navigator content presenter.
                    Assert.IsTrue(Frame.Content is Page2);

                    Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                    completed = true;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => completed);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame GoBack method and cancellation with browser history integration.")]
        public void NavigationBackCancellationBrowserIntegrated()
        {
            bool completed = false;
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);

            // This should be enabled by default but let's be explicit.
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for third navigation
                    if (this.NavigationOperationCount > 2)
                    {
                        // Failure
                        Assert.Fail();
                    }
                };
            Frame.Navigating +=
                (sender, args) =>
                {
                    if (this.NavigationOperationCount == 2)
                    {
                        args.Cancel = true;
                    }
                };
            Frame.NavigationStopped +=
                (sender, args) =>
                {
                    // Verify contents of event args.
                    Assert.AreEqual<object>(null, args.Content);
                    Assert.AreEqual<Uri>(page1Uri, args.Uri);

                    // Verify the Source is still page2
                    Assert.AreEqual<Uri>(page2Uri, this.Frame.CurrentSource);
                    Assert.AreEqual<Uri>(page2Uri, this.Frame.Source);

                    // Verify contents in navigator content presenter.
                    Assert.IsTrue(Frame.Content is Page2);

                    Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                    completed = true;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => completed);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame GoForward method.")]
        public void NavigationForward()
        {
            bool verifyFirstForwardOperation = false,
                 verifySecondForwardOperation = false;
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;

            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            Uri page3Uri = new Uri(TestPagesPath + "Page3.xaml", UriKind.Relative);

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    if (this.NavigationOperationCount == 6)
                    {
                        // Verify contents of event args.
                        // This should be of type Page2 when we go forward to our last state.
                        Assert.IsTrue(args.Content is Page2);

                        // Verify contents in navigator content presenter.
                        // This should be of type Page2 when we go forward to our last state.
                        Assert.IsTrue(Frame.Content is Page2);

                        Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                        Assert.AreEqual<Uri>(page2Uri, this.Frame.CurrentSource);
                        Assert.AreEqual<Uri>(page2Uri, this.Frame.Source);

                        verifyFirstForwardOperation = true;
                    }
                    else if (this.NavigationOperationCount == 7)
                    {
                        // Verify contents of event args.
                        // This should be of type Page3 when we go forward to our last state.
                        Assert.IsTrue(args.Content is Page3);

                        // Verify contents in navigator content presenter.
                        // This should be of type Page3 when we go forward to our last state.
                        Assert.IsTrue(Frame.Content is Page3);

                        Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                        Assert.AreEqual<Uri>(page3Uri, this.Frame.CurrentSource);
                        Assert.AreEqual<Uri>(page3Uri, this.Frame.Source);

                        verifySecondForwardOperation = true;
                    }
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page3Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 3);

            // Fourth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => this.NavigationOperationCount == 4);

            // Fifth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => this.NavigationOperationCount == 5);

            // Sixth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => verifyFirstForwardOperation);

            // Seventh navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => verifySecondForwardOperation);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame GoForward method with browser history integration.")]
        public void NavigationForwardBrowserIntegrated()
        {
            bool verifyFirstForwardOperation = false,
                 verifySecondForwardOperation = false;

            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            Uri page3Uri = new Uri(TestPagesPath + "Page3.xaml", UriKind.Relative);

            // This should be enabled by default but let's be explicit.
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    if (this.NavigationOperationCount == 6)
                    {
                        // Verify contents of event args.
                        // This should be of type Page2 when we go forward to our last state.
                        Assert.IsTrue(args.Content is Page2);

                        // Verify contents in navigator content presenter.
                        // This should be of type Page2 when we go forward to our last state.
                        Assert.IsTrue(Frame.Content is Page2);

                        Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                        Assert.AreEqual<Uri>(page2Uri, this.Frame.CurrentSource);
                        Assert.AreEqual<Uri>(page2Uri, this.Frame.Source);

                        verifyFirstForwardOperation = true;
                    }
                    else if (this.NavigationOperationCount == 7)
                    {
                        // Verify contents of event args.
                        // This should be of type Page3 when we go forward to our last state.
                        Assert.IsTrue(args.Content is Page3);

                        // Verify contents in navigator content presenter.
                        // This should be of type Page3 when we go forward to our last state.
                        Assert.IsTrue(Frame.Content is Page3);

                        Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                        Assert.AreEqual<Uri>(page3Uri, this.Frame.CurrentSource);
                        Assert.AreEqual<Uri>(page3Uri, this.Frame.Source);

                        verifySecondForwardOperation = true;
                    }
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page3Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 3);

            // Fourth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => this.NavigationOperationCount == 4);

            // Fifth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => this.NavigationOperationCount == 5);

            // Sixth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => verifyFirstForwardOperation);

            // Seventh navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => verifySecondForwardOperation);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame GoForward method cancellation.")]
        public void NavigationForwardCancellation()
        {
            bool completed = false;
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for fourth navigation
                    if (this.NavigationOperationCount > 3)
                    {
                        Assert.Fail();
                    }
                };
            Frame.Navigating +=
                (sender, args) =>
                {
                    if (this.NavigationOperationCount == 3)
                    {
                        args.Cancel = true;
                    }
                };
            Frame.NavigationStopped +=
                (sender, args) =>
                {
                    // Verify contents of event args.
                    Assert.AreEqual<object>(null, args.Content);
                    Assert.AreEqual<object>(page2Uri, args.Uri);

                    // Verify Source and CurrentSource are still page1
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);

                    // Verify contents in navigator content presenter.
                    Assert.IsTrue(Frame.Content is Page1);

                    Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                    completed = true;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => this.NavigationOperationCount == 3);

            // Fourth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => completed);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame GoForward method cancellation with browser history integration.")]
        public void NavigationForwardCancellationBrowserIntegrated()
        {
            bool completed = false;
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);

            // This should be enabled by default but let's be explicit.
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for fourth navigation
                    if (this.NavigationOperationCount > 3)
                    {
                        Assert.Fail();
                    }
                };
            Frame.Navigating +=
                (sender, args) =>
                {
                    if (this.NavigationOperationCount == 3)
                    {
                        args.Cancel = true;
                    }
                };
            Frame.NavigationStopped +=
                (sender, args) =>
                {
                    // Verify contents of event args.
                    Assert.AreEqual<object>(null, args.Content);
                    Assert.AreEqual<object>(page2Uri, args.Uri);

                    // Verify Source and CurrentSource are still page1
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);

                    // Verify contents in navigator content presenter.
                    Assert.IsTrue(Frame.Content is Page1);

                    Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                    completed = true;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => this.NavigationOperationCount == 3);

            // Fourth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => completed);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests that Source property can trigger and reflect navigation operations.")]
        public void SourceProperty()
        {
            Uri page1 = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2 = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);

            // First navigation, wait for completion.
            this.EnqueueCallback(() =>
            {
                Frame.Source = page1;
                Assert.AreNotEqual<Uri>(this.Frame.Source, this.Frame.CurrentSource);
                Assert.IsNull(this.Frame.CurrentSource);
            });
            this.EnqueueConditional(() => this.NavigationOperationCount == 1);

            // Verify the content is correct
            this.EnqueueCallback(() =>
            {
                Assert.IsInstanceOfType(this.Frame.Content, typeof(Page1));
                Assert.AreEqual<Uri>(page1, this.Frame.Source);
                Assert.AreEqual<Uri>(page1, this.Frame.CurrentSource);

                Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);
            });

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => this.Frame.Source = page2);
            this.EnqueueConditional(() => this.NavigationOperationCount == 2);

            // Verify the content is correct
            this.EnqueueCallback(() =>
            {
                Assert.IsInstanceOfType(this.Frame.Content, typeof(Page2));
                Assert.AreEqual<Uri>(page2, this.Frame.Source);
                Assert.AreEqual<Uri>(page2, this.Frame.CurrentSource);

                Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Source is set in XAML, and no deeplink is present, verify that the Frame ends up at the correct location")]
        public void SourceOnLoaded()
        {
            Uri sourceUri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            bool completed = false;

            this.Frame.Source = sourceUri;
            this.Frame.Loaded +=
                (sender, args) =>
                {
                    Assert.AreEqual<Uri>(sourceUri, this.Frame.Source);
                    Assert.IsNull(this.Frame.CurrentSource);
                };
            this.Frame.Navigated +=
                (sender, args) =>
                {
                    Assert.AreEqual<Uri>(sourceUri, this.Frame.Source);
                    Assert.AreEqual<Uri>(sourceUri, this.Frame.CurrentSource);
                    Assert.IsInstanceOfType(this.Frame.Content, typeof(Page1));

                    completed = true;
                };

            this.EnqueueConditional(() => completed);
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Deeplink to the same place as the Source property of the frame, verify that we only go to the resulting page once.")]
        public void SourceAndDeeplinkSame()
        {
            string newGuid = Guid.NewGuid().ToString();
            string deeplinkString = TestPagesPath + "Page1.xaml";
            Uri deeplinkUri = new Uri(deeplinkString, UriKind.Relative);
            HtmlPage.Window.CurrentBookmark = deeplinkString;

            NavigationEventArgs resultArgs = null;
            int navigatedCount = 0;

            this.Frame.Navigated +=
                (sender, args) =>
                {
                    resultArgs = args;
                    navigatedCount++;
                };

            this.Frame.Source = deeplinkUri;

            // Setting the BrowserHistoryIdentifier should force it to check for deeplinks,
            // discover the deeplink set above, and navigate to it.  However, this is the same
            // page as the Source of the Frame already - this should work and end up at the resulting
            // page only one time.
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;

            this.EnqueueConditional(() => navigatedCount == 1);
            this.EnqueueCallback(() =>
            {
                Assert.IsNotNull(resultArgs);
                Assert.AreEqual<Uri>(deeplinkUri, resultArgs.Uri);

                Assert.AreEqual<Uri>(deeplinkUri, this.Frame.CurrentSource);
                Assert.AreEqual<Uri>(deeplinkUri, this.Frame.Source);

                Assert.IsInstanceOfType(this.Frame.Content, typeof(Page1));

                Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);
            });
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Deeplink to a page that is not the Source of the Frame, verify we end up at the deeplink.")]
        public void SourceAndDeeplinkDifferent()
        {
            bool ready = false;
            string newGuid = Guid.NewGuid().ToString();
            string sourceString = TestPagesPath + "Page1.xaml";
            Uri sourceUri = new Uri(sourceString, UriKind.Relative);
            string deeplinkString = TestPagesPath + "Page2.xaml";
            Uri deeplinkUri = new Uri(deeplinkString, UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            HtmlPage.Window.CurrentBookmark = deeplinkUri.OriginalString;

            NavigationEventArgs resultArgs = null;

            // Attach navigated handler to catch event args
            this.Frame.Navigated += 
                (sender, args) =>
                {
                    if (ready)
                    {
                        resultArgs = args;
                    }
                };

            // First, navigate by source.
            this.EnqueueCallback(() => this.Frame.Source = sourceUri);
            this.EnqueueConditional(() => this.NavigationOperationCount == 1);

            // 
            this.EnqueueCallback(() =>
            {
                ready = true;

                // Prepare for deeplink
                this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;

                // Check for deeplinks
                this.Frame.ApplyDeepLinks();
            });

            this.EnqueueConditional(() => this.NavigationOperationCount == 2 &&
                                          resultArgs != null &&
                                          resultArgs.Uri == deeplinkUri);
            this.EnqueueCallback(() =>
            {
                Assert.IsInstanceOfType(this.Frame.Content, typeof(Page2));

                Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);

                Assert.AreEqual<Uri>(deeplinkUri, this.Frame.CurrentSource);
                Assert.AreEqual<Uri>(deeplinkUri, this.Frame.Source);
            });
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the Frame updates the page title correctly.")]
        public void NavigationTitleUpdate()
        {
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "PageEmptyStringTitle.xaml", UriKind.Relative);
            Uri page3Uri = new Uri(TestPagesPath + "Page3.xaml", UriKind.Relative);
            Uri page4Uri = new Uri(TestPagesPath + "PageNullStringTitle.xaml", UriKind.Relative);

            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;

            this.EnqueueCallback(() => this.Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 1);
            this.EnqueueCallback(
                () =>
                {
                    Assert.IsInstanceOfType(this.Frame.Content, typeof(Page1));
                    Assert.AreEqual<string>("Page1 Page", BrowserHelper.Title);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);
                });

            this.EnqueueCallback(() => this.Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 2);
            this.EnqueueCallback(
                () =>
                {
                    Assert.IsInstanceOfType(this.Frame.Content, typeof(PageEmptyStringTitle));
                    Assert.AreEqual<string>("", BrowserHelper.Title);
                    Assert.AreEqual<Uri>(page2Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page2Uri, this.Frame.CurrentSource);
                });

            this.EnqueueCallback(() => this.Frame.Navigate(page3Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 3);
            this.EnqueueCallback(
                () =>
                {
                    Assert.IsInstanceOfType(this.Frame.Content, typeof(Page3));
                    Assert.AreEqual<string>("Page3 Page", BrowserHelper.Title);
                    Assert.AreEqual<Uri>(page3Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page3Uri, this.Frame.CurrentSource);
                });

            this.EnqueueCallback(() => this.Frame.Navigate(page4Uri));
            this.EnqueueConditional(() => this.NavigationOperationCount == 4);
            this.EnqueueCallback(
                () =>
                {
                    // When Page.Title is null, the page title should be the Uri of the page
                    Assert.IsInstanceOfType(this.Frame.Content, typeof(PageNullStringTitle));
                    Assert.AreEqual<string>(page4Uri.OriginalString, BrowserHelper.Title);
                    Assert.AreEqual<Uri>(page4Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page4Uri, this.Frame.CurrentSource);
                });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Verifies that a Frame navigation invoked from a background thread executes as expected.")]
        public void NavigationFromBackgroundThread()
        {
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);

            ThreadPool.QueueUserWorkItem(state => this.Frame.Navigate(page1Uri));

            this.EnqueueConditional(() => this.NavigationOperationCount == 1);
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);
                Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);

                Assert.IsInstanceOfType(this.Frame.Content, typeof(Page1));

                Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);
            });
            this.EnqueueTestComplete();
        }

        #region Event Sequence tests

        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the Frame events fire in correct order during a successful navigation.")]
        public void NavigationByUriEventSequence()
        {
            bool complete = false;
            int eventCount = 0;

            string page1String = TestPagesPath + "Page1.xaml";
            Uri page1Uri = new Uri(page1String, UriKind.Relative);

            // Wire up event handlers
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail();
            this.Frame.NavigationStopped += (sender, args) => Assert.Fail();
            this.Frame.Navigating +=
                (sender, args) =>
                {
                    // Verify this was the first event raised.
                    Assert.AreEqual<int>(1, ++eventCount);

                    Assert.IsNull(this.Frame.Source);
                    Assert.IsNull(this.Frame.CurrentSource);
                };
            this.Frame.Navigated +=
                 (sender, args) =>
                 {
                     // Verify this was the fourth event raised.
                     Assert.AreEqual<int>(2, ++eventCount);

                     Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);
                     Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);

                     complete = true;
                 };

            this.EnqueueCallback(() => this.Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete && (this.Frame.Content as Page1).VirtualsCalled.Count == 1);

            // Verify that the page virtuals fired in the correct order
            this.EnqueueCallback(() =>
            {
                var content = this.Frame.Content as Page1;
                Assert.AreEqual<string>("OnNavigatedTo", content.VirtualsCalled[0]);
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the Frame events fire in correct order during a canceled navigation.")]
        public void NavigationByUriCanceledEventSequence()
        {
            bool complete = false;
            int eventCount = 0;

            // Wire up event handlers
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail();
            this.Frame.Navigated += (sender, args) => Assert.Fail();
            this.Frame.Navigating +=
                (sender, args) =>
                {
                    // Verify this was the first event raised.
                    Assert.AreEqual<int>(1, ++eventCount);

                    Assert.IsNull(this.Frame.Source);
                    Assert.IsNull(this.Frame.CurrentSource);

                    // Cancel navigation
                    args.Cancel = true;
                };
            this.Frame.NavigationStopped +=
                 (sender, args) =>
                 {
                     // Verify this was the second event raised.
                     Assert.AreEqual<int>(2, ++eventCount);

                     Assert.IsNull(this.Frame.Source);
                     Assert.IsNull(this.Frame.CurrentSource);

                     complete = true;
                 };

            this.EnqueueCallback(() => this.Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete);
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the Frame events fire in correct order during a unsuccessful navigation.")]
        public void NavigationFailureEventSequence()
        {
            bool complete = false;
            int eventCount = 0;

            // Wire up event handlers
            this.Frame.Navigated += (sender, args) => Assert.Fail();
            this.Frame.NavigationStopped += (sender, args) => Assert.Fail();
            this.Frame.Navigating +=
                (sender, args) =>
                {
                    // Verify this was the first event raised.
                    Assert.AreEqual<int>(1, ++eventCount);

                    Assert.IsNull(this.Frame.Source);
                    Assert.IsNull(this.Frame.CurrentSource);
                };
            this.Frame.NavigationFailed +=
                 (sender, args) =>
                 {
                     // Verify this was the third event raised.
                     Assert.AreEqual<int>(2, ++eventCount);

                     Assert.IsNull(this.Frame.Source);
                     Assert.IsNull(this.Frame.CurrentSource);

                     args.Handled = true;

                     complete = true;
                 };

            this.EnqueueCallback(() => this.Frame.Navigate(new Uri("__InvalidUri__", UriKind.Relative)));
            this.EnqueueConditional(() => complete);
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the Frame processes multiple Uri navigation calls correctly")]
        public void NavigationMultipleUriEventSequence()
        {
            string nav1 = TestPagesPath + "Page1.xaml",
                   nav2 = TestPagesPath + "Page2.xaml",
                   nav3 = TestPagesPath + "Page3.xaml";

            Uri nav1Uri = new Uri(nav1, UriKind.Relative);
            Uri nav2Uri = new Uri(nav2, UriKind.Relative);
            Uri nav3Uri = new Uri(nav3, UriKind.Relative);

            bool complete = false;
            int eventCount = 0;

            this.Frame.NavigationFailed += (sender, args) => Assert.Fail();
            this.Frame.Navigating +=
                (sender, args) =>
                {
                    Assert.IsNull(this.Frame.Source);
                    Assert.IsNull(this.Frame.CurrentSource);

                    if (args.Uri.OriginalString == nav1)
                    {
                        // Verify this was the first event raised.
                        Assert.AreEqual<int>(1, ++eventCount);
                    }
                    else if (args.Uri.OriginalString == nav2)
                    {
                        // Verify this was the second event raised.
                        Assert.AreEqual<int>(2, ++eventCount);
                    }
                    else if (args.Uri.OriginalString == nav3)
                    {
                        // Verify this was the third event raised.
                        Assert.AreEqual<int>(3, ++eventCount);
                    }
                    else
                    {
                        Assert.Fail();
                    }
                };
            this.Frame.NavigationStopped +=
                (sender, args) =>
                {
                    if (args.Uri.OriginalString == nav1)
                    {
                        // Verify this was the fourth event raised.
                        Assert.AreEqual<int>(4, ++eventCount);

                        Assert.IsNull(this.Frame.Source);
                        Assert.IsNull(this.Frame.CurrentSource);
                    }
                    else if (args.Uri.OriginalString == nav2)
                    {
                        // Verify this was the fifth event raised.
                        Assert.AreEqual<int>(5, ++eventCount);

                        Assert.IsNull(this.Frame.Source);
                        Assert.IsNull(this.Frame.CurrentSource);
                    }
                    else
                    {
                        Assert.Fail();
                    }
                };
            this.Frame.Navigated +=
                 (sender, args) =>
                 {
                     if (args.Uri.OriginalString == nav3)
                     {
                         // Verify this was the sixth event raised.
                         Assert.AreEqual<int>(6, ++eventCount);

                         Assert.AreEqual<Uri>(nav3Uri, this.Frame.Source);
                         Assert.AreEqual<Uri>(nav3Uri, this.Frame.CurrentSource);

                         var content = this.Frame.Content as Page3;
                         Assert.AreEqual<int>(1, content.VirtualsCalled.Count);
                         Assert.AreEqual<string>("OnNavigatedTo", content.VirtualsCalled[0]);

                         complete = true;
                     }
                     else
                     {
                         Assert.Fail();
                     }
                 };

            this.EnqueueCallback(
                () =>
                {
                    this.Frame.Navigate(nav1Uri);
                    this.Frame.Navigate(nav2Uri);
                    this.Frame.Navigate(nav3Uri);
                });

            this.EnqueueConditional(() => complete && (this.Frame.Content as Page3).VirtualsCalled.Count == 1);

            // Verify that the page virtuals fired in the correct order
            this.EnqueueCallback(() =>
            {
                var content = this.Frame.Content as Page3;
                Assert.AreEqual<string>("OnNavigatedTo", content.VirtualsCalled[0]);
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("For WPF compatibility, verify that we raise events in the correct order when navigating to a Fragment on a new page")]
        public void NavigateToNewPageWithFragmentEventSequence()
        {
            bool complete = false;
            int eventCount = 0;

            string fragment = "frag1";
            Uri testUri = new Uri(TestPagesPath + "Page1.xaml#" + fragment, UriKind.Relative);

            // Wire up event handlers
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail();
            this.Frame.Navigating +=
                (sender, args) =>
                {
                    // Verify this was the first event raised.
                    Assert.AreEqual<int>(1, ++eventCount);

                    Assert.IsNull(this.Frame.Source);
                    Assert.IsNull(this.Frame.CurrentSource);
                    Assert.AreEqual(testUri, args.Uri);
                    Assert.AreEqual(NavigationMode.New, args.NavigationMode);
                };
            this.Frame.Navigated +=
                 (sender, args) =>
                 {
                     // Verify this was the second event raised.
                     Assert.AreEqual<int>(2, ++eventCount);

                     Assert.AreEqual(testUri, this.Frame.Source);
                     Assert.AreEqual(testUri, this.Frame.CurrentSource);
                     Assert.AreEqual(testUri, args.Uri);
                 };
            this.Frame.FragmentNavigation +=
                (sender, args) =>
                {
                    // Verify this was the third event raised.
                    Assert.AreEqual<int>(3, ++eventCount);

                    Assert.AreEqual(testUri, this.Frame.Source);
                    Assert.AreEqual(testUri, this.Frame.CurrentSource);
                    Assert.AreEqual(fragment, args.Fragment);

                    // Test complete
                    complete = true;
                };

            this.EnqueueCallback(() => this.Frame.Navigate(testUri));
            this.EnqueueConditional(() => complete && (this.Frame.Content as Page1).VirtualsCalled.Count == 2);

            // Verify that the page virtuals fired in the correct order
            this.EnqueueCallback(() =>
                {
                    var content = this.Frame.Content as Page1;
                    Assert.AreEqual<string>("OnNavigatedTo", content.VirtualsCalled[0]);
                    Assert.AreEqual<string>("OnFragmentNavigation", content.VirtualsCalled[1]);
                });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("For WPF compatibility, verify that we raise events in the correct order when navigating to a Fragment on the existing page, when using ONLY the fragment")]
        public void NavigateToFragmentOnExistingPageEventSequence()
        {
            bool complete = false;
            int eventCount = 0;
            int navigatingCount = 0;
            int navigatedCount = 0;

            string fragment = "frag1";
            Uri testUri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri testUriWithFragment = new Uri(TestPagesPath + "Page1.xaml#" + fragment, UriKind.Relative);
            Uri testUriFragmentOnly = new Uri("#" + fragment, UriKind.Relative);

            // Wire up event handlers
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail();
            this.Frame.Navigating +=
                (sender, args) =>
                {
                    if (++navigatingCount == 1)
                    {
                        // Verify this was the first event raised.
                        Assert.AreEqual<int>(1, ++eventCount);

                        Assert.IsNull(this.Frame.Source);
                        Assert.IsNull(this.Frame.CurrentSource);
                        Assert.AreEqual(testUri, args.Uri);
                        Assert.AreEqual(NavigationMode.New, args.NavigationMode);
                    }
                    else
                    {
                        // Verify this was the 3rd event raised (Navigating, Navigated, Navigating)
                        Assert.AreEqual<int>(3, ++eventCount);

                        Assert.AreEqual(testUri, this.Frame.Source);
                        Assert.AreEqual(testUri, this.Frame.CurrentSource);
                        Assert.AreEqual(testUriWithFragment, args.Uri);
                        Assert.AreEqual(NavigationMode.New, args.NavigationMode);
                    }
                };
            this.Frame.Navigated +=
                 (sender, args) =>
                 {
                     if (++navigatedCount == 1)
                     {
                         // Verify this was the second event raised.
                         Assert.AreEqual<int>(2, ++eventCount);

                         Assert.AreEqual(testUri, this.Frame.Source);
                         Assert.AreEqual(testUri, this.Frame.CurrentSource);
                         Assert.AreEqual(testUri, args.Uri);

                         // Now start a new navigation that goes to the same page, but with a fragment
                         this.Frame.Dispatcher.BeginInvoke(() => this.Frame.Navigate(testUriFragmentOnly));
                     }
                     else
                     {
                         // Verify this was the fifth event raised (Navigating, Navigated, Navigating, FragmentNavigation, Navigated).
                         Assert.AreEqual<int>(5, ++eventCount);

                         Assert.AreEqual(testUriWithFragment, this.Frame.Source);
                         Assert.AreEqual(testUriWithFragment, this.Frame.CurrentSource);
                         Assert.AreEqual(testUriWithFragment, args.Uri);

                         // Test complete
                         complete = true;
                     }
                 };
            this.Frame.FragmentNavigation +=
                (sender, args) =>
                {
                    // Verify this was the fourth event raised (Navigating, Navigated, Navigating, FragmentNavigation).
                    Assert.AreEqual<int>(4, ++eventCount);

                    Assert.AreEqual(testUriWithFragment, this.Frame.Source);
                    Assert.AreEqual(testUriWithFragment, this.Frame.CurrentSource);
                    Assert.AreEqual(fragment, args.Fragment);
                };

            this.EnqueueCallback(() => this.Frame.Navigate(testUri));
            this.EnqueueConditional(() => complete && (this.Frame.Content as Page1).VirtualsCalled.Count == 2);

            // Verify that the page virtuals fired in the correct order
            this.EnqueueCallback(() =>
            {
                var content = this.Frame.Content as Page1;
                Assert.AreEqual<string>("OnNavigatedTo", content.VirtualsCalled[0]);
                Assert.AreEqual<string>("OnFragmentNavigation", content.VirtualsCalled[1]);
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("For WPF compatibility, verify that we raise events in the correct order when navigating to a Fragment on the existing page, when using a full Uri containing the fragment")]
        public void NavigateToFragmentOnExistingPageByFullUriEventSequence()
        {
            bool complete = false;
            int eventCount = 0;
            int navigatingCount = 0;
            int navigatedCount = 0;

            string fragment = "frag1";
            Uri testUri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri testUriWithFragment = new Uri(TestPagesPath + "Page1.xaml#" + fragment, UriKind.Relative);

            // Wire up event handlers
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail();
            this.Frame.Navigating +=
                (sender, args) =>
                {
                    if (++navigatingCount == 1)
                    {
                        // Verify this was the first event raised.
                        Assert.AreEqual<int>(1, ++eventCount);

                        Assert.IsNull(this.Frame.Source);
                        Assert.IsNull(this.Frame.CurrentSource);
                        Assert.AreEqual(testUri, args.Uri);
                        Assert.AreEqual(NavigationMode.New, args.NavigationMode);
                    }
                    else
                    {
                        // Verify this was the 3rd event raised (Navigating, Navigated, Navigating)
                        Assert.AreEqual<int>(3, ++eventCount);

                        Assert.AreEqual(testUri, this.Frame.Source);
                        Assert.AreEqual(testUri, this.Frame.CurrentSource);
                        Assert.AreEqual(testUriWithFragment, args.Uri);
                        Assert.AreEqual(NavigationMode.New, args.NavigationMode);
                    }
                };
            this.Frame.Navigated +=
                 (sender, args) =>
                 {
                     if (++navigatedCount == 1)
                     {
                         // Verify this was the second event raised.
                         Assert.AreEqual<int>(2, ++eventCount);

                         Assert.AreEqual(testUri, this.Frame.Source);
                         Assert.AreEqual(testUri, this.Frame.CurrentSource);
                         Assert.AreEqual(testUri, args.Uri);

                         // Now start a new navigation that goes to the same page, but with a fragment
                         this.Frame.Dispatcher.BeginInvoke(() => this.Frame.Navigate(testUriWithFragment));
                     }
                     else
                     {
                         // Verify this was the fifth event raised (Navigating, Navigated, Navigating, FragmentNavigation, Navigated).
                         Assert.AreEqual<int>(5, ++eventCount);

                         Assert.AreEqual(testUriWithFragment, this.Frame.Source);
                         Assert.AreEqual(testUriWithFragment, this.Frame.CurrentSource);
                         Assert.AreEqual(testUriWithFragment, args.Uri);

                         // Test complete
                         complete = true;
                     }
                 };
            this.Frame.FragmentNavigation +=
                (sender, args) =>
                {
                    // Verify this was the fourth event raised (Navigating, Navigated, Navigating, FragmentNavigation).
                    Assert.AreEqual<int>(4, ++eventCount);

                    Assert.AreEqual(testUriWithFragment, this.Frame.Source);
                    Assert.AreEqual(testUriWithFragment, this.Frame.CurrentSource);
                    Assert.AreEqual(fragment, args.Fragment);
                };

            this.EnqueueCallback(() => this.Frame.Navigate(testUri));
            this.EnqueueConditional(() => complete && (this.Frame.Content as Page1).VirtualsCalled.Count == 2);

            // Verify that the page virtuals fired in the correct order
            this.EnqueueCallback(() =>
            {
                var content = this.Frame.Content as Page1;
                Assert.AreEqual<string>("OnNavigatedTo", content.VirtualsCalled[0]);
                Assert.AreEqual<string>("OnFragmentNavigation", content.VirtualsCalled[1]);
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Navigate to a page with a fragment, then to another fragment on that page, verify the events are raised in the correct order")]
        public void NavigateToPageWithFragmentThenSecondFragmentEventSequence()
        {
            bool complete = false;
            int eventCount = 0;

            string fragment = "frag1";
            string fragment2 = "frag2";
            Uri testUri = new Uri(TestPagesPath + "Page1.xaml#" + fragment, UriKind.Relative);
            Uri testWithFragment2uri = new Uri(TestPagesPath + "Page1.xaml#" + fragment2, UriKind.Relative);
            Uri fragment2Uri = new Uri("#" + fragment2, UriKind.Relative);

            // Wire up event handlers
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail();
            this.Frame.Navigating +=
                (sender, args) =>
                {
                    eventCount++;
                    if (eventCount == 1)
                    {
                        Assert.IsNull(this.Frame.Source);
                        Assert.IsNull(this.Frame.CurrentSource);
                        Assert.AreEqual(testUri, args.Uri);
                        Assert.AreEqual(NavigationMode.New, args.NavigationMode);
                    }
                };
            this.Frame.Navigated +=
                 (sender, args) =>
                 {
                     eventCount++;

                     if (eventCount == 2)
                     {
                         Assert.AreEqual(testUri, this.Frame.Source);
                         Assert.AreEqual(testUri, this.Frame.CurrentSource);
                         Assert.AreEqual(testUri, args.Uri);
                     }
                     else if (eventCount == 4)
                     {
                         Assert.AreEqual(testWithFragment2uri, this.Frame.Source);
                         Assert.AreEqual(testWithFragment2uri, this.Frame.CurrentSource);
                         Assert.AreEqual(testWithFragment2uri, args.Uri);
                     }
                 };
            this.Frame.FragmentNavigation +=
                (sender, args) =>
                {
                    eventCount++;

                    if (eventCount == 3)
                    {
                        Assert.AreEqual(testUri, this.Frame.Source);
                        Assert.AreEqual(testUri, this.Frame.CurrentSource);
                        Assert.AreEqual(fragment, args.Fragment);

                        this.Frame.Dispatcher.BeginInvoke(() => this.Frame.Navigate(fragment2Uri));
                    }
                    else if (eventCount == 5)
                    {
                        Assert.AreEqual(testWithFragment2uri, this.Frame.Source);
                        Assert.AreEqual(testWithFragment2uri, this.Frame.CurrentSource);
                        Assert.AreEqual(fragment2, args.Fragment);

                        // Test complete
                        complete = true;
                    }
                };

            this.EnqueueCallback(() => this.Frame.Navigate(testUri));
            this.EnqueueConditional(() => complete && (this.Frame.Content as Page1).VirtualsCalled.Count == 3);

            // Verify that the page virtuals fired in the correct order
            this.EnqueueCallback(() =>
            {
                var content = this.Frame.Content as Page1;
                Assert.AreEqual<string>("OnNavigatedTo", content.VirtualsCalled[0]);
                Assert.AreEqual<string>("OnFragmentNavigation", content.VirtualsCalled[1]);
                Assert.AreEqual<string>("OnFragmentNavigation", content.VirtualsCalled[2]);
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Navigate to Page1, then to Page2, verify that the events are raised in the correct order")]
        public void NavigateToPageThenAnotherPageEventSequence()
        {
            bool complete = false;
            int eventCount = 0;
            Page1 page1Content = null;
            Page2 page2Content = null;

            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);

            // Wire up event handlers
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail();
            this.Frame.Navigating +=
                (sender, args) =>
                {
                    eventCount++;

                    if (eventCount == 1)
                    {
                        Assert.IsNull(this.Frame.Source);
                        Assert.IsNull(this.Frame.CurrentSource);
                        Assert.AreEqual(page1Uri, args.Uri);
                        Assert.AreEqual(NavigationMode.New, args.NavigationMode);
                    }
                    else if (eventCount == 3)
                    {
                        Assert.AreEqual(page1Uri, this.Frame.Source);
                        Assert.AreEqual(page1Uri, this.Frame.CurrentSource);
                        Assert.AreEqual(page2Uri, args.Uri);
                        Assert.AreEqual(NavigationMode.New, args.NavigationMode);
                    }
                };
            this.Frame.Navigated +=
                 (sender, args) =>
                 {
                     eventCount++;

                     if (eventCount == 2)
                     {
                         Assert.AreEqual(page1Uri, this.Frame.Source);
                         Assert.AreEqual(page1Uri, this.Frame.CurrentSource);
                         Assert.AreEqual(page1Uri, args.Uri);

                         page1Content = this.Frame.Content as Page1;

                         this.Frame.Dispatcher.BeginInvoke(() => this.Frame.Navigate(page2Uri));
                     }
                     else if (eventCount == 4)
                     {
                         Assert.AreEqual(page2Uri, this.Frame.Source);
                         Assert.AreEqual(page2Uri, this.Frame.CurrentSource);
                         Assert.AreEqual(page2Uri, args.Uri);

                         page2Content = this.Frame.Content as Page2;

                         complete = true;
                     }
                 };

            this.EnqueueCallback(() => this.Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete &&
                                          page1Content != null && page1Content.VirtualsCalled.Count == 3 &&
                                          page2Content != null && page2Content.VirtualsCalled.Count == 1);

            // Verify that the page virtuals fired in the correct order
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual<string>("OnNavigatedTo", page1Content.VirtualsCalled[0]);
                Assert.AreEqual<string>("OnNavigatingFrom", page1Content.VirtualsCalled[1]);
                Assert.AreEqual<string>("OnNavigatedFrom", page1Content.VirtualsCalled[2]);

                Assert.AreEqual<string>("OnNavigatedTo", page2Content.VirtualsCalled[0]);
            });

            this.EnqueueTestComplete();
        }

        #endregion Event Sequence tests

        #endregion
    }
}
