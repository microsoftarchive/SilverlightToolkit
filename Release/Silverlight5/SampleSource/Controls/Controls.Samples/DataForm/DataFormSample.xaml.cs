// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the DataForm.
    /// </summary>
    [Sample("(0)DataForm", DifficultyLevel.Basic, "DataForm")]
    public partial class DataFormSample : UserControl
    {
        /// <summary>
        /// Initializes the DataFormSample.
        /// </summary>
        public DataFormSample()
        {
            InitializeComponent();
            DataContext = Contact.People;
        }
    }
}
