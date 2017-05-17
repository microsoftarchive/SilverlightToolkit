// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// Interface for LogProviders that want access to external test settings.
    /// </summary>
    public interface ITestSettingsLogProvider 
    {
        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="settings">The settings.</param>
        void Initialize(UnitTestSettings settings);
    }
}