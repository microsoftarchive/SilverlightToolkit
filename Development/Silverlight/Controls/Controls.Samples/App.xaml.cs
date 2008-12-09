// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// Microsoft.Windows.Controls samples application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the App class.
        /// </summary>
        public App()
        {
            Startup += delegate
            {
                RootVisual = new SampleBrowser(this.GetType().Assembly, SampleTreeItems);
            };
            InitializeComponent();
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