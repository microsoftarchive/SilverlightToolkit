// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Web;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Set of helper methods used for the test service.
    /// </summary>
    internal static class ServiceHelper
    {
        /// <summary>
        /// Returns a simple REST Error response.
        /// </summary>
        /// <param name="value">The value to send.</param>
        /// <returns>Returns a string.</returns>
        public static string Error(string value)
        {
            return Response("fail", "<error msg=\"" + value + "\" />");
        }

        /// <summary>
        /// Returns an HTML decoded string.
        /// </summary>
        /// <param name="value">The value instance.</param>
        /// <returns>Returns a string.</returns>
        public static string HtmlDecode(string value)
        {
            return HttpUtility.HtmlDecode(value);
        }

        /// <summary>
        /// Returns an HTML encoded string.
        /// </summary>
        /// <param name="value">The value instance.</param>
        /// <returns>Returns a string.</returns>
        public static string HtmlEncode(string value)
        {
            return HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        /// Returns the OK response.
        /// </summary>
        /// <returns>Returns a string.</returns>
        public static string OK()
        {
            return OK(string.Empty);
        }

        /// <summary>
        /// Returns the OK response with a value.
        /// </summary>
        /// <param name="value">The value instance.</param>
        /// <returns>Returns a string.</returns>
        public static string OK(string value)
        {
            return Response("ok", value);
        }

        /// <summary>
        /// Returns a REST response.
        /// </summary>
        /// <param name="status">The status of the response.</param>
        /// <param name="value">The value instance.</param>
        /// <returns>Returns a string.</returns>
        public static string Response(string status, string value)
        {
            return ("<rsp stat=\"" + status + "\">" + value + "</rsp>");
        }

        /// <summary>
        /// URL decodes a string.
        /// </summary>
        /// <param name="value">The value instance.</param>
        /// <returns>Returns a string.</returns>
        public static string UrlDecode(string value)
        {
            return HttpUtility.UrlDecode(value);
        }

        /// <summary>
        /// URL encodes a string.
        /// </summary>
        /// <param name="value">The value instance.</param>
        /// <returns>Returns a string.</returns>
        public static string UrlEncode(string value)
        {
            return HttpUtility.UrlEncode(value);
        }
    }
}