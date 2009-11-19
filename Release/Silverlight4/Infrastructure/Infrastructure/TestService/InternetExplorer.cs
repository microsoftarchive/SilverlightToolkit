// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
        /// Stores the initial Internet Explorer 8 recovery value.
        /// </summary>
        // private bool? _initialIe8RecoverySetting;

        /// <summary>
        /// Starts the web browser.
        /// </summary>
        /// <param name="uri">The URI to point the browser to upon startup.</param>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "This use is by design.")]
        public override void Start(Uri uri)
        {
            ////if (InternetExplorer8Helper.IsInternetExplorer8())
            ////{
            ////    bool initialSetting = InternetExplorer8Helper.GetAutomaticRecoveryValue();
            ////    _initialIe8RecoverySetting = initialSetting;

            ////    // Turn off automatic recovery if it's enabled
            ////    if (initialSetting)
            ////    {
            ////        WarnRecoveryUsersWithMessageBox();
            ////        InternetExplorer8Helper.SetAutomaticRecoveryValue(false);
            ////    }
            ////}

            Id = ProtectedModeHelper.LaunchInternetExplorer(uri);

            base.Start(uri);
        }

        /// <summary>
        /// Closes the web browser.
        /// </summary>
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
            catch (Exception)
            {
            }

            // Restore IE setting
            ////if (_initialIe8RecoverySetting != null)
            ////{
            ////    InternetExplorer8Helper.SetAutomaticRecoveryValue((bool)_initialIe8RecoverySetting);
            ////}
        }

        /// <summary>
        /// Warns the user about the detected automatic recovery mode.
        /// </summary>
        public static void WarnRecoveryUsersWithMessageBox()
        {
            if (InternetExplorer8Helper.IsInternetExplorer8() && InternetExplorer8Helper.GetAutomaticRecoveryValue())
            {
                // MessageBox.Show("Warning! Windows Internet Explorer 8's automatic crash recovery feature is currently turned on. This will affect the test execution experience.\n\nConsider running the process as an Administrator, or disabling Automatic Crash Recovery.\n\nTo disable Automatic Crash Recovery through the IE UI:\n- Open Internet Options\n- Open the Advanced tab\n- Uncheck the \"Enable Automatic Crash Recovery\" option and close the browser.\n\nTo set through the registry:\n- Open the registry editor\n- Navigate into HKCU\\Software\\Microsoft\\Internet Explorer\\Recovery\\\n- Set the AutoRecover value to \"0\" for Enabled, and \"2\" for disabled (recommended).", "Recovery mode warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}