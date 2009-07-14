// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// A class that rotates through a list of styles.
    /// </summary>
    internal class StyleDispenser : IStyleDispenser
    {
        /// <summary>
        /// A linked list of styles dispensed.
        /// </summary>
        private LinkedList<StyleDispensedEventArgs> _stylesDispensed = new LinkedList<StyleDispensedEventArgs>();

        /// <summary>
        /// A bag of weak references to connected style enumerators.
        /// </summary>
        private WeakReferenceBag<StyleEnumerator> _styleEnumerators = new WeakReferenceBag<StyleEnumerator>();

        /// <summary>
        /// Value indicating whether to ignore that the style enumerator has 
        /// dispensed a style.
        /// </summary>
        private bool _ignoreStyleDispensedByEnumerator;

        /// <summary>
        /// The list of styles of rotate.
        /// </summary>
        private IList<Style> _styles;

        /// <summary>
        /// Gets or sets the list of styles to rotate.
        /// </summary>
        public IList<Style> Styles
        {
            get
            {
                return _styles;
            }
            set
            {
                if (value != _styles)
                {
                    {
                        INotifyCollectionChanged notifyCollectionChanged = _styles as INotifyCollectionChanged;
                        if (notifyCollectionChanged != null)
                        {
                            notifyCollectionChanged.CollectionChanged -= StylesCollectionChanged;
                        }
                    }
                    _styles = value;
                    {
                        INotifyCollectionChanged notifyCollectionChanged = _styles as INotifyCollectionChanged;
                        if (notifyCollectionChanged != null)
                        {
                            notifyCollectionChanged.CollectionChanged += StylesCollectionChanged;
                        }
                    }

                    this.ResetStyles();
                }
            }
        }

        /// <summary>
        /// This method is raised when the styles collection is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void StylesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!(e.Action == NotifyCollectionChangedAction.Add && (this.Styles.Count - e.NewItems.Count) == e.NewStartingIndex))
            {
                this.ResetStyles();
            }
        }

        /// <summary>
        /// The parent of the style dispenser.
        /// </summary>
        private IStyleDispenser _parent;

        /// <summary>
        /// Gets or sets the parent of the style dispenser.
        /// </summary>
        public IStyleDispenser Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    OnParentChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the StyleDispenser class.
        /// </summary>
        public StyleDispenser()
        {
        }

        /// <summary>
        /// Resets the state of the StyleDispenser and its style enumerators.
        /// </summary>
        public void ResetStyles()
        {
            OnResetting();
        }

        /// <summary>
        /// Unregisters a style enumerator so that it can be garbage collected.
        /// </summary>
        /// <param name="enumerator">The style enumerator.</param>
        internal void Unregister(StyleEnumerator enumerator)
        {
            _styleEnumerators.Remove(enumerator);
        }

        /// <summary>
        /// Returns a rotating enumerator of Style objects that coordinates with 
        /// the style dispenser object to ensure that no two enumerators are
        /// currently on the same style if possible.  If the style
        /// dispenser is reset or its collection of styles is changed then
        /// the enumerators will also be reset.
        /// </summary>
        /// <param name="stylePredicate">A predicate that returns a value
        /// indicating whether to return a style.</param>
        /// <returns>An enumerator of styles.</returns>
        public IEnumerator<Style> GetStylesWhere(Func<Style, bool> stylePredicate)
        {
            StyleEnumerator enumerator = new StyleEnumerator(this, stylePredicate);

            _ignoreStyleDispensedByEnumerator = true;
            try
            {
                foreach (StyleDispensedEventArgs args in _stylesDispensed)
                {
                    enumerator.StyleDispenserStyleDispensed(this, args);
                }
            }
            finally
            {
                _ignoreStyleDispensedByEnumerator = false;
            }

            _styleEnumerators.Add(enumerator);
            return enumerator;
        }

        /// <summary>
        /// This method is raised when an enumerator dispenses a style.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        internal void EnumeratorStyleDispensed(object sender, StyleDispensedEventArgs e)
        {
            if (!_ignoreStyleDispensedByEnumerator)
            {
                OnEnumeratorStyleDispensed(this, e);
            }
        }

        /// <summary>
        /// Raises the ParentChanged event.
        /// </summary>
        private void OnParentChanged()
        {
            foreach (StyleEnumerator enumerator in _styleEnumerators)
            {
                enumerator.StyleDispenserParentChanged();
            }
        }

        /// <summary>
        /// Raises the EnumeratorStyleDispensed event.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="args">Information about the event.</param>
        private void OnEnumeratorStyleDispensed(object source, StyleDispensedEventArgs args)
        {
            // Remove this item from the list of dispensed styles.
            _stylesDispensed.Remove(args);

            // Add this item to the end of the list of dispensed styles.
            _stylesDispensed.AddLast(args);

            foreach (StyleEnumerator enumerator in _styleEnumerators)
            {
                enumerator.StyleDispenserStyleDispensed(source, args);
            }            
        }

        /// <summary>
        /// This method raises the EnumeratorsResetting event.
        /// </summary>
        private void OnResetting()
        {
            _stylesDispensed.Clear();

            foreach (StyleEnumerator enumerator in _styleEnumerators)
            {
                enumerator.StyleDispenserResetting();
            }  
        }
    }
}