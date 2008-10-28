// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// The SampleAttribute identifies sample pages and their paths in the
    /// sample browser tree.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed partial class SampleAttribute : Attribute
    {
        /// <summary>
        /// Gets the path to the sample in the Sample Browser tree.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SampleAttribute class.
        /// </summary>
        /// <param name="path">
        /// Path to the sample in the Sample Browser tree.
        /// </param>
        public SampleAttribute(string path)
        {
            Debug.Assert(!string.IsNullOrEmpty(path), "path should not be empty!");
            Path = path;
        }
    }
}