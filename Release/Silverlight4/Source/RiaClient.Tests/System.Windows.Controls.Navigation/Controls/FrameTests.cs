//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Browser;
using System.Windows.Navigation;
using System.Windows.Navigation.UnitTests;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NavigationResource = System.Windows.Navigation.Resource;
using NavigationUnitTests1 = Microsoft.AppFx.UnitTests.NavigationUnitTests1;
using NavigationUnitTests2 = Microsoft.AppFx.UnitTests.NavigationUnitTests2;

namespace System.Windows.Controls.UnitTests
{
    [TestClass]
    public class FrameTests : SilverlightTest
    {
        #region Static fields and constants

        private static readonly string TestPagesPath = @"/System.Windows.Controls.Navigation/Controls/TestPages/";
        private static readonly string NavigationUnitTests1Path = @"/Microsoft.AppFx.UnitTests.Silverlight.NavigationUnitTests1;component/";
        private static readonly string NavigationUnitTests2Path = @"/Microsoft.AppFx.UnitTests.Silverlight.NavigationUnitTests2;component/";

        #endregion

        #region Test Fields and Properties

        private string _pageTitle = string.Empty;

        public Frame Frame { get; set; }

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
                this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
                NavigationEventRecordList.SetShouldRecord(this.Frame, false);
            }
            this.TestPanel.Children.Clear();
            this.TestPanel.UpdateLayout();

            Application.Current.Host.NavigationState = "#";

            // Create a new navigator
            this.Frame = new Frame();
            NavigationEventRecordList.Reset();
            NavigationEventRecordList.SetShouldRecord(this.Frame, true);
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
            Assert.IsFalse(this.Frame.NavigationService.Journal.UseNavigationState);

            // Set to UsesParentJournal and verify results
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            Assert.AreEqual(JournalOwnership.UsesParentJournal, this.Frame.JournalOwnership);
            Assert.IsInstanceOfType(this.Frame.NavigationService.Journal, typeof(Journal));
            Assert.IsTrue(this.Frame.NavigationService.Journal.UseNavigationState);
        }

        [TestMethod]
        [Description("Verifies the JournalOwnership property behaves correctly in a nested frame situation.")]
        public void JournalOwnershipBehaviorForNestedFrame()
        {
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
            this.TestPanel.UpdateLayout();

            Frame innerFrame = new Frame();
            this.Frame.Content = innerFrame;

            this.TestPanel.UpdateLayout();

            // Verify JournalOwnership is Automatic by default
            Assert.AreEqual(JournalOwnership.Automatic, this.Frame.JournalOwnership);

            // Set to OwnsJournal and verify results
            innerFrame.JournalOwnership = JournalOwnership.OwnsJournal;
            Assert.AreEqual(JournalOwnership.OwnsJournal, innerFrame.JournalOwnership);
            Assert.IsInstanceOfType(innerFrame.NavigationService.Journal, typeof(Journal));
            Assert.IsFalse(innerFrame.NavigationService.Journal.UseNavigationState);

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

        [Asynchronous]
        [TestMethod]
        [Description("Tests the Frame's default properties.")]
        public void DefaultProperties()
        {
            bool isLoaded = false;

            this.Frame.Loaded += (sender, args) => isLoaded = true;
            this.EnqueueCallback(() => this.TestPanel.Children.Add(this.Frame));
            this.EnqueueConditional(() => isLoaded);

            this.EnqueueCallback(() =>
            {
                Assert.IsInstanceOfType(this.Frame.NavigationService.ContentLoader, typeof(PageResourceContentLoader));
                Assert.IsNotNull(this.Frame.NavigationService);
                Assert.AreEqual(JournalOwnership.Automatic, this.Frame.JournalOwnership);
                Assert.IsNull(this.Frame.Source);
                Assert.IsNull(this.Frame.CurrentSource);
                Assert.IsFalse(this.Frame.CanGoBack);
                Assert.IsFalse(this.Frame.CanGoForward);
                Assert.IsNull(this.Frame.UriMapper);
                Assert.AreEqual(10, this.Frame.CacheSize);
            });

            this.EnqueueTestComplete();
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
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
            int complete = 0;
            bool verifyFirstBackOperation = false,
                 verifySecondBackOperation = false;
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            Uri page3Uri = new Uri(TestPagesPath + "Page3.xaml", UriKind.Relative);

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for third navigation
                    if (complete == 3)
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
                    else if (complete == 4)
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

                    complete++;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => complete == 2);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page3Uri));
            this.EnqueueConditional(() => complete == 3);

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
            int complete = 0;
            bool verifyFirstBackOperation = false,
                 verifySecondBackOperation = false;

            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            Uri page3Uri = new Uri(TestPagesPath + "Page3.xaml", UriKind.Relative);

            // This should be enabled by default but let's be explicit.
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for third navigation
                    if (complete == 3)
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
                    else if (complete == 4)
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

                    complete++;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => complete == 2);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page3Uri));
            this.EnqueueConditional(() => complete == 3);

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
            int complete = 0;
            bool stopped = false;
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for third navigation
                    if (complete > 1)
                    {
                        // Failure
                        Assert.Fail();
                    }

                    complete++;
                };
            Frame.Navigating +=
                (sender, args) =>
                {
                    if (complete == 2)
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

                    stopped = true;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => complete == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => stopped);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame GoBack method and cancellation with browser history integration.")]
        public void NavigationBackCancellationBrowserIntegrated()
        {
            int complete = 0;
            bool stopped = false;
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);

            // This should be enabled by default but let's be explicit.
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for third navigation
                    if (complete > 1)
                    {
                        // Failure
                        Assert.Fail();
                    }

                    complete++;
                };
            Frame.Navigating +=
                (sender, args) =>
                {
                    if (complete == 2)
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

                    stopped = true;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => complete == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => stopped);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame GoForward method.")]
        public void NavigationForward()
        {
            int complete = 0;
            bool verifyFirstForwardOperation = false,
                 verifySecondForwardOperation = false;
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            Uri page3Uri = new Uri(TestPagesPath + "Page3.xaml", UriKind.Relative);

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    if (complete == 5)
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
                    else if (complete == 6)
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

                    complete++;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => complete == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page3Uri));
            this.EnqueueConditional(() => complete == 3);

            // Fourth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 4);

            // Fifth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 5);

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
            int complete = 0;
            bool verifyFirstForwardOperation = false,
                 verifySecondForwardOperation = false;

            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            Uri page3Uri = new Uri(TestPagesPath + "Page3.xaml", UriKind.Relative);

            // This should be enabled by default but let's be explicit.
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    if (complete == 5)
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
                    else if (complete == 6)
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

                    complete++;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => complete == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page3Uri));
            this.EnqueueConditional(() => complete == 3);

            // Fourth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 4);

            // Fifth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 5);

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
            int complete = 0;
            bool stopped = false;
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for fourth navigation
                    if (complete > 3)
                    {
                        Assert.Fail();
                    }

                    complete++;
                };
            Frame.Navigating +=
                (sender, args) =>
                {
                    if (complete == 3)
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

                    stopped = true;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => complete == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 3);

            // Fourth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => stopped);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame GoForward method cancellation with browser history integration.")]
        public void NavigationForwardCancellationBrowserIntegrated()
        {
            int complete = 0;
            bool stopped = false;
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);

            // This should be enabled by default but let's be explicit.
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Check for fourth navigation
                    if (complete > 3)
                    {
                        Assert.Fail();
                    }

                    complete++;
                };
            Frame.Navigating +=
                (sender, args) =>
                {
                    if (complete == 3)
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

                    stopped = true;
                };

            // First navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => complete == 2);

            // Third navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 3);

            // Fourth navigation, wait for completion.
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => stopped);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests that Source property can trigger and reflect navigation operations.")]
        public void SourceProperty()
        {
            int complete = 0;
            Uri page1 = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2 = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            this.Frame.Navigated += (sender, args) => complete++;

            // First navigation, wait for completion.
            this.EnqueueCallback(() =>
            {
                Frame.Source = page1;
                Assert.AreNotEqual<Uri>(this.Frame.Source, this.Frame.CurrentSource);
                Assert.IsNull(this.Frame.CurrentSource);
            });
            this.EnqueueConditional(() => complete == 1);

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
            this.EnqueueConditional(() => complete == 2);

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

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
            int complete = 0;
            bool ready = false;
            string newGuid = Guid.NewGuid().ToString();
            string sourceString = TestPagesPath + "Page1.xaml";
            Uri sourceUri = new Uri(sourceString, UriKind.Relative);
            string deeplinkString = TestPagesPath + "Page2.xaml";
            Uri deeplinkUri = new Uri(deeplinkString, UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
                    complete++;
                };

            // First, navigate by source.
            this.EnqueueCallback(() => this.Frame.Source = sourceUri);
            this.EnqueueConditional(() => complete == 1);

            // 
            this.EnqueueCallback(() =>
            {
                ready = true;

                // Prepare for deeplink
                this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;

                // Check for deeplinks
                this.Frame.ApplyDeepLinks();
            });

            this.EnqueueConditional(() => complete == 2 &&
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
            int complete = 0;
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "PageEmptyStringTitle.xaml", UriKind.Relative);
            Uri page3Uri = new Uri(TestPagesPath + "Page3.xaml", UriKind.Relative);
            Uri page4Uri = new Uri(TestPagesPath + "PageNullStringTitle.xaml", UriKind.Relative);

            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            this.Frame.Navigated += (sender, args) => complete++;

            this.EnqueueCallback(() => this.Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete == 1);
            this.EnqueueCallback(
                () =>
                {
                    Assert.IsInstanceOfType(this.Frame.Content, typeof(Page1));
                    Assert.AreEqual<string>("Page1 Page", BrowserHelper.Title);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);
                });

            this.EnqueueCallback(() => this.Frame.Navigate(page2Uri));
            this.EnqueueConditional(() => complete == 2);
            this.EnqueueCallback(
                () =>
                {
                    Assert.IsInstanceOfType(this.Frame.Content, typeof(PageEmptyStringTitle));
                    Assert.AreEqual<string>("", BrowserHelper.Title);
                    Assert.AreEqual<Uri>(page2Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page2Uri, this.Frame.CurrentSource);
                });

            this.EnqueueCallback(() => this.Frame.Navigate(page3Uri));
            this.EnqueueConditional(() => complete == 3);
            this.EnqueueCallback(
                () =>
                {
                    Assert.IsInstanceOfType(this.Frame.Content, typeof(Page3));
                    Assert.AreEqual<string>("Page3 Page", BrowserHelper.Title);
                    Assert.AreEqual<Uri>(page3Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(page3Uri, this.Frame.CurrentSource);
                });

            this.EnqueueCallback(() => this.Frame.Navigate(page4Uri));
            this.EnqueueConditional(() => complete == 4);
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
            int complete = 0;
            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            this.Frame.Navigated += (sender, args) => complete++;

            ThreadPool.QueueUserWorkItem(state => this.Frame.Navigate(page1Uri));

            this.EnqueueConditional(() => complete == 1);
            this.EnqueueCallback(() =>
            {
                Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);
                Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);

                Assert.IsInstanceOfType(this.Frame.Content, typeof(Page1));

                Assert.ReferenceEquals((this.Frame.Content as Page).NavigationService, this.Frame.NavigationService);
            });
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Navigating to a fragment when the Frame has never had content should raise NavigationFailed.")]
        public void FragmentNavigationInitiallyShouldFail()
        {
            bool completed = false;
            Uri fragmentUri = new Uri("#fragment", UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            // Wire up events
            Frame.NavigationFailed +=
                (sender, args) =>
                {
                    Assert.AreEqual(fragmentUri, args.Uri);
                    Assert.IsInstanceOfType(args.Exception, typeof(InvalidOperationException));

                    args.Handled = true;
                    completed = true;
                };
            Frame.Navigated +=
                (sender, args) =>
                {
                    Assert.Fail("Navigated was raised when this should have failed.");
                };

            // Perform a navigation operation by Uri
            this.EnqueueCallback(() => Frame.Navigate(fragmentUri));
            this.EnqueueConditional(() => completed);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Navigating to a UserControl is supported")]
        public void NavigationToUserControl()
        {
            bool completed = false;
            Uri userControl1Uri = new Uri(TestPagesPath + "UserControl1.xaml", UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Verify contents of event args
                    Assert.IsTrue(args.Content is UserControl1);

                    // Verify contents in navigator content presenter
                    Assert.IsTrue(Frame.Content is UserControl1);

                    // Verify Uri
                    Assert.AreEqual<Uri>(userControl1Uri, args.Uri);
                    Assert.AreEqual<Uri>(userControl1Uri, this.Frame.Source);
                    Assert.AreEqual<Uri>(userControl1Uri, this.Frame.CurrentSource);
                                        
                    Assert.AreEqual<int>(0, this.Frame.NavigationService.Journal.BackStack.Count);
                    Assert.AreEqual<int>(0, this.Frame.NavigationService.Journal.ForwardStack.Count);

                    completed = true;
                };

            // Perform a navigation operation by Uri
            this.EnqueueCallback(() => Frame.Navigate(userControl1Uri));
            this.EnqueueConditional(() => completed);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Navigating to a non-UserControl (a ContentControl in this test) raises NavigationFailed")]
        public void NavigationFailsWhenNotUserControl()
        {
            bool completed = false;
            Uri userControlUri = new Uri(TestPagesPath + "ContentControl1.xaml", UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            // Wire up events
            Frame.NavigationFailed +=
                (sender, args) =>
                {
                    Assert.AreEqual(userControlUri, args.Uri);
                    Assert.IsInstanceOfType(args.Exception, typeof(InvalidOperationException));

                    string exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                                                            NavigationResource.NavigationService_ContentIsNotAUserControl,
                                                            typeof(ContentControl1).ToString(),
                                                            "System.Windows.Controls.UserControl");
                    Assert.AreEqual(exceptionMessage, args.Exception.Message);

                    args.Handled = true;
                    completed = true;
                };
            Frame.Navigated +=
                (sender, args) =>
                {
                    Assert.Fail("Navigated was raised when this should have failed.");
                };

            // Perform a navigation operation by Uri
            this.EnqueueCallback(() => Frame.Navigate(userControlUri));
            this.EnqueueConditional(() => completed);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("If a mapping is set up from 'abc' -> '/Page1.xaml', then loading abc, followed by loading /Page1.xaml, should result in two instances of that page being loaded.  Regression check for Bug #661300")]
        public void NavigatingToPageByUnmappedAndMappedUri()
        {
            int complete = 0;
            string page1String = TestPagesPath + "Page1.xaml";
            Uri page1Uri = new Uri(page1String, UriKind.Relative);
            string abcString = "abc";
            Uri abcUri = new Uri(abcString, UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            Page1 firstInstance = null;

            UriMapping mapping = new UriMapping();
            mapping.Uri = abcUri;
            mapping.MappedUri = page1Uri;

            UriMapper mapper = new UriMapper();
            mapper.UriMappings.Add(mapping);
            Frame.UriMapper = mapper;

            // Wire up events
            Frame.NavigationFailed += (sender, args) => Assert.Fail(args.Exception.Message);
            Frame.Navigated +=
                (sender, args) =>
                {
                    // Verify contents of event args
                    Assert.IsTrue(args.Content is Page1);

                    // Verify contents in navigator content presenter
                    Assert.IsTrue(Frame.Content is Page1);

                    // Verify navigation context
                    Page1 frameContent = Frame.Content as Page1;
                    Assert.IsNotNull(frameContent.NavigationContext);
                    Assert.IsNotNull(frameContent.NavigationContext.QueryString);

                    Assert.AreEqual<int>(0, frameContent.NavigationContext.QueryString.Count);

                    Assert.ReferenceEquals(frameContent.NavigationService, this.Frame.NavigationService);

                    Assert.AreEqual<int>(complete, this.Frame.NavigationService.Journal.BackStack.Count);
                    Assert.AreEqual<int>(0, this.Frame.NavigationService.Journal.ForwardStack.Count);


                    if (complete == 0)
                    {
                        firstInstance = Frame.Content as Page1;

                        // Verify Uri
                        Assert.AreEqual<Uri>(page1Uri, args.Uri);
                        Assert.AreEqual<Uri>(page1Uri, this.Frame.Source);
                        Assert.AreEqual<Uri>(page1Uri, this.Frame.CurrentSource);
                    }
                    else if (complete == 1)
                    {
                        // Verify Uri
                        Assert.AreEqual<Uri>(abcUri, args.Uri);
                        Assert.AreEqual<Uri>(abcUri, this.Frame.Source);
                        Assert.AreEqual<Uri>(abcUri, this.Frame.CurrentSource);

                        // Verify the instance was not re-used
                        Assert.IsFalse(Object.ReferenceEquals(firstInstance, frameContent));
                    }

                    complete++;
                };

            // Perform a navigation operation by Uri
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete == 1);

            this.EnqueueCallback(() => Frame.Navigate(abcUri));
            this.EnqueueConditional(() => complete == 2);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Even when an unusual deeplink is injected, NavigationFailed should always be raised instead of an exception being thrown")]
        public void UnusualDeeplinksStillRaiseNavigationFailed()
        {
            string deeplinkString = @"\')-alert()-(\'";
            bool complete = false;

            HtmlPage.Window.CurrentBookmark = deeplinkString;

            this.Frame.NavigationFailed +=
                (sender, args) =>
                {
                    args.Handled = true;
                    complete = true;
                };
            this.Frame.Navigated += (sender, args) => Assert.Fail("Navigated should not be raised in this test.");

            // Setting the JournalOwnership to UsesParentJournal will force it to check for deeplinks,
            // discover the deeplink set above, and navigate to it.  However, this deeplink will fail
            // to go anywhere, so we verify that NavigationFailed is raised.
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            this.EnqueueConditional(() => complete);
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("If no Source is set, but a UriMapping exists that goes from null/empty -> something, use that as the 'home page'")]
        public void NoSourceWithEmptyUriMappingLoadsDefaultContent()
        {
            // Make sure the Frame isn't loaded by using a new Frame (not the one set up in the unit tests normally
            this.Cleanup();
            this.Frame = new Frame();

            bool complete = false;
            UriMapper mapper = new UriMapper();
            UriMapping emptyMapping = new UriMapping();
            emptyMapping.MappedUri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            mapper.UriMappings.Add(emptyMapping);

            this.Frame.UriMapper = mapper;

            this.Frame.Navigated +=
                (sender, args) =>
                {
                    Assert.AreEqual(String.Empty, args.Uri.OriginalString);

                    Assert.IsInstanceOfType(args.Content, typeof(Page1));
                    Assert.IsInstanceOfType(this.Frame.Content, typeof(Page1));

                    Assert.AreEqual(String.Empty, this.Frame.Source.OriginalString);
                    Assert.AreEqual(String.Empty, this.Frame.CurrentSource.OriginalString);

                    complete = true;
                };

            this.TestPanel.Children.Add(this.Frame);

            this.EnqueueConditional(() => complete);

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Calling Navigate before Loaded is raised (ex: in the constructor of a derived Frame) will navigate to that page once Loaded occurs")]
        public void NavigatingBeforeLoadedNavigatesUponLoaded()
        {
            bool complete = false;
            this.Cleanup();
            TestDerivedFrame derivedFrame = new TestDerivedFrame();
            derivedFrame.JournalOwnership = JournalOwnership.OwnsJournal;
            derivedFrame.Source = new Uri(TestDerivedFrame.DefaultSource, UriKind.Relative);

            derivedFrame.Navigated +=
                (sender, args) =>
                {
                    // Verify that we ended up at the thing navigated to in the constructor, NOT the Source
                    Assert.AreEqual(TestDerivedFrame.DefaultPage, args.Uri.OriginalString);

                    Assert.IsInstanceOfType(args.Content, typeof(Page1));
                    Assert.IsInstanceOfType(derivedFrame.Content, typeof(Page1));

                    Assert.AreEqual(TestDerivedFrame.DefaultPage, derivedFrame.Source.OriginalString);
                    Assert.AreEqual(TestDerivedFrame.DefaultPage, derivedFrame.CurrentSource.OriginalString);

                    complete = true;
                };

            this.TestPanel.Children.Add(derivedFrame);

            this.EnqueueConditional(() => complete);

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Navigating between XAML in the entry point assembly and non-entry point assembly XAML, using Navigate, GoBack, and GoForward all work correctly")]
        public void NonEntryPointAssemblyXAML()
        {
            Uri entryPointUri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri nonEntryPoint1Uri = new Uri(NavigationUnitTests1Path + "Page1.xaml", UriKind.Relative);
            Uri nonEntryPoint2Uri = new Uri(NavigationUnitTests2Path + "Page1.xaml", UriKind.Relative);
            int complete = 0;

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            this.Frame.Navigated +=
                (sender, args) =>
                {
                    if (complete == 0 ||
                        complete == 4)
                    {
                        Assert.AreEqual(entryPointUri, args.Uri);

                        Assert.IsInstanceOfType(args.Content, typeof(Page1));
                        Assert.IsInstanceOfType(this.Frame.Content, typeof(Page1));

                        Assert.AreEqual(entryPointUri, this.Frame.Source);
                        Assert.AreEqual(entryPointUri, this.Frame.CurrentSource);
                    }

                    else if (complete == 1 ||
                             complete == 3 ||
                             complete == 5)
                    {
                        Assert.AreEqual(nonEntryPoint1Uri, args.Uri);

                        Assert.IsInstanceOfType(args.Content, typeof(NavigationUnitTests1.Page1));
                        Assert.IsInstanceOfType(this.Frame.Content, typeof(NavigationUnitTests1.Page1));

                        Assert.AreEqual(nonEntryPoint1Uri, this.Frame.Source);
                        Assert.AreEqual(nonEntryPoint1Uri, this.Frame.CurrentSource);
                    }

                    else if (complete == 2 ||
                             complete == 6)
                    {
                        Assert.AreEqual(nonEntryPoint2Uri, args.Uri);

                        Assert.IsInstanceOfType(args.Content, typeof(NavigationUnitTests2.Page1));
                        Assert.IsInstanceOfType(this.Frame.Content, typeof(NavigationUnitTests2.Page1));

                        Assert.AreEqual(nonEntryPoint2Uri, this.Frame.Source);
                        Assert.AreEqual(nonEntryPoint2Uri, this.Frame.CurrentSource);
                    }

                    complete++;
                };

            this.EnqueueCallback(() => this.Frame.Navigate(entryPointUri));
            this.EnqueueConditional(() => complete == 1);

            this.EnqueueCallback(() => this.Frame.Navigate(nonEntryPoint1Uri));
            this.EnqueueConditional(() => complete == 2);

            this.EnqueueCallback(() => this.Frame.Navigate(nonEntryPoint2Uri));
            this.EnqueueConditional(() => complete == 3);

            this.EnqueueCallback(() => this.Frame.GoBack());
            this.EnqueueConditional(() => complete == 4);

            this.EnqueueCallback(() => this.Frame.GoBack());
            this.EnqueueConditional(() => complete == 5);

            this.EnqueueCallback(() => this.Frame.GoForward());
            this.EnqueueConditional(() => complete == 6);

            this.EnqueueCallback(() => this.Frame.GoForward());
            this.EnqueueConditional(() => complete == 7);

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Non-entry point XAML Uri's must begin with '/' to be valid")]
        public void NonEntryPointAssemblyXAMLWithoutLeadingSlashInvalid()
        {
            Uri nonEntryPointUriWithoutSlash = new Uri(NavigationUnitTests1Path.Substring(1) + "Page1.xaml", UriKind.Relative);
            bool complete = false;

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            this.Frame.Navigated += (sender, args) => Assert.Fail("Navigated should never happen in this test.");

            this.Frame.NavigationFailed +=
                (sender, args) =>
                {
                    args.Handled = true;

                    Assert.IsInstanceOfType(args.Exception, typeof(ArgumentException));
                    Assert.AreEqual(nonEntryPointUriWithoutSlash, args.Uri);
                    complete = true;
                };

            this.EnqueueCallback(() => this.Frame.Navigate(nonEntryPointUriWithoutSlash));
            this.EnqueueConditional(() => complete);

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Updating the Page's Title after it has been navigated to updates the browser's title")]
        public void UpdatingTitleAfterNavigationUpdatesInBrowser()
        {
            bool complete = false;
            string page1String = TestPagesPath + "Page1.xaml";
            Uri page1Uri = new Uri(page1String, UriKind.Relative);

            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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

                    Assert.AreEqual<int>(0, this.Frame.NavigationService.Journal.BackStack.Count);
                    Assert.AreEqual<int>(0, this.Frame.NavigationService.Journal.ForwardStack.Count);

                    Assert.AreEqual<string>("Page1 Page", BrowserHelper.Title);

                    (args.Content as Page).Title = "New Title";

                    Assert.AreEqual<string>("New Title", BrowserHelper.Title);

                    complete = true;
                };

            // Perform a navigation operation by Uri
            this.EnqueueCallback(() => Frame.Navigate(page1Uri));
            this.EnqueueConditional(() => complete);

            // Success
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Tests the Frame Navigation method while integrated with browser, when a query string is passed to the page that includes a parameter that escapes an '&'.")]
        public void NavigationWithQueryStringAndEscapedParameterBrowserIntegrated()
        {
            int completed = 0;
            string page1String = TestPagesPath + "Page1.xaml?field1=val1&field2=val2%26f%3Df&field3=val3"; //"%26f%3Df" is "&f=f" percent-encoded
            Uri page1Uri = new Uri(page1String, UriKind.Relative);
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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

                    Assert.AreEqual<int>(3, frameContent.NavigationContext.QueryString.Count);

                    Assert.AreEqual<string>("val1", frameContent.NavigationContext.QueryString["field1"]);
                    Assert.AreEqual<string>("val2&f=f", frameContent.NavigationContext.QueryString["field2"]);
                    Assert.AreEqual<string>("val3", frameContent.NavigationContext.QueryString["field3"]);
                    Assert.IsFalse(frameContent.NavigationContext.QueryString.ContainsKey("f"));

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
        [Description("Frame with a UriMapper with mappings that do not handle an empty Uri should not attempt to navigate to an empty Uri on load")]
        public void ShouldNotNavigateToEmptyUriOnLoadWhenUriMapperThatDoesNotMapEmptyUri()
        {
            // Make sure the Frame isn't loaded by using a new Frame (not the one set up in the unit tests normally
            this.Cleanup();
            this.Frame = new Frame();
            this.Frame.Navigated += (sender, args) => Assert.Fail("Frame should never navigate during this test");

            bool loaded = false;
            UriMapper mapper = new UriMapper();
            UriMapping nonEmptyMapping = new UriMapping();
            nonEmptyMapping.Uri = new Uri("abc", UriKind.Relative);
            nonEmptyMapping.MappedUri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            mapper.UriMappings.Add(nonEmptyMapping);

            this.Frame.UriMapper = mapper;

            this.Frame.Loaded +=
                (sender, args) =>
                {
                    loaded = true;
                };

            this.TestPanel.Children.Add(this.Frame);

            this.EnqueueConditional(() => loaded);

            // Briefly pause in case Frame would try to Navigate
            this.EnqueueDelay(500);

            this.EnqueueTestComplete();
        }

        #region Event Sequence tests

        private Uri ReentrancyUri(string pageNumber)
        {
            return new Uri(TestPagesPath + "Reentrancy/ReentrancyPage" + pageNumber + ".xaml", UriKind.Relative);
        }

        [TestMethod]
        [Asynchronous]
        [Description("Try navigating in OnNavigatedTo - verify events and virtuals are raised/called in the expected order.  This test uses a browser-integrated journal.")]
        public void NavigateInOnNavigatedToWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
            this.NavigateInOnNavigatedToCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Try navigating in OnNavigatedTo - verify events and virtuals are raised/called in the expected order.")]
        public void NavigateInOnNavigatedTo()
        {
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
            this.NavigateInOnNavigatedToCore();
        }

        private void NavigateInOnNavigatedToCore()
        {
            bool complete = false;

            this.Frame.Navigating += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", args.Uri));
                    }
                };
            this.Frame.Navigated += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigated", args.Uri));
                    }
                };
            this.Frame.NavigationStopped += (sender, args) => Assert.Fail("NavigationStopped should never occur during this test.");
            this.Frame.FragmentNavigation += (sender, args) => Assert.Fail("FragmentNavigation should never occur during this test.");
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail("NavigationFailed should never occur during this test.");

            NavigationEventRecordList.EventRecords.CollectionChanged += new NotifyCollectionChangedEventHandler(
                (obj, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Add)
                    {
                        NavigationEventRecord record = args.NewItems[0] as NavigationEventRecord;
                        if (record.GeneratedBy == typeof(ReentrancyPage2) &&
                            record.Name == "OnNavigatedTo")
                        {
                            complete = true;
                        }
                    }
                });

            // Here's the order we expect things in
            List<NavigationEventRecord> expectedRecords = new List<NavigationEventRecord>();
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("1")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigated", ReentrancyUri("1")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage1), "OnNavigatedTo", ReentrancyUri("1")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("2")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage1), "OnNavigatingFrom", ReentrancyUri("2")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigated", ReentrancyUri("2")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage1), "OnNavigatedFrom", ReentrancyUri("2")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage2), "OnNavigatedTo", ReentrancyUri("2")));

            DateTime testBegunTime = DateTime.Now;
            DateTime testEndTime = DateTime.Now;

            this.EnqueueCallback(() => 
                {
                    NavigationEventRecordList.EventRecords.Clear();
                    testBegunTime = DateTime.Now;
                    this.Frame.Navigate(ReentrancyUri("1"));
                });

            this.EnqueueConditional(() =>
            {
                testEndTime = DateTime.Now;
                if ((testEndTime - testBegunTime).Seconds > 10)
                {
                    Assert.Fail("Timed out");
                }
                return complete;
            });

            this.EnqueueCallback(() =>
            {
                bool shouldAssert = false;

                if (expectedRecords.Count != NavigationEventRecordList.EventRecords.Count)
                {
                    shouldAssert = true;
                }
                
                for (int i = 0; i < expectedRecords.Count; i++)
                {
                    try
                    {
                        if (expectedRecords[i].GeneratedBy != NavigationEventRecordList.EventRecords[i].GeneratedBy)
                        {
                            shouldAssert = true;
                        }
                        if (expectedRecords[i].Name != NavigationEventRecordList.EventRecords[i].Name)
                        {
                            shouldAssert = true;
                        }
                        if (expectedRecords[i].Uri != NavigationEventRecordList.EventRecords[i].Uri)
                        {
                            shouldAssert = true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (shouldAssert)
                {
                    string error = String.Empty;
                    error += "Expected:<br>";
                    foreach (var record in expectedRecords)
                    {
                        error += "GB:" + record.GeneratedBy.ToString().Substring(record.GeneratedBy.ToString().LastIndexOf('.')) + ", Name:" + record.Name + ", Uri:" + record.Uri.OriginalString.Substring(record.Uri.OriginalString.LastIndexOf('/')) + "<br>";
                    }
                    error += "<br>Actual:<br>";
                    foreach (var record in NavigationEventRecordList.EventRecords)
                    {
                        error += "GB:" + record.GeneratedBy.ToString().Substring(record.GeneratedBy.ToString().LastIndexOf('.')) + ", Name:" + record.Name + ", Uri:" + record.Uri.OriginalString.Substring(record.Uri.OriginalString.LastIndexOf('/')) + "<br>";
                    }
                    Assert.Fail(error);
                }
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Try navigating in OnNavigatingFrom - verify events and virtuals are raised/called in the expected order.  This test uses a browser-integrated journal.")]
        public void NavigateInOnNavigatingFromWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
            this.NavigateInOnNavigatingFromCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Try navigating in OnNavigatingFrom - verify events and virtuals are raised/called in the expected order.")]
        public void NavigateInOnNavigatingFrom()
        {
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
            this.NavigateInOnNavigatingFromCore();
        }

        private void NavigateInOnNavigatingFromCore()
        {
            int complete = 0;
            bool finalEventRecorded = false;

            this.Frame.Navigating += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", args.Uri));
                    }
                };
            this.Frame.Navigated += (sender, args) => 
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigated", args.Uri));
                    }
                    complete++;
                };
            this.Frame.NavigationStopped += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", args.Uri));
                    }
                };
            this.Frame.FragmentNavigation += (sender, args) => Assert.Fail("FragmentNavigation should never occur during this test.");
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail("NavigationFailed should never occur during this test.");

            NavigationEventRecordList.EventRecords.CollectionChanged += new NotifyCollectionChangedEventHandler(
                (obj, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Add)
                    {
                        NavigationEventRecord record = args.NewItems[0] as NavigationEventRecord;
                        if (record.GeneratedBy == typeof(ReentrancyPage5) &&
                            record.Name == "OnNavigatedTo")
                        {
                            finalEventRecorded = true;
                        }
                    }
                });

            // Here's the order we expect things in
            List<NavigationEventRecord> expectedRecords = new List<NavigationEventRecord>();
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage7), "OnNavigatingFrom", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage7), "OnNavigatingFrom", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("9")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage7), "OnNavigatingFrom", ReentrancyUri("9")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage7), "OnNavigatingFrom", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", ReentrancyUri("9")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigated", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage7), "OnNavigatedFrom", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage5), "OnNavigatedTo", ReentrancyUri("5")));

            DateTime testBegunTime = DateTime.Now;
            DateTime testEndTime = DateTime.Now;

            this.EnqueueCallback(() => this.Frame.Navigate(ReentrancyUri("7")));
            this.EnqueueConditional(() => complete == 1);

            this.EnqueueCallback(() =>
            {
                NavigationEventRecordList.EventRecords.Clear();
                testBegunTime = DateTime.Now;
                this.Frame.Navigate(ReentrancyUri("3"));
                this.Frame.Navigate(ReentrancyUri("4")); // this will get canceled, and a new navigation will begin to "9"
                this.Frame.Navigate(ReentrancyUri("5"));
            });

            this.EnqueueConditional(() =>
            {
                testEndTime = DateTime.Now;
                if ((testEndTime - testBegunTime).Seconds > 10)
                {
                    Assert.Fail("Timed out");
                }
                return finalEventRecorded;
            });

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(expectedRecords.Count, NavigationEventRecordList.EventRecords.Count, "Number of events is wrong");

                for (int i = 0; i < expectedRecords.Count; i++)
                {
                    Assert.AreEqual(expectedRecords[i].GeneratedBy, NavigationEventRecordList.EventRecords[i].GeneratedBy, "Wrong GeneratedBy for event " + i);
                    Assert.AreEqual(expectedRecords[i].Name, NavigationEventRecordList.EventRecords[i].Name, "Wrong Name for event " + i);
                    Assert.AreEqual(expectedRecords[i].Uri, NavigationEventRecordList.EventRecords[i].Uri, "Wrong Uri for event " + i);
                }
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Try navigating and calling StopLoading in OnNavigatingFrom - verify events and virtuals are raised/called in the expected order.  This test uses a browser-integrated journal.")]
        public void NavigateAndStopLoadingInOnNavigatingFromWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
            this.NavigateAndStopLoadingInOnNavigatingFromCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Try navigating and calling StopLoading in OnNavigatingFrom - verify events and virtuals are raised/called in the expected order.")]
        public void NavigateAndStopLoadingInOnNavigatingFrom()
        {
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
            this.NavigateAndStopLoadingInOnNavigatingFromCore();
        }

        private void NavigateAndStopLoadingInOnNavigatingFromCore()
        {
            int complete = 0;
            bool finalEventRecorded = false;

            this.Frame.Navigating += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", args.Uri));
                    }
                };
            this.Frame.Navigated += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigated", args.Uri));
                    }
                    complete++;
                };
            this.Frame.NavigationStopped += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", args.Uri));
                    }
                };
            this.Frame.FragmentNavigation += (sender, args) => Assert.Fail("FragmentNavigation should never occur during this test.");
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail("NavigationFailed should never occur during this test.");

            NavigationEventRecordList.EventRecords.CollectionChanged += new NotifyCollectionChangedEventHandler(
                (obj, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Add)
                    {
                        NavigationEventRecord record = args.NewItems[0] as NavigationEventRecord;
                        if (record.GeneratedBy == typeof(ReentrancyPage5) &&
                            record.Name == "OnNavigatedTo")
                        {
                            finalEventRecorded = true;
                        }
                    }
                });

            // Here's the order we expect things in
            List<NavigationEventRecord> expectedRecords = new List<NavigationEventRecord>();
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage8), "OnNavigatingFrom", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage8), "OnNavigatingFrom", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("9")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage8), "OnNavigatingFrom", ReentrancyUri("9")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage8), "OnNavigatingFrom", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", ReentrancyUri("9")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigated", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage8), "OnNavigatedFrom", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage5), "OnNavigatedTo", ReentrancyUri("5")));

            DateTime testBegunTime = DateTime.Now;
            DateTime testEndTime = DateTime.Now;

            this.EnqueueCallback(() => this.Frame.Navigate(ReentrancyUri("8")));
            this.EnqueueConditional(() => complete == 1);

            this.EnqueueCallback(() =>
            {
                NavigationEventRecordList.EventRecords.Clear();
                testBegunTime = DateTime.Now;
                this.Frame.Navigate(ReentrancyUri("3"));
                this.Frame.Navigate(ReentrancyUri("4")); // this will get canceled, and a new navigation will begin to "9"
                this.Frame.Navigate(ReentrancyUri("5"));
            });

            this.EnqueueConditional(() =>
            {
                testEndTime = DateTime.Now;
                if ((testEndTime - testBegunTime).Seconds > 10)
                {
                    Assert.Fail("Timed out");
                }
                return finalEventRecorded;
            });

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(expectedRecords.Count, NavigationEventRecordList.EventRecords.Count, "Number of events is wrong");

                for (int i = 0; i < expectedRecords.Count; i++)
                {
                    Assert.AreEqual(expectedRecords[i].GeneratedBy, NavigationEventRecordList.EventRecords[i].GeneratedBy, "Wrong GeneratedBy for event " + i);
                    Assert.AreEqual(expectedRecords[i].Name, NavigationEventRecordList.EventRecords[i].Name, "Wrong Name for event " + i);
                    Assert.AreEqual(expectedRecords[i].Uri, NavigationEventRecordList.EventRecords[i].Uri, "Wrong Uri for event " + i);
                }
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the Frame events fire in correct order during a successful navigation.")]
        public void NavigationByUriEventSequence()
        {
            bool complete = false;
            int eventCount = 0;

            string page1String = TestPagesPath + "Page1.xaml";
            Uri page1Uri = new Uri(page1String, UriKind.Relative);

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
        [Description("Verifies that the Frame processes multiple Uri navigation calls correctly.  This test uses a browser-integrated journal.")]
        public void NavigationMultipleUriEventSequenceWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
            NavigationMultipleUriEventSequenceCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the Frame processes multiple Uri navigation calls correctly")]
        public void NavigationMultipleUriEventSequence()
        {
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
            NavigationMultipleUriEventSequenceCore();
        }

        private void NavigationMultipleUriEventSequenceCore()
        {
            int complete = 0;
            bool finalEventRecorded = false;

            this.Frame.Navigating += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", args.Uri));
                    }
                };
            this.Frame.Navigated += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigated", args.Uri));
                    }
                    complete++;
                };
            this.Frame.NavigationStopped += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", args.Uri));
                    }
                };
            this.Frame.FragmentNavigation += (sender, args) => Assert.Fail("FragmentNavigation should never occur during this test.");
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail("NavigationFailed should never occur during this test.");

            NavigationEventRecordList.EventRecords.CollectionChanged += new NotifyCollectionChangedEventHandler(
                (obj, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Add)
                    {
                        NavigationEventRecord record = args.NewItems[0] as NavigationEventRecord;
                        if (record.GeneratedBy == typeof(ReentrancyPage5) &&
                            record.Name == "OnNavigatedTo")
                        {
                            finalEventRecorded = true;
                        }
                    }
                });

            // Here's the order we expect things in
            List<NavigationEventRecord> expectedRecords = new List<NavigationEventRecord>();
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage2), "OnNavigatingFrom", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage2), "OnNavigatingFrom", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage2), "OnNavigatingFrom", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigated", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage2), "OnNavigatedFrom", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage5), "OnNavigatedTo", ReentrancyUri("5")));

            DateTime testBegunTime = DateTime.Now;
            DateTime testEndTime = DateTime.Now;

            this.EnqueueCallback(() => this.Frame.Navigate(ReentrancyUri("2")));
            this.EnqueueConditional(() => complete == 1);

            this.EnqueueCallback(() =>
            {
                NavigationEventRecordList.EventRecords.Clear();
                testBegunTime = DateTime.Now;
                this.Frame.Navigate(ReentrancyUri("3"));
                this.Frame.Navigate(ReentrancyUri("4"));
                this.Frame.Navigate(ReentrancyUri("5"));
            });

            this.EnqueueConditional(() =>
            {
                testEndTime = DateTime.Now;
                if ((testEndTime - testBegunTime).Seconds > 10)
                {
                    Assert.Fail("Timed out");
                }
                return finalEventRecorded;
            });

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(expectedRecords.Count, NavigationEventRecordList.EventRecords.Count, "Number of events is wrong");

                for (int i = 0; i < expectedRecords.Count; i++)
                {
                    Assert.AreEqual(expectedRecords[i].GeneratedBy, NavigationEventRecordList.EventRecords[i].GeneratedBy, "Wrong GeneratedBy for event " + i);
                    Assert.AreEqual(expectedRecords[i].Name, NavigationEventRecordList.EventRecords[i].Name, "Wrong Name for event " + i);
                    Assert.AreEqual(expectedRecords[i].Uri, NavigationEventRecordList.EventRecords[i].Uri, "Wrong Uri for event " + i);
                }
            });

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the Frame processes multiple Uri navigation calls correctly, when one of them is canceled in Navigating.  This test uses a browser-integrated journal.")]
        public void NavigationMultipleUriWithCancelationEventSequenceWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
            NavigationMultipleUriWithCancelationEventSequenceCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Verifies that the Frame processes multiple Uri navigation calls correctly, when one of them is canceled in Navigating")]
        public void NavigationMultipleUriWithCancelationEventSequence()
        {
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
            NavigationMultipleUriWithCancelationEventSequenceCore();
        }

        private void NavigationMultipleUriWithCancelationEventSequenceCore()
        {
            int complete = 0;
            bool finalEventRecorded = false;

            this.Frame.Navigating += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", args.Uri));
                    }
                };
            this.Frame.Navigated += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigated", args.Uri));
                    }
                    complete++;
                };
            this.Frame.NavigationStopped += (sender, args) =>
                {
                    if (NavigationEventRecordList.GetShouldRecord(this.Frame))
                    {
                        NavigationEventRecordList.EventRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", args.Uri));
                    }
                };
            this.Frame.FragmentNavigation += (sender, args) => Assert.Fail("FragmentNavigation should never occur during this test.");
            this.Frame.NavigationFailed += (sender, args) => Assert.Fail("NavigationFailed should never occur during this test.");

            NavigationEventRecordList.EventRecords.CollectionChanged += new NotifyCollectionChangedEventHandler(
                (obj, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Add)
                    {
                        NavigationEventRecord record = args.NewItems[0] as NavigationEventRecord;
                        if (record.GeneratedBy == typeof(ReentrancyPage5) &&
                            record.Name == "OnNavigatedTo")
                        {
                            finalEventRecorded = true;
                        }
                    }
                });

            // Here's the order we expect things in
            List<NavigationEventRecord> expectedRecords = new List<NavigationEventRecord>();
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage6), "OnNavigatingFrom", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage6), "OnNavigatingFrom", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", ReentrancyUri("4")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigating", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage6), "OnNavigatingFrom", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "NavigationStopped", ReentrancyUri("3")));
            expectedRecords.Add(new NavigationEventRecord(typeof(Frame), "Navigated", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage6), "OnNavigatedFrom", ReentrancyUri("5")));
            expectedRecords.Add(new NavigationEventRecord(typeof(ReentrancyPage5), "OnNavigatedTo", ReentrancyUri("5")));

            DateTime testBegunTime = DateTime.Now;
            DateTime testEndTime = DateTime.Now;

            this.EnqueueCallback(() => this.Frame.Navigate(ReentrancyUri("6")));
            this.EnqueueConditional(() => complete == 1);

            this.EnqueueCallback(() =>
            {
                NavigationEventRecordList.EventRecords.Clear();
                testBegunTime = DateTime.Now;
                this.Frame.Navigate(ReentrancyUri("3"));
                this.Frame.Navigate(ReentrancyUri("4")); // this will get canceled
                this.Frame.Navigate(ReentrancyUri("5"));
            });

            this.EnqueueConditional(() =>
            {
                testEndTime = DateTime.Now;
                if ((testEndTime - testBegunTime).Seconds > 10)
                {
                    Assert.Fail("Timed out");
                }
                return finalEventRecorded;
            });

            this.EnqueueCallback(() =>
            {
                Assert.AreEqual(expectedRecords.Count, NavigationEventRecordList.EventRecords.Count, "Number of events is wrong");

                for (int i = 0; i < expectedRecords.Count; i++)
                {
                    Assert.AreEqual(expectedRecords[i].GeneratedBy, NavigationEventRecordList.EventRecords[i].GeneratedBy, "Wrong GeneratedBy for event " + i);
                    Assert.AreEqual(expectedRecords[i].Name, NavigationEventRecordList.EventRecords[i].Name, "Wrong Name for event " + i);
                    Assert.AreEqual(expectedRecords[i].Uri, NavigationEventRecordList.EventRecords[i].Uri, "Wrong Uri for event " + i);
                }
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

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
        [Description("Navigate to a page with a fragment, where the fragment is hidden by a UriMapping")]
        public void NavigateToFragmentWithMapping()
        {
            bool complete = false;
            int eventCount = 0;

            string fragment = "frag1";
            Uri testUri = new Uri("/Page1/" + fragment, UriKind.Relative);

            UriMapping mapping = new UriMapping()
            {
                Uri = new Uri("/{pageName}/{fragment}", UriKind.Relative),
                MappedUri = new Uri(TestPagesPath + "{pageName}.xaml#{fragment}", UriKind.Relative)
            };

            UriMapper mapper = new UriMapper();
            mapper.UriMappings.Add(mapping);

            this.Frame.UriMapper = mapper;

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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
        [Description("Navigate to Page1, then to Page2, verify that the events are raised in the correct order")]
        public void NavigateToPageThenAnotherPageEventSequence()
        {
            bool complete = false;
            int eventCount = 0;
            Page1 page1Content = null;
            Page2 page2Content = null;

            Uri page1Uri = new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative);
            Uri page2Uri = new Uri(TestPagesPath + "Page2.xaml", UriKind.Relative);

            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

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

        #region AutomationPeer tests

        [TestMethod]
        [Description("Verifies that the Frame Automation Peer is created correctly.")]
        public void FrameAutomationPeerTest()
        {
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            FrameAutomationPeer peer = FrameAutomationPeer.CreatePeerForElement(this.Frame) as FrameAutomationPeer;
            Assert.IsNotNull(peer);
            Assert.AreEqual(AutomationControlType.Pane, peer.GetAutomationControlType());
            Assert.AreEqual("Frame", peer.GetClassName());
            Assert.IsFalse(peer.HasKeyboardFocus());
            Assert.IsTrue(peer.IsContentElement());
            Assert.IsTrue(peer.IsControlElement());
            Assert.IsFalse(peer.IsKeyboardFocusable());
        }

        [TestMethod]
        [Description("Verifies that the Frame Automation Peer returns a name correctly.")]
        public void FrameAutomationPeerGetName()
        {
            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);

            FrameAutomationPeer peer = FrameAutomationPeer.CreatePeerForElement(this.Frame) as FrameAutomationPeer;
            Assert.IsNotNull(peer);

            string labeledByText = "Frame Labeled By This Control";
            TextBlock tbLabel = new TextBlock();
            tbLabel.Text = labeledByText;
            this.TestPanel.Children.Add(tbLabel);

            // Defaults to the class name
            Assert.AreEqual("Frame", peer.GetName());

            // If it's named, it uses that
            string frameName = "Frame's Name";
            this.Frame.Name = frameName;
            Assert.AreEqual(frameName, peer.GetName());

            // If it's labeled, it uses that
            AutomationProperties.SetLabeledBy(this.Frame, tbLabel);
            Assert.AreEqual(labeledByText, peer.GetName());
        }

        #endregion AutomationPeer tests

        #endregion
    }
}
