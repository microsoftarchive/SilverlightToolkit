// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Microsoft.Phone.Testing.Client;
using Microsoft.Phone.Testing.Harness;
using Microsoft.Phone.Testing.Service;

namespace Microsoft.Phone.Testing
{
  /// <summary>
  /// A central entry point for unit test projects and applications.
  /// </summary>
  public partial class UnitTestSystem
  {
    /// <summary>
    /// A partial method for PrepareDefaultLogManager.
    /// </summary>
    /// <param name="settings">The test harness settings.</param>
    public static void PrepareCustomLogProviders(UnitTestSettings settings)
    {
      // TODO: Consider what to do on this one...
      // Should probably update to use the newer log system with events,
      // and then after that figure out when it applies... perhaps only
      // when someone first requests to use it.
      ////if (HtmlPage.IsEnabled)
      ////{
      ////settings.LogProviders.Add(new TextFailuresLogProvider());
      ////}
    }

    /// <summary>
    /// A partial method for setting the TestService.
    /// </summary>
    /// <param name="settings">The test harness settings.</param>
    public static void SetTestService(UnitTestSettings settings)
    {
      // REVIEW Following line commented because TestService need to be reviewed.
      //settings.TestService = new WindowsPhoneTestService(settings);
    }

    /// <summary>
    /// Creates a new TestPage visual that in turn will setup and begin a 
    /// unit test run.
    /// </summary>
    /// <returns>A new RootVisual.</returns>
    /// <remarks>Assumes the calling assembly is a test assembly.</remarks>
    public static UIElement CreateTestPage()
    {
      UnitTestSettings settings = CreateDefaultSettings(Assembly.GetCallingAssembly());
      return CreateTestPage(settings);
    }

    /// <summary>
    /// Creates a new TestPage visual that in turn will setup and begin a 
    /// unit test run.
    /// </summary>
    /// <param name="settings">Test harness settings to be applied.</param>
    /// <returns>A new RootVisual.</returns>
    /// <remarks>Assumes the calling assembly is a test assembly.</remarks>
    public static UIElement CreateTestPage(UnitTestSettings settings)
    {
      UnitTestSystem system = new UnitTestSystem();

      Type testPageType = typeof(TestPage);

      Type testPageInterface = typeof(ITestPage);
      if (settings.TestPanelType != null && testPageInterface.IsAssignableFrom(settings.TestPanelType))
      {
        testPageType = settings.TestPanelType;
      }

      object testPage;
      try
      {
        // Try creating with an instance of the test harness
        testPage = Activator.CreateInstance(testPageType, settings.TestHarness);
      }
      catch
      {
        // Fall back to a standard instance only
        testPage = Activator.CreateInstance(testPageType);
      }

      PrepareTestService(settings, () => system.Run(settings));

      // NOTE: A silent failure would be if the testPanel is not a
      // UIElement, and it returns anyway.
      return testPage as UIElement;
    }

    /// <summary>
    /// Merge any settings provided by a test service with the parameters 
    /// that were passed inside the TestHarnessSettings.
    /// </summary>
    /// <param name="testService">The test service.</param>
    /// <param name="inputSettings">The run settings.</param>
    private static void MergeSettingsAndParameters(TestServiceProvider testService, UnitTestSettings inputSettings)
    {
      if (testService != null && testService.HasService(TestServiceFeature.RunSettings))
      {
        SettingsProvider settings = testService.GetService<SettingsProvider>(TestServiceFeature.RunSettings);
        foreach (string key in settings.Settings.Keys)
        {
          if (inputSettings.Parameters.ContainsKey(key))
          {
            Debug.WriteLine("MergeSettingsAndParameters: Overwriting " + key + " key during merge.");
          }
          inputSettings.Parameters[key] = settings.Settings[key];
        }
      }
    }

    /// <summary>
    /// Initializes the test service and its contained providers.
    /// </summary>
    /// <param name="inputSettings">The run settings.</param>
    /// <param name="complete">Action to call once the test service is 
    /// initialized and ready to continue the run's execution.</param>
    private static void PrepareTestService(UnitTestSettings inputSettings, Action complete)
    {
      TestServiceProvider testService = inputSettings.TestService;
      if (testService != null && testService.Initialized == false)
      {
        Action after = delegate
        {
          MergeSettingsAndParameters(testService, inputSettings);
          complete();
        };
        testService.InitializeCompleted += delegate(object sender, EventArgs e) { after(); };
        testService.Initialize();
      }
      else
      {
        complete();
      }
    }
  }
}
