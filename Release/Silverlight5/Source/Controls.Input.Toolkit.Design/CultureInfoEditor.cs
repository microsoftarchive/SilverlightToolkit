// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Reflection;
using Microsoft.Windows.Design.PropertyEditing;
using System.Globalization;
using System.ComponentModel;

namespace System.Windows.Controls.Input.Design
{
    /// <summary>
    /// Editor for CultureInfo.
    /// </summary>
    /// <remarks>Currently does not support binding from xaml to the editor.</remarks>
    public class CultureInfoEditor : PropertyValueEditor
    {
        /// <summary>
        /// The ComboBox being used to edit the value.
        /// </summary>
        private ComboBox _owner;

        /// <summary>
        /// Preserve the constructor prototype from PropertyValueEditor.
        /// </summary>
        /// <param name="inlineEditorTemplate">Inline editor template.</param>
        public CultureInfoEditor(DataTemplate inlineEditorTemplate)
            : base(inlineEditorTemplate)
        { }

        /// <summary>
        /// Default constructor builds a ComboBox inline editor template.
        /// </summary>
        public CultureInfoEditor()
        {
            // not using databinding here because Silverlight does not support
            // the WPF CultureConverter that is used by Blend.
            FrameworkElementFactory comboBox = new FrameworkElementFactory(typeof(ComboBox));
            comboBox.AddHandler(
                ComboBox.LoadedEvent,
                new RoutedEventHandler(
                    (sender, e) =>
                    {
                        _owner = (ComboBox)sender;
                        _owner.SelectionChanged += EditorSelectionChanged;
                        INotifyPropertyChanged data = _owner.DataContext as INotifyPropertyChanged;
                        if (data != null)
                        {
                            data.PropertyChanged += DatacontextPropertyChanged;
                        }
                        _owner.DataContextChanged += CultureDatacontextChanged;
                    }));

            comboBox.SetValue(ComboBox.IsEditableProperty, false);
            comboBox.SetValue(ComboBox.DisplayMemberPathProperty, "DisplayName");
            comboBox.SetValue(ComboBox.ItemsSourceProperty, CultureInfo.GetCultures(CultureTypes.SpecificCultures));
            DataTemplate dt = new DataTemplate();
            dt.VisualTree = comboBox;

            InlineEditorTemplate = dt;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the owner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private void EditorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // serialize with name.
            object DataContext = _owner.DataContext;
            DataContext
                .GetType()
                .GetProperty("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                .SetValue(DataContext, ((CultureInfo)_owner.SelectedItem).Name, new object[] { });
        }

        /// <summary>
        /// Handles the PropertyChanged event of the context object.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void DatacontextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // deserialize from name.
            if (e.PropertyName == "Value")
            {
                object value = sender
                    .GetType()
                    .GetProperty("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                    .GetValue(sender, new object[] { });

                if (value != null)
                {
                    if (value is string)
                    {
                        CultureInfo setCulture = new CultureInfo(value.ToString());
                        _owner.SelectedItem = setCulture;
                    }
                }
            }
        }

        /// <summary>
        /// Called when the context is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void CultureDatacontextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            INotifyPropertyChanged old = e.OldValue as INotifyPropertyChanged;
            if (old != null)
            {
                old.PropertyChanged -= DatacontextPropertyChanged;
            }
            INotifyPropertyChanged newDataContext = e.NewValue as INotifyPropertyChanged;
            if (newDataContext != null)
            {
                newDataContext.PropertyChanged += DatacontextPropertyChanged;
            }
        }
    }
}