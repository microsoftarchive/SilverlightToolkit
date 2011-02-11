// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;

namespace Microsoft.Silverlight.Testing
{
    /// <summary>
    /// An interface for any test page instances to implement.
    /// </summary>
    public interface ITestPage
    {
        /// <summary>
        /// Gets the test panel instance.
        /// </summary>
        Panel TestPanel { get; }
    }
}