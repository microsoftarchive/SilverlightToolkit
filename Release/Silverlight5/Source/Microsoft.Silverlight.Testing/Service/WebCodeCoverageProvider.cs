// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Microsoft.Silverlight.Testing.Harness;

// NOTE: This entire namespace and its implementation is likely to be replaced
// in the future.

namespace Microsoft.Silverlight.Testing.Service
{
    /// <summary>
    /// A provider of code coverage information to an external process.
    /// </summary>
    public class WebCodeCoverageProvider : CodeCoverageProvider
    {
        /// <summary>
        /// The MethodName_SaveCodeCoverage method name.
        /// </summary>
        private const string MethodName_SaveCodeCoverage = "saveCodeCoverageBase64";

        /// <summary>
        /// Initializes a new code coverage provider.
        /// </summary>
        /// <param name="testService">The test service.</param>
        public WebCodeCoverageProvider(TestServiceProvider testService) 
            : base(testService)
        {
        }

        /// <summary>
        /// Save string-based code coverage data.
        /// </summary>
        /// <param name="data">The code coverage data, as a string.</param>
        /// <param name="callback">The callback action.</param>
        public override void SaveCoverageData(string data, Action<ServiceResult> callback)
        {
            string guid = TestService.UniqueTestRunIdentifier;
            Dictionary<string, string> parameters = string.IsNullOrEmpty(guid) ? WebTestService.Dictionary() : WebTestService.Dictionary("guid", guid);
            IncrementBusyServiceCounter();
            ((SilverlightTestService)TestService).WebService.CallMethod(
                MethodName_SaveCodeCoverage,
                parameters,
                data,
                (sr) =>
                {
                    if (callback != null)
                    {
                        callback(sr);
                    }
                    DecrementBusyServiceCounter();
                });
        }
    }
}