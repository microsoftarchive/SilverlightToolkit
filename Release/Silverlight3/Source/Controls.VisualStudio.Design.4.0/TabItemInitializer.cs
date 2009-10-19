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
    /// Set the designer defaults for TabItem.
    /// </summary>
    internal class TabItemInitializer : DefaultInitializer
    {
        /// <summary>
        /// Initialize a TabItem instance.
        /// </summary>
        /// <param name="item">The item to initialize. This should not be a null reference.</param>
        /// <param name="context">The editing context.</param>
        public override void InitializeDefaults(ModelItem item, EditingContext context)
        {
            // Set the default Header text and add a Grid
            Util.SparseSetValue(item.Properties[MyPlatformTypes.TabItem.HeaderProperty], item.Name);
            ModelItem grid = ModelFactory.CreateItem(context, MyPlatformTypes.Grid.TypeId, CreateOptions.None);
            item.Content.SetValue(grid);

            // Clear FrameworkElement defaults
            item.Properties[MyPlatformTypes.FrameworkElement.HeightProperty].ClearValue();
            item.Properties[MyPlatformTypes.FrameworkElement.WidthProperty].ClearValue();
            item.Properties[MyPlatformTypes.FrameworkElement.HorizontalAlignmentProperty].ClearValue();
            item.Properties[MyPlatformTypes.FrameworkElement.VerticalAlignmentProperty].ClearValue();
        }
    }
}