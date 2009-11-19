// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Firefox web browser automation implementation.
    /// </summary>
    /// <remarks>We've kept the implementation process-based to allow for
    /// the easiest control.</remarks>
    public class Firefox : WebBrowserProcess
    {
        /// <summary>
        /// Initializes a new instance of the Chrome type.
        /// </summary>
        public Firefox() : base()
        {
            // These build tasks should be fun in 32-bit.
            string pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            if (!Directory.Exists(pf))
            {
                throw new InvalidOperationException("Could not locate the application directory.");
            }

            string firefox = 
                Path.Combine(
                Path.Combine(
                pf,
                "Mozilla Firefox"),
                "firefox.exe");

            if (!File.Exists(firefox))
            {
                throw new FileNotFoundException("The Firefox web browser application could not be located on this system.", firefox);
            }

            Executable = firefox;
        }
    }
}