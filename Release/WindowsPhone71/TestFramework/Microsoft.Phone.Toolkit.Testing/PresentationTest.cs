// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Windows.Controls;

namespace Microsoft.Phone.Testing
{
  /// <summary>
  /// Implementation of useful properties and features for presentation 
  /// platform tests.
  /// 
  /// Tests using this functionality will not be compatible with the full 
  /// desktop framework's Visual Studio Team Test environment.
  /// </summary>
  public abstract class PresentationTest : WorkItemTest
  {
    /// <summary>
    /// Gets the test panel.
    /// </summary>
    public Panel TestPanel
    {
      get
      {
        return UnitTestHarness.TestPanelManager.TestPanel;
      }
    }
  }
}
