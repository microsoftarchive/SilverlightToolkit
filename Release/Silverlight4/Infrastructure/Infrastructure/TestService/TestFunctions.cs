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
using System.Net;
using System.Text;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Set of test functions.
    /// </summary>
    internal class TestFunctions
    {
        /// <summary>
        /// Low-level processing for a test service function.
        /// </summary>
        /// <param name="data">The test data.</param>
        /// <param name="response">The test response.</param>
        /// <param name="postData">The POST data string, optional.</param>
        /// <returns>Returns a result.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "response", Justification = "This parameter may be needed.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Keeping for compatibility reasons.")]
        public string ProcessFunction(List<string> data, HttpListenerResponse response, string postData)
        {
            string func = data[0];
            data.RemoveAt(0);
            Dictionary<string, string> request = new Dictionary<string, string>();
            
            while (data.Count > 1)
            {
                string key = data[0];
                string value = data[1];
                data.RemoveRange(0, 2);
                request[key] = value;
            }

            if (postData != null)
            {
                request["post"] = postData;
            }

            string path = Environment.CurrentDirectory;
            if (Directory.Exists(TestServiceEngine.Current.RootDirectory))
            {
                path = TestServiceEngine.Current.RootDirectory;
            }

            string guid = string.Empty;
            request.TryGetValue("guid", out guid);

            switch (func)
            {
                case "ping":
                    return ServiceHelper.OK("Hello!");

                case "reportTestResults":
                    {
                        TestRunResult trr = new TestRunResult();
                        trr.Log = string.Empty;
                        trr.Total = int.Parse(request["total"], CultureInfo.InvariantCulture);
                        trr.Failures = int.Parse(request["failures"], CultureInfo.InvariantCulture);
                        TestRunResult result = trr;

                        TestServiceEngine.Current.ProcessResult(result);
                        return ServiceHelper.OK(guid);
                    }

                case "getRunParameters":
                    {
                        StringBuilder str = new StringBuilder();
                        str.AppendLine("<testRun>");
                        str.AppendLine("<options>");
                        if (!string.IsNullOrEmpty(TestServiceEngine.Current.TagExpression))
                        {
                            str.AppendLine(string.Format(CultureInfo.InvariantCulture, "<option name=\"{0}\" value=\"{1}\" />", "tagExpression", ServiceHelper.UrlEncode(TestServiceEngine.Current.TagExpression)));
                        }
                        str.AppendLine(string.Format(CultureInfo.InvariantCulture, "<option name=\"{0}\" value=\"{1}\" />", "testRunNamePrefix", TestServiceEngine.Current.TestRunPrefix));
                        str.AppendLine(string.Format(CultureInfo.InvariantCulture, "<option name=\"{0}\" value=\"{1}\" />", "computerName", ServiceHelper.UrlEncode(Environment.MachineName)));
                        str.AppendLine(string.Format(CultureInfo.InvariantCulture, "<option name=\"{0}\" value=\"{1}\" />", "userName", ServiceHelper.UrlEncode(Environment.UserName)));
                        if (!string.IsNullOrEmpty(TestServiceEngine.Current.LogFile))
                        {
                            string logShortName = TestServiceEngine.Current.LogFile;
                            string dir = Path.GetDirectoryName(TestServiceEngine.Current.LogFile);
                            if (Directory.Exists(dir))
                            {
                                logShortName = Path.GetFileName(TestServiceEngine.Current.LogFile);
                            }
                            str.AppendLine(string.Format(CultureInfo.InvariantCulture, "<option name=\"{0}\" value=\"{1}\" />", "log", ServiceHelper.UrlEncode(logShortName)));
                        }
                        str.AppendLine("</options>");
                        str.AppendLine("<arguments>");
                        str.AppendLine("</arguments>");
                        str.AppendLine("</testRun>");
                        Debug.WriteLine("getRunParameters:");
                        Debug.WriteLine(str.ToString());
                        return ServiceHelper.OK(str.ToString());
                    }

                case "saveLogFile":
                    {
                        string logName = request["logName"];
                        FileInfo logInfo = new FileInfo(Path.Combine(path, logName));
                        if (!Directory.Exists(logInfo.DirectoryName))
                        {
                            logInfo = new FileInfo(logName);
                        }
                        if (!Directory.Exists(logInfo.DirectoryName))
                        {
                            Console.WriteLine("Could not find directory to store " + logName);
                            return ServiceHelper.Error("Could not store log file.");
                        }
                        else
                        {
                            File.WriteAllText(logInfo.FullName, postData);
                        }

                        return ServiceHelper.OK();
                    }

                    // Improved code coverage transport, base 64 - store as the
                    // simple bit character string for compatibility
                case "saveCodeCoverageBase64":
                    string ba = ByteArrayToBitString(Convert.FromBase64String(postData));
                    Console.WriteLine("Saving code coverage information...");
                    File.WriteAllText(Path.Combine(path, "RawCodeCoverage.txt"), ba);

                    return ServiceHelper.OK();

                    // Legacy code coverage format, simple string of raw 0 and 1
                    // characters over the wire. Highly inefficient.
                case "saveCodeCoverage":
                    Console.WriteLine("Saving code coverage information...");
                    File.WriteAllText(Path.Combine(path, "RawCodeCoverage.txt"), postData);

                    return ServiceHelper.OK();
            }
            Console.WriteLine("No implementation for " + func);
            return string.Empty;
        }

        /// <summary>
        /// Converts an array of bytes into a string containing simple bit
        /// characters.
        /// </summary>
        /// <param name="bytes">The set of bytes.</param>
        /// <returns>Returns a new string with the contained data.</returns>
        private static string ByteArrayToBitString(byte[] bytes)
        {
            StringBuilder s = new StringBuilder(bytes.Length * 16);
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                for (int bit = 0; bit < 8; bit++)
                {
                    byte flag = (byte)(1 << (7 - bit));
                    bool v = (flag & b) == flag;
                    s.Append(v ? '1' : '0');
                }
            }

            return s.ToString();
        }
    }
}