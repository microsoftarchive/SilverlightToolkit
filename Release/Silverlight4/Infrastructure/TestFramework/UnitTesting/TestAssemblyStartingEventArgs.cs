// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// Test assembly starting event arguments.
    /// </summary>
    public class TestAssemblyStartingEventArgs : UnitTestHarnessEventArgs
    {
        /// <summary>
        /// Gets the assembly metadata information.
        /// </summary>
        public IAssembly Assembly { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestAssemblyStartingEventArgs
        /// type.
        /// </summary>
        /// <param name="assembly">The assembly metadata.</param>
        /// <param name="harness">The unit test harness instance.</param>
        public TestAssemblyStartingEventArgs(IAssembly assembly, UnitTestHarness harness) : base(harness)
        {
            Assembly = assembly;
        }
    }
}