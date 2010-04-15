// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Text.RegularExpressions;

namespace Microsoft.Silverlight.Toolkit.Build.Tasks
{
    /// <summary>
    /// Replace strings inside of a set of files, updating their contents. The 
    /// task operates outside the realm of source control and should typically 
    /// be used on a copy of sources, instead of an enlistment's source, to 
    /// prevent overwriting and checking in the modified files.
    /// </summary>
    public partial class StringReplaceTask : Task
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
        /// Gets or sets a value indicating whether the token is considered
        /// to be a regular expression.
        /// </summary>
        /// <value><c>True</c> if token is a regular expression; otherwise, <c>false</c>.</value>
        public bool TokenIsRegularExpression { get; set; }
        
        /// <summary>
        /// Gets or sets the replacement text.
        /// </summary>
        public string ReplacementText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to write files even if
        /// they are marked as read only files.
        /// </summary>
        public bool OverwriteReadOnlyFiles { get; set; }

        /// <summary>
        /// Initializes a new instance of the StringReplaceTask class.
        /// </summary>
        public StringReplaceTask()
        {
        }

        /// <summary>
        /// Retrieve the encoding for a file by reading its contents and 
        /// returning the final Encoding that the stream reader has.
        /// </summary>
        /// <param name="sourcePath">The source file.</param>
        /// <returns>Returns the encoding.</returns>
        internal static Encoding GetFileEncoding(string sourcePath)
        {
            Encoding encoding = Encoding.Default;
            using (StreamReader reader = new StreamReader(File.Open(sourcePath, FileMode.Open, FileAccess.Read)))
            {
                reader.ReadLine();
                encoding = reader.CurrentEncoding;
            }
            return encoding;
        }

        /// <summary>
        /// Replace the token string inside of a set of files with a new value.
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

            // Process the files
            bool succeeded = true;
            int updatedFiles = 0;
            for (int i = 0; i < SourceFiles.Length; i++)
            {
                try
                {
                    string sourcePath = SourceFiles[i].ItemSpec;
                    FileInfo sourceInfo = new FileInfo(sourcePath);

                    // Make sure they didn't pass a directory as an item
                    if (Directory.Exists(sourcePath))
                    {
                        Log.LogError("Cannot move item {0} because it is a directory!", sourcePath);
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

                    Encoding encoding = GetFileEncoding(sourcePath);
                    string content = File.ReadAllText(sourcePath);
                    string newContent = string.Empty;
                    if (TokenIsRegularExpression)
                    {
                        Regex r = new Regex(TokenText);
                        newContent = r.Replace(content, ReplacementText);
                    }
                    else
                    {
                        newContent = content.Replace(TokenText, ReplacementText);
                    }

                    // Check for a change
                    if (string.CompareOrdinal(content, newContent) != 0)
                    {
                        // Make it possible to overwrite read-only files
                        if (OverwriteReadOnlyFiles && sourceInfo.IsReadOnly)
                        {
                            Log.LogMessage(MessageImportance.Low, "Removing read-only attribute from file {0}.", sourcePath);
                            sourceInfo.Attributes = FileAttributes.Normal;
                        }

                        // Replace and rewrite the file
                        // Log.LogMessage(MessageImportance.Normal, "Replaced token \"{0}\" with \"{1}\" in \"{2}.", TokenText, ReplacementText, sourcePath);
                        File.WriteAllText(sourcePath, newContent, encoding);
                        updatedFiles++;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex);
                    succeeded = false;
                }
            }

            Log.LogMessage(MessageImportance.High, "Replaced all \"{0}\" tokens with \"{1}\" in {2} files.", TokenText, ReplacementText, updatedFiles);
            return succeeded;
        }
    }
}