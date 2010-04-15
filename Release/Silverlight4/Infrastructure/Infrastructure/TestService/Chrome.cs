// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Chrome web browser automation implementation.
    /// </summary>
    /// <remarks>We've kept the implementation process-based to allow for
    /// the easiest control.</remarks>
    public class Chrome : WebBrowserProcess
    {
        /// <summary>
        /// Initializes a new instance of the Chrome type.
        /// </summary>
        public Chrome() : base()
        {
            string lad = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrEmpty(lad))
            {
                lad = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }

            if (!Directory.Exists(lad))
            {
                throw new InvalidOperationException("Could not locate the local application data directory.");
            }

            string chrome = Path.Combine(
                Path.Combine(
                Path.Combine(
                Path.Combine(
                lad,
                "Google"),
                "Chrome"),
                "Application"),
                "chrome.exe");

            if (!File.Exists(chrome))
            {
                throw new FileNotFoundException("The Chrome web browser application could not be located on this system.", chrome);
            }

            Executable = chrome;
        }
    }
}