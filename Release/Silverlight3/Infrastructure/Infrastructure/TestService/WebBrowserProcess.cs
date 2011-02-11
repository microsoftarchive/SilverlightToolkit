// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// A simple web browser wrapper that allows any simple process to be used
    /// as a web browser instance if it takes a URL in the startup parameters.
    /// </summary>
    public class WebBrowserProcess : WebBrowser
    {
        /// <summary>
        /// Path to the browser.
        /// </summary>
        private string _exe;

        /// <summary>
        /// Initializes a new instance of the WebBrowserProcess type.
        /// </summary>
        public WebBrowserProcess()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the WebBrowserProcess type with a
        /// provided path to the web browser.
        /// </summary>
        /// <param name="path">The path to the executable on the system.</param>
        public WebBrowserProcess(string path)
            : this()
        {
            Executable = path;
        }

        /// <summary>
        /// Gets or sets the location of the web browser process.
        /// </summary>
        protected string Executable
        {
            get
            {
                return _exe;
            }

            set
            {
                if (!File.Exists(value))
                {
                    throw new FileNotFoundException("The web browser could not be located on the system.", value);
                }

                _exe = value;
            }
        }

        /// <summary>
        /// Gets or sets the process instance.
        /// </summary>
        protected Process Process { get; set; }

        /// <summary>
        /// Backing field for the process ID of the browser.
        /// </summary>
        private int? _pid;

        /// <summary>
        /// Gets a value indicating whether the browser process is currently
        /// running.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to keep requests going.")]
        public override bool IsRunning
        {
            get
            {
                try
                {
                    return ((Process != null) && !Process.HasExited);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the process id.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to keep requests going.")]
        protected int? Id
        {
            get { return _pid; }
            set
            {
                _pid = value;
                if (_pid != null)
                {
                    try
                    {
                        Process = Process.GetProcessById((int)_pid);
                    }
                    catch
                    {
                        Process = null;
                    }
                }
                else
                {
                    Process = null;
                }
            }
        }

        /// <summary>
        /// Retrieves the command line arguments to use when starting the
        /// executable.
        /// </summary>
        /// <param name="uri">The web address to browse to.</param>
        /// <returns>Returns the command line arguments.</returns>
        protected virtual string GetCommandLineArguments(Uri uri)
        {
            return uri.ToString();
        }

        /// <summary>
        /// Starts the web browser pointing to a particular address.
        /// </summary>
        /// <param name="uri">The startup URI.</param>
        public override void Start(Uri uri)
        {
            // Start the browser process
            if (Process == null && !string.IsNullOrEmpty(_exe))
            {
                ProcessStartInfo psi = new ProcessStartInfo(
                    _exe,
                    GetCommandLineArguments(uri));
                Process = Process.Start(psi);
            }
        }

        /// <summary>
        /// Closes the process.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to keep requests going.")]
        public override void Close()
        {
            try
            {
                TimeSpan pollDelta = TimeSpan.FromMilliseconds(100);
                if (Process != null)
                {
                    // Not the cleanest implementation ever, will look to 
                    // improve as time allows. Inconsistent performance on
                    // terminal services.
                    if (Process.MainWindowHandle != IntPtr.Zero)
                    {
                        // 3 seconds worth of attempts
                        for (int i = 0; i < 30; i++)
                        {
                            if (Process == null)
                            {
                                break;
                            }
                            if (Process.HasExited)
                            {
                                break;
                            }

                            Process.CloseMainWindow();

                            Thread.Sleep(pollDelta);
                        }
                    }
                }

                if (Process != null && !Process.HasExited)
                {
                    Process.Close();
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
                if (Process != null && !Process.HasExited)
                {
                    Process.Kill();
                }
            }
            catch (Win32Exception)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch
            {
            }
            finally
            {
                Console.WriteLine("Browser closed.");
            }
        }
    }
}