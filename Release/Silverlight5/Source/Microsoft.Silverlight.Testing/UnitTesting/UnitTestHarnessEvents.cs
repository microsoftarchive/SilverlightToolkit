// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// A helper utility for firing events as the unit test harness from any
    /// component, internal or not. Enables expansion.
    /// </summary>
    public class UnitTestHarnessEvents
    {
        /// <summary>
        /// Stored instance of the harness.
        /// </summary>
        private UnitTestHarness _harness;

        /// <summary>
        /// Initializes a new intance of the UnitTestHarnessEvents helper.
        /// </summary>
        /// <param name="harness">The harness reference.</param>
        public UnitTestHarnessEvents(UnitTestHarness harness)
        {
            if (harness == null)
            {
                throw new ArgumentNullException("harness");
            }

            _harness = harness;
        }

        /// <summary>
        /// Calls the test assembly starting event.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void SendTestAssemblyStarting(TestAssemblyStartingEventArgs e)
        {
            _harness.OnTestAssemblyStarting(e);
        }

        /// <summary>
        /// Calls the test assembly completed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void SendTestAssemblyCompleted(TestAssemblyCompletedEventArgs e)
        {
            _harness.OnTestAssemblyCompleted(e);
        }

        /// <summary>
        /// Calls the test class starting event handlers.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void SendTestClassStarting(TestClassStartingEventArgs e)
        {
            _harness.OnTestClassStarting(e);
        }

        /// <summary>
        /// Calls the test class completed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void SendTestClassCompleted(TestClassCompletedEventArgs e)
        {
            _harness.OnTestClassCompleted(e);
        }

        /// <summary>
        /// Calls the test method starting event.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void SendTestMethodStarting(TestMethodStartingEventArgs e)
        {
            _harness.OnTestMethodStarting(e);
        }

        /// <summary>
        /// Calls the test method completed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void SendTestMethodCompleted(TestMethodCompletedEventArgs e)
        {
            _harness.OnTestMethodCompleted(e);
        }

        /// <summary>
        /// Calls the test run starting event.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void SendTestRunStarting(TestRunStartingEventArgs e)
        {
            _harness.OnTestRunStarting(e);
        }
    }
}