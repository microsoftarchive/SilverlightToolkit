// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Silverlight.Testing.Tools
{
    /// <summary>
    /// Simple factory for the set of web browsers.
    /// </summary>
    public static class WebBrowserFactory
    {
        /// <summary>
        /// The default web browser type.
        /// </summary>
        private const WebBrowserBrand DefaultBrand = WebBrowserBrand.InternetExplorer;

        /// <summary>
        /// Creates the default web browser.
        /// </summary>
        /// <returns>Returns a new web browser instance.</returns>
        public static WebBrowser CreateDefault()
        {
            return Create(DefaultBrand);
        }

        /// <summary>
        /// Creates a web browser instance.
        /// </summary>
        /// <param name="brand">The web browser type.</param>
        /// <returns>Returns a new web browser instance.</returns>
        public static WebBrowser Create(WebBrowserBrand brand)
        {
            WebBrowser wb = Construct(brand);
            return wb;
        }

        /// <summary>
        /// Construct a browser from a browser type.
        /// </summary>
        /// <param name="brand">The browser type.</param>
        /// <returns>Returns a new web browser instance.</returns>
        private static WebBrowser Construct(WebBrowserBrand brand)
        {
            switch (brand)
            {
                case WebBrowserBrand.InternetExplorer:
                    return new InternetExplorer();

                case WebBrowserBrand.Chrome:
                    return new Chrome();

                case WebBrowserBrand.Firefox:
                    return new Firefox();

                case WebBrowserBrand.OutOfBrowser:
                    throw new NotImplementedException("Out-of-Browser support not yet implemented.");

                case WebBrowserBrand.Opera:
                case WebBrowserBrand.Safari:
                    throw new NotImplementedException(string.Format(CultureInfo.CurrentUICulture, "The requested brand {0} is not current implemented."));

                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "The requested brand {0} is not supported."));
            }
        }

        /// <summary>
        /// Parse and create a browser from a string.
        /// </summary>
        /// <param name="browserName">The browser name or type.</param>
        /// <returns>Returns a new instance of the browser.</returns>
        public static WebBrowser Parse(string browserName)
        {
            return Create(ParseBrand(browserName));
        }

        /// <summary>
        /// Parses a string to find the browser type object.
        /// </summary>
        /// <param name="browserName">The browser name as a string.</param>
        /// <returns>Returns a new browser brand instance.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "String parsing function of known data, acceptable cyclomatic complexity.")]
        public static WebBrowserBrand ParseBrand(string browserName)
        {
            browserName = browserName.ToUpperInvariant();
            switch (browserName)
            {
                case "INTERNETEXPLORER":
                case "I":
                case "IE":
                case "WINDOWSIE":
                case "WINDOWSINTERNETEXPLORER":
                case "IEXPLORE":
                case "IEXPLORE.EXE":
                    return WebBrowserBrand.InternetExplorer;

                case "FF":
                case "F":
                case "FIREFOX":
                case "MOZILLA FIREFOX":
                case "FIREFOX.EXE":
                case "MOZILLA":
                    return WebBrowserBrand.Firefox;

                case "CHROME":
                case "C":
                case "CHROME.EXE":
                case "GOOGLE CHROME":
                    return WebBrowserBrand.Chrome;

                case "OPERA":
                case "O":
                    return WebBrowserBrand.Opera;

                case "SAFARI":
                case "APPLE SAFARI":
                case "S":
                    return WebBrowserBrand.Safari;

                case "OOB":
                case "OUTOFBROWSER":
                case "SLLAUNCHER":
                    return WebBrowserBrand.OutOfBrowser;

                case "CUSTOM":
                default:
                    return WebBrowserBrand.Custom;
            }
        }
    }
}