// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Phone.Testing.Metadata
{
  /// <summary>
  /// A representation of a test's priority.
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "These interfaces are used for wrapping attributes of different types, so this is needed for the facade pattern")]
  public interface IPriority { }
}
