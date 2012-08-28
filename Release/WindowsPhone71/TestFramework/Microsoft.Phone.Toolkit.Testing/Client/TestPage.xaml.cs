// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Testing.Harness;

namespace Microsoft.Phone.Testing.Client
{
  /// <summary>
  /// A user control designed for mobile platforms. The control should be used
  /// as the root visual for a Silverlight plugin if developers would like to 
  /// use the advanced TestSurface functionality.
  /// </summary>
  public partial class TestPage : UserControl, ITestPage
  {
    /// <summary>
    /// Backing field for the unit test harness instance.
    /// </summary>
    private UnitTestHarness _harness;

    /// <summary>
    /// Backing field for the startup timer.
    /// </summary>
    private DispatcherTimer _delayedInitializationTimer;

    /// <summary>
    /// Backing field for the model manager.
    /// </summary>
    private DataManager _model;

    /// <summary>
    /// Holds a reference to the Application Bar save button.
    /// </summary>
    private ApplicationBarIconButton _saveButton;

    /// <summary>
    /// Holds a reference to the Application Bar email button.
    /// </summary>
    private ApplicationBarIconButton _emailButton;

    /// <summary>
    /// Gets the test surface, a dynamic Panel that removes its children 
    /// elements after each test completes.
    /// </summary>
    public Panel TestPanel
    {
      get { return TestStage; }
    }

    /// <summary>
    /// Gets the unit test harness instance.
    /// </summary>
    public UnitTestHarness UnitTestHarness
    {
      get { return _harness; }
    }

    /// <summary>
    /// Initializes a new instance of the MobileTestPage class.
    /// </summary>
    public TestPage()
    {
      InitializeComponent();

      // Initialize the View with an empty ViewModel
      DataContext = new InitViewModel();

      _delayedInitializationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
      _delayedInitializationTimer.Tick += OnDelayedInitializationTick;
      _delayedInitializationTimer.Start();
    }

    /// <summary>
    /// Initializes the MobileTestPage object.
    /// </summary>
    /// <param name="harness">The test harness instance.</param>
    public TestPage(UnitTestHarness harness)
      : this()
    {
      harness.TestPage = this;
      _harness = harness;
    }

    #region Save and Email

    /// <summary>
    /// Saves the context results to the the application
    /// isolated storage area.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments.</param>
    private void OnSaveResultsButtonClick(object sender, EventArgs e)
    {
      // for storing results it will be used recursively
      TestResultPlainData resultsData = CollectContextResults(true);
      string folderName = "TestResults";
      string fileName = resultsData.FileName;

      // File is going to be stored on the root folder of the app
      using (IsolatedStorageFile testStore = IsolatedStorageFile.GetUserStoreForApplication())
      {
        // Check if test results folder exists or create it
        if (!testStore.DirectoryExists(folderName))
        {
          testStore.CreateDirectory(folderName);
        }

        // indicates the name of the file to be used, every time a new test
        // is stored, the results file is replaced.
        using (IsolatedStorageFileStream testFileStream = new IsolatedStorageFileStream(System.IO.Path.Combine(folderName, fileName), FileMode.Create, testStore))
        {
          using (StreamWriter testFileWriter = new StreamWriter(testFileStream))
          {
            testFileWriter.Write(resultsData.Text);
            MessageBox.Show(String.Format("A text file with the test results has been created.\nUse the Isolated Storage Explorer to extract the files from the device or Emulator.\n\nFile Name: {0}", fileName), "Save Process Completed", MessageBoxButton.OK);
          }
        }
      }
    }

    /// <summary>
    /// Fires the Email Task to send context results to the designated email
    /// address.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event arguments.</param>
    private void OnEmailResultsButtonClick(object sender, EventArgs e)
    {
      // For results email only the current results view is send.
      TestResultPlainData resultsData = CollectContextResults(false);

      EmailComposeTask ect = new EmailComposeTask();
      ect.Subject = "[WPUTFx] Test Results";
      ect.Body = resultsData.Text;

      ect.Show();
    }

    /// <summary>
    /// Collects the test results on a string representation.
    /// </summary>
    /// <param name="recursive">Indicates if the results are gathered with
    /// details.</param>
    /// <returns>A string containing the result text.</returns>
    private TestResultPlainData CollectContextResults(bool recursive)
    {
      string results = string.Empty;
      string fileName = string.Empty;
      StringBuilder sb = new StringBuilder();

      DateTime currentDate = DateTime.Now;

      sb.AppendLine("Windows Phone Test Framework");
      sb.AppendLine("=====================");
      sb.AppendLine("Test Data");
      sb.AppendLine("=====================");

      // Main Result view is visible (full results)
      if (TestExecution.Visibility == Visibility.Visible)
      {
        fileName = String.Format("Test.{0}.txt", currentDate.ToString("yyyy.mm.dd.HH.MM.ss"));

        foreach (TestAssemblyData tAssembly in _model.Data.TestAssemblies)
        {
          sb.AppendLine("Assembly Name:");
          sb.AppendLine(tAssembly.Name);
          sb.AppendFormat("{0} passed methods, {1} failed methods", tAssembly.PassCount, tAssembly.FailCount);
          sb.AppendLine();
          sb.AppendLine("***");

          foreach (TestClassData tClass in tAssembly.TestClasses)
          {
            sb.AppendLine("  Class Name:");
            sb.AppendLine("  " + tClass.Name);
            sb.AppendFormat("  {0} passed methods, {1} failed methods", tClass.PassCount, tClass.FailCount);
            sb.AppendLine();
            sb.AppendLine("  ---");
            foreach (TestMethodData tMethod in tClass.TestMethods)
            {
              sb.AppendFormat("    [{0}] {1}", tMethod.Passed ? "PASS" : "FAIL", tMethod.Name);
              if (recursive)
              {
                sb.Append(MethodDetailToString(tMethod));
                sb.AppendLine("    +++");
              }
              sb.AppendLine();
            }
            sb.AppendLine("  ###");
          }
        }
        results = sb.ToString();
      }
      else
      {
        // Method details view is visible.
        if (TestMethodView.Visibility == Visibility.Visible)
        {
          if (TestMethodView.Children.Count == 1)
          {
            TestMethodData tMethod = (TestMethodView.Children[0] as TestMethodDetails).DataContext as TestMethodData;
            fileName = String.Format("{0}.{1}.{2}.{3}.txt", tMethod.Parent.Namespace, tMethod.Parent.Name, tMethod.Name, currentDate.ToString("yyyy.mm.dd.HH.MM.ss"));

            sb.AppendLine("Assembly Name:");
            sb.AppendLine(tMethod.Parent.Parent.Name);
            sb.AppendLine("***");
            sb.AppendLine("Class Name:");
            sb.AppendLine(tMethod.Parent.Name);
            sb.AppendLine("Method Name:");
            sb.AppendLine(tMethod.Name);
            sb.Append(MethodDetailToString(tMethod));

            results = sb.ToString();
          }
        }
      }
      return new TestResultPlainData { FileName = fileName, Text = results };
    }

    /// <summary>
    /// Formats the test method data into a string.
    /// </summary>
    /// <param name="methodData">The test method data</param>
    /// <returns>A string representing the test method data.</returns>
    private string MethodDetailToString(TestMethodData methodData)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine();
      sb.AppendFormat("    {0}", methodData.Description);
      sb.AppendLine();
      sb.AppendFormat("    Result: {0}", methodData.Result.Result);
      sb.AppendLine();
      sb.AppendLine("    Executing Time");
      sb.AppendFormat("    Started: {0}", methodData.Result.Started);
      sb.AppendLine();
      sb.AppendFormat("    Finished: {0}", methodData.Result.Finished);
      sb.AppendLine();
      sb.AppendFormat("    Elapsed Time: {0}", methodData.ReadableElapsedTime);
      sb.AppendLine();
      sb.AppendLine();
      if (methodData.KnownBugs != null)
      {
        sb.AppendLine("    ...");
        sb.AppendLine("    Known Issues");
        sb.AppendLine("    ...");
        if (methodData.Passed)
        {
          sb.AppendLine("    These issues are marked as known for this test and resulted in it being marked as a success:");
        }
        else
        {
          sb.AppendLine("    These known issues should be marked fixed.");
          sb.AppendLine("    The test passed otherwise.");
        }
        foreach (string bug in methodData.KnownBugs)
        {
          sb.AppendLine("    * " + bug);
        }
      }

      if (methodData.FixedBugs != null)
      {
        sb.AppendLine("    ...");
        sb.AppendLine("    Fixed Issues");
        sb.AppendLine("    ...");
        if (!methodData.Passed)
        {
          sb.AppendLine("    These issues were marked as fixed, but should still be investigated for this failing test:");
        }
        foreach (string bug in methodData.FixedBugs)
        {
          sb.AppendLine("    * " + bug);
        }
      }

      if (!string.IsNullOrEmpty(methodData.SimplifiedExpectedExceptionName))
      {
        sb.AppendLine("    ...");
        sb.AppendLine("    Expected Exception (Negative Test)");
        sb.AppendLine("    ...");
        sb.AppendLine("    This test expected an exception of type");
        sb.AppendLine("    " + methodData.SimplifiedExpectedExceptionName);
      }

      if (methodData.Result.Exception != null)
      {
        sb.AppendLine("    ...");
        sb.AppendLine("    Exception Details");
        sb.AppendLine("    ...");
        sb.AppendFormat("    {0}  was unhandled", methodData.SimplifiedExceptionName);
        sb.AppendLine();
        sb.AppendLine("    " + methodData.Result.Exception.Message);
        sb.AppendLine(methodData.SimplifiedExceptionStackTrace);

        if (methodData.Result.Exception.InnerException != null)
        {
          sb.AppendLine("    This test result also contained an inner exception:");
          sb.AppendFormat("    {0}", methodData.Result.Exception.InnerException);
          sb.AppendLine();
        }
      }
      return sb.ToString();
    }

    #endregion


    /// <summary>
    /// Waits for the Settings to become available, either by the service or
    /// system setting the instance property.
    /// </summary>
    /// <param name="sender">The source timer.</param>
    /// <param name="e">The event arguments.</param>
    private void OnDelayedInitializationTick(object sender, EventArgs e)
    {
      if (_harness != null && _harness.Settings != null)
      {
        _delayedInitializationTimer.Stop();
        _delayedInitializationTimer.Tick -= OnDelayedInitializationTick;
        _delayedInitializationTimer = null;

        PassButton.Style = this.Resources["CounterButtonStyle"] as Style;
        FailButton.Style = this.Resources["CounterButtonStyle"] as Style;
        TotalButton.Style = this.Resources["CounterButtonStyle"] as Style;

        UIElement rootVisual = Application.Current.RootVisual;
        if (rootVisual is PhoneApplicationFrame)
        {
          PhoneApplicationPage mainPage = (((PhoneApplicationFrame)rootVisual).Content as PhoneApplicationPage);

          mainPage.BackKeyPress += OnBackKeyPress;
        }

        // Tag expression support
        TagEditor tagEditor = null;
        if (_harness.Settings != null)
        {
          if (_harness.Settings.StartRunImmediately)
          {
            tagEditor = null;
          }
          else
          {
            if (_harness.Settings.TagExpression != null)
            {
              if (_harness.Settings.SampleTags != null)
              {
                tagEditor = new TagEditor(_harness.Settings.TagExpression, _harness.Settings.SampleTags);
              }
              else
              {
                tagEditor = new TagEditor(_harness.Settings.TagExpression);
              }
            }
            else
            {
              tagEditor = new TagEditor();
            }
          }
        }
        else
        {
          tagEditor = new TagEditor();
        }

        if (tagEditor == null)
        {
          StartTestRun();
          return;
        }

        TagEditorHolder.Children.Add(tagEditor);

        // Hide Current content and display TagEditor Interface
        TestExecution.Visibility = Visibility.Collapsed;
        TagEditorHolder.Visibility = Visibility.Visible;

        tagEditor.Complete += OnTagExpressionSelected;
      }
    }

    /// <summary>
    /// Handles the interception of the back key 
    /// button to determine what action to take.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnBackKeyPress(object sender, CancelEventArgs e)
    {
      if (TestMethodView.Visibility == Visibility.Visible)
      {
        TestMethodView.Visibility = Visibility.Collapsed;
        TestExecution.Visibility = Visibility.Visible;
        e.Cancel = true;
        return;
      }
      e.Cancel = false;
    }

    /// <summary>
    /// Handles the completion event on the tag expression editor to begin
    /// the test run using the user-provided settings.
    /// </summary>
    /// <param name="sender">The source object.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnTagExpressionSelected(object sender, TagExpressionEventArgs e)
    {
      TagEditor te = sender as TagEditor;
      if (te != null)
      {
        te.Complete -= OnTagExpressionSelected;
      }

      //TagEditorHolder.Children.Clear();
      TagEditorHolder.Visibility = Visibility.Collapsed;
      TestExecution.Visibility = Visibility.Visible;

      if (_harness != null)
      {
        if (e != null && _harness.Settings != null)
        {
          _harness.Settings.TagExpression = e.TagExpression;
        }

        // Start the test run
        StartTestRun();
      }
    }

    /// <summary>
    /// Starts the test run.
    /// </summary>
    private void StartTestRun()
    {
      //TagEditorHolder.Visibility = Visibility.Collapsed;
      //TestExecution.Visibility = Visibility.Visible;

      // Configure the Application Bar buttons.
      UIElement rootVisual = Application.Current.RootVisual;
      if (rootVisual is PhoneApplicationFrame)
      {
        IApplicationBar appBar;
        PhoneApplicationPage mainPage = (((PhoneApplicationFrame)rootVisual).Content as PhoneApplicationPage);

        // The test project should not have an app bar configured
        // if so, replace it.

        appBar = new ApplicationBar();
        mainPage.ApplicationBar = appBar;
        appBar.IsMenuEnabled = false;

        _emailButton = new ApplicationBarIconButton();
        _emailButton.IconUri = new Uri("/Toolkit.Content/appbar.feature.email.rest.png", UriKind.Relative);
        _emailButton.Text = "email";
        _emailButton.Click += OnEmailResultsButtonClick;
        _emailButton.IsEnabled = false;

        _saveButton = new ApplicationBarIconButton();
        _saveButton.IconUri = new Uri("/Toolkit.Content/appbar.save.rest.png", UriKind.Relative);
        _saveButton.Text = "save";
        _saveButton.Click += OnSaveResultsButtonClick;
        _saveButton.IsEnabled = false;


        appBar.Buttons.Add(_emailButton);
        appBar.Buttons.Add(_saveButton);
      } 
      
      if (!(DataContext is TestRunData))
      {
        _model = DataManager.Create(_harness);
        _model.Hook();

        DataContext = _model.Data;

        _harness.TestHarnessCompleted += OnTestHarnessCompleted;
      }

      _harness.Run();
      RunningTestPanel.Visibility = System.Windows.Visibility.Visible;
      RunningTestProgress.IsIndeterminate = true;
    }

    /// <summary>
    /// Handles the test harness complete event, to display results.
    /// </summary>
    /// <param name="sender">The source object.</param>
    /// <param name="e">The event data.</param>
    private void OnTestHarnessCompleted(object sender, TestHarnessCompletedEventArgs e)
    {
      foreach (TestAssemblyData assembly in _model.Data.TestAssemblies)
      {
        assembly.UpdateCounts();
      }

      RunningTestPanel.Visibility = System.Windows.Visibility.Collapsed;
      RunningTestProgress.IsIndeterminate = false;

      _emailButton.IsEnabled = true;
      _saveButton.IsEnabled = true;
    }

    /// <summary>
    /// Shows the method results details.
    /// </summary>
    /// <param name="sender">The <see cref="T:System.Windows.Controls.ListBoxItem"/> that fires the event.</param>
    /// <param name="e">Information about the gesture.</param>
    private void OnMethodItemTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      ListBoxItem selectedMethod = sender as ListBoxItem;
      if (selectedMethod == null)
      {
        return;
      }

      TestMethodView.Children.Clear();
      TestMethodView.Children.Add(new TestMethodDetails() { DataContext = selectedMethod.DataContext });

      TestExecution.Visibility = Visibility.Collapsed;
      TestMethodView.Visibility = Visibility.Visible;
    }
  }
}