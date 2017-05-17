// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Silverlight.Testing.Harness;

// NOTE: This entire namespace and its implementation is likely to be replaced
// in the future.

namespace Microsoft.Silverlight.Testing.Service
{
    /// <summary>
    /// A provider of string dictionary values.
    /// </summary>
    public class SettingsProvider : ProviderBase
    {
        /// <summary>
        /// Initializes a new SettingsProvider object.
        /// </summary>
        /// <param name="testService">The test service.</param>
        public SettingsProvider(TestServiceProvider testService)
            : this(testService, "Settings")
        {
        }

        /// <summary>
        /// Initializes a new SettingsProvider object.
        /// </summary>
        /// <param name="testService">The test service.</param>
        /// <param name="serviceName">The service name.</param>
        public SettingsProvider(TestServiceProvider testService, string serviceName) : base(testService, serviceName)
        {
            Settings = new Dictionary<string, string>();
            IsReadOnly = true;
        }

        /// <summary>
        /// Gets the settings dictionary.
        /// </summary>
        public IDictionary<string, string> Settings { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the settings are read-only.
        /// </summary>
        public bool IsReadOnly { get; protected set; }

        /// <summary>
        /// Gets the settings source for end-user display.
        /// </summary>
        public string SourceName { get; protected set; }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <remarks>Classes that inherit from SettingsProvider: hide this 
        /// function.  Do not call up through to this base method.</remarks>
        /// <param name="callback">The service callback.</param>
        public virtual void SaveSettings(Action<ServiceResult> callback)
        {
            string message = IsReadOnly ? "Settings cannot be saved, they are read only." : "Save is not implemented.";
            Callback(callback, ServiceResult.CreateExceptionalResult(new NotSupportedException(message)));
        }
    }
}