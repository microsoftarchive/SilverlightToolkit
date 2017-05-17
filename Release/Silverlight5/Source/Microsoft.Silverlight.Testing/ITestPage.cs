// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
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