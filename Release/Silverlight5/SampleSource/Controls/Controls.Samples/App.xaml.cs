// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// System.Windows.Controls samples application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the App class.
        /// </summary>
        public App()
        {
            if (App.Current.InstallState == InstallState.Installed)
            {
                App.Current.CheckAndDownloadUpdateCompleted += OnCheckAndDownloadUpdateCompleted;
                App.Current.CheckAndDownloadUpdateAsync();
            }

            Startup += delegate
            {
                RootVisual = new SampleBrowser(SampleTreeItems);
            };
            InitializeComponent();
        }

        /// <summary>
        /// Checks for the update completed event.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
        {
            if (e.UpdateAvailable)
            {
                MessageBox.Show("An application update has been downloaded and "
                              + "will be available the next time you start the "
                              + "sample application.");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("While checking for an application update, the "
                              + "following message was encountered:"
                              + Environment.NewLine 
                              + Environment.NewLine 
                              + e.Error.Message);
            }
        }

        /// <summary>
        /// Gets a collection of SampleTreeItems to populate the SampleBrowser TreeView.
        /// </summary>
        public static IEnumerable<SampleTreeItem> SampleTreeItems
        {
            get
            {
                IEnumerable<object> data = Application.Current.Resources["SampleTreeView"] as IEnumerable<object>;
                return (data != null) ?
                    data.OfType<SampleTreeItem>() :
                    Enumerable.Empty<SampleTreeItem>();
            }
        }
    }
}