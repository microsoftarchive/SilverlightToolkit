// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using System.Windows.Forms;

namespace Microsoft.CodeCoverage
{
    /// <summary>
    /// Application to view code coverage data.
    /// </summary>
    public static partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="arguments">Command line arguments.</param>
        [STAThread]
        public static void Main(string[] arguments)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Viewer viewer = new Viewer();
            if (arguments != null && arguments.Length >= 1 && File.Exists(arguments[0]))
            {
                if (arguments.Length == 1)
                {
                    viewer.LoadCoverageData(arguments[0], null);
                }
                else if (arguments.Length == 2)
                {
                    viewer.LoadCoverageData(arguments[0], arguments[1]);
                }
            }

            Application.Run(viewer);
        }
    }
}