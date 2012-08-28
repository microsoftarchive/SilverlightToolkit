// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

namespace Microsoft.Phone.Testing.Client
{
  /// <summary>
  /// This class has a temporal view model to initialize the view
  /// before any test data is available or configured.
  /// </summary>
  public class InitViewModel : PropertyChangedBase
  {
    /// <summary>
    /// Initializes the view model with initial values.
    /// </summary>
    public InitViewModel()
    {
      PassedScenarios = 0;
      FailedScenarios = 0;
      TotalScenarios = 0;
      NotifyPropertyChanged("FailedScenarios");
      NotifyPropertyChanged("PassedScenarios");
      NotifyPropertyChanged("TotalScenarios");
    }

    /// <summary>
    /// Gets the Passed Scenarios counter.
    /// </summary>
    public int PassedScenarios { get; private set; }

    /// <summary>
    /// Gets the Failed Scenarios counter.
    /// </summary>
    public int FailedScenarios { get; private set; }

    /// <summary>
    /// Gets the Total Scenarios counter.
    /// </summary>
    public int TotalScenarios { get; private set; }
  }
}
