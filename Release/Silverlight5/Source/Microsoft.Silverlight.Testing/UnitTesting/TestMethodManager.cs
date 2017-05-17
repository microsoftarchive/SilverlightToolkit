// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Silverlight.Testing.Harness;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// Manager for planning, processing, and reporting the result of a single 
    /// test method for a unit test provider.
    /// </summary>
    public partial class TestMethodManager : UnitTestCompositeWorkItem
    {
        /// <summary>
        /// Underlying test class object.
        /// </summary>
        private ITestClass _testClass;

        /// <summary>
        /// Underlying test method object.
        /// </summary>
        private ITestMethod _testMethod;

        /// <summary>
        /// Reference to an instance of the test class.
        /// </summary>
        private object _instance;

        /// <summary>
        /// Scenario result of the test method.
        /// </summary>
        private ScenarioResult _result;

        /// <summary>
        /// The started time of execution.
        /// </summary>
        private DateTime _started;

        /// <summary>
        /// Stores a collection of written lines.
        /// </summary>
        private List<string> _output;

        /// <summary>
        /// Gets or sets a value indicating whether the bug attribute's logic
        /// has already been processed.
        /// </summary>
        private bool BugAttributeProcessed { get; set; }

        /// <summary>
        /// A value indicating whether the bug attribute was present on this 
        /// method.  If it is, the result will be inverted at completion.
        /// </summary>
        private bool _bugAttributePresent;

        /// <summary>
        /// Contains the main test contents.
        /// </summary>
        private UnitTestMethodContainer _mainTestMethodContainer;

        /// <summary>
        /// Constructor for a test method manager, which handles executing a single test method 
        /// for a unit test provider.
        /// </summary>
        /// <param name="testHarness">The unit test harness object.</param>
        /// <param name="testClass">The test class metadata object.</param>
        /// <param name="testMethod">The test method metadata object.</param>
        /// <param name="instance">The test class instance.</param>
        /// <param name="provider">The unit test provider.</param>
        public TestMethodManager(UnitTestHarness testHarness, ITestClass testClass, ITestMethod testMethod, object instance, IUnitTestProvider provider) : base(testHarness, provider)
        {
            _testClass = testClass;
            _testMethod = testMethod;
            _instance = instance;
        }
        
        /// <summary>
        /// Log a start message.
        /// </summary>
        private void LogStartMessage()
        {
            LogWriter.TestMethodStage(_testMethod, TestStage.Starting);
            TestHarness.Events.SendTestMethodStarting(new TestMethodStartingEventArgs(_testMethod, _testClass, TestHarness));
        }

        /// <summary>
        /// Log an end message.
        /// </summary>
        private void LogEndMessage()
        {
            LogWriter.TestMethodStage(_testMethod, TestStage.Finishing);
        }

        /// <summary>
        /// Gets the write line output list.
        /// </summary>
        public List<string> WriteLineOutput
        {
            get { return _output; }
        }

        /// <summary>
        /// Handles the write line event for the test method.
        /// </summary>
        /// <param name="e">The string event arguments.</param>
        protected virtual void OnWriteLine(StringEventArgs e)
        {
            if (_output == null)
            {
                _output = new List<string> { e.Value };
            }
            else
            {
                _output.Add(e.Value);
            }

            LogWriter.DebugWriteLine(e.Value);
        }

        /// <summary>
        /// First invoke, plan for the method's execution.
        /// </summary>
        protected override void FirstInvoke()
        {
            // [Ignore]
            if (Provider.HasCapability(UnitTestProviderCapabilities.MethodCanIgnore) && _testMethod.Ignore) 
            {
                LogWriter.Ignore(TestGranularity.TestScenario, _testMethod.Name);
                return;
            }

            _testMethod.DecorateInstance(_instance);

            _testMethod.WriteLine += (object sender, StringEventArgs e) => OnWriteLine(e);

            // Log Start
            LogStartMessage();

            // [Bug] attributes that are not fixed modify test method logic
            ICollection<Attribute> bugs = ReflectionUtility.GetAttributes(_testMethod, typeof(BugAttribute), false);
            if (bugs != null && bugs.Count > 0)
            {
                foreach (Attribute attribute in bugs)
                {
                    BugAttribute bug = attribute as BugAttribute;
                    if (!bug.Fixed)
                    {
                        _bugAttributePresent = true;
                        LogWriter.KnownIssue(bug.Description);
                    }
                }
            }

            // [TestInitialize]
            if (_testClass.TestInitializeMethod != null) 
            {
                EnqueueMethodDispatcher(_testClass.TestInitializeMethod);
            }

            // Track the approximate starting time - actual start time is >= 1 dispatcher interval 
            EnqueueQuick(() => _started = DateTime.Now);

            // [TestMethod] - actual test scenario
            _mainTestMethodContainer = new UnitTestMethodContainer(TestHarness, _instance, _testMethod.Method, _testMethod, TestGranularity.TestScenario);
            _mainTestMethodContainer.UnhandledException += new EventHandler<UnhandledExceptionEventArgs>(UnhandledMethodException);
            _mainTestMethodContainer.Complete += new EventHandler(CompleteMethod);
            Enqueue(_mainTestMethodContainer);

            // [TestCleanup]
            if (_testClass.TestCleanupMethod != null) 
            {
                EnqueueMethodDispatcher(_testClass.TestCleanupMethod);
            }

            // Log End
            EnqueueQuick(LogEndMessage);
        }

        /// <summary>
        /// Sets the start and finish times on the ScenarioResult object.
        /// </summary>
        private void SetResultTimes()
        {
            _result.Started = _started;
            _result.Finished = DateTime.Now;
        }

        /// <summary>
        /// Creates the ScenarioResult instance for this test method.
        /// </summary>
        /// <param name="outcome">The initial test outcome value.</param>
        private void CreateNewResult(TestOutcome outcome)
        {
            _result = new ScenarioResult(_testMethod, _testClass, outcome, null);
            SetResultTimes();
        }

        /// <summary>
        /// Process the result.
        /// </summary>
        /// <param name="sender">Source object.</param>
        /// <param name="e">Event arguments.</param>
        private void CompleteMethod(object sender, EventArgs e)
        {
            if (_testMethod.ExpectedException != null && _result == null)
            {
                CreateNewResult(TestOutcome.Failed);

                // Don't log this when the bug attribute is present
                if (_bugAttributePresent == false)
                {
                    LogWriter.NoExceptionWhenExpected(_testMethod.ExpectedException.ExceptionType, _testClass, _testMethod);
                }
            }
            if (_result == null) 
            {
                CreateNewResult(TestOutcome.Passed);
            }

            // Invert the result when the bug attribute is present
            if (_bugAttributePresent && !BugAttributeProcessed)
            {
                BugAttributeProcessed = true;
                bool bugVerified = _result.Result == TestOutcome.Failed;
                TestOutcome newOutcome = bugVerified ? TestOutcome.Passed : TestOutcome.Failed;
                _result.Result = newOutcome;
                
                // Log a warning when the bug cannot be verified
                if (!bugVerified)
                {
                    LogWriter.Warning(Properties.UnitTestMessage.TestMethodManager_CompleteMethod_UnVerifiedBug);
                }
            }

            LogWriter.TestResult(_result);

            // Store the result
            TestHarness.TrackScenarioResult(_result);

            // Overlaps, could consider fixing in a future release
            TestHarness.Events.SendTestMethodCompleted(new TestMethodCompletedEventArgs(_result));
        }

        /// <summary>
        /// Process an unhandled exception for the method.
        /// </summary>
        /// <param name="sender">Source object.</param>
        /// <param name="e">Unhandled exception event arguments.</param>
        private void UnhandledMethodException(object sender, UnhandledExceptionEventArgs e)
        {
            TestOutcome res;
            Exception excp = (Exception)e.ExceptionObject;

            IExpectedException expected = _testMethod.ExpectedException;
            Type expectedType = expected != null ? expected.ExceptionType : null;

            if (excp == null) 
            {
                throw new InvalidOperationException();
            }

            // Unwrap the Exception
            if (excp.GetType() == typeof(TargetInvocationException) && excp.InnerException != null) 
            {
                excp = excp.InnerException;
            }

            res = excp is TimeoutException ? TestOutcome.Timeout : excp.Message.Contains("Inconclusive") ? TestOutcome.Inconclusive : TestOutcome.Failed;
            if (expected != null)
            {
                // Was it expected?
                Type excpType = excp.GetType();
                if (excpType == expectedType || excpType.IsSubclassOf(expectedType))
                {
                    res = TestOutcome.Passed;
                }
                else
                {
                    LogWriter.IncorrectException(_testMethod.ExpectedException.ExceptionType, excpType, _testClass, _testMethod);
                    LogWriter.LogException(excp, _testClass, _testMethod);
                }
            }
            else
            {
                // Regular Exception type
                LogWriter.LogException(excp, _testClass, _testMethod);
            }

            // Create the result
            _result = new ScenarioResult(_testMethod, _testClass, res, excp);
            SetResultTimes();

            // If an asynchronous method, do not run any additional parts of the method container
            if (_mainTestMethodContainer.RemainingWork)
            {
                while (_mainTestMethodContainer.RemainingWork)
                {
                    _mainTestMethodContainer.Dequeue();
                }
            }
        }

        /// <summary>
        /// Create a new method container to enclose a reflected method for execution.
        /// </summary>
        /// <param name="method">The method reflection object.</param>
        private void EnqueueMethodDispatcher(MethodInfo method)
        {
            Enqueue(new UnitTestMethodContainer(TestHarness, _instance, method, null, TestGranularity.TestScenario));
        }
    }
}