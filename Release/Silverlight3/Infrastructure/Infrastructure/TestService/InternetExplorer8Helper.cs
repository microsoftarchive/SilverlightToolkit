// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Win32;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// A set of Internet Explorer 8 helper methods.
    /// </summary>
    public static class InternetExplorer8Helper
    {
        // Internally, the recovery value in decimal is stored as:
        // - 2: disable
        // - 0: enabled

        /// <summary>
        /// The registry key that stores the recovery information.
        /// </summary>
        private const string RecoveryRegistryKey = @"Software\Microsoft\Internet Explorer\Recovery";

        /// <summary>
        /// The recovery value name.
        /// </summary>
        private const string RecoveryValueName = "AutoRecover";

        /// <summary>
        /// Makes a good faith effort to return the tab hosting process, a
        /// iexplore process with a window title, based on a small delta around
        /// the tab process start time.
        /// </summary>
        /// <param name="tabProcess">The process of the tab.</param>
        /// <returns>Returns a match, if any.</returns>
        public static Process FindTabHostingProcess(Process tabProcess)
        {
            DateTime tabStartTime = tabProcess.StartTime;
            return (
                from p in Process.GetProcessesByName("iexplore")
                where (p.MainWindowHandle != IntPtr.Zero) && (p.Id != tabProcess.Id)
                let diff = Math.Abs((p.StartTime - tabStartTime).TotalMilliseconds)
                orderby diff
                select p)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets a value indicating whether the machine appears to have Internet
        /// Explorer 8 or newer, supporting the recovery feature.
        /// </summary>
        /// <returns>Returns true when the machine supports tab recovery.</returns>
        public static bool IsInternetExplorer8()
        {
            RegistryKey key = GetRecoveryRegistryKey();
            return (key != null);
        }

        /// <summary>
        /// Retrieves the current value of the automatic recovery feature.
        /// </summary>
        /// <returns>Returns true when the recovery mode is on, true assumed
        /// as the default otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "The Get prefix indicates that there may be an exceptional amount of work performed in this method.")]
        public static bool GetAutomaticRecoveryValue()
        {
            RegistryKey key = GetRecoveryRegistryKey();
            if (key != null)
            {
                object o = key.GetValue(RecoveryValueName);
                if (o != null && o is int)
                {
                    int value = (int)o;
                    return (value == 0);
                }
            }

            return true;
        }

        /// <summary>
        /// Retrieves the registry key instance for recovery.
        /// </summary>
        /// <returns>Returns a registry key instance from the current user hive.</returns>
        private static RegistryKey GetRecoveryRegistryKey()
        {
            if (UserAccountControlHelper.WindowsSupportsUserAccountControl())
            {
                return Registry.CurrentUser.OpenSubKey(RecoveryRegistryKey);
            }

            return null;
        }

        /// <summary>
        /// Sets the automatic recovery value.
        /// </summary>
        /// <param name="value">The value to set, true or false for whether the
        /// feature is enabled.</param>
        public static void SetAutomaticRecoveryValue(bool value)
        {
            RegistryKey key = GetRecoveryRegistryKey();
            if (key != null)
            {
                int dword = value ? 0 : 2;
                try
                {
                    key.SetValue(RecoveryValueName, dword);
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }
    }
}