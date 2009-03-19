// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// Implicitly applies the Rainier orange theme to all of its descendent
    /// FrameworkElements.
    /// </summary>
    /// <remarks>
    /// The theme is applied using ImplicitStyleManager.
    /// </remarks>
    /// <QualityBand>Preview</QualityBand>
    public partial class RainierOrangeTheme : Theme
    {
        /// <summary>
        /// Initializes a new instance of the RainierOrangeTheme class.
        /// </summary>
        public RainierOrangeTheme()
            : base(typeof(RainierOrangeTheme).Assembly, "System.Windows.Controls.Theming.Theme.xaml")
        {
            DefaultStyleKey = typeof(RainierOrangeTheme);
        }
    }
}