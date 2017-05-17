// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Silverlight.Testing.Harness;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// Manages the unit test status and model by attaching to the unit test
    /// harness instance. Validates that all key logging can be done without
    /// special hooks inside of the unit test harness implementation.
    /// </summary>
    public class DataManager
    {
        /// <summary>
        /// The unit test harness instance.
        /// </summary>
        private UnitTestHarness _h;

        /// <summary>
        /// The test run data.
        /// </summary>
        private TestRunData _d;

        /// <summary>
        /// Map assembly metadata to data objects.
        /// </summary>
        private Dictionary<IAssembly, TestAssemblyData> _assemblyData;

        /// <summary>
        /// Map test class metadata to data objects.
        /// </summary>
        private Dictionary<ITestClass, TestClassData> _classData;

        /// <summary>
        /// Map test metadata to data objects.
        /// </summary>
        private Dictionary<ITestMethod, TestMethodData> _methodData;

        /// <summary>
        /// Backing field for the last result.
        /// </summary>
        private TestMethodData _lastResult;

        /// <summary>
        /// Backing field for the last failing result.
        /// </summary>
        private TestMethodData _lastFailingResult;

        /// <summary>
        /// Gets the unit test harness instance.
        /// </summary>
        protected UnitTestHarness UnitTestHarness
        {
            get { return _h; }
        }

        /// <summary>
        /// Initializes a new instance of the DataManager type.
        /// </summary>
        /// <param name="harness">The unit test harness instance.</param>
        private DataManager(UnitTestHarness harness)
        {
            _d = new TestRunData(harness);
            _h = harness;

            _assemblyData = new Dictionary<IAssembly, TestAssemblyData>(2);
            _classData = new Dictionary<ITestClass, TestClassData>(50);
            _methodData = new Dictionary<ITestMethod, TestMethodData>(300);
        }

        /// <summary>
        /// Initializes a new instance of the DataManager.
        /// </summary>
        /// <param name="harness">The unit test harness instance.</param>
        /// <returns>Returns a new instance of a DataManager class.</returns>
        public static DataManager Create(UnitTestHarness harness)
        {
            return new DataManager(harness);
        }

        /// <summary>
        /// Gets the unit test model.
        /// </summary>
        public TestRunData Data { get { return _d; } }

        /// <summary>
        /// Connect to unit test harness events for processing and updating the
        /// underlying unit test run model.
        /// </summary>
        public virtual void Hook()
        {
            if (_h == null)
            {
                throw new ArgumentNullException("Harness");
            }

            _h.TestMethodStarting += OnTestMethodStarting;
            _h.TestMethodCompleted += OnTestMethodCompleted;
            _h.TestClassStarting += OnTestClassStarting;
            _h.TestClassCompleted += OnTestClassCompleted;
            _h.TestRunStarting += OnTestRunStarting;
        }

        /// <summary>
        /// Unhook from the unit test harness events.
        /// </summary>
        public virtual void Unhook()
        {
            _h.TestMethodStarting -= OnTestMethodStarting;
            _h.TestMethodCompleted -= OnTestMethodCompleted;
            _h.TestClassStarting -= OnTestClassStarting;
            _h.TestClassCompleted -= OnTestClassCompleted;
            _h.TestRunStarting -= OnTestRunStarting;
        }

        /// <summary>
        /// Process the starting of the test run.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnTestRunStarting(object sender, TestRunStartingEventArgs e)
        {
            if (_d != null && e != null)
            {
                _d.Title = e.TestHarnessName;
                _d.TotalScenarios = _h.TestMethodCount;
            }
        }

        /// <summary>
        /// Process the test class starting event.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnTestClassStarting(object sender, TestClassStartingEventArgs e)
        {
            TestClassData tac = GetClassModel(e.TestClass);
            tac.IsExpanded = true;

            _d.CurrentTest = e.TestClass.Name;
        }

        /// <summary>
        /// Process the test class complete event.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnTestClassCompleted(object sender, TestClassCompletedEventArgs e)
        {
            TestClassData tac = GetClassModel(e.TestClass);
            if (tac != null)
            {
                tac.CollapseUnlessFailures();
            }
        }

        /// <summary>
        /// Process the start of a test method.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnTestMethodStarting(object sender, TestMethodStartingEventArgs e)
        {
            TestClassData tac = GetClassModel(e.TestClass);
            TestMethodData tmd = GetMethodModel(e.TestMethod, tac);
            if (!tac.IsExpanded)
            {
                tac.IsExpanded = true;
            }
            tmd.IsRunning = true;

            _d.CurrentTestMethod = e.TestMethod.Name;
        }

        /// <summary>
        /// Process the completion of test methods.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnTestMethodCompleted(object sender, TestMethodCompletedEventArgs e)
        {
            ScenarioResult result = e.Result;
            if (result == null)
            {
                throw new InvalidOperationException("The result was not present.");
            }

            ProcessResult(result);
        }

        /// <summary>
        /// Process a result.
        /// </summary>
        /// <param name="result">The result data.</param>
        private void ProcessResult(ScenarioResult result)
        {
            TestClassData tac = GetClassModel(result.TestClass);
            TestMethodData tmd = GetMethodModel(result.TestMethod, tac);

            tmd.IsRunning = false;

            tmd.IsNotable = !tmd.Passed;

            if (_d == null)
            {
                return;
            }

            // Link to previous
            tmd.PreviousResult = _lastResult;
            _lastResult = tmd;

            _d.RunScenarios++;
            if (result.Result != TestOutcome.Passed)
            {
                _d.FailedScenarios++;

                // Link to previous failure
                tmd.PreviousFailingResult = _lastFailingResult;
                _lastFailingResult = tmd;

                // Automatically check the item for the user
                tmd.IsChecked = true;
            }

            tmd.Result = result;
        }

        #region Create the data model

        /// <summary>
        /// Gets or creates the data model object for an assembly.
        /// </summary>
        /// <param name="assembly">The test assembly.</param>
        /// <returns>Returns the data object.</returns>
        public TestAssemblyData GetAssemblyModel(IAssembly assembly)
        {
            TestAssemblyData data;
            if (!_assemblyData.TryGetValue(assembly, out data))
            {
                data = new TestAssemblyData(assembly);
                _assemblyData.Add(assembly, data);

                // Make sure in parent collection
                _d.TestAssemblies.Add(data);
            }

            return data;
        }

        /// <summary>
        /// Gets or creates the data model object for a test class.
        /// </summary>
        /// <param name="testClass">The test class.</param>
        /// <returns>Returns the data object.</returns>
        public TestClassData GetClassModel(ITestClass testClass)
        {
            TestClassData data;
            if (!_classData.TryGetValue(testClass, out data))
            {
                TestAssemblyData tad = GetAssemblyModel(testClass.Assembly);
                data = new TestClassData(testClass, tad);
                _classData.Add(testClass, data);

                // Make sure in parent collection
                tad.TestClasses.Add(data);
            }

            return data;
        }

        /// <summary>
        /// Gets or creates the data model object for a test method.
        /// </summary>
        /// <param name="testMethod">The test method.</param>
        /// <param name="parentTestClass">The parent test class data object.</param>
        /// <returns>Returns the data object.</returns>
        public TestMethodData GetMethodModel(ITestMethod testMethod, TestClassData parentTestClass)
        {
            TestMethodData data;
            if (!_methodData.TryGetValue(testMethod, out data))
            {
                data = new TestMethodData(testMethod, parentTestClass);
                _methodData.Add(testMethod, data);

                // Make sure in parent collection
                parentTestClass.TestMethods.Add(data);
            }

            return data;
        }

        #endregion
    }
}