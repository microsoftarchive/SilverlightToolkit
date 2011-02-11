// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Microsoft.Silverlight.Testing.Tools;

namespace Microsoft.Silverlight.Toolkit.Build.Tasks
{
    /// <summary>
    /// A task that executes a unit test application by providing a set of 
    /// defined parameters to the test execution script.
    /// </summary>
    public partial class RunUnitTestsTask : Task
    {
        /// <summary>
        /// The Vstt namespace.
        /// </summary>
        private static readonly XNamespace VsttNamespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        /// <summary>
        /// Set of known build folder names.
        /// </summary>
        private static readonly List<string> _knownBuildFolderNames = new List<string>
        {
            "BIN",
            "DEBUG",
            "OBJ",
            "RELEASE",
            "CLIENTBIN",
        };

        /// <summary>
        /// Number of milliseconds to wait before kicking off the next
        /// asynchronous browser and its test.
        /// </summary>
        private const int AsynchronousKickOffDelay = 2500;

        /// <summary>
        /// Gets or sets the test pages to use.
        /// </summary>
        [Required]
        public ITaskItem[] TestPages { get; set; }

        /// <summary>
        /// Gets or sets the tag expression.
        /// </summary>
        public string TagExpression { get; set; }

        /// <summary>
        /// Gets or sets the timeout period in seconds. If set to 0, the 
        /// timeout parameter is not provided to the test script.
        /// </summary>
        public string Timeout { get; set; }

        /// <summary>
        /// Gets or sets the browser name. Valid values are "firefox", "ie", 
        /// and the absolute path to a web browser application.
        /// </summary>
        public string Browser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to treat test failures as 
        /// errors. They are treated as warnings typically.
        /// </summary>
        public bool TreatTestFailuresAsErrors { get; set; }

        /// <summary>
        /// Gets or sets the test results filename. If null, a default is
        /// selected.
        /// </summary>
        public string TestResultsFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to delete the log files from
        /// the test run.
        /// </summary>
        public bool DeleteLogFiles { get; set; }

        /// <summary>
        /// Initializes a new instance of the RunUnitTestsTask class.
        /// </summary>
        public RunUnitTestsTask()
        {
        }

        /// <summary>
        /// Run the set of test applications.
        /// </summary>
        /// <returns>A value indicating whether the task succeeded.</returns>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Understood when dealing with processes.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception is an error")]
        public override bool Execute()
        {
            // There's nothing to do if we have no source files
            if (TestPages == null || TestPages.Length == 0)
            {
                return true;
            }

            // Process the files
            bool succeeded = true;
            try
            {
                for (int i = 0; i < TestPages.Length; i++)
                {
                    string testPath = TestPages[i].ItemSpec;
                    FileInfo testApplication = new FileInfo(testPath);

                    // Make sure they didn't pass a directory as an item
                    if (Directory.Exists(testPath))
                    {
                        Log.LogError("Cannot move item {0} because it is a directory!", testPath);
                        succeeded = false;
                        continue;
                    }

                    // Make sure the source exists
                    if (!testApplication.Exists)
                    {
                        Log.LogError("Cannot process file {0} that does not exist!", testPath);
                        succeeded = false;
                        continue;
                    }

                    string testName = GetTestName(testApplication.Directory);
                    if (!string.IsNullOrEmpty(TagExpression))
                    {
                        testName += " (" + TagExpression + ")";
                    }

                    string name = TestResultsFile;
                    if (string.IsNullOrEmpty(name))
                    {
                        int k = 1;
                        name = "TestResults.trx";
                        while (File.Exists(Path.Combine(testApplication.DirectoryName, name)))
                        {
                            name = string.Format(CultureInfo.InvariantCulture, "TestResults{0}.trx", k++);
                        }
                    }
                    FileInfo log = new FileInfo(Path.Combine(testApplication.DirectoryName, name));

                    WebBrowserBrand wbb = WebBrowserFactory.ParseBrand(Browser);
                    TestRunOptions tro = new TestRunOptions
                    {
                        // StartupUri = testApplication.Name,
                        Browser = wbb,
                        LocalPath = testApplication.DirectoryName,
                        TagExpression = TagExpression,
                    };
                    tro.Page = testApplication.Name;
                    tro.Log = log.FullName;

                    if (wbb == WebBrowserBrand.Custom)
                    {
                        tro.SetCustomBrowser(Browser);
                    }
                    TestRun tr = new TestRun(
                        new TestServiceOptions(),
                        tro);
                    tr.Run();

                    // Interpret results
                    string pass = null;
                    string total = null;

                    if (log.Exists)
                    {
                        DisplayTestResults(log, ref total, ref pass);

                        if (DeleteLogFiles)
                        {
                            log.Delete();
                        }
                    }
                    else
                    {
                        Log.LogWarning(
                            "The log file {0} was never written by the test service for the {1} test.",
                            log.FullName,
                            testName);
                    }

                    if (tr.Total == 0)
                    {
                        Log.LogWarning("There were 0 reported scenarios executed. Check that the tag expression is appropriate.");
                    }
                    else if (tr.Failures == 0)
                    {
                        Log.LogMessage(
                            MessageImportance.High,
                            "Unit tests ({0}): {1}{2}",
                            testName,
                            pass != null ? " " + pass + " passing tests" : "",
                            total != null ? " " + total + " total tests" : "");
                    }
                    else
                    {
                        succeeded = false;
                        LogFailureMessage(
                            "Unit test failures in test " +
                            testName +
                            ", " +
                            tr.Failures.ToString(CultureInfo.CurrentUICulture) +
                            " failed scenarios out of " +
                            tr.Total.ToString(CultureInfo.CurrentUICulture) +
                            " total scenarios executed.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                succeeded = false;
            }

            return succeeded;
        }

        /// <summary>
        /// Logs information from the TRX log file, if present and parsed ok.
        /// </summary>
        /// <param name="log">The log file.</param>
        /// <param name="total">A string to hold any total information.</param>
        /// <param name="pass">A string to hold any pass information.</param>
        private void DisplayTestResults(FileInfo log, ref string total, ref string pass)
        {
            XName utr = VsttNamespace + "UnitTestResult";
            XName output = VsttNamespace + "Output";

            XDocument trx = XDocument.Parse(File.ReadAllText(log.FullName));

            XElement counters = trx.Root.Descendants(VsttNamespace + "Counters").FirstOrDefault();
            if (counters != null)
            {
                pass = counters.Attribute("passed") != null ? counters.Attribute("passed").Value : null;
                total = counters.Attribute("total") != null ? counters.Attribute("total").Value : null;
            }

            IEnumerable<XElement> failures = trx.Root
                .Descendants(utr)
                .Where(node => node.Attribute("outcome").Value == "Failed");
            foreach (XElement failure in failures)
            {
                Log.LogWarning("Test {0} failed.", failure.Attribute("testName").Value);
                foreach (XElement o in failure.Descendants(output))
                {
                    Log.LogMessage(o.ToString());
                }
            }
        }

        /// <summary>
        /// Guesses the possible test name by excluding common build folder
        /// names such as ClientBin, bin, debug, etc. Returns the best guess,
        /// for use as a nice name for the test.
        /// </summary>
        /// <param name="directory">The directory that contains the test XAP.</param>
        /// <returns>Returns the best guess for the test name.</returns>
        private static string GetTestName(DirectoryInfo directory)
        {
            DirectoryInfo dir = directory;
            string name;
            do
            {
                name = dir.Name.ToUpperInvariant();
                dir = dir.Parent;
            }
            while (_knownBuildFolderNames.Contains(name) || dir == null);

            return name;
        }

        /// <summary>
        /// Logs an overal test run failure as a warning or error.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void LogFailureMessage(string message)
        {
            if (TreatTestFailuresAsErrors)
            {
                Log.LogError(message);
            }
            else
            {
                Log.LogWarning(message);
            }
        }
    }
}