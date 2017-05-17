// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
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
    public class CodeCoverageProvider : ProviderBase
    {
        /// <summary>
        /// Initializes a new code coverage provider.
        /// </summary>
        /// <param name="testService">The test service.</param>
        public CodeCoverageProvider(TestServiceProvider testService)
            : base(testService, "CodeCoverage")
        {
        }

        /// <summary>
        /// Save string-based code coverage data.
        /// </summary>
        /// <param name="data">The code coverage data, as a string.</param>
        /// <param name="callback">The callback action.</param>
        public virtual void SaveCoverageData(string data, Action<ServiceResult> callback)
        {
            Callback(callback, ServiceResult.CreateExceptionalResult(new NotSupportedException("This feature is not currently supported.")));
        }
    }
}