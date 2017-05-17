// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.Theming;
using System.Windows.Media;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Buisness Object used in the ThemeBrowserSample.
    /// </summary>
    public sealed partial class ThemeCatalogItem
    {
        /// <summary>
        ///  Gets the name of the theme.
        /// </summary>
        public string ThemeName { get; private set; }

        /// <summary>
        /// Gets the thumnail Image representing the theme.
        /// </summary>
        public Image ThumbnailImage { get; private set; }

        /// <summary>
        /// Gets a new Theme instance.
        /// </summary>
        public Func<Theme> ThemeConstructor { get; private set; }

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
        /// <param name="constructor">
        /// Constructs a new Theme instance.
        /// </param>
        internal ThemeCatalogItem(string themeName, string resourceName, Func<Theme> constructor)
        {
            ThemeName = themeName;
            ThumbnailImage = SharedResources.GetImage(resourceName);
            ThemeConstructor = constructor;
        }

        #region Sample Data
        /// <summary>
        /// Gets a Theme object that represents Shiny Blue.
        /// </summary>
        public static ThemeCatalogItem ShinyBlue
        {
            get
            {
                return new ThemeCatalogItem(
                    "Shiny Blue",
                    "thumbShinyBlue.png",
                    () => new ShinyBlueTheme());
            }
        }

        /// <summary>
        /// Gets a Theme object that represents Shiny Red.
        /// </summary>
        public static ThemeCatalogItem ShinyRed
        {
            get
            {
                return new ThemeCatalogItem(
                    "Shiny Red",
                    "thumbShinyRed.png",
                    () => new ShinyRedTheme());
            }
        }

        /// <summary>
        /// Gets a Theme object that represents RainierOrange Theme.
        /// </summary>
        public static ThemeCatalogItem RainierOrange
        {
            get
            {
                return new ThemeCatalogItem(
                    "Rainier Orange",
                    "thumbRainierOrange.png",
                    () => new RainierOrangeTheme());
            }
        }

        /// <summary>
        /// Gets a Theme object that represents RainierPurple Theme.
        /// </summary>
        public static ThemeCatalogItem RainierPurple
        {
            get
            {
                return new ThemeCatalogItem(
                    "Rainier Purple",
                    "thumbRainierPurple.png",
                    () => new RainierPurpleTheme());
            }
        }

        /// <summary>
        /// Gets a Theme object that represents ExpressionDark.
        /// </summary>
        public static ThemeCatalogItem ExpressionDark
        {
            get
            {
                return new ThemeCatalogItem(
                    "Expression Dark",
                    "thumbExpresssionDark.png",
                    () => new ExpressionDarkTheme());
            }
        }

        /// <summary>
        /// Gets a Theme object that represents ExpressionDark.
        /// </summary>
        public static ThemeCatalogItem ExpressionLight
        {
            get
            {
                return new ThemeCatalogItem(
                    "Expression Light",
                    "thumbExpresssionLight.png",
                    () => new ExpressionLightTheme());
            }
        }

        /// <summary>
        /// Gets a Theme object that represents Bureau Blue.
        /// </summary>
        public static ThemeCatalogItem BureauBlue
        {
            get
            {
                return new ThemeCatalogItem(
                    "Bureau Blue",
                    "BureauBlue.png",
                    () => new BureauBlueTheme());
            }
        }

        /// <summary>
        /// Gets a Theme object that represents Bureau Black.
        /// </summary>
        public static ThemeCatalogItem BureauBlack
        {
            get
            {
                return new ThemeCatalogItem(
                    "Bureau Black",
                    "BureauBlack.png",
                    () => new BureauBlackTheme());
            }
        }

        /// <summary>
        /// Gets a Theme object that represents Bureau Black.
        /// </summary>
        public static ThemeCatalogItem WhistlerBlue
        {
            get
            {
                return new ThemeCatalogItem(
                    "Whistler Blue",
                    "WhistlerBlue.png",
                    () => new WhistlerBlueTheme());
            }
        }

        /// <summary>
        /// Gets a Theme object that represents System Colors.
        /// </summary>
        public static ThemeCatalogItem SystemColors
        {
            get
            {
                return new ThemeCatalogItem(
                    "System Colors",
                    "SystemColors.png",
                    () => new SystemColorsTheme());
            }
        }

        /// <summary>
        /// Gets a Theme object that represents Twilight Blue.
        /// </summary>
        public static ThemeCatalogItem TwilightBlue
        {
            get
            {
                return new ThemeCatalogItem(
                    "Twilight Blue",
                    "TwilightBlue.png",
                    () => new TwilightBlueTheme());
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
                return new ThemeCatalogItem(
                    "Bubble Creme",
                    "BubbleCreme.png",
                    () => new BubbleCremeTheme());
            }
        }

        /// <summary>
        /// Gets a collection of theme objects.
        /// </summary>
        public static IEnumerable<ThemeCatalogItem> ThemesCatalog
        {
            get { return new ThemeCatalogItem[] { BubbleCreme, TwilightBlue, ExpressionDark, ExpressionLight, WhistlerBlue, BureauBlack, BureauBlue, ShinyBlue, ShinyRed, RainierOrange, RainierPurple, SystemColors }; }
        }
        #endregion Sample Data
    }
}