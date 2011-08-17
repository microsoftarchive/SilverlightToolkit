// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Windows.Design.Model;

namespace Microsoft.Phone.Controls.Design
{
    /// <summary>
    /// Initializes the default values for ExtendedTextBox.
    /// </summary>
    internal class PhoneTextBoxInitializer : DefaultInitializer
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
        }
    }
}