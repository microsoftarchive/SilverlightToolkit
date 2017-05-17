// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows;
using Microsoft.Silverlight.Testing.Harness;

// NOTE: This entire namespace and its implementation is likely to be replaced
// in the future.

namespace Microsoft.Silverlight.Testing.Service
{
    /// <summary>
    /// The Silverlight test service provider is built for compilation with 
    /// Silverlight builds of the test framework.  Populates with the important 
    /// providers for web browser-hosted test runs.
    /// </summary>
    public partial class SilverlightTestService : TestServiceProvider
    {
        /// <summary>
        /// A special verification class used by SilverlightTestService.
        /// </summary>
        private class ServiceVerifier
        {        
            /// <summary>
            /// The name of a simple 'ping' method exposed by the service.
            /// </summary>
            private const string VerificationServiceName = "ping";

            /// <summary>
            /// Gets or sets the service hostname.
            /// </summary>
            public string Hostname { get; set; }
            
            /// <summary>
            /// Gets or sets the service port.
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// Gets or sets path to the simple POX service.
            /// </summary>
            public string ServicePath { get; set; }

            /// <summary>
            /// Gets the URI to the service.
            /// </summary>
            public Uri ServiceUri
            {
                get
                {
                    string scheme = "http://";
                    if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Source != null)
                    {
                        // TODO: Need to understand better the cross-security
                        // scenarios that customers are using for the service,
                        // since they would need a custom service hosted on a
                        // server for this to work. For now, we force to the
                        // same URI but on the https:// scheme in those 
                        // situations.
                        if (Application.Current.Host.Source.Scheme == "https")
                        {
                            scheme = "https://";
                        }
                    }
                    return new Uri(scheme + Hostname + ":" + Port + ServicePath);
                }
            }

            /// <summary>
            /// Attempts to verify the service connection.  Calls the proper 
            /// success/failure Action once a verification result is possible.
            /// </summary>
            /// <param name="success">The Action to call upon connection 
            /// verification.</param>
            /// <param name="failure">An Action to call upon failure.</param>
            public void Verify(Action success, Action failure)
            {
                WebTestService pox = new WebTestService(ServiceUri);
                pox.CallMethod(
                    VerificationServiceName,
                    delegate(ServiceResult result)
                    {
                        Action action = result != null && result.Successful ? success : failure;
                        action();
                    });
            }
        }
    }
}