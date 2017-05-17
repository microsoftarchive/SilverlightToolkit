// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Silverlight.Testing.UnitTesting.Metadata
{
    /// <summary>
    /// Work item for a test.
    /// </summary>
    public interface IWorkItemMetadata
    {
        /// <summary>
        /// Gets the associated information from the work item.
        /// </summary>
        string Data { get; }
    }
}