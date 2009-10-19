// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// Set the designer defaults for TabControl.
    /// </summary>
    internal class TabControlInitializer : DefaultInitializer
    {
        /// <summary>
        /// TabControl initializer.
        /// </summary>
        /// <param name="item">Model item for a TabControl.</param>
        /// <param name="context">Editing context.</param>
        public override void InitializeDefaults(ModelItem item, EditingContext context)
        {
            // Use SparseSetValue to avoid setting values to their defaults
            Util.SparseSetValue(item.Properties[MyPlatformTypes.FrameworkElement.WidthProperty], 200d);
            Util.SparseSetValue(item.Properties[MyPlatformTypes.FrameworkElement.HeightProperty], 100d);

            // Add a default TabItem
            ModelItem newTabItem = ModelFactory.CreateItem(context, MyPlatformTypes.TabItem.TypeId, CreateOptions.InitializeDefaults);
            item.Content.Collection.Add(newTabItem);
        }
    }
}
