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
        }
    }
}
