// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;

namespace System.Windows.Controls.DataVisualization
{
    /// <summary>
    /// Extensions methods for the IStyleDispenser interface.
    /// </summary>
    public static class StyleDispenserExtensions
    {
        /// <summary>
        /// Returns a style enumerator that returns styles with a specified 
        /// target type or styles with a target type that is an ancestor of the
        /// specified target type.
        /// </summary>
        /// <param name="dispenser">The style dispenser.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="takeAncestors">A value indicating whether to accept 
        /// ancestors of the target type.</param>
        /// <returns>A style enumerator.</returns>
        public static IEnumerator<Style> GetStylesWithTargetType(this IStyleDispenser dispenser, Type targetType, bool takeAncestors)
        {
            return dispenser.GetStylesWhere(style => style.TargetType != null
                && (targetType == style.TargetType || (takeAncestors && style.TargetType.IsAssignableFrom(targetType))));
        }
    }
}
