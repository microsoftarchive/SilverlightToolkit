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
    public class TestMethodCompletedEventArgs : UnitTestHarnessEventArgs
    {
        /// <summary>
        /// Gets the test method result.
        /// </summary>
        public ScenarioResult Result { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestMethodCompletedEventArgs
        /// type.
        /// </summary>
        /// <param name="result">The result instance.</param>
        public TestMethodCompletedEventArgs(ScenarioResult result)
            : this(result, result.TestClass.Assembly.TestHarness)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestMethodCompletedEventArgs
        /// type.
        /// </summary>
        /// <param name="result">The result instance.</param>
        /// <param name="harness">The unit test harness.</param>
        public TestMethodCompletedEventArgs(ScenarioResult result, UnitTestHarness harness)
            : base(harness)
        {
            Result = result;
        }
    }
}