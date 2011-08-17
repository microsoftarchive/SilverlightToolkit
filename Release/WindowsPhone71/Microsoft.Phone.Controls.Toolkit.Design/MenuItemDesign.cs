// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;

namespace Microsoft.Phone.Controls.Design
{
    /// <summary>
    /// Class that implements the DefaultInitializer for MenuItem.
    /// </summary>
    class MenuItemInitializer : DefaultInitializer
    {
        /// <summary>
        /// Initializes default for the specified ModelItem.
        /// </summary>
        /// <param name="item">Specified ModelItem.</param>
        public override void InitializeDefaults(ModelItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            // Set properties
            item.Properties["Header"].SetValue(typeof(MenuItem).Name);
        }
    }

    /// <summary>
    /// New item factory for MenuItems.
    /// </summary>
    class MenuItemFactory : NewItemFactory
    {
        /// <summary>
        /// Creates an instance of the object.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Instance.</returns>
        public override object CreateInstance(Type type)
        {
            MenuItem item = new MenuItem();
            item.Header = typeof(MenuItem).Name;
            return item;
        }
    }
}
