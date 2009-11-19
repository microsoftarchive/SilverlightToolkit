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
    public class FrameNavigationCacheModeEnabledTests : SilverlightTest
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
            this.Frame.JournalOwnership = JournalOwnership.OwnsJournal;

            // Add to test surface
            this.TestPanel.Children.Add(this.Frame);
        }

        #endregion

        #region Test Methods

        #region CacheSize < 0

        [TestMethod]
        [Description("Attempt to set CacheSize less than 0, verify that it throws")]
        public void CacheSizeLessThanZeroThrows()
        {
            try
            {
                this.Frame.CacheSize = -1;
                Assert.Fail("This test should throw an exception, but did not.");
            }
            catch (InvalidOperationException ex)
            {
                //Prevents build warning.
                Assert.IsNotNull(ex);
            }
        }

        #endregion

        #region CacheSize = 0

        [TestMethod]
        [Asynchronous]
        [Description("When CacheSize=0, verify that going to a NavigationCacheMode=Enabled page is the same as NavigationCacheMode=Disabled.    This test uses a browser-integrated journal.")]
        public void CacheSizeZeroWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            CacheSizeZeroCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("When CacheSize=0, verify that going to a NavigationCacheMode=Enabled page is the same as NavigationCacheMode=Disabled")]
        public void CacheSizeZero()
        {
            CacheSizeZeroCore();
        }

        private void CacheSizeZeroCore()
        {
            this.Frame.CacheSize = 0;

            string originalStateInNavigationCacheModeEnabled = String.Empty;
            string stateSetInNavigationCacheModeEnabled = "New state from CacheSizeZeroNavigationCacheModeEnabled";
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
                    if (complete == 0)
                    {
                        originalStateInNavigationCacheModeEnabled = (Frame.Content as NavigationCacheModeEnabledPage).GetTextBoxText();
                    }
                    if (complete == 2)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeEnabledPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeEnabledPage);

                        // Verify that the content in it is back to its original value.
                        // This ensures it must NOT have been reused
                        Assert.AreEqual(originalStateInNavigationCacheModeEnabled, (Frame.Content as NavigationCacheModeEnabledPage).GetTextBoxText(), "CacheSizeZeroNavigationCacheModeEnabled TextBox Contents");
                    }

                    complete++;
                };

            // First navigation, to a NavigationCacheModeEnabled page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeEnabledPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 1);

            // Change some state on the NavigationCacheModeEnabled page (this should get thrown away when the page is navigated away from)
            this.EnqueueCallback(() => (Frame.Content as NavigationCacheModeEnabledPage).SetTextBoxText(stateSetInNavigationCacheModeEnabled));

            // Second navigation, to the NavigationCacheModeRequired page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeRequiredPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 2);

            // Third navigation, go to the NavigationCacheModeEnabled page again, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeEnabledPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 3);

            this.EnqueueTestComplete();
        }

        #endregion

        #region CacheSize > 0

        [TestMethod]
        [Asynchronous]
        [Description("When CacheSize>0, verify that cached pages are reused.  This test uses a browser-integrated journal.")]
        public void CacheSizeNonzeroWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            CacheSizeNonzeroCore();
        }
        
        [TestMethod]
        [Asynchronous]
        [Description("When CacheSize>0, verify that cached pages are reused.")]
        public void CacheSizeNonzero()
        {
            CacheSizeNonzeroCore();
        }

        private void CacheSizeNonzeroCore()
        {
            this.Frame.CacheSize = 5;

            string stateSetInNavigationCacheModeEnabled = "New state from CacheSizeNonzero";
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
                        Assert.IsTrue(args.Content is NavigationCacheModeEnabledPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeEnabledPage);

                        // Verify that the content in it is still what it was before the GoBack/GoForward
                        // This ensures it must have been reused
                        Assert.AreEqual(stateSetInNavigationCacheModeEnabled, (Frame.Content as NavigationCacheModeEnabledPage).GetTextBoxText(), "CacheSizeNonzero TextBox Contents");

                        Assert.AreEqual(1, this.Frame.NavigationService.Cache.CacheMRUPagesSize);
                        Assert.AreEqual(1, this.Frame.NavigationService.Cache.CachePagesSize);
                    }

                    complete++;
                };

            // First navigation, to a NavigationCacheModeEnabled page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeEnabledPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 1);

            // Change some state on the page that has NavigationCacheMode == Enabled
            this.EnqueueCallback(() => (Frame.Content as NavigationCacheModeEnabledPage).SetTextBoxText(stateSetInNavigationCacheModeEnabled));

            // Second navigation, to another page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 2);

            // Third navigation, navigate back to the same NavigationCacheModeEnabled page, by Uri, wait for completion
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeEnabledPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 3);

            this.EnqueueTestComplete();
        }

        #endregion

        #region Cache throws away oldest first

        [TestMethod]
        [Asynchronous]
        [Description("The cache throws away the oldest entry first.  This test uses a browser-integrated journal.")]
        public void OldestThrownAwayFirstWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            OldestThrownAwayFirstCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("The cache throws away the oldest entry first")]
        public void OldestThrownAwayFirst()
        {
            OldestThrownAwayFirstCore();
        }

        private void OldestThrownAwayFirstCore()
        {
            this.Frame.CacheSize = 3;

            string stateSetInNavigationCacheModeEnabled = "New state from OldestThrownAwayFirst";
            int complete = 1;

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
                    Assert.IsTrue(args.Content is NavigationCacheModeEnabledPage);
                    Assert.IsTrue(Frame.Content is NavigationCacheModeEnabledPage);

                    if (complete < 6)
                    {
                        Assert.AreEqual(complete.ToString(), (args.Content as Page).NavigationContext.QueryString["id"]);
                    }
                    if (complete == 6)
                    {
                        // Verify that the content in it is reset back to its original state (because the cached instance could
                        // not be reused).
                        Assert.AreNotEqual(stateSetInNavigationCacheModeEnabled, (Frame.Content as NavigationCacheModeEnabledPage).GetTextBoxText(), "OldestThrownAwayFirst TextBox Contents");

                        // Verify that the cache is still full
                        Assert.AreEqual(3, this.Frame.NavigationService.Cache.CacheMRUPagesSize);
                        Assert.AreEqual(3, this.Frame.NavigationService.Cache.CachePagesSize);
                    }

                    complete++;
                };

            // First navigation, to a NavigationCacheModeEnabled page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeEnabledPage.xaml?id=1", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 2);

            // Change some state on the page that has NavigationCacheMode == Enabled
            this.EnqueueCallback(() => (Frame.Content as NavigationCacheModeEnabledPage).SetTextBoxText(stateSetInNavigationCacheModeEnabled));

            // Navigate to 4 other pages (?id=2, ?id=3, ?id=4 and ?id=5) so that the cache fills up and discards ?id=1
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeEnabledPage.xaml?id=2", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 3);
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeEnabledPage.xaml?id=3", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 4);
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeEnabledPage.xaml?id=4", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 5);
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeEnabledPage.xaml?id=5", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 6);

            // Navigate back to ?id=1, but since the cache was filled up, the old instance should have been discarded, so this
            // will create a new instance.  Wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeEnabledPage.xaml?id=1", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 7);

            this.EnqueueTestComplete();
        }

        #endregion

        #region NavigationCacheMode Disabled -> Enabled

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeDisabled, navigate away, come back, verify it was not reused, change it to NavigationCacheModeEnabled, navigate away, come back, verify it is reused.  This test uses a browser-integrated journal.")]
        public void ChangeNavigationCacheModeDisabledToEnabledDuringLifetimeVerifyReusedWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            ChangeNavigationCacheModeDisabledToEnabledDuringLifetimeVerifyReusedCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeDisabled, navigate away, come back, verify it was not reused, change it to NavigationCacheModeEnabled, navigate away, come back, verify it is reused.")]
        public void ChangeNavigationCacheModeDisabledToEnabledDuringLifetimeVerifyReused()
        {
            ChangeNavigationCacheModeDisabledToEnabledDuringLifetimeVerifyReusedCore();
        }

        private void ChangeNavigationCacheModeDisabledToEnabledDuringLifetimeVerifyReusedCore()
        {
            string originalStateInNavigationCacheModeChanges = String.Empty;
            string stateSetInNavigationCacheModeChanges = "New state from ChangeNavigationCacheModeDisabledToEnabledDuringLifetimeVerifyReused";
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
                        originalStateInNavigationCacheModeChanges = (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText();
                    }
                    if (complete == 3)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeChangesPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeChangesPage);

                        // Verify that content is still what we set it to, and that NavigationCacheMode == Disabled
                        Assert.AreEqual(originalStateInNavigationCacheModeChanges, (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText(), "ChangeNavigationCacheModeDisabledToEnabledDuringLifetimeVerifyReusedCore TextBox Contents this.NavigationOperationCount==4");
                        Assert.AreEqual(NavigationCacheMode.Disabled, (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode);

                        Assert.AreEqual(0, Frame.NavigationService.Cache.CachePagesSize);
                        Assert.AreEqual(0, Frame.NavigationService.Cache.CacheMRUPagesSize);
                    }
                    if (complete == 5)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeChangesPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeChangesPage);

                        // Verify that content is what we set it to (because it was reused)
                        // and that NavigationCacheMode == Enabled
                        Assert.AreEqual(stateSetInNavigationCacheModeChanges, (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText(), "ChangeNavigationCacheModeDisabledToEnabledDuringLifetimeVerifyReusedCore TextBox Contents this.NavigationOperationCount==6");
                        Assert.AreEqual(NavigationCacheMode.Enabled, (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode);

                        Assert.AreEqual(1, Frame.NavigationService.Cache.CachePagesSize);
                        Assert.AreEqual(1, Frame.NavigationService.Cache.CacheMRUPagesSize);
                    }

                    complete++;
                };

            // First navigation, to any page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, to the NavigationCacheModeChanges page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeChangesPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 2);

            // Change some state on the page but leave its NavigationCacheMode set to Disabled
            this.EnqueueCallback(() => (Frame.Content as NavigationCacheModeChangesPage).SetTextBoxText(stateSetInNavigationCacheModeChanges));

            // Third navigation, go back (the NavigationCacheModeChanges page should still be reused at this point), wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 3);

            // Fourth navigation, go back (to the NavigationCacheModeChanges page, which is set to NavigationCacheMode == Disabled currently)
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 4);

            // Change some state on the page and set it's NavigationCacheMode to Enabled
            this.EnqueueCallback(() =>
            {
                (Frame.Content as NavigationCacheModeChangesPage).SetTextBoxText(stateSetInNavigationCacheModeChanges);
                (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode = NavigationCacheMode.Enabled;
            });

            // Fifth navigation, go forward to Page1
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => complete == 5);

            // Sixth navigation, go back (to the NavigationCacheModeChanges page, which is now set to NavigationCacheMode == Enabled)
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 6);

            this.EnqueueTestComplete();
        }

        #endregion

        #region NavigationCacheMode Enabled -> Disabled

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeEnabled, navigate away, come back, verify it was reused, change it to NavigationCacheModeDisabled, navigate away, come back, verify it is not reused and that the cache is empty.  This test uses a browser-integrated journal.")]
        public void ChangeNavigationCacheModeEnabledToNeverDuringLifetimeVerifyReusedWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            ChangeNavigationCacheModeEnabledToNeverDuringLifetimeVerifyReusedCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeEnabled, navigate away, come back, verify it was reused, change it to NavigationCacheModeDisabled, navigate away, come back, verify it is not reused and that the cache is empty.")]
        public void ChangeNavigationCacheModeEnabledToNeverDuringLifetimeVerifyReused()
        {
            ChangeNavigationCacheModeEnabledToNeverDuringLifetimeVerifyReusedCore();
        }

        private void ChangeNavigationCacheModeEnabledToNeverDuringLifetimeVerifyReusedCore()
        {
            string originalStateInNavigationCacheModeChanges = String.Empty;
            string stateSetInNavigationCacheModeChanges = "New state from ChangeNavigationCacheModeEnabledToNeverDuringLifetimeVerifyReused";
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
                        originalStateInNavigationCacheModeChanges = (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText();
                    }
                    if (complete == 3)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeChangesPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeChangesPage);

                        // Verify that content is still what we set it to, and that NavigationCacheMode == Enabled
                        Assert.AreEqual(stateSetInNavigationCacheModeChanges, (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText(), "ChangeNavigationCacheModeEnabledToNeverDuringLifetimeVerifyReusedCore TextBox Contents this.NavigationOperationCount==4");
                        Assert.AreEqual(NavigationCacheMode.Enabled, (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode);

                        Assert.AreEqual(1, Frame.NavigationService.Cache.CachePagesSize);
                        Assert.AreEqual(1, Frame.NavigationService.Cache.CacheMRUPagesSize);
                    }
                    if (complete == 4)
                    {
                        // Verify that the cache is now empty, because we changed the only page in it to
                        // NavigationCacheMode == Disabled
                        Assert.AreEqual(0, Frame.NavigationService.Cache.CachePagesSize);
                        Assert.AreEqual(0, Frame.NavigationService.Cache.CacheMRUPagesSize);
                    }
                    if (complete == 5)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeChangesPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeChangesPage);

                        // Verify that content is the original (because it was not reused)
                        // and that NavigationCacheMode == Disabled
                        Assert.AreEqual(originalStateInNavigationCacheModeChanges, (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText(), "ChangeNavigationCacheModeEnabledToNeverDuringLifetimeVerifyReusedCore TextBox Contents this.NavigationOperationCount==6");
                        Assert.AreEqual(NavigationCacheMode.Disabled, (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode);

                        Assert.AreEqual(0, Frame.NavigationService.Cache.CachePagesSize);
                        Assert.AreEqual(0, Frame.NavigationService.Cache.CacheMRUPagesSize);
                    }

                    complete++;
                };

            // First navigation, to any page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, to the NavigationCacheModeChanges page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeChangesPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 2);

            // Change some state on the page and set its NavigationCacheMode set to Enabled
            this.EnqueueCallback(() =>
             {
                 (Frame.Content as NavigationCacheModeChangesPage).SetTextBoxText(stateSetInNavigationCacheModeChanges);
                 (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode = NavigationCacheMode.Enabled;
             });

            // Third navigation, go back (the NavigationCacheModeChanges page should still be reused at this point), wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 3);

            // Fourth navigation, go back (to the NavigationCacheModeChanges page, which is set to NavigationCacheMode == Enabled currently)
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 4);

            // Change some state on the page and set it's NavigationCacheMode to Disabled
            this.EnqueueCallback(() =>
            {
                (Frame.Content as NavigationCacheModeChangesPage).SetTextBoxText(stateSetInNavigationCacheModeChanges);
                (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode = NavigationCacheMode.Disabled;
            });

            // Fifth navigation, go forward to Page1
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => complete == 5);

            // Sixth navigation, go back (to the NavigationCacheModeChanges page, which is now set to NavigationCacheMode == Disabled)
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 6);

            this.EnqueueTestComplete();
        }

        #endregion

        #region NavigationCacheMode Enabled -> Required

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeEnabled, navigate away, come back, verify it was reused, change it to NavigationCacheModeRequired, navigate away, come back, verify it is still reused and that the cache is empty.  This test uses a browser-integrated journal.")]
        public void ChangeNavigationCacheModeEnabledToRequiredDuringLifetimeVerifyReusedWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            ChangeNavigationCacheModeEnabledToRequiredDuringLifetimeVerifyReusedCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeEnabled, navigate away, come back, verify it was reused, change it to NavigationCacheModeRequired, navigate away, come back, verify it is still reused and that the cache is empty.")]
        public void ChangeNavigationCacheModeEnabledToRequiredDuringLifetimeVerifyReused()
        {
            ChangeNavigationCacheModeEnabledToRequiredDuringLifetimeVerifyReusedCore();
        }

        private void ChangeNavigationCacheModeEnabledToRequiredDuringLifetimeVerifyReusedCore()
        {
            string originalStateInNavigationCacheModeChanges = String.Empty;
            string stateSetInNavigationCacheModeChanges = "New state from ChangeNavigationCacheModeEnabledToRequiredDuringLifetimeVerifyReused";
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
                        originalStateInNavigationCacheModeChanges = (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText();
                    }
                    if (complete == 3)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeChangesPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeChangesPage);

                        // Verify that content is still what we set it to, and that NavigationCacheMode == Enabled
                        Assert.AreEqual(stateSetInNavigationCacheModeChanges, (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText(), "ChangeNavigationCacheModeEnabledToRequiredDuringLifetimeVerifyReusedCore TextBox Contents this.NavigationOperationCount==4");
                        Assert.AreEqual(NavigationCacheMode.Enabled, (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode);

                        Assert.AreEqual(1, Frame.NavigationService.Cache.CachePagesSize);
                        Assert.AreEqual(1, Frame.NavigationService.Cache.CacheMRUPagesSize);
                    }
                    if (complete == 4)
                    {
                        // Verify that the cache is now empty, because we changed the only page in it to
                        // NavigationCacheMode == Required, which doesn't take up cache space
                        Assert.AreEqual(0, Frame.NavigationService.Cache.CachePagesSize);
                        Assert.AreEqual(0, Frame.NavigationService.Cache.CacheMRUPagesSize);
                    }
                    if (complete == 5)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeChangesPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeChangesPage);

                        // Verify that content is what we set it to (because it was reused)
                        // and that NavigationCacheMode == Required
                        Assert.AreEqual(stateSetInNavigationCacheModeChanges, (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText(), "ChangeNavigationCacheModeEnabledToRequiredDuringLifetimeVerifyReusedCore TextBox Contents this.NavigationOperationCount==6");
                        Assert.AreEqual(NavigationCacheMode.Required, (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode);

                        Assert.AreEqual(0, Frame.NavigationService.Cache.CachePagesSize);
                        Assert.AreEqual(0, Frame.NavigationService.Cache.CacheMRUPagesSize);
                    }

                    complete++;
                };

            // First navigation, to any page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, to the NavigationCacheModeChanges page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeChangesPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 2);

            // Change some state on the page and set its NavigationCacheMode set to Enabled
            this.EnqueueCallback(() =>
            {
                (Frame.Content as NavigationCacheModeChangesPage).SetTextBoxText(stateSetInNavigationCacheModeChanges);
                (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode = NavigationCacheMode.Enabled;
            });

            // Third navigation, go back (the NavigationCacheModeChanges page should still be reused at this point), wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 3);

            // Fourth navigation, go back (to the NavigationCacheModeChanges page, which is set to NavigationCacheMode == Enabled currently)
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 4);

            // Change some state on the page and set it's NavigationCacheMode to Required
            this.EnqueueCallback(() =>
            {
                (Frame.Content as NavigationCacheModeChangesPage).SetTextBoxText(stateSetInNavigationCacheModeChanges);
                (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode = NavigationCacheMode.Required;
            });

            // Fifth navigation, go forward to Page1
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => complete == 5);

            // Sixth navigation, go back (to the NavigationCacheModeChanges page, which is now set to NavigationCacheMode == Required)
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 6);

            this.EnqueueTestComplete();
        }

        #endregion

        #region NavigationCacheMode Required -> Enabled

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeRequired, navigate away, come back, verify it was reused, change it to NavigationCacheModeEnabled, navigate away, come back, verify it is still reused.  This test uses a browser-integrated journal.")]
        public void ChangeNavigationCacheModeRequiredToEnabledDuringLifetimeVerifyReusedWithBrowserIntegration()
        {
            this.Frame.JournalOwnership = JournalOwnership.UsesParentJournal;
            ChangeNavigationCacheModeRequiredToEnabledDuringLifetimeVerifyReusedCore();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Start somewhere, navigate to a page which is NavigationCacheModeRequired, navigate away, come back, verify it was reused, change it to NavigationCacheModeEnabled, navigate away, come back, verify it is still reused.")]
        public void ChangeNavigationCacheModeRequiredToEnabledDuringLifetimeVerifyReused()
        {
            ChangeNavigationCacheModeRequiredToEnabledDuringLifetimeVerifyReusedCore();
        }

        private void ChangeNavigationCacheModeRequiredToEnabledDuringLifetimeVerifyReusedCore()
        {
            string originalStateInNavigationCacheModeChanges = String.Empty;
            string stateSetInNavigationCacheModeChanges = "New state from ChangeNavigationCacheModeRequiredToEnabledDuringLifetimeVerifyReused";
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
                        originalStateInNavigationCacheModeChanges = (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText();
                    }
                    if (complete == 3)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeChangesPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeChangesPage);

                        // Verify that content is still what we set it to, and that NavigationCacheMode == Required
                        Assert.AreEqual(stateSetInNavigationCacheModeChanges, (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText(), "ChangeNavigationCacheModeRequiredToEnabledDuringLifetimeVerifyReusedCore TextBox Contents this.NavigationOperationCount==4");
                        Assert.AreEqual(NavigationCacheMode.Required, (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode);

                        Assert.AreEqual(0, Frame.NavigationService.Cache.CachePagesSize);
                        Assert.AreEqual(0, Frame.NavigationService.Cache.CacheMRUPagesSize);
                    }
                    if (complete == 4)
                    {
                        // Verify that the cache now has an entry, because we changed the page to
                        // NavigationCacheMode == Enabled
                        Assert.AreEqual(1, Frame.NavigationService.Cache.CachePagesSize);
                        Assert.AreEqual(1, Frame.NavigationService.Cache.CacheMRUPagesSize);
                    }
                    if (complete == 5)
                    {
                        // Verify contents of event args.
                        Assert.IsTrue(args.Content is NavigationCacheModeChangesPage);

                        // Verify contents in navigator content presenter.
                        Assert.IsTrue(Frame.Content is NavigationCacheModeChangesPage);

                        // Verify that content is what we set it to (because it was reused)
                        // and that NavigationCacheMode == Required
                        Assert.AreEqual(stateSetInNavigationCacheModeChanges, (Frame.Content as NavigationCacheModeChangesPage).GetTextBoxText(), "ChangeNavigationCacheModeRequiredToEnabledDuringLifetimeVerifyReusedCore TextBox Contents this.NavigationOperationCount==6");
                        Assert.AreEqual(NavigationCacheMode.Enabled, (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode);

                        Assert.AreEqual(1, Frame.NavigationService.Cache.CachePagesSize);
                        Assert.AreEqual(1, Frame.NavigationService.Cache.CacheMRUPagesSize);
                    }

                    complete++;
                };

            // First navigation, to any page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 1);

            // Second navigation, to the NavigationCacheModeChanges page, wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "NavigationCacheModeChangesPage.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 2);

            // Change some state on the page and set its NavigationCacheMode set to Required
            this.EnqueueCallback(() =>
            {
                (Frame.Content as NavigationCacheModeChangesPage).SetTextBoxText(stateSetInNavigationCacheModeChanges);
                (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode = NavigationCacheMode.Required;
            });

            // Third navigation, go back (the NavigationCacheModeChanges page should still be reused at this point), wait for completion.
            this.EnqueueCallback(() => Frame.Navigate(new Uri(TestPagesPath + "Page1.xaml", UriKind.Relative)));
            this.EnqueueConditional(() => complete == 3);

            // Fourth navigation, go back (to the NavigationCacheModeChanges page, which is set to NavigationCacheMode == Required currently)
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 4);

            // Change some state on the page and set it's NavigationCacheMode to Enabled
            this.EnqueueCallback(() =>
            {
                (Frame.Content as NavigationCacheModeChangesPage).SetTextBoxText(stateSetInNavigationCacheModeChanges);
                (Frame.Content as NavigationCacheModeChangesPage).NavigationCacheMode = NavigationCacheMode.Enabled;
            });

            // Fifth navigation, go forward to Page1
            this.EnqueueCallback(() => Frame.GoForward());
            this.EnqueueConditional(() => complete == 5);

            // Sixth navigation, go back (to the NavigationCacheModeChanges page, which is now set to NavigationCacheMode == Enabled)
            this.EnqueueCallback(() => Frame.GoBack());
            this.EnqueueConditional(() => complete == 6);

            this.EnqueueTestComplete();
        }

        #endregion

        #endregion

    }
}
