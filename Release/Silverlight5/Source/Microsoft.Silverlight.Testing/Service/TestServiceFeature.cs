// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

// NOTE: This entire namespace and its implementation is likely to be replaced
// in the future.

namespace Microsoft.Silverlight.Testing.Service
{
    /// <summary>
    /// Set of known, well-defined test service features.
    /// </summary>
    public enum TestServiceFeature
    {
        /// <summary>
        /// Code coverage reporting.
        /// </summary>
        CodeCoverageReporting,

        /// <summary>
        /// Provides run parameters and settings.
        /// </summary>
        RunSettings,

        /// <summary>
        /// Provides test reporting services.
        /// </summary>
        TestReporting,

        /// <summary>
        /// Provides environment information.
        /// </summary>
        EnvironmentServices,
    }
}