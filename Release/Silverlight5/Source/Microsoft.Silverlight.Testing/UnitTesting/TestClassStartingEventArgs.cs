// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// Information about the start of a test class event.
    /// </summary>
    public class TestClassStartingEventArgs : UnitTestHarnessEventArgs
    {
        /// <summary>
        /// Gets the test class instance.
        /// </summary>
        public ITestClass TestClass { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestClassStartingEventArgs type.
        /// </summary>
        /// <param name="testClass">The test class metadata.</param>
        /// <param name="harness">The unit test harness reference.</param>
        public TestClassStartingEventArgs(ITestClass testClass, UnitTestHarness harness)
            : base(harness)
        {
            TestClass = testClass;
        }
    }
}