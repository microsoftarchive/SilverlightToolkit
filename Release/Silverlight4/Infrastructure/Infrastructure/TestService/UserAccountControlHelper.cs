// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// A set of User Account Control helper methods.
    /// </summary>
    internal static class UserAccountControlHelper
    {
        /// <summary>
        /// Gets a value indicating whether the process is elevated.
        /// </summary>
        /// <returns>Returns whether the current process is elevated.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used in the pre-branched version of this code file.")]
        public static bool IsElevated()
        {
            if (WindowsSupportsUserAccountControl())
            {
                AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                WindowsPrincipal wp = new WindowsPrincipal(wi);

                return (wp.IsInRole(WindowsBuiltInRole.Administrator));
            }
            else
            {
                // Always "elevated" for previous Windows operating systems.
                return true;
            }
        }

        /// <summary>
        /// Requires elevated permissions and throws an exception when that is
        /// not the case.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used in the pre-branched version of this code file.")]
        public static void RequireElevatedPermissions()
        {
            if (!IsElevated())
            {
                throw new InvalidOperationException("The requested operation requires elevated Windows permissions.");
            }
        }

        /// <summary>
        /// Simple check for whether the operating system version supports User
        /// Account Control. Windows Vista, Windows 7, and server variants are
        /// all supported with this major version greater-than-or-equal math.
        /// </summary>
        /// <returns>Returns true on modern Windows operating systems.</returns>
        internal static bool WindowsSupportsUserAccountControl()
        {
            return Environment.OSVersion.Version.Major >= 6;
        }
    }
}