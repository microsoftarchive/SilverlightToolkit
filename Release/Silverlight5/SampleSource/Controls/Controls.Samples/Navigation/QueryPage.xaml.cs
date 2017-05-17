// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Navigation;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// A Page that displays values assigned by query string.
    /// </summary>
    public partial class QueryPage : Page
    {
        /// <summary>
        /// Initializes a QueryPage.
        /// </summary>
        public QueryPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Executes when the user navigates to this page.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            x.Text = GetArgument("x");
            y.Text = GetArgument("y");
            z.Text = GetArgument("z");
        }

        /// <summary>
        /// Gets a value passed in through the query if available.
        /// </summary>
        /// <param name="argName">The key to find.</param>
        /// <returns>The value associated with the key if available, otherwise "{Not Specified}".</returns>
        private string GetArgument(string argName)
        {
            if (NavigationContext.QueryString.ContainsKey(argName))
            {
                return NavigationContext.QueryString[argName];
            }
            return "{Not Specified}";
        }
    }
}
