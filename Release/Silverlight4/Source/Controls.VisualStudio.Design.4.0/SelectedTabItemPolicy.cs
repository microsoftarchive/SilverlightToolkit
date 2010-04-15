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
    /// This policy returns TabItem when it is selected or when its content is selected.
    /// Used by TabItemAdornerProvider (below).
    /// </summary>
    internal class SelectedTabItemPolicy : SelectionPolicy
    {
        /// <summary>
        /// Return all TabItems who are visual tree ancestor of the primary selection.
        /// </summary>
        /// <param name="selection">The current selection.</param>
        /// <returns>All TabItems who are visual tree ancestor of primary selection.</returns>
        protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection)
        {
            ModelItem primary = selection.PrimarySelection;
            List<ModelItem> itemsToReturn = new List<ModelItem>();
            if (primary != null)
            {
                // travel up the hierarchy to check the control selected is a child of TabItem.
                for (ModelItem current = primary; current != null; current = current.Parent)
                {
                    if (current.IsItemOfType(MyPlatformTypes.TabItem.TypeId))
                    {
                        itemsToReturn.Add(current);
                    }
                }
            }
            return itemsToReturn;
        }
    }
}