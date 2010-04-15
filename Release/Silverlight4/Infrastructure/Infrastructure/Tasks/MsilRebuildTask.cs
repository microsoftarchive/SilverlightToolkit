// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
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
    /// A task for building Microsoft Intermediate Language (MSIL) source. Uses
    /// the CLR v2.0 version. This task is specialized for a specific 
    /// Silverlight tool task and as a result may not be useful elsewhere.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Msil", Justification = "Proper spelling.")]
    public class MsilRebuildTask : Task
    {
        /// <summary>
        /// Initializes a new instance of the MsilRebuildTask.
        /// </summary>
        public MsilRebuildTask() : base()
        {
        }

        /// <summary>
        /// Gets or sets the assembly to work with. The operation is destructive
        /// to the assembly.
        /// </summary>
        [Required]
        public ITaskItem Assembly { get; set; }

        /// <summary>
        /// Retrieves the location of a .NET Framework tool.
        /// </summary>
        /// <param name="name">The filename of the tool.</param>
        /// <param name="isSdk">A value indicating whether the tool is an SDK
        /// tool or a tool that should be in the framework runtime directory.</param>
        /// <returns>Returns the tool location, or null.</returns>
        private string GetTool(string name, bool isSdk)
        {
            string path = isSdk ? ToolLocationHelper.GetPathToDotNetFrameworkSdkFile(
                name,
                TargetDotNetFrameworkVersion.Version35) :
                ToolLocationHelper.GetPathToDotNetFrameworkFile(
                name,
                TargetDotNetFrameworkVersion.Version20);
            if (path == null || !File.Exists(path))
            {
                Log.LogError("The tool {0} could not be located.", name);
                path = null;
            }

            return path;
        }

        /// <summary>
        /// Runs the MSIL rebuild task.
        /// </summary>
        /// <returns>Returns a value indicating whether the build should
        /// continue.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Proper pattern for build tasks.")]
        public override bool Execute()
        {
            if (Assembly == null)
            {
                Log.LogMessage("No Assembly was set for the MsilRebuildTask.");
                return true;
            }

            FileInfo assembly = new FileInfo(Assembly.ItemSpec);
            if (!assembly.Exists)
            {
                Log.LogError("The assembly {0} could not be found.", assembly.FullName);
                return false;
            }

            bool status = true;

            try
            {
                // To IL
                string msilpath = Path.ChangeExtension(assembly.Name, ".il");
                string resFilename = Path.ChangeExtension(assembly.Name, ".res");

                string ildasm = GetTool("ildasm.exe", true);
                string ilasm = GetTool("ilasm.exe", false);
                if (ildasm == null)
                {
                    throw new InvalidOperationException();
                }
                if (ilasm == null)
                {
                    throw new InvalidOperationException();
                }

                CommandLineBuilder c = new CommandLineBuilder();
                c.AppendFileNameIfNotNull(assembly.Name);
                c.AppendSwitch("/NOBAR");
                c.AppendSwitch("/UTF8");
                c.AppendSwitch("/CAVERBAL");
                c.AppendSwitch("/TYPELIST");
                c.AppendSwitch("/HEADERS");
                c.AppendSwitchIfNotNull("/OUT=", msilpath);

                ProcessStartInfo ild = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = c.ToString(),
                    FileName = ildasm,
                    WorkingDirectory = assembly.DirectoryName,
                };
                Process ildasmProcess = Process.Start(ild);
                ildasmProcess.WaitForExit();

                if (!File.Exists(Path.Combine(assembly.DirectoryName, msilpath)))
                {
                    status = false;
                    Log.LogWarning("The IL file {0} was not found.", msilpath);
                }

                if (status)
                {
                    // Remove the original assembly
                    assembly.Delete();

                    // Back to DLL
                    CommandLineBuilder a = new CommandLineBuilder();
                    a.AppendSwitch("/nologo");
                    a.AppendSwitch("/output:" + assembly.Name);
                    a.AppendSwitchIfNotNull("/resource:", resFilename);
                    a.AppendSwitch("/pdb");
                    a.AppendSwitch("/dll");
                    a.AppendFileNameIfNotNull(msilpath);

                    ProcessStartInfo ila = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = a.ToString(),
                        FileName = ilasm,
                        WorkingDirectory = assembly.DirectoryName,
                    };

                    Process ilasmProcess = Process.Start(ila);
                    ilasmProcess.WaitForExit();

                    if (File.Exists(assembly.FullName))
                    {
                        Log.LogMessage("Re-generated assembly {0}", assembly.Name);
                    }
                    else
                    {
                        status = false;
                        Log.LogError("Was not able to re-generate the assembly {0}", assembly.FullName);
                    }
                }

                // Remove temporary files
                if (File.Exists(msilpath))
                {
                    File.Delete(msilpath);
                }
                if (File.Exists(resFilename))
                {
                    File.Delete(resFilename);
                }
            }
            catch (Exception e)
            {
                status = false;
                Log.LogWarning(e.Message);
            }

            return status;
        }
    }
}