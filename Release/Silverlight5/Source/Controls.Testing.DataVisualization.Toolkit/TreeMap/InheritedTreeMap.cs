// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.DataVisualization;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Represents the inherited TreeMap used for tests.
    /// </summary>
    public class InheritedTreeMap : TreeMap
    {
        /// <summary>
        /// Delegate used to call verification methods.
        /// </summary>
        /// <param name="oldValue">The old Value.</param>
        /// <param name="newValue">The new Value.</param>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Used for testing only.")]
        public delegate void ChangedCallback(object oldValue, object newValue);

        /// <summary>
        /// Event called for OnItemDefinitionSelectorPropertyChangedEvent.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Used for testing only.")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Used for testing only.")]
        public event ChangedCallback OnItemDefinitionSelectorPropertyChangedEvent;

        /// <summary>
        /// Event called for OnItemDefinitionPropertyChangedEvent.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Used for testing only.")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Used for testing only.")]
        public event ChangedCallback OnItemDefinitionPropertyChangedEvent;

        /// <summary>
        /// Event called for OnItemDefinitionPropertyChangedEvent.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Used for testing only.")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Used for testing only.")]
        public event ChangedCallback OnItemsSourcePropertyChangedEvent;

        /// <summary>
        /// Event called for OnInterpolatorsPropertyChanged.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Used for testing only.")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Used for testing only.")]
        public event ChangedCallback OnInterpolatorsPropertyChangedEvent;

        /// <summary>
        /// Exposes virtual protected for testing.
        /// </summary>
        /// <param name="oldValue">The old Value.</param>
        /// <param name="newValue">The new Value.</param>
        protected override void OnItemDefinitionSelectorPropertyChanged(TreeMapItemDefinitionSelector oldValue, TreeMapItemDefinitionSelector newValue)
        {
            base.OnItemDefinitionSelectorPropertyChanged(oldValue, newValue);

            if (OnItemDefinitionSelectorPropertyChangedEvent != null)
            {
                OnItemDefinitionSelectorPropertyChangedEvent(oldValue, newValue);                
            }
        }

        /// <summary>
        /// Exposes virtual protected for testing.
        /// </summary>
        /// <param name="oldValue">The old Value.</param>
        /// <param name="newValue">The new Value.</param>
        protected override void OnItemDefinitionPropertyChanged(TreeMapItemDefinition oldValue, TreeMapItemDefinition newValue)
        {
            base.OnItemDefinitionPropertyChanged(oldValue, newValue);

            if (OnItemDefinitionPropertyChangedEvent != null)
            {
                OnItemDefinitionPropertyChangedEvent(oldValue, newValue);
            }
        }

        /// <summary>
        /// Exposes virtual protected for testing.
        /// </summary>
        /// <param name="oldValue">The old Values.</param>
        /// <param name="newValue">The new Values.</param>
        protected override void OnItemsSourcePropertyChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourcePropertyChanged(oldValue, newValue);

            if (OnItemsSourcePropertyChangedEvent != null)
            {
                OnItemsSourcePropertyChangedEvent(oldValue, newValue);
            }
        }

        /// <summary>
        /// Exposes virtual protected for testing.
        /// </summary>
        /// <param name="oldValue">The old Value.</param>
        /// <param name="newValue">The new Value.</param>
        protected override void OnInterpolatorsPropertyChanged(Collection<Interpolator> oldValue, Collection<Interpolator> newValue)
        {
            base.OnInterpolatorsPropertyChanged(oldValue, newValue);

            if (OnInterpolatorsPropertyChangedEvent != null)
            {
                OnInterpolatorsPropertyChangedEvent(oldValue, newValue);
            }
        }
    }
}
