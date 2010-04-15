// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// To register design time metadata for TabControl.
    /// </summary>
    internal class TabControlMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for TabControl.
        /// </summary>
        public TabControlMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.TabControl),
                b =>
                {
                    b.AddCustomAttributes(
                        // Initialize the TabControl when it is added to the designer
                        new FeatureAttribute(typeof(TabControlInitializer)),
                        // Add Tab Context Menu item (MenuAction)
                        new FeatureAttribute(typeof(TabControlContextMenuProvider)),
                        // Shadow the SelectedIndex property to allow us to change tabs at design time 
                        new FeatureAttribute(typeof(TabControlDesignModeValueProvider)),
                        // Controls how controls are added to the TabControl
                        new FeatureAttribute(typeof(TabControlParentAdapter)),
                        // Data Binding Attributes, clear the one from itemscontrol and selector
                        new ComplexBindingPropertiesAttribute("", ""),
                        new LookupBindingPropertiesAttribute("", "", "", ""));
                });
        }
    }
}