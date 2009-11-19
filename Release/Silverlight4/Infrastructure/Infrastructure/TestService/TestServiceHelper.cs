// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Net;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Set of helper methods for the test service.
    /// </summary>
    internal static class TestServiceHelper
    {
        /// <summary>
        /// The external interface token.
        /// </summary>
        internal const string ExternalInterface = "externalInterface";

        /// <summary>
        /// The URI format string used.
        /// </summary>
        private const string SimpleUriFormat = "http://{0}/";

        /// <summary>
        /// Pings the web service to help in shutdown.
        /// </summary>
        /// <param name="hostname">The hostname to use.</param>
        public static void PingService(string hostname)
        {
            WebClient wc = new WebClient();
            Uri uri = new Uri(string.Format(
                CultureInfo.InvariantCulture,
                SimpleUriFormat,
                hostname) + ExternalInterface + "/ping/");
            wc.DownloadString(uri);
        }

        /// <summary>
        /// A generic client access policy.
        /// </summary>
        public const string OpenClientAccessPolicy = @"<?xml version=""1.0"" encoding=""utf-8""?>
<access-policy>
  <cross-domain-access>
    <policy>
      <allow-from http-request-headers=""*"">
        <domain uri=""*""/>
      </allow-from>
      <grant-to>
        <resource path=""/"" include-subpaths=""true""/>
      </grant-to>
    </policy>
  </cross-domain-access>
</access-policy>";
    }
}