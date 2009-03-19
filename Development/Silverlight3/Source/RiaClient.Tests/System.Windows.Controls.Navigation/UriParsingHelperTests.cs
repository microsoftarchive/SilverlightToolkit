//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace System.Windows.Navigation.UnitTests
{
    [TestClass]
    public class UriParsingHelperTests : SilverlightTest
    {
        Uri[] internalUris;
        string[] expectedExternalValues;

        public UriParsingHelperTests()
        {
            internalUris = new Uri[] {
                RelativeUri("Page1.xaml"),
                RelativeUri("Page1.xaml?id=123"), // query string only
                RelativeUri("Page1.xaml#fragment1"), // fragment only
                RelativeUri("Page1.xaml?id=123#fragment1"), // query string + fragment
                RelativeUri("Page1.xaml?id=123&sort=asc"), // query string with multiple values
                RelativeUri("Page1.xaml?id=123&sort=asc#fragment1"), // query string with multiple values + fragment
                RelativeUri("Page1.xaml?id="), // query string with empty value
                RelativeUri("Page1.xaml?id=#fragment1"), // query string with empty value + fragment
                RelativeUri("Page1.xaml?id=&sort=asc"), // query string with empty value + multiple values
                RelativeUri("Page1.xaml?id=&sort=asc#fragment1"), // query string with empty value + multiple values + fragment
                RelativeUri("Views/Page1.xaml"), // folder paths
                RelativeUri("Views/Page1.xaml?id=123"), // folder paths + query string
                RelativeUri("Views/Page1.xaml#fragment1"), // folder paths + fragment
                RelativeUri("Views/Page1.xaml?id=123#fragment1"), // folder paths + query string + fragment
                RelativeUri("Views/Page1$.xaml?id=123#fragment1"), // path that includes "$", which should get encoded
                RelativeUri("Views/Page1;.xaml?id=123#fragment1"), // path that includes ";", which should get encoded
                RelativeUri("Views/Page1.xaml?id=12$3"), // query string value includes "$"
                RelativeUri("Views/Page1.xaml?id=12;3"), // query string value includes ";"
                RelativeUri("Views/Page1.xaml?i$d=123"), // query string key includes "$"
                RelativeUri("Views/Page1.xaml?i;d=123"), // query string key includes ";"
                RelativeUri("Views/Page1.xaml#fragment$1"), // fragment includes "$"
                RelativeUri("Views/Page1.xaml#fragment;1"), // fragment includes ";"
                RelativeUri("Views/完全采用统.xaml"), // global characters in path
                RelativeUri("Views/Page1.xaml?完全采用统=123"), // global characters in query string key
                RelativeUri("Views/Page1.xaml?id=完全采用统"), // global characters in query string value
                RelativeUri("Views/Page1.xaml#完全采用统") // global characters in fragment
            };

            expectedExternalValues = new string[] {
                "Page1.xaml",
                "Page1.xaml;id/123",
                "Page1.xaml$fragment1",
                "Page1.xaml;id/123$fragment1",
                "Page1.xaml;id/123/sort/asc",
                "Page1.xaml;id/123/sort/asc$fragment1",
                "Page1.xaml;id/",
                "Page1.xaml;id/$fragment1",
                "Page1.xaml;id//sort/asc",
                "Page1.xaml;id//sort/asc$fragment1",
                "Views/Page1.xaml",
                "Views/Page1.xaml;id/123",
                "Views/Page1.xaml$fragment1",
                "Views/Page1.xaml;id/123$fragment1",
                "Views/Page1%24.xaml;id/123$fragment1",
                "Views/Page1%3B.xaml;id/123$fragment1",
                "Views/Page1.xaml;id/12%243",
                "Views/Page1.xaml;id/12%3B3",
                "Views/Page1.xaml;i%24d/123",
                "Views/Page1.xaml;i%3Bd/123",
                "Views/Page1.xaml$fragment%241",
                "Views/Page1.xaml$fragment%3B1",
                "Views/%E5%AE%8C%E5%85%A8%E9%87%87%E7%94%A8%E7%BB%9F.xaml",
                "Views/Page1.xaml;%E5%AE%8C%E5%85%A8%E9%87%87%E7%94%A8%E7%BB%9F/123",
                "Views/Page1.xaml;id/%E5%AE%8C%E5%85%A8%E9%87%87%E7%94%A8%E7%BB%9F",
                "Views/Page1.xaml$%E5%AE%8C%E5%85%A8%E9%87%87%E7%94%A8%E7%BB%9F"
            };
        }

        #region InternalUriMerge tests

        [TestMethod]
        [Description("Merge a fragment and a basic Uri")]
        public void InternalUriMergeBaseToFragment()
        {
            Uri baseUri = new Uri("/Page1.xaml", UriKind.Relative);
            Uri newUri = new Uri("#fragment1", UriKind.Relative);

            Uri mergedUri = UriParsingHelper.InternalUriMerge(baseUri, newUri);

            Assert.AreEqual<string>("/Page1.xaml#fragment1", mergedUri.OriginalString);
        }

        [TestMethod]
        [Description("Merge a fragment and a basic Uri with a fragment")]
        public void InternalUriMergeBaseWithFragmentToFragment()
        {
            Uri baseUri = new Uri("/Page1.xaml#fragment1", UriKind.Relative);
            Uri newUri = new Uri("#fragment2", UriKind.Relative);

            Uri mergedUri = UriParsingHelper.InternalUriMerge(baseUri, newUri);

            Assert.AreEqual<string>("/Page1.xaml#fragment2", mergedUri.OriginalString);
        }

        [TestMethod]
        [Description("Merge a new Uri and a basic Uri")]
        public void InternalUriMergeBaseToNewUri()
        {
            Uri baseUri = new Uri("/Page1.xaml", UriKind.Relative);
            Uri newUri = new Uri("/SubPages/Page2.xaml", UriKind.Relative);

            Uri mergedUri = UriParsingHelper.InternalUriMerge(baseUri, newUri);

            Assert.AreEqual(newUri, mergedUri);
        }

        [TestMethod]
        [Description("Merge a new Uri with a query and a basic Uri")]
        public void InternalUriMergeBaseToUriWithQuery()
        {
            Uri baseUri = new Uri("/Page1.xaml", UriKind.Relative);
            Uri newUri = new Uri("/Page2.xaml?id=123&sort=asc", UriKind.Relative);

            Uri mergedUri = UriParsingHelper.InternalUriMerge(baseUri, newUri);

            Assert.AreEqual(newUri, mergedUri);
        }
        
        #endregion InternalUriMerge tests

        #region InternalUriToExternalValue tests

        private Uri RelativeUri(string uriValue)
        {
            return new Uri(uriValue, UriKind.Relative);
        }

        [TestMethod]
        public void InternalUriToExternalValue()
        {
            for(int i = 0; i < internalUris.Length; i++) {
                Assert.AreEqual<string>(expectedExternalValues[i], UriParsingHelper.InternalUriToExternalValue(internalUris[i]));
            }
        }

        #endregion InternalUriToExternalValue tests

        #region InternalUriFromExternalValue tests

        [TestMethod]
        public void InternalUriFromExternalValue()
        {
            for (int i = 0; i < internalUris.Length; i++)
            {
                Assert.AreEqual<string>(internalUris[i].OriginalString, UriParsingHelper.InternalUriFromExternalValue(expectedExternalValues[i]));
            }
        }

        #endregion InternalUriFromExternalValue tests
    }
}
