// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Test service hosting implementation.
    /// </summary>
    public class TestService
    {
        /// <summary>
        /// Backing field for the log file location.
        /// </summary>
        private string _logFile;

        /// <summary>
        /// Backing field for the rooting path.
        /// </summary>
        private string _rootDirectory;

        /// <summary>
        /// The tage expression in use.
        /// </summary>
        private string _tagExpression;

        /// <summary>
        /// The prefix used for the test run.
        /// </summary>
        private string _testRunPrefix;

        /// <summary>
        /// Backing field for the service options.
        /// </summary>
        private TestServiceOptions _serviceOptions;

        /// <summary>
        /// Initializes a new instance of the TestService type. Once started,
        /// the service will listen on the provided parameters' socket.
        /// </summary>
        /// <param name="serviceOptions">The service options to use.</param>
        public TestService(TestServiceOptions serviceOptions)
        {
            _serviceOptions = serviceOptions;
        }

        /// <summary>
        /// Gets a value indicating whether the test service is still running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return Service != null && Service.Listening;
            }
        }

        /// <summary>
        /// Starts the test service hosting thread.
        /// </summary>
        public void Start()
        {
            if (Thread != null)
            {
                throw new InvalidOperationException("A thread is already present with the server.");
            }
            Thread = new Thread(new ThreadStart(StartupServer));
            Thread.Start();
        }

        /// <summary>
        /// Implementation of the startup routine for the host service.
        /// </summary>
        private void StartupServer()
        {
            Service = new TestServiceEngine(_serviceOptions)
            {
                RootDirectory = _rootDirectory,
                TagExpression = _tagExpression,
                LogFile = _logFile,
            };
            Service.ServeRequests();
        }

        /// <summary>
        /// Stops the host service.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to keep requests going.")]
        public void Stop()
        {
            if (Service.Listening)
            {
                Service.Listening = false;
                try
                {
                    try
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            TestServiceHelper.PingService(Service.HostName);
                        }
                    }
                    catch (WebException)
                    {
                    }
                }
                catch
                {
                }
            }
            Thread = null;
        }

        /// <summary>
        /// Gets or sets the log file.
        /// </summary>
        public string LogFile
        {
            get
            {
                return _logFile;
            }

            set
            {
                _logFile = value;
                if (Service != null)
                {
                    Service.LogFile = value;
                }
            }
        }

        /// <summary>
        /// Gets the test run result.
        /// </summary>
        public TestRunResult Result
        {
            get
            {
                if (Service == null)
                {
                    return null;
                }
                return Service.Result;
            }
        }

        /// <summary>
        /// Gets or sets the root directory.
        /// </summary>
        public string RootDirectory
        {
            get
            {
                return _rootDirectory;
            }
            set
            {
                _rootDirectory = value;
                if (Service != null)
                {
                    Service.RootDirectory = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the test service instance.
        /// </summary>
        private TestServiceEngine Service { get; set; }

        /// <summary>
        /// Gets or sets the tag expression.
        /// </summary>
        public string TagExpression
        {
            get
            {
                return _tagExpression;
            }

            set
            {
                _tagExpression = value;
                if (Service != null)
                {
                    Service.TagExpression = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the test run prefix.
        /// </summary>
        public string TestRunPrefix
        {
            get
            {
                return _testRunPrefix;
            }

            set
            {
                _testRunPrefix = value;
                if (Service != null)
                {
                    Service.TestRunPrefix = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the thread used for the service.
        /// </summary>
        private Thread Thread { get; set; }
    }
}