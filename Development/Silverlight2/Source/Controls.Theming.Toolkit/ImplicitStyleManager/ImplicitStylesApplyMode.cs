// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.Theming.ImplicitStylesApplyMode", Justification = "Used by ImplicitStyleManager.")]

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// Specifies the mode in which styles are implicitly applied in 
    /// ImplicitStyleManager.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum ImplicitStylesApplyMode
    {
        /// <summary>
        /// Specifies that the ImplicitStyleManager does not implicitly apply 
        /// styles.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that the ImplicitStyleManager will implicitly apply styles 
        /// to the descendent visual tree once and will not attempt refreshes.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OneTime", Justification = "One time is more commonly written as two words.")]
        OneTime,

        /// <summary>
        /// Specifies that the ImplicitStyleManager will implicitly apply styles 
        /// to the descendent visual tree once the visual tree changes. 
        /// Using this setting may incur a serious performance hit.
        /// </summary>
        Auto
    }
}