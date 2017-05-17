// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// Generic unit test harness event arguments base class that contains a
    /// reference to the harness.
    /// </summary>
    public class UnitTestHarnessEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the UnitTestHarnessEventArgs class.
        /// </summary>
        /// <param name="harness">The test harness.</param>
        public UnitTestHarnessEventArgs(UnitTestHarness harness)
            : base()
        {
            UnitTestHarness = harness;
        }

        /// <summary>
        /// Gets the unit test harness reference.
        /// </summary>
        public UnitTestHarness UnitTestHarness { get; private set; }
    }
}