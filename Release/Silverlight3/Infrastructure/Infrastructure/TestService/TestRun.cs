// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Manages a test run.
    /// </summary>
    public class TestRun
    {
        /// <summary>
        /// The default sleep amount while polling.
        /// </summary>
        private const int PollingThreadSleepMilliseconds = 40;

        /// <summary>
        /// The amount to sleep after execution.
        /// </summary>
        private const int FinalSleepDelayMilliseconds = 150;

        /// <summary>
        /// The standard hosting URL.
        /// </summary>
        private const string StandardLocalHostingUrl = "http://{0}:{1}/{2}?test_run={3}";

        /// <summary>
        /// Gets the number of failing scenarios.
        /// </summary>
        public int Failures { get; private set; }
        
        /// <summary>
        /// Gets the log file.
        /// </summary>
        public string Log { get; private set; }
        
        /// <summary>
        /// Gets or sets the test run options.
        /// </summary>
        public TestRunOptions Options { get; set; }
        
        /// <summary>
        /// Backing field for storing the test service options.
        /// </summary>
        private TestServiceOptions _testServiceOptions;

        /// <summary>
        /// Gets the test service instance.
        /// </summary>
        protected TestService TestService { get; private set; }
        
        /// <summary>
        /// Gets the total number of scenarios encountered.
        /// </summary>
        public int Total { get; private set; }
        
        /// <summary>
        /// Gets the web browser instance.
        /// </summary>
        protected WebBrowser WebBrowser { get; private set; }

        /// <summary>
        /// Gets the TestServiceOptions.
        /// </summary>
        public TestServiceOptions TestServiceOptions { get { return _testServiceOptions; } }

        /// <summary>
        /// Initializes a new instance of the TestRun type.
        /// </summary>
        /// <param name="serviceOptions">The test service options to use.</param>
        public TestRun(TestServiceOptions serviceOptions) : this(serviceOptions, new TestRunOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the TestRun type.
        /// </summary>
        /// <param name="serviceOptions">The service options to use.</param>
        /// <param name="runOptions">The test run options to apply.</param>
        public TestRun(TestServiceOptions serviceOptions, TestRunOptions runOptions)
        {
            _testServiceOptions = serviceOptions;
            TestService = new TestService(_testServiceOptions);
            Options = runOptions;
        }

        /// <summary>
        /// Executes a test run.
        /// </summary>
        /// <returns>Returns the result of the Silverlight Unit Test run.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to keep requests going.")]
        public bool Run()
        {
            try
            {
                Failures = -1;
                string page = Options.Page;
                string path = Options.LocalPath;
                if (File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    TestService.RootDirectory = fi.DirectoryName;
                    page = fi.Name;
                }
                else if (Directory.Exists(path))
                {
                    TestService.RootDirectory = path;
                    if (!File.Exists(Path.Combine(path, page)))
                    {
                        page = string.Empty;
                    }
                }
                TestService.TestRunPrefix = Options.Browser.ToString();
                TestService.Start();
                if (!string.IsNullOrEmpty(Options.TagExpression))
                {
                    TestService.TagExpression = Options.TagExpression;
                }
                if (!string.IsNullOrEmpty(Options.Log))
                {
                    TestService.LogFile = Options.Log;
                }

                WebBrowser = Options.BrowserInstance != null ? Options.BrowserInstance : WebBrowserFactory.Create(Options.Browser);

                Uri uri = new Uri(
                    string.Format(
                    CultureInfo.InvariantCulture,
                    StandardLocalHostingUrl,
                    _testServiceOptions.MachineName,
                    _testServiceOptions.Port,
                    page,
                    Options.RunId));

                WebBrowser.Start(uri);

                while (TestService.IsRunning && WebBrowser.IsRunning && TestService.Result == null)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(PollingThreadSleepMilliseconds));
                }

                TestRunResult result = TestService.Result;
                if (result != null)
                {
                    Total = result.Total;
                    Log = result.Log;
                    Failures = result.Failures;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(FinalSleepDelayMilliseconds));
                WebBrowser.Close();
                TestService.Stop();
            }
            return (Failures == 0);
        }
    }
}