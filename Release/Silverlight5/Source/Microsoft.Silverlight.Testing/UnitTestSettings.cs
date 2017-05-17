// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Silverlight.Testing.Harness;

namespace Microsoft.Silverlight.Testing
{
    /// <summary>
    /// Settings for the unit test system.
    /// </summary>
    public class UnitTestSettings
    {
        /// <summary>
        /// The tag expression key name.
        /// </summary>
        private const string TagExpressionKey = "tagExpression";

        /// <summary>
        /// By default test methods are sorted.
        /// </summary>
        private const bool DefaultSortTestMethods = true;

        /// <summary>
        /// By default test classes are sorted.
        /// </summary>
        private const bool DefaultSortTestClasses = true;

        /// <summary>
        /// Gets the parameters from the response file.
        /// </summary>
        /// <value>The parameters.</value>
        public IDictionary<string, string> Parameters { get; private set; }

        /// <summary>
        /// Gets the components initialized by the entry-point assembly. These
        /// are the dynamically loaded objects that may be needed by the
        /// TestHarness.
        /// </summary>
        /// <value>The components.</value>
        public IDictionary<string, object> Components { get; private set; }

        /// <summary>
        /// Gets the log providers.
        /// </summary>
        public IList<LogProvider> LogProviders { get; private set; }

        /// <summary>
        /// Gets or sets a set of sample tags for use in a tag editor screen.
        /// </summary>
        public IList<string> SampleTags { get; set; }

        /// <summary>
        /// Gets the list of test assemblies.
        /// </summary>
        /// <value>The test assembly.</value>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Leaving open for extensibility purposes")]
        public IList<Assembly> TestAssemblies { get; private set; }

        /// <summary>
        /// Backing field for the test service.
        /// </summary>
        private TestServiceProvider _testService;

        /// <summary>
        /// Gets or sets a value indicating whether the test service has been
        /// directly set.
        /// </summary>
        internal bool TestServiceSetterCalled { get; set; }

        /// <summary>
        /// Gets or sets the test service provider.  The test service lights up 
        /// advanced out-of-process communication, reporting, logging, and 
        /// other valuable services.
        /// </summary>
        public TestServiceProvider TestService
        {
            get { return _testService; }
            set
            {
                _testService = value;
                TestServiceSetterCalled = true;
            }
        }

        /// <summary>
        /// Gets or sets the test harness.
        /// </summary>
        /// <value>The test harness.</value>
        public UnitTestHarness TestHarness { get; set; }

        /// <summary>
        /// Gets or sets the test service hostname to try using. Defaults to
        /// localhost.
        /// </summary>
        public string TestServiceHostname { get; set; }

        /// <summary>
        /// Gets or sets the test service port to try using. Defaults to 8000.
        /// </summary>
        public int TestServicePort { get; set; }

        /// <summary>
        /// Gets or sets the test service path to try using. Defaults to
        /// /externalInterface/.
        /// </summary>
        public string TestServicePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to start the run
        /// immediately when the test system is run. Defaults to false to enable
        /// the test service to load and information to be provided in the
        /// user interface.
        /// </summary>
        public bool StartRunImmediately { get; set; }

        /// <summary>
        /// Settings for the unit test system.
        /// </summary>
        public UnitTestSettings()
        {
            SampleTags = new List<string>();
            Components = new Dictionary<string, object>();
            LogProviders = new List<LogProvider>();
            Parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            TestAssemblies = new List<Assembly>();
            TestService = new TestServiceProvider();
            SortTestMethods = DefaultSortTestMethods;
            SortTestClasses = DefaultSortTestClasses;
            TestClassesToRun = new Collection<string>();
            ShowTagExpressionEditor = true;

            // Service defaults
            TestServiceHostname = "localhost";
            TestServicePath = "/externalInterface/";
            TestServicePort = 8000;
        }

        /// <summary>
        /// Gets or sets the type of the TestPanel to create. The type must
        /// derive from Microsoft.Silverlight.Testing.ITestPanel.
        /// </summary>
        public Type TestPanelType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether test methods are sorted 
        /// alphabetically. By default this value is true.
        /// </summary>
        /// <remarks>
        /// It is worth understanding that the order of unit test 
        /// execution should not affect the results of a test run.  Any expected
        /// ordering and verification from multiple test methods should be 
        /// refactored into a single unit test.
        /// </remarks>
        public bool SortTestMethods { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether test classes are sorted 
        /// alphabetically. This setting is True by default.
        /// </summary>
        public bool SortTestClasses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the tag expression
        /// editor user interface before starting the test run.
        /// </summary>
        public bool ShowTagExpressionEditor { get; set; }

        /// <summary>
        /// Gets or sets the tag expression used for selecting tests to run. 
        /// </summary>
        public string TagExpression
        {
            get 
            { 
                string value;
                if (Parameters.TryGetValue(TagExpressionKey, out value))
                {
                    return value;
                }

                return null;
            }

            set { Parameters[TagExpressionKey] = value; }
        }

        /// <summary>
        /// Gets a list of test classes to run. Enables filtering.
        /// </summary>
        /// <remarks>This property should be considered obsolete.</remarks>
        public Collection<string> TestClassesToRun { get; private set; }
    }
}