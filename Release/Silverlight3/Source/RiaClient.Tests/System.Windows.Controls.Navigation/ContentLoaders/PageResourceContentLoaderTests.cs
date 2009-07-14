//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Navigation.UnitTests
{
    //

    [TestClass]
    public class PageResourceContentLoaderTests : SilverlightTest
    {
        #region Static fields and constants

        private static readonly string TestPagesPath = @"/System.Windows.Controls.Navigation/ContentLoaders/TestPages/";

        #endregion

        #region Properties

        private ContentLoaderBase PageResourceContentLoader { get; set; }

        #endregion

        #region Methods

        [TestInitialize]
        public void TestInitialize()
        {
            this.PageResourceContentLoader = new PageResourceContentLoader();
        }

        [TestMethod]
        [Asynchronous]
        public void NullUriIsError()
        {
            int errorCount = 0;

            AsyncCallback callback = (asyncResult) =>
            {
                try
                {
                    Assert.IsFalse(asyncResult.CompletedSynchronously);
                    Assert.IsTrue(asyncResult.IsCompleted);
                    this.PageResourceContentLoader.EndLoad(asyncResult);
                }
                catch (Exception ex)
                {
                    Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
                    errorCount++;
                }
            };

            this.EnqueueCallback(() => this.PageResourceContentLoader.BeginLoad(null,callback,null));
            this.EnqueueConditional(() => errorCount == 1);
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void LoadPureXaml()
        {
            bool complete = false;

            Uri uriToPureXaml = new Uri(TestPagesPath + "PureXamlPage.xaml", UriKind.Relative);

            AsyncCallback callback =
                (asyncResult) =>
                {
                    Assert.IsFalse(asyncResult.CompletedSynchronously);
                    Assert.IsTrue(asyncResult.IsCompleted);
                    object content = this.PageResourceContentLoader.EndLoad(asyncResult);
                    Assert.IsInstanceOfType(content, typeof(Page));
                    complete = true;
                };

            this.EnqueueCallback(() => this.PageResourceContentLoader.BeginLoad(uriToPureXaml, callback, null));
            this.EnqueueConditional(() => complete);
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void LoadXamlWithXClass()
        {
            bool complete = false;

            Uri uriToXamlWithCodebehind = new Uri(TestPagesPath + "PageWithCodebehind.xaml", UriKind.Relative);

            AsyncCallback callback =
                (asyncResult) =>
                {
                    Assert.IsFalse(asyncResult.CompletedSynchronously);
                    Assert.IsTrue(asyncResult.IsCompleted);
                    object content = this.PageResourceContentLoader.EndLoad(asyncResult);
                    Assert.IsInstanceOfType(content, typeof(PageWithCodebehind));

                    complete = true;
                };

            this.EnqueueCallback(() => this.PageResourceContentLoader.BeginLoad(uriToXamlWithCodebehind, callback, null));
            this.EnqueueConditional(() => complete);
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void LoadXamlWithXClassAndComment()
        {
            // This unit test specifically exists to ensure no regression on a bug where XAML files which contained a
            // comment at the top of the file failed to load correctly (due to a bug in the regular expression parsing for x:Class)

            bool complete = false;

            Uri uriToXamlWithCodebehindAndComment = new Uri(TestPagesPath + "PageWithCodebehindAndComment.xaml", UriKind.Relative);

            AsyncCallback callback =
                (asyncResult) =>
                {
                    Assert.IsFalse(asyncResult.CompletedSynchronously);
                    Assert.IsTrue(asyncResult.IsCompleted);
                    object content = this.PageResourceContentLoader.EndLoad(asyncResult);
                    Assert.IsInstanceOfType(content, typeof(PageWithCodebehindAndComment));

                    complete = true;
                };

            this.EnqueueCallback(() => this.PageResourceContentLoader.BeginLoad(uriToXamlWithCodebehindAndComment, callback, null));
            this.EnqueueConditional(() => complete);
            this.EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void LoadXamlWithQueryString()
        {
            bool complete = false;

            Uri uriToXamlWithCodebehind = new Uri(TestPagesPath + "PageWithCodebehind.xaml?field1=val1&field2=val2", UriKind.Relative);

            AsyncCallback callback =
                (asyncResult) =>
                {
                    Assert.IsFalse(asyncResult.CompletedSynchronously);
                    Assert.IsTrue(asyncResult.IsCompleted);
                    object content = this.PageResourceContentLoader.EndLoad(asyncResult);
                    Assert.IsInstanceOfType(content, typeof(PageWithCodebehind));

                    complete = true;
                };

            this.EnqueueCallback(() => this.PageResourceContentLoader.BeginLoad(uriToXamlWithCodebehind, callback, null));
            this.EnqueueConditional(() => complete);
            this.EnqueueTestComplete();
        }

        #endregion
    }
}
