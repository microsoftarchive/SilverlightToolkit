// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// This UserControl contains a scenario where an element up in the tree has
    /// ApplyMode set to Auto, and an element within that tree has ApplyMode set
    /// to OneTime. 
    /// </summary>
    public partial class SubtreeDifferentMode : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the SubtreeDifferentMode class.
        /// </summary>
        public SubtreeDifferentMode()
        {
            InitializeComponent();
        }
    }
}