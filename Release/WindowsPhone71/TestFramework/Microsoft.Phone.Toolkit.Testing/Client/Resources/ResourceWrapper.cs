// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using Microsoft.Phone.Testing.Properties;

namespace Microsoft.Phone.Testing.Client.Resources
{
  /// <summary>
  /// Exposes the string resources to be used as binding expression in XAML code.
  /// </summary>
  public sealed class ResourceWrapper
  {
    /// <summary>
    /// Instance of the Resource file.
    /// </summary>
    private static UnitTestMessage appStrings = new UnitTestMessage();

    /// <summary>
    /// Application string resource dictionary to be used in XAML.
    /// </summary>
    public UnitTestMessage AppStrings
    {
      get
      {
        return appStrings;
      }
    }
  }
}
