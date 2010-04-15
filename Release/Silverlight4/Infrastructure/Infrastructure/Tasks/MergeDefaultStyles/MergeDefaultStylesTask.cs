// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Microsoft.Silverlight.Toolkit.Build.Tasks
{
    /// <summary>
    /// Build task to automatically merge the default styles for controls into
    /// a single generic.xaml file.
    /// </summary>
    public class MergeDefaultStylesTask : Task
    {
        /// <summary>
        /// Gets or sets the root directory of the project where the
        /// generic.xaml file resides.
        /// </summary>
        [Required]
        public string ProjectDirectory { get; set; }

        /// <summary>
        /// Gets or sets the project items marked with the "DefaultStyle" build
        /// action.
        /// </summary>
        [Required]
        public ITaskItem[] DefaultStyles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable TFS integration
        /// for this task.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Tfs", Justification = "This is Team Foundation Server.")]
        public bool EnableTfs { get; set; }

        /// <summary>
        /// Initializes a new instance of the MergeDefaultStylesTask class.
        /// </summary>
        public MergeDefaultStylesTask()
            : base()
        {
        }

        /// <summary>
        /// Merge the project items marked with the "DefaultStyle" build action
        /// into a single generic.xaml file.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task succeeded.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Task should not throw exceptions.")]
        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Low, "Merging default styles into Generic.xaml.");

            // Get the original generic.xaml
            string originalPath = Path.Combine(ProjectDirectory, Path.Combine("Themes", "Generic.xaml"));
            if (!File.Exists(originalPath))
            {
                Log.LogError("{0} does not exist!", originalPath);
                return false;
            }
            Log.LogMessage(MessageImportance.Low, "Found original Generic.xaml at {0}.", originalPath);
            string original = null;
            Encoding encoding = Encoding.Default;
            try
            {
                using (StreamReader reader = new StreamReader(File.Open(originalPath, FileMode.Open, FileAccess.Read)))
                {
                    original = reader.ReadToEnd();
                    encoding = reader.CurrentEncoding;
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }

            // Create the merged Generic.xaml
            List<DefaultStyle> styles = new List<DefaultStyle>();
            foreach (ITaskItem item in DefaultStyles)
            {
                string path = Path.Combine(ProjectDirectory, item.ItemSpec);
                if (!File.Exists(path))
                {
                    Log.LogWarning("Ignoring missing DefaultStyle {0}.", path);
                    continue;
                }

                try
                {
                    Log.LogMessage(MessageImportance.Low, "Processing file {0}.", item.ItemSpec);
                    styles.Add(DefaultStyle.Load(path));
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex);
                }
            }
            string merged = null;
            try
            {
                merged = DefaultStyle.Merge(styles).GenerateXaml();
            }
            catch (InvalidOperationException ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
            
            // Write the new generic.xaml
            if (original != merged)
            {
                Log.LogMessage(MessageImportance.Low, "Writing merged Generic.xaml.");
                if (EnableTfs && TryTfs.Checkout(originalPath))
                {
                    Log.LogMessage("Checked out Generic.xaml.");
                }
                else
                {
                    // Remove read-only flag
                    File.SetAttributes(originalPath, FileAttributes.Normal);
                }

                try
                {
                    File.WriteAllText(originalPath, merged, encoding);
                    Log.LogMessage("Successfully merged Generic.xaml.");
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex);
                    return false;
                }
            }
            else
            {
                Log.LogMessage("Existing Generic.xaml was up to date.");
            }

            return true;
        }
    }
}