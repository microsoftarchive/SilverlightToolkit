// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Set of options for the test run.
    /// </summary>
    public class TestRunOptions
    {
        /// <summary>
        /// The default browser brand.
        /// </summary>
        private const WebBrowserBrand DefaultBrand = WebBrowserBrand.InternetExplorer;

        /// <summary>
        /// The filename to store results in.
        /// </summary>
        private const string DefaultTestResultsFilename = "TestResults.trx";

        /// <summary>
        /// The filename of the HTML test page.
        /// </summary>
        private const string DefaultTestPageFilename = "TestPage.html";

        /// <summary>
        /// Gets or sets the browser type.
        /// </summary>
        public WebBrowserBrand Browser { get; set; }

        /// <summary>
        /// Gets or sets the web browser instance. Advanced version of the
        /// WebBrowserBrand property.
        /// </summary>
        public WebBrowser BrowserInstance { get; set; }

        /// <summary>
        /// Gets or sets the local path for the run.
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        public string Log { get; set; }

        /// <summary>
        /// Gets a dictionary of properties provided to the Silverlight Unit
        /// Test Framework instance.
        /// </summary>
        public IDictionary<string, string> Properties { get; private set; }

        /// <summary>
        /// Gets or sets the token used to identify the run.
        /// </summary>
        public string RunId { get; set; }

        /// <summary>
        /// Gets or sets the startup URI.
        /// </summary>
        public Uri StartupUri { get; set; }

        /// <summary>
        /// Gets or sets the tag expression.
        /// </summary>
        public string TagExpression { get; set; }

        /// <summary>
        /// Gets or sets the timeout for the overall run.
        /// </summary>
        public DateTime Timeout { get; set; }

        /// <summary>
        /// Gets or sets the filename of the test page itself.
        /// </summary>
        public string Page { get; set; }

        /// <summary>
        /// Initializes a new instance of the TestRunOptions type.
        /// </summary>
        public TestRunOptions()
        {
            Page = DefaultTestPageFilename;
            Browser = DefaultBrand;
            RunId = Guid.NewGuid().ToString();
            Timeout = DateTime.MaxValue;
            Properties = new Dictionary<string, string>();
            LocalPath = Environment.CurrentDirectory;
            Log = Path.Combine(LocalPath, DefaultTestResultsFilename);
        }

        /// <summary>
        /// Creates a custom browser.
        /// </summary>
        /// <param name="path">The path to the browser.</param>
        public void SetCustomBrowser(string path)
        {
            Browser = WebBrowserBrand.Custom;
            BrowserInstance = new WebBrowserProcess(path);
        }

        /// <summary>
        /// Creates a custom browser.
        /// </summary>
        /// <param name="instance">The web browser instance.</param>
        public void SetCustomBrowser(WebBrowser instance)
        {
            Browser = WebBrowserBrand.Custom;
            BrowserInstance = instance;
        }
    }
}