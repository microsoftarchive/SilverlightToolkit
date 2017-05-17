// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Silverlight.Testing.Harness;
using Microsoft.Silverlight.Testing.Service;
using Microsoft.Silverlight.Testing.UnitTesting.Metadata;

namespace Microsoft.Silverlight.Testing.Harness
{
    /// <summary>
    /// A test harness for interacting with unit test providers such as Visual 
    /// Studio Team Test's metadata.
    /// </summary>
    public class UnitTestHarness
    {
        /// <summary>
        /// Display name for this harness.
        /// </summary>
        internal const string HarnessName = "Unit Testing";

        /// <summary>
        /// Manages the attachment state of a global exception handler.
        /// </summary>
        private GlobalExceptionHandler _globalExceptions;

        /// <summary>
        /// Container of all work items for the test harness.
        /// </summary>
        private CompositeWorkItem _harnessTasks;

        /// <summary>
        /// Manager of the stack of dispatchers, so that the appropriate parent 
        /// container handles exceptions and completion events.
        /// </summary>
        private WorkItemsManager _dispatcherStack;

        /// <summary>
        /// Gets the list of results.
        /// </summary>
        public List<ScenarioResult> Results { get; private set; }

        /// <summary>
        /// Number of valid test assemblies encountered.
        /// </summary>
        private int _validTestAssemblies;

        /// <summary>
        /// The current run's known number of test methods.
        /// </summary>
        private int _knownTestMethods;

        /// <summary>
        /// Backing field for the event firing helper.
        /// </summary>
        private UnitTestHarnessEvents _events;

        /// <summary>
        /// Gets the log message writer instance.  This can be used to easily 
        /// post informative messages to the log message queue and providers.
        /// </summary>
        public UnitTestLogMessageWriter LogWriter { get; private set; }

        /// <summary>
        /// Gets or sets the logic factory used for instantiating the 
        /// unit test logic and management objects.
        /// </summary>
        public UnitTestLogicFactory LogicFactory { get; set; }

        /// <summary>
        /// Initiate unit test harness.
        /// </summary>
        public UnitTestHarness()
        {
            _events = new UnitTestHarnessEvents(this);
            State = new TestHarnessState();
            LogProviders = new List<LogProvider>();
            _queuedLogMessages = new Queue<LogMessage>();

            _globalExceptions = new GlobalExceptionHandler(GlobalUnhandledExceptionListener);
            _dispatcherStack = new WorkItemsManager();
            Results = new List<ScenarioResult>();
            LogWriter = new UnitTestLogMessageWriter(this);
            LogicFactory = new UnitTestLogicFactory(this);
        }

        /// <summary>
        /// Gets or sets the overall harness state - overloaded types can be 
        /// used to store additional information.
        /// </summary>
        public TestHarnessState State { get; set; }

        /// <summary>
        /// Gets the log providers list.
        /// </summary>
        public IList<LogProvider> LogProviders { get; private set; }

        /// <summary>
        /// Queue of log messages awaiting processing.
        /// </summary>
        private Queue<LogMessage> _queuedLogMessages;

        /// <summary>
        /// Gets or sets the wrapper that handles calling the next Run step 
        /// method until complete; allows for a virtual Run method.
        /// </summary>
        protected RunDispatcher RunDispatcher { get; set; }

        /// <summary>
        /// Gets the dictionary of Parameters passed into the test harness.
        /// </summary>
        public IDictionary<string, string> Parameters { get; private set; }

        /// <summary>
        /// Gets or sets the settings used to initialize the test harness.
        /// </summary>
        public UnitTestSettings Settings { get; set; }

        /// <summary>
        /// Gets a set of events that can be fired for test results and other
        /// important test runtime events.
        /// </summary>
        public UnitTestHarnessEvents Events
        {
            get { return _events; }
        }

        /// <summary>
        /// The test harness is publishing results.
        /// </summary>
        public event EventHandler Publishing;

        /// <summary>
        /// Gets the TestService referenced by the test harness settings. The 
        /// test service provides advanced, optional functionality that is 
        /// useful to harness and test case developers. A typical test service 
        /// operates outside the process or security boundary.
        /// </summary>
        public TestServiceProvider TestService
        {
            get
            {
                return Settings == null ? null : Settings.TestService;
            }
        }

        /// <summary>
        /// Adds a log provider to the listening log providers group.
        /// </summary>
        /// <param name="provider">Log provider object.</param>
        public void AddLogProvider(LogProvider provider)
        {
            LogProviders.Add(provider);
        }

        /// <summary>
        /// Enqueue a log message object for processing by the log providers.
        /// </summary>
        /// <param name="message">The log message object.</param>
        public void QueueLogMessage(LogMessage message)
        {
            _queuedLogMessages.Enqueue(message);
        }

        /// <summary>
        /// Begin running the test harness.
        /// </summary>
        /// <remarks>
        /// Make sure to subscribe to the Complete event before calling this 
        /// method, in some harnesses this may be a synchronous Run followed 
        /// immediately by the Complete event being fired.
        /// </remarks>
        public virtual void Run()
        {
            Initialize();

            // Initialize any providers that need access to the settings
            InitializeLogProviders();

            // Continue the run
            RestartRunDispatcher();
        }

        /// <summary>
        /// Complete event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "The protected method makes this testable.")]
        protected void RunDispatcherComplete(object sender, EventArgs e)
        {
            RunDispatcher = null;

            OnTestHarnessCompleted();
            PublishResults();
        }

        /// <summary>
        /// Stores a log file for the test run. Depending on the execution 
        /// environment, this call may not successful.
        /// </summary>
        /// <param name="logName">The name of the log file.</param>
        /// <param name="fileContent">The log file content as a string.</param>
        public virtual void WriteLogFile(string logName, string fileContent)
        {
            if (TestService != null && TestService.HasService(TestServiceFeature.TestReporting))
            {
                TestReportingProvider trp = TestService.GetService<TestReportingProvider>(TestServiceFeature.TestReporting);
                if (trp != null)
                {
                    trp.WriteLog(null, logName, fileContent);
                }
            }
        }

        /// <summary>
        /// If supported by any attached test service, this publishes the final
        /// test results. Typical harness implementations may immediately close
        /// the web browser channel upon receiving the message, so any other
        /// reporting should be done first.
        /// </summary>
        protected void PublishFinalResult()
        {
            // Final publish on the test service
            if (TestService != null && TestService.HasService(TestServiceFeature.TestReporting))
            {
                TestReportingProvider trp = TestService.GetService<TestReportingProvider>(TestServiceFeature.TestReporting);
                if (trp != null)
                {
                    int failures = State.Failures;
                    int total = State.TotalScenarios;
                    trp.ReportFinalResult(null, State.Failed, failures, total, "" /* message is not supported right now */);
                }
            }
        }

        /// <summary>
        /// Process all queued log messages.
        /// </summary>
        protected void ProcessLogMessages()
        {
            while (_queuedLogMessages != null && _queuedLogMessages.Count > 0)
            {
                LogMessage message = _queuedLogMessages.Dequeue();
                foreach (LogProvider provider in LogProviders)
                {
                    provider.Process(message);
                }
            }
        }

        /// <summary>
        /// Fill member variables with any non-null settings of the same type.
        /// </summary>
        /// <param name="settings">Settings container class.</param>
        private void InitializeSettings(UnitTestSettings settings)
        {
            if (settings != null)
            {
                if (settings.LogProviders != null)
                {
                    LogProviders = settings.LogProviders;
                }
                if (settings.Parameters != null)
                {
                    Parameters = settings.Parameters;
                }
                Settings = settings;
            }
        }

        /// <summary>
        /// Initializes all log providers.
        /// </summary>
        private void InitializeLogProviders()
        {
            if (LogProviders != null)
            {
                foreach (LogProvider provider in LogProviders)
                {
                    ITestSettingsLogProvider prov = provider as ITestSettingsLogProvider;
                    if (prov != null)
                    {
                        prov.Initialize(Settings);
                    }
                }
            }
        }

        /// <summary>
        /// Complete event fired when the test harness has finished its test 
        /// run.
        /// </summary>
        public event EventHandler<TestHarnessCompletedEventArgs> TestHarnessCompleted;

        /// <summary>
        /// Call the TestHarnessCompleted event.
        /// </summary>
        protected void OnTestHarnessCompleted()
        {
            var handler = TestHarnessCompleted;
            if (handler != null)
            {
                handler(this, new TestHarnessCompletedEventArgs(State));
            }
        }

        /// <summary>
        /// Call the Publishing event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected void OnPublishing(EventArgs e)
        {
            var handler = Publishing;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Overrides the PublishResults method so that final reporting is only
        /// done once all other logging is finished.
        /// </summary>
        protected virtual void PublishResults()
        {
            if (IsReportingTestServiceConnected())
            {
                SetOverallStatus("Reporting results...");
            }

            OnPublishing(EventArgs.Empty);

            // Publish the final results when all other reporting is done.
            PublishFinalResults();
        }

        /// <summary>
        /// Publish final results. If not yet ready, will keep waiting around
        /// as a work item until it is done.
        /// </summary>
        private void PublishFinalResults()
        {
            if (TestService != null && TestService.BusyServiceReferenceCounter > 0)
            {
                if (_harnessTasks == null)
                {
                    _harnessTasks = new CompositeWorkItem();
                }
                _harnessTasks.Enqueue(new CallbackWorkItem(() => { }));
                _harnessTasks.Enqueue(new CallbackWorkItem(PublishFinalResults));
                if (RunDispatcher == null)
                {
                    RunDispatcher = RunDispatcher.Create(RunNextStep, Dispatcher);
                    RunDispatcher.Run();
                }
            }
            else
            {
                _harnessTasks = null;
                RunDispatcher = null;
                PublishFinalResult();

                if (IsReportingTestServiceConnected())
                {
                    SetOverallStatus("Reporting complete...");
                }
            }
        }

        /// <summary>
        /// Stored dispatcher instance.
        /// </summary>
        private Dispatcher _dispatcher;

        /// <summary>
        /// Gets a dispatcher instance.
        /// </summary>
        protected Dispatcher Dispatcher
        {
            get
            {
                if (_dispatcher == null)
                {
                    UIElement uie = TestPage as UIElement;
                    if (uie != null)
                    {
                        _dispatcher = uie.Dispatcher;
                    }
                    else if (Application.Current != null && Application.Current.RootVisual != null)
                    {
                        _dispatcher = Application.Current.RootVisual.Dispatcher;
                    }
                    if (_dispatcher == null)
                    {
                        var r = new System.Windows.Shapes.Rectangle();
                        _dispatcher = r.Dispatcher;
                    }
                }

                return _dispatcher;
            }
        }

        /// <summary>
        /// Checks if a reporting provider is connected to the test service.
        /// </summary>
        /// <returns>Returns true if a reporting provider is connected to the
        /// test service.</returns>
        private bool IsReportingTestServiceConnected()
        {
            return TestService != null && TestService.HasService(TestServiceFeature.TestReporting);
        }

        /// <summary>
        /// Immediately sets the overall status using a log message and
        /// processes the message queue.
        /// </summary>
        /// <param name="message">The message to set.</param>
        private void SetOverallStatus(string message)
        {
            LogMessage lm = new LogMessage(LogMessageType.TestInfrastructure);
            lm.Message = message;
            lm.Decorators["UpdateOverallStatus"] = true;
            LogWriter.Enqueue(lm);
            ProcessLogMessages();
        }

        /// <summary>
        /// Sets the unit test harness property for a test case that inherits 
        /// from the abstract base type 'CustomTest'.
        /// </summary>
        /// <param name="customTest">A CustomText instance.</param>
        public void PrepareCustomTestInstance(CustomFrameworkUnitTest customTest)
        {
            customTest.UnitTestHarness = this;
        }

        /// <summary>
        /// Gets the root container for test work to be completed.
        /// </summary>
        public CompositeWorkItem RootCompositeWorkItem
        { 
            get { return _harnessTasks; }
        }

        /// <summary>
        /// Gets the known number of test methods in the current test run.
        /// </summary>
        public int TestMethodCount 
        {
            get { return _knownTestMethods; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to intercept exceptions at 
        /// the app domain level and funnel into the current container or not.
        /// </summary>
        public bool InterceptAllExceptions
        {
            get
            { 
                return _globalExceptions.AttachGlobalHandler;
            }
            set 
            {
                _globalExceptions.AttachGlobalHandler = value;
            }
        }

        /// <summary>
        /// Gets the internal DispatcherStack being used by the test harness.
        /// </summary>
        public WorkItemsManager DispatcherStack 
        { 
            get { return _dispatcherStack; }
        }

        /// <summary>
        /// Initialize the harness with a set of test assemblies.
        /// </summary>
        public virtual void Initialize()
        {
            Publishing += (sender, args) => ReportCodeCoverage(TestService);

            // Create the initial dispatcher tasks
            CreateHarnessTasks();

            if (Settings != null)
            {
                InitializeSettings(Settings);
            }

            // Attach to the unhandled exception handler
            InterceptAllExceptions = true;

            LogWriter.TestInfrastructure(Properties.UnitTestMessage.UnitTestHarness_Initialize_UnitTestHarnessInitialize);
            PrepareTestAssemblyTasks();
        }

        /// <summary>
        /// Restarts the run dispatcher.
        /// </summary>
        public virtual void RestartRunDispatcher()
        {
            if (_harnessTasks == null)
            {
                CreateHarnessTasks();
            }

            RunDispatcher = RunDispatcher.Create(RunNextStep, Dispatcher);
            RunDispatcher.Complete += new EventHandler(RunDispatcherComplete);
            RunDispatcher.Run();
        }

        /// <summary>
        /// Track the results for our execution and also track the fail state.
        /// </summary>
        /// <param name="result">Scenario result to process.</param>
        public void TrackScenarioResult(ScenarioResult result)
        {
            Results.Add(result);
            State.IncrementTotalScenarios();

            if (result.Result != TestOutcome.Passed)
            {
                State.IncrementFailures();
            }
        }

        /// <summary>
        /// The test assembly starting event.
        /// </summary>
        public event EventHandler<TestAssemblyStartingEventArgs> TestAssemblyStarting;

        /// <summary>
        /// Fires the test assembly starting event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected internal virtual void OnTestAssemblyStarting(TestAssemblyStartingEventArgs e)
        {
            var handler = TestAssemblyStarting;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// On the play or pause change of the dispatcher.
        /// </summary>
        public event EventHandler IsDispatcherRunningChanged;

        /// <summary>
        /// Fires the play pause event.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected virtual void OnIsDispatcherRunningChanged(EventArgs e)
        {
            var handler = IsDispatcherRunningChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dispatcher is currently
        /// running.
        /// </summary>
        public bool IsDispatcherRunning
        {
            get
            {
                if (RunDispatcher != null)
                {
                    return RunDispatcher.IsRunning;
                }

                return false;
            }

            set
            {
                if (RunDispatcher != null)
                {
                    RunDispatcher.IsRunning = value;
                    OnIsDispatcherRunningChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Test assembly completed event.
        /// </summary>
        public event EventHandler<TestAssemblyCompletedEventArgs> TestAssemblyCompleted;

        /// <summary>
        /// Fires the test assembly completed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected internal virtual void OnTestAssemblyCompleted(TestAssemblyCompletedEventArgs e)
        {
            var handler = TestAssemblyCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Test class starting event.
        /// </summary>
        public event EventHandler<TestClassStartingEventArgs> TestClassStarting;

        /// <summary>
        /// Fires the test class starting event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected internal virtual void OnTestClassStarting(TestClassStartingEventArgs e)
        {
            var handler = TestClassStarting;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// The test class completed event.
        /// </summary>
        public event EventHandler<TestClassCompletedEventArgs> TestClassCompleted;

        /// <summary>
        /// Fires the test class completed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected internal virtual void OnTestClassCompleted(TestClassCompletedEventArgs e)
        {
            var handler = TestClassCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// The test method starting event.
        /// </summary>
        public event EventHandler<TestMethodStartingEventArgs> TestMethodStarting;

        /// <summary>
        /// Fires the test method starting event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected internal virtual void OnTestMethodStarting(TestMethodStartingEventArgs e)
        {
            var handler = TestMethodStarting;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// An event that is fired whenever a test method completes.
        /// </summary>
        public event EventHandler<TestMethodCompletedEventArgs> TestMethodCompleted;

        /// <summary>
        /// Notifies observers that a test method has been completed. Also
        /// clears the test panel's visual tree.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected internal virtual void OnTestMethodCompleted(TestMethodCompletedEventArgs e)
        {
            var handler = TestMethodCompleted;
            if (handler != null)
            {
                handler(this, e);
            }

            // Clear the test panel
            if (TestPanelManager != null)
            {
                TestPanelManager.ClearUsedChildren();
            }
        }

        /// <summary>
        /// An event that is fired when the test run is starting.
        /// </summary>
        public event EventHandler<TestRunStartingEventArgs> TestRunStarting;

        /// <summary>
        /// Notifies observers that a test run has been started.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected internal virtual void OnTestRunStarting(TestRunStartingEventArgs e)
        {
            #region Legacy logging code

            // Log the filter information
            if (LogWriter != null)
            {
                LogWriter.TestRunFilterSelected(e.TestRunFilter);
            }

            if (e.EnqueuedAssemblies > 0)
            {
                LogWriter.UnitTestHarnessStage(this, HarnessName, TestStage.Starting);
            }
            else
            {
                LogWriter.Warning(Properties.UnitTestMessage.UnitTestHarness_TestAssembliesNotActionable);
            }

            #endregion

            var handler = TestRunStarting;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Reference to the test page object.
        /// </summary>
        private ITestPage _testPage;

        /// <summary>
        /// Gets or sets the test page.
        /// </summary>
        public ITestPage TestPage
        {
            get { return _testPage; }
            set
            {
                _testPage = value;

                if (TestPanelManager == null)
                {
                    TestPanelManager = new TestPanelManager();
                }

                TestPanelManager.TestPage = _testPage;
            }
        }

        /// <summary>
        /// Gets the test panel manager instance.
        /// </summary>
        public TestPanelManager TestPanelManager { get; private set; }

        /// <summary>
        /// Attempts to report the code coverage information using the test 
        /// service provider. If there is no available coverage reporting 
        /// service, this is a silent failure. Only reports if >= 1 blocks 
        /// are hit.
        /// </summary>
        /// <param name="testService">The test service.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Allows for future extensibility.")]
        private void ReportCodeCoverage(TestServiceProvider testService)
        {
            if (CodeCoverage.HitBlockCount > 0 && testService != null && testService.HasService(TestServiceFeature.CodeCoverageReporting))
            {
                CodeCoverageProvider ccp = testService.GetService<CodeCoverageProvider>(TestServiceFeature.CodeCoverageReporting);
                if (ccp != null)
                {
                    string data = CodeCoverage.GetCoverageData();
                    ccp.SaveCoverageData(data, /* no callback Action */ null);
                }
            }
        }

        /// <summary>
        /// Enqueue a test assembly from a simple Assembly reference.
        /// </summary>
        /// <param name="assembly">The test assembly.</param>
        /// <param name="runFilter">The run filter settings for the test assembly's run.</param>
        public void EnqueueTestAssembly(Assembly assembly, TestRunFilter runFilter)
        {
            IAssembly testAssembly = UnitTestProviders.GetAssemblyWrapper(this, assembly);
            if (testAssembly != null)
            {
                EnqueueTestAssembly(testAssembly, runFilter);
            }
        }

        /// <summary>
        /// Enqueues a test assembly.
        /// </summary>
        /// <param name="testAssembly">The test assembly metadata.</param>
        /// <param name="runFilter">The run filter settings for the test assembly's run.</param>
        public void EnqueueTestAssembly(IAssembly testAssembly, TestRunFilter runFilter)
        {
            AssemblyManager assemblyManager = LogicFactory.CreateAssemblyManager(testAssembly.Provider, runFilter, testAssembly);
            _harnessTasks.Enqueue(assemblyManager);
            _knownTestMethods += CalculateTotalMethods(assemblyManager, testAssembly, runFilter);
            ++_validTestAssemblies;
        }

        /// <summary>
        /// Flush the current log manager and then perform the next invoke.
        /// </summary>
        /// <returns>Returns true if work remains.</returns>
        protected virtual bool RunNextStep()
        {
            ProcessLogMessages();
            if (RootCompositeWorkItem == null)
            {
                return false;
                // This can automatically complete instead of throwing.
                // throw new InvalidOperationException(Properties.UnitTestMessage.UnitTestHarness_RunNextStep_NoCompositeWorkItemsExist);
            }
            return RootCompositeWorkItem.Invoke();
        }

        /// <summary>
        /// Creates the test run filter for the initial run.
        /// </summary>
        /// <param name="settings">The unit test settings.</param>
        /// <returns>Returns a new TestRunFilter instance.</returns>
        protected virtual TestRunFilter CreateTestRunFilter(UnitTestSettings settings)
        {
            return String.IsNullOrEmpty(settings.TagExpression) ? new TestRunFilter(settings, this) : new TagTestRunFilter(settings, this, settings.TagExpression);
        }

        /// <summary>
        /// Determine what test assemblies need to be executed. Enqueue tasks 
        /// for the unit test assembly providers to run the tests.
        /// </summary>
        private void PrepareTestAssemblyTasks()
        {
            UnitTestSettings settings = Settings;
            TestRunFilter filter = CreateTestRunFilter(settings);

            foreach (Assembly assembly in Settings.TestAssemblies)
            {
                EnqueueTestAssembly(assembly, filter);
            }

            TestRunStartingEventArgs startingArguments = new TestRunStartingEventArgs(settings, filter)
            {
                TestHarnessName = HarnessName,
                EnqueuedAssemblies = _validTestAssemblies,
            };

            OnTestRunStarting(startingArguments);
        }

        /// <summary>
        /// Calculates the number of methods for a run.
        /// </summary>
        /// <param name="assemblyManager">The assembly manager.</param>
        /// <param name="assembly">The test assembly.</param>
        /// <param name="filter">The test run filter.</param>
        /// <returns>Returns the number of known methods returned.</returns>
        private static int CalculateTotalMethods(AssemblyManager assemblyManager, IAssembly assembly, TestRunFilter filter)
        {
            int total = 0;
            List<ITestClass> cls = filter.GetTestClasses(assembly, assemblyManager.ClassInstances);
            foreach (ITestClass test in cls)
            {
                object instance = assemblyManager.ClassInstances.GetInstance(test.Type);
                total += filter.GetTestMethods(test, instance).Count;
            }
            return total;
        }

        /// <summary>
        /// Event fired at the completion of the harness' work.
        /// </summary>
        /// <param name="sender">Sender object instance.</param>
        /// <param name="e">Event arguments.</param>
        private void HarnessComplete(object sender, EventArgs e)
        {
            LogWriter.UnitTestHarnessStage(this, HarnessName, TestStage.Finishing);
            _harnessTasks = null;
        }

        /// <summary>
        /// Listener event for any unhandled exceptions.
        /// </summary>
        /// <param name="sender">Sender object instance.</param>
        /// <param name="e">Event arguments.</param>
        private void GlobalUnhandledExceptionListener(object sender, EventArgs e)
        {
            if (DispatcherStack.CurrentCompositeWorkItem is CompositeWorkItem)
            {
                CompositeWorkItem cd = (CompositeWorkItem)DispatcherStack.CurrentCompositeWorkItem;
                Exception exception = GlobalExceptionHandler.GetExceptionObject(e);
                cd.WorkItemException(exception);
                GlobalExceptionHandler.ChangeExceptionBubbling(e, /* handled */ true);
            }
            else
            {
                GlobalExceptionHandler.ChangeExceptionBubbling(e, /* handled */ false);
            }
        }

        /// <summary>
        /// Creates the set of harness tasks to run and hooks up to the Complete event.
        /// </summary>
        private void CreateHarnessTasks()
        {
            _harnessTasks = new CompositeWorkItem();
            _harnessTasks.Complete += HarnessComplete;
        }
    }
}