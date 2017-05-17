// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// Test method starting event arguments.
    /// </summary>
    public class TestMethodStartingEventArgs : UnitTestHarnessEventArgs
    {
        /// <summary>
        /// Gets the test method metadata.
        /// </summary>
        public ITestMethod TestMethod { get; private set; }

        /// <summary>
        /// Gets the test class metadata.
        /// </summary>
        public ITestClass TestClass { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestMethodStartingEventArgs type.
        /// </summary>
        /// <param name="testMethod">The test method metadata.</param>
        /// <param name="testClass">The test class metadata.</param>
        /// <param name="harness">The test harness instance.</param>
        public TestMethodStartingEventArgs(ITestMethod testMethod, ITestClass testClass, UnitTestHarness harness) : base(harness)
        {
            TestMethod = testMethod;
            TestClass = testClass;
        }
    }
}