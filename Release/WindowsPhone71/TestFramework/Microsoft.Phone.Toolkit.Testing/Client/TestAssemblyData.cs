// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Phone.Testing.Metadata;

namespace Microsoft.Phone.Testing.Client
{
  /// <summary>
  /// A data object storing the hierarchical results for a test assembly in a
  /// test run.
  /// </summary>
  public class TestAssemblyData : PropertyChangedBase, IProvideResultReports
  {
    /// <summary>
    /// Initializes a new instance of the TestAssemblyData type.
    /// </summary>
    /// <param name="testAssembly">The test assembly metadata.</param>
    public TestAssemblyData(IAssembly testAssembly)
    {
      if (testAssembly == null)
      {
        throw new ArgumentNullException("testAssembly");
      }

      _classes = new ObservableCollection<TestClassData>();

      // A weak reference or snap of data can be used to prevent leaks.
      Name = testAssembly.Name;

      // Always expand test assemblies
      IsExpanded = true;
    }

    /// <summary>
    /// Backing field for the expanded property.
    /// </summary>
    private bool _expanded;

    /// <summary>
    /// Gets or sets a value indicating whether the item is expanded in
    /// a hierarchical display.
    /// </summary>
    public bool IsExpanded
    {
      get
      {
        return _expanded;
      }

      set
      {
        bool old = _expanded;
        _expanded = value;
        if (old != _expanded)
        {
          NotifyPropertyChanged("IsExpanded");
        }
      }
    }

    /// <summary>
    /// Gets or sets the checked value. Don't think this is actually used.
    /// </summary>
    public bool? IsChecked { get; set; }

    /// <summary>
    /// Backing field for a passed value.
    /// </summary>
    private bool _passed = true;

    /// <summary>
    /// Gets or sets a value indicating whether the test passed. If failed,
    /// will propagate to the parent metadata object.
    /// </summary>
    public bool Passed
    {
      get { return _passed; }

      set
      {
        bool old = _passed;
        _passed = value;
        if (old != _passed)
        {
          NotifyPropertyChanged("Passed");
        }
      }
    }

    /// <summary>
    /// Gets the name of the assembly.
    /// </summary>
    public string Name { get; private set; }

    #region TestClasses

    /// <summary>
    /// Backing store for the set of test class.
    /// </summary>
    private ObservableCollection<TestClassData> _classes;

    /// <summary>
    /// Gets an observable collection of test class data objects.
    /// </summary>
    public ObservableCollection<TestClassData> TestClasses
    {
      get { return _classes; }
    }

    #endregion

    /// <summary>
    /// Notifies changes on properties caused by
    /// children objects.
    /// </summary>
    internal void UpdateCounts()
    {
      foreach (TestClassData tClass in TestClasses)
      {
        tClass.UpdateCounts();
      }
      NotifyPropertyChanged("FailCount");
      NotifyPropertyChanged("PassCount");
    }

    /// <summary>
    /// Reports the number of methods that has 
    /// passed under the current assembly.
    /// </summary>
    public int PassCount
    {
      get
      {
        int count = 0;
        foreach (TestClassData classData in TestClasses)
        {
          count += classData.PassCount;
        }
        return count;
      }
    }

    /// <summary>
    /// Reports the number of methods that has
    /// failed under the current assembly.
    /// </summary>
    public int FailCount
    {
      get
      {
        int count = 0;
        foreach (TestClassData classData in TestClasses)
        {
          count += classData.FailCount;
        }
        return count;
      }
    }

    /// <summary>
    /// Retrieves the results report.
    /// </summary>
    /// <returns>Returns a string containing the report.</returns>
    public string GetResultReport()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Test Assembly: ");
      sb.AppendLine(Name);
      sb.AppendLine(Passed ? "Passed" : "Failed");

      foreach (TestClassData tcd in TestClasses)
      {
        sb.AppendLine(tcd.GetResultReport());
      }

      return sb.ToString();
    }
  }
}