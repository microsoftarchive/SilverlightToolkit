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
        /// Gets the service type that is in use.
        /// </summary>
        public ServiceType ServiceType { get; private set; }

        /// <summary>
        /// The service verifier and information.
        /// </summary>
        private ServiceVerifier _webService;

        /// <summary>
        /// Gets the web service proxy.
        /// </summary>
        public WebTestService WebService { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SilverlightTestService class.
        /// </summary>
        public SilverlightTestService() : base()
        {
            if (Application.Current.Host != null && Application.Current.Host.Source != null)
            {
                Uri uri = Application.Current.Host.Source;
                switch (uri.Scheme)
                {
                    case "file":
                        ServiceType = ServiceType.Direct;
                        break;

                    case "http":
                        ServiceType = ServiceType.WebService;
                        SetServicePath(null);
                        SetCustomIdentification();
                        break;

                    // Default: unknowns plus https://
                    default:
                        ServiceType = ServiceType.None;
                        // TODO: Provide a non-fatal warning here
                        break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the SilverlightTestService class.
        /// </summary>
        /// <param name="settings">Unit test settings to read the service path
        /// and other information from.</param>
        public SilverlightTestService(UnitTestSettings settings)
            : this()
        {
            if (ServiceType == ServiceType.WebService)
            {
                SetServicePath(settings);
            }
        }

        /// <summary>
        /// Initializes the Silverlight test service.  Performs a service check 
        /// if needed before initializing the other providers.
        /// </summary>
        public override void Initialize()
        {
            if (ServiceType.WebService == ServiceType)
            {
                AttemptServiceConnection();
            }
            else
            {
                ContinueInitialization();
            }
        }

        /// <summary>
        /// Sets the custom ID information for the test run, if passed into 
        /// the run.
        /// </summary>
        public void SetCustomIdentification()
        {
            string testRun = "test_run";
            char[] seps = { '&', '?' };

            // Set randmon Guid as test run id in case we can not get ID from test service.
            UniqueTestRunIdentifier = Guid.NewGuid().ToString();

            if (Application.Current != null && Application.Current.Host != null && Application.Current.Host.Source != null)
            {
                Uri page = Application.Current.Host.Source;
                if (page.Query.Contains(testRun))
                {
                    string rest = page.Query.Substring(page.Query.IndexOf(testRun, StringComparison.OrdinalIgnoreCase) + testRun.Length + 1);
                    int after = rest.IndexOfAny(seps);
                    if (after >= 0)
                    {
                        rest = rest.Substring(0, after);
                    }
                    UniqueTestRunIdentifier = rest;
                }
            }
        }

        /// <summary>
        /// Determine the service path to attempt to use, and prepares the 
        /// verification object using those parameters.
        /// </summary>
        /// <param name="settings">Unit test settings object to try and read
        /// settings from.</param>
        private void SetServicePath(UnitTestSettings settings)
        {
            bool useOverrides = (settings != null && !string.IsNullOrEmpty(settings.TestServiceHostname) && !string.IsNullOrEmpty(settings.TestServicePath));
            _webService = new ServiceVerifier
            {
               Hostname = useOverrides ? settings.TestServiceHostname : "localhost",
               Port = useOverrides ? settings.TestServicePort : 8000,
               ServicePath = useOverrides ? settings.TestServicePath : "/externalInterface/",
            };
        }

        /// <summary>
        /// Pauses the initialization process to attempt a service connection. 
        /// The result will alter the underlying ServiceType being used by 
        /// this provider to ensure a fallback experience can be used.  
        /// 
        /// This verification step will block the initialization and entire 
        /// test run until it continues.
        /// </summary>
        private void AttemptServiceConnection()
        {
            _webService.Verify(
                // Success
                delegate
                {
                    WebService = new WebTestService(_webService.ServiceUri);
                    ContinueInitialization();
                },
                // Failure, fallback to the direct mode
                delegate
                {
                    ServiceType = ServiceType.Direct;
                    _webService = null;
                    ContinueInitialization();
                });
        }

        /// <summary>
        /// Continues the initialization process for the test service provider.
        /// </summary>
        private void ContinueInitialization()
        {
            PopulateProviders();

            // Base initializes the service providers and then fires off the 
            // complete event
            base.Initialize();
        }

        /// <summary>
        /// Populates with the standard providers for Silverlight in-browser 
        /// testing.
        /// </summary>
        private void PopulateProviders()
        {
            if (ServiceType.Direct == ServiceType)
            {
                // Settings provider
                IsolatedStorageSettingsProvider settings = new IsolatedStorageSettingsProvider(this);
                RegisterService(TestServiceFeature.RunSettings, settings);
            }

            if (ServiceType.WebService == ServiceType)
            {
                // Command line run settings provider
                SettingsProvider settings = new WebSettingsProvider(this);
                RegisterService(TestServiceFeature.RunSettings, settings);

                // Code coverage provider
                CodeCoverageProvider coverage = new WebCodeCoverageProvider(this);
                RegisterService(TestServiceFeature.CodeCoverageReporting, coverage);

                // Reporting provider
                TestReportingProvider reporting = new WebTestReportingProvider(this);
                RegisterService(TestServiceFeature.TestReporting, reporting);

                // Environment provider
                EnvironmentProvider environment = new WebEnvironmentProvider(this);
                RegisterService(TestServiceFeature.EnvironmentServices, environment);
            }
        }
    }
}