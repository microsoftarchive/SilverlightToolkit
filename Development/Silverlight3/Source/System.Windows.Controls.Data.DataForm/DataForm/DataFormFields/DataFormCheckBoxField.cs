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
    /// <summary>
    /// Check box field for the DataForm.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormCheckBoxField : DataFormBoundField
    {
        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty IsThreeStateProperty =
            DependencyProperty.Register(
                "IsThreeState",
                typeof(bool),
                typeof(DataFormCheckBoxField),
                null);

        /// <summary>
        /// Gets or sets a value indicating whether or not this check box
        /// should have three states.
        /// </summary>
        public bool IsThreeState
        {
            get
            {
                return (bool)this.GetValue(IsThreeStateProperty);
            }

            set
            {
                if (value != this.IsThreeState)
                {
                    this.SetValue(IsThreeStateProperty, value);
                }
            }
        }

        /// <summary>
        /// Generates the display input control.
        /// </summary>
        /// <returns>The display input control.</returns>
        protected override FrameworkElement GenerateElement()
        {
            return this.GenerateCheckBox(true /* isReadOnly */);
        }

        /// <summary>
        /// Generates the edit input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The edit input control.</returns>
        protected override FrameworkElement GenerateEditingElement(bool isReadOnly)
        {
            return this.GenerateCheckBox(isReadOnly);
        }

        /// <summary>
        /// Generates the insert input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The insert input control.</returns>
        protected override FrameworkElement GenerateInsertElement(bool isReadOnly)
        {
            return this.GenerateCheckBox(isReadOnly);
        }

        /// <summary>
        /// Generates a check box.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the check box should be read-only.</param>
        /// <returns>The check box.</returns>
        private CheckBox GenerateCheckBox(bool isReadOnly)
        {
            CheckBox checkBox = new CheckBox() { IsEnabled = !isReadOnly, VerticalAlignment = VerticalAlignment.Center };

            if (this.Binding != null)
            {
                checkBox.SetBinding(CheckBox.IsCheckedProperty, this.Binding);
            }

            checkBox.IsThreeState = this.IsThreeState;
            checkBox.HorizontalAlignment = HorizontalAlignment.Left;
            checkBox.Click += new RoutedEventHandler(this.OnCheckBoxClicked);

            return checkBox;
        }

        /// <summary>
        /// Handles the case where the check box was clicked.
        /// </summary>
        /// <param name="sender">The check box.</param>
        /// <param name="e">The event args.</param>
        private void OnCheckBoxClicked(object sender, RoutedEventArgs e)
        {
            this.CommitEdit();
        }
    }
}