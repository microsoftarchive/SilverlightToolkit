// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Data;
using Microsoft.Windows.Design.PropertyEditing;

namespace System.Windows.Controls.Data.Design
{
    /// <summary>
    /// DataGridLengthEditor for the Designer
    /// </summary>
    internal partial class DataGridLengthEditor : PropertyValueEditor
    {
        /// <summary>
        /// Default constructor builds the default DataGridLengthEditor inline editor template.
        /// </summary>
        public DataGridLengthEditor()
        {
            FrameworkElementFactory dataGridLengthEditorGrid = new FrameworkElementFactory(typeof(DataGridLengthEditorGrid));
            Binding binding = new Binding("Value");
            binding.Mode = BindingMode.TwoWay;
            dataGridLengthEditorGrid.SetBinding(DataGridLengthEditorGrid.ValueProperty, binding);

            DataTemplate dataTemplate = new DataTemplate();
            dataTemplate.VisualTree = dataGridLengthEditorGrid;
            this.InlineEditorTemplate = dataTemplate;
        }
    }
}
