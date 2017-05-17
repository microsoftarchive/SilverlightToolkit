// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Implementation of DomainUpDown that uses a TransitioningContentControl
    /// it its template.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    /// <remarks>Implemented in the sample project. The sample page will set
    /// the custom template that this control expects.</remarks>
    public class TransitioningDomainUpDown : DomainUpDown
    {
        /// <summary>
        /// The name of the state that represents a transition effect upwards.
        /// </summary>
        public const string UpTransitionState = "UpTransition";

        /// <summary>
        /// The name of the state that represents a transition effect downwards.
        /// </summary>
        public const string DownTransitionState = "DownTransition";

        /// <summary>
        /// Gets or sets the transition element.
        /// </summary>
        /// <value>The transition element.</value>
        private TransitioningContentControl TransitionElement { get; set; }

        /// <summary>
        /// Occurs when the TransitioningDomainUpDown is incremented.
        /// </summary>
        public event EventHandler Incremented;

        /// <summary>
        /// Occurs when the TransitioningDomainUpDown is decremented.
        /// </summary>
        public event EventHandler Decremented;

        /// <summary>
        /// Gets or sets the Style for the TransitioningContentControl.
        /// </summary>
        public Style TransitioningContentControlStyle
        {
            get { return TransitionElement.Style; }
            set { TransitionElement.Style = value; }
        }

        /// <summary>
        /// Builds the visual tree for the DomainUpDown control when a new
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TransitionElement = GetTemplateChild("Visualization") as TransitioningContentControl;
        }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// Will not go beyond the first or last item unless IsCyclic is set.
        /// </summary>
        protected override void OnIncrement()
        {
            if (TransitionElement != null)
            {
                TransitionElement.Transition = UpTransitionState;
            }
            base.OnIncrement();

            EventArgs args = new EventArgs();
            OnIncremented(args);
        }

        /// <summary>
        /// Raised when the spin direction is SpinDirection.Decrease.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.EventArgs"/> 
        /// instance containing the event data.</param>
        protected virtual void OnIncremented(EventArgs e)
        {
            EventHandler handler = Incremented;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// Will not go beyond the first or last item unless IsCyclic is set.
        /// </summary>
        protected override void OnDecrement()
        {
            if (TransitionElement != null)
            {
                TransitionElement.Transition = DownTransitionState;
            }
            base.OnDecrement();

            EventArgs args = new EventArgs();
            OnDecremented(args);
        }

        /// <summary>
        /// Raised when the spin direction is SpinDirection.Decrease.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.EventArgs"/> 
        /// instance containing the event data.</param>
        protected virtual void OnDecremented(EventArgs e)
        {
            EventHandler handler = Decremented;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Decrement the TransitioningDomainUpDown.
        /// </summary>
        public void Decrement()
        {
            OnDecrement();
        }

        /// <summary>
        /// Increment the TransitioningDomainUpDown.
        /// </summary>
        public void Increment()
        {
            OnIncrement();
        }
    }
}