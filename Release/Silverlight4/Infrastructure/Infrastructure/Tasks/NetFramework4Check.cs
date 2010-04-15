// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.Silverlight.Toolkit.Build.Tasks
{
    /// <summary>
    /// Checks for the existance of the installed .NET Framework 4 version
    /// through a very simplistic directory-based search.
    /// </summary>
    public partial class NetFramework4Check : Task
    {
        /// <summary>
        /// Gets or sets a value indicating whether the framework version was
        /// located.
        /// </summary>
        [Output]
        public bool FrameworkFound { get; set; }

        /// <summary>
        /// Initializes a new instance of the NetFramework4Check class.
        /// </summary>
        public NetFramework4Check()
        {
        }

        /// <summary>
        /// Checks for .NET 4.
        /// </summary>
        /// <returns>A value indicating whether the task succeeded.</returns>
        public override bool Execute()
        {
            string path = Environment.GetEnvironmentVariable("windir");
            path = Path.Combine(path, "Microsoft.NET");
            path = Path.Combine(path, "Framework");
            FrameworkFound = (Directory.GetDirectories(path, "v4*").Length > 0);
            return true;
        }
    }
}