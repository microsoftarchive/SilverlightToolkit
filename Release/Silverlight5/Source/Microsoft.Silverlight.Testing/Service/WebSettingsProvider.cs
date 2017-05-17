// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Silverlight.Testing.Harness;

// NOTE: This entire namespace and its implementation is likely to be replaced
// in the future.

namespace Microsoft.Silverlight.Testing.Service
{
    /// <summary>
    /// A test service that reads command line settings.
    /// </summary>
    public class WebSettingsProvider : SettingsProvider
    {
        /// <summary>
        /// Name of the method MethodName_GetRunParameters.
        /// </summary>
        private const string MethodName_GetRunParameters = "getRunParameters";

        /// <summary>
        /// Initializes a new settings provider instance.
        /// </summary>
        /// <param name="testService">The test service.</param>
        public WebSettingsProvider(TestServiceProvider testService)
            : base(testService)
        {
        }

        /// <summary>
        /// Initialize the web settings provider.
        /// </summary>
        public override void Initialize()
        {
            string guid = TestService.UniqueTestRunIdentifier;
            if (string.IsNullOrEmpty(guid))
            {
                ReadRunParameters(null);
            }
            else
            {
                ((SilverlightTestService)TestService).WebService.CallMethod(MethodName_GetRunParameters, WebTestService.Dictionary("guid", guid), ReadRunParameters);
            }
        }

        /// <summary>
        /// Read the run parameters.
        /// </summary>
        /// <param name="result">The service result.</param>
        private void ReadRunParameters(ServiceResult result)
        {
            SimpleXElement xe = result == null ? null : result.TryGetElement();
            if (xe != null)
            {
                Dictionary<string, string> settings = xe.Descendants("option").ToTransformedDictionary((option) => option.Attribute("name"), (option) => option.Attribute("value"));
                foreach (string key in settings.Keys)
                {
                    Settings[key] = settings[key];
                }
            }

            // Allow the other services to initialize
            base.Initialize();
        }
    }
}