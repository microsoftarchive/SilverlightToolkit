// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents a file used by a sample and its source.
    /// </summary>
    public class SourceFile
    {
        /// <summary>
        /// Gets or sets the path to the file.
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// Gets or sets the source code in the file.
        /// </summary>
        public string Source { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the SourceFile class.
        /// </summary>
        public SourceFile()
        {
        }
    }
}