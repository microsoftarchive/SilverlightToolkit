// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Launch a protected mode IE and provide the process ID.
    /// </summary>
    public static class ProtectedModeHelper
    {
        /// <summary>
        /// Launch Internet Explorer and return the PID. Requires Vista, Windows
        /// 7, or newer. An Exception will be thrown on platforms prior to it.
        /// </summary>
        /// <param name="uri">The url to navigate to. Providing null will 
        /// navigate the browser to the user's homepage.</param>
        /// <returns>Returns the IE process ID if successful, or 0.</returns>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "The application has an understood level of trust for the native code calls.")]
        public static int LaunchInternetExplorer(Uri uri)
        {
            string address = uri.ToString();

            if (Environment.OSVersion.Version.Major >= 6)
            {
                PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
                IELAUNCHURLINFO li = new IELAUNCHURLINFO();
                li.cbSize = Marshal.SizeOf(typeof(IELAUNCHURLINFO));
                if (SafeNativeMethods.IELaunchURL(address, ref pi, ref li) < 0)
                {
                    return 0;
                } 

                return pi.dwProcessId;
            }
            else
            {
                throw new NotSupportedException("Protected Mode requires Windows Vista, Windows 7, or later.");
            }
        }
    }
}