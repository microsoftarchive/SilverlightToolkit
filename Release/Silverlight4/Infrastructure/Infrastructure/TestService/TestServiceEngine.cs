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
using System.Net;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Win32;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// The underlying service implementation.
    /// </summary>
    public class TestServiceEngine
    {
        /// <summary>
        /// The content type to use for HTML.
        /// </summary>
        private const string ContentTypeHtml = "text/html";

        /// <summary>
        /// The content type to use for unknown resources.
        /// </summary>
        private const string ContentTypeUnknown = "application/unknown";

        /// <summary>
        /// The content type HTTP header key name.
        /// </summary>
        private const string ContentTypeValueName = "Content Type";

        /// <summary>
        /// The content type to use for Silverlight XAPs.
        /// </summary>
        private const string ContentTypeXap = "application/x-silverlight-2";

        /// <summary>
        /// The content type to use for XML documents.
        /// </summary>
        private const string ContentTypeXml = "text/xml";

        /// <summary>
        /// The cross domain policy filename.
        /// </summary>
        private const string CrossDomainFilename = "clientaccesspolicy.xml";

        /// <summary>
        /// The external interface URI.
        /// </summary>
        private const string ExternalInterface = "externalInterface/";

        /// <summary>
        /// The URL format used for the prefix.
        /// </summary>
        private const string PrefixUrlFormat = "http://localhost:{0}/";

        /// <summary>
        /// The shutdown delay.
        /// </summary>
        private TimeSpan ShutdownDelay;

        /// <summary>
        /// Static instance of shared helper functions for the test service.
        /// </summary>
        private static TestFunctions Functions = new TestFunctions();

        /// <summary>
        /// The listener thread.
        /// </summary>
        private Thread _listenerThread;

        /// <summary>
        /// The forced shutdown time.
        /// </summary>
        private DateTime _shutdownTime = DateTime.MaxValue;

        /// <summary>
        /// Initializes a new instance of the test service.
        /// </summary>
        /// <param name="options">The test service options.</param>
        public TestServiceEngine(TestServiceOptions options)
        {
            ServerName = options.MachineName;
            Port = options.Port;
            ShutdownDelay = options.ShutdownDelay;
        }

        /// <summary>
        /// Begins the shutdown process by setting the shutdown time to the
        /// current time plus the delay.
        /// </summary>
        internal void BeginShutdownProcess()
        {
            _shutdownTime = DateTime.Now + ShutdownDelay;
        }

        /// <summary>
        /// Checks whether the directory is within another root directory.
        /// </summary>
        /// <param name="dir">The directory path to check.</param>
        /// <param name="root">The root to check from.</param>
        /// <returns>Returns true if the directory is within the root.</returns>
        private static bool DirectoryIsWithinDirectory(DirectoryInfo dir, DirectoryInfo root)
        {
            DirectoryInfo currentDirectory = dir;
            while ((currentDirectory != null) && (currentDirectory.FullName != root.FullName))
            {
                currentDirectory = currentDirectory.Parent;
            }
            return (currentDirectory != null);
        }

        /// <summary>
        /// Checks whether the directory is within another root directory.
        /// </summary>
        /// <param name="dir">The directory path to check.</param>
        /// <param name="root">The root to check from.</param>
        /// <returns>Returns true if the directory is within the root.</returns>
        private static bool DirectoryIsWithinDirectory(string dir, string root)
        {
            return DirectoryIsWithinDirectory(new DirectoryInfo(dir), new DirectoryInfo(root));
        }

        /// <summary>
        /// Checks whether the file is within a root directory.
        /// </summary>
        /// <param name="file">The path to check.</param>
        /// <param name="directory">The root directory.</param>
        /// <returns>Returns true if the directory is within the root.</returns>
        private static bool FileIsWithinDirectory(string file, string directory)
        {
            DirectoryInfo fileDirectory = new FileInfo(file).Directory;
            DirectoryInfo root = new DirectoryInfo(directory);
            return DirectoryIsWithinDirectory(fileDirectory, root);
        }

        /// <summary>
        /// Returns the local path from an HTTP listener request.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>Returns the local path.</returns>
        private static string GetLocalPath(HttpListenerRequest request)
        {
            string filePath = request.Url.LocalPath;
            if (!(string.IsNullOrEmpty(filePath) || (filePath[0] != '/')))
            {
                filePath = filePath.Substring(1, filePath.Length - 1);
            }
            return filePath;
        }

        /// <summary>
        /// Retrieves a simple MIME type from the path.
        /// </summary>
        /// <param name="path">The path whose extension will be used.</param>
        /// <returns>Returns the MIME type to use.</returns>
        private static string GetMimeType(string path)
        {
            string ext = new FileInfo(path).Extension;
            switch (ext)
            {
                case "xml":
                case "xaml":
                    return ContentTypeXml;

                case "xap":
                    return ContentTypeXap;
            }
            return GetMimeTypeFromRegistry(ext);
        }

        /// <summary>
        /// Retrieves the MIME type for an extension from the registry.
        /// </summary>
        /// <param name="ext">The extension to check, without the dot.</param>
        /// <returns>Returns the MIME type of an unknown MIME fallback value.</returns>
        private static string GetMimeTypeFromRegistry(string ext)
        {
            string mimeType = ContentTypeUnknown;
            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(ext);
            if ((regKey != null) && (regKey.GetValue(ContentTypeValueName) != null))
            {
                mimeType = regKey.GetValue(ContentTypeValueName).ToString();
            }
            return mimeType;
        }

        /// <summary>
        /// Lists the directory contents in a simple HTML format. Writes to the
        /// HTTP response.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="directory">The directory to list.</param>
        private static void ListDirectoryContents(HttpListenerResponse response, string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            SetContentType(response, ContentTypeHtml);
            using (StreamWriter sw = new StreamWriter(response.OutputStream))
            {
                sw.WriteLine("<html>");
                SendBasicHeader(sw, "Directory listing for " + di.FullName);
                sw.WriteLine("<body>");
                sw.WriteLine("<h2>Directory listing for {0}</h2>", di.FullName);
                sw.WriteLine("<hr /><ul>");
                foreach (FileInfo file in di.GetFiles())
                {
                    sw.WriteLine("<li>");
                    sw.WriteLine("\t<a href=\"{0}\">{0}</a>", file.Name);
                    sw.WriteLine("</li>");
                }
                sw.WriteLine("</ul>");
                SendInformationalFooter(sw);
                sw.WriteLine("</body>");
            }
        }

        /// <summary>
        /// Logs to the debug stream a message.
        /// </summary>
        /// <param name="value">The format or value.</param>
        /// <param name="o">Optional set of parameter obejcts.</param>
        private static void Log(string value, params object[] o)
        {
            string text = (o != null && o.Length == 0) ? value : string.Format(CultureInfo.InvariantCulture, value, o);
            Debug.WriteLine(text);
        }

        /// <summary>
        /// Process a GET request.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="response">The response object.</param>
        /// <param name="rootDirectory">The root directory for the service.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to keep requests going.")]
        private static void ProcessGetRequest(HttpListenerRequest request, HttpListenerResponse response, string rootDirectory)
        {
            try
            {
                if (GetLocalPath(request) == CrossDomainFilename)
                {
                    ServeCrossDomainFile(response);
                }
                else if ((request.Url.Segments.Length > 1) && (request.Url.Segments[1] == ExternalInterface))
                {
                    ServeFunction(request, response);
                }
                else if (rootDirectory != null)
                {
                    ServeFile(rootDirectory, request, response);
                }
                else
                {
                    SetHttpStatus(response, HttpStatusCode.NotFound);
                }
            }
            catch (HttpListenerException)
            {
            }
            catch
            {
            }
        }

        /// <summary>
        /// Process a POST request.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="response">The response object.</param>
        /// <param name="rootDirectory">The root directory for the service.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "rootDirectory", Justification = "This parameter may be needed in the future.")]
        private static void ProcessPostRequest(HttpListenerRequest request, HttpListenerResponse response, string rootDirectory)
        {
            string results = null;
            using (Stream input = request.InputStream)
            {
                using (StreamReader reader = new StreamReader(input))
                {
                    results = reader.ReadToEnd();
                }
            }
            if ((request.Url.Segments.Length > 1) && (request.Url.Segments[1] == ExternalInterface))
            {
                ServeFunction(request, response, results);
            }
        }

        /// <summary>
        /// Process a test run result and then begin the shutdown process.
        /// </summary>
        /// <param name="result">The test run result.</param>
        internal void ProcessResult(TestRunResult result)
        {
            Result = result;
            BeginShutdownProcess();
        }

        /// <summary>
        /// Store the results from the request.
        /// </summary>
        /// <param name="directory">The directory to store in.</param>
        /// <param name="request">The request object.</param>
        private static void SaveResults(string directory, HttpListenerRequest request)
        {
            Log("Saving test results", new object[0]);
            string results = null;
            using (Stream input = request.InputStream)
            {
                using (StreamReader reader = new StreamReader(input))
                {
                    results = reader.ReadToEnd();
                }
            }
            XElement coverage = XDocument.Parse(results).Root.Element("CodeCoverage");
            if (coverage != null)
            {
                string data = coverage.Value;
                if (data != null)
                {
                    data = data.Trim();
                }
                File.WriteAllText(Path.Combine(directory, "RawCodeCoverage.txt"), data);
            }
        }

        /// <summary>
        /// Sends a basic header back on the request wire.
        /// </summary>
        /// <param name="sw">The stream writer.</param>
        /// <param name="title">The title for the page.</param>
        private static void SendBasicHeader(StreamWriter sw, string title)
        {
            sw.Write(
@"<head>
<title>{0}</title>
<style type=""text/css"">
    body, p { font-family:Calibri,Arial; font-size:small; }
    #footer { text-align:left;color:#888; }
</style>
</head>",
            title);
        }

        /// <summary>
        /// Sends an HTML footer.
        /// </summary>
        /// <param name="sw">The stream writer.</param>
        private static void SendInformationalFooter(StreamWriter sw)
        {
            sw.Write(
@"<hr />
<div id=""footer"">
<p style=""text-align:center"">Silverlight Unit Test Framework<br />
<small><a href=""http://silverlight.codeplex.com/"">Silverlight Toolkit</a></small>
</p>\r\n\r\n<p><small>Copyright &copy; Microsoft Corporation. All Rights Reserved.<br />
<a href=""http://go.microsoft.com/fwlink/?LinkID=131993"">Microsoft Public License</a></p>

<p>
<strong>This product is in the Experimental quality band of the 
Silverlight Toolkit.</strong><br />

<small>This local web service is only meant for testing needs and may expose 
files, folders, and other information.
</small></p>

</div>");
        }

        /// <summary>
        /// Serves up the XML cross domain policy file.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        private static void ServeCrossDomainFile(HttpListenerResponse response)
        {
            SetHttpStatus(response, HttpStatusCode.OK);
            SetContentType(response, "text/xml");
            ServeString(response, TestServiceHelper.OpenClientAccessPolicy);
        }

        /// <summary>
        /// Save a file in the directory.
        /// </summary>
        /// <param name="directory">The directory to save in.</param>
        /// <param name="request">The request object.</param>
        /// <param name="response">The repsonse object.</param>
        private static void ServeFile(string directory, HttpListenerRequest request, HttpListenerResponse response)
        {
            string filePath = GetLocalPath(request);
            Log("Requested file {0}", new object[] { filePath });
            string path = Path.Combine(directory, filePath);
            response.Headers.Add("Cache-Control", "no-cache");
            if (Directory.Exists(path) && DirectoryIsWithinDirectory(path, directory))
            {
                ListDirectoryContents(response, path);
            }
            else if (!File.Exists(path))
            {
                Log("404 - File not found -" + path, new object[0]);
                SetHttpStatus(response, HttpStatusCode.NotFound);
            }
            else if (!FileIsWithinDirectory(path, directory))
            {
                SetHttpStatus(response, HttpStatusCode.Unauthorized);
                Log("401 - Unauthorized file location " + path, new object[0]);
            }
            else
            {
                Log("Writing file {0}", new object[] { path });
                SetContentType(response, GetMimeType(path));
                using (Stream output = response.OutputStream)
                {
                    using (FileStream input = File.OpenRead(path))
                    {
                        response.ContentLength64 = input.Length;
                        int read = 0;
                        byte[] buffer = new byte[0x800];
                        do
                        {
                            read = input.Read(buffer, 0, buffer.Length);
                            output.Write(buffer, 0, read);
                        }
                        while (read > 0);
                    }
                }
            }
        }

        /// <summary>
        /// Serve a function without any POST data.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="response">The response object.</param>
        private static void ServeFunction(HttpListenerRequest request, HttpListenerResponse response)
        {
            ServeFunction(request, response, null);
        }

        /// <summary>
        /// Serve a function.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="response">The response object.</param>
        /// <param name="postData">The raw HTTP POST data as a string.</param>
        private static void ServeFunction(HttpListenerRequest request, HttpListenerResponse response, string postData)
        {
            Debug.WriteLine(request.Url.ToString());
            string path = request.Url.AbsolutePath.Replace(ExternalInterface, string.Empty);
            if ((path.Length > 0) && (path[0] == '/'))
            {
                path = path.Substring(1);
            }
            if (path[path.Length - 1] == '/')
            {
                path = path.Substring(0, path.Length - 1);
            }
            List<string> data = path.Split(new char[] { '/' }).ToList<string>();
            SetContentType(response, ContentTypeXml);
            using (StreamWriter sw = new StreamWriter(response.OutputStream))
            {
                sw.Write(Functions.ProcessFunction(data, response, postData));
            }
        }

        /// <summary>
        /// Serve requests.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to keep the thread processing regardless.")]
        public void ServeRequests()
        {
            _listenerThread = new Thread(new ThreadStart(StartShutdownMonitorThread));
            _listenerThread.Start();
            Current = this;
            Listening = true;
            Server = new HttpListener();
            try
            {
                string prefix = string.Format(
                    CultureInfo.InvariantCulture,
                    PrefixUrlFormat,
                    Port);
                Server.Prefixes.Add(prefix);
                Server.Start();
                if (!string.IsNullOrEmpty(RootDirectory))
                {
                    Log("Starting server for {0}", new object[] { RootDirectory });
                }
                else
                {
                    Log("Starting server without a root directory", new object[0]);
                }
                Log("Listening on {0}", new object[] { prefix });
                while (Listening)
                {
                    Log("(Waiting for next request...)", new object[0]);
                    try
                    {
                        HttpListenerContext context = Server.GetContext();
                        HttpListenerResponse response = context.Response;
                        try
                        {
                            HttpListenerRequest request = context.Request;
                            Log("Received {0} request", new object[] { request.HttpMethod });
                            if (request.HttpMethod == "GET")
                            {
                                ProcessGetRequest(request, response, RootDirectory);
                            }
                            else if (request.HttpMethod == "POST")
                            {
                                ProcessPostRequest(request, response, RootDirectory);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            Log("Sending response", new object[0]);
                            response.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Log("Closing server...", new object[0]);
                try
                {
                    Server.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); 
                }
            }
        }

        /// <summary>
        /// Serve a string back through the stream.
        /// </summary>
        /// <param name="response">The response object.</param>
        /// <param name="value">The string value to write.</param>
        private static void ServeString(HttpListenerResponse response, string value)
        {
            response.Headers.Add("Cache-Control", "no-cache");
            using (Stream output = response.OutputStream)
            {
                using (StreamWriter sw = new StreamWriter(output))
                {
                    sw.Write(value);
                }
            }
        }

        /// <summary>
        /// Sets the content type on the HTTP response.
        /// </summary>
        /// <param name="response">The response object.</param>
        /// <param name="contentType">The content type.</param>
        private static void SetContentType(HttpListenerResponse response, string contentType)
        {
            response.Headers.Add("Content-type", contentType);
        }

        /// <summary>
        /// Sets the HTTP status.
        /// </summary>
        /// <param name="response">The response object.</param>
        /// <param name="status">The status to set.</param>
        private static void SetHttpStatus(HttpListenerResponse response, HttpStatusCode status)
        {
            response.StatusCode = (int)status;
        }

        /// <summary>
        /// Start the shutdown monitoring thread that will gracefully shut down
        /// by performing the final HTTP request after a shutdown has been
        /// initiated.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Improving thread robustness.")]
        private void StartShutdownMonitorThread()
        {
            while (Listening && (DateTime.Now < _shutdownTime))
            {
                Thread.Sleep(100);
            }
            if (Listening)
            {
                Listening = false;
            }
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    TestServiceHelper.PingService(HostName);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets or sets the current test service reference.
        /// </summary>
        internal static TestServiceEngine Current { get; set; }

        /// <summary>
        /// Gets the name name with port.
        /// </summary>
        public string HostName
        {
            get
            {
                return (ServerName + ":" + Port);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the service is currently
        /// listening.
        /// </summary>
        public bool Listening { get; set; }

        /// <summary>
        /// Gets or sets the log file path.
        /// </summary>
        public string LogFile { get; set; }

        /// <summary>
        /// Gets the port used for listening.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Gets or sets the test run result.
        /// </summary>
        public TestRunResult Result { get; set; }

        /// <summary>
        /// Gets or sets the root directory to use for hosting.
        /// </summary>
        public string RootDirectory { get; set; }

        /// <summary>
        /// Gets or sets the HTTP listener object.
        /// </summary>
        private HttpListener Server { get; set; }

        /// <summary>
        /// Gets the server name used for listening.
        /// </summary>
        public string ServerName { get; private set; }

        /// <summary>
        /// Gets or sets the tag expression in use for the run.
        /// </summary>
        public string TagExpression { get; set; }

        /// <summary>
        /// Gets or sets the test run prefix.
        /// </summary>
        public string TestRunPrefix { get; set; }
    }
}