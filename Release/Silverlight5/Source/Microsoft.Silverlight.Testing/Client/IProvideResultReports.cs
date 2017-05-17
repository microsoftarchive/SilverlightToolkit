// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A type that provides a string result report.
    /// </summary>
    public interface IProvideResultReports
    {
        /// <summary>
        /// Generates a simple text result report for the metadata.
        /// </summary>
        /// <returns>Reports a text report.</returns>
        string GetResultReport();
    }
}