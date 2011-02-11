// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// The test class completed event arguments.
    /// </summary>
    public class TestClassCompletedEventArgs : UnitTestHarnessEventArgs
    {
        /// <summary>
        /// Gets the test class metadata.
        /// </summary>
        public ITestClass TestClass { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestClassCompletedEventArgs
        /// class.
        /// </summary>
        /// <param name="testClass">Test class metadata.</param>
        /// <param name="harness">The harness instance.</param>
        public TestClassCompletedEventArgs(ITestClass testClass, UnitTestHarness harness)
            : base(harness)
        {
            TestClass = testClass;
        }
    }
}