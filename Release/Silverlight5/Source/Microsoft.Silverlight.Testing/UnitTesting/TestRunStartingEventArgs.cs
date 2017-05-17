// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// Test method completed event arguments, contains the result.
    /// </summary>
    public class TestRunStartingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the TestRunStartingEventArgs
        /// type.
        /// </summary>
        /// <param name="settings">The test run filter.</param>
        /// <param name="filter">The test run settings object.</param>
        public TestRunStartingEventArgs(UnitTestSettings settings, TestRunFilter filter)
            : base()
        {
            Settings = settings;
            TestRunFilter = filter;
        }

        /// <summary>
        /// Gets the unit test run settings.
        /// </summary>
        public UnitTestSettings Settings { get; private set; }

        /// <summary>
        /// Gets the test run filter.
        /// </summary>
        public TestRunFilter TestRunFilter { get; private set; }

        /// <summary>
        /// Gets or sets the test harness name.
        /// </summary>
        public string TestHarnessName { get; set; }

        /// <summary>
        /// Gets or sets the number of valid, enqueued assemblies scheduled.
        /// </summary>
        public int EnqueuedAssemblies { get; set; }
    }
}