// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Theming", Justification = "Correct spelling")]

namespace System.Windows.Controls.Design.Common
{
    /// <summary>
    /// Names for ToolboxCategoryAttribute.
    /// </summary>
    internal static class ToolboxCategoryPaths
    {
        /// <summary>
        /// Basic Controls category.
        /// </summary>
        public const string BasicControls = "Basic Controls";

        /// <summary>
        /// Controls category.
        /// </summary>
        public const string Controls = "";

        /// <summary>
        /// Control Parts category.
        /// </summary>
        public const string ControlParts = "Control Parts";

        /// <summary>
        /// DataVisualization category.
        /// </summary>
        public const string DataVisualization = "DataVisualization";

        /// <summary>
        /// DataVisualization/Control Parts category.
        /// </summary>
        public const string DataVisualizationControlParts = "DataVisualization/Control Parts";

        /// <summary>
        /// Theming category.
        /// </summary>
        public const string Theming = "Theming";
    }
}
