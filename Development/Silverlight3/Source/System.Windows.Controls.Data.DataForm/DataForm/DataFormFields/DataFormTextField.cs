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
    /// Text field for the DataForm.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormTextField : DataFormBoundField
    {
        /// <summary>
        /// Generates the display input control.
        /// </summary>
        /// <returns>The display input control.</returns>
        protected override FrameworkElement GenerateElement()
        {
            return this.GenerateTextBox(true /* isReadOnly */);
        }

        /// <summary>
        /// Generates the edit input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The edit input control.</returns>
        protected override FrameworkElement GenerateEditingElement(bool isReadOnly)
        {
            return this.GenerateTextBox(isReadOnly);
        }

        /// <summary>
        /// Generates the insert input control.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the element should be read-only.</param>
        /// <returns>The insert input control.</returns>
        protected override FrameworkElement GenerateInsertElement(bool isReadOnly)
        {
            return this.GenerateTextBox(isReadOnly);
        }

        /// <summary>
        /// Generates a text box.
        /// </summary>
        /// <param name="isReadOnly">Whether or not the text box should be read-only.</param>
        /// <returns>The text box.</returns>
        private TextBox GenerateTextBox(bool isReadOnly)
        {
            TextBox textBox = new TextBox() { IsReadOnly = isReadOnly, VerticalAlignment = VerticalAlignment.Center };

            if (this.Binding != null)
            {
                textBox.SetBinding(TextBox.TextProperty, this.Binding);
            }

            textBox.TextChanged += new TextChangedEventHandler(this.OnTextBoxTextChanged);

            return textBox;
        }

        /// <summary>
        /// Handles the case where the text box's text changed.
        /// </summary>
        /// <param name="sender">The text box.</param>
        /// <param name="e">The event args.</param>
        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            // Commit the changes on every text change if the text box's
            // text is invalid.
            if (!this.IsValid)
            {
                this.CommitEdit();
            }
        }
    }
}
