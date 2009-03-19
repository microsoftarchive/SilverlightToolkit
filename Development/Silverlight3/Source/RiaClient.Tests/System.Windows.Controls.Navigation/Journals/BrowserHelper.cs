//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Windows.Browser;

namespace System.Windows.Navigation.UnitTests
{
    /// <summary>
    /// A collection of browser helper methods.
    /// </summary>
    public static class BrowserHelper
    {
        #region Constants

        private const string HistoryKey = "history";
        private const string HistoryBackKey = "back";
        private const string HistoryForwardKey = "forward";
        private const string TitleKey = "title";
        private const string LocationKey = "location";
        private const string HashKey = "hash";
        private const string HostKey = "host";
        private const string HostNameKey = "hostname";
        private const string HrefKey = "href";
        private const string PathNameKey = "pathname";
        private const string PortKey = "port";
        private const string ProtocolKey = "protocol";
        private const string SearchKey = "search";

        #endregion Constants

        #region Properties

        /// <summary>
        /// Gets or sets the browser title value.
        /// </summary>
        public static string Title
        {
            get { return HtmlPage.Document.GetProperty(TitleKey) as string; }
            set { HtmlPage.Document.SetProperty(TitleKey, value); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Navigates the browser backward.
        /// </summary>
        public static void GoBack()
        {
            ScriptObject history = HtmlPage.Window.GetProperty(HistoryKey) as ScriptObject;
            history.Invoke(HistoryBackKey);
        }

        /// <summary>
        /// Navigates the browser forward.
        /// </summary>
        public static void GoForward()
        {
            ScriptObject history = HtmlPage.Window.GetProperty(HistoryKey) as ScriptObject;
            history.Invoke(HistoryForwardKey);
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Helper class used to manipulate the browser location object.
        /// </summary>
        public static class Location
        {
            public static string Hash
            {
                get
                {
                    string hash = GetProperty(HashKey) as string;
                    if (hash.StartsWith("#", StringComparison.Ordinal))
                    {
                        return hash.Substring(1);
                    }
                    return hash;
                }
                set
                {
                    string hash = value;
                    if (hash != null && hash.StartsWith("#", StringComparison.Ordinal))
                    {
                        hash = hash.Substring(1);
                    }

                    SetProperty(HashKey, hash);
                }
            }
            public static string Host
            {
                get { return GetProperty(HostKey) as string; }
                set { SetProperty(HostKey, value); }
            }
            public static string HostName
            {
                get { return GetProperty(HostNameKey) as string; }
                set { SetProperty(HostNameKey, value); }
            }
            public static string Href
            {
                get { return GetProperty(HrefKey) as string; }
                set { SetProperty(HrefKey, value); }
            }
            public static string PathName
            {
                get { return GetProperty(PathNameKey) as string; }
                set { SetProperty(PathNameKey, value); }
            }
            public static int Port
            {
                get { return int.Parse(GetProperty(PortKey) as string, CultureInfo.InvariantCulture); }
                set { SetProperty(PortKey, value); }
            }
            public static string Protocol
            {
                get { return GetProperty(ProtocolKey) as string; }
                set { SetProperty(ProtocolKey, value); }
            }
            public static string Search
            {
                get { return GetProperty(SearchKey) as string; }
                set { SetProperty(SearchKey, value); }
            }
            private static object GetProperty(string name)
            {
                ScriptObject location = HtmlPage.Document.GetProperty(LocationKey) as ScriptObject;
                return location.GetProperty(name);
            }
            private static void SetProperty(string name, object value)
            {
                ScriptObject location = HtmlPage.Document.GetProperty(LocationKey) as ScriptObject;
                location.SetProperty(name, value);
            }
        }

        #endregion Nested Types
    }
}
