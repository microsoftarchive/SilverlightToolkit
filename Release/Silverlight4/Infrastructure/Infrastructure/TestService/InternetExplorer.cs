// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Internet Explorer test automation implementation.
    /// </summary>
    /// <remarks>We've kept the implementation process-based to allow for
    /// the easiest control.</remarks>
    public class InternetExplorer : WebBrowserProcess
    {
        /// <summary>
        /// Starts the web browser.
        /// </summary>
        /// <param name="uri">The URI to point the browser to upon startup.</param>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "This use is by design.")]
        public override void Start(Uri uri)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                Id = ProtectedModeHelper.LaunchInternetExplorer(uri);
            }
            else
            {
                string path = Path.Combine(Environment.GetEnvironmentVariable("PROGRAMFILES"), "Internet Explorer");
                Executable = Path.Combine(path, "iexplore.exe");
            }

            base.Start(uri);
        }

        /// <summary>
        /// Closes the web browser.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to keep the run going regardless.")]
        public override void Close()
        {
            try
            {
                bool smartClose = false;
                if (Process != null && InternetExplorer8Helper.IsInternetExplorer8())
                {
                    Process host = InternetExplorer8Helper.FindTabHostingProcess(Process);
                    if (host != null)
                    {
                        host.CloseMainWindow();
                        smartClose = true;
                    }
                }

                if (!smartClose)
                {
                    if (Process.MainWindowHandle != IntPtr.Zero)
                    {
                        Process.CloseMainWindow();
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(.3));
                        if (!Process.HasExited)
                        {
                            base.Close();
                        }
                    }
                    else
                    {
                        base.Close();
                    }
                }
            }
            catch
            {
            }
        }
    }
}