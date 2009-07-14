// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// Implicitly applies the Bubble Creme theme to all of its descendent
    /// FrameworkElements.
    /// </summary>
    /// <remarks>
    /// The theme is applied using ImplicitStyleManager.
    /// </remarks>
    /// <QualityBand>Preview</QualityBand>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Creme", Justification = "Correct Spelling")]
    public partial class BubbleCremeTheme : Theme
    {
        /// <summary>
        /// Initializes a new instance of the BubbleCremeTheme class.
        /// </summary>
        public BubbleCremeTheme()
            : base(typeof(BubbleCremeTheme).Assembly, "System.Windows.Controls.Theming.Theme.xaml")
        {
            DefaultStyleKey = typeof(BubbleCremeTheme);
        }
    }
}