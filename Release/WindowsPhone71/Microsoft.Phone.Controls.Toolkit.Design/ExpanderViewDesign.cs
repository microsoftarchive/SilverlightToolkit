// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Windows.Design.Model;

namespace Microsoft.Phone.Controls.Design
{
    /// <summary>
    /// Initializes the default values for ExpanderView.
    /// </summary>
    internal class ExpanderViewInitializer : DefaultInitializer
    {
        /// <summary>
        /// Initializes the default values for the ModelItem.
        /// </summary>
        /// <param name="modelItem">The ModelItem.</param>
        public override void InitializeDefaults(ModelItem modelItem)
        {
            if (modelItem == null)
            {
                throw new ArgumentNullException("modelItem");
            }

#if VISUAL_STUDIO_DESIGNER
            modelItem.Properties["Width"].SetValue(456d);
            modelItem.Properties["Height"].SetValue(111d);
#endif
        }
    }
}