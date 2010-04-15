// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Silverlight.Toolkit.Build.Tasks
{
    /// <summary>
    /// A static helper class for working with strong names.
    /// </summary>
    public static class StrongNameHelper
    {
        /// <summary>
        /// String used to look through Sn.exe output.
        /// </summary>
        private const string PublicKeyToken = "Public key token is ";

        /// <summary>
        /// Uses the strong name utility to return the public key token for a 
        /// given assembly.
        /// </summary>
        /// <param name="strongNameUtility">Path that points to the strong name 
        /// utility (sn.exe) in an enlistment.</param>
        /// <param name="dll">The assembly filename.</param>
        /// <returns>Returns the public key token.</returns>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "This build task needs to create a process.")]
        public static string GetPublicKeyToken(string strongNameUtility, string dll)
        {
            Process sn = new Process
            {
                StartInfo = new ProcessStartInfo(strongNameUtility, "-Tp " + dll)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                }
            };
            sn.Start();
            sn.WaitForExit();
            string output = sn.StandardOutput.ReadToEnd();
            int i = output.IndexOf(PublicKeyToken, StringComparison.Ordinal);
            if (i < 0)
            {
                return string.Empty;
            }
            return output.Substring(i + PublicKeyToken.Length).TrimEnd();
        }
    }
}