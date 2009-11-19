// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System.Globalization;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// Context Menu Items (MenuActions) for TabControl and TabItem.
    /// </summary>
    internal class TabControlContextMenuProvider : PrimarySelectionContextMenuProvider
    {
        /// <summary>
        /// Add tab menu action field.
        /// </summary>
        private MenuAction _addTabMenuAction;

        /// <summary>
        /// Add a "Add Tab" context menu item.
        /// </summary>
        public TabControlContextMenuProvider()
        {
            this.UpdateItemStatus += new EventHandler<MenuActionEventArgs>(TabControlContextMenuProvider_UpdateItemStatus);

            _addTabMenuAction = new MenuAction(Properties.Resources.TabControl_AddTabMenuItem);
            _addTabMenuAction.ImageUri = new Uri(
                String.Format(
                    CultureInfo.InvariantCulture,
                    "pack://application:,,,/{0};component/Images/AddTab.bmp", 
                    this.GetType().Assembly.GetName().Name),
                UriKind.Absolute);

            _addTabMenuAction.Execute += new EventHandler<MenuActionEventArgs>(AddTabMenuAction_Execute);
            this.Items.Add(_addTabMenuAction);
        }

        /// <summary>
        /// Update the state of the menu items.
        /// If the Selected Item is a TabControl, a TabItem or 
        /// the contents of a TabItem, then make the menu item visible.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event argument.</param>
        private void TabControlContextMenuProvider_UpdateItemStatus(object sender, MenuActionEventArgs e)
        {
            // Update AddTab visibility.
            if (e.Selection.SelectionCount == 1)
            {
                ModelItem primarySelection = e.Selection.PrimarySelection;

                if (primarySelection.IsItemOfType(MyPlatformTypes.TabItem.TypeId))
                {
                    // ensure the TabItem is parented to a TabControl.
                    ModelItem parent = primarySelection.Parent;
                    _addTabMenuAction.Visible = (parent != null && parent.IsItemOfType(MyPlatformTypes.TabControl.TypeId));
                }
                else if (primarySelection.IsItemOfType(MyPlatformTypes.TabControl.TypeId))
                {
                    _addTabMenuAction.Visible = true;
                }
                else
                {
                    // if it's the TabItem.Content, treat it as the TabItem
                    ModelItem directParent = primarySelection.Parent;
                    if (directParent != null && directParent.IsItemOfType(MyPlatformTypes.TabItem.TypeId))
                    {
                        ModelItem tabControl = (directParent != null) ? directParent.Parent : null;
                        _addTabMenuAction.Visible = (tabControl != null && tabControl.IsItemOfType(MyPlatformTypes.TabControl.TypeId));
                    }
                }
            }
            else
            {
                _addTabMenuAction.Visible = false;
            }
        }

        /// <summary>
        /// AddTab menu item action.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void AddTabMenuAction_Execute(object sender, MenuActionEventArgs e)
        {
            ModelItem primarySelection = e.Selection.PrimarySelection;
            ModelItem tabControl = null;
            if (primarySelection.IsItemOfType(MyPlatformTypes.TabControl.TypeId))
            {
                tabControl = e.Selection.PrimarySelection;
            }
            else if (primarySelection.IsItemOfType(MyPlatformTypes.TabItem.TypeId))
            {
                ModelItem parent = e.Selection.PrimarySelection.Parent;
                if (parent != null && parent.IsItemOfType(MyPlatformTypes.TabControl.TypeId))
                {
                    tabControl = parent;
                }
            }
            if (tabControl != null)
            {
                using (ModelEditingScope changes = tabControl.BeginEdit(Properties.Resources.TabControl_AddTabMenuItem))
                {
                    ModelItem newTabItem = ModelFactory.CreateItem(e.Context, MyPlatformTypes.TabItem.TypeId, CreateOptions.InitializeDefaults);
                    tabControl.Content.Collection.Add(newTabItem);

                    // change selection to newly created TabItem.
                    Selection sel = new Selection(newTabItem);
                    e.Context.Items.SetValue(sel);

                    changes.Complete();
                }
            }
        }
    }
}