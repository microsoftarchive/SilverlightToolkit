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
        List<Uri> internalUris = new List<Uri>();
        List<string> expectedExternalValues = new List<string>();
        List<QueryStringDictionaryTest> expectedQueryStringDictionaries = new List<QueryStringDictionaryTest>();

        public UriParsingHelperTests()
        {
            // QS dictionaries that are used in many tests
            var emptyQS = new QueryStringDictionaryTest() { Count = 0 };
            var id123QS = new QueryStringDictionaryTest()
            {
                Count = 1,
                Keys = new string[] { "id" },
                Values = new string[] { "123" }
            };
            var id123SortAscQS = new QueryStringDictionaryTest()
            {
                Count = 2,
                Keys = new string[] { "id", "sort" },
                Values = new string[] { "123", "asc" }
            };
            var idEmptyQS = new QueryStringDictionaryTest()
            {
                Count = 1,
                Keys = new string[] { "id" },
                Values = new string[] { String.Empty }
            };
            var idEmptySortAscQS = new QueryStringDictionaryTest()
            {
                Count = 2,
                Keys = new string[] { "id", "sort" },
                Values = new string[] { String.Empty, "asc" }
            };


            AddTest(RelativeUri("Page1.xaml"),
                    "Page1.xaml",
                    emptyQS);

            AddTest(RelativeUri("Page1.xaml?id=123"), // 1, query string only
                    "Page1.xaml?id=123",
                    id123QS);

            AddTest(RelativeUri("Page1.xaml#fragment1"), // 2, fragment only
                    "Page1.xaml$fragment1",
                    emptyQS);

            AddTest(RelativeUri("Page1.xaml?id=123#fragment1"), // 3, query string + fragment
                    "Page1.xaml?id=123$fragment1",
                    id123QS);

            AddTest(RelativeUri("Page1.xaml?id=123&sort=asc"), // 4, query string with multiple values
                    "Page1.xaml?id=123&sort=asc",
                    id123SortAscQS);

            AddTest(RelativeUri("Page1.xaml?id=123&sort=asc#fragment1"), // 5, query string with multiple values + fragment
                    "Page1.xaml?id=123&sort=asc$fragment1",
                    id123SortAscQS);

            AddTest(RelativeUri("Page1.xaml?id="), // 6, query string with empty value
                    "Page1.xaml?id=",
                    idEmptyQS);

            AddTest(RelativeUri("Page1.xaml?id=#fragment1"), // 7, query string with empty value + fragment
                    "Page1.xaml?id=$fragment1",
                    idEmptyQS);

            AddTest(RelativeUri("Page1.xaml?id=&sort=asc"), // 8, query string with empty value + multiple values
                    "Page1.xaml?id=&sort=asc",
                    idEmptySortAscQS);

            AddTest(RelativeUri("Page1.xaml?id=&sort=asc#fragment1"), // 9, query string with empty value + multiple values + fragment
                    "Page1.xaml?id=&sort=asc$fragment1",
                    idEmptySortAscQS);

            AddTest(RelativeUri("/Views/Page1.xaml"), // 10, folder paths
                    "/Views/Page1.xaml",
                    emptyQS);

            AddTest(RelativeUri("/Views/Page1.xaml?id=123"), // 11, folder paths + query string
                    "/Views/Page1.xaml?id=123",
                    id123QS);

            AddTest(RelativeUri("/Views/Page1.xaml#fragment1"), // folder paths + fragment
                    "/Views/Page1.xaml$fragment1",
                    emptyQS);

            AddTest(RelativeUri("/Views/Page1.xaml?id=123#fragment1"), // folder paths + query string + fragment
                    "/Views/Page1.xaml?id=123$fragment1",
                    id123QS);

            AddTest(RelativeUri("/Views/Page1$.xaml?id=123#fragment1"), // path that includes "$", which should get encoded
                    "/Views/Page1%24.xaml?id=123$fragment1",
                    id123QS);

            AddTest(RelativeUri("/Views/Page1.xaml?id=12$3"), // query string value includes "$"
                    "/Views/Page1.xaml?id=12%243",
                    new QueryStringDictionaryTest()
                    {
                        Count = 1,
                        Keys = new string[] { "id" },
                        Values = new string[] { "12$3" }
                    });

            AddTest(RelativeUri("/Views/Page1.xaml?id=12?3"), // query string value includes "?"
                    "/Views/Page1.xaml?id=12?3",
                    new QueryStringDictionaryTest()
                    {
                        Count = 1,
                        Keys = new string[] { "id" },
                        Values = new string[] { "12?3" }
                    });

            AddTest(RelativeUri("/Views/Page1.xaml?i$d=123"), // query string key includes "$"
                    "/Views/Page1.xaml?i%24d=123",
                    new QueryStringDictionaryTest()
                    {
                        Count = 1,
                        Keys = new string[] { "i$d" },
                        Values = new string[] { "123" }
                    });

            AddTest(RelativeUri("/Views/Page1.xaml?i?d=123"), // query string key includes "?"
                    "/Views/Page1.xaml?i?d=123",
                    new QueryStringDictionaryTest()
                    {
                        Count = 1,
                        Keys = new string[] { "i?d" },
                        Values = new string[] { "123" }
                    });

            AddTest(RelativeUri("/Views/Page1.xaml#fragment$1"), // fragment includes "$"
                    "/Views/Page1.xaml$fragment%241",
                    emptyQS);

            AddTest(RelativeUri("/Views/Page1.xaml#fragment?1"), // fragment includes "?"
                    "/Views/Page1.xaml$fragment?1",
                    emptyQS);

            AddTest(RelativeUri("/Views/完全采用统.xaml"), // global characters in path
                    "/Views/%E5%AE%8C%E5%85%A8%E9%87%87%E7%94%A8%E7%BB%9F.xaml",
                    emptyQS);

            AddTest(RelativeUri("/Views/Page1.xaml?完全采用统=123"), // global characters in query string key
                    "/Views/Page1.xaml?%E5%AE%8C%E5%85%A8%E9%87%87%E7%94%A8%E7%BB%9F=123",
                    new QueryStringDictionaryTest()
                    {
                        Count = 1,
                        Keys = new string[] { "完全采用统" },
                        Values = new string[] { "123" }
                    });

            AddTest(RelativeUri("/Views/Page1.xaml?id=完全采用统"), // global characters in query string value
                    "/Views/Page1.xaml?id=%E5%AE%8C%E5%85%A8%E9%87%87%E7%94%A8%E7%BB%9F",
                    new QueryStringDictionaryTest()
                    {
                        Count = 1,
                        Keys = new string[] { "id" },
                        Values = new string[] { "完全采用统" }
                    });

            AddTest(RelativeUri("/Views/Page1.xaml#完全采用统"), // global characters in fragment
                    "/Views/Page1.xaml$%E5%AE%8C%E5%85%A8%E9%87%87%E7%94%A8%E7%BB%9F",
                    emptyQS);

            AddTest(RelativeUri("/Views/Page1.xaml?page=/Views/Page2.xaml"), // path passed as a query string parameter
                    "/Views/Page1.xaml?page=/Views/Page2.xaml",
                    new QueryStringDictionaryTest()
                    {
                        Count = 1,
                        Keys = new string[] { "page" },
                        Values = new string[] { "/Views/Page2.xaml" }
                    });

            AddTest(RelativeUri("/Views/Page1.xaml?id=1%262&sort=asc"), // query string value includes escaped "&"
                    "/Views/Page1.xaml?id=1%262&sort=asc",
                    new QueryStringDictionaryTest()
                    {
                        Count = 2,
                        DecodeResults = true,
                        Keys = new string[] { "id", "sort" },
                        Values = new string[] { "1&2", "asc" }
                    });

            AddTest(RelativeUri("/Views/Page1.xaml?id=1%262&sort=asc"), // query string value includes escaped "&", don't decode dictionary
                                "/Views/Page1.xaml?id=1%262&sort=asc",
                                new QueryStringDictionaryTest()
                                {
                                    Count = 2,
                                    Keys = new string[] { "id", "sort" },
                                    Values = new string[] { "1%262", "asc" }
                                });

            AddTest(RelativeUri("/Views/Page1.xaml?i%26d=1&sort=asc"), // query string key includes escaped "&"
                    "/Views/Page1.xaml?i%26d=1&sort=asc",
                    new QueryStringDictionaryTest()
                    {
                        Count = 2,
                        DecodeResults = true,
                        Keys = new string[] { "i&d", "sort" },
                        Values = new string[] { "1", "asc" }
                    });

            AddTest(RelativeUri("/Views/Page1.xaml?i%26d=1&sort=asc"), // query string key includes escaped "&", don't decode dictionary
                    "/Views/Page1.xaml?i%26d=1&sort=asc",
                    new QueryStringDictionaryTest()
                    {
                        Count = 2,
                        Keys = new string[] { "i%26d", "sort" },
                        Values = new string[] { "1", "asc" }
                    });

            AddTest(RelativeUri("/Views/Page1.xaml?id=1=2&sort=asc"), // query string value includes "="
                    "/Views/Page1.xaml?id=1=2&sort=asc",
                    new QueryStringDictionaryTest()
                    {
                        Count = 2,
                        Keys = new string[] { "id", "sort" },
                        Values = new string[] { "1=2", "asc" }
                    });
        }

        private void AddTest(Uri internalUri, string expectedExternalValue, QueryStringDictionaryTest queryStringDictionary)
        {
            internalUris.Add(internalUri);
            expectedExternalValues.Add(expectedExternalValue);
            expectedQueryStringDictionaries.Add(queryStringDictionary);
        }

        #region InternalUriMerge tests

        [TestMethod]
        [Description("")]
        public void InternalUriMergeTests()
        {
            // Test all the possible combinations of these values (except for BaseUri of #fragment1 as base Uri's cannot be fragments by definition):
            // /Views/Home.xaml
            // /Home.xaml
            // /FooAssembly;component/FooHome.xaml
            // /BarAssembly;component/BarHome.xaml
            // #fragment1
            // abc

            InternalUriMergeTest[] tests = new InternalUriMergeTest[] {
                // BaseUri of /Views/Home.xaml, all permutations of NewUri
                new InternalUriMergeTest() { BaseUri = RelativeUri("/Views/Home.xaml"),
                                             NewUri = RelativeUri("/Home.xaml"),
                                             ResultingUri = RelativeUri("/Home.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/Views/Home.xaml"),
                                             NewUri = RelativeUri("/FooAssembly;component/FooHome.xaml"),
                                             ResultingUri = RelativeUri("/FooAssembly;component/FooHome.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/Views/Home.xaml"),
                                             NewUri = RelativeUri("/BarAssembly;component/BarHome.xaml"),
                                             ResultingUri = RelativeUri("/BarAssembly;component/BarHome.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/Views/Home.xaml"),
                                             NewUri = RelativeUri("#fragment1"),
                                             ResultingUri = RelativeUri("/Views/Home.xaml#fragment1") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/Views/Home.xaml"),
                                             NewUri = RelativeUri("abc"),
                                             ResultingUri = RelativeUri("abc") },


                // BaseUri of /Home.xaml, all permutations of NewUri
                new InternalUriMergeTest() { BaseUri = RelativeUri("/Home.xaml"),
                                             NewUri = RelativeUri("/Views/Home.xaml"),
                                             ResultingUri = RelativeUri("/Views/Home.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/Home.xaml"),
                                             NewUri = RelativeUri("/FooAssembly;component/FooHome.xaml"),
                                             ResultingUri = RelativeUri("/FooAssembly;component/FooHome.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/Home.xaml"),
                                             NewUri = RelativeUri("/BarAssembly;component/BarHome.xaml"),
                                             ResultingUri = RelativeUri("/BarAssembly;component/BarHome.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/Home.xaml"),
                                             NewUri = RelativeUri("#fragment1"),
                                             ResultingUri = RelativeUri("/Home.xaml#fragment1") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/Home.xaml"),
                                             NewUri = RelativeUri("abc"),
                                             ResultingUri = RelativeUri("abc") },



                // BaseUri of FooAssembly;component/FooHome.xaml, all permutations of NewUri
                new InternalUriMergeTest() { BaseUri = RelativeUri("/FooAssembly;component/FooHome.xaml"),
                                             NewUri = RelativeUri("/Views/Home.xaml"),
                                             ResultingUri = RelativeUri("/Views/Home.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/FooAssembly;component/FooHome.xaml"),
                                             NewUri = RelativeUri("/Home.xaml"),
                                             ResultingUri = RelativeUri("/Home.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/FooAssembly;component/FooHome.xaml"),
                                             NewUri = RelativeUri("/BarAssembly;component/BarHome.xaml"),
                                             ResultingUri = RelativeUri("/BarAssembly;component/BarHome.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/FooAssembly;component/FooHome.xaml"),
                                             NewUri = RelativeUri("#fragment1"),
                                             ResultingUri = RelativeUri("/FooAssembly;component/FooHome.xaml#fragment1") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/FooAssembly;component/FooHome.xaml"),
                                             NewUri = RelativeUri("abc"),
                                             ResultingUri = RelativeUri("abc") },


                // BaseUri of BarAssembly;component/BarHome.xaml, all permutations of NewUri
                new InternalUriMergeTest() { BaseUri = RelativeUri("/BarAssembly;component/BarHome.xaml"),
                                             NewUri = RelativeUri("/Views/Home.xaml"),
                                             ResultingUri = RelativeUri("/Views/Home.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/BarAssembly;component/BarHome.xaml"),
                                             NewUri = RelativeUri("/Home.xaml"),
                                             ResultingUri = RelativeUri("/Home.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/BarAssembly;component/BarHome.xaml"),
                                             NewUri = RelativeUri("/FooAssembly;component/FooHome.xaml"),
                                             ResultingUri = RelativeUri("/FooAssembly;component/FooHome.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/BarAssembly;component/BarHome.xaml"),
                                             NewUri = RelativeUri("#fragment1"),
                                             ResultingUri = RelativeUri("/BarAssembly;component/BarHome.xaml#fragment1") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("/BarAssembly;component/BarHome.xaml"),
                                             NewUri = RelativeUri("abc"),
                                             ResultingUri = RelativeUri("abc") },


                // BaseUri of abc, all permutations of NewUri
                new InternalUriMergeTest() { BaseUri = RelativeUri("abc"),
                                             NewUri = RelativeUri("/Views/Home.xaml"),
                                             ResultingUri = RelativeUri("/Views/Home.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("abc"),
                                             NewUri = RelativeUri("/Home.xaml"),
                                             ResultingUri = RelativeUri("/Home.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("abc"),
                                             NewUri = RelativeUri("/FooAssembly;component/FooHome.xaml"),
                                             ResultingUri = RelativeUri("/FooAssembly;component/FooHome.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("abc"),
                                             NewUri = RelativeUri("/BarAssembly;component/BarHome.xaml"),
                                             ResultingUri = RelativeUri("/BarAssembly;component/BarHome.xaml") },

                new InternalUriMergeTest() { BaseUri = RelativeUri("abc"),
                                             NewUri = RelativeUri("#fragment1"),
                                             ResultingUri = RelativeUri("abc#fragment1") }
            };


            foreach (var test in tests)
            {
                Uri mergedUri = UriParsingHelper.InternalUriMerge(test.BaseUri, test.NewUri);
                Assert.AreEqual(test.ResultingUri, mergedUri,
                                string.Format("When merging\n{0}\nand\n{1}\nThe result should have been\n{2}\n, but it was\n{3}",
                                              test.BaseUri,
                                              test.NewUri,
                                              test.ResultingUri,
                                              mergedUri));
            }
        }

        private struct InternalUriMergeTest
        {
            public Uri BaseUri;
            public Uri NewUri;
            public Uri ResultingUri;
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
            for (int i = 0; i < internalUris.Count; i++)
            {
                Assert.AreEqual<string>(expectedExternalValues[i], UriParsingHelper.InternalUriToExternalValue(internalUris[i]));
            }
        }

        #endregion InternalUriToExternalValue tests

        #region InternalUriFromExternalValue tests

        [TestMethod]
        public void InternalUriFromExternalValue()
        {
            for (int i = 0; i < internalUris.Count; i++)
            {
                Assert.AreEqual<string>(internalUris[i].OriginalString, UriParsingHelper.InternalUriFromExternalValue(expectedExternalValues[i]));
            }
        }

        #endregion InternalUriFromExternalValue tests

        #region InternalUriParseQueryStringToDictionary tests

        [TestMethod]
        public void InternalUriParseQueryStringToDictionary()
        {
            for (int i = 0; i < expectedQueryStringDictionaries.Count; i++)
            {
                IDictionary<string, string> actualQueryStringDictionary = UriParsingHelper.InternalUriParseQueryStringToDictionary(internalUris[i], expectedQueryStringDictionaries[i].DecodeResults /* decodeResults */);
                Assert.AreEqual(expectedQueryStringDictionaries[i].Count, actualQueryStringDictionary.Count);
                if (expectedQueryStringDictionaries[i].Keys != null)
                {
                    for (int keyIndex = 0; keyIndex < expectedQueryStringDictionaries[i].Keys.Length; keyIndex++)
                    {
                        string key = expectedQueryStringDictionaries[i].Keys[keyIndex];
                        Assert.IsTrue(actualQueryStringDictionary.Keys.Contains(key));
                        Assert.AreEqual(expectedQueryStringDictionaries[i].Values[keyIndex], actualQueryStringDictionary[key]);
                    }
                }
            }
        }

        private struct QueryStringDictionaryTest
        {
            public bool DecodeResults;
            public int Count;
            public string[] Keys;
            public string[] Values;
        }

        #endregion

        #region InternalUriIsNavigable tests

        [TestMethod]
        public void InternalUriIsNavigableTests()
        {
            // Dictionary holds the Uri, and a boolean indicating if it is supposed to be navigable
            Dictionary<Uri, bool> urisToTest = new Dictionary<Uri, bool>();

            urisToTest.Add(RelativeUri("Page1.xaml"), false);
            urisToTest.Add(RelativeUri("/Page1.xaml"), true);
            urisToTest.Add(RelativeUri("/Views/Page1.xaml"), true);
            urisToTest.Add(RelativeUri("Assembly1;component/Page1.xaml"), false);
            urisToTest.Add(RelativeUri("/Assembly1;component/Page1.xaml"), true);
            urisToTest.Add(RelativeUri("/Assembly1;component/Views/Page1.xaml"), true);
            urisToTest.Add(RelativeUri("#foo"), true);

            foreach (Uri uri in urisToTest.Keys)
            {
                // The Uri should be navigable/un-navigable as specified in the dictionary
                Assert.AreEqual(urisToTest[uri], UriParsingHelper.InternalUriIsNavigable(uri));

                // If this a fragment Uri the following extra tests do not make sense, so don't do them
                if (uri.OriginalString != "#foo")
                {
                    // A query string should not change its navigability
                    Uri uriWithQuery = new Uri(uri.OriginalString + "?id=123&sort=asc", UriKind.Relative);
                    Assert.AreEqual(urisToTest[uri], UriParsingHelper.InternalUriIsNavigable(uriWithQuery));

                    // A fragment should not change its navigability
                    Uri uriWithFragment = new Uri(uri.OriginalString + "#abc", UriKind.Relative);
                    Assert.AreEqual(urisToTest[uri], UriParsingHelper.InternalUriIsNavigable(uriWithFragment));

                    // A query string and a fragment should not change its navigability
                    Uri uriWithQueryAndFragment = new Uri(uri.OriginalString + "?id=123&sort=asc#abc", UriKind.Relative);
                    Assert.AreEqual(urisToTest[uri], UriParsingHelper.InternalUriIsNavigable(uriWithQueryAndFragment));
                }
            }
        }

        #endregion

    }
}
