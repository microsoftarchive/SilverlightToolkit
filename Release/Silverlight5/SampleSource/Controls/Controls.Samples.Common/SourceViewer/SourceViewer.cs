// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Markup;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// The SourceViewer is used to browse the source for a sample.
    /// </summary>
    [ContentProperty("Files")]
    public class SourceViewer : Control
    {
        /// <summary>
        /// A collection of source files to view.
        /// </summary>
        private Collection<SourceFile> _files = new Collection<SourceFile>();

        /// <summary>
        /// Gets a collection of source files to view.
        /// </summary>
        public Collection<SourceFile> Files
        {
            get { return _files; }
        }

        /// <summary>
        /// Initializes a new instance of the SourceViewer class.
        /// </summary>
        public SourceViewer()
        {
            // Notify the SampleBrowser that we are the current source viewer.
            // This is an odd coupling, but it was done because we're
            // shoe-horning source viewing into the browser even though the
            // source is declared in the sample page.
             Loaded += delegate 
             {
                 if (SampleBrowser.Current != null)
                 {
                     SampleBrowser.Current.SetSourceViewer(this);
                 }
             };
        }
    }
}