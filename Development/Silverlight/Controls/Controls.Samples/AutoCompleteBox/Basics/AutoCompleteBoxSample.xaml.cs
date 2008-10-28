// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// The AutoCompleteGettingStarted sample page shows several common uses 
    /// of the AutoCompleteBox control.
    /// </summary>
    [Sample("AutoCompleteBox/Basics/(0)AutoCompleteBox")]
    public partial class AutoCompleteBoxSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the AutoCompleteGettingStarted class.
        /// </summary>
        public AutoCompleteBoxSample()
        {
            InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(AutoCompleteGettingStarted_Loaded);
        }

        /// <summary>
        /// Hook up to the Loaded event.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void AutoCompleteGettingStarted_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Words
            WordComplete.ItemsSource = Words.GetAliceInWonderland();

            // Sliders
            SetDelay.ValueChanged += (s, args) => DynamicDelayAutoCompleteBox.MinimumPopulateDelay = (int)Math.Floor(SetDelay.Value);
            SetPrefixLength.ValueChanged += (s, args) => WordComplete.MinimumPrefixLength = (int)Math.Floor(SetPrefixLength.Value);
        }
    }
}