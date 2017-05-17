// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Silverlight.Testing.Harness;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

namespace Microsoft.Silverlight.Testing
{
    /// <summary>
    /// A central entry point for unit test projects and applications.
    /// </summary>
    public partial class UnitTestSystem
    {
        /// <summary>
        /// Friendly unit test system name.
        /// </summary>
        private const string UnitTestSystemName = "Silverlight Unit Test Framework";

        /// <summary>
        /// Gets the test system name built into the assembly.
        /// </summary>
        public static string SystemName { get { return UnitTestSystemName; } }

        /// <summary>
        /// Gets a string representing the file version attribute of the main
        /// unit test framework assembly, if present.
        /// </summary>
        public static string FrameworkFileVersion
        {
            get
            {
                string version = "Unknown";
                Assembly utf = typeof(UnitTestSystem).Assembly;
                AssemblyFileVersionAttribute afva;
                if (utf.TryGetAssemblyAttribute(out afva))
                {
                    version = afva.Version;
                }
                return version;
            }
        }

        /// <summary>
        /// Register another available unit test provider for the unit test system.
        /// </summary>
        /// <param name="provider">A unit test provider.</param>
        public static void RegisterUnitTestProvider(IUnitTestProvider provider)
        {
            if (!UnitTestProviders.Providers.Contains(provider))
            {
                UnitTestProviders.Providers.Add(provider);
            }
        }

        /// <summary>
        /// Test harness instance.
        /// </summary>
        private UnitTestHarness _harness;

        /// <summary>
        /// Start a new unit test run.
        /// </summary>
        /// <param name="settings">Unit test settings object.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This makes the purpose clear to test developers")]
        public void Run(UnitTestSettings settings)
        {
            // Avoid having the Run method called twice
            if (_harness != null)
            {
                return;
            }

            _harness = settings.TestHarness;
            if (_harness == null)
            {
                throw new InvalidOperationException(Properties.UnitTestMessage.UnitTestSystem_Run_NoTestHarnessInSettings);
            }

            if (settings.TestService == null && !settings.TestServiceSetterCalled)
            {
                SetTestService(settings);
            }

            _harness.Settings = settings;
            _harness.TestHarnessCompleted += (sender, args) => OnTestHarnessCompleted(args);

            if (settings.StartRunImmediately)
            {
                _harness.Run();
            }
        }

        /// <summary>
        /// Prepares the default log manager.
        /// </summary>
        /// <param name="settings">The test harness settings.</param>
        public static void SetStandardLogProviders(UnitTestSettings settings)
        {
            // Debug provider
            DebugOutputProvider debugger = new DebugOutputProvider();
            debugger.ShowAllFailures = true;
            settings.LogProviders.Add(debugger);

            // Visual Studio log provider
            try
            {
                TryAddVisualStudioLogProvider(settings);
            }
            catch
            {
            }

            PrepareCustomLogProviders(settings);
        }

        /// <summary>
        /// Tries to instantiate and initialize a VSTT provider. Requires that 
        /// XLinq is available and included in the application package.
        /// </summary>
        /// <param name="settings">The test harness settings object.</param>
        private static void TryAddVisualStudioLogProvider(UnitTestSettings settings)
        {
            VisualStudioLogProvider trx = new VisualStudioLogProvider();
            settings.LogProviders.Add(trx);
        }

        /// <summary>
        /// Creates the default settings that would be used by the UnitTestHarness
        /// if none were specified.
        /// </summary>
        /// <returns>A new RootVisual.</returns>
        /// <remarks>Assumes the calling assembly is a test assembly.</remarks>
        public static UnitTestSettings CreateDefaultSettings()
        {
            return CreateDefaultSettings(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// A completed test harness handler.
        /// </summary>
        public event EventHandler<TestHarnessCompletedEventArgs> TestHarnessCompleted;
        
        /// <summary>
        /// Call the TestHarnessCompleted event.
        /// </summary>
        /// <param name="args">The test harness completed event arguments.</param>
        private void OnTestHarnessCompleted(TestHarnessCompletedEventArgs args)
        {
            var handler = TestHarnessCompleted;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Create a default settings object for unit testing.
        /// </summary>
        /// <param name="callingAssembly">The assembly reflection object.</param>
        /// <returns>A unit test settings instance.</returns>
        private static UnitTestSettings CreateDefaultSettings(Assembly callingAssembly)
        {
            UnitTestSettings settings = new UnitTestSettings();
            if (callingAssembly != null)
            {
                settings.TestAssemblies.Add(callingAssembly);
            }
            SetStandardLogProviders(settings);
            settings.TestHarness = new UnitTestHarness();
            // Sets initial but user can override
            SetTestService(settings);
            return settings;
        }
    }
}