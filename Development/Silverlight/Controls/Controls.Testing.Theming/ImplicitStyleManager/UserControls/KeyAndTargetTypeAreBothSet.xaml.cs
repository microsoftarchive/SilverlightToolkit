// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// This UserControl contains an element where both key and TargetType are
    /// set.  Key should take precedence - ImplicitStyleManager should not pick
    /// up any styles with keys in them.
    /// </summary>
    public partial class KeyAndTargetTypeAreBothSet : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the KeyAndTargetTypeAreBothSet class.
        /// </summary>
        public KeyAndTargetTypeAreBothSet()
        {
            InitializeComponent();
        }
    }
}