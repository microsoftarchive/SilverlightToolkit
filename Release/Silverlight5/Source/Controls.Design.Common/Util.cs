// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// Utility class.
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// Only set the value into the control if it isn't the default.
        /// </summary>
        /// <param name="property">Property Id.</param>
        /// <param name="value">Property value.</param>
        internal static void SparseSetValue(ModelProperty property, object value)
        {
            if (object.Equals(property.DefaultValue, value))
            {
                if (property.IsSet)
                {
                    property.ClearValue();
                }
            }
            else
            {
                property.SetValue(value);
            }
        }

        /// <summary>
        /// Utility method to invalidate a property.
        /// </summary>
        /// <param name="item">The model item.</param>
        /// <param name="propertyIdentifier">The property to be invalidated.</param>
        internal static void InvalidateProperty(ModelItem item, PropertyIdentifier propertyIdentifier)
        {
            item.Context.Services.GetRequiredService<ValueTranslationService>().InvalidateProperty(item, propertyIdentifier);
        }
    }
}
