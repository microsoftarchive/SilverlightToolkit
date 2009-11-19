//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.UnitTests
{
    [TestClass]
    public class FrameNavigationCacheModeRequiredTests : SilverlightTest
    {

        #region Static fields and constants

        private static readonly string TestPagesPath = @"/System.Windows.Controls.Navigation/Controls/TestPages/";

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
            }

            HtmlPage.Window.CurrentBookmark = String.Empty;

            // Create a new navigator
            this.Frame = new Frame();
            this.Frame.CacheSize = 0; // All these tests are independent of a cache size, so they should all work with CacheSize=0
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;

            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
        }

        #endregion

        #region Test Methods

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeRequired, then GoBack, then GoForward - verify that the page was reused, and that its state wasn't modified.  This test uses a browser-integrated journal.")]
        public void ToNavigationCacheModeRequiredGoBackGoForwardStateSameWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            ToNavigationCacheModeRequiredGoBackGoForwardStateSameCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeRequired, then GoBack, then GoForward - verify that the page was reused, and that its state wasn't modified.")]
        public void ToNavigationCacheModeRequiredGoBackGoForwardStateSame()
        {
            ToNavigationCacheModeRequiredGoBackGoForwardStateSameCore();
        }

        private void ToNavigationCacheModeRequiredGoBackGoForwardStateSameCore()
        {
            string stateSetInNavigationCacheModeRequired = "New state from ToNavigationCacheModeRequiredGoBackGoForwardStateSame";
            int complete = 0;

            // Wire up events
            Frame.NavigationFailed +=
                (sender, args) =>
                {
                    // Failure
                    Assert.Fail(args.Exception.Message);
                };
            Frame.Navigated +=
                (sender, args) =>
                {
                    if (complete == 3)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeRequiredPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeRequiredPage);

                        // Verify that the content in it is still what it was before the GoBack/GoForward
                        // This ensures it must have been reused
                        Assert.AreEqual(stateSetInNavigationCacheModeRequired, (Frame.Content as NavigationCacheModeRequiredPage).GetTextBoxText(), "ToNavigationCacheModeRequiredGoBackGoForwardStateSameCore TextBox Contents");
                    }
                    complete++;
                };

            // First navigation, to any page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, to the NavigationCacheModeRequired page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeRequiredPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 2);

            // Change some state on the page that has NavigationCacheModeRequired set to true
            this.EnqueueCallback(() => (Frame.Content as NavigationCacheModeRequiredPage).SetTextBoxText(stateSetInNavigationCacheModeRequired));

            // Third navigation, go back (the NavigationCacheModeRequired page should still be reused at this point), wait for completion.
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 3);

            // Fourth navigation, go forward (back to the NavigationCacheModeRequired page)
            this.EnqueueCallback(() => Frame.GoForward());

            this.EnqueueConditional(() => complete == 4);
            this.EnqueueTestComplete();
        }
        
        [TestMethod]
        [Asynchronous]
        [Description("Start on a NavigationCacheModeRequired page, navigate elsewhere, navigate again to the same NavigationCacheModeRequired page (by same Uri), and verify that the page was reused, and that its state wasn't modified.  This test uses a browser-integrated journal.")]
        public void FromNavigationCacheModeRequiredToOtherNavigateBackStateSameWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            FromNavigationCacheModeRequiredToOtherNavigateBackStateSameCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Start on a NavigationCacheModeRequired page, navigate elsewhere, navigate again to the same NavigationCacheModeRequired page (by same Uri), and verify that the page was reused, and that its state wasn't modified")]
        public void FromNavigationCacheModeRequiredToOtherNavigateBackStateSame()
        {
            FromNavigationCacheModeRequiredToOtherNavigateBackStateSameCore();
        }

        private void FromNavigationCacheModeRequiredToOtherNavigateBackStateSameCore()
        {
            string stateSetInNavigationCacheModeRequired = "New state from FromNavigationCacheModeRequiredToOtherNavigateBackStateSame";
            int complete = 0;

            // Wire up events
            Frame.NavigationFailed +=
                (sender, args) =>
                {
                    // Failure
                    Assert.Fail(args.Exception.Message);
                };
            Frame.Navigated +=
                (sender, args) =>
                {
                    if (complete == 2)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeRequiredPage, "Content is not of the correct type in the event arguments");

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeRequiredPage, "Content is not of the correct type in Frame.Content");

                        // Verify that the content in it is still what it was before the GoBack/GoForward
                        // This ensures it must have been reused
                        Assert.AreEqual(stateSetInNavigationCacheModeRequired, (Frame.Content as NavigationCacheModeRequiredPage).GetTextBoxText(), "FromNavigationCacheModeRequiredToOtherNavigateBackStateSameCore TextBox Contents");
                    }
                    complete++;
                };

            // First navigation, to a NavigationCacheModeRequired page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeRequiredPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 1);

            // Change some state on the page that has NavigationCacheModeRequired set to true
            this.EnqueueCallback(() => (Frame.Content as NavigationCacheModeRequiredPage).SetTextBoxText(stateSetInNavigationCacheModeRequired));

            // Second navigation, to another page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 2);

            // Third navigation, navigate back to the same NavigationCacheModeRequired page, by Uri, wait for completion
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeRequiredPage.xaml", UriKind.Relative)));

            this.EnqueueConditional(() => complete == 3);
            this.EnqueueTestComplete();
        }
                
        [TestMethod]
        [Asynchronous]
        [Description("Start on a NavigationCacheModeDisabled page, change its state, navigate to a NavigationCacheModeRequired page, navigate back to the same Uri as the starting page, verify it was not reused and its state is back to original.  This test uses a browser-integrated journal.")]
        public void FromNavigationCacheModeDisabledToNavigationCacheModeRequiredAndBackNotReusedWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            FromNavigationCacheModeDisabledToNavigationCacheModeRequiredAndBackNotReusedCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Start on a NavigationCacheModeDisabled page, change its state, navigate to a NavigationCacheModeRequired page, navigate back to the same Uri as the starting page, verify it was not reused and its state is back to original.")]
        public void FromNavigationCacheModeDisabledToNavigationCacheModeRequiredAndBackNotReused()
        {
            FromNavigationCacheModeDisabledToNavigationCacheModeRequiredAndBackNotReusedCore();
        }

        private void FromNavigationCacheModeDisabledToNavigationCacheModeRequiredAndBackNotReusedCore()
        {
            string originalStateInNavigationCacheModeDisabled = String.Empty;
            string stateSetInNavigationCacheModeDisabled = "New state from FromNavigationCacheModeDisabledToNavigationCacheModeRequiredAndBackNotReused";
            int complete = 0;

            // Wire up events
            Frame.NavigationFailed +=
                (sender, args) =>
                {
                    // Failure
                    Assert.Fail(args.Exception.Message + "\ncomplete=" + complete);
                };
            Frame.Navigated +=
                (sender, args) =>
                {
                    if (complete == 0)
                    {
                        originalStateInNavigationCacheModeDisabled = (Frame.Content as NavigationCacheModeDisabledPage).GetTextBoxText();
                    }
                    if (complete == 2)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeDisabledPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeDisabledPage);

                        // Verify that the content in it is back to its original value.
                        // This ensures it must NOT have been reused
                        Assert.AreEqual(originalStateInNavigationCacheModeDisabled, (Frame.Content as NavigationCacheModeDisabledPage).GetTextBoxText(), "FromNavigationCacheModeDisabledToNavigationCacheModeRequiredAndBackNotReusedCore TextBox Contents");
                    }

                    complete++;
                };

            // First navigation, to a NavigationCacheModeDisabled page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeDisabledPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 1);

            // Change some state on the NavigationCacheModeDisabled page (this should get thrown away when the page is navigated away from)
            this.EnqueueCallback(() => (Frame.Content as NavigationCacheModeDisabledPage).SetTextBoxText(stateSetInNavigationCacheModeDisabled));

            // Second navigation, to the NavigationCacheModeRequired page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeRequiredPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 2);

            // Third navigation, go to the NavigationCacheModeDisabled page again, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeDisabledPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 3);

            this.EnqueueTestComplete();
        }
        
        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeRequired, navigate away, come back, verify state is the same, change it to NavigationCacheModeDisabled, navigate away, come back, verify it is not reused anymore.  This test uses a browser-integrated journal.")]
        public void ChangeNavigationCacheModeRequiredDuringLifetimeVerifyReusedOrNotAccuratelyWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            ChangeNavigationCacheModeRequiredDuringLifetimeVerifyReusedOrNotAccuratelyCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeRequired, navigate away, come back, verify state is the same, change it to NavigationCacheModeDisabled, navigate away, come back, verify it is not reused anymore.")]
        public void ChangeNavigationCacheModeRequiredDuringLifetimeVerifyReusedOrNotAccurately()
        {
            ChangeNavigationCacheModeRequiredDuringLifetimeVerifyReusedOrNotAccuratelyCore();
        }

        private void ChangeNavigationCacheModeRequiredDuringLifetimeVerifyReusedOrNotAccuratelyCore()
        {
            string originalStateInNavigationCacheModeRequired = String.Empty;
            string stateSetInNavigationCacheModeRequired = "New state from ChangeNavigationCacheModeRequiredDuringLifetimeVerifyReusedOrNotAccurately";
            NavigationCacheModeChangesPage instanceReused = null;
            int complete = 0;

            // Wire up events
            Frame.NavigationFailed +=
                (sender, args) =>
                {
                    // Failure
                    Assert.Fail(args.Exception.Message);
                };
            Frame.Navigated +=
                (sender, args) =>
                {
                    if (complete == 1)
                    {
                        originalStateInNavigationCacheModeRequired = (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText();
                    }
                    if (complete == 3)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeChangesPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeChangesPage);

                        // Verify that content is still what we set it to, and that NavigationCacheMode == Required
                        Assert.AreEqual(stateSetInNavigationCacheModeRequired, (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText(), "ChangeNavigationCacheModeRequiredDuringLifetimeVerifyReusedOrNotAccuratelyCore TextBox Contents this.NavigationOperationCount==4");
                        Assert.AreEqual(NavigationCacheMode.Required, (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode);

                        instanceReused = Frame.Content as NavigationCacheModeChangesPage;
                    }
                    if (complete == 5)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeChangesPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeChangesPage);

                        // Verify that content is back to original (because it was not reused)
                        // and that NavigationCacheMode == Required, and that this is not the same object
                        // instance as when it was reused
                        Assert.AreEqual(originalStateInNavigationCacheModeRequired, (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText(), "ChangeNavigationCacheModeRequiredDuringLifetimeVerifyReusedOrNotAccuratelyCore TextBox Contents this.NavigationOperationCount==6");
                        Assert.AreEqual(NavigationCacheMode.Disabled, (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode);
                        Assert.IsFalse(((object)instanceReused).Equals(Frame.Content));
                    }
                    complete++;
                };
                        
            // First navigation, to any page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, to the NavigationCacheModeChanges page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeChangesPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 2);

            // Change some state on the page and set it's NavigationCacheMode to Required
            this.EnqueueCallback(() =>
            {
                (Frame.Content as NavigationCacheModeChangesPage).SetTextBoxText(stateSetInNavigationCacheModeRequired);
                (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode = NavigationCacheMode.Required;
            });

            // Third navigation, go back (the NavigationCacheModeChanges page should still be reused at this point), wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 3);

            // Fourth navigation, go back (to the NavigationCacheModeChanges page, which is set to NavigationCacheMode == Required currently)
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 4);

            // Change the NavigationCacheMode status of the page to Disabled
            this.EnqueueCallback(() => (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode = NavigationCacheMode.Disabled);

            // Fifth navigation, go forward to the NavigationCacheModeDisabled page
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => complete == 5);

            // Sixth navigation, go back (to the NavigationCacheModeChanges page, which is NOT set to NavigationCacheMode == Required currently)
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 6);

            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a NavigationCacheModeRequired page, navigate somewhere else, then set the browser's address bar directly back to the Uri for the NavigationCacheModeRequired page, verify that it was reused.  This test uses a browser-integrated journal.")]
        public void BrowserAddressBarToNavigationCacheModeRequiredIsReused()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            int complete = 0;

            string stateSetInNavigationCacheModeRequired = "New state from BrowserAddressBarToNavigationCacheModeRequiredIsReused";
            string keepAliveFragment = string.Empty;

            // Wire up events
            Frame.NavigationFailed +=
                (sender, args) =>
                {
                    // Failure
                    Assert.Fail(args.Exception.Message);
                };
            Frame.Navigated +=
                (sender, args) =>
                {
                    if (complete == 0)
                    {
                        // This doesn't work in all browsers.  See below.
                        //keepAliveFragment = HtmlPage.Document.DocumentUri.Fragment;

                        // Resorting to an evaluation of window.location because 
                        // HtmlPage.Document.DocumentUri isn't updated properly in
                        // all browsers.
                        keepAliveFragment = HtmlPage.Window.Eval("window.location.href") as string;

                        int anchorPos = keepAliveFragment.IndexOf('#');
                        if (anchorPos > 0)
                        {
                            keepAliveFragment = keepAliveFragment.Substring(anchorPos + 1);
                        }
                    }
                    else if (complete == 1)
                    {
                        // This causes the third Navigated event to fire because it updates the browser,
                        // which causes the Journal to get an event and fire its Journal.Navigated event,
                        // which then in turn causes the Frame to raise a Navigated event.
                        HtmlPage.Window.CurrentBookmark = keepAliveFragment;
                    }
                    else if (complete == 2)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeRequiredPage, "Content is not of the expected type in event arguments");

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeRequiredPage, "Content is not of the expected type in Frame.Content");

                        // Verify that the content in it is still what it was before the GoBack/GoForward
                        // This ensures it must have been reused
                        Assert.AreEqual(stateSetInNavigationCacheModeRequired, (Frame.Content as NavigationCacheModeRequiredPage).GetTextBoxText(), "BrowserAddressBarToNavigationCacheModeRequiredIsReused AreEqual TextBox Contents");
                    }

                    complete++;
                };

            // First navigation, to a NavigationCacheModeRequired page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeRequiredPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 1);

            // Change some state on the page that has NavigationCacheMode == Required
            this.EnqueueCallback(() => (Frame.Content as NavigationCacheModeRequiredPage).SetTextBoxText(stateSetInNavigationCacheModeRequired));

            // Second navigation, to a NavigationCacheModeDisabled page
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeDisabledPage.xaml", UriKind.Relative)));

            this.EnqueueConditional(() => complete == 3);
            this.EnqueueTestComplete();
        }

        #endregion

    }
}
