// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using Microsoft.Phone.Controls;

namespace Microsoft.Phone.Testing.Client
{
  /// <summary>
  /// Provides a Browser page to navigate to MSDN documentation
  /// for exceptions found on the results.
  /// </summary>
  public partial class DocView : PhoneApplicationPage
  {
    /// <summary>
    /// Initializes the DocView instance.
    /// </summary>
    public DocView()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Overrides the navigation event when arriving to the page, to collect
    /// the query string parameters.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      string urlString = String.Format("http://msdn.microsoft.com/en-us/library/{0}(VS.95,loband).aspx", NavigationContext.QueryString["t"].ToLowerInvariant());

      DocBrowser.Source = new Uri(urlString, UriKind.Absolute);
    }
  }
}