// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Controls.Samples.SyntaxHighlighting;
using System.Windows.Markup;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents a file used by a sample and its source.
    /// </summary>
    [ContentProperty("Source")]
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

        /// <summary>
        /// Gets the extension of the filename.
        /// </summary>
        public string Extension
        {
            get
            {
                string path = this.Path;
                if (!string.IsNullOrEmpty(path))
                {
                    return System.IO.Path.GetExtension(path);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the source language type to use for displaying the source code.
        /// </summary>
        internal SourceLanguageType SourceType
        {
            get
            {
                string ext = Extension.ToLowerInvariant();
                switch (ext)
                {
                    case ".xml":
                    case ".xaml":
                        return SourceLanguageType.Xaml;

                    case ".cs":
                    case ".xaml.cs":
                        return SourceLanguageType.CSharp;

                    case ".vb":
                    case ".xaml.vb":
                        return SourceLanguageType.VisualBasic;

                    default:
                        return SourceLanguageType.CSharp;
                }
            }
        }
    }
}