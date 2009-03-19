// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// An enumerator that dispenses styles sequentially by coordinating with
    /// related enumerators.  Enumerators are related through an association
    /// with a parent StyleDispenser class.
    /// </summary>
    internal class StyleEnumerator : IEnumerator<Style>
    {
        /// <summary>
        /// The index of current item in the StyleDispenser's list.
        /// </summary>
        private int? index;

        /// <summary>
        /// Gets or sets the current style.
        /// </summary>
        private Style CurrentStyle { get; set; }

        /// <summary>
        /// The parent enumerator.
        /// </summary>
        private IEnumerator<Style> _parentEnumerator;

        /// <summary>
        /// Gets the parent enumerator.
        /// </summary>
        private IEnumerator<Style> ParentEnumerator
        {
            get
            {
                if (_parentEnumerator == null && this.StyleDispenser.Parent != null)
                {
                    _parentEnumerator = this.StyleDispenser.Parent.GetStylesWhere(StylePredicate);
                }
                return _parentEnumerator;
            }
        }

        /// <summary>
        /// Initializes a new instance of a StyleEnumerator.
        /// </summary>
        /// <param name="dispenser">The style dispenser that dispensed this
        /// StyleEnumerator.</param>
        /// <param name="stylePredicate">A predicate used to determine which
        /// styles to return.</param>
        public StyleEnumerator(StyleDispenser dispenser, Func<Style, bool> stylePredicate)
        {
            this.StyleDispenser = dispenser;
            this.StylePredicate = stylePredicate;
        }

        /// <summary>
        /// The style dispenser parent has changed.
        /// </summary>
        internal void StyleDispenserParentChanged()
        {
            _parentEnumerator = null;
        }

        /// <summary>
        /// Returns the index of the next suitable style in the style dispenser
        /// list.
        /// </summary>
        /// <param name="startIndex">The index at which to start looking.</param>
        /// <returns>The index of the next suitable style.</returns>
        private int? GetIndexOfNextSuitableStyle(int startIndex)
        {
            if (StyleDispenser.Styles == null || StyleDispenser.Styles.Count == 0)
            {
                return new int?();
            }

            if (startIndex >= StyleDispenser.Styles.Count)
            {
                startIndex = 0;
            }

            int counter = startIndex;
            do
            {
                if (StylePredicate(StyleDispenser.Styles[counter]))
                {
                    return counter;
                }

                counter = (counter + 1) % StyleDispenser.Styles.Count;
            }
            while (startIndex != counter);

            return new int?();
        }

        /// <summary>
        /// Resets the style dispenser.
        /// </summary>
        internal void StyleDispenserResetting()
        {
            if (!ShouldRetrieveFromParentEnumerator)
            {
                index = new int?();
            }
        }

        /// <summary>
        /// Gets or sets a predicate that returns a value indicating whether a 
        /// style should be returned by this enumerator.
        /// </summary>
        /// <returns>A value indicating whether a style can be returned by this
        /// enumerator.</returns>
        private Func<Style, bool> StylePredicate { get; set; }

        /// <summary>
        /// This method is invoked when one of the related style enumerator's 
        /// dispenses a style.  The enumerator checks to see if the style 
        /// dispensed would've been the next style it would have returned.  If
        /// so it updates it's index to the position after the previously
        /// returned style.
        /// </summary>
        /// <param name="sender">The style dispenser.</param>
        /// <param name="e">Information about the event.</param>
        internal void StyleDispenserStyleDispensed(object sender, StyleDispensedEventArgs e)
        {
            if (!ShouldRetrieveFromParentEnumerator && StylePredicate(e.Style))
            {
                int? nextStyleIndex = GetIndexOfNextSuitableStyle(index ?? 0);
                if ((nextStyleIndex ?? -1) == e.Index)
                {
                    index = (e.Index + 1) % StyleDispenser.Styles.Count;
                }
            }
        }

        /// <summary>
        /// Raises the style dispensed event.
        /// </summary>
        /// <param name="args">Information about the style dispensed.</param>
        protected virtual void OnStyleDispensed(StyleDispensedEventArgs args)
        {
            StyleDispenser.EnumeratorStyleDispensed(this, args);
        }

        /// <summary>
        /// Gets the style dispenser that dispensed this enumerator.
        /// </summary>
        public StyleDispenser StyleDispenser { get; private set; }

        /// <summary>
        /// Gets the current style.
        /// </summary>
        public Style Current
        {
            get { return CurrentStyle; }
        }

        /// <summary>
        /// Gets the current style.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return CurrentStyle; }
        }

        /// <summary>
        /// Moves to the next style.
        /// </summary>
        /// <returns>A value indicating whether there are any more suitable
        /// styles.</returns>
        public bool MoveNext()
        {
            if (ShouldRetrieveFromParentEnumerator && ParentEnumerator != null)
            {
                bool isMore = ParentEnumerator.MoveNext();
                if (isMore)
                {
                    this.CurrentStyle = ParentEnumerator.Current;
                }
                return isMore;
            }

            index = GetIndexOfNextSuitableStyle(index ?? 0);
            if (index == null)
            {
                CurrentStyle = null;
                Dispose();
                return false;
            }
            
            CurrentStyle = StyleDispenser.Styles[index.Value];
            OnStyleDispensed(new StyleDispensedEventArgs(index.Value, CurrentStyle));

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether a enumerator should return styles
        /// from its parent enumerator.
        /// </summary>
        private bool ShouldRetrieveFromParentEnumerator
        {
            get { return this.StyleDispenser.Styles == null; }
        }

        /// <summary>
        /// Resets the style enumerator.
        /// </summary>
        public void Reset()
        {
            throw new NotSupportedException(Properties.Resources.StyleEnumerator_CantResetEnumeratorResetStyleDispenserInstead);
        }

        /// <summary>
        /// Stops listening to style dispenser.
        /// </summary>
        public void Dispose()
        {
            if (_parentEnumerator != null)
            {
                _parentEnumerator.Dispose();
            }

            this.StyleDispenser.Unregister(this);
            GC.SuppressFinalize(this);
        }
    }
}