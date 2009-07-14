//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Navigation.UnitTests
{
    [TestClass]
    public class UriMappingTests : SilverlightTest
    {
        [TestMethod]
        [Description("If there are no parameters in the 'Uri' template, then the 'MappedUri' just comes out as specified with no modification")]
        public void NoUriParametersIsPassthrough()
        {
            string UriPattern = "Simple";
            string MappedUriPattern = "Result/123?x=10";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri(UriPattern, UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            Assert.AreEqual<string>(MappedUriPattern, result.OriginalString);
        }

        [TestMethod]
        [Description("If the actual Uri does not match to the Uri pattern, MapUri returns false")]
        public void NonMatchingFails()
        {
            string UriPattern = "Simple";
            string MappedUriPattern = "Result/123?x=10";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri(UriPattern + "/123", UriKind.Relative);

            Uri result = alias.MapUri(testUri); ;

            Assert.IsNull(result);
        }

        [TestMethod]
        [Description("A simple match works as expected")]
        public void SimpleMatch()
        {
            string UriPattern = "Simple/{id}";
            string MappedUriPattern = "Result/{id}";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/123", UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            Assert.AreEqual<string>("Result/123", result.OriginalString);
        }

        [TestMethod]
        [Description("Query string values are preserved, even if they have the same name as one of the template parameters.  The query string is left alone.")]
        public void SimpleMatchOriginalUriQueryStringPreserved()
        {
            string UriPattern = "Simple/{id}";
            string MappedUriPattern = "Result/{id}";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/123?x=10&id=456", UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            IDictionary<string, string> queryStringValues = UriParsingHelper.InternalUriParseQueryStringToDictionary(result, false /* decodeResults */);
            string uriBase = UriParsingHelper.InternalUriGetBaseValue(result);

            Assert.AreEqual<string>("Result/123", uriBase);
            Assert.AreEqual(2, queryStringValues.Count);
            Assert.IsTrue(queryStringValues.ContainsKey("id") && queryStringValues["id"] == "456");
            Assert.IsTrue(queryStringValues.ContainsKey("x") && queryStringValues["x"] == "10");
        }

        [TestMethod]
        [Description("If a parameter with the same name is used twice in the 'MappedUri' template, this is valid and will be replaced all the times by the one found in the 'Uri' template")]
        public void MappedUriParameterUsedTwiceValid()
        {
            string UriPattern = "Simple/{id}";
            string MappedUriPattern = "Result/{id}.xaml?id={id}";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/123?x=10", UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            IDictionary<string, string> queryStringValues = UriParsingHelper.InternalUriParseQueryStringToDictionary(result, false /* decodeResults */);
            string uriBase = UriParsingHelper.InternalUriGetBaseValue(result);

            Assert.AreEqual<string>("Result/123.xaml", uriBase);
            Assert.IsTrue(queryStringValues.ContainsKey("id") && queryStringValues["id"] == "123");
            Assert.IsTrue(queryStringValues.ContainsKey("x") && queryStringValues["x"] == "10");
        }

        [TestMethod]
        [Description("If a paremeter with the same name is used twice in the 'Uri' template, it is invalid and throws upon attempting to process.")]
        public void UriParameterUsedTwiceThrows()
        {
            string UriPattern = "Simple/{id}/{id}";
            string MappedUriPattern = "Result/{id}";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/123/123", UriKind.Relative);

            try
            {
                Uri result = alias.MapUri(testUri);

                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // Expected
            }
            catch (Exception)
            {
                Assert.Fail("Exception other than InvalidOperationException thrown");
            }
        }

        [TestMethod]
        [Description("The 'Uri' template cannot contain a query string")]
        public void UriCannotContainQueryString()
        {
            string UriPattern = "Simple?var1={x}";
            string MappedUriPattern = "Result.xaml?id={x}";

            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple?var1=10", UriKind.Relative);

            try
            {
                Uri result = alias.MapUri(testUri);

                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // This is expected
            }
            catch (Exception)
            {
                Assert.Fail("Exception other than InvalidOperationException thrown");
            }
        }

        [TestMethod]
        [Description("If a parameter is in the 'Uri' template but is not in the 'MappedUri' template, it is just thrown away")]
        public void UriParameterNotInMappedUriTemplateValid()
        {
            string UriPattern = "Simple/{foo}/{id}";
            string MappedUriPattern = "Result/{id}";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/fooVal/123?x=10", UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            IDictionary<string, string> queryStringValues = UriParsingHelper.InternalUriParseQueryStringToDictionary(result, false /* decodeResults */);
            string uriBase = UriParsingHelper.InternalUriGetBaseValue(result);

            Assert.AreEqual<string>("Result/123", uriBase);
            Assert.IsFalse(queryStringValues.ContainsKey("id"));
            Assert.IsTrue(queryStringValues.ContainsKey("x") && queryStringValues["x"] == "10");
        }

        [TestMethod]
        [Description("If a paremeter is in the 'MappedUri' template but is not in the 'Uri' template, it will be replaced by String.Empty")]
        public void MappedUriParameterNotInUriBecomesEmptyString()
        {
            string UriPattern = "Simple/{id}";
            string MappedUriPattern = "Result/{foo}/{id}";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/123?x=10", UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            IDictionary<string, string> queryStringValues = UriParsingHelper.InternalUriParseQueryStringToDictionary(result, false /* decodeResults */);
            string uriBase = UriParsingHelper.InternalUriGetBaseValue(result);

            Assert.AreEqual<string>("Result//123", uriBase);
            Assert.IsFalse(queryStringValues.ContainsKey("id"));
            Assert.IsFalse(queryStringValues.ContainsKey("foo"));
            Assert.IsTrue(queryStringValues.ContainsKey("x") && queryStringValues["x"] == "10");
        }

        [TestMethod]
        [Description("A parameter in the 'Uri' template can be matched by zero characters - resulting in String.Empty being used in the 'MappedUri' template replacements as if the parameter weren't present")]
        public void UriParameterMatchesNothing()
        {
            string UriPattern = "Simple/{id}";
            string MappedUriPattern = "Result/{id}";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/?x=10", UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            IDictionary<string, string> queryStringValues = UriParsingHelper.InternalUriParseQueryStringToDictionary(result, false /* decodeResults */);
            string uriBase = UriParsingHelper.InternalUriGetBaseValue(result);

            Assert.AreEqual<string>("Result/", uriBase);
            Assert.IsFalse(queryStringValues.ContainsKey("id"));
            Assert.IsTrue(queryStringValues.ContainsKey("x") && queryStringValues["x"] == "10");
        }

        [TestMethod]
        [Description("If a query string value is in the 'MappedUri' template as well as the actual Uri navigated to, it should come out in the resulting Uri as only the one from the actual Uri")]
        public void QueryStringValueInMappedUriAndActualUriHasActualUriWin()
        {
            string UriPattern = "Simple/{id}";
            string MappedUriPattern = "Result/{id}?ShowDetails=true";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/123?ShowDetails=false", UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            IDictionary<string, string> queryStringValues = UriParsingHelper.InternalUriParseQueryStringToDictionary(result, false /* decodeResults */);
            string uriBase = UriParsingHelper.InternalUriGetBaseValue(result);

            Assert.AreEqual<string>("Result/123", uriBase);
            Assert.IsFalse(queryStringValues.ContainsKey("id"));
            Assert.IsTrue(queryStringValues.ContainsKey("ShowDetails") && queryStringValues["ShowDetails"] == "false");
        }

        [TestMethod]
        [Description("If no 'Uri' template was specified, MapUri returns null for non-null input")]
        public void NoUriTemplateDoesNotMatchNonNullInput()
        {
            string MappedUriPattern = "Result.xaml?id={x}";

            UriMapping alias = new UriMapping();
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple?var1=10", UriKind.Relative);

            Assert.IsNull(alias.MapUri(testUri));
        }

        [TestMethod]
        [Description("If no 'Uri' template was specified, MapUri returns the MappedUri exactly for null or String.Empty input")]
        public void NoUriTemplateReturnsMappedUriExactlyWithNullInput()
        {
            string MappedUriPattern = "Result.aspx?id={x}";

            UriMapping alias = new UriMapping();
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri1 = null;
            Uri testUri2 = new Uri(String.Empty, UriKind.Relative);

            Uri result1 = alias.MapUri(testUri1);
            Uri result2 = alias.MapUri(testUri2);

            Assert.AreEqual(MappedUriPattern, result1.OriginalString);
            Assert.AreEqual(MappedUriPattern, result2.OriginalString);
        }

        [TestMethod]
        [Description("If no 'MappedUri' template was specified, throw on MapUri")]
        public void NoMappedUriTemplateThrows()
        {
            string UriPattern = "Simple/var1={x}";

            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/var1=10", UriKind.Relative);

            try
            {
                Uri result = alias.MapUri(testUri);

                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // This is expected
            }
            catch (Exception)
            {
                Assert.Fail("Exception other than InvalidOperationException thrown");
            }
        }

        [TestMethod]
        [Description("If we think of the query string as a set of name/value pairs, verify that it is valid for an identifier to be in the name")]
        public void ParameterUsedAsNameInQueryStringValid()
        {
            string UriPattern = "Simple/{id}/{foo}";
            string MappedUriPattern = "Result.xaml?{foo}2=6";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/123/myVar?x=10", UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            IDictionary<string, string> queryStringValues = UriParsingHelper.InternalUriParseQueryStringToDictionary(result, false /* decodeResults */);
            string uriBase = UriParsingHelper.InternalUriGetBaseValue(result);

            Assert.AreEqual<string>("Result.xaml", uriBase);
            Assert.IsFalse(queryStringValues.ContainsKey("id"));
            Assert.IsFalse(queryStringValues.ContainsKey("foo"));
            Assert.IsTrue(queryStringValues.ContainsKey("x") && queryStringValues["x"] == "10");
            Assert.IsTrue(queryStringValues.ContainsKey("myVar2") && queryStringValues["myVar2"] == "6");
        }

        [TestMethod]
        [Description("If a query string is present in the 'Uri' template then we throw on MapUri")]
        public void QueryStringInUriTemplateThrows()
        {
            string UriPattern = "Simple?var1={x}";
            string MappedUriPattern = "Result";

            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple?var1=10", UriKind.Relative);

            try
            {
                Uri result = alias.MapUri(testUri);

                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // This is expected
            }
            catch (Exception)
            {
                Assert.Fail("Exception other than InvalidOperationException thrown");
            }
        }

        [TestMethod]
        [Description("If a fragment is present in the 'Uri' template then we throw on MapUri")]
        public void FragmentInUriTemplateThrows()
        {
            string UriPattern = "Simple/var1={x}#foo";
            string MappedUriPattern = "Result";

            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/var1=10#foo", UriKind.Relative);

            try
            {
                Uri result = alias.MapUri(testUri);

                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // This is expected
            }
            catch (Exception)
            {
                Assert.Fail("Exception other than InvalidOperationException thrown");
            }
        }

        [TestMethod]
        [Description("If a fragment is present in the 'MappedUri' but not in the Uri being navigated to then it is in the resulting Uri")]
        public void FragmentInMappedUriCarriesOver()
        {
            string UriPattern = "Simple/{id}/{foo}";
            string MappedUriPattern = "Result.xaml?{foo}2=6#fragment1";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/123/myVar?x=10", UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            IDictionary<string, string> queryStringValues = UriParsingHelper.InternalUriParseQueryStringToDictionary(result, false /* decodeResults */);
            string uriBase = UriParsingHelper.InternalUriGetBaseValue(result);
            string fragment = UriParsingHelper.InternalUriGetFragment(result);

            Assert.AreEqual<string>("Result.xaml", uriBase);
            Assert.IsFalse(queryStringValues.ContainsKey("id"));
            Assert.IsFalse(queryStringValues.ContainsKey("foo"));
            Assert.IsTrue(queryStringValues.ContainsKey("x") && queryStringValues["x"] == "10");
            Assert.IsTrue(queryStringValues.ContainsKey("myVar2") && queryStringValues["myVar2"] == "6");
            Assert.AreEqual("fragment1", fragment);
        }

        [TestMethod]
        [Description("If a fragment is present in the 'MappedUri' and a different fragment is present in the uri being navigated to, the one in the original Uri should win")]
        public void FragmentInOriginalUriWins()
        {
            string UriPattern = "Simple/{id}/{foo}";
            string MappedUriPattern = "Result.xaml?{foo}2=6#fragment1";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/123/myVar?x=10#fragment2", UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            IDictionary<string, string> queryStringValues = UriParsingHelper.InternalUriParseQueryStringToDictionary(result, false /* decodeResults */);
            string uriBase = UriParsingHelper.InternalUriGetBaseValue(result);
            string fragment = UriParsingHelper.InternalUriGetFragment(result);

            Assert.AreEqual<string>("Result.xaml", uriBase);
            Assert.IsFalse(queryStringValues.ContainsKey("id"));
            Assert.IsFalse(queryStringValues.ContainsKey("foo"));
            Assert.IsTrue(queryStringValues.ContainsKey("x") && queryStringValues["x"] == "10");
            Assert.IsTrue(queryStringValues.ContainsKey("myVar2") && queryStringValues["myVar2"] == "6");
            Assert.AreEqual("fragment2", fragment);
        }

        [TestMethod]
        [Description("If a fragment contains an identifier, it gets replaced correctly.")]
        public void FragmentWithIdentifier()
        {
            string UriPattern = "Simple/{id}/{foo}";
            string MappedUriPattern = "Result.xaml?{foo}2=6#fragment{id}";
            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple/123/myVar?x=10", UriKind.Relative);

            Uri result = alias.MapUri(testUri);

            Assert.IsNotNull(result);
            IDictionary<string, string> queryStringValues = UriParsingHelper.InternalUriParseQueryStringToDictionary(result, false /* decodeResults */);
            string uriBase = UriParsingHelper.InternalUriGetBaseValue(result);
            string fragment = UriParsingHelper.InternalUriGetFragment(result);

            Assert.AreEqual<string>("Result.xaml", uriBase);
            Assert.IsFalse(queryStringValues.ContainsKey("id"));
            Assert.IsFalse(queryStringValues.ContainsKey("foo"));
            Assert.IsTrue(queryStringValues.ContainsKey("x") && queryStringValues["x"] == "10");
            Assert.IsTrue(queryStringValues.ContainsKey("myVar2") && queryStringValues["myVar2"] == "6");
            Assert.AreEqual("fragment123", fragment);
        }

        [TestMethod]
        [Description("If a fragment is the only thing present in the 'MappedUri' template then we throw on MapUri")]
        public void OnlyFragmentInMappedUriTemplateThrows()
        {
            string UriPattern = "Simple";
            string MappedUriPattern = "#Result";

            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple", UriKind.Relative);

            try
            {
                Uri result = alias.MapUri(testUri);

                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // This is expected
            }
            catch (Exception)
            {
                Assert.Fail("Exception other than InvalidOperationException thrown");
            }
        }

        [TestMethod]
        [Description("If a query string is the only thing present in the 'MappedUri' template then we throw on MapUri")]
        public void OnlyQueryStringInMappedUriTemplateThrows()
        {
            string UriPattern = "Simple";
            string MappedUriPattern = "?Result=1";

            UriMapping alias = new UriMapping();
            alias.Uri = new Uri(UriPattern, UriKind.Relative);
            alias.MappedUri = new Uri(MappedUriPattern, UriKind.Relative);

            Uri testUri = new Uri("Simple", UriKind.Relative);

            try
            {
                Uri result = alias.MapUri(testUri);

                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                // This is expected
            }
            catch (Exception)
            {
                Assert.Fail("Exception other than InvalidOperationException thrown");
            }
        }

        #region Globalization test methods

        private static void GlobalizedTestCore(Uri uri, Uri mappedUri, Uri inputUri, Uri expectedOutputUri)
        {
            UriMapping mapping = new UriMapping();
            mapping.Uri = uri;
            mapping.MappedUri = mappedUri;

            Uri outputUri = mapping.MapUri(inputUri);

            Assert.AreEqual(expectedOutputUri, outputUri);
        }

        [TestMethod]
        [Description("Use a globalized string in a query string value")]
        public void GlobalizedQueryStringValue()
        {
            Uri uri = new Uri("Simple/{id}", UriKind.Relative);
            Uri mappedUri = new Uri("Result?id={id}", UriKind.Relative);
            Uri inputUri = new Uri("Simple/完全采用统", UriKind.Relative);
            Uri expectedOutputUri = new Uri("Result?id=完全采用统", UriKind.Relative);

            GlobalizedTestCore(uri, mappedUri, inputUri, expectedOutputUri);
        }

        [TestMethod]
        [Description("Use a globalized string in a query string key")]
        public void GlobalizedQueryStringKey()
        {
            Uri uri = new Uri("Simple/{id}", UriKind.Relative);
            Uri mappedUri = new Uri("Result?{id}=123", UriKind.Relative);
            Uri inputUri = new Uri("Simple/完全采用统", UriKind.Relative);
            Uri expectedOutputUri = new Uri("Result?完全采用统=123", UriKind.Relative);

            GlobalizedTestCore(uri, mappedUri, inputUri, expectedOutputUri);
        }

        [TestMethod]
        [Description("Use a globalized string in an identifire")]
        public void GlobalizedIdentifier()
        {
            Uri uri = new Uri("Simple/{完全采用统}", UriKind.Relative);
            Uri mappedUri = new Uri("Result?id={完全采用统}", UriKind.Relative);
            Uri inputUri = new Uri("Simple/123", UriKind.Relative);
            Uri expectedOutputUri = new Uri("Result?id=123", UriKind.Relative);

            GlobalizedTestCore(uri, mappedUri, inputUri, expectedOutputUri);
        }

        [TestMethod]
        [Description("Use a globalized string in the fragment")]
        public void GlobalizedFragment()
        {
            Uri uri = new Uri("Simple/{id}", UriKind.Relative);
            Uri mappedUri = new Uri("Result#abc-{id}", UriKind.Relative);
            Uri inputUri = new Uri("Simple/完全采用统", UriKind.Relative);
            Uri expectedOutputUri = new Uri("Result#abc-完全采用统", UriKind.Relative);

            GlobalizedTestCore(uri, mappedUri, inputUri, expectedOutputUri);
        }

        [TestMethod]
        [Description("Use a globalized string in the path")]
        public void GlobalizedPath()
        {
            Uri uri = new Uri("Simple/{id}", UriKind.Relative);
            Uri mappedUri = new Uri("完全采用统?id={id}", UriKind.Relative);
            Uri inputUri = new Uri("Simple/123", UriKind.Relative);
            Uri expectedOutputUri = new Uri("完全采用统?id=123", UriKind.Relative);

            GlobalizedTestCore(uri, mappedUri, inputUri, expectedOutputUri);
        }

        #endregion Globalization test methods
    }
}
