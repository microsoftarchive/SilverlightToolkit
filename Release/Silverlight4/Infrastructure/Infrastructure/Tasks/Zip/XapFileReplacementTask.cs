// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Text.RegularExpressions;

namespace Microsoft.Silverlight.Toolkit.Build.Tasks
{
    /// <summary>
    /// Replace a set of files inside of a Zip (Xap) file. This is a destructive
    /// operation in that the original Xap file is modified.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xap", Justification = "Xap is the proper name for the Silverlight Zip files.")]
    public partial class XapFileReplacementTask : Task
    {
        /// <summary>
        /// Gets or sets the Xap file to use.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Xap", Justification = "Xap is the proper name for the Silverlight Zip files.")]
        [Required]
        public ITaskItem XapFile { get; set; }

        /// <summary>
        /// Gets or sets the files to replace.
        /// </summary>
        [Required]
        public ITaskItem[] Files { get; set; }

        /// <summary>
        /// Gets or sets an optional replacement token. When set, if the token
        /// is present inside any of the source filenames, the token text is
        /// cleared before being written. Allows for File.Token.dll to be 
        /// stored as File.dll, for instance, when the token value is "Token".
        /// </summary>
        public string FileReplacementToken { get; set; }

        /// <summary>
        /// Initializes a new instance of the XapFileReplacementTask class.
        /// </summary>
        public XapFileReplacementTask()
        {
        }

        /// <summary>
        /// Replace the file(s) inside the Xap.
        /// </summary>
        /// <returns>A value indicating whether the task succeeded.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Any exception is an error")]
        public override bool Execute()
        {
            // There's nothing to do if we have no files or not Xap name given
            if (XapFile == null || Files == null || Files.Length == 0)
            {
                return true;
            }

            string xapPath = XapFile.ItemSpec;
            if (!File.Exists(xapPath))
            {
                Log.LogError("The Xap file {0} could not be found.", xapPath);
                return false;
            }

            bool succeeded = true;

            try
            {
                ZipArchive xap = new ZipArchive(xapPath, FileAccess.ReadWrite);

                // Process the files
                for (int i = 0; i < Files.Length; i++)
                {
                    string sourcePath = Files[i].ItemSpec;
                    FileInfo sourceInfo = new FileInfo(sourcePath);

                    string saveFileAs = sourceInfo.Name;
                    if (!string.IsNullOrEmpty(FileReplacementToken))
                    {
                        saveFileAs = saveFileAs.Replace(FileReplacementToken, string.Empty);
                    }

                    // Make sure they didn't pass a directory as an item
                    if (Directory.Exists(sourcePath))
                    {
                        Log.LogError("Cannot process item \"{0}\" because it is a directory!", sourcePath);
                        succeeded = false;
                        continue;
                    }

                    // Make sure the source exists
                    if (!sourceInfo.Exists)
                    {
                        Log.LogError("Cannot process file \"{0}\" that does not exist!", sourcePath);
                        succeeded = false;
                        continue;
                    }

                    ZipArchiveFile zaf = xap[saveFileAs];
                    if (zaf == null)
                    {
                        Log.LogError(
                            "The file \"{0}\" was not found inside the Xap file \"{1}\"",
                            saveFileAs,
                            xapPath);
                        succeeded = false;
                    }
                    else
                    {
                        zaf.Delete();

                        // Overwrite the contents inside the Xap
                        using (Stream fileInsideXap = xap.Create(saveFileAs))
                        {
                            using (Stream newFile = sourceInfo.OpenRead())
                            {
                                ZipArchiveFile.CopyStream(newFile, fileInsideXap);
                            }
                        }

                        Log.LogMessage(
                            MessageImportance.High,
                            "File \"{0}\" inside the Xap file \"{1}\" was replaced with the contents of \"{2}\"",
                            saveFileAs,
                            xapPath,
                            sourcePath);
                    }
                }

                // Close the Xap file, saving any changes
                xap.Close();

                Log.LogMessage("Xap file \"{0}\" saved.", xapPath);
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                succeeded = false;
            }

            return succeeded;
        }
    }
}