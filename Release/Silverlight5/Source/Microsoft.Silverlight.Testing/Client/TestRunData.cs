// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using Microsoft.Silverlight.Testing.Harness;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A data object that generates property change notifications and can
    /// be used for rich data binding to test results. Does keep a reference
    /// to all results.
    /// </summary>
    public class TestRunData : PropertyChangedBase
    {
        /// <summary>
        /// The unit test harness instance.
        /// </summary>
        private UnitTestHarness _harness;

        /// <summary>
        /// Backing field for information about the test application in use.
        /// </summary>
        private TestApplicationInformation _information;

        /// <summary>
        /// Initializes a new instance of the test run results.
        /// </summary>
        /// <param name="unitTestHarness">The unit test harness.</param>
        public TestRunData(UnitTestHarness unitTestHarness)
        {
            Passed = true;
            _assemblies = new ObservableCollection<TestAssemblyData>();
            _harness = unitTestHarness;
            _information = new TestApplicationInformation();
            _harness.TestAssemblyStarting += (x, xe) => IsRunning = true;
            _harness.TestAssemblyCompleted += (x, xe) =>
                {
                    IsRunning = false;
                    NotifyPropertyChanged("PassedAndComplete");
                };
            _harness.IsDispatcherRunningChanged += (x, xe) => NotifyPropertyChanged("IsDispatcherRunning");
        }

        /// <summary>
        /// Gets the test application information instance.
        /// </summary>
        public TestApplicationInformation TestApplicationInformation { get { return _information; } }

        /// <summary>
        /// Gets the unit test harness instance.
        /// </summary>
        public UnitTestHarness UnitTestHarness { get { return _harness; } }

        #region TestAssemblies

        /// <summary>
        /// Backing store for the set of test assemblies.
        /// </summary>
        private ObservableCollection<TestAssemblyData> _assemblies;

        /// <summary>
        /// Gets an observable collection of test assembly data objects.
        /// </summary>
        public ObservableCollection<TestAssemblyData> TestAssemblies
        {
            get { return _assemblies; }
        }

        #endregion

        #region Title

        /// <summary>
        /// Title backing field.
        /// </summary>
        private string _title;

        /// <summary>
        /// Gets or sets the informational run title.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        #endregion

        #region Test assemblies

        #endregion

        #region TotalScenarios

        /// <summary>
        /// Stores the total number of expected scenarios.
        /// </summary>
        private int _total;

        /// <summary>
        /// Gets or sets the number of total scenarios.
        /// </summary>
        public int TotalScenarios
        {
            get { return _total; }
            set
            {
                _total = value;
                NotifyPropertyChanged("TotalScenarios");
                NotifyPropertyChanged("PassedScenarios");
            }
        }

        #endregion

        #region CurrentTestMethod

        /// <summary>
        /// Stores the current test information.
        /// </summary>
        private string _currentTestMethod;

        /// <summary>
        /// Gets or sets the current test name.
        /// </summary>
        public string CurrentTestMethod
        {
            get
            {
                return _currentTestMethod;
            }

            set
            {
                _currentTestMethod = value;
                NotifyPropertyChanged("CurrentTestMethod");
            }
        }

        #endregion

        #region CurrentTest

        /// <summary>
        /// Stores the current test information.
        /// </summary>
        private string _currentTest;

        /// <summary>
        /// Gets or sets the current test name.
        /// </summary>
        public string CurrentTest
        {
            get
            {
                return _currentTest;
            }

            set
            {
                _currentTest = value;
                NotifyPropertyChanged("CurrentTest");
            }
        }

        #endregion

        #region RunScenarios

        /// <summary>
        /// Stores the number of run scenarios.
        /// </summary>
        private int _runScenarios;

        /// <summary>
        /// Gets or sets the number of run scenarios.
        /// </summary>
        public int RunScenarios
        {
            get { return _runScenarios; }
            set
            {
                _runScenarios = value;
                NotifyPropertyChanged("RunScenarios");
                NotifyPropertyChanged("PassedScenarios");
            }
        }

        #endregion

        #region FailedScenarios

        /// <summary>
        /// Count of failed scenarios.
        /// </summary>
        private int _failedScenarios;

        /// <summary>
        /// Gets or sets the number of failed scenarios.
        /// </summary>
        public int FailedScenarios
        {
            get { return _failedScenarios; }
            set
            {
                _failedScenarios = value;
                NotifyPropertyChanged("FailedScenarios");
                NotifyPropertyChanged("PassedScenarios");

                if (value == 1)
                {
                    Passed = false;
                    NotifyPropertyChanged("Passed");
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether the run is complete and passed.
        /// </summary>
        public bool PassedAndComplete
        {
            get { return Passed && (RunScenarios > 0); }
        }

        /// <summary>
        /// Gets a value indicating whether all passing results have been
        /// processed.
        /// </summary>
        public bool Passed { get; private set; }

        #region PassedScenarios

        /// <summary>
        /// Gets the number of passed scenarios, defined as the number of
        /// run scenarios minus the number of failed scenarios tracked.
        /// </summary>
        public int PassedScenarios
        {
            get { return _runScenarios - _failedScenarios; }
        }

        #endregion

        #region IsDispatcherRunning

        /// <summary>
        /// Gets a value indicating whether the dispatcher is currently running.
        /// </summary>
        public bool IsDispatcherRunning
        {
            get
            {
                return _harness.IsDispatcherRunning;
            }
        }

        #endregion

        #region IsRunning

        /// <summary>
        /// Backing field for whether the app is running.
        /// </summary>
        private bool _running;

        /// <summary>
        /// Gets a value indicating whether the test run is in action.
        /// </summary>
        public bool IsRunning
        {
            get { return _running; }
            private set
            {
                _running = value;
                NotifyPropertyChanged("IsRunning");
            }
        }

        #endregion
    }
}