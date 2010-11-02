// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Class that implements a container for the ListPicker control.
    /// </summary>
    [TemplateVisualState(GroupName = SelectionStatesGroupName, Name = SelectionStatesUnselectedStateName)]
    [TemplateVisualState(GroupName = SelectionStatesGroupName, Name = SelectionStatesSelectedStateName)]
    public class ListPickerItem : ContentControl
    {
        private const string SelectionStatesGroupName = "SelectionStates";
        private const string SelectionStatesUnselectedStateName = "Unselected";
        private const string SelectionStatesSelectedStateName = "Selected";

        /// <summary>
        /// Initializes a new instance of the ListPickerItem class.
        /// </summary>
        public ListPickerItem()
        {
            DefaultStyleKey = typeof(ListPickerItem);
        }

        /// <summary>
        /// Builds the visual tree for the control when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            VisualStateManager.GoToState(this, IsSelected ? SelectionStatesSelectedStateName : SelectionStatesUnselectedStateName, false);
        }

        internal bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                VisualStateManager.GoToState(this, _isSelected ? SelectionStatesSelectedStateName : SelectionStatesUnselectedStateName, true);
            }
        }
        private bool _isSelected;
    }
}
