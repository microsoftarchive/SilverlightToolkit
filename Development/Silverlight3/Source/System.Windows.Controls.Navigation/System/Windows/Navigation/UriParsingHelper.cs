//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Browser;

namespace System.Windows.Navigation
{
    internal static class UriParsingHelper
    {
        #region Static fields and constants

        private const string QueryStringDelimiter = "?";
        private const string ValueDelimiter = "=";
        private const string StatePairDelimiter = "&";
        private const string FragmentDelimiter = "#";
        private static readonly char[] externalQueryStringDelimiter = new char[] { ';' };
        private static readonly char[] externalFragmentDelimiter = new char[] { '$' };
        private const string ExternalQueryStringDelimiterPercentEncoded = "%3B";
        private const string ExternalFragmentDelimiterPercentEncoded = "%24";

        #endregion

        #region Methods
                
        #region Methods acting on internal Uris

        internal static Uri MakeAbsolute(Uri baseUri)
        {
            if (baseUri == null || baseUri.OriginalString.StartsWith("/",StringComparison.Ordinal))
            {
                return new Uri("http://localhost" + baseUri, UriKind.Absolute);
            }
            else
            {
                return new Uri("http://localhost/" + baseUri, UriKind.Absolute);
            }
        }

        internal static string InternalUriToExternalValue(Uri uri)
        {
            Uri absoluteUri = MakeAbsolute(uri);
            
            UriComponents pathComponents = UriComponents.Path;
            if (uri != null && uri.OriginalString.StartsWith("/", StringComparison.Ordinal))
            {
                pathComponents |= UriComponents.KeepDelimiter;
            }

            string path = absoluteUri.GetComponents(pathComponents, UriFormat.UriEscaped)
                                     .Replace(externalQueryStringDelimiter[0].ToString(), ExternalQueryStringDelimiterPercentEncoded)
                                     .Replace(externalFragmentDelimiter[0].ToString(), ExternalFragmentDelimiterPercentEncoded);

            string query = absoluteUri.GetComponents(UriComponents.Query, UriFormat.UriEscaped)
                                      .Replace(ValueDelimiter[0],'/')
                                      .Replace(StatePairDelimiter[0],'/')
                                      .Replace(externalQueryStringDelimiter[0].ToString(), ExternalQueryStringDelimiterPercentEncoded)
                                      .Replace(externalFragmentDelimiter[0].ToString(), ExternalFragmentDelimiterPercentEncoded);

            string fragment = absoluteUri.GetComponents(UriComponents.Fragment, UriFormat.UriEscaped)
                                         .Replace(externalQueryStringDelimiter[0].ToString(), ExternalQueryStringDelimiterPercentEncoded)
                                         .Replace(externalFragmentDelimiter[0].ToString(), ExternalFragmentDelimiterPercentEncoded);

            string final = path;
            if (!String.IsNullOrEmpty(query))
            {
                final += externalQueryStringDelimiter[0] + query;
            }
            if (!String.IsNullOrEmpty(fragment))
            {
                final += externalFragmentDelimiter[0] + fragment;
            }

            return final;
        }

        internal static string InternalUriFromExternalValue(string externalVal)
        {
            string path = String.Empty,
                   query = String.Empty,
                   fragment = String.Empty;

            string[] queryWithFragmentParts = externalVal.Split(externalQueryStringDelimiter, StringSplitOptions.RemoveEmptyEntries);
            if (queryWithFragmentParts.Length > 0)
            {
                string[] pathParts = queryWithFragmentParts[0].Split(externalFragmentDelimiter, StringSplitOptions.RemoveEmptyEntries);
                path = pathParts[0].Replace(ExternalQueryStringDelimiterPercentEncoded, externalQueryStringDelimiter[0].ToString())
                                   .Replace(ExternalFragmentDelimiterPercentEncoded, externalFragmentDelimiter[0].ToString());
            }
            if (queryWithFragmentParts.Length > 1)
            {
                string[] queryParts = queryWithFragmentParts[1].Split(externalFragmentDelimiter, StringSplitOptions.RemoveEmptyEntries);
                string queryPart = queryParts[0];
                string[] kvps = queryPart.Split(new char[] { '/' });
                List<string> queryPairs = new List<string>();
                for (int i = 0; i < kvps.Length; i+=2)
                {
                    if (i < kvps.Length - 1)
                    {
                        queryPairs.Add(kvps[i] + ValueDelimiter + kvps[i + 1]);
                    }
                    else
                    {
                        queryPairs.Add(kvps[i] + ValueDelimiter);
                    }
                }
                query = String.Join(StatePairDelimiter, queryPairs.ToArray())
                              .Replace(ExternalQueryStringDelimiterPercentEncoded, externalQueryStringDelimiter[0].ToString())
                              .Replace(ExternalFragmentDelimiterPercentEncoded, externalFragmentDelimiter[0].ToString());
            }
            int fragmentIndex = externalVal.IndexOf(externalFragmentDelimiter[0]);
            if (fragmentIndex != -1)
            {
                // add 1 to the index to avoid getting the "$" in the resulting fragment
                fragment = externalVal.Substring(fragmentIndex + 1)
                                      .Replace(ExternalQueryStringDelimiterPercentEncoded, externalQueryStringDelimiter[0].ToString())
                                      .Replace(ExternalFragmentDelimiterPercentEncoded, externalFragmentDelimiter[0].ToString());
            }

            string final = HttpUtility.UrlDecode(path);
            if (!String.IsNullOrEmpty(query))
            {
                final += HttpUtility.UrlDecode(QueryStringDelimiter + query);
            }
            if (!String.IsNullOrEmpty(fragment))
            {
                final += HttpUtility.UrlDecode(FragmentDelimiter + fragment);
            }

            return final;
        }

        internal static Uri InternalUriMerge(Uri baseUri, Uri newUri)
        {
            if ((baseUri != null && baseUri.OriginalString.StartsWith("/", StringComparison.Ordinal) && newUri.OriginalString.IndexOf(";component/", StringComparison.Ordinal) == -1) ||
                newUri.OriginalString.StartsWith("/", StringComparison.Ordinal))
            {
                return new Uri(new Uri(MakeAbsolute(baseUri), newUri).GetComponents(UriComponents.PathAndQuery | UriComponents.Fragment,
                                                                              UriFormat.SafeUnescaped),
                               UriKind.Relative);
            }
            else
            {
                return new Uri(new Uri(MakeAbsolute(baseUri), newUri).GetComponents(UriComponents.PathAndQuery | UriComponents.Fragment,
                                                                              UriFormat.SafeUnescaped).Substring(1),
                               UriKind.Relative);
            }
        }

        internal static bool InternalUriIsNavigable(Uri uri)
        {
            return uri != null &&
                   (InternalUriIsFragment(uri) ||
                    InternalUriIsRelativeToAppRoot(uri) ||
                    InternalUriIsRelativeWithComponent(uri) ||
                    String.IsNullOrEmpty(uri.OriginalString));
        }

        internal static bool InternalUriIsRelativeToAppRoot(Uri uri)
        {
            return !uri.IsAbsoluteUri && uri.OriginalString.StartsWith("/",StringComparison.Ordinal);
        }

        internal static bool InternalUriIsRelativeWithComponent(Uri uri)
        {
            return !uri.IsAbsoluteUri && uri.OriginalString.Contains(";component/");
        }

        /// <summary>
        /// Parses the Uri to determine if it is a fragment
        /// </summary>
        /// <param name="uri">The uri to parse</param>
        /// <returns>True if this Uri is a fragment, false if it is not</returns>
        internal static bool InternalUriIsFragment(Uri uri)
        {
            return uri != null &&
                   !uri.IsAbsoluteUri &&
                   !String.IsNullOrEmpty(uri.OriginalString) &&
                   uri.OriginalString.StartsWith(FragmentDelimiter, StringComparison.Ordinal);
        }

        /// <summary>
        /// Parses the Uri to retrieve the fragment, if present
        /// </summary>
        /// <param name="uri">The uri to parse</param>
        /// <returns>The fragment, or null if there is not one</returns>
        internal static string InternalUriGetFragment(Uri uri)
        {
            return MakeAbsolute(uri).GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
        }

        /// <summary>
        /// Parses the Uri to strip off the fragment
        /// </summary>
        /// <param name="uri">The uri to parse</param>
        /// <returns>The uri without the fragment</returns>
        internal static string InternalUriGetAllButFragment(Uri uri)
        {
            UriComponents components = UriComponents.PathAndQuery;

            if (uri != null && uri.OriginalString.StartsWith("/", StringComparison.Ordinal))
            {
                components |= UriComponents.KeepDelimiter;
            }

            return MakeAbsolute(uri).GetComponents(components, UriFormat.SafeUnescaped);
        }

        /// <summary>
        /// Parse the query string out of a Uri (the part following the '?')
        /// </summary>
        /// <param name="uri">The uri to parse for a query string</param>
        /// <returns>The query string, without a leading '?'.  Empty string in the case of no query string present.</returns>
        internal static string InternalUriGetQueryString(Uri uri)
        {
            return MakeAbsolute(uri).GetComponents(UriComponents.Query, UriFormat.Unescaped);
        }

        /// <summary>
        /// Cut the query string off a given Uri, to process only the part before the '?', and strips off the fragment
        /// </summary>
        /// <param name="uri">The uri to parse</param>
        /// <returns>The uri without its query string, and without its fragment</returns>
        internal static string InternalUriGetBaseValue(Uri uri)
        {
            UriComponents components = UriComponents.Path;

            if (uri.OriginalString.StartsWith("/", StringComparison.Ordinal))
            {
                components |= UriComponents.KeepDelimiter;
            }
            
            return MakeAbsolute(uri).GetComponents(components, UriFormat.SafeUnescaped);
        }

        /// <summary>
        /// Parses the query string into name/value pairs
        /// </summary>
        /// <param name="uri">The Uri to parse the query string from</param>
        /// <returns>A dictionary containing one entry for each name/value pair in the query string</returns>
        internal static IDictionary<string, string> InternalUriParseQueryStringToDictionary(Uri uri)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>(StringComparer.Ordinal);

            string[] kvps = InternalUriGetQueryString(uri).Split(StatePairDelimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string kvp in kvps)
            {
                int delimiterIndex = kvp.IndexOf(ValueDelimiter, StringComparison.Ordinal);
                if (delimiterIndex == -1)
                {
                    dict.Add(
                        HttpUtility.UrlDecode(kvp),
                        String.Empty);
                }
                else
                {
                    dict.Add(
                        HttpUtility.UrlDecode(kvp.Substring(0, delimiterIndex)),
                        HttpUtility.UrlDecode(kvp.Substring(delimiterIndex + 1)));
                }
            }

            return dict;
        }

        internal static Uri InternalUriCreateWithQueryStringValues(string uriBase, IDictionary<string, string> queryStringValues, string fragment)
        {
            StringBuilder sb = new StringBuilder(200);
            sb = sb.Append(uriBase);

            if (queryStringValues.Count > 0)
            {
                sb = sb.Append(QueryStringDelimiter);

                foreach (string key in queryStringValues.Keys)
                {
                    sb = sb.AppendFormat(CultureInfo.InvariantCulture,
                                         "{0}{1}{2}{3}",
                                         key,
                                         ValueDelimiter[0],
                                         //PercentEncode(queryStringValues[key], InternalUriValueChars),
                                         queryStringValues[key],
                                         StatePairDelimiter[0]);
                }

                // Strip off the last delimiter between internal state pairs
                sb = sb.Remove(sb.Length - 1, 1);
            }

            if (!String.IsNullOrEmpty(fragment))
            {
                sb.AppendFormat(CultureInfo.InvariantCulture,
                                "{0}{1}",
                                FragmentDelimiter,
                                fragment);
            }

            return new Uri(sb.ToString(), UriKind.Relative);
        }

        #endregion

        #endregion
    }
}
