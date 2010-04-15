// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Stores the result of a test run.
    /// </summary>
    public class TestRunResult
    {
        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        public string Log { get; set; }

        /// <summary>
        /// Gets or sets the total number of executed scenarios.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the number of failures.
        /// </summary>
        public int Failures { get; set; }
    }
}