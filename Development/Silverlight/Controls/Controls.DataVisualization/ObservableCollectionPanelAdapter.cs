// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.DataVisualization
{
    /// <summary>
    /// An object that synchronizes changes in an observable collection to 
    /// a panel.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the collection.
    /// </typeparam>
    internal class ObservableCollectionPanelAdapter<T>
        where T : UIElement
    {
        /// <summary>
        /// The collection to synchronize with a panel.
        /// </summary>
        private IEnumerable _collection;

        /// <summary>
        /// Gets or sets the collection to synchronize with a panel.
        /// </summary>
        public IEnumerable Collection
        {
            get
            {
                return _collection;
            }
            set
            {
                IEnumerable oldValue = _collection;
                INotifyCollectionChanged oldObservableCollection = oldValue as INotifyCollectionChanged;
                INotifyCollectionChanged newObservableCollection = value as INotifyCollectionChanged;
                _collection = value;

                if (oldObservableCollection != null)
                {
                    oldObservableCollection.CollectionChanged -= OnCollectionChanged;
                }

                if (value == null && Panel != null)
                {
                    Panel.Children.Clear();
                }
                if (newObservableCollection != null)
                {
                    newObservableCollection.CollectionChanged += OnCollectionChanged;
                }
            }
        }

        /// <summary>
        /// Gets or sets the panel to synchronize with the collection.
        /// </summary>
        public Panel Panel { get; set; }

        /// <summary>
        /// Method that synchronizes the panel's child collection with the 
        /// contents of the observable collection when it changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Panel != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    Panel.Children.Clear();
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    for (int cnt = 0; cnt < e.OldItems.Count; cnt++)
                    {
                        T oldItem = e.OldItems[cnt] as T;
                        T newItem = e.NewItems[cnt] as T;

                        int index = Panel.Children.IndexOf(oldItem);

                        if (index != -1)
                        {
                            Panel.Children[index] = newItem;
                        }
                        else
                        {
                            Panel.Children.Remove(oldItem);
                            Panel.Children.Add(newItem);
                        }
                    }
                }
                else
                {
                    if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
                    {
                        foreach (UIElement element in e.OldItems)
                        {
                            Panel.Children.Remove(element);
                        }
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
                    {
                        e.NewItems
                            .OfType<T>()
                            .ForEachWithIndex((item, index) =>
                                {
                                    Panel.Children.Insert(e.NewStartingIndex + index, item);
                                });
                    }
                }
            }
        }

        /// <summary>
        /// A method that populates a panel with the items in the collection.
        /// </summary>
        public void PopulatePanel()
        {
            if (Panel != null)
            {
                if (Collection != null)
                {
                    foreach (T item in Collection)
                    {
                        Panel.Children.Add(item);
                    }
                }
                else
                {
                    Panel.Children.Clear();
                }
            }
        }
    }
}