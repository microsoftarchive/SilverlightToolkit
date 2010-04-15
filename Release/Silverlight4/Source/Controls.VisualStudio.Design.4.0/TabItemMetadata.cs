// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// To register design time metadata for TabItem.
    /// </summary>
    internal class TabItemMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for TabItem.
        /// </summary>
        public TabItemMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.TabItem),
                b =>
                {
                    b.AddCustomAttributes(
                        MyPlatformTypes.TabItem.HeaderProperty.Name,
                        new TypeConverterAttribute(typeof(StringConverter)));
                    b.AddCustomAttributes(
                        new ToolboxBrowsableAttribute(false),
                        new DefaultPropertyAttribute(MyPlatformTypes.TabItem.HeaderProperty.Name),
                        new FeatureAttribute(typeof(TabItemInitializer)),               // Initialize the TabItem when it is added to the designer
                        new FeatureAttribute(typeof(TabControlContextMenuProvider)),    // Add Tab Context Menu item (MenuAction)
                        new FeatureAttribute(typeof(TabItemDesignModeValueProvider)),   // Shadow the IsSelected property to allow us to change tabs at design time
                        new FeatureAttribute(typeof(TabItemParentAdapter)),             // Controls how controls are added to the TabItem
                        new FeatureAttribute(typeof(TabItemAdornerProvider)));          // Changes the value of isSelected property of TabItem to true to activate the tabItem on selection
                });
        }
    }
}