// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Abstract web browser class.
    /// </summary>
    public abstract class WebBrowser
    {
        /// <summary>
        /// Blank page address book.
        /// </summary>
        private static readonly Uri DefaultStartUri = new Uri("about:blank");

        /// <summary>
        /// Initializes a new instance of the WebBrowser type.
        /// </summary>
        protected WebBrowser()
        {
        }

        /// <summary>
        /// Starts the web browser.
        /// </summary>
        public virtual void Start()
        {
            Start(DefaultStartUri);
        }

        /// <summary>
        /// Starts the web browser, pointing at a given address.
        /// </summary>
        /// <param name="uri">The URI to startup.</param>
        public abstract void Start(Uri uri);

        /// <summary>
        /// Closes the web browser.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Gets a value indicating whether the browser is running.
        /// </summary>
        public abstract bool IsRunning { get; }
    }
}