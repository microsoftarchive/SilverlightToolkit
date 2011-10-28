// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// A collection of items that supports multiple selection.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(MultiselectList))]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public class MultiselectList : ItemsControl
    {
        /// <summary>
        /// Collection of items that are currently selected.
        /// </summary>
        public IList SelectedItems { get; private set; }

        /// <summary>
        /// Occurs when there is a change to the SelectedItems collection.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when the selection mode is opened or closed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsSelectionEnabledChanged;

        #region IsSelectionEnabled DependencyProperty

        /// <summary>
        /// Gets or sets the flag that indicates if the list
        /// is in selection mode or not.
        /// </summary>
        public bool IsSelectionEnabled
        {
            get { return (bool)GetValue(IsInSelectionModeProperty); }
            set { SetValue(IsInSelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the IsSelectionEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInSelectionModeProperty =
            DependencyProperty.Register("IsSelectionEnabled", typeof(bool), typeof(MultiselectList), new PropertyMetadata(false, OnIsSelectionEnabledPropertyChanged));

        /// <summary>
        /// Opens or closes the selection mode accordingly.
        /// If closing, it unselects any selected item.
        /// Finally, it fires up an IsSelectionEnabledChanged event.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnIsSelectionEnabledPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MultiselectList target = (MultiselectList)obj;

            if ((bool)e.NewValue)
            {
                target.OpenSelection();
            }
            else
            {
                if (target.SelectedItems.Count > 0)
                {
                    IList removedItems = new List<object>();

                    //All the selected items will be unselected.
                    foreach (object item in target.SelectedItems)
                    {
                        removedItems.Add(item);
                    }

                    //Unselect the containers that are not virtualized.
                    for (int k = 0; k < target.Items.Count && target.SelectedItems.Count > 0; k++)
                    {
                        MultiselectItem item = (MultiselectItem)target.ItemContainerGenerator.ContainerFromIndex(k);
                        if (item != null)
                        {
                            if (item.IsSelected == true)
                            {
                                item._canTriggerSelectionChanged = false;
                                item.IsSelected = false;
                                item._canTriggerSelectionChanged = true;
                            }
                        }
                    }

                    //Clear the selected items and trigger event.
                    target.SelectedItems.Clear();
                    target.OnSelectionChanged(removedItems, new object[0]);
                }

                target.CloseSelection();
            }

            var handler = target.IsSelectionEnabledChanged;
            if (handler != null)
            {
                handler(obj, e);
            }
        }

        #endregion

        #region ItemInfoTemplate DependencyProperty

        /// <summary>
        /// Gets or sets the data template that is to be
        /// used on the item information field of the MultiselectItems. 
        /// </summary>
        public DataTemplate ItemInfoTemplate
        {
            get { return (DataTemplate)GetValue(ItemInfoTemplateProperty); }
            set { SetValue(ItemInfoTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemInfoTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemInfoTemplateProperty =
            DependencyProperty.Register("ItemInfoTemplate", typeof(DataTemplate), typeof(MultiselectList), new PropertyMetadata(null, null));

        #endregion

        #region ItemContainerStyle DependencyProperty

        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemContainerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(MultiselectList), new PropertyMetadata(null, null));

        #endregion

        /// <summary>
        /// Initializes a new instance of the MultiselectList class.
        /// </summary>     
        public MultiselectList()
        {
            DefaultStyleKey = typeof(MultiselectList);
            SelectedItems = new List<object>();
        }

        /// <summary>
        /// Toogles the selection mode based on the count of selected items,
        /// and fires a SelectionChanged event.
        /// </summary>
        /// <param name="removedItems">
        /// A collection containing the items that were unselected.
        /// </param>
        /// <param name="addedItems">
        /// A collection containing the items that were selected.
        /// </param>
        internal void OnSelectionChanged(IList removedItems, IList addedItems)
        {
            if (SelectedItems.Count <= 0)
            {
                IsSelectionEnabled = false;
            }
            else if (SelectedItems.Count == 1 && removedItems.Count == 0)
            {
                IsSelectionEnabled = true;
            }

            var handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, new SelectionChangedEventArgs(removedItems, addedItems));
            }
        }

        /// <summary>
        /// Triggers the visual state changes and visual transitions
        /// to open the list into selection mode.
        /// </summary>
        private void OpenSelection()
        {
            IList<WeakReference> items = ItemsControlExtensions.GetItemsInViewPort(this);

            //Only animate the containers in the view port.
            foreach (var i in items)
            {
                MultiselectItem item = (MultiselectItem)(((WeakReference)i).Target);
                item.State = SelectionEnabledState.Opened;
                item.UpdateVisualState(true);
            }

            Dispatcher.BeginInvoke(() =>
            {
                for (int j = 0; j < this.Items.Count; j++)
                {
                    MultiselectItem item = (MultiselectItem)ItemContainerGenerator.ContainerFromIndex(j);
                    if (item != null)
                    {
                        item.State = SelectionEnabledState.Opened;
                        item.UpdateVisualState(false);
                    }
                }
            });
        }

        /// <summary>
        /// Triggers the visual state changes and visual transitions
        /// to close the list out of selection mode.
        /// </summary>
        private void CloseSelection()
        {
            IList<WeakReference> items = ItemsControlExtensions.GetItemsInViewPort(this);

            //Only animate the containers in the view port.
            foreach (var i in items)
            {
                MultiselectItem item = (MultiselectItem)(((WeakReference)i).Target);
                item.State = SelectionEnabledState.Closed;
                item.UpdateVisualState(true);
            }

            Dispatcher.BeginInvoke(() =>
            {
                for (int j = 0; j < this.Items.Count; j++)
                {
                    MultiselectItem item = (MultiselectItem)ItemContainerGenerator.ContainerFromIndex(j);
                    if (item != null)
                    {
                        item.State = SelectionEnabledState.Closed;
                        item.UpdateVisualState(false);
                    }
                }
            });
        }

        #region ItemsControl overriden methods

        /// <summary>
        /// Overrides the DependencyObject used by this ItemsControl
        /// to be a MultiselectItem.
        /// </summary>
        /// <returns>
        /// A new instance of a MultiselectItem.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiselectItem();
        }

        /// <summary>
        /// Acknowledges an item as being of the same type as its container
        /// if it is a MultiselectItem.
        /// </summary>
        /// <param name="item">An item owned by the ItemsControl.</param>
        /// <returns>True if the item is a MultiselectItem.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is MultiselectItem);
        }

        /// <summary>
        /// Resets new or reused item containers appropiately.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            MultiselectItem container = (MultiselectItem)element;

            container.Style = this.ItemContainerStyle;
            container._isBeingVirtualized = true;

            //Select or unselect item containers to put or clear checkmarks.
            container.IsSelected = SelectedItems.Contains(item);

            //Open or close item containers depending on the selection mode.
            container.State = IsSelectionEnabled ? SelectionEnabledState.Opened : SelectionEnabledState.Closed;

            container.UpdateVisualState(false);
            container._isBeingVirtualized = false;
        }

        /// <summary>
        /// Unselects any selected item which was removed from the source.
        /// </summary>
        /// <param name="e">The event information.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (SelectedItems.Count > 0)
            {
                IList removedItems = new List<object>();

                for (int i = 0; i < SelectedItems.Count; i++)
                {
                    var item = SelectedItems[i];
                    if (!(Items.Contains(item)))
                    {
                        SelectedItems.Remove(item);
                        removedItems.Add(item);
                        i--;
                    }
                }

                OnSelectionChanged(removedItems, new object[0]);
            }
        }

        #endregion
    }
}
