// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;

// NOTE: This entire namespace and its implementation is likely to be replaced
// in the future.

namespace Microsoft.Silverlight.Testing.Service
{
    /// <summary>
    /// Provides out-of-process access to operating system functions and other 
    /// services such as visual verification, if present.
    /// </summary>
    public partial class WebTestService
    {
        /// <summary>
        /// The request data class, stores information used in a request for 
        /// associating this data with the response.
        /// </summary>
        private class RequestData
        {
            /// <summary>
            /// Initializes a new request data object.
            /// </summary>
            /// <param name="uri">The request Uri.</param>
            /// <param name="request">The request object.</param>
            /// <param name="callback">The callback action.</param>
            public RequestData(Uri uri, HttpWebRequest request, Action<ServiceResult> callback)
            {
                Request = request;
                RequestUri = uri;
                Callback = callback;
            }

            /// <summary>
            /// Gets or sets the optional post data for the request.
            /// </summary>
            public string PostData { get; set; }

            /// <summary>
            /// Gets the web request.
            /// </summary>
            public HttpWebRequest Request { get; private set; }

            /// <summary>
            /// Gets the request Uri.
            /// </summary>
            public Uri RequestUri { get; private set; }

            /// <summary>
            /// Gets the callback action.
            /// </summary>
            public Action<ServiceResult> Callback { get; private set; }

            /// <summary>
            /// Converts the request data object into a web service result 
            /// object.
            /// </summary>
            /// <param name="response">The response object.</param>
            /// <returns>Returns a new WebServiceResult instance.</returns>
            public WebServiceResult ConvertToResult(WebResponse response)
            {
                return new WebServiceResult(Request, response, this);
            }
        }
    }
}