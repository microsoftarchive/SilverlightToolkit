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
    public interface IMobileTestPage
    {
        /// <summary>
        /// Requests navigation back a page.
        /// </summary>
        /// <returns>A value indicating whether the operation was successful.</returns>
        bool NavigateBack();
    }
}