// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.DataVisualization
{
    /// <summary>
    /// Represents a control that displays a list of items and has a title.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "TitleStyle", StyleTargetType = typeof(Title))]
    [TemplatePart(Name = Legend.LegendItemsAreaName, Type = typeof(Panel))]
    public partial class Legend : Control
    {
        /// <summary>
        /// Legend area name.
        /// </summary>
        private const string LegendItemsAreaName = "LegendItemsArea";

        #region public object Title
        /// <summary>
        /// Gets or sets the title content of the Legend.
        /// </summary>
        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(object),
                typeof(Legend),
                new PropertyMetadata(null, OnTitlePropertyChanged));

        /// <summary>
        /// TitleProperty property changed handler.
        /// </summary>
        /// <param name="d">Legend that changed its Title.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Legend)d).UpdateLegendVisibility();
        }
        #endregion public object Title

        #region public Style TitleStyle
        /// <summary>
        /// Gets or sets the style applied to the Title property of the Legend.
        /// </summary>
        public Style TitleStyle
        {
            get { return GetValue(TitleStyleProperty) as Style; }
            set { SetValue(TitleStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the TitleStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleStyleProperty =
            DependencyProperty.Register(
                "TitleStyle",
                typeof(Style),
                typeof(Legend),
                null);
        #endregion public Style TitleStyle

        /// <summary>
        /// Initializes a new instance of the Legend class.
        /// </summary>
        public Legend()
        {
            DefaultStyleKey = typeof(Legend);
        }

        /// <summary>
        /// Loads template parts when template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (LegendItemsArea != null)
            {
                LegendItemsArea.Children.Clear();
                _legendItemsLegendItemsAreaAdapter.Collection = null;
            }

            LegendItemsArea = GetTemplateChild(LegendItemsAreaName) as Panel;
            if (LegendItemsArea != null)
            {
                _legendItemsLegendItemsAreaAdapter.Panel = LegendItemsArea;
                _legendItemsLegendItemsAreaAdapter.PopulatePanel();
            }
        }

        /// <summary>
        /// Gets or sets the reference to the LegendItemsArea.
        /// </summary>
        private Panel LegendItemsArea { get; set; }

        /// <summary>
        /// Object that synchronizes the collection of legend items with the 
        /// children in the legend items area container.
        /// </summary>
        private ObservableCollectionPanelAdapter<UIElement> _legendItemsLegendItemsAreaAdapter = new ObservableCollectionPanelAdapter<UIElement>();

        #region public IList LegendItems
        /// <summary>
        /// Gets or sets a collection of legend items to insert into the legend 
        /// area.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need this property to be able to be set in XAML.")]
        public IList LegendItems
        {
            get { return GetValue(LegendItemsProperty) as IList; }
            set { SetValue(LegendItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the LegendItems dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendItemsProperty =
            DependencyProperty.Register(
                "LegendItems",
                typeof(IList),
                typeof(Legend),
                new PropertyMetadata(null, OnLegendItemsPropertyChanged));

        /// <summary>
        /// LegendItemsProperty property changed handler.
        /// </summary>
        /// <param name="d">Legend that changed its LegendItems.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnLegendItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Legend source = (Legend) d;
            IList oldValue = (IList) e.OldValue;
            IList newValue = (IList) e.NewValue;            
            source.OnLegendItemsPropertyChanged(oldValue, newValue);
        }
        
        /// <summary>
        /// LegendItemsProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnLegendItemsPropertyChanged(IList oldValue, IList newValue)
        {
            if (oldValue != null)
            {
                INotifyCollectionChanged oldNotifyCollectionChanged = oldValue as INotifyCollectionChanged;
                if (oldNotifyCollectionChanged != null)
                {
                    oldNotifyCollectionChanged.CollectionChanged -= this.CollectionChanged;
                }

                if (LegendItemsArea != null)
                {
                    foreach (UIElement item in oldValue)
                    {
                        LegendItemsArea.Children.Remove(item);
                    }
                }
                _legendItemsLegendItemsAreaAdapter.Collection = null;
            }

            _legendItemsLegendItemsAreaAdapter.Collection = newValue;
            _legendItemsLegendItemsAreaAdapter.PopulatePanel();

            INotifyCollectionChanged newObservableCollection = newValue as INotifyCollectionChanged;
            if (newObservableCollection != null)
            {
                newObservableCollection.CollectionChanged += this.CollectionChanged;
            }

            UpdateLegendVisibility();
        }
        #endregion public IList LegendItems

        /// <summary>
        /// Handles the CollectionChanged event for ItemsSource.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateLegendVisibility();
        }

        /// <summary>
        /// Updates the Legend's Visibility property according to whether it has anything to display.
        /// </summary>
        private void UpdateLegendVisibility()
        {
            IList legendItems = LegendItems;
            bool implementsINotifyCollectionChanged = null != (legendItems as INotifyCollectionChanged);
            // Visible iff: Title is set OR ItemsSource is set and it (doesn't implement INCC OR (does AND is currently empty))
            bool visible = (null != Title) || ((null != legendItems && legendItems.Count != 0) && (!implementsINotifyCollectionChanged || !legendItems.Cast<object>().IsEmpty()));
            Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
