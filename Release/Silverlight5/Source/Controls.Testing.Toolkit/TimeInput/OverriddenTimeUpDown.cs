// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// TimeUpDown class that allows setting of text.
    /// </summary>
    public class OverriddenTimeUpDown : TimeUpDown
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverriddenTimeUpDown"/> class.
        /// </summary>
        public OverriddenTimeUpDown()
            : base()
        {
            Culture = new CultureInfo("en-US");
            DefaultStyleKey = typeof(TimeUpDown);
        }

        /// <summary>
        /// Gets the display text box.
        /// </summary>
        /// <value>The display text box.</value>
        public TextBox DisplayTextBox
        {
            get { return ((Panel)VisualTreeHelper.GetChild(this, 0)).FindName("Text") as TextBox; }
        }

        /// <summary>
        /// Gets the button spinner.
        /// </summary>
        /// <value>The button spinner.</value>
        public Spinner ButtonSpinner
        {
            get { return ((Panel)VisualTreeHelper.GetChild(this, 0)).FindName("Spinner") as Spinner; }
        }

        /// <summary>
        /// Applies the text.
        /// </summary>
        /// <param name="text">The text we will simulate.</param>
        internal void ApplyText(string text)
        {
            ApplyValue(text);

            if (TextApplied != null)
            {
                TextApplied(this, new EventArgs());
            }
        }

        /// <summary>
        /// Occurs after the text has been parsed and applied.
        /// </summary>
        public event EventHandler TextApplied;

        /// <summary>
        /// Gets the hint popup.
        /// </summary>
        /// <value>The hint popup.</value>
        public Popup HintPopup
        {
            get { return ((Panel)VisualTreeHelper.GetChild(this, 0)).FindName("TimeHintPopup") as Popup; }
        }

        /// <summary>
        /// Sets the caret in the displaybox.
        /// </summary>
        /// <param name="Position">The position.</param>
        internal void SetCursor(int Position)
        {
            DisplayTextBox.SelectionStart = Position;
            DisplayTextBox.SelectionLength = 0;
        }

        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <returns>The position.</returns>
        internal int GetCursor()
        {
            return DisplayTextBox.SelectionStart;
        }

        /// <summary>
        /// Raises increment spin event.
        /// </summary>
        internal void Increment()
        {
            Increment(1);
        }

        /// <summary>
        /// Raises increment spin event.
        /// </summary>
        /// <param name="amount">The amount of increments.</param>
        internal void Increment(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                OnSpin(new SpinEventArgs(SpinDirection.Increase));
            }
        }

        /// <summary>
        /// Raises decrement spin event.
        /// </summary>
        internal void Decrement()
        {
            Decrement(1);
        }

        /// <summary>
        /// Raises decrement spin event.
        /// </summary>
        /// <param name="amount">The amount of decrements.</param>
        internal void Decrement(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                OnSpin(new SpinEventArgs(SpinDirection.Decrease));
            }
        }

        /// <summary>
        /// Sets a text in the textbox, without parsing it.
        /// </summary>
        /// <param name="text">The text to be set in the textbox.</param>
        /// <remarks>Sets focus to the textbox.</remarks>
        internal void SetText(string text)
        {
            DisplayTextBox.Focus();
            DisplayTextBox.Text = text;
        }
    }
}
