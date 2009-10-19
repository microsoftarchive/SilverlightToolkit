// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Theming;
using System.Collections.Generic;
using System.Windows.Controls.DataVisualization.Charting;
using System.Collections;
using System.Linq;
using System.Windows.Data;

namespace System.Windows.Controls.Samples
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
            SampleDataForm.ItemsSource = Employee.Executives;

            PagedCollectionView pcv = new PagedCollectionView(Employee.Executives);
            pcv.PageSize = 1;
            DataContext = pcv;
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
            ImplicitStyleManager.SetApplyMode(Root, ImplicitStylesApplyMode.OneTime);
            ImplicitStyleManager.Apply(Root);
        }

        /// <summary>
        /// Applies the style.
        /// </summary>
        /// <param name="uri">The URI of the theme used.</param>
        internal void ApplyStyle(Uri uri)
        {
            ImplicitStyleManager.SetResourceDictionaryUri(this, uri);
            ImplicitStyleManager.SetApplyMode(this, ImplicitStylesApplyMode.OneTime);
            ImplicitStyleManager.Apply(this);

            // explicitly set content of expander.
            ImplicitStyleManager.SetApplyMode(expander.Content as FrameworkElement, ImplicitStylesApplyMode.OneTime);
            ImplicitStyleManager.Apply(expander.Content as FrameworkElement);
        }
    }
}