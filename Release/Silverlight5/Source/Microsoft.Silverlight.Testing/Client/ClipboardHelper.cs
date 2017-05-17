// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Reflection;
using System.Windows;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// Exposes clipboard functionality within Silverlight 3 applications when a
    /// Silverlight 4 runtime is in use.
    /// </summary>
    internal static class ClipboardHelper
    {
        /// <summary>
        /// An empty array of object type.
        /// </summary>
        private static readonly object[] EmptyObjectArray = new object[] { };

        /// <summary>
        /// Backing field for set text.
        /// </summary>
        private static MethodInfo _setText;

        /// <summary>
        /// Backing field for get text.
        /// </summary>
        private static MethodInfo _getText;

        /// <summary>
        /// Backing field for the contains text method.
        /// </summary>
        private static MethodInfo _containsText;

        /// <summary>
        /// A value indicating whether the clipboard feature is present. This
        /// assumes that all 3 methods are present, in their current form for a
        /// more recent runtime.
        /// </summary>
        private static bool? _clipboardFeatureSupported = null;

        /// <summary>
        /// Gets a value indicating whether the clipboard feature is available
        /// and supported.
        /// </summary>
        public static bool IsClipboardFeatureSupported
        {
            get
            {
                if (_clipboardFeatureSupported == null)
                {
                    PrepareClipboardInstance();
                }

                return (bool)_clipboardFeatureSupported;
            }
        }

        /// <summary>
        /// Sets Unicode text data to store on the clipboard, for later access 
        /// with System.Windows.Clipboard.GetText().
        /// </summary>
        /// <param name="text">A string that contains the Unicode text data to 
        /// store on the clipboard.</param>
        public static void SetText(string text)
        {
            RequireClipboardFeature();
            if (_setText != null)
            {
                _setText.Invoke(null, new object[] { text });
            }
        }

        /// <summary>
        /// Retrieves Unicode text data from the system clipboard, if Unicode 
        /// text data exists.
        /// </summary>
        /// <returns>If Unicode text data is present on the system clipboard, 
        /// returns a string that contains the Unicode text data. Otherwise, 
        /// returns an empty string.
        /// </returns>
        public static string GetText()
        {
            RequireClipboardFeature();
            return _getText.Invoke(null, EmptyObjectArray) as string;
        }

        /// <summary>
        /// Queries the clipboard for the presence of data in the Unicode text 
        /// format.
        /// </summary>
        /// <returns>True if the system clipboard contains Unicode text data; 
        /// otherwise, false.</returns>
        public static bool ContainsText()
        {
            RequireClipboardFeature();
            return (bool)_containsText.Invoke(null, EmptyObjectArray);
        }

        /// <summary>
        /// Prepares to use the System.Windows.Clipboard type and throws an
        /// exception if the feature cannot be completely located.
        /// </summary>
        private static void RequireClipboardFeature()
        {
            PrepareClipboardInstance();
            if (_clipboardFeatureSupported == false)
            {
                throw new InvalidOperationException("The Clipboard feature is not available. Please upgrade your Silverlight.");
            }
        }

        /// <summary>
        /// Prepares the type and reflects for new Silverlight features.
        /// </summary>
        private static void PrepareClipboardInstance()
        {
            if (_clipboardFeatureSupported == false)
            {
                return;
            }

            Assembly systemWindows = typeof(UIElement).Assembly;
            Type clipboard = systemWindows.GetType("System.Windows.Clipboard", false);
            if (clipboard != null)
            {
                _setText = clipboard.GetMethod("SetText");
                _getText = clipboard.GetMethod("GetText");
                _containsText = clipboard.GetMethod("ContainsText");

                if (_setText != null && _getText != null && _containsText != null)
                {
                    _clipboardFeatureSupported = true;
                }
            }
        }
    }
}