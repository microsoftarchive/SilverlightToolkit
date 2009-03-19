// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.Theming.Theme", Justification = "Simplification of implicit styling.")]

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// Uses ImplicitStyleManager to implicitly apply a set of styles to all of
    /// its descendent FrameworkElements.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public abstract partial class Theme : ContentControl
    {
        /// <summary>
        /// Gets or sets the mode defining how styles in the theme are
        /// implicitly applied.
        /// </summary>
        /// <remarks>
        /// The default value is OneTime.
        /// </remarks>
        public ImplicitStylesApplyMode ApplyMode
        {
            get { return ImplicitStyleManager.GetApplyMode(this); }
            set { ImplicitStyleManager.SetApplyMode(this, value); }
        }

        /// <summary>
        /// Initializes a new instance of the Theme class.
        /// </summary>
        private Theme()
        {
            // Default the ApplyMode to OneTime
            ApplyMode = ImplicitStylesApplyMode.OneTime;
        }

        /// <summary>
        /// Initializes a new instance of the Theme class.
        /// </summary>
        /// <param name="themeAssembly">
        /// Assembly with the embedded resource containing the theme to apply.
        /// </param>
        /// <param name="themeResourceName">
        /// Name of the embedded resource containing the theme to apply.
        /// </param>
        protected Theme(Assembly themeAssembly, string themeResourceName)
            : this()
        {
            if (themeAssembly == null)
            {
                throw new ArgumentNullException("themeAssembly");
            }

            // Get the resource stream for the theme.
            using (Stream stream = themeAssembly.GetManifestResourceStream(themeResourceName))
            {
                if (stream == null)
                {
                    throw new ResourceNotFoundException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.ImplicitStyleManager_ResourceNotFound,
                            themeResourceName),
                        new Uri(themeResourceName, UriKind.Relative));
                }

                // Load the theme
                LoadTheme(stream);
            }
        }

        /// <summary>
        /// Initializes a new instance of the Theme class.
        /// </summary>
        /// <param name="themeResourceStream">
        /// A resource stream containing the theme to apply.
        /// </param>
        protected Theme(Stream themeResourceStream)
            : this()
        {
            if (themeResourceStream == null)
            {
                throw new ArgumentNullException("themeResourceStream");
            }

            LoadTheme(themeResourceStream);
        }

        /// <summary>
        /// Load a theme from a resource stream.
        /// </summary>
        /// <param name="themeResourceStream">
        /// A resource stream containing the theme to load.
        /// </param>
        private void LoadTheme(Stream themeResourceStream)
        {
            Debug.Assert(themeResourceStream != null, "themeResourceStream should not be null!");

            // Load the theme
            using (Stream stream = themeResourceStream)
            {
                ResourceDictionary resources = ResourceParser.Parse(stream, true);
                ImplicitStyleManager.SetExternalResourceDictionary(this, resources);
            }
        }
    }
}