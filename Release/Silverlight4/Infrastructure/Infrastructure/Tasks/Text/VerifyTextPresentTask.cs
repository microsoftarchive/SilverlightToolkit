// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Microsoft.Silverlight.Toolkit.Build.Tasks
{
    /// <summary>
    /// Verify that in the set of source files, the token text appears. An error
    /// will be reported if it is not found.
    /// </summary>
    public partial class VerifyTextPresentTask : Task
    {
        /// <summary>
        /// Gets or sets the files to search within.
        /// </summary>
        [Required]
        public ITaskItem[] SourceFiles { get; set; }

        /// <summary>
        /// Gets or sets the token text that is searched for using an exact 
        /// ordinal match. 
        /// </summary>
        [Required]
        public string TokenText { get; set; }

        /// <summary>
        /// Initializes a new instance of the VerifyTextPresentTask class.
        /// </summary>
        public VerifyTextPresentTask()
        {
        }

        /// <summary>
        /// Verify the token string is present inside the set of files.
        /// </summary>
        /// <returns>A value indicating whether the task succeeded.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception is an error")]
        public override bool Execute()
        {
            // There's nothing to do if we have no source files
            if (SourceFiles == null || SourceFiles.Length == 0)
            {
                return true;
            }

            // Verify the files
            bool succeeded = true;
            for (int i = 0; i < SourceFiles.Length; i++)
            {
                try
                {
                    string sourcePath = SourceFiles[i].ItemSpec;
                    FileInfo sourceInfo = new FileInfo(sourcePath);

                    // Make sure they didn't pass a directory as an item
                    if (Directory.Exists(sourcePath))
                    {
                        Log.LogError("Cannot check item {0} because it is a directory!", sourcePath);
                        succeeded = false;
                        continue;
                    }

                    // Make sure the source exists
                    if (!sourceInfo.Exists)
                    {
                        Log.LogError("Cannot process file {0} that does not exist!", sourcePath);
                        succeeded = false;
                        continue;
                    }

                    string content = File.ReadAllText(sourcePath);
                    if (!content.Contains(TokenText))
                    {
                        Log.LogError("The token \"{0}\" was not found inside of {1}.", TokenText, sourcePath);
                        succeeded = false;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex);
                    succeeded = false;
                }
            }

            if (succeeded && SourceFiles != null)
            {
                Log.LogMessage(MessageImportance.Normal, "Verified that the \"{0}\" token was present in the {1} input files.", TokenText, SourceFiles.Length);
            }

            return succeeded;
        }
    }
}