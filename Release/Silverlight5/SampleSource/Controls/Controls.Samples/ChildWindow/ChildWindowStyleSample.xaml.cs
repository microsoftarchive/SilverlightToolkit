// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Linq;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating styling of the ChildWindow.
    /// </summary>
    [Sample("Styling", DifficultyLevel.Intermediate, "ChildWindow")]
    public partial class ChildWindowStyleSample : UserControl
    {
        /// <summary>
        /// Keeps an instance of a ChildWindow that will be shown when a button is clicked.
        /// </summary>
        private StyledChildWindow scw;

        /// <summary>
        /// Initializes a new instance of the ChildWindowStyleSample class.
        /// </summary>
        public ChildWindowStyleSample()
        {
            InitializeComponent();
            scw = new StyledChildWindow();
            scw.Closed += new EventHandler(Scw_Closed);
            thumbs.ItemsSource = from p in Photograph.GetPhotographs()
                                 orderby p.Name
                                 select p;
            thumbs.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the "Closed" event of the ChildWindow.
        /// </summary>
        /// <param name="sender">Child Window.</param>
        /// <param name="e">Event Arguments.</param>
        private void Scw_Closed(object sender, EventArgs e)
        {
            dialogResult.Text = scw.DialogResult.ToString();
        }

        /// <summary>
        /// Handles clicking the "Show" button.
        /// </summary>
        /// <param name="sender">Clicked Button.</param>
        /// <param name="e">Event Arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by event defined in Xaml.")]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            scw.Title = titleText.Text;
            scw.DataContext = (from p in Photograph.GetPhotographs()
                               where p.Name.Equals((thumbs.SelectedItem as Photograph).Name)
                               select p).First().Image;
            scw.Show();
        }
    }
}
