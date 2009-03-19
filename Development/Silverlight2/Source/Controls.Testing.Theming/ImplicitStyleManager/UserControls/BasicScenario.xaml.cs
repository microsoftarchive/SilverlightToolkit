// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Controls;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// This UserControl contains a scenario where styles are available in the
    /// local element's resource dictionary.
    /// </summary>
    public partial class BasicScenario : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the BasicScenario class.
        /// </summary>
        public BasicScenario()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the button inside the user control.
        /// </summary>
        public Button InnerButton { get { return btn; } }
    }
}