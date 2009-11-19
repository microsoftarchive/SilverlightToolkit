// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Collections.Generic;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// This adorner provider changes the value of IsSelected property of 
    /// TabItem to true to activate the tabItem on selection.
    /// This allows the user to click on a TabItem and make it the active tab.
    /// </summary>
    [UsesItemPolicy(typeof(SelectedTabItemPolicy))]
    internal class TabItemAdornerProvider : AdornerProvider
    {
        /// <summary>
        /// Returns true if this adorner provider supports the tool passed in.
        /// By default this returns true for SelectionTool.
        /// </summary>
        /// <param name="tool">Tool to be checked.</param>
        /// <returns>true if the tool passed in is supported.</returns>
        public override bool IsToolSupported(Tool tool)
        {
            if (tool is SelectionTool || tool is CreationTool)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// When this TabItem is activated set IsSelected to true.
        /// </summary>
        /// <param name="item">A ModelItem representing the adorned element.</param>
        protected override void Activate(ModelItem item)
        {
            if (item != null && item.IsItemOfType(MyPlatformTypes.TabItem.TypeId))
            {
                if (Context != null)
                {
                    Tool tool = Context.Items.GetValue<Tool>();
                    if (tool == null || tool.FocusedTask == null)
                    {
                        TabItemDesignModeValueProvider.SetDesignTimeIsSelected(item, true); // activate the tab that has been selected
                        item.View.UpdateLayout();
                    }
                }
            }

            base.Activate(item);
        }
    }
}