// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;

namespace System.Windows.Controls.Data.Design
{
    // The DataGridMenuProvider class provides two context menu items
    // at design time. These are implemented with the MenuAction class.
    internal class DataGridMenuProvider : PrimarySelectionContextMenuProvider
    {
#if Development
        private MenuAction generateStockColumnsMenuAction;
        private MenuAction removeColumnsMenuAction;
#endif
        private MenuAction editPropertyBoundColumnsMenuAction;

        /// <summary>
        ///  The provider's constructor sets up the MenuAction objects and the MenuGroup which holds them.
        /// </summary>
        public DataGridMenuProvider()
        {
            // Set up the MenuGroup which holds the MenuAction items.
            MenuGroup dataOperationsGroup = new MenuGroup("DataGroup", "DataGrid");
            dataOperationsGroup.HasDropDown = true;

            editPropertyBoundColumnsMenuAction = new MenuAction(Properties.Resources.Edit_Property_Bound_Columns);
            editPropertyBoundColumnsMenuAction.Execute += new EventHandler<MenuActionEventArgs>(EditPropertyBoundColumnsMenuAction_Execute);
            dataOperationsGroup.Items.Add(editPropertyBoundColumnsMenuAction);

// These methods are not used, but they are very useful when developing the Design experience
#if Development

            generateStockColumnsMenuAction = new MenuAction(Properties.Resources.Generate_Columns);
            generateStockColumnsMenuAction.Execute += new EventHandler<MenuActionEventArgs>(GenerateStockColumnsMenuAction_Execute);

            removeColumnsMenuAction = new MenuAction(Properties.Resources.Remove_Columns);
            removeColumnsMenuAction.Execute += new EventHandler<MenuActionEventArgs>(RemoveColumnsMenuAction_Execute);

            dataOperationsGroup.Items.Add(generateStockColumnsMenuAction);
            dataOperationsGroup.Items.Add(removeColumnsMenuAction);
#endif

            this.Items.Add(dataOperationsGroup);        // Can have groups - show up as sub menus

            // The UpdateItemStatus event is raised immediately before 
            // the menu show, which provides the opportunity to set states.
            UpdateItemStatus += new EventHandler<MenuActionEventArgs>(DataGridMenuProvider_UpdateItemStatus);
        }

        /// <summary>
        ///     Update the state of the menu items based on the state of the model
        /// </summary>
        private void DataGridMenuProvider_UpdateItemStatus(object sender, MenuActionEventArgs e)
        {
            ModelItem selectedDataGrid = e.Selection.PrimarySelection;
            object dataSource = selectedDataGrid.Properties[PlatformTypes.DataGrid.ItemsSourceProperty].ComputedValue;

            if (dataSource == null)
            {
                editPropertyBoundColumnsMenuAction.Enabled = false;
            }
            else
            {
                editPropertyBoundColumnsMenuAction.Enabled = true;
            }
        }

        /// <summary>
        ///     Add and configure Columns
        /// </summary>
        private void EditPropertyBoundColumnsMenuAction_Execute(object sender, MenuActionEventArgs e)
        {
            DataGridActions.EditPropertyBoundColumns(e.Selection.PrimarySelection, e.Context);
        }

// These methods are not used, but they are very useful when developing the Design experience
#if Development
        /// <summary>
        ///     Add Columns using DisplayMemberBinding
        /// </summary>
        private void GenerateStockColumnsMenuAction_Execute(object sender, MenuActionEventArgs e)
        {
            DataGridActions.GenerateColumns(e.Selection.PrimarySelection, e.Context);
        }

        /// <summary>
        ///     Remove columns
        /// </summary>
        private void RemoveColumnsMenuAction_Execute(object sender, MenuActionEventArgs e)
        {
            DataGridActions.RemoveColumns(e.Selection.PrimarySelection, e.Context);
        }
#endif

    }
}
