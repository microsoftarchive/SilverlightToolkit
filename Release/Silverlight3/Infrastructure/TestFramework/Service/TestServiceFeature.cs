// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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