// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Overridden DomainUpDown for test.
    /// </summary>
    internal class OverriddenDomainUpDown : DomainUpDown
    {
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
        /// Increments this instance.
        /// </summary>
        internal void Increment()
        {
            OnIncrement();
        }

        /// <summary>
        /// Decrements this instance.
        /// </summary>
        internal void Decrement()
        {
            OnDecrement();
        }

        /// <summary>
        /// Builds the visual tree for the DomainUpDown control when a new
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Spinner = GetTemplateChild("Spinner") as Spinner;
        }

        /// <summary>
        /// Gets or sets the spinner.
        /// </summary>
        /// <value>The spinner.</value>
        public Spinner Spinner { get; set; }
    }
}
