// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Linq;
using System.Windows.Navigation;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// A Page that displays an image to which a frame can navigate.
    /// </summary>
    public partial class Page1 : Page
    {
        /// <summary>
        /// Initializes a Page1.
        /// </summary>
        public Page1()
        {
            InitializeComponent();

            DataContext = Photograph.GetPhotographs().First().Image;
        }
    }
}
