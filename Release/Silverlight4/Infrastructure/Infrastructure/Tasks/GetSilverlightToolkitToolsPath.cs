// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Microsoft.Silverlight.Toolkit.Build.Tasks
{
    /// <summary>
    /// Gets the tools path for the Silverlight Toolkit, which today ends up
    /// being the same path as the targets file is in.
    /// </summary>
    public partial class GetSilverlightToolkitToolsPath : Task
    {
        /// <summary>
        /// Gets or sets the tools path.
        /// </summary>
        [Output]
        public string ToolsPath { get; set; }

        /// <summary>
        /// Initializes a new instance of the GetSilverlightToolkitToolsPath
        /// class.
        /// </summary>
        public GetSilverlightToolkitToolsPath()
        {
        }

        /// <summary>
        /// Find the path of the Silverlight Toolkit tools, containing files
        /// such as the code coverage support.
        /// </summary>
        /// <returns>A value indicating whether the task succeeded.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception is an error")]
        public override bool Execute()
        {
            ToolsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return true;
        }
    }
}