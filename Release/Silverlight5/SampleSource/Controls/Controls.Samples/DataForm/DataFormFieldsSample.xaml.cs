// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the DataForm with fields set explicitly.
    /// </summary>
    [Sample("Custom Field Selection", DifficultyLevel.Intermediate)]
    [Category("DataForm")]
    public partial class DataFormFieldsSample : UserControl
    {
        /// <summary>
        /// Initializes a DataFormFieldsSample.
        /// </summary>
        public DataFormFieldsSample()
        {
            InitializeComponent();
            DataContext = Contact.People;
        }
    }
}
