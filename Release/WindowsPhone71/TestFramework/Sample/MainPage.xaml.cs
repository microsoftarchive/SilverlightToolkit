// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using Microsoft.Phone.Controls;
using Microsoft.Phone.Testing;

namespace UnitTestingTests
{
  /// <summary>
  /// Main Page for the test application
  /// </summary>
  public partial class MainPage : PhoneApplicationPage
  {
    /// <summary>
    /// Initializes the page and hooks up the test framework.
    /// </summary>
    public MainPage()
    {
      InitializeComponent();

      // Creates a test page that will replace this page contents.
      this.Content = UnitTestSystem.CreateTestPage();
    }
  }
}