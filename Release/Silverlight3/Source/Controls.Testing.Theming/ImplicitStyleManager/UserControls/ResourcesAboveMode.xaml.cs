// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// This UserControl contains a scenario where resources are added to an element
    /// higher in the tree compared to where the ApplyMode property is added.
    /// </summary>
    public partial class ResourcesAboveMode : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the ResourcesAboveMode class.
        /// </summary>
        public ResourcesAboveMode()
        {
            InitializeComponent();
        }
    }
}