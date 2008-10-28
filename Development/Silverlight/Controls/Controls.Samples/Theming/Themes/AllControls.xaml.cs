// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Controls.Theming;
using System.Collections.Generic;
using Microsoft.Windows.Controls.DataVisualization.Charting;
using System.Collections;
using System.Linq;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// A user control with examples of every control to demonstrate themes.
    /// </summary>
    public partial class AllControls : UserControl
    {
        /// <summary>
        /// Gets or sets the Background Brush for this user Control.
        /// </summary>
        public Brush PreferredBackground
        {
            get { return Root.Background; }
            set { Root.Background = value; }
        }

        /// <summary>
        /// Initializes a new instance of the AllControls class.
        /// </summary>
        public AllControls()
        {
            InitializeComponent();
            LayoutUpdated += OnLayoutUpdated;

            SampleDataGrid.ItemsSource = Employee.Executives;
            SampleAutoComplete.ItemsSource = Catalog.VacationMediaItems;
            TripsList.ItemsSource = Catalog.PlannedVacationMediaItems;
            VacationImage.Source = Catalog.VacationCatalog.CatalogImage.Source;

            VactionTreeView.LayoutUpdated += OnTreeViewLayoutUpdated;
            ObjectCollection collection = new ObjectCollection();
            collection.Add(Catalog.VacationCatalog);
            VactionTreeView.ItemsSource = collection;

            ExpanderImage.Source = SharedResources.GetImage("SilverlightThemesLogo.jpg").Source;

            System.Windows.Threading.DispatcherTimer myDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 4000);  
            myDispatcherTimer.Tick += new EventHandler(OnTick);
            myDispatcherTimer.Start();
        }

        /// <summary>
        /// Represents the current tab.
        /// </summary>
        private int _currentTab;

        /// <summary>
        /// Fires every 4 miliseconds while the DispatcherTimer is active.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnTick(object sender, EventArgs e)
        {
            _currentTab = (_currentTab + 1) % ChartTabControl.Items.Count;
            ChartTabControl.SelectedIndex = _currentTab;       
        }

        /// <summary>
        /// Initialize ImplicitStyleManager once the visual tree is ready.
        /// </summary>
        /// <param name="sender">The UserControl.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            // Only apply once
            LayoutUpdated -= OnLayoutUpdated;

            // ImplicitStyleManager is design to only style controls in the
            // namescope it was defined in.  Since user controls create new
            // namescopes, the ImplicitStyleManager acting on the Theme controls
            // will not style the controls in the AllControls user control.  By
            // applying ImplicitStyleManager to the root of the user control
            // (without giving it a theme to use), it will walk up the visual
            // tree to the Theme control and use its styles.
            ImplicitStyleManager.SetApplyMode(Root, ImplicitStylesApplyMode.Auto);
            ImplicitStyleManager.Apply(Root);
        }

        /// <summary>
        /// Expanding the TreeView Items.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTreeViewLayoutUpdated(object sender, EventArgs e)
        {
            VactionTreeView.LayoutUpdated -= OnTreeViewLayoutUpdated;
            VactionTreeView.ExpandAll();
        }

        /// <summary>
        /// Fired when selected item in the tree changes.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The selection changed event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Declared as an event handler in XAML.")]
        private void OnVacationChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MediaItem media = VactionTreeView.SelectedItem as MediaItem;
            if (media != null)
            {
                VacationImage.Source = media.Image.Source;
            }
            else
            {
                Catalog catalog = VactionTreeView.SelectedItem as Catalog;
                if (catalog != null)
                {
                    VacationImage.Source = catalog.CatalogImage.Source;
                }
            }
        }
    }
}