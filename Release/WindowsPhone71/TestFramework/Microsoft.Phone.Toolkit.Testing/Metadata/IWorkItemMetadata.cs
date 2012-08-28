// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Phone.Testing.Metadata
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
