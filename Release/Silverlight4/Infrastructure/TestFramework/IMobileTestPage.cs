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
    public interface IMobileTestPage
    {
        /// <summary>
        /// Requests navigation back a page.
        /// </summary>
        /// <returns>A value indicating whether the operation was successful.</returns>
        bool NavigateBack();
    }
}