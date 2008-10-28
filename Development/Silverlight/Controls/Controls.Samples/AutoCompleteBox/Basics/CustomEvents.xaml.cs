// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// The PopulationEvents class shows how a developer might hook into the 
    /// population events to provide custom data.
    /// </summary>
    [Sample("AutoCompleteBox/Basics/(2)Custom events")]
    public partial class CustomEvents : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the type.
        /// </summary>
        public CustomEvents()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(PopulationEvents_Loaded);
        }

        /// <summary>
        /// Handle the Loaded event of the page.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void PopulationEvents_Loaded(object sender, RoutedEventArgs e)
        {
            NowAutoComplete.Populating += OnPopulatingSynchronous;
            NowAutoComplete2.Populating += OnPopulatingSynchronous;
            LaterAutoComplete.Populating += OnPopulatingAsynchronous;
            LaterAutoComplete2.Populating += OnPopulatingAsynchronous;
        }

        /// <summary>
        /// The Populating event handler.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnPopulatingSynchronous(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox source = (AutoCompleteBox)sender;

            source.ItemsSource = new string[]
            {
                e.Parameter + "1",
                e.Parameter + "2",
                e.Parameter + "3",
            };
        }

        /// <summary>
        /// The populating handler.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnPopulatingAsynchronous(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox source = (AutoCompleteBox)sender;

            // Cancel the populating value: this will allow us to call 
            // PopulateComplete as necessary.
            e.Cancel = true;
            
            // Use the dispatcher to simulate an asynchronous callback when 
            // data becomes available
            Dispatcher.BeginInvoke(
                delegate
                {
                    source.ItemsSource = new string[]
                    {
                        e.Parameter + "1",
                        e.Parameter + "2",
                        e.Parameter + "3",
                    };

                    // Population is complete
                    source.PopulateComplete();
                });
        }
    }
}
