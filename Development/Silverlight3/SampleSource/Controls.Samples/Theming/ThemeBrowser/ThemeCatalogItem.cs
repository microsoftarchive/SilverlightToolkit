// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using System.Windows.Controls;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Buisness Object used in the ThemeBrowserSample.
    /// </summary>
    public sealed partial class ThemeCatalogItem
    {
        /// <summary>
        ///  Gets or sets the name of the theme.
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        /// Gets or sets the thumnail Image representing the theme.
        /// </summary>
        public Image ThumbnailImage { get; set; }

        /// <summary>
        /// Gets or sets the prefered background brush for this theme.
        /// </summary>
        public Brush PreferredBackground { get; set; }
        
        /// <summary>
        /// Gets or sets the path of the theme xaml file for this theme.
        /// </summary>
        public string ThemeXamlPath { get; set; }

        /// <summary>
        /// Initializes a new instance of the ThemeCatologItem class.
        /// </summary>
        public ThemeCatalogItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ThemeCatologItem class.
        /// </summary>
        /// <param name="themeName">Name of the theme.</param>
        /// <param name="resourceName">
        /// Name of the resource defining the Thumbnail for the theme.
        /// </param>
        /// <param name="themeXamlPath">
        /// Path to the XAML file for this theme.
        /// </param>
        /// <param name="preferredBackground">
        /// Preferred Background for the theme.
        /// </param>
        internal ThemeCatalogItem(string themeName, string resourceName, string themeXamlPath, Brush preferredBackground)
        {
            ThemeName = themeName;
            ThumbnailImage = SharedResources.GetImage(resourceName);
            ThemeXamlPath = themeXamlPath;
            PreferredBackground = preferredBackground;
        }

        #region Sample Data
        /// <summary>
        /// Gets a Theme object that represents Shiny Blue.
        /// </summary>
        public static ThemeCatalogItem ShinyBlue
        {
            get
            {
                LinearGradientBrush brush = GetLinearBrush(Color.FromArgb(255, 132, 150, 170), Color.FromArgb(255, 43, 49, 56));
                return new ThemeCatalogItem(
                    "Shiny Blue",
                    "thumbShinyBlue.png",
                    @"System.Windows.Controls.Samples;component/theming/ThemeBrowser/ShinyBlue.xaml",
                    brush);
            }
        }

        /// <summary>
        /// Gets a Theme object that represents Shiny Red.
        /// </summary>
        public static ThemeCatalogItem ShinyRed
        {
            get
            {
                LinearGradientBrush brush = GetLinearBrush(Color.FromArgb(255, 209, 209, 209), Color.FromArgb(255, 174, 49, 56));
                return new ThemeCatalogItem(
                    "Shiny Red",
                    "thumbShinyRed.png",
                    "System.Windows.Controls.Samples;component/Theming/ThemeBrowser/ShinyRed.xaml",
                    brush);
            }
        }

        /// <summary>
        /// Gets a Theme object that represents RainierOrange Theme.
        /// </summary>
        public static ThemeCatalogItem RainierOrange
        {
            get
            {
                LinearGradientBrush brush = GetLinearBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 7, 15, 19));
                return new ThemeCatalogItem(
                    "Rainier Orange",
                    "thumbRainierOrange.png",
                    @"System.Windows.Controls.Samples;component/theming/ThemeBrowser/RainierOrange.xaml",
                    brush);
            }
        }

        /// <summary>
        /// Gets a Theme object that represents RainierPurple Theme.
        /// </summary>
        public static ThemeCatalogItem RainierPurple
        {
            get
            {
                LinearGradientBrush brush = GetLinearBrush(Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 7, 15, 19));
                return new ThemeCatalogItem(
                    "Rainier Purple",
                    "thumbRainierPurple.png",
                    @"System.Windows.Controls.Samples;component/theming/ThemeBrowser/RainierPurple.xaml",
                    brush);
            }
        }

        /// <summary>
        /// Gets a Theme object that represents ExpressionDark.
        /// </summary>
        public static ThemeCatalogItem ExpressionDark
        {
            get
            {
                LinearGradientBrush brush = GetLinearBrush(Color.FromArgb(255, 23, 23, 23), Color.FromArgb(255, 10, 10, 10));
                return new ThemeCatalogItem(
                    "Expression Dark",
                    "thumbExpresssionDark.png",
                    @"System.Windows.Controls.Samples;component/theming/ThemeBrowser/ExpressionDark.xaml",
                    brush);
            }
        }

        /// <summary>
        /// Gets a Theme object that represents ExpressionDark.
        /// </summary>
        public static ThemeCatalogItem ExpressionLight
        {
            get
            {
                LinearGradientBrush brush = GetLinearBrush(Color.FromArgb(255, 189, 189, 189), Color.FromArgb(255, 160, 160, 160));                   
                return new ThemeCatalogItem(
                    "Expression Light",
                    "thumbExpresssionLight.png",
                    @"System.Windows.Controls.Samples;component/theming/ThemeBrowser/ExpressionLight.xaml",
                    brush);
            }
        }

        /// <summary>
        /// Gets a Theme object that represents Bureau Blue.
        /// </summary>
        public static ThemeCatalogItem BureauBlue
        {
            get
            {
                LinearGradientBrush brush = GetLinearBrush(Color.FromArgb(255, 191, 219, 255), Color.FromArgb(255, 166, 194, 229));
                return new ThemeCatalogItem(
                    "Bureau Blue",
                    "BureauBlue.png",
                    @"System.Windows.Controls.Samples;component/theming/ThemeBrowser/BureauBlue.xaml",
                    brush);
            }
        }

        /// <summary>
        /// Gets a Theme object that represents Bureau Black.
        /// </summary>
        public static ThemeCatalogItem BureauBlack
        {
            get
            {
                LinearGradientBrush brush = GetLinearBrush(Color.FromArgb(255, 91, 91, 91), Color.FromArgb(255, 13, 13, 13));
                return new ThemeCatalogItem(
                    "Bureau Black",
                    "BureauBlack.png",
                    @"System.Windows.Controls.Samples;component/theming/ThemeBrowser/BureauBlack.xaml",
                    brush);
            }
        }

        /// <summary>
        /// Gets a Theme object that represents Bureau Black.
        /// </summary>
        public static ThemeCatalogItem WhistlerBlue
        {
            get
            {
                LinearGradientBrush brush = GetLinearBrush(Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 255, 255, 255));
                return new ThemeCatalogItem(
                    "Whistler Blue",
                    "WhistlerBlue.png",
                    @"System.Windows.Controls.Samples;component/theming/ThemeBrowser/WhistlerBlue.xaml",
                    brush);
            }
        }

        /// <summary>
        /// Gets a Theme object that represents Twilight Blue.
        /// </summary>
        public static ThemeCatalogItem TwilightBlue
        {
            get
            {
                LinearGradientBrush brush = GetLinearBrush(Color.FromArgb(255, 120, 183, 225), Color.FromArgb(255, 120, 183, 225));
                return new ThemeCatalogItem(
                    "Twilight Blue",
                    "TwilightBlue.png",
                    @"System.Windows.Controls.Samples;component/theming/ThemeBrowser/TwilightBlue.xaml",
                    brush);
            }
        }

        /// <summary>
        /// Gets a Theme object that represents Bubble Creme.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Creme", Justification = "Correct Spelling")]
        public static ThemeCatalogItem BubbleCreme
        {
            get
            {
                LinearGradientBrush brush = GetLinearBrush(Color.FromArgb(255, 249, 249, 230), Color.FromArgb(255, 249, 249, 230));
                return new ThemeCatalogItem(
                    "Bubble Creme",
                    "BubbleCreme.png",
                    @"System.Windows.Controls.Samples;component/theming/ThemeBrowser/BubbleCreme.xaml",
                    brush);
            }
        }

        /// <summary>
        /// Gets a collection of theme objects.
        /// </summary>
        public static IEnumerable<ThemeCatalogItem> ThemesCatalog
        {
            get { return new ThemeCatalogItem[] { BubbleCreme, TwilightBlue, ExpressionDark, ExpressionLight, WhistlerBlue, BureauBlack, BureauBlue, ShinyBlue, ShinyRed, RainierOrange, RainierPurple }; }
        }
        
        /// <summary>
        /// Create a Theme background LinearGradientBrush.
        /// </summary>
        /// <param name="startColor">
        /// Represents the start color of the gradient.
        /// </param>
        /// <param name="endColor">
        /// Represents the end color of the gradient.
        /// </param>
        /// <returns>Returns a linear gradient brush.</returns>
        private static LinearGradientBrush GetLinearBrush(Color startColor, Color endColor)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            GradientStop colorStop1 = new GradientStop();
            colorStop1.Color = startColor;
            GradientStop colorStop2 = new GradientStop();
            colorStop2.Color = endColor;
            colorStop2.Offset = 1;
            brush.StartPoint = new System.Windows.Point(0.5, 1);
            brush.EndPoint = new System.Windows.Point(0.5, 0);
            brush.GradientStops = new GradientStopCollection { colorStop1, colorStop2 };
            return brush;
        }
        #endregion Sample Data
    }
}