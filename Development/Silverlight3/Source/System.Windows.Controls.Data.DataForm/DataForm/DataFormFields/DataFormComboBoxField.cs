//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace System.Windows.Controls
{
    using System.Collections;
    using System.Windows.Controls.Common;
    using System.Windows.Data;

    /// <summary>
    /// Combo box field for the DataForm.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormComboBoxField : DataFormBoundField
    {
        /// <summary>
        /// Identifies the DisplayMemberPath dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                "DisplayMemberPath",
                typeof(string),
                typeof(DataFormComboBoxField),
                new PropertyMetadata(OnDisplayMemberPathPropertyChanged));

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(DataFormComboBoxField),
                new PropertyMetadata(OnItemsSourcePropertyChanged));

        /// <summary>
        /// Identifies the SelectedIndexBinding dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexBindingProperty =
            DependencyProperty.Register(
                "SelectedIndexBinding",
                typeof(Binding),
                typeof(DataFormComboBoxField),
                new PropertyMetadata(OnSelectedIndexBindingPropertyChanged));

        /// <summary>
        /// Gets or sets the display member path for the combo box.
        /// </summary>
        public string DisplayMemberPath
        {
            get
            {
                return this.GetValue(DisplayMemberPathProperty) as string;
            }

            set
            {
                this.SetValue(DisplayMemberPathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the items source for the combo box.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return this.GetValue(ItemsSourceProperty) as IEnumerable;
            }

            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected index binding for the combo box.
        /// </summary>
        public Binding SelectedIndexBinding
        {
            get
            {
                return this.GetValue(SelectedIndexBindingProperty) as Binding;
            }

            set
            {
                this.SetValue(SelectedIndexBindingProperty, value);
            }
        }

        /// <summary>
        /// Generates the display input control.
        /// </summary>
        /// <returns>The display input control.</returns>
        protected override FrameworkElement GenerateElement()
        {
            return this.GenerateComboBox(true /* isReadOnly */);
        }

        /// <summary>
        /// Generates the edit input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The edit input control.</returns>
        protected override FrameworkElement GenerateEditingElement(bool isReadOnly)
        {
            return this.GenerateComboBox(isReadOnly);
        }

        /// <summary>
        /// Generates the insert input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The insert input control.</returns>
        protected override FrameworkElement GenerateInsertElement(bool isReadOnly)
        {
            return this.GenerateComboBox(isReadOnly);
        }

        /// <summary>
        /// DisplayMemberPath property changed handler.
        /// </summary>
        /// <param name="d">DataFormComboBoxField that changed its DisplayMemberPath value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDisplayMemberPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormComboBoxField comboBoxField = d as DataFormComboBoxField;
            if (comboBoxField != null && !comboBoxField.AreHandlersSuspended())
            {
                ComboBox comboBox = comboBoxField.Element as ComboBox;

                if (comboBox != null)
                {
                    comboBox.DisplayMemberPath = comboBoxField.DisplayMemberPath;
                }
            }
        }

        /// <summary>
        /// ItemsSource property changed handler.
        /// </summary>
        /// <param name="d">DataFormComboBoxField that changed its ItemsSource value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormComboBoxField comboBoxField = d as DataFormComboBoxField;
            if (comboBoxField != null && !comboBoxField.AreHandlersSuspended())
            {
                ComboBox comboBox = comboBoxField.Element as ComboBox;

                if (comboBox != null)
                {
                    comboBox.ItemsSource = comboBoxField.ItemsSource;
                }
            }
        }

        /// <summary>
        /// SelectedIndexBinding property changed handler.
        /// </summary>
        /// <param name="d">DataFormComboBoxField that changed its SelectedIndexBinding value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnSelectedIndexBindingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataFormComboBoxField comboBoxField = d as DataFormComboBoxField;
            if (comboBoxField != null && !comboBoxField.AreHandlersSuspended())
            {
                if (comboBoxField.OwningForm != null && comboBoxField.OwningForm.IsEditing)
                {
                    comboBoxField.OwningForm.ForceEndEdit();
                }

                if (comboBoxField.SelectedIndexBinding != null)
                {
                    if (!String.IsNullOrEmpty(comboBoxField.SelectedIndexBinding.Path.Path))
                    {
                        comboBoxField.SelectedIndexBinding.Mode = BindingMode.TwoWay;
                    }

                    if (comboBoxField.SelectedIndexBinding.Converter == null)
                    {
                        comboBoxField.SelectedIndexBinding.Converter = new DataFormValueConverter();
                    }

                    if (comboBoxField.OwningForm != null && comboBoxField.OwningForm.UIGenerated)
                    {
                        comboBoxField.OwningForm.GenerateUI();
                    }
                }
            }
        }

        /// <summary>
        /// Generates a combo box.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the combo box should be read-only.</param>
        /// <returns>The combo box.</returns>
        private ComboBox GenerateComboBox(bool isReadOnly)
        {
            ComboBox comboBox = new ComboBox() { IsEnabled = !isReadOnly };

            if (this.ItemsSource != null)
            {
                comboBox.ItemsSource = this.ItemsSource;
            }

            if (this.SelectedIndexBinding != null)
            {
                comboBox.SetBinding(ComboBox.SelectedIndexProperty, this.SelectedIndexBinding);
            }

            if (this.Binding != null)
            {
                comboBox.SetBinding(ComboBox.SelectedItemProperty, this.Binding);
            }

            comboBox.DisplayMemberPath = this.DisplayMemberPath;
            comboBox.DropDownClosed += new EventHandler(this.OnComboBoxDropDownClosed);

            return comboBox;
        }

        /// <summary>
        /// Handles the case where the combo box's drop-down menu was closed.
        /// </summary>
        /// <param name="sender">The combo box.</param>
        /// <param name="e">The event args.</param>
        private void OnComboBoxDropDownClosed(object sender, EventArgs e)
        {
            this.CommitEdit();
        }
    }
}
