//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Browser;
using System.Windows.Data.Test.Utilities;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace System.Windows.Navigation.UnitTests
{
    /// <summary>
    /// Unit tests for the UriFragmentHelper type.
    /// </summary>
    [TestClass]
    public sealed class UriFragmentHelperTests : SilverlightTest
    {
        #region Properties

        /// <summary>
        /// Gets the value of the browser title before running UriFragmentHelper tests.
        /// </summary>
        public string OriginalBrowserTitle
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the value of Navigated events fired on UriFragmentHelper.
        /// </summary>
        public int NavigationCount
        {
            get;
            private set;
        }

        #endregion

        #region Methods
        #region Test Initializer Methods

        /// <summary>
        /// Initializes the class for testing.
        /// </summary>
        [ClassInitialize]
        public void ClassInitialize()
        {
            // Record original values before we begin tests.
            this.OriginalBrowserTitle = BrowserHelper.Title;
        }

        /// <summary>
        /// Performs cleanup after running UriFragmentHelper tests.
        /// </summary>
        [ClassCleanup]
        public void ClassCleanup()
        {
            // Reset the browser hash and title to the values observed before tests.
            this.ResetBrowser();

            // Remove our event handler
            UriFragmentHelper.Navigated -= HtmlUriStateHelper_Navigated;
        }

        /// <summary>
        /// Initializes the browser fragment (hash) before running a UriFragmentHelper test.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            // Remove our event handler
            UriFragmentHelper.Navigated -= HtmlUriStateHelper_Navigated;

            this.ResetBrowser();
            this.NavigationCount = 0;

            // Attach an event handler
            UriFragmentHelper.Navigated += HtmlUriStateHelper_Navigated;
        }

        #endregion Test Initializer Methods
        
        #region Test Methods

        /// <summary>
        /// Verifies the IsEnabled property behaves correctly.
        /// </summary>
        [TestMethod]
        [Description("Verifies the IsEnabled property behaves correctly.")]
        public void IsEnabled()
        {
            Assert.IsTrue(HtmlPage.IsEnabled == UriFragmentHelper.IsEnabled);

        }

        /// <summary>
        /// Verifies the Navigate method behaves correctly.
        /// </summary>
        [TestMethod, Asynchronous]
        [Description("Verifies the Navigate method behaves correctly.")]
        public void Navigate()
        {
            // Generate some test values
            string fragment1 = Guid.NewGuid().ToString();
            string fragment2 = Guid.NewGuid().ToString();
            string fragment3 = Guid.NewGuid().ToString();
            string title1 = Guid.NewGuid().ToString();
            string title2 = Guid.NewGuid().ToString();
            string title3 = Guid.NewGuid().ToString();

            // Perform a navigation and verify the browser location fragment (hash) value.
            this.EnqueueCallback(() => UriFragmentHelper.Navigate(fragment1, title1));
            this.EnqueueConditional(() => this.NavigationCount == 1);
            this.EnqueueCallback(
                () =>
                {
                    Assert.AreEqual<string>(fragment1, BrowserHelper.Location.Hash);
                    Assert.AreEqual<string>(title1, BrowserHelper.Title);
                });

            // Perform a navigation and verify the browser location fragment (hash) value.
            this.EnqueueCallback(() => UriFragmentHelper.Navigate(fragment2, title2));
            this.EnqueueConditional(() => this.NavigationCount == 2);
            this.EnqueueCallback(
                () =>
                {
                    Assert.AreEqual<string>(fragment2, BrowserHelper.Location.Hash);
                    Assert.AreEqual<string>(title2, BrowserHelper.Title);
                });

            // Perform a navigation and verify the browser location fragment (hash) value.
            this.EnqueueCallback(() => UriFragmentHelper.Navigate(fragment3, title3));
            this.EnqueueConditional(() => this.NavigationCount == 3);
            this.EnqueueCallback(
                () =>
                {
                    Assert.AreEqual<string>(fragment3, BrowserHelper.Location.Hash);
                    Assert.AreEqual<string>(title3, BrowserHelper.Title);
                });

            // Navigate the browser back and verify the browser location fragment (hash) value.
            this.EnqueueCallback(() => BrowserHelper.GoBack());
            this.EnqueueConditional(() => this.NavigationCount == 4);
            this.EnqueueCallback(
                () =>
                {
                    Assert.AreEqual<string>(fragment2, BrowserHelper.Location.Hash);

                    //// Note: Firefox, IE8 and Safari do not preserve titles.
                    //if (HtmlPage.BrowserInformation.Name.ToUpper(CultureInfo.InvariantCulture).Contains("INTERNET EXPLORER"))
                    //{
                    //    Assert.AreEqual<string>(title2, BrowserHelper.Title);
                    //}
                });

            // Navigate the browser back and verify the browser location fragment (hash) value.
            this.EnqueueCallback(() => BrowserHelper.GoBack());
            this.EnqueueConditional(() => this.NavigationCount == 5);
            this.EnqueueCallback(
                () =>
                {
                    Assert.AreEqual<string>(fragment1, BrowserHelper.Location.Hash);

                    //// Note: Firefox, IE8 and Safari do not preserve titles.
                    //if (HtmlPage.BrowserInformation.Name.ToUpper(CultureInfo.InvariantCulture).Contains("INTERNET EXPLORER"))
                    //{
                    //    Assert.AreEqual<string>(title1, BrowserHelper.Title);
                    //}
                });

            // Navigate the browser forward and verify the browser location fragment (hash) value.
            this.EnqueueCallback(() => BrowserHelper.GoForward());
            this.EnqueueConditional(() => this.NavigationCount == 6);
            this.EnqueueCallback(
                () =>
                {
                    Assert.AreEqual<string>(fragment2, BrowserHelper.Location.Hash);

                    ////// Note: Firefox, IE8 and Safari do not preserve titles.
                    //if (HtmlPage.BrowserInformation.Name.ToUpper(CultureInfo.InvariantCulture).Contains("INTERNET EXPLORER"))
                    //{
                    //    Assert.AreEqual<string>(title2, BrowserHelper.Title);
                    //}
                });

            // Navigate the browser forward and verify the browser location fragment (hash) value.
            this.EnqueueCallback(() => BrowserHelper.GoForward());
            this.EnqueueConditional(() => this.NavigationCount == 7);
            this.EnqueueCallback(
                () =>
                {
                    Assert.AreEqual<string>(fragment3, BrowserHelper.Location.Hash);

                    //// Note: Firefox, IE8 and Safari do not preserve titles.
                    //if (HtmlPage.BrowserInformation.Name.ToUpper(CultureInfo.InvariantCulture).Contains("INTERNET EXPLORER"))
                    //{
                    //    Assert.AreEqual<string>(title3, BrowserHelper.Title);
                    //}
                });
            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Verifies the Navigated event behaves correctly.
        /// </summary>
        [TestMethod, Asynchronous]
        [Description("Verifies the Navigated event behaves correctly.")]
        public void Navigated()
        {
            // Generate some test values
            int counter = 0;
            string fragment1 = Guid.NewGuid().ToString();
            string fragment2 = Guid.NewGuid().ToString();
            string fragment3 = Guid.NewGuid().ToString();

            EventHandler eventHandler =
                (sender, args) =>
                {
                    string cleanFragment = UriFragmentHelper.CurrentFragment;

                    switch (counter)
                    {
                        case 0:
                            Assert.AreEqual<string>(fragment1, cleanFragment);
                            break;
                        case 1:
                            Assert.AreEqual<string>(fragment2, cleanFragment);
                            break;
                        case 2:
                            Assert.AreEqual<string>(fragment3, cleanFragment);
                            break;
                        default:
                            Assert.Fail();
                            break;
                    }
                    counter++;
                };
            UriFragmentHelper.Navigated += eventHandler;

            this.EnqueueCallback(() => UriFragmentHelper.Navigate(fragment1));
            this.EnqueueConditional(() => counter == 1);
            this.EnqueueCallback(() => UriFragmentHelper.Navigate(fragment2));
            this.EnqueueConditional(() => counter == 2);
            this.EnqueueCallback(() => UriFragmentHelper.Navigate(fragment3));
            this.EnqueueConditional(() => counter == 3);

            this.EnqueueCallback(() => UriFragmentHelper.Navigated -= eventHandler);

            this.EnqueueTestComplete();
        }
        
        /// <summary>
        /// Verifies the does not allow for browser location URI values to be longer than 1024 characters.
        /// </summary>
        [TestMethod, Asynchronous]
        [Description("Verifies the does not allow for browser location URI values to be longer than 1024 characters.")]
        public void BrowserUriLength()
        {
            ScriptObject location = HtmlPage.Window.GetProperty("location") as ScriptObject;
            string currentUri = location.Invoke("toString") as string;

            // Cross-browser compat check
            if (!currentUri.Contains("#"))
            {
                currentUri += "#";
            }

            string validUri = new string('a', UriFragmentHelper.MaxUriLength - currentUri.Length);
            string invalidUri = new string('a', UriFragmentHelper.MaxUriLength - currentUri.Length + 1);

            this.EnqueueCallback(() => UriFragmentHelper.Navigate(validUri));
            this.EnqueueConditional(() => this.NavigationCount == 1);
            this.EnqueueCallback(() => ExceptionHelper.ExpectException<InvalidOperationException>(() =>
                UriFragmentHelper.Navigate(invalidUri)));
            this.EnqueueTestComplete();
        }

        #endregion Test Methods

        /// <summary>
        /// Resets the browser location fragment (hash) value to be empty.
        /// </summary>
        private void ResetBrowser()
        {
            UriFragmentHelper.Navigate(string.Empty, this.OriginalBrowserTitle);
        }

        /// <summary>
        /// Raised when UriFragmentHelper has navigated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">Browser navigation event args.</param>
        private void HtmlUriStateHelper_Navigated(object sender, EventArgs eventArgs)
        {
            this.NavigationCount++;
        }

        #endregion Methods
    }
}
