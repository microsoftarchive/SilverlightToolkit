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
    /// A provider of environment variables and environmental information that 
    /// uses the test service provider infrastructure.
    /// </summary>
    public class EnvironmentProvider : ProviderBase
    {
        /// <summary>
        /// Initializes a new environment provider.
        /// </summary>
        /// <param name="testService">The test service.</param>
        public EnvironmentProvider(TestServiceProvider testService)
            : base(testService, "Environment")
        {
        }

        /// <summary>
        /// Retrieve an environment variable from the system.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <param name="callback">The callback action.</param>
        public virtual void GetEnvironmentVariable(string name, Action<ServiceResult> callback)
        {
            Callback(callback, ServiceResult.CreateExceptionalResult(new NotSupportedException("Environment is not yet implemented.")));
        }
    }
}