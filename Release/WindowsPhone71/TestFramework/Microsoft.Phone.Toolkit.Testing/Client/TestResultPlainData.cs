// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;

namespace Microsoft.Phone.Testing.Client
{
  /// <summary>
  /// Structure that holds information about plain text test results.
  /// </summary>
  internal struct TestResultPlainData
  {
    /// <summary>
    /// filename for results to be stored.
    /// </summary>
    internal string FileName;

    /// <summary>
    /// Test results on plain text.
    /// </summary>
    internal string Text;
  }
}
