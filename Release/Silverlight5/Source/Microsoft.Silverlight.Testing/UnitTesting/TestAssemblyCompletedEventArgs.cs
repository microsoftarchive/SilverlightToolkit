// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// Assembly complete event arguments.
    /// </summary>
    public class TestAssemblyCompletedEventArgs : UnitTestHarnessEventArgs
    {
        /// <summary>
        /// Gets the assembly metadata.
        /// </summary>
        public IAssembly Assembly { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestAssemblyCompletedEventArgs
        /// type.
        /// </summary>
        /// <param name="assembly">The assembly metadata.</param>
        /// <param name="harness">The test harness instance.</param>
        public TestAssemblyCompletedEventArgs(IAssembly assembly, UnitTestHarness harness)
            : base(harness)
        {
            Assembly = assembly;
        }
    }
}