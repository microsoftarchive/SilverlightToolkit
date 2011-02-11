// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Set of known web browsers.
    /// </summary>
    public enum WebBrowserBrand
    {
        /// <summary>
        /// Windows Internet Explorer browser.
        /// </summary>
        InternetExplorer,

        /// <summary>
        /// Mozilla Firefox browser.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Firefox", Justification = "Known browser name.")]
        Firefox,

        /// <summary>
        /// Google Chrome browser.
        /// </summary>
        Chrome,

        /// <summary>
        /// Apple Safari browser.
        /// </summary>
        Safari,

        /// <summary>
        /// Opera browser.
        /// </summary>
        Opera,

        /// <summary>
        /// Silverlight Out-of-Browser experience.
        /// </summary>
        OutOfBrowser,

        /// <summary>
        /// Custom browser, used when a path to a browser is provided instead.
        /// </summary>
        Custom,
    }
}